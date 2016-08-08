using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using DetuksSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; namespace ARAMDetFull.Champions
{
    class AurelionSol : Champion
    {

        public AurelionSol()
        {
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                {
                    new ConditionalItem(ItemId.Rylais_Crystal_Scepter),
                    new ConditionalItem(ItemId.Mercurys_Treads, ItemId.Ninja_Tabi, ItemCondition.ENEMY_AP),
                    new ConditionalItem(ItemId.Rod_of_Ages),
                    new ConditionalItem(ItemId.Abyssal_Scepter, ItemId.Zhonyas_Hourglass, ItemCondition.ENEMY_AP),
                    new ConditionalItem(ItemId.Rabadons_Deathcap),
                    new ConditionalItem(ItemId.Ludens_Echo),
                },
                startingItems = new List<ItemId>
                {
                    ItemId.Giants_Belt
                }
            };
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!W.LSIsReady())
                Q.Cast(target);
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.LSIsReady())
                return;
            if (!HasPassive())
            {
                if (target.LSIsValidTarget(350))
                {
                    return;
                }

                if (player.LSDistance(target) > 350 && player.LSDistance(target) < W.Range)
                {
                    W.Cast();
                }
            }
            else if (HasPassive())
            {
                if (player.LSDistance(target) > 350 && player.LSDistance(target) < W.Range + 100)
                {
                    return;
                }

                if (player.LSDistance(target) > 350 + 150)
                {
                    W.Cast();
                }
            }
        }

        public override void useE(Obj_AI_Base target)
        {
        }

        public override void useR(Obj_AI_Base target)
        {
            if (R.LSIsReady())
            {
                R.CastIfWillHit(target, 2);
            }
        }

        public override void setUpSpells()
        {
            Q = new Spell(SpellSlot.Q, 650f);
            W = new Spell(SpellSlot.W, 1500f);
            E = new Spell(SpellSlot.E, 400f);
            R = new Spell(SpellSlot.R, 1420f);

            Q.SetSkillshot(0.25f, 180, 850, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.25f, 300, 4500, false, SkillshotType.SkillshotLine);

        }

        public override void useSpells()
        {
            var tar = ARAMTargetSelector.getBestTarget(Q.Range);
            if (tar != null) useQ(tar);
            tar = ARAMTargetSelector.getBestTarget(W.Range+200);
            if (tar != null)
                useW(tar);
            else if(HasPassive())
                W.Cast();

            tar = ARAMTargetSelector.getBestTarget(R.Range);
            if (tar != null) useR(tar);
        }

        private bool HasPassive()
        {
            return player.LSHasBuff("AurelionSolWActive");
        }
    }
}
