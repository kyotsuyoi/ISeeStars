using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace ISS
{
    internal class Player
    {
        private Texture2D texture;
        private Vector2 position;
        private float speed;
        private float jumpPower;
        //private bool attacking;
        //private bool running;

        private readonly AnimationManager _anims = new AnimationManager();

        public Player(Vector2 position)
        {
            this.position = position;
            speed = 0f;
            texture = Globals.Content.Load<Texture2D>("sprite_player01x4");
            _anims.AddAnimation(new Vector2(0, 0), new Animation(texture, 4, 2, 0, 2, 0.25f, 1)); //Stand
            _anims.AddAnimation(new Vector2(1, 0), new Animation(texture, 4, 2, 0, 3, 0.1f, 2));  //Right
            _anims.AddAnimation(new Vector2(-1, 0), new Animation(texture, 4, 2, 0, 3, 0.1f, 2)); //Left
            //_anims.AddAnimation(new Vector2(0, -1), new Animation(texture, 4, 2, 0, 3, 0.25f, 2)); //Jump            
            _anims.AddAnimation(new Vector2(0, 1), new Animation(texture, 4, 2, 3, 3, 0.25f, 1)); //Crouch
        }

        //public bool Attacking { get => attacking; set => attacking = value; }
        public void Update()
        {
            if (InputManager.Moving)
            {
                position += Vector2.Normalize(InputManager.Direction) * speed * Globals.ElapsedSeconds;
            }
            _anims.Update(InputManager.Direction);

            if (position.Y <= Globals.ScreenY-150)
            {
                position.Y += Globals.Gravity;
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
                position += Vector2.Normalize(new Vector2(0,-1)) * jumpPower * Globals.ElapsedSeconds;
                jumpPower -= Globals.Gravity;
            }

            if (InputManager.Crouch)
            {
                _anims.Update(new Vector2(0, 1));
            }
        }
         
            public void Draw()
        {
            _anims.Draw(position);
        }

    }
}