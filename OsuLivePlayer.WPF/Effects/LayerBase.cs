using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using OSharp.Animation.WPF;

namespace OsuLivePlayer.WPF.Effects
{
    public abstract class LayerBase
    {
        protected readonly StoryboardCanvasHost Group;

        public LayerBase(StoryboardCanvasHost group)
        {
            Group = group;
        }

        public abstract void StartListen();

        public abstract void StopListen();
    }
}
