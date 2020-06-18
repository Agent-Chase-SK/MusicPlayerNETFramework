using System;

namespace MusicPlayerAPI.Players
{
    internal class PlayerStatusChangedEventArgs : EventArgs
    {
        public PlayBackStatus Status { get; }

        public PlayerStatusChangedEventArgs(PlayBackStatus status) => Status = status;
    }
}