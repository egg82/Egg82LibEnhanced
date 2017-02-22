using Egg82LibEnhanced.Base;
using Egg82LibEnhanced.Engines;
using Egg82LibEnhanced.Patterns;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Test.Sprites;

namespace Test.States {
	public class PhysicsTestState : BaseState {
		//vars
		private ObjectPool<PhysicsCircleSprite> circleFactory = new ObjectPool<PhysicsCircleSprite>(new PhysicsCircleSprite(), 50);
		private List<PhysicsCircleSprite> circles = new List<PhysicsCircleSprite>();

		private Body[] borders = new Body[4];

		//constructor
		public PhysicsTestState() {

		}

		//public

		//private
		protected override void OnEnter() {
			borders[0] = createBorder(Window.Width, 1.0d);
			borders[0].Position = new Vector2(PhysicsEngine.ToMeters(Window.Width / 2.0d), PhysicsEngine.ToMeters(-0.5d));
			borders[1] = createBorder(Window.Width, 1.0d);
			borders[1].Position = new Vector2(PhysicsEngine.ToMeters(Window.Width / 2.0d), PhysicsEngine.ToMeters(Window.Height + 0.5d));
			borders[2] = createBorder(1.0d, Window.Height);
			borders[2].Position = new Vector2(PhysicsEngine.ToMeters(-0.5d), PhysicsEngine.ToMeters(Window.Height / 2.0d));
			borders[3] = createBorder(1.0d, Window.Height);
			borders[3].Position = new Vector2(PhysicsEngine.ToMeters(Window.Width + 0.5d), PhysicsEngine.ToMeters(Window.Height / 2.0d));

			while (circleFactory.NumFreeInstances > 0) {
				PhysicsCircleSprite sprite = circleFactory.GetObject();
				circles.Add(sprite);
				AddChild(sprite);
				sprite.CreateBody(Window.PhysicsWorld);
			}
		}
		protected override void OnExit() {
			while (circles.Count > 0) {
				PhysicsCircleSprite sprite = circles[0];
				circles.RemoveAt(0);
				sprite.DestroyBody(Window.PhysicsWorld);
				RemoveChild(sprite);
				circleFactory.ReturnObject(sprite);
			}
		}
		protected override void OnUpdate(double deltaTime) {
			
		}

		private Body createBorder(double width, double height) {
			Body b = BodyFactory.CreateRectangle(Window.PhysicsWorld, PhysicsEngine.ToMeters(width), PhysicsEngine.ToMeters(height), 1.0f);
			b.BodyType = BodyType.Static;
			b.AngularDamping = 0.0f;
			b.Restitution = 0.0f;
			b.Friction = 0.0f;
			b.LinearDamping = 0.0f;
			b.FixedRotation = true;
			b.SleepingAllowed = true;
			return b;
		}
	}
}
