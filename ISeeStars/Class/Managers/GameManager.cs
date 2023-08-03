using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ISS
{
    public class GameManager
    {
        private readonly BGManager _bgm = new BGManager();
        private InputManager _inputManager = new InputManager();
        public Player player;
        public SongManager songManager;
        private int _interactObjectsId = -1;
        private readonly List<GameObject> _objects = new List <GameObject>();

        //Verify if the player is falling
        private float lastY = 0;
        public GameMenu gameMenu;

        public GameManager(Vector2 playerPosition)
        {
            _bgm.AddLayer(new Layer(Globals.Content.Load<Texture2D>("bgMarsL0002"), 0.0f, 0.0f, true));
            _bgm.AddLayer(new Layer(Globals.Content.Load<Texture2D>("bgMarsL1"), 0.1f, 0.2f, false));
            _bgm.AddLayer(new Layer(Globals.Content.Load<Texture2D>("bgMarsL2"), 0.2f, 0.5f, false));
            _bgm.AddLayer(new Layer(Globals.Content.Load<Texture2D>("bgMarsL3"), 0.3f, 1.0f, false));
            //_bgm.AddLayer(new Layer(Globals.Content.Load<Texture2D>("bgMarsL4"), 0.4f, 0.2f, -100.0f));

            songManager = new SongManager();

            player = new Player(playerPosition);
            GameObject gameObject0 = new GameObject(new Vector2(-110, 0), EnumGameObjectType.Default);
            GameObject gameObject1 =  new GameObject(new Vector2(10,  0), EnumGameObjectType.Health);
            GameObject gameObject2 = new GameObject(new Vector2(110, 0), EnumGameObjectType.Oxygen);
            GameObject gameObject3 = new GameObject(new Vector2(210, 0), EnumGameObjectType.Energy);

            _objects.Add(gameObject0);
            _objects.Add(gameObject1);
            _objects.Add(gameObject2);
            _objects.Add(gameObject3);

            gameMenu = new GameMenu(0,3,songManager.GetVolume());
        }

        public void Update()
        {
            _inputManager.Update(player, songManager, gameMenu);

            if (gameMenu.IsActive())
            {
                gameMenu.Update();
                return;
            }

            _bgm.Update(_inputManager.BackgroundMovement);
            //tileObject.Update(_im.BackgroundMovement);
            foreach (var object1 in _objects)
            {
                object1.Update(_inputManager.BackgroundMovement);
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

            SpriteFont font = Globals.Content.Load<SpriteFont>("fontMedium");

            var hh = (int)Globals.Time / 60;
            var mm = (int)Globals.Time % 60;
            var game_time = "Time:" + hh.ToString().PadLeft(2, '0') + ":" + mm.ToString().PadLeft(2, '0');
            Globals.SpriteBatch.DrawString(font, game_time, new Vector2(10, 120), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            Globals.SpriteBatch.DrawString(font, game_time, new Vector2(12, 122), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

            Globals.SpriteBatch.DrawString(font, "Y:" + (int)(player.Position.Y + player.Size.Y), new Vector2(10, 140), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            Globals.SpriteBatch.DrawString(font, "Y:" + (int)(player.Position.Y + player.Size.Y), new Vector2(12, 142), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

            //Globals.SpriteBatch.DrawString(font, "Position:" + _bgm.GetLayer3().Layer0PositionX(), new Vector2(10, 100), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            //Globals.SpriteBatch.DrawString(font, "Position:" + _bgm.GetLayer3().Layer0PositionX(), new Vector2(12, 102), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

            Globals.SpriteBatch.DrawString(font, "Jump:" + player.Jump, new Vector2(10, 160), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            Globals.SpriteBatch.DrawString(font, "Jump:" + player.Jump, new Vector2(12, 162), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);
            Globals.SpriteBatch.DrawString(font, "Fly:" + player.isFly(), new Vector2(10, 180), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1); ;
            Globals.SpriteBatch.DrawString(font, "Fly:" + player.isFly(), new Vector2(12, 182), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);
            Globals.SpriteBatch.DrawString(font, "Run:" + player.Running, new Vector2(10, 200), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            Globals.SpriteBatch.DrawString(font, "Run:" + player.Running, new Vector2(12, 202), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);
            Globals.SpriteBatch.DrawString(font, "Crouch:" + player.Crouch, new Vector2(10, 220), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            Globals.SpriteBatch.DrawString(font, "Crouch:" + player.Crouch, new Vector2(12, 222), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);
            Globals.SpriteBatch.DrawString(font, "JumpPower:" + player.JumpPower, new Vector2(10, 240), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            Globals.SpriteBatch.DrawString(font, "JumpPower:" + player.JumpPower, new Vector2(12, 242), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);
            Globals.SpriteBatch.DrawString(font, "Ground:" + player.Grounded, new Vector2(10, 260), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            Globals.SpriteBatch.DrawString(font, "Ground:" + player.Grounded, new Vector2(12, 262), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

            Globals.SpriteBatch.DrawString(font, "interact_key:" + InputManager.interact_key_pressed.ToString(), new Vector2(10, 280), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            Globals.SpriteBatch.DrawString(font, "interact_key:" + InputManager.interact_key_pressed.ToString(), new Vector2(12, 282), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);
            Globals.SpriteBatch.DrawString(font, "menu_key_pressed:" + InputManager.menu_key_pressed.ToString(), new Vector2(10, 300), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            Globals.SpriteBatch.DrawString(font, "menu_key_pressed:" + InputManager.menu_key_pressed.ToString(), new Vector2(12, 302), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);
            
            Globals.SpriteBatch.DrawString(font, "menu_selected:" + InputManager.menu_selected.ToString(), new Vector2(10, 320), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            Globals.SpriteBatch.DrawString(font, "menu_selected:" + InputManager.menu_selected.ToString(), new Vector2(12, 322), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);
            
            if (player.Interact)
            {
                Globals.SpriteBatch.DrawString(font, "Y", new Vector2(
                    _objects[_interactObjectsId].Position.X + _objects[_interactObjectsId].Origin.X-5, 
                    _objects[_interactObjectsId].Position.Y-10), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9997f);
                Globals.SpriteBatch.DrawString(font, "Y", new Vector2(
                    _objects[_interactObjectsId].Position.X + _objects[_interactObjectsId].Origin.X-7, 
                    _objects[_interactObjectsId].Position.Y-12), Color.Yellow, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9998f);
            }

            if (gameMenu.IsActive()) gameMenu.Draw();
        }

        private void CheckCollisions(Player player)
        {
            var isCollide = false;
            for (int i = 0; i < _objects.Count; i++)
            {
                isCollide = SquareCollision(_objects[i].Position, _objects[i].Size, player.Position, player.Size);
                if (isCollide)
                {
                    player.Interact = true;
                    player.GameObjectInteract = _objects[i];
                    _interactObjectsId = i;
                    var bottom = player.Position.Y + player.Size.Y;
                    var bottomLimit = bottom - player.Size.Y * 0.1;
                    if ((bottom >= _objects[i].Position.Y) && (bottomLimit < _objects[i].Position.Y)
                        && (lastY < player.Position.Y))
                    {
                        player.Position.Y = _objects[i].Position.Y - player.Size.Y;
                        player.Ground = _objects[i].Position.Y;
                        player.Grounded = true;
                    }
                    break;
                }
                player.Interact = false;
                player.GameObjectInteract = null;
                _interactObjectsId = -1;
            }
            lastY = player.Position.Y;

            if(!isCollide) player.Ground = Globals.GroundLevel;
        }

        private void CheckCollisions()
        {
            for (int i = 0; i < _objects.Count - 1; i++)
            {
                for (int j = i + 1; j < _objects.Count; j++)
                {
                    if ((_objects[i].Position - _objects[j].Position).Length() < (_objects[i].Origin.X + _objects[j].Origin.X))
                    {
                        _objects[j].Position.Y = _objects[i].Position.Y - _objects[j].Size.Y;
                        break;
                    }
                }
            }
        }

        private bool SquareCollision(Vector2 PositionA, Vector2 SizeA, Vector2 PositionB, Vector2 SizeB)
        {
            if (
                PositionA.X < PositionB.X + SizeB.X * 0.8 &&
                PositionA.X + SizeA.X * 0.8 > PositionB.X &&
                PositionA.Y < PositionB.Y + SizeB.Y *1.1 &&
                PositionA.Y + SizeA.Y *1.1 > PositionB.Y
            )
            {
                return true;
            }
            return false;
        }
    }
}
