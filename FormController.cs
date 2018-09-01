using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using OsuLivePlayer.Model;
using OsuLivePlayer.RenderForm;
using OsuLivePlayer.Util.DxUtil;

namespace OsuLivePlayer
{
    internal static class FormController
    {
        public static void CreateDirectXForm(DxLoadSettings settings, OsuModel osuModel)
        {
            try
            {
                Task.Run(() =>
                {
                    Application.EnableVisualStyles();
                    Application.Run(new DxRenderForm(settings, osuModel));
                });

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
