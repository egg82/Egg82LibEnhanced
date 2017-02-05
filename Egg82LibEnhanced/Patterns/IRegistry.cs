using System;

namespace Egg82LibEnhanced.Patterns {
	public interface IRegistry {
		//functions
		void SetRegister(string name, Type type, dynamic data);
		dynamic GetRegister(string name);
		Type GetRegisterType(string name);
		bool HasRegister(string name);

		void Clear();
		string[] RegistryNames { get; }
	}
}
