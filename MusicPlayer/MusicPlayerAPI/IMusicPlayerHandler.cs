﻿using MusicPlayerAPI.Players;
using MusicPlayerAPI.SongList;
using System;
using System.Collections.Generic;

namespace MusicPlayerAPI
{
    internal interface IMusicPlayerHandler : IDisposable
    {
        string ActiveSong { get; }

        IList<string> Songs { get; }

        SongListStatus ListStatus { get; }

        PlayBackStatus PlayerStatus { get; }

        int Volume { get; set; }

        string[] SupportedExtensions { get; }

        event EventHandler ListStatusChanged;

        event EventHandler PlayerStatusChanged;

        event EventHandler ActiveSongChanged;

        void LoadSongs(string path);

        void SelectSong(string song);

        void Play();

        void Pause();

        void Stop();
    }
}