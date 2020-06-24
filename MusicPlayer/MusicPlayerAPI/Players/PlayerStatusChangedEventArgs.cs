using System;

namespace MusicPlayerAPI.Players
{
    public class PlayerStatusChangedEventArgs : EventArgs
    {
        public PlayBackStatus Status { get; }

        public PlayerStatusChangedEventArgs(PlayBackStatus status) => Status = status;
    }
}