using MusicPlayerAPI.Util;
using MusicPlayerAPI.Util.ExtensionCheckers;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text.RegularExpressions;

namespace MusicPlayerAPI.SongList
{
    public class SimpleRecursiveSongList : ISongList
    {
        private SongListStatus _status;
        private IDictionary<string, string> _songList;
        private readonly IExtensionChecker _extensionChecker = new WavMp3();

        public SongListStatus Status
        {
            get => _status;
            private set
            {
                _status = value;
                OnStatusChanged();
            }
        }

        public IDictionary<string, string> Songs
        {
            get
            {
                if (_songList == null)
                {
                    throw new InvalidOperationException("No songs Loaded");
                }
                return _songList.ToImmutableDictionary();
            }
        }

        public string[] SupportedExtensions
        {
            get => _extensionChecker.GetSuportedExtensions();
        }

        public event EventHandler StatusChanged;

        public SimpleRecursiveSongList() => Status = SongListStatus.NoSelectedFolder;

        public bool LoadSongs(string path)
        {
            if (!PathChecker.IsPathValid(path))
            {
                return false;
            }
            Status = SongListStatus.Loading;
            if (_songList == null || _songList.Count != 0)
            {
                _songList = new Dictionary<string, string>();
            }
            try
            {
                RecDirSearch(path);
            }
            catch (Exception)
            {
                _songList = null;
                Status = SongListStatus.NoSelectedFolder;
                return false;
            }
            Status = SongListStatus.Loaded;
            return true;
        }

        private void RecDirSearch(string dirPath)
        {
            foreach (string file in Directory.GetFiles(dirPath))
            {
                if (_extensionChecker.IsSupportedExtension(file))
                {
                    string title = Regex.Replace(Path.GetFileNameWithoutExtension(file), @"[^\u0000-\u007F]+", string.Empty);
                    _songList.Add(title, file);
                }
            }
            foreach (string dir in Directory.GetDirectories(dirPath))
            {
                RecDirSearch(dir);
            }
        }

        private void OnStatusChanged() => StatusChanged?.Invoke(this, new ListStatusChangedEventArgs(Status));
    }
}