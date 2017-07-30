using Newtonsoft.Json;
using RollbarDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Egg82LibEnhanced.Reflection.ExceptionHandlers.Internal {
	internal class LoggingRollbarClient : IRollbarClient {
		//vars
		private RollbarConfig _config = null;
		private SynchronizedCollection<System.Exception> exceptions = new SynchronizedCollection<System.Exception>();
		private System.Exception lastException = null;

		public bool LimitReached = false;

		//constructor
		public LoggingRollbarClient() {
			
		}

		//public
		public void Connect(RollbarConfig config) {
			_config = config;
		}

		public RollbarConfig Config {
			get {
				return _config;
			}
		}
		public bool Connected {
			get {
				return (_config != null) ? true : false;
			}
		}
		
		public Guid PostItem(Payload payload) {
			string result = sendPost("item/", payload);
			return parseResponse(result);
		}
		public async Task<Guid> PostItemAsync(Payload payload) {
			string result = await sendPostAsync("item/", payload);
			return parseResponse(result);
		}

		public List<System.Exception> GetUnsentExceptions() {
			return new List<System.Exception>(exceptions);
		}
		public void SetUnsentExceptions(List<System.Exception> list) {
			exceptions.Clear();
			if (list == null) {
				return;
			}
			
			foreach (System.Exception ex in list) {
				exceptions.Add(ex);
			}
		}

		public void AddException(System.Exception ex) {
			exceptions.Add(ex);
		}
		public void SetLastException(System.Exception ex) {
			lastException = ex;
		}

		public void ClearExceptions() {
			exceptions.Clear();
		}

		//private
		private static Guid parseResponse(string result) {
			RollbarResponse response = JsonConvert.DeserializeObject<RollbarResponse>(result);
			return (response.Error == 0) ? Guid.Parse(response.Result.Uuid) : Guid.Empty;
		}
		private string sendPost<T>(string url, T payload) {
			//byte[] serialized = compress(JsonConvert.SerializeObject(payload));
			byte[] serialized = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload));
			HttpWebRequest request = createRequest(url, serialized);

			Stream requestStream = request.GetRequestStream();
			requestStream.Write(serialized, 0, serialized.Length);
			requestStream.Flush();
			requestStream.Close();

			HttpWebResponse response = null;
			try {
				response = (HttpWebResponse) request.GetResponse();
			} catch (WebException ex) {
				response = (HttpWebResponse) ex.Response;
			}
			if (response.StatusCode != HttpStatusCode.OK) {
				if (response.StatusCode == (HttpStatusCode) 429) {
					LimitReached = true;
				}

				exceptions.Add(lastException);
				lastException = null;
			}

			return new StreamReader(response.GetResponseStream()).ReadToEnd();
		}
		private async Task<string> sendPostAsync<T>(string url, T payload) {
			//byte[] serialized = compress(JsonConvert.SerializeObject(payload));
			byte[] serialized = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload));
			HttpWebRequest request = createRequest(url, serialized);

			Stream requestStream = await request.GetRequestStreamAsync();
			requestStream.Write(serialized, 0, serialized.Length);
			requestStream.Flush();
			requestStream.Close();

			HttpWebResponse response = null;
			try {
				response = (HttpWebResponse) await request.GetResponseAsync();
			} catch (WebException ex) {
				response = (HttpWebResponse) ex.Response;
			}
			if (response.StatusCode != HttpStatusCode.OK) {
				if (response.StatusCode == (HttpStatusCode) 429) {
					LimitReached = true;
				}

				exceptions.Add(lastException);
				lastException = null;
			}

			return new StreamReader(response.GetResponseStream()).ReadToEnd();
		}

		private HttpWebRequest createRequest(string url, byte[] postData) {
			HttpWebRequest request = (HttpWebRequest) WebRequest.Create($"{Config.EndPoint}{url}");
			request.Method = "POST";
			request.Accept = "application/json";
			request.ContentType = "application/json";
			request.ContentLength = postData.LongLength;
			request.UserAgent = "Egg82LibEnhanced/LoggingRollbarClient";
			//request.Headers.Set(HttpRequestHeader.ContentEncoding, "gzip");
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
	}
}
