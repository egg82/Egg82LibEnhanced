using Egg82LibEnhanced.Display;
using Egg82LibEnhanced.Geom;
using System;
using System.Runtime.InteropServices;

namespace Egg82LibEnhanced.Core {
	public class Mouse {
		//externs
		[DllImport("user32.dll")]
		private static extern bool SetCursorPos(int X, int Y);

		//vars
		private PrecisePoint _location = new PrecisePoint();
		private double _wheelDelta = 0.0d;
		private bool _leftButtonDown = false;
		private bool _middleButtonDown = false;
		private bool _rightButtonDown = false;
		private bool _extraButton1Down = false;
		private bool _extraButton2Down = false;

		private Window _currentWindow = null;

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
				if (value == _location.X || double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}

				_location.X = value;
				//SetCursorPos((int) (CurrentWindow.X + _location.X), (int) (CurrentWindow.Y + _location.Y));
			}
		}
		public double Y {
			get {
				return _location.Y;
			}
			internal set {
				if (value == _location.Y || double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}

				_location.Y = value;
				//SetCursorPos((int) (CurrentWindow.X + _location.X), (int) (CurrentWindow.Y + _location.Y));
			}
		}
		public Window CurrentWindow {
			get {
				return _currentWindow;
			}
			internal set {
				if (value == null || value == _currentWindow) {
					return;
				}

				_currentWindow = value;
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
