using Egg82LibEnhanced.Engines;
using Egg82LibEnhanced.Enums;
using Egg82LibEnhanced.Geom;
using Egg82LibEnhanced.Patterns;
using Egg82LibEnhanced.Utils;
using SFML.Window;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Egg82LibEnhanced.Graphics {
	public class TextBox : DisplayObject, IInteractable {
		//vars
		public event EventHandler Entered = null;
		public event EventHandler Exited = null;
		public event EventHandler Pressed = null;
		public event EventHandler Released = null;
		public event EventHandler ReleasedOutside = null;

		private IInputEngine inputEngine = ServiceLocator.GetService(typeof(IInputEngine));

		private Bitmap fontBitmap = new Bitmap(1, 1);
		private Font _font = null;
		private string _text = null;
		private bool _antiAliasing = true;
		private Color _color = System.Drawing.Color.White;
		private PreciseRectangle _hitBox = new PreciseRectangle(0.0d, 0.0d, 1.0d, 1.0d);

		private InteractableState _state = InteractableState.Normal;

		//constructor
		public TextBox(Font font, string text = null) {
			if (font == null) {
				throw new ArgumentNullException("font");
			}

			inputEngine.MouseDown += onMouseDown;
			inputEngine.MouseUp += onMouseUp;
			inputEngine.MouseMove += onMouseMove;

			_font = font;
			_text = text;

			drawString();
		}
		~TextBox() {
			inputEngine.MouseDown -= onMouseDown;
			inputEngine.MouseUp -= onMouseUp;
			inputEngine.MouseMove -= onMouseMove;

			if (Texture != null) {
				Texture.Dispose();
			}
		}

		//public
		public PreciseRectangle HitBox {
			get {
				return (PreciseRectangle) _hitBox.Clone();
			}
		}
		public double HitX {
			get {
				return _hitBox.X;
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}
				_hitBox.X = value;
			}
		}
		public double HitY {
			get {
				return _hitBox.Y;
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}
				_hitBox.Y = value;
			}
		}
		public double HitWidth {
			get {
				return _hitBox.Width;
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}
				_hitBox.Width = value;
			}
		}
		public double HitHeight {
			get {
				return _hitBox.Height;
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}
				_hitBox.Height = value;
			}
		}

		public bool AntiAliasing {
			get {
				return _antiAliasing;
			}
			set {
				if (value == _antiAliasing) {
					return;
				}
				_antiAliasing = value;
				drawString();
			}
		}

		public Font Font {
			get {
				return _font;
			}
			set {
				if (value == null || value == _font) {
					return;
				}
				_font = value;
				drawString();
			}
		}
		public Color TextColor {
			get {
				return _color;
			}
			set {
				if (value == null || value == _color) {
					return;
				}
				_color = value;
				drawString();
			}
		}
		public string Text {
			get {
				return _text;
			}
			set {
				if (value == _text) {
					return;
				}
				_text = value;
				drawString();
			}
		}

		public InteractableState State {
			get {
				return _state;
			}
		}

		//private
		protected override void OnUpdate(double deltaTime) {
			
		}

		private void onMouseDown(object sender, MouseButtonEventArgs e) {
			if (e.Button == Mouse.Button.Left && e.X >= GlobalX + _hitBox.X && e.X <= GlobalX + _hitBox.X + _hitBox.Width && e.Y >= GlobalY + _hitBox.Y && e.Y <= GlobalY + _hitBox.Y + _hitBox.Height) {
				if (_state != InteractableState.Down) {
					_state = InteractableState.Down;
					Pressed?.Invoke(this, EventArgs.Empty);
				}
			}
		}
		private void onMouseUp(object sender, MouseButtonEventArgs e) {
			if (e.Button == Mouse.Button.Left && _state == InteractableState.Down) {
				if (e.X >= GlobalX + _hitBox.X && e.X <= GlobalX + _hitBox.X + _hitBox.Width && e.Y >= GlobalY + _hitBox.Y && e.Y <= GlobalY + _hitBox.Y + _hitBox.Height) {
					_state = InteractableState.Hover;
					Released?.Invoke(this, EventArgs.Empty);
				} else {
					_state = InteractableState.Normal;
					ReleasedOutside?.Invoke(this, EventArgs.Empty);
				}
			}
		}
		private void onMouseMove(object sender, MouseMoveEventArgs e) {
			if (e.X >= GlobalX + _hitBox.X && e.X <= GlobalX + _hitBox.X + _hitBox.Width && e.Y >= GlobalY + _hitBox.Y && e.Y <= GlobalY + _hitBox.Y + _hitBox.Height) {
				if (_state == InteractableState.Normal) {
					_state = InteractableState.Hover;
					Entered?.Invoke(this, EventArgs.Empty);
				}
			} else {
				if (_state == InteractableState.Hover) {
					_state = InteractableState.Normal;
					Exited?.Invoke(this, EventArgs.Empty);
				}
			}
		}

		private void drawString() {
			if (Texture != null) {
				Texture.Dispose();
			}

			if (string.IsNullOrWhiteSpace(_text)) {
				using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(fontBitmap)) {
					g.Clear(System.Drawing.Color.Transparent);
				}
				Texture = TextureUtil.FromBitmap(fontBitmap);
				return;
			}

			SizeF size = new SizeF();
			using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(fontBitmap)) {
				g.TextRenderingHint = (_antiAliasing) ? TextRenderingHint.AntiAliasGridFit : TextRenderingHint.SingleBitPerPixel;
				g.SmoothingMode = (_antiAliasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
				size = g.MeasureString(_text, _font);
				if (size.Width == fontBitmap.Width && size.Height == fontBitmap.Height) {
					g.Clear(System.Drawing.Color.Transparent);
					g.DrawString(_text, _font, new Pen(_color).Brush, 0.0f, 0.0f);
				}
			}

			if (size.Width != fontBitmap.Width || size.Height != fontBitmap.Height) {
				fontBitmap.Dispose();
				fontBitmap = new Bitmap((int) size.Width, (int) size.Height);
				using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(fontBitmap)) {
					g.TextRenderingHint = (_antiAliasing) ? TextRenderingHint.AntiAliasGridFit : TextRenderingHint.SingleBitPerPixel;
					g.SmoothingMode = (_antiAliasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
					g.Clear(System.Drawing.Color.Transparent);
					g.DrawString(_text, _font, new Pen(_color).Brush, 0.0f, 0.0f);
				}
			}
			
			bool update = false;
			if (_hitBox.X == X && _hitBox.Y == Y && _hitBox.Width == Width && _hitBox.Height == Height) {
				update = true;
			}
			Texture = TextureUtil.FromBitmap(fontBitmap);
			if (update) {
				_hitBox.Width = Width;
				_hitBox.Height = Height;
			}
		}
	}
}
