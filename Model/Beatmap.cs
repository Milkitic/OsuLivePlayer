namespace OsuLivePlayer.Model
{
    public struct Beatmap
    {
        public string Artist { get; set; }
        public string ArtistUnicode { get; set; }
        public string AudioFilename { get; set; }
        public string BackgroundFilename { get; set; }
        public int BeatmapId { get; set; }
        public int BeatmapSetId { get; set; }
        public string Creator { get; set; }
        public string Difficulty { get; set; }
        public string DownloadLink { get; set; }
        public string DownloadLinkSet { get; set; }
        public string Filename { get; set; }
        public string FilenameFull { get; set; }
        public string Folder { get; set; }
        public int OsuClientId { get; set; }
        public string Title { get; set; }
        public string TitleUnicode { get; set; }
        public string Version { get; set; }
        public string VideoFilename { get; set; }
    }
}
