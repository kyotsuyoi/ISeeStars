using System.Collections.Generic;

namespace ISS
{

    public class BGManager
    {
        private readonly List<Layer> _layers;

        public BGManager()
        {
            _layers = new List<Layer>();
        }

        public void AddLayer(Layer layer)
        {
            _layers.Add(layer);
        }

        public void Update(float movement)
        {
            foreach (var layer in _layers)
            {
                layer.Update(movement);
            }
        }

        public void Draw()
        {
            foreach (var layer in _layers)
            {
                layer.Draw();
            }
        }
    }
}
