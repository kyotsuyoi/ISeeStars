using Microsoft.Xna.Framework.Audio;

namespace ISS
{
    internal class SoundEffectI
    {
        private EnumSoundOrigin enumSoundOrigin;
        private EnumSoundFX enumSoundFX;
        private SoundEffectInstance soundEffectInstance;

        public SoundEffectI(EnumSoundOrigin EnumSoundOrigin, EnumSoundFX EnumSoundFX, SoundEffectInstance SoundEffectInstance) 
        { 
            enumSoundOrigin = EnumSoundOrigin;
            enumSoundFX = EnumSoundFX;
            soundEffectInstance = SoundEffectInstance;
        }

        public EnumSoundOrigin EnumSoundOrigin { get => enumSoundOrigin; }
        public EnumSoundFX EnumSoundFX { get => enumSoundFX; }
        public SoundEffectInstance SoundEffectInstance { get => soundEffectInstance; }
    }
}
