﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ISS
{
    public class GameObject
    {
        private Texture2D texture;
        public Vector2 Position;
        public Vector2 Size;
        public Vector2 Origin { get; }
        private Vector2 lastScreenSize;
        public int Type;
        private float _refillValue;
        private int _refill = 4;
        //private int _refillBlink;

        private Texture2D rect_red = null;
        private Texture2D rect_green = null;
        private Texture2D rect_bar = null;

        public GameObject(Vector2 position, int type)
        {
            Type = type;
            DefineType();
            Origin = new Vector2(texture.Width / 2, texture.Height / 2);
            Size = new Vector2(texture.Width, texture.Height);
            Position = position;
            lastScreenSize = Globals.ScreenSize;

            var rect_size = new Vector2(8, 4);
            rect_red = new Texture2D(Globals.SpriteBatch.GraphicsDevice, (int)rect_size.X, (int)rect_size.Y);
            rect_green = new Texture2D(Globals.SpriteBatch.GraphicsDevice, (int)rect_size.X, (int)rect_size.Y);

            var data = new Color[(int)rect_size.X * (int)rect_size.Y];
            for (int i = 0; i < data.Length; i++) data[i] = Color.Red;
            rect_red.SetData(data);

            data = new Color[(int)rect_size.X * (int)rect_size.Y];
            for (int i = 0; i < data.Length; i++) data[i] = Color.Green;
            rect_green.SetData(data);

            var rect_bar_size = new Vector2(64, 4);
            rect_bar = new Texture2D(Globals.SpriteBatch.GraphicsDevice, (int)rect_bar_size.X, (int)rect_bar_size.Y);
            Color[] data_bar = new Color[(int)rect_bar_size.X * (int)rect_bar_size.Y];
            for (int i = 0; i < data_bar.Length; i++) data_bar[i] = Color.White;
            rect_bar.SetData(data_bar);
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
            UpdateLoadingBar(false);
        }
        public void Draw() 
        {
            Globals.SpriteBatch.Draw(texture, Position, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.8f);
            DrawLoadingBar();
            DrawRefillBar();
        }

        private void DefineType() 
        {
            switch (Type)
            {
                case 0:
                    texture = Globals.Content.Load<Texture2D>("objectMachineHealth");
                    break; 
                case 1:
                    texture = Globals.Content.Load<Texture2D>("objectMachineOxygen");
                    break;
                case 2:
                    texture = Globals.Content.Load<Texture2D>("objectMachineEnergy");
                    break;
            }
        }

        private void DrawLoadingBar()
        {
            if (Type == 0 || Type == 1 || Type == 2)
            {
                //var rect_width = (int)(64 * _refillValue / 100);
                var rect_width = _refillValue / 100;
                if (rect_width > 0)
                {
                    var rect_color = Color.Red;
                    if (_refillValue > 24) rect_color = Color.Orange;
                    if (_refillValue > 50) rect_color = Color.Yellow;
                    if (_refillValue > 75) rect_color = Color.Green;
                    //if (_powerValue > 99) rect_color = Color.Blue;

                    var rect_pos_bar = new Vector2(Position.X + 4, Position.Y + texture.Height - 16);
                    Globals.SpriteBatch.Draw(rect_bar, rect_pos_bar, null, rect_color, 0, Vector2.Zero, new Vector2(rect_width, 1), SpriteEffects.None, 0.81f);
                }             
            }          
        }

        private void DrawRefillBar()
        {            
            switch (_refill)
            {
                case 0:
                    var rect_pos = new Vector2(Position.X + 4, Position.Y + texture.Height - 28);
                    Globals.SpriteBatch.Draw(rect_red, rect_pos, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.81f);
                    break;

                case 1:
                    rect_pos = new Vector2(Position.X + 4, Position.Y + texture.Height - 28);
                    Globals.SpriteBatch.Draw(rect_green, rect_pos, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.81f);

                    rect_pos = new Vector2(Position.X + 20, Position.Y + texture.Height - 28);
                    Globals.SpriteBatch.Draw(rect_red, rect_pos, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.81f);
                    break;

                case 2:
                    rect_pos = new Vector2(Position.X + 4, Position.Y + texture.Height - 28);
                    Globals.SpriteBatch.Draw(rect_green, rect_pos, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.81f);

                    rect_pos = new Vector2(Position.X + 20, Position.Y + texture.Height - 28);
                    Globals.SpriteBatch.Draw(rect_green, rect_pos, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.81f);

                    rect_pos = new Vector2(Position.X + 44, Position.Y + texture.Height - 28);
                    Globals.SpriteBatch.Draw(rect_red, rect_pos, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.81f);
                    break;

                case 3:
                    rect_pos = new Vector2(Position.X + 4, Position.Y + texture.Height - 28);
                    Globals.SpriteBatch.Draw(rect_green, rect_pos, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.81f);

                    rect_pos = new Vector2(Position.X + 20, Position.Y + texture.Height - 28);
                    Globals.SpriteBatch.Draw(rect_green, rect_pos, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.81f);

                    rect_pos = new Vector2(Position.X + 44, Position.Y + texture.Height - 28);
                    Globals.SpriteBatch.Draw(rect_green, rect_pos, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.81f);

                    rect_pos = new Vector2(Position.X + 60, Position.Y + texture.Height - 28);
                    Globals.SpriteBatch.Draw(rect_red, rect_pos, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.81f);
                    break;

                case 4:
                    rect_pos = new Vector2(Position.X + 4, Position.Y + texture.Height - 28);
                    Globals.SpriteBatch.Draw(rect_green, rect_pos, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.81f);

                    rect_pos = new Vector2(Position.X + 20, Position.Y + texture.Height - 28);
                    Globals.SpriteBatch.Draw(rect_green, rect_pos, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.81f);

                    rect_pos = new Vector2(Position.X + 44, Position.Y + texture.Height - 28);
                    Globals.SpriteBatch.Draw(rect_green, rect_pos, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.81f);

                    rect_pos = new Vector2(Position.X + 60, Position.Y + texture.Height - 28);
                    Globals.SpriteBatch.Draw(rect_green, rect_pos, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.81f);
                    break;
            }
        }

        public void UpdateLoadingBar(bool update_type_0)
        {
            if (Type == 1)
            {
                if (Globals.Time < 360f || Globals.Time > 1080f)
                {
                    _refillValue += 0.01f;
                }
                else
                {
                    _refillValue += 0.005f;
                }
            }

            if (Type == 2)
            {
                if(Globals.Time > 360f && Globals.Time < 1080f)
                {
                    _refillValue += 0.01f;
                }
                if (Globals.Time > 540f && Globals.Time < 900f)
                {
                    _refillValue += 0.01f;
                }
                if (Globals.Time > 630f && Globals.Time < 810f)
                {
                    _refillValue += 0.01f;
                }
            }

            if (Type == 0 && update_type_0)
            {
                _refillValue += 0.1f;                
            }

            if (_refillValue >= 100 && _refill < 4)
            {
                _refill++;
                _refillValue = 0;
            }
            if (_refillValue >= 100) _refillValue = 100;
        }

        public bool ConsumeRefill()
        {
            if (_refill < 1) return false;
            _refill--;
            return true;
        }

        public bool IsFull()
        {
            if (_refill > 3 && _refillValue > 99) {
                return true;
            }
            return false;
        }
    }
}
