using Egg82LibEnhanced.Geom;
using Egg82LibEnhanced.Utils;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Egg82LibEnhanced.Graphics {
	public abstract class DisplayObjectContainer : DisplayObject {
		//vars
		private List<DisplayObject> children = new List<DisplayObject>();
		private bool _flattened = false;
		private PreciseRectangle _childrenBounds = new PreciseRectangle(0.0d, 0.0d, 1.0d, 1.0d);
		private bool boundsChanged = false;
		private Bitmap flattenedBitmap = null;
		private Texture oldTexture = null;

		//constructor
		public DisplayObjectContainer() {
			Graphics.BoundsChanged += onBoundsChanged;
		}
		~DisplayObjectContainer() {
			for (int i = 0; i < children.Count; i++) {
				children[i].BoundsChanged -= onBoundsChanged;
			}
		}

		//public
		public override PreciseRectangle LocalBounds {
			get {
				if (boundsChanged && !_flattened) {
					applyBounds();
				}
				return (PreciseRectangle) _childrenBounds.Clone();
			}
		}

		public override void Update(double deltaTime) {
			for (int i = 0; i < children.Count; i++) {
				children[i].Update(deltaTime);
			}
			if (boundsChanged && !_flattened) {
				applyBounds();
			}
			base.Update(deltaTime);
			if (boundsChanged && !_flattened) {
				applyBounds();
			}
		}
		public override void SwapBuffers() {
			for (int i = 0; i < children.Count; i++) {
				children[i].SwapBuffers();
			}
			base.SwapBuffers();
		}
		public override void Draw(RenderTarget target, Transform parentTransform) {
			base.Draw(target, parentTransform);

			for (int i = children.Count - 1; i >= 0; i--) {
				children[i].Draw(target, globalTransform);
			}
		}

		public void AddChild(DisplayObject obj, int index = 0) {
			if (obj == null) {
				throw new ArgumentNullException("obj");
			}
			if (obj.Parent != null) {
				throw new Exception("object cannot be added if it already has a parent.");
			}

			if (children.Contains(obj)) {
				return;
			}
			if (index > children.Count) {
				index = children.Count;
			}
			if (index < 0) {
				index = 0;
			}

			obj.BoundsChanged += onBoundsChanged;
			obj.Parent = this;
			obj.Window = Window;
			children.Insert(index, obj);

			if (_flattened) {
				Unflatten();
				Flatten();
			}

			boundsChanged = true;
		}
		public void RemoveChild(DisplayObject obj) {
			if (obj == null) {
				throw new ArgumentNullException("obj");
			}

			int index = children.IndexOf(obj);
			if (index == -1) {
				throw new Exception("object is not a child of this object.");
			}

			obj.BoundsChanged -= onBoundsChanged;
			children.RemoveAt(index);
			obj.Parent = null;
			obj.Window = null;

			if (_flattened) {
				Unflatten();
				Flatten();
			}

			boundsChanged = true;
		}
		public DisplayObject GetChildAt(int index) {
			if (index < 0 || index >= children.Count) {
				throw new IndexOutOfRangeException();
			}

			return children[index];
		}
		public int GetChildIndex(DisplayObject child) {
			if (child == null) {
				throw new ArgumentNullException("child");
			}

			return children.IndexOf(child);
		}
		public void SetChildIndex(DisplayObject child, int index) {
			if (child == null) {
				throw new ArgumentNullException("child");
			}
			if (index > children.Count) {
				index = children.Count;
			}
			if (index < 0) {
				index = 0;
			}

			int currentIndex = children.IndexOf(child);
			if (currentIndex == -1) {
				return;
			}

			children.RemoveAt(currentIndex);
			children.Insert(index, child);

			if (_flattened) {
				Unflatten();
				Flatten();
			}
		}
		public int NumChildren {
			get {
				return children.Count;
			}
		}
		
		public override double X {
			get {
				if (boundsChanged && !_flattened) {
					applyBounds();
				}
				return base.X + _childrenBounds.X;
			}
			set {
				if (boundsChanged && !_flattened) {
					applyBounds();
				}
				base.X = value - _childrenBounds.X;
				boundsChanged = true;
			}
		}
		public override double Y {
			get {
				if (boundsChanged && !_flattened) {
					applyBounds();
				}
				return base.Y + _childrenBounds.Y;
			}
			set {
				if (boundsChanged && !_flattened) {
					applyBounds();
				}
				base.Y = value - _childrenBounds.Y;
				boundsChanged = true;
			}
		}
		public override double Width {
			get {
				if (boundsChanged && !_flattened) {
					applyBounds();
				}
				return Math.Max(base.Width, _childrenBounds.Width);
			}
			set {
				if (boundsChanged && !_flattened) {
					applyBounds();
				}
				base.Width = value - _childrenBounds.Width;
				boundsChanged = true;
			}
		}
		public override double Height {
			get {
				if (boundsChanged && !_flattened) {
					applyBounds();
				}
				return Math.Max(base.Height, _childrenBounds.Height);
			}
			set {
				if (boundsChanged && !_flattened) {
					applyBounds();
				}
				base.Height = value - _childrenBounds.Height;
				boundsChanged = true;
			}
		}

		public void Flatten() {
			if (_flattened) {
				return;
			}

			flattenedBitmap = GetFlattenedBitmap();

			oldTexture = Texture;
			Texture = TextureUtil.FromBitmap(flattenedBitmap);
			TextureWidth = flattenedBitmap.Width;
			TextureHeight = flattenedBitmap.Height;
			X += _childrenBounds.X;
			Y += _childrenBounds.Y;
		}
		public void Unflatten() {
			if (!_flattened) {
				return;
			}

			Texture.Dispose();
			flattenedBitmap.Dispose();
			Texture = oldTexture;
			flattenedBitmap = null;
			X -= _childrenBounds.X;
			Y -= _childrenBounds.Y;
		}
		public bool Flattened {
			get {
				return _flattened;
			}
		}

		public Bitmap GetFlattenedBitmap() {
			if (boundsChanged && !_flattened) {
				applyBounds();
			}

			Bitmap retVal = new Bitmap((int) _childrenBounds.Width, (int) _childrenBounds.Height);

			using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(retVal)) {
				g.Clear(System.Drawing.Color.Transparent);

				Bitmap graphics = TextureUtil.BitmapFromTexture(GraphicsTexture);
				g.DrawImage(graphics, new Rectangle((int) (_childrenBounds.X * -1.0d), (int) (_childrenBounds.Y * -1.0d), graphics.Width, graphics.Height));
				graphics.Dispose();
				if (Texture != null) {
					Bitmap texture = TextureUtil.BitmapFromTexture(Texture);
					g.DrawImage(texture, new Rectangle((int) (_childrenBounds.X * -1.0d), (int) (_childrenBounds.Y * -1.0d), texture.Width, texture.Height));
					texture.Dispose();
				}

				for (int i = children.Count - 1; i >= 0; i--) {
					Bitmap childGraphics = TextureUtil.BitmapFromTexture(children[i].GraphicsTexture);
					g.DrawImage(childGraphics, new Rectangle((int) (_childrenBounds.X * -1.0d + children[i].X), (int) (_childrenBounds.Y * -1.0d + children[i].Y), childGraphics.Width, childGraphics.Height));
					childGraphics.Dispose();

					if (children[i].Texture != null) {
						Bitmap childTexture = TextureUtil.BitmapFromTexture(children[i].Texture);
						g.DrawImage(childTexture, new Rectangle((int) (_childrenBounds.X * -1.0d + children[i].X), (int) (_childrenBounds.Y * -1.0d + children[i].Y), childTexture.Width, childTexture.Height));
						childTexture.Dispose();
					}
				}
			}

			return retVal;
		}

		//private
		private void applyBounds() {
			double minX = base.X;
			double minY = base.Y;
			double maxWidth = base.Width;
			double maxHeight = base.Height;

			for (int i = 0; i < children.Count; i++) {
				minX = Math.Min(minX, children[i].X);
				minY = Math.Min(minY, children[i].Y);
				maxWidth = Math.Max(maxWidth, children[i].X + children[i].Width);
				maxHeight = Math.Max(maxHeight, children[i].Y + children[i].Height);
			}

			_childrenBounds.X = minX;
			_childrenBounds.Y = minY;
			_childrenBounds.Width = maxWidth;
			_childrenBounds.Height = maxHeight;

			boundsChanged = false;
		}
		private void onBoundsChanged(object sender, EventArgs e) {
			boundsChanged = true;
		}
	}
}
