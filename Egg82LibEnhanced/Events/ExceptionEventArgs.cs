using System;

namespace Egg82LibEnhanced.Events {
	public class ExceptionEventArgs : EventArgs {
		//vars
		public static readonly new ExceptionEventArgs Empty = new ExceptionEventArgs(null);

		private Exception _exception = null;

		//constructor
		public ExceptionEventArgs(Exception ex) {
			_exception = ex;
		}

		//public
		public Exception Exception {
			get {
				return _exception;
			}
		}

		//private

	}
}
