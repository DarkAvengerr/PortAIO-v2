using LeagueSharp;
using LeagueSharp.SDK;


using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace PrideStalker_Rengar.Handlers
{
    internal class Item : Core
    {
        public static void CastProtobelt()
        {
            var target = Variables.TargetSelector.GetTarget(1000, DamageType.Physical);

            if (Items.CanUseItem(3152) && target.IsValidTarget())
            {
                Items.UseItem(3152, Player.ServerPosition.Extend(target.ServerPosition, Player.AttackRange));
            }
        }

        public static void CastHydra()
        {
            if (Items.CanUseItem(3074))
            {
                Items.UseItem(3074);
            }

            if (Items.CanUseItem(3077))
            {
                Items.UseItem(3077);
            }
        }

        public static void CastYomu()
        {
            if (Items.CanUseItem(3142))
            {
                Items.UseItem(3142);
            }
        }
    }
}
