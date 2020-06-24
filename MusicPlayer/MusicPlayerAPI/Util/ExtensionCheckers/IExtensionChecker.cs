namespace MusicPlayerAPI.Util.ExtensionCheckers
{
    internal interface IExtensionChecker
    {
        bool IsSupportedExtension(string path);

        string[] GetSuportedExtensions();
    }
}