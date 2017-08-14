using FileExchangerClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;

namespace FileExchangerClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            var clientSettings = ConfigurationManager.GetSection("clientSettings") as ConfigurationSections.ClientSettingsSection;
            var downloadsPath = clientSettings != null ? clientSettings.DownloadsPath : Path.GetTempPath();
            var chunkSize = clientSettings != null ? clientSettings.ChunkSize : 200;

            var mainViewModel = new MainWindowViewModel(new Settings.ClientSettings { DownloadsPath = downloadsPath, ChunkSize = chunkSize });
            var mainView = new MainWindow(mainViewModel);

            Current.MainWindow = mainView;
            Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

            //AppDomain.CurrentDomain.UnhandledException += AppDomainUnhandledException;

            mainView.Show();
        }
        

    }
}
