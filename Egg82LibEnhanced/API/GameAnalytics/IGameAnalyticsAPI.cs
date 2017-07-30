using System;
using static Egg82LibEnhanced.API.GameAnalytics.GameAnalyticsAPI;

namespace Egg82LibEnhanced.API.GameAnalytics {
	public interface IGameAnalyticsAPI {
		//functions
		bool IsInitialized();
		string GetGameKey();

		void SendInit(string gameKey, string secretKey, ulong sessionNum);

		void SendUserSessionStart();
		void SendUserSessionEnd();

		void SendPurchase(string itemCategory, string itemName, long amountInPennies, ulong transactionNum, string currencyType = "USD");
		void SendPurchase(string itemCategory, string itemName, long amountInPennies, ulong transactionNum, string purchaseMenuName, string currencyType = "USD");
		void SendPurchase(string itemCategory, string itemName, long amountInPennies, ulong transactionNum, string purchaseMenuName, StoreType store, string receipt, string currencyType = "USD");
		void SendPurchase(string itemCategory, string itemName, long amountInPennies, ulong transactionNum, string purchaseMenuName, StoreType store, string receipt, string googlePlayIAPSignature, string currencyType = "USD");

		void SendVirtualCurrencyFlow(FlowType type, string currencyName, double amount, string itemCategory, string itemName);

		void SendProgression(ProgressionType type, string levelName, string subWorldName = null, string worldName = null);
		void SendProgression(ProgressionType type, ulong attemptNum, long score, string levelName, string subWorldName = null, string worldName = null);

		void SendError(ErrorSeverity severity, string message);
		void SendError(string message);

		void SendOther(double value, string rootNode, string nodeLevel1 = null, string nodeLevel2 = null, string nodeLevel3 = null, string nodeLevel4 = null);
	}
}
