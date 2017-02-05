using Egg82LibEnhanced.Base;
using Egg82LibEnhanced.Events;
using SFML.Window;
using System;

namespace Egg82LibEnhanced.Engines {
	public interface IInputEngine {
		//events
		event EventHandler<KeyEventArgs> KeyDown;
		event EventHandler<KeyEventArgs> KeyUp;

		event EventHandler<ButtonEventArgs> ButtonDown;
		event EventHandler<ButtonEventArgs> ButtonUp;
		event EventHandler<StickEventArgs> StickMoved;
		event EventHandler<TriggerEventArgs> TriggerPressed;

		event EventHandler<MouseMoveEventArgs> MouseMove;
		event EventHandler<MouseWheelScrollEventArgs> MouseWheel;
		event EventHandler<MouseButtonEventArgs> MouseDown;
		event EventHandler<MouseButtonEventArgs> MouseUp;

		//functions
		Core.Mouse Mouse { get; }
		Core.Keyboard Keyboard { get; }
		Core.Controllers Controllers { get; }

		BaseWindow FocusedWindow { get; }

		void AddWindow(BaseWindow window);
		void RemoveWindow(BaseWindow window);
		void Update();
	}
}
