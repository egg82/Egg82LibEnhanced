using Egg82LibEnhanced.Display;
using Egg82LibEnhanced.Engines;
using Egg82LibEnhanced.Enums;
using Egg82LibEnhanced.Geom;
using Egg82LibEnhanced.Patterns;
using System;
using Test.Sprites;
using static SFML.Window.Keyboard;

namespace Test.States {
	class InputTestState : State {
		//vars
		private InputCircleSprite sprite = new InputCircleSprite();

		private IInputEngine inputEngine = ServiceLocator.GetService(typeof(IInputEngine));
		private int[] leftKeys = new int[] { (int) Key.A, (int) Key.Left, (int) Key.Num4 };
		private int[] rightKeys = new int[] { (int) Key.D, (int) Key.Right, (int) Key.Num6 };

		//constructor
		public InputTestState() {
			
		}

		//public

		//private
		protected override void OnEnter() {
			sprite.X = Window.Width / 2.0d - sprite.Width / 2.0d;
			sprite.Y = Window.Height / 2.0d - sprite.Height / 2.0d;
			AddChild(sprite);
		}
		protected override void OnExit() {
			RemoveChild(sprite);
		}
		protected override void OnUpdate(double deltaTime) {
			if (inputEngine.Keyboard.IsAnyKeyDown(leftKeys)) {
				sprite.X -= sprite.Speed;
				sprite.Rotation -= sprite.Speed;
			}
			if (inputEngine.Keyboard.IsAnyKeyDown(rightKeys)) {
				sprite.X += sprite.Speed;
				sprite.Rotation += sprite.Speed;
			}

			if (inputEngine.Controllers.NumControllers > 0) {
				PrecisePoint left = inputEngine.Controllers.GetStickPosition(0, XboxStickSide.Left);
				if (left.Length > inputEngine.Controllers.StickDeadZone) {
					sprite.X += left.X * sprite.Speed;
					sprite.Rotation += left.X * sprite.Speed;
				}
			}
		}
	}
}
