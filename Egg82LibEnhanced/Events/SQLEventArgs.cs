using Egg82LibEnhanced.Core;
using System;

namespace Egg82LibEnhanced.Events {
	public class SQLEventArgs : EventArgs {
		//vars
		public static readonly new SQLEventArgs Empty = new SQLEventArgs(null, null, new SQLError(), new SQLData(), Guid.Empty);

		private string _query;
		private Tuple<string, dynamic>[] _queryParameters;
		private SQLError _error;
		private SQLData _data;
		private Guid _guid;

		//constructor
		public SQLEventArgs(string query, Tuple<string, dynamic>[] queryParameters, SQLError error, SQLData data, Guid guid) {
			_query = query;
			_queryParameters = queryParameters;
			_error = error;
			_data = data;
			_guid = guid;
		}

		//public
		public string Query {
			get {
				return _query;
			}
		}
		public Tuple<string, dynamic>[] QueryParameters {
			get {
				return _queryParameters;
			}
		}
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
		public Guid Guid {
			get {
				return _guid;
			}
		}

		//private

	}
}
