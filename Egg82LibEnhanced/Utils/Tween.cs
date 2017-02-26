using Egg82LibEnhanced.Enums;
using Egg82LibEnhanced.Events;
using System;
using System.Collections.Generic;

namespace Egg82LibEnhanced.Utils {
	public class Tween {
		//vars
		private static List<Tween> tweens = new List<Tween>();
		private static object listLock = new object();

		/// <summary>
		/// Called when the tween has finished updating.
		/// </summary>
		public event EventHandler Complete = null;

		private Action<double> setFunc = null;
		private double _start = 0.0d;
		private double _end = 0.0d;
		private double _duration = 0.0d;
		private EasingType _type = EasingType.None;
		private double totalMillis = 0.0d;

		//constructor
		/// <summary>
		/// An object used to smoothly translate a field from start to finish with various types of easing. The tween is automatically started upon creation.
		/// </summary>
		/// <param name="setFunc">The method that is called with the new value every time the tween is updated.</param>
		/// <param name="start">The starting value of the tween.</param>
		/// <param name="end">The ending value of the tween.</param>
		/// <param name="duration">The duration, in milliseconds, of the tween.</param>
		/// <param name="type">(optional) The type of easing the tween will use.</param>
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
		}
		~Tween() {
			bool running = IsRunning;
			lock (listLock) {
				tweens.Remove(this);
			}

			if (running) {
				Complete?.Invoke(this, EventArgs.Empty);
			}
		}

		//public
		/// <summary>
		/// Creates a tween that sets a property of the specified object. It starts at the specified value and ends at the property's current value.
		/// </summary>
		/// <param name="obj">The object to change.</param>
		/// <param name="propertyName">The name of the property to use.</param>
		/// <param name="start">The starting value.</param>
		/// <param name="duration">The duration, in milliseconds, of the tween.</param>
		/// <param name="type">(optional) The type of easing the tween will use.</param>
		/// <returns>The tween that was created.</returns>
		public static Tween From(object obj, string propertyName, double start, double duration, EasingType type = EasingType.Linear) {
			return From(ReflectUtil.FunctionFromPropertyGetter<double>(obj, propertyName), ReflectUtil.ActionFromPropertySetter<double>(obj, propertyName), start, duration, type);
		}
		/// <summary>
		/// Creates a tween that sets a method. It starts at the specified value and ends at the current value.
		/// </summary>
		/// <param name="getFunc">The method that provides the current value.</param>
		/// <param name="setFunc">The method that is called with the new value every time the tween is updated.</param>
		/// <param name="start">The starting value.</param>
		/// <param name="duration">The duration, in milliseconds, of the tween.</param>
		/// <param name="type">(optional) The type of easing the tween will use.</param>
		/// <returns>The tween that was created.</returns>
		public static Tween From(Func<double> getFunc, Action<double> setFunc, double start, double duration, EasingType type = EasingType.Linear) {
			return new Tween(setFunc, start, getFunc.Invoke(), duration, type);
		}

		/// <summary>
		/// Creates a tween that sets a property of the specified object. It starts at the property's current value and ends at the specified value.
		/// </summary>
		/// <param name="obj">The object to change.</param>
		/// <param name="propertyName">The name of the property to use.</param>
		/// <param name="end">The ending value.</param>
		/// <param name="duration">The duration, in milliseconds, of the tween.</param>
		/// <param name="type">(optional) The type of easing the tween will use.</param>
		/// <returns>The tween that was created.</returns>
		public static Tween To(object obj, string propertyName, double end, double duration, EasingType type = EasingType.Linear) {
			return To(ReflectUtil.FunctionFromPropertyGetter<double>(obj, propertyName), ReflectUtil.ActionFromPropertySetter<double>(obj, propertyName), end, duration, type);
		}
		/// <summary>
		/// Creates a tween that sets a method. It starts at the specified value and ends at the current value.
		/// </summary>
		/// <param name="getFunc">The method that provides the current value.</param>
		/// <param name="setFunc">The method that is called with the new value every time the tween is updated.</param>
		/// <param name="end">The ending value.</param>
		/// <param name="duration">The duration, in milliseconds, of the tween.</param>
		/// <param name="type">(optional) The type of easing the tween will use.</param>
		/// <returns>The tween that was created.</returns>
		public static Tween To(Func<double> getFunc, Action<double> setFunc, double end, double duration, EasingType type = EasingType.Linear) {
			return new Tween(setFunc, getFunc.Invoke(), end, duration, type);
		}

