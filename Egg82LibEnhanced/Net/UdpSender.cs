using Egg82LibEnhanced.Events;
using Egg82LibEnhanced.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Egg82LibEnhanced.Net {
	public class UdpSender {
		//vars
		public event EventHandler<ExceptionEventArgs> Error = null;
		public event EventHandler DataSent = null;

		public bool CompatibilityMode = false;

		private Socket socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
		private SynchronizedCollection<Tuple<IPEndPoint, byte[]>> backlog = new SynchronizedCollection<Tuple<IPEndPoint, byte[]>>();
		private bool ready = false;

		//constructor
		public UdpSender(bool compatibilityMode = false) {
			CompatibilityMode = compatibilityMode;
		}

		//public
		public void Send(byte[] data, string address, ushort port) {
			if (data == null || data.Length == 0) {
				return;
			}

			IPAddress ip = NetUtil.GetAddress(address);
			if (ip == null) {
				Error?.Invoke(this, new ExceptionEventArgs(new Exception("address is invalid.")));
				return;
			}
			IPEndPoint ep = new IPEndPoint(ip, port);
			
			if (!ready || backlog.Count > 0) {
				backlog.Add(new Tuple<IPEndPoint, byte[]>(ep, data));
			} else {
				ready = false;
				if (!CompatibilityMode) {
					data = buildArray(data);
				}
				try {
					socket.BeginSendTo(data, 0, data.Length, SocketFlags.None, ep, new AsyncCallback(onSend), socket);
				} catch (Exception ex) {
					Error?.Invoke(this, new ExceptionEventArgs(ex));
					return;
				}
			}
		}

		//private
		private void onSend(IAsyncResult r) {
			Socket socket = (Socket) r.AsyncState;

			try {
				socket.EndSendTo(r);
			} catch (Exception ex) {
				Error?.Invoke(this, new ExceptionEventArgs(ex));
			}

			DataSent?.Invoke(this, EventArgs.Empty);
			sendNext();
		}

		private void sendNext() {
			if (backlog.Count == 0) {
				ready = true;
				return;
			}

			bool good = true;
			do {
				good = true;
				Tuple<IPEndPoint, byte[]> data = backlog[0];

				if (!CompatibilityMode) {
					data = new Tuple<IPEndPoint, byte[]>(data.Item1, buildArray(data.Item2));
				}

				backlog.RemoveAt(0);
				try {
					socket.BeginSendTo(data.Item2, 0, data.Item2.Length, SocketFlags.None, data.Item1, new AsyncCallback(onSend), socket);
				} catch (Exception ex) {
					Error?.Invoke(this, new ExceptionEventArgs(ex));
					good = false;
				}
			} while (!good && backlog.Count > 0);
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

		private byte[] intToBytes(int value) {
			byte[] retVal = BitConverter.GetBytes(value);
			if (BitConverter.IsLittleEndian) {
				Array.Reverse(retVal);
			}
			return retVal;
		}
	}
}
