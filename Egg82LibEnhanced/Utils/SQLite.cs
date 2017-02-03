using Egg82LibEnhanced.Core;
using Egg82LibEnhanced.Events;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Threading;
using System.Timers;

namespace Egg82LibEnhanced.Utils {
	public class SQLite {
		//vars
		public event EventHandler OnConnect = null;
		public event EventHandler OnDisconnect = null;
		public event EventHandler<SQLiteEventArgs> OnData = null;
		public event EventHandler<SQLiteEventArgs> OnError = null;

		private SQLiteConnection conn = null;
		private SQLiteCommand command = null;
		
		private SynchronizedCollection<Tuple<string, object>> backlog = null;
		private bool _busy = false;
		private bool _connected = false;
		private System.Timers.Timer backlogTimer = new System.Timers.Timer(100.0d);

		private string dbFile = null;

		//constructor
		public SQLite() {
			backlogTimer.Elapsed += onBacklogTimerElapsed;
			backlogTimer.AutoReset = true;
		}

		//public
		public void Connect(string dbFile, string password = null) {
			if (dbFile == null || dbFile == string.Empty) {
				return;
			}

			if (!FileUtil.PathExists(dbFile)) {
				FileUtil.CreateFile(dbFile);
			} else {
				if (!FileUtil.PathIsFile(dbFile)) {
					return;
				}
			}

			this.dbFile = dbFile;
			_connected = false;
			_busy = true;
			
			if (password != null) {
				try {
					conn = new SQLiteConnection("Data Source=" + dbFile + ";Version=3;Password=" + password + ";");
					conn.Open();
					conn.ChangePassword(password);
					conn.Close();
				} catch (Exception) {
					conn = new SQLiteConnection("Data Source=" + dbFile + ";Version=3;");
					conn.Open();
					conn.ChangePassword(password);
					conn.Close();
				}
			} else {
				conn = new SQLiteConnection("Data Source=" + dbFile + ";Version=3;");
			}

			backlog = new SynchronizedCollection<Tuple<string, object>>();
			conn.StateChange += onConnStateChange;
			conn.OpenAsync();
		}

		public void Disconnect() {
			if (!_connected) {
				return;
			}
			conn.Close();
		}

		public void Query(string q, object data = null) {
			if (q == null || q == string.Empty) {
				return;
			}

			if (!_connected) {
				backlog.Add(new Tuple<string, object>(q, data));
			} else {
				if (_busy || backlog.Count > 0) {
					backlog.Add(new Tuple<string, object>(q, data));
				} else {
					_busy = true;
					queryInternal(q, data);
				}
			}
		}

		public bool Connected {
			get {
				return _connected;
			}
		}
		public bool Busy {
			get {
				return _busy;
			}
		}

		//private
		private async void queryInternal(string q, object data) {
			command = new SQLiteCommand(q, conn);

			DbDataReader reader = null;
			try {
				reader = await command.ExecuteReaderAsync();
			} catch (Exception ex) {
				if (OnError != null) {
					OnError.Invoke(this, new SQLiteEventArgs(new SQLError() {
						queryData = data,
						ex = ex
					}, new SQLData()));
				}
				return;
			}

			SQLData d = new SQLData();
			d.queryData = data;
			d.recordsAffected = reader.RecordsAffected;

			DataColumnCollection columns = reader.GetSchemaTable().Columns;
			List<string> tColumns = new List<string>();
			for (int i = 0; i < columns.Count; i++) {
				tColumns.Add(columns[i].ColumnName);
			}
			d.columns = tColumns.ToArray();

			List<object[]> tData = new List<object[]>();
			while (await reader.ReadAsync()) {
				object[] tVals = new object[tColumns.Count];
				reader.GetValues(tVals);
				tData.Add(tVals);
			}

			d.data = new object[tData.Count, tColumns.Count];
			for (int i = 0; i < tData.Count; i++) {
				for (int j = 0; j < tColumns.Count; j++) {
					d.data[i, j] = tData[i][j];
				}
			}

			if (OnData != null) {
				OnData.Invoke(this, new SQLiteEventArgs(new SQLError(), d));
			}
			new Thread(delegate() {
				sendNext();
			}).Start();
		}

		private void onConnStateChange(object sender, StateChangeEventArgs e) {
			if (e.CurrentState == ConnectionState.Open) {
				_connected = true;
				backlogTimer.Start();

				if (OnConnect != null) {
					OnConnect.Invoke(this, EventArgs.Empty);
				}
				sendNext();
			} else if (e.CurrentState == ConnectionState.Closed || e.CurrentState == ConnectionState.Broken) {
				backlogTimer.Stop();

				string oldDbFile = dbFile;
				dbFile = null;

				conn = null;
				command = null;
				backlog.Clear();
				_connected = false;
				_busy = false;

				if (OnDisconnect != null) {
					OnDisconnect.Invoke(this, EventArgs.Empty);
				}
			}
		}

		private void sendNext() {
			string q = null;
			object d = null;
			
			if (backlog.Count == 0) {
				_busy = false;
				return;
			}

			q = backlog[0].Item1;
			d = backlog[0].Item2;
			backlog.RemoveAt(0);
			queryInternal(q, d);
		}

		private void onBacklogTimerElapsed(object sender, ElapsedEventArgs e) {
			if (!_busy && backlog.Count > 0) {
				_busy = true;
				sendNext();
			}
		}
	}
}
