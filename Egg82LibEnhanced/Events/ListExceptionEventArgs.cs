using System;

namespace Egg82LibEnhanced.Events {
	public class ListExceptionEventArgs : EventArgs {
		//vars
		public static readonly new ListExceptionEventArgs Empty = new ListExceptionEventArgs(-1, null);

		private int _index = -1;
		private Exception _exception = null;

		//constructor
		public ListExceptionEventArgs(int index, Exception ex) {
			_index = index;
			_exception = ex;
		}

		//public
		public int Index {
			get {
				return _index;
			}
		}
		public Exception Exception {
			get {
				return _exception;
			}
		}

		//private

	}
}
