using Egg82LibEnhanced.Core;
using Egg82LibEnhanced.Patterns.Interfaces;
using Egg82LibEnhanced.Utils;
using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;

namespace Egg82LibEnhanced.Base {
	public abstract class BaseSprite : IUpdatable, IDrawable, IQuad {
		//vars
		public bool Visible = true;
		
		private SpriteGraphics _graphics = new SpriteGraphics();
		private Sprite graphicsSprite = new Sprite();
		
		private List<BaseSprite> children = new List<BaseSprite>();
		private BaseSprite _parent = null;
		private BaseWindow _window = null;
		
		private RenderStates renderState = RenderStates.Default;
		private Sprite renderSprite = new Sprite();

		private PrecisePoint previousScale = new PrecisePoint(1.0d, 1.0d);
		private PrecisePoint _scale = new PrecisePoint(1.0d, 1.0d);
		private double previousRotation = 0.0d;
		private double _rotation = 0.0d;
		private PreciseRectangle previousBounds = new PreciseRectangle(0.0d, 0.0d, 1.0d, 1.0d);
		private PreciseRectangle _bounds = new PreciseRectangle(0.0d, 0.0d, 1.0d, 1.0d);
		private PreciseRectangle previousGlobalBounds = new PreciseRectangle(0.0d, 0.0d, 1.0d, 1.0d);
		private PreciseRectangle _globalBounds = new PreciseRectangle(0.0d, 0.0d, 1.0d, 1.0d);
		private PreciseRectangle previousTextureBounds = new PreciseRectangle(0.0d, 0.0d, 1.0d, 1.0d);
		private PreciseRectangle _textureBounds = new PreciseRectangle(0.0d, 0.0d, 1.0d, 1.0d);

		private PrecisePoint _transformOriginOffset = new PrecisePoint(0.0d, 0.0d);
		private Transform globalTransform = Transform.Identity;
		private Transform localTransform = Transform.Identity;

		private SpriteSkew _skew = new SpriteSkew();
		private Vertex[] skewBox = new Vertex[4];

		//constructor
		public BaseSprite() {
			_graphics.BoundsChanged += onGraphicsBoundsChanged;
			graphicsSprite.Texture = TextureUtil.FromBitmap(_graphics.Bitmap);
		}
		~BaseSprite() {
			if (_window != null) {
				_window.QuadTree.Remove(this);
			}
		}

		//public
		public void Update(double deltaTime) {
			for (int i = 0; i < children.Count; i++) {
				children[i].Update(deltaTime);
			}
			
			OnUpdate(deltaTime);

			if (previousGlobalBounds.X != _globalBounds.X || previousGlobalBounds.Y != _globalBounds.Y) {
				previousGlobalBounds.X = _globalBounds.X;
				previousGlobalBounds.Y = _globalBounds.Y;
				if (_window != null) {
					_window.QuadTree.Move(this);
				}
			}
		}
		public void SwapBuffers() {
			for (int i = 0; i < children.Count; i++) {
				children[i].SwapBuffers();
			}
			OnSwapBuffers();
		}
		public void Draw(RenderTarget target, Transform parentTransform) {
			if (!Visible) {
				return;
			}

			applyTransforms();
			renderGraphics();

			globalTransform = parentTransform * localTransform;
			renderState.Transform = globalTransform;
			target.Draw(graphicsSprite, renderState);
			target.Draw(renderSprite, renderState);

			for (int i = children.Count - 1; i >= 0; i--) {
				children[i].Draw(target, globalTransform);
			}
		}

		public PreciseRectangle LocalBounds {
			get {
				return (PreciseRectangle) _bounds.Clone();
			}
		}
		public PreciseRectangle GlobalBounds {
			get {
				return (PreciseRectangle) _globalBounds.Clone();
			}
		}

