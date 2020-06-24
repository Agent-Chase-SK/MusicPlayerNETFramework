using MusicPlayerAPI.Players;
using MusicPlayerAPI.SongList;
using System;
using System.Collections.Generic;

namespace MusicPlayerAPI
{
    public interface IMusicPlayerHandler : IDisposable
    {
        string ActiveSong { get; set; }

        IList<string> Songs { get; }

        SongListStatus ListStatus { get; }

        PlayBackStatus PlayerStatus { get; }

        int Volume { get; set; }

        string[] SupportedExtensions { get; }

        event EventHandler ListStatusChanged;

        event EventHandler PlayerStatusChanged;

        event EventHandler ActiveSongChanged;

        void LoadSongs(string path);

        void Play();

        void Pause();

        void Stop();
    }
}