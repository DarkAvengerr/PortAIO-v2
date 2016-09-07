
#region

using LeagueSharp.SDK;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Reforged_Riven.Extras
{
    internal class Usables
    {
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

        public static void CastYoumoo()
        {
            if (!Items.CanUseItem(3142)) return;

            Items.UseItem(3142);
        }
    }
}
