using FarseerPhysics.Dynamics;
using System;

namespace Egg82LibEnhanced.Engines {
	public interface IPhysicsEngine {
		//functions
		double Speed { get; set; }
		double SimulationAccuracy { get; set; }

		World CreateWorld();
		//PhysicsWorld CreateWorld();
		void RemoveWorld(World world);
		//void RemoveWorld(PhysicsWorld world);
		void Update(double deltaTime);
	}
}
