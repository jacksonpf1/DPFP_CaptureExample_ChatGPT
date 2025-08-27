using System.Configuration;
using System.Data;
using System.Windows;
using DPFP_CaptureExample_ChatGPT.Services;
using DPFP_CaptureExample_ChatGPT.ViewModels;


namespace DPFP_CaptureExample_ChatGPT
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<FingerprintService>();
            containerRegistry.RegisterForNavigation<MainWindow, MainWindowViewModel>();
        }

        protected override Window CreateShell()
        {
            var w = Container.Resolve<MainWindow>();
            return w;
        }
    }

}
