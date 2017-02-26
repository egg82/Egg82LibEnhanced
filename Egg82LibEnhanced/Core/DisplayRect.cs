using Egg82LibEnhanced.Geom;
using System;

namespace Egg82LibEnhanced.Core {
	public class DisplayRect {
		//vars
		internal event EventHandler Changed = null;

		private PreciseRectangle _bounds = new PreciseRectangle();

		//constructor
		public DisplayRect() {

		}

		//public
		public PreciseRectangle Bounds {
			get {
				return (PreciseRectangle) _bounds.Clone();
			}
		}
		
		public double X {
			get {
				return _bounds.X;
			}
			set {
				if (value == _bounds.X || double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}

				_bounds.X = value;
				Changed?.Invoke(this, EventArgs.Empty);
			}
		}
		public double Y {
			get {
				return _bounds.Y;
			}
			set {
				if (value == _bounds.Y || double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}

				_bounds.Y = value;
				Changed?.Invoke(this, EventArgs.Empty);
			}
		}
		public double Width {
			get {
				return _bounds.Width;
			}
			set {
				if (value == _bounds.Width || double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}

				_bounds.Width = value;
				Changed?.Invoke(this, EventArgs.Empty);
			}
		}
		public double Height {
			get {
				return _bounds.Height;
			}
			set {
				if (value == _bounds.Height || double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}

				_bounds.Height = value;
				Changed?.Invoke(this, EventArgs.Empty);
			}
		}

		//private

	}
}
