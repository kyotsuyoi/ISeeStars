using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Newtonsoft.Json.Bson;

namespace ISS
{
    public class Player
    {
        private Texture2D texture;
        public Vector2 Position;
        public Vector2 Size;
        private float speed;
        public float JumpPower;
        public bool Jump = false;
        private bool fly = false;
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

        private int energyCooldown = 0;

        //private readonly ProgressBar _healthBar;
        private readonly ProgressBarAnimated _healthBarAnimated;
        private readonly ProgressBarAnimated _oxigenBarAnimated;
        private readonly ProgressBarAnimated _energyBarAnimated;

        private readonly AnimationManager _anims = new AnimationManager();

        public Player(Vector2 position, float health = 100f, float oxigen = 100f, float energy = 100f)
        {
            speed = 0f;
            texture = Globals.Content.Load<Texture2D>("sprite_player02x4");
            _anims.AddAnimation(new Vector2(0, 0), new Animation(texture, 4, 3, 0, 2, 0.25f, 1)); //Stand
            _anims.AddAnimation(new Vector2(1, 0), new Animation(texture, 4, 3, 0, 3, 0.1f, 2));  //Right
            _anims.AddAnimation(new Vector2(-1, 0), new Animation(texture, 4, 3, 0, 3, 0.1f, 2)); //Left
            _anims.AddAnimation(new Vector2(0, -1), new Animation(texture, 4, 3, 0, 3, 0.05f, 3)); //Jump & Fly
            _anims.AddAnimation(new Vector2(0, 1), new Animation(texture, 4, 3, 3, 3, 0.25f, 1)); //Crouch
            //_anims.AddAnimation(new Vector2(0, -1), new Animation(texture, 4, 3, 0, 3, 0.25f, 3)); //Fly

            Origin = new Vector2((texture.Width / 4)/2, (texture.Height / 3)/2);
            Size = new Vector2(texture.Width / 4, texture.Height / 3);
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
            //if (InputManager.Moving)
            //{
            //    Position += Vector2.Normalize(InputManager.Direction) * speed * Globals.ElapsedSeconds;
            //}
            if (Running || fly)
            {
                _anims.Update(InputManager.Direction, true, true);
            }
            else
            {
                _anims.Update(InputManager.Direction, false, true);
            }

            if (Running)
            {
                OxygenLost();
            }

            JumpResolve();

            if (Crouch)
            {
                _anims.Update(new Vector2(0, 1), false, false);
            }

            if (Interact)
            {
                switch (GameObjectInteract.Type)
                {
                    case 0:
                        HeathCharge();
                        break;

                    case 1:
                        OxygenCharge();
                        break;

                    case 2:
                        EnergyCharge();
                        break;
                }
            }

            Position.X = Globals.ScreenSize.X/2 - Size.X/2;
            OxygenLost();
            //EnergyCharge();
            energyCooldown--;

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

        public void TakeDamage(float damage)
        {
            _health -= damage;
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

        public void HeathCharge()
        {
            //_health += Globals.ElapsedSeconds * 1f;
            //if (_health >= 100f) _oxygen = 100f;
            if((_oxygen > 0 && _energy > 0) && !GameObjectInteract.IsFull())
            {
                _oxygen -= Globals.ElapsedSeconds * 1f;
                _energy -= Globals.ElapsedSeconds * 1f;
                GameObjectInteract.UpdateLoadingBar(true);
            }
        }

        public void OxygenLost()
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

        public void OxygenCharge()
        {
            _oxygen += Globals.ElapsedSeconds * 10f;
            if (_oxygen >= 100f) _oxygen = 100f;
        }

        public void EnergyLost(float value = 10)
        {
            _energy -= Globals.ElapsedSeconds * value;
            if (_energy < 0)
            {
                energyCooldown = 100;
                _energy = 0;
            }
        }

        public void EnergyCharge()
        {
            if (energyCooldown > 0) return;
            _energy += Globals.ElapsedSeconds * 0.5f;
            if (_energy >= 100f) _energy = 100f;
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
                JumpPower = 875f;
                Grounded = false;
            }

            if (JumpPower > 0 && !fly)
            {
                Position += Vector2.Normalize(new Vector2(0, -1)) * JumpPower * Globals.ElapsedSeconds;
                JumpPower -= Globals.Gravity;
                if(JumpPower < 0) JumpPower = 0;
            }

            if (Position.Y + Size.Y < Ground)
            {
                Position.Y += Globals.Gravity;
                if (fly)
                {
                    _anims.Update(new Vector2(0, -1), false, true);
                }
                else
                {
                    _anims.Update(new Vector2(0, -1), false, false);
                }
            }

            if (Position.Y + Size.Y > Ground)
            { 
                Position = new Vector2(Position.X, Ground - Size.Y);
                JumpPower = 0;
                Jump = false;
                fly = false;
                Grounded = true;
            }

            if (fly)
            {
                if (Grounded)
                {
                    //JumpPower += 600;
                }
                Position += Vector2.Normalize(new Vector2(0, -1)) * JumpPower * Globals.ElapsedSeconds;
                Grounded = false;
                Jump = false;
                EnergyLost(JumpPower * 0.02f);
            }
        }

        public void Interaction()
        {
            if (Interact)
            {
                switch (GameObjectInteract.Type)
                {
                    case 0:
                        if (_health < 100 && GameObjectInteract.ConsumeRefill())
                        {
                            HealthRefill();
                        }
                        break;

                    case 1:
                        if (_oxygen < 100 && GameObjectInteract.ConsumeRefill())
                        {
                            OxygenRefill();
                        }
                        break;

                    case 2:
                        if (_energy < 100 && GameObjectInteract.ConsumeRefill())
                        {
                            EnergyRefill();
                        }
                        break;
                }
            }
        }

        public float Oxygen()
        {
            return _oxygen;
        }       

        public void setFly(float value) { 
            if (value > 0)
            {
                fly = true;
                if (value < 0) value = 0;
                JumpPower = 800f * value;
                if (JumpPower < 300f) JumpPower = 300f;
            }
            else
            {
                fly = false;
            }
        }

        public bool isFly()
        {
            return fly;
        }
    }
}