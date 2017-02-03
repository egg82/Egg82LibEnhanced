using Egg82LibEnhanced.Utils;
using System;
using System.Collections.Generic;

namespace Egg82LibEnhanced.Patterns.States {
	public class FiniteStateMachine {
		//vars
		private FiniteState currentState = null;
		private Dictionary<Type, FiniteState> states = new Dictionary<Type, FiniteState>();

		//constructor
		public FiniteStateMachine() {

		}

		//public
		public void AddState(Type state) {
			if (state == null) {
				throw new ArgumentNullException("state");
			}
			if (!ReflectUtil.DoesExtend(typeof(FiniteState), state)) {
				throw new Exception("state does not extend FiniteState.");
			}

			FiniteState ns = null;
			try {
				ns = (FiniteState) Activator.CreateInstance(state, this);
			} catch (Exception ex) {
				throw new Exception("Cannot create state.", ex);
			}

			FiniteState oldState = null;
			if (states.TryGetValue(state, out oldState)) {
				if (currentState == oldState) {
					currentState = states[state] = ns;
					currentState.Enter();
				} else {
					states[state] = ns;
				}
				if (ReflectUtil.DoesExtend(typeof(IDisposable), oldState.GetType())) {
					((IDisposable) oldState).Dispose();
				}
			} else {
				states.Add(state, ns);
			}

			if (currentState == null) {
				currentState = ns;
				currentState.Enter();
			}
		}
		public bool HasState(Type state) {
			if (state == null) {
				throw new ArgumentNullException("state");
			}
			return states.ContainsKey(state);
		}
		public void RemoveState(Type state) {
			if (state == null) {
				throw new ArgumentNullException("state");
			}
			states.Remove(state);
		}

		public bool TrySwapStates(Type state) {
			if (state == null) {
				throw new ArgumentNullException("state");
			}
			if (currentState == null || !states.ContainsKey(state) || !currentState.HasExitState(state)) {
				return false;
			}

			currentState.Exit();
			currentState = states[state];
			currentState.Enter();

			return true;
		}

		//private

	}
}
