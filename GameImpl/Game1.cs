using GameImpl.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

namespace GameImpl {
    public class Game1 : Nez.Core {
        private int _drawFrames = 0;
        private int _physicsFrames = 0;
        Stopwatch _elapsedTime = Stopwatch.StartNew();

        public Game1() {
            float physicsFps = 70;
            this.TargetElapsedTime = TimeSpan.FromSeconds(1 / physicsFps);
            this.IsFixedTimeStep = true;
        }

        protected override void Initialize() {
            base.Initialize();
            DebugRenderEnabled = false;

            Window.AllowUserResizing = true;
            Scene = new BasicScene();
        }

        protected override void FixedUpdate(GameTime gameTime) {
            base.FixedUpdate(gameTime);
            _physicsFrames += 1;
        }

        protected override void DrawUpdate(GameTime gameTime) {
            base.DrawUpdate(gameTime);
            _drawFrames += 1;
            System.Threading.Thread.Sleep(5);

            if (_elapsedTime.ElapsedMilliseconds >= 1000) {
                Console.WriteLine("draw fps: {0}, physics fps: {1}", _drawFrames, _physicsFrames);
                Console.WriteLine("Running slowly: {0}", gameTime.IsRunningSlowly);
                _drawFrames = 0;
                _physicsFrames = 0;
                _elapsedTime.Restart();
            }
        }

        protected override void Draw(GameTime gameTime) {
            base.Draw(gameTime);
        }
    }
}
