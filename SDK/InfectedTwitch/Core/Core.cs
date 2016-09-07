#region

using LeagueSharp;
using LeagueSharp.SDK;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Infected_Twitch.Core
{
    internal class Core
    {
        public static bool HasPassive => Player.HasBuff("TwitchHideInShadows");

        public static Orbwalker Orbwalker => Variables.Orbwalker;
        public static AIHeroClient Player => ObjectManager.Player;
        public static AIHeroClient Target => Variables.TargetSelector.GetTarget(1200, DamageType.Physical);

        public static bool SafeTarget(Obj_AI_Base target)
        {
            return target != null
                && target.IsValidTarget()
                && !target.IsDead
                && !target.HasBuffOfType(BuffType.Invulnerability)
                && !target.HasBuffOfType(BuffType.SpellShield);
        }
    }
}
