using Sync.Tools;
using Sync.Tools.ConfigurationAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuLivePlayer.Config
{
    public class DisplayConfig : IConfigurable
    {
        [Bool]
        public ConfigurationElement PreferMetadataInOriginalLanguage { get; set; } = "True";
        [Bool]
        public ConfigurationElement Vsync { get; set; } = "True";
        [Integer(MaxValue = 200)]
        public ConfigurationElement LimitFps { get; set; } = "60";
        [Integer(MinValue = 640)]
        public ConfigurationElement WindowWidth { get; set; } = "1280";
        [Integer(MinValue = 480)]
        public ConfigurationElement WindowHeight { get; set; } = "720";

        public void onConfigurationLoad()
        {

        }

        public void onConfigurationSave()
        {

        }

        public void onConfigurationReload()
        {
            OsuLivePlayerPlugin.Settings.ReloadFromConfig(this);
        }
    }
}
