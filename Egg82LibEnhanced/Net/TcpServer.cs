using Egg82LibEnhanced.Core;
using Egg82LibEnhanced.Events;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Egg82LibEnhanced.Net {
	public class TcpServer {
		//vars
		public event EventHandler<ExceptionEventArgs> Error = null;
		public event EventHandler<ListEventArgs> ClientConnected = null;

		public event EventHandler<ListEventArgs> ClientDisconnected = null;
		public event EventHandler<ListExceptionEventArgs> ClientError = null;
		public event EventHandler<ListDataCompleteEventArgs> ClientDataReceived = null;
		public event EventHandler<ListEventArgs> ClientDataSent = null;

		private Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
		private bool _compatibilityMode = false;
		private List<ServerClient> clients = new List<ServerClient>();
		private object lockObj = new object();

		//constructor
		public TcpServer(bool compatibilityMode = false) {
			_compatibilityMode = compatibilityMode;
		}
		~TcpServer() {
			DisconnectAll();
		}

		//public
		public void Open(ushort port) {
			if (socket.IsBound) {
				Close();
			}

			try {
				socket.Bind(new IPEndPoint(IPAddress.Any, port));
				socket.Listen(100);
				socket.BeginAccept(new AsyncCallback(onAccept), null);
			} catch (Exception ex) {
				if (Error != null) {
					Error.Invoke(this, new ExceptionEventArgs(ex));
				}
				return;
			}
		}
		public void Close() {
			if (!socket.IsBound) {
				return;
			}

			DisconnectAll();
			clients.Clear();
			try {
				socket.Disconnect(true);
			} catch (Exception ex) {
				if (Error != null) {
					Error.Invoke(this, new ExceptionEventArgs(ex));
				}
				return;
			}
		}

		public void Send(int client, byte[] data) {
			if (client >= clients.Count) {
				return;
			}
			clients[client].Send(data);
		}
		public void SendAll(byte[] data) {
			for (int i = 0; i < clients.Count; i++) {
				clients[i].Send(data);
			}
		}

		public int NumClients {
			get {
				return clients.Count;
			}
		}

		public void Disconnect(int client) {
			if (client >= clients.Count) {
				return;
			}
			clients[client].Disconnect();
		}
		public void DisconnectAll() {
			for (int i = 0; i < clients.Count; i++) {
				clients[i].Disconnect();
			}
			clients.Clear();
		}

		public string GetClientIp(int client) {
			if (client >= clients.Count) {
				return null;
			}
			return clients[client].ConnectedIp;
		}

		public bool CompatibilityMode {
			get {
				return _compatibilityMode;
			}
			set {
				if (value == _compatibilityMode) {
					return;
				}
				_compatibilityMode = value;
				for (int i = 0; i < clients.Count; i++) {
					clients[i].compatibilityMode = value;
				}
			}
		}

		//private
		private void onAccept(IAsyncResult r) {
			ServerClient client = new ServerClient(socket.EndAccept(r), _compatibilityMode);
			client.Disconnected += onDisconnected;
			client.Error += onError;
			client.DataReceived += onDataReceived;
			client.DataSent += onDataSent;

			int index = -1;
			lock (lockObj) {
				clients.Add(client);
				index = clients.Count - 1;
			}

			if (ClientConnected != null) {
				ClientConnected.Invoke(this, new ListEventArgs(index));
			}
		}

		private void onDisconnected(object sender, EventArgs e) {
			int index = clients.IndexOf((ServerClient) sender);
			if (index < 0) {
				return;
			}

			lock (lockObj) {
				clients.RemoveAt(index);
			}

			if (ClientDisconnected != null) {
				ClientDisconnected.Invoke(this, new ListEventArgs(index));
			}
		}
		private void onError(object sender, ExceptionEventArgs e) {
			int index = clients.IndexOf((ServerClient) sender);
			if (index < 0) {
				return;
			}

			lock (lockObj) {
				clients.RemoveAt(index);
			}

			if (ClientError != null) {
				ClientError.Invoke(this, new ListExceptionEventArgs(index, e.Exception));
			}

			((ServerClient) sender).Disconnect();
		}
		private void onDataReceived(object sender, DataCompleteEventArgs e) {
			int index = clients.IndexOf((ServerClient) sender);
			if (index < 0) {
				return;
			}

			if (ClientDataReceived != null) {
				ClientDataReceived.Invoke(this, new ListDataCompleteEventArgs(index, e.Data));
			}
		}
		private void onDataSent(object sender, EventArgs e) {
			int index = clients.IndexOf((ServerClient) sender);
			if (index < 0) {
				return;
			}

			if (ClientDataSent != null) {
				ClientDataSent.Invoke(this, new ListEventArgs(index));
			}
		}
	}
}
