using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
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

        private List<EnumSoundFX> soundFXes = new List<EnumSoundFX>();
        private List<EnumSoundFX> soundFXesI = new List<EnumSoundFX>();
        private List<EnumSoundFX> soundFXesIStop = new List<EnumSoundFX>();

        public static bool jump_key_pressed = false;
        public static bool fly_key_pressed = false;

        //private GameMenu gameMenu;

        public void Update(Player player, SoundManager songManager, GameMenu gameMenu)
        {
            KeyboardState keyboard = Keyboard.GetState();
            BackgroundMovement = 0;
            _direction = Vector2.Zero;

            MenuControl(keyboard, songManager, gameMenu, player);
            if (gameMenu.IsActive())return;
            PlayerControl(keyboard, player, gameMenu);
        }

        private void PlayerControl(KeyboardState keyboard, Player player, GameMenu gameMenu)
        {
            if (!player.IsAlive())
            {
                fly_key_pressed = false;
                if (soundFXesIStop.Contains(EnumSoundFX.JetPack)) return;
                soundFXesIStop.Add(EnumSoundFX.JetPack);
                return;
            }
            //Movement L R
            if ((keyboard.IsKeyDown(Keys.D) || GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed) && !player.Crouch && !player.ObstructionRight)
            {
                _direction.X++;
                BackgroundMovement = -_BackgroundSpeed;
                _side = 'R';
            }
            else if ((keyboard.IsKeyDown(Keys.A) || GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed) && !player.Crouch && !player.ObstructionLeft)
            {
                _direction.X--;
                BackgroundMovement = +_BackgroundSpeed;
                _side = 'L';
            }

            //Crouch
            player.Crouch = false;
            if ((keyboard.IsKeyDown(Keys.S) || GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed) && !player.Jump && !player.IsFlying())
            {
                player.Crouch = true;
            }

            //Fly
            if ((keyboard.IsKeyDown(Keys.U) || GamePad.GetState(PlayerIndex.One).Triggers.Left > 0.7f) && player.getEnergy() > 0)
            {
                if(keyboard.IsKeyDown(Keys.U) && GamePad.GetState(PlayerIndex.One).Triggers.Left <= 0){
                    player.SetFly(1);
                }
                else
                {
                    player.SetFly(GamePad.GetState(PlayerIndex.One).Triggers.Left);
                }
            }
            else
            {
                player.SetFly(0);
            }

            if ((keyboard.IsKeyDown(Keys.U) || GamePad.GetState(PlayerIndex.One).Triggers.Left > 0.7f) && player.getEnergy() > 0 && !fly_key_pressed && !player.IsHurt())
            {
                fly_key_pressed = true;
                soundFXesI.Add(EnumSoundFX.JetPack);
            }
            if (keyboard.IsKeyUp(Keys.U) && GamePad.GetState(PlayerIndex.One).Triggers.Left <= 0)
            {
                fly_key_pressed = false;
                if (soundFXesIStop.Contains(EnumSoundFX.JetPack)) return;
                soundFXesIStop.Add(EnumSoundFX.JetPack);
            }

            //Run
            player.Running = false;
            if ((keyboard.IsKeyDown(Keys.H) || GamePad.GetState(PlayerIndex.One).Buttons.X == ButtonState.Pressed) && !player.Crouch && player.GetOxygen() > 0)
            {
                player.Running = true;
            }

            _BackgroundSpeed = 100f;
            if (player.IsFlying())
            {
                _BackgroundSpeed = 300f;
            }

            if (player.Running)
            {
                _BackgroundSpeed = 200f;
            }

            //Jump
            if ((keyboard.IsKeyDown(Keys.Space) || GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed) && 
                !player.Jump && !player.Crouch && player.Grounded && !jump_key_pressed && !player.IsHurt())
            {
                player.Jump = true;
                jump_key_pressed = true;
                soundFXes.Add(EnumSoundFX.Jump);
            }
            if (keyboard.IsKeyUp(Keys.Space) && GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Released) jump_key_pressed = false;

            //Interaction

            if ((keyboard.IsKeyDown(Keys.I) || GamePad.GetState(PlayerIndex.One).Buttons.Y == ButtonState.Pressed) && !interact_key_pressed)
            {
                interact_key_pressed = true;

                var interactType = player.Interaction();
                if (interactType == EnumInteractionType.MachineDefault && player.getEnergy() < 50f) soundFXes.Add(EnumSoundFX.MenuNotOpen);
                else if (interactType == EnumInteractionType.MachineDefault && player.getEnergy() >= 50f)
                {
                    soundFXes.Add(EnumSoundFX.MenuOpen);
                    gameMenu.Activate(true);
                    gameMenu.DefineType(EnumGameMenuType.MachineDefault);
                }

            }
            if (keyboard.IsKeyUp(Keys.I) && GamePad.GetState(PlayerIndex.One).Buttons.Y == ButtonState.Released) interact_key_pressed = false;
        }

        private void MenuControl(KeyboardState keyboard, SoundManager songManager, GameMenu gameMenu, Player player)
        {
            menu_selected = gameMenu.GetSelected();
            //MENU
            if ((keyboard.IsKeyDown(Keys.Escape) || GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed) && !menu_key_pressed)
            {
                if (gameMenu.IsActive() && gameMenu.GetMenuType() == EnumGameMenuType.Settings)
                {
                    gameMenu.Activate(false);
                    soundFXes.Add(EnumSoundFX.MenuClose);
                    gameMenu.SaveSettings();
                }
                else
                {
                    soundFXes.Add(EnumSoundFX.MenuOpen);
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
                soundFXes.Add(EnumSoundFX.MenuNavigation);
                menu_keyup_pressed = true;
                gameMenu.SelectPrevious();
            }
            if ((keyboard.IsKeyUp(Keys.W) && GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Released)) menu_keyup_pressed = false;

            //DOWN
            if ((keyboard.IsKeyDown(Keys.S) || GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed) && !menu_keydown_pressed)
            {
                soundFXes.Add(EnumSoundFX.MenuNavigation);
                menu_keydown_pressed = true;
                gameMenu.SelectNext();
            }
            if ((keyboard.IsKeyUp(Keys.S) && GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Released)) menu_keydown_pressed = false;

            //LEFT
            if ((keyboard.IsKeyDown(Keys.A) || GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed) /*&& !menu_keyleft_pressed*/)
            {
                menu_keyleft_pressed = true;
                soundFXes.Add(EnumSoundFX.MenuNavigation);
                if (gameMenu.GetMenuType() == EnumGameMenuType.Settings)
                {
                    if(gameMenu.GetSelected() == 0)
                    {
                        songManager.SetBGMVolume(false);
                        gameMenu.SettingsSoundVolume(songManager.GetBGMVolume());
                    }
                    if (gameMenu.GetSelected() == 1)
                    {
                        songManager.SetFXVolume(false);
                        gameMenu.SettingsFXVolume(songManager.GetFXVolume());
                    }
                }
            }
            //if ((keyboard.IsKeyDown(Keys.A) || GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Released)) menu_keyleft_pressed = false;

            //RIGHT
            if ((keyboard.IsKeyDown(Keys.D) || GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed) /*&& !menu_keyright_pressed*/)
            {
                menu_keyright_pressed = true;
                soundFXes.Add(EnumSoundFX.MenuNavigation);
                if (gameMenu.GetMenuType() == EnumGameMenuType.Settings)
                {
                    if (gameMenu.GetSelected() == 0)
                    {
                        songManager.SetBGMVolume(true);
                        gameMenu.SettingsSoundVolume(songManager.GetBGMVolume());
                    }
                    if (gameMenu.GetSelected() == 1)
                    {
                        songManager.SetFXVolume(true);
                        gameMenu.SettingsFXVolume(songManager.GetFXVolume());
                    }
                }
            }
            //if ((keyboard.IsKeyDown(Keys.D) || GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Released)) menu_keyright_pressed = false;

            //SELECT
            if ((keyboard.IsKeyDown(Keys.Enter) || GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed) && !menu_keyselect_pressed)
            {
                menu_keyselect_pressed = true;
                soundFXes.Add(EnumSoundFX.MenuSelected);
                if (gameMenu.GetMenuType() == EnumGameMenuType.Settings)
                {
                    switch (gameMenu.GetSelected())
                    {
                        case 2:
                            player.Revive();
                            player.HealthRefill();
                            //player.OxygenRefill();
                            player.EnergyRefill();
                            gameMenu.Activate(false);
                            soundFXes.Add(EnumSoundFX.MenuClose);
                            jump_key_pressed = true;
                            break;
                        case 3:
                            exit = true;
                            break;
                    }
                }

                if (gameMenu.GetMenuType() == EnumGameMenuType.MachineDefault)
                {
                    switch (gameMenu.GetSelected())
                    {
                        case 0:
                            player.GameObjectInteract.DefineType(EnumGameObjectType.Health);
                            player.setEnergy(50);
                            break;
                        case 1:
                            player.GameObjectInteract.DefineType(EnumGameObjectType.Oxygen);
                            player.setEnergy(50);
                            break;
                        case 2:
                            player.GameObjectInteract.DefineType(EnumGameObjectType.Energy);
                            player.setEnergy(50);
                            break;
                        case 3:
                            break;
                    }
                    gameMenu.Activate(false);
                    soundFXes.Add(EnumSoundFX.MenuClose);
                    jump_key_pressed = true;
                }
            }
            if ((keyboard.IsKeyDown(Keys.Enter) || GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Released)) menu_keyselect_pressed = false;
        }

        public EnumSoundFX GetSoundFX()
        {
            if (soundFXes.Count == 0) return EnumSoundFX.None;
            var soundFX = soundFXes[0];
            soundFXes.Remove(soundFX);
            return soundFX;
        }

        public EnumSoundFX GetSoundFXInstance()
        {
            if (soundFXesI.Count == 0) return EnumSoundFX.None;
            var soundFX = soundFXesI[0];
            soundFXesI.Remove(soundFX);
            return soundFX;
        }

        public EnumSoundFX GetStopSoundFXInstance()
        {
            if (soundFXesIStop.Count == 0) return EnumSoundFX.None;
            var soundFX = soundFXesIStop[0];
            soundFXesIStop.Remove(soundFX);
            return soundFX;
        }
    }
}
