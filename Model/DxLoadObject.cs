using OsuLivePlayer.Config;
using OsuLivePlayer.Controller;
using OsuLivePlayer.Util;
using System.Drawing;

namespace OsuLivePlayer.Model
{
    public class DxLoadObject
    {
        public RenderSettings Render { get; private set; }
        public PreferenceSettings Preference { get; private set; }

        public static DxLoadObject Default => new DxLoadObject
        {
            Render = new RenderSettings
            {
                UseVsync = true,
                WindowSize = new Size(640, 480)
            },
            Preference = new PreferenceSettings
            {
                PreferUnicode = true
            }
        };

        public void ReloadFromConfig(DisplayConfig config)
        {
            if (Preference.PreferUnicode != config.PreferMetadataInOriginalLanguage.ToBool())
            {
                Preference.PreferUnicode = config.PreferMetadataInOriginalLanguage.ToBool();
                LogUtil.LogSuccess("Metadata In Original Language: " + (Preference.PreferUnicode ? "On" : "Off"));
            }

            if (Render.LimitFps != config.LimitFps.ToInt())
            {
                Render.LimitFps = config.LimitFps.ToInt();
                LogUtil.LogSuccess("Limit Fps: " + Render.LimitFps);
            }

            if (Render.WindowSize.Width != config.WindowWidth.ToInt() || Render.WindowSize.Height != config.WindowHeight.ToInt())
            {
                Render.WindowSize.Width = config.WindowWidth.ToInt();
                Render.WindowSize.Height = config.WindowHeight.ToInt();
                FormController.CloseDirectXForm();
                LogUtil.LogSuccess("Widow Size: " + Render.WindowSize);
            }

            if (Render.UseVsync != config.Vsync.ToBool())
            {
                Render.UseVsync = config.Vsync.ToBool();
                FormController.CloseDirectXForm();
                LogUtil.LogSuccess("Vsync: " + (Render.UseVsync ? "On" : "Off"));
            }

            FormController.CreateDirectXForm(this, OrtdpController.OsuModel);
        }
    }

    public class PreferenceSettings
    {
        public bool PreferUnicode;
    }

    public class RenderSettings
    {
        public Size WindowSize;
        public bool UseVsync;
        public int? LimitFps;
    }
}
