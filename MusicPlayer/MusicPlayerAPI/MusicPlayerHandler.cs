using MusicPlayerAPI.Players;
using MusicPlayerAPI.SongList;
using MusicPlayerAPI.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MusicPlayerAPI
{
    public class MusicPlayerHandler : IDisposable
    {
        private readonly IPlayer _player;
        private readonly ISongList _songList;
        private string _activeSong;

        public string ActiveSong
        {
            get => _activeSong;
            set
            {
                _activeSong = value;
                OnActiveSongChanged();
            }
        }

        public event EventHandler ListStatusChanged;
        public event EventHandler ActiveSongCahnged;
        public event EventHandler PlayerStatusCahnged;

        public MusicPlayerHandler(IPlayer player, ISongList songList)
        {
            if (player == null || songList == null)
            {
                throw new ArgumentException("Must provide implementations");
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

        public IList<string> GetSongList() => _songList.GetSongs().Keys.OrderBy(keys => keys).ToList();

        public void SelectSong(string song)
        {
            IDictionary<string, string> songList = _songList.GetSongs();
            if (!songList.ContainsKey(song))
            {
                throw new ArgumentException($"Song: {song} not in list");
            }
            if (!_player.LoadMusicFile(songList[song]))
            {
                throw new IOException($"Failed to load song: {song}");
            }
        }

        public void Play() => _player.Play();

        public void Pause() => _player.Pause();

        public void Stop() => _player.Stop();

        public int GetVolume() => IntVolume.ToIntVolume(_player.GetVolume());

        public void SetVoume(int volume) => _player.SetVolume(IntVolume.FromIntVolume(volume));

        public string[] GetSupportedExtensions() => _player.GetSupportedExtensions();

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

        private void OnPlayerStatusChanged(PlayBackStatus status) => ListStatusChanged?.Invoke(this, new PlayerStatusChangedEventArgs(status));

        private void OnActiveSongChanged() => ListStatusChanged?.Invoke(this, new SongChangedEventArgs(ActiveSong));
    }
}