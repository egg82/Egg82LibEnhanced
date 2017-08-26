using Egg82LibEnhanced.Display;
using Egg82LibEnhanced.Display.Interactable;
using Egg82LibEnhanced.Engines;
using Egg82LibEnhanced.Enums;
using Egg82LibEnhanced.Geom;
using Egg82LibEnhanced.Patterns;
using Egg82LibEnhanced.Utils;
using System;
using System.Drawing;
using Test.Sprites;
using static SFML.Window.Keyboard;

namespace Test.States {
	class InputTestState : State {
		//vars
		private InputCircleSprite sprite = new InputCircleSprite();

		private IInputEngine inputEngine = ServiceLocator.GetService<IInputEngine>();
		private int[] leftKeys = new int[] { (int) Key.A, (int) Key.Left, (int) Key.Num4 };
		private int[] rightKeys = new int[] { (int) Key.D, (int) Key.Right, (int) Key.Num6 };

		private string atlasPath = FileUtil.CURRENT_DIRECTORY + FileUtil.DIRECTORY_SEPARATOR_CHAR + ".." + FileUtil.DIRECTORY_SEPARATOR_CHAR + ".." + FileUtil.DIRECTORY_SEPARATOR_CHAR + "Assets" + FileUtil.DIRECTORY_SEPARATOR_CHAR + "Images" + FileUtil.DIRECTORY_SEPARATOR_CHAR + "terrain.png";
		private TextureAtlas atlas = null;

		private Button b = null;
		private TextBox t = null;

		//constructor
		public InputTestState() {
			
		}

		//public

		//private
		protected override void OnEnter() {
			atlas = new TextureAtlas(TextureUtil.BitmapFromBytes(FileUtil.Read(atlasPath, 0)), 32, 32);
			
			b = new Button(ref atlas, atlas.GetNames()[MathUtil.FairRoundedRandom(0, atlas.GetNames().Length - 1)], atlas.GetNames()[MathUtil.FairRoundedRandom(0, atlas.GetNames().Length - 1)], atlas.GetNames()[MathUtil.FairRoundedRandom(0, atlas.GetNames().Length - 1)]);
			b.Entered += onInteractableEntered;
			b.Exited += onInteractableExited;
			b.Pressed += onInteractablePressed;
			b.Released += onInteractableReleased;
			b.ReleasedOutside += onInteractableReleasedOutside;
			AddChild(b);

			t = new TextBox(new Font("Times New Roman", 18.0f, FontStyle.Bold), "Hello, World!");
			t.X = Window.Width / 2.0d - t.Width / 2.0d;
			t.Entered += onInteractableEntered;
			t.Exited += onInteractableExited;
			t.Pressed += onInteractablePressed;
			t.Released += onInteractableReleased;
			t.ReleasedOutside += onInteractableReleasedOutside;
			AddChild(t);

			sprite.X = Window.Width / 2.0d - sprite.Width / 2.0d;
			sprite.Y = Window.Height / 2.0d - sprite.Height / 2.0d;
			AddChild(sprite);
		}
		protected override void OnExit() {
			RemoveChild(sprite);
		}
		protected override void OnUpdate(double deltaTime) {
			if (inputEngine.Keyboard.IsAnyKeyDown(leftKeys)) {
				sprite.X -= sprite.Speed * deltaTime;
				sprite.Rotation -= sprite.Speed * deltaTime;
			}
			if (inputEngine.Keyboard.IsAnyKeyDown(rightKeys)) {
				sprite.X += sprite.Speed * deltaTime;
				sprite.Rotation += sprite.Speed * deltaTime;
			}

			if (inputEngine.Controllers.NumControllers > 0) {
				PrecisePoint left = inputEngine.Controllers.GetStickPosition(0, XboxStickSide.Left);
				if (left.Length > inputEngine.Controllers.StickDeadZone) {
					sprite.X += left.X * sprite.Speed * deltaTime;
					sprite.Rotation += left.X * sprite.Speed * deltaTime;
				}
			}
		}

		private void onInteractableEntered(object sender, EventArgs e) {
			Console.WriteLine(sender.GetType().Name + " entered");
		}
		private void onInteractableExited(object sender, EventArgs e) {
			Console.WriteLine(sender.GetType().Name + " exited");
		}
		private void onInteractablePressed(object sender, EventArgs e) {
			Console.WriteLine(sender.GetType().Name + " pressed");
		}
		private void onInteractableReleased(object sender, EventArgs e) {
			Console.WriteLine(sender.GetType().Name + " released");
		}
		private void onInteractableReleasedOutside(object sender, EventArgs e) {
			Console.WriteLine(sender.GetType().Name + " released outside");
		}
	}
}
