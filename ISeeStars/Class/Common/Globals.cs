using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ISS
{
    public static class Globals
    {
        public static ContentManager Content { get; set; }
        public static SpriteBatch SpriteBatch { get; set; }
        public static float ElapsedSeconds { get; set; }
        public static float Gravity { get; set; }
        public static Vector2 ScreenSize { get; set; }
        public static float GroundLevel { get; set; }
        public static float Time { get; set; }

        public static void Update(GameTime gameTime, Vector2 screenSize)
        {
            ElapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            ScreenSize = screenSize;
            GroundLevel = ScreenSize.Y - 176;
        }
    }
}
