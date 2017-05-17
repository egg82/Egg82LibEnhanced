using Egg82LibEnhanced.Core;
using Egg82LibEnhanced.Events;
using Egg82LibEnhanced.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Threading;
using System.Timers;

namespace Egg82LibEnhanced.SQL {
	public class SQLite {
		//vars
		public event EventHandler OnConnect = null;
		public event EventHandler OnDisconnect = null;
		public event EventHandler<SQLEventArgs> OnData = null;
		public event EventHandler<SQLEventArgs> OnError = null;

		private SQLiteConnection conn = null;
		private SQLiteCommand command = null;
		
		private SynchronizedCollection<Tuple<string, Tuple<string, dynamic>[], Guid>> backlog = null;
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

			backlog = new SynchronizedCollection<Tuple<string, Tuple<string, dynamic>[], Guid>>();
			conn.StateChange += onConnStateChange;
			conn.OpenAsync();
		}

		public void Disconnect() {
			if (!_connected) {
				return;
			}
			conn.Close();
		}

		public Guid Query(string q, params Tuple<string, dynamic>[] queryParameters) {
			if (q == null || q == string.Empty) {
				return Guid.Empty;
			}

			Guid g = Guid.NewGuid();

			if (!_connected) {
				backlog.Add(new Tuple<string, Tuple<string, dynamic>[], Guid>(q, queryParameters, g));
			} else {
				if (_busy || backlog.Count > 0) {
					backlog.Add(new Tuple<string, Tuple<string, dynamic>[], Guid>(q, queryParameters, g));
				} else {
					_busy = true;
					queryInternal(q, queryParameters, g);
				}
			}

			return g;
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
		private async void queryInternal(string q, Tuple<string, dynamic>[] parameters, Guid g) {
			command = new SQLiteCommand(q, conn);

			if (parameters != null && parameters.Length > 0) {
				for (int i = 0; i < parameters.Length; i++) {
					command.Parameters.AddWithValue(parameters[i].Item1, parameters[i].Item2);
				}
			}

			DbDataReader reader = null;
			try {
				reader = await command.ExecuteReaderAsync();
			} catch (Exception ex) {
					OnError?.Invoke(this, new SQLEventArgs(q, parameters, new SQLError() {
						ex = ex
					}, new SQLData(), g));
				return;
			}

			SQLData d = new SQLData();
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
			
			OnData?.Invoke(this, new SQLEventArgs(q, parameters, new SQLError(), d, g));
			new Thread(delegate() {
				sendNext();
			}).Start();
		}

		private void onConnStateChange(object sender, StateChangeEventArgs e) {
			if (e.CurrentState == ConnectionState.Open) {
				_connected = true;
				backlogTimer.Start();
				
				OnConnect?.Invoke(this, EventArgs.Empty);
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
				
				OnDisconnect?.Invoke(this, EventArgs.Empty);
			}
		}

		private void sendNext() {
			string q = null;
			Tuple<string, dynamic>[] p = null;
			Guid g = Guid.Empty;
			
			if (backlog.Count == 0) {
				_busy = false;
				return;
			}

			q = backlog[0].Item1;
			p = backlog[0].Item2;
			g = backlog[0].Item3;
			backlog.RemoveAt(0);
			queryInternal(q, p, g);
		}

		private void onBacklogTimerElapsed(object sender, ElapsedEventArgs e) {
			if (!_busy && backlog.Count > 0) {
				_busy = true;
				sendNext();
			}
		}
	}
}
