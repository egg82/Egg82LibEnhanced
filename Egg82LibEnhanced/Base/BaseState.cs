using Egg82LibEnhanced.Graphics;
using System;
using System.Collections.Generic;

namespace Egg82LibEnhanced.Base {
	public abstract class BaseState : DisplayObjectContainer {
		//vars
		private List<Type> exitStates = new List<Type>();
		internal bool Ready = false;

		//constructor
		public BaseState() {

		}

		//public
		public abstract void OnEnter();
		public abstract void OnExit();
		virtual public void OnResize(double width, double height) {

		}
		
		public bool HasExitState(Type state) {
			if (state == null) {
				throw new ArgumentNullException("state");
			}
			return exitStates.Contains(state);
		}

		//private
		protected void AddExitState(Type state) {
			if (state == null) {
				throw new ArgumentNullException("state");
			}
			if (!exitStates.Contains(state)) {
				exitStates.Add(state);
			}
		}
		protected void RemoveExitState(Type state) {
			if (state == null) {
				throw new ArgumentNullException("state");
			}
			exitStates.Remove(state);
		}
	}
}
