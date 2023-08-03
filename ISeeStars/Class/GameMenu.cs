using ISS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ISS
{
    public class GameMenu
    {
        private Texture2D texture; 
        private readonly int type;
        private bool active = false;
        private int selectedSize;
        private int selected;
        private float volumeBarValue;

        private Texture2D rect_select;
        private Texture2D rect_bar;

        public GameMenu(int Type, int SelectedSize, float VolumeBarValue) 
        {
            type = Type;
            selectedSize = SelectedSize;
            DefineType();

            volumeBarValue = VolumeBarValue;

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
            var position = new Vector2((Globals.ScreenSize.X / 2) - (texture.Width / 2 ), (Globals.ScreenSize.Y / 2) - (texture.Height / 2));
            Globals.SpriteBatch.Draw(texture, position, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.9999f);

            var rect_color = Color.Green;
            var selectedPosition = 75;
            if (selected == 0) selectedPosition *= 0;
            if (selected == 1) selectedPosition *= 1;
            if (selected == 2) selectedPosition *= 2;
            if (selected == 3) selectedPosition *= 3;
            var rect_pos_bar = new Vector2(position.X + 50, position.Y + 80 + selectedPosition);
            Globals.SpriteBatch.Draw(rect_select, rect_pos_bar, null, rect_color, 0, Vector2.Zero, new Vector2(1, 1), SpriteEffects.None, 1f);

            //var rect_width = volumeBarValue / 100;
            if (volumeBarValue > 0)
            {
                rect_color = Color.Green;
                if (volumeBarValue > 0.5f) rect_color = Color.Yellow;
                if (volumeBarValue > 0.7f) rect_color = Color.Orange;
                if (volumeBarValue > 0.9f) rect_color = Color.Red;

                rect_pos_bar = new Vector2(position.X + 156, position.Y + 85);
                Globals.SpriteBatch.Draw(rect_bar, rect_pos_bar, null, rect_color, 0, Vector2.Zero, new Vector2(volumeBarValue, 1), SpriteEffects.None, 1f);
            }
        }

        private void DefineType()
        {
            switch (type)
            {
                case 0:
                    texture = Globals.Content.Load<Texture2D>("menuConfig");
                    break;
                case 1:
                    texture = Globals.Content.Load<Texture2D>("menuMachineChange");
                    break;
            }
        }

        public int GetMenuType()
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

        public void SetSelected()
        {
            selected = 0;
        }

        public void SettingsVolumeBar(float Value)
        {
            volumeBarValue = Value;
        }
    }
}
