using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sync.Tools;
using Sync.Tools.ConfigurationAttribute;

namespace OsuLivePlayer.Config
{
    public class DisplayConfig : IConfigurable
    {
        [Bool]
        public ConfigurationElement PreferMetadataInOriginalLanguage { get; set; } = "True";
        [Bool]
        public ConfigurationElement LimitFps { get; set; } = "True";
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
            OsuLivePlayerPlugin.Object.ReloadFromConfig(this);
        }
    }
}
