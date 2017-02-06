using Egg82LibEnhanced.Utils;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml;

namespace Egg82LibEnhanced.Graphics {
	public class TextureAtlas : IDisposable {
		//vars
		private bool fromTexture = false;
		private Bitmap atlasBitmap = null;
		private Dictionary<string, Bitmap> subBitmaps = new Dictionary<string, Bitmap>();
		private Dictionary<string, Texture> subTextures = new Dictionary<string, Texture>();

		//constructor
		public TextureAtlas(Texture texture, XmlDocument atlasXml) : this(TextureUtil.BitmapFromTexture(texture), atlasXml) {
			fromTexture = true;
		}
		public TextureAtlas(Bitmap bitmap, XmlDocument atlasXml) {
			if (bitmap == null) {
				throw new ArgumentNullException("bitmap");
			}
			if (atlasXml == null) {
				throw new ArgumentNullException("atlasXml");
			}

			atlasBitmap = bitmap;
			if (atlasXml != null) {
				ParseXml(atlasXml);
			}
		}
		public TextureAtlas(Texture texture, int columns, int rows) : this(TextureUtil.BitmapFromTexture(texture), columns, rows) {
			fromTexture = true;
		}
		public TextureAtlas(Bitmap bitmap, int columns, int rows) {
			if (bitmap == null) {
				throw new ArgumentNullException("bitmap");
			}
			if (rows <= 0) {
				throw new Exception("rows cannot be less than or equal to zero.");
			}
			if (columns <= 0) {
				throw new Exception("columns cannot be less than or equal to zero.");
			}

			atlasBitmap = bitmap;

			int width = (int) (bitmap.Width / columns);
			int height = (int) (bitmap.Height / rows);

			for (int x = 0; x < columns; x++) {
				for (int y = 0; y < rows; y++) {
					Rectangle region = new Rectangle(x * width, y * height, width, height);
					AddRegion((x + 1) + "_" + (y + 1), region);
					AddRegion(MathUtil.ToXY(columns, x + 1, y + 1).ToString(), region);
				}
			}
		}
		~TextureAtlas() {
			Dispose();
		}

		//public
		public Bitmap GetBitmap(string name) {
			if (name == null) {
				throw new ArgumentNullException("name");
			}

			Bitmap retVal = null;
			if (subBitmaps.TryGetValue(name, out retVal)) {
				return retVal;
			}
			return null;
		}
		public Texture GetTexture(string name) {
			if (name == null) {
				throw new ArgumentNullException("name");
			}

			Texture retVal = null;
			if (subTextures.TryGetValue(name, out retVal)) {
				return retVal;
			}
			return null;
		}
		public bool HasValue(string name) {
			if (name == null) {
				throw new ArgumentNullException("name");
			}
			return subBitmaps.ContainsKey(name);
		}

		public Bitmap[] GetBitmaps(string prefix = "") {
			if (prefix == null) {
				throw new ArgumentNullException("prefix");
			}

			string[] names = GetNames(prefix);
			Bitmap[] retVal = new Bitmap[names.Length];

			for (int i = 0; i < names.Length; i++) {
				retVal[i] = subBitmaps[names[i]];
			}

			return retVal;
		}
		public Texture[] GetTextures(string prefix = "") {
			if (prefix == null) {
				throw new ArgumentNullException("prefix");
			}

			string[] names = GetNames(prefix);
			Texture[] retVal = new Texture[names.Length];

			for (int i = 0; i < names.Length; i++) {
				retVal[i] = subTextures[names[i]];
			}

			return retVal;
		}
		public string[] GetNames(string prefix = "") {
			if (prefix == null) {
				throw new ArgumentNullException("prefix");
			}

			if (prefix == "") {
				return subBitmaps.Keys.ToArray();
			}

			List<string> names = new List<string>();
			string[] subtextureNames = subBitmaps.Keys.ToArray();
			for (int i = 0; i < subtextureNames.Length; i++) {
				if (subtextureNames[i].IndexOf(prefix) == 0) {
					names.Add(subtextureNames[i]);
				}
			}

			return names.ToArray();
		}

		public void AddRegion(string name, Rectangle region, bool rotated = false) {
			if (name == null) {
				throw new ArgumentNullException("name");
			}

			Bitmap bitmap = TextureUtil.GetRegion(atlasBitmap, region, (rotated) ? -90.0d : 0.0d);

			if (subBitmaps.ContainsKey(name)) {
				subBitmaps[name] = bitmap;
				subTextures[name].Dispose();
				subTextures[name] = TextureUtil.FromBitmap(bitmap);
			} else {
				subBitmaps.Add(name, bitmap);
				subTextures.Add(name, TextureUtil.FromBitmap(bitmap));
			}
		}

		public void Dispose() {
			foreach (KeyValuePair<string, Bitmap> kvp in subBitmaps) {
				kvp.Value.Dispose();
				subTextures[kvp.Key].Dispose();
			}
			if (fromTexture) {
				atlasBitmap.Dispose();
			}
		}

		//private
		protected virtual void ParseXml(XmlDocument atlasXml) {
			XmlNodeList subTextures = atlasXml.GetElementsByTagName("SubTexture");
			foreach (XmlNode subTexture in subTextures) {
				Rectangle region = new Rectangle();

				string name = parseString(subTexture, "name");
				int x = parseInt(subTexture, "x");
				int y = parseInt(subTexture, "y");
				int width = parseInt(subTexture, "width");
				int height = parseInt(subTexture, "height");
				bool rotated = parseBool(subTexture, "rotated");

				if (x < 0) {
					throw new InvalidOperationException("X \"" + x + "\" is invalid for given XML at: " + name + ".");
				}
				if (y < 0) {
					throw new InvalidOperationException("Y \"" + y + "\" is invalid for given XML at: " + name + ".");
				}
				if (width <= 0) {
					throw new InvalidOperationException("Width \"" + width + "\" is invalid for given XML at: " + name + ".");
				}
				if (height <= 0) {
					throw new InvalidOperationException("Height \"" + height + "\" is invalid for given XML at: " + name + ".");
				}

				region.X = x;
				region.Y = y;
				region.Width = width;
				region.Height = height;

				AddRegion(name, region, rotated);
			}
		}

		private string parseString(XmlNode node, string value) {
			if (node.Attributes == null || node.Attributes[value] == null) {
				throw new InvalidOperationException("Could not parse string \"" + value + "\" from given XML.");
			}
			return node.Attributes[value].Value;
		}
		private int parseInt(XmlNode node, string value) {
			int retVal = 0;

			if (node.Attributes == null || node.Attributes[value] == null) {
				return retVal;
			}

			if (int.TryParse(node.Attributes[value].Value, out retVal)) {
				if (double.IsNaN(retVal) || double.IsInfinity(retVal)) {
					return 0;
				}
				return retVal;
			}
			return retVal;
		}
		private bool parseBool(XmlNode node, string value) {
			if (node.Attributes == null || node.Attributes[value] == null) {
				return false;
			}

			bool retVal = false;
			if (bool.TryParse(node.Attributes[value].Value, out retVal)) {
				return retVal;
			}
			throw new InvalidOperationException("Could not parse bool \"" + value + "\" from given XML at: " + parseString(node, "name") + ".");
		}
	}
}
