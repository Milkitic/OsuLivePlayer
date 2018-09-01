using System.Drawing;
using OsuLivePlayer.Model;

namespace OsuLivePlayer.Util.DxUtil
{
    public class DxLoadSettings
    {
        public RenderSettings RenderSettings { get; set; }
        public PreferenceSettings PreferenceSettings { get; set; }

        public static DxLoadSettings Default => new DxLoadSettings
        {
            RenderSettings = new RenderSettings
            {
                UseVsync = true,
                WindowSize = new Size(1280, 720)
            },
            PreferenceSettings = new PreferenceSettings
            {
                PreferUnicode = true
            }
        };
    }

    public class PreferenceSettings
    {
        public bool PreferUnicode { get; set; }
    }

    public class RenderSettings
    {
        public Size WindowSize { get; set; }
        public bool UseVsync { get; set; }
    }
}
