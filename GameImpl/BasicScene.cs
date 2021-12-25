using GameImpl.Components;
using GameImpl.Core;
using GameImpl.Entities;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Nez.Tiled;
using System;
using System.IO;
using System.Linq;

namespace GameImpl {
    public class BasicScene : SampleScene {
        public BasicScene() : base(true, true) { }

        public override void Initialize() {
            // setup a pixel perfect screen that fits our map
            SetDesignResolution(640, 480, SceneResolutionPolicy.ShowAllPixelPerfect);
            Screen.SetSize(640 * 2, 480 * 2);

            // load up our TiledMap
            var map = Content.LoadTiledMap(GameImpl.Content.Files.TiledMap);

            var tiledEntity = CreateEntity("tiled-map-entity");
            tiledEntity.AddComponent(new TiledMapRenderer(map, "main"));

            var topLeft = new Vector2(0, 0);
            var bottomRight = new Vector2(map.TileWidth * map.Width, map.TileWidth * map.Height);

            // optional:
            tiledEntity.AddComponent(new CameraBounds(topLeft, bottomRight));

            // create our Player and add a TiledMapMover to handle collisions with the tilemap
            var playerEntity = CreateEntity("player", new Vector2(100, -100));
            playerEntity.AddComponent(new Player());
            playerEntity.AddComponent(new BoxCollider(-8, -8, 16, 16));
            playerEntity.AddComponent(new TiledMapMover(map.GetLayer<TmxLayer>("main")));

            Camera.Entity.AddComponent(new FollowCamera(playerEntity));
        }
    }
}