		public BaseSprite Parent {
			get {
				return _parent;
			}
			internal set {
				_parent = value;
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
		public double X {
			get {
				return _bounds.X;
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}
				_bounds.X = value;
			}
		}
		public double GlobalY {
			get {
				return _globalBounds.Y;
			}
		}
		public double Y {
			get {
				return _bounds.Y;
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}
				_bounds.Y = value;
			}
		}
		public double GlobalWidth {
			get {
				return _globalBounds.Width;
			}
		}
		public double Width {
			get {
				return _bounds.Width;
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
				_bounds.Width = value;
			}
		}
		public double GlobalHeight {
			get {
				return _globalBounds.Height;
			}
		}
		public double Height {
			get {
				return _bounds.Height;
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
				_bounds.Height = value;
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
				applyTransforms();
				_transformOriginOffset.X = value;
				graphicsSprite.Origin = new Vector2f((float) _transformOriginOffset.X, (float) _transformOriginOffset.Y);
				renderSprite.Origin = new Vector2f((float) _transformOriginOffset.X, (float) _transformOriginOffset.Y);
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
				applyTransforms();
				_transformOriginOffset.Y = value;
				graphicsSprite.Origin = new Vector2f((float) _transformOriginOffset.X, (float) _transformOriginOffset.Y);
				renderSprite.Origin = new Vector2f((float) _transformOriginOffset.X, (float) _transformOriginOffset.Y);
			}
		}

		public Texture Texture {
			get {
				return renderSprite.Texture;
			}
			set {
				if (value == null) {
					throw new ArgumentNullException("value");
				}
				if (renderSprite.Texture == null || (_textureBounds.X == 0.0d && _textureBounds.Y == 0.0d && _textureBounds.Width == renderSprite.TextureRect.Width && _textureBounds.Height == renderSprite.TextureRect.Height)) {
					_textureBounds.Width = value.Size.X;
					_textureBounds.Height = value.Size.Y;
				}
				
				renderSprite.Texture = value;
				renderSprite.TextureRect = new IntRect(0, 0, (int) _textureBounds.Width, (int) _textureBounds.Height);
				applyBounds();
				applyGlobalBounds();
			}
		}
		public double TextureBoundsX {
			get {
				return _textureBounds.X;
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}
				_textureBounds.X = value;
			}
		}
		public double TextureBoundsY {
			get {
				return _textureBounds.Y;
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}
				_textureBounds.Y = value;
			}
		}
		public double TextureBoundsWidth {
			get {
				return _textureBounds.Width;
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}
				_textureBounds.Width = value;
			}
		}
		public double TextureBoundsHeight {
			get {
				return _textureBounds.Height;
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}
				_textureBounds.Height = value;
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
			}
		}
		public Color Color {
			get {
				return renderSprite.Color;
			}
			set {
				graphicsSprite.Color = value;
				renderSprite.Color = value;
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

		public SpriteSkew Skew {
			get {
				return _skew;
			}
		}

		public void AddChild(BaseSprite sprite, int index = 0) {
			if (sprite == null) {
				throw new ArgumentNullException("sprite");
			}
			if (sprite.Parent != null) {
				throw new Exception("sprite cannot be added if it already has a parent.");
			}

			if (children.Contains(sprite)) {
				return;
			}
			if (index > children.Count) {
				index = children.Count;
			}
			if (index < 0) {
				index = 0;
			}

			children.Insert(index, sprite);
			sprite.Parent = this;
			sprite.Window = _window;
		}
		public void RemoveChild(BaseSprite sprite) {
			if (sprite == null) {
				throw new ArgumentNullException("sprite");
			}

			int index = children.IndexOf(sprite);
			if (index == -1) {
				throw new Exception("sprite is not a child of this object.");
			}

			children.RemoveAt(index);
			sprite.Parent = null;
			sprite.Window = null;
		}
		public BaseSprite GetChildAt(int index) {
			if (index < 0 || index >= children.Count) {
				return null;
			}
			return children[index];
		}
		public int IndexOf(BaseSprite child) {
			if (child == null) {
				throw new ArgumentNullException("child");
			}

			return children.IndexOf(child);
		}
		public void SetIndex(BaseSprite child, int index) {
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
		}

		//private
		protected abstract void OnUpdate(double deltaTime);
		virtual protected void OnSwapBuffers() {

		}
		
		private void applyTransforms() {
			bool globalBoundsChanged = false;

			if (previousBounds.X != _bounds.X || previousBounds.Y != _bounds.Y) {
				localTransform.Translate((float) (_bounds.X - previousBounds.X), (float) (_bounds.Y - previousBounds.Y));
				previousBounds.X = _bounds.X;
				previousBounds.Y = _bounds.Y;
				globalBoundsChanged = true;
			}
			if (_rotation != previousRotation) {
				previousRotation = _rotation;
				graphicsSprite.Rotation = (float) _rotation;
				renderSprite.Rotation = (float) _rotation;
			}
			applyScale();
			if (previousBounds.Width != _bounds.Width || previousBounds.Height != _bounds.Height) {
				previousBounds.Width = _bounds.Width;
				previousBounds.Height = _bounds.Height;
				globalBoundsChanged = true;
				//renderSprite.TextureRect = new IntRect(renderSprite.TextureRect.Left, renderSprite.TextureRect.Top, (int) _bounds.Width, (int) _bounds.Height);
			}
			if (_skew.Changed) {
				skewBox[0] = new Vertex(new Vector2f(0.0f, 0.0f), new Vector2f((float) _skew.TopLeftX, (float) _skew.TopLeftY));
				skewBox[1] = new Vertex(new Vector2f(0.0f, (float) _bounds.Height), new Vector2f((float) _skew.BottomLeftX, (float) (_bounds.Height + _skew.BottomLeftY)));
				skewBox[2] = new Vertex(new Vector2f((float) _bounds.Height, (float) _bounds.Width), new Vector2f((float) (_bounds.Width + _skew.BottomRightX), (float) (_bounds.Height + _skew.BottomRightY)));
				skewBox[3] = new Vertex(new Vector2f((float) _bounds.Width, 0.0f), new Vector2f((float) (_bounds.Width + _skew.TopRightX), (float) _skew.TopRightY));
				globalBoundsChanged = true;
				_skew.Changed = false;
			}
			if (previousTextureBounds.X != _textureBounds.X || previousTextureBounds.Y != _textureBounds.Y || previousTextureBounds.Width != _textureBounds.Width || previousTextureBounds.Height != _textureBounds.Height) {
				if (previousTextureBounds.Width != _textureBounds.Width || previousTextureBounds.Height != _textureBounds.Height) {
					globalBoundsChanged = true;
				}
				previousTextureBounds.X = _textureBounds.X;
				previousTextureBounds.Y = _textureBounds.Y;
				previousTextureBounds.Width = _textureBounds.Width;
				previousTextureBounds.Height = _textureBounds.Height;
				renderSprite.TextureRect = new IntRect(0, 0, (int) _textureBounds.Width, (int) _textureBounds.Height);
			}

			if (globalBoundsChanged) {
				applyGlobalBounds();
			}
		}
		private void applyScale() {
			if (_scale.X != previousScale.X || _scale.Y != previousScale.Y) {
				previousScale.X = _scale.X;
				previousScale.Y = _scale.Y;
				applyGlobalBounds();
				graphicsSprite.Scale = new Vector2f((float) _scale.X, (float) _scale.Y);
				renderSprite.Scale = new Vector2f((float) _scale.X, (float) _scale.Y);
			}
		}
		private void applyBounds() {
			_bounds.Width = (double) Math.Max(renderSprite.TextureRect.Width, _graphics.Bitmap.Width);
			_bounds.Height = (double) Math.Max(renderSprite.TextureRect.Height, _graphics.Bitmap.Height);
		}
		private void applyGlobalBounds() {
			_globalBounds.X = (_parent != null) ? _parent.GlobalX + _bounds.X + Math.Min(_skew.TopLeftX, _skew.BottomLeftX) : _bounds.X + Math.Min(_skew.TopLeftX, _skew.BottomLeftX);
			_globalBounds.Y = (_parent != null) ? _parent.GlobalY + _bounds.Y + Math.Min(_skew.TopLeftY, _skew.TopRightY) : _bounds.Y + Math.Min(_skew.TopLeftY, _skew.TopRightY);
			_globalBounds.Width = (_textureBounds.Width * _scale.X) + Math.Max(_skew.TopRightX, _skew.BottomRightX);
			_globalBounds.Height = (_textureBounds.Height * _scale.Y) + Math.Max(_skew.BottomLeftY, _skew.BottomRightY);
		}

		private void renderGraphics() {
			if (!_graphics.Changed) {
				return;
			}

			graphicsSprite.Texture.Dispose();
			graphicsSprite.Texture = TextureUtil.FromBitmap(_graphics.Bitmap);
			graphicsSprite.TextureRect = new IntRect(0, 0, _graphics.Bitmap.Width, _graphics.Bitmap.Height);
			//TextureUtil.UpdateTextureWithBitmap(_graphics.Bitmap, graphicsSprite.Texture);

			_graphics.Changed = false;
		}
		private void onGraphicsBoundsChanged(object sender, EventArgs e) {
			applyBounds();
			applyGlobalBounds();
		}
	}
}
