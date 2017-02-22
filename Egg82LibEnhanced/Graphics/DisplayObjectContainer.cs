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
		private Bitmap flattenedBitmap = null;
		private Texture oldTexture = null;

		//constructor
		public DisplayObjectContainer() {
			WindowChanged += onWindowChanged;
		}
		~DisplayObjectContainer() {
			WindowChanged -= onWindowChanged;
			RemoveAllChildren();
		}

		//public
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
			
			obj.Parent = this;
			obj.Window = Window;
			children.Insert(index, obj);

			if (_flattened) {
				Unflatten();
				Flatten();
			}
		}
		public void RemoveChild(DisplayObject obj) {
			if (obj == null) {
				throw new ArgumentNullException("obj");
			}

			int index = children.IndexOf(obj);
			if (index == -1) {
				throw new Exception("object is not a child of this object.");
			}
			
			children.RemoveAt(index);
			obj.Parent = null;
			obj.Window = null;

			if (_flattened) {
				Unflatten();
				Flatten();
			}
		}
		public void RemoveAllChildren() {
			for (int i = 0; i < children.Count; i++) {
				children[i].Parent = null;
				children[i].Window = null;
			}
			children.Clear();

			if (_flattened) {
				Unflatten();
				Flatten();
			}
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

		public void Flatten() {
			if (_flattened) {
				return;
			}

			flattenedBitmap = GetFlattenedBitmap();

			oldTexture = Texture;
			Texture = TextureUtil.FromBitmap(flattenedBitmap);
			TextureWidth = flattenedBitmap.Width;
			TextureHeight = flattenedBitmap.Height;
		}
		public void Unflatten() {
			if (!_flattened) {
				return;
			}

			Texture.Dispose();
			flattenedBitmap.Dispose();
			Texture = oldTexture;
			flattenedBitmap = null;
		}
		public bool Flattened {
			get {
				return _flattened;
			}
		}

		public Bitmap GetFlattenedBitmap() {
			PreciseRectangle bounds = getBounds();

			Bitmap retVal = new Bitmap((int) (Math.Abs(bounds.X) + bounds.Width), (int) (Math.Abs(bounds.Y) + bounds.Height));

			using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(retVal)) {
				g.Clear(System.Drawing.Color.Transparent);
				
				g.DrawImageUnscaled(GraphicsBitmap, new Point((int) Math.Abs(bounds.X), (int) Math.Abs(bounds.Y)));
				if (Texture != null) {
					Bitmap texture = TextureUtil.BitmapFromTexture(Texture);
					g.DrawImageUnscaled(texture, new Point((int) Math.Abs(bounds.X), (int) Math.Abs(bounds.Y)));
					texture.Dispose();
				}

				for (int i = children.Count - 1; i >= 0; i--) {
					Bitmap childGraphics = children[i].GraphicsBitmap;
					g.DrawImageUnscaled(childGraphics, new Point((int) (Math.Abs(bounds.X) + children[i].X), (int) (Math.Abs(bounds.Y) + children[i].Y)));
					childGraphics.Dispose();

					if (children[i].Texture != null) {
						Bitmap childTexture = TextureUtil.BitmapFromTexture(children[i].Texture);
						g.DrawImageUnscaled(childTexture, new Point((int) (Math.Abs(bounds.X) + children[i].X), (int) (Math.Abs(bounds.Y) + children[i].Y)));
						childTexture.Dispose();
					}
				}
			}

			return retVal;
		}

		//private
		internal override void Update(double deltaTime) {
			for (int i = 0; i < children.Count; i++) {
				children[i].Update(deltaTime);
			}
			base.Update(deltaTime);
		}
		internal override void SwapBuffers() {
			for (int i = 0; i < children.Count; i++) {
				children[i].SwapBuffers();
			}
			base.SwapBuffers();
		}
		internal override void Draw(RenderTarget target, Transform parentTransform, SFML.Graphics.Color parentColor) {
			base.Draw(target, parentTransform, parentColor);

			for (int i = children.Count - 1; i >= 0; i--) {
				children[i].Draw(target, Transform, parentColor * Color);
			}
		}

		private PreciseRectangle getBounds() {
			PreciseRectangle retVal = new PreciseRectangle(0.0d, 0.0d, Width, Height);

			for (int i = 0; i < children.Count; i++) {
				retVal.X = Math.Min(retVal.X, children[i].X);
				retVal.Y = Math.Min(retVal.Y, children[i].Y);
				retVal.Width = Math.Max(retVal.Width, children[i].X + children[i].Width);
				retVal.Height = Math.Max(retVal.Height, children[i].Y + children[i].Height);
			}

			return retVal;
		}
		private void onWindowChanged(object sender, EventArgs e) {
			for (int i = 0; i < children.Count; i++) {
				children[i].Window = Window;
			}
		}
	}
}
