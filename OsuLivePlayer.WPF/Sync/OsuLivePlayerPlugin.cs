using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using OsuRTDataProvider;
using Sync.Plugins;

namespace OsuLivePlayer.WPF.Sync
{
    [SyncRequirePlugin(typeof(OsuRTDataProviderPlugin))]
    public class OsuLivePlayerPlugin : Plugin
    {
        public static readonly GeneralConfig GeneralConfig = new GeneralConfig();
        public const string PluginName = "OsuLivePlayerPlugin.WPF";

        public OsuLivePlayerPlugin() : base(PluginName, "yf_bmp")
        {
            if (Application.Current == null)
            {
                var thread = new Thread(() => new Application().Run())
                {
                    Name = "STA WPF Application Thread"
                };
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }

            EventBus.BindEvent<PluginEvents.LoadCompleteEvent>(OnLoadComplete);
        }


        public override void OnEnable()
        {
            EventBus.BindEvent<PluginEvents.InitCommandEvent>(OnInitCommand);

            if (MainWindow.Current != null)
            {
                MainWindow.Current.Config = GeneralConfig;
            }
        }

        private static void StartMainWindow()
        {
            Application.Current.Dispatcher?.Invoke(() =>
            {
                var mainWindow = new MainWindow();
                mainWindow.Show();
            });
        }

        public override void OnDisable()
        {
            MainWindow.Current?.Close();
        }

        private void OnLoadComplete(PluginEvents.LoadCompleteEvent e)
        {
            if (!(e.Host.EnumPluings().FirstOrDefault(plugin => plugin is OsuRTDataProviderPlugin) is
                OsuRTDataProviderPlugin ortdpPlugin))
            {
                Logger.LogError("Ortdp was not found.");
                return;
            }

            Logger.LogSuccess(PluginName + " is loaded.");
            MainWindow.OrtdpPlugin = ortdpPlugin;
            if (!Directory.Exists(GeneralConfig.WorkPath))
                Directory.CreateDirectory(GeneralConfig.WorkPath);

            StartMainWindow();
        }

        private void OnInitCommand(PluginEvents.InitCommandEvent t)
        {
            t.Commands.Dispatch.bind("liveplayer", args =>
            {
                StartMainWindow();
                return true;
            }, "Show live player window.");
        }
    }
}
