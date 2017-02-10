using Egg82LibEnhanced.Enums;
using Egg82LibEnhanced.Events;
using System;
using System.Collections.Generic;

namespace Egg82LibEnhanced.Utils {
	public class Tween {
		//vars
		private static List<Tween> tweens = new List<Tween>();
		private static object listLock = new object();
		private static PreciseTimer timer = timer = new PreciseTimer((1.0d / 60.0d) * 1000.0d, true, true);

		public event EventHandler Complete = null;

		private Action<double> setFunc = null;
		private double _start = 0.0d;
		private double _end = 0.0d;
		private double _duration = 0.0d;
		private EasingType _type = EasingType.None;
		private double totalMillis = 0.0d;

		//constructor
		public Tween(Action<double> setFunc, double start, double end, double duration, EasingType type = EasingType.Linear) {
			if (setFunc == null) {
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
			
			lock (listLock) {
				tweens.Add(this);
			}

			this.setFunc = setFunc;
			_start = start;
			_end = end;
			_duration = duration;
			_type = type;
			
			timer.Elapsed += onTimer;
		}
		~Tween() {
			bool running = IsRunning;
			timer.Elapsed -= onTimer;
			lock (listLock) {
				tweens.Remove(this);
			}

			if (running) {
				Complete?.Invoke(this, EventArgs.Empty);
			}
		}

		//public
		public static Tween From(object obj, string propertyName, double start, double duration, EasingType type = EasingType.Linear) {
			return From(ReflectUtil.FunctionFromPropertyGetter<double>(obj, propertyName), ReflectUtil.ActionFromPropertySetter<double>(obj, propertyName), start, duration, type);
		}
		public static Tween From(Func<double> getFunc, Action<double> setFunc, double start, double duration, EasingType type = EasingType.Linear) {
			return new Tween(setFunc, start, getFunc.Invoke(), duration, type);
		}

		public static Tween To(object obj, string propertyName, double end, double duration, EasingType type = EasingType.Linear) {
			return To(ReflectUtil.FunctionFromPropertyGetter<double>(obj, propertyName), ReflectUtil.ActionFromPropertySetter<double>(obj, propertyName), end, duration, type);
		}
		public static Tween To(Func<double> getFunc, Action<double> setFunc, double end, double duration, EasingType type = EasingType.Linear) {
			return new Tween(setFunc, getFunc.Invoke(), end, duration, type);
		}

		public static Tween FromTo(object obj, string propertyName, double start, double end, double duration, EasingType type = EasingType.Linear) {
			return FromTo(ReflectUtil.ActionFromPropertySetter<double>(obj, propertyName), start, end, duration, type);
		}
		public static Tween FromTo(Action<double> setFunc, double start, double end, double duration, EasingType type = EasingType.Linear) {
			return new Tween(setFunc, start, end, duration, type);
		}

		public static double UpdateInterval {
			get {
				return timer.Interval;
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}
				if (value < 0.001d) {
					value = 0.001d;
				}
				timer.Interval = value;
			}
		}

		public bool IsRunning {
			get {
				lock (listLock) {
					return tweens.Contains(this);
				}
			}
		}

		public double StartD {
			get {
				return _start;
			}
		}
		public double EndD {
			get {
				return _end;
			}
		}
		public double DurationD {
			get {
				return _duration;
			}
		}
		public EasingType TypeD {
			get {
				return _type;
			}
		}

		public void Pause() {
			timer.Elapsed -= onTimer;
			lock (listLock) {
				tweens.Remove(this);
			}
		}
		public void Stop() {
			timer.Elapsed -= onTimer;
			lock (listLock) {
				tweens.Remove(this);
			}
			totalMillis = 0.0d;
			Complete?.Invoke(this, EventArgs.Empty);
		}
		public void Start() {
			lock (listLock) {
				tweens.Add(this);
			}
			timer.Elapsed += onTimer;
		}

		//private
		private void onTimer(object sender, PreciseElapsedEventArgs e) {
			totalMillis += e.DeltaTime;
			if (totalMillis < _duration) {
				setFunc.Invoke(Easing.Ease(_start, _end, _duration, totalMillis, _type));
			} else {
				setFunc.Invoke(Easing.Ease(_start, _end, _duration, _duration, _type));
				timer.Elapsed -= onTimer;
				lock (listLock) {
					tweens.Remove(this);
				}
				Complete?.Invoke(this, EventArgs.Empty);
			}
		}
	}
}
