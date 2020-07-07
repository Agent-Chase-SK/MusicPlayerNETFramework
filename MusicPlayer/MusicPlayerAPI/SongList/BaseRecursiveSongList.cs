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
    public abstract class BaseRecursiveSongList : ISongList
    {
        private SongListStatus _status = SongListStatus.NoSelectedFolder;
        protected IDictionary<string, string> _songs;
        private IExtensionChecker _extensionChecker;

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

        public IExtensionChecker ExtensionChecker
        {
            get
            {
                if (_extensionChecker == null)
                {
                    throw new InvalidOperationException("Checker not set");
                }
                return _extensionChecker;
            }
            set => _extensionChecker = value;
        }

        public event EventHandler StatusChanged;

        public abstract void LoadSongs(string path);

        protected void CheckBeforeLoading(string path)
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

        protected void RecDirSearch(string dirPath)
        {
            foreach (string file in Directory.GetFiles(dirPath))
            {
                if (ExtensionChecker.IsSupportedExtension(file))
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

        protected string UtfOnlyChars(string text)
        {
            return Regex.Replace(text, @"[^\u0000-\u007F]+", string.Empty);
        }

        private void OnStatusChanged() => StatusChanged?.Invoke(this, EventArgs.Empty);
    }
}