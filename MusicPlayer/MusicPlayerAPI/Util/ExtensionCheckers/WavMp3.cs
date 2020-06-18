using System;
using System.IO;
using System.Linq;

namespace MusicPlayerAPI.Util.ExtensionCheckers
{
    internal class WavMp3 : IExtensionChecker
    {
        private static readonly string[] _supportedExtensions = { ".wav", ".mp3" };

        public string[] GetSuportedExtensions()
        {
            return (string[])_supportedExtensions.Clone();
        }

        public bool IsSupportedExtension(string path)
        {
            string extension = Path.GetExtension(path);
            return _supportedExtensions.Contains(extension);
        }
    }
}