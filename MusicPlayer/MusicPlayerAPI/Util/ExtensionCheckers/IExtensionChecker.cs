using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayerAPI.Util.ExtensionCheckers
{
    public interface IExtensionChecker
    {
        bool IsSupportedExtension(string path);

        string[] GetSuportedExtensions();
    }
}
