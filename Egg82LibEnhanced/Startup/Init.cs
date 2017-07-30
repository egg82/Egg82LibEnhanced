using Egg82LibEnhanced.Display;
using System;

namespace Egg82LibEnhanced.Startup {
	public abstract class Init : State {
		//vars

		//constructor
		public Init() {

		}

		//public
		public void Begin() {
			Start.ProvideDefaultServices();
			Enter();

			do {
				Start.UpdateEvents();
			} while (Start.NumWindowsOpen > 0);

			Exit();
			Start.DestroyDefaultServices();
		}

		//private
		protected sealed override void OnUpdate(double deltaTime) {
			
		}
		protected sealed override void OnResize(double width, double height) {

		}
	}
}
