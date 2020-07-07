using MusicPlayerAPI.Util.Enums;
using MusicPlayerAPI.Util.ExtensionCheckers;
using System;
using System.Collections.Generic;

namespace MusicPlayerAPI.SongList
{
    public interface ISongList
    {
        IDictionary<string, string> Songs { get; }

        SongListStatus Status { get; }

        IExtensionChecker ExtensionChecker { get; set; }

        event EventHandler StatusChanged;

        void LoadSongs(string path);
    }
}