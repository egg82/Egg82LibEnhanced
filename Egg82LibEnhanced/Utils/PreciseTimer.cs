using Egg82LibEnhanced.Events;
using System;
using System.Diagnostics;
using System.Threading;

namespace Egg82LibEnhanced.Utils {
	public class PreciseTimer {
		//vars
		public event EventHandler<PreciseElapsedEventArgs> Elapsed = null;
		public bool AutoReset = false;

		private double _interval = 0.0d;
		private volatile bool _running = false;
		private int processors = Environment.ProcessorCount;

		private Thread timerThread = null;

		//constructor
		public PreciseTimer(double interval, bool autoStart = false, bool autoReset = false) {
			if (interval < 0.0d) {
				interval = 0.0d;
			}
			_interval = interval;

			AutoReset = autoReset;

			setThread();
			if (autoStart) {
				_running = true;
				timerThread.Start();
			}
		}
		~PreciseTimer() {
			Stop();
			timerThread = null;
		}

		//public
		public void Start() {
			if (_running || _interval == 0.0d) {
				return;
			}
			_running = true;

			try {
				timerThread.Abort();
				setThread();
				timerThread.Start();
			} catch (Exception) {

			}
		}
		public void Stop() {
			_running = false;
		}

		public double Interval {
			get {
				return _interval;
			}
			set {
				if (value < 0.0d) {
					value = 0.0d;
					Stop();
				}
				_interval = value;
			}
		}
		public bool Running {
			get {
				return _running;
			}
		}

		//private
		private void setThread() {
			timerThread = new Thread(delegate () {
				Stopwatch watch = new Stopwatch();

				watch.Start();
				double lastTime = 0.0d;
				if (processors <= 1) {
					do {
						while (lastTime + watch.Elapsed.TotalMilliseconds < _interval) {
							Thread.Sleep(1);
						}
						double ms = watch.Elapsed.TotalMilliseconds;
						double dt = ms - lastTime;
						lastTime = ms;
						if (Elapsed != null) {
							Elapsed.Invoke(this, new PreciseElapsedEventArgs(ms, dt));
						}
					} while (_running && AutoReset);
				} else {
					do {
						while (lastTime + watch.Elapsed.TotalMilliseconds < _interval) {
							Thread.SpinWait(1000);
						}
						double ms = watch.Elapsed.TotalMilliseconds;
						double dt = ms - lastTime;
						lastTime = ms;
						if (Elapsed != null) {
							Elapsed.Invoke(this, new PreciseElapsedEventArgs(ms, dt));
						}
					} while (_running && AutoReset);
				}
				watch.Stop();

				_running = false;
			});
			timerThread.Priority = ThreadPriority.AboveNormal;
		}
	}
}
