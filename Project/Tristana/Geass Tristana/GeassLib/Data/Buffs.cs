using LeagueSharp;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace GeassLib.Data
{
    public class Buffs
    {

        private static readonly BuffType[] Bufftype =
        {
            BuffType.Snare,
            BuffType.Blind,
            BuffType.Charm,
            BuffType.Stun,
            BuffType.Fear,
            BuffType.Slow,
            BuffType.Taunt,
            BuffType.Suppression
        };

        public static BuffType[] GetTypes => Bufftype;
    }
}
