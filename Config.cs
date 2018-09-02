using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sync.Tools;
using Sync.Tools.ConfigurationAttribute;

namespace OsuLivePlayer
{
    public class Config : IConfigurable
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
