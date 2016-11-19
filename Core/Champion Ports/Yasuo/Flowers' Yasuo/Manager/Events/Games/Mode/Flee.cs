using EloBuddy; 
using LeagueSharp.Common; 
 namespace Flowers_Yasuo.Manager.Events.Games.Mode
{
    using System.Linq;
    using Spells;
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Flee : Logic
    {
        internal static void Init()
        {
            if (IsDashing)
            {
                if (Menu.Item("FleeQ", true).GetValue<bool>() && Q.IsReady() && !SpellManager.HaveQ3)
                {
                    var qMinion = MinionManager.GetMinions(lastEPos, 220, MinionTypes.All, MinionTeam.NotAlly).FirstOrDefault();

                    if (qMinion.IsValidTarget(220))
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(10, () => Q.Cast(Me.Position, true));
                    }
                }
            }
            else
            {
                if (Menu.Item("FleeE", true).GetValue<bool>() && E.IsReady())
                {
                    var eMinion =
                        MinionManager.GetMinions(Me.Position, E.Range, MinionTypes.All, MinionTeam.NotAlly)
                            .OrderBy(x => x.Position.Distance(Game.CursorPos))
                            .FirstOrDefault();

                    if (eMinion != null && SpellManager.CanCastE(eMinion) && E.IsReady() && Me.IsFacing(eMinion))
                    {
                        E.CastOnUnit(eMinion, true);
                    }
                }

                if (Menu.Item("FleeQ3", true).GetValue<bool>() && SpellManager.HaveQ3 && Q3.IsReady() &&
                    HeroManager.Enemies.Any(x => x.IsValidTarget(Q3.Range)))
                {
                    SpellManager.CastQ3();
                }
            }
        }
    }
}
