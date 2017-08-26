using Egg82LibEnhanced.Utils;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting;

namespace Egg82LibEnhanced.Patterns {
	public class Registry<K> : IRegistry<K> {
		//vars
		private K[] keyCache = new K[0];
		private bool keysDirty = false;
		private ConcurrentDictionary<K, Tuple<Type, dynamic>> registry = new ConcurrentDictionary<K, Tuple<Type, dynamic>>();
		private ConcurrentDictionary<dynamic, K> reverseRegistry = new ConcurrentDictionary<dynamic, K>();

		//constructor
		public Registry() {

		}

		//public
		public void SetRegister(K key, dynamic data) {
			if (key == null) {
				throw new ArgumentNullException("key");
			}

			Type type = null;
			if (data != null) {
				try {
					type = ReflectUtil.TryConvert<ObjectHandle>(data).Unwrap().GetType();
				} catch (Exception) {
					try {
						type = data.GetType();
					} catch (Exception ex2) {
						throw new Exception("Cannot get data type.", ex2);
					}
				}
			}

            Tuple<Type, dynamic> pair = null;
            if (registry.TryGetValue(key, out pair)) {
				registry.TryUpdate(key, new Tuple<Type, dynamic>(type, data), pair);
				reverseRegistry.TryRemove(pair.Item2, out K _);
			} else {
				registry.TryAdd(key, new Tuple<Type, dynamic>(type, data));
				keysDirty = true;
			}

			reverseRegistry.TryRemove(data, out K _);
			reverseRegistry.TryAdd(data, key);
		}
		public dynamic RemoveRegister(K key) {
			if (key == null) {
				throw new ArgumentNullException("key");
			}

			Tuple<Type, dynamic> pair = null;
			if (registry.TryRemove(key, out pair)) {
				reverseRegistry.TryRemove(pair.Item2, out K _);
				keysDirty = true;
				return pair.Item2;
			}
			return null;
		}
		public T RemoveRegister<T>(K key) {
			if (key == null) {
				throw new ArgumentNullException("key");
			}

			Tuple<Type, dynamic> pair = null;
			if (registry.TryRemove(key, out pair)) {
				reverseRegistry.TryRemove(pair.Item2, out K _);
				keysDirty = true;

				if (pair.Item2 == null) {
					return default(T);
				}

				if (!ReflectUtil.DoesExtend(typeof(T), pair.Item1)) {
					try {
						return ReflectUtil.TryConvert<T>(pair.Item2);
					} catch (Exception ex) {
						throw new Exception("data type cannot be converted to the type specified.", ex);
					}
				} else {
					return (T) pair.Item2;
				}
			}
			return default(T);
		}

		public dynamic GetRegister(K key) {
			if (key == null) {
				throw new ArgumentNullException("key");
			}

			Tuple<Type, dynamic> pair = null;
			if (registry.TryRemove(key, out pair)) {
				return pair.Item2;
			}
			return null;
		}
		public T GetRegister<T>(K key) {
			if (key == null) {
				throw new ArgumentNullException("key");
			}

			Tuple<Type, dynamic> pair = null;
			if (registry.TryGetValue(key, out pair)) {
				if (pair.Item2 == null) {
					return default(T);
				}

				if (!ReflectUtil.DoesExtend(typeof(T), pair.Item1)) {
					try {
						return ReflectUtil.TryConvert<T>(pair.Item2);
					} catch (Exception ex) {
						throw new Exception("data type cannot be converted to the type specified.", ex);
					}
				} else {
					return (T) pair.Item2;
				}
			}
			return default(T);
		}
		public K GetKey(dynamic data) {
			K key = default(K);
			reverseRegistry.TryGetValue(data, out key);
			return key;
		}
		public Type GetRegisterType(K key) {
			if (key == null) {
				throw new ArgumentNullException("key");
			}

			Tuple<Type, dynamic> pair = null;
			if (registry.TryGetValue(key, out pair)) {
				return pair.Item1;
			}
			return null;
		}

		public Type GetKeyClass() {
			return typeof(K);
		}

		public bool HasRegister(K key) {
			if (key == null) {
				return false;
			}
			return registry.ContainsKey(key);
		}
		public bool HasValue(dynamic data) {
			return reverseRegistry.ContainsKey(data);
		}

		public void Clear() {
			registry.Clear();
			reverseRegistry.Clear();
			keyCache = new K[0];
			keysDirty = false;
		}

		public K[] GetKeys() {
			if (keysDirty) {
				keyCache = registry.Keys.ToArray();
				keysDirty = false;
			}
			return (K[]) keyCache.Clone();
		}

		//private

	}
}
