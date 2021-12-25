


namespace GameImpl
{
    class Content
    {
		public static class Files
		{
#if WINDOWS
			public const string Tileset = @"Content\Files\tileset.png";
#else
			public const string Tileset = @"Content/Files/tileset.png";
#endif
#if WINDOWS
			public const string Player = @"Content\Files\player.png";
#else
			public const string Player = @"Content/Files/player.png";
#endif
#if WINDOWS
			public const string TiledMap = @"Content\Files\tiledMap.tmx";
#else
			public const string TiledMap = @"Content/Files/tiledMap.tmx";
#endif
		}


    }
}



