using System;

namespace Egg82LibEnhanced.Patterns.Interfaces {
	public interface IUpdatable {
		//functions
		void Update(double deltaTime);
		void SwapBuffers();
	}
}