		/// <summary>
		/// Creates a tween that sets a property of the specified object. It starts at the specified value and ends at the specified value.
		/// </summary>
		/// <param name="obj">The object to change.</param>
		/// <param name="propertyName">The name of the property to use.</param>
		/// <param name="start">The starting value.</param>
		/// <param name="end">The ending value.</param>
		/// <param name="duration">The duration, in milliseconds, of the tween.</param>
		/// <param name="type">(optional) The type of easing the tween will use.</param>
		/// <returns>The tween that was created.</returns>
		public static Tween FromTo(object obj, string propertyName, double start, double end, double duration, EasingType type = EasingType.Linear) {
			return FromTo(ReflectUtil.ActionFromPropertySetter<double>(obj, propertyName), start, end, duration, type);
		}
		/// <summary>
		/// Creates a tween that sets a method. It starts at the specified value and ends at the specified value.
		/// </summary>
		/// <param name="getFunc">The method that provides the current value.</param>
		/// <param name="setFunc">The method that is called with the new value every time the tween is updated.</param>
		/// <param name="start">The starting value.</param>
		/// <param name="end">The ending value.</param>
		/// <param name="duration">The duration, in milliseconds, of the tween.</param>
		/// <param name="type">(optional) The type of easing the tween will use.</param>
		/// <returns>The tween that was created.</returns>
		public static Tween FromTo(Action<double> setFunc, double start, double end, double duration, EasingType type = EasingType.Linear) {
			return new Tween(setFunc, start, end, duration, type);
		}
		
		/// <summary>
		/// Whether or not the tween is currently running.
		/// </summary>
		public bool IsRunning {
			get {
				lock (listLock) {
					return tweens.Contains(this);
				}
			}
		}

		/// <summary>
		/// The starting value of the tween. Read-only.
		/// </summary>
		public double StartD {
			get {
				return _start;
			}
		}
		/// <summary>
		/// The ending value of the tween. Read-only.
		/// </summary>
		public double EndD {
			get {
				return _end;
			}
		}
		/// <summary>
		/// The duration, in milliseconds, of the tween. Read-only.
		/// </summary>
		public double DurationD {
			get {
				return _duration;
			}
		}
		/// <summary>
		/// The easing type of the tween. Read-only.
		/// </summary>
		public EasingType TypeD {
			get {
				return _type;
			}
		}

		/// <summary>
		/// Pauses the tween, saving its current position.
		/// </summary>
		public void Pause() {
			lock (listLock) {
				tweens.Remove(this);
			}
		}
		/// <summary>
		/// Stops and resets the tween.
		/// </summary>
		public void Stop() {
			lock (listLock) {
				tweens.Remove(this);
			}
			totalMillis = 0.0d;
			Complete?.Invoke(this, EventArgs.Empty);
		}
		/// <summary>
		/// Starts/Resumes the tween.
		/// </summary>
		public void Start() {
			lock (listLock) {
				tweens.Add(this);
			}
		}

		//private
		internal static void Update(double deltaTime) {
			lock (listLock) {
				if (tweens.Count == 0) {
					return;
				}
				for (int i = 0; i < tweens.Count; i++) {
					tweens[i].update(deltaTime);
				}
			}
		}
		private void update(double deltaTime) {
			totalMillis += deltaTime;
			if (totalMillis < _duration) {
				setFunc.Invoke(Easing.Ease(_start, _end, _duration, totalMillis, _type));
			} else {
				setFunc.Invoke(Easing.Ease(_start, _end, _duration, _duration, _type));
				Stop();
			}
		}
	}
}
