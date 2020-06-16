using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayerAPI.Players
{
    class PlayerStatusChangedEventArgs : EventArgs
    {
        public PlayBackStatus Status { get; }

        public PlayerStatusChangedEventArgs(PlayBackStatus status) => Status = status;
    }
}
