using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using DetuksSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; namespace ARAMDetFull.Champions
{
    class Tryndamere : Champion
    {
        public Tryndamere()
        {
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                {
                    new ConditionalItem(ItemId.Statikk_Shiv),
                    new ConditionalItem(ItemId.Berserkers_Greaves,ItemId.Mercurys_Treads,ItemCondition.ENEMY_LOSING),
                    new ConditionalItem(ItemId.Infinity_Edge),
                    new ConditionalItem(ItemId.Youmuus_Ghostblade,ItemId.Warmogs_Armor,ItemCondition.ENEMY_LOSING),
                    new ConditionalItem(ItemId.The_Bloodthirster),
                    new ConditionalItem(ItemId.Maw_of_Malmortius, ItemId.Frozen_Mallet,ItemCondition.ENEMY_AP),
                },
                startingItems = new List<ItemId>
                {
                    ItemId.Vampiric_Scepter, ItemId.Boots_of_Speed
                }
            };
            AttackableUnit.OnDamage +=AttackableUnitOnOnDamage;
        }

        private void AttackableUnitOnOnDamage(AttackableUnit sender, AttackableUnitDamageEventArgs args)
        {
            if (args.Target.NetworkId != player.NetworkId)
                return;
            if (R.IsReady() && player.HealthPercent<25)
            {
                R.Cast();
                Aggresivity.addAgresiveMove(new AgresiveMove(9999,5000,true));
            }

            if (Q.IsReady() && player.HealthPercent < 30 && !player.HasBuff("UndyingRage"))
            {
                Q.Cast();
            }
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.IsReady() || target == null)
                return;
            if(player.HealthPercent<30 && !player.HasBuff("UndyingRage"))
                Q.Cast();
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady() || target == null)
                return;
            if(player.IsFacing(target) && !target.IsFacing(player))
                W.Cast();
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady() || target == null || !safeGap(target))
                return;
            E.Cast(target);
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
            tar = ARAMTargetSelector.getBestTarget(E.Range);
            if (tar != null) useE(tar);
        }


        public override void setUpSpells()
        {
            Q = new Spell(SpellSlot.Q, 700);
            W = new Spell(SpellSlot.W, 830);
            E = new Spell(SpellSlot.E, 660);
            R = new Spell(SpellSlot.R, 800);
            E.SetSkillshot(0, 93, 1300, false, SkillshotType.SkillshotLine);
        }


        public override void farm()
        {
        }
    }
}
