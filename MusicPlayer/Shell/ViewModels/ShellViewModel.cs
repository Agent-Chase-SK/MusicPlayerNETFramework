using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shell.ViewModels
{
    class ShellViewModel : BindableBase
    {
        public ShellViewModel(IRegionViewRegistry regionViewRegistry)
        {
            regionViewRegistry.RegisterViewWithRegion(Regions.MainRegion, Regions.MusicPlayerViewType);
        }
    }
}
