using ISS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.IO;

namespace ISS
{
    public class GameMenu
    {
        private Texture2D texture;
        private EnumGameMenuType type;
        private bool active = false;
        private int selectedSize;
        private int selected;
        private float soundVolume;
        private float fXVolume;

        private Texture2D rect_select;
        private Texture2D rect_bar;

        Settings settings = Settings.Load();

        public GameMenu(EnumGameMenuType Type, int SelectedSize)
        {
            type = Type;
            selectedSize = SelectedSize;
            DefineType(Type);

            soundVolume = settings.soundVolume;
            fXVolume = settings.fxVolume;

            var rect_bar_size = new Vector2(20, 20);
            rect_select = new Texture2D(Globals.SpriteBatch.GraphicsDevice, (int)rect_bar_size.X, (int)rect_bar_size.Y);
            Color[] data_bar = new Color[(int)rect_bar_size.X * (int)rect_bar_size.Y];
            for (int i = 0; i < data_bar.Length; i++) data_bar[i] = Color.White;
            rect_select.SetData(data_bar);

            rect_bar_size = new Vector2(192, 8);
            rect_bar = new Texture2D(Globals.SpriteBatch.GraphicsDevice, (int)rect_bar_size.X, (int)rect_bar_size.Y);
            data_bar = new Color[(int)rect_bar_size.X * (int)rect_bar_size.Y];
            for (int i = 0; i < data_bar.Length; i++) data_bar[i] = Color.White;
            rect_bar.SetData(data_bar);
        }

        public void Update()
        {

        }

        public void Draw()
        {
            var position = new Vector2((Globals.ScreenSize.X / 2) - (texture.Width / 2), (Globals.ScreenSize.Y / 2) - (texture.Height / 2));
            Globals.SpriteBatch.Draw(texture, position, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.9999f);

            var rect_color = Color.Green;
            var selectedPosition = 75;
            if (selected == 0) selectedPosition *= 0;
            if (selected == 1) selectedPosition *= 1;
            if (selected == 2) selectedPosition *= 2;
            if (selected == 3) selectedPosition *= 3;
            var rect_pos_bar = new Vector2(position.X + 50, position.Y + 80 + selectedPosition);
            Globals.SpriteBatch.Draw(rect_select, rect_pos_bar, null, rect_color, 0, Vector2.Zero, new Vector2(1, 1), SpriteEffects.None, 1f);

            switch (type)
            {
                case EnumGameMenuType.Settings:
                    //var rect_width = volumeBarValue / 100;
                    if (soundVolume > 0)
                    {
                        rect_color = Color.Green;
                        if (soundVolume > 0.5f) rect_color = Color.Yellow;
                        if (soundVolume > 0.7f) rect_color = Color.Orange;
                        if (soundVolume > 0.9f) rect_color = Color.Red;

                        rect_pos_bar = new Vector2(position.X + 156, position.Y + 85);
                        Globals.SpriteBatch.Draw(rect_bar, rect_pos_bar, null, rect_color, 0, Vector2.Zero, new Vector2(soundVolume, 1), SpriteEffects.None, 1f);
                    }

                    if (fXVolume > 0)
                    {
                        rect_color = Color.Green;
                        if (fXVolume > 0.5f) rect_color = Color.Yellow;
                        if (fXVolume > 0.7f) rect_color = Color.Orange;
                        if (fXVolume > 0.9f) rect_color = Color.Red;

                        rect_pos_bar = new Vector2(position.X + 156, position.Y + 85 + 75);
                        Globals.SpriteBatch.Draw(rect_bar, rect_pos_bar, null, rect_color, 0, Vector2.Zero, new Vector2(fXVolume, 1), SpriteEffects.None, 1f);
                    }
                    break;

                case EnumGameMenuType.MachineDefault:

                    break;
            }
        }

        public void DefineType(EnumGameMenuType Type)
        {
            type = Type;
            switch (Type)
            {
                case EnumGameMenuType.Settings:
                    texture = Globals.Content.Load<Texture2D>("Menu/Settings");
                    break;
                case EnumGameMenuType.MachineDefault:
                    texture = Globals.Content.Load<Texture2D>("Menu/MachineDefault");
                    break;
            }
        }

        public EnumGameMenuType GetMenuType()
        {
            return type;
        }

        public bool IsActive()
        {
            return active;
        }

        public void Activate(bool activate)
        {
            active = activate;
            if (!activate)
            {
                selected = 0;
            }
        }

        public void SelectNext()
        {
            selected++;
            if (selected > selectedSize) selected = 0;
        }

        public void SelectPrevious()
        {
            selected--;
            if (selected < 0) selected = selectedSize;
        }

        public int GetSelected()
        {
            return selected;
        }

        public void SettingsSoundVolume(float Value)
        {
            soundVolume = Value;
        }

        public float GetSettingsSoundVolume()
        {
            return soundVolume;
        }

        public void SettingsFXVolume(float Value)
        {
            fXVolume = Value;
        }

        public float GetSettingsFXVolume()
        {
            return fXVolume;
        }

        public void SaveSettings()
        {
            this.settings.fxVolume = fXVolume;
            this.settings.soundVolume = soundVolume;
            this.settings.Save();
        }
    }

    class Settings : GameSettings<Settings>
    {
        public float fxVolume = 1f;
        public float soundVolume = 1f;
    }

    class GameSettings<T> where T : new()
    {
        private const string DEFAULT_FILE_NAME = "settings.json";
        public void Save(string fileName = DEFAULT_FILE_NAME)
        {
            try
            {
                File.WriteAllText(fileName, JsonConvert.SerializeObject(this));
            }catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }

        public static void Save(T pSettings, string fileName = DEFAULT_FILE_NAME)
        {
            File.WriteAllText(fileName, JsonConvert.SerializeObject(pSettings));
        }

        public static T Load(string fileName = DEFAULT_FILE_NAME)
        {
            T t = new T();
            if (File.Exists(fileName))
                t = JsonConvert.DeserializeObject<T>(File.ReadAllText(fileName));
            return t;
        }
    }
}
