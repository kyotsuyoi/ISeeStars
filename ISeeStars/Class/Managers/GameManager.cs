using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.WIC;
using System.Collections.Generic;
using System.Security.AccessControl;

namespace ISS
{
    public class GameManager
    {
        private readonly BGManager _bgm = new BGManager();
        private InputManager _inputManager = new InputManager();
        public Player player;
        public SoundManager soundManager;
        private int _interactObjectsId = -1;
        private readonly List<GameObject> _objects = new List <GameObject>();

        public GameMenu gameMenu;
        public Collision collision = new();

        public GameManager(Vector2 playerPosition)
        {
            _bgm.AddLayer(new Layer(Globals.Content.Load<Texture2D>("Background/bgMarsL0002"), 0.0f, 0.0f, true));
            _bgm.AddLayer(new Layer(Globals.Content.Load<Texture2D>("Background/bgMarsL1"), 0.1f, 0.2f, false));
            _bgm.AddLayer(new Layer(Globals.Content.Load<Texture2D>("Background/bgMarsL2"), 0.2f, 0.5f, false));
            _bgm.AddLayer(new Layer(Globals.Content.Load<Texture2D>("Background/bgMarsL3"), 0.3f, 1.0f, false));
            //_bgm.AddLayer(new Layer(Globals.Content.Load<Texture2D>("bgMarsL4"), 0.4f, 0.2f, -100.0f));

            soundManager = new SoundManager();

            player = new Player(playerPosition);
            GameObject gameObject0 = new GameObject(new Vector2(-110, 0), EnumGameObjectType.WoodenBox);
            GameObject gameObject1 =  new GameObject(new Vector2(10,  0), EnumGameObjectType.Default);
            GameObject gameObject2 = new GameObject(new Vector2(110, 0), EnumGameObjectType.Default);
            GameObject gameObject3 = new GameObject(new Vector2(210, 0), EnumGameObjectType.Default);
            GameObject gameObject4 = new GameObject(new Vector2(450, 0), EnumGameObjectType.MetalWall);

            _objects.Add(gameObject0);
            _objects.Add(gameObject1);
            _objects.Add(gameObject2);
            _objects.Add(gameObject3);
            _objects.Add(gameObject4);

            for (int i = 0; i < 5; i++)
            {
                GameObject go = new GameObject(new Vector2(-110, gameObject0.Position.Y -= i * 80), EnumGameObjectType.WoodenBox);
                _objects.Add(go);
            }

            gameMenu = new GameMenu(EnumGameMenuType.Settings, 3, soundManager.GetBGMVolume());
        }

        public void Update()
        {
            _inputManager.Update(player, soundManager, gameMenu);

            soundManager.PlayFX(player.GetSoundFX());
            soundManager.PlayFXInstance(player.GetSoundFXInstance(), EnumSoundOrigin.Player);
            soundManager.StopFXInstance(player.GetStopSoundFXInstance(), EnumSoundOrigin.Player);

            soundManager.PlayFX(_inputManager.GetSoundFX());
            soundManager.PlayFXInstance(_inputManager.GetSoundFXInstance(), EnumSoundOrigin.Player);
            soundManager.StopFXInstance(_inputManager.GetStopSoundFXInstance(), EnumSoundOrigin.Player);

            if (gameMenu.IsActive())
            {
                gameMenu.Update();
                return;
            }

            _bgm.Update(_inputManager.BackgroundMovement);
            //tileObject.Update(_im.BackgroundMovement);
            foreach (var item in _objects)
            {
                item.Update(_inputManager.BackgroundMovement);
            }
            player.Update();

            CheckCollisions();
            CheckInteractCollisions();
            CheckOtherCollisions();

            TestCollision();
        }

        public void Draw()
        {
            _bgm.Draw();
            //tileObject.Draw();
            foreach (var item in _objects)
            {
                item.Draw();
            }
            player.Draw();           
            if (gameMenu.IsActive()) gameMenu.Draw();
            DrawDebugInfo();
        }

