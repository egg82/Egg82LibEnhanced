using Egg82LibEnhanced.Core;
using Egg82LibEnhanced.Geom;
using Egg82LibEnhanced.Utils;
using SFML.Graphics;
using SFML.System;
using System;

namespace Egg82LibEnhanced.Display {
	public abstract class DisplayObject : IQuad {
		//vars
		/// <summary>
		/// Whether or not the DisplayObject will be drawn to the screen.
		/// </summary>
		public bool Visible = true;

		internal event EventHandler LocalBoundsChanged = null;
		
		private RenderStates graphicsRenderState = RenderStates.Default;
		private VertexArray graphicsArray = new VertexArray(PrimitiveType.Quads, 4);
		private RenderStates renderState = RenderStates.Default;
		private VertexArray renderArray = new VertexArray(PrimitiveType.Quads, 4);

		private volatile bool verticesChanged = false;
		private DisplayRect _textureBounds = new DisplayRect();
		private Color _color = new Color(255, 255, 255, 255);
		
		private PreciseRectangle _localBounds = new PreciseRectangle(0.0d, 0.0d, 1.0d, 1.0d);
		private PrecisePoint _offset = new PrecisePoint();
		private PrecisePoint _scale = new PrecisePoint(1.0d, 1.0d);
		private double _rotation = 0.0d;

		private bool _textureSmoothing = true;

		private PreciseRectangle _previousGlobalBounds = new PreciseRectangle(0.0d, 0.0d, 1.0d, 1.0d);
		private PreciseRectangle _globalBounds = new PreciseRectangle(0.0d, 0.0d, 1.0d, 1.0d);

		private DisplayGraphics _graphics = new DisplayGraphics();
		private DisplaySkew _skew = new DisplaySkew();

		private DisplayObject _parent = null;
		private Window _window = null;
		
		protected internal Transform LocalTransform = Transform.Identity;
		private Color oldParentColor = new Color(255, 255, 255, 255);

		//constructor
		/// <summary>
		/// An object used for displaying graphics to the screen.
		/// </summary>
		public DisplayObject() {
			_graphics.BoundsChanged += onGraphicsBoundsChanged;
			_textureBounds.Changed += onTextureBoundsChanged;
			_skew.Changed += onSkewChanged;
			graphicsRenderState.Texture = TextureUtil.FromBitmap(_graphics.Bitmap);
		}
		~DisplayObject() {
			if (_window != null) {
				_window.QuadTree.Remove(this);
			}
			_graphics.BoundsChanged -= onGraphicsBoundsChanged;
			_textureBounds.Changed -= onTextureBoundsChanged;
			_skew.Changed -= onSkewChanged;
			graphicsRenderState.Texture.Dispose();
			_graphics.Dispose();
		}

		//public
		/// <summary>
		/// Whether or not to smooth-out the DisplayObject's textures.
		/// </summary>
		public virtual bool TextureSmoothing {
			get {
				return _textureSmoothing;
			}
			set {
				if (value == _textureSmoothing) {
					return;
				}

				_textureSmoothing = value;
				renderState.Texture.Smooth = _textureSmoothing;
				graphicsRenderState.Texture.Smooth = _textureSmoothing;
			}
		}

		/// <summary>
		/// A copy of the DisplayObject's quad tree bounds, which is the same as its global bounds.
		/// </summary>
		public PreciseRectangle QuadBounds {
			get {
				return (PreciseRectangle) _globalBounds.Clone();
			}
		}

