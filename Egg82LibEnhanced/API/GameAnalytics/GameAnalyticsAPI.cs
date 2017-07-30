using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Egg82LibEnhanced.API.GameAnalytics {
	public class GameAnalyticsAPI : IGameAnalyticsAPI {
		//vars
		private static string apiUrl = "https://api.gameanalytics.com/v2";

		private string gameKey = null;
		private byte[] secretKey = null;

		private string device = Regex.Replace(Environment.OSVersion.VersionString, @"\s", "");
		private uint version = 2;
		private string userId = null;
		private string sdkVersion = "rest api v2";
		private string osVersion = null;
		private string manufacturer = null;
		private string platform = null;
		private string sessionId = Guid.NewGuid().ToString();
		private ulong sessionNum = 0L;
		private string build = null;

		private int tsOffset = 0;
		private bool initialized = false;
		private string[] flags = null;

		private long sessionStartTime = -1L;
		
		private JsonMergeSettings mergeSettings = new JsonMergeSettings() {
			MergeArrayHandling = MergeArrayHandling.Union,
			MergeNullValueHandling = MergeNullValueHandling.Ignore
		};
		private JObject baseJsonData = null;

		//enums
		public enum StoreType {
			Apple = 1,
			GooglePlay = 2,
			Unknown = 3
		}
		public enum FlowType {
			Spend = -1,
			Receive = 1
		}
		public enum ProgressionType {
			Start = 0,
			Fail = -1,
			Complete = 1
		}
		public enum ErrorSeverity {
			Debug = 1000,
			Info = 100,
			Warning = 50,
			Error = 10,
			Critical = 0
		}

		// structs
		private struct InitData {
			public string platform;
			public string os_version;
			public string sdk_version;
		}
		private struct InitResponseData {
			public bool enabled;
			public long server_ts;
			public string[] flags;
		}
		
		private struct ErrorResponseData {
			public BaseData @event;
			public InternalErrorData[] errors;
		}
		private struct InternalErrorData {
			public string error_type;
			public string path;
		}

		private struct BaseData {
			public string device;
			public uint v;
			public string user_id;
			public long client_ts;
			public string sdk_version;
			public string os_version;
			public string manufacturer;
			public string platform;
			public string session_id;
			public ulong session_num;
			
			public string build;
		}

		private struct UserSessionStartData {
			public string category;
		}
		private struct UserSessionEndData {
			public string category;
			public uint length;
		}

		private struct BusinessData {
			public string category;
			public string event_id;
			public long amount;
			public string currency;
			public ulong transaction_num;
			public string cart_type;
			public ReceiptInfo receipt_info;
		}
		private struct ReceiptInfo {
			public string receipt;
			public string store;
			public string signature;
		}

		private struct ResourceData {
			public string category;
			public string event_id;
			public double amount;
		}

		private struct ProgressionData {
			public string category;
			public string event_id;
			public ulong attempt_num;
			public long score;
		}

		private struct DesignData {
			public string category;
			public string event_id;
			public double value;
		}

		private struct ErrorData {
			public string category;
			public string severity;
			public string message;
		}

		//constructor
		public GameAnalyticsAPI() {
			try {
				userId = (string) RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey("SOFTWARE").OpenSubKey("Microsoft").OpenSubKey("Cryptography").GetValue("MachineGuid", "default");
			} catch (Exception) {

			}

			if (userId == null || userId == string.Empty || userId == "default") {
				userId = Guid.NewGuid().ToString();
				try {
					RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey("SOFTWARE").OpenSubKey("Microsoft").OpenSubKey("Cryptography").SetValue("MachineGuid", userId);
				} catch (Exception) {

				}
			}

			try {
				build = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
			} catch (Exception) {
				build = "unknown";
			}

			platform = getPlatform(Environment.OSVersion.Platform);
			manufacturer = getManufacturer(Environment.OSVersion.Platform);
			osVersion = getPlatform(Environment.OSVersion.Platform) + " " + Environment.OSVersion.Version.Major + "." + Environment.OSVersion.Version.Minor + "." + Environment.OSVersion.Version.Build;
		}

		//public
		public bool IsInitialized() {
			return initialized;
		}
		public string GetGameKey() {
			return gameKey;
		}
		
		public void SendInit(string gameKey, string secretKey, ulong sessionNum) {
			if (gameKey == null || gameKey == string.Empty) {
				throw new ArgumentNullException("gameKey");
			}
			if (secretKey == null || secretKey == string.Empty) {
				throw new ArgumentNullException("secretKey");
			}

			this.gameKey = gameKey.Trim();
			this.secretKey = Encoding.UTF8.GetBytes(secretKey.Trim());
			this.sessionNum = sessionNum;

			byte[] postData = compress(JsonConvert.SerializeObject(new InitData {
				platform = platform,
				os_version = osVersion,
				sdk_version = sdkVersion
			}));

			HttpWebRequest request = createRequest(apiUrl + "/" + gameKey + "/init", postData.LongLength, hmac256(postData, this.secretKey));
			HttpWebResponse response = null;
			string responseData = sendRequest(request, postData, out response);

			if (response.StatusCode != HttpStatusCode.OK) {
				throw new Exception("GameAnalytics response code " + ((int) response.StatusCode) + " (" + response.StatusDescription + ")");
			}

			InitResponseData data = JsonConvert.DeserializeObject<InitResponseData>(responseData);
			initialized = data.enabled;
			tsOffset = (int) (getCurrentTime() - data.server_ts);
			flags = data.flags;

			recreateBaseJsonData();
		}

		public void SendUserSessionStart() {
			if (!initialized) {
				throw new Exception("GameAnalytics has not been initialized. Please run SendInit() to start sending data.");
			}

			sessionStartTime = getCurrentTime();

			byte[] postData = compress(getJsonString(new UserSessionStartData {
				category = "user"
			}));

			sendEvent(postData);
		}
		public void SendUserSessionEnd() {
			if (!initialized) {
				throw new Exception("GameAnalytics has not been initialized. Please run SendInit() to start sending data.");
			}
			if (sessionStartTime == -1L) {
				throw new Exception("UserSessionStart has not been called. Please run UserSessionStart before running UserSessionEnd.");
			}

			byte[] postData = compress(getJsonString(new UserSessionEndData {
				category = "session_end",
				length = (uint) ((getCurrentTime() - sessionStartTime) / 1000L)
			}));

			sendEvent(postData);
		}

		public void SendPurchase(string itemCategory, string itemName, long amountInPennies, ulong transactionNum, string currencyType = "USD") {
			if (!initialized) {
				throw new Exception("GameAnalytics has not been initialized. Please run SendInit() to start sending data.");
			}
			if (itemCategory == null || itemCategory == string.Empty) {
				throw new ArgumentNullException("itemCategory");
			}
			if (itemName == null || itemName == string.Empty) {
				throw new ArgumentNullException("itemName");
			}
			if (currencyType == null || currencyType == string.Empty) {
				throw new ArgumentNullException("currencyType");
			}

			byte[] postData = compress(getJsonString(new BusinessData {
				category = "business",
				event_id = itemCategory + ":" + itemName,
				amount = amountInPennies,
				currency = currencyType.ToUpper(),
				transaction_num = transactionNum
			}));

			sendEvent(postData);
		}
		public void SendPurchase(string itemCategory, string itemName, long amountInPennies, ulong transactionNum, string purchaseMenuName, string currencyType = "USD") {
			if (!initialized) {
				throw new Exception("GameAnalytics has not been initialized. Please run SendInit() to start sending data.");
			}
			if (itemCategory == null || itemCategory == string.Empty) {
				throw new ArgumentNullException("itemCategory");
			}
			if (itemName == null || itemName == string.Empty) {
				throw new ArgumentNullException("itemName");
			}
			if (purchaseMenuName == null || purchaseMenuName == string.Empty) {
				throw new ArgumentNullException("itemName");
			}
			if (currencyType == null || currencyType == string.Empty) {
				throw new ArgumentNullException("currencyType");
			}

			byte[] postData = compress(getJsonString(new BusinessData {
				category = "business",
				event_id = itemCategory + ":" + itemName,
				amount = amountInPennies,
				currency = currencyType.ToUpper(),
				transaction_num = transactionNum,
				cart_type = purchaseMenuName
			}));

			sendEvent(postData);
		}
		public void SendPurchase(string itemCategory, string itemName, long amountInPennies, ulong transactionNum, string purchaseMenuName, StoreType store, string receipt, string currencyType = "USD") {
			if (!initialized) {
				throw new Exception("GameAnalytics has not been initialized. Please run SendInit() to start sending data.");
			}
			if (itemCategory == null || itemCategory == string.Empty) {
				throw new ArgumentNullException("itemCategory");
			}
			if (itemName == null || itemName == string.Empty) {
				throw new ArgumentNullException("itemName");
			}
			if (purchaseMenuName == null || purchaseMenuName == string.Empty) {
				throw new ArgumentNullException("itemName");
			}
			if (currencyType == null || currencyType == string.Empty) {
				throw new ArgumentNullException("currencyType");
			}
			if (store == StoreType.GooglePlay) {
				throw new Exception("IAP signature is required for Google Play receipts.");
			}

			byte[] postData = compress(getJsonString(new BusinessData {
				category = "business",
				event_id = itemCategory + ":" + itemName,
				amount = amountInPennies,
				currency = currencyType.ToUpper(),
				transaction_num = transactionNum,
				cart_type = purchaseMenuName,
				receipt_info = new ReceiptInfo {
					store = (store == StoreType.Apple) ? "apple" : (store == StoreType.GooglePlay) ? "google_play" : "unknown",
					receipt = Convert.ToBase64String(Encoding.UTF8.GetBytes(receipt))
				}
			}));

			sendEvent(postData);
		}
		public void SendPurchase(string itemCategory, string itemName, long amountInPennies, ulong transactionNum, string purchaseMenuName, StoreType store, string receipt, string googlePlayIAPSignature, string currencyType = "USD") {
			if (!initialized) {
				throw new Exception("GameAnalytics has not been initialized. Please run SendInit() to start sending data.");
			}
			if (itemCategory == null || itemCategory == string.Empty) {
				throw new ArgumentNullException("itemCategory");
			}
			if (itemName == null || itemName == string.Empty) {
				throw new ArgumentNullException("itemName");
			}
			if (purchaseMenuName == null || purchaseMenuName == string.Empty) {
				throw new ArgumentNullException("itemName");
			}
			if (currencyType == null || currencyType == string.Empty) {
				throw new ArgumentNullException("currencyType");
			}

			byte[] postData = compress(getJsonString(new BusinessData {
				category = "business",
				event_id = itemCategory + ":" + itemName,
				amount = amountInPennies,
				currency = currencyType.ToUpper(),
				transaction_num = transactionNum,
				cart_type = purchaseMenuName,
				receipt_info = new ReceiptInfo {
					store = (store == StoreType.Apple) ? "apple" : (store == StoreType.GooglePlay) ? "google_play" : "unknown",
					receipt = Convert.ToBase64String(Encoding.UTF8.GetBytes(receipt)),
					signature = googlePlayIAPSignature
				}
			}));

			sendEvent(postData);
		}

		public void SendVirtualCurrencyFlow(FlowType type, string currencyName, double amount, string itemCategory, string itemName) {
			if (currencyName == null || currencyName == string.Empty) {
				throw new ArgumentNullException("currencyName");
			}
			if (itemCategory == null || itemCategory == string.Empty) {
				throw new ArgumentNullException("itemCategory");
			}
			if (itemName == null || itemName == string.Empty) {
				throw new ArgumentNullException("itemName");
			}

			byte[] postData = compress(getJsonString(new ResourceData {
				category = "resource",
				event_id = ((type == FlowType.Spend) ? "Sink" : "Source") + ":" + currencyName + ":" + itemCategory + ":" + itemName,
				amount = (type == FlowType.Spend) ? Math.Abs(amount) * -1 : Math.Abs(amount)
			}));

			sendEvent(postData);
		}

		public void SendProgression(ProgressionType type, string levelName, string subWorldName = null, string worldName = null) {
			if (subWorldName == string.Empty) {
				subWorldName = null;
			}
			if (worldName == string.Empty) {
				subWorldName = null;
			}
			if (levelName == null || levelName == string.Empty) {
				throw new ArgumentNullException("worldName");
			}

			if (worldName != null && subWorldName == null) {
				subWorldName = worldName;
				worldName = null;
			}

			byte[] postData = compress(getJsonString(new ProgressionData {
				category = "resource",
				event_id = ((type == ProgressionType.Start) ? "Start:" : (type == ProgressionType.Complete) ? "Complete:" : "Fail:") + ((worldName != null) ? worldName + ":" : "") + ((subWorldName != null) ? subWorldName + ":" : "") + levelName
			}));

			sendEvent(postData);
		}
		public void SendProgression(ProgressionType type, ulong attemptNum, long score, string levelName, string subWorldName = null, string worldName = null) {
			if (subWorldName == string.Empty) {
				subWorldName = null;
			}
			if (worldName == string.Empty) {
				subWorldName = null;
			}
			if (levelName == null || levelName == string.Empty) {
				throw new ArgumentNullException("worldName");
			}

			if (worldName != null && subWorldName == null) {
				subWorldName = worldName;
				worldName = null;
			}

			byte[] postData = compress(getJsonString(new ProgressionData {
				category = "resource",
				event_id = ((type == ProgressionType.Start) ? "Start:" : (type == ProgressionType.Complete) ? "Complete:" : "Fail:") + ((worldName != null) ? worldName + ":" : "") + ((subWorldName != null) ? subWorldName + ":" : "") + levelName,
				attempt_num = attemptNum,
				score = score
			}));

			sendEvent(postData);
		}

		public void SendError(String message) {
			SendError(ErrorSeverity.Error, message);
		}
		public void SendError(ErrorSeverity severity, string message) {
			if (message == null || message == string.Empty) {
				throw new ArgumentNullException("message");
			}

			byte[] postData = compress(getJsonString(new ErrorData {
				category = "error",
				severity = (severity == ErrorSeverity.Critical) ? "critical" : (severity == ErrorSeverity.Debug) ? "debug" : (severity == ErrorSeverity.Info) ? "info" : (severity == ErrorSeverity.Warning) ? "warning" : "error",
				message = message
			}));

			sendEvent(postData);
		}

		public void SendOther(double value, string rootNode, string nodeLevel1 = null, string nodeLevel2 = null, string nodeLevel3 = null, string nodeLevel4 = null) {
			if (nodeLevel4 == string.Empty) {
				nodeLevel4 = null;
			}
			if (nodeLevel3 == string.Empty) {
				nodeLevel3 = null;
			}
			if (nodeLevel2 == string.Empty) {
				nodeLevel2 = null;
			}
			if (nodeLevel1 == string.Empty) {
				nodeLevel1 = null;
			}

			for (int i = 0; i < 4; i++) {
				if (nodeLevel1 != null && rootNode == null) {
					rootNode = nodeLevel1;
					nodeLevel1 = null;
				}
				if (nodeLevel2 != null && nodeLevel1 == null) {
					nodeLevel1 = nodeLevel2;
					nodeLevel2 = null;
				}
				if (nodeLevel3 != null && nodeLevel2 == null) {
					nodeLevel2 = nodeLevel3;
					nodeLevel3 = null;
				}
				if (nodeLevel4 != null && nodeLevel3 == null) {
					nodeLevel3 = nodeLevel4;
					nodeLevel4 = null;
				}
			}

			if (rootNode == null || rootNode == string.Empty) {
				throw new ArgumentNullException("rootNode");
			}

			byte[] postData = compress(getJsonString(new DesignData {
				category = "design",
				event_id = rootNode + ":" + ((nodeLevel1 != null) ? nodeLevel1 + ":" : "") + ((nodeLevel2 != null) ? nodeLevel2 + ":" : "") + ((nodeLevel3 != null) ? nodeLevel3 + ":" : "") + ((nodeLevel4 != null) ? nodeLevel4 : ""),
				value = value
			}));

			sendEvent(postData);
		}

		//private
		private string getPlatform(PlatformID platform) {
			if (platform == PlatformID.MacOSX) {
				return "macintosh";
			} else if (platform == PlatformID.Unix) {
				return "unix";
			} else if (platform == PlatformID.Win32NT || platform == PlatformID.Win32S || platform == PlatformID.Win32Windows || platform == PlatformID.WinCE) {
				return "windows";
			} else if (platform == PlatformID.Xbox) {
				return "xbox";
			}

			return "unknown: '" + platform + "'";
		}
		private string getManufacturer(PlatformID platform) {
			if (platform == PlatformID.MacOSX) {
				return "apple";
			} else if (platform == PlatformID.Unix) {
				return "bell";
			} else if (platform == PlatformID.Win32NT || platform == PlatformID.Win32S || platform == PlatformID.Win32Windows || platform == PlatformID.WinCE || platform == PlatformID.Xbox) {
				return "microsoft";
			}

			return "unknown";
		}

		private void sendEvent(byte[] postData) {
			HttpWebRequest request = createRequest(apiUrl + "/" + gameKey + "/events", postData.LongLength, hmac256(postData, secretKey));
			HttpWebResponse response = null;
			string responseData = sendRequest(request, postData, out response);

			if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.BadRequest) {
				throw new Exception("GameAnalytics response code " + ((int) response.StatusCode) + " (" + response.StatusDescription + ")");
			}

			JToken token = JToken.Parse(responseData);
			if (token is JArray) {
				ErrorResponseData[] data = JsonConvert.DeserializeObject<ErrorResponseData[]>(responseData);
				JArray errors = new JArray();
				for (int i = 0; i < data.Length; i++) {
					for (int j = 0; j < data[i].errors.Length; j++) {
						errors.Add(JsonConvert.DeserializeObject<object>(JsonConvert.SerializeObject(data[i].errors[j])));
					}
				}
				if (errors.HasValues) {
					throw new Exception("GameAnalytics error(s): " + errors.ToString(Formatting.None));
				}
			}
		}

		private string getJsonString(object obj) {
			JObject jObj = JObject.FromObject(obj);
			jObj.Merge(baseJsonData, mergeSettings);
			return new JArray(jObj).ToString(Formatting.None);
		}
		private void recreateBaseJsonData() {
			baseJsonData = JObject.FromObject(new BaseData {
				device = device,
				v = version,
				user_id = userId,
				client_ts = getCurrentTime() - tsOffset,
				sdk_version = sdkVersion,
				os_version = osVersion,
				manufacturer = manufacturer,
				platform = platform,
				session_id = sessionId,
				session_num = sessionNum,
				build = build,
			});
		}
		
		private HttpWebRequest createRequest(string url, long dataLength, byte[] hmac) {
			HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
			request.Method = "POST";
			request.Accept = "application/json";
			request.ContentType = "application/json";
			request.ContentLength = dataLength;
			request.UserAgent = "Egg82LibEnhanced/GameAnalyticsAPI";
			request.Headers.Set(HttpRequestHeader.ContentEncoding, "gzip");
			request.Headers.Set(HttpRequestHeader.Authorization, Convert.ToBase64String(hmac));
			return request;
		}
		private byte[] compress(string raw) {
			byte[] data = Encoding.UTF8.GetBytes(raw);
			using (MemoryStream stream = new MemoryStream()) {
				using (GZipStream gzip = new GZipStream(stream, CompressionMode.Compress, true)) {
					gzip.Write(data, 0, data.Length);
				}
				return stream.ToArray();
			}
		}

		private long getCurrentTime() {
			return DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
		}

		private string sendRequest(HttpWebRequest request, byte[] postData, out HttpWebResponse response) {
			Stream requestStream = request.GetRequestStream();
			requestStream.Write(postData, 0, postData.Length);
			requestStream.Flush();
			requestStream.Close();

			try {
				response = (HttpWebResponse) request.GetResponse();
			} catch (WebException ex) {
				response = (HttpWebResponse) ex.Response;
			}

			return new StreamReader(response.GetResponseStream()).ReadToEnd();
		}
		
		private byte[] hmac256(byte[] input, byte[] key) {
			HMACSHA256 hmac = new HMACSHA256(key);
			byte[] retVal = hmac.ComputeHash(input);
			hmac.Dispose();
			return retVal;
		}
	}
}
