using Egg82LibEnhanced.Enums;
using System;

namespace Egg82LibEnhanced.Events {
	public class ButtonEventArgs : EventArgs {
		//vars
		public static readonly new ButtonEventArgs Empty = new ButtonEventArgs(XboxButtonCode.None);

		private XboxButtonCode _code = XboxButtonCode.None;

		//constructor
		public ButtonEventArgs(XboxButtonCode code) {
			_code = code;
		}

		//public
		public XboxButtonCode Code {
			get {
				return _code;
			}
		}

		//private

	}
}
