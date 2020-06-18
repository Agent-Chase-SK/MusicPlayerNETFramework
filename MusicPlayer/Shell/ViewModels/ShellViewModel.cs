using Prism.Mvvm;
using Prism.Regions;

namespace Shell.ViewModels
{
    internal class ShellViewModel : BindableBase
    {
        public ShellViewModel(IRegionViewRegistry regionViewRegistry)
        {
            regionViewRegistry.RegisterViewWithRegion(Regions.MainRegion, Regions.MusicPlayerViewType);
        }
    }
}