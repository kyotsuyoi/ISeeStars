using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace ISS
{
    public class SoundManager
    {
        private Song BGMusic;

        private List<SoundEffect> soundEffects;

        public SoundManager() 
        {
            BGMusic = Globals.Content.Load<Song>("BGM01");
            MediaPlayer.Volume = 1f;

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(BGMusic);

            SoundEffect.MasterVolume = 1f;
            soundEffects = new List<SoundEffect>
            {
                Globals.Content.Load<SoundEffect>("SoundFX_PlayerTakeDamage01"),
                Globals.Content.Load<SoundEffect>("SoundFX_PlayerTakeDamage02"),
                Globals.Content.Load<SoundEffect>("SoundFX_PlayerDead"),
                Globals.Content.Load<SoundEffect>("SoundFX_PlayerJump01"),
                Globals.Content.Load<SoundEffect>("SoundFX_PlayerJump02"),
                Globals.Content.Load<SoundEffect>("SoundFX_MenuNavigation"),
                Globals.Content.Load<SoundEffect>("SoundFX_MenuSelected"),
                Globals.Content.Load<SoundEffect>("SoundFX_MenuOpen"),
                Globals.Content.Load<SoundEffect>("SoundFX_MenuNotOpen"),
                Globals.Content.Load<SoundEffect>("SoundFX_TouchingGround")
            };

            //soundEffects[0].Play();
            //var instance = soundEffects[0].CreateInstance();
            //instance.IsLooped = true;
            //instance.Play();
        }

        public void SetBGMVolumePlus(bool plus)
        {
            // 0.0f is silent, 1.0f is full volume
            if (plus)
            {
                MediaPlayer.Volume += 0.01f;
            }
            else
            {
                MediaPlayer.Volume -= 0.01f;
            }
        }

        public float GetBGMVolume() { return MediaPlayer.Volume; }

        public void SetFXVolume(bool plus) 
        {
            if (plus)
            {
                if (SoundEffect.MasterVolume >= 1f) return;
                SoundEffect.MasterVolume += 0.01f;
            }
            else
            {
                if (SoundEffect.MasterVolume <= 0.01f) return;
                SoundEffect.MasterVolume -= 0.01f;
            }
        }

        public float GetFXVolume() {  return SoundEffect.MasterVolume; }

        public void PlayFX(EnumSoundFX soundFX) {
            if (soundFX == EnumSoundFX.None) return;
            soundEffects[(int)soundFX-1].Play(); 
        }
    }
}
