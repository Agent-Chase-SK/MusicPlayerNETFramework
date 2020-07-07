using MusicPlayerAPI.Util.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace MusicPlayerAPI.SongList
{
    public class SimpleRecursiveSongList : BaseRecursiveSongList
    {
        public SimpleRecursiveSongList() => Songs = new Dictionary<string, string>();

        public override void LoadSongs(string path)
        {
            CheckBeforeLoading(path);
            Status = SongListStatus.Loading;
            if (_songs.Count != 0)
            {
                _songs = new Dictionary<string, string>();
            }
            TryRecSearch(path);
        }

        private void TryRecSearch(string path)
        {
            try
            {
                RecDirSearch(path);
                Status = SongListStatus.Loaded;
            }
            catch (Exception)
            {
                Status = SongListStatus.LoadError;
                Songs = new Dictionary<string, string>();
                return;
            }
        }
    }
}