using Egg82LibEnhanced.Enums;
using System;
using System.Collections.Generic;

namespace Egg82LibEnhanced.Tweening {
	public class Tween {
		//vars
		private static SynchronizedCollection<Tween> tweens = new SynchronizedCollection<Tween>();

		/// <summary>
		/// Called when the tween has finished updating.
		/// </summary>
		public event EventHandler Complete = null;

		private Action<double> onUpdate = null;
		private double _start = 0.0d;
		private double _end = 0.0d;
		private double _duration = 0.0d;
		private EasingType _type = EasingType.None;
		private double totalMillis = 0.0d;

		//constructor
		/// <summary>
		/// An object used to smoothly translate a field from start to finish with various types of easing. The tween is automatically started upon creation.
		/// </summary>
		/// <param name="onUpdate">The method that is called with the new value every time the tween is updated.</param>
		/// <param name="start">The starting value of the tween.</param>
		/// <param name="end">The ending value of the tween.</param>
		/// <param name="duration">The duration, in milliseconds, of the tween.</param>
		/// <param name="type">(optional) The type of easing the tween will use.</param>
		public Tween(Action<double> onUpdate, double start, double end, double duration, EasingType type = EasingType.Linear) {
			if (onUpdate == null) {
				throw new ArgumentNullException("setFunc");
			}
			if (double.IsNaN(start) || double.IsInfinity(start)) {
				throw new InvalidOperationException("start cannot be NaN or infinity.");
			}
			if (double.IsNaN(end) || double.IsInfinity(end)) {
				throw new InvalidOperationException("end cannot be NaN or infinity.");
			}
			if (double.IsNaN(duration) || double.IsInfinity(duration)) {
				throw new InvalidOperationException("duration cannot be NaN or infinity.");
			}

			if (type == EasingType.None) {
				type = EasingType.Linear;
			}
			
			tweens.Add(this);

			this.onUpdate = onUpdate;
			_start = start;
			_end = end;
			_duration = duration;
			_type = type;
		}
		~Tween() {
			bool running = IsRunning;
			tweens.Remove(this);

			if (running) {
				Complete?.Invoke(this, EventArgs.Empty);
			}
		}

		//public
		/// <summary>
		/// Creates a tween that sets a method. It starts at the specified value and ends at the specified value.
		/// </summary>
		/// <param name="onUpdate">The method that is called with the new value every time the tween is updated.</param>
		/// <param name="start">The starting value.</param>
		/// <param name="end">The ending value.</param>
		/// <param name="duration">The duration, in milliseconds, of the tween.</param>
		/// <param name="type">(optional) The type of easing the tween will use.</param>
		/// <returns>The tween that was created.</returns>
		public static Tween FromTo(Action<double> onUpdate, double start, double end, double duration, EasingType type = EasingType.Linear) {
			return new Tween(onUpdate, start, end, duration, type);
		}
		
		/// <summary>
		/// Whether or not the tween is currently running.
		/// </summary>
		public bool IsRunning {
			get {
				return tweens.Contains(this);
			}
		}

		/// <summary>
		/// The starting value of the tween. Read-only.
		/// </summary>
		public double StartValue {
			get {
				return _start;
			}
		}
		/// <summary>
		/// The ending value of the tween. Read-only.
		/// </summary>
		public double EndValue {
			get {
				return _end;
			}
		}
		/// <summary>
		/// The duration, in milliseconds, of the tween. Read-only.
		/// </summary>
		public double Duration {
			get {
				return _duration;
			}
		}
		/// <summary>
		/// The easing type of the tween. Read-only.
		/// </summary>
		public EasingType Type {
			get {
				return _type;
			}
		}

		/// <summary>
		/// Pauses the tween, saving its current position.
		/// </summary>
		public void Pause() {
			tweens.Remove(this);
		}
		/// <summary>
		/// Stops and resets the tween.
		/// </summary>
		public void Stop() {
			tweens.Remove(this);
			totalMillis = 0.0d;
			Complete?.Invoke(this, EventArgs.Empty);
		}
		/// <summary>
		/// Starts/Resumes the tween.
		/// </summary>
		public void Start() {
			tweens.Add(this);
		}

		//private
		internal static void Update(double deltaTime) {
			if (tweens.Count == 0) {
				return;
			}
			for (int i = 0; i < tweens.Count; i++) {
				tweens[i].update(deltaTime);
			}
		}
		private void update(double deltaTime) {
			totalMillis += deltaTime;
			if (totalMillis < _duration) {
				onUpdate.Invoke(Easing.Ease(_start, _end, _duration, totalMillis, _type));
			} else {
				onUpdate.Invoke(Easing.Ease(_start, _end, _duration, _duration, _type));
				Stop();
			}
		}
	}
}
