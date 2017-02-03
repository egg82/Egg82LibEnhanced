using Egg82LibEnhanced.Patterns.Prototypes;
using Egg82LibEnhanced.Utils;
using SFML.Graphics;
using System;
using System.IO;

namespace Test.Sprites {
	class CircleSprite : Egg82LibEnhanced.Graphics.Sprite, IPrototype {
		//vars
		private double speed = MathUtil.Random(1.0d, 2.0d);

		//constructor
		public CircleSprite(Texture tex = null) {
			if (tex != null) {
				Texture = tex;
			} else {
				Texture = new Texture(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "Images" + Path.DirectorySeparatorChar + "ball.png");
				Texture.Smooth = true;
			}
		}

		//public
		public IPrototype Clone() {
			return new CircleSprite(Texture);
		}

		//private
		protected override void OnUpdate(double deltaTime) {
			if (MathUtil.Random(0.0d, 1.0d) >= 0.5d) {
				X += (float) (speed * deltaTime);
			} else {
				X -= (float) (speed * deltaTime);
			}
			if (MathUtil.Random(0.0d, 1.0d) >= 0.5d) {
				Y += (float) (speed * deltaTime);
			} else {
				Y -= (float) (speed * deltaTime);
			}
		}
	}
}
