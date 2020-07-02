using MusicPlayerAPI.Util.Enums;
using System;

namespace MusicPlayerAPI.Util.EventArguments
{
    internal class PlayerActionEventArgs : EventArgs
    {
        public PlayerActionType ActionType { get; }

        public string Path { get; }

        public float? Volume { get; }

        public PlayerActionEventArgs(PlayerActionType actionType, string path, float? volume)
        {
            if (actionType == PlayerActionType.Load && string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path must be provided");
            }
            if (actionType == PlayerActionType.SetVolume && volume == null)
            {
                throw new ArgumentException("Volume must be provided");
            }
            ActionType = actionType;
            Path = path;
            Volume = volume;
        }
    }
}