using Egg82LibEnhanced.Geom;
using System;

namespace Egg82LibEnhanced.Core {
	public class DisplaySkew {
		//vars
		internal event EventHandler Changed = null;

		private PrecisePoint _topLeft = new PrecisePoint();
		private PrecisePoint _topRight = new PrecisePoint();
		private PrecisePoint _bottomLeft = new PrecisePoint();
		private PrecisePoint _bottomRight = new PrecisePoint();

		//constructor
		public DisplaySkew() {

		}

		//public
		public double TopLeftX {
			get {
				return _topLeft.X;
			}
			set {
				if (value == _topLeft.X || double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}

				_topLeft.X = value;
				Changed?.Invoke(this, EventArgs.Empty);
			}
		}
		public double TopLeftY {
			get {
				return _topLeft.Y;
			}
			set {
				if (value == _topLeft.Y || double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}

				_topLeft.Y = value;
				Changed?.Invoke(this, EventArgs.Empty);
			}
		}
		public double TopRightX {
			get {
				return _topRight.X;
			}
			set {
				if (value == _topRight.X || double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}

				_topRight.X = value;
				Changed?.Invoke(this, EventArgs.Empty);
			}
		}
		public double TopRightY {
			get {
				return _topRight.Y;
			}
			set {
				if (value == _topRight.Y || double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}

				_topRight.Y = value;
				Changed?.Invoke(this, EventArgs.Empty);
			}
		}
		public double BottomLeftX {
			get {
				return _bottomLeft.X;
			}
			set {
				if (value == _bottomLeft.X || double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}

				_bottomLeft.X = value;
				Changed?.Invoke(this, EventArgs.Empty);
			}
		}
		public double BottomLeftY {
			get {
				return _bottomLeft.Y;
			}
			set {
				if (value == _bottomLeft.Y || double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}

				_bottomLeft.Y = value;
				Changed?.Invoke(this, EventArgs.Empty);
			}
		}
		public double BottomRightX {
			get {
				return _bottomRight.X;
			}
			set {
				if (value == _bottomRight.X || double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}

				_bottomRight.X = value;
				Changed?.Invoke(this, EventArgs.Empty);
			}
		}
		public double BottomRightY {
			get {
				return _bottomRight.Y;
			}
			set {
				if (value == _bottomRight.Y || double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}

				_bottomRight.Y = value;
				Changed?.Invoke(this, EventArgs.Empty);
			}
		}

		//private

	}
}
