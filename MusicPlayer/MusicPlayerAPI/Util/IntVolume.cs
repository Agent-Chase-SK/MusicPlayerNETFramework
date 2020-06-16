using System;

namespace MusicPlayerAPI.Util
{
    public static class IntVolume
    {
        public static int ToIntVolume(float volume)
        {
            if (volume < 0.0f || volume > 1.0f)
            {
                throw new ArgumentException("Volume must be between 0 and 1");
            }
            return (int)Math.Round(volume * 100.0f);
        }

        public static float FromIntVolume(int volume)
        {
            if (volume < 0 || volume > 100)
            {
                throw new ArgumentException("Volume must be between 0 and 100");
            }
            return volume / 100f;
        }
    }
}