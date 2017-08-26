using Egg82LibEnhanced.API.GameAnalytics;
using Egg82LibEnhanced.Display;
using Egg82LibEnhanced.Engines;
using Egg82LibEnhanced.Enums;
using Egg82LibEnhanced.Patterns;
using Egg82LibEnhanced.Reflection.ExceptionHandlers;
using Egg82LibEnhanced.Reflection.ExceptionHandlers.Builders;
using Egg82LibEnhanced.Startup;
using Egg82LibEnhanced.Utils;
using System;
using System.Timers;

namespace Test.States {
	public class InitState : Init {
		//vars
#pragma warning disable 0414
		private Window graphicsWindow = null;
		private Window physicsWindow = null;
		private Window inputWindow = null;
		private Window clientServerWindow = null;
		private Window audioWindow = null;
		private Window cryptoWindow = null;
		private Window gameDoorsWindow = null;
#pragma warning restore 0414

		private IGameAnalyticsAPI api = null;

		private IExceptionHandler exceptionHandler = null;
		private Timer exceptionHandlerTimer = new Timer(60.0d * 60.0d * 1000.0d);

		//constructor
		public InitState() {
			
		}

		//public

		//private
		protected override void OnEnter() {
			//provideExceptionHandler();
			//provideGameAnalytics();

			//throw new Exception("Test 1.0.0.0");

			IGameEngine gameEngine = ServiceLocator.GetService<IGameEngine>();
			//gameEngine.DrawSync = false;
			//gameEngine.UpdateInterval = (1.0d / 65.0d) / 1000.0d;
			//gameEngine.DrawInterval = (1.0d / 65.0d) / 1000.0d;

			/*graphicsWindow = new Window(1280, 720, "Graphics Test", WindowStyle.Titlebar | WindowStyle.Close | WindowStyle.Resize, false, true, 16);
			graphicsWindow.AddState(new GraphicsTestState());*/

			/*physicsWindow = new Window(1280, 720, "Physics Test", WindowStyle.Titlebar | WindowStyle.Close, true, true, 16);
			physicsWindow.AddState(new PhysicsTestState());*/

			/*inputWindow = new Window(1280, 720, "Input Test", WindowStyle.Titlebar | WindowStyle.Close, true, true, 16);
			inputWindow.AddState(new InputTestState());*/

			/*clientServerWindow = new Window(1280, 720, "Client/Server Test", WindowStyle.Titlebar | WindowStyle.Close, true, true, 16);
			clientServerWindow.AddState(new ClientServerTestState());*/

			/*audioWindow = new Window(1280, 720, "Audio Test", WindowStyle.Titlebar | WindowStyle.Close, true, true, 16);
			audioWindow.AddState(new AudioTestState());*/

			/*cryptoWindow = new Window(1280, 720, "Crypto Test", WindowStyle.Titlebar | WindowStyle.Close, true, true, 16);
			cryptoWindow.AddState(new CryptoTestState());*/

			string pixelFontPath = FileUtil.CURRENT_DIRECTORY + FileUtil.DIRECTORY_SEPARATOR_CHAR + ".." + FileUtil.DIRECTORY_SEPARATOR_CHAR + ".." + FileUtil.DIRECTORY_SEPARATOR_CHAR + "Assets" + FileUtil.DIRECTORY_SEPARATOR_CHAR + "Fonts" + FileUtil.DIRECTORY_SEPARATOR_CHAR + "ARCADECLASSIC.TTF";
			FontUtil.AddFont("pixel", FileUtil.Read(pixelFontPath, 0));

			gameDoorsWindow = new Window(1280, 720, "Baking For Doors", WindowStyle.Titlebar | WindowStyle.Close);
			gameDoorsWindow.AddState(new GameDoorInitState());
		}
		protected override void OnExit() {
			if (api != null && api.IsInitialized()) {
				api.SendUserSessionEnd();
			}
		}

		private void provideExceptionHandler() {
			IExceptionHandler oldExceptionHandler = ServiceLocator.GetService<IExceptionHandler>();
			ServiceLocator.RemoveServices<IExceptionHandler>();

			ServiceLocator.ProvideService(typeof(RollbarExceptionHandler), false);
			exceptionHandler = ServiceLocator.GetService<IExceptionHandler>();
			exceptionHandler.Connect(new RollbarBuilder("d29c2cb0a2cd43528c513509380115a0", "production"));
			exceptionHandler.SetUnsentExceptions(oldExceptionHandler.GetUnsentExceptions());
			exceptionHandler.SetDomains(oldExceptionHandler.GetDomains());
			oldExceptionHandler.Disconnect();

			checkExceptionLimitReached();
			exceptionHandlerTimer.Elapsed += checkExceptionLimitReached;
			exceptionHandlerTimer.AutoReset = true;
			exceptionHandlerTimer.Start();
		}
		private void provideGameAnalytics() {
			ServiceLocator.ProvideService(typeof(GameAnalyticsAPI), false);
			api = ServiceLocator.GetService<IGameAnalyticsAPI>();
			api.SendInit("698deede3c3e55cfa6a4e242b21ed0c6", "e894c3b2524f011a3aa8e53f88a86659356406f3", 1);
			api.SendUserSessionStart();
		}

		private void checkExceptionLimitReached(object sender = null, ElapsedEventArgs e = null) {
			if (exceptionHandler.IsLimitReached()) {
				IExceptionHandler oldExceptionHandler = ServiceLocator.GetService<IExceptionHandler>();
				ServiceLocator.RemoveServices<IExceptionHandler>();

				ServiceLocator.ProvideService(typeof(GameAnalyticsExceptionHandler), false);
				exceptionHandler = ServiceLocator.GetService<IExceptionHandler>();
				exceptionHandler.Connect(new GameAnalyticsBuilder("698deede3c3e55cfa6a4e242b21ed0c6", "e894c3b2524f011a3aa8e53f88a86659356406f3"));
				exceptionHandler.SetUnsentExceptions(oldExceptionHandler.GetUnsentExceptions());
				exceptionHandler.SetDomains(oldExceptionHandler.GetDomains());
				oldExceptionHandler.Disconnect();
			}
		}
	}
}
