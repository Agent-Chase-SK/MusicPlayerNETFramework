using System;

namespace MusicPlayerAPI.SongList
{
    public class ListStatusChangedEventArgs : EventArgs
    {
        public SongListStatus Status { get; }

        public ListStatusChangedEventArgs(SongListStatus status) => Status = status;
    }
}