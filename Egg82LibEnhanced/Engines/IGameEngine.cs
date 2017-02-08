using Egg82LibEnhanced.Base;
using System;

namespace Egg82LibEnhanced.Engines {
	public interface IGameEngine {
		//functions
		void AddWindow(BaseWindow window);
		void RemoveWindow(BaseWindow window);
		int NumWindows { get; }
		double UpdateInterval { get; set; }
		double DrawInterval { get; set; }
		bool DrawSync { get; set; }
	}
}
