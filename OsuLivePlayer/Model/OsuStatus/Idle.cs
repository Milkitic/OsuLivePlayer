using OsuRTDataProvider.Listen;
using OsuRTDataProvider.Mods;
using OrtdpBeatmap = OsuRTDataProvider.BeatmapInfo.Beatmap;

namespace OsuLivePlayer.Model.OsuStatus
{
    public class Idle
    {
        public ModsInfo OldMods;
        public ModsInfo NowMods;
        public OrtdpBeatmap NowMap;

        public void OnModsChanged(ModsInfo mods)
        {
            OldMods = NowMods;
            NowMods = mods;
        }

        public void OnBeatmapChanged(OrtdpBeatmap map)
        {
            NowMap = map;
        }

        public void OnPlayModeChanged(OsuPlayMode lastMode, OsuPlayMode mode)
        {
            // todo
        }
    }
}
