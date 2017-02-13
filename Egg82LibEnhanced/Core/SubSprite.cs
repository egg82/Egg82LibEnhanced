using SFML.Graphics;
using System;
using System.Drawing;

namespace Egg82LibEnhanced.Core {
	public class SubSprite : Graphics.Sprite {
		//vars

		//constructor
		public SubSprite(Texture texture, Rectangle region, bool rotated) {
			Texture = texture;
			TextureX = region.X;
			TextureY = region.Y;
			TextureWidth = region.Width;
			TextureHeight = region.Height;
			if (rotated) {
				Rotation = -90.0d;
			}
		}

		//public

		//private

	}
}
