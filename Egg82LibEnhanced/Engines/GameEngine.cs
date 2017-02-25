using Egg82LibEnhanced.Base;
using Egg82LibEnhanced.Events;
using Egg82LibEnhanced.Patterns;
using Egg82LibEnhanced.Utils;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Egg82LibEnhanced.Engines {
	public class GameEngine : IGameEngine {
		//externs
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern ExecutionState SetThreadExecutionState(ExecutionState esFlags);
		
		//enums
		[Flags]
		private enum ExecutionState : uint {
			ES_SYSTEM_REQUIRED = 0x00000001,
			ES_DISPLAY_REQUIRED = 0x00000002,
			//ES_USER_PRESENT = 0x00000004,
			ES_AWAYMODE_REQUIRED = 0x00000040,
			ES_CONTINUOUS = 0x80000000
		}
		
		//vars
		private List<BaseWindow> windows = new List<BaseWindow>();
		private PreciseTimer updateTimer = new PreciseTimer((1.0d / 60.0d) * 1000.0d);
		private PreciseTimer inputTimer = new PreciseTimer((1.0d / 120.0d) * 1000.0d);
		private PreciseTimer physicsTimer = new PreciseTimer((1.0d / 60.0d) * 1000.0d);
		private PreciseTimer drawTimer = new PreciseTimer((1.0d / 60.0d) * 1000.0d);
		private double _targetUpdateInterval = (1.0d / 60.0d) * 1000.0d;
		private volatile bool _drawSync = true;
		private object drawLock = new object();

		private IInputEngine inputEngine = (IInputEngine) ServiceLocator.GetService(typeof(IInputEngine));
		private IPhysicsEngine physicsEngine = (IPhysicsEngine) ServiceLocator.GetService(typeof(IPhysicsEngine));

		//constructor
		public GameEngine() {
			SetThreadExecutionState(ExecutionState.ES_CONTINUOUS | ExecutionState.ES_DISPLAY_REQUIRED | ExecutionState.ES_SYSTEM_REQUIRED);

			updateTimer.Elapsed += onUpdateTimer;
			updateTimer.AutoReset = true;
			inputTimer.Elapsed += onInputTimer;
			inputTimer.AutoReset = true;
			physicsTimer.Elapsed += onPhysicsTimer;
			physicsTimer.AutoReset = true;
			drawTimer.Elapsed += onDrawTimer;
			drawTimer.AutoReset = true;

			int processors = Environment.ProcessorCount;
			if (processors >= 4) {
				drawTimer.ProcessorNumber = processors - 3;
				updateTimer.ProcessorNumber = processors - 2;
				inputTimer.ProcessorNumber = processors - 1;
				physicsTimer.ProcessorNumber = processors;
			} else if (processors >= 2) {
				drawTimer.ProcessorNumber = 1;
				updateTimer.ProcessorNumber = 1;
				inputTimer.ProcessorNumber = 2;
				physicsTimer.ProcessorNumber = 2;
			}

			updateTimer.Start();
			inputTimer.Start();
			physicsTimer.Start();
			drawTimer.Start();
		}
		~GameEngine() {
			drawTimer.Stop();
			physicsTimer.Stop();
			inputTimer.Stop();
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
					drawTimer.Interval = checkDrawInterval(drawTimer.Interval);
				}
			}
		}

		public double UpdateInterval {
			get {
				return updateTimer.Interval;
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}
				if (value < 0.001d) {
					value = 0.001d;
				}
				updateTimer.Interval = value;
				inputTimer.Interval = (value < 0.002d) ? 0.001d : value / 2.0d;
				physicsTimer.Interval = value;

				if (_drawSync) {
					drawTimer.Interval = checkDrawInterval(drawTimer.Interval);
				}
			}
		}
		public double TargetUpdateInterval {
			get {
				return _targetUpdateInterval;
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					return;
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
					return;
				}
				if (value < 0.001d) {
					value = 0.001d;
				}

				drawTimer.Interval = (_drawSync) ? checkDrawInterval(value) : value;
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
		private void onInputTimer(object sender, PreciseElapsedEventArgs e) {
			inputEngine.Update();
		}
		private void onPhysicsTimer(object sender, PreciseElapsedEventArgs e) {
			physicsEngine.Update(e.DeltaTime * 0.001d);
		}
		private void update(PreciseElapsedEventArgs e) {
			double deltaTime = e.DeltaTime / _targetUpdateInterval;
			
			Tween.Update(e.DeltaTime);

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

		private double checkDrawInterval(double value) {
			if (updateTimer.Interval % value != 0.0d) {
				value = Math.Round((value / updateTimer.Interval), MidpointRounding.AwayFromZero) * updateTimer.Interval;
			}

			if (value < 0.001d) {
				value = 0.001d;
			}

			return value;
		}
	}
}
