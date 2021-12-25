using GameImpl.Entities;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Tiled;
using NUnit.Framework;
using System;

// The tests here use quite large speeds so we can avoid floating point math
namespace GameImpl.Tests.Physics {
    [TestFixture]
    public class RigidBodyTests {
        readonly TiledMapMover.CollisionState characterOnGround = new TiledMapMover.CollisionState {
            Below = true
        };
        readonly TiledMapMover.CollisionState characterInAir = new TiledMapMover.CollisionState {
            Below = false
        };

        private RigidBody CreateBody(Vector2 velocity) {
            return new RigidBody(450, 50, 10) {
                velocity = velocity,
                maxXSpeed = 6000,
                brakeMultiplier = 3
            };
        }

        [Test]
        public void Gravity() {
            var expectedVelocityBeforeUpdate = Vector2.Zero;
            var expectedVelocityWithOneUpdate = new Vector2(0, 300 * 100);
            var expectedVelocityWithTwoUpdates = new Vector2(0, 2 * 300 * 100);

            Time.DeltaTime = 100;
            var body = new RigidBody(450, 50, 10) {
                gravity = 300
            };

            Assert.AreEqual(expectedVelocityBeforeUpdate, body.velocity);
            (body as IUpdatable).FixedUpdate();
            Assert.AreEqual(expectedVelocityWithOneUpdate, body.velocity);
            (body as IUpdatable).FixedUpdate();
            Assert.AreEqual(expectedVelocityWithTwoUpdates, body.velocity);
        }

        private void VelocityTest(Vector2 before, Action<RigidBody> action, Vector2 after) {
            Time.DeltaTime = 100;
            var body = CreateBody(before);

            Assert.AreEqual(before, body.velocity);
            action(body);
            Assert.AreEqual(after, body.velocity);
        }

        [Test]
        public void FloorNonMaxRight() {
            VelocityTest(
                    before: Vector2.Zero,
                    action: body => body.AccelRight(characterOnGround),
                    after: new Vector2(50 * 100, 0));
        }

        [Test]
        public void FloorMaxAccelRight() {
            VelocityTest(
                    before: new Vector2(50 * 100, 0),
                    action: body => body.AccelRight(characterOnGround),
                    after: new Vector2(6000, 0));
        }

        [Test]
        public void FloorNonMaxAccelLeft() {
            VelocityTest(
                    before: Vector2.Zero,
                    action: body => body.AccelLeft(characterOnGround),
                    after: new Vector2(-50 * 100, 0));
        }

        [Test]
        public void FloorMaxAccelLeft() {
            VelocityTest(
                    before: new Vector2(-50 * 100, 0),
                    action: body => body.AccelLeft(characterOnGround),
                    after: new Vector2(-6000, 0));
        }

        [Test]
        public void SlowDownPos() {
            VelocityTest(
                    before: new Vector2(18000, 0),
                    action: body => body.SlowDown(characterOnGround),
                    after: new Vector2(3000, 0));
        }

        [Test]
        public void SlowDownPosToZero() {
            VelocityTest(
                    before: new Vector2(3000, 0),
                    action: body => body.SlowDown(characterOnGround),
                    after: Vector2.Zero);
        }

        [Test]
        public void SlowDownNeg() {
            VelocityTest(
                    before: new Vector2(-18000, 0),
                    action: body => body.SlowDown(characterOnGround),
                    after: new Vector2(-3000, 0));
        }

        [Test]
        public void SlowDownNegToZero() {
            VelocityTest(
                    before: new Vector2(-3000, 0),
                    action: body => body.SlowDown(characterOnGround),
                    after: Vector2.Zero);
        }

        [Test]
        public void AirMaxAccelRight() {
            VelocityTest(
                    before: new Vector2(10 * 100, 0),
                    action: body => body.AccelRight(characterInAir),
                    after: new Vector2(2 * 10 * 100, 0));
        }

        [Test]
        public void TurnRateFromLeftToRight() {
            var before = new Vector2(-50 * 10, 0);
            var after = new Vector2(-50 * 10 + 3 * 50 * 10, 0);

            Time.DeltaTime = 10;
            var body = CreateBody(before);

            Assert.AreEqual(before, body.velocity);
            body.AccelRight(characterOnGround);
            Assert.AreEqual(after, body.velocity);
        }

        [Test]
        public void TurnRateFromRightToLeft() {
            var before = new Vector2(50 * 10, 0);
            var after = new Vector2(50 * 10 - 3 * 50 * 10, 0);

            Time.DeltaTime = 10;
            var body = CreateBody(before);

            Assert.AreEqual(before, body.velocity);
            body.AccelLeft(characterOnGround);
            Assert.AreEqual(after, body.velocity);
        }

