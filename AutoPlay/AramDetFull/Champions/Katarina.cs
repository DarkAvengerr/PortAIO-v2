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
            if (!Q.LSIsReady())
                return;
            Q.CastOnUnit(target);
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.LSIsReady())
                return;
            W.Cast();
            Aggresivity.addAgresiveMove(new AgresiveMove(20,2000));
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.LSIsReady())
                return;
            if (player.Health < 24 && player.LSCountEnemiesInRange(550) > 0)
            {
                var minion = ObjectManager.Get<Obj_AI_Base>().Where(o => o.IsValid && !o.IsDead && o.LSDistance(player,true)<E.Range*E.Range).OrderBy(m => m.LSDistance(ARAMSimulator.fromNex.Position,true)).FirstOrDefault();
                if(minion != null)
                    E.CastOnUnit(minion);
            } else if (safeGap(target))
                E.CastOnUnit(target);
        }

        public override void useR(Obj_AI_Base target)
        {
            if (!R.LSIsReady())
                return;
            if (R.GetDamage(target, 1) * 8 > target.Health || player.LSCountEnemiesInRange(450) > 1 || player.HealthPercent<25)
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

            if (E.LSIsReady() && GetComboDamage(target)<target.Health &&
                ObjectManager.Player.LSDistance(target, false) < E.Range + target.BoundingRadius)
                E.CastOnUnit(target, true);

            if (Q.LSIsReady() && Q.IsKillable(target, 1) &&
                ObjectManager.Player.LSDistance(target, false) < Q.Range + target.BoundingRadius)
                Q.CastOnUnit(target, true);

            if (W.LSIsReady() && W.IsKillable(target) &&
                ObjectManager.Player.LSDistance(target, false) < W.Range)
                W.Cast();

            if (Q.LSIsReady() && E.LSIsReady() &&
                ObjectManager.Player.IsKillable(target,
                    new[] { Tuple.Create(SpellSlot.Q, 1), Tuple.Create(SpellSlot.E, 0) }) &&
                ObjectManager.Player.LSDistance(target, false) < Q.Range + target.BoundingRadius)
            {
                Q.CastOnUnit(target, true);
                E.CastOnUnit(target, true);
            }

            if (Q.LSIsReady() && E.LSIsReady() && W.LSIsReady() &&
                ObjectManager.Player.IsKillable(target,
                    new[] { Tuple.Create(SpellSlot.Q, 0), Tuple.Create(SpellSlot.E, 0), Tuple.Create(SpellSlot.W, 0) }) &&
                ObjectManager.Player.LSDistance(target, false) < Q.Range + target.BoundingRadius)
            {
                Q.Cast(target);
                E.Cast(target);
                if (ObjectManager.Player.LSDistance(target, false) < W.Range)
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

            if (useQ && Q.LSIsReady())
            {
                foreach (var minion in allMinions.Where(minion => minion.LSIsValidTarget() && HealthPrediction.GetHealthPrediction(minion, (int)(ObjectManager.Player.LSDistance(minion, false) * 1000 / 1400))
                < 0.75 * ObjectManager.Player.LSGetSpellDamage(minion, SpellSlot.Q, 1)))
                {
                    Q.Cast(minion);
                    return;
                }
            }
            else if (useW && W.LSIsReady())
            {
                if (!allMinions.Any(minion => minion.LSIsValidTarget(W.Range) && minion.Health < 0.75 * ObjectManager.Player.LSGetSpellDamage(minion, SpellSlot.W))) return;
                W.Cast();
                return;
            }

            foreach (var minion in allMinions)
            {
                if (useQ)
                    Q.Cast(minion);

                if (useW && ObjectManager.Player.LSDistance(minion, false) < W.Range)
                    W.Cast(minion);
            }
        }

        private float GetComboDamage(Obj_AI_Base enemy)
        {
            var damage = 0d;

            if (Q.LSIsReady(420))
                damage += ObjectManager.Player.LSGetSpellDamage(enemy, SpellSlot.Q);


            if (W.LSIsReady())
                damage += ObjectManager.Player.LSGetSpellDamage(enemy, SpellSlot.W);

            if (E.LSIsReady())
                damage += ObjectManager.Player.LSGetSpellDamage(enemy, SpellSlot.E);


            if (R.LSIsReady())
                damage += ObjectManager.Player.LSGetSpellDamage(enemy, SpellSlot.R, 1) * 8;
            return (float)damage;
        }
    }
}
