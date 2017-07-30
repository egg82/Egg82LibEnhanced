using Egg82LibEnhanced.Events;
using Egg82LibEnhanced.Startup;
using System;

namespace Egg82LibEnhanced.Mod {
	public abstract class ModBase : IMod {
		//vars
		public event EventHandler<ExceptionEventArgs> Error;
		public event EventHandler<ModDataEventArgs> Data;

		//constructor
		public ModBase() {

		}

		//public
		public void Load() {
			Start.ProvideModServices();
			OnLoad();
		}
		public void Unload() {
			OnUnload();
			Start.DestroyModServices();
		}
		public abstract void OnData(string name, Type type, dynamic data);

		//private
		protected abstract void OnLoad();
		protected abstract void OnUnload();
	}
}
