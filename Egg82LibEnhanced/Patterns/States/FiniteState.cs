using System;
using System.Collections.Generic;

namespace Egg82LibEnhanced.Patterns.States {
	public abstract class FiniteState {
		//vars
		private List<Type> exitStates = new List<Type>();
		protected FiniteStateMachine FiniteStateMachine = null;

		//constructor
		public FiniteState(FiniteStateMachine machine) {
			if (machine == null) {
				throw new ArgumentNullException("machine");
			}
			FiniteStateMachine = machine;
		}

		//public
		public abstract void Enter();
		public abstract void Exit();

		public void AddExitState(Type state) {
			if (state == null) {
				throw new ArgumentNullException("state");
			}
			if (!exitStates.Contains(state)) {
				exitStates.Add(state);
			}
		}
		public void RemoveExitState(Type state) {
			if (state == null) {
				throw new ArgumentNullException("state");
			}
			exitStates.Remove(state);
		}
		public bool HasExitState(Type state) {
			if (state == null) {
				throw new ArgumentNullException("state");
			}
			return exitStates.Contains(state);
		}

		//private

	}
}
