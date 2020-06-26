using MusicPlayerAPI;
using MusicPlayerAPI.Players;
using MusicPlayerAPI.SongList;
using MusicPlayerAPI.Util.Enums;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MusicPlayerModule.ViewModels
{
    internal class MusicPlayerViewModel : BindableBase, IDisposable
    {
        private readonly IMusicPlayerHandler _musicPlayerHandler;
        private IList<string> _songs;
        private string _selectedSong = null;
        private string _currentStatus = "No song selected";
        private string _activeSong = "";

        public IList<string> Songs
        {
            get => _songs;
            private set
            {
                if (!value.SequenceEqual(_musicPlayerHandler.Songs))
                {
                    throw new InvalidOperationException("New value not equal to actual song list");
                }
                SetProperty(ref _songs, value);
            }
        }

        public string SelectedSong
        {
            get => _selectedSong;
            set => SetProperty(ref _selectedSong, value);
        }

        public string CurrentStatus
        {
            get => _currentStatus;
            private set => SetProperty(ref _currentStatus, value);
        }

        public string ActiveSong
        {
            get => _activeSong;
            private set
            {
                if (value != _musicPlayerHandler.ActiveSong)
                {
                    throw new InvalidOperationException("New value not equal to actual active song");
                }
                SetProperty(ref _activeSong, value);
            }
        }

        public DelegateCommand SelectFolderCommand { get; set; }
        public DelegateCommand SelectSongCommand { get; set; }
        public DelegateCommand PlayCommand { get; set; }
        public DelegateCommand PauseCommand { get; set; }
        public DelegateCommand StopCommand { get; set; }

        public MusicPlayerViewModel(IPlayer player, ISongList songList)
        {
            _musicPlayerHandler = new MusicPlayerHandler(player, songList);

            _musicPlayerHandler.ListStatusChanged += ListStatusChangedDetected;
            _musicPlayerHandler.PlayerStatusChanged += PlayerStatusChangedDetected;
            _musicPlayerHandler.ActiveSongChanged += AciveSongChangedDetected;

            Songs = _musicPlayerHandler.Songs;

            SelectFolderCommand = new DelegateCommand(SelectFolderExecute, SelectFolderCanExecute);
            SelectSongCommand = new DelegateCommand(SelectSongExecute, SelectSongCanExecute).ObservesProperty(() => SelectedSong);
            PlayCommand = new DelegateCommand(PlayExecute, PlayPauseStopCanExecute);
            PauseCommand = new DelegateCommand(PauseExecute, PlayPauseStopCanExecute);
            StopCommand = new DelegateCommand(StopExecute, PlayPauseStopCanExecute);
        }

        public void Dispose()
        {
            _musicPlayerHandler.ListStatusChanged -= ListStatusChangedDetected;
            _musicPlayerHandler.PlayerStatusChanged -= PlayerStatusChangedDetected;
            _musicPlayerHandler.ActiveSongChanged -= AciveSongChangedDetected;
            _musicPlayerHandler.Dispose();
        }

        private void SelectFolderExecute()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    LoadSongs(dialog.SelectedPath);
                }
            }
        }

        private bool SelectFolderCanExecute() => _musicPlayerHandler.ListStatus != SongListStatus.Loading;

        private void SelectSongExecute() => SelectActiveSong();

        private bool SelectSongCanExecute() => SelectedSong != null;

        private void PlayExecute() => _musicPlayerHandler.Play();

        private void PauseExecute() => _musicPlayerHandler.Pause();

        private void StopExecute() => _musicPlayerHandler.Stop();

        private bool PlayPauseStopCanExecute() => _musicPlayerHandler.ActiveSong != null;

        private void ListStatusChangedDetected(object sender, EventArgs args)
        {
            SelectFolderCommand.RaiseCanExecuteChanged();
            if (_musicPlayerHandler.ListStatus == SongListStatus.Loaded)
            {
                Songs = _musicPlayerHandler.Songs;
            }
            if (_musicPlayerHandler.ListStatus == SongListStatus.LoadError)
            {
                MessageBox.Show("Failed to load song list");
            }
        }

        private void PlayerStatusChangedDetected(object sender, EventArgs args) => CurrentStatus = CreateStatusMsg();

        private void AciveSongChangedDetected(object sender, EventArgs args)
        {
            PlayCommand.RaiseCanExecuteChanged();
            PauseCommand.RaiseCanExecuteChanged();
            StopCommand.RaiseCanExecuteChanged();
        }

        private void LoadSongs(string path)
        {
            try
            {
                _musicPlayerHandler.LoadSongs(path);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void SelectActiveSong()
        {
            try
            {
                _musicPlayerHandler.ActiveSong = SelectedSong;
                CurrentStatus = CreateStatusMsg();
                ActiveSong = _musicPlayerHandler.ActiveSong;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
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