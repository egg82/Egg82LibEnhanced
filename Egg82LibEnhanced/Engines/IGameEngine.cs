using Egg82LibEnhanced.Display;
using System;

namespace Egg82LibEnhanced.Engines {
	public interface IGameEngine {
		//functions
		void AddWindow(Window window);
		void RemoveWindow(Window window);
		int NumWindows { get; }
		double UpdateInterval { get; set; }
		double TargetUpdateInterval { get; set; }
		double DrawInterval { get; set; }
		bool DrawSync { get; set; }
	}
}
