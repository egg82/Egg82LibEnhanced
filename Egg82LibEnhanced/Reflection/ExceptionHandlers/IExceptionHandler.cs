using Egg82LibEnhanced.Reflection.ExceptionHandlers.Builders;
using System;
using System.Collections.Generic;

namespace Egg82LibEnhanced.Reflection.ExceptionHandlers {
	public interface IExceptionHandler {
		//functions
		void Connect(IBuilder builder);
		void Disconnect();

		void AddDomain(AppDomain domain);
		void RemoveDomain(AppDomain domain);

		List<Exception> GetUnsentExceptions();
		void SetUnsentExceptions(List<Exception> list);
		List<AppDomain> GetDomains();
		void SetDomains(List<AppDomain> list);

		bool IsLimitReached();
	}
}
