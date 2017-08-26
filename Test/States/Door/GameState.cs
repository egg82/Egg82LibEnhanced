using Egg82LibEnhanced.Display;
using Egg82LibEnhanced.Display.Interactable;
using Egg82LibEnhanced.Patterns;
using Egg82LibEnhanced.Utils;
using System;
using System.Drawing;
using Test.Registries;

namespace Test.States.Door {
	class GameState : State {
		//vars
		private TextureAtlas doorAtlas = ServiceLocator.GetService<GameDoorRegistry>().GetRegister<TextureAtlas>("doors");
		private TextureAtlas foodAtlas = ServiceLocator.GetService<GameDoorRegistry>().GetRegister<TextureAtlas>("food");

		private string[] doorNames = new string[0];

		private Sprite currentDoor = new Sprite();
		private TextBox doorText = new TextBox(new Font(FontUtil.GetFont("main"), 16.0f, FontStyle.Bold));

		private string[] startText = new string[] {
			"Oh, well hello there fine young human.\nWould you please make a traditional\n{RECIPE} for this old door?\nI would be ever-so-grateful! *wink*"
		};
		private string[] successText = new string[] {
			"You spoil me so! *giggle*\nI bet you're pretty handy with OTHER tools as well?"
		};
		private string[] failText = new string[] {
			"Oh no, no, no! That's not right at all..\nI'm sorry, but I think this relationship may not work out for either of us."
		};

		//constructor
		public GameState() {

		}

		//public

		//private
		protected override void OnEnter() {
			doorNames = doorAtlas.GetNames();

			currentDoor.ScaleX = currentDoor.ScaleY = 5.0d;
			getNewLevel();

			currentDoor.X = Window.Width / 2.0d - currentDoor.Width / 2.0d;
			currentDoor.Y = Window.Height / 4.0d - currentDoor.Height / 2.0d;
			AddChild(currentDoor);

			StringFormat sf = new StringFormat();
			sf.Alignment = StringAlignment.Center;
			doorText.TextFormat = sf;
			AddChild(doorText);
		}
		protected override void OnExit() {
			
		}
		protected override void OnUpdate(double deltaTime) {
			
		}

		private void getNewLevel() {
			currentDoor.Texture = doorAtlas.GetTexture(doorNames[MathUtil.FairRoundedRandom(0, doorNames.Length - 1)]);
			getRandomStartText();
		}
		private void getRandomStartText() {
			doorText.Text = startText[MathUtil.FairRoundedRandom(0, startText.Length - 1)].Replace("{RECIPE}", getNewRecipe());
			doorText.X = Window.Width / 2.0d - doorText.Width / 2.0d;
			doorText.Y = Window.Height - Window.Height / 8.0d - doorText.Height / 2.0d;
		}
		private string getNewRecipe() {
			return "";
		}
	}
}
