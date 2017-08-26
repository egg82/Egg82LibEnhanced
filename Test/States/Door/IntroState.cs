using Egg82LibEnhanced.Display;
using Egg82LibEnhanced.Display.Interactable;
using Egg82LibEnhanced.Tweening;
using Egg82LibEnhanced.Utils;
using System;
using System.Drawing;

namespace Test.States.Door {
	class IntroState : State {
		//vars
		private TextBox introText = new TextBox(new Font(FontUtil.GetFont("main"), 14.0f, FontStyle.Bold));

		//constructor
		public IntroState() {

		}

		//public

		//private
		protected override void OnEnter() {
			introText.Text = "A long time ago in a galaxy far, far away..";
			introText.X = Window.Width / 2.0d - introText.Width / 2.0d;
			introText.Y = Window.Height / 2.0d - introText.Height / 2.0d;
			AddChild(introText);

			Tween.FromTo(introTextAlpha, 0, 255, 3000.0d).Complete += onIntroTextVisible1;
		}
		protected override void OnExit() {

		}
		protected override void OnUpdate(double deltaTime) {

		}

		private void introTextAlpha(double value) {
			introText.Color = new SFML.Graphics.Color(255, 255, 255, (byte) value);
		}
		private void onIntroTextVisible1(object sender, EventArgs e) {
			Tween.FromTo(introTextAlpha, 255, 0, 2500.0d).Complete += onIntroTextComplete1;
		}
		private void onIntroTextComplete1(object sender, EventArgs e) {
			introText.Text = "Wait, what was I saying?";
			introText.X = Window.Width / 2.0d - introText.Width / 2.0d;
			introText.Y = Window.Height / 2.0d - introText.Height / 2.0d;

			Tween.FromTo(introTextAlpha, 0, 255, 2500.0d).Complete += onIntroTextVisible2;
		}
		private void onIntroTextVisible2(object sender, EventArgs e) {
			Tween.FromTo(introTextAlpha, 255, 0, 3000.0d).Complete += onIntroTextComplete2;
		}
		private void onIntroTextComplete2(object sender, EventArgs e) {
			introText.Text = "Ah, forget it. Here's a game about doors and cooking.";
			introText.X = Window.Width / 2.0d - introText.Width / 2.0d;
			introText.Y = Window.Height / 2.0d - introText.Height / 2.0d;

			Tween.FromTo(introTextAlpha, 0, 255, 3000.0d).Complete += onIntroTextVisible3;
		}
		private void onIntroTextVisible3(object sender, EventArgs e) {
			Tween.FromTo(introTextAlpha, 255, 0, 3000.0d).Complete += onIntroTextComplete3;
		}
		private void onIntroTextComplete3(object sender, EventArgs e) {
			Window.SwapStates(this, new GameState());
		}
	}
}
