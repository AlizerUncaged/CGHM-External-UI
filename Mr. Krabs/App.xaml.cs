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
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
             base.OnStartup(e);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
            MessageBox.Show(
                $"Please screenshot this and paste on issue page. {Environment.NewLine}An unhandled error occured: {Environment.NewLine}{e.ExceptionObject.ToString()}{Environment.NewLine}Press OK to file an issue.",
                @"Please screenshot this and paste on issue page.",
                MessageBoxButton.OK, 
                MessageBoxImage.Error);

            const string issueLink = @"https://github.com/AlizerUncaged/CGHM-External-UI/issues/new";
            Process.Start(issueLink);
        }
    }
}
