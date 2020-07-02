using MusicPlayerAPI.Util;
using MusicPlayerAPI.Util.Enums;
using MusicPlayerAPI.Util.EventArguments;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MusicPlayerAPI.Players
{
    public class LoopNAudioPlayer : IPlayer
    {
        private readonly LoopPlayer _loopPlayer;
        private float _volume = 1f;
        private PlayBackStatus _status = PlayBackStatus.Stopped;

        public float Volume
        {
            get => _volume;
            set
            {
                _volume = value;
                OnActionRequest(PlayerActionType.SetVolume, volume: value);
            }
        }

        public string[] SupportedExtensions { get; private set; }

        public PlayBackStatus Status
        {
            get => _status;
            private set
            {
                _status = value;
                OnStatusChanged();
            }
        }

        private event EventHandler ActionRequest;

        public event EventHandler StatusChanged;

        public LoopNAudioPlayer()
        {
            _loopPlayer = new LoopPlayer(this, new NAudioPlayer());
            _loopPlayer.StatusChanged += StatusChangedDetected;

            Task.Run(() => _loopPlayer.RunLoop());
        }

        public void Dispose()
        {
            _loopPlayer.StatusChanged -= StatusChangedDetected;
            OnActionRequest(PlayerActionType.TerminateLoop);
            _loopPlayer.Dispose();
        }

        public bool LoadMusicFile(string path)
        {
            if (!PathChecker.IsPathValid(path))
            {
                return false;
            }
            OnActionRequest(PlayerActionType.Load, path: path);
            return true;
        }

        public void Pause()
        {
            OnActionRequest(PlayerActionType.Pause);
        }

        public void Play()
        {
            OnActionRequest(PlayerActionType.Play);
        }

        public void Stop()
        {
            OnActionRequest(PlayerActionType.Stop);
        }

        private void OnActionRequest(PlayerActionType actionType, string path = null, float? volume = null)
        {
            ActionRequest?.Invoke(this, new PlayerActionEventArgs(actionType, path, volume));
        }

        private void OnStatusChanged()
        {
            StatusChanged?.Invoke(this, EventArgs.Empty);
        }

        private void StatusChangedDetected(object sender, EventArgs args)
        {
            if (!(args is PlayerStatusEventArgs eventArgs))
            {
                throw new ArgumentException("EventArgs were null");
            }
            Status = eventArgs.Status;
        }

        private class LoopPlayer : IDisposable
        {
            private bool _terminateLoop;
            private readonly IPlayer _player;
            private readonly LoopNAudioPlayer _loopNAudioPlayer;
            private ManualResetEvent _resetEvent;
            private Queue<PlayerActionEventArgs> actionQueue = new Queue<PlayerActionEventArgs>();

            internal event EventHandler StatusChanged;

            public LoopPlayer(LoopNAudioPlayer loopNAudioPlayer, IPlayer player)
            {
                _terminateLoop = false;
                _player = player;
                _loopNAudioPlayer = loopNAudioPlayer;
                _resetEvent = new ManualResetEvent(false);

                _player.StatusChanged += StatusChangedDetected;
                _loopNAudioPlayer.ActionRequest += ActionRequestDetected;

                _loopNAudioPlayer.SupportedExtensions = _player.SupportedExtensions;
            }

            public void Dispose()
            {
                _player.StatusChanged -= StatusChangedDetected;
                _loopNAudioPlayer.ActionRequest -= ActionRequestDetected;
                _player.Dispose();
            }

            public void RunLoop()
            {
                while (!_terminateLoop)
                {
                    _resetEvent.WaitOne();
                    HandleRequest();
                    if (actionQueue.Count == 0)
                    {
                        _resetEvent.Reset();
                    }
                }
            }

            private void HandleRequest()
            {
                if (actionQueue.Count == 0)
                {
                    return;
                }
                PlayerActionEventArgs currentActionArgs = actionQueue.Dequeue();
                switch (currentActionArgs.ActionType)
                {
                    case PlayerActionType.Load:
                        _player.LoadMusicFile(currentActionArgs.Path);
                        break;

                    case PlayerActionType.Play:
                        _player.Play();
                        break;

                    case PlayerActionType.Pause:
                        _player.Pause();
                        break;

                    case PlayerActionType.Stop:
                        _player.Stop();
                        break;

                    case PlayerActionType.SetVolume:
                        SetVolume(currentActionArgs.Volume);
                        break;

                    case PlayerActionType.TerminateLoop:
                        _terminateLoop = true;
                        break;

                    default:
                        throw new InvalidOperationException("This shouldnt happen (playerActionType)");
                }
            }

            private void SetVolume(float? volume)
            {
                if (volume != null)
                {
                    TrySetVolume((float)volume);
                }
            }

            private void TrySetVolume(float volume)
            {
                try
                {
                    _player.Volume = volume;
                }
                catch (InvalidOperationException) { }
            }

            private void OnStatusChanged()
            {
                StatusChanged?.Invoke(this, new PlayerStatusEventArgs(_player.Status));
            }

            private void ActionRequestDetected(object sender, EventArgs args)
            {
                if (!(args is PlayerActionEventArgs eventArgs))
                {
                    throw new ArgumentException("EventArgs were null");
                }
                actionQueue.Enqueue(new PlayerActionEventArgs(eventArgs.ActionType, eventArgs.Path, eventArgs.Volume));
                _resetEvent.Set();
            }

            private void StatusChangedDetected(object sender, EventArgs args)
            {
                OnStatusChanged();
            }
        }
    }
}