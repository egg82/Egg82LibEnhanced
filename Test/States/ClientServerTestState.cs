using Egg82LibEnhanced.Base;
using Egg82LibEnhanced.Events;
using Egg82LibEnhanced.Net;
using System;
using System.Text;

namespace Test.States {
	public class ClientServerTestState : BaseState {
		//vars
		private TcpClient client = new TcpClient();
		private TcpServer server = new TcpServer();
		private ushort port = 29348;

		private bool clientConnected = false;
		private int serverClientId = -1;

		//constructor
		public ClientServerTestState() {

		}

		//public

		//private
		protected override void OnEnter() {
			client.Connected += onClientConnected;
			client.Disconnected += onClientDisconnected;
			client.Error += onClientError;
			client.DataSent += onClientDataSent;
			client.DataReceived += onClientDataReceived;

			server.ClientConnected += onServerClientConnected;
			server.ClientDisconnected += onServerClientDisconnected;
			server.Error += onServerError;
			server.ClientError += onServerClientError;
			server.ClientDataSent += onServerClientDataSent;
			server.ClientDataReceived += onServerClientDataReceived;

			server.Open(port);
			client.Connect("127.0.01", port);
		}
		protected override void OnExit() {

		}
		protected override void OnUpdate(double deltaTime) {
			
		}

		private void onClientConnected(object sender, EventArgs e) {
			clientConnected = true;
			Console.WriteLine("Client connected!");
			trySendInitialData();
		}
		private void onClientDisconnected(object sender, EventArgs e) {
			clientConnected = false;
			Console.WriteLine("Client disconnected.");
		}
		private void onClientError(object sender, ExceptionEventArgs e) {
			client.Disconnect();
			Console.WriteLine("Client error: " + e.Exception.Message);
		}
		private void onClientDataSent(object sender, EventArgs e) {
			Console.WriteLine("Client data sent!");
		}
		private void onClientDataReceived(object sender, DataCompleteEventArgs e) {
			Console.WriteLine("Client data received: " + Encoding.UTF8.GetString(e.Data));
			client.Disconnect();
		}

		private void onServerError(object sender, ExceptionEventArgs e) {
			server.Close();
			Console.WriteLine("Server error: " + e.Exception.Message);
		}
		private void onServerClientConnected(object sender, ListEventArgs e) {
			serverClientId = e.Index;
			Console.WriteLine("Server client connected!");
			trySendInitialData();
		}
		private void onServerClientDisconnected(object sender, ListEventArgs e) {
			serverClientId = -1;
			Console.WriteLine("Server client disconnected.");
		}
		private void onServerClientError(object sender, ListExceptionEventArgs e) {
			Console.WriteLine("Server client error: " + e.Exception.Message);
		}
		private void onServerClientDataSent(object sender, ListEventArgs e) {
			Console.WriteLine("Server client data sent!");
		}
		private void onServerClientDataReceived(object sender, ListDataCompleteEventArgs e) {
			Console.WriteLine("Server client data received: " + Encoding.UTF8.GetString(e.Data));
			server.Send(e.Index, Encoding.UTF8.GetBytes("Testing from Server :)"));
			Console.WriteLine("Server sent data: \"Testing from Server :)\"");
		}

		private void trySendInitialData() {
			if (!clientConnected || serverClientId == -1) {
				return;
			}

			client.Send(Encoding.UTF8.GetBytes("Testing from Client :)"));
			Console.WriteLine("Client sent data: \"Testing from Client :)\"");
		}
	}
}
