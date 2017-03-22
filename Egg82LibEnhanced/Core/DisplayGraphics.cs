using Egg82LibEnhanced.Geom;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Egg82LibEnhanced.Core {
	public class DisplayGraphics : IDisposable {
		//vars
		internal event EventHandler BoundsChanged = null;
		
		public bool Antialiasing = true;

		internal Bitmap Bitmap = new Bitmap(1, 1);
		internal volatile bool Changed = false;

		//constructor
		public DisplayGraphics() {
			using (Graphics g = Graphics.FromImage(Bitmap)) {
				g.Clear(Color.Transparent);
			}
		}

		//public
		public void DrawArc(Pen pen, double x, double y, double width, double height, double startAngle, double sweepAngle) {
			if (x + width + pen.Width > Bitmap.Width || y + height + pen.Width > Bitmap.Height) {
				Bitmap newBitmap = new Bitmap((int) Math.Max(x + width + pen.Width, Bitmap.Width), (int) Math.Max(y + height + pen.Width, Bitmap.Height));
				using (Graphics g = Graphics.FromImage(newBitmap)) {
					g.DrawImageUnscaled(Bitmap, new Point(0, 0));
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawArc(pen, (float) x, (float) y, (float) width, (float) height, (float) startAngle, (float) sweepAngle);
				}
				Bitmap = newBitmap;
				BoundsChanged?.Invoke(this, EventArgs.Empty);
			} else {
				using (Graphics g = Graphics.FromImage(Bitmap)) {
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawArc(pen, (float) x, (float) y, (float) width, (float) height, (float) startAngle, (float) sweepAngle);
				}
			}
			
			Changed = true;
		}
		public void DrawBezier(Pen pen, double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4) {
			int maxX = (int) Math.Max(Math.Max(Math.Max(Math.Max(x1 + pen.Width, x2 + pen.Width), x3 + pen.Width), x4 + pen.Width), Bitmap.Width);
			int maxY = (int) Math.Max(Math.Max(Math.Max(Math.Max(y1 + pen.Width, y2 + pen.Width), y3 + pen.Width), y4 + pen.Width), Bitmap.Height);
			if (maxX > Bitmap.Width || maxY > Bitmap.Height) {
				Bitmap newBitmap = new Bitmap(maxX, maxY);
				using (Graphics g = Graphics.FromImage(newBitmap)) {
					g.DrawImageUnscaled(Bitmap, new Point(0, 0));
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawBezier(pen, (float) x1, (float) y1, (float) x2, (float) y2, (float) x3, (float) y3, (float) x4, (float) y4);
				}
				Bitmap = newBitmap;
				BoundsChanged?.Invoke(this, EventArgs.Empty);
			} else {
				using (Graphics g = Graphics.FromImage(Bitmap)) {
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawBezier(pen, (float) x1, (float) y1, (float) x2, (float) y2, (float) x3, (float) y3, (float) x4, (float) y4);
				}
			}

			Changed = true;
		}
		public void DrawBeziers(Pen pen, PrecisePoint[] points) {
			int maxX = Bitmap.Width;
			int maxY = Bitmap.Height;

			for (int i = 0; i < points.Length; i++) {
				if (points[i].X + pen.Width > maxX) {
					maxX = (int) (points[i].X + pen.Width);
				}
				if (points[i].Y + pen.Width > maxY) {
					maxY = (int) (points[i].Y + pen.Width);
				}
			}

			if (maxX > Bitmap.Width || maxY > Bitmap.Height) {
				Bitmap newBitmap = new Bitmap(maxX, maxY);
				using (Graphics g = Graphics.FromImage(newBitmap)) {
					g.DrawImageUnscaled(Bitmap, new Point(0, 0));
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawBeziers(pen, precisePointArrayToPointFArray(points));
				}
				Bitmap = newBitmap;
				BoundsChanged?.Invoke(this, EventArgs.Empty);
			} else {
				using (Graphics g = Graphics.FromImage(Bitmap)) {
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawBeziers(pen, precisePointArrayToPointFArray(points));
				}
			}

			Changed = true;
		}
		public void DrawClosedCurve(Pen pen, PrecisePoint[] points, double tension, FillMode fillMode) {
			int maxX = Bitmap.Width;
			int maxY = Bitmap.Height;

			for (int i = 0; i < points.Length; i++) {
				if ((points[i].X * tension) + pen.Width > maxX) {
					maxX = (int) ((points[i].X * tension) + pen.Width);
				}
				if ((points[i].Y * tension) + pen.Width > maxY) {
					maxY = (int) ((points[i].Y * tension) + pen.Width);
				}
			}

			if (maxX > Bitmap.Width || maxY > Bitmap.Height) {
				Bitmap newBitmap = new Bitmap(maxX, maxY);
				using (Graphics g = Graphics.FromImage(newBitmap)) {
					g.DrawImageUnscaled(Bitmap, new Point(0, 0));
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawClosedCurve(pen, precisePointArrayToPointFArray(points), (float) tension, fillMode);
				}
				Bitmap = newBitmap;
				BoundsChanged?.Invoke(this, EventArgs.Empty);
			} else {
				using (Graphics g = Graphics.FromImage(Bitmap)) {
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawClosedCurve(pen, precisePointArrayToPointFArray(points), (float) tension, fillMode);
				}
			}
			
			Changed = true;
		}
		public void DrawCurve(Pen pen, PrecisePoint[] points, double offset, int numberOfSegments, double tension) {
			int maxX = Bitmap.Width;
			int maxY = Bitmap.Height;

			for (int i = 0; i < points.Length; i++) {
				if (((points[i].X + offset) * tension) + pen.Width > maxX) {
					maxX = (int) (((points[i].X + offset) * tension) + pen.Width);
				}
				if (((points[i].Y + offset) * tension) + pen.Width > maxY) {
					maxY = (int) (((points[i].Y + offset) * tension) + pen.Width);
				}
			}

			if (maxX > Bitmap.Width || maxY > Bitmap.Height) {
				Bitmap newBitmap = new Bitmap(maxX, maxY);
				using (Graphics g = Graphics.FromImage(newBitmap)) {
					g.DrawImageUnscaled(Bitmap, new Point(0, 0));
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawCurve(pen, precisePointArrayToPointFArray(points), (int) offset, numberOfSegments, (float) tension);
				}
				Bitmap = newBitmap;
				BoundsChanged?.Invoke(this, EventArgs.Empty);
			} else {
				using (Graphics g = Graphics.FromImage(Bitmap)) {
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawCurve(pen, precisePointArrayToPointFArray(points), (int) offset, numberOfSegments, (float) tension);
				}
			}
			
			Changed = true;
		}
		public void DrawEllipse(Pen pen, double x, double y, double width, double height) {
			if (x + width + pen.Width > Bitmap.Width || y + height + pen.Width > Bitmap.Height) {
				Bitmap newBitmap = new Bitmap((int) Math.Max(x + width + pen.Width, Bitmap.Width), (int) Math.Max(y + height + pen.Width, Bitmap.Height));
				using (Graphics g = Graphics.FromImage(newBitmap)) {
					g.DrawImageUnscaled(Bitmap, new Point(0, 0));
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawEllipse(pen, (float) x, (float) y, (float) width, (float) height);
				}
				Bitmap = newBitmap;
				BoundsChanged?.Invoke(this, EventArgs.Empty);
			} else {
				using (Graphics g = Graphics.FromImage(Bitmap)) {
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawEllipse(pen, (float) x, (float) y, (float) width, (float) height);
				}
			}
			
			Changed = true;
		}
		public void DrawLine(Pen pen, double x1, double y1, double x2, double y2) {
			int maxX = (int) Math.Max(Math.Max(x1 + pen.Width, x2 + pen.Width), Bitmap.Width);
			int maxY = (int) Math.Max(Math.Max(y1 + pen.Width, y2 + pen.Width), Bitmap.Height);

			if (maxX > Bitmap.Width || maxY > Bitmap.Height) {
				Bitmap newBitmap = new Bitmap(maxX, maxY);
				using (Graphics g = Graphics.FromImage(newBitmap)) {
					g.DrawImageUnscaled(Bitmap, new Point(0, 0));
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawLine(pen, (float) x1, (float) y1, (float) x2, (float) y2);
				}
				Bitmap = newBitmap;
				BoundsChanged?.Invoke(this, EventArgs.Empty);
			} else {
				using (Graphics g = Graphics.FromImage(Bitmap)) {
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawLine(pen, (float) x1, (float) y1, (float) x2, (float) y2);
				}
			}
			
			Changed = true;
		}
		public void DrawLines(Pen pen, PrecisePoint[] points) {
			int maxX = Bitmap.Width;
			int maxY = Bitmap.Height;

			for (int i = 0; i < points.Length; i++) {
				if (points[i].X + pen.Width > maxX) {
					maxX = (int) (points[i].X + pen.Width);
				}
				if (points[i].Y + pen.Width > maxY) {
					maxY = (int) (points[i].Y + pen.Width);
				}
			}

			if (maxX > Bitmap.Width || maxY > Bitmap.Height) {
				Bitmap newBitmap = new Bitmap(maxX, maxY);
				using (Graphics g = Graphics.FromImage(newBitmap)) {
					g.DrawImageUnscaled(Bitmap, new Point(0, 0));
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawLines(pen, precisePointArrayToPointFArray(points));
				}
				Bitmap = newBitmap;
				BoundsChanged?.Invoke(this, EventArgs.Empty);
			} else {
				using (Graphics g = Graphics.FromImage(Bitmap)) {
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawLines(pen, precisePointArrayToPointFArray(points));
				}
			}
			
			Changed = true;
		}
		public void DrawPie(Pen pen, double x, double y, double width, double height, double startAngle, double sweepAngle) {
			if (x + width + pen.Width > Bitmap.Width || y + height + pen.Width > Bitmap.Height) {
				Bitmap newBitmap = new Bitmap((int) Math.Max(x + width + pen.Width, Bitmap.Width), (int) Math.Max(y + height + pen.Width, Bitmap.Height));
				using (Graphics g = Graphics.FromImage(newBitmap)) {
					g.DrawImageUnscaled(Bitmap, new Point(0, 0));
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawPie(pen, (float) x, (float) y, (float) width, (float) height, (float) startAngle, (float) sweepAngle);
				}
				Bitmap = newBitmap;
				BoundsChanged?.Invoke(this, EventArgs.Empty);
			} else {
				using (Graphics g = Graphics.FromImage(Bitmap)) {
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawPie(pen, (float) x, (float) y, (float) width, (float) height, (float) startAngle, (float) sweepAngle);
				}
			}
			
			Changed = true;
		}
		public void DrawPolygon(Pen pen, PrecisePoint[] points) {
			int maxX = Bitmap.Width;
			int maxY = Bitmap.Height;

			for (int i = 0; i < points.Length; i++) {
				if (points[i].X + pen.Width > maxX) {
					maxX = (int) (points[i].X + pen.Width);
				}
				if (points[i].Y + pen.Width > maxY) {
					maxY = (int) (points[i].Y + pen.Width);
				}
			}

			if (maxX > Bitmap.Width || maxY > Bitmap.Height) {
				Bitmap newBitmap = new Bitmap(maxX, maxY);
				using (Graphics g = Graphics.FromImage(newBitmap)) {
					g.DrawImageUnscaled(Bitmap, new Point(0, 0));
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawPolygon(pen, precisePointArrayToPointFArray(points));
				}
				Bitmap = newBitmap;
				BoundsChanged?.Invoke(this, EventArgs.Empty);
			} else {
				using (Graphics g = Graphics.FromImage(Bitmap)) {
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawPolygon(pen, precisePointArrayToPointFArray(points));
				}
			}
			
			Changed = true;
		}
		public void DrawRectangle(Pen pen, double x, double y, double width, double height) {
			if (x + width + pen.Width > Bitmap.Width || y + height + pen.Width > Bitmap.Height) {
				Bitmap newBitmap = new Bitmap((int) Math.Max(x + width + pen.Width, Bitmap.Width), (int) Math.Max(y + height + pen.Width, Bitmap.Height));
				using (Graphics g = Graphics.FromImage(newBitmap)) {
					g.DrawImageUnscaled(Bitmap, new Point(0, 0));
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawRectangle(pen, (float) x, (float) y, (float) width, (float) height);
				}
				Bitmap = newBitmap;
				BoundsChanged?.Invoke(this, EventArgs.Empty);
			} else {
				using (Graphics g = Graphics.FromImage(Bitmap)) {
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawRectangle(pen, (float) x, (float) y, (float) width, (float) height);
				}
			}
			
			Changed = true;
		}
		public void DrawRectangles(Pen pen, PreciseRectangle[] rects) {
			int maxWidth = Bitmap.Width;
			int maxHeight = Bitmap.Height;

			for (int i = 0; i < rects.Length; i++) {
				if (rects[i].X + rects[i].Width + pen.Width > maxWidth) {
					maxWidth = (int) (rects[i].X + rects[i].Width + pen.Width);
				}
				if (rects[i].Y + rects[i].Height + pen.Width > maxHeight) {
					maxHeight = (int) (rects[i].Y + rects[i].Height + pen.Width);
				}
			}

			if (maxWidth > Bitmap.Width || maxHeight > Bitmap.Height) {
				Bitmap newBitmap = new Bitmap(maxWidth, maxHeight);
				using (Graphics g = Graphics.FromImage(newBitmap)) {
					g.DrawImageUnscaled(Bitmap, new Point(0, 0));
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawRectangles(pen, preciseRectangleArrayToRectangleFArray(rects));
				}
				Bitmap = newBitmap;
				BoundsChanged?.Invoke(this, EventArgs.Empty);
			} else {
				using (Graphics g = Graphics.FromImage(Bitmap)) {
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawRectangles(pen, preciseRectangleArrayToRectangleFArray(rects));
				}
			}
			
			Changed = true;
		}
		public void FillClosedCurve(Pen pen, PrecisePoint[] points, FillMode fillMode, double tension) {
			int maxX = Bitmap.Width;
			int maxY = Bitmap.Height;

			for (int i = 0; i < points.Length; i++) {
				if ((points[i].X * tension) + pen.Width > maxX) {
					maxX = (int) ((points[i].X * tension) + pen.Width);
				}
				if ((points[i].Y * tension) + pen.Width > maxY) {
					maxY = (int) ((points[i].Y * tension) + pen.Width);
				}
			}

			if (maxX > Bitmap.Width || maxY > Bitmap.Height) {
				Bitmap newBitmap = new Bitmap(maxX, maxY);
				using (Graphics g = Graphics.FromImage(newBitmap)) {
					g.DrawImageUnscaled(Bitmap, new Point(0, 0));
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.FillClosedCurve(pen.Brush, precisePointArrayToPointFArray(points), fillMode, (float) tension);
				}
				Bitmap = newBitmap;
				BoundsChanged?.Invoke(this, EventArgs.Empty);
			} else {
				using (Graphics g = Graphics.FromImage(Bitmap)) {
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.FillClosedCurve(pen.Brush, precisePointArrayToPointFArray(points), fillMode, (float) tension);
				}
			}
			
			Changed = true;
		}
		public void FillEllipse(Pen pen, double x, double y, double width, double height) {
			if (x + width + pen.Width > Bitmap.Width || y + height + pen.Width > Bitmap.Height) {
				Bitmap newBitmap = new Bitmap((int) Math.Max(x + width + pen.Width, Bitmap.Width), (int) Math.Max(y + height + pen.Width, Bitmap.Height));
				using (Graphics g = Graphics.FromImage(newBitmap)) {
					g.DrawImageUnscaled(Bitmap, new Point(0, 0));
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.FillEllipse(pen.Brush, (float) x, (float) y, (float) width, (float) height);
				}
				Bitmap = newBitmap;
				BoundsChanged?.Invoke(this, EventArgs.Empty);
			} else {
				using (Graphics g = Graphics.FromImage(Bitmap)) {
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.FillEllipse(pen.Brush, (float) x, (float) y, (float) width, (float) height);
				}
			}
			
			Changed = true;
		}
		public void FillPie(Pen pen, double x, double y, double width, double height, double startAngle, double sweepAngle) {
			if (x + width + pen.Width > Bitmap.Width || y + height + pen.Width > Bitmap.Height) {
				Bitmap newBitmap = new Bitmap((int) Math.Max(x + width + pen.Width, Bitmap.Width), (int) Math.Max(y + height + pen.Width, Bitmap.Height));
				using (Graphics g = Graphics.FromImage(newBitmap)) {
					g.DrawImageUnscaled(Bitmap, new Point(0, 0));
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.FillPie(pen.Brush, (float) x, (float) y, (float) width, (float) height, (float) startAngle, (float) sweepAngle);
				}
				Bitmap = newBitmap;
				BoundsChanged?.Invoke(this, EventArgs.Empty);
			} else {
				using (Graphics g = Graphics.FromImage(Bitmap)) {
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.FillPie(pen.Brush, (float) x, (float) y, (float) width, (float) height, (float) startAngle, (float) sweepAngle);
				}
			}
			
			Changed = true;
		}
		public void FillPolygon(Pen pen, PrecisePoint[] points, FillMode fillMode) {
			int maxX = Bitmap.Width;
			int maxY = Bitmap.Height;

			for (int i = 0; i < points.Length; i++) {
				if (points[i].X + pen.Width > maxX) {
					maxX = (int) (points[i].X + pen.Width);
				}
				if (points[i].Y + pen.Width > maxY) {
					maxY = (int) (points[i].Y + pen.Width);
				}
			}

			if (maxX > Bitmap.Width || maxY > Bitmap.Height) {
				Bitmap newBitmap = new Bitmap(maxX, maxY);
				using (Graphics g = Graphics.FromImage(newBitmap)) {
					g.DrawImageUnscaled(Bitmap, new Point(0, 0));
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.FillPolygon(pen.Brush, precisePointArrayToPointFArray(points), fillMode);
				}
				Bitmap = newBitmap;
				BoundsChanged?.Invoke(this, EventArgs.Empty);
			} else {
				using (Graphics g = Graphics.FromImage(Bitmap)) {
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.FillPolygon(pen.Brush, precisePointArrayToPointFArray(points), fillMode);
				}
			}
			
			Changed = true;
		}
		public void FillRectangle(Pen pen, double x, double y, double width, double height) {
			if (x + width + pen.Width > Bitmap.Width || y + height + pen.Width > Bitmap.Height) {
				Bitmap newBitmap = new Bitmap((int) Math.Max(x + width + pen.Width, Bitmap.Width), (int) Math.Max(y + height + pen.Width, Bitmap.Height));
				using (Graphics g = Graphics.FromImage(newBitmap)) {
					g.DrawImageUnscaled(Bitmap, new Point(0, 0));
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.FillRectangle(pen.Brush, (float) x, (float) y, (float) width, (float) height);
				}
				Bitmap = newBitmap;
				BoundsChanged?.Invoke(this, EventArgs.Empty);
			} else {
				using (Graphics g = Graphics.FromImage(Bitmap)) {
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.FillRectangle(pen.Brush, (float) x, (float) y, (float) width, (float) height);
				}
			}
			
			Changed = true;
		}
		public void FillRectangles(Pen pen, PreciseRectangle[] rects) {
			int maxWidth = Bitmap.Width;
			int maxHeight = Bitmap.Height;

			for (int i = 0; i < rects.Length; i++) {
				if (rects[i].X + rects[i].Width + pen.Width > maxWidth) {
					maxWidth = (int) (rects[i].X + rects[i].Width + pen.Width);
				}
				if (rects[i].Y + rects[i].Height + pen.Width > maxHeight) {
					maxHeight = (int) (rects[i].Y + rects[i].Height + pen.Width);
				}
			}

			if (maxWidth > Bitmap.Width || maxHeight > Bitmap.Height) {
				Bitmap newBitmap = new Bitmap(maxWidth, maxHeight);
				using (Graphics g = Graphics.FromImage(newBitmap)) {
					g.DrawImageUnscaled(Bitmap, new Point(0, 0));
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.FillRectangles(pen.Brush, preciseRectangleArrayToRectangleFArray(rects));
				}
				Bitmap = newBitmap;
				BoundsChanged?.Invoke(this, EventArgs.Empty);
			} else {
				using (Graphics g = Graphics.FromImage(Bitmap)) {
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.FillRectangles(pen.Brush, preciseRectangleArrayToRectangleFArray(rects));
				}
			}
			
			Changed = true;
		}

		public void FillColor(Color color) {
			using (Graphics g = Graphics.FromImage(Bitmap)) {
				g.Clear(color);
			}
		}
		public void Clear() {
			Bitmap.Dispose();
			Bitmap = new Bitmap(1, 1);
			using (Graphics g = Graphics.FromImage(Bitmap)) {
				g.Clear(Color.Transparent);
			}
			BoundsChanged?.Invoke(this, EventArgs.Empty);
			Changed = true;
		}

		public void Dispose() {
			Bitmap.Dispose();
			Bitmap = null;
		}

		//private
		private PointF[] precisePointArrayToPointFArray(PrecisePoint[] points) {
			if (points == null) {
				return null;
			}

			PointF[] newArr = new PointF[points.Length];
			for (int i = 0; i < points.Length; i++) {
				newArr[i].X = (float) points[i].X;
				newArr[i].Y = (float) points[i].Y;
			}

			return newArr;
		}
		private RectangleF[] preciseRectangleArrayToRectangleFArray(PreciseRectangle[] rects) {
			if (rects == null) {
				return null;
			}

			RectangleF[] newArr = new RectangleF[rects.Length];
			for (int i = 0; i < rects.Length; i++) {
				newArr[i].X = (float) rects[i].X;
				newArr[i].Y = (float) rects[i].Y;
				newArr[i].Width = (float) rects[i].Width;
				newArr[i].Height = (float) rects[i].Height;
			}

			return newArr;
		}
	}
}