		/*/// <summary>
		/// A copy of the DisplayObject's global bounds.
		/// </summary>
		public PreciseRectangle GlobalBounds {
			get {
				return (PreciseRectangle) _globalBounds.Clone();
			}
		}*/
		/// <summary>
		/// The DisplayObject's global X position. This will always refer to the DisplayObject's leftmost co-ordinate.
		/// </summary>
		public double GlobalX {
			get {
				return _globalBounds.X;
			}
			set {
				X += value - _globalBounds.X;
			}
		}
		/// <summary>
		/// The DisplayObject's global Y position. This will always refer to the DisplayObject's topmost co-ordinate.
		/// </summary>
		public double GlobalY {
			get {
				return _globalBounds.Y;
			}
			set {
				Y += value - _globalBounds.Y;
			}
		}
		/*/// <summary>
		/// A copy of the DisplayObject's local bounds.
		/// </summary>
		public PreciseRectangle LocalBounds {
			get {
				return (PreciseRectangle) _localBounds.Clone();
			}
		}*/
		/// <summary>
		/// The DisplayObject's local X position. This will always refer to the DisplayObject's leftmost co-ordinate.
		/// </summary>
		public virtual double X {
			get {
				return _localBounds.X;
			}
			set {
				if (value == _localBounds.X || double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}

				/*
				 * Transform.Translate points in whatever direction rotation is going
				 * and scales to whatever the Transform's scale is. Solution:
				 * Undo rotation & scale, translate, then re-rotate and scale.
				 */
				 
				PrecisePoint prevScale = unScale();
				double prevRotation = unRotate();
				LocalTransform.Translate((float) (value - _localBounds.X), 0.0f);
				reRotate(prevRotation);
				reScale(prevScale);

				_localBounds.X = value;
				ApplyGlobalBounds();
				LocalBoundsChanged?.Invoke(this, EventArgs.Empty);
			}
		}
		/// <summary>
		/// The DisplayObject's local Y position. This will always refer to the DisplayObject's topmost co-ordinate.
		/// </summary>
		public virtual double Y {
			get {
				return _localBounds.Y;
			}
			set {
				if (value == _localBounds.Y || double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}

				/*
				 * Transform.Translate points in whatever direction rotation is going
				 * and scales to whatever the Transform's scale is. Solution:
				 * Undo rotation & scale, translate, then re-rotate and scale.
				 */

				PrecisePoint prevScale = unScale();
				double prevRotation = unRotate();
				LocalTransform.Translate(0.0f, (float) (value - _localBounds.Y));
				reRotate(prevRotation);
				reScale(prevScale);
				
				_localBounds.Y = value;
				ApplyGlobalBounds();
				LocalBoundsChanged?.Invoke(this, EventArgs.Empty);
			}
		}
		/// <summary>
		/// The DisplayObject's width. Read-only.
		/// </summary>
		public virtual double Width {
			get {
				return _localBounds.Width;
			}
		}
		/// <summary>
		/// The DisplayObject's height. Read-only.
		/// </summary>
		public virtual double Height {
			get {
				return _localBounds.Height;
			}
		}

		/// <summary>
		/// The DisplayObject's texture.
		/// </summary>
		public Texture Texture {
			get {
				return renderState.Texture;
			}
			set {
				/*
				 * Tip: Never access the current Texture's properties
				 * before switching because a lot of the frameowrk's
				 * methods call Dispose before swapping textures out.
				 */

				if (value == null) {
					renderState.Texture = null;
					_textureBounds.Width = 0.0d;
					_textureBounds.Height = 0.0d;
				} else {
					value.Smooth = _textureSmoothing;
					
					_textureBounds.Width = value.Size.X;
					_textureBounds.Height = value.Size.Y;
					applyLocalBounds();
					verticesChanged = true;

					renderState.Texture = value;
				}
			}
		}
		/// <summary>
		/// The Texture's display rectangle. This means you may have a large texture and display only part of it.
		/// </summary>
		public DisplayRect TextureRect {
			get {
				return _textureBounds;
			}
		}
		/// <summary>
		/// The DisplayGraphics object attached to this DisplayObject.
		/// </summary>
		public DisplayGraphics Graphics {
			get {
				return _graphics;
			}
		}
		/// <summary>
		/// The DisplayObject's skew.
		/// </summary>
		public DisplaySkew Skew {
			get {
				return _skew;
			}
		}
		/// <summary>
		/// The DisplayObject's color.
		/// </summary>
		public Color Color {
			get {
				return _color;
			}
			set {
				_color = value;
				verticesChanged = true;
			}
		}
		/// <summary>
		/// The DisplayObject's blend mode.
		/// </summary>
		public BlendMode BlendMode {
			get {
				return renderState.BlendMode;
			}
			set {
				graphicsRenderState.BlendMode = value;
				renderState.BlendMode = value;
			}
		}
		/// <summary>
		/// The DisplayObject's shader.
		/// </summary>
		public Shader Shader {
			get {
				return renderState.Shader;
			}
			set {
				graphicsRenderState.Shader = value;
				renderState.Shader = value;
			}
		}

