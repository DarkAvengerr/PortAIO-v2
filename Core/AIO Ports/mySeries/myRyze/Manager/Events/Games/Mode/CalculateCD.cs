using EloBuddy; 
using LeagueSharp.Common; 
namespace myRyze.Manager.Events.Games
{
    using LeagueSharp;

    internal class CalculateCD : Logic
    {
        internal static void Init()
        {
            QcdEnd = Q.Instance.CooldownExpires;
            WcdEnd = W.Instance.CooldownExpires;
            EcdEnd = E.Instance.CooldownExpires;

            Qcd = Q.Level > 0 ? CheckCD(QcdEnd) : -1;
            Wcd = W.Level > 0 ? CheckCD(WcdEnd) : -1;
            Ecd = E.Level > 0 ? CheckCD(EcdEnd) : -1;
        }

        private static float CheckCD(float Expires)
        {
            var time = Expires - Game.Time;

            if (time < 0)
            {
                time = 0;

                return time;
            }

            return time;
        }
    }
}