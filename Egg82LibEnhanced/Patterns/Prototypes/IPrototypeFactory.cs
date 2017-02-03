using System;

namespace Egg82LibEnhanced.Patterns.Prototypes {
	public interface IPrototypeFactory {
		//functions
		void AddPrototype(string name, IPrototype prototype);
		void RemovePrototype(string name);
		bool HasPrototype(string name);
		IPrototype CreateInstance(string name);
		IPrototype[] CreateInstances(string name, int numInstances);
	}
}
