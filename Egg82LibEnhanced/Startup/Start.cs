using Egg82LibEnhanced.Crypto;
using Egg82LibEnhanced.Display;
using Egg82LibEnhanced.Engines;
using Egg82LibEnhanced.Engines.Nulls;
using Egg82LibEnhanced.Patterns;
using Egg82LibEnhanced.Patterns.Prototypes;
using Egg82LibEnhanced.Reflection.ExceptionHandlers;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Egg82LibEnhanced.Startup {
	internal class Start {
		//vars
		private static SynchronizedCollection<Window> windows = new SynchronizedCollection<Window>();
		private static int _numWindowsOpen = 0;

		//constructor
		public Start() {

		}

		//public
		public static void ProvideDefaultServices() {
			ServiceLocator.ProvideService(typeof(NullExceptionHandler));
			ServiceLocator.ProvideService(typeof(PrototypeFactory));
			ServiceLocator.ProvideService(typeof(AudioEngine));
			ServiceLocator.ProvideService(typeof(ModEngine));
			ServiceLocator.ProvideService(typeof(CryptoHelper));
			ServiceLocator.ProvideService(typeof(PhysicsEngine), false);
			ServiceLocator.ProvideService(typeof(InputEngine), false);
			ServiceLocator.ProvideService(typeof(GameEngine), false);
		}
		public static void ProvideModServices() {
			ServiceLocator.ProvideService(typeof(NullExceptionHandler));
			ServiceLocator.ProvideService(typeof(PrototypeFactory));
			ServiceLocator.ProvideService(typeof(AudioEngine));
			ServiceLocator.ProvideService(typeof(CryptoHelper));
		}
		public static void DestroyModServices() {
			ServiceLocator.RemoveServices<PrototypeFactory>();
			ServiceLocator.RemoveServices<IAudioEngine>();
			ServiceLocator.RemoveServices<ICryptoHelper>();
			ServiceLocator.RemoveServices<IExceptionHandler>();

			GC.Collect();
		}
		public static void DestroyDefaultServices() {
			ServiceLocator.RemoveServices<IGameEngine>();
			ServiceLocator.RemoveServices<IInputEngine>();
			ServiceLocator.RemoveServices<IPhysicsEngine>();

			ServiceLocator.RemoveServices<IAudioEngine>();
			ServiceLocator.RemoveServices<ICryptoHelper>();
			ServiceLocator.RemoveServices<ICryptoHelper>();
			ServiceLocator.RemoveServices<PrototypeFactory>();
			ServiceLocator.RemoveServices<IExceptionHandler>();

			GC.Collect();
		}

		public static void UpdateEvents() {
			_numWindowsOpen = 0;
			for (int i = windows.Count - 1; i >= 0; i--) {
				if (windows[i].NeedsRepacement) {
					Window replacement = windows[i].GetReplacement();
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
			Thread.Yield();
		}
		public static int NumWindowsOpen {
			get {
				return _numWindowsOpen;
			}
		}

		//private
		internal static void AddWindow(Window window) {
			if (windows.Contains(window)) {
				return;
			}
			windows.Add(window);
		}
		internal static void RemoveWindow(Window window) {
			int index = windows.IndexOf(window);
			if (index == -1) {
				return;
			}
			windows.RemoveAt(index);
		}
	}
}
