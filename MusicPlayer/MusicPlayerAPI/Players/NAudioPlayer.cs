using MusicPlayerAPI.Util;
using MusicPlayerAPI.Util.Enums;
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

        private NAudio.Wave.WaveOut AudioOutput
        {
            get => _audioOutput;
            set
            {
                if (_audioOutput != null)
                {
                    _audioOutput.PlaybackStopped -= PlaybackStoppedDetected;
                }
                _audioOutput = value;
                if (_audioOutput != null)
                {
                    _audioOutput.PlaybackStopped += PlaybackStoppedDetected;
                }
            }
        }

        public float Volume
        {
            get
            {
                CheckLoadedSong();
                return AudioOutput.Volume;
            }

            set
            {
                CheckLoadedSong();
                if (value < 0.0f || value > 1.0f)
                {
                    throw new ArgumentException("Volume must be between 0 and 1");
                }
                AudioOutput.Volume = value;
            }
        }

        public PlayBackStatus Status
        {
            get => ToPlayBackStatus(AudioOutput.PlaybackState);
        }

        public string[] SupportedExtensions
        {
            get => _extensionChecker.GetSuportedExtensions();
        }

        public event EventHandler StatusChanged;

        public void Dispose()
        {
            if (AudioOutput != null)
            {
                if (AudioOutput.PlaybackState != NAudio.Wave.PlaybackState.Stopped)
                {
                    AudioOutput.Stop();
                    OnStatusChanged();
                }
                AudioOutput.Dispose();
                AudioOutput = null;
            }
            if (_stream != null)
            {
                _stream.Dispose();
                _stream = null;
            }
        }

        public bool LoadMusicFile(string path)
        {
            if (!PathChecker.IsPathValid(path))
            {
                return false;
            }
            if (_stream != null || AudioOutput != null)
            {
                Dispose();
            }
            if (!_extensionChecker.IsSupportedExtension(path))
            {
                return false;
            }
            AudioOutput = new NAudio.Wave.WaveOut
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
                AudioOutput.Init(_stream);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public void Pause()
        {
            CheckLoadedSong();
            if (AudioOutput.PlaybackState == NAudio.Wave.PlaybackState.Playing)
            {
                AudioOutput.Pause();
                OnStatusChanged();
            }
        }

        public void Play()
        {
            CheckLoadedSong();
            if (AudioOutput.PlaybackState == NAudio.Wave.PlaybackState.Paused)
            {
                AudioOutput.Resume();
                OnStatusChanged();
            }
            if (AudioOutput.PlaybackState == NAudio.Wave.PlaybackState.Stopped)
            {
                AudioOutput.Play();
                OnStatusChanged();
            }
        }

        public void Stop()
        {
            CheckLoadedSong();
            if (AudioOutput.PlaybackState != NAudio.Wave.PlaybackState.Stopped)
            {
                AudioOutput.Stop();
                OnStatusChanged();
                _stream.CurrentTime = TimeSpan.Zero;
            }
        }

        private void OnStatusChanged() => StatusChanged?.Invoke(this, EventArgs.Empty);

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

        private void CheckLoadedSong()
        {
            if (AudioOutput == null || _stream == null)
            {
                throw new InvalidOperationException("No song is loaded");
            }
        }

        private void PlaybackStoppedDetected(object sender, EventArgs args)
        {
            OnStatusChanged();
        }
    }
}