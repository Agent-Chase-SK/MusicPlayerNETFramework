namespace MusicPlayerAPI.Util.ExtensionCheckers
{
    public interface IExtensionChecker
    {
        bool IsSupportedExtension(string path);

        string[] GetSuportedExtensions();
    }
}