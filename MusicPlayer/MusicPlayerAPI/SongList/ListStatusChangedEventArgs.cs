using System;

namespace MusicPlayerAPI.SongList
{
    internal class ListStatusChangedEventArgs : EventArgs
    {
        public SongListStatus Status { get; }

        public ListStatusChangedEventArgs(SongListStatus status) => Status = status;
    }
}