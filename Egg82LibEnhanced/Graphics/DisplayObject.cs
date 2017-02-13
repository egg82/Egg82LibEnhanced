using Egg82LibEnhanced.Base;
using Egg82LibEnhanced.Core;
using Egg82LibEnhanced.Geom;
using Egg82LibEnhanced.Patterns.Interfaces;
using Egg82LibEnhanced.Utils;
using SFML.Graphics;
using SFML.System;
using System;

namespace Egg82LibEnhanced.Graphics {
	public abstract class DisplayObject : IUpdatable, IDrawable, IQuad {
		//vars
		public bool Visible = true;

		public event EventHandler BoundsChanged = null;

		private SpriteGraphics _graphics = new SpriteGraphics();
		private SFML.Graphics.Sprite graphicsSprite = new SFML.Graphics.Sprite();
		private RenderStates renderState = RenderStates.Default;
		private SFML.Graphics.Sprite renderSprite = new SFML.Graphics.Sprite();

		private DisplayObject _parent = null;
		private BaseWindow _window = null;

		private PrecisePoint previousScale = new PrecisePoint(1.0d, 1.0d);
		private PrecisePoint _scale = new PrecisePoint(1.0d, 1.0d);
		private double previousRotation = 0.0d;
		private double _rotation = 0.0d;
		private PrecisePoint previousXY = new PrecisePoint();
		private PreciseRectangle _localBounds = new PreciseRectangle(0.0d, 0.0d, 1.0d, 1.0d);
		//private FloatRect localBoundsCache = new FloatRect(0.0f, 0.0f, 1.0f, 1.0f);
		private PreciseRectangle previousGlobalBounds = new PreciseRectangle(0.0d, 0.0d, 1.0d, 1.0d);
		private PreciseRectangle _globalBounds = new PreciseRectangle(0.0d, 0.0d, 1.0d, 1.0d);
		private PreciseRectangle _localTextureBounds = new PreciseRectangle(0.0d, 0.0d, 1.0d, 1.0d);
		private bool _textureSmoothing = true;

		private PrecisePoint _transformOriginOffset = new PrecisePoint(0.0d, 0.0d);
		internal Transform globalTransform = Transform.Identity;
		private Transform localTransform = Transform.Identity;

		private SpriteSkew _skew = new SpriteSkew();
		private Vertex[] skewBox = new Vertex[4];

		private Color _color = new Color(255, 255, 255, 255);

		//constructor
		public DisplayObject() {
			_graphics.BoundsChanged += onGraphicsBoundsChanged;
			graphicsSprite.Texture = TextureUtil.FromBitmap(_graphics.Bitmap);
		}
		~DisplayObject() {
			if (_window != null) {
				_window.QuadTree.Remove(this);
			}
			_graphics.BoundsChanged -= onGraphicsBoundsChanged;
			graphicsSprite.Texture.Dispose();
			_graphics.Dispose();
		}

		//public
		public virtual void Update(double deltaTime) {
			applyGlobalBounds();
			OnUpdate(deltaTime);
			applyGlobalBounds();
		}
		public virtual void SwapBuffers() {
			OnSwapBuffers();
		}
		public virtual void Draw(RenderTarget target, Transform parentTransform) {
			if (!Visible) {
				return;
			}
			
			renderGraphics();

			if (_parent != null) {
				graphicsSprite.Color = _color * _parent.Color;
				renderSprite.Color = _color * _parent.Color;
			} else {
				graphicsSprite.Color = _color;
				renderSprite.Color = _color;
			}

			globalTransform = parentTransform * localTransform;
			renderState.Transform = globalTransform;
			target.Draw(graphicsSprite, renderState);
			target.Draw(renderSprite, renderState);
		}

		public virtual PreciseRectangle LocalBounds {
			get {
				return (PreciseRectangle) _localBounds.Clone();
			}
		}
		public PreciseRectangle GlobalBounds {
			get {
				return (PreciseRectangle) _globalBounds.Clone();
			}
		}
		public PreciseRectangle extureBounds {
			get {
				return (PreciseRectangle) _localTextureBounds.Clone();
			}
		}