        private void CheckInteractCollisions()
        {
            var isCollide = false;
            for (int i = 0; i < _objects.Count; i++)
            {
                isCollide = SquareCollision(_objects[i].Position, _objects[i].Size, player.Position, player.Size);
                if (isCollide)
                {
                    _interactObjectsId = i;
                    player.ObjectCollideResolve(_objects[i]);
                    break;
                }
                player.Interact = false;
                player.GameObjectInteract = null;
                _interactObjectsId = -1;
            }

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

        private void CheckOtherCollisions()
        {
            Rectangle playerRect = new Rectangle((int)player.Position.X, (int)player.Position.Y, (int)player.Size.X, (int)player.Size.Y);

            for (int i = 0; i < _objects.Count; i++)
            {
                Rectangle objectRec = _objects[i].GetRectangle();
                if (_objects[i].GetObjectType() == EnumGameObjectType.MetalWall)
                {
                    if (playerRect.Bottom * 0.95 > objectRec.Top)
                    {
                        var val1 = objectRec.Left * 1.02;
                        var val2 = objectRec.Left + objectRec.Size.X;
                        var b1 = playerRect.Right >= val1;
                        var b2 = playerRect.Right <= val2;

                        if (playerRect.Right >= objectRec.Left * 1.02 &&
                            playerRect.Right <= objectRec.Right)
                        {
                            player.ObstructionRight = true;
                        }
                        if (playerRect.Left <= objectRec.Right * 0.98 &&
                            playerRect.Left >= objectRec.Left)
                        {
                            player.ObstructionLeft = true;
                        }
                    }
                }                
            }
        }

        private void TestCollision()
        {
            Rectangle playerRect = new Rectangle((int)player.Position.X, (int)player.Position.Y, (int)player.Size.X, (int)player.Size.Y);

            collision.contactLeft = 0;
            collision.contactRight = 0;
            collision.contactTop = 0;
            collision.contactBottom = 0;
            for (int i = 0; i < _objects.Count; i++)
            {
                Rectangle objectRec = _objects[i].GetRectangle();
                collision.CheckCollisionSide(playerRect, objectRec);
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

        private void DrawDebugInfo()
        {
#if DEBUG
            SpriteFont font = Globals.Content.Load<SpriteFont>("Font/fontMedium");

            Globals.SpriteBatch.DrawString(font, ((int)player.GetHealth()).ToString().PadLeft(3, '0'), new Vector2(150, 20), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            Globals.SpriteBatch.DrawString(font, ((int)player.GetHealth()).ToString().PadLeft(3, '0'), new Vector2(152, 22), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

            Globals.SpriteBatch.DrawString(font, ((int)player.GetOxygen()).ToString().PadLeft(3, '0'), new Vector2(150, 55), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            Globals.SpriteBatch.DrawString(font, ((int)player.GetOxygen()).ToString().PadLeft(3, '0'), new Vector2(152, 57), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

            Globals.SpriteBatch.DrawString(font, ((int)player.GetEnergy()).ToString().PadLeft(3, '0'), new Vector2(150, 90), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            Globals.SpriteBatch.DrawString(font, ((int)player.GetEnergy()).ToString().PadLeft(3, '0'), new Vector2(152, 92), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

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
            Globals.SpriteBatch.DrawString(font, "Fly:" + player.IsFlying(), new Vector2(10, 180), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1); ;
            Globals.SpriteBatch.DrawString(font, "Fly:" + player.IsFlying(), new Vector2(12, 182), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);
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

            Globals.SpriteBatch.DrawString(font, "FallingDown:" + player.GetFallingDown(), new Vector2(10, 340), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            Globals.SpriteBatch.DrawString(font, "FallingDown:" + player.GetFallingDown(), new Vector2(12, 342), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

            //Globals.SpriteBatch.DrawString(font, "ObstructionLeft:" + player.ObstructionLeft, new Vector2(10, 360), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            //Globals.SpriteBatch.DrawString(font, "ObstructionLeft:" + player.ObstructionLeft, new Vector2(12, 362), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

            //Globals.SpriteBatch.DrawString(font, "ObstructionRight:" + player.ObstructionRight, new Vector2(10, 380), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            //Globals.SpriteBatch.DrawString(font, "ObstructionRight:" + player.ObstructionRight, new Vector2(12, 382), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

            Globals.SpriteBatch.DrawString(font, "contactLeft:" + collision.contactLeft, new Vector2(10, 360), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            Globals.SpriteBatch.DrawString(font, "contactLeft:" + collision.contactLeft, new Vector2(12, 362), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

            Globals.SpriteBatch.DrawString(font, "contactRight:" + collision.contactRight, new Vector2(10, 380), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            Globals.SpriteBatch.DrawString(font, "contactRight:" + collision.contactRight, new Vector2(12, 382), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

            Globals.SpriteBatch.DrawString(font, "contactTop:" + collision.contactTop, new Vector2(10, 400), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            Globals.SpriteBatch.DrawString(font, "contactTop:" + collision.contactTop, new Vector2(12, 402), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

            Globals.SpriteBatch.DrawString(font, "contactBottom:" + collision.contactBottom, new Vector2(10, 420), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            Globals.SpriteBatch.DrawString(font, "contactBottom:" + collision.contactBottom, new Vector2(12, 422), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

            if (player.Interact)
            {
                Globals.SpriteBatch.DrawString(font, "Y", new Vector2(
                    _objects[_interactObjectsId].Position.X + _objects[_interactObjectsId].Origin.X - 5,
                    _objects[_interactObjectsId].Position.Y - 10), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9997f);
                Globals.SpriteBatch.DrawString(font, "Y", new Vector2(
                    _objects[_interactObjectsId].Position.X + _objects[_interactObjectsId].Origin.X - 7,
                    _objects[_interactObjectsId].Position.Y - 12), Color.Yellow, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9998f);
            }
#endif
        }
    }
}
