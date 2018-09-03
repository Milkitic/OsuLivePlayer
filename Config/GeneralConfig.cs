using System.IO;
using OsuLivePlayer.Util;
using Sync.Tools;
using Sync.Tools.ConfigurationAttribute;

namespace OsuLivePlayer.Config
{
    public class GeneralConfig : IConfigurable
    {
        [Path(IsDirectory = true, RequireRestart = true)]
        public ConfigurationElement WorkPath { get; set; } =
            Path.Combine(new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.FullName, typeof(OsuLivePlayerPlugin).Name);
        
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
