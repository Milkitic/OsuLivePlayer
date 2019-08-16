using OsuLivePlayer.Model;
using OsuLivePlayer.Model.OsuStatus;
using OsuLivePlayer.Render;
using OsuLivePlayer.Util;
using OsuLivePlayer.Util.DxUtil;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OsuLivePlayer.Controller
{
    internal static class FormController
    {
        private static Form _dxForm;


        static FormController()
        {
            Application.EnableVisualStyles();
        }

        public static void CreateDirectXForm(DxLoadObject obj, OsuModel osuModel)
        {
            try
            {
                if (_dxForm != null && !_dxForm.IsDisposed) return;
                _dxForm = new DxRenderForm(obj, osuModel);
                Task.Run(() => { Application.Run(_dxForm); });
            }
            catch (Exception e)
            {
                LogUtil.LogError(e.ToString());
            }
        }

        public static void CloseDirectXForm()
        {
            try
            {
                if (_dxForm != null && !_dxForm.IsDisposed)
                {
                    _dxForm.Close();
                    _dxForm.Dispose();
                }
            }
            catch (Exception e)
            {
                LogUtil.LogError(e.ToString());
            }
        }

        public static void ReCreateDirectXForm(DxLoadObject obj, OsuModel osuModel)
        {
            CloseDirectXForm();
            CreateDirectXForm(obj, osuModel);
        }
    }
}
