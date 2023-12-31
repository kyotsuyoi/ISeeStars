﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace ISS
{
    public class Player
    {
        private Texture2D texture;
        public Vector2 Position;
        public Vector2 Size;
        //private float speed;
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
        private int fallingDown = 0;
        //Verify if the player is falling
        public float LastY = 0;

        //private readonly ProgressBar _healthBar;
        private readonly ProgressBarAnimated _healthBarAnimated;
        private readonly ProgressBarAnimated _oxigenBarAnimated;
        private readonly ProgressBarAnimated _energyBarAnimated;

        private readonly AnimationManager _anims = new AnimationManager();
        private List<EnumSoundFX> soundFXes = new List<EnumSoundFX>();
        private List<EnumSoundFX> soundFXesI = new List<EnumSoundFX>();
        private List<EnumSoundFX> soundFXesIStop = new List<EnumSoundFX>();

        private bool Alive = true;
        private int hurts = 0;
        public bool ObstructionLeft = false;
        public bool ObstructionRight = false;

        public Player(Vector2 position, float health = 100f, float oxigen = 100f, float energy = 100f)
        {
            //speed = 0f;
            int framesX = 5;
            int framesY = 3;
            texture = Globals.Content.Load<Texture2D>("Sprite/Player01-1");
            _anims.AddAnimation(new Vector2(0, 0),  new Animation(texture, framesX, framesY, 0, 2, 0.25f, 1)); //Stand
            _anims.AddAnimation(new Vector2(1, 0),  new Animation(texture, framesX, framesY, 0, 3, 0.1f, 2));  //Right
            _anims.AddAnimation(new Vector2(-1, 0), new Animation(texture, framesX, framesY, 0, 3, 0.1f, 2)); //Left
            _anims.AddAnimation(new Vector2(0, -1), new Animation(texture, framesX, framesY, 0, 4, 0.05f, 3)); //Jump & Fly
            _anims.AddAnimation(new Vector2(0, 1),  new Animation(texture, framesX, framesY, 3, 4, 0.25f, 1)); //Crouch & Die
            //_anims.AddAnimation(new Vector2(0, -1), new Animation(texture, framesX, framesY, 0, 3, 0.25f, 3)); //Fly

            Origin = new Vector2((texture.Width / framesX) /2, (texture.Height / framesY) /2);
            Size = new Vector2(texture.Width / framesX, texture.Height / framesY);
            Position = new Vector2(position.X + Size.X/2, position.Y);

            var BarBackground = Globals.Content.Load<Texture2D>("Hud/BarBackground");
            var BarHealth = Globals.Content.Load<Texture2D>("Hud/BarHealth");
            var BarOxigen = Globals.Content.Load<Texture2D>("Hud/BarOxigen");
            var BarEnergy = Globals.Content.Load<Texture2D>("Hud/BarEnergy");

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

            //_healthBar.Update(_health);
            _healthBarAnimated.Update(_health);

            if (!Alive)
            {
                Crouch = true;
                _anims.Update(new Vector2(0, 1), false, 1);
                fly = false;
                FallingDownResolve();
                JumpFlyResolve();
                return;
            }

            if(hurts > 0)
            {
                Crouch = true;
                _anims.Update(new Vector2(0, 1), false, 1);
                hurts--;
                return;
            }

            _oxigenBarAnimated.Update(_oxygen);
            _energyBarAnimated.Update(_energy);

            if (Running || fly)
            {
                _anims.Update(InputManager.Direction, true);
            }
            else
            {
                _anims.Update(InputManager.Direction, false);
            }

            if (Running) OxygenLost(); EnergyCharge();
            FallingDownResolve();
            JumpFlyResolve();

            if (Crouch)_anims.Update(new Vector2(0, 1), false, 0);            

            if (Interact)
            {
                switch (GameObjectInteract.GetObjectType())
                {
                    case EnumGameObjectType.Health:
                        HeathCharge();
                        break;

                    case EnumGameObjectType.Oxygen:
                        OxygenCharge();
                        break;

                    case EnumGameObjectType.Energy:
                        EnergyCharge();
                        break;
                }
            }

            Position.X = Globals.ScreenSize.X/2 - Size.X/2;
            OxygenLost();
            //EnergyCharge();
            energyCooldown--;

            if(!Interact && ObstructionLeft) ObstructionLeft = false;
            if (!Interact && ObstructionRight) ObstructionRight = false;
        }
         
        public void Draw()
        {
            _anims.Draw(Position);
            //_healthBar.Draw();
            _healthBarAnimated.Draw();
            _oxigenBarAnimated.Draw();
            _energyBarAnimated.Draw();

            if (!Alive)
            {
                SpriteFont font = Globals.Content.Load<SpriteFont>("Font/fontMedium");
                Globals.SpriteBatch.DrawString(font, " ___", new Vector2(Position.X + 2, Position.Y-60), Color.Red, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9998f);
                Globals.SpriteBatch.DrawString(font, "/     \\", new Vector2(Position.X + 4, Position.Y -40), Color.Red, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9997f);
                Globals.SpriteBatch.DrawString(font, "| ()() |", new Vector2(Position.X + 2, Position.Y -20), Color.Red, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9997f);
                Globals.SpriteBatch.DrawString(font, "\\  ^  /", new Vector2(Position.X + 2, Position.Y), Color.Red, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9997f);
                Globals.SpriteBatch.DrawString(font, "  |||||", new Vector2(Position.X + 2, Position.Y + 20), Color.Red, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9997f);
            }

            if (Alive && hurts > 0)
            {
                SpriteFont font = Globals.Content.Load<SpriteFont>("Font/fontMedium");
                Globals.SpriteBatch.DrawString(font, "  ===", new Vector2(Position.X, Position.Y - 80), Color.Red, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9998f);
                Globals.SpriteBatch.DrawString(font, "    ||", new Vector2(Position.X + 4, Position.Y - 60), Color.Red, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9997f);
                Globals.SpriteBatch.DrawString(font, "  //\\\\", new Vector2(Position.X + 2, Position.Y -40), Color.Red, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9997f);
                Globals.SpriteBatch.DrawString(font, "  ||__||", new Vector2(Position.X - 4, Position.Y -20), Color.Red, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9997f);
                Globals.SpriteBatch.DrawString(font, "  \\\\//", new Vector2(Position.X + 2, Position.Y), Color.Red, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9997f);
                Globals.SpriteBatch.DrawString(font, "    ||", new Vector2(Position.X + 4, Position.Y + 20), Color.Red, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9997f);
            }
        }

        public void TakeDamage(float damage)
        {
            _health -= damage;
            if (_health < 0)
            {
                _health = 0;
                if(Alive) soundFXes.Add(EnumSoundFX.PlayerDead);
                soundFXes.Add(EnumSoundFX.Hit);
                Alive = false;
            }
            if (!Alive) return;
            if(damage > 40)
            {
                soundFXes.Add(EnumSoundFX.PlayerTakeDamage2);
                soundFXes.Add(EnumSoundFX.Hit);
                hurts = 100;
            }
            else
            {
                soundFXes.Add(EnumSoundFX.PlayerTakeDamage1);
            }
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
            if (this.Interact)
            {
                if (GameObjectInteract.GetObjectType() != EnumGameObjectType.Oxygen)
                {
                    _oxygen -= Globals.ElapsedSeconds;
                }
            }
            else
            {
                _oxygen -= Globals.ElapsedSeconds;
            }

            if (_oxygen <= 0)
            {
                _oxygen = 0;
                _health -= Globals.ElapsedSeconds * 8;
                soundFXesI.Add(EnumSoundFX.PlayerSuffocating);
                if (_health < 0)
                {
                    Alive = false;
                    soundFXes.Add(EnumSoundFX.PlayerDead);
                    _health = 0;
                    soundFXesIStop.Add(EnumSoundFX.PlayerSuffocating);
                }
                texture = Globals.Content.Load<Texture2D>("Sprite/Player01-2");
            }
            else
            {
                if (soundFXesIStop.Contains(EnumSoundFX.PlayerSuffocating)) return;
                soundFXesIStop.Add(EnumSoundFX.PlayerSuffocating);
                texture = Globals.Content.Load<Texture2D>("Sprite/Player01-1");
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

        public void setEnergy(float value)
        {
            _energy -= value;
            if(_energy<0) _energy = 0;
        }

        private void JumpFlyResolve()
        {     
            if (Jump && JumpPower <= 0 && Grounded)
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
                    _anims.Update(new Vector2(0, -1), false);
                }
                else
                {
                    _anims.Update(new Vector2(0, -1), false, 0);
                }
            }

            if (Position.Y + Size.Y > Ground)
            { 
                Position = new Vector2(Position.X, Ground - Size.Y);
                JumpPower = 0;
                Jump = false;
                fly = false;
                Grounded = true;
                soundFXes.Add(EnumSoundFX.TouchingGround);
            }

            if (fly)
            {               
                Position += Vector2.Normalize(new Vector2(0, -1)) * JumpPower * Globals.ElapsedSeconds;
                Grounded = false;
                Jump = false;
                EnergyLost(JumpPower * 0.02f);
            }           

        }

        private void FallingDownResolve()
        {
            if (LastY == Position.Y)
            {
                Grounded = true;
            }
            if (LastY < Position.Y)
            {
                Grounded = false;
            }
            LastY = Position.Y;

            if (Grounded && fallingDown -15 > 0 && !fly)
            {
                TakeDamage(fallingDown);
                Jump = false;
                JumpPower = 300;
            }

            if (!Grounded && JumpPower <= 425)
            {
                fallingDown++;
            }
            else
            {
                fallingDown = 0;
            }
        }

        public EnumInteractionType Interaction()
        {
            if (Interact)
            {
                switch (GameObjectInteract.GetObjectType())
                {
                    case EnumGameObjectType.Default:
                        return EnumInteractionType.MachineDefault;

                    case EnumGameObjectType.Health:
                        if (_health < 100 && GameObjectInteract.ConsumeRefill())
                        {
                            HealthRefill();
                            soundFXes.Add(EnumSoundFX.MenuSelected);
                        }
                        else
                        {
                            soundFXes.Add(EnumSoundFX.MenuNotOpen);
                        }
                        break;

                    case EnumGameObjectType.Oxygen:
                        if (_oxygen < 100 && GameObjectInteract.ConsumeRefill())
                        {
                            OxygenRefill();
                            soundFXes.Add(EnumSoundFX.MenuSelected);
                        }
                        else
                        {
                            soundFXes.Add(EnumSoundFX.MenuNotOpen);
                        }
                        break;

                    case EnumGameObjectType.Energy:
                        if (_energy < 100 && GameObjectInteract.ConsumeRefill())
                        {
                            EnergyRefill();
                            soundFXes.Add(EnumSoundFX.MenuSelected);
                        }
                        else
                        {
                            soundFXes.Add(EnumSoundFX.MenuNotOpen);
                        }
                        break;
                }
            }
            return EnumInteractionType.None;
        }

        public float GetOxygen()
        {
            return _oxygen;
        }       

        public void SetFly(float value) { 
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

        public bool IsFlying()
        {
            return fly;
        }

        public EnumSoundFX GetSoundFX()
        {
            if (soundFXes.Count == 0) return EnumSoundFX.None;
            var soundFX = soundFXes[0];
            soundFXes.Remove(soundFX);
            return soundFX;
        }

        public int GetFallingDown()
        {
            return fallingDown;
        }

        public void Revive()
        {
            Alive = true;
        }

        public bool IsAlive()
        {
           return Alive;
        }

        public float GetHealth()
        {
            return _health;
        }

        public float GetEnergy()
        {
            return _energy;
        }

        public EnumSoundFX GetSoundFXInstance()
        {
            if (soundFXesI.Count == 0) return EnumSoundFX.None;
            var soundFX = soundFXesI[0];
            soundFXesI.Remove(soundFX);
            return soundFX;
        }

        public EnumSoundFX GetStopSoundFXInstance()
        {
            if (soundFXesIStop.Count == 0) return EnumSoundFX.None;
            var soundFX = soundFXesIStop[0];
            soundFXesIStop.Remove(soundFX);
            return soundFX;
        }

        public void ObjectCollideResolve(GameObject GameObjectInteract)
        {
            Interact = true;
            this.GameObjectInteract = GameObjectInteract;

            var bottom = Position.Y + Size.Y;
            var bottomLimit = bottom - Size.Y * 0.1;
            if ((bottom >= GameObjectInteract.Position.Y) && (bottomLimit < GameObjectInteract.Position.Y)
                && (LastY < Position.Y))
            {
                Position.Y = GameObjectInteract.Position.Y - Size.Y;
                Ground = GameObjectInteract.Position.Y;
                Grounded = true;
            }
        }

        public bool IsHurt()
        {
            if(hurts > 0) return true;
            return false;
        }
    }
}