using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Mr.Krabs {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        protected override void OnStartup(StartupEventArgs e) {
            // catch all exceptions if not on debug mode

#if (!DEBUG) 
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
#endif
            base.OnStartup(e);
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
            ExceptionCatched(e.Exception);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
            if (e.ExceptionObject is Exception ex)
                ExceptionCatched(ex);
        }

        private void ExceptionCatched(Exception ex) {
            if (ex != null)
                MessageBox.Show(
                    $"Please screenshot this and paste on issue page. {Environment.NewLine}An unhandled error occured: {Environment.NewLine}{ex}{Environment.NewLine}Press OK to file an issue.",
                    @"Please screenshot this and paste on issue page.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

            const string issueLink = @"https://github.com/AlizerUncaged/CGHM-External-UI/issues/new";
            Process.Start(issueLink);
        }
    }
}