		public DisplayObject Parent {
			get {
				return _parent;
			}
			internal set {
				_parent = value;
				if (value == null) {
					globalTransform = Transform.Identity;
				}
				applyGlobalBounds();
			}
		}
		public BaseWindow Window {
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
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}
				_localBounds.X = value;
				applyLocation();
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
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}
				_localBounds.Y = value;
				applyLocation();
				BoundsChanged?.Invoke(this, EventArgs.Empty);
			}
		}
		public double GlobalWidth {
			get {
				return _globalBounds.Width;
			}
		}
		public virtual double Width {
			get {
				return _localBounds.Width;
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}
				if (value == 0.0d) {
					value = 1.0d;
				} else if (value < 0.0d) {
					value *= -1;
				}
				_localBounds.Width = value;
				applyLocalBounds();
				BoundsChanged?.Invoke(this, EventArgs.Empty);
			}
		}
		public double GlobalHeight {
			get {
				return _globalBounds.Height;
			}
		}
		public virtual double Height {
			get {
				return _localBounds.Height;
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}
				if (value == 0.0d) {
					value = 1.0d;
				} else if (value < 0.0d) {
					value *= -1;
				}
				_localBounds.Height = value;
				applyLocalBounds();
				BoundsChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		public double TransformOriginX {
			get {
				return _transformOriginOffset.X;
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}
				_transformOriginOffset.X = value;
			}
		}
		public double TransformOriginY {
			get {
				return _transformOriginOffset.Y;
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}
				_transformOriginOffset.Y = value;
			}
		}

		public Texture Texture {
			get {
				return renderSprite.Texture;
			}
			set {
				if (value == null) {
					renderSprite.TextureRect = new IntRect(0, 0, 0, 0);
					renderSprite.Texture = null;
					_localTextureBounds.Width = 0.0d;
					_localTextureBounds.Height = 0.0d;
				} else {
					value.Smooth = _textureSmoothing;

					if (renderSprite.Texture == null || (_localTextureBounds.X == renderSprite.TextureRect.Left && _localTextureBounds.Y == renderSprite.TextureRect.Top && _localTextureBounds.Width == renderSprite.TextureRect.Width && _localTextureBounds.Height == renderSprite.TextureRect.Height)) {
						_localTextureBounds.Width = value.Size.X;
						_localTextureBounds.Height = value.Size.Y;
						applyTextureBounds();
						applyLocalBounds();
					}

					renderSprite.Texture = value;
					renderSprite.TextureRect = new IntRect((int) _localTextureBounds.X, (int) _localTextureBounds.Y, (int) _localTextureBounds.Width, (int) _localTextureBounds.Height);
				}
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
				renderSprite.Texture.Smooth = _textureSmoothing;
				graphicsSprite.Texture.Smooth = _textureSmoothing;
			}
		}
		public Texture GraphicsTexture {
			get {
				return graphicsSprite.Texture;
			}
		}
		public double TextureX {
			get {
				return _localTextureBounds.X;
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}
				_localTextureBounds.X = value;
				applyTextureBounds();
			}
		}
		public double TextureY {
			get {
				return _localTextureBounds.Y;
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}
				_localTextureBounds.Y = value;
				applyTextureBounds();
			}
		}
		public double TextureWidth {
			get {
				return _localTextureBounds.Width;
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}
				_localTextureBounds.Width = value;
				applyTextureBounds();
				applyLocalBounds();
			}
		}
		public double TextureHeight {
			get {
				return _localTextureBounds.Height;
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}
				_localTextureBounds.Height = value;
				applyTextureBounds();
				applyLocalBounds();
			}
		}

		public double ScaleX {
			get {
				return _scale.X;
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}
				_scale.X = value;
				applyScale();
			}
		}
		public double ScaleY {
			get {
				return _scale.Y;
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}
				_scale.Y = value;
				applyScale();
			}
		}
		public double Rotation {
			get {
				return _rotation;
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}
				_rotation = value;
				applyRotation();
			}
		}

		public Color Color {
			get {
				return _color;
			}
			set {
				_color = value;
			}
		}
		public double ColorR {
			get {
				return (double) (_color.R / 255.0d);
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}
				value = MathUtil.Clamp(0.0d, 255.0d, value);
				_color.R = (byte) (value * 255.0d);
			}
		}
		public double ColorG {
			get {
				return (double) (_color.G / 255.0d);
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}
				value = MathUtil.Clamp(0.0d, 255.0d, value);
				_color.G = (byte) (value * 255.0d);
			}
		}
		public double ColorB {
			get {
				return (double) (_color.B / 255.0d);
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}
				value = MathUtil.Clamp(0.0d, 255.0d, value);
				_color.B = (byte) (value * 255.0d);
			}
		}
		public double ColorA {
			get {
				return (double) (_color.A / 255.0d);
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}
				value = MathUtil.Clamp(0.0d, 255.0d, value);
				_color.A = (byte) (value * 255.0d);
			}
		}

		public BlendMode BlendMode {
			get {
				return renderState.BlendMode;
			}
			set {
				renderState.BlendMode = value;
			}
		}
		public Shader Shader {
			get {
				return renderState.Shader;
			}
			set {
				renderState.Shader = value;
			}
		}

		public SpriteGraphics Graphics {
			get {
				return _graphics;
			}
		}

		public PrecisePoint LocalToGlobal(PrecisePoint point) {
			return PrecisePoint.FromVector2f(globalTransform.TransformPoint((float) point.X, (float) point.Y));
		}
		public PreciseRectangle LocalToGlobal(PreciseRectangle rect) {
			return PreciseRectangle.FromFloatRect(globalTransform.TransformRect(new FloatRect((float) rect.X, (float) rect.Y, (float) rect.Width, (float) rect.Height)));
		}
		public PrecisePoint GlobalToLocal(PrecisePoint point) {
			return PrecisePoint.FromVector2f(localTransform.TransformPoint((float) point.X, (float) point.Y));
		}
		public PreciseRectangle GlobalToLocal(PreciseRectangle rect) {
			return PreciseRectangle.FromFloatRect(localTransform.TransformRect(new FloatRect((float) rect.X, (float) rect.Y, (float) rect.Width, (float) rect.Height)));
		}

		/*public SpriteSkew Skew {
			get {
				return _skew;
			}
		}*/

		//private
		protected abstract void OnUpdate(double deltaTime);
		virtual protected void OnSwapBuffers() {

		}

		private void applyLocation() {
			if (previousXY.X != _localBounds.X || previousXY.Y != _localBounds.Y) {
				localTransform.Translate((float) (_localBounds.X - previousXY.X), (float) (_localBounds.Y - previousXY.Y));
				previousXY.X = _localBounds.X;
				previousXY.Y = _localBounds.Y;
				//localBoundsCache.Left = (float) _localBounds.X;
				//localBoundsCache.Top = (float) _localBounds.Y;
				applyGlobalBounds();
			}
		}
		private void applyRotation() {
			if (previousRotation != _rotation) {
				localTransform.Rotate((float) (_rotation - previousRotation), (float) _transformOriginOffset.X, (float) _transformOriginOffset.Y);
				previousRotation = _rotation;
				applyGlobalBounds();
			}
		}
		private void applyScale() {
			if (previousScale.X != _scale.X || previousScale.Y != _scale.Y) {
				localTransform.Scale((float) (1.0d / previousScale.X), (float) (1.0d / previousScale.Y), (float) _transformOriginOffset.X, (float) _transformOriginOffset.Y);
				localTransform.Scale((float) _scale.X, (float) _scale.Y, (float) _transformOriginOffset.X, (float) _transformOriginOffset.Y);
				previousScale.X = _scale.X;
				previousScale.Y = _scale.Y;
				applyGlobalBounds();
			}
		}
		private void applySkew() {
			skewBox[0] = new Vertex(new Vector2f(0.0f, 0.0f), new Vector2f((float) _skew.TopLeftX, (float) _skew.TopLeftY));
			skewBox[1] = new Vertex(new Vector2f(0.0f, (float) _localBounds.Height), new Vector2f((float) _skew.BottomLeftX, (float) (_localBounds.Height + _skew.BottomLeftY)));
			skewBox[2] = new Vertex(new Vector2f((float) _localBounds.Height, (float) _localBounds.Width), new Vector2f((float) (_localBounds.Width + _skew.BottomRightX), (float) (_localBounds.Height + _skew.BottomRightY)));
			skewBox[3] = new Vertex(new Vector2f((float) _localBounds.Width, 0.0f), new Vector2f((float) (_localBounds.Width + _skew.TopRightX), (float) _skew.TopRightY));
			applyGlobalBounds();
		}
		private void applyTextureBounds() {
			renderSprite.TextureRect = new IntRect((int) _localTextureBounds.X, (int) _localTextureBounds.Y, (int) _localTextureBounds.Width, (int) _localTextureBounds.Height);
			applyGlobalBounds();
		}
		private void applyLocalBounds() {
			_localBounds.Width = (double) Math.Max(_localTextureBounds.Width, _graphics.Bitmap.Width);
			_localBounds.Height = (double) Math.Max(_localTextureBounds.Height, _graphics.Bitmap.Height);
			//localBoundsCache.Width = (float) _localBounds.Width;
			//localBoundsCache.Height = (float) _localBounds.Height;
			applyGlobalBounds();
		}

		private void applyGlobalBounds() {
			/*FloatRect r = globalTransform.TransformRect(localBoundsCache);
			_globalBounds.X = r.Left;
			_globalBounds.Y = r.Top;
			_globalBounds.Width = r.Width;
			_globalBounds.Height = r.Height;*/

			_globalBounds.X = (_parent != null) ? _parent.GlobalX + _localBounds.X : _localBounds.X;
			_globalBounds.Y = (_parent != null) ? _parent.GlobalY + _localBounds.Y : _localBounds.Y;
			//_globalBounds.X = (_parent != null) ? _parent.GlobalX + _localBounds.X - (_transformOriginOffset.X / 2.0d) + Math.Min(_skew.TopLeftX, _skew.BottomLeftX) : _localBounds.X - (_transformOriginOffset.X / 2.0d) + Math.Min(_skew.TopLeftX, _skew.BottomLeftX);
			//_globalBounds.Y = (_parent != null) ? _parent.GlobalY + _localBounds.Y - (_transformOriginOffset.Y / 2.0d) + Math.Min(_skew.TopLeftY, _skew.TopRightY) : _localBounds.Y - (_transformOriginOffset.Y / 2.0d) + Math.Min(_skew.TopLeftY, _skew.TopRightY);
			_globalBounds.Width = (_localTextureBounds.Width * _scale.X);
			_globalBounds.Height = (_localTextureBounds.Height * _scale.Y);
			//_globalBounds.Width = (_textureBounds.Width * _scale.X) + Math.Max(_skew.TopRightX, _skew.BottomRightX);
			//_globalBounds.Height = (_textureBounds.Height * _scale.Y) + Math.Max(_skew.BottomLeftY, _skew.BottomRightY);

			if (previousGlobalBounds.X != _globalBounds.X || previousGlobalBounds.Y != _globalBounds.Y) {
				previousGlobalBounds.X = _globalBounds.X;
				previousGlobalBounds.Y = _globalBounds.Y;
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

			graphicsSprite.Texture.Dispose();
			Texture tex = TextureUtil.FromBitmap(_graphics.Bitmap);
			tex.Smooth = _textureSmoothing;
			graphicsSprite.Texture = tex;
			graphicsSprite.TextureRect = new IntRect(0, 0, _graphics.Bitmap.Width, _graphics.Bitmap.Height);
			//TextureUtil.UpdateTextureWithBitmap(_graphics.Bitmap, graphicsSprite.Texture);
		}
		private void onGraphicsBoundsChanged(object sender, EventArgs e) {
			applyLocalBounds();
		}
		private void onSkewChanged(object sender, EventArgs e) {
			applySkew();
		}
	}
}
