using Egg82LibEnhanced.Patterns.Prototypes;
using System;

namespace Egg82LibEnhanced.Geom {
	public class PreciseLine : IEquatable<PreciseLine>, IPrototype {
		//vars
		private PrecisePoint start = new PrecisePoint();
		private PrecisePoint end = new PrecisePoint();

		//enums
		private enum CohenSutherlandBitCode {
			INSIDE = 0,
			LEFT = 1,
			RIGHT = 2,
			BOTTOM = 4,
			TOP = 8
		}

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
		public double Slope {
			get {
				return (end.Y - start.Y) / (end.X - start.X);
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
		public PrecisePoint Intersects(PreciseEllipse ellipse, double epsilon = 0.0d) {
			if (ellipse == null) {
				throw new ArgumentNullException("ellipse");
			}
			if (epsilon <= 0.0d) {
				throw new InvalidOperationException("epsilon cannot be <= 0");
			}

			double aa = 0.0d;
			double bb = 0.0d;
			double cc = 0.0d;
			double m = 0.0d;

			double a = ellipse.Major;
			double b = ellipse.Minor;

			PrecisePoint intersection1 = null;
			PrecisePoint intersection2 = null;
			PrecisePoint center = ellipse.Center;

			if (start.X == end.X) {
				// Vertical line
				aa = a * a;
				bb = -2.0d * center.Y * a * a;
				cc = -a * a * b * b + b * b * (start.X - center.X) * (start.X - center.X);
			} else {
				// Non-vertical line
				m = Slope;
				double c = start.Y - m * start.X;

				aa = b * b + a * a * m * m;
				bb = 2 * a * a * c * m - 2 * a * a * center.Y * m - 2 * center.X * b * b;
				cc = b * b * center.X * center.X + a * a * c * c - 2 * a * a * center.Y * c + a * a * center.Y * center.Y - a * a * b * b;
			}

			double d = bb * bb - 4.0d * aa * cc;

			if (d > 0.0d) {
				// Intersections found
				if (start.X == end.X) {
					// Vertical line
					double y1 = (-bb + Math.Sqrt(d)) / (2 * aa);
					intersection1 = new PrecisePoint(start.X, y1);
					double y2 = (-bb - Math.Sqrt(d)) / (2 * aa);
					intersection2 = new PrecisePoint(start.X, y2);
				} else {
					// Non-vertical line
					double x1 = (-bb + Math.Sqrt(d)) / (2 * aa);
					double y1 = start.Y + m * (x1 - start.X);
					intersection1 = new PrecisePoint(x1, y1);

					double x2 = (-bb - Math.Sqrt(d)) / (2 * aa);
					double y2 = start.Y + m * (x2 - start.X);
					intersection2 = new PrecisePoint(x2, y2);
				}
			} else {
				// No intersections
				return null;
			}

			// Get closest result
			double startDistance = (intersection1.X - start.X) * (intersection1.X - start.X) + (intersection1.Y - start.Y) * (intersection1.Y - start.Y);
			double endDistance = (intersection2.X - start.X) * (intersection2.X - start.X) + (intersection2.Y - start.Y) * (intersection2.Y - start.Y);

			PrecisePoint result = null;

			if (startDistance <= endDistance) {
				result = intersection1;
			} else {
				result = intersection2;
			}

			// Is the result part of this line?
			if (!Intersects(result, epsilon)) {
				return null;
			}
			return result;
		}
		public PrecisePoint Intersects(PreciseRectangle rectangle, double epsilon = 1e-4) {
			if (epsilon <= 0.0d) {
				throw new InvalidOperationException("epsilon cannot be <= 0");
			}

			PrecisePoint p0 = (PrecisePoint) start.Clone();
			PrecisePoint p1 = (PrecisePoint) end.Clone();
			CohenSutherlandBitCode outCode0 = ComputeOutCode(rectangle, p0, epsilon);
			CohenSutherlandBitCode outCode1 = ComputeOutCode(rectangle, p1, epsilon);

			while (true) {
				if (0 == (outCode0 | outCode1)) {
					// Both points are inside
					return (!p0.Equals(start) || Intersects(p0, epsilon)) ? p0 : null;
				} else if (0 != (outCode0 & outCode1)) {
					// Both points are outside and does not intersect
					return (!p0.Equals(start) || Intersects(p0, epsilon)) ? p0 : null;
				} else {
					PrecisePoint p = new PrecisePoint();
					CohenSutherlandBitCode outCodeOut = (outCode0 != 0) ? outCode0 : outCode1;

					if (0 != (outCodeOut & CohenSutherlandBitCode.TOP)) {
						p.X = p0.X + (p1.X - p0.X) * (rectangle.Bottom - p0.Y) / (p1.Y - p0.Y);
						p.Y = rectangle.Bottom;
					} else if (0 != (outCodeOut & CohenSutherlandBitCode.BOTTOM)) {
						p.X = p0.X + (p1.X - p0.X) * (rectangle.Top - p0.Y) / (p1.Y - p0.Y);
						p.Y = rectangle.Top;
					} else if (0 != (outCodeOut & CohenSutherlandBitCode.RIGHT)) {
						p.Y = p0.Y + (p1.Y - p0.Y) * (rectangle.Right - p0.X) / (p1.X - p0.X);
						p.X = rectangle.Right;
					} else if (0 != (outCodeOut & CohenSutherlandBitCode.LEFT)) {
						p.Y = p0.Y + (p1.Y - p0.Y) * (rectangle.Left - p0.X) / (p1.X - p0.X);
						p.X = rectangle.Left;
					}

					if (outCodeOut == outCode0) {
						outCode0 = ComputeOutCode(rectangle, p0 = p, epsilon);
					} else {
						outCode1 = ComputeOutCode(rectangle, p1 = p, epsilon);
					}
				}
			}
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
			if (epsilon < 0.0d) {
				throw new InvalidOperationException("epsilon cannot be < 0");
			}

			double crossProduct = (point.Y - start.Y) * (end.X - start.X) - (point.X - start.X) * (end.Y - start.Y);

			if (epsilon * -1.0d <= crossProduct && crossProduct <= epsilon) {
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
		private CohenSutherlandBitCode ComputeOutCode(PreciseRectangle r, PrecisePoint p, double epsilon) {
			CohenSutherlandBitCode code = CohenSutherlandBitCode.INSIDE;

			if (r.Left - p.X >= epsilon) {
				code |= CohenSutherlandBitCode.LEFT;
			} else if (p.X - r.Right >= epsilon) {
				code |= CohenSutherlandBitCode.RIGHT;
			}
			if (r.Top - p.Y >= epsilon) {
				code |= CohenSutherlandBitCode.BOTTOM;
			} else if (p.Y - r.Bottom >= epsilon) {
				code |= CohenSutherlandBitCode.TOP;
			}

			return code;
		}
	}
}
