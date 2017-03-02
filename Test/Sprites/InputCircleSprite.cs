using SFML.Graphics;
using System;
using System.IO;

namespace Test.Sprites {
	class InputCircleSprite : Egg82LibEnhanced.Display.Sprite {
		//vars
		private double _speed = 1.0d;

		//constructor
		public InputCircleSprite() {
			Texture = new Texture(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + ".."  + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + "Assets" + Path.DirectorySeparatorChar + "Images" + Path.DirectorySeparatorChar + "ball.png");
			TextureSmoothing = true;
			TransformOffsetX = Width / 2.0d;
			TransformOffsetY = Height / 2.0d;
		}

		//public
		public double Speed {
			get {
				return _speed;
			}
		}

		//private
		
	}
}
