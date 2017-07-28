﻿using System;
using System.ComponentModel;
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
			if (classToTest == null || baseClass == null) {
				return false;
			}

			return classToTest == baseClass || classToTest.IsSubclassOf(baseClass) || baseClass.IsAssignableFrom(classToTest);
		}

		public static T TryConvert<T>(dynamic input) {
			TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
			if (converter != null) {
				return converter.ConvertFrom(input);
			}
			return default(T);
		}
		public static dynamic TryConvert(Type type, dynamic input) {
			TypeConverter converter = TypeDescriptor.GetConverter(type);
			if (converter != null) {
				return converter.ConvertFrom(input);
			}
			return null;
		}

		public static Func<T> FunctionFromPropertyGetter<T>(object obj, string propertyName) {
			return (Func<T>) Delegate.CreateDelegate(typeof(Func<T>), obj, obj.GetType().GetProperty(propertyName).GetGetMethod());
		}
		public static Action<T> ActionFromPropertySetter<T>(object obj, string propertyName) {
			return (Action<T>) Delegate.CreateDelegate(typeof(Action<T>), obj, obj.GetType().GetProperty(propertyName).GetSetMethod());
		}

		//private

	}
}
