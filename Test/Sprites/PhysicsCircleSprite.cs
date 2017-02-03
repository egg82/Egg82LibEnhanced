using Egg82LibEnhanced.Base;
using Egg82LibEnhanced.Engines;
using Egg82LibEnhanced.Patterns.Prototypes;
using Egg82LibEnhanced.Utils;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using SFML.Graphics;
using System;
using System.IO;

namespace Test.Sprites {
	class PhysicsCircleSprite : BaseSprite, IPrototype {
		//vars
		private Body physicsBody = null;
		private float minSpeed = 180.0f;
		private float maxSpeed = 200.0f;

		//constructor
		public PhysicsCircleSprite(Texture tex = null) {
			if (tex != null) {
				Texture = tex;
			} else {
				Texture = new Texture(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "Images" + Path.DirectorySeparatorChar + "ball.png");
				Texture.Smooth = true;
			}
			TransformOriginX = 64.0d;
			TransformOriginY = 64.0d;
		}

		//public
		public void CreateBody(World world) {
			ScaleX = ScaleY = 0.5d;
			physicsBody = BodyFactory.CreateCircle(world, PhysicsEngine.ToMeters(128.0d / 4.0d), 0.003f, new Vector2(PhysicsEngine.ToMeters(MathUtil.Random(GlobalWidth / 2.0d, Window.Width - GlobalWidth / 2.0d)), PhysicsEngine.ToMeters(MathUtil.Random(GlobalHeight / 2.0d, Window.Height - GlobalHeight / 2.0d))));
			physicsBody.BodyType = BodyType.Dynamic;
			physicsBody.AngularDamping = 0.0f;
			physicsBody.Restitution = 1.0f;
			physicsBody.Friction = 0.0f;
			physicsBody.LinearDamping = 0.0f;
			physicsBody.FixedRotation = false;
			physicsBody.SleepingAllowed = true;

			physicsBody.ApplyAngularImpulse((float) MathUtil.Random(-1.0d, 1.0d));
			physicsBody.ApplyLinearImpulse(new Vector2((float) MathUtil.Random(-1.0d, 1.0d), (float) MathUtil.Random(-1.0d, 1.0d)));
		}
		public void DestroyBody(World world) {
			world.RemoveBody(physicsBody);
		}

		public IPrototype Clone() {
			return new PhysicsCircleSprite(Texture);
		}

		//private
		protected override void OnUpdate(double deltaTime) {
			if (physicsBody == null) {
				return;
			}
			
			Vector2 newVelocity = new Vector2(physicsBody.LinearVelocity.X, physicsBody.LinearVelocity.Y);
			if (newVelocity.X < minSpeed && newVelocity.X > minSpeed * -1.0f) {
				if (newVelocity.X < 0.0f) {
					newVelocity.X -= (minSpeed - Math.Abs(newVelocity.X)) / 10.0f;
				} else {
					newVelocity.X += (minSpeed - newVelocity.X) / 10.0f;
				}
			} else if (newVelocity.X > maxSpeed) {
				newVelocity.X += (maxSpeed - newVelocity.X) / 10.0f;
			} else if (newVelocity.X < maxSpeed * -1.0f) {
				newVelocity.X -= (maxSpeed - (newVelocity.X * -1.0f)) / 10.0f;
			}
			if (newVelocity.Y < minSpeed && newVelocity.Y > minSpeed * -1.0f) {
				if (newVelocity.Y < 0.0f) {
					newVelocity.Y -= (minSpeed - Math.Abs(newVelocity.Y)) / 10.0f;
				} else {
					newVelocity.Y += (minSpeed - newVelocity.Y) / 10.0f;
				}
			} else if (newVelocity.Y > maxSpeed) {
				newVelocity.Y += (maxSpeed - newVelocity.Y) / 10.0f;
			} else if (newVelocity.Y < maxSpeed * -1.0f) {
				newVelocity.Y -= (maxSpeed - (newVelocity.Y * -1.0f)) / 10.0f;
			}
			physicsBody.LinearVelocity = newVelocity;

			X = PhysicsEngine.ToPixels(physicsBody.Position.X);
			Y = PhysicsEngine.ToPixels(physicsBody.Position.Y);
			Rotation = physicsBody.Rotation;
		}
	}
}
