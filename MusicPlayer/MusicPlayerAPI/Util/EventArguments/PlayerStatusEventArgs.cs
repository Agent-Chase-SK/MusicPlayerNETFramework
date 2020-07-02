using MusicPlayerAPI.Util.Enums;
using System;

namespace MusicPlayerAPI.Util.EventArguments
{
    internal class PlayerStatusEventArgs : EventArgs
    {
        public PlayBackStatus Status { get; }

        public PlayerStatusEventArgs(PlayBackStatus status)
        {
            Status = status;
        }
    }
}