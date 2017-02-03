using Egg82LibEnhanced.Base;
using Egg82LibEnhanced.Enums.Engines;
using Egg82LibEnhanced.Events;
using Egg82LibEnhanced.Utils;
using SFML.Window;
using SharpDX.XInput;
using System;
using System.Collections.Generic;

namespace Egg82LibEnhanced.Engines {
	public class InputEngine : IInputEngine {
		//vars
		public event EventHandler<KeyEventArgs> KeyDown = null;
		public event EventHandler<KeyEventArgs> KeyUp = null;
		public event EventHandler<ButtonEventArgs> ButtonDown = null;
		public event EventHandler<ButtonEventArgs> ButtonUp = null;
		public event EventHandler<MouseMoveEventArgs> MouseMove = null;
		public event EventHandler<MouseWheelEventArgs> MouseWheel = null;
		public event EventHandler<MouseButtonEventArgs> MouseDown = null;
		public event EventHandler<MouseButtonEventArgs> MouseUp = null;

		private bool[] keys = new bool[256];
		private Controller[] controllers = new Controller[4];
		private Gamepad[] states = new Gamepad[4];
		private bool _lastUsingController = false;
		private double _deadZone = 0.05d;
		private int _numControllers = 0;

		private PrecisePoint _mouseLocation = new PrecisePoint();
		private int _mouseWheel = 0;
		private bool _leftMouseDown = false;
		private bool _middleMouseDown = false;
		private bool _rightMouseDown = false;
		private bool _mouseExtra1Down = false;
		private bool _mouseExtra2Down = false;

		private List<BaseWindow> windows = new List<BaseWindow>();
		private BaseWindow focusedWindow = null;

		//constructor
		public InputEngine() {
			controllers[0] = new Controller(UserIndex.One);
			controllers[1] = new Controller(UserIndex.Two);
			controllers[2] = new Controller(UserIndex.Three);
			controllers[3] = new Controller(UserIndex.Four);

			for (int i = 0; i < controllers.Length; i++) {
				states[i] = controllers[i].IsConnected ? controllers[i].GetState().Gamepad : new Gamepad();
			}
		}
		~InputEngine() {
			BaseWindow window = null;
			for (int i = 0; i < windows.Count; i++) {
				window = windows[i];
				window.GainedFocus -= onFocused;
				window.KeyPressed -= onKeyDown;
				window.KeyReleased -= onKeyUp;
				window.MouseMoved -= onMouseMove;
				window.MouseWheelMoved -= onMouseWheel;
				window.MouseButtonPressed -= onMouseDown;
				window.MouseButtonReleased -= onMouseUp;
			}
			windows.Clear();
		}

		//public
		public bool IsAnyKeyDown(int[] keyCodes) {
			if (keyCodes == null) {
				return true;
			}
			for (int i = 0; i < keyCodes.Length; i++) {
				if (keyCodes[i] < 0 || keyCodes[i] >= keys.Length) {
					continue;
				}
				if (keys[keyCodes[i]]) {
					return true;
				}
			}
			return false;
		}
		public bool AreAllKeysDown(int[] keyCodes) {
			if (keyCodes == null) {
				return true;
			}
			int numKeys = 0;
			for (int i = 0; i < keyCodes.Length; i++) {
				if (keyCodes[i] < 0 || keyCodes[i] >= keys.Length) {
					return false;
				}
				if (keys[keyCodes[i]]) {
					numKeys++;
				}
			}
			return (numKeys == keyCodes.Length) ? true : false;
		}

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

