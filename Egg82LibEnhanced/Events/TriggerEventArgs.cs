using Egg82LibEnhanced.Enums;
using System;

namespace Egg82LibEnhanced.Events {
	public class TriggerEventArgs : EventArgs {
		//vars
		public static readonly new TriggerEventArgs Empty = new TriggerEventArgs(XboxTriggerCode.None, 0.0d);

		private XboxTriggerCode _code = XboxTriggerCode.None;
		private double _position = 0.0d;

		//constructor
		public TriggerEventArgs(XboxTriggerCode code, double position) {
			_code = code;
			_position = position;
		}

		//public
		public XboxTriggerCode Code {
			get {
				return _code;
			}
		}
		public double Position {
			get {
				return _position;
			}
		}

		//private

	}
}
