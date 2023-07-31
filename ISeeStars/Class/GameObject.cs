using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ISS
{
    public class GameObject
    {
        private Texture2D texture;
        public Vector2 Position;
        public Vector2 Size;
        private readonly AnimationManager _anims = new AnimationManager();
        public Vector2 Origin { get; }
        private Vector2 lastScreenSize;
        public int Type;

        public GameObject(Vector2 position, int type)
        {
            Type = type;
            DefineType();
            Origin = new Vector2(texture.Width / 2, texture.Height / 2);
            Size = new Vector2(texture.Width, texture.Height);
            Position = position;
            lastScreenSize = Globals.ScreenSize;
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

            if(lastScreenSize != Globals.ScreenSize)
            {
                var dif = Globals.ScreenSize - lastScreenSize;
                Position.X += dif.X/2;
                lastScreenSize = Globals.ScreenSize;
            }
        }
        public void Draw() 
        {
            Globals.SpriteBatch.Draw(texture, Position, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.8f);
        }

        private void DefineType() 
        {
            switch (Type)
            {
                case 0:
                    texture = Globals.Content.Load<Texture2D>("objectBoxHealth");
                    break; 
                case 1:
                    texture = Globals.Content.Load<Texture2D>("objectBoxOxygen");
                    break;
                case 2:
                    texture = Globals.Content.Load<Texture2D>("objectBoxEnergy");
                    break;
            }
        }
    }
}
