using Egg82LibEnhanced.Engines;
using Egg82LibEnhanced.Enums;
using Egg82LibEnhanced.Geom;
using Egg82LibEnhanced.Patterns;
using SFML.Window;
using System;

namespace Egg82LibEnhanced.Display.Interactable {
	public class Button : DisplayObject, IInteractable {
		//vars
		public event EventHandler Entered = null;
		public event EventHandler Exited = null;
		public event EventHandler Pressed = null;
		public event EventHandler Released = null;
		public event EventHandler ReleasedOutside = null;

		private IInputEngine inputEngine = ServiceLocator.GetService<IInputEngine>();

		private TextureAtlas atlas = null;
		private string _normalTexture = null;
		private string _downTexture = null;
		private string _hoverTexture = null;
		private PreciseRectangle _hitBox = new PreciseRectangle(0.0d, 0.0d, 1.0d, 1.0d);

		private InteractableState _state = InteractableState.Normal;

		//constructor
		public Button(ref TextureAtlas atlas, string normalTexture, string downTexture = null, string hoverTexture = null) {
			if (normalTexture == null) {
				throw new ArgumentNullException("normalTexture");
			}

			inputEngine.MouseDown += onMouseDown;
			inputEngine.MouseUp += onMouseUp;
			inputEngine.MouseMove += onMouseMove;

			this.atlas = atlas;
			_normalTexture = normalTexture;
			_downTexture = downTexture;
			_hoverTexture = hoverTexture;

			Texture = atlas.GetTexture(normalTexture);
			_hitBox.Width = Width;
			_hitBox.Height = Height;
		}
		~Button() {
			inputEngine.MouseDown -= onMouseDown;
			inputEngine.MouseUp -= onMouseUp;
			inputEngine.MouseMove -= onMouseMove;
		}

		//public
		public InteractableState State {
			get {
				return _state;
			}
		}

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

		public string NormalTexture {
			get {
				return _normalTexture;
			}
			set {
				if (value == null || value == _normalTexture) {
					return;
				}
				_normalTexture = value;
				if (_state == InteractableState.Normal) {
					/*bool update = false;
					if (_hitBox.X == X && _hitBox.Y == Y && _hitBox.Width == Width && _hitBox.Height == Height) {
						update = true;
					}*/
					Texture = atlas.GetTexture(_normalTexture);
					//if (update) {
						_hitBox.X = 0.0d;
						_hitBox.Y = 0.0d;
						_hitBox.Width = Width;
						_hitBox.Height = Height;
					//}
				}
			}
		}
		public string DownTexture {
			get {
				return _downTexture;
			}
			set {
				if (value == _downTexture) {
					return;
				}
				_downTexture = value;
				if (_state == InteractableState.Down) {
					/*bool update = false;
					if (_hitBox.X == X && _hitBox.Y == Y && _hitBox.Width == Width && _hitBox.Height == Height) {
						update = true;
					}*/
					Texture = atlas.GetTexture(_downTexture);
					//if (update) {
						_hitBox.X = 0.0d;
						_hitBox.Y = 0.0d;
						_hitBox.Width = Width;
						_hitBox.Height = Height;
					//}
				}
			}
		}
		public string HoverTexture {
			get {
				return _hoverTexture;
			}
			set {
				if (value == _hoverTexture) {
					return;
				}
				_hoverTexture = value;
				if (_state == InteractableState.Hover) {
					/*bool update = false;
					if (_hitBox.X == X && _hitBox.Y == Y && _hitBox.Width == Width && _hitBox.Height == Height) {
						update = true;
					}*/
					Texture = atlas.GetTexture(_hoverTexture);
					//if (update) {
						_hitBox.X = 0.0d;
						_hitBox.Y = 0.0d;
						_hitBox.Width = Width;
						_hitBox.Height = Height;
					//}
				}
			}
		}

		//private
		protected override void OnUpdate(double deltaTime) {
			
		}

		private void onMouseDown(object sender, MouseButtonEventArgs e) {
			if (e.Button == Mouse.Button.Left && e.X >= GlobalX + _hitBox.X && e.X <= GlobalX + _hitBox.X + _hitBox.Width && e.Y >= GlobalY + _hitBox.Y && e.Y <= GlobalY + _hitBox.Y + _hitBox.Height) {
				if (_state != InteractableState.Down) {
					if (_downTexture != null) {
						/*bool update = false;
						if (_hitBox.X == X && _hitBox.Y == Y && _hitBox.Width == Width && _hitBox.Height == Height) {
							update = true;
						}*/
						Texture = atlas.GetTexture(_downTexture);
						//if (update) {
							_hitBox.X = 0.0d;
							_hitBox.Y = 0.0d;
							_hitBox.Width = Width;
							_hitBox.Height = Height;
						//}
					}
					_state = InteractableState.Down;
					Pressed?.Invoke(this, EventArgs.Empty);
				}
			}
		}
		private void onMouseUp(object sender, MouseButtonEventArgs e) {
			if (e.Button == Mouse.Button.Left && _state == InteractableState.Down) {
				if (e.X >= GlobalX + _hitBox.X && e.X <= GlobalX + _hitBox.X + _hitBox.Width && e.Y >= GlobalY + _hitBox.Y && e.Y <= GlobalY + _hitBox.Y + _hitBox.Height) {
					/*bool update = false;
					if (_hitBox.X == X && _hitBox.Y == Y && _hitBox.Width == Width && _hitBox.Height == Height) {
						update = true;
					}*/
					Texture = (_hoverTexture != null) ? atlas.GetTexture(_hoverTexture) : atlas.GetTexture(_normalTexture);
					//if (update) {
						_hitBox.X = 0.0d;
						_hitBox.Y = 0.0d;
						_hitBox.Width = Width;
						_hitBox.Height = Height;
					//}
					_state = InteractableState.Hover;
					Released?.Invoke(this, EventArgs.Empty);
				} else {
					/*bool update = false;
					if (_hitBox.X == X && _hitBox.Y == Y && _hitBox.Width == Width && _hitBox.Height == Height) {
						update = true;
					}*/
					Texture = atlas.GetTexture(_normalTexture);
					//if (update) {
						_hitBox.X = 0.0d;
						_hitBox.Y = 0.0d;
						_hitBox.Width = Width;
						_hitBox.Height = Height;
					//}
					_state = InteractableState.Normal;
					ReleasedOutside?.Invoke(this, EventArgs.Empty);
				}
			}
		}
		private void onMouseMove(object sender, MouseMoveEventArgs e) {
			if (e.X >= GlobalX + _hitBox.X && e.X <= GlobalX + _hitBox.X + _hitBox.Width && e.Y >= GlobalY + _hitBox.Y && e.Y <= GlobalY + _hitBox.Y + _hitBox.Height) {
				if (_state == InteractableState.Normal) {
					if (_hoverTexture != null) {
						/*bool update = false;
						if (_hitBox.X == X && _hitBox.Y == Y && _hitBox.Width == Width && _hitBox.Height == Height) {
							update = true;
						}*/
						Texture = atlas.GetTexture(_hoverTexture);
						//if (update) {
							_hitBox.X = 0.0d;
							_hitBox.Y = 0.0d;
							_hitBox.Width = Width;
							_hitBox.Height = Height;
						//}
					}
					_state = InteractableState.Hover;
					Entered?.Invoke(this, EventArgs.Empty);
				}
			} else {
				if (_state == InteractableState.Hover) {
					/*bool update = false;
					if (_hitBox.X == X && _hitBox.Y == Y && _hitBox.Width == Width && _hitBox.Height == Height) {
						update = true;
					}*/
					Texture = atlas.GetTexture(_normalTexture);
					//if (update) {
						_hitBox.X = 0.0d;
						_hitBox.Y = 0.0d;
						_hitBox.Width = Width;
						_hitBox.Height = Height;
					//}
					_state = InteractableState.Normal;
					Exited?.Invoke(this, EventArgs.Empty);
				}
			}
		}
	}
}
