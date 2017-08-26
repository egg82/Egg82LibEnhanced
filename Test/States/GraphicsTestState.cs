using Egg82LibEnhanced.Display;
using Egg82LibEnhanced.Patterns;
using Egg82LibEnhanced.Utils;
using System;
using System.Collections.Generic;
using Test.Sprites;

namespace Test.States {
	class GraphicsTestState : State {
		//vars
		private ObjectPool<CircleSprite> circleFactory = new ObjectPool<CircleSprite>(new CircleSprite(), 1000);
		private List<CircleSprite> circles = new List<CircleSprite>();

		private string atlasPath = FileUtil.CURRENT_DIRECTORY + FileUtil.DIRECTORY_SEPARATOR_CHAR + ".." + FileUtil.DIRECTORY_SEPARATOR_CHAR + ".." + FileUtil.DIRECTORY_SEPARATOR_CHAR + "Assets" + FileUtil.DIRECTORY_SEPARATOR_CHAR + "Images" + FileUtil.DIRECTORY_SEPARATOR_CHAR + "terrain.png";
		private TextureAtlas atlas = null;
		private TileMap background = null;

		//constructor
		public GraphicsTestState() {
			
		}

		//public

		//private
		protected override void OnEnter() {
			atlas = new TextureAtlas(TextureUtil.BitmapFromBytes(FileUtil.Read(atlasPath, 0)), 32, 32);
			background = new TileMap(ref atlas, 40, 23, 32, 32);

			for (int x = 0; x < 40; x++) {
				for (int y = 0; y < 23; y++) {
					background.SetTileName(x, y, MathUtil.FairRoundedRandom(0, 32 * 32).ToString());
				}
			}
			AddChild(background);

			while (circleFactory.NumFreeInstances > 0) {
				CircleSprite sprite = circleFactory.GetObject();
				sprite.TransformOffsetX = sprite.Width / 2.0d;
				sprite.TransformOffsetY = sprite.Height / 2.0d;
				sprite.Rotation = MathUtil.Random(-180.0d, 180.0d);
				sprite.ScaleX = sprite.ScaleY = MathUtil.Random(0.1d, 0.3d);
				sprite.X = MathUtil.Random(0.0d, Window.Width - sprite.Width);
				sprite.Y = MathUtil.Random(0.0d, Window.Height - sprite.Height);
				sprite.Color = new SFML.Graphics.Color(255, 255, 255, (byte) MathUtil.FairRoundedRandom(25, 255));
				circles.Add(sprite);
				AddChild(sprite);
			}
		}
		protected override void OnExit() {
			while (circles.Count > 0) {
				CircleSprite sprite = circles[0];
				circles.RemoveAt(0);
				RemoveChild(sprite);
				circleFactory.ReturnObject(sprite);
			}
			RemoveChild(background);
			atlas.Dispose();
		}
		protected override void OnUpdate(double deltaTime) {
			for (int i = 0; i < circles.Count; i++) {
				if (circles[i].X < 0.0d) {
					circles[i].X = 0.0d;
				} else if (circles[i].X + circles[i].Width > Window.Width) {
					circles[i].X = Window.Width - circles[i].Width;
				}
				if (circles[i].Y < 0.0d) {
					circles[i].Y = 0.0d;
				} else if (circles[i].Y + circles[i].Height > Window.Height) {
					circles[i].Y = Window.Height - circles[i].Height;
				}
			}
		}
	}
}
