using Egg82LibEnhanced.Geom;
using Egg82LibEnhanced.Utils;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using Egg82LibEnhanced.Base;

namespace Egg82LibEnhanced.Graphics {
	public abstract class DisplayObjectContainer : DisplayObject {
		//vars
		private List<DisplayObject> children = new List<DisplayObject>();
		private PreciseRectangle childBounds = new PreciseRectangle();
		private bool _flattened = false;
		private Bitmap flattenedBitmap = null;
		private Texture oldTexture = null;

		//constructor
		/// <summary>
		/// An object used to contain/modify multiple DisplayObjects.
		/// </summary>
		public DisplayObjectContainer() {
			BoundsChanged += onBoundsChanged;
		}
		~DisplayObjectContainer() {
			BoundsChanged -= onBoundsChanged;
			RemoveAllChildren();
		}

		//public
		/// <summary>
		/// The DisplayObjectContainer's local X position, including children. This will always refer to the leftmost co-ordinate of the leftmost child DisplayObject.
		/// </summary>
		public override double X {
			get {
				return base.X + childBounds.X;
			}
			set {
				base.X = value - childBounds.X;
				for (int i = 0; i < children.Count; i++) {
					children[i].ApplyGlobalBounds();
				}
			}
		}
		/// <summary>
		/// The DisplayObjectContainer's local Y position, including children. This will always refer to the topmost co-ordinate of the topmost child DisplayObject.
		/// </summary>
		public override double Y {
			get {
				return base.Y + childBounds.Y;
			}
			set {
				base.Y = value - childBounds.Y;
				for (int i = 0; i < children.Count; i++) {
					children[i].ApplyGlobalBounds();
				}
			}
		}
		/// <summary>
		/// The DisplayObject's width, including children. Read-only.
		/// </summary>
		public override double Width {
			get {
				return childBounds.Width;
			}
		}
		/// <summary>
		/// The DisplayObject's height, including children. Read-only.
		/// </summary>
		public override double Height {
			get {
				return childBounds.Height;
			}
		}

		/// <summary>
		/// Whether or not to smooth-out the DisplayObjectContainer's textures. This includes all child textures.
		/// </summary>
		public override bool TextureSmoothing {
			get {
				return base.TextureSmoothing;
			}
			set {
				base.TextureSmoothing = value;
				for (int i = 0; i < children.Count; i++) {
					children[i].TextureSmoothing = value;
				}
			}
		}

		/// <summary>
		/// The DisplayObject's current parent. A DisplayObject may have zero or one parent(s).
		/// </summary>
		public override DisplayObject Parent {
			get {
				return base.Parent;
			}
			internal set {
				base.Parent = value;
				for (int i = 0; i < children.Count; i++) {
					children[i].ApplyGlobalBounds();
				}
			}
		}
		/// <summary>
		/// The DisplayObject's current window. A DisplayObject may have zero or one window(s).
		/// </summary>
		public override BaseWindow Window {
			get {
				return base.Window;
			}
			internal set {
				base.Window = value;
				for (int i = 0; i < children.Count; i++) {
					children[i].Window = Window;
				}
			}
		}

		/// <summary>
		/// Add a child DisplayObject to the DisplayObjectContainer.
		/// </summary>
		/// <param name="obj">The child DisplayObject to add.</param>
		/// <param name="index">(optional) The index at which to add the child. The default is the top of the stack.</param>
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
			obj.BoundsChanged += onChildBoundsChanged;
			children.Insert(index, obj);
			onChildBoundsChanged(obj, EventArgs.Empty);
		}
		/// <summary>
		/// Removes a child DisplayObject from the DisplayObjectContainer.
		/// </summary>
		/// <param name="obj">The child DisplayObject to remove.</param>
		public void RemoveChild(DisplayObject obj) {
			if (obj == null) {
				throw new ArgumentNullException("obj");
			}

			int index = children.IndexOf(obj);
			if (index == -1) {
				throw new Exception("object is not a child of this object.");
			}

			obj.BoundsChanged -= onChildBoundsChanged;
			children.RemoveAt(index);
			obj.Parent = null;
			obj.Window = null;
			onChildBoundsChanged(obj, EventArgs.Empty);
		}
		/// <summary>
		/// Removes all child DisplayObjects.
		/// </summary>
		public void RemoveAllChildren() {
			for (int i = 0; i < children.Count; i++) {
				children[i].BoundsChanged -= onChildBoundsChanged;
				children[i].Parent = null;
				children[i].Window = null;
			}
			children.Clear();
		}
		/// <summary>
		/// Returns a child DisplayObject at the specified index.
		/// </summary>
		/// <param name="index">The index of the child.</param>
		/// <returns>The child, or null if the speicified index is out-of-bounds.</returns>
		public DisplayObject GetChildAt(int index) {
			if (index < 0 || index >= children.Count) {
				throw new IndexOutOfRangeException();
			}

			return children[index];
		}
		/// <summary>
		/// Returns the index of the provided child DisplayObject.
		/// </summary>
		/// <param name="child">The child.</param>
		/// <returns>The index of the provided child, or -1 if the provided DisplayObject is not a child of this DisplayObjectConatiner.</returns>
		public int GetChildIndex(DisplayObject child) {
			if (child == null) {
				throw new ArgumentNullException("child");
			}

			return children.IndexOf(child);
		}
		/// <summary>
		/// Sets the index of the provided child DisplayObject to the provided index.
		/// </summary>
		/// <param name="child">The child.</param>
		/// <param name="index">The index to set the child to.</param>
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
			if (currentIndex == -1 || index == currentIndex) {
				return;
			}

