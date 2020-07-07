using MusicPlayerAPI.Util.Enums;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MusicPlayerAPI.SongList
{
    public class AsyncRecursiveSongList : BaseRecursiveSongList
    {
        public override SongListStatus Status
        {
            get
            {
                lock (this)
                {
                    return base.Status;
                }
            }
            protected set
            {
                lock (this)
                {
                    base.Status = value;
                }
            }
        }

        public AsyncRecursiveSongList() => Songs = new ConcurrentDictionary<string, string>();

        public override void LoadSongs(string path)
        {
            CheckBeforeLoading(path);
            Status = SongListStatus.Loading;
            if (_songs.Count != 0)
            {
                _songs = new ConcurrentDictionary<string, string>();
            }
            RunRecSearch(path);
        }

        private void RunRecSearch(string path)
        {
            Task.Run(() =>
            {
                try
                {
                    RecDirSearch(path);
                    Status = SongListStatus.Loaded;
                }
                catch (Exception)
                {
                    Status = SongListStatus.LoadError;
                    Songs = new ConcurrentDictionary<string, string>();
                    return;
                }
            });
        }
    }
}