using Egg82LibEnhanced.Base;
using Egg82LibEnhanced.Enums;
using Egg82LibEnhanced.Events;
using Egg82LibEnhanced.Geom;
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
		public event EventHandler<StickEventArgs> StickMoved = null;
		public event EventHandler<TriggerEventArgs> TriggerPressed = null;

		public event EventHandler<MouseMoveEventArgs> MouseMove = null;
		public event EventHandler<MouseWheelScrollEventArgs> MouseWheel = null;
		public event EventHandler<MouseButtonEventArgs> MouseDown = null;
		public event EventHandler<MouseButtonEventArgs> MouseUp = null;

		private Core.Mouse _mouse = new Core.Mouse();
		private Core.Keyboard _keyboard = new Core.Keyboard();
		private Core.Controllers _controllers = new Core.Controllers();

		private short maxAnglePos = 8192;
		private short maxAngleNeg = -8193;

		private List<BaseWindow> windows = new List<BaseWindow>();
		private BaseWindow _focusedWindow = null;

		//constructor
		public InputEngine() {
			
		}
		~InputEngine() {
			BaseWindow window = null;
			for (int i = 0; i < windows.Count; i++) {
				window = windows[i];
				window.GainedFocus -= onFocused;
				window.KeyPressed -= onKeyDown;
				window.KeyReleased -= onKeyUp;
				window.MouseMoved -= onMouseMove;
				window.MouseWheelScrolled -= onMouseWheel;
				window.MouseButtonPressed -= onMouseDown;
				window.MouseButtonReleased -= onMouseUp;
			}
			windows.Clear();
		}

		//public
		public Core.Mouse Mouse {
			get {
				return _mouse;
			}
		}
		public Core.Keyboard Keyboard {
			get {
				return _keyboard;
			}
		}
		public Core.Controllers Controllers {
			get {
				return _controllers;
			}
		}

		public BaseWindow FocusedWindow {
			get {
				return _focusedWindow;
			}
		}

		public void AddWindow(BaseWindow window) {
			if (window == null) {
				throw new ArgumentNullException("window");
			}

			if (windows.Contains(window)) {
				return;
			}
			
			window.GainedFocus += onFocused;
			window.KeyPressed += onKeyDown;
			window.KeyReleased += onKeyUp;
			window.MouseMoved += onMouseMove;
			window.MouseWheelScrolled += onMouseWheel;
			window.MouseButtonPressed += onMouseDown;
			window.MouseButtonReleased += onMouseUp;
			windows.Add(window);
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
			window.MouseWheelScrolled -= onMouseWheel;
			window.MouseButtonPressed -= onMouseDown;
			window.MouseButtonReleased -= onMouseUp;
		}

		public void Update() {
			_controllers.NumControllers = 0;

			for (int i = 0; i < _controllers.controllers.Length; i++) {
				if (!_controllers.controllers[i].IsConnected) {
					_controllers.states[i] = default(Gamepad);
					continue;
				}

				_controllers.NumControllers++;

				State s = _controllers.controllers[i].GetState();
				if (s.PacketNumber == _controllers.stateNumbers[i]) {
					continue;
				}
				_controllers.stateNumbers[i] = s.PacketNumber;

				Gamepad tempState = s.Gamepad;

				if ((tempState.Buttons & GamepadButtonFlags.A) != GamepadButtonFlags.None && (_controllers.states[i].Buttons & GamepadButtonFlags.A) == GamepadButtonFlags.None) {
					_controllers.CurrentlyUsingController = true;
					if (ButtonDown != null) {
						ButtonDown.Invoke(_focusedWindow, new ButtonEventArgs(XboxButtonCode.A));
					}
				} else if ((tempState.Buttons & GamepadButtonFlags.A) == GamepadButtonFlags.None && (_controllers.states[i].Buttons & GamepadButtonFlags.A) != GamepadButtonFlags.None) {
					_controllers.CurrentlyUsingController = true;
					if (ButtonUp != null) {
						ButtonUp.Invoke(_focusedWindow, new ButtonEventArgs(XboxButtonCode.A));
					}
				}
				if ((tempState.Buttons & GamepadButtonFlags.B) != GamepadButtonFlags.None && (_controllers.states[i].Buttons & GamepadButtonFlags.B) == GamepadButtonFlags.None) {
					_controllers.CurrentlyUsingController = true;
					if (ButtonDown != null) {
						ButtonDown.Invoke(_focusedWindow, new ButtonEventArgs(XboxButtonCode.B));
					}
				} else if ((tempState.Buttons & GamepadButtonFlags.B) == GamepadButtonFlags.None && (_controllers.states[i].Buttons & GamepadButtonFlags.B) != GamepadButtonFlags.None) {
					_controllers.CurrentlyUsingController = true;
					if (ButtonUp != null) {
						ButtonUp.Invoke(_focusedWindow, new ButtonEventArgs(XboxButtonCode.B));
					}
				}
				if ((tempState.Buttons & GamepadButtonFlags.Y) != GamepadButtonFlags.None && (_controllers.states[i].Buttons & GamepadButtonFlags.Y) == GamepadButtonFlags.None) {
					_controllers.CurrentlyUsingController = true;
					if (ButtonDown != null) {
						ButtonDown.Invoke(_focusedWindow, new ButtonEventArgs(XboxButtonCode.Y));
					}
				} else if ((tempState.Buttons & GamepadButtonFlags.Y) == GamepadButtonFlags.None && (_controllers.states[i].Buttons & GamepadButtonFlags.Y) != GamepadButtonFlags.None) {
					_controllers.CurrentlyUsingController = true;
					if (ButtonUp != null) {
						ButtonUp.Invoke(_focusedWindow, new ButtonEventArgs(XboxButtonCode.Y));
					}
				}
				if ((tempState.Buttons & GamepadButtonFlags.X) != GamepadButtonFlags.None && (_controllers.states[i].Buttons & GamepadButtonFlags.X) == GamepadButtonFlags.None) {
					_controllers.CurrentlyUsingController = true;
					if (ButtonDown != null) {
						ButtonDown.Invoke(_focusedWindow, new ButtonEventArgs(XboxButtonCode.X));
					}
				} else if ((tempState.Buttons & GamepadButtonFlags.X) == GamepadButtonFlags.None && (_controllers.states[i].Buttons & GamepadButtonFlags.X) != GamepadButtonFlags.None) {
					_controllers.CurrentlyUsingController = true;
					if (ButtonUp != null) {
						ButtonUp.Invoke(_focusedWindow, new ButtonEventArgs(XboxButtonCode.X));
					}
				}

				if ((tempState.Buttons & GamepadButtonFlags.LeftShoulder) != GamepadButtonFlags.None && (_controllers.states[i].Buttons & GamepadButtonFlags.LeftShoulder) == GamepadButtonFlags.None) {
					_controllers.CurrentlyUsingController = true;
					if (ButtonDown != null) {
						ButtonDown.Invoke(_focusedWindow, new ButtonEventArgs(XboxButtonCode.LeftBumper));
					}
				} else if ((tempState.Buttons & GamepadButtonFlags.LeftShoulder) == GamepadButtonFlags.None && (_controllers.states[i].Buttons & GamepadButtonFlags.LeftShoulder) != GamepadButtonFlags.None) {
					_controllers.CurrentlyUsingController = true;
					if (ButtonUp != null) {
						ButtonUp.Invoke(_focusedWindow, new ButtonEventArgs(XboxButtonCode.LeftBumper));
					}
				}
				if ((tempState.Buttons & GamepadButtonFlags.RightShoulder) != GamepadButtonFlags.None && (_controllers.states[i].Buttons & GamepadButtonFlags.RightShoulder) == GamepadButtonFlags.None) {
					_controllers.CurrentlyUsingController = true;
					if (ButtonDown != null) {
						ButtonDown.Invoke(_focusedWindow, new ButtonEventArgs(XboxButtonCode.RightBumper));
					}
				} else if ((tempState.Buttons & GamepadButtonFlags.RightShoulder) == GamepadButtonFlags.None && (_controllers.states[i].Buttons & GamepadButtonFlags.RightShoulder) != GamepadButtonFlags.None) {
					_controllers.CurrentlyUsingController = true;
					if (ButtonUp != null) {
						ButtonUp.Invoke(_focusedWindow, new ButtonEventArgs(XboxButtonCode.RightBumper));
					}
				}

				if ((tempState.Buttons & GamepadButtonFlags.LeftThumb) != GamepadButtonFlags.None && (_controllers.states[i].Buttons & GamepadButtonFlags.LeftThumb) == GamepadButtonFlags.None) {
					_controllers.CurrentlyUsingController = true;
					if (ButtonDown != null) {
						ButtonDown.Invoke(this, new ButtonEventArgs(XboxButtonCode.LeftStick));
					}
				} else if ((tempState.Buttons & GamepadButtonFlags.LeftThumb) == GamepadButtonFlags.None && (_controllers.states[i].Buttons & GamepadButtonFlags.LeftThumb) != GamepadButtonFlags.None) {
					_controllers.CurrentlyUsingController = true;
					if (ButtonUp != null) {
						ButtonUp.Invoke(_focusedWindow, new ButtonEventArgs(XboxButtonCode.LeftStick));
					}
				}
				if ((tempState.Buttons & GamepadButtonFlags.RightThumb) != GamepadButtonFlags.None && (_controllers.states[i].Buttons & GamepadButtonFlags.RightThumb) == GamepadButtonFlags.None) {
					_controllers.CurrentlyUsingController = true;
					if (ButtonDown != null) {
						ButtonDown.Invoke(_focusedWindow, new ButtonEventArgs(XboxButtonCode.RightStick));
					}
				} else if ((tempState.Buttons & GamepadButtonFlags.RightThumb) == GamepadButtonFlags.None && (_controllers.states[i].Buttons & GamepadButtonFlags.RightThumb) != GamepadButtonFlags.None) {
					_controllers.CurrentlyUsingController = true;
					if (ButtonUp != null) {
						ButtonUp.Invoke(_focusedWindow, new ButtonEventArgs(XboxButtonCode.RightStick));
					}
				}

				if ((tempState.Buttons & GamepadButtonFlags.Start) != GamepadButtonFlags.None && (_controllers.states[i].Buttons & GamepadButtonFlags.Start) == GamepadButtonFlags.None) {
					_controllers.CurrentlyUsingController = true;
					if (ButtonDown != null) {
						ButtonDown.Invoke(_focusedWindow, new ButtonEventArgs(XboxButtonCode.Start));
					}
				} else if ((tempState.Buttons & GamepadButtonFlags.Start) == GamepadButtonFlags.None && (_controllers.states[i].Buttons & GamepadButtonFlags.Start) != GamepadButtonFlags.None) {
					_controllers.CurrentlyUsingController = true;
					if (ButtonUp != null) {
						ButtonUp.Invoke(_focusedWindow, new ButtonEventArgs(XboxButtonCode.Start));
					}
				}
				if ((tempState.Buttons & GamepadButtonFlags.Back) != GamepadButtonFlags.None && (_controllers.states[i].Buttons & GamepadButtonFlags.Back) == GamepadButtonFlags.None) {
					_controllers.CurrentlyUsingController = true;
					if (ButtonDown != null) {
						ButtonDown.Invoke(_focusedWindow, new ButtonEventArgs(XboxButtonCode.Back));
					}
				} else if ((tempState.Buttons & GamepadButtonFlags.Back) == GamepadButtonFlags.None && (_controllers.states[i].Buttons & GamepadButtonFlags.Back) != GamepadButtonFlags.None) {
					_controllers.CurrentlyUsingController = true;
					if (ButtonUp != null) {
						ButtonUp.Invoke(_focusedWindow, new ButtonEventArgs(XboxButtonCode.Back));
					}
				}

				if ((tempState.Buttons & GamepadButtonFlags.DPadUp) != GamepadButtonFlags.None && (_controllers.states[i].Buttons & GamepadButtonFlags.DPadUp) == GamepadButtonFlags.None) {
					_controllers.CurrentlyUsingController = true;
					if (ButtonDown != null) {
						ButtonDown.Invoke(_focusedWindow, new ButtonEventArgs(XboxButtonCode.Up));
					}
				} else if ((tempState.Buttons & GamepadButtonFlags.DPadUp) == GamepadButtonFlags.None && (_controllers.states[i].Buttons & GamepadButtonFlags.DPadUp) != GamepadButtonFlags.None) {
					_controllers.CurrentlyUsingController = true;
					if (ButtonUp != null) {
						ButtonUp.Invoke(_focusedWindow, new ButtonEventArgs(XboxButtonCode.Up));
					}
				}
				if ((tempState.Buttons & GamepadButtonFlags.DPadDown) != GamepadButtonFlags.None && (_controllers.states[i].Buttons & GamepadButtonFlags.DPadDown) == GamepadButtonFlags.None) {
					_controllers.CurrentlyUsingController = true;
					if (ButtonDown != null) {
						ButtonDown.Invoke(_focusedWindow, new ButtonEventArgs(XboxButtonCode.Down));
					}
				} else if ((tempState.Buttons & GamepadButtonFlags.DPadDown) == GamepadButtonFlags.None && (_controllers.states[i].Buttons & GamepadButtonFlags.DPadDown) != GamepadButtonFlags.None) {
					_controllers.CurrentlyUsingController = true;
					if (ButtonUp != null) {
						ButtonUp.Invoke(_focusedWindow, new ButtonEventArgs(XboxButtonCode.Down));
					}
				}
				if ((tempState.Buttons & GamepadButtonFlags.DPadLeft) != GamepadButtonFlags.None && (_controllers.states[i].Buttons & GamepadButtonFlags.DPadLeft) == GamepadButtonFlags.None) {
					_controllers.CurrentlyUsingController = true;
					if (ButtonDown != null) {
						ButtonDown.Invoke(_focusedWindow, new ButtonEventArgs(XboxButtonCode.Left));
					}
				} else if ((tempState.Buttons & GamepadButtonFlags.DPadLeft) == GamepadButtonFlags.None && (_controllers.states[i].Buttons & GamepadButtonFlags.DPadLeft) != GamepadButtonFlags.None) {
					_controllers.CurrentlyUsingController = true;
					if (ButtonUp != null) {
						ButtonUp.Invoke(_focusedWindow, new ButtonEventArgs(XboxButtonCode.Left));
					}
				}
				if ((tempState.Buttons & GamepadButtonFlags.DPadRight) != GamepadButtonFlags.None && (_controllers.states[i].Buttons & GamepadButtonFlags.DPadRight) == GamepadButtonFlags.None) {
					_controllers.CurrentlyUsingController = true;
					if (ButtonDown != null) {
						ButtonDown.Invoke(_focusedWindow, new ButtonEventArgs(XboxButtonCode.Right));
					}
				} else if ((tempState.Buttons & GamepadButtonFlags.DPadRight) == GamepadButtonFlags.None && (_controllers.states[i].Buttons & GamepadButtonFlags.DPadRight) != GamepadButtonFlags.None) {
					_controllers.CurrentlyUsingController = true;
					if (ButtonUp != null) {
						ButtonUp.Invoke(_focusedWindow, new ButtonEventArgs(XboxButtonCode.Right));
					}
				}

				short dzPos = (short) (_controllers.StickDeadZone * 32767.0d);
				short dzNeg = (short) (_controllers.StickDeadZone * 32768.0d * -1.0d);

				if (tempState.LeftThumbY >= dzPos && Math.Abs(tempState.LeftThumbX) < maxAnglePos) {
					_controllers.CurrentlyUsingController = true;
					if (StickMoved != null) {
						StickMoved.Invoke(_focusedWindow, new StickEventArgs(XboxStickCode.LeftN, new PrecisePoint((tempState.LeftThumbX < 0) ? tempState.LeftThumbX / 32768.0d : tempState.LeftThumbX / 32767.0d, (tempState.LeftThumbY < 0) ? tempState.LeftThumbY / 32768.0d : tempState.LeftThumbY / 32767.0d)));
					}
				} else if (tempState.LeftThumbX >= dzPos && Math.Abs(tempState.LeftThumbY) < maxAnglePos) {
					_controllers.CurrentlyUsingController = true;
					if (StickMoved != null) {
						StickMoved.Invoke(_focusedWindow, new StickEventArgs(XboxStickCode.LeftE, new PrecisePoint((tempState.LeftThumbX < 0) ? tempState.LeftThumbX / 32768.0d : tempState.LeftThumbX / 32767.0d, (tempState.LeftThumbY < 0) ? tempState.LeftThumbY / 32768.0d : tempState.LeftThumbY / 32767.0d)));
					}
				} else if (tempState.LeftThumbY <= dzNeg && Math.Abs(tempState.LeftThumbX) < maxAnglePos) {
					_controllers.CurrentlyUsingController = true;
					if (StickMoved != null) {
						StickMoved.Invoke(_focusedWindow, new StickEventArgs(XboxStickCode.LeftS, new PrecisePoint((tempState.LeftThumbX < 0) ? tempState.LeftThumbX / 32768.0d : tempState.LeftThumbX / 32767.0d, (tempState.LeftThumbY < 0) ? tempState.LeftThumbY / 32768.0d : tempState.LeftThumbY / 32767.0d)));
					}
				} else if (tempState.LeftThumbX <= dzNeg && Math.Abs(tempState.LeftThumbY) < maxAnglePos) {
					_controllers.CurrentlyUsingController = true;
					if (StickMoved != null) {
						StickMoved.Invoke(_focusedWindow, new StickEventArgs(XboxStickCode.LeftW, new PrecisePoint((tempState.LeftThumbX < 0) ? tempState.LeftThumbX / 32768.0d : tempState.LeftThumbX / 32767.0d, (tempState.LeftThumbY < 0) ? tempState.LeftThumbY / 32768.0d : tempState.LeftThumbY / 32767.0d)));
					}
				} else if (tempState.LeftThumbY >= dzPos && tempState.LeftThumbX >= maxAnglePos) {
					_controllers.CurrentlyUsingController = true;
					if (StickMoved != null) {
						StickMoved.Invoke(_focusedWindow, new StickEventArgs(XboxStickCode.LeftNE, new PrecisePoint((tempState.LeftThumbX < 0) ? tempState.LeftThumbX / 32768.0d : tempState.LeftThumbX / 32767.0d, (tempState.LeftThumbY < 0) ? tempState.LeftThumbY / 32768.0d : tempState.LeftThumbY / 32767.0d)));
					}
				} else if (tempState.LeftThumbY >= dzPos && tempState.LeftThumbX <= maxAngleNeg) {
					_controllers.CurrentlyUsingController = true;
					if (StickMoved != null) {
						StickMoved.Invoke(_focusedWindow, new StickEventArgs(XboxStickCode.LeftNW, new PrecisePoint((tempState.LeftThumbX < 0) ? tempState.LeftThumbX / 32768.0d : tempState.LeftThumbX / 32767.0d, (tempState.LeftThumbY < 0) ? tempState.LeftThumbY / 32768.0d : tempState.LeftThumbY / 32767.0d)));
					}
				} else if (tempState.LeftThumbY <= dzNeg && tempState.LeftThumbX >= maxAnglePos) {
					_controllers.CurrentlyUsingController = true;
					if (StickMoved != null) {
						StickMoved.Invoke(_focusedWindow, new StickEventArgs(XboxStickCode.LeftSE, new PrecisePoint((tempState.LeftThumbX < 0) ? tempState.LeftThumbX / 32768.0d : tempState.LeftThumbX / 32767.0d, (tempState.LeftThumbY < 0) ? tempState.LeftThumbY / 32768.0d : tempState.LeftThumbY / 32767.0d)));
					}
				} else if (tempState.LeftThumbY <= dzNeg && tempState.LeftThumbX <= maxAngleNeg) {
					_controllers.CurrentlyUsingController = true;
					if (StickMoved != null) {
						StickMoved.Invoke(_focusedWindow, new StickEventArgs(XboxStickCode.LeftSW, new PrecisePoint((tempState.LeftThumbX < 0) ? tempState.LeftThumbX / 32768.0d : tempState.LeftThumbX / 32767.0d, (tempState.LeftThumbY < 0) ? tempState.LeftThumbY / 32768.0d : tempState.LeftThumbY / 32767.0d)));
					}
				}
				if (tempState.RightThumbY >= dzPos && Math.Abs(tempState.RightThumbX) < maxAnglePos) {
					_controllers.CurrentlyUsingController = true;
					if (StickMoved != null) {
						StickMoved.Invoke(_focusedWindow, new StickEventArgs(XboxStickCode.RightN, new PrecisePoint((tempState.RightThumbX < 0) ? tempState.RightThumbX / 32768.0d : tempState.RightThumbX / 32767.0d, (tempState.RightThumbY < 0) ? tempState.RightThumbY / 32768.0d : tempState.RightThumbY / 32767.0d)));
					}
				} else if (tempState.RightThumbX >= dzPos && Math.Abs(tempState.RightThumbY) < maxAnglePos) {
					_controllers.CurrentlyUsingController = true;
					if (StickMoved != null) {
						StickMoved.Invoke(_focusedWindow, new StickEventArgs(XboxStickCode.RightE, new PrecisePoint((tempState.RightThumbX < 0) ? tempState.RightThumbX / 32768.0d : tempState.RightThumbX / 32767.0d, (tempState.RightThumbY < 0) ? tempState.RightThumbY / 32768.0d : tempState.RightThumbY / 32767.0d)));
					}
				} else if (tempState.RightThumbY <= dzNeg && Math.Abs(tempState.RightThumbX) < maxAnglePos) {
					_controllers.CurrentlyUsingController = true;
					if (StickMoved != null) {
						StickMoved.Invoke(_focusedWindow, new StickEventArgs(XboxStickCode.RightS, new PrecisePoint((tempState.RightThumbX < 0) ? tempState.RightThumbX / 32768.0d : tempState.RightThumbX / 32767.0d, (tempState.RightThumbY < 0) ? tempState.RightThumbY / 32768.0d : tempState.RightThumbY / 32767.0d)));
					}
				} else if (tempState.RightThumbX <= dzNeg && Math.Abs(tempState.RightThumbY) < maxAnglePos) {
					_controllers.CurrentlyUsingController = true;
					if (StickMoved != null) {
						StickMoved.Invoke(_focusedWindow, new StickEventArgs(XboxStickCode.RightW, new PrecisePoint((tempState.RightThumbX < 0) ? tempState.RightThumbX / 32768.0d : tempState.RightThumbX / 32767.0d, (tempState.RightThumbY < 0) ? tempState.RightThumbY / 32768.0d : tempState.RightThumbY / 32767.0d)));
					}
				} else if (tempState.RightThumbY >= dzPos && tempState.RightThumbX >= maxAnglePos) {
					_controllers.CurrentlyUsingController = true;
					if (StickMoved != null) {
						StickMoved.Invoke(_focusedWindow, new StickEventArgs(XboxStickCode.RightNE, new PrecisePoint((tempState.RightThumbX < 0) ? tempState.RightThumbX / 32768.0d : tempState.RightThumbX / 32767.0d, (tempState.RightThumbY < 0) ? tempState.RightThumbY / 32768.0d : tempState.RightThumbY / 32767.0d)));
					}
				} else if (tempState.RightThumbY >= dzPos && tempState.RightThumbX <= maxAngleNeg) {
					_controllers.CurrentlyUsingController = true;
					if (StickMoved != null) {
						StickMoved.Invoke(_focusedWindow, new StickEventArgs(XboxStickCode.RightNW, new PrecisePoint((tempState.RightThumbX < 0) ? tempState.RightThumbX / 32768.0d : tempState.RightThumbX / 32767.0d, (tempState.RightThumbY < 0) ? tempState.RightThumbY / 32768.0d : tempState.RightThumbY / 32767.0d)));
					}
				} else if (tempState.RightThumbY <= dzNeg && tempState.RightThumbX >= maxAnglePos) {
					_controllers.CurrentlyUsingController = true;
					if (StickMoved != null) {
						StickMoved.Invoke(_focusedWindow, new StickEventArgs(XboxStickCode.RightSE, new PrecisePoint((tempState.RightThumbX < 0) ? tempState.RightThumbX / 32768.0d : tempState.RightThumbX / 32767.0d, (tempState.RightThumbY < 0) ? tempState.RightThumbY / 32768.0d : tempState.RightThumbY / 32767.0d)));
					}
				} else if (tempState.RightThumbY <= dzNeg && tempState.RightThumbX <= maxAngleNeg) {
					_controllers.CurrentlyUsingController = true;
					if (StickMoved != null) {
						StickMoved.Invoke(_focusedWindow, new StickEventArgs(XboxStickCode.RightSW, new PrecisePoint((tempState.RightThumbX < 0) ? tempState.RightThumbX / 32768.0d : tempState.RightThumbX / 32767.0d, (tempState.RightThumbY < 0) ? tempState.RightThumbY / 32768.0d : tempState.RightThumbY / 32767.0d)));
					}
				}

				byte dzTrig = (byte) (_controllers.TriggerDeadZone * 255.0d);

				if (tempState.LeftTrigger >= dzTrig) {
					_controllers.CurrentlyUsingController = true;
					if (TriggerPressed != null) {
						TriggerPressed.Invoke(_focusedWindow, new TriggerEventArgs(XboxTriggerCode.Left, tempState.LeftTrigger / 255.0d));
					}
				}
				if (tempState.RightTrigger >= dzTrig) {
					_controllers.CurrentlyUsingController = true;
					if (TriggerPressed != null) {
						TriggerPressed.Invoke(_focusedWindow, new TriggerEventArgs(XboxTriggerCode.Right, tempState.RightTrigger / 255.0d));
					}
				}

				_controllers.states[i] = tempState;
			}
		}

		//private
		private void onKeyDown(object sender, KeyEventArgs e) {
			_controllers.CurrentlyUsingController = false;
			int key = (int) e.Code;

			if (key == -1) {
				return;
			}
			
			if (_keyboard.keys[key]) {
				return;
			}
			_keyboard.keys[key] = true;

			if (KeyDown != null) {
				KeyDown.Invoke(sender, e);
			}
		}
		private void onKeyUp(object sender, KeyEventArgs e) {
			_controllers.CurrentlyUsingController = false;
			int key = (int) e.Code;

			if (key == -1) {
				return;
			}

			if (!_keyboard.keys[key]) {
				return;
			}
			_keyboard.keys[key] = false;

			if (KeyUp != null) {
				KeyUp.Invoke(sender, e);
			}
		}

		private void onMouseMove(object sender, MouseMoveEventArgs e) {
			_controllers.CurrentlyUsingController = false;
			
			_mouse.X = (double) e.X;
			_mouse.Y = (double) e.Y;

			if (MouseMove != null) {
				MouseMove.Invoke(sender, e);
			}
		}
		private void onMouseWheel(object sender, MouseWheelScrollEventArgs e) {
			_controllers.CurrentlyUsingController = false;
			_mouse.WheelDelta = (double) e.Delta;

			if (MouseWheel != null) {
				MouseWheel.Invoke(sender, e);
			}
		}
		private void onMouseDown(object sender, MouseButtonEventArgs e) {
			_controllers.CurrentlyUsingController = false;

			if (e.Button == SFML.Window.Mouse.Button.Left) {
				_mouse.LeftButtonDown = true;
			} else if (e.Button == SFML.Window.Mouse.Button.Middle) {
				_mouse.MiddleButtonDown = true;
			} else if (e.Button == SFML.Window.Mouse.Button.Right) {
				_mouse.RightButtonDown = true;
			} else if (e.Button == SFML.Window.Mouse.Button.XButton1) {
				_mouse.ExtraButton1Down = true;
			} else if (e.Button == SFML.Window.Mouse.Button.XButton2) {
				_mouse.ExtraButton2Down = true;
			}

			if (MouseDown != null) {
				MouseDown.Invoke(sender, e);
			}
		}
		private void onMouseUp(object sender, MouseButtonEventArgs e) {
			_controllers.CurrentlyUsingController = false;

			if (e.Button == SFML.Window.Mouse.Button.Left) {
				_mouse.LeftButtonDown = false;
			} else if (e.Button == SFML.Window.Mouse.Button.Middle) {
				_mouse.MiddleButtonDown = false;
			} else if (e.Button == SFML.Window.Mouse.Button.Right) {
				_mouse.RightButtonDown = false;
			} else if (e.Button == SFML.Window.Mouse.Button.XButton1) {
				_mouse.ExtraButton1Down = false;
			} else if (e.Button == SFML.Window.Mouse.Button.XButton2) {
				_mouse.ExtraButton2Down = false;
			}

			if (MouseUp != null) {
				MouseUp.Invoke(sender, e);
			}
		}

		private void onFocused(object sender, EventArgs e) {
			_focusedWindow = (BaseWindow) sender;
		}
	}
}
