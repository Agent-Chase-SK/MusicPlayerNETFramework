using MusicPlayerAPI;
using MusicPlayerAPI.Players;
using MusicPlayerAPI.SongList;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Windows.Forms;

namespace MusicPlayerModule.ViewModels
{
    class MusicPlayerViewModel : BindableBase, IDisposable
    {
        private readonly MusicPlayerHandler _musicPlayerHandler;

        public DelegateCommand SelectFolderCommand { get; set; }

        public MusicPlayerViewModel(IPlayer player, ISongList songList)
        {
            _musicPlayerHandler = new MusicPlayerHandler(player, songList);

            SelectFolderCommand = new DelegateCommand(SelectFolderExecute, SelectFolderCanExecute).ObservesProperty(() => Tmp);
        }

        public void Dispose()
        {
            _musicPlayerHandler.Dispose();
        }

        private void SelectFolderExecute()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    Tmp = dialog.SelectedPath; //TMP
                }
            }
        }

        private bool SelectFolderCanExecute()
        {
            return _musicPlayerHandler.ListStatus != SongListStatus.Loading;
        }

        //TMP
        private string tmp = "";
        public string Tmp
        {
            get => tmp;
            set => SetProperty(ref tmp, value);
        }
    }
}
