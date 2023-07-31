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

        public void Update(object key, bool fastAnimation)
        {
            if (_anims.TryGetValue(key, out Animation value))
            {
                value.Start();
                _anims[key].Update(fastAnimation);
                _lastKey = key;
            }
        }

        public void Draw(Vector2 position)
        {
            _anims[_lastKey].Draw(position);
        }
    }

}