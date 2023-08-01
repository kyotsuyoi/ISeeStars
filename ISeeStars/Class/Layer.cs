using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ISS
{
    public class Layer
    {
        private readonly Texture2D _texture;
        private Vector2 _position;
        private Vector2 _position2;
        private Vector2 _position3;
        private Vector2 _position4;
        private readonly float _depth;
        private readonly float _moveScale;
        private readonly float _defaultSpeed;
        private readonly bool _movementY;

         private float _rotate = 0;

        public Layer(Texture2D texture, float depth, float moveScale, bool movementY, float defaultSpeed = 0.0f)
        {
            _texture = texture;
            _depth = depth;
            _moveScale = moveScale;
            _defaultSpeed = defaultSpeed;
            _position = Vector2.Zero; 
            _position2 = Vector2.Zero;
            _position3 = Vector2.Zero;
            _position4 = Vector2.Zero;

            _movementY = movementY;
        }

        public void Update(float movement)
        {
            if (_movementY)
            {
                //_rotate += 0.0001f;
                _rotate += 0.0001f;
                if (_rotate > 6.278f)
                {
                    _rotate = 0;
                }

                _position.Y = Globals.ScreenSize.Y*0.8f;
                _position.X = Globals.ScreenSize.X /2;

                Globals.Time = _rotate * 1440 / 6.278f;                
                return;
            }

            _position.X += (((movement * _moveScale) + _defaultSpeed) * Globals.ElapsedSeconds);
            _position.X %= _texture.Width;

            if (_position.X >= 0)
            {
                _position2.X = _position.X - _texture.Width;
                _position3.X = _position.X + _texture.Width;

                _position4.X = _position.X - _texture.Width*2;
            }
            else
            {
                _position2.X = _position.X + _texture.Width;
                _position3.X = _position.X - _texture.Width;

                _position4.X = _position.X + _texture.Width * 2;
            }

            _position.Y = Globals.ScreenSize.Y - _texture.Height;
            _position2.Y = _position.Y;
            _position3.Y = _position.Y;
            _position4.Y = _position.Y;
        }

        public void Draw()
        {
            if (_movementY)
            {
                Globals.SpriteBatch.Draw(_texture, _position, null, Color.White, _rotate, new Vector2(_texture.Height/2, _texture.Width/2), Vector2.One, SpriteEffects.None, _depth);
                return;
            }
            var _positionX = _position.X;
            var _position2X = _position2.X;
            var _position3X = _position3.X;
            var _position4X = _position4.X;
            _position.X += Globals.ScreenSize.X / 2 - _texture.Width / 2;
            _position2.X += Globals.ScreenSize.X / 2 - _texture.Width / 2;
            _position3.X += Globals.ScreenSize.X / 2 - _texture.Width / 2;
            _position4.X += Globals.ScreenSize.X / 2 - _texture.Width / 2;
            Globals.SpriteBatch.Draw(_texture, _position, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, _depth);
            Globals.SpriteBatch.Draw(_texture, _position2, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, _depth);
            Globals.SpriteBatch.Draw(_texture, _position3, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, _depth);
            Globals.SpriteBatch.Draw(_texture, _position4, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, _depth);
            _position.X = _positionX;
            _position2.X = _position2X;
            _position3.X = _position3X;
            _position4.X = _position4X;
        }
    }
}
