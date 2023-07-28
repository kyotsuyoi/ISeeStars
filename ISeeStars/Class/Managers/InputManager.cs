using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ISS
{
    public class InputManager
    {
        private static float _BackgroundSpeed = 100f;
        private static Vector2 _direction;
        private static char _side;

        public static Vector2 Direction => _direction;
        public static bool Moving => _direction != Vector2.Zero;
        public float BackgroundMovement { get; set; }
        public static bool Jump { get; set; }
        public static bool Crouch { get; set; }
        public static bool Running { get; set; }
        public static char Side => _side;

        public static int JumpDelay { get; set; }

        public void Update()
        {
            KeyboardState ks = Keyboard.GetState();
            BackgroundMovement = 0;
            _direction = Vector2.Zero;

            if ((ks.IsKeyDown(Keys.D) || GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed) && !Crouch)
            {
                _direction.X++;
                BackgroundMovement = -_BackgroundSpeed;
                _side = 'R';
            }
            else if (ks.IsKeyDown(Keys.A) || GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed && !Crouch)
            {
                _direction.X--;
                BackgroundMovement = +_BackgroundSpeed;
                _side = 'L';
            }

            Crouch = false;
            if (ks.IsKeyDown(Keys.S) || GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed)
            {
                Crouch = true;
            }

            Running = false;
            _BackgroundSpeed = 100f;
            if (ks.IsKeyDown(Keys.H) || GamePad.GetState(PlayerIndex.One).Buttons.X == ButtonState.Pressed && !Crouch)
            {
                Running = true;
                _BackgroundSpeed = 200f;
            }

            if (ks.IsKeyDown(Keys.Space) || GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed && JumpDelay <= 0 && !Crouch)
            {
                Jump = true;
                JumpDelay=100;
            }

            JumpDelay--;
        }
    }
}
