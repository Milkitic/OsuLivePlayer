using OsuLivePlayer.Model;
using OsuRTDataProvider;

namespace OsuLivePlayer
{
    internal class OrtdpController
    {
        private readonly OsuRTDataProviderPlugin _ortdpPlugin;
        public static OsuModel OsuModel { get; private set; }

        public OrtdpController(OsuRTDataProviderPlugin ortdpPlugin)
        {
            _ortdpPlugin = ortdpPlugin;
        }

        public void StartReceive()
        {
            OsuModel = new OsuModel();
            Status_Start();
            Idle_Start();
            GamePlay_Start();
        }

        public void StopReceive()
        {
            Idle_Stop();
            GamePlay_Stop();
            Status_Stop();
            OsuModel = null;
        }

        private void Status_Start()
        {
            _ortdpPlugin.ListenerManager.OnStatusChanged += OsuModel.OnStatusChanged;
        }

        private void GamePlay_Start()
        {
            _ortdpPlugin.ListenerManager.OnAccuracyChanged += OsuModel.GamePlay.OnAccuracyChanged;
            _ortdpPlugin.ListenerManager.OnComboChanged += OsuModel.GamePlay.OnComboChanged;
            _ortdpPlugin.ListenerManager.OnCount50Changed += OsuModel.GamePlay.OnCount50Changed;
            _ortdpPlugin.ListenerManager.OnCount100Changed += OsuModel.GamePlay.OnCount100Changed;
            _ortdpPlugin.ListenerManager.OnCount300Changed += OsuModel.GamePlay.OnCount300Changed;
            _ortdpPlugin.ListenerManager.OnCountGekiChanged += OsuModel.GamePlay.OnCountGekiChanged;
            _ortdpPlugin.ListenerManager.OnCountKatuChanged += OsuModel.GamePlay.OnCountKatuChanged;
            _ortdpPlugin.ListenerManager.OnCountMissChanged += OsuModel.GamePlay.OnCountMissChanged;
            _ortdpPlugin.ListenerManager.OnHealthPointChanged += OsuModel.GamePlay.OnHealthPointChanged;
            _ortdpPlugin.ListenerManager.OnPlayingTimeChanged += OsuModel.GamePlay.OnPlayingTimeChanged;
            _ortdpPlugin.ListenerManager.OnScoreChanged += OsuModel.GamePlay.OnScoreChanged;
        }

        private void Idle_Start()
        {
            _ortdpPlugin.ListenerManager.OnBeatmapChanged += OsuModel.Idle.OnBeatmapChanged;
            _ortdpPlugin.ListenerManager.OnModsChanged += OsuModel.Idle.OnModsChanged;
            _ortdpPlugin.ListenerManager.OnPlayModeChanged += OsuModel.Idle.OnPlayModeChanged;
        }

        private void Status_Stop()
        {
            _ortdpPlugin.ListenerManager.OnStatusChanged -= OsuModel.OnStatusChanged;
        }

        private void GamePlay_Stop()
        {
            _ortdpPlugin.ListenerManager.OnAccuracyChanged -= OsuModel.GamePlay.OnAccuracyChanged;
            _ortdpPlugin.ListenerManager.OnComboChanged -= OsuModel.GamePlay.OnComboChanged;
            _ortdpPlugin.ListenerManager.OnCount50Changed -= OsuModel.GamePlay.OnCount50Changed;
            _ortdpPlugin.ListenerManager.OnCount100Changed -= OsuModel.GamePlay.OnCount100Changed;
            _ortdpPlugin.ListenerManager.OnCount300Changed -= OsuModel.GamePlay.OnCount300Changed;
            _ortdpPlugin.ListenerManager.OnCountGekiChanged -= OsuModel.GamePlay.OnCountGekiChanged;
            _ortdpPlugin.ListenerManager.OnCountKatuChanged -= OsuModel.GamePlay.OnCountKatuChanged;
            _ortdpPlugin.ListenerManager.OnCountMissChanged -= OsuModel.GamePlay.OnCountMissChanged;
            _ortdpPlugin.ListenerManager.OnHealthPointChanged -= OsuModel.GamePlay.OnHealthPointChanged;
            _ortdpPlugin.ListenerManager.OnPlayingTimeChanged -= OsuModel.GamePlay.OnPlayingTimeChanged;
            _ortdpPlugin.ListenerManager.OnScoreChanged -= OsuModel.GamePlay.OnScoreChanged;
        }

        private void Idle_Stop()
        {
            _ortdpPlugin.ListenerManager.OnBeatmapChanged -= OsuModel.Idle.OnBeatmapChanged;
            _ortdpPlugin.ListenerManager.OnModsChanged -= OsuModel.Idle.OnModsChanged;
            _ortdpPlugin.ListenerManager.OnPlayModeChanged -= OsuModel.Idle.OnPlayModeChanged;
        }
    }
}
