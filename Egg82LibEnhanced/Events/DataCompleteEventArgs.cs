using System;

namespace Egg82LibEnhanced.Events {
	public class DataCompleteEventArgs : EventArgs {
		//vars
		public static readonly new DataCompleteEventArgs Empty = new DataCompleteEventArgs(null);

		private byte[] _data = null;

		//constructor
		public DataCompleteEventArgs(byte[] data) {
			_data = data;
		}

		//public
		public byte[] Data {
			get {
				return _data;
			}
		}

		//private

	}
}
