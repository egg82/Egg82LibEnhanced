using Egg82LibEnhanced.Events;
using Egg82LibEnhanced.Utils;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Egg82LibEnhanced.Core {
	public class PhysicsWorld {
		//vars
		public event EventHandler<PhysicsCollideEventArgs> Collided = null;
		
		private double _shapeAccuracy = 1.0d;
		private int factor = 0;
		private int[] primes = null;

		private World _world = null;
		private List<Body> naturalBodies = new List<Body>();
		private List<Body> clippedBodies = new List<Body>();

		//constructor
		public PhysicsWorld(Vector2 gravity) {
			_world = new World(gravity);
			_world.ContactManager.OnBroadphaseCollision += onCollision;
			primes = AtkinSieve.normalize(AtkinSieve.generate(32), 0, 32);
			refactor();
		}

		//public
		public double ShapeAccuracy {
			get {
				return _shapeAccuracy;
			}
			set {
				if (double.IsInfinity(value) || double.IsNaN(value)) {
					return;
				}
				_shapeAccuracy = MathUtil.Clamp(0.0d, 1.0d, value);
			}
		}

		public void AddBody(Body body) {
			if (naturalBodies.Contains(body)) {
				return;
			}
			naturalBodies.Add(body);
			Body newBody = body.DeepClone();
			clippedBodies.Add(newBody);
			if (factor != 0) {
				newBody.FixtureList.ForEach(checkVertices);
			}
			_world.BodyList.Add(newBody);
		}
		public void RemoveBody(Body body) {
			int index = naturalBodies.IndexOf(body);
			if (index == -1) {
				return;
			}

			naturalBodies.RemoveAt(index);
			clippedBodies.RemoveAt(index);
			_world.BodyList.Remove(body);
		}

		public void RemoveAllBodies() {
			naturalBodies.Clear();
			clippedBodies.Clear();
			_world.BodyList.Clear();
		}
		
		public int NumBodies {
			get {
				return naturalBodies.Count;
			}
		}

		public void Step(float dt) {
			if (dt == 0.0f || naturalBodies.Count == 0) {
				return;
			}
			_world.Step(dt);
		}

		public World World {
			get {
				return _world;
			}
		}

		//private
		private void refactor() {
			double factor = 0;
			double tempFactor = 0;

			if (_shapeAccuracy == 1.0d) {
				this.factor = 0;
				return;
			}

			factor = 1.0d / (1.0d - _shapeAccuracy);
			if (factor == Math.Floor(factor)) {
				this.factor = (int) Math.Floor(factor);
				return;
			}

			for (int i = 0; i < primes.Length; i++) {
				tempFactor = factor * primes[i];
				if (tempFactor == Math.Floor(tempFactor)) {
					this.factor = primes[i];
					return;
				}
			}

			this.factor = 0;
		}

		private void checkVertices(Fixture fixture) {
			Shape shape = fixture.Shape;
			if (fixture.ShapeType != ShapeType.Polygon) {
				return;
			}

			PolygonShape poly = (PolygonShape) shape;
			Vertices verts = poly.Vertices;

			if (verts.Count < 10) {
				return;
			}

			for (int i = verts.Count - 1; i >= 0; i--) {
				if (i % factor == 0) {
					verts.RemoveAt(i);
				}
			}
		}

		private void onCollision(ref FixtureProxy fixture1, ref FixtureProxy fixture2) {
			if (Collided != null) {
				Collided.Invoke(this, new PhysicsCollideEventArgs(naturalBodies[clippedBodies.IndexOf(fixture1.Fixture.Body)], naturalBodies[clippedBodies.IndexOf(fixture2.Fixture.Body)]));
			}
		}
	}
}
