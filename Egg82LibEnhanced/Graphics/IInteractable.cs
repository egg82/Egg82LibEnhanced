using Egg82LibEnhanced.Enums;
using Egg82LibEnhanced.Geom;
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

		PreciseRectangle HitBox { get; }
		double HitX { get; set; }
		double HitY { get; set; }
		double HitWidth { get; set; }
		double HitHeight { get; set; }
	}
}
