using System;
using System.Collections.Generic;
using System.Linq;

namespace Egg82LibEnhanced.Patterns {
	public class Registry : IRegistry {
		//vars
		private string[] keyCache = new string[0];
		private Dictionary<string, dynamic> registry = new Dictionary<string, dynamic>();

		//constructor
		public Registry() {

		}

		//public
		virtual public void SetRegister(string type, dynamic data) {
			if (type == null) {
				return;
			}

			dynamic result = null;

			if (data == null) {
				registry.Remove(type);
				keyCache = registry.Keys.ToArray();
			} else {
				if (result != null) {
					registry[type] = data;
				} else {
					registry.Add(type, data);
					keyCache = registry.Keys.ToArray();
				}
			}
		}
		virtual public dynamic GetRegister(string type) {
			dynamic result = null;
			registry.TryGetValue(type, out result);
			return result;
		}
		virtual public bool HasRegister(string type) {
			return registry.ContainsKey(type);
		}

		virtual public void Clear() {
			registry.Clear();
			keyCache = new string[0];
		}

		public string[] RegistryNames {
			get {
				return (string[]) keyCache.Clone();
			}
		}

		//private

	}
}
