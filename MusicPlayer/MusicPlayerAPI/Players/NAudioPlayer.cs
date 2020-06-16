using MusicPlayerAPI.Util;
using MusicPlayerAPI.Util.ExtensionCheckers;
using System;
using System.IO;

namespace MusicPlayerAPI.Players
{
    public class NAudioPlayer : IPlayer
    {
        private NAudio.Wave.BlockAlignReductionStream _stream;
        private NAudio.Wave.WaveOut _audioOutput;
        private readonly IExtensionChecker _extensionChecker = new WavMp3();

        public event EventHandler StatusChanged;

        public void Dispose()
        {
            if (_audioOutput != null)
            {
                if (_audioOutput.PlaybackState != NAudio.Wave.PlaybackState.Stopped)
                {
                    _audioOutput.Stop();
                    OnStatusChanged();
                }
                _audioOutput.Dispose();
                _audioOutput = null;
            }
            if (_stream != null)
            {
                _stream.Dispose();
                _stream = null;
            }
        }

        public float GetVolume()
        {
            if (_audioOutput == null || _stream == null)
            {
                throw new InvalidOperationException("No song is loaded");
            }
            return _audioOutput.Volume;
        }

        public void SetVolume(float volume)
        {
            if (_audioOutput == null || _stream == null)
            {
                throw new InvalidOperationException("No song is loaded");
            }
            if (volume < 0.0f || volume > 1.0f)
            {
                throw new ArgumentException("Volume must be between 0 and 1");
            }
            _audioOutput.Volume = volume;
        }

        public bool LoadMusicFile(string path)
        {
            if (!PathChecker.IsPathValid(path))
            {
                return false;
            }
            if (_stream != null || _audioOutput != null)
            {
                Dispose();
            }
            if (!_extensionChecker.IsSupportedExtension(path))
            {
                return false;
            }
            _audioOutput = new NAudio.Wave.WaveOut
            {
                DeviceNumber = -1
            };
            try
            {
                if (Path.GetExtension(path) == ".wav")
                {
                    NAudio.Wave.WaveStream waveStream = new NAudio.Wave.WaveChannel32(new NAudio.Wave.WaveFileReader(path));
                    _stream = new NAudio.Wave.BlockAlignReductionStream(waveStream);
                }
                else
                {
                    NAudio.Wave.WaveStream waveStream = NAudio.Wave.WaveFormatConversionStream.CreatePcmStream(new NAudio.Wave.Mp3FileReader(path));
                    _stream = new NAudio.Wave.BlockAlignReductionStream(waveStream);
                }
                _audioOutput.Init(_stream);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public void Pause()
        {
            if (_audioOutput == null || _stream == null)
            {
                throw new InvalidOperationException("No song is loaded");
            }
            if (_audioOutput.PlaybackState == NAudio.Wave.PlaybackState.Playing)
            {
                _audioOutput.Pause();
                OnStatusChanged();
            }
        }

        public void Play()
        {
            if (_audioOutput == null || _stream == null)
            {
                throw new InvalidOperationException("No song is loaded");
            }
            if (_audioOutput.PlaybackState == NAudio.Wave.PlaybackState.Paused)
            {
                _audioOutput.Resume();
                OnStatusChanged();
            }
            if (_audioOutput.PlaybackState == NAudio.Wave.PlaybackState.Stopped)
            {
                _audioOutput.Play();
                OnStatusChanged();
            }
        }

        public void Stop()
        {
            if (_audioOutput == null || _stream == null)
            {
                throw new InvalidOperationException("No song is loaded");
            }
            if (_audioOutput.PlaybackState != NAudio.Wave.PlaybackState.Stopped)
            {
                _audioOutput.Stop();
                OnStatusChanged();
                _stream.CurrentTime = TimeSpan.Zero;
            }
        }

        private void OnStatusChanged() => StatusChanged?.Invoke(this, new PlayerStatusChangedEventArgs(ToPlayBackStatus(_audioOutput.PlaybackState)));

        private PlayBackStatus ToPlayBackStatus(NAudio.Wave.PlaybackState state)
        {
            switch (state)
            {
                case NAudio.Wave.PlaybackState.Stopped:
                    return PlayBackStatus.Stopped;

                case NAudio.Wave.PlaybackState.Playing:
                    return PlayBackStatus.Playing;

                case NAudio.Wave.PlaybackState.Paused:
                    return PlayBackStatus.Paused;

                default:
                    throw new InvalidCastException("Unknown state");
            }
        }

        public string[] GetSupportedExtensions()
        {
            return _extensionChecker.GetSuportedExtensions();
        }
    }
}