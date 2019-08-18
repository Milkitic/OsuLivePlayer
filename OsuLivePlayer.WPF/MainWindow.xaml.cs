using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OSharp.Animation.WPF;
using OsuLivePlayer.WPF.Effects;
using OsuLivePlayer.WPF.Sync;
using OsuRTDataProvider;

namespace OsuLivePlayer.WPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private OsuRTDataProviderPlugin _ortdpPlugin;

        public static OsuRTDataProviderPlugin OrtdpPlugin { get; internal set; }

        public static MainWindow Current { get; private set; }
        public GeneralConfig Config { get; set; }

        public List<LayerBase> LayerList { get; set; } = new List<LayerBase>();

        public MainWindow()
        {
            InitializeComponent();
            Current = this;
        }

        public void StartListen()
        {
            OrtdpPlugin.ListenerManager.OnBeatmapChanged += ListenerManager_OnBeatmapChanged;
            foreach (var layerBase in LayerList)
            {
                layerBase.StartListen();
            }
        }

        public void StopListen()
        {
            OrtdpPlugin.ListenerManager.OnBeatmapChanged -= ListenerManager_OnBeatmapChanged;
            foreach (var layerBase in LayerList)
            {
                layerBase.StopListen();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (OrtdpPlugin == null)
            {
                OrtdpPlugin = new OsuRTDataProviderPlugin();
            }

            var o = new StoryboardGroup(this.MainCanvas);
            var s = new StoryboardCanvasHost(this.BgCanvas);
            LayerList.AddRange(new LayerBase[]
            {
                //new BackgroundLayer(o) ,
                new BackgroundLayer2(s)
            });
            StartListen();
        }

        private void ListenerManager_OnBeatmapChanged(OsuRTDataProvider.BeatmapInfo.Beatmap map)
        {
            Application.Current.Dispatcher?.BeginInvoke(new Action(() =>
            {
                LblSongInfo.Content = map.TitleUnicode;
            }));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            StopListen();
        }
    }
}
