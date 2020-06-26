using MusicPlayerAPI.Players;
using MusicPlayerAPI.SongList;
using MusicPlayerAPI.Util;
using MusicPlayerAPI.Util.Enums;
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
        private IDictionary<string, string> _songs;

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
            get => _songs.Keys.OrderBy(key => key).ToList();
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

            _songs = _songList.Songs;

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
            _songList.LoadSongs(path);
        }

        public void Play() => _player.Play();

        public void Pause() => _player.Pause();

        public void Stop() => _player.Stop();

        private void SelectSong(string song)
        {
            if (!_songs.ContainsKey(song))
            {
                throw new ArgumentException($"Song: {song} not in list");
            }
            if (!_player.LoadMusicFile(_songs[song]))
            {
                throw new IOException($"Failed to load song: {song}");
            }
            _activeSong = song;
        }

        private void ListStatusChangedDetected(object sender, EventArgs args)
        {
            if (_songList.Status == SongListStatus.Loaded)
            {
                _songs = _songList.Songs;
            }
            OnListStatusChanged();
        }

        private void PlayerStatusChangedDetected(object sender, EventArgs args) => OnPlayerStatusChanged();

        private void OnListStatusChanged() => ListStatusChanged?.Invoke(this, EventArgs.Empty);

        private void OnPlayerStatusChanged() => PlayerStatusChanged?.Invoke(this, EventArgs.Empty);

        private void OnActiveSongChanged() => ActiveSongChanged?.Invoke(this, EventArgs.Empty);
    }
}