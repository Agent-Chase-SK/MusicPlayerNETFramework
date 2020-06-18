using System;

namespace MusicPlayerAPI.Players
{
    public interface IPlayer : IDisposable
    {
        float Volume { get; set; }

        string[] SupportedExtensions { get; }

        PlayBackStatus Status { get; }

        event EventHandler StatusChanged;

        bool LoadMusicFile(string path);

        void Play();

        void Pause();

        void Stop();
    }
}