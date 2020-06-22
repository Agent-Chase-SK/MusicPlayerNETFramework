using MusicPlayerAPI.Players;
using MusicPlayerAPI.SongList;
using MusicPlayerAPI.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MusicPlayerAPI
{
    public class MusicPlayerHandler : IMusicPlayerHandler
    {
        private readonly IPlayer _player;
        private readonly ISongList _songList;
        private string _activeSong;

        public string ActiveSong
        {
            get => _activeSong;
            set
            {
                SelectSong(value);
                OnActiveSongChanged();
            }
        }

        public IList<string> Songs
        {
            get
            {
                try
                {
                    return _songList.Songs.Keys.OrderBy(key => key).ToList();
                }
                catch (InvalidOperationException)
                {
                    return new List<string>();
                }
            }
        }

        public SongListStatus ListStatus
        {
            get => _songList.Status;
        }

        public PlayBackStatus PlayerStatus
        {
            get => _player.Status;
        }

        public int Volume
        {
            get => IntVolume.ToIntVolume(_player.Volume);
            set => _player.Volume = IntVolume.FromIntVolume(value);
        }

        public string[] SupportedExtensions
        {
            get => _player.SupportedExtensions;
        }

        public event EventHandler ListStatusChanged;

        public event EventHandler PlayerStatusChanged;

        public event EventHandler ActiveSongChanged;

        public MusicPlayerHandler(IPlayer player, ISongList songList)
        {
            if (player == null || songList == null)
            {
                throw new ArgumentException("Must provide implementations");
            }
            if (!player.SupportedExtensions.SequenceEqual(songList.SupportedExtensions))
            {
                throw new ArgumentException("Implementations incompatibile");
            }
            _player = player;
            _songList = songList;
            _songList.StatusChanged += ListStatusChangedDetected;
            _player.StatusChanged += PlayerStatusChangedDetected;
        }

        public void Dispose()
        {
            _songList.StatusChanged -= ListStatusChangedDetected;
            _player.StatusChanged -= PlayerStatusChangedDetected;
            _player.Dispose();
        }

        public void LoadSongs(string path)
        {
            if (!PathChecker.IsPathValid(path))
            {
                throw new ArgumentException($"{path} is invalid path");
            }
            if (!_songList.LoadSongs(path))
            {
                throw new IOException("Failed to load songs");
            }
        }

        public void Play() => _player.Play();

        public void Pause() => _player.Pause();

        public void Stop() => _player.Stop();

        private void SelectSong(string song)
        {
            IDictionary<string, string> songList = _songList.Songs;
            if (!songList.ContainsKey(song))
            {
                throw new ArgumentException($"Song: {song} not in list");
            }
            if (!_player.LoadMusicFile(songList[song]))
            {
                throw new IOException($"Failed to load song: {song}");
            }
            _activeSong = song;
        }

        private void ListStatusChangedDetected(object sender, EventArgs args)
        {
            if (!(args is ListStatusChangedEventArgs eventArgs))
            {
                throw new ArgumentException("EventArgs were null");
            }
            OnListStatusChanged(eventArgs.Status);
        }

        private void PlayerStatusChangedDetected(object sender, EventArgs args)
        {
            if (!(args is PlayerStatusChangedEventArgs eventArgs))
            {
                throw new ArgumentException("EventArgs were null");
            }
            OnPlayerStatusChanged(eventArgs.Status);
        }

        private void OnListStatusChanged(SongListStatus status) => ListStatusChanged?.Invoke(this, new ListStatusChangedEventArgs(status));

        private void OnPlayerStatusChanged(PlayBackStatus status) => PlayerStatusChanged?.Invoke(this, new PlayerStatusChangedEventArgs(status));

        private void OnActiveSongChanged() => ActiveSongChanged?.Invoke(this, new SongChangedEventArgs(ActiveSong));
    }
}