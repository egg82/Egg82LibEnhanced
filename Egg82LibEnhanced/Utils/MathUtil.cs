using System;

namespace Egg82LibEnhanced.Utils {
	public class MathUtil {
		//vars
		private static Random rnd = new Random();

		//constructor
		public MathUtil() {

		}

		//public
		public static double Random(double min, double max) {
			return rnd.NextDouble() * (max - min) + min;
		}
		public static int FairRoundedRandom(int min, int max) {
			int num;
			max++;

			do {
				num = (int) Math.Floor(rnd.NextDouble() * (max - min) + min);
			} while (num > max - 1);

			return num;
		}

		public static double Clamp(double min, double max, double val) {
			return Math.Min(max, Math.Max(min, val));
		}
		public static float Clamp(float min, float max, float val) {
			return Math.Min(max, Math.Max(min, val));
		}
		public static int Clamp(int min, int max, int val) {
			return Math.Min(max, Math.Max(min, val));
		}
		public static short Clamp(short min, short max, short val) {
			return Math.Min(max, Math.Max(min, val));
		}
		public static long Clamp(long min, long max, long val) {
			return Math.Min(max, Math.Max(min, val));
		}
		public static byte Clamp(byte min, byte max, byte val) {
			return Math.Min(max, Math.Max(min, val));
		}
		public static uint Clamp(uint min, uint max, uint val) {
			return Math.Min(max, Math.Max(min, val));
		}
		public static ushort Clamp(ushort min, ushort max, ushort val) {
			return Math.Min(max, Math.Max(min, val));
		}
		public static ulong Clamp(ulong min, ulong max, ulong val) {
			return Math.Min(max, Math.Max(min, val));
		}

		public static int ToXY(int width, int x, int y) {
			return y * width + x;
		}
		public static int ToX(int width, int xy) {
			return xy % width;
		}
		public static int ToY(int width, int xy) {
			return (int) Math.Floor((double) xy / (double) width);
		}

		public static double TicksToMilliseconds(long ticks = -1L) {
			return ((ticks == -1L) ? DateTime.Now.Ticks : ticks) / TimeSpan.TicksPerMillisecond;
		}

		public static uint UpperPowerOfTwo(uint v) {
			v--;
			v |= v >> 1;
			v |= v >> 2;
			v |= v >> 4;
			v |= v >> 8;
			v |= v >> 16;
			v++;
			return v;
		}
		public static ulong UpperPowerOfTwo(ulong v) {
			v--;
			v |= v >> 1;
			v |= v >> 2;
			v |= v >> 4;
			v |= v >> 8;
			v |= v >> 16;
			v |= v >> 32;
			v++;
			return v;
		}

		//private

	}
}
