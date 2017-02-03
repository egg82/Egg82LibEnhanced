using Egg82LibEnhanced.Enums.Engines;
using Egg82LibEnhanced.Utils;
using SharpDX.XInput;
using System;

namespace Egg82LibEnhanced.Core {
	public class Controllers {
		//vars
		private bool _currentlyUsingController = false;
		internal Controller[] controllers = new Controller[4];
		internal Gamepad[] states = new Gamepad[4];
		private double _deadZone = 0.05d;
		private int _numControllers = 0;

		//constructor
		public Controllers() {
			controllers[0] = new Controller(UserIndex.One);
			controllers[1] = new Controller(UserIndex.Two);
			controllers[2] = new Controller(UserIndex.Three);
			controllers[3] = new Controller(UserIndex.Four);

			for (int i = 0; i < controllers.Length; i++) {
				states[i] = controllers[i].IsConnected ? controllers[i].GetState().Gamepad : new Gamepad();
			}
		}

		//public
		public bool IsAnyButtonDown(int controller, XboxButtonCode[] buttonCodes) {
			if (controller < 0 || controller >= states.Length || !controllers[controller].IsConnected) {
				return false;
			}
			if (buttonCodes == null) {
				return true;
			}

			for (int i = 0; i < buttonCodes.Length; i++) {
				if (buttonCodes[i] == XboxButtonCode.A && (states[controller].Buttons & GamepadButtonFlags.A) != GamepadButtonFlags.None) {
					return true;
				} else if (buttonCodes[i] == XboxButtonCode.B && (states[controller].Buttons & GamepadButtonFlags.B) != GamepadButtonFlags.None) {
					return true;
				} else if (buttonCodes[i] == XboxButtonCode.Y && (states[controller].Buttons & GamepadButtonFlags.Y) != GamepadButtonFlags.None) {
					return true;
				} else if (buttonCodes[i] == XboxButtonCode.X && (states[controller].Buttons & GamepadButtonFlags.X) != GamepadButtonFlags.None) {
					return true;
				} else if (buttonCodes[i] == XboxButtonCode.LeftBumper && (states[controller].Buttons & GamepadButtonFlags.LeftShoulder) != GamepadButtonFlags.None) {
					return true;
				} else if (buttonCodes[i] == XboxButtonCode.RightBumper && (states[controller].Buttons & GamepadButtonFlags.RightShoulder) != GamepadButtonFlags.None) {
					return true;
				} else if (buttonCodes[i] == XboxButtonCode.LeftStick && (states[controller].Buttons & GamepadButtonFlags.LeftThumb) != GamepadButtonFlags.None) {
					return true;
				} else if (buttonCodes[i] == XboxButtonCode.RightStick && (states[controller].Buttons & GamepadButtonFlags.RightThumb) != GamepadButtonFlags.None) {
					return true;
				} else if (buttonCodes[i] == XboxButtonCode.Start && (states[controller].Buttons & GamepadButtonFlags.Start) != GamepadButtonFlags.None) {
					return true;
				} else if (buttonCodes[i] == XboxButtonCode.Back && (states[controller].Buttons & GamepadButtonFlags.Back) != GamepadButtonFlags.None) {
					return true;
				} else if (buttonCodes[i] == XboxButtonCode.Up && (states[controller].Buttons & GamepadButtonFlags.DPadUp) != GamepadButtonFlags.None) {
					return true;
				} else if (buttonCodes[i] == XboxButtonCode.Down && (states[controller].Buttons & GamepadButtonFlags.DPadDown) != GamepadButtonFlags.None) {
					return true;
				} else if (buttonCodes[i] == XboxButtonCode.Left && (states[controller].Buttons & GamepadButtonFlags.DPadLeft) != GamepadButtonFlags.None) {
					return true;
				} else if (buttonCodes[i] == XboxButtonCode.Right && (states[controller].Buttons & GamepadButtonFlags.DPadRight) != GamepadButtonFlags.None) {
					return true;
				}
			}

			return false;
		}
		public bool AreAllButtonsDown(int controller, XboxButtonCode[] buttonCodes) {
			if (controller < 0 || controller >= states.Length || !controllers[controller].IsConnected) {
				return false;
			}
			if (buttonCodes == null) {
				return true;
			}

			int numButtons = 0;
			for (int i = 0; i < buttonCodes.Length; i++) {
				if (buttonCodes[i] == XboxButtonCode.A && (states[controller].Buttons & GamepadButtonFlags.A) != GamepadButtonFlags.None) {
					numButtons++;
				} else if (buttonCodes[i] == XboxButtonCode.B && (states[controller].Buttons & GamepadButtonFlags.B) != GamepadButtonFlags.None) {
					numButtons++;
				} else if (buttonCodes[i] == XboxButtonCode.Y && (states[controller].Buttons & GamepadButtonFlags.Y) != GamepadButtonFlags.None) {
					numButtons++;
				} else if (buttonCodes[i] == XboxButtonCode.X && (states[controller].Buttons & GamepadButtonFlags.X) != GamepadButtonFlags.None) {
					numButtons++;
				} else if (buttonCodes[i] == XboxButtonCode.LeftBumper && (states[controller].Buttons & GamepadButtonFlags.LeftShoulder) != GamepadButtonFlags.None) {
					numButtons++;
				} else if (buttonCodes[i] == XboxButtonCode.RightBumper && (states[controller].Buttons & GamepadButtonFlags.RightShoulder) != GamepadButtonFlags.None) {
					numButtons++;
				} else if (buttonCodes[i] == XboxButtonCode.LeftStick && (states[controller].Buttons & GamepadButtonFlags.LeftThumb) != GamepadButtonFlags.None) {
					numButtons++;
				} else if (buttonCodes[i] == XboxButtonCode.RightStick && (states[controller].Buttons & GamepadButtonFlags.RightThumb) != GamepadButtonFlags.None) {
					numButtons++;
				} else if (buttonCodes[i] == XboxButtonCode.Start && (states[controller].Buttons & GamepadButtonFlags.Start) != GamepadButtonFlags.None) {
					numButtons++;
				} else if (buttonCodes[i] == XboxButtonCode.Back && (states[controller].Buttons & GamepadButtonFlags.Back) != GamepadButtonFlags.None) {
					numButtons++;
				} else if (buttonCodes[i] == XboxButtonCode.Up && (states[controller].Buttons & GamepadButtonFlags.DPadUp) != GamepadButtonFlags.None) {
					numButtons++;
				} else if (buttonCodes[i] == XboxButtonCode.Down && (states[controller].Buttons & GamepadButtonFlags.DPadDown) != GamepadButtonFlags.None) {
					numButtons++;
				} else if (buttonCodes[i] == XboxButtonCode.Left && (states[controller].Buttons & GamepadButtonFlags.DPadLeft) != GamepadButtonFlags.None) {
					numButtons++;
				} else if (buttonCodes[i] == XboxButtonCode.Right && (states[controller].Buttons & GamepadButtonFlags.DPadRight) != GamepadButtonFlags.None) {
					numButtons++;
				}
			}

			return (numButtons == buttonCodes.Length) ? true : false;
		}

		public double GetTrigger(int controller, XboxTriggerCode triggerCode) {
			if (controller < 0 || controller >= states.Length || !controllers[controller].IsConnected) {
				return 0.0d;
			}

			return (triggerCode == XboxTriggerCode.Left) ? states[controller].LeftTrigger / 255.0d : states[controller].RightTrigger / 255.0d;
		}
		public PrecisePoint GetStickPosition(int controller, XboxStickCode stickCode) {
			if (controller < 0 || controller >= states.Length || !controllers[controller].IsConnected) {
				return new PrecisePoint();
			}

			return (stickCode == XboxStickCode.Left) ? new PrecisePoint((states[controller].LeftThumbX < 0) ? states[controller].LeftThumbX / 32768.0d : states[controller].LeftThumbX / 32767.0d, (states[controller].LeftThumbY < 0) ? states[controller].LeftThumbY / 32768.0d : states[controller].LeftThumbY / 32767.0d) : new PrecisePoint((states[controller].RightThumbX < 0) ? states[controller].RightThumbX / 32768.0d : states[controller].RightThumbX / 32767.0d, (states[controller].RightThumbY < 0) ? states[controller].RightThumbY / 32768.0d : states[controller].RightThumbY / 32767.0d);
		}

		public bool CurrentlyUsingController {
			get {
				for (int i = 0; i < states.Length; i++) {
					if (states[i].LeftThumbX > _deadZone * 32767.0d || states[i].LeftThumbX < _deadZone * 32768.0d) {
						_currentlyUsingController = true;
						return true;
					} else if (states[i].RightThumbX > _deadZone * 32767.0d || states[i].RightThumbX < _deadZone * 32768.0d) {
						_currentlyUsingController = true;
						return true;
					}
					if (states[i].LeftTrigger > _deadZone * 255.0d) {
						_currentlyUsingController = true;
						return true;
					}
					if (states[i].RightTrigger > _deadZone * 255.0d) {
						_currentlyUsingController = true;
						return true;
					}
				}

				return _currentlyUsingController;
			}
			internal set {
				_currentlyUsingController = value;
			}
		}

		public void Vibrate(int controller, double leftIntensity, double rightIntensity) {
			if (controller < 0 || controller >= controllers.Length || !controllers[controller].IsConnected) {
				return;
			}

			controllers[controller].SetVibration(new Vibration() {
				LeftMotorSpeed = (ushort) (MathUtil.Clamp(0.0d, 1.0d, leftIntensity) * 65535.0d),
				RightMotorSpeed = (ushort) (MathUtil.Clamp(0.0d, 1.0d, rightIntensity) * 65535.0d)
			});
		}

		public int NumControllers {
			get {
				return _numControllers;
			}
			internal set {
				_numControllers = value;
			}
		}

		public double DeadZone {
			get {
				return _deadZone;
			}
			set {
				if (double.IsNaN(value)) {
					return;
				}
				_deadZone = value;
			}
		}

		//private

	}
}
