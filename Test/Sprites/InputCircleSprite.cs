using Egg82LibEnhanced.Base;
using SFML.Graphics;
using System;
using System.IO;

namespace Test.Sprites {
	class InputCircleSprite : BaseSprite {
		//vars
		private double _speed = 1.0d;

		//constructor
		public InputCircleSprite() {
			Texture = new Texture(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "Images" + Path.DirectorySeparatorChar + "ball.png");
			Texture.Smooth = true;
			TransformOriginX = Width / 2.0d;
			TransformOriginY = Height / 2.0d;
		}

		//public
		public double Speed {
			get {
				return _speed;
			}
		}

		//private
		protected override void OnUpdate(double deltaTime) {
			
		}
	}
}
