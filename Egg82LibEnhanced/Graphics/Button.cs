using Egg82LibEnhanced.Engines;
using Egg82LibEnhanced.Enums;
using Egg82LibEnhanced.Patterns;
using SFML.Graphics;
using System;

namespace Egg82LibEnhanced.Graphics {
	public class Button : DisplayObject, IInteractable {
		//vars
		public event EventHandler Entered = null;
		public event EventHandler Exited = null;
		public event EventHandler Pressed = null;
		public event EventHandler Released = null;
		
		private IInputEngine inputEngine = ServiceLocator.GetService(typeof(IInputEngine));

		private TextureAtlas atlas = null;
		private string _normalTexture = null;
		private string _downTexture = null;
		private string _hoverTexture = null;

		private InteractableState _state = InteractableState.Normal;

		//constructor
		public Button(ref TextureAtlas atlas, string normalTexture, string downTexture = null, string hoverTexture = null) {
			if (normalTexture == null) {
				throw new ArgumentNullException("normalTexture");
			}

			this.atlas = atlas;
			_normalTexture = normalTexture;
			_downTexture = downTexture;
			_hoverTexture = hoverTexture;

			Texture = atlas.GetTexture(normalTexture);
		}

		//public
		public InteractableState State {
			get {
				return _state;
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
					Texture = atlas.GetTexture(_normalTexture);
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
					Texture = atlas.GetTexture(_downTexture);
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
					Texture = atlas.GetTexture(_hoverTexture);
				}
			}
		}

		//private
		protected override void OnUpdate(double deltaTime) {
			if (inputEngine.Mouse.X >= GlobalX && inputEngine.Mouse.X <= GlobalX + GlobalWidth && inputEngine.Mouse.Y >= GlobalY && inputEngine.Mouse.Y <= GlobalY + GlobalHeight) {
				if (inputEngine.Mouse.LeftButtonDown) {
					if (_state != InteractableState.Down) {
						if (_downTexture != null) {
							Texture = atlas.GetTexture(_downTexture);
						}
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
						if (_hoverTexture != null) {
							Texture = atlas.GetTexture(_hoverTexture);
						}
						_state = InteractableState.Hover;
						if (Entered != null) {
							Entered.Invoke(this, EventArgs.Empty);
						}
					}
				}
			} else {
				if (_state != InteractableState.Normal) {
					Texture = atlas.GetTexture(_normalTexture);
					_state = InteractableState.Normal;
					if (Exited != null) {
						Exited.Invoke(this, EventArgs.Empty);
					}
				}
			}
		}
	}
}
