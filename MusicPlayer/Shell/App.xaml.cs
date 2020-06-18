using MusicPlayerAPI.Players;
using MusicPlayerAPI.SongList;
using MusicPlayerModule.Views;
using Prism.Ioc;
using Prism.Unity;
using Shell.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Shell
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<ShellView>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IPlayer, NAudioPlayer>();
            containerRegistry.RegisterSingleton<ISongList, SimpleRecursiveSongList>();

            containerRegistry.RegisterForNavigation<MusicPlayerView>();
        }
    }
}