			children.RemoveAt(currentIndex);
			children.Insert(index, child);
		}
		/// <summary>
		/// The number of children this DisplayObjectContainer has.
		/// </summary>
		public int NumChildren {
			get {
				return children.Count;
			}
		}

		/// <summary>
		/// Flattens the DisplayObjectConatiner, creating a snapshot of all
		/// its children and using that as its texture. Increases performance,
		/// but any changes made to children will not be shown until unflattened.
		/// </summary>
		public void Flatten() {
			if (_flattened) {
				return;
			}

			flattenedBitmap = GetFlattenedBitmap();

			oldTexture = Texture;
			Texture = TextureUtil.FromBitmap(flattenedBitmap);
			TextureRect.Width = flattenedBitmap.Width;
			TextureRect.Height = flattenedBitmap.Height;
		}
		/// <summary>
		/// Unflattens the DisplayObjectContainer if it has been flattened.
		/// </summary>
		public void Unflatten() {
			if (!_flattened) {
				return;
			}

			Texture.Dispose();
			flattenedBitmap.Dispose();
			Texture = oldTexture;
			flattenedBitmap = null;
			onChildBoundsChanged(null, EventArgs.Empty);
		}
		/// <summary>
		/// Whether or not the DisplayObjectContainer has been flattened.
		/// </summary>
		public bool Flattened {
			get {
				return _flattened;
			}
		}

		/// <summary>
		/// Creates and returns a new Bitmap which contains a snapshot of all the DisplayObjectContainer's children.
		/// </summary>
		/// <returns>The Bitmap snapshot.</returns>
		public Bitmap GetFlattenedBitmap() {
			Bitmap retVal = new Bitmap((int) childBounds.Width, (int) childBounds.Height);

			using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(retVal)) {
				g.Clear(System.Drawing.Color.Transparent);
				
				g.DrawImageUnscaled(GraphicsBitmap, new Point((int) Math.Abs(childBounds.X), (int) Math.Abs(childBounds.Y)));
				if (Texture != null) {
					Bitmap texture = TextureUtil.BitmapFromTexture(Texture);
					g.DrawImageUnscaled(texture, new Point((int) Math.Abs(childBounds.X), (int) Math.Abs(childBounds.Y)));
					texture.Dispose();
				}

				for (int i = children.Count - 1; i >= 0; i--) {
					Bitmap childGraphics = children[i].GraphicsBitmap;
					g.DrawImageUnscaled(childGraphics, new Point((int) (Math.Abs(childBounds.X) + children[i].X), (int) (Math.Abs(childBounds.Y) + children[i].Y)));
					childGraphics.Dispose();

					if (children[i].Texture != null) {
						Bitmap childTexture = TextureUtil.BitmapFromTexture(children[i].Texture);
						g.DrawImageUnscaled(childTexture, new Point((int) (Math.Abs(childBounds.X) + children[i].X), (int) (Math.Abs(childBounds.Y) + children[i].Y)));
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

		private void onChildBoundsChanged(object sender, EventArgs e) {
			if (_flattened) {
				return;
			}

			childBounds.X = 0.0d;
			childBounds.Y = 0.0d;
			childBounds.Width = base.Width;
			childBounds.Height = base.Height;

			for (int i = 0; i < children.Count; i++) {
				childBounds.X = Math.Min(childBounds.X, children[i].X);
				childBounds.Y = Math.Min(childBounds.Y, children[i].Y);
				childBounds.Right = Math.Max(childBounds.Right, children[i].X + children[i].Width);
				childBounds.Bottom = Math.Max(childBounds.Bottom, children[i].Y + children[i].Height);
			}
		}
		private void onBoundsChanged(object sender, EventArgs e) {
			if (_flattened) {
				return;
			}

			if (base.Width > childBounds.Width) {
				childBounds.Width = base.Width;
			}
			if (base.Height > childBounds.Height) {
				childBounds.Height = base.Height;
			}
		}
	}
}
