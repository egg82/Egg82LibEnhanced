using Egg82LibEnhanced.Graphics;
using System;

namespace Egg82LibEnhanced.Base {
	public abstract class BaseState : DisplayObjectContainer {
		//vars
		internal volatile bool Ready = false;

		//constructor
		public BaseState() {

		}

		//public

		//private
		internal virtual void Enter() {
			OnEnter();
			Ready = true;
		}
		internal virtual void Exit() {
			Ready = false;
			OnExit();
		}
		internal virtual void Resize(double width, double height) {
			OnResize(width, height);
		}

		protected abstract void OnEnter();
		protected abstract void OnExit();
		protected virtual void OnResize(double width, double height) {

		}
	}
}
