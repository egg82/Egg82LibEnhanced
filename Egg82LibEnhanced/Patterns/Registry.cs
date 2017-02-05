using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Egg82LibEnhanced.Patterns {
	public class Registry : IRegistry {
		//vars
		private string[] keyCache = new string[0];
		private Dictionary<string, Tuple<Type, dynamic>> registry = new Dictionary<string, Tuple<Type, dynamic>>();

		//constructor
		public Registry() {

		}

		//public
		virtual public void SetRegister(string name, Type type, dynamic data) {
			if (name == null) {
				throw new ArgumentNullException("name");
			}
			if (data.GetType() != type) {
				dynamic converted = tryConvert(type, data);
				if (converted != null) {
					data = converted;
				} else {
					throw new Exception("data type cannot be converted to the type specified.");
				}
			}

			if (data == null) {
				registry.Remove(name);
				keyCache = registry.Keys.ToArray();
			} else {
				if (registry.ContainsKey(name)) {
					registry[name] = new Tuple<Type, dynamic>(type, data);
				} else {
					registry.Add(name, new Tuple<Type, dynamic>(type, data));
					keyCache = registry.Keys.ToArray();
				}
			}
		}
		virtual public dynamic GetRegister(string name) {
			if (name == null) {
				throw new ArgumentNullException("name");
			}

			Tuple<Type, dynamic> result = null;
			if (registry.TryGetValue(name, out result)) {
				return result.Item2;
			}
			return null;
		}
		virtual public Type GetRegisterType(string name) {
			if (name == null) {
				throw new ArgumentNullException("name");
			}

			Tuple<Type, dynamic> result = null;
			if (registry.TryGetValue(name, out result)) {
				return result.Item1;
			}
			return null;
		}
		virtual public bool HasRegister(string name) {
			if (name == null) {
				throw new ArgumentNullException("name");
			}

			return registry.ContainsKey(name);
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
		private dynamic tryConvert(Type type, dynamic input) {
			try {
				TypeConverter converter = TypeDescriptor.GetConverter(type);
				if (converter != null) {
					return converter.ConvertFrom(input);
				}
				return null;
			} catch (Exception) {
				return null;
			}
		}
	}
}
