using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Windows.UI.Xaml;

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
        public static char Side => _side;

        public static bool interact_key_pressed = false;

        public void Update(Player player, SongManager songManager)
        {
            KeyboardState ks = Keyboard.GetState();
            BackgroundMovement = 0;
            _direction = Vector2.Zero;

            if ((ks.IsKeyDown(Keys.D) || GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed) && !player.Crouch)
            {
                _direction.X++;
                BackgroundMovement = -_BackgroundSpeed;
                _side = 'R';
            }
            else if ((ks.IsKeyDown(Keys.A) || GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed) && !player.Crouch)
            {
                _direction.X--;
                BackgroundMovement = +_BackgroundSpeed;
                _side = 'L';
            }

            player.Crouch = false;
            if (ks.IsKeyDown(Keys.S) || GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed && !player.Jump && !player.isFly())
            {
                player.Crouch = true;
            }

            if (ks.IsKeyDown(Keys.C) || GamePad.GetState(PlayerIndex.One).Triggers.Left > 0 && player.getEnergy() > 0)
            {
                player.setFly(GamePad.GetState(PlayerIndex.One).Triggers.Left);                
            }
            else
            {
                player.setFly(0);
            }

            player.Running = false;
            if ((ks.IsKeyDown(Keys.H) || GamePad.GetState(PlayerIndex.One).Buttons.X == ButtonState.Pressed) && !player.Crouch && player.Oxygen() > 0)
            {
                player.Running = true;
            }

            _BackgroundSpeed = 100f;
            if (player.isFly())
            {
                _BackgroundSpeed = 300f;
            }

            if (player.Running)
            {
                _BackgroundSpeed = 200f;
            }

            if ((ks.IsKeyDown(Keys.Space) || GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed) && !player.Jump && !player.Crouch && player.Grounded)
            {
                player.Jump = true;
            }

            if ((ks.IsKeyDown(Keys.I) || GamePad.GetState(PlayerIndex.One).Buttons.Y == ButtonState.Pressed) && !interact_key_pressed)
            {
                interact_key_pressed = true;
                player.Interaction();
            }
            if(ks.IsKeyDown(Keys.I) || GamePad.GetState(PlayerIndex.One).Buttons.Y == ButtonState.Released){
                interact_key_pressed = false;
            }

            if (ks.IsKeyDown(Keys.Up))
            {
                songManager.MediaPlayer_VolumePlus(true);
            }

            if (ks.IsKeyDown(Keys.Down))
            {
                songManager.MediaPlayer_VolumePlus(false);
            }

            //MouseLeftClicked = (Mouse.GetState().LeftButton == ButtonState.Pressed) && (_lastMouseState.LeftButton == ButtonState.Released);
            //MouseRightClicked = (Mouse.GetState().RightButton == ButtonState.Pressed) && (_lastMouseState.RightButton == ButtonState.Released);
            //_lastMouseState = Mouse.GetState();
        }
    }
}
