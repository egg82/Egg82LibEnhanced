using Egg82LibEnhanced.Events;
using System;
using System.Net;
using System.Net.Cache;

namespace Egg82LibEnhanced.Net {
	public class SimpleUrlLoader : IDisposable {
		//vars
		public event EventHandler<ProgressEventArgs> Progress = null;
		public event EventHandler<DataCompleteEventArgs> Completed = null;
		public event EventHandler<ExceptionEventArgs> Error = null;

		private WebClient client = new WebClient();
		private bool _loading = false;
		private Uri _uri = null;
		private string _url = null;
		private byte[] _data = null;
		private long _loadedBytes = 0;
		private long _totalBytes = 0;

		//constructor
		public SimpleUrlLoader() {
			client.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
			client.DownloadProgressChanged += onProgress;
			client.DownloadDataCompleted += onComplete;
		}
		~SimpleUrlLoader() {
			Dispose();
		}

		//public
		public void Load(string url) {
			Load(new Uri(url));
		}
		public void Load(Uri uri) {
			if (_loading) {
				return;
			}
			_loading = true;

			_uri = uri;
			_url = uri.AbsoluteUri;
			_data = null;
			_loadedBytes = 0;
			_totalBytes = 0;
			try {
				client.DownloadDataAsync(uri);
			} catch (Exception ex) {
				Error?.Invoke(this, new ExceptionEventArgs(ex));
				return;
			}
		}
		public void Cancel() {
			if (!_loading) {
				return;
			}

			_url = null;
			client.CancelAsync();
			_loading = false;
		}

		public bool Loading {
			get {
				return _loading;
			}
		}
		public Uri Uri {
			get {
				return _uri;
			}
		}
		public string Url {
			get {
				return _url;
			}
		}
		public byte[] Data {
			get {
				return (byte[]) _data.Clone();
			}
		}

		public long LoadedBytes {
			get {
				return _loadedBytes;
			}
		}
		public long TotalBytes {
			get {
				return _totalBytes;
			}
		}

		public void Dispose() {
			if (client == null) {
				return;
			}

			client.Dispose();
			client = null;
		}

		//private
		private void onProgress(object sender, DownloadProgressChangedEventArgs e) {
			_loadedBytes = e.BytesReceived;
			_totalBytes = e.TotalBytesToReceive;
			Progress?.Invoke(this, new ProgressEventArgs((double) _loadedBytes, (double) _totalBytes));
		}
		private void onComplete(object sender, DownloadDataCompletedEventArgs e) {
			if (e.Error != null) {
				Error?.Invoke(this, new ExceptionEventArgs(e.Error));
				return;
			}
			if (e.Cancelled) {
				return;
			}
			_data = e.Result;
			_loading = false;
			Completed?.Invoke(this, new DataCompleteEventArgs((byte[]) _data.Clone()));
		}
	}
}
