using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ISS
{
    public class InputManager
    {
        private readonly float _BackgroundSpeed = 100f;
        private static Vector2 _direction;
        private static char _side;

        public static Vector2 Direction => _direction;
        public static bool Moving => _direction != Vector2.Zero;
        public float BackgroundMovement { get; set; }
        public static bool Jump { get; set; }
        public static bool Crouch { get; set; }
        public static char Side => _side;

        public static int JumpDelay { get; set; }

        public void Update()
        {
            KeyboardState ks = Keyboard.GetState();
            BackgroundMovement = 0;
            _direction = Vector2.Zero;

            if (ks.IsKeyDown(Keys.D) && !Crouch)
            {
                _direction.X++;
                BackgroundMovement = -_BackgroundSpeed;
                _side = 'R';
            }
            else if (ks.IsKeyDown(Keys.A) && !Crouch)
            {
                _direction.X--;
                BackgroundMovement = +_BackgroundSpeed;
                _side = 'L';
            }
            
            if (ks.IsKeyDown(Keys.S))
            {
                Crouch = true;
            }
            else
            {
                Crouch = false;
            }

            if (ks.IsKeyDown(Keys.Space) && JumpDelay <= 0 && !Crouch)
            {
                Jump = true;
                JumpDelay=100;
            }

            JumpDelay--;
        }
    }
}
