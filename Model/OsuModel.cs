using OsuRTDataProvider.Listen;

namespace OsuLivePlayer.Model
{
    public class OsuModel
    {
        public GamePlay GamePlay { get; } = new GamePlay();
        public Idle Idle { get; } = new Idle();
        public OsuListenerManager.OsuStatus LastStatus { get; private set; }
        public OsuListenerManager.OsuStatus Status { get; private set; }

        public void OnStatusChanged(OsuListenerManager.OsuStatus lastStatus, OsuListenerManager.OsuStatus status)
        {
            LastStatus = lastStatus;
            Status = status;
        }
    }
}
