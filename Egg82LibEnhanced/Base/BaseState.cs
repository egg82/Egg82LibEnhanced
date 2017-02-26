using Egg82LibEnhanced.Graphics;
using System;

namespace Egg82LibEnhanced.Base {
	public abstract class BaseState : DisplayObjectContainer {
		//vars
		internal volatile bool Ready = false;

		//constructor
		/// <summary>
		/// An object used as both a container for DisplayObjects and a representation of a "frame" of sorts.
		/// Think of states as a way to organize objects. You can have a main menu state, a level select state,
		/// a options menu state, and a state for level 0, 1, 2, etc.
		/// </summary>
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

		/// <summary>
		/// Called when the BaseState has been added to the window and is ready to be updated and drawn.
		/// Override this method to create everything needed for this state to be used.
		/// </summary>
		protected abstract void OnEnter();
		/// <summary>
		/// Called when the state is about the be removed from the window and is ready to be disposed of.
		/// Override this method to destroy everything that needs to be unloaded before switching states.
		/// </summary>
		protected abstract void OnExit();
		/// <summary>
		/// Called when the window has been resized.
		/// </summary>
		/// <param name="width">The new width of the window.</param>
		/// <param name="height">The new height of the window.</param>
		protected virtual void OnResize(double width, double height) {

		}
	}
}
