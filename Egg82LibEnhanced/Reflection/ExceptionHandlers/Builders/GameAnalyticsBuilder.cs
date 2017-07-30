using System;

namespace Egg82LibEnhanced.Reflection.ExceptionHandlers.Builders {
	public class GameAnalyticsBuilder : IBuilder {
		//vars
		private string gameKey = null;
		private string secretKey = null;

		//constructor
		public GameAnalyticsBuilder(string gameKey, string secretKey) {
			this.gameKey = gameKey;
			this.secretKey = secretKey;
		}

		//public
		public string[] GetParams() {
			return new string[] {
				gameKey,
				secretKey
			};
		}

		//private

	}
}
