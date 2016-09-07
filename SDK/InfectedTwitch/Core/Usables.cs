#region

using LeagueSharp.SDK;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Infected_Twitch.Core
{
    internal class Usables : Core
    {
        public static void CastYomu()
        {
            if (!Items.CanUseItem(3142)) return;

                Items.UseItem(3142);
        }

        public static void Botrk()
        {
            // BOTRK
            if (Items.CanUseItem(3153) && GameObjects.Player.HealthPercent <= 90)
            {
                Items.UseItem(3153, Target);
            }
            // Bilgewater Cutlass
            if (Items.CanUseItem(3144))
            {
                Items.UseItem(3144, Target);
            }
        }
    }
}
