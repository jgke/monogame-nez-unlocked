using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameImpl.Core {
    internal class GameObject {
        private Texture2D texture;
        private readonly string textureName;
        public Vector2 position;

        public GameObject(string textureName, Vector2 position) {
            this.textureName = textureName;
            this.position = position;
        }
        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content) {
            texture = content.Load<Texture2D>(textureName);
        }

        public void Draw(GameTime gametime, SpriteBatch spriteBatch) {
            spriteBatch.Draw(texture, position, Color.White);
        }
    }
}
