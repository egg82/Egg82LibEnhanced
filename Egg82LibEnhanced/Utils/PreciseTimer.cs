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
		private bool _running = false;
		private int processors = Environment.ProcessorCount;

		private Thread timerThread = null;

		//constructor
		public PreciseTimer(double interval) {
			if (interval < 0.0d) {
				interval = 0.0d;
			}
			_interval = interval;

			timerThread = new Thread(delegate() {
				Stopwatch watch = new Stopwatch();

				watch.Start();
				double lastTime = 0.0d;
				double ms = 0.0d;
				if (processors <= 1) {
					do {
						while (lastTime + watch.Elapsed.TotalMilliseconds < _interval) {
							Thread.Sleep(1);
						}
						ms = watch.Elapsed.TotalMilliseconds;
						if (Elapsed != null) {
							Elapsed.Invoke(this, new PreciseElapsedEventArgs(ms, lastTime + ms));
						}
						lastTime = watch.Elapsed.TotalMilliseconds;
						watch.Restart();
					} while (_running && AutoReset);
				} else {
					do {
						while (lastTime + watch.Elapsed.TotalMilliseconds < _interval) {
							Thread.SpinWait(1000);
						}
						ms = watch.Elapsed.TotalMilliseconds;
						if (Elapsed != null) {
							Elapsed.Invoke(this, new PreciseElapsedEventArgs(ms, lastTime + ms));
						}
						lastTime = watch.Elapsed.TotalMilliseconds;
						watch.Restart();
					} while (_running && AutoReset);
				}
				watch.Stop();

				_running = false;
			});
			timerThread.Priority = ThreadPriority.AboveNormal;
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
			
			timerThread.Start();
			_running = true;
		}
		public void Stop() {
			if (!_running) {
				return;
			}
			
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

	}
}
