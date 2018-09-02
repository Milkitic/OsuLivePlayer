using System.IO;
using System.Linq;
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
        public static readonly Config Config = new Config();
        private bool _initSuccessfully = true;

        public OsuLivePlayerPlugin() : base(typeof(OsuLivePlayerPlugin).Name, "yf_extension")
        {
            var configManager = new PluginConfigurationManager(this);
            configManager.AddItem(Config);

            EventBus.BindEvent<PluginEvents.LoadCompleteEvent>(OnLoadComplete);
            EventBus.BindEvent<PluginEvents.ProgramReadyEvent>(OnProgramReady);
        }

        private void OnProgramReady(PluginEvents.ProgramReadyEvent e)
        {
            if (!_initSuccessfully) return;
            FormController.CreateDirectXForm(DxLoadSettings.Default, OrtdpController.OsuModel);
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

            if (!Directory.Exists(Config.WorkPath))
                Directory.CreateDirectory(Config.WorkPath);

            LogUtil.LogInfo("Ortdp has been loaded.");
            _ortdpController = new OrtdpController(ortdpPlugin);
            _ortdpController.StartReceive();
        }
    }
}
