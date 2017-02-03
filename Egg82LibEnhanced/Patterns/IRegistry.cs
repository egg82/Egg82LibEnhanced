using System;

namespace Egg82LibEnhanced.Patterns {
	public interface IRegistry {
		//functions
		void SetRegister(string type, dynamic data);
		dynamic GetRegister(string type);
		bool HasRegister(string type);

		void Clear();
		string[] RegistryNames { get; }
	}
}
