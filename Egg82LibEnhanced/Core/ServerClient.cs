using Egg82LibEnhanced.Events;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Timers;

namespace Egg82LibEnhanced.Core {
	public class ServerClient {
		//vars
		public event EventHandler Disconnected = null;
		public event EventHandler<ExceptionEventArgs> Error = null;
		public event EventHandler<DataCompleteEventArgs> DataReceived = null;
		public event EventHandler DataSent = null;
		
		public bool compatibilityMode = false;

		private Timer disconnectTimer = new Timer(100.0d);
		private Socket socket = null;
		private List<byte> currentPacket = new List<byte>();
		private SynchronizedCollection<byte[]> backlog = new SynchronizedCollection<byte[]>();
		private byte[] buffer = new byte[128];
		private bool ready = true;

		//constructor
		public ServerClient(Socket socket, bool compatibilityMode = false) {
			this.socket = socket;
			this.compatibilityMode = compatibilityMode;
			disconnectTimer.Elapsed += onTimer;
			disconnectTimer.AutoReset = true;

			disconnectTimer.Start();
			if(socket.Connected) {
				socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(onReceive), socket);
			}
		}

		//public
		public void Disconnect() {
			if (!socket.Connected) {
				return;
			}

			disconnectTimer.Stop();

			backlog.Clear();
			currentPacket.Clear();
			try {
				socket.Disconnect(false);
				socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
			} catch (Exception ex) {
				if (Error != null) {
					Error.Invoke(this, new ExceptionEventArgs(ex));
				}
				return;
			}

			if (Disconnected != null) {
				Disconnected.Invoke(this, EventArgs.Empty);
			}
		}

		public void Send(byte[] data) {
			if (data == null || data.Length == 0) {
				return;
			}

			if (!socket.Connected) {
				backlog.Add(data);
			} else {
				if (!ready || backlog.Count > 0) {
					backlog.Add(data);
				} else {
					ready = false;
					if (!compatibilityMode) {
						data = buildArray(data);
					}
					socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(onSend), socket);
				}
			}
		}
		public string ConnectedIp {
			get {
				return ((IPEndPoint) socket.RemoteEndPoint).Address.ToString();
			}
		}

		public bool IsConnected {
			get {
				try {
					//do not switch conditions. Potential race condition if you do.
					return socket.Connected && !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
				} catch (Exception) {
					return false;
				}
			}
		}

		//private
		private void onSend(IAsyncResult r) {
			Socket socket = (Socket) r.AsyncState;

			socket.EndSend(r);
			if (DataSent != null) {
				DataSent.Invoke(this, EventArgs.Empty);
			}
			if (socket.Connected) {
				sendNext();
			}
		}
		private void onReceive(IAsyncResult r) {
			Socket socket = (Socket) r.AsyncState;

			int bytesRead = socket.EndReceive(r);

			if (!compatibilityMode) {
				if (bytesRead == buffer.Length) {
					currentPacket.AddRange(buffer);
					checkPacket();
				} else if (bytesRead > 0) {
					currentPacket.AddRange(clipArray(buffer, bytesRead));
					checkPacket();
				}
			} else {
				if (DataReceived != null) {
					DataReceived.Invoke(this, new DataCompleteEventArgs((byte[]) buffer.Clone()));
				}
			}

			if (socket.Connected) {
				socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(onReceive), socket);
			}
		}

		private void onTimer(object sender, ElapsedEventArgs e) {
			if (IsConnected) {
				return;
			}

			disconnectTimer.Stop();

			backlog.Clear();
			currentPacket.Clear();
			if (Disconnected != null) {
				Disconnected.Invoke(this, EventArgs.Empty);
			}
		}

		private void sendNext() {
			if (backlog.Count == 0) {
				ready = true;
				return;
			}

			byte[] data = (!compatibilityMode) ? buildArray(backlog[0]) : backlog[0];
			backlog.RemoveAt(0);
			socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(onSend), socket);
		}

		private void checkPacket() {
			byte[] totalPacket = currentPacket.ToArray();
			int length = getHeader(totalPacket);
			if (length != 0 || currentPacket.Count - 4 >= length) {
				byte[] data = unbuildArray(totalPacket, length);
				currentPacket.RemoveRange(0, length + 4);
				if (DataReceived != null) {
					DataReceived.Invoke(this, new DataCompleteEventArgs(data));
				}
				checkPacket();
			}
		}

		private byte[] buildArray(byte[] data) {
			if (data.Length >= int.MaxValue - 4) {
				throw new Exception("data cannot exceed int.MaxValue - 4");
			}
			byte[] retVal = new byte[data.Length + 4];
			Array.Copy(intToBytes(data.Length), 0, retVal, 0, 4);
			Array.Copy(data, 0, retVal, 4, data.Length);
			return retVal;
		}
		private byte[] unbuildArray(byte[] data, int length) {
			byte[] retVal = new byte[length];
			Array.Copy(data, 4, retVal, 0, length);
			return retVal;
		}
		private byte[] clipArray(byte[] data, int length) {
			byte[] retVal = new byte[length];
			Array.Copy(data, 0, retVal, 0, length);
			return retVal;
		}
		private int getHeader(byte[] data) {
			if (data.Length < 4) {
				return 0;
			}
			byte[] retVal = new byte[4];
			Array.Copy(data, 0, retVal, 0, 4);
			return bytesToInt(retVal);
		}

		private byte[] intToBytes(int value) {
			byte[] retVal = BitConverter.GetBytes(value);
			if (BitConverter.IsLittleEndian) {
				Array.Reverse(retVal);
			}
			return retVal;
		}
		private int bytesToInt(byte[] bytes) {
			if (BitConverter.IsLittleEndian) {
				Array.Reverse(bytes);
			}
			return BitConverter.ToInt32(bytes, 0);
		}
	}
}
