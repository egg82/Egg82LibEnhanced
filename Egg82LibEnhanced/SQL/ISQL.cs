using Egg82LibEnhanced.Events;
using System;

namespace Egg82LibEnhanced.SQL {
	interface ISQL {
		//events
		event EventHandler OnConnect;
		event EventHandler OnDisconnect;
		event EventHandler<SQLEventArgs> OnData;
		event EventHandler<SQLEventArgs> OnError;

		//functions
		void Connect(string address, ushort port, string user, string pass, string dbName);
		void Connect(string address, string user, string pass, string dbName);
		void Connect(string filePath, string password = null);

		void Disconnect();

		Guid Query(string q, params Tuple<string, dynamic>[] queryParameters);

		bool IsConnected();
		bool IsBusy();
		bool IsExternal();
	}
}
