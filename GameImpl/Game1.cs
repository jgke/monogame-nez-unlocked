using System;
using Nez;

namespace MyGame
{
    class Game1 : Core
    {
        public Game1()
        {
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            this.PhysicsFps = 5; // Change this to something sensible later
            TargetElapsedTime = TimeSpan.FromSeconds(1f / PhysicsFps);
            IsFixedTimeStep = true;
        }

        public int PhysicsFps { get; set; }

        protected override void Initialize()
        {
            base.Initialize();

            Scene = new DefaultScene();
        }
    }
}
