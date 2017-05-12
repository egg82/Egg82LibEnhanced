using System;

namespace Egg82LibEnhanced.Geom {
	public class PreciseEllipse {
		//vars
		private double _x = 0.0d;
		private double _y = 0.0d;
		private double _width = 0.0d;
		private double _height = 0.0d;

		//constructor
		public PreciseEllipse(double x, double y, double width, double height) {
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
			_width = Math.Abs(width);
			_height = Math.Abs(height);
		}

		//public
		public double X {
			get {
				return _x;
			}
			set {
				if (double.IsNaN(value)) {
					throw new ArgumentNullException("value");
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
					throw new ArgumentNullException("value");
				}
				_y = value;
			}
		}

		public double Major {
			get {
				return _width / 2.0d;
			}
			set {
				if (double.IsNaN(value)) {
					throw new ArgumentNullException("value");
				}
				_width = value * 2.0d;
			}
		}
		public double Width {
			get {
				return _width;
			}
			set {
				if (double.IsNaN(value)) {
					throw new ArgumentNullException("value");
				}
				_width = value;
			}
		}

		public double Minor {
			get {
				return _height / 2.0d;
			}
			set {
				if (double.IsNaN(value)) {
					throw new ArgumentNullException("value");
				}
				_height = value * 2.0d;
			}
		}
		public double Height {
			get {
				return _height;
			}
			set {
				if (double.IsNaN(value)) {
					throw new ArgumentNullException("value");
				}
				_height = value;
			}
		}

		public PrecisePoint Center {
			get {
				return new PrecisePoint(_x + _width / 2.0d, _y + _height / 2.0d);
			}
			set {
				if (value == null) {
					throw new ArgumentNullException("value");
				}
				_x = value.X - _width / 2.0d;
				_y = value.Y - _height / 2.0d;
			}
		}

		//private

	}
}
