using Egg82LibEnhanced.Events;
using System;
using System.Diagnostics;
using System.Threading;

namespace Egg82LibEnhanced.Patterns {
	public abstract class SynchronousCommand {
		//vars
		public event EventHandler Complete = null;
		public event EventHandler<ExceptionEventArgs> Error = null;
		public event EventHandler<ProgressEventArgs> Progress = null;
		
		private int timer = 0;

		//constructor
		public SynchronousCommand(int delay = 0) {
			if (delay < 0) {
				throw new InvalidOperationException("delay cannot be less than zero.");
			} else if (delay == 0) {
				return;
			} else {
				timer = delay;
			}
		}

		//public
		public void Start() {
			if (timer != 0) {
				Stopwatch watch = new Stopwatch();
				watch.Start();
				try {
					Thread.Sleep(timer);
				} catch (Exception) {

				}
				watch.Stop();
				onExecute(watch.Elapsed.TotalMilliseconds);
			} else {
				onExecute(0.0d);
			}
		}

		//private
		abstract protected void onExecute(double elapsedMilliseconds);
	}
}
