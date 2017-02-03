using Egg82LibEnhanced.Enums;
using System;

namespace Egg82LibEnhanced.Graphics {
	public interface IInteractable {
		//events
		event EventHandler Entered;
		event EventHandler Exited;
		event EventHandler Pressed;
		event EventHandler Released;

		//functions
		InteractableState State { get; }
	}
}
