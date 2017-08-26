using Egg82LibEnhanced.Events;
using Egg82LibEnhanced.Utils;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace Egg82LibEnhanced.Core {
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
		private double ticksPerMillisecond = TimeSpan.TicksPerMillisecond;

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
				if (_interval == value) {
					return;
				}
				if (value == 0.0d) {
					Stop();
				}

				_interval = value;
				Stop();
				Start();
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
				ProcessThread currentThread = getCurrentThread();
				Thread.BeginThreadAffinity();

				int oldAffinity = _processorNumber;
				if (_processorNumber != 0) {
					currentThread.ProcessorAffinity = new IntPtr(1 << _processorNumber - 1);
				}

				watch.Start();
				long lastTime = 0L;
				long dt = 0L;
				double target = _interval * ticksPerMillisecond;
				double interval = target;
				long frameAverage = 0L;
				int numFramesCalculated = 0;
				do {
					if (oldAffinity != _processorNumber) {
						currentThread.ProcessorAffinity = new IntPtr((_processorNumber != 0) ? 1 << _processorNumber - 1 : 0xFFFF);
						oldAffinity = _processorNumber;
					}

					// dt is the last frame's Delta Time. Playing short-term catch-up on the next frame.
					// We use Elapsed.Ticks as a high-performance replacement for Elapsed.TotalMilliseconds
					// Why not use ElapsedTicks? Because its output is orders of magnitude wrong. I have no idea why.
					long ms = 0L;
					//while ((dt - interval) + ((ms = watch.Elapsed.Ticks) - lastTime) < interval) {
					while (dt + (ms = watch.Elapsed.Ticks) - lastTime - interval < interval) { //apparently this is the reduced version of the above line
						if (processors > 1) {
							Thread.SpinWait(1); // This is most efficient
						} else {
							Thread.Yield(); // Next best thing to SpinWait so we don't start locking up our only CPU
						}
					}
					dt = ms - lastTime;
					lastTime = ms;

					if (numFramesCalculated >= 99) {
						frameAverage += dt;
						frameAverage = frameAverage / (numFramesCalculated + 1);
						interval += 0.08d * (target - frameAverage);
						interval = MathUtil.Clamp(1.0d, double.MaxValue, interval);
						frameAverage = 0L;
						numFramesCalculated = 0;
					} else {
						frameAverage += dt;
						numFramesCalculated++;
					}

					Elapsed?.Invoke(this, new PreciseElapsedEventArgs(ms / ticksPerMillisecond, dt / ticksPerMillisecond));
				} while (_running && AutoReset);
				watch.Stop();

				if (oldAffinity != 0) {
					currentThread.ProcessorAffinity = new IntPtr(0xFFFF);
				}

				Thread.EndThreadAffinity();
				_running = false;
			}) {
				Priority = ThreadPriority.AboveNormal
			};
		}

		private ProcessThread getCurrentThread() {
			int id = GetCurrentThreadId();
			return (from ProcessThread thread in Process.GetCurrentProcess().Threads where thread.Id == id select thread).Single();
		}
	}
}
