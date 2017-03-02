using Egg82LibEnhanced.Patterns.Prototypes;
using SFML.Graphics;
using System;
using System.Drawing;

namespace Egg82LibEnhanced.Geom {
	public class PreciseRectangle : IEquatable<PreciseRectangle>, IPrototype {
		//vars
		private double _x = 0.0d;
		private double _y = 0.0d;
		private double _width = 0.0d;
		private double _height = 0.0d;

		//constructor
		public PreciseRectangle(double x = 0.0d, double y = 0.0d, double width = 0.0d, double height = 0.0d) {
			if (double.IsNaN(x)) {
				throw new ArgumentNullException("x");
			}
			if (double.IsNaN(y)) {
				throw new ArgumentNullException("y");
			}
			if (double.IsNaN(width)) {
				throw new ArgumentNullException("width");
			}
			if (double.IsNaN(height)) {
				throw new ArgumentNullException("height");
			}

			_x = x;
			_y = y;
			_width = width;
			_height = height;
		}

		//public
		public static PreciseRectangle FromRectangle(Rectangle r) {
			if (r == null) {
				throw new ArgumentNullException("r");
			}
			return new PreciseRectangle((double) r.X, (double) r.Y, (double) r.Width, (double) r.Height);
		}
		public static PreciseRectangle FromFloatRect(FloatRect r) {
			return new PreciseRectangle((double) r.Left, (double) r.Top, (double) r.Width, (double) r.Height);
		}

		public double Top {
			get {
				return _y;
			}
			set {
				if (double.IsNaN(value)) {
					return;
				}
				_height += _y - value;
				_y = value;
			}
		}
		public double Bottom {
			get {
				return _y + _height;
			}
			set {
				if (double.IsNaN(value)) {
					return;
				}
				_height = value - _y;
			}
		}
		public double Left {
			get {
				return _x;
			}
			set {
				if (double.IsNaN(value)) {
					return;
				}
				_width += _x - value;
				_x = value;
			}
		}
		public double Right {
			get {
				return _x + _width;
			}
			set {
				if (double.IsNaN(value)) {
					return;
				}
				_width = value - _x;
			}
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
		public double Width {
			get {
				return _width;
			}
			set {
				if (double.IsNaN(value)) {
					return;
				}
				_width = value;
			}
		}
		public double Height {
			get {
				return _height;
			}
			set {
				if (double.IsNaN(value)) {
					return;
				}
				_height = value;
			}
		}

		public PrecisePoint TopLeft {
			get {
				return new PrecisePoint(_x, _y);
			}
			set {
				if (value == null) {
					throw new ArgumentNullException("value");
				}
				_x = value.X;
				_y = value.Y;
			}
		}
		public PrecisePoint BottomRight {
			get {
				return new PrecisePoint(_x + _width, _y + _height);
			}
			set {
				if (value == null) {
					throw new ArgumentNullException("value");
				}
				Right = value.X;
				Bottom = value.Y;
			}
		}

		public PrecisePoint Size {
			get {
				return new PrecisePoint(_width, _height);
			}
			set {
				if (value == null) {
					throw new ArgumentNullException("value");
				}
				_width = value.X;
				_height = value.Y;
			}
		}

		public bool Contains(PrecisePoint point) {
			if (point == null) {
				throw new ArgumentNullException("point");
			}
			return Contains(point.X, point.Y);
		}
		public bool Contains(double x, double y) {
			if (double.IsNaN(x)) {
				throw new ArgumentNullException("x");
			}
			if (double.IsNaN(y)) {
				throw new ArgumentNullException("y");
			}
			return (x >= _x && y >= _y && x <= _x + _width && y <= _y + _height) ? true : false;
		}
		public bool Contains(PreciseRectangle rectangle) {
			if (rectangle == null) {
				throw new ArgumentNullException("rectangle");
			}

			return (rectangle.X >= _x && rectangle.X + rectangle.Width <= _x + _width && rectangle.Y >= _y && rectangle.Y + rectangle.Height <= _y + _height) ? true : false;
		}

		public bool Intersects(PreciseRectangle rectangle) {
			if (rectangle == null) {
				throw new ArgumentNullException("rectangle");
			}

			bool outside = ((rectangle.X <= _x && rectangle.X + rectangle.Width <= _x) || (rectangle.X >= _x + _width && rectangle.X + rectangle.Width >= _x + _width) || (rectangle.Y <= _y && rectangle.Y + rectangle.Height <= _y) || (rectangle.Y >= _y + _height && rectangle.Y + rectangle.Height >= _y + _height)) ? true : false;
			return !outside;
		}

		public PreciseRectangle Intersection(PreciseRectangle rectangle) {
			if (rectangle == null) {
				throw new ArgumentNullException("rectangle");
			}

			double left = Math.Max(_x, rectangle.X);
			double right = Math.Min(_x + _width, rectangle.X + rectangle.Width);
			double top = Math.Max(_y, rectangle.Y);
			double bottom = Math.Min(_y + _height, rectangle.Y + rectangle.Height);

			if (left > right || top > bottom) {
				return new PreciseRectangle();
			}
			return new PreciseRectangle(left, top, right - left, bottom - top);
		}

		public PreciseRectangle Union(PreciseRectangle rectangle) {
			if (rectangle == null) {
				throw new ArgumentNullException("rectangle");
			}

			double left = Math.Max(_x, rectangle.X);
			double right = Math.Min(_x + _width, rectangle.X + rectangle.Width);
			double top = Math.Max(_y, rectangle.Y);
			double bottom = Math.Min(_y + _height, rectangle.Y + rectangle.Height);

			return new PreciseRectangle(left, top, right - left, bottom - top);
		}

		public void Inflate(double dx, double dy) {
			if (double.IsNaN(dx)) {
				throw new ArgumentNullException("dx");
			}
			if (double.IsNaN(dy)) {
				throw new ArgumentNullException("dy");
			}

			_x -= dx;
			_y -= dy;
			_width += 2.0d * dx;
			_height += 2.0d * dy;
		}

		public void Empty() {
			_x = _y = _width = _height = 0.0d;
		}

		public void CopyFromRectangle(PreciseRectangle rectangle) {
			if (rectangle == null) {
				throw new ArgumentNullException("rectangle");
			}

			_x = rectangle.X;
			_y = rectangle.Y;
			_width = rectangle.Width;
			_height = rectangle.Height;
		}

		public void Normalize() {
			if (_width < 0.0d) {
				_width = -_width;
				_x -= _width;
			}
			if (_height < 0.0d) {
				_height = -_height;
				_y -= _height;
			}
		}

		public bool IsEmpty {
			get {
				return (_width == 0.0d || _height == 0.0d) ? true : false;
			}
		}

		public bool Equals(PreciseRectangle other) {
			if (other == null) {
				return false;
			}
			if (other == this) {
				return true;
			}

			return (_x == other.X && _y == other.Y && _width == other.Width && _height == other.Height) ? true : false;
		}

		public IPrototype Clone() {
			return new PreciseRectangle(_x, _y, _width, _height);
		}

		public override string ToString() {
			return "Egg82LibEnhanced.Utils.PreciseRectangle {X = " + _x + ", Y = " + _y + ", Width = " + _width + ", Height = " + _height + "}";
		}

		//private

	}
}
