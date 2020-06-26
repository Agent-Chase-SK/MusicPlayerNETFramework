using MusicPlayerAPI.Util;
using MusicPlayerAPI.Util.Enums;
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
        private IDictionary<string, string> _songs = new Dictionary<string, string>();
        private readonly IExtensionChecker _extensionChecker = new WavMp3();

        public SongListStatus Status
        {
            get => _status;
            protected set
            {
                _status = value;
                OnStatusChanged();
            }
        }

        public IDictionary<string, string> Songs
        {
            get
            {
                if (Status == SongListStatus.Loading)
                {
                    throw new InvalidOperationException("List is loading");
                }
                return _songs.ToImmutableDictionary();
            }
            protected set => _songs = value;
        }

        public string[] SupportedExtensions
        {
            get => _extensionChecker.GetSuportedExtensions();
        }

        public event EventHandler StatusChanged;

        public SimpleRecursiveSongList() => Status = SongListStatus.NoSelectedFolder;

        public void LoadSongs(string path)
        {
            CheckBeforeLoading(path);
            Status = SongListStatus.Loading;
            if (_songs.Count != 0)
            {
                _songs = new Dictionary<string, string>();
            }
            StartRecSearch(path);
        }

        protected virtual void StartRecSearch(string path)
        {
            try
            {
                RecDirSearch(path);
            }
            catch (Exception)
            {
                Status = SongListStatus.LoadError;
                Songs = new Dictionary<string, string>();
                return;
            }
            Status = SongListStatus.Loaded;
        }

        protected void RecDirSearch(string dirPath)
        {
            foreach (string file in Directory.GetFiles(dirPath))
            {
                if (_extensionChecker.IsSupportedExtension(file))
                {
                    string title = UtfOnlyChars(Path.GetFileNameWithoutExtension(file));
                    _songs.Add(title, file);
                }
            }
            foreach (string dir in Directory.GetDirectories(dirPath))
            {
                RecDirSearch(dir);
            }
        }

        private void CheckBeforeLoading(string path)
        {
            if (Status == SongListStatus.Loading)
            {
                throw new InvalidOperationException("List is loading");
            }
            if (!PathChecker.IsPathValid(path))
            {
                throw new ArgumentException("Path is invalid");
            }
        }

        private string UtfOnlyChars(string text)
        {
            return Regex.Replace(text, @"[^\u0000-\u007F]+", string.Empty);
        }

        private void OnStatusChanged() => StatusChanged?.Invoke(this, EventArgs.Empty);
    }
}