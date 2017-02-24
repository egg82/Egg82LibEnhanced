using Egg82LibEnhanced.Base;
using Egg82LibEnhanced.Core;
using Egg82LibEnhanced.Geom;
using Egg82LibEnhanced.Utils;
using SFML.Graphics;
using SFML.System;
using System;
using System.Drawing;

namespace Egg82LibEnhanced.Graphics {
	public abstract class DisplayObject : IQuad {
		//vars
		public bool Visible = true;
		
		internal event EventHandler BoundsChanged = null;

		private RenderStates graphicsRenderState = RenderStates.Default;
		private VertexArray graphicsArray = new VertexArray(PrimitiveType.Quads, 4);
		private RenderStates renderState = RenderStates.Default;
		private VertexArray renderArray = new VertexArray(PrimitiveType.Quads, 4);

		private volatile bool verticesChanged = false;
		private PreciseRectangle _textureBounds = new PreciseRectangle();
		private SFML.Graphics.Color _color = new SFML.Graphics.Color(255, 255, 255, 255);
		
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
		private BaseWindow _window = null;

		private Transform globalTransform = Transform.Identity;
		private Transform localTransform = Transform.Identity;

		//constructor
		public DisplayObject() {
			_graphics.BoundsChanged += onGraphicsBoundsChanged;
			_skew.SkewChanged += onSkewChanged;
			graphicsRenderState.Texture = TextureUtil.FromBitmap(_graphics.Bitmap);
		}
		~DisplayObject() {
			if (_window != null) {
				_window.QuadTree.Remove(this);
			}
			_graphics.BoundsChanged -= onGraphicsBoundsChanged;
			_skew.SkewChanged -= onSkewChanged;
			graphicsRenderState.Texture.Dispose();
			_graphics.Dispose();
		}

		//public
		public PreciseRectangle LocalBounds {
			get {
				return (PreciseRectangle) _localBounds.Clone();
			}
		}
		public PreciseRectangle GlobalBounds {
			get {
				return (PreciseRectangle) _globalBounds.Clone();
			}
		}
		
		public double GlobalX {
			get {
				return _globalBounds.X;
			}
		}
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
				localTransform.Translate((float) (value - _localBounds.X), 0.0f);
				reRotate(prevRotation);
				reScale(prevScale);

