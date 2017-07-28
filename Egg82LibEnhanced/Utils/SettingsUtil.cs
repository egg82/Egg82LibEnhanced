using Egg82LibEnhanced.Patterns;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
		public static void Load(string path, IRegistry<string> registry) {
			Load(path, registry, Encoding.UTF8);
		}
		public static void Load(string path, IRegistry<string> registry, Encoding enc) {
			if (!FileUtil.PathExists(path)) {
				throw new Exception("path does not exist.");
			}
			if (!FileUtil.PathIsFile(path)) {
				throw new Exception("path is not a file.");
			}

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

			dynamic[] json = JsonConvert.DeserializeObject<dynamic[]>(str);

			if (json.Length > 0) {
				setRegistry(json, registry);
			}
		}

		public static void Save(string path, IRegistry<string> registry) {
			Save(path, registry, Encoding.UTF8);
		}
		public static void Save(string path, IRegistry<string> registry, Encoding enc) {
			if (FileUtil.PathExists(path)) {
				if (!FileUtil.PathIsFile(path)) {
					throw new Exception("path is not a file.");
				}
			} else {
				FileUtil.CreateFile(path);
			}

			string[] names = registry.GetKeys();

			bool fileWasOpen = true;

			if (!FileUtil.IsOpen(path)) {
				FileUtil.Open(path);
				fileWasOpen = false;
			}

			List<dynamic> json = new List<dynamic>();
			foreach (string name in names) {
				json.Add(new { name = name, data = registry.GetRegister(name) });
			}

			FileUtil.Erase(path);
			FileUtil.Write(path, enc.GetBytes(JsonConvert.SerializeObject(json.ToArray(), Formatting.Indented)), 0);

			if (!fileWasOpen) {
				FileUtil.Close(path);
			}
		}

		public static void LoadSave(string path, IRegistry<string> registry) {
			LoadSave(path, registry, Encoding.UTF8);
		}
		public static void LoadSave(string path, IRegistry<string> registry, Encoding enc) {
			if (!FileUtil.PathExists(path)) {
				FileUtil.CreateFile(path);
			}

			Load(path, registry, enc);
			Save(path, registry, enc);
		}

		//private
		private static void setRegistry(dynamic[] json, IRegistry<string> registry) {
			for (int i = 0; i < json.Length; i++) {
				if (json[i].data.Type == JTokenType.Array) {
					try {
						if (registry.HasRegister(json[i].name.Value)) {
							registry.SetRegister(json[i].name.Value, ReflectUtil.TryConvert(registry.GetRegisterType(json[i].name.Value), json[i].data.ToObject(registry.GetRegisterType(json[i].name.Value))));
						} else {
							registry.SetRegister(json[i].name.Value, ReflectUtil.TryConvert<object[]>(json[i].data.ToObject(registry.GetRegisterType(json[i].name.Value))));
						}
					} catch (Exception) {
						
					}
				} else {
					if (json[i].data.Value == null) {
						try {
							if (registry.HasRegister(json[i].name.Value)) {
								registry.SetRegister(json[i].name.Value, ReflectUtil.TryConvert(registry.GetRegisterType(json[i].name.Value), JsonConvert.DeserializeObject(json[i].data.ToString())));
							} else {
								registry.SetRegister(json[i].name.Value, ReflectUtil.TryConvert<object>(JsonConvert.DeserializeObject(json[i].data.ToString())));
							}
						} catch (Exception) {

						}
					} else {
						try {
							if (registry.HasRegister(json[i].name.Value)) {
								registry.SetRegister(json[i].name.Value, ReflectUtil.TryConvert(registry.GetRegisterType(json[i].name.Value), json[i].data.Value));
							} else {
								registry.SetRegister(json[i].name.Value, ReflectUtil.TryConvert<object>(json[i].data.Value));
							}
						} catch (Exception) {

						}
					}
				}
			}
		}
	}
}
