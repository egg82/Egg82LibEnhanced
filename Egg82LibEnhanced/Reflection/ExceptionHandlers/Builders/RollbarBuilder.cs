using System;

namespace Egg82LibEnhanced.Reflection.ExceptionHandlers.Builders {
	public class RollbarBuilder : IBuilder {
		//vars
		private string accessToken = null;
		private string environment = null;

		//constructor
		public RollbarBuilder(string accessToken, string environment) {
			this.accessToken = accessToken;
			this.environment = environment;
		}

		//public
		public string[] GetParams() {
			return new string[] {
				accessToken,
				environment
			};
		}

		//private

	}
}
