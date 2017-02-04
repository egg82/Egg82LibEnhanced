using System;

namespace Egg82LibEnhanced.Events {
	public class ModDataEventArgs : EventArgs {
		//vars
		public static readonly new ModDataEventArgs Empty = new ModDataEventArgs(null);

		private dynamic _data = null;

		//constructor
		public ModDataEventArgs(dynamic data) {
			_data = data;
		}

		//public
		public dynamic Data {
			get {
				return _data;
			}
		}

		//private

	}
}
