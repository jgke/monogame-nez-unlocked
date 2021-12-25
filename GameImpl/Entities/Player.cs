using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Persistence;
using Nez.Sprites;
using Nez.Textures;
using Nez.Tiled;
using System;

namespace GameImpl.Entities {
    public class Player : Component, ITriggerListener, IUpdatable {
        public float JumpHeight = 16 * 6;
        float jumpLength = 0;

        SpriteAnimator _animator;
        //TiledMapMover _mover;
        TiledMapMover _mover;
        BoxCollider _boxCollider;
        TiledMapMover.CollisionState _collisionState = new TiledMapMover.CollisionState();
        readonly float groundAccel = 1000;
        readonly float airAccel = 750;
        RigidBody _rigidBody;

        VirtualButton _jumpInput;
        VirtualButton _climbInput;
        VirtualIntegerAxis _xAxisInput;
        VirtualIntegerAxis _yAxisInput;

        public override void OnAddedToEntity() {
            var texture = Entity.Scene.Content.LoadTexture(GameImpl.Content.Files.Player);
            var sprites = Sprite.SpritesFromAtlas(texture, 16, 16);

            _boxCollider = Entity.GetComponent<BoxCollider>();
            _mover = Entity.GetComponent<TiledMapMover>();
            _animator = Entity.AddComponent(new SpriteAnimator(sprites[0]));
            _rigidBody = Entity.AddComponent(new RigidBody(600, groundAccel, airAccel));

            var spritesheetWidth = 1;

            _animator.AddAnimation("Idle", new[]
            {
                sprites[0]
                // sprites[1], sprites[2] etc
            });

            _animator.AddAnimation("Run", new[]
            {
                sprites[spritesheetWidth + 0],
                // sprites[spritesheetWidth + 1] etc
            });

            _animator.AddAnimation("Falling", new[]
            {
                sprites[2 * spritesheetWidth + 0]
            });

            _animator.AddAnimation("Jumping", new[]
            {
                sprites[3 * spritesheetWidth + 0]
            });

            SetupInput();
        }

        public override void OnRemovedFromEntity() {
            // deregister virtual input
            _jumpInput.Deregister();
            _climbInput.Deregister();
            _xAxisInput.Deregister();
            _yAxisInput.Deregister();
        }

        void SetupInput() {
            // setup input for jumping. we will allow z on the keyboard or a on the gamepad
            _jumpInput = new VirtualButton();
            _jumpInput.Nodes.Add(new VirtualButton.KeyboardKey(Keys.A));
            _jumpInput.Nodes.Add(new VirtualButton.GamePadButton(0, Buttons.A));

            // setup input for climb
            _climbInput = new VirtualButton();
            _climbInput.Nodes.Add(new VirtualButton.KeyboardKey(Keys.O));
            _climbInput.Nodes.Add(new VirtualButton.GamePadButton(0, Buttons.B));

            // horizontal input from dpad, left stick or keyboard left/right
            _xAxisInput = new VirtualIntegerAxis();
            _xAxisInput.Nodes.Add(new VirtualAxis.GamePadDpadLeftRight());
            _xAxisInput.Nodes.Add(new VirtualAxis.GamePadLeftStickX());
            _xAxisInput.Nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.Left, Keys.Right));

            // horizontal input from dpad, left stick or keyboard left/right
            _yAxisInput = new VirtualIntegerAxis();
            _yAxisInput.Nodes.Add(new VirtualAxis.GamePadDpadUpDown());
            _yAxisInput.Nodes.Add(new VirtualAxis.GamePadLeftStickY());
            _yAxisInput.Nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.Up, Keys.Down));
        }

        void UpdateAnimation(Vector2 moveDir) {
            string animation = "Idle";
            if (moveDir.X < 0) {
                if (_collisionState.Below) {
                    animation = "Run";
                }
                _animator.FlipX = true;
            } else if (moveDir.X > 0) {
                if (_collisionState.Below) {
                    animation = "Run";
                }
                _animator.FlipX = false;
            }

            if (_jumpInput.IsDown && jumpLength > 0) {
                animation = "Jumping";
            }

            if (!_collisionState.Below && _rigidBody.velocity.Y > 0) {
                animation = "Falling";
            }

            if (!_animator.IsAnimationActive(animation)) {
                _animator.Play(animation);
            }
        }

        bool OnSlope() {
            return _collisionState.Below &&
                ((Math.Abs(_collisionState.SlopeAngle) >= 40 && _rigidBody.velocity.X > 350) ||
                 (Math.Abs(_collisionState.SlopeAngle) <= -40 && _rigidBody.velocity.X < -350));
        }

        bool CanJump() {
            return _collisionState.Below;
        }

        bool CanContinueJump() {
            return jumpLength > 0 && !_collisionState.Above;
        }

        void UpdateMovement(Vector2 moveDir) {
            if (moveDir.X < 0) {
                _rigidBody.AccelLeft(_collisionState);
            } else if (moveDir.X > 0) {
                _rigidBody.AccelRight(_collisionState);
            } else {
                _rigidBody.SlowDown(_collisionState);
            }

            if (CanJump() && _jumpInput.IsDown) {
                jumpLength = 0.25f;
                _rigidBody.velocity.Y = -Mathf.Sqrt(2f * JumpHeight * 1000);
            } else if (CanContinueJump() && _jumpInput.IsDown) {
                _rigidBody.velocity.Y = -Mathf.Sqrt(2f * JumpHeight * 1000);
                jumpLength -= Time.DeltaTime;
            } else {
                jumpLength = 0;
            }

            _mover.Move(_rigidBody.velocity * Time.DeltaTime, _boxCollider, _collisionState);
            _rigidBody.UpdateCollisions(_collisionState);
        }

        void IUpdatable.FixedUpdate() {
            // handle movement and animations
            var moveDir = new Vector2(_xAxisInput.Value, _yAxisInput.Value);

            UpdateAnimation(moveDir);
            UpdateMovement(moveDir);

            //Console.WriteLine("Phys {0} {1}",
            //        Entity.PreviousTransform.Position,
            //        Entity.Transform.Position);

        }

        void IUpdatable.DrawUpdate() {
            // handle movement and animations
            //Console.WriteLine(
            //        "Draw {0} {1} {2} {3}",
            //        Entity.PreviousTransform.Position,
            //        _animator.GraphicsTransform.Position,
            //        Entity.Transform.Position,
            //        Time.Alpha
            //        );
        }

        #region ITriggerListener implementation

        void ITriggerListener.OnTriggerEnter(Collider other, Collider self) {
            Debug.Log("triggerEnter: {0}", other.Entity.Name);
        }

        void ITriggerListener.OnTriggerExit(Collider other, Collider self) {
            Debug.Log("triggerExit: {0}", other.Entity.Name);
        }

        #endregion
    }
}
