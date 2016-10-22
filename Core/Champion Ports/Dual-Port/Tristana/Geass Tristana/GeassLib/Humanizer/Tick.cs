using EloBuddy; 
 using LeagueSharp.Common; 
 namespace GeassLib.Humanizer
{
    public class Tick
    {
        private float _maxDelay;
        private float _minDelay;
        private float _nextTick;

        public Tick(float currentTime, float min = 0, float max = 100)
        {
            _nextTick = currentTime;
            _minDelay = min;
            _maxDelay = max;
        }

        public float GetMaxDelay()
        {
            return _maxDelay;
        }

        public float GetMinDelay()
        {
            return _minDelay;
        }

        public bool IsReady(float currentTime)
        {
            return currentTime > _nextTick;
        }

        public void SetMinAndMax(float min, float max)
        {
            _minDelay = min;
            _maxDelay = max;
        }

        public void UseTick(float next, float currentTime)
        {
            _nextTick = currentTime + next;
        }
    }
}