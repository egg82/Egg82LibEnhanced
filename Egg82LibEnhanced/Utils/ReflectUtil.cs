using System;
using System.Reflection;

namespace Egg82LibEnhanced.Utils {
	public class ReflectUtil {
		//vars

		//constructor
		public ReflectUtil() {

		}

		//public
		public static void InvokeMethod(string name, object c, params object[] parameters) {
			try {
				MethodInfo method = c.GetType().GetMethod(name);
				method.Invoke(c, parameters);
			} catch (Exception) {

			}
		}

		public static bool DoesExtend(Type baseClass, Type classToTest) {
			return classToTest == baseClass || classToTest.IsSubclassOf(baseClass) || classToTest.IsAssignableFrom(baseClass);
		}

		//private

	}
}
