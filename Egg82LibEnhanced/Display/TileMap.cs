using Egg82LibEnhanced.Utils;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Egg82LibEnhanced.Display {
	public class TileMap : DisplayObject {
		//vars
		private Bitmap currentBitmap = new Bitmap(1, 1);
		private TextureAtlas atlas = null;
		private Bitmap[,] bitmaps = null;
		private string[,] bitmapNames = null;
		private int numRows = 0;
		private int numColumns = 0;
		private int tileWidth = 0;
		private int tileHeight = 0;
		private bool tilesChanged = false;

		//constructor
		public TileMap(ref TextureAtlas atlas, int numColumns, int numRows, int tileWidth, int tileHeight) {
			if (atlas == null) {
				throw new ArgumentNullException("atlas");
			}
			if (numRows <= 0) {
				throw new Exception("numRows cannot be less than or equal to zero.");
			}
			if (numColumns <= 0) {
				throw new Exception("numColumns cannot be less than or equal to zero.");
			}
			if (tileWidth <= 0) {
				throw new Exception("tileWidth cannot be less than or equal to zero.");
			}
			if (tileHeight <= 0) {
				throw new Exception("tileHeight cannot be less than or equal to zero.");
			}

			this.atlas = atlas;
			bitmaps = new Bitmap[numColumns, numRows];
			bitmapNames = new string[numColumns, numRows];
			this.numRows = numRows;
			this.numColumns = numColumns;
			this.tileWidth = tileWidth;
			this.tileHeight = tileHeight;

			using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(currentBitmap)) {
				g.Clear(System.Drawing.Color.Transparent);
			}
			Texture = TextureUtil.FromBitmap(currentBitmap);
		}

		//public
		public string GetTileName(int x, int y) {
			if (x < 0) {
				throw new Exception("x cannot be less than zero.");
			} else if (x > numColumns) {
				throw new Exception("x cannot be greater than numColumns.");
			}
			if (y < 0) {
				throw new Exception("y cannot be less than zero.");
			} else if (y > numRows) {
				throw new Exception("y cannot be greater than numRows.");
			}

			return bitmapNames[x, y];
		}
		public void SetTileName(int x, int y, string name) {
			if (x < 0) {
				throw new Exception("x cannot be less than zero.");
			} else if (x > numColumns) {
				throw new Exception("x cannot be greater than numColumns.");
			}
			if (y < 0) {
				throw new Exception("y cannot be less than zero.");
			} else if (y > numRows) {
				throw new Exception("y cannot be greater than numRows.");
			}

			if (name != null && atlas.HasValue(name)) {
				if (bitmapNames[x, y] != name) {
					bitmaps[x, y] = atlas.GetBitmap(name);
					bitmapNames[x, y] = name;
					tilesChanged = true;
				}
			} else {
				if (string.IsNullOrEmpty(bitmapNames[x, y])) {
					bitmaps[x, y] = null;
					bitmapNames[x, y] = null;
					tilesChanged = true;
				}
			}
		}

		//private
		protected override void OnUpdate(double deltaTime) {
			if (!tilesChanged) {
				return;
			}
			tilesChanged = false;

			currentBitmap.Dispose();
			Texture.Dispose();

			currentBitmap = new Bitmap(tileWidth * numColumns, tileHeight * numRows);
			using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(currentBitmap)) {
				g.SmoothingMode = SmoothingMode.AntiAlias;
				g.Clear(System.Drawing.Color.Transparent);
				for (int x = 0; x < numColumns; x++) {
					for (int y = 0; y < numRows; y++) {
						if (!string.IsNullOrEmpty(bitmapNames[x, y])) {
							g.DrawImage(bitmaps[x, y], new Point(x * tileWidth, y * tileHeight));
						}
					}
				}
			}

			Texture = TextureUtil.FromBitmap(currentBitmap);
		}
	}
}
