using Egg82LibEnhanced.Events;
using Egg82LibEnhanced.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Timers;

namespace Egg82LibEnhanced.Net {
	public class TcpClient {
		//vars
		public event EventHandler Connected = null;
		public event EventHandler Disconnected = null;
		public event EventHandler<ExceptionEventArgs> Error = null;
		public event EventHandler<DataCompleteEventArgs> DataReceived = null;
		public event EventHandler DataSent = null;

		public bool CompatibilityMode = false;

		private string _remoteAddress = null;
		private ushort _remotePort = 0;

		private Timer disconnectTimer = new Timer(100.0d);

		private Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
		private List<byte> currentPacket = new List<byte>();
		private SynchronizedCollection<byte[]> backlog = new SynchronizedCollection<byte[]>();
		private byte[] buffer = null;
		private bool ready = false;

		//constructor
		public TcpClient(bool compatibilityMode = false, int bufferSize = 128) {
			if (bufferSize <= 0) {
				throw new InvalidOperationException("bufferSize cannot be <= 0");
			}

			buffer = new byte[bufferSize];
			CompatibilityMode = compatibilityMode;
			disconnectTimer.Elapsed += onTimer;
			disconnectTimer.AutoReset = true;
		}
		~TcpClient() {
			Disconnect();
		}

		//public
		public void Connect(string address, ushort port) {
			if (IsConnected) {
				Disconnect();
			}
			ready = false;

			IPAddress ip = NetUtil.GetAddress(address);
			if (ip == null) {
				Error?.Invoke(this, new ExceptionEventArgs(new Exception("address is invalid.")));
				return;
			}
			IPEndPoint ep = new IPEndPoint(ip, port);

			try {
				socket.BeginConnect(ep, new AsyncCallback(onConnect), socket);
			} catch (Exception ex) {
				Error?.Invoke(this, new ExceptionEventArgs(ex));
				return;
			}

			_remoteAddress = address;
			_remotePort = port;
		}
		public void Disconnect() {
			if (!IsConnected) {
				return;
			}

			_remoteAddress = null;
			_remotePort = 0;

			disconnectTimer.Stop();

			backlog.Clear();
			currentPacket.Clear();
			try {
				socket.Disconnect(false);
				socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
			} catch (Exception ex) {
				Error?.Invoke(this, new ExceptionEventArgs(ex));
				return;
			}
			
			Disconnected?.Invoke(this, EventArgs.Empty);
		}

		public void Send(byte[] data) {
			if (data == null || data.Length == 0) {
				return;
			}

			if (!IsConnected) {
				backlog.Add(data);
			} else {
				if (!ready || backlog.Count > 0) {
					backlog.Add(data);
				} else {
					ready = false;
					if (!CompatibilityMode) {
						data = buildArray(data);
					}
					try {
						socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(onSend), socket);
					} catch (Exception ex) {
						Error?.Invoke(this, new ExceptionEventArgs(ex));
						Disconnect();
						return;
					}
				}
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
		public string RemoteAddress {
			get {
				return _remoteAddress;
			}
		}
		public ushort RemotePort {
			get {
				return _remotePort;
			}
		}

		//private
		private void onConnect(IAsyncResult r) {
			Socket socket = (Socket) r.AsyncState;

			try {
				socket.EndConnect(r);
			} catch (Exception ex) {
				Error?.Invoke(this, new ExceptionEventArgs(ex));
				Disconnect();
				return;
			}
			
			disconnectTimer.Start();
			if (IsConnected) {
				try {
					socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(onReceive), socket);
				} catch (Exception ex) {
					Error?.Invoke(this, new ExceptionEventArgs(ex));
					Disconnect();
					return;
				}
			}
			Connected?.Invoke(this, EventArgs.Empty);
			sendNext();
		}
		private void onSend(IAsyncResult r) {
			Socket socket = (Socket) r.AsyncState;

			try {
				socket.EndSend(r);
			} catch (Exception ex) {
				Error?.Invoke(this, new ExceptionEventArgs(ex));
				Disconnect();
				return;
			}
			
			DataSent?.Invoke(this, EventArgs.Empty);
			if (IsConnected) {
				sendNext();
			}
		}
		private void onReceive(IAsyncResult r) {
			Socket socket = (Socket) r.AsyncState;
			int bytesRead = 0;

			try {
				bytesRead = socket.EndReceive(r);
			} catch (Exception ex) {
				Error?.Invoke(this, new ExceptionEventArgs(ex));
				Disconnect();
				return;
			}

			if (!CompatibilityMode) {
				if (bytesRead == buffer.Length) {
					currentPacket.AddRange(buffer);
					checkPacket();
				} else if (bytesRead > 0) {
					currentPacket.AddRange(clipArray(buffer, bytesRead));
					checkPacket();
				}
			} else {
				DataReceived?.Invoke(this, new DataCompleteEventArgs((byte[]) buffer.Clone()));
			}

			if (IsConnected) {
				try {
					socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(onReceive), socket);
				} catch (Exception ex) {
					Error?.Invoke(this, new ExceptionEventArgs(ex));
					Disconnect();
					return;
				}
			}
		}

		private void onTimer(object sender, ElapsedEventArgs e) {
			if (IsConnected) {
				return;
			}

			disconnectTimer.Stop();

			backlog.Clear();
			currentPacket.Clear();
			Disconnected?.Invoke(this, EventArgs.Empty);
		}

		private void sendNext() {
			if (backlog.Count == 0) {
				ready = true;
				return;
			}

			byte[] data = (!CompatibilityMode) ? buildArray(backlog[0]) : backlog[0];
			backlog.RemoveAt(0);
			try {
				socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(onSend), socket);
			} catch (Exception ex) {
				Error?.Invoke(this, new ExceptionEventArgs(ex));
				Disconnect();
				return;
			}
		}

		private void checkPacket() {
			if (currentPacket.Count == 0) {
				return;
			}

			byte[] totalPacket = currentPacket.ToArray();
			int length = getHeader(totalPacket);

			while (length == 0) {
				currentPacket.RemoveRange(0, 4);
				totalPacket = currentPacket.ToArray();
				length = getHeader(totalPacket);
			}

			if (currentPacket.Count - 4 >= length) {
				byte[] data = unbuildArray(totalPacket, length);
				currentPacket.RemoveRange(0, length + 4);
				DataReceived?.Invoke(this, new DataCompleteEventArgs(data));
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
