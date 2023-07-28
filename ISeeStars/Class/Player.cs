using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace ISS
{
    internal class Player
    {
        private Texture2D texture;
        public Vector2 Position;
        private float speed;
        private float jumpPower;
        //private bool attacking;
        //private bool running;
        public bool interact = false;
        public Vector2 Origin { get; }
        public int GroundLevel = Globals.GroundLevel;
        public Rectangle Rectangle { get; set; }
        public Vector2 Velocity;

        private readonly AnimationManager _anims = new AnimationManager();

        public Player(Vector2 position)
        {
            speed = 0f;
            texture = Globals.Content.Load<Texture2D>("sprite_player01x4");
            _anims.AddAnimation(new Vector2(0, 0), new Animation(texture, 4, 2, 0, 2, 0.25f, 1)); //Stand
            _anims.AddAnimation(new Vector2(1, 0), new Animation(texture, 4, 2, 0, 3, 0.1f, 2));  //Right
            _anims.AddAnimation(new Vector2(-1, 0), new Animation(texture, 4, 2, 0, 3, 0.1f, 2)); //Left
            _anims.AddAnimation(new Vector2(0, -1), new Animation(texture, 4, 2, 0, 0, 0.25f, 2)); //Jump            
            _anims.AddAnimation(new Vector2(0, 1), new Animation(texture, 4, 2, 3, 3, 0.25f, 1)); //Crouch

            Origin = new Vector2((texture.Width / 4)/2, (texture.Height / 2)/2);
            Position = new Vector2(position.X - Origin.X, position.Y);

            Rectangle = new Rectangle((int)Position.X, (int)Position.Y, texture.Width / 4, texture.Height / 2);
        }

        //public bool Attacking { get => attacking; set => attacking = value; }
        public void Update()
        {
            if (InputManager.Moving)
            {
                Position += Vector2.Normalize(InputManager.Direction) * speed * Globals.ElapsedSeconds;
            }
            _anims.Update(InputManager.Direction);

            if (Position.Y + (texture.Height/2) > GroundLevel)
            {
                Position = new Vector2(Position.X, GroundLevel- (texture.Height / 2));
            }

            if (Position.Y + (texture.Height / 2) < GroundLevel)
            {
                Position.Y += Globals.Gravity;                
            }
            else
            {
                jumpPower = 0;
            }

            if (InputManager.Jump && jumpPower <= 0)
            {
                InputManager.Jump = false;
                jumpPower = 1000f;
                //Debug.Print("\nPlayer InputManager.Jump:"+ InputManager.Jump);
            }
            if (jumpPower>0)
            {
                Position += Vector2.Normalize(new Vector2(0,-1)) * jumpPower * Globals.ElapsedSeconds;
                jumpPower -= Globals.Gravity;
                _anims.Update(new Vector2(0, -1));
            }

            if (InputManager.Crouch)
            {
                _anims.Update(new Vector2(0, 1));
            }
        }
         
        public void Draw()
        {
            _anims.Draw(Position);
        }
    }
}