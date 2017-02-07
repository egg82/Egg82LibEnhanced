using Egg82LibEnhanced.Geom;
using System;

namespace Egg82LibEnhanced.Core {
	public class SpriteSkew {
		//vars
		private PrecisePoint _topLeft = new PrecisePoint();
		private PrecisePoint _topRight = new PrecisePoint();
		private PrecisePoint _bottomLeft = new PrecisePoint();
		private PrecisePoint _bottomRight = new PrecisePoint();

		private bool _changed = false;

		//constructor
		public SpriteSkew() {

		}

		//public
		public bool Changed {
			get {
				return _changed;
			}
			internal set {
				_changed = value;
			}
		}

		public double TopLeftX {
			get {
				return _topLeft.X;
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}
				_topLeft.X = value;
				_changed = true;
			}
		}
		public double TopLeftY {
			get {
				return _topLeft.Y;
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}
				_topRight.Y = value;
				_changed = true;
			}
		}
		public double TopRightX {
			get {
				return _topRight.X;
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}
				_topRight.X = value;
				_changed = true;
			}
		}
		public double TopRightY {
			get {
				return _topRight.Y;
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}
				_topRight.Y = value;
				_changed = true;
			}
		}
		public double BottomLeftX {
			get {
				return _bottomLeft.X;
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}
				_bottomLeft.X = value;
				_changed = true;
			}
		}
		public double BottomLeftY {
			get {
				return _bottomLeft.Y;
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}
				_bottomLeft.Y = value;
				_changed = true;
			}
		}
		public double BottomRightX {
			get {
				return _bottomRight.X;
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}
				_bottomRight.X = value;
				_changed = true;
			}
		}
		public double BottomRightY {
			get {
				return _bottomRight.Y;
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}
				_bottomRight.Y = value;
				_changed = true;
			}
		}

		//private

	}
}
