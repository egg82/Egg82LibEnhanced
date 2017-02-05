using Egg82LibEnhanced.Enums;
using Egg82LibEnhanced.Utils;
using System;

namespace Egg82LibEnhanced.Events {
	public class StickEventArgs : EventArgs {
		//vars
		public static readonly new StickEventArgs Empty = new StickEventArgs(XboxStickCode.None, new PrecisePoint());

		private XboxStickCode _code = XboxStickCode.None;
		private PrecisePoint _position = null;

		//constructor
		public StickEventArgs(XboxStickCode code, PrecisePoint position) {
			_code = code;
			_position = position;
		}

		//public
		public XboxStickCode Code {
			get {
				return _code;
			}
		}
		public PrecisePoint Position {
			get {
				return _position;
			}
		}

		//private

	}
}
