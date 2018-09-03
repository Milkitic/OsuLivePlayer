using System.IO;
using System.Linq;
using OsuLivePlayer.Config;
using OsuLivePlayer.Controller;
using OsuLivePlayer.Model;
using OsuLivePlayer.Util;
using OsuLivePlayer.Util.DxUtil;
using OsuRTDataProvider;
using Sync.Plugins;
using Sync.Tools;

namespace OsuLivePlayer
{
    [SyncRequirePlugin(typeof(OsuRTDataProviderPlugin))]
    public class OsuLivePlayerPlugin : Plugin
    {
        private OrtdpController _ortdpController;
        public static readonly GeneralConfig GeneralConfig = new GeneralConfig();
        public static readonly DisplayConfig DisplayConfig = new DisplayConfig();

        public static DxLoadObject Object;
        private bool _initSuccessfully = true;

        public OsuLivePlayerPlugin() : base(typeof(OsuLivePlayerPlugin).Name, "yf_extension")
        {
            var configManager = new PluginConfigurationManager(this);
            configManager.AddItem(GeneralConfig);
            configManager.AddItem(DisplayConfig);

            EventBus.BindEvent<PluginEvents.LoadCompleteEvent>(OnLoadComplete);
            EventBus.BindEvent<PluginEvents.ProgramReadyEvent>(OnProgramReady);
        }

        private void OnProgramReady(PluginEvents.ProgramReadyEvent e)
        {
            if (!_initSuccessfully) return;
            Object = DxLoadObject.Default;
            Object.ReloadFromConfig(DisplayConfig);
            //FormController.CreateDirectXForm(Object, OrtdpController.OsuModel);
        }

        public override void OnEnable()
        {
            base.OnEnable();
            LogUtil.LogInfo("Plugin has been enabled.");
            // todo
        }

        public override void OnDisable()
        {
            base.OnDisable();
            LogUtil.LogInfo("Plugin has been disabled.");
            // todo
        }

        private void OnLoadComplete(PluginEvents.LoadCompleteEvent e)
        {
            if (!(e.Host.EnumPluings().FirstOrDefault(plugin => plugin is OsuRTDataProviderPlugin) is
                OsuRTDataProviderPlugin ortdpPlugin))
            {
                LogUtil.LogError("Ortdp was not found.");
                _initSuccessfully = false;
                return;
            }

            if (!Directory.Exists(GeneralConfig.WorkPath))
                Directory.CreateDirectory(GeneralConfig.WorkPath);

            LogUtil.LogInfo("Ortdp has been loaded.");
            _ortdpController = new OrtdpController(ortdpPlugin);
            _ortdpController.StartReceive();
        }
    }
}
