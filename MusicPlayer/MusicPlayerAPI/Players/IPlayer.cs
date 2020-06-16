using System;

namespace MusicPlayerAPI.Players
{
    public interface IPlayer : IDisposable
    {
        event EventHandler StatusChanged;

        bool LoadMusicFile(string path);

        void Play();

        void Pause();

        void Stop();

        float GetVolume();

        void SetVolume(float volume);

        string[] GetSupportedExtensions();
    }
}