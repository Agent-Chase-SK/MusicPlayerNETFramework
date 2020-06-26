using MusicPlayerAPI.Util.Enums;
using System;
using System.Collections.Generic;

namespace MusicPlayerAPI.SongList
{
    public interface ISongList
    {
        IDictionary<string, string> Songs { get; }

        string[] SupportedExtensions { get; }

        SongListStatus Status { get; }

        event EventHandler StatusChanged;

        void LoadSongs(string path);
    }
}