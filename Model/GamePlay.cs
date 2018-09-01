namespace OsuLivePlayer.Model
{
    public class GamePlay
    {
        public double Acc { get; private set; }
        public int Combo { get; private set; }
        public int Score { get; private set; }
        public int Hit50 { get; private set; }
        public int Hit100 { get; private set; }
        public int HitKatu { get; private set; } //喝
        public int Hit300 { get; private set; }
        public int HitGeki { get; private set; } //激
        public int MissCount { get; private set; }
        public double Health { get; private set; }
        public int CurrentOffset { get; private set; }

        public void OnAccuracyChanged(double acc) => Acc = acc;
        public void OnComboChanged(int combo) => Combo = combo;
        public void OnScoreChanged(int score) => Score = score;
        public void OnCount50Changed(int hit) => Hit50 = hit;
        public void OnCount100Changed(int hit) => Hit100 = hit;
        public void OnCountKatuChanged(int hit) => HitKatu = hit; //喝
        public void OnCount300Changed(int hit) => Hit300 = hit;
        public void OnCountGekiChanged(int hit) => HitGeki = hit; //激
        public void OnCountMissChanged(int hit) => MissCount = hit;
        public void OnHealthPointChanged(double hp) => Health = hp;
        public void OnPlayingTimeChanged(int ms) => CurrentOffset = ms;
    }
}
