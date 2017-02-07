﻿using Egg82LibEnhanced.Base;
using Egg82LibEnhanced.Events;
using Egg82LibEnhanced.Patterns;
using Egg82LibEnhanced.Utils;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Egg82LibEnhanced.Engines {
	public class GameEngine : IGameEngine {
		//vars
		private List<BaseWindow> windows = new List<BaseWindow>();
		private PreciseTimer updateTimer = new PreciseTimer((1.0d / 120.0d) * 1000.0d);
		private double targetUpdateInterval = (1.0d / 60.0d) * 1000.0d;
		private PreciseTimer drawTimer = new PreciseTimer((1.0d / 60.0d) * 1000.0d);

		private IInputEngine inputEngine = (IInputEngine) ServiceLocator.GetService(typeof(IInputEngine));
		private IPhysicsEngine physicsEngine = (IPhysicsEngine) ServiceLocator.GetService(typeof(IPhysicsEngine));

		//constructor
		public GameEngine() {
			updateTimer.Elapsed += onUpdateTimer;
			updateTimer.AutoReset = true;

			drawTimer.Elapsed += onDrawTimer;
			drawTimer.AutoReset = true;

			updateTimer.Start();
			drawTimer.Start();
		}
		~GameEngine() {
			drawTimer.Stop();
			updateTimer.Stop();
		}

		//public
		public void AddWindow(BaseWindow window) {
			if (window == null) {
				throw new ArgumentNullException("window");
			}
			if (windows.Contains(window)) {
				return;
			}

			windows.Add(window);
		}
		public void RemoveWindow(BaseWindow window) {
			if (window == null) {
				throw new ArgumentNullException("window");
			}
			int index = windows.IndexOf(window);
			if (index == -1) {
				return;
			}

			windows.RemoveAt(index);
		}
		public BaseWindow GetWindowAt(int index) {
			if (index < 0 || index >= windows.Count) {
				return null;
			}
			return windows[index];
		}
		public int IndexOf(BaseWindow window) {
			if (window == null) {
				throw new ArgumentNullException("window");
			}

			return windows.IndexOf(window);
		}
		public int NumWindows {
			get {
				return windows.Count;
			}
		}

		public double UpdateInterval {
			get {
				return targetUpdateInterval;
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					throw new InvalidOperationException("value cannot be NaN or infinity.");
				}
				if (value < 0.002d) {
					targetUpdateInterval = 0.002d;
				} else {
					targetUpdateInterval = value;
				}
				updateTimer.Interval = targetUpdateInterval / 2.0d;
				//checkDrawInterval();
			}
		}
		public double DrawInterval {
			get {
				return drawTimer.Interval;
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					throw new InvalidOperationException("value cannot be NaN or infinity.");
				}
				if (value < 0.001d) {
					drawTimer.Interval = 0.001d;
				} else {
					drawTimer.Interval = value;
				}
				//checkDrawInterval();
			}
		}

		//private
		private void onUpdateTimer(object sender, PreciseElapsedEventArgs e) {
			double deltaTime = e.DeltaTime / targetUpdateInterval;
			
			inputEngine.Update();
			physicsEngine.Update(deltaTime * 0.001d);

			for (int i = 0; i < windows.Count; i++) {
				windows[i].Update(deltaTime);
			}
			for (int i = 0; i < windows.Count; i++) {
				windows[i].SwapBuffers();
			}
		}
		private void onDrawTimer(object sender, PreciseElapsedEventArgs e) {
			for (int i = 0; i < windows.Count; i++) {
				windows[i].Draw();
			}
		}

		private void checkDrawInterval() {
			if (targetUpdateInterval % drawTimer.Interval != 0.0d) {
				drawTimer.Interval = Math.Round((drawTimer.Interval / targetUpdateInterval), MidpointRounding.AwayFromZero) * targetUpdateInterval;
			}
		}
	}
}
