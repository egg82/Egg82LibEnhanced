using Egg82LibEnhanced.Events;
using System;

namespace Egg82LibEnhanced.Core {
	public interface IMod {
		//events
		event EventHandler<ExceptionEventArgs> Error;
		event EventHandler<ModDataEventArgs> Data;

		//functions
		void OnLoad();
		void OnUnload();
		void OnData(dynamic data);
	}
}
