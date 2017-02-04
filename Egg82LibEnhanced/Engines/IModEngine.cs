using Egg82LibEnhanced.Core;
using System;

namespace Egg82LibEnhanced.Engines {
	public interface IModEngine {
		//functions
		void LoadMod(string name, string path);
		void RemoveMod(string name);
		ModContainer GetMod(string name);
		int NumMods { get; }
	}
}
