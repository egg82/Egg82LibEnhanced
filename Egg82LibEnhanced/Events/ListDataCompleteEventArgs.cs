using System;

namespace Egg82LibEnhanced.Events {
	public class ListDataCompleteEventArgs : EventArgs {
		//vars
		public static readonly new ListDataCompleteEventArgs Empty = new ListDataCompleteEventArgs(-1, null);

		private int _index = -1;
		private byte[] _data = null;

		//constructor
		public ListDataCompleteEventArgs(int index, byte[] data) {
			_index = index;
			_data = data;
		}

		//public
		public int Index {
			get {
				return _index;
			}
		}
		public byte[] Data {
			get {
				return _data;
			}
		}

		//private

	}
}
