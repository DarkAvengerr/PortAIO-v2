
using LeagueSharp.Common.Data;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Nechrito_Twitch
{
    internal class Usables
    {
        public static void CastYoumoo()
        {
            if (LeagueSharp.Common.Data.ItemData.Youmuus_Ghostblade.GetItem().IsReady()) LeagueSharp.Common.Data.ItemData.Youmuus_Ghostblade.GetItem().Cast();
        }

        public static void Botrk()
        {
            if (LeagueSharp.Common.Data.ItemData.Blade_of_the_Ruined_King.GetItem().IsReady()) LeagueSharp.Common.Data.ItemData.Blade_of_the_Ruined_King.GetItem().Cast();
        }
    }
}
