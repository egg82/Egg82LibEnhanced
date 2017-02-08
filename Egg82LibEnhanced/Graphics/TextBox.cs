using Egg82LibEnhanced.Engines;
using Egg82LibEnhanced.Enums;
using Egg82LibEnhanced.Patterns;
using Egg82LibEnhanced.Utils;
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

		private Bitmap fontBitmap = new Bitmap(1, 1);
		private Font _font = null;
		private string _text = null;
		private bool _antiAliasing = true;
		private Color _color = System.Drawing.Color.White;

		private InteractableState _state = InteractableState.Normal;

		private IInputEngine inputEngine = ServiceLocator.GetService(typeof(IInputEngine));

		//constructor
		public TextBox(Font font, string text = null) {
			if (font == null) {
				throw new ArgumentNullException("font");
			}

			_font = font;
			_text = text;

			drawString();
		}
		~TextBox() {
			if (Texture != null) {
				Texture.Dispose();
			}
		}

		//public
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
			if (inputEngine.Mouse.X >= GlobalX && inputEngine.Mouse.X <= GlobalX + GlobalWidth && inputEngine.Mouse.Y >= GlobalY && inputEngine.Mouse.Y <= GlobalY + GlobalHeight) {
				if (inputEngine.Mouse.LeftButtonDown) {
					if (_state != InteractableState.Down) {
						_state = InteractableState.Down;
						if (Pressed != null) {
							Pressed.Invoke(this, EventArgs.Empty);
						}
					}
				} else {
					if (_state == InteractableState.Down) {
						if (Released != null) {
							Released.Invoke(this, EventArgs.Empty);
						}
					}
					if (_state != InteractableState.Hover) {
						_state = InteractableState.Hover;
						if (Entered != null) {
							Entered.Invoke(this, EventArgs.Empty);
						}
					}
				}
			} else {
				if (_state != InteractableState.Normal) {
					_state = InteractableState.Normal;
					if (Exited != null) {
						Exited.Invoke(this, EventArgs.Empty);
					}
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

			Texture = TextureUtil.FromBitmap(fontBitmap);
		}
	}
}
