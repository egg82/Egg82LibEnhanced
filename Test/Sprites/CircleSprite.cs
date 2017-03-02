using Egg82LibEnhanced.Patterns.Prototypes;
using Egg82LibEnhanced.Utils;
using SFML.Graphics;
using System;
using System.IO;

namespace Test.Sprites {
	class CircleSprite : Egg82LibEnhanced.Display.Sprite, IPrototype {
		//vars
		private double speed = MathUtil.Random(1.0d, 2.0d);

		//constructor
		public CircleSprite(Texture tex = null) {
			if (tex != null) {
				Texture = tex;
			} else {
				Texture = new Texture(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + ".."  + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + "Assets" + Path.DirectorySeparatorChar + "Images" + Path.DirectorySeparatorChar + "ball.png");
			}
			TextureSmoothing = true;
		}

		//public
		public IPrototype Clone() {
			return new CircleSprite(Texture);
		}

		//private
		protected override void OnUpdate(double deltaTime) {
			X += (speed * deltaTime) * ((MathUtil.Random(0.0d, 1.0d) >= 0.5d) ? 1.0d : -1.0d);
			Y += (speed * deltaTime) * ((MathUtil.Random(0.0d, 1.0d) >= 0.5d) ? 1.0d : -1.0d);
		}
	}
}
