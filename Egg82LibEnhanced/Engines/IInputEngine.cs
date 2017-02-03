using Egg82LibEnhanced.Base;
using Egg82LibEnhanced.Enums.Engines;
using Egg82LibEnhanced.Events;
using Egg82LibEnhanced.Utils;
using SFML.Window;
using System;

namespace Egg82LibEnhanced.Engines {
	public interface IInputEngine {
		//events
		event EventHandler<KeyEventArgs> KeyDown;
		event EventHandler<KeyEventArgs> KeyUp;
		event EventHandler<ButtonEventArgs> ButtonDown;
		event EventHandler<ButtonEventArgs> ButtonUp;
		event EventHandler<MouseMoveEventArgs> MouseMove;
		event EventHandler<MouseWheelEventArgs> MouseWheel;
		event EventHandler<MouseButtonEventArgs> MouseDown;
		event EventHandler<MouseButtonEventArgs> MouseUp;

		//functions
		bool IsAnyKeyDown(int[] keyCodes);
		bool AreAllKeysDown(int[] keyCodes);
		bool IsAnyButtonDown(int controller, XboxButtonCode[] buttonCodes);
		bool AreAllButtonsDown(int controller, XboxButtonCode[] buttonCodes);

		double GetTrigger(int controller, XboxTriggerCode triggerCode);
		PrecisePoint GetStickPosition(int controller, XboxStickCode stickCode);
		bool IsCurrentlyUsingController { get; }
		void Vibrate(int controller, double leftIntensity, double rightIntensity);
		int NumControllers { get; }
		double DeadZone { get; set; }

		bool IsLeftMouseDown { get; }
		bool IsMiddleMouseDown { get; }
		bool IsRightMouseDown { get; }
		bool IsMouseExtra1Down { get; }
		bool IsMouseExtra2Down { get; }

		PrecisePoint MousePosition { get; }
		double MouseX { get; }
		double MouseY { get; }
		int MouseWheelDelta { get; }

		void AddWindow(BaseWindow window);
		void RemoveWindow(BaseWindow window);
		void Update();
	}
}
