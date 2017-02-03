using Egg82LibEnhanced.Utils;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Egg82LibEnhanced.Core {
	public class SpriteGraphics : IDisposable {
		//vars
		internal event EventHandler BoundsChanged = null;
		
		public bool Antialiasing = true;

		private Bitmap graphicsBitmap = new Bitmap(1, 1);
		private bool _changed = false;

		//constructor
		public SpriteGraphics() {
			using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(graphicsBitmap)) {
				g.Clear(Color.Transparent);
			}
		}
		~SpriteGraphics() {
			
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
		public Bitmap Bitmap {
			internal get {
				return graphicsBitmap;
			}
			set {
				throw new InvalidOperationException("Bitmap cannot be set.");
			}
		}

		public void DrawArc(Pen pen, double x, double y, double width, double height, double startAngle, double sweepAngle) {
			if (x + width + pen.Width > graphicsBitmap.Width || y + height + pen.Width > graphicsBitmap.Height) {
				Bitmap newBitmap = new Bitmap((int) (x + width + pen.Width), (int) (y + height + pen.Width));
				using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(newBitmap)) {
					g.DrawImageUnscaled(graphicsBitmap, new Point(0, 0));
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawArc(pen, (float) x, (float) y, (float) width, (float) height, (float) startAngle, (float) sweepAngle);
				}
				graphicsBitmap = newBitmap;
				if (BoundsChanged != null) {
					BoundsChanged.Invoke(this, EventArgs.Empty);
				}
			} else {
				using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(graphicsBitmap)) {
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawArc(pen, (float) x, (float) y, (float) width, (float) height, (float) startAngle, (float) sweepAngle);
				}
			}
			
			_changed = true;
		}
		public void DrawBezier(Pen pen, double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4) {
			int maxX = (int) Math.Max(Math.Max(Math.Max(x1 + pen.Width, x2 + pen.Width), x3 + pen.Width), x4 + pen.Width);
			int maxY = (int) Math.Max(Math.Max(Math.Max(y1 + pen.Width, y2 + pen.Width), y3 + pen.Width), y4 + pen.Width);
			if (maxX > graphicsBitmap.Width || maxY > graphicsBitmap.Height) {
				Bitmap newBitmap = new Bitmap(maxX, maxY);
				using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(newBitmap)) {
					g.DrawImageUnscaled(graphicsBitmap, new Point(0, 0));
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawBezier(pen, (float) x1, (float) y1, (float) x2, (float) y2, (float) x3, (float) y3, (float) x4, (float) y4);
				}
				graphicsBitmap = newBitmap;
				if (BoundsChanged != null) {
					BoundsChanged.Invoke(this, EventArgs.Empty);
				}
			} else {
				using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(graphicsBitmap)) {
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawBezier(pen, (float) x1, (float) y1, (float) x2, (float) y2, (float) x3, (float) y3, (float) x4, (float) y4);
				}
			}

			_changed = true;
		}
		public void DrawBeziers(Pen pen, PrecisePoint[] points) {
			int maxX = 0;
			int maxY = 0;

			for (int i = 0; i < points.Length; i++) {
				if (points[i].X + pen.Width > maxX) {
					maxX = (int) (points[i].X + pen.Width);
				}
				if (points[i].Y + pen.Width > maxY) {
					maxY = (int) (points[i].Y + pen.Width);
				}
			}

			if (maxX > graphicsBitmap.Width || maxY > graphicsBitmap.Height) {
				Bitmap newBitmap = new Bitmap(maxX, maxY);
				using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(newBitmap)) {
					g.DrawImageUnscaled(graphicsBitmap, new Point(0, 0));
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawBeziers(pen, precisePointArrayToPointFArray(points));
				}
				graphicsBitmap = newBitmap;
				if (BoundsChanged != null) {
					BoundsChanged.Invoke(this, EventArgs.Empty);
				}
			} else {
				using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(graphicsBitmap)) {
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawBeziers(pen, precisePointArrayToPointFArray(points));
				}
			}
			
			_changed = true;
		}
		public void DrawClosedCurve(Pen pen, PrecisePoint[] points, double tension, FillMode fillMode) {
			int maxX = 0;
			int maxY = 0;

			for (int i = 0; i < points.Length; i++) {
				if ((points[i].X * tension) + pen.Width > maxX) {
					maxX = (int) ((points[i].X * tension) + pen.Width);
				}
				if ((points[i].Y * tension) + pen.Width > maxY) {
					maxY = (int) ((points[i].Y * tension) + pen.Width);
				}
			}

			if (maxX > graphicsBitmap.Width || maxY > graphicsBitmap.Height) {
				Bitmap newBitmap = new Bitmap(maxX, maxY);
				using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(newBitmap)) {
					g.DrawImageUnscaled(graphicsBitmap, new Point(0, 0));
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawClosedCurve(pen, precisePointArrayToPointFArray(points), (float) tension, fillMode);
				}
				graphicsBitmap = newBitmap;
				if (BoundsChanged != null) {
					BoundsChanged.Invoke(this, EventArgs.Empty);
				}
			} else {
				using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(graphicsBitmap)) {
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawClosedCurve(pen, precisePointArrayToPointFArray(points), (float) tension, fillMode);
				}
			}
			
			_changed = true;
		}
		public void DrawCurve(Pen pen, PrecisePoint[] points, double offset, int numberOfSegments, double tension) {
			int maxX = 0;
			int maxY = 0;

			for (int i = 0; i < points.Length; i++) {
				if (((points[i].X + offset) * tension) + pen.Width > maxX) {
					maxX = (int) (((points[i].X + offset) * tension) + pen.Width);
				}
				if (((points[i].Y + offset) * tension) + pen.Width > maxY) {
					maxY = (int) (((points[i].Y + offset) * tension) + pen.Width);
				}
			}

			if (maxX > graphicsBitmap.Width || maxY > graphicsBitmap.Height) {
				Bitmap newBitmap = new Bitmap(maxX, maxY);
				using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(newBitmap)) {
					g.DrawImageUnscaled(graphicsBitmap, new Point(0, 0));
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawCurve(pen, precisePointArrayToPointFArray(points), (int) offset, numberOfSegments, (float) tension);
				}
				graphicsBitmap = newBitmap;
				if (BoundsChanged != null) {
					BoundsChanged.Invoke(this, EventArgs.Empty);
				}
			} else {
				using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(graphicsBitmap)) {
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawCurve(pen, precisePointArrayToPointFArray(points), (int) offset, numberOfSegments, (float) tension);
				}
			}
			
			_changed = true;
		}
		public void DrawEllipse(Pen pen, double x, double y, double width, double height) {
			if (x + width + pen.Width > graphicsBitmap.Width || y + height + pen.Width > graphicsBitmap.Height) {
				Bitmap newBitmap = new Bitmap((int) (x + width + pen.Width), (int) (y + height + pen.Width));
				using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(newBitmap)) {
					g.DrawImageUnscaled(graphicsBitmap, new Point(0, 0));
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawEllipse(pen, (float) x, (float) y, (float) width, (float) height);
				}
				graphicsBitmap = newBitmap;
				if (BoundsChanged != null) {
					BoundsChanged.Invoke(this, EventArgs.Empty);
				}
			} else {
				using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(graphicsBitmap)) {
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawEllipse(pen, (float) x, (float) y, (float) width, (float) height);
				}
			}
			
			_changed = true;
		}
		public void DrawLine(Pen pen, double x1, double y1, double x2, double y2) {
			int maxX = (int) Math.Max(x1 + pen.Width, x2 + pen.Width);
			int maxY = (int) Math.Max(y1 + pen.Width, y2 + pen.Width);

			if (maxX > graphicsBitmap.Width || maxY > graphicsBitmap.Height) {
				Bitmap newBitmap = new Bitmap(maxX, maxY);
				using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(newBitmap)) {
					g.DrawImageUnscaled(graphicsBitmap, new Point(0, 0));
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawLine(pen, (float) x1, (float) y1, (float) x2, (float) y2);
				}
				graphicsBitmap = newBitmap;
				if (BoundsChanged != null) {
					BoundsChanged.Invoke(this, EventArgs.Empty);
				}
			} else {
				using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(graphicsBitmap)) {
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawLine(pen, (float) x1, (float) y1, (float) x2, (float) y2);
				}
			}
			
			_changed = true;
		}
		public void DrawLines(Pen pen, PrecisePoint[] points) {
			int maxX = 0;
			int maxY = 0;

			for (int i = 0; i < points.Length; i++) {
				if (points[i].X + pen.Width > maxX) {
					maxX = (int) (points[i].X + pen.Width);
				}
				if (points[i].Y + pen.Width > maxY) {
					maxY = (int) (points[i].Y + pen.Width);
				}
			}

			if (maxX > graphicsBitmap.Width || maxY > graphicsBitmap.Height) {
				Bitmap newBitmap = new Bitmap(maxX, maxY);
				using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(newBitmap)) {
					g.DrawImageUnscaled(graphicsBitmap, new Point(0, 0));
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawLines(pen, precisePointArrayToPointFArray(points));
				}
				graphicsBitmap = newBitmap;
				if (BoundsChanged != null) {
					BoundsChanged.Invoke(this, EventArgs.Empty);
				}
			} else {
				using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(graphicsBitmap)) {
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawLines(pen, precisePointArrayToPointFArray(points));
				}
			}
			
			_changed = true;
		}
		public void DrawPie(Pen pen, double x, double y, double width, double height, double startAngle, double sweepAngle) {
			if (x + width + pen.Width > graphicsBitmap.Width || y + height + pen.Width > graphicsBitmap.Height) {
				Bitmap newBitmap = new Bitmap((int) (x + width + pen.Width), (int) (y + height + pen.Width));
				using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(newBitmap)) {
					g.DrawImageUnscaled(graphicsBitmap, new Point(0, 0));
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawPie(pen, (float) x, (float) y, (float) width, (float) height, (float) startAngle, (float) sweepAngle);
				}
				graphicsBitmap = newBitmap;
				if (BoundsChanged != null) {
					BoundsChanged.Invoke(this, EventArgs.Empty);
				}
			} else {
				using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(graphicsBitmap)) {
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawPie(pen, (float) x, (float) y, (float) width, (float) height, (float) startAngle, (float) sweepAngle);
				}
			}
			
			_changed = true;
		}
		public void DrawPolygon(Pen pen, PrecisePoint[] points) {
			int maxX = 0;
			int maxY = 0;

			for (int i = 0; i < points.Length; i++) {
				if (points[i].X + pen.Width > maxX) {
					maxX = (int) (points[i].X + pen.Width);
				}
				if (points[i].Y + pen.Width > maxY) {
					maxY = (int) (points[i].Y + pen.Width);
				}
			}

			if (maxX > graphicsBitmap.Width || maxY > graphicsBitmap.Height) {
				Bitmap newBitmap = new Bitmap(maxX, maxY);
				using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(newBitmap)) {
					g.DrawImageUnscaled(graphicsBitmap, new Point(0, 0));
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawPolygon(pen, precisePointArrayToPointFArray(points));
				}
				graphicsBitmap = newBitmap;
				if (BoundsChanged != null) {
					BoundsChanged.Invoke(this, EventArgs.Empty);
				}
			} else {
				using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(graphicsBitmap)) {
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawPolygon(pen, precisePointArrayToPointFArray(points));
				}
			}
			
			_changed = true;
		}
		public void DrawRectangle(Pen pen, double x, double y, double width, double height) {
			if (x + width + pen.Width > graphicsBitmap.Width || y + height + pen.Width > graphicsBitmap.Height) {
				Bitmap newBitmap = new Bitmap((int) (x + width + pen.Width), (int) (y + height + pen.Width));
				using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(newBitmap)) {
					g.DrawImageUnscaled(graphicsBitmap, new Point(0, 0));
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawRectangle(pen, (float) x, (float) y, (float) width, (float) height);
				}
				graphicsBitmap = newBitmap;
				if (BoundsChanged != null) {
					BoundsChanged.Invoke(this, EventArgs.Empty);
				}
			} else {
				using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(graphicsBitmap)) {
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawRectangle(pen, (float) x, (float) y, (float) width, (float) height);
				}
			}
			
			_changed = true;
		}
		public void DrawRectangles(Pen pen, PreciseRectangle[] rects) {
			int maxWidth = 0;
			int maxHeight = 0;

			for (int i = 0; i < rects.Length; i++) {
				if (rects[i].X + rects[i].Width + pen.Width > maxWidth) {
					maxWidth = (int) (rects[i].X + rects[i].Width + pen.Width);
				}
				if (rects[i].Y + rects[i].Height + pen.Width > maxHeight) {
					maxHeight = (int) (rects[i].Y + rects[i].Height + pen.Width);
				}
			}

			if (maxWidth > graphicsBitmap.Width || maxHeight > graphicsBitmap.Height) {
				Bitmap newBitmap = new Bitmap(maxWidth, maxHeight);
				using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(newBitmap)) {
					g.DrawImageUnscaled(graphicsBitmap, new Point(0, 0));
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawRectangles(pen, preciseRectangleArrayToRectangleFArray(rects));
				}
				graphicsBitmap = newBitmap;
				if (BoundsChanged != null) {
					BoundsChanged.Invoke(this, EventArgs.Empty);
				}
			} else {
				using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(graphicsBitmap)) {
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.DrawRectangles(pen, preciseRectangleArrayToRectangleFArray(rects));
				}
			}
			
			_changed = true;
		}
		public void FillClosedCurve(Pen pen, PrecisePoint[] points, FillMode fillMode, double tension) {
			int maxX = 0;
			int maxY = 0;

			for (int i = 0; i < points.Length; i++) {
				if ((points[i].X * tension) + pen.Width > maxX) {
					maxX = (int) ((points[i].X * tension) + pen.Width);
				}
				if ((points[i].Y * tension) + pen.Width > maxY) {
					maxY = (int) ((points[i].Y * tension) + pen.Width);
				}
			}

			if (maxX > graphicsBitmap.Width || maxY > graphicsBitmap.Height) {
				Bitmap newBitmap = new Bitmap(maxX, maxY);
				using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(newBitmap)) {
					g.DrawImageUnscaled(graphicsBitmap, new Point(0, 0));
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.FillClosedCurve(pen.Brush, precisePointArrayToPointFArray(points), fillMode, (float) tension);
				}
				graphicsBitmap = newBitmap;
				if (BoundsChanged != null) {
					BoundsChanged.Invoke(this, EventArgs.Empty);
				}
			} else {
				using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(graphicsBitmap)) {
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.FillClosedCurve(pen.Brush, precisePointArrayToPointFArray(points), fillMode, (float) tension);
				}
			}
			
			_changed = true;
		}
		public void FillEllipse(Pen pen, double x, double y, double width, double height) {
			if (x + width + pen.Width > graphicsBitmap.Width || y + height + pen.Width > graphicsBitmap.Height) {
				Bitmap newBitmap = new Bitmap((int) (x + width + pen.Width), (int) (y + height + pen.Width));
				using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(newBitmap)) {
					g.DrawImageUnscaled(graphicsBitmap, new Point(0, 0));
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.FillEllipse(pen.Brush, (float) x, (float) y, (float) width, (float) height);
				}
				graphicsBitmap = newBitmap;
				if (BoundsChanged != null) {
					BoundsChanged.Invoke(this, EventArgs.Empty);
				}
			} else {
				using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(graphicsBitmap)) {
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.FillEllipse(pen.Brush, (float) x, (float) y, (float) width, (float) height);
				}
			}
			
			_changed = true;
		}
		public void FillPie(Pen pen, double x, double y, double width, double height, double startAngle, double sweepAngle) {
			if (x + width + pen.Width > graphicsBitmap.Width || y + height + pen.Width > graphicsBitmap.Height) {
				Bitmap newBitmap = new Bitmap((int) (x + width + pen.Width), (int) (y + height + pen.Width));
				using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(newBitmap)) {
					g.DrawImageUnscaled(graphicsBitmap, new Point(0, 0));
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.FillPie(pen.Brush, (float) x, (float) y, (float) width, (float) height, (float) startAngle, (float) sweepAngle);
				}
				graphicsBitmap = newBitmap;
				if (BoundsChanged != null) {
					BoundsChanged.Invoke(this, EventArgs.Empty);
				}
			} else {
				using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(graphicsBitmap)) {
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.FillPie(pen.Brush, (float) x, (float) y, (float) width, (float) height, (float) startAngle, (float) sweepAngle);
				}
			}
			
			_changed = true;
		}
		public void FillPolygon(Pen pen, PrecisePoint[] points, FillMode fillMode) {
			int maxX = 0;
			int maxY = 0;

			for (int i = 0; i < points.Length; i++) {
				if (points[i].X + pen.Width > maxX) {
					maxX = (int) (points[i].X + pen.Width);
				}
				if (points[i].Y + pen.Width > maxY) {
					maxY = (int) (points[i].Y + pen.Width);
				}
			}

			if (maxX > graphicsBitmap.Width || maxY > graphicsBitmap.Height) {
				Bitmap newBitmap = new Bitmap(maxX, maxY);
				using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(newBitmap)) {
					g.DrawImageUnscaled(graphicsBitmap, new Point(0, 0));
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.FillPolygon(pen.Brush, precisePointArrayToPointFArray(points), fillMode);
				}
				graphicsBitmap = newBitmap;
				if (BoundsChanged != null) {
					BoundsChanged.Invoke(this, EventArgs.Empty);
				}
			} else {
				using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(graphicsBitmap)) {
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.FillPolygon(pen.Brush, precisePointArrayToPointFArray(points), fillMode);
				}
			}
			
			_changed = true;
		}
		public void FillRectangle(Pen pen, double x, double y, double width, double height) {
			if (x + width + pen.Width > graphicsBitmap.Width || y + height + pen.Width > graphicsBitmap.Height) {
				Bitmap newBitmap = new Bitmap((int) (x + width + pen.Width), (int) (y + height + pen.Width));
				using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(newBitmap)) {
					g.DrawImageUnscaled(graphicsBitmap, new Point(0, 0));
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.FillRectangle(pen.Brush, (float) x, (float) y, (float) width, (float) height);
				}
				graphicsBitmap = newBitmap;
				if (BoundsChanged != null) {
					BoundsChanged.Invoke(this, EventArgs.Empty);
				}
			} else {
				using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(graphicsBitmap)) {
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.FillRectangle(pen.Brush, (float) x, (float) y, (float) width, (float) height);
				}
			}
			
			_changed = true;
		}
		public void FillRectangles(Pen pen, PreciseRectangle[] rects) {
			int maxWidth = 0;
			int maxHeight = 0;

			for (int i = 0; i < rects.Length; i++) {
				if (rects[i].X + rects[i].Width + pen.Width > maxWidth) {
					maxWidth = (int) (rects[i].X + rects[i].Width + pen.Width);
				}
				if (rects[i].Y + rects[i].Height + pen.Width > maxHeight) {
					maxHeight = (int) (rects[i].Y + rects[i].Height + pen.Width);
				}
			}

			if (maxWidth > graphicsBitmap.Width || maxHeight > graphicsBitmap.Height) {
				Bitmap newBitmap = new Bitmap(maxWidth, maxHeight);
				using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(newBitmap)) {
					g.DrawImageUnscaled(graphicsBitmap, new Point(0, 0));
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.FillRectangles(pen.Brush, preciseRectangleArrayToRectangleFArray(rects));
				}
				graphicsBitmap = newBitmap;
				if (BoundsChanged != null) {
					BoundsChanged.Invoke(this, EventArgs.Empty);
				}
			} else {
				using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(graphicsBitmap)) {
					g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.FillRectangles(pen.Brush, preciseRectangleArrayToRectangleFArray(rects));
				}
			}
			
			_changed = true;
		}

		public void FillColor(Color color) {
			using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(graphicsBitmap)) {
				g.Clear(color);
			}
		}
		public void Clear() {
			graphicsBitmap.Dispose();
			graphicsBitmap = new Bitmap(1, 1);
			using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(graphicsBitmap)) {
				g.Clear(Color.Transparent);
			}
			if (BoundsChanged != null) {
				BoundsChanged.Invoke(this, EventArgs.Empty);
			}
			_changed = true;
		}

		public void Dispose() {
			graphicsBitmap.Dispose();
			graphicsBitmap = null;
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
