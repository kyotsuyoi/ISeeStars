using Microsoft.Xna.Framework.Media;

namespace ISS
{
    public class SongManager
    {
        private Song BGSong;
        public SongManager() 
        {
            BGSong = Globals.Content.Load<Song>("BGM01");
            MediaPlayer.Volume = 0.4f;

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(BGSong);
        }
        public void MediaPlayer_VolumePlus(bool plus)
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

        public void Update()
        {
           
        }
    }
}
