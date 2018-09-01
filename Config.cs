using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sync.Tools;
using Sync.Tools.ConfigurationAttribute;

namespace OsuLivePlayer
{
    internal class Config : IConfigurable
    {
        [Integer(MaxValue = 4096, MinValue = 320, RequireRestart = true)]
        public ConfigurationElement Width { get; set; }
        [Integer(MaxValue = 2160, MinValue = 180, RequireRestart = true)]
        public ConfigurationElement Height { get; set; }

        [Bool(RequireRestart = true)]
        public ConfigurationElement UseVsync { get; set; }

        public void onConfigurationLoad()
        {

        }

        public void onConfigurationSave()
        {

        }

        public void onConfigurationReload()
        {

        }
    }
}
