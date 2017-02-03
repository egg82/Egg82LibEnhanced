using Egg82LibEnhanced.Core;
using System;

namespace Egg82LibEnhanced.Events {
	public class SQLiteEventArgs : EventArgs {
		//vars
		public static readonly new SQLiteEventArgs Empty = new SQLiteEventArgs(new SQLError(), new SQLData());

		private SQLError _error;
		private SQLData _data;

		//constructor
		public SQLiteEventArgs(SQLError error, SQLData data) {
			_error = error;
			_data = data;
		}

		//public
		public SQLError Error {
			get {
				return _error;
			}
		}
		public SQLData Data {
			get {
				return _data;
			}
		}

		//private

	}
}
