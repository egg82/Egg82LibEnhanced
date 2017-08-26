using Egg82LibEnhanced.Engines;
using Egg82LibEnhanced.Enums;
using Egg82LibEnhanced.Patterns;
using Egg82LibEnhanced.Utils;
using System;
using System.Drawing;
using SFML.Window;
using Egg82LibEnhanced.Geom;
using System.Drawing.Drawing2D;

namespace Egg82LibEnhanced.Display.Interactable {
	public class Slider : DisplayObject, IInteractable {
		//vars
		public event EventHandler ValueChanged = null;
		public event EventHandler Entered = null;
		public event EventHandler Exited = null;
		public event EventHandler Pressed = null;
		public event EventHandler Released = null;
		public event EventHandler ReleasedOutside = null;

		private IInputEngine inputEngine = ServiceLocator.GetService<IInputEngine>();

		private InteractableState _state = InteractableState.Normal;
		private PreciseRectangle _hitBox = new PreciseRectangle(0.0d, 0.0d, 1.0d, 1.0d);

		private bool needsUpdate = false;
		private Bitmap barBitmap = null;
		private Bitmap tickBitmap = null;
		private Bitmap sliderBitmap = null;
		private double tickX = 0.0d;

		//constructor
		public Slider(ref TextureAtlas atlas, string barTexture, string tickTexture) {
			if (atlas == null) {
				throw new ArgumentNullException("atlas");
			}
			if (barTexture == null) {
				throw new ArgumentNullException("barTexture");
			}
			if (tickTexture == null) {
				throw new ArgumentNullException("tickTexture");
			}

			inputEngine.MouseDown += onMouseDown;
			inputEngine.MouseUp += onMouseUp;
			inputEngine.MouseMove += onMouseMove;

			barBitmap = atlas.GetBitmap(barTexture);
			tickBitmap = atlas.GetBitmap(tickTexture);
			sliderBitmap = new Bitmap(barBitmap.Width + tickBitmap.Width, Math.Max(barBitmap.Height, tickBitmap.Height));
			drawSlider();
		}
		~Slider() {
			inputEngine.MouseDown -= onMouseDown;
			inputEngine.MouseUp -= onMouseUp;
			inputEngine.MouseMove -= onMouseMove;

			if (Texture != null) {
				Texture.Dispose();
			}
		}

		//public
		public InteractableState State {
			get {
				return _state;
			}
		}

		public double Value {
			get {
				return MathUtil.Clamp(0.0d, 1.0d, (tickX + tickBitmap.Width / 2.0d) / barBitmap.Width);
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}

				tickX = MathUtil.Clamp(0.0d, barBitmap.Width, value * barBitmap.Width) - tickBitmap.Width / 2.0d;
				needsUpdate = true;
			}
		}
		
		//private
		protected override void OnUpdate(double deltaTime) {
			if (needsUpdate) {
				drawSlider();
				needsUpdate = false;
			}
		}

		private void drawSlider() {
			using (Graphics g = System.Drawing.Graphics.FromImage(sliderBitmap)) {
				g.SmoothingMode = SmoothingMode.AntiAlias;
				g.Clear(System.Drawing.Color.Transparent);
				g.DrawImage(barBitmap, new Point(tickBitmap.Width / 2, 3));
				g.DrawImage(tickBitmap, new Point((int) (tickX - tickBitmap.Width / 2.0d), 0));
			}
		}

		private void onMouseDown(object sender, MouseButtonEventArgs e) {
			if (e.Button == Mouse.Button.Left && inputEngine.Mouse.CurrentWindow == Window && e.X >= GlobalX + _hitBox.X && e.X <= GlobalX + _hitBox.X + _hitBox.Width && e.Y >= GlobalY + _hitBox.Y && e.Y <= GlobalY + _hitBox.Y + _hitBox.Height) {
				if (_state != InteractableState.Down) {
					_state = InteractableState.Down;
					Pressed?.Invoke(this, EventArgs.Empty);
				}
			}
		}
		private void onMouseUp(object sender, MouseButtonEventArgs e) {
			if (e.Button == Mouse.Button.Left && _state == InteractableState.Down) {
				if (inputEngine.Mouse.CurrentWindow == Window && e.X >= GlobalX + _hitBox.X && e.X <= GlobalX + _hitBox.X + _hitBox.Width && e.Y >= GlobalY + _hitBox.Y && e.Y <= GlobalY + _hitBox.Y + _hitBox.Height) {
					_state = InteractableState.Hover;
					Released?.Invoke(this, EventArgs.Empty);
				} else {
					_state = InteractableState.Normal;
					ReleasedOutside?.Invoke(this, EventArgs.Empty);
				}
			}
		}
		private void onMouseMove(object sender, MouseMoveEventArgs e) {
			if (inputEngine.Mouse.CurrentWindow == Window && e.X >= GlobalX + _hitBox.X && e.X <= GlobalX + _hitBox.X + _hitBox.Width && e.Y >= GlobalY + _hitBox.Y && e.Y <= GlobalY + _hitBox.Y + _hitBox.Height) {
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

			if (_state == InteractableState.Down) {
				tickX = MathUtil.Clamp(GlobalX, GlobalX + barBitmap.Width, inputEngine.Mouse.X) - GlobalX - tickBitmap.Width / 2.0d;
				needsUpdate = true;
				ValueChanged?.Invoke(this, EventArgs.Empty);
			}
		}
	}
}
