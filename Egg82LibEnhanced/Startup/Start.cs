using Egg82LibEnhanced.Base;
using Egg82LibEnhanced.Engines;
using Egg82LibEnhanced.Engines.Nulls;
using Egg82LibEnhanced.Patterns;
using Egg82LibEnhanced.Utils;
using System;
using System.Collections.Generic;

namespace Egg82LibEnhanced.Startup {
	public class Start {
		//vars
		private static List<BaseWindow> windows = new List<BaseWindow>();
		private static int _numWindowsOpen = 0;

		//constructor
		public Start() {

		}

		//public
		public static void ProvideDefaultServices(bool physics = false) {
			ServiceLocator.ProvideService(typeof(AudioEngine));
			ServiceLocator.ProvideService(typeof(ModEngine));
			ServiceLocator.ProvideService(typeof(CryptoUtil));

			if (physics) {
				ServiceLocator.ProvideService(typeof(PhysicsEngine), false);
			} else {
				ServiceLocator.ProvideService(typeof(NullPhysicsEngine), false);
			}
			ServiceLocator.ProvideService(typeof(InputEngine), false);
			ServiceLocator.ProvideService(typeof(GameEngine), false);
		}
		public static void ProvideModServices() {
			ServiceLocator.ProvideService(typeof(AudioEngine));
			ServiceLocator.ProvideService(typeof(CryptoUtil));
		}
		public static void DestroyModServices() {
			ServiceLocator.RemoveService(typeof(IAudioEngine));
			ServiceLocator.RemoveService(typeof(ICryptoUtil));

			GC.Collect();
		}
		public static void DestroyDefaultServices() {
			ServiceLocator.RemoveService(typeof(IGameEngine));
			ServiceLocator.RemoveService(typeof(IInputEngine));
			ServiceLocator.RemoveService(typeof(IPhysicsEngine));

			ServiceLocator.RemoveService(typeof(IAudioEngine));
			ServiceLocator.RemoveService(typeof(IModEngine));
			ServiceLocator.RemoveService(typeof(ICryptoUtil));

			GC.Collect();
		}

		public static void UpdateEvents() {
			_numWindowsOpen = 0;
			for (int i = windows.Count - 1; i >= 0; i--) {
				if (windows[i].NeedsRepacement) {
					BaseWindow replacement = windows[i].GetReplacement();
					windows[i].Close();
					windows[i] = replacement;
				}
				windows[i].DispatchEvents();
				if (windows[i].IsOpen) {
					_numWindowsOpen++;
				} else {
					windows.RemoveAt(i);
				}
			}
		}
		public static int NumWindowsOpen {
			get {
				return _numWindowsOpen;
			}
		}

		//private
		internal static void AddWindow(BaseWindow window) {
			if (windows.Contains(window)) {
				return;
			}
			windows.Add(window);
		}
		internal static void RemoveWindow(BaseWindow window) {
			int index = windows.IndexOf(window);
			if (index == -1) {
				return;
			}
			windows.RemoveAt(index);
		}
	}
}
