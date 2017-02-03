using FarseerPhysics.Dynamics;
using System;

namespace Egg82LibEnhanced.Events {
	public class PhysicsCollideEventArgs : EventArgs {
		//vars
		public static readonly new PhysicsCollideEventArgs Empty = new PhysicsCollideEventArgs(null, null);

		private Body _body1 = null;
		private Body _body2 = null;

		//constructor
		public PhysicsCollideEventArgs(Body body1, Body body2) {
			_body1 = body1;
			_body2 = body2;
		}

		//public
		public Body Body1 {
			get {
				return _body1;
			}
		}
		public Body Body2 {
			get {
				return _body2;
			}
		}

		//private

	}
}
