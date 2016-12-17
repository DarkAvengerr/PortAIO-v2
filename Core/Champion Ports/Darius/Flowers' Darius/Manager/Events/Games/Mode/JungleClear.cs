using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Darius.Manager.Events.Games.Mode
{
    using System.Linq;
    using FlowersDariusCommon;
    using LeagueSharp.Common;

    internal class JungleClear : Logic
    {
        internal static void Init()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("JungleClearMana")) && ManaManager.SpellFarm)
            {
                var mobs = MinionManager.GetMinions(Me.Position, Q.Range, MinionTypes.All, MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth);

                if (mobs.Count(x => x.DistanceToPlayer() <= Q.Range) > 0)
                {
                    Q.Cast(true);
                }
            }
        }
    }
}
