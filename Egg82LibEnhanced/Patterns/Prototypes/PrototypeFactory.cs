using System;
using System.Collections.Generic;

namespace Egg82LibEnhanced.Patterns.Prototypes {
	public class PrototypeFactory : IPrototypeFactory {
		//vars
		private Dictionary<string, IPrototype> masterInstances = new Dictionary<string, IPrototype>();

		//constructor
		public PrototypeFactory() {

		}

		//public
		public void AddPrototype(string name, IPrototype prototype) {
			if (name == null) {
				throw new ArgumentNullException("name");
			}
			if (prototype == null) {
				throw new ArgumentNullException("prototype");
			}

			if (masterInstances.ContainsKey(name)) {
				masterInstances[name] = prototype;
			} else {
				masterInstances.Add(name, prototype);
			}
		}
		public void RemovePrototype(string name) {
			if (name == null) {
				throw new ArgumentNullException("name");
			}
			masterInstances.Remove(name);
		}
		public bool HasPrototype(string name) {
			if (name == null) {
				throw new ArgumentNullException("name");
			}
			return masterInstances.ContainsKey(name);
		}
		public IPrototype CreateInstance(string name) {
			if (name == null) {
				throw new ArgumentNullException("name");
			}

			IPrototype masterInstance = null;
			if (masterInstances.TryGetValue(name, out masterInstance)) {
				return masterInstance.Clone();
			}
			return null;
		}
		public IPrototype[] CreateInstances(string name, int numInstances) {
			if (numInstances <= 0) {
				return null;
			}

			IPrototype masterInstance = null;
			if (masterInstances.TryGetValue(name, out masterInstance)) {
				IPrototype[] instances = new IPrototype[numInstances];
				for (int i = 0; i < numInstances; i++) {
					instances[i] = masterInstance.Clone();
				}
				return instances;
			}
			return null;
		}

		//private

	}
}