		public bool IsCurrentlyUsingController {
			get {
				for (int i = 0; i < states.Length; i++) {
					if (states[i].LeftThumbX > _deadZone * 32767.0d || states[i].LeftThumbX < _deadZone * 32768.0d) {
						_lastUsingController = true;
						return true;
					} else if (states[i].RightThumbX > _deadZone * 32767.0d || states[i].RightThumbX < _deadZone * 32768.0d) {
						_lastUsingController = true;
						return true;
					}
					if (states[i].LeftTrigger > _deadZone * 255.0d) {
						_lastUsingController = true;
						return true;
					}
					if (states[i].RightTrigger > _deadZone * 255.0d) {
						_lastUsingController = true;
						return true;
					}
				}

				return _lastUsingController;
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

		public bool IsLeftMouseDown {
			get {
				return _leftMouseDown;
			}
		}
		public bool IsMiddleMouseDown {
			get {
				return _middleMouseDown;
			}
		}
		public bool IsRightMouseDown {
			get {
				return _rightMouseDown;
			}
		}
		public bool IsMouseExtra1Down {
			get {
				return _mouseExtra1Down;
			}
		}
		public bool IsMouseExtra2Down {
			get {
				return _mouseExtra2Down;
			}
		}

		public PrecisePoint MousePosition {
			get {
				return (PrecisePoint) _mouseLocation.Clone();
			}
		}
		public double MouseX {
			get {
				return _mouseLocation.X;
			}
		}
		public double MouseY {
			get {
				return _mouseLocation.Y;
			}
		}
		public int MouseWheelDelta {
			get {
				return _mouseWheel;
			}
		}

		public int NumControllers {
			get {
				return _numControllers;
			}
		}

		public void AddWindow(BaseWindow window) {
			if (window == null) {
				throw new ArgumentNullException("window");
			}

			if (windows.Contains(window)) {
				return;
			}

			windows.Add(window);
			window.GainedFocus += onFocused;
			window.KeyPressed += onKeyDown;
			window.KeyReleased += onKeyUp;
			window.MouseMoved += onMouseMove;
			window.MouseWheelMoved += onMouseWheel;
			window.MouseButtonPressed += onMouseDown;
			window.MouseButtonReleased += onMouseUp;
		}
		public void RemoveWindow(BaseWindow window) {
			if (window == null) {
				throw new ArgumentNullException("window");
			}

			int index = windows.IndexOf(window);
			if (index == -1) {
				return;
			}

			windows.RemoveAt(index);
			window.GainedFocus -= onFocused;
			window.KeyPressed -= onKeyDown;
			window.KeyReleased -= onKeyUp;
			window.MouseMoved -= onMouseMove;
			window.MouseWheelMoved -= onMouseWheel;
			window.MouseButtonPressed -= onMouseDown;
			window.MouseButtonReleased -= onMouseUp;
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

		public void Update() {
			_numControllers = 0;

			for (int i = 0; i < controllers.Length; i++) {
				if (!controllers[i].IsConnected) {
					states[i] = default(Gamepad);
					continue;
				}

				Gamepad tempState = controllers[i].GetState().Gamepad;
				_numControllers++;

				if ((tempState.Buttons & GamepadButtonFlags.A) != GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.A) == GamepadButtonFlags.None) {
					_lastUsingController = true;
					if (ButtonDown != null) {
						ButtonDown.Invoke(focusedWindow, new ButtonEventArgs(XboxButtonCode.A));
					}
				} else if ((tempState.Buttons & GamepadButtonFlags.A) == GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.A) != GamepadButtonFlags.None) {
					_lastUsingController = true;
					if (ButtonUp != null) {
						ButtonUp.Invoke(focusedWindow, new ButtonEventArgs(XboxButtonCode.A));
					}
				}
				if ((tempState.Buttons & GamepadButtonFlags.B) != GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.B) == GamepadButtonFlags.None) {
					_lastUsingController = true;
					if (ButtonDown != null) {
						ButtonDown.Invoke(focusedWindow, new ButtonEventArgs(XboxButtonCode.B));
					}
				} else if ((tempState.Buttons & GamepadButtonFlags.B) == GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.B) != GamepadButtonFlags.None) {
					_lastUsingController = true;
					if (ButtonUp != null) {
						ButtonUp.Invoke(focusedWindow, new ButtonEventArgs(XboxButtonCode.B));
					}
				}
				if ((tempState.Buttons & GamepadButtonFlags.Y) != GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.Y) == GamepadButtonFlags.None) {
					_lastUsingController = true;
					if (ButtonDown != null) {
						ButtonDown.Invoke(focusedWindow, new ButtonEventArgs(XboxButtonCode.Y));
					}
				} else if ((tempState.Buttons & GamepadButtonFlags.Y) == GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.Y) != GamepadButtonFlags.None) {
					_lastUsingController = true;
					if (ButtonUp != null) {
						ButtonUp.Invoke(focusedWindow, new ButtonEventArgs(XboxButtonCode.Y));
					}
				}
				if ((tempState.Buttons & GamepadButtonFlags.X) != GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.X) == GamepadButtonFlags.None) {
					_lastUsingController = true;
					if (ButtonDown != null) {
						ButtonDown.Invoke(focusedWindow, new ButtonEventArgs(XboxButtonCode.X));
					}
				} else if ((tempState.Buttons & GamepadButtonFlags.X) == GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.X) != GamepadButtonFlags.None) {
					_lastUsingController = true;
					if (ButtonUp != null) {
						ButtonUp.Invoke(focusedWindow, new ButtonEventArgs(XboxButtonCode.X));
					}
				}

				if ((tempState.Buttons & GamepadButtonFlags.LeftShoulder) != GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.LeftShoulder) == GamepadButtonFlags.None) {
					_lastUsingController = true;
					if (ButtonDown != null) {
						ButtonDown.Invoke(focusedWindow, new ButtonEventArgs(XboxButtonCode.LeftBumper));
					}
				} else if ((tempState.Buttons & GamepadButtonFlags.LeftShoulder) == GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.LeftShoulder) != GamepadButtonFlags.None) {
					_lastUsingController = true;
					if (ButtonUp != null) {
						ButtonUp.Invoke(focusedWindow, new ButtonEventArgs(XboxButtonCode.LeftBumper));
					}
				}
				if ((tempState.Buttons & GamepadButtonFlags.RightShoulder) != GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.RightShoulder) == GamepadButtonFlags.None) {
					_lastUsingController = true;
					if (ButtonDown != null) {
						ButtonDown.Invoke(focusedWindow, new ButtonEventArgs(XboxButtonCode.RightBumper));
					}
				} else if ((tempState.Buttons & GamepadButtonFlags.RightShoulder) == GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.RightShoulder) != GamepadButtonFlags.None) {
					_lastUsingController = true;
					if (ButtonUp != null) {
						ButtonUp.Invoke(focusedWindow, new ButtonEventArgs(XboxButtonCode.RightBumper));
					}
				}

				if ((tempState.Buttons & GamepadButtonFlags.LeftThumb) != GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.LeftThumb) == GamepadButtonFlags.None) {
					_lastUsingController = true;
					if (ButtonDown != null) {
						ButtonDown.Invoke(this, new ButtonEventArgs(XboxButtonCode.LeftStick));
					}
				} else if ((tempState.Buttons & GamepadButtonFlags.LeftThumb) == GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.LeftThumb) != GamepadButtonFlags.None) {
					_lastUsingController = true;
					if (ButtonUp != null) {
						ButtonUp.Invoke(focusedWindow, new ButtonEventArgs(XboxButtonCode.LeftStick));
					}
				}
				if ((tempState.Buttons & GamepadButtonFlags.RightThumb) != GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.RightThumb) == GamepadButtonFlags.None) {
					_lastUsingController = true;
					if (ButtonDown != null) {
						ButtonDown.Invoke(focusedWindow, new ButtonEventArgs(XboxButtonCode.RightStick));
					}
				} else if ((tempState.Buttons & GamepadButtonFlags.RightThumb) == GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.RightThumb) != GamepadButtonFlags.None) {
					_lastUsingController = true;
					if (ButtonUp != null) {
						ButtonUp.Invoke(focusedWindow, new ButtonEventArgs(XboxButtonCode.RightStick));
					}
				}

				if ((tempState.Buttons & GamepadButtonFlags.Start) != GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.Start) == GamepadButtonFlags.None) {
					_lastUsingController = true;
					if (ButtonDown != null) {
						ButtonDown.Invoke(focusedWindow, new ButtonEventArgs(XboxButtonCode.Start));
					}
				} else if ((tempState.Buttons & GamepadButtonFlags.Start) == GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.Start) != GamepadButtonFlags.None) {
					_lastUsingController = true;
					if (ButtonUp != null) {
						ButtonUp.Invoke(focusedWindow, new ButtonEventArgs(XboxButtonCode.Start));
					}
				}
				if ((tempState.Buttons & GamepadButtonFlags.Back) != GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.Back) == GamepadButtonFlags.None) {
					_lastUsingController = true;
					if (ButtonDown != null) {
						ButtonDown.Invoke(focusedWindow, new ButtonEventArgs(XboxButtonCode.Back));
					}
				} else if ((tempState.Buttons & GamepadButtonFlags.Back) == GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.Back) != GamepadButtonFlags.None) {
					_lastUsingController = true;
					if (ButtonUp != null) {
						ButtonUp.Invoke(focusedWindow, new ButtonEventArgs(XboxButtonCode.Back));
					}
				}

				if ((tempState.Buttons & GamepadButtonFlags.DPadUp) != GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.DPadUp) == GamepadButtonFlags.None) {
					_lastUsingController = true;
					if (ButtonDown != null) {
						ButtonDown.Invoke(focusedWindow, new ButtonEventArgs(XboxButtonCode.Up));
					}
				} else if ((tempState.Buttons & GamepadButtonFlags.DPadUp) == GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.DPadUp) != GamepadButtonFlags.None) {
					_lastUsingController = true;
					if (ButtonUp != null) {
						ButtonUp.Invoke(focusedWindow, new ButtonEventArgs(XboxButtonCode.Up));
					}
				}
				if ((tempState.Buttons & GamepadButtonFlags.DPadDown) != GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.DPadDown) == GamepadButtonFlags.None) {
					_lastUsingController = true;
					if (ButtonDown != null) {
						ButtonDown.Invoke(focusedWindow, new ButtonEventArgs(XboxButtonCode.Down));
					}
				} else if ((tempState.Buttons & GamepadButtonFlags.DPadDown) == GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.DPadDown) != GamepadButtonFlags.None) {
					_lastUsingController = true;
					if (ButtonUp != null) {
						ButtonUp.Invoke(focusedWindow, new ButtonEventArgs(XboxButtonCode.Down));
					}
				}
				if ((tempState.Buttons & GamepadButtonFlags.DPadLeft) != GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.DPadLeft) == GamepadButtonFlags.None) {
					_lastUsingController = true;
					if (ButtonDown != null) {
						ButtonDown.Invoke(focusedWindow, new ButtonEventArgs(XboxButtonCode.Left));
					}
				} else if ((tempState.Buttons & GamepadButtonFlags.DPadLeft) == GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.DPadLeft) != GamepadButtonFlags.None) {
					_lastUsingController = true;
					if (ButtonUp != null) {
						ButtonUp.Invoke(focusedWindow, new ButtonEventArgs(XboxButtonCode.Left));
					}
				}
				if ((tempState.Buttons & GamepadButtonFlags.DPadRight) != GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.DPadRight) == GamepadButtonFlags.None) {
					_lastUsingController = true;
					if (ButtonDown != null) {
						ButtonDown.Invoke(focusedWindow, new ButtonEventArgs(XboxButtonCode.Right));
					}
				} else if ((tempState.Buttons & GamepadButtonFlags.DPadRight) == GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.DPadRight) != GamepadButtonFlags.None) {
					_lastUsingController = true;
					if (ButtonUp != null) {
						ButtonUp.Invoke(focusedWindow, new ButtonEventArgs(XboxButtonCode.Right));
					}
				}

				states[i] = tempState;
			}
		}

		//private
		private void onKeyDown(object sender, KeyEventArgs e) {
			_lastUsingController = false;
			int key = (int) e.Code;

			if (key == -1) {
				return;
			}
			
			if (keys[key]) {
				return;
			}
			keys[key] = true;

			if (KeyDown != null) {
				KeyDown.Invoke(sender, e);
			}
		}
		private void onKeyUp(object sender, KeyEventArgs e) {
			_lastUsingController = false;
			int key = (int) e.Code;

			if (key == -1) {
				return;
			}

			if (!keys[key]) {
				return;
			}
			keys[key] = false;

			if (KeyUp != null) {
				KeyUp.Invoke(sender, e);
			}
		}

		private void onMouseMove(object sender, MouseMoveEventArgs e) {
			_lastUsingController = false;
			
			_mouseLocation.X = (double) e.X;
			_mouseLocation.Y = (double) e.Y;

			if (MouseMove != null) {
				MouseMove.Invoke(sender, e);
			}
		}
		private void onMouseWheel(object sender, MouseWheelEventArgs e) {
			_lastUsingController = false;
			_mouseWheel = e.Delta;

			if (MouseWheel != null) {
				MouseWheel.Invoke(sender, e);
			}
		}
		private void onMouseDown(object sender, MouseButtonEventArgs e) {
			_lastUsingController = false;

			if (e.Button == Mouse.Button.Left) {
				_leftMouseDown = true;
			} else if (e.Button == Mouse.Button.Middle) {
				_middleMouseDown = true;
			} else if (e.Button == Mouse.Button.Right) {
				_rightMouseDown = true;
			} else if (e.Button == Mouse.Button.XButton1) {
				_mouseExtra1Down = true;
			} else if (e.Button == Mouse.Button.XButton2) {
				_mouseExtra2Down = true;
			}

			if (MouseDown != null) {
				MouseDown.Invoke(sender, e);
			}
		}
		private void onMouseUp(object sender, MouseButtonEventArgs e) {
			_lastUsingController = false;

			if (e.Button == Mouse.Button.Left) {
				_leftMouseDown = false;
			} else if (e.Button == Mouse.Button.Middle) {
				_middleMouseDown = false;
			} else if (e.Button == Mouse.Button.Right) {
				_rightMouseDown = false;
			} else if (e.Button == Mouse.Button.XButton1) {
				_mouseExtra1Down = false;
			} else if (e.Button == Mouse.Button.XButton2) {
				_mouseExtra2Down = false;
			}

			if (MouseUp != null) {
				MouseUp.Invoke(sender, e);
			}
		}

		private void onFocused(object sender, EventArgs e) {
			focusedWindow = (BaseWindow) sender;
		}
	}
}
