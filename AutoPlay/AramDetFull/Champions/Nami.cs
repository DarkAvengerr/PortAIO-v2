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
    class Nami : Champion
    {
        public Nami()
        {
            DeathWalker.BeforeAttack += DeathWalkerOnBeforeAttack;
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.Athenes_Unholy_Grail),
                            new ConditionalItem(ItemId.Sorcerers_Shoes),
                            new ConditionalItem(ItemId.Rabadons_Deathcap),
                            new ConditionalItem(ItemId.Zhonyas_Hourglass),
                            new ConditionalItem(ItemId.Abyssal_Scepter),
                            new ConditionalItem(ItemId.Banshees_Veil),
                        },
                startingItems = new List<ItemId>
                        {
                            ItemId.Chalice_of_Harmony,ItemId.Boots_of_Speed
                        }
            };
        }

        private void DeathWalkerOnBeforeAttack(DeathWalker.BeforeAttackEventArgs args)
        {
            if (E.LSIsReady() && args.Unit is AIHeroClient && args.Unit.IsAlly && args.Target is AIHeroClient && E.IsInRange(args.Unit))
            {
                E.Cast(args.Unit);
            }
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.LSIsReady() || target == null)
                return;
            if (target.LSIsValidTarget(Q.Range + 100))
                Q.Cast(target);
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.LSIsReady() || target == null)
                return;
            HealLogic(target);
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.LSIsReady() || target == null)
                return;
            if ((target.HasBuffOfType(BuffType.Poison)))
            {
                E.CastOnUnit(target);
            }
        }

        public override void useR(Obj_AI_Base target)
        {
            if (target == null)
                return;
            if (target.LSIsValidTarget(R.Range) && R.LSIsReady())
            {
                if(R.CastIfWillHit(target, 2))
                    Aggresivity.addAgresiveMove(new AgresiveMove(55, 4000, true));
            }
        }

        public override void useSpells()
        {
            var tar = ARAMTargetSelector.getBestTarget(Q.Range);
            useQ(tar);
            tar = ARAMTargetSelector.getBestTarget(W.Range);
            useW(tar);
            tar = ARAMTargetSelector.getBestTarget(E.Range);
            useE(tar);
            tar = ARAMTargetSelector.getBestTarget(R.Range);
            useR(tar);
        }

        
        public override void setUpSpells()
        {
            Q = new Spell(SpellSlot.Q, 875);
            W = new Spell(SpellSlot.W, 725);
            E = new Spell(SpellSlot.E, 800);
            R = new Spell(SpellSlot.R, 1800);

            Q.SetSkillshot(1f, 150f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.5f, 220f, 850f, false, SkillshotType.SkillshotLine);
        }

        private double WHeal
        {
            get
            {
                int[] heal = { 0, 65, 95, 125, 155, 185 };
                return heal[W.Level] + player.FlatMagicDamageMod * 0.3;
            }
        }

        private void HealLogic(Obj_AI_Base target)
        {
            var ally = AllyBelowHp(35, W.Range);
            if (ally != null) // force heal low ally
            {
                W.CastOnUnit(ally);
                return;
            }

            if (player.LSDistance(target) > W.Range) // target out of range try bounce
            {
                var bounceTarget =
                    ObjectManager.Get<AIHeroClient>()
                        .SingleOrDefault(hero => hero.LSIsValidTarget(W.Range) && hero.LSDistance(target) < W.Range);

                if (bounceTarget != null && bounceTarget.MaxHealth - bounceTarget.Health > WHeal) // use bounce & heal
                {
                    W.CastOnUnit(bounceTarget);
                }
            }
            else // target in range
            {
                W.CastOnUnit(target);
            }
        }

        public static AIHeroClient AllyBelowHp(int percentHp, float range)
        {
            foreach (var ally in ObjectManager.Get<AIHeroClient>())
            {
                if (ally.IsMe)
                {
                    if (((ObjectManager.Player.Health / ObjectManager.Player.MaxHealth) * 100) < percentHp)
                    {
                        return ally;
                    }
                }
                else if (ally.IsAlly)
                {
                    if (Vector3.Distance(ObjectManager.Player.Position, ally.Position) < range &&
                        ((ally.Health / ally.MaxHealth) * 100) < percentHp)
                    {
                        return ally;
                    }
                }
            }

            return null;
        }
    }
}
