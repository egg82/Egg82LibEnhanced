using Egg82LibEnhanced.Engines;
using Egg82LibEnhanced.Patterns;
using Egg82LibEnhanced.Startup;
using SFML.Window;
using System;
using Test.States;

namespace Test {
	class Program {
		private static Egg82LibEnhanced.Display.Window graphicsWindow = null;
		private static Egg82LibEnhanced.Display.Window physicsWindow = null;
		private static Egg82LibEnhanced.Display.Window inputWindow = null;
		private static Egg82LibEnhanced.Display.Window clientServerWindow = null;
		private static Egg82LibEnhanced.Display.Window audioWindow = null;
		private static Egg82LibEnhanced.Display.Window cryptoWindow = null;

		static void Main(string[] args) {
			Start.ProvideDefaultServices(true);

			IGameEngine gameEngine = ServiceLocator.GetService<IGameEngine>();
			//gameEngine.DrawSync = false;
			//gameEngine.UpdateInterval = (1.0d / 65.0d) / 1000.0d;
			//gameEngine.DrawInterval = (1.0d / 65.0d) / 1000.0d;

			graphicsWindow = new Egg82LibEnhanced.Display.Window(1280, 720, "Graphics Test", Styles.Titlebar | Styles.Close | Styles.Resize, false, true, 16);
			graphicsWindow.AddState(new GraphicsTestState());

			/*physicsWindow = new Egg82LibEnhanced.Display.Window(1280, 720, "Physics Test", Styles.Titlebar | Styles.Close, true, true, 16);
			physicsWindow.AddState(new PhysicsTestState());

			inputWindow = new Egg82LibEnhanced.Display.Window(1280, 720, "Input Test", Styles.Titlebar | Styles.Close, true, true, 16);
			inputWindow.AddState(new InputTestState());*/

			/*clientServerWindow = new Egg82LibEnhanced.Display.Window(1280, 720, "Client/Server Test", Styles.Titlebar | Styles.Close, true, true, 16);
			clientServerWindow.AddState(new ClientServerTestState());*/

			/*audioWindow = new Egg82LibEnhanced.Display.Window(1280, 720, "Audio Test", Styles.Titlebar | Styles.Close, true, true, 16);
			audioWindow.AddState(new AudioTestState());*/

			/*cryptoWindow = new Egg82LibEnhanced.Display.Window(1280, 720, "Crypto Test", Styles.Titlebar | Styles.Close, true, true, 16);
			cryptoWindow.AddState(new CryptoTestState());*/

			do {
				Start.UpdateEvents();
			} while (Start.NumWindowsOpen > 0);

			Start.DestroyDefaultServices();
			Environment.Exit(0);
		}
	}
}
