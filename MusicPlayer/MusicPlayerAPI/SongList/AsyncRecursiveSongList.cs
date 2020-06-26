using MusicPlayerAPI.Util.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MusicPlayerAPI.SongList
{
    public class AsyncRecursiveSongList : SimpleRecursiveSongList
    {
        protected override void StartRecSearch(string path)
        {
            AsyncRecSearch(path);
        }

        private void AsyncRecSearch(string path)
        {
            Task.Run(() =>
            {
                try
                {
                    RecDirSearch(path);
                }
                catch (Exception)
                {
                    Status = SongListStatus.LoadError;
                    Songs = new Dictionary<string, string>();
                    return;
                }
                Status = SongListStatus.Loaded;
            });
        }
    }
}