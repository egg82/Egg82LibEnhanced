using FarseerPhysics.Dynamics;
using System;

namespace Egg82LibEnhanced.Engines.Nulls {
	public class NullPhysicsEngine : IPhysicsEngine {
		//vars
		public double Speed { get; set; }
		public double SimulationAccuracy { get; set; }

		//constructor
		public NullPhysicsEngine() {

		}

		public World CreateWorld() {
			return null;
		}
		public void RemoveWorld(World world) {

		}
		public void Update(double deltaTime) {

		}
	}
}
