using System;

namespace Egg82LibEnhanced.Events {
	public class ProgressEventArgs : EventArgs {
		//vars
		public static readonly new ProgressEventArgs Empty = new ProgressEventArgs(0.0d, 0.0d);

		private double _loaded = 0.0d;
		private double _total = 0.0d;

		//constructor
		public ProgressEventArgs(double loaded, double total) {
			_loaded = loaded;
			_total = total;
		}

		//public
		public double Loaded {
			get {
				return _loaded;
			}
		}
		public double Total {
			get {
				return _total;
			}
		}

		//private

	}
}
