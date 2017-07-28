using Egg82LibEnhanced.Patterns;
using System;
using System.Net;

namespace Egg82LibEnhanced.Utils {
	public class NetUtil {
		//vars
		private static IRegistry<string> addressCache = new Registry<string>();

		//constructor
		public NetUtil() {

		}

		//public
		public static void ClearAddressCache() {
			addressCache.Clear();
		}
		public static IPAddress GetAddress(string address) {
			IPAddress retVal = addressCache.GetRegister(address);

			if (retVal == null) {
				if (!IPAddress.TryParse(address, out retVal)) {
					try {
						IPHostEntry dnsEntry = Dns.GetHostEntry(address);
						if (dnsEntry.AddressList.Length > 0) {
							retVal = dnsEntry.AddressList[0];
						}
					} catch (Exception) {
						return null;
					}
				}

				if (retVal != null) {
					addressCache.SetRegister(address, retVal);
				}
			}

			return retVal;
		}

		//private

	}
}
