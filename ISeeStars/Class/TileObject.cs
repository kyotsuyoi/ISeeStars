using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ISS
{
    internal class TileObject
    {
        private Texture2D texture;
        public Vector2 Position;
        private readonly AnimationManager _anims = new AnimationManager();
        public Vector2 Origin { get; }

        public TileObject(Vector2 position) {
            texture = Globals.Content.Load<Texture2D>("objectBox");
            Origin = new Vector2(texture.Width / 2, texture.Height / 2);
            Position = position;
        }

        public void Update(float movement) {
            Position.X += (movement) * Globals.ElapsedSeconds;

            if (Position.Y + (texture.Height) > Globals.GroundLevel)
            {
                Position = new Vector2(Position.X, Globals.GroundLevel - (texture.Height));
            }

            if (Position.Y + texture.Height < Globals.GroundLevel)
            {
                Position.Y += Globals.Gravity;
            }
        }
        public void Draw() 
        {
            Globals.SpriteBatch.Draw(texture, Position, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.8f);
        }
    }
}
