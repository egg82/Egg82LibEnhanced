using Egg82LibEnhanced.Geom;
using System;

namespace Egg82LibEnhanced.Core {
	public class SpriteSkew {
		//vars
		private PrecisePoint _topLeft = new PrecisePoint();
		private PrecisePoint _topRight = new PrecisePoint();
		private PrecisePoint _bottomLeft = new PrecisePoint();
		private PrecisePoint _bottomRight = new PrecisePoint();

		//constructor
		public SpriteSkew() {

		}

		//public
		public double TopLeftX {
			get {
				return _topLeft.X;
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}
				_topLeft.X = value;
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
			}
		}

		//private

	}
}
