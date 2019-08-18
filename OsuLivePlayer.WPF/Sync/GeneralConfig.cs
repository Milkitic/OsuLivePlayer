using System;
using System.IO;
using Sync.Tools;
using Sync.Tools.ConfigurationAttribute;

namespace OsuLivePlayer.WPF.Sync
{
    public class GeneralConfig : IConfigurable
    {
        [Path(IsDirectory = true, RequireRestart = true)]
        public ConfigurationElement WorkPath { get; set; } = Path.Combine(
            new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName,
            typeof(OsuLivePlayerPlugin).Name);

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
