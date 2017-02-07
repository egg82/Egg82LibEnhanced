using System;

namespace Egg82LibEnhanced.Events {
	public class PreciseElapsedEventArgs : EventArgs {
		//vars
		public static readonly new PreciseElapsedEventArgs Empty = new PreciseElapsedEventArgs(0.0d, 0.0d);

		private double _elapsedMilliseconds = 0.0d;
		private double _deltaTime = 0.0d;

		//constructor
		public PreciseElapsedEventArgs(double elapsedMilliseconds, double deltaTime) {
			_elapsedMilliseconds = elapsedMilliseconds;
			_deltaTime = deltaTime;
		}

		//public
		public double ElapsedMilliseconds {
			get {
				return _elapsedMilliseconds;
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
