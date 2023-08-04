using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Windows.Devices.Input.Preview;

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
        public static bool menu_key_pressed = false;
        public static bool menu_keyup_pressed = false;
        public static bool menu_keydown_pressed = false;
        public static bool menu_keyselect_pressed = false;
        public static bool menu_keyleft_pressed = false;
        public static bool menu_keyright_pressed = false;

        public static int menu_selected;

        public static bool exit = false;

        //private GameMenu gameMenu;

        public void Update(Player player, SongManager songManager, GameMenu gameMenu)
        {
            KeyboardState keyboard = Keyboard.GetState();
            BackgroundMovement = 0;
            _direction = Vector2.Zero;

            MenuControl(keyboard, songManager, gameMenu, player);
            if (gameMenu.IsActive()) return;
            PlayerControl(keyboard, player, gameMenu);
        }

        private void PlayerControl(KeyboardState keyboard, Player player, GameMenu gameMenu)
        {
            //Movement L R
            if ((keyboard.IsKeyDown(Keys.D) || GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed) && !player.Crouch)
            {
                _direction.X++;
                BackgroundMovement = -_BackgroundSpeed;
                _side = 'R';
            }
            else if ((keyboard.IsKeyDown(Keys.A) || GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed) && !player.Crouch)
            {
                _direction.X--;
                BackgroundMovement = +_BackgroundSpeed;
                _side = 'L';
            }

            //Crouch
            player.Crouch = false;
            if (keyboard.IsKeyDown(Keys.S) || GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed && !player.Jump && !player.isFly())
            {
                player.Crouch = true;
            }

            //Fly
            if ((keyboard.IsKeyDown(Keys.U) || GamePad.GetState(PlayerIndex.One).Triggers.Left > 0) && player.getEnergy() > 0)
            {
                if(keyboard.IsKeyDown(Keys.U) && GamePad.GetState(PlayerIndex.One).Triggers.Left <= 0){
                    player.setFly(1);
                }
                else
                {
                    player.setFly(GamePad.GetState(PlayerIndex.One).Triggers.Left);
                }
            }
            else
            {
                player.setFly(0);
            }

            //Run
            player.Running = false;
            if ((keyboard.IsKeyDown(Keys.H) || GamePad.GetState(PlayerIndex.One).Buttons.X == ButtonState.Pressed) && !player.Crouch && player.Oxygen() > 0)
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

            //Jump
            if ((keyboard.IsKeyDown(Keys.Space) || GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed) && !player.Jump && !player.Crouch && player.Grounded)
            {
                player.Jump = true;
            }

            //Interaction
            if ((keyboard.IsKeyDown(Keys.I) || GamePad.GetState(PlayerIndex.One).Buttons.Y == ButtonState.Pressed) && !interact_key_pressed)
            {
                interact_key_pressed = true;
                if (player.Interaction() == EnumInteractionType.MachineDefault && player.getEnergy() >= 50f)
                {
                    gameMenu.Activate(true);
                    gameMenu.DefineType(EnumGameMenuType.MachineDefault);
                }
            }
            if (keyboard.IsKeyUp(Keys.I) && GamePad.GetState(PlayerIndex.One).Buttons.Y == ButtonState.Released) interact_key_pressed = false;            
        }

        private void MenuControl(KeyboardState keyboard, SongManager songManager, GameMenu gameMenu, Player player)
        {

            menu_selected = gameMenu.GetSelected();
            //MENU
            if ((keyboard.IsKeyDown(Keys.Escape) || GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed) && !menu_key_pressed)
            {
                if (gameMenu.IsActive() && gameMenu.GetMenuType() == EnumGameMenuType.Settings)
                {
                    gameMenu.Activate(false);
                }
                else
                {                    
                    gameMenu.Activate(true);
                    gameMenu.DefineType(EnumGameMenuType.Settings);
                }
                menu_key_pressed = true;
            }
            if (keyboard.IsKeyUp(Keys.Escape) && GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Released) menu_key_pressed = false;

            if (!gameMenu.IsActive()) return;

            //UP
            if ((keyboard.IsKeyDown(Keys.W) || GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed) && !menu_keyup_pressed)
            {
                menu_keyup_pressed = true;
                gameMenu.SelectPrevious();
            }
            if ((keyboard.IsKeyUp(Keys.W) && GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Released)) menu_keyup_pressed = false;

            //DOWN
            if ((keyboard.IsKeyDown(Keys.S) || GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed) && !menu_keydown_pressed)
            {
                menu_keydown_pressed = true;
                gameMenu.SelectNext();
            }
            if ((keyboard.IsKeyUp(Keys.S) && GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Released)) menu_keydown_pressed = false;

            //LEFT
            if ((keyboard.IsKeyDown(Keys.A) || GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed) /*&& !menu_keyleft_pressed*/)
            {
                menu_keyleft_pressed = true;
                if (gameMenu.GetMenuType() == EnumGameMenuType.Settings && gameMenu.GetSelected() == 0)
                {
                    songManager.MediaPlayer_VolumePlus(false);
                    gameMenu.SettingsVolumeBar(songManager.GetVolume());
                }
            }
            //if ((keyboard.IsKeyDown(Keys.A) || GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Released)) menu_keyleft_pressed = false;

            //RIGHT
            if ((keyboard.IsKeyDown(Keys.D) || GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed) /*&& !menu_keyright_pressed*/)
            {
                menu_keyright_pressed = true;

                if (gameMenu.GetMenuType() == EnumGameMenuType.Settings && gameMenu.GetSelected() == 0)
                {
                    songManager.MediaPlayer_VolumePlus(true);
                    gameMenu.SettingsVolumeBar(songManager.GetVolume());
                }
            }
            //if ((keyboard.IsKeyDown(Keys.D) || GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Released)) menu_keyright_pressed = false;

            //SELECT
            if ((keyboard.IsKeyDown(Keys.Enter) || GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed) && !menu_keyselect_pressed)
            {
                menu_keyselect_pressed = true;
                if (gameMenu.GetMenuType() == EnumGameMenuType.Settings && gameMenu.GetSelected() == 3)
                {
                    exit = true;
                }

                if (gameMenu.GetMenuType() == EnumGameMenuType.MachineDefault)
                {
                    switch (gameMenu.GetSelected())
                    {
                        case 0:
                            player.GameObjectInteract.DefineType(EnumGameObjectType.Health);
                            break;
                        case 1:
                            player.GameObjectInteract.DefineType(EnumGameObjectType.Oxygen);
                            break;
                        case 2:
                            player.GameObjectInteract.DefineType(EnumGameObjectType.Energy);
                            break;
                        case 3:
                            break;
                    }
                    player.setEnergy(50);
                    gameMenu.Activate(false);
                }
            }
            if ((keyboard.IsKeyDown(Keys.Enter) || GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Released)) menu_keyselect_pressed = false;
        }
    }
}
