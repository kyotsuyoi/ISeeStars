using ISS;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace ISS
{
    public class SoundManager
    {
        private Song BGMusic;

        private List<SoundEffect> soundEffects;
        private List<SoundEffectI> soundEffectInstances = new();

        public SoundManager(float soundVolume, float fxVolume)
        {
            BGMusic = Globals.Content.Load<Song>("BGM/BGM01");
            MediaPlayer.Volume = soundVolume;

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(BGMusic);

            SoundEffect.MasterVolume = fxVolume;
            soundEffects = new List<SoundEffect>
            {
                Globals.Content.Load<SoundEffect>("SoundFX/PlayerTakeDamage01"),
                Globals.Content.Load<SoundEffect>("SoundFX/PlayerTakeDamage02"),
                Globals.Content.Load<SoundEffect>("SoundFX/PlayerDead"),
                Globals.Content.Load<SoundEffect>("SoundFX/Jump"),
                Globals.Content.Load<SoundEffect>("SoundFX/MenuNavigation1"),
                Globals.Content.Load<SoundEffect>("SoundFX/MenuSelected"),
                Globals.Content.Load<SoundEffect>("SoundFX/MenuOpen"),
                Globals.Content.Load<SoundEffect>("SoundFX/MenuNotOpen"),
                Globals.Content.Load<SoundEffect>("SoundFX/MenuClose"),
                Globals.Content.Load<SoundEffect>("SoundFX/TouchingGround"),
                Globals.Content.Load<SoundEffect>("SoundFX/JetPack"),
                Globals.Content.Load<SoundEffect>("SoundFX/Hit"),
                Globals.Content.Load<SoundEffect>("SoundFX/PlayerSuffocating")
            };
        }

        public void SetBGMVolume(bool plus)
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

        public float GetFXVolume() { return SoundEffect.MasterVolume; }

        public void PlayFX(EnumSoundFX soundFX)
        {
            if (soundFX == EnumSoundFX.None) return;
            soundEffects[(int)soundFX - 1].Play();
        }

        public void PlayFXInstance(EnumSoundFX soundFX, EnumSoundOrigin enumSoundOrigin)
        {
            if(soundFX == EnumSoundFX.None || enumSoundOrigin == EnumSoundOrigin.None) return; 
            SoundEffectInstance instance = soundEffects[(int)soundFX - 1].CreateInstance();
            instance.IsLooped = true;
            SoundEffectI soundEffectI = new SoundEffectI(enumSoundOrigin, soundFX, instance);
            if (!soundEffectInstances.Contains(soundEffectI))
            {
                soundEffectInstances.Add(soundEffectI);
                SoundEffectI sound = soundEffectInstances.Find(item => item.EnumSoundOrigin == enumSoundOrigin && item.EnumSoundFX == soundFX);
                sound.SoundEffectInstance.Play();
            }
        }

        public void StopFXInstance(EnumSoundFX soundFX, EnumSoundOrigin enumSoundOrigin)
        {
            if (soundFX == EnumSoundFX.None || enumSoundOrigin == EnumSoundOrigin.None) return;
            SoundEffectI sound = soundEffectInstances.Find(item => item.EnumSoundOrigin == enumSoundOrigin && item.EnumSoundFX == soundFX);
            if (sound != null)
            {
                sound.SoundEffectInstance.Stop();
            }
        }
    }
}


