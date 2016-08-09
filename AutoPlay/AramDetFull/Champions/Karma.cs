using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;using DetuksSharp;
using LeagueSharp.Common;

using EloBuddy; namespace ARAMDetFull.Champions
{
    class KarmaA : Champion
    {


        public KarmaA()
        {
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                    {
                        new ConditionalItem(ItemId.Morellonomicon),
                        new ConditionalItem(ItemId.Sorcerers_Shoes),
                        new ConditionalItem(ItemId.Ludens_Echo),
                        new ConditionalItem(ItemId.Rabadons_Deathcap),
                        new ConditionalItem(ItemId.Abyssal_Scepter,ItemId.Zhonyas_Hourglass,ItemCondition.ENEMY_AP),
                        new ConditionalItem(ItemId.Void_Staff),
                    },
                startingItems = new List<ItemId>
                    {
                        ItemId.Needlessly_Large_Rod
                    }
            };
        }


        public override void useQ(Obj_AI_Base target)
        {
            if (Q.IsReady())
            {
                if (R.IsReady())
                    R.Cast();
                Q.Cast(target);
            }
        }

        public override void useW(Obj_AI_Base target)
        {
            if ((player.Health / ObjectManager.Player.MaxHealth) /
                    (target.Health / target.MaxHealth) < 1)
            {
                if (R.IsReady())
                    R.Cast();

                W.Cast(target);
            }
        }

        public override void useE(Obj_AI_Base target)
        {
        }


        public override void useR(Obj_AI_Base target)
        {
        }

        public override void useSpells()
        {
            var tar = ARAMTargetSelector.getBestTarget(Q.Range);
            if (tar != null) useQ(tar);
            tar = ARAMTargetSelector.getBestTarget(W.Range);
            if (tar != null) useW(tar);

        }
        

        public override void killSteal()
        {
            base.killSteal();
            if(E.IsReady())
            foreach (var hero in
                    DeathWalker.AllAllys
                        .Where(
                            hero =>
                                hero.IsValidTarget(E.Range, false) && hero.IsAlly &&
                                ObjectManager.Get<AIHeroClient>().Count(h => h.IsValidTarget() && h.Distance(hero) < 400) >
                                1))
            {
                if (R.IsReady())
                    R.Cast();
                E.Cast(hero);
            }
        }

        public override void setUpSpells()
        {
            //Create the spells
            Q = new Spell(SpellSlot.Q, 1050f);
            W = new Spell(SpellSlot.W, 700f);
            E = new Spell(SpellSlot.E, 800f);
            R = new Spell(SpellSlot.R);

            Q.SetSkillshot(0.25f, 60f, 1700f, true, SkillshotType.SkillshotLine);
            W.SetTargetted(0.25f, float.MaxValue);
        }


    }
}
