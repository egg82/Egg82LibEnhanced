using Egg82LibEnhanced.Enums;
using Egg82LibEnhanced.Geom;
using Egg82LibEnhanced.Utils;
using SharpDX.XInput;
using System;

namespace Egg82LibEnhanced.Core {
	public class Controllers {
		//vars
		private bool _currentlyUsingController = false;
		internal Controller[] controllers = new Controller[4];
		internal Gamepad[] states = new Gamepad[4];
		internal int[] stateNumbers = new int[4];
		private double _stickDeadZone = 0.05d;
		private double _triggerDeadZone = 0.1d;
		private int _numControllers = 0;
		private short maxAnglePos = 8192;
		private short maxAngleNeg = -8193;
		private short dzPos = 0;
		private short dzNeg = 0;
		private byte dzTrig = 0;

		//constructor
		public Controllers() {
			dzPos = (short) (_stickDeadZone * 32767.0d);
			dzNeg = (short) (_stickDeadZone * 32768.0d * -1.0d);
			dzTrig = (byte) (_triggerDeadZone * 255.0d);

			controllers[0] = new Controller(UserIndex.One);
			controllers[1] = new Controller(UserIndex.Two);
			controllers[2] = new Controller(UserIndex.Three);
			controllers[3] = new Controller(UserIndex.Four);

			for (int i = 0; i < controllers.Length; i++) {
				states[i] = controllers[i].IsConnected ? controllers[i].GetState().Gamepad : new Gamepad();
			}
		}