		/// <summary>
		/// The DisplayObject's scale, width.
		/// </summary>
		public double ScaleX {
			get {
				return _scale.X;
			}
			set {
				if (value == _scale.X || double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}

				LocalTransform.Scale((float) (1.0d / _scale.X), 1.0f, (float) _offset.X, (float) _offset.Y);
				LocalTransform.Scale((float) value, 1.0f, (float) _offset.X, (float) _offset.Y);
				// Ensures that X, Y always points to the top-left corner
				_localBounds.X += (_offset.X * (1.0d - value)) - (_offset.X * (1.0d - _scale.X));
				_scale.X = value;
				applyLocalBounds();
			}
		}
		/// <summary>
		/// The DisplayObject's scale, height.
		/// </summary>
		public double ScaleY {
			get {
				return _scale.Y;
			}
			set {
				if (value == _scale.Y || double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}

				LocalTransform.Scale(1.0f, (float) (1.0d / _scale.Y), (float) _offset.X, (float) _offset.Y);
				LocalTransform.Scale(1.0f, (float) value, (float) _offset.X, (float) _offset.Y);
				// Ensures that X, Y always points to the top-left corner
				_localBounds.Y += (_offset.Y * (1.0d - value)) - (_offset.Y * (1.0d - _scale.Y));
				_scale.Y = value;
				applyLocalBounds();
			}
		}
		/// <summary>
		/// The DisplayObject's rotation in degrees.
		/// </summary>
		public double Rotation {
			get {
				return _rotation;
			}
			set {
				if (value == _rotation || double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}
				
				LocalTransform.Rotate((float) (value - _rotation), (float) _offset.X, (float) _offset.Y);
				_rotation = value;
			}
		}
		/// <summary>
		/// The DisplayObject's tranform offset, X. Rotation and scale transform the object at the transform offset position.
		/// </summary>
		public double TransformOffsetX {
			get {
				return _offset.X;
			}
			set {
				if (value == _offset.X || double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}

				//Making sure X, Y always points to top-left corner
				//X += _offset.X - value;
				_offset.X = value;
			}
		}
		/// <summary>
		/// The DisplayObject's tranform offset, Y. Rotation and scale transform the object at the transform offset position.
		/// </summary>
		public double TransformOffsetY {
			get {
				return _offset.Y;
			}
			set {
				if (value == _offset.Y || double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}

				//Making sure X, Y always points to top-left corner
				//X += _offset.Y - value;
				_offset.Y = value;
			}
		}

		/// <summary>
		/// The DisplayObject's current parent. A DisplayObject may have zero or one parent(s).
		/// </summary>
		public DisplayObject Parent {
			get {
				return _parent;
			}
		}
		/// <summary>
		/// The DisplayObject's current window. A DisplayObject may have zero or one window(s).
		/// </summary>
		public Window Window {
			get {
				return _window;
			}
		}

		//private
		internal virtual void Update(double deltaTime) {
			OnUpdate(deltaTime);
		}
		internal virtual void SwapBuffers() {
			OnSwapBuffers();
		}
		internal virtual void Draw(RenderTarget target, Transform parentTransform, Color parentColor) {
			if (!Visible) {
				return;
			}

			renderGraphics();

			if (parentColor != oldParentColor) {
				oldParentColor = parentColor;
				verticesChanged = true;
			}

			if (verticesChanged) {
				Color globalColor = parentColor * _color;
				graphicsArray[0] = new Vertex(new Vector2f((float) _skew.TopLeftX, (float) _skew.TopLeftY), globalColor, new Vector2f(0.0f, 0.0f));
				graphicsArray[1] = new Vertex(new Vector2f((float) (graphicsRenderState.Texture.Size.X + _skew.TopRightX), (float) _skew.TopRightY), globalColor, new Vector2f((float) graphicsRenderState.Texture.Size.X, 0.0f));
				graphicsArray[2] = new Vertex(new Vector2f((float) (graphicsRenderState.Texture.Size.X + _skew.BottomRightX), (float) (graphicsRenderState.Texture.Size.Y + _skew.BottomRightY)), globalColor, new Vector2f((float) graphicsRenderState.Texture.Size.X, (float) graphicsRenderState.Texture.Size.Y));
				graphicsArray[3] = new Vertex(new Vector2f((float) _skew.BottomLeftX, (float) (graphicsRenderState.Texture.Size.Y + _skew.BottomLeftY)), globalColor, new Vector2f(0.0f, (float) graphicsRenderState.Texture.Size.Y));
				renderArray[0] = new Vertex(new Vector2f((float) _skew.TopLeftX, (float) _skew.TopLeftY), globalColor, new Vector2f((float) _textureBounds.X, (float) _textureBounds.Y));
				renderArray[1] = new Vertex(new Vector2f((float) (_textureBounds.Width + _skew.TopRightX), (float) _skew.TopRightY), globalColor, new Vector2f((float) (_textureBounds.X + _textureBounds.Width), (float) _textureBounds.Y));
				renderArray[2] = new Vertex(new Vector2f((float) (_textureBounds.Width + _skew.BottomRightX), (float) (_textureBounds.Height + _skew.BottomRightY)), globalColor, new Vector2f((float) (_textureBounds.X + _textureBounds.Width), (float) (_textureBounds.Y + _textureBounds.Height)));
				renderArray[3] = new Vertex(new Vector2f((float) _skew.BottomLeftX, (float) (_textureBounds.Height + _skew.BottomLeftY)), globalColor, new Vector2f((float) _textureBounds.X, (float) (_textureBounds.Y + _textureBounds.Height)));
				verticesChanged = false;
			}

			Transform globalTransform = parentTransform * LocalTransform;
			graphicsRenderState.Transform = globalTransform;
			renderState.Transform = globalTransform;

			target.Draw(graphicsArray, graphicsRenderState);
			target.Draw(renderArray, renderState);
		}

		internal void ApplyGlobalBounds() {
			_globalBounds.X = (_parent != null) ? _parent.GlobalX + _localBounds.X : _localBounds.X;
			_globalBounds.Y = (_parent != null) ? _parent.GlobalY + _localBounds.Y : _localBounds.Y;
			_globalBounds.Width = _localBounds.Width;
			_globalBounds.Height = _localBounds.Height;

			if (!_globalBounds.Equals(_previousGlobalBounds)) {
				_previousGlobalBounds.X = _globalBounds.X;
				_previousGlobalBounds.Y = _globalBounds.Y;
				_previousGlobalBounds.Width = _globalBounds.Width;
				_previousGlobalBounds.Height = _globalBounds.Height;
				if (_window != null) {
					_window.QuadTree.Move(this);
				}
			}
		}

		virtual internal void SetParent(DisplayObject value) {
			if (value == _parent) {
				return;
			}

			_parent = value;
			ApplyGlobalBounds();
		}
		virtual internal void SetWindow(Window value) {
			if (value == _window) {
				return;
			}

			if (_window != null) {
				_window.QuadTree.Remove(this);
			}
			if (value != null) {
				value.QuadTree.Add(this);
			}

			_window = value;
		}

		/// <summary>
		/// Called once every frame, after physics and input has been updated. Override this to check inputs, move objects, etc.
		/// </summary>
		/// <param name="deltaTime">A value between 0 and 1 representing the amount of time (in percentage, out of the amount of time that's supposed to be taken per frame) between this frame and the last.</param>
		protected abstract void OnUpdate(double deltaTime);
		/// <summary>
		/// Called once every frame, after all objects have had OnUpdate called. Override this to provide double-buffering.
		/// </summary>
		virtual protected void OnSwapBuffers() {

		}

		private double unRotate() {
			double oldRotation = _rotation;
			if (_rotation != 0.0d) {
				LocalTransform.Rotate((float) -_rotation, (float) _offset.X, (float) _offset.Y);
				_rotation = 0.0d;
			}
			return oldRotation;
		}
		private void reRotate(double rotation) {
			if (rotation != _rotation) {
				LocalTransform.Rotate((float) rotation, (float) _offset.X, (float) _offset.Y);
				_rotation = rotation;
			}
		}
		private PrecisePoint unScale() {
			PrecisePoint oldScale = new PrecisePoint(_scale.X, _scale.Y);
			if (_scale.X != 1.0d || _scale.Y != 1.0d) {
				LocalTransform.Scale((float) (1.0d / _scale.X), (float) (1.0d / _scale.Y), (float) _offset.X, (float) _offset.Y);
				_scale.X = 1.0d;
				_scale.Y = 1.0d;
			}
			return oldScale;
		}
		private void reScale(PrecisePoint scale) {
			if (scale.X != _scale.X || scale.Y != _scale.Y) {
				LocalTransform.Scale((float) scale.X, (float) scale.Y, (float) _offset.X, (float) _offset.Y);
				_scale.X = scale.X;
				_scale.Y = scale.Y;
			}
		}
		
		private void applyLocalBounds() {
			_localBounds.Width = (Math.Max(_textureBounds.Width, _graphics.Bitmap.Width) + Math.Max(_skew.TopRightX, _skew.BottomRightX)) * _scale.X;
			_localBounds.Height = (Math.Max(_textureBounds.Height, _graphics.Bitmap.Height) + Math.Max(_skew.BottomLeftY, _skew.BottomRightY)) * _scale.Y;

			ApplyGlobalBounds();
			LocalBoundsChanged?.Invoke(this, EventArgs.Empty);
		}

		private void renderGraphics() {
			if (!_graphics.Changed) {
				return;
			}
			_graphics.Changed = false;

			graphicsRenderState.Texture.Dispose();
			Texture tex = TextureUtil.FromBitmap(_graphics.Bitmap);
			tex.Smooth = _textureSmoothing;
			graphicsRenderState.Texture = tex;
		}
		private void onGraphicsBoundsChanged(object sender, EventArgs e) {
			applyLocalBounds();
			verticesChanged = true;
		}
		private void onTextureBoundsChanged(object sender, EventArgs e) {
			applyLocalBounds();
			verticesChanged = true;
		}
		private void onSkewChanged(object sender, EventArgs e) {
			applyLocalBounds();
			verticesChanged = true;
		}
	}
}