				_localBounds.X = value;
				ApplyGlobalBounds();
				BoundsChanged?.Invoke(this, EventArgs.Empty);
			}
		}
		public double GlobalY {
			get {
				return _globalBounds.Y;
			}
		}
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
				localTransform.Translate(0.0f, (float) (value - _localBounds.Y));
				reRotate(prevRotation);
				reScale(prevScale);
				
				_localBounds.Y = value;
				ApplyGlobalBounds();
				BoundsChanged?.Invoke(this, EventArgs.Empty);
			}
		}
		public virtual double Width {
			get {
				return _localBounds.Width;
			}
		}
		public virtual double Height {
			get {
				return _localBounds.Height;
			}
		}

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

					//if (renderState.Texture == null || (_textureBounds.X == renderArray[0].TexCoords.X && _textureBounds.Y == renderArray[0].TexCoords.Y && _textureBounds.X + _textureBounds.Width == renderArray[2].TexCoords.X && _textureBounds.Y + _textureBounds.Height == renderArray[2].TexCoords.Y)) {
						_textureBounds.Width = value.Size.X;
						_textureBounds.Height = value.Size.Y;
						applyLocalBounds();
						verticesChanged = true;
					//}

					renderState.Texture = value;
				}
			}
		}
		public double TextureX {
			get {
				return _textureBounds.X;
			}
			set {
				if (value == _textureBounds.X || double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}

				_textureBounds.X = value;
				applyLocalBounds();
				verticesChanged = true;
			}
		}
		public double TextureY {
			get {
				return _textureBounds.Y;
			}
			set {
				if (value == _textureBounds.Y || double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}

				_textureBounds.Y = value;
				applyLocalBounds();
				verticesChanged = true;
			}
		}
		public double TextureWidth {
			get {
				return _textureBounds.Width;
			}
			set {
				if (value == _textureBounds.Width || double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}

				_textureBounds.Width = value;
				applyLocalBounds();
				verticesChanged = true;
			}
		}
		public double TextureHeight {
			get {
				return _textureBounds.Height;
			}
			set {
				if (value == _textureBounds.Height || double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}

				_textureBounds.Height = value;
				applyLocalBounds();
				verticesChanged = true;
			}
		}

		public bool TextureSmoothing {
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

		public DisplayGraphics Graphics {
			get {
				return _graphics;
			}
		}
		public DisplaySkew Skew {
			get {
				return _skew;
			}
		}

		public double ScaleX {
			get {
				return _scale.X;
			}
			set {
				if (value == _scale.X || double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}

				localTransform.Scale((float) (1.0d / _scale.X), 1.0f, (float) _offset.X, (float) _offset.Y);
				localTransform.Scale((float) value, 1.0f, (float) _offset.X, (float) _offset.Y);
				_scale.X = value;
				applyLocalBounds();
			}
		}
		public double ScaleY {
			get {
				return _scale.Y;
			}
			set {
				if (value == _scale.Y || double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}

				localTransform.Scale(1.0f, (float) (1.0d / _scale.Y), (float) _offset.X, (float) _offset.Y);
				localTransform.Scale(1.0f, (float) value, (float) _offset.X, (float) _offset.Y);
				_scale.Y = value;
				applyLocalBounds();
			}
		}
		public double Rotation {
			get {
				return _rotation;
			}
			set {
				if (value == _rotation || double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}
				
				localTransform.Rotate((float) (value - _rotation), (float) _offset.X, (float) _offset.Y);
				_rotation = value;
			}
		}

		public double OffsetX {
			get {
				return _offset.X;
			}
			set {
				if (value == _offset.X || double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}

				//Making sure X, Y always points to top-left corner
				X += _offset.X - value;
				_offset.X = value;
			}
		}
		public double OffsetY {
			get {
				return _offset.Y;
			}
			set {
				if (value == _offset.Y || double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}

				//Making sure X, Y always points to top-left corner
				X += _offset.Y - value;
				_offset.Y = value;
			}
		}

		public virtual DisplayObject Parent {
			get {
				return _parent;
			}
			internal set {
				if (value == _parent) {
					return;
				}

				_parent = value;
				ApplyGlobalBounds();
			}
		}
		public virtual BaseWindow Window {
			get {
				return _window;
			}
			internal set {
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
		}

		public SFML.Graphics.Color Color {
			get {
				return _color;
			}
			set {
				_color = value;
				verticesChanged = true;
			}
		}
		public BlendMode BlendMode {
			get {
				return renderState.BlendMode;
			}
			set {
				graphicsRenderState.BlendMode = value;
				renderState.BlendMode = value;
			}
		}
		public Shader Shader {
			get {
				return renderState.Shader;
			}
			set {
				graphicsRenderState.Shader = value;
				renderState.Shader = value;
			}
		}

		//private
		internal virtual void Update(double deltaTime) {
			OnUpdate(deltaTime);
		}
		internal virtual void SwapBuffers() {
			OnSwapBuffers();
		}
		internal virtual void Draw(RenderTarget target, Transform parentTransform, SFML.Graphics.Color parentColor) {
			if (!Visible) {
				return;
			}

			renderGraphics();

			if (verticesChanged) {
				graphicsArray[0] = new Vertex(new Vector2f((float) _skew.TopLeftX, (float) _skew.TopLeftY), _color * parentColor, new Vector2f(0.0f, 0.0f));
				graphicsArray[1] = new Vertex(new Vector2f((float) (graphicsRenderState.Texture.Size.X + _skew.TopRightX), (float) _skew.TopRightY), _color * parentColor, new Vector2f((float) graphicsRenderState.Texture.Size.X, 0.0f));
				graphicsArray[2] = new Vertex(new Vector2f((float) (graphicsRenderState.Texture.Size.X + _skew.BottomRightX), (float) (graphicsRenderState.Texture.Size.Y + _skew.BottomRightY)), _color * parentColor, new Vector2f((float) graphicsRenderState.Texture.Size.X, (float) graphicsRenderState.Texture.Size.Y));
				graphicsArray[3] = new Vertex(new Vector2f((float) _skew.BottomLeftX, (float) (graphicsRenderState.Texture.Size.Y + _skew.BottomLeftY)), _color * parentColor, new Vector2f(0.0f, (float) graphicsRenderState.Texture.Size.Y));
				renderArray[0] = new Vertex(new Vector2f((float) _skew.TopLeftX, (float) _skew.TopLeftY), _color * parentColor, new Vector2f((float) _textureBounds.X, (float) _textureBounds.Y));
				renderArray[1] = new Vertex(new Vector2f((float) (_textureBounds.Width + _skew.TopRightX), (float) _skew.TopRightY), _color * parentColor, new Vector2f((float) (_textureBounds.X + _textureBounds.Width), (float) _textureBounds.Y));
				renderArray[2] = new Vertex(new Vector2f((float) (_textureBounds.Width + _skew.BottomRightX), (float) (_textureBounds.Height + _skew.BottomRightY)), _color * parentColor, new Vector2f((float) (_textureBounds.X + _textureBounds.Width), (float) (_textureBounds.Y + _textureBounds.Height)));
				renderArray[3] = new Vertex(new Vector2f((float) _skew.BottomLeftX, (float) (_textureBounds.Height + _skew.BottomLeftY)), _color * parentColor, new Vector2f((float) _textureBounds.X, (float) (_textureBounds.Y + _textureBounds.Height)));
				verticesChanged = false;
			}

			globalTransform = parentTransform * localTransform;
			graphicsRenderState.Transform = globalTransform;
			renderState.Transform = globalTransform;

			target.Draw(graphicsArray, graphicsRenderState);
			target.Draw(renderArray, renderState);
		}

		protected abstract void OnUpdate(double deltaTime);
		virtual protected void OnSwapBuffers() {

		}

		internal Transform Transform {
			get {
				return globalTransform;
			}
		}
		internal Bitmap GraphicsBitmap {
			get {
				return _graphics.Bitmap;
			}
		}

		private double unRotate() {
			double oldRotation = _rotation;
			if (_rotation != 0.0d) {
				localTransform.Rotate((float) -_rotation, (float) _offset.X, (float) _offset.Y);
				_rotation = 0.0d;
			}
			return oldRotation;
		}
		private void reRotate(double rotation) {
			if (rotation != _rotation) {
				localTransform.Rotate((float) rotation, (float) _offset.X, (float) _offset.Y);
				_rotation = rotation;
			}
		}
		private PrecisePoint unScale() {
			PrecisePoint oldScale = new PrecisePoint(_scale.X, _scale.Y);
			if (_scale.X != 1.0d || _scale.Y != 1.0d) {
				localTransform.Scale((float) (1.0d / _scale.X), (float) (1.0d / _scale.Y), (float) _offset.X, (float) _offset.Y);
				_scale.X = 1.0d;
				_scale.Y = 1.0d;
			}
			return oldScale;
		}
		private void reScale(PrecisePoint scale) {
			if (scale.X != _scale.X || scale.Y != _scale.Y) {
				localTransform.Scale((float) scale.X, (float) scale.Y, (float) _offset.X, (float) _offset.Y);
				_scale.X = scale.X;
				_scale.Y = scale.Y;
			}
		}
		
		private void applyLocalBounds() {
			_localBounds.Width = Math.Max(_textureBounds.Width, _graphics.Bitmap.Width) + Math.Max(_skew.TopRightX, _skew.BottomRightX) * _scale.X;
			_localBounds.Height = Math.Max(_textureBounds.Height, _graphics.Bitmap.Height) + Math.Max(_skew.BottomLeftY, _skew.BottomRightY) * _scale.Y;

			ApplyGlobalBounds();
			BoundsChanged?.Invoke(this, EventArgs.Empty);
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

		private void renderGraphics() {
			if (!_graphics.Changed) {
				return;
			}
			_graphics.Changed = false;

			graphicsRenderState.Texture.Dispose();
			Texture tex = TextureUtil.FromBitmap(_graphics.Bitmap);
			tex.Smooth = _textureSmoothing;
			graphicsRenderState.Texture = tex;
			//TextureUtil.UpdateTextureWithBitmap(_graphics.Bitmap, graphicsSprite.Texture);
		}
		private void onGraphicsBoundsChanged(object sender, EventArgs e) {
			applyLocalBounds();
			verticesChanged = true;
		}
		private void onSkewChanged(object sender, EventArgs e) {
			applyLocalBounds();
			verticesChanged = true;
		}
	}
}
