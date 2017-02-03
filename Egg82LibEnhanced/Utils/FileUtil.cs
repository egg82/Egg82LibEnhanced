using Egg82LibEnhanced.Core;
using System;
using System.Collections.Generic;
using System.IO;

namespace Egg82LibEnhanced.Utils {
	public class FileUtil {
		//vars
		private static Dictionary<string, FileStream> streams = new Dictionary<string, FileStream>();

		//constructor
		public FileUtil() {

		}

		//public
		public static void CreateFile(string path, bool createDirectory = true) {
			if (PathExists(path)) {
				return;
			}
			
			if (createDirectory) {
				Directory.CreateDirectory(Path.GetDirectoryName(path));
			}
			File.Create(path).Dispose();
		}
		public static void DeleteFile(string path) {
			if (!PathExists(path)) {
				return;
			}
			if (!PathIsFile(path)) {
				throw new Exception("Path is not a file.");
			}
			
			File.Delete(path);
		}

		public static void CreateDirectory(string path) {
			if (PathExists(path)) {
				return;
			}
			
			Directory.CreateDirectory(Path.GetDirectoryName(path));
		}
		public static void DeleteDirectory(string path) {
			if (!PathExists(path)) {
				return;
			}
			if (PathIsFile(path)) {
				throw new Exception("Path is not a directory.");
			}
			
			Directory.Delete(Path.GetDirectoryName(path));
		}

		public static bool PathIsFile(string path) {
			if (!PathExists(path)) {
				return false;
			}

			FileAttributes attr = File.GetAttributes(path);

			if (attr.HasFlag(FileAttributes.Directory)) {
				return false;
			}

			return true;
		}
		public static bool PathExists(string path) {
			return (Directory.Exists(path) || File.Exists(path)) ? true : false;
		}
		public static bool FileIsLocked(string path) {
			if (!PathExists(path) || !PathIsFile(path)) {
				return false;
			}

			FileStream stream = null;

			try {
				stream = File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
			} catch (Exception) {
				return true;
			}

			if (stream != null) {
				stream.Dispose();
			}

			return false;
		}
		
		public static void Open(string path) {
			path = path.ToLower();

			if (!PathExists(path)) {
				throw new Exception("Path does not exist.");
			}
			if (!PathIsFile(path)) {
				throw new Exception("Path is not a file.");
			}

			if (streams.ContainsKey(path)) {
				Close(path);
			}
			
			streams.Add(path, File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None));
		}
		public static void Close(string path) {
			path = path.ToLower();

			if (!streams.ContainsKey(path)) {
				return;
			}

			streams[path].Dispose();
			streams.Remove(path);
		}

		public static void CloseAll() {
			foreach (KeyValuePair<string, FileStream> entry in streams) {
				entry.Value.Dispose();
			}
			streams.Clear();
		}

		public static bool IsOpen(string path) {
			path = path.ToLower();

			return (streams.ContainsKey(path)) ? true : false;
		}

		public static long GetTotalBytes(string path) {
			path = path.ToLower();

			if (!streams.ContainsKey(path)) {
				return 0L;
			}

			return streams[path].Length;
		}

		public static byte[] Read(string path, long position, int length) {
			path = path.ToLower();

			if (!streams.ContainsKey(path)) {
				throw new Exception("File is not open.");
			}

			if (position < 0) {
				position = 0;
			}

			byte[] buffer = new byte[Math.Min(length, streams[path].Length - position)];
			
			streams[path].Position = position;
			streams[path].Read(buffer, 0, length);

			return buffer;
		}
		public static void Write(string path, byte[] bytes, long position) {
			path = path.ToLower();

			if (bytes.LongLength > int.MaxValue) {
				throw new Exception("bytes length must be an int.");
			}

			if (bytes == null || bytes.Length == 0) {
				return;
			}

			if (!streams.ContainsKey(path)) {
				throw new Exception("File is not open.");
			}

			if (position < 0) {
				position = 0;
			}
			
			streams[path].Position = position;
			streams[path].Write(bytes, 0, bytes.Length);
			streams[path].Flush(true);
		}

		public static void Erase(string path) {
			path = path.ToLower();

			if (streams.ContainsKey(path)) {
				streams[path].SetLength(0);
				streams[path].Flush(true);
			} else {
				DeleteFile(path);
				CreateFile(path);
			}
		}

		//private

	}
}
