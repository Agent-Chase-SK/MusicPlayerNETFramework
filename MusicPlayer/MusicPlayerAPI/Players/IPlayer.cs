using MusicPlayerAPI.Util.Enums;
using MusicPlayerAPI.Util.ExtensionCheckers;
using System;

namespace MusicPlayerAPI.Players
{
    public interface IPlayer : IDisposable
    {
        float Volume { get; set; }

        PlayBackStatus Status { get; }

        IExtensionChecker ExtensionChecker { get; }

        event EventHandler StatusChanged;

        bool LoadMusicFile(string path);

        void Play();

        void Pause();

        void Stop();
    }
}