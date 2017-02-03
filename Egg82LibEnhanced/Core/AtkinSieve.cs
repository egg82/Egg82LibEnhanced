using System;
using System.Collections.Generic;

namespace Egg82LibEnhanced.Core {
	public class AtkinSieve {
		//vars

		//constructor
		public AtkinSieve() {

		}

		//public
		public static bool[] generate(int max) {
			if (max < 3) {
				max = 3;
			}

			double sqrt = Math.Sqrt(max);
			List<bool> retVal = new List<bool>();
			int n = 0;
			int x = 0;

			for (int i = 0; i < max; i++) {
				retVal.Add(false);
			}
			
			retVal[2] = true;
			retVal[3] = true;

			for (x = 1; x <= sqrt; x++) {
				for (int y = 1; y <= sqrt; y++) {
					n = (4 * x * y) + (y * y);
					if (n <= max && (n % 12 == 1 || n % 12 == 5)) {
						retVal[n] = !retVal[n];
					}
					n = (3 * x * x) + (y * y);
					if (n <= max && n % 12 == 7) {
						retVal[n] = !retVal[n];
					}
					n = (3 * x * x) - (y * y);
					if (x > y && n <= max && n % 12 == 11) {
						retVal[n] = !retVal[n];
					}
				}
			}

			for (n = 5; n <= sqrt; n++) {
				if (retVal[n]) {
					x = n * n;
					for (int i = x; i <= max; i += x) {
						retVal[i] = false;
					}
				}
			}

			return retVal.ToArray();
		}
		public static int[] normalize(bool[] raw, int min, int max = int.MaxValue) {
			List<int> retVal = new List<int>();

			if (max >= raw.Length) {
				max = raw.Length - 1;
			}
			if (min < 0) {
				min = 0;
			}
			if (min > max) {
				int temp = max;
				max = min;
				min = temp;
			}

			for (int i = 0; i < max; i++) {
				if (raw[i]) {
					retVal.Add(i);
				}
			}

			return retVal.ToArray();
		}

		//private

	}
}
