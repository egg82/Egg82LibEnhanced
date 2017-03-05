using Egg82LibEnhanced.Events;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace Egg82LibEnhanced.Utils {
	public class PreciseTimer {
		//externs
		[DllImport("kernel32.dll")]
		private static extern int GetCurrentThreadId();

		//vars
		public event EventHandler<PreciseElapsedEventArgs> Elapsed = null;
		public volatile bool AutoReset = false;
		
		private double _interval = 0.0d;
		private volatile bool _running = false;
		private int processors = Environment.ProcessorCount;
		private volatile int _processorNumber = 0;

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
		public int ProcessorNumber {
			get {
				return _processorNumber;
			}
			set {
				if (value < 0) {
					value = 0;
				}
				if (value > processors) {
					value = processors;
				}
				_processorNumber = value;
			}
		}
		
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
				}
				if (value == 0.0d) {
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
			timerThread = new Thread(delegate() {
				Stopwatch watch = new Stopwatch();
				ProcessThread currentThread = getCurrentThread();
				Thread.BeginThreadAffinity();

				int oldAffinity = _processorNumber;
				if (_processorNumber != 0) {
					currentThread.ProcessorAffinity = new IntPtr(1 << _processorNumber - 1);
				}

				watch.Start();
				double lastTime = 0.0d;
				double dt = 0.0d;
				/*
				 * 0.002 ms is about the time it takes
				 * to do the below operations on a reasonable
				 * CPU. Need to take that into account!
				 */
				double interval = _interval - 0.002d;
				do {
					if (oldAffinity != _processorNumber) {
						currentThread.ProcessorAffinity = new IntPtr((_processorNumber != 0) ? 1 << _processorNumber - 1 : 0xFFFF);
						oldAffinity = _processorNumber;
					}
					
					//dt is the last frame's Delta Time. Playing catch-up.
					//while ((dt - interval) + (watch.Elapsed.TotalMilliseconds - lastTime) < interval) {
					while(dt + watch.Elapsed.TotalMilliseconds - lastTime - interval < interval) {
						if (processors > 1) {
							Thread.SpinWait(1000);
						} else {
							Thread.Sleep(1);
						}
					}
					double ms = watch.Elapsed.TotalMilliseconds;
					dt = ms - lastTime;
					lastTime = ms;

					Elapsed?.Invoke(this, new PreciseElapsedEventArgs(ms, dt));
				} while (_running && AutoReset);
				watch.Stop();

				if (oldAffinity != 0) {
					currentThread.ProcessorAffinity = new IntPtr(0xFFFF);
				}

				Thread.EndThreadAffinity();
				_running = false;
			});
			timerThread.Priority = ThreadPriority.AboveNormal;
		}

		private ProcessThread getCurrentThread() {
			int id = GetCurrentThreadId();
			return (from ProcessThread thread in Process.GetCurrentProcess().Threads where thread.Id == id select thread).Single();
		}
	}
}
