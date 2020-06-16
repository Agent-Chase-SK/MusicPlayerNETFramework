using System;
using System.IO;
using System.Linq;
using System.Security;

namespace MusicPlayerAPI.Util
{
    public static class PathChecker
    {
        public static bool IsPathValid(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }
            try
            {
                Path.GetFullPath(path);
            }
            catch (Exception ex) when (ex is ArgumentException || ex is SecurityException || ex is NotSupportedException || ex is PathTooLongException)
            {
                return false;
            }
            return true;
        }
    }
}