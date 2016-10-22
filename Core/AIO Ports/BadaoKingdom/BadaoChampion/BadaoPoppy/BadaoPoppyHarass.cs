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
    public static class BadaoPoppyHarass
    {
        public static void BadaoActiavate()
        {
            Game.OnUpdate += Game_OnUpdate;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (BadaoMainVariables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Mixed)
                return;
            if (BadaoPoppyHelper.UseQHarass())
            {
                var target = TargetSelector.GetTarget(BadaoMainVariables.Q.Range, TargetSelector.DamageType.Physical);
                if (target.BadaoIsValidTarget())
                {
                    if (BadaoMainVariables.Q.Cast(target) == Spell.CastStates.SuccessfullyCasted)
                        BadaoPoppyVariables.QCastTick = Environment.TickCount;
                }
            }
        }

        private static void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (BadaoMainVariables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Mixed || !unit.IsMe || !(target is AIHeroClient))
                return;
            if (target.Position.Distance(ObjectManager.Player.Position) <= 200 + 125 + 140)
                BadaoChecker.BadaoUseTiamat();
        }
    }
}
