using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Nasus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;


    public enum Spells
    {
        Q,
        W,
        E,
        R
    }

    public class LNasus : Standards
    {

        private static IEnumerable<AIHeroClient> Enemies => HeroManager.Enemies;

        public static void OnLoad(EventArgs args)
        {
            try
            {
                if (!Player.IsChampion("Nasus"))
                {
                    return;
                }

                Chat.Print("[Nasus - The Crazy Dog] Loaded");
                Chat.Print("[Nasus - The Crazy Dog] Use ElUtilitySuite by jQuery for better experience");

                MenuInit.Initialize();
                Game.OnUpdate += GameOnUpdate;
                Orbwalking.BeforeAttack += OrbwalkingBeforeAttack;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void Combo()
        {
            var target = TargetSelector.GetSelectedTarget()
                ?? TargetSelector.GetTarget(spells[Spells.W].Range, TargetSelector.DamageType.Physical);

            var HPPercentToR = MenuInit.Menu.SubMenu("MenuCombo").Item("Combo.Min.HP.Use.R").GetValue<Slider>().Value;

            if (target.IsValidTarget() == false)
            {
                return;
            }
            if (target != null)
            {
                if (IsActive("Combo.Use.W") && spells[Spells.W].IsReady() && spells[Spells.W].IsInRange(target))
                {
                    spells[Spells.W].Cast(target);
                }
                if (IsActive("Combo.Use.E") && spells[Spells.E].IsReady() && spells[Spells.E].IsInRange(target))
                {
                    var prediction = spells[Spells.E].GetPrediction(target).Hitchance;
                    if (prediction >= HitChance.VeryHigh)
                    {
                        spells[Spells.E].Cast(target);
                    }
                    else { return; }
                }
            }
            if (IsActive("Combo.Use.R") && spells[Spells.R].IsReady()
                   && Player.CountEnemiesInRange(spells[Spells.E].Range) >= 1
                   && Player.HealthPercent <= HPPercentToR)
            {
                spells[Spells.R].CastOnUnit(Player);
            }
        }

        private static void LaneClear()
        {
            var MinionQ     = MinionManager.GetMinions(spells[Spells.Q].Range);
            var EMinion     = MinionManager.GetMinions(spells[Spells.E].Range + spells[Spells.E].Width);
            var ELocation   = spells[Spells.E].GetCircularFarmLocation(EMinion, spells[Spells.E].Range);

            if (IsActive("LaneClear.Use.Q"))
            {
                foreach (var minion in MinionQ)
                {
                    if (minion.Health <= spells[Spells.Q].GetDamage(minion))
                    {
                        spells[Spells.Q].Cast();
                        Orbwalker.ForceTarget(minion);
                    }
                }
            }
            if (IsActive("LaneClear.Use.E"))
            {
                foreach (var minion in EMinion)
                {
                    if (spells[Spells.E].IsInRange(minion))
                    {
                        spells[Spells.E].Cast(ELocation.Position);
                    }
                }
            }

        }

        private static void GameOnUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    break;
            }
        }

        private static void OrbwalkingBeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    var target = TargetSelector.GetSelectedTarget()
                        ?? TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Physical);

                    if (target.IsValidTarget() && IsActive("Combo.Use.Q") && spells[Spells.Q].IsInRange(target) && spells[Spells.Q].IsReady())
                    {
                        spells[Spells.Q].Cast();
                    }
                    break;

                case Orbwalking.OrbwalkingMode.LastHit:
                    var MinionQ = MinionManager.GetMinions(spells[Spells.Q].Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health);

                    if(IsActive("Use.StackQ"))
                    {
                        foreach(var minion in MinionQ)
                        {
                            if (minion.Health <= spells[Spells.Q].GetDamage(minion) 
                                + Player.GetAutoAttackDamage(minion) && spells[Spells.Q].IsReady())
                            {
                                spells[Spells.Q].Cast();
                                Orbwalker.ForceTarget(minion);
                            }
                        }
                    }
                    break;
            }
        }

    }
}
