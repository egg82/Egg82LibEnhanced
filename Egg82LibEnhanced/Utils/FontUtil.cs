using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Runtime.InteropServices;

namespace Egg82LibEnhanced.Utils {
	public class FontUtil {
		//vars
		private static Dictionary<string, FontFamily> fonts = new Dictionary<string, FontFamily>();
		private static PrivateFontCollection fontCollection = new PrivateFontCollection();

		//constructor
		public FontUtil() {

		}

		//public
		public static void AddFont(string name, byte[] font) {
			if (name == null) {
				throw new ArgumentNullException("name");
			}
			if (font == null) {
				throw new ArgumentNullException("font");
			}

			FontFamily family = null;
			if (fonts.TryGetValue(name, out family)) {
				family.Dispose();
			}
			
			GCHandle handle = GCHandle.Alloc(font, GCHandleType.Pinned);
			IntPtr address = handle.AddrOfPinnedObject();
			try {
				fontCollection.AddMemoryFont(address, font.Length);
			} catch (Exception) {

			}
			handle.Free();

			if (family != null) {
				fonts[name] = fontCollection.Families[fontCollection.Families.Length - 1];
			} else {
				fonts.Add(name, fontCollection.Families[fontCollection.Families.Length - 1]);
			}
		}
		public static void RemoveFont(string name) {
			if (name == null) {
				throw new ArgumentNullException("name");
			}

			FontFamily family = null;
			if (fonts.TryGetValue(name, out family)) {
				family.Dispose();
				fonts.Remove(name);
			}
		}
		public static FontFamily GetFont(string name) {
			if (name == null) {
				throw new ArgumentNullException("name");
			}

			FontFamily family = null;
			if (fonts.TryGetValue(name, out family)) {
				return family;
			}
			return null;
		}

		//private

	}
}
