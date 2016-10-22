using System.Drawing.Text;
using Jhin___The_Virtuoso.Extensions;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Jhin___The_Virtuoso.Modes
{
    static class Jungle
    {
        /// <summary>
        /// Execute Jungle
        /// </summary>
        public static void ExecuteJungle()
        {
            if (ObjectManager.Player.ManaPercent < Menus.Config.Item("jungle.mana").GetValue<Slider>().Value)
            {
                return;
            }

            var mobs = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Spells.Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            
            if (mobs == null || (mobs.Count == 0))
            {
                return;
            }

            if (Spells.Q.IsReady() && Menus.Config.Item("q.clear").GetValue<bool>())
            {
                Spells.Q.Cast(mobs[0]);
            }

            if (Spells.W.IsReady() && Menus.Config.Item("w.clear").GetValue<bool>())
            {
                Spells.W.Cast(mobs[0]);
            }
        }
    }
}
