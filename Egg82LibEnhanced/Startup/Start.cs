﻿using Egg82LibEnhanced.Base;
using Egg82LibEnhanced.Engines;
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
		public static void ProvideDefaultServices() {
			ServiceLocator.ProvideService(typeof(AudioEngine));
			ServiceLocator.ProvideService(typeof(ModEngine));
			ServiceLocator.ProvideService(typeof(CryptoUtil));

			ServiceLocator.ProvideService(typeof(PhysicsEngine), false);
			ServiceLocator.ProvideService(typeof(InputEngine), false);
			ServiceLocator.ProvideService(typeof(GameEngine), false);
		}
		public static void DestroyServices() {
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
				windows[i].UpdateEvents();
				if (windows[i].IsOpen()) {
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
		internal static void addWindow(BaseWindow window) {
			if (windows.Contains(window)) {
				return;
			}
			windows.Add(window);
		}
		internal static void removeWindow(BaseWindow window) {
			int index = windows.IndexOf(window);
			if (index == -1) {
				return;
			}
			windows.RemoveAt(index);
		}
	}
}
