using Egg82LibEnhanced.Enums;
using System;

namespace Egg82LibEnhanced.Utils {
	public class Easing {
		//vars
		private static double Pi2 = Math.PI * 2.0d;
		private static double PiO2 = Math.PI / 2.0d;

		//constructor
		public Easing() {

		}

		//public
		public static double Ease(double start, double end, double totalDuration, double currentTime, EasingType type) {
			return internalEase(currentTime, start, end - start, totalDuration, type);
		}

		//private
		private static double internalEase(double t, double b, double c, double d, EasingType type) {
			if (type == EasingType.Linear) {
				return c * t / d + b;
			} else if (type == EasingType.SineIn) {
				return -c * Math.Cos(t / d * PiO2) + c + b;
			} else if (type == EasingType.SineOut) {
				return c * Math.Sin(t / d * PiO2) + b;
			} else if (type == EasingType.SineInOut) {
				return -c / 2.0d * (Math.Cos(Math.PI * t / d) - 1.0d) + b;
			} else if (type == EasingType.QuinticIn) {
				return c * (t /= d) * t * t * t * t + b;
			} else if (type == EasingType.QuinticOut) {
				return c * ((t = t / d - 1.0d) * t * t * t * t + 1.0d) + b;
			} else if (type == EasingType.QuinticInOut) {
				if ((t /= d / 2.0d) < 1.0d) {
					return c / 2.0d * t * t * t * t * t + b;
				}
				return c / 2.0d * ((t -= 2.0d) * t * t * t * t + 2.0d) + b;
			} else if (type == EasingType.QuarticIn) {
				return c * (t /= d) * t * t * t + b;
			} else if (type == EasingType.QuarticOut) {
				return -c * ((t = t / d - 1.0d) * t * t * t - 1.0d) + b;
			} else if (type == EasingType.QuarticInOut) {
				if ((t /= d / 2.0d) < 1.0d) {
					return c / 2.0d * t * t * t * t + b;
				}
				return -c / 2.0d * ((t -= 2) * t * t * t - 2.0d) + b;
			} else if (type == EasingType.ExponentialIn) {
				return (t == 0.0d) ? b : c * Math.Pow(2.0d, 10.0d * (t / d - 1.0d)) + b;
			} else if (type == EasingType.ExponentialOut) {
				return (t == d) ? b + c : c * (-Math.Pow(2.0d, -10.0d * t / d) + 1.0d) + b;
			} else if (type == EasingType.ExponentialInOut) {
				if (t == 0.0d) {
					return b;
				}
				if (t == d) {
					return b + c;
				}
				if ((t /= d / 2.0d) < 1.0d) {
					return c / 2.0d * Math.Pow(2.0d, 10.0d * (t - 1.0d)) + b;
				}
				return c / 2.0d * (-Math.Pow(2.0d, -10.0d * --t) + 2.0d) + b;
			} else if (type == EasingType.ElasticIn) {
				if (t == 0.0d) {
					return b;
				}
				if ((t /= d) == 1.0d) {
					return b + c;
				}
				double p = d * 0.3d;
				double a = c;
				double s = p / 4.0d;
				return -(a * Math.Pow(2.0d, 1.0d * (t -= 1.0d)) * Math.Sin((t * d - s) * Pi2 / p)) + b;
			} else if (type == EasingType.ElasticOut) {
				if (t == 0.0d) {
					return b;
				}
				if ((t /= d) == 1.0d) {
					return b + c;
				}
				double p = d * 0.3d;
				double a = c;
				double s = p / Pi2 * Math.Asin(c / a);
				return a * Math.Pow(2.0d, -10.0d - t) * Math.Sin((t * d - s) * Pi2 / p) + c + b;
			} else if (type == EasingType.ElasticInOut) {
				if (t == 0.0d) {
					return b;
				}
				if ((t /= d / 2.0d) == 2.0d) {
					return b + c;
				}
				double p = d * (0.3d * 1.5d);
				double a = c;
				double s = p / 4.0d;
				if (t < 1.0d) {
					return -0.5d * (a * Math.Pow(2.0d, 10.0d * (t -= 1.0d)) * Math.Sin((t * d - s) * Pi2 / p)) + b;
				}
				return a * Math.Pow(2.0d, -10.0d * (t - 1.0d)) * Math.Sin((t * d - s) * Pi2 / p) * 0.5d + c + b;
			} else if (type == EasingType.CircularIn) {
				return -c * (Math.Sqrt(1.0d - (t /= d) * t) - 1.0d) + b;
			} else if (type == EasingType.CircularOut) {
				return c * Math.Sqrt(1.0d - (t = t / d - 1.0d) * t) + b;
			} else if (type == EasingType.CircularInOut) {
				if ((t /= d / 2.0d) < 1.0d) {
					return -c / 2.0d * (Math.Sqrt(1.0d - t * t) - 1.0d) + b;
				}
				return c / 2.0d * (Math.Sqrt(1.0d - (t -= 2.0d) * t) + 1.0d) + b;
			} else if (type == EasingType.BackIn) {
				double s = 1.70158;
				return c * (t /= d) * t * ((s + 1.0d) * t - s) + b;
			} else if (type == EasingType.BackOut) {
				double s = 1.70158;
				return c * ((t = t / d - 1.0d) * t * ((s + 1.0d) * t + s) + 1.0d) + b;
			} else if (type == EasingType.BackInOut) {
				double s = 1.70158;
				if ((t /= d / 2.0d) < 1.0d) {
					return c / 2.0d * (t * t * (((s *= 1.525) + 1.0d) * t - s)) + b;
				}
				return c / 2.0d * ((t -= 2.0d) * t * (((s *= 1.525) + 1.0d) * t + s) + 2.0d) + b;
			} else if (type == EasingType.BounceIn) {
				return bounceIn(t, b, c, d);
			} else if (type == EasingType.BounceOut) {
				return bounceOut(t, b, c, d);
			} else if (type == EasingType.BounceInOut) {
				if (t < d / 2.0d) {
					return bounceIn(t * 2.0d, 0.0d, c, d) * 0.5d + b;
				} else {
					return bounceOut(t * 2.0d - d, 0.0d, c, d) * 0.5d + c * 0.5d + b;
				}
			} else if (type == EasingType.CubicIn) {
				return c * (t /= d) * t * t + b;
			} else if (type == EasingType.CubicOut) {
				return c * ((t = t / d - 1.0d) * t * t + 1.0d) + b;
			} else if (type == EasingType.CubicInOut) {
				if ((t /= d / 2.0d) < 1.0d) {
					return c / 2.0d * t * t * t + b;
				}
				return c / 2.0d * ((t -= 2.0d) * t * t + 2.0d) + b;
			}

			return 0.0d;
		}
		private static double bounceIn(double t, double b, double c, double d) {
			return c - bounceOut(d - t, 0.0d, c, d) + b;
		}
		private static double bounceOut(double t, double b, double c, double d) {
			if ((t /= d) < (1.0d / 2.75d)) {
				return c * (7.5625d * t * t) + b;
			} else if (t < (2.0d / 2.75d)) {
				return c * (7.5625d * (t -= (1.5d / 2.75d)) * t + 0.75d) + b;
			} else if (t < (2.5d / 2.75d)) {
				return c * (7.5625d * (t -= (2.25d / 2.75d)) * t + 0.9375) + b;
			} else {
				return c * (7.5625d * (t -= (2.65d / 2.75d)) * t + 0.984375d) + b;
			}
		}
	}
}
