using System.Windows;

namespace WpfDicomViewerDemo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
            : base()
        {
            WpfDemosCommonCode.DemosTools.EnableLicenseExceptionDisplaying();
        }
    }
}
