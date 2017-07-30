using Egg82LibEnhanced.Reflection.ExceptionHandlers.Builders;
using System;
using System.Collections.Generic;

namespace Egg82LibEnhanced.Reflection.ExceptionHandlers {
	public class NullExceptionHandler : IExceptionHandler {
		//vars
		private SynchronizedCollection<Exception> exceptions = new SynchronizedCollection<Exception>();
		private SynchronizedCollection<AppDomain> domains = new SynchronizedCollection<AppDomain>();

		//constructor
		public NullExceptionHandler() {
			AddDomain(AppDomain.CurrentDomain);
		}

		//public
		public void Connect(IBuilder builder) {
			throw new NotImplementedException("This API does not support exceptions.");
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
			exceptions.Add(ex);
		}
	}
}
