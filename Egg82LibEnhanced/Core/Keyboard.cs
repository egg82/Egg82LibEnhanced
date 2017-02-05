using System;

namespace Egg82LibEnhanced.Core {
	public class Keyboard {
		//vars
		internal bool[] keys = new bool[256];

		//constructor
		public Keyboard() {

		}

		//public
		public bool IsAnyKeyDown(int[] keyCodes) {
			if (keyCodes == null) {
				return true;
			}
			for (int i = 0; i < keyCodes.Length; i++) {
				if (keyCodes[i] < 0 || keyCodes[i] >= keys.Length) {
					continue;
				}
				if (keys[keyCodes[i]]) {
					return true;
				}
			}
			return false;
		}
		public bool AreAllKeysDown(int[] keyCodes) {
			if (keyCodes == null) {
				return true;
			}
			for (int i = 0; i < keyCodes.Length; i++) {
				if (keyCodes[i] < 0 || keyCodes[i] >= keys.Length) {
					return false;
				}
				if (keys[keyCodes[i]]) {
					continue;
				} else {
					return false;
				}
			}
			return true;
		}

		//private

	}
}
