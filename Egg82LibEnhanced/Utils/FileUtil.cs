using System;
using System.Collections.Generic;
using System.IO;

namespace Egg82LibEnhanced.Utils {
	public class FileUtil {
		//vars
		public static readonly string CURRENT_DIRECTORY = Directory.GetCurrentDirectory();
		public static readonly char DIRECTORY_SEPARATOR_CHAR = Path.DirectorySeparatorChar;
		public static readonly string LINE_SEPARATOR = Environment.NewLine;

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
			return (streams.ContainsKey(path)) ? true : false;
		}

		public static long GetTotalBytes(string path) {
			if (!streams.ContainsKey(path)) {
				return 0L;
			}

			return streams[path].Length;
		}

		public static byte[] Read(string path, long position, long length = -1) {
			if (path == null) {
				throw new ArgumentNullException("path");
			}
			if (!streams.ContainsKey(path)) {
				throw new Exception("File is not open.");
			}

			if (position < 0) {
				position = 0;
			}
			long totalBytes = GetTotalBytes(path);
			if (length < 0 || length > totalBytes - position) {
				length = totalBytes - position;
			}

			byte[] buffer = new byte[Math.Min(length, streams[path].Length - position)];
			
			streams[path].Position = position;
			while (length > int.MaxValue) {
				streams[path].Read(buffer, 0, int.MaxValue);
				length -= int.MaxValue;
				streams[path].Position += int.MaxValue;
			}
			streams[path].Read(buffer, 0, (int) length);

			return buffer;
		}
		public static void Write(string path, byte[] bytes, long position) {
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
			long length = bytes.LongLength;
			while (length > int.MaxValue) {
				streams[path].Write(bytes, 0, int.MaxValue);
				length -= int.MaxValue;
				streams[path].Position += int.MaxValue;
			}
			streams[path].Write(bytes, 0, (int) length);
			streams[path].Flush(true);
		}

		public static void Erase(string path) {
			if (streams.ContainsKey(path)) {
				streams[path].SetLength(0L);
				streams[path].Flush(true);
			} else {
				DeleteFile(path);
				CreateFile(path);
			}
		}

		//private

	}
}
