using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace ISS
{
    public class Player
    {
        private Texture2D texture;
        public Vector2 Position;
        public Vector2 Size;
        private float speed;
        public float JumpPower;
        //public float JumpDelay;
        public bool Jump = false;
        public bool Float = false;
        //private bool attacking;
        public bool Running;
        public bool Crouch;
        public bool Interact = false;
        public GameObject GameObjectInteract = null;
        public Vector2 Origin { get; }
        //public Vector2 Velocity;
        public float Ground;
        public bool Grounded;

        private readonly float _maxHealth;
        private float _health;
        private readonly float _maxOxygen;
        private float _oxygen;
        private readonly float _maxEnergy;
        private float _energy;
        //private readonly ProgressBar _healthBar;
        private readonly ProgressBarAnimated _healthBarAnimated;
        private readonly ProgressBarAnimated _oxigenBarAnimated;
        private readonly ProgressBarAnimated _energyBarAnimated;

        private readonly AnimationManager _anims = new AnimationManager();

        public Player(Vector2 position, float health = 100f, float oxigen = 100f, float energy = 100f)
        {
            speed = 0f;
            texture = Globals.Content.Load<Texture2D>("sprite_player01x4_helmet_a");
            _anims.AddAnimation(new Vector2(0, 0), new Animation(texture, 4, 2, 0, 2, 0.25f, 1)); //Stand
            _anims.AddAnimation(new Vector2(1, 0), new Animation(texture, 4, 2, 0, 3, 0.1f, 2));  //Right
            _anims.AddAnimation(new Vector2(-1, 0), new Animation(texture, 4, 2, 0, 3, 0.1f, 2)); //Left
            _anims.AddAnimation(new Vector2(0, -1), new Animation(texture, 4, 2, 0, 0, 0.25f, 2)); //Jump            
            _anims.AddAnimation(new Vector2(0, 1), new Animation(texture, 4, 2, 3, 3, 0.25f, 1)); //Crouch

            Origin = new Vector2((texture.Width / 4)/2, (texture.Height / 2)/2);
            Size = new Vector2(texture.Width / 4, texture.Height / 2);
            Position = new Vector2(position.X + Size.X/2, position.Y);

            var BarBackground = Globals.Content.Load<Texture2D>("BarBackground");
            var BarHealth = Globals.Content.Load<Texture2D>("BarHealth");
            var BarOxigen = Globals.Content.Load<Texture2D>("BarOxigen");
            var BarEnergy = Globals.Content.Load<Texture2D>("BarEnergy");

            _maxHealth = health;
            _health = health;
            _maxOxygen = oxigen;
            _oxygen = oxigen;
            _maxEnergy = energy;
            _energy = energy;

            //_healthBar = new ProgressBar(BarBackground, BarHealth, _maxHealth, new Vector2(100, 100));
            _healthBarAnimated = new ProgressBarAnimated(BarBackground, BarHealth, _maxHealth, new Vector2(10, 10));
            _oxigenBarAnimated = new ProgressBarAnimated(BarBackground, BarOxigen, _maxOxygen, new Vector2(10, 45));
            _energyBarAnimated = new ProgressBarAnimated(BarBackground, BarEnergy, _maxEnergy, new Vector2(10, 80));

            //Rectangle = new Rectangle((int)Position.X, (int)Position.Y, texture.Width / 4, texture.Height / 2);
        }

        //public bool Attacking { get => attacking; set => attacking = value; }
        public void Update()
        {
            if (InputManager.Moving)
            {
                Position += Vector2.Normalize(InputManager.Direction) * speed * Globals.ElapsedSeconds;
            }
            _anims.Update(InputManager.Direction, Running);
            if (Running)
            {
                OxygenLost();
            }

            JumpResolve();

            if (Crouch)
            {
                _anims.Update(new Vector2(0, 1), false);
            }

            Position.X = Globals.ScreenSize.X/2 - Size.X/2;
            OxigenLost();

            //_healthBar.Update(_health);
            _healthBarAnimated.Update(_health);
            _oxigenBarAnimated.Update(_oxygen);
            _energyBarAnimated.Update(_energy);
        }
         
        public void Draw()
        {
            _anims.Draw(Position);
            //_healthBar.Draw();
            _healthBarAnimated.Draw();
            _oxigenBarAnimated.Draw();
            _energyBarAnimated.Draw();
        }

        public void TakeDamage(float dmg)
        {
            _health -= dmg;
            if (_health < 0) _health = 0;
        }

        public void Heal(float heal)
        {
            _health += heal;
            if (_health > _maxHealth) _health = _maxHealth;
        }

        public void HealthRefill()
        {
            _health += 100;
            if (_health > _maxHealth) _health = _maxHealth;
        }

        public void OxigenLost()
        {
            _oxygen -= Globals.ElapsedSeconds;
            if (_oxygen <= 0)
            {
                _oxygen = 0;
                _health -= Globals.ElapsedSeconds * 8;
                if (_health < 0) _health = 0;
            }
        }

        public void OxygenRefill()
        {
            _oxygen += 100;
            if (_oxygen > _maxOxygen) _oxygen = _maxOxygen;
        }

        public void OxygenLost()
        {
            _oxygen -= Globals.ElapsedSeconds * 1;
            if (_oxygen < 0) _oxygen = 0;
        }

        public void EnergyLost()
        {
            _energy -= Globals.ElapsedSeconds*20;
            if (_energy < 0) _energy = 0;
        }

        public void EnergyRefill()
        {
            _energy += 100;
            if (_energy > _maxEnergy) _energy = _maxEnergy;
        }

        public float getEnergy()
        {
            return _energy;
        }

        private void JumpResolve()
        {
            if (Jump && JumpPower <= 0)
            {
                JumpPower = 850f;
                Grounded = false;
            }

            if (Position.Y + Size.Y < Ground)
            {
                Position.Y += Globals.Gravity;
                _anims.Update(new Vector2(0, -1), false);
            }

            if (Position.Y + Size.Y > Ground)
            { 
                Position = new Vector2(Position.X, Ground - Size.Y);
                JumpPower = 0;
                Jump = false;
                Float = false;
                Grounded = true;
            }

            if (JumpPower > 0 && !Float)
            {
                Position += Vector2.Normalize(new Vector2(0, -1)) * JumpPower * Globals.ElapsedSeconds;
                JumpPower -= Globals.Gravity;
            }

            if (Float)
            {
                if (Grounded)
                {
                    JumpPower += 600;
                }
                JumpPower+=2;
                Position += Vector2.Normalize(new Vector2(0, -1)) * JumpPower * Globals.ElapsedSeconds;
                Grounded = false;
                Jump = false;
                EnergyLost();
            }
        }

        public void Interaction()
        {
            if (Interact)
            {
                switch (GameObjectInteract.Type)
                {
                    case 0:
                        HealthRefill();
                        break;

                    case 1:
                        OxygenRefill();
                        break;

                    case 2:
                        EnergyRefill();
                        break;
                }
            }
        }

        public float Oxygen()
        {
            return _oxygen;
        }
    }
}