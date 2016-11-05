using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using ItemData = LeagueSharp.Common.Data.ItemData;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HeavenStrikeRyze
{
    public static class Combo
    {
        private static AIHeroClient Player { get { return ObjectManager.Player; } }

        public static void BadaoActivate()
        {
            Game.OnUpdate += Game_OnUpdate;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
        }

        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (Program._orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
                return;

        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Program._orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
                return;
            var target = TargetSelector.GetTarget(600, TargetSelector.DamageType.Magical);
            var target1 = TargetSelector.GetTarget(Program._q.Range, TargetSelector.DamageType.Magical);
            if (Program.mode == "AoE/Shield" && Player.Level > 2)
            {
                var tarqs1 = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsValidTarget() && x.IsMinion && x.Distance(Player.Position) <= Program._q.Range);
                var tars1 = tarqs1.Where(x => x.Distance(Player.Position) <= Program._e.Range);
                var heroes = HeroManager.Enemies.Where(x => x.IsValidTarget());
                var tarqs = new List<Obj_AI_Base>();
                tarqs.AddRange(tarqs1);
                tarqs.AddRange(heroes.Where(x => x.IsValidTarget(Program._q.Range)));
                var tars = new List<Obj_AI_Base>();
                tars.AddRange(tars1);
                tars.AddRange(heroes.Where(x => x.IsValidTarget(Program._e.Range)));

                Obj_AI_Base MainTarget = null;

                // in investigation
                MainTarget = tarqs.Where(x => Helper.HasEBuff(x) && Program._q.GetPrediction(x).Hitchance >= HitChance.Low && Program._q.IsReady()
                    && Helper.GetchainedTarget(x).Count() >= 2 && Helper.GetchainedTarget(x).Count(y => y is AIHeroClient) >= 1)
                    .MaxOrDefault(x => Helper.GetchainedTarget(x).Count(y => y is AIHeroClient));
                if (MainTarget != null)
                {
                    Program._q.Cast(Program._q.GetPrediction(MainTarget).UnitPosition);
                }

                MainTarget = tars.Where(x => Helper.HasEBuff(x) && Program._e.IsReady()
                    && Helper.GetchainedTarget(x).Count(y => y is AIHeroClient) >= 1)
                    .MaxOrDefault(x => Helper.GetchainedTarget(x).Count(y => y is AIHeroClient));
                if (MainTarget != null)
                {
                    Program._e.Cast(MainTarget);
                }

                MainTarget = tars.Where(x => x.Health <= Helper.Edamge(x) && Program._e.IsReady()
                    && Helper.GetchainedTarget(x).Count(y => y is AIHeroClient) >= 1)
                    .MaxOrDefault(x => Helper.GetchainedTarget(x).Count(y => y is AIHeroClient));
                if (MainTarget != null)
                {
                    Program._e.Cast(MainTarget);
                }

                if (Helper.Qstack() == 2 && target1.IsValidTarget() && !target1.IsZombie)
                {
                    Program._q2.Cast(target1);
                }

                if (Helper.Qstack() != 2 && target.IsValidTarget() && !target.IsZombie)
                {
                    if (Program._e.IsReady())
                        Program._e.Cast(target);
                    if (Program._w.IsReady())
                        Program._w.Cast(target);
                }

            }
            if (Program.mode == "Burst" || Player.Level <= 2)
            {
                if (target.IsValidTarget() && !target.IsZombie)
                {
                    if (Program._q.IsReady())
                    {
                        Helper.CastQTarget(target, true);
                        return;
                    }
                    if (Program._w.IsReady())
                    {
                        Program._w.Cast(target);
                        return;
                    }
                    if (Program._e.IsReady())
                    {
                        Program._e.Cast(target);
                        return;
                    }
                }
                else
                {

                    if (target1.IsValidTarget() && !target1.IsZombie)
                    {
                        Helper.CastQTarget(target1);
                    }
                }
            }
        }
    }
}
