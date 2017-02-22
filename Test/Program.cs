﻿using Egg82LibEnhanced.Base;
using Egg82LibEnhanced.Startup;
using SFML.Window;
using System;
using Test.States;

namespace Test {
	class Program {
		private static BaseWindow graphicsWindow = null;
		private static BaseWindow physicsWindow = null;
		private static BaseWindow inputWindow = null;
		private static BaseWindow clientServerWindow = null;
		private static BaseWindow audioWindow = null;
		private static BaseWindow cryptoWindow = null;

		static void Main(string[] args) {
			Start.ProvideDefaultServices(true);

			graphicsWindow = new BaseWindow(1280, 720, "Graphics Test", Styles.Titlebar | Styles.Close, false, 16);
			graphicsWindow.AddState(new GraphicsTestState());
			
			/*physicsWindow = new BaseWindow(1280, 720, "Physics Test", Styles.Titlebar | Styles.Close, true, 16);
			physicsWindow.AddState(new PhysicsTestState());*/

			/*inputWindow = new BaseWindow(1280, 720, "Input Test", Styles.Titlebar | Styles.Close, true, 16);
			inputWindow.AddState(new InputTestState());*/

			/*clientServerWindow = new BaseWindow(1280, 720, "Client/Server Test", Styles.Titlebar | Styles.Close, true, 16);
			clientServerWindow.AddState(new ClientServerTestState());*/

			/*audioWindow = new BaseWindow(1280, 720, "Audio Test", Styles.Titlebar | Styles.Close, true, 16);
			audioWindow.AddState(new AudioTestState());*/

			/*cryptoWindow = new BaseWindow(1280, 720, "Crypto Test", Styles.Titlebar | Styles.Close, true, 16);
			cryptoWindow.AddState(new CryptoTestState());*/

			do {
				Start.UpdateEvents();
			} while (Start.NumWindowsOpen > 0);

			Start.DestroyDefaultServices();
			Environment.Exit(0);
		}
	}
}