        [Test]
        public void LockedVelocity() {
            var locked = new Vector2(50 * 10, 0);
            var afterLocked = new Vector2(50 * 10 / 2, 0);

            Time.DeltaTime = 10;
            var body = CreateBody(Vector2.Zero);

            body.lockedVelocity = locked;
            body.velocityLockedFor = 25;

            (body as IUpdatable).FixedUpdate();
            Assert.AreEqual(15, body.velocityLockedFor);
            Assert.AreEqual(locked, body.velocity);
            (body as IUpdatable).FixedUpdate();
            Assert.AreEqual(5, body.velocityLockedFor);
            Assert.AreEqual(locked, body.velocity);
            (body as IUpdatable).FixedUpdate();
            Assert.AreEqual(0, body.velocityLockedFor);
            Assert.AreEqual(afterLocked, body.velocity);
        }

        [Test]
        public void LockedVelocityExactlyToZero() {
            var locked = new Vector2(50 * 10, 0);
            var afterLocked = new Vector2(50 * 10 / 2, 0);

            Time.DeltaTime = 10;
            var body = CreateBody(Vector2.Zero);

            body.lockedVelocity = locked;
            body.velocityLockedFor = 20;

            (body as IUpdatable).FixedUpdate();
            Assert.AreEqual(10, body.velocityLockedFor);
            Assert.AreEqual(locked, body.velocity);
            (body as IUpdatable).FixedUpdate();
            Assert.AreEqual(0, body.velocityLockedFor);
            Assert.AreEqual(afterLocked, body.velocity);
        }

        [Test]
        public void GroundCollision() {
            VelocityTest(
                    before: new Vector2(50, 50),
                    action: body => body.UpdateCollisions(characterOnGround),
                    after: new Vector2(50, 0));
        }

        [Test]
        public void LeftCollision() {
            TiledMapMover.CollisionState collisionToLeftWall = new TiledMapMover.CollisionState {
                Left = true
            };
            VelocityTest(
                    before: new Vector2(-50, 50),
                    action: body => body.UpdateCollisions(collisionToLeftWall),
                    after: new Vector2(0, 50));
        }

        [Test]
        public void LeftCollisionWhileMovingRight() {
            TiledMapMover.CollisionState collisionToLeftWall = new TiledMapMover.CollisionState {
                Left = true
            };
            VelocityTest(
                    before: new Vector2(50, 50),
                    action: body => body.UpdateCollisions(collisionToLeftWall),
                    after: new Vector2(50, 50));
        }

        [Test]
        public void RightCollision() {
            TiledMapMover.CollisionState collisionToRightWall = new TiledMapMover.CollisionState {
                Right = true
            };
            VelocityTest(
                    before: new Vector2(50, 50),
                    action: body => body.UpdateCollisions(collisionToRightWall),
                    after: new Vector2(0, 50));
        }

        [Test]
        public void RightCollisionWhileMovingLeft() {
            TiledMapMover.CollisionState collisionToRightWall = new TiledMapMover.CollisionState {
                Right = true
            };
            VelocityTest(
                    before: new Vector2(-50, 50),
                    action: body => body.UpdateCollisions(collisionToRightWall),
                    after: new Vector2(-50, 50));
        }

        [Test]
        public void SlopeLeft() {
            TiledMapMover.CollisionState slopeLeftCollision = new TiledMapMover.CollisionState {
                Left = true,
                Below = true,
                SlopeAngle = 45,
            };
            VelocityTest(
                    before: new Vector2(-50, 50),
                    action: body => body.UpdateCollisions(slopeLeftCollision),
                    after: new Vector2(-50, 0));
        }

        [Test]
        public void SlopeRight() {
            TiledMapMover.CollisionState slopeRightCollision = new TiledMapMover.CollisionState {
                Right = true,
                Below = true,
                SlopeAngle = 45,
            };
            VelocityTest(
                    before: new Vector2(50, 50),
                    action: body => body.UpdateCollisions(slopeRightCollision),
                    after: new Vector2(50, 0));
        }

        [Test]
        public void CeilingCollision() {
            TiledMapMover.CollisionState ceilingCollision = new TiledMapMover.CollisionState {
                Above = true,
            };
            VelocityTest(
                    before: new Vector2(0, -50),
                    action: body => body.UpdateCollisions(ceilingCollision),
                    after: new Vector2(0, 0));
        }
    }
}
