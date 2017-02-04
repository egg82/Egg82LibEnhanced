using Egg82LibEnhanced.Utils;
using System;

namespace Egg82LibEnhanced.Core {
	public class Mouse {
		//vars
		private PrecisePoint _location = new PrecisePoint();
		private double _wheelDelta = 0.0d;
		private bool _leftButtonDown = false;
		private bool _middleButtonDown = false;
		private bool _rightButtonDown = false;
		private bool _extraButton1Down = false;
		private bool _extraButton2Down = false;

		//constructor
		public Mouse() {

		}

		//public
		public bool LeftButtonDown {
			get {
				return _leftButtonDown;
			}
			internal set {
				_leftButtonDown = value;
			}
		}
		public bool MiddleButtonDown {
			get {
				return _middleButtonDown;
			}
			internal set {
				_middleButtonDown = value;
			}
		}
		public bool RightButtonDown {
			get {
				return _rightButtonDown;
			}
			internal set {
				_rightButtonDown = value;
			}
		}
		public bool ExtraButton1Down {
			get {
				return _extraButton1Down;
			}
			internal set {
				_extraButton1Down = value;
			}
		}
		public bool ExtraButton2Down {
			get {
				return _extraButton2Down;
			}
			internal set {
				_extraButton2Down = value;
			}
		}
		public double X {
			get {
				return _location.X;
			}
			internal set {
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}
				_location.X = value;
			}
		}
		public double Y {
			get {
				return _location.Y;
			}
			internal set {
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}
				_location.Y = value;
			}
		}
		public double WheelDelta {
			get {
				return _wheelDelta;
			}
			internal set {
				_wheelDelta = value;
			}
		}

		//private

	}
}
