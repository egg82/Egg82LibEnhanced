using Egg82LibEnhanced.Events;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Egg82LibEnhanced.Net {
	public class UdpReceiver {
		//vars
		public event EventHandler<ExceptionEventArgs> Error = null;
		public event EventHandler<DataCompleteEventArgs> DataReceived = null;

		public bool CompatibilityMode = false;

		private Socket socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
		private List<byte> currentPacket = new List<byte>();
		private byte[] buffer = null;

		//constructor
		public UdpReceiver(bool compatibilityMode = false, int bufferSize = 128) {
			if (bufferSize <= 0) {
				throw new InvalidOperationException("bufferSize cannot be <= 0");
			}

			buffer = new byte[bufferSize];
			CompatibilityMode = compatibilityMode;
		}

		//public
		public void Open(ushort port) {
			if (socket.IsBound) {
				Close();
			}

			try {
				socket.Bind(new IPEndPoint(IPAddress.Any, port));
				socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(onReceive), null);
			} catch (Exception ex) {
				Error?.Invoke(this, new ExceptionEventArgs(ex));
				return;
			}
		}
		public void Close() {
			if (!socket.IsBound) {
				return;
			}
			
			try {
				socket.Close();
				socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
			} catch (Exception ex) {
				Error?.Invoke(this, new ExceptionEventArgs(ex));
				return;
			}
		}

		public bool IsOpen {
			get {
				return socket.IsBound;
			}
		}

		//private
		private void onReceive(IAsyncResult r) {
			Socket socket = (Socket) r.AsyncState;
			int bytesRead = 0;

			try {
				bytesRead = socket.EndReceive(r);
			} catch (Exception ex) {
				Error?.Invoke(this, new ExceptionEventArgs(ex));
				Close();
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

			if (IsOpen) {
				try {
					socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(onReceive), null);
				} catch (Exception ex) {
					Error?.Invoke(this, new ExceptionEventArgs(ex));
					Close();
					return;
				}
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

		private int bytesToInt(byte[] bytes) {
			if (BitConverter.IsLittleEndian) {
				Array.Reverse(bytes);
			}
			return BitConverter.ToInt32(bytes, 0);
		}
	}
}
