using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ISS
{
    public class AnimationManager
    {
        private readonly Dictionary<object, Animation> _anims = new Dictionary<object, Animation>();
        private object _lastKey;

        public void AddAnimation(object key, Animation animation)
        {
            _anims.Add(key, animation);
            _lastKey = key;
        }

        public void Update(object key, bool attacking)
        {
            //Adjust diagonal move animation
            if ((Vector2)key == new Vector2(-1, 1) || (Vector2)key == new Vector2(1, 1) || (Vector2)key == new Vector2(-1, -1) || (Vector2)key == new Vector2(1, -1))
            {
                key = _lastKey;
            }

            if (_anims.TryGetValue(key, out Animation value))
            {
                value.Start();
                _anims[key].Update();
                _lastKey = key;
            }
            else
            {
                if (attacking) return;
                _anims[_lastKey].Stop();
                _anims[_lastKey].Reset();
            }
        }

        public void Update(object key)
        {
            if (_anims.TryGetValue(key, out Animation value))
            {
                value.Start();
                _anims[key].Update();
                _lastKey = key;
            }
        }

        public void Draw(Vector2 position)
        {
            _anims[_lastKey].Draw(position);
        }
    }

}