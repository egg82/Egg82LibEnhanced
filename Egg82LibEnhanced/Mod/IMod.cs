using Egg82LibEnhanced.Events;
using System;

namespace Egg82LibEnhanced.Mod {
	public interface IMod {
		//functions
		event EventHandler<ExceptionEventArgs> Error;
		event EventHandler<ModDataEventArgs> Data;

		void OnData(string name, Type type, dynamic data);
	}
}
