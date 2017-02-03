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

		//private

	}
}
