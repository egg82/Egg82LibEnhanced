using Egg82LibEnhanced.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Egg82LibEnhanced.Patterns {
	public class ServiceLocator {
		//vars
		private static SynchronizedCollection<Type> services = new SynchronizedCollection<Type>();
		private static ConcurrentDictionary<Type, dynamic> initializedServices = new ConcurrentDictionary<Type, dynamic>();
		private static ConcurrentDictionary<Type, dynamic> lookupCache = new ConcurrentDictionary<Type, dynamic>();

		//constructor
		public ServiceLocator() {

		}

		//public
		public static T GetService<T>() {
			Type type = typeof(T);
			dynamic result = null;

			if (!initializedServices.TryGetValue(type, out result)) {
				int index = services.IndexOf(type);
				if (index > -1) {
					result = initializeService(type);
					initializedServices.TryAdd(type, result);
				}
			}

			if (result == null) {
				lookupCache.TryGetValue(type, out result);
			}

			if (result == null) {
				for (int i = 0; i < services.Count; i++) {
					Type t = services[i];
					if (ReflectUtil.DoesExtend(type, t)) {
						initializedServices.TryGetValue(t, out result);
						if (result == null) {
							result = initializeService(t);
							initializedServices.TryAdd(t, result);
						}
						lookupCache.TryAdd(type, result);
						break;
					}
				}
			}

			if (result == null) {
				return default(T);
			} else {
				return (T) result;
			}
		}
		public static void ProvideService(Type type, bool lazyInitialize = true) {
			if (type == null) {
				throw new ArgumentNullException("type");
			}

			initializedServices.TryRemove(type, out _);
			foreach (KeyValuePair<Type, dynamic> kvp in lookupCache) {
				if (ReflectUtil.DoesExtend(kvp.Key, type)) {
					lookupCache.TryRemove(kvp.Key, out _);
				}
			}

			int index = services.IndexOf(type);
			if (index > -1) {
				services[index] = type;
			} else {
				services.Add(type);
			}

			if (!lazyInitialize) {
				initializedServices.TryAdd(type, initializeService(type));
			}
		}
		public static List<T> RemoveServices<T>() {
			Type type = typeof(T);
			List<T> retVal = new List<T>();

			foreach (KeyValuePair<Type, dynamic> kvp in lookupCache) {
				if (ReflectUtil.DoesExtend(kvp.Key, type)) {
					lookupCache.TryRemove(kvp.Key, out _);
				}
			}

			for (int i = services.Count - 1; i >= 0; i--) {
				Type t = services[i];
				if (ReflectUtil.DoesExtend(type, t)) {
					dynamic result = null;
					initializedServices.TryRemove(t, out result);
					if (result != null) {
						retVal.Add((T) result);
					}
					services.RemoveAt(i);
				}
			}

			return retVal;
		}
		public static bool HasService(Type type) {
			if (type == null) {
				return false;
			}
			return services.Contains(type);
		}
		public static bool ServiceIsInitialized(Type type) {
			if (type == null) {
				return false;
			}
			return initializedServices.ContainsKey(type);
		}

		//private
		private static dynamic initializeService(Type service) {
			dynamic instance = null;

			try {
				instance = Activator.CreateInstance(service);
			} catch (Exception ex) {
				throw new Exception("Service cannot be initialized.", ex);
			}

			return instance;
		}
	}
}
