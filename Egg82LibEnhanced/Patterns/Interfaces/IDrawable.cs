using SFML.Graphics;
using System;

namespace Egg82LibEnhanced.Patterns.Interfaces {
	public interface IDrawable {
		//functions
		void Draw(RenderTarget target, Transform parentTransform);
	}
}
