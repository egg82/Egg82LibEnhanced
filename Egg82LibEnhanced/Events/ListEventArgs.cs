using System;

namespace Egg82LibEnhanced.Events {
	public class ListEventArgs : EventArgs {
		//vars
		public static readonly new ListEventArgs Empty = new ListEventArgs(-1);

		private int _index = -1;

		//constructor
		public ListEventArgs(int index) {
			_index = index;
		}

		//public
		public int Index {
			get {
				return _index;
			}
		}

		//private

	}
}
