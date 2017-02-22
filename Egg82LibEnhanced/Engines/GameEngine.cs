using Egg82LibEnhanced.Base;
using Egg82LibEnhanced.Events;
using Egg82LibEnhanced.Patterns;
using Egg82LibEnhanced.Utils;
using System;
using System.Collections.Generic;

namespace Egg82LibEnhanced.Engines {
	public class GameEngine : IGameEngine {
		//vars
		private List<BaseWindow> windows = new List<BaseWindow>();
		private PreciseTimer updateTimer = new PreciseTimer((1.0d / 120.0d) * 1000.0d);
		private double _targetUpdateInterval = (1.0d / 60.0d) * 1000.0d;
		private PreciseTimer drawTimer = new PreciseTimer((1.0d / 60.0d) * 1000.0d);
		private bool _drawSync = true;
		private object drawLock = new object();

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

		public bool DrawSync {
			get {
				return _drawSync;
			}
			set {
				_drawSync = value;
				if (_drawSync) {
					checkDrawInterval();
				}
			}
		}

		public double UpdateInterval {
			get {
				return updateTimer.Interval;
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					throw new InvalidOperationException("value cannot be NaN or infinity.");
				}
				if (value < 0.001d) {
					value = 0.001d;
				}
				updateTimer.Interval = value;

				if (_drawSync) {
					checkDrawInterval();
				}
			}
		}
		public double TargetUpdateInterval {
			get {
				return _targetUpdateInterval;
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					throw new InvalidOperationException("value cannot be NaN or infinity.");
				}
				if (value < 0.001d) {
					value = 0.001d;
				}
				_targetUpdateInterval = value;
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
					value = 0.001d;
				}
				drawTimer.Interval = value;

				if (_drawSync) {
					checkDrawInterval();
				}
			}
		}

		//private
		private void onUpdateTimer(object sender, PreciseElapsedEventArgs e) {
			if (DrawSync) {
				lock (drawLock) {
					update(e);
				}
			} else {
				update(e);
			}
		}
		private void update(PreciseElapsedEventArgs e) {
			double deltaTime = e.DeltaTime / _targetUpdateInterval;
			
			Tween.Update(e.DeltaTime);
			inputEngine.Update();
			physicsEngine.Update(e.DeltaTime * 0.001d);

			for (int i = 0; i < windows.Count; i++) {
				windows[i].Update(deltaTime);
			}
			for (int i = 0; i < windows.Count; i++) {
				windows[i].SwapBuffers();
			}
		}
		private void onDrawTimer(object sender, PreciseElapsedEventArgs e) {
			if (DrawSync) {
				lock (drawLock) {
					draw();
				}
			} else {
				draw();
			}
		}
		private void draw() {
			try {
				for (int i = 0; i < windows.Count; i++) {
					windows[i].Draw();
				}
			} catch (Exception) {
				
			}
		}

		private void checkDrawInterval() {
			if (updateTimer.Interval % drawTimer.Interval != 0.0d) {
				drawTimer.Interval = Math.Round((drawTimer.Interval / updateTimer.Interval), MidpointRounding.AwayFromZero) * updateTimer.Interval;
			}
		}
	}
}
