using Egg82LibEnhanced.Base;
using Egg82LibEnhanced.Enums.Engines;
using Egg82LibEnhanced.Events;
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

		private Core.Mouse _mouse = new Core.Mouse();
		private Core.Keyboard _keyboard = new Core.Keyboard();
		private Core.Controllers _controllers = new Core.Controllers();

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
				window.MouseWheelMoved -= onMouseWheel;
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

		public void Update() {
			_controllers.NumControllers = 0;

			for (int i = 0; i < _controllers.controllers.Length; i++) {
				if (!_controllers.controllers[i].IsConnected) {
					_controllers.states[i] = default(Gamepad);
					continue;
				}

				Gamepad tempState = _controllers.controllers[i].GetState().Gamepad;
				_controllers.NumControllers++;

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
		private void onMouseWheel(object sender, MouseWheelEventArgs e) {
			_controllers.CurrentlyUsingController = false;
			_mouse.WheelDelta = e.Delta;

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
