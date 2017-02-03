using Egg82LibEnhanced.Utils;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Egg82LibEnhanced.Engines {
	public class PhysicsEngine : IPhysicsEngine {
		//vars
		private const double BASE_ACCURACY = 64.0d;

		private double _simulationAccuracy = 1.0d;
		private double _speed = 1.0d;

		private List<World> worlds = new List<World>();
		//private List<PhysicsWorld> worlds = new List<PhysicsWorld>();

		//constructor
		public PhysicsEngine() {
			ConvertUnits.SetDisplayUnitToSimUnitRatio((float) (_simulationAccuracy * BASE_ACCURACY));
		}
		~PhysicsEngine() {
			for (int i = 0; i < worlds.Count; i++) {
				worlds[i].BodyList.Clear();
				//worlds[i].RemoveAllBodies();
			}
			worlds.Clear();
		}

		//public
		public double Speed {
			get {
				return _speed;
			}
			set {
				if (double.IsInfinity(value) || double.IsNaN(value)) {
					return;
				}
				if (value < 0) {
					throw new Exception("Speed cannot be less than zero.");
				}
				_speed = value;
			}
		}
		public double SimulationAccuracy {
			get {
				return _simulationAccuracy;
			}
			set {
				if (double.IsInfinity(value) || double.IsNaN(value)) {
					return;
				}
				_simulationAccuracy = MathUtil.Clamp(0.0d, 1.0d, value);
				ConvertUnits.SetDisplayUnitToSimUnitRatio((float) (_simulationAccuracy * BASE_ACCURACY));
			}
		}

		public World CreateWorld() {
			World world = new World(new Vector2(0.0f, 0.0f));
			worlds.Add(world);
			return world;
		}
		/*public PhysicsWorld CreateWorld() {
			PhysicsWorld world = new PhysicsWorld(new Vector2(0.0f, 9.82f));
			worlds.Add(world);
			return world;
		}*/
		public void RemoveWorld(World world) {
			if (world == null) {
				throw new ArgumentNullException("world");
			}
			worlds.Remove(world);
		}
		/*public void RemoveWorld(PhysicsWorld world) {
			if (world == null) {
				throw new ArgumentNullException("world");
			}
			worlds.Remove(world);
		}*/

		public void Update(double deltaTime) {
			if (deltaTime == 0.0d || worlds.Count == 0) {
				return;
			}

			float dt = (float) (deltaTime * _speed);
			for (int i = 0; i < worlds.Count; i++) {
				worlds[i].Step(dt);
			}
		}

		public static double ToPixels(float meters) {
			return (double) ConvertUnits.ToDisplayUnits(meters);
		}
		public static float ToMeters(double pixels) {
			return ConvertUnits.ToSimUnits(pixels);
		}

		//private

	}
}
