using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoKingdom.BadaoChampion.BadaoPoppy
{
    public static class BadaoPoppyJungleClear
    {
        public static void BadaoActivate()
        {
            Game.OnUpdate += Game_OnUpdate;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
        }

        private static void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (BadaoMainVariables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear)
                return;
            if (target.Team != GameObjectTeam.Neutral)
                return;
            if (target.Position.Distance(ObjectManager.Player.Position) <= 200 + 125 + 140)
                BadaoChecker.BadaoUseTiamat();
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (BadaoMainVariables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear)
                return;
            if (!BadaoPoppyHelper.ManaJungle())
                return;
            if (BadaoPoppyHelper.UseEJungle() && Environment.TickCount - BadaoPoppyVariables.QCastTick >= 1250)
            {
                var minion = MinionManager.GetMinions(BadaoMainVariables.E.Range, MinionTypes.All, MinionTeam.Neutral
                                        , MinionOrderTypes.MaxHealth).FirstOrDefault();
                if (minion.BadaoIsValidTarget() && BadaoMath.GetFirstWallPoint(minion.Position.To2D(),
                    minion.Position.To2D().Extend(ObjectManager.Player.Position.To2D(), -300 - minion.BoundingRadius)) != null)
                {
                    BadaoMainVariables.E.Cast(minion);
                }
            }
            if (BadaoPoppyHelper.UseQJungle())
            {
                var minion = MinionManager.GetMinions(BadaoMainVariables.Q.Range, MinionTypes.All, MinionTeam.Neutral
                                        , MinionOrderTypes.MaxHealth).FirstOrDefault();
                if (minion != null)
                {
                    if(BadaoMainVariables.Q.Cast(minion) == Spell.CastStates.SuccessfullyCasted)
                        BadaoPoppyVariables.QCastTick = Environment.TickCount;
                }
            }
        }
    }
}
