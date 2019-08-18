using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using OsuLivePlayer.WPF.Sync;
using OsuRTDataProvider;

namespace OsuLivePlayer.WPF
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {

        public void InitializeComponent()
        {
            base.StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
        }

        [STAThread]
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            var app = new App();
            app.InitializeComponent();
            app.Run();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            
        }
    }
}
