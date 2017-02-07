using Egg82LibEnhanced.Patterns.Prototypes;
using SFML.System;
using System;
using System.Drawing;

namespace Egg82LibEnhanced.Geom {
	public class PrecisePoint : IEquatable<PrecisePoint>, IPrototype {
		//vars
		private double _x = 0.0d;
		private double _y = 0.0d;

		//constructor
		public PrecisePoint(double x = 0.0d, double y = 0.0d) {
			if (double.IsNaN(x)) {
				throw new ArgumentNullException("x");
			}
			if (double.IsNaN(y)) {
				throw new ArgumentNullException("y");
			}

			_x = x;
			_y = y;
		}

		//public
		public static PrecisePoint FromPoint(Point p) {
			if (p == null) {
				throw new ArgumentNullException("p");
			}
			return new PrecisePoint((double) p.X, (double) p.Y);
		}
		public static PrecisePoint FromVector2f(Vector2f p) {
			return new PrecisePoint((double) p.X, (double) p.Y);
		}

		public double X {
			get {
				return _x;
			}
			set {
				if (double.IsNaN(value)) {
					return;
				}
				_x = value;
			}
		}
		public double Y {
			get {
				return _y;
			}
			set {
				if (double.IsNaN(value)) {
					return;
				}
				_y = value;
			}
		}

		public double Length {
			get {
				return Math.Sqrt(_x * _x + _y * _y);
			}
			set {
				if (double.IsNaN(value)) {
					return;
				}
				_x = _x * value;
				_y = _y * value;
			}
		}
		public double Angle {
			get {
				return Math.Atan2(_y, _x);
			}
		}

		public bool IsOrigin {
			get {
				return (_x == 0.0d && _y == 0.0d) ? true : false;
			}
		}
		
		public void Invert() {
			_x = -_x;
			_y = -_y;
		}

		public void AddPoint(PrecisePoint point) {
			if (point == null) {
				throw new ArgumentNullException("point");
			}

			_x += point.X;
			_y += point.Y;
		}
		public void SubtractPoint(PrecisePoint point) {
			if (point == null) {
				throw new ArgumentNullException("point");
			}

			_x -= point.X;
			_y -= point.Y;
		}

		public void RotateBy(double angle) {
			if (double.IsNaN(angle)) {
				throw new ArgumentNullException("angle");
			}

			double sin = Math.Sin(angle);
			double cos = Math.Cos(angle);

			_x = _x * cos - _y * sin;
			_y = _x * sin + _y * cos;
		}
		public void Normalize() {
			if (IsOrigin) {
				return;
			}

			double inverseLength = 1.0d / Length;
			_x *= inverseLength;
			_y *= inverseLength;
		}

		public double Dot(PrecisePoint other) {
			if (other == null) {
				throw new ArgumentNullException("other");
			}
			return _x * other.X + _y * other.Y;
		}

		public void CopyFromPoint(PrecisePoint point) {
			if (point == null) {
				throw new ArgumentNullException("point");
			}
			_x = point.X;
			_y = point.Y;
		}

		public bool Equals(PrecisePoint other) {
			if (other == null) {
				return false;
			}
			if (other == this) {
				return true;
			}

			return (_x == other.X && _y == other.Y) ? true : false;
		}

		public double Distance(PrecisePoint p2) {
			if (p2 == null) {
				throw new ArgumentNullException("p2");
			}
			return Math.Sqrt((_x - p2.X) * (_x - p2.X) + (_y - p2.Y) * (_y - p2.Y));
		}

		public IPrototype Clone() {
			return new PrecisePoint(_x, _y);
		}

		public override string ToString() {
			return "Egg82LibEnhanced.Utils.PrecisePoint {X = " + _x + ", Y = " + _y + "}";
		}

		//private

	}
}
