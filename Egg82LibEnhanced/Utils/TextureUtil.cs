using SFML.Graphics;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Egg82LibEnhanced.Utils {
	public class TextureUtil {
		//vars

		//constructor
		public TextureUtil() {

		}

		//public
		public static Texture FromBitmap(Bitmap bitmap) {
			if (bitmap == null) {
				throw new ArgumentNullException("bitmap");
			}
			using (MemoryStream stream = new MemoryStream()) {
				bitmap.Save(stream, ImageFormat.Png);
				return new Texture(stream);
			}
		}
		public static Bitmap[,] SplitBitmap(Bitmap bitmap, int rows, int columns) {
			if (bitmap == null) {
				throw new ArgumentNullException("bitmap");
			}
			if (rows <= 0) {
				throw new Exception("rows cannot be less than or equal to zero.");
			}
			if (columns <= 0) {
				throw new Exception("columns cannot be less than or equal to zero.");
			}

			int width = (int) (bitmap.Width / columns);
			int height = (int) (bitmap.Height / rows);
			Bitmap[,] retVal = new Bitmap[columns, rows];

			Rectangle dest = new Rectangle(0, 0, width, height);
			for (int x = 0; x < columns; x++) {
				for (int y = 0; y < rows; y++) {
					Rectangle region = new Rectangle(x * width, y * height, width, height);
					retVal[x, y] = new Bitmap(width, height);
					using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(retVal[x, y])) {
						g.Clear(System.Drawing.Color.Transparent);
						g.SmoothingMode = SmoothingMode.AntiAlias;
						g.DrawImage(bitmap, dest, region, GraphicsUnit.Pixel);
					}
				}
			}

			return retVal;
		}
		public static Bitmap GetRegion(Bitmap bitmap, Rectangle region, double rotation = 0.0d) {
			if (bitmap == null) {
				throw new ArgumentNullException("bitmap");
			}
			if (region.X < 0) {
				throw new Exception("region X cannot be less than zero.");
			}
			if (region.Y < 0) {
				throw new Exception("region Y cannot be less than zero.");
			}
			if (region.Width <= 0) {
				throw new Exception("region Width cannot be less than or equal to zero.");
			} else if (region.Width > bitmap.Width) {
				throw new Exception("region Width cannot be greater than bitmap Width.");
			}
			if (region.Height <= 0) {
				throw new Exception("region Height cannot be less than or equal to zero.");
			} else if (region.Height > bitmap.Height) {
				throw new Exception("region Height cannot be greater than bitmap Height.");
			}

			Rectangle dest = new Rectangle(0, 0, region.Width, region.Height);
			Bitmap retVal = new Bitmap(region.Width, region.Height);
			using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(retVal)) {
				g.Clear(System.Drawing.Color.Transparent);
				g.SmoothingMode = SmoothingMode.AntiAlias;
				g.DrawImage(bitmap, dest, region, GraphicsUnit.Pixel);
				if (rotation != 0.0d) {
					g.RotateTransform((float) rotation);
				}
			}
			return retVal;
		}
		public static Bitmap BitmapFromBytes(byte[] bytes) {
			using (MemoryStream stream = new MemoryStream(bytes)) {
				return new Bitmap(stream);
			}
		}

		public static Texture FromBytes(byte[] image, int width, int height) {
			if (image == null) {
				throw new ArgumentNullException("image");
			}
			if (width <= 0 || height <= 0) {
				return null;
			}
			Texture tex = new Texture((uint) width, (uint) height);
			tex.Update(image);
			return tex;
		}

		public static Bitmap BitmapFromTexture(Texture tex) {
			SFML.Graphics.Image im = tex.CopyToImage();
			byte[] pixels = im.Pixels;
			im.Dispose();
			return BitmapFromBytes(pixels);
		}

		/*public static void UpdateTextureWithBitmap(Bitmap bitmap, Texture tex) {
			if (bitmap == null) {
				throw new ArgumentNullException("bitmap");
			}
			if (tex == null) {
				throw new ArgumentNullException("tex");
			}

			SFML.Graphics.Color[,] colors = new SFML.Graphics.Color[bitmap.Width, bitmap.Height];
			SFML.Graphics.Image newImage = null;

			for (int x = 0; x < bitmap.Width; x++) {
				for (int y = 0; y < bitmap.Height; y++) {
					System.Drawing.Color c = bitmap.GetPixel(x, y);
					colors[y, x] = new SFML.Graphics.Color(c.R, c.G, c.B, c.A);
				}
			}
			newImage = new SFML.Graphics.Image(colors);
			tex.Update(newImage);
		}*/

		//private

	}
}
