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
		private Bitmap atlasTexture = null;
		private Dictionary<string, Texture> subTextures = new Dictionary<string, Texture>();

		//constructor
		public TextureAtlas(Bitmap texture, XmlDocument atlasXml) {
			if (texture == null) {
				throw new ArgumentNullException("texture");
			}
			if (atlasXml == null) {
				throw new ArgumentNullException("atlasXml");
			}

			atlasTexture = texture;
			if (atlasXml != null) {
				ParseXml(atlasXml);
			}
		}
		public TextureAtlas(Bitmap texture, int columns, int rows) {
			if (texture == null) {
				throw new ArgumentNullException("texture");
			}
			if (rows <= 0) {
				throw new Exception("rows cannot be less than or equal to zero.");
			}
			if (columns <= 0) {
				throw new Exception("columns cannot be less than or equal to zero.");
			}

			atlasTexture = texture;

			int width = (int) (texture.Width / columns);
			int height = (int) (texture.Height / rows);
			
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
		public bool HasTexture(string name) {
			if (name == null) {
				throw new ArgumentNullException("name");
			}
			return subTextures.ContainsKey(name);
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
				return subTextures.Keys.ToArray();
			}

			List<string> names = new List<string>();
			string[] subtextureNames = subTextures.Keys.ToArray();
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

			if (subTextures.ContainsKey(name)) {
				subTextures[name] = TextureUtil.FromBitmap(TextureUtil.GetRegion(atlasTexture, region, (rotated) ? -90.0d : 0.0d));
			} else {
				subTextures.Add(name, TextureUtil.FromBitmap(TextureUtil.GetRegion(atlasTexture, region, (rotated) ? -90.0d : 0.0d)));
			}
		}

		public void Dispose() {
			foreach (KeyValuePair<string, Texture> kvp in subTextures) {
				kvp.Value.Dispose();
			}
			atlasTexture.Dispose();
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
				throw new InvalidOperationException("Could not parse bool \"" + value + "\" from given XML at: " + parseString(node, "name") + ".");
			}

			bool retVal = false;
			if (bool.TryParse(node.Attributes[value].Value, out retVal)) {
				return retVal;
			}
			throw new InvalidOperationException("Could not parse bool \"" + value + "\" from given XML at: " + parseString(node, "name") + ".");
		}
	}
}
