using MusicPlayerAPI;
using MusicPlayerAPI.Players;
using MusicPlayerAPI.SongList;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MusicPlayerModule.ViewModels
{
    class MusicPlayerViewModel : BindableBase, IDisposable
    {
        private readonly IMusicPlayerHandler _musicPlayerHandler;
        private IList<string> _songs;
        private string _selectedSong = null;
        private string _currentStatus = "No song selected";

        public IList<string> Songs
        {
            get => _songs;
            set => SetProperty(ref _songs, value);
        }

        public string SelectedSong
        {
            get => _selectedSong;
            set => SetProperty(ref _selectedSong, value);
        }

        public string CurrentStatus
        {
            get => _currentStatus;
            set => SetProperty(ref _currentStatus, value);
        }

        public DelegateCommand SelectFolderCommand { get; set; }

        public DelegateCommand SelectSongCommand { get; set; }

        public MusicPlayerViewModel(IPlayer player, ISongList songList)
        {
            _musicPlayerHandler = new MusicPlayerHandler(player, songList);

            _musicPlayerHandler.ListStatusChanged += ListStatusChangedDetected;
            _musicPlayerHandler.PlayerStatusChanged += PlayerStatusChangedDetected;

            Songs = _musicPlayerHandler.Songs;

            SelectFolderCommand = new DelegateCommand(SelectFolderExecute, SelectFolderCanExecute);
            SelectSongCommand = new DelegateCommand(SelectSongExecute, SelectSongCanExecute).ObservesProperty(() => SelectedSong);
        }

        public void Dispose()
        {
            _musicPlayerHandler.ListStatusChanged -= ListStatusChangedDetected;
            _musicPlayerHandler.PlayerStatusChanged -= PlayerStatusChangedDetected;
            _musicPlayerHandler.Dispose();
        }

        private void SelectFolderExecute()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    _musicPlayerHandler.LoadSongs(dialog.SelectedPath);
                    Songs = _musicPlayerHandler.Songs;
                }
            }
        }

        private bool SelectFolderCanExecute()
        {
            return _musicPlayerHandler.ListStatus != SongListStatus.Loading;
        }

        private void SelectSongExecute()
        {
            _musicPlayerHandler.ActiveSong = SelectedSong;
            CurrentStatus = CreateStatusMsg();
        }

        private bool SelectSongCanExecute()
        {
            return SelectedSong != null;
        }

        private void ListStatusChangedDetected(object sender, EventArgs args)
        {
            SelectFolderCommand.RaiseCanExecuteChanged();
        }

        private void PlayerStatusChangedDetected(object sender, EventArgs args)
        {
            CurrentStatus = CreateStatusMsg();
        }

        private string CreateStatusMsg()
        {
            PlayBackStatus status = _musicPlayerHandler.PlayerStatus;
            switch (status)
            {
                case PlayBackStatus.Stopped:
                    return "Stopped:";
                case PlayBackStatus.Playing:
                    return "Now playing:";
                case PlayBackStatus.Paused:
                    return "Paused:";
                default:
                    throw new InvalidCastException($"Unknown status: {status}");
            }
        }
    }
}
