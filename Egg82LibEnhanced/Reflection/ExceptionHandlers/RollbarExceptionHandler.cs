using Egg82LibEnhanced.Reflection.ExceptionHandlers.Builders;
using Egg82LibEnhanced.Reflection.ExceptionHandlers.Internal;
using Microsoft.Win32;
using RollbarDotNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Timers;

namespace Egg82LibEnhanced.Reflection.ExceptionHandlers {
	public class RollbarExceptionHandler : IExceptionHandler {
		//
		private LoggingRollbarClient rollbar = new LoggingRollbarClient();
		private SynchronizedCollection<AppDomain> domains = new SynchronizedCollection<AppDomain>();

		private Timer resendTimer = null;

		private Person person = null;
		private string version = null;

		//constructor
		public RollbarExceptionHandler() {
			string userId = null;
			
			try {
				userId = (string) RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey("SOFTWARE").OpenSubKey("Microsoft").OpenSubKey("Cryptography").GetValue("MachineGuid", "default");
			} catch (System.Exception) {

			}

			if (userId == null || userId == string.Empty || userId == "default") {
				userId = Guid.NewGuid().ToString();
				try {
					RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey("SOFTWARE").OpenSubKey("Microsoft").OpenSubKey("Cryptography").SetValue("MachineGuid", userId);
				} catch (System.Exception) {

				}
			}
			person = new Person(userId);

			try {
				version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
			} catch (System.Exception) {
				version = "unknown";
			}
		}

		//public
		public void Connect(IBuilder builder) {
			string[] p = builder.GetParams();
			if (p == null || p.Length != 2) {
				throw new ArgumentException("params must have a length of 2. Use Egg82LibEnhanced.Reflection.ExceptionHandlers.Builders.RollbarBuilder");
			}
			
			rollbar.Connect(new RollbarConfig {
				AccessToken = p[0],
				Environment = p[1]
			});

			List<System.Exception> exceptions = rollbar.GetUnsentExceptions();
			rollbar.ClearExceptions();
			foreach (System.Exception ex in exceptions) {
				rollbar.SetLastException(ex);
				rollbar.PostItem(new Payload(rollbar.Config.AccessToken, new Data(rollbar.Config.Environment, new Body(ex)) {
					Person = person,
					CodeVersion = version
				}));
			}

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

		public List<System.Exception> GetUnsentExceptions() {
			return rollbar.GetUnsentExceptions();
		}
		public void SetUnsentExceptions(List<System.Exception> list) {
			rollbar.SetUnsentExceptions(list);

			if (rollbar.Connected) {
				List<System.Exception> exceptions = rollbar.GetUnsentExceptions();
				rollbar.ClearExceptions();
				foreach (System.Exception ex in exceptions) {
					rollbar.SetLastException(ex);
					rollbar.PostItem(new Payload(rollbar.Config.AccessToken, new Data(rollbar.Config.Environment, new Body(ex)) {
						Person = person,
						CodeVersion = version
					}));
				}
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
			return rollbar.LimitReached;
		}

		//private
		private void onException(object sender, UnhandledExceptionEventArgs e) {
			System.Exception ex = (System.Exception) e.ExceptionObject;
			rollbar.SetLastException(ex);
			rollbar.PostItem(new Payload(rollbar.Config.AccessToken, new Data(rollbar.Config.Environment, new Body(ex)) {
				Person = person,
				CodeVersion = version
			}));
		}

		private void onResendTimer(object sender, ElapsedEventArgs e) {
			List<System.Exception> exceptions = rollbar.GetUnsentExceptions();
			rollbar.ClearExceptions();
			foreach (System.Exception ex in exceptions) {
				rollbar.SetLastException(ex);
				rollbar.PostItem(new Payload(rollbar.Config.AccessToken, new Data(rollbar.Config.Environment, new Body(ex)) {
					Person = person,
					CodeVersion = version
				}));
			}
		}
	}
}
