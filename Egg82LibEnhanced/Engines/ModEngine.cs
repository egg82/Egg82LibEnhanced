using Egg82LibEnhanced.Core;
using Egg82LibEnhanced.Patterns;
using Egg82LibEnhanced.Reflection.ExceptionHandlers;
using Egg82LibEnhanced.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Egg82LibEnhanced.Engines {
	public class ModEngine : IModEngine, IDisposable {
		//vars
		private ConcurrentDictionary<string, ModContainer> mods = new ConcurrentDictionary<string, ModContainer>();

		//constructor
		public ModEngine() {

		}
		~ModEngine() {
			Dispose();
		}

		//public
		public void LoadMod(string name, string path) {
			if (name == null) {
				throw new ArgumentNullException("name");
			}
			if (path == null) {
				throw new ArgumentNullException("path");
			}
			if (!FileUtil.PathExists(path)) {
				throw new Exception("path does not exist.");
			}
			if (!FileUtil.PathIsFile(path)) {
				throw new Exception("path is not a file.");
			}

			AppDomain domain = AppDomain.CreateDomain(name);
			ServiceLocator.GetService<IExceptionHandler>().AddDomain(domain);
			Type t = typeof(IMod);
			IMod mod = null;
			try {
				mod = (IMod) domain.CreateInstanceFromAndUnwrap(path, t.Name);
			} catch (Exception ex) {
				throw new Exception("Cannot create instance of mod.", ex);
			}
			if (mod == null) {
				throw new Exception("Cannot create instance of mod.");
			}

			mod.OnLoad();

			if (mods.ContainsKey(name)) {
				mods[name].Dispose();
				mods[name] = new ModContainer(domain, mod);
			} else {
				mods.TryAdd(name, new ModContainer(domain, mod));
			}
		}
		public void RemoveMod(string name) {
			if (name == null) {
				throw new ArgumentNullException("name");
			}

			ModContainer retVal = null;
			if (mods.TryGetValue(name, out retVal)) {
				retVal.Dispose();
				ServiceLocator.GetService<IExceptionHandler>().RemoveDomain(retVal.Domain);
				AppDomain.Unload(retVal.Domain);
				mods.TryRemove(name, out _);
			}
		}
		public ModContainer GetMod(string name) {
			if (name == null) {
				throw new ArgumentNullException("name");
			}

			ModContainer retVal = null;
			if (mods.TryGetValue(name, out retVal)) {
				return retVal;
			}
			return null;
		}
		public int NumMods {
			get {
				return mods.Count;
			}
		}

		public void Dispose() {
			foreach (KeyValuePair<string, ModContainer> kvp in mods) {
				kvp.Value.Dispose();
			}
		}

		//private

	}
}
