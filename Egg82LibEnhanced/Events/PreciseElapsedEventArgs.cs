using System;

namespace Egg82LibEnhanced.Events {
	public class PreciseElapsedEventArgs : EventArgs {
		//vars
		public static readonly new PreciseElapsedEventArgs Empty = new PreciseElapsedEventArgs(0.0d, 0.0d);
		
		private double _totalMilliseconds = 0.0d;
		private double _deltaTime = 0.0d;

		//constructor
		public PreciseElapsedEventArgs(double totalMilliseconds, double deltaTime) {
			_totalMilliseconds = totalMilliseconds;
			_deltaTime = deltaTime;
		}

		//public
		public double TotalMilliseconds {
			get {
				return _totalMilliseconds;
			}
		}
		public double DeltaTime {
			get {
				return _deltaTime;
			}
		}

		//private

	}
}
