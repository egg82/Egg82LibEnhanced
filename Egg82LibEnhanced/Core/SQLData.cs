using System;

namespace Egg82LibEnhanced.Core {
	public struct SQLData {
		public string[] columns;
		public object[,] data;
		public int recordsAffected;
	}
}
