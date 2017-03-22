using Egg82LibEnhanced.Patterns.Prototypes;
using System;

namespace Egg82LibEnhanced.Geom {
	public class PreciseLine : IEquatable<PreciseLine>, IPrototype {
		//vars
		private PrecisePoint start = new PrecisePoint();
		private PrecisePoint end = new PrecisePoint();

		//constructor
		public PreciseLine(double startX, double startY, double endX, double endY) {
			if (double.IsNaN(startX)) {
				throw new ArgumentNullException("startX");
			}
			if (double.IsNaN(startY)) {
				throw new ArgumentNullException("startY");
			}
			if (double.IsNaN(endX)) {
				throw new ArgumentNullException("endX");
			}
			if (double.IsNaN(endY)) {
				throw new ArgumentNullException("endY");
			}

			start.X = startX;
			start.Y = startY;
			end.X = endX;
			end.Y = endY;
		}
		public PreciseLine(PrecisePoint start, PrecisePoint end) {
			if (start == null) {
				throw new ArgumentNullException("start");
			}
			if (end == null) {
				throw new ArgumentNullException("end");
			}

			this.start.X = start.X;
			this.start.Y = start.Y;
			this.end.X = end.X;
			this.end.Y = end.Y;
		}

		//public
		public PrecisePoint Start {
			get {
				return start;
			}
		}
		public PrecisePoint End {
			get {
				return end;
			}
		}

		public double Length {
			get {
				return Math.Abs(end.Distance(start));
			}
		}
		public double Angle {
			get {
				return ((Math.Atan2(end.Y - start.Y, end.X - start.X) * 180.0d / Math.PI) % 360.0d) + 90.0d;
			}
		}

		public PrecisePoint Intersects(PreciseLine[] polygon) {
			if (polygon == null) {
				throw new ArgumentNullException("polygon");
			}

			PrecisePoint closest = null;
			double closestDistance = double.MaxValue;
			for (int i = 0; i < polygon.Length; i++) {
				PrecisePoint intersect = Intersects(polygon[i]);

				if (intersect == null) {
					continue;
				}

				double distance = start.Distance(intersect);
				if (closest == null || distance < closestDistance) {
					closest = intersect;
					closestDistance = distance;
				}
			}

			return closest;
		}
		public PrecisePoint Intersects(PreciseEllipse ellipse) {
			
		}
		public PrecisePoint Intersects(PreciseRectangle rectangle) {
			
		}
		public PrecisePoint Intersects(PreciseLine line) {
			double a1 = end.Y - start.Y;
			double b1 = start.X - end.X;
			double c1 = a1 * start.X + b1 * start.Y;

			double a2 = line.End.Y - line.Start.Y;
			double b2 = line.Start.X - line.End.X;
			double c2 = a2 * line.Start.X + b2 * line.Start.Y;

			double delta = a1 * b2 - a2 * b1;
			if (delta == 0.0d) {
				return null;
			}

			// Result is guaranteed to be on the lines
			PrecisePoint result = new PrecisePoint((b2 * c1 - b1 * c2) / delta, (a1 * c2 - a2 * c1) / delta);

			// ..But we're dealing with line SEGMENTS. Use bounding-box collisions.

			// Point & AABB collision
			if (end.X - start.X >= 0) {
				if (result.X < start.X || result.X > end.X) {
					return null;
				}
			} else {
				if (result.X > start.X || result.X < end.X) {
					return null;
				}
			}
			if (end.Y - start.Y >= 0) {
				if (result.Y < start.Y || result.Y > end.Y) {
					return null;
				}
			} else {
				if (result.Y > start.Y || result.Y < end.Y) {
					return null;
				}
			}
			// Same as above. Code is ugly, but hopefully fast.
			if (line.End.X - line.Start.X >= 0) {
				if (result.X < line.Start.X || result.X > line.End.X) {
					return null;
				}
			} else {
				if (result.X > line.Start.X || result.X < line.End.X) {
					return null;
				}
			}
			if (line.End.Y - line.Start.Y >= 0) {
				if (result.Y < line.Start.Y || result.Y > line.End.Y) {
					return null;
				}
			} else {
				if (result.Y > line.Start.Y || result.Y < line.End.Y) {
					return null;
				}
			}

			return result;
		}
		public bool Intersects(PrecisePoint point, double epsilon = 0.0d) {
			double crossproduct = (point.Y - start.Y) * (end.X - start.X) - (point.X - start.X) * (end.Y - start.Y);

			if (epsilon * -1.0d <= crossproduct && crossproduct <= epsilon) {
				if (Math.Min(start.X, end.X) <= point.X && point.X <= Math.Max(start.X, end.X)) {
					if (Math.Min(start.Y, end.Y) <= point.Y && point.Y <= Math.Max(start.Y, end.Y)) {
						return true;
					}
				}
			}

			return false;
		}

		public bool Equals(PreciseLine other) {
			if (other == null) {
				return false;
			}
			if (other == this) {
				return true;
			}

			return (start.Equals(other.Start) && end.Equals(other.End)) ? true : false;
		}

		public IPrototype Clone() {
			return new PreciseLine(start, end);
		}
		public override string ToString() {
			return "Egg82LibEnhanced.Geom.PreciseLine {Start.X = " + start.X + ", Start.Y = " + start.Y + ", End.X = " + end.X + ", End.Y = " + end.Y + "}";
		}

		//private

	}
}
