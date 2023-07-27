using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ISS
{
    public class GameManager
    {
        private readonly BGManager _bgm = new BGManager();
        private readonly InputManager _im = new InputManager();
        private Player player;

        public GameManager(Vector2 playerPosition)
        {
            _bgm.AddLayer(new Layer(Globals.Content.Load<Texture2D>("Layer0"), 0.0f, 0.0f));
            _bgm.AddLayer(new Layer(Globals.Content.Load<Texture2D>("Layer1"), 0.1f, 0.2f));
            _bgm.AddLayer(new Layer(Globals.Content.Load<Texture2D>("Layer2"), 0.2f, 0.5f));
            _bgm.AddLayer(new Layer(Globals.Content.Load<Texture2D>("Layer3"), 0.3f, 1.0f));
            _bgm.AddLayer(new Layer(Globals.Content.Load<Texture2D>("Layer4"), 0.4f, 0.2f, -100.0f));

            player = new Player(playerPosition);
        }

        public void Update()
        {
            _im.Update();
            _bgm.Update(_im.BackgroundMovement);
            player.Update();
        }

        public void Draw()
        {
            _bgm.Draw();
            player.Draw();
        }
    }
}