		//public
		public bool IsAnyButtonDown(int controller, int[] buttonCodes) {
			if (controller < 0 || controller >= states.Length || !controllers[controller].IsConnected) {
				return false;
			}
			if (buttonCodes == null) {
				return true;
			}

			for (int i = 0; i < buttonCodes.Length; i++) {
				if (buttonCodes[i] == (int) XboxButtonCode.A && (states[controller].Buttons & GamepadButtonFlags.A) != GamepadButtonFlags.None) {
					return true;
				} else if (buttonCodes[i] == (int) XboxButtonCode.B && (states[controller].Buttons & GamepadButtonFlags.B) != GamepadButtonFlags.None) {
					return true;
				} else if (buttonCodes[i] == (int) XboxButtonCode.Y && (states[controller].Buttons & GamepadButtonFlags.Y) != GamepadButtonFlags.None) {
					return true;
				} else if (buttonCodes[i] == (int) XboxButtonCode.X && (states[controller].Buttons & GamepadButtonFlags.X) != GamepadButtonFlags.None) {
					return true;
				} else if (buttonCodes[i] == (int) XboxButtonCode.LeftBumper && (states[controller].Buttons & GamepadButtonFlags.LeftShoulder) != GamepadButtonFlags.None) {
					return true;
				} else if (buttonCodes[i] == (int) XboxButtonCode.RightBumper && (states[controller].Buttons & GamepadButtonFlags.RightShoulder) != GamepadButtonFlags.None) {
					return true;
				} else if (buttonCodes[i] == (int) XboxButtonCode.LeftStick && (states[controller].Buttons & GamepadButtonFlags.LeftThumb) != GamepadButtonFlags.None) {
					return true;
				} else if (buttonCodes[i] == (int) XboxButtonCode.RightStick && (states[controller].Buttons & GamepadButtonFlags.RightThumb) != GamepadButtonFlags.None) {
					return true;
				} else if (buttonCodes[i] == (int) XboxButtonCode.Start && (states[controller].Buttons & GamepadButtonFlags.Start) != GamepadButtonFlags.None) {
					return true;
				} else if (buttonCodes[i] == (int) XboxButtonCode.Back && (states[controller].Buttons & GamepadButtonFlags.Back) != GamepadButtonFlags.None) {
					return true;
				} else if (buttonCodes[i] == (int) XboxButtonCode.Up && (states[controller].Buttons & GamepadButtonFlags.DPadUp) != GamepadButtonFlags.None) {
					return true;
				} else if (buttonCodes[i] == (int) XboxButtonCode.Down && (states[controller].Buttons & GamepadButtonFlags.DPadDown) != GamepadButtonFlags.None) {
					return true;
				} else if (buttonCodes[i] == (int) XboxButtonCode.Left && (states[controller].Buttons & GamepadButtonFlags.DPadLeft) != GamepadButtonFlags.None) {
					return true;
				} else if (buttonCodes[i] == (int) XboxButtonCode.Right && (states[controller].Buttons & GamepadButtonFlags.DPadRight) != GamepadButtonFlags.None) {
					return true;
				} else {
					if (buttonCodes[i] == (int) XboxStickCode.LeftN && states[controller].LeftThumbY >= dzPos && Math.Abs(states[controller].LeftThumbX) < maxAnglePos) {
						return true;
					} else if (buttonCodes[i] == (int) XboxStickCode.LeftE && states[controller].LeftThumbX >= dzPos && Math.Abs(states[controller].LeftThumbY) < maxAnglePos) {
						return true;
					} else if (buttonCodes[i] == (int) XboxStickCode.LeftS && states[controller].LeftThumbY <= dzNeg && Math.Abs(states[controller].LeftThumbX) < maxAnglePos) {
						return true;
					} else if (buttonCodes[i] == (int) XboxStickCode.LeftW && states[controller].LeftThumbX <= dzNeg && Math.Abs(states[controller].LeftThumbY) < maxAnglePos) {
						return true;
					} else if (buttonCodes[i] == (int) XboxStickCode.LeftNE && states[controller].LeftThumbY >= dzPos && states[controller].LeftThumbX >= maxAnglePos) {
						return true;
					} else if (buttonCodes[i] == (int) XboxStickCode.LeftNW && states[controller].LeftThumbY >= dzPos && states[controller].LeftThumbX <= maxAngleNeg) {
						return true;
					} else if (buttonCodes[i] == (int) XboxStickCode.LeftSE && states[controller].LeftThumbY <= dzNeg && states[controller].LeftThumbX >= maxAnglePos) {
						return true;
					} else if (buttonCodes[i] == (int) XboxStickCode.LeftSW && states[controller].LeftThumbY <= dzNeg && states[controller].LeftThumbX <= maxAngleNeg) {
						return true;
					} else if (buttonCodes[i] == (int) XboxStickCode.RightN && states[controller].RightThumbY >= dzPos && Math.Abs(states[controller].RightThumbX) < maxAnglePos) {
						return true;
					} else if (buttonCodes[i] == (int) XboxStickCode.RightE && states[controller].RightThumbX >= dzPos && Math.Abs(states[controller].RightThumbY) < maxAnglePos) {
						return true;
					} else if (buttonCodes[i] == (int) XboxStickCode.RightS && states[controller].RightThumbY <= dzNeg && Math.Abs(states[controller].RightThumbX) < maxAnglePos) {
						return true;
					} else if (buttonCodes[i] == (int) XboxStickCode.RightW && states[controller].RightThumbX <= dzNeg && Math.Abs(states[controller].RightThumbY) < maxAnglePos) {
						return true;
					} else if (buttonCodes[i] == (int) XboxStickCode.RightNE && states[controller].RightThumbY >= dzPos && states[controller].RightThumbX >= maxAnglePos) {
						return true;
					} else if (buttonCodes[i] == (int) XboxStickCode.RightNW && states[controller].RightThumbY >= dzPos && states[controller].RightThumbX <= maxAngleNeg) {
						return true;
					} else if (buttonCodes[i] == (int) XboxStickCode.RightSE && states[controller].RightThumbY <= dzNeg && states[controller].RightThumbX >= maxAnglePos) {
						return true;
					} else if (buttonCodes[i] == (int) XboxStickCode.RightSW && states[controller].RightThumbY <= dzNeg && states[controller].RightThumbX <= maxAngleNeg) {
						return true;
					} else {
						if (buttonCodes[i] == (int) XboxTriggerCode.Left && states[controller].LeftTrigger > dzTrig) {
							return true;
						} else if (buttonCodes[i] == (int) XboxTriggerCode.Right && states[controller].RightTrigger > dzTrig) {
							return true;
						}
					}
				}
			}

			return false;
		}
		public bool AreAllButtonsDown(int controller, int[] buttonCodes) {
			if (controller < 0 || controller >= states.Length || !controllers[controller].IsConnected) {
				return false;
			}
			if (buttonCodes == null) {
				return true;
			}
			
			for (int i = 0; i < buttonCodes.Length; i++) {
				if (buttonCodes[i] == (int) XboxButtonCode.A && (states[controller].Buttons & GamepadButtonFlags.A) != GamepadButtonFlags.None) {
					continue;
				} else if (buttonCodes[i] == (int) XboxButtonCode.B && (states[controller].Buttons & GamepadButtonFlags.B) != GamepadButtonFlags.None) {
					continue;
				} else if (buttonCodes[i] == (int) XboxButtonCode.Y && (states[controller].Buttons & GamepadButtonFlags.Y) != GamepadButtonFlags.None) {
					continue;
				} else if (buttonCodes[i] == (int) XboxButtonCode.X && (states[controller].Buttons & GamepadButtonFlags.X) != GamepadButtonFlags.None) {
					continue;
				} else if (buttonCodes[i] == (int) XboxButtonCode.LeftBumper && (states[controller].Buttons & GamepadButtonFlags.LeftShoulder) != GamepadButtonFlags.None) {
					continue;
				} else if (buttonCodes[i] == (int) XboxButtonCode.RightBumper && (states[controller].Buttons & GamepadButtonFlags.RightShoulder) != GamepadButtonFlags.None) {
					continue;
				} else if (buttonCodes[i] == (int) XboxButtonCode.LeftStick && (states[controller].Buttons & GamepadButtonFlags.LeftThumb) != GamepadButtonFlags.None) {
					continue;
				} else if (buttonCodes[i] == (int) XboxButtonCode.RightStick && (states[controller].Buttons & GamepadButtonFlags.RightThumb) != GamepadButtonFlags.None) {
					continue;
				} else if (buttonCodes[i] == (int) XboxButtonCode.Start && (states[controller].Buttons & GamepadButtonFlags.Start) != GamepadButtonFlags.None) {
					continue;
				} else if (buttonCodes[i] == (int) XboxButtonCode.Back && (states[controller].Buttons & GamepadButtonFlags.Back) != GamepadButtonFlags.None) {
					continue;
				} else if (buttonCodes[i] == (int) XboxButtonCode.Up && (states[controller].Buttons & GamepadButtonFlags.DPadUp) != GamepadButtonFlags.None) {
					continue;
				} else if (buttonCodes[i] == (int) XboxButtonCode.Down && (states[controller].Buttons & GamepadButtonFlags.DPadDown) != GamepadButtonFlags.None) {
					continue;
				} else if (buttonCodes[i] == (int) XboxButtonCode.Left && (states[controller].Buttons & GamepadButtonFlags.DPadLeft) != GamepadButtonFlags.None) {
					continue;
				} else if (buttonCodes[i] == (int) XboxButtonCode.Right && (states[controller].Buttons & GamepadButtonFlags.DPadRight) != GamepadButtonFlags.None) {
					continue;
				} else {
					if (buttonCodes[i] == (int) XboxStickCode.LeftN && states[controller].LeftThumbY >= dzPos && Math.Abs(states[controller].LeftThumbX) < maxAnglePos) {
						continue;
					} else if (buttonCodes[i] == (int) XboxStickCode.LeftE && states[controller].LeftThumbX >= dzPos && Math.Abs(states[controller].LeftThumbY) < maxAnglePos) {
						continue;
					} else if (buttonCodes[i] == (int) XboxStickCode.LeftS && states[controller].LeftThumbY <= dzNeg && Math.Abs(states[controller].LeftThumbX) < maxAnglePos) {
						continue;
					} else if (buttonCodes[i] == (int) XboxStickCode.LeftW && states[controller].LeftThumbX <= dzNeg && Math.Abs(states[controller].LeftThumbY) < maxAnglePos) {
						continue;
					} else if (buttonCodes[i] == (int) XboxStickCode.LeftNE && states[controller].LeftThumbY >= dzPos && states[controller].LeftThumbX >= maxAnglePos) {
						continue;
					} else if (buttonCodes[i] == (int) XboxStickCode.LeftNW && states[controller].LeftThumbY >= dzPos && states[controller].LeftThumbX <= maxAngleNeg) {
						continue;
					} else if (buttonCodes[i] == (int) XboxStickCode.LeftSE && states[controller].LeftThumbY <= dzNeg && states[controller].LeftThumbX >= maxAnglePos) {
						continue;
					} else if (buttonCodes[i] == (int) XboxStickCode.LeftSW && states[controller].LeftThumbY <= dzNeg && states[controller].LeftThumbX <= maxAngleNeg) {
						continue;
					} else if (buttonCodes[i] == (int) XboxStickCode.RightN && states[controller].RightThumbY >= dzPos && Math.Abs(states[controller].RightThumbX) < maxAnglePos) {
						continue;
					} else if (buttonCodes[i] == (int) XboxStickCode.RightE && states[controller].RightThumbX >= dzPos && Math.Abs(states[controller].RightThumbY) < maxAnglePos) {
						continue;
					} else if (buttonCodes[i] == (int) XboxStickCode.RightS && states[controller].RightThumbY <= dzNeg && Math.Abs(states[controller].RightThumbX) < maxAnglePos) {
						continue;
					} else if (buttonCodes[i] == (int) XboxStickCode.RightW && states[controller].RightThumbX <= dzNeg && Math.Abs(states[controller].RightThumbY) < maxAnglePos) {
						continue;
					} else if (buttonCodes[i] == (int) XboxStickCode.RightNE && states[controller].RightThumbY >= dzPos && states[controller].RightThumbX >= maxAnglePos) {
						continue;
					} else if (buttonCodes[i] == (int) XboxStickCode.RightNW && states[controller].RightThumbY >= dzPos && states[controller].RightThumbX <= maxAngleNeg) {
						continue;
					} else if (buttonCodes[i] == (int) XboxStickCode.RightSE && states[controller].RightThumbY <= dzNeg && states[controller].RightThumbX >= maxAnglePos) {
						continue;
					} else if (buttonCodes[i] == (int) XboxStickCode.RightSW && states[controller].RightThumbY <= dzNeg && states[controller].RightThumbX <= maxAngleNeg) {
						continue;
					} else {
						if (buttonCodes[i] == (int) XboxTriggerCode.Left && states[controller].LeftTrigger > dzTrig) {
							continue;
						} else if (buttonCodes[i] == (int) XboxTriggerCode.Right && states[controller].RightTrigger > dzTrig) {
							continue;
						} else {
							return false;
						}
					}
				}
			}

			return true;
		}

		public double GetTrigger(int controller, XboxTriggerCode triggerCode) {
			if (controller < 0 || controller >= states.Length || !controllers[controller].IsConnected) {
				return 0.0d;
			}

			return (triggerCode == XboxTriggerCode.Left) ? states[controller].LeftTrigger / 255.0d : states[controller].RightTrigger / 255.0d;
		}
		public PrecisePoint GetStickPosition(int controller, XboxStickSide stickSide) {
			if (controller < 0 || controller >= states.Length || !controllers[controller].IsConnected) {
				return new PrecisePoint();
			}

			return (stickSide == XboxStickSide.Left) ? new PrecisePoint((states[controller].LeftThumbX < 0) ? states[controller].LeftThumbX / 32768.0d : states[controller].LeftThumbX / 32767.0d, (states[controller].LeftThumbY < 0) ? states[controller].LeftThumbY / 32768.0d : states[controller].LeftThumbY / 32767.0d) : new PrecisePoint((states[controller].RightThumbX < 0) ? states[controller].RightThumbX / 32768.0d : states[controller].RightThumbX / 32767.0d, (states[controller].RightThumbY < 0) ? states[controller].RightThumbY / 32768.0d : states[controller].RightThumbY / 32767.0d);
		}

		public bool CurrentlyUsingController {
			get {
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

		public double StickDeadZone {
			get {
				return _stickDeadZone;
			}
			set {
				if (double.IsNaN(value)) {
					return;
				}
				_stickDeadZone = value;
				dzPos = (short) (_stickDeadZone * 32767.0d);
				dzNeg = (short) (_stickDeadZone * 32768.0d * -1.0d);
			}
		}
		public double TriggerDeadZone {
			get {
				return _triggerDeadZone;
			}
			set {
				if (double.IsNaN(value)) {
					return;
				}
				_triggerDeadZone = value;
				dzTrig = (byte) (_triggerDeadZone * 255.0d);
			}
		}

		//private

	}
}
