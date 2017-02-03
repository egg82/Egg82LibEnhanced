using Egg82LibEnhanced.Patterns;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Egg82LibEnhanced.Utils {
	public class SettingsUtil {
		//vars

		//constructor
		public SettingsUtil() {

		}

		//public
		public void Load(string path, IRegistry registry) {
			Load(path, registry, Encoding.UTF8);
		}
		public void Load(string path, IRegistry registry, Encoding enc) {
			bool fileWasOpen = true;

			if (!FileUtil.IsOpen(path)) {
				FileUtil.Open(path);
				fileWasOpen = false;
			}

			int totalBytes = ((FileUtil.GetTotalBytes(path) > int.MaxValue) ? int.MaxValue : (int) FileUtil.GetTotalBytes(path));
			string str = enc.GetString(FileUtil.Read(path, 0, totalBytes)).Replace("\r", "").Replace("\n", "");

			if (!fileWasOpen) {
				FileUtil.Close(path);
			}

			if (str == string.Empty) {
				return;
			}

			object[] json = JsonConvert.DeserializeObject<object[]>(str);

			if (json.Length > 0) {
				setRegistry(json, registry);
			}
		}

		public void Save(string path, IRegistry registry) {
			Save(path, registry, Encoding.UTF8);
		}
		public void Save(string path, IRegistry registry, Encoding enc) {
			string[] names = registry.RegistryNames;

			bool fileWasOpen = true;

			if (!FileUtil.IsOpen(path)) {
				FileUtil.Open(path);
				fileWasOpen = false;
			}

			List<object> json = new List<object>();
			foreach (string name in names) {
				json.Add(new { name = name, data = registry.GetRegister(name) });
			}

			FileUtil.Erase(path);
			FileUtil.Write(path, enc.GetBytes(JsonConvert.SerializeObject(json.ToArray(), Formatting.Indented)), 0);

			if (!fileWasOpen) {
				FileUtil.Close(path);
			}
		}

		public void LoadSave(string path, IRegistry registry) {
			LoadSave(path, registry, Encoding.UTF8);
		}
		public void LoadSave(string path, IRegistry registry, Encoding enc) {
			Load(path, registry, enc);
			Save(path, registry, enc);
		}

		//private
		private void setRegistry(dynamic[] json, IRegistry registry) {
			for (int i = 0; i < json.Length; i++) {
				if (json[i].data.Value == null) {
					try {
						registry.SetRegister(json[i].name.Value, JsonConvert.DeserializeObject(json[i].data.ToString()));
					} catch (Exception) {

					}
				} else {
					try {
						registry.SetRegister(json[i].name.Value, json[i].data.Value);
					} catch (Exception) {

					}
				}
			}
		}
	}
}
