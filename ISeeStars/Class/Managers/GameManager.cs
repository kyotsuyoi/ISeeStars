using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System.Collections.Generic;

namespace ISS
{
    public class GameManager
    {
        private readonly BGManager _bgm = new BGManager();
        private readonly InputManager _im = new InputManager();
        private Player player;
        //private TileObject tileObject;

        private readonly List<TileObject> _objects = new List <TileObject>();

        public GameManager(Vector2 playerPosition)
        {
            _bgm.AddLayer(new Layer(Globals.Content.Load<Texture2D>("bgMarsL0"), 0.0f, 0.0f));
            _bgm.AddLayer(new Layer(Globals.Content.Load<Texture2D>("bgMarsL1"), 0.1f, 0.2f));
            _bgm.AddLayer(new Layer(Globals.Content.Load<Texture2D>("bgMarsL2"), 0.2f, 0.5f));
            _bgm.AddLayer(new Layer(Globals.Content.Load<Texture2D>("bgMarsL3"), 0.3f, 1.0f));
            //_bgm.AddLayer(new Layer(Globals.Content.Load<Texture2D>("bgMarsL4"), 0.4f, 0.2f, -100.0f));

            player = new Player(playerPosition);
            TileObject tileObject = new TileObject(new Vector2(10,400));
            TileObject tileObject2 = new TileObject(new Vector2(85, 0));

            _objects.Add(tileObject);
            _objects.Add(tileObject2);
        }

        public void Update()
        {
            _im.Update();
            _bgm.Update(_im.BackgroundMovement);
            //tileObject.Update(_im.BackgroundMovement);
            foreach (var object1 in _objects)
            {
                object1.Update(_im.BackgroundMovement);
            }
            player.Update();

            CheckCollisions();
            CheckCollisions(player);
        }

        public void Draw()
        {
            _bgm.Draw();
            //tileObject.Draw();
            foreach (var object1 in _objects)
            {
                object1.Draw();
            }
            player.Draw();
        }

        private void CheckCollisions()
        {
            for (int i = 0; i < _objects.Count - 1; i++)
            {
                for (int j = i + 1; j < _objects.Count; j++)
                {
                    if ((_objects[i].Position - _objects[j].Position).Length() < (_objects[i].Origin.X + _objects[j].Origin.X))
                    {
                        //ResolveCollision(_circles[i], _circles[j]);
                        break;
                    }
                }
            }
        }

        private void CheckCollisions(Player player)
        {
            for (int i = 0; i < _objects.Count; i++)
            {
                if ((_objects[i].Position - player.Position).Length() < (_objects[i].Origin.X + player.Origin.X))
                {
                    //ResolveCollision(_circles[i], _circles[j]);
                    break;
                }                
            }
        }
    }
}
