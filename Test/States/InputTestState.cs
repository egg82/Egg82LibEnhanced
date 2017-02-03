using Egg82LibEnhanced.Base;
using Egg82LibEnhanced.Engines;
using Egg82LibEnhanced.Enums.Engines;
using Egg82LibEnhanced.Patterns;
using Egg82LibEnhanced.Utils;
using System;
using Test.Sprites;
using static SFML.Window.Keyboard;

namespace Test.States {
	class InputTestState : BaseState {
		//vars
		private InputCircleSprite sprite = new InputCircleSprite();

		private IInputEngine inputEngine = ServiceLocator.GetService(typeof(IInputEngine));
		private int[] leftKeys = new int[] { (int) Key.A, (int) Key.Left, (int) Key.Num4 };
		private int[] rightKeys = new int[] { (int) Key.D, (int) Key.Right, (int) Key.Num6 };

		//constructor
		public InputTestState() {

		}

		//public
		public override void OnEnter() {
			sprite.X = Window.Width / 2.0d;
			sprite.Y = Window.Height / 2.0d;
			AddChild(sprite);
		}
		public override void OnExit() {

		}

		//private
		protected override void OnUpdate(double deltaTime) {
			if (inputEngine.Keyboard.IsAnyKeyDown(leftKeys)) {
				sprite.X -= sprite.Speed;
				sprite.Rotation -= sprite.Speed;
			}
			if (inputEngine.Keyboard.IsAnyKeyDown(rightKeys)) {
				sprite.X += sprite.Speed;
				sprite.Rotation += sprite.Speed;
			}

			PrecisePoint stick = inputEngine.Controllers.GetStickPosition(0, XboxStickCode.Left);
			if (stick.Length > inputEngine.Controllers.DeadZone) {
				sprite.X += stick.X * sprite.Speed;
				sprite.Rotation += stick.X * sprite.Speed;
			}
		}
	}
}
