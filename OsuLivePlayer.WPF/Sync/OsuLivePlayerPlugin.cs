using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsuRTDataProvider;
using Sync.Plugins;

namespace OsuLivePlayer.WPF.Sync
{
    [SyncRequirePlugin(typeof(OsuRTDataProviderPlugin))]
    class OsuLivePlayerPlugin : Plugin
    {
        public OsuLivePlayerPlugin() : base("OsuLivePlayerPlugin.WPF", "yf_bmp")
        {
        }
    }
}
