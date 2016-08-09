using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;using DetuksSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; namespace ARAMDetFull.Champions
{
    class Katarina : Champion
    {
        public Katarina()
        {
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.Ludens_Echo),
                            new ConditionalItem(ItemId.Sorcerers_Shoes),
                            new ConditionalItem(ItemId.Rabadons_Deathcap),
                            new ConditionalItem(ItemId.Abyssal_Scepter, ItemId.Void_Staff,ItemCondition.ENEMY_AP),
                            new ConditionalItem(ItemId.Zhonyas_Hourglass),
                            new ConditionalItem(ItemId.Banshees_Veil),
                        },
                startingItems = new List<ItemId>
                        {
                            ItemId.Needlessly_Large_Rod
                        }
            };
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.IsReady())
                return;
            Q.CastOnUnit(target);
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady())
                return;
            W.Cast();
            Aggresivity.addAgresiveMove(new AgresiveMove(20,2000));
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady())
                return;
            if (player.Health < 24 && player.CountEnemiesInRange(550) > 0)
            {
                var minion = ObjectManager.Get<Obj_AI_Base>().Where(o => o.IsValid && !o.IsDead && o.Distance(player,true)<E.Range*E.Range).OrderBy(m => m.Distance(ARAMSimulator.fromNex.Position,true)).FirstOrDefault();
                if(minion != null)
                    E.CastOnUnit(minion);
            } else if (safeGap(target))
                E.CastOnUnit(target);
        }

        public override void useR(Obj_AI_Base target)
        {
            if (!R.IsReady())
                return;
            if (R.GetDamage(target, 1) * 8 > target.Health || player.CountEnemiesInRange(450) > 1 || player.HealthPercent<25)
                R.Cast();
        }

        public override void setUpSpells()
        {
            Q = new Spell(SpellSlot.Q, 675);
            W = new Spell(SpellSlot.W, 375);
            E = new Spell(SpellSlot.E, 700);
            R = new Spell(SpellSlot.R, 450);
        }

        public override void useSpells()
        {
            try
            {
                if (player.IsChannelingImportantSpell())
                    return;
                var tar = ARAMTargetSelector.getBestTarget(Q.Range);
                if (tar != null) useQ(tar);
                tar = ARAMTargetSelector.getBestTarget(E.Range);
                if (tar != null) useE(tar);
                tar = ARAMTargetSelector.getBestTarget(W.Range);
                if (tar != null) useW(tar);
                tar = ARAMTargetSelector.getBestTarget(R.Range);
                if (tar != null) useR(tar);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }

        public override void killSteal()
        {
            var target = ARAMTargetSelector.getBestTarget(E.Range);
            if (target == null) return;

            if (E.IsReady() && GetComboDamage(target)<target.Health &&
                ObjectManager.Player.Distance(target, false) < E.Range + target.BoundingRadius)
                E.CastOnUnit(target, true);

            if (Q.IsReady() && Q.IsKillable(target, 1) &&
                ObjectManager.Player.Distance(target, false) < Q.Range + target.BoundingRadius)
                Q.CastOnUnit(target, true);

            if (W.IsReady() && W.IsKillable(target) &&
                ObjectManager.Player.Distance(target, false) < W.Range)
                W.Cast();

            if (Q.IsReady() && E.IsReady() &&
                ObjectManager.Player.IsKillable(target,
                    new[] { Tuple.Create(SpellSlot.Q, 1), Tuple.Create(SpellSlot.E, 0) }) &&
                ObjectManager.Player.Distance(target, false) < Q.Range + target.BoundingRadius)
            {
                Q.CastOnUnit(target, true);
                E.CastOnUnit(target, true);
            }

            if (Q.IsReady() && E.IsReady() && W.IsReady() &&
                ObjectManager.Player.IsKillable(target,
                    new[] { Tuple.Create(SpellSlot.Q, 0), Tuple.Create(SpellSlot.E, 0), Tuple.Create(SpellSlot.W, 0) }) &&
                ObjectManager.Player.Distance(target, false) < Q.Range + target.BoundingRadius)
            {
                Q.Cast(target);
                E.Cast(target);
                if (ObjectManager.Player.Distance(target, false) < W.Range)
                {
                    W.Cast();
                }
            }
        }

        public override void farm()
        {
            if (!DeathWalker.canMove()) return;
            var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range);
            var useQ = true;
            var useW = true;

            if (useQ && Q.IsReady())
            {
                foreach (var minion in allMinions.Where(minion => minion.IsValidTarget() && HealthPrediction.GetHealthPrediction(minion, (int)(ObjectManager.Player.Distance(minion, false) * 1000 / 1400))
                < 0.75 * ObjectManager.Player.GetSpellDamage(minion, SpellSlot.Q, 1)))
                {
                    Q.Cast(minion);
                    return;
                }
            }
            else if (useW && W.IsReady())
            {
                if (!allMinions.Any(minion => minion.IsValidTarget(W.Range) && minion.Health < 0.75 * ObjectManager.Player.GetSpellDamage(minion, SpellSlot.W))) return;
                W.Cast();
                return;
            }

            foreach (var minion in allMinions)
            {
                if (useQ)
                    Q.Cast(minion);

                if (useW && ObjectManager.Player.Distance(minion, false) < W.Range)
                    W.Cast(minion);
            }
        }

        private float GetComboDamage(Obj_AI_Base enemy)
        {
            var damage = 0d;

            if (Q.IsReady(420))
                damage += ObjectManager.Player.GetSpellDamage(enemy, SpellSlot.Q);


            if (W.IsReady())
                damage += ObjectManager.Player.GetSpellDamage(enemy, SpellSlot.W);

            if (E.IsReady())
                damage += ObjectManager.Player.GetSpellDamage(enemy, SpellSlot.E);


            if (R.IsReady())
                damage += ObjectManager.Player.GetSpellDamage(enemy, SpellSlot.R, 1) * 8;
            return (float)damage;
        }
    }
}
