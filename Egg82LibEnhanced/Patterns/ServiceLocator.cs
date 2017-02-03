using Egg82LibEnhanced.Utils;
using System;
using System.Collections.Generic;

namespace Egg82LibEnhanced.Patterns {
	public class ServiceLocator {
		//vars
		private static List<Type> services = new List<Type>();
		private static Dictionary<Type, dynamic> initializedServices = new Dictionary<Type, dynamic>();

		//constructor
		public ServiceLocator() {

		}

		//public
		public static dynamic GetService(Type type) {
			if (type == null) {
				throw new ArgumentNullException("type");
			}

			dynamic result = null;
			int index = services.IndexOf(type);

			if (!initializedServices.TryGetValue(type, out result) && index > -1) {
				result = initializeService(services[index]);
				initializedServices.Add(type, result);
			}

			if (result == null) {
				for (int i = 0; i < services.Count; i++) {
					if (ReflectUtil.DoesExtend(services[i], type)) {
						if (!initializedServices.TryGetValue(services[i], out result)) {
							result = initializeService(services[i]);
							initializedServices.Add(type, result);
						}
						break;
					}
				}
			}

			return result;
		}
		public static void ProvideService(Type type, bool lazyInitialize = true) {
			if (type == null) {
				throw new ArgumentNullException("type");
			}

			dynamic result = null;

			if (initializedServices.TryGetValue(type, out result)) {
				if (ReflectUtil.DoesExtend(typeof(IDisposable), result)) {
					((IDisposable) result).Dispose();
				}
				initializedServices.Remove(type);
			}

			int index = services.IndexOf(type);
			if (index > -1) {
				services[index] = type;
			} else {
				services.Add(type);
			}

			if (!lazyInitialize) {
				initializedServices.Add(type, initializeService(type));
			}
		}
		public static void RemoveService(Type type) {
			if (type == null) {
				throw new ArgumentNullException("type");
			}

			dynamic result = null;
			if (initializedServices.TryGetValue(type, out result)) {
				if (ReflectUtil.DoesExtend(typeof(IDisposable), result.GetType())) {
					((IDisposable) result).Dispose();
				}
				initializedServices.Remove(type);
			}
			services.Remove(type);

			if (result == null) {
				for (int i = 0; i < services.Count; i++) {
					if (ReflectUtil.DoesExtend(services[i], type)) {
						if (initializedServices.TryGetValue(services[i], out result)) {
							if (ReflectUtil.DoesExtend(typeof(IDisposable), result.GetType())) {
								((IDisposable) result).Dispose();
							}
							initializedServices.Remove(services[i]);
						}
						services.RemoveAt(i);
						return;
					}
				}
			}
		}
		public static bool HasService(Type type) {
			if (type == null) {
				throw new ArgumentNullException("type");
			}
			return services.Contains(type);
		}
		public static bool ServiceIsInitialized(Type type) {
			if (type == null) {
				throw new ArgumentNullException("type");
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
