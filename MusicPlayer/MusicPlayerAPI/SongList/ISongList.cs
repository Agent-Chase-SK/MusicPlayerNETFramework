using System;
using System.Collections.Generic;

namespace MusicPlayerAPI.SongList
{
    public interface ISongList
    {
        event EventHandler StatusChanged;

        bool LoadSongs(string path);

        IDictionary<string, string> GetSongs();
    }
}