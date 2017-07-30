using Egg82LibEnhanced.API.GameAnalytics;
using Egg82LibEnhanced.Patterns;
using Egg82LibEnhanced.Reflection.ExceptionHandlers.Builders;
using System;
using System.Collections.Generic;
using System.Timers;

namespace Egg82LibEnhanced.Reflection.ExceptionHandlers {
	public class GameAnalyticsExceptionHandler : IExceptionHandler {
		//vars
		private IGameAnalyticsAPI api = null;

		private SynchronizedCollection<Exception> exceptions = new SynchronizedCollection<Exception>();
		private SynchronizedCollection<AppDomain> domains = new SynchronizedCollection<AppDomain>();

		private Timer resendTimer = null;

		//functions
		public GameAnalyticsExceptionHandler() {

		}

		//public
		public void Connect(IBuilder builder) {
			string[] p = builder.GetParams();
			if (p == null || p.Length != 2) {
				throw new ArgumentException("params must have a length of 2. Use Egg82LibEnhanced.Reflection.ExceptionHandlers.Builders.GameAnalyticsBuilder");
			}

			api = ServiceLocator.GetService<IGameAnalyticsAPI>();
			if (api == null || api.GetGameKey() != p[0]) {
				api = new GameAnalyticsAPI();
				api.SendInit(p[0], p[1], 1);
			}
			
			foreach (Exception ex in exceptions) {
				api.SendError(ex.ToString());
			}
			exceptions.Clear();

			AddDomain(AppDomain.CurrentDomain);

			resendTimer = new Timer(60.0d * 60.0d * 1000.0d);
			resendTimer.Elapsed += onResendTimer;
			resendTimer.AutoReset = true;
			resendTimer.Start();
		}
		public void Disconnect() {
			while (domains.Count > 0) {
				RemoveDomain(domains[0]);
			}
		}

		public void AddDomain(AppDomain domain) {
			if (domain == null) {
				throw new ArgumentNullException("domain");
			}

			domain.UnhandledException += onException;
			domains.Add(domain);
		}
		public void RemoveDomain(AppDomain domain) {
			if (domain == null) {
				throw new ArgumentNullException("domain");
			}

			domain.UnhandledException -= onException;
			domains.Remove(domain);
		}

		public List<Exception> GetUnsentExceptions() {
			return new List<Exception>(exceptions);
		}
		public void SetUnsentExceptions(List<Exception> list) {
			exceptions.Clear();
			if (list == null) {
				return;
			}

			foreach (Exception ex in list) {
				exceptions.Add(ex);
			}

			if (api != null && api.IsInitialized()) {
				foreach (Exception ex in exceptions) {
					api.SendError(ex.ToString());
				}
				exceptions.Clear();
			}
		}

		public List<AppDomain> GetDomains() {
			return new List<AppDomain>(domains);
		}
		public void SetDomains(List<AppDomain> list) {
			while (domains.Count > 0) {
				RemoveDomain(domains[0]);
			}
			if (list == null) {
				return;
			}

			foreach (AppDomain d in list) {
				AddDomain(d);
			}
		}

		public bool IsLimitReached() {
			return false;
		}

		//private
		private void onException(object sender, UnhandledExceptionEventArgs e) {
			Exception ex = (Exception) e.ExceptionObject;
			api.SendError(ex.ToString());
		}

		private void onResendTimer(object sender, ElapsedEventArgs e) {
			foreach (Exception ex in exceptions) {
				api.SendError(ex.ToString());
			}
			exceptions.Clear();
		}
	}
}
