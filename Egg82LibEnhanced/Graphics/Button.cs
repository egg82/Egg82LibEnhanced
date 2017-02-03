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

		private Texture normalTexture = null;
		private Texture downTexture = null;
		private Texture hoverTexture = null;

		private InteractableState _state = InteractableState.Normal;

		//constructor
		public Button(Texture normalTexture, Texture downTexture = null, Texture hoverTexture = null) {
			if (normalTexture == null) {
				throw new ArgumentNullException("normalTexture");
			}

			this.normalTexture = normalTexture;
			this.downTexture = downTexture;
			this.hoverTexture = hoverTexture;

			Texture = normalTexture;
		}

		//public
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
						if (downTexture != null) {
							Texture = downTexture;
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
						if (hoverTexture != null) {
							Texture = hoverTexture;
						}
						_state = InteractableState.Hover;
						if (Entered != null) {
							Entered.Invoke(this, EventArgs.Empty);
						}
					}
				}
			} else {
				if (_state != InteractableState.Normal) {
					Texture = normalTexture;
					_state = InteractableState.Normal;
					if (Exited != null) {
						Exited.Invoke(this, EventArgs.Empty);
					}
				}
			}
		}
	}
}
