using System.Windows;
using SupremeScreenSharePro.Modules;

namespace SupremeScreenSharePro
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (!SecurityManager.IsAdministrator())
            {
                MessageBox.Show("This application requires Administrator privileges to run correctly. Please restart as Administrator.", "Admin Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            SecurityManager.SelfDestruct();
        }
    }
}
