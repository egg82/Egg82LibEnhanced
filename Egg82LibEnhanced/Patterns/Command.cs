using Egg82LibEnhanced.Events;
using Egg82LibEnhanced.Utils;
using System;
using System.Threading;
using System.Timers;

namespace Egg82LibEnhanced.Patterns {
	public abstract class Command {
		//vars
		public event EventHandler Complete = null;
		public event EventHandler<ExceptionEventArgs> Error = null;
		public event EventHandler<ProgressEventArgs> Progress = null;
		
		private System.Timers.Timer timer = null;
		private PreciseTimer preciseTimer = null;
		private double startTime = 0.0d;

		//constructor
		public Command(double delay = 0.0d) {
			if (delay < 0.0d) {
				throw new InvalidOperationException("delay cannot be less than zero.");
			} else if (delay == 0.0d) {
				return;
			} else if (delay < 17.0d) {
				preciseTimer = new PreciseTimer(delay);
				preciseTimer.AutoReset = false;
				preciseTimer.Elapsed += onPreciseElapsed;
			} else {
				timer = new System.Timers.Timer(delay);
				timer.AutoReset = false;
				timer.Elapsed += onElapsed;
			}
		}

		//public
		public void Start() {
			if (timer != null) {
				startTime = MathUtil.TicksToMilliseconds();
				timer.Start();
			} else if (preciseTimer != null) {
				preciseTimer.Start();
			} else {
				new Thread(delegate() {
					onExecute(0.0d);
				}).Start();
			}
		}

		//private
		abstract protected void onExecute(double elapsedMilliseconds);

		private void onElapsed(object sender, ElapsedEventArgs e) {
			onExecute(MathUtil.TicksToMilliseconds(e.SignalTime.Ticks) - startTime);
		}
		private void onPreciseElapsed(object sender, PreciseElapsedEventArgs e) {
			onExecute(e.ElapsedMilliseconds);
		}
	}
}
