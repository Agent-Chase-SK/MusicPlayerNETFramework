using System;

namespace MusicPlayerAPI
{
    internal class SongChangedEventArgs : EventArgs
    {
        public string Song { get; }

        public SongChangedEventArgs(string song) => Song = song;
    }
}