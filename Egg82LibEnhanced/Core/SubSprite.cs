using SFML.Graphics;
using System;
using System.Drawing;

namespace Egg82LibEnhanced.Core {
	public class SubSprite : Graphics.Sprite {
		//vars

		//constructor
		public SubSprite(Texture texture, Rectangle region, bool rotated) {
			Texture = texture;
			TextureBoundsX = region.X;
			TextureBoundsY = region.Y;
			TextureBoundsWidth = region.Width;
			TextureBoundsHeight = region.Height;
			if (rotated) {
				Rotation = -90.0d;
			}
		}

		//public

		//private

	}
}
