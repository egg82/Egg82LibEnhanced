using Egg82LibEnhanced.Mod;
using System;

namespace Egg82LibEnhanced.Core {
	public class ModContainer : IDisposable {
		//vars
		private AppDomain _domain = null;
		private ModBase _mod = null;

		//constructor
		public ModContainer(AppDomain domain, ModBase mod) {
			_domain = domain;
			_mod = mod;
		}

		//public
		public AppDomain Domain {
			get {
				return _domain;
			}
		}
		public IMod Mod {
			get {
				return _mod;
			}
		}

		public void Dispose() {
			_mod.Unload();
		}

		//private

	}
}
