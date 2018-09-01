using System;
using OsuRTDataProvider.Listen;
using OsuRTDataProvider.Mods;
using OrtdpBeatmap = OsuRTDataProvider.BeatmapInfo.Beatmap;

namespace OsuLivePlayer.Model
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
            //NowMap = new Beatmap
            //{
            //    Artist = map.Artist,
            //    ArtistUnicode = map.ArtistUnicode,
            //    AudioFilename = map.AudioFilename,
            //    BackgroundFilename = map.BackgroundFilename,
            //    BeatmapId = map.BeatmapID,
            //    BeatmapSetId = map.BeatmapSetID,
            //    Creator = map.Creator,
            //    Difficulty = map.Difficulty,
            //    DownloadLink = map.DownloadLink,
            //    DownloadLinkSet = map.DownloadLinkSet,
            //    Filename = map.Filename,
            //    FilenameFull = map.FilenameFull,
            //    Folder = map.Folder,
            //    OsuClientId = map.OsuClientID,
            //    Title = map.Title,
            //    TitleUnicode = map.TitleUnicode,
            //    Version = map.Version,
            //    VideoFilename = map.VideoFilename,
            //};
        }

        public void OnPlayModeChanged(OsuPlayMode lastMode, OsuPlayMode mode)
        {
        }
    }
}
