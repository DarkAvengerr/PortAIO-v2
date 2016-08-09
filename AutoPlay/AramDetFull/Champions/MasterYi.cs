using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DetuksSharp;
using LeagueSharp;using DetuksSharp;
using LeagueSharp.Common;

using EloBuddy; namespace ARAMDetFull.Champions
{
    class MasterYi : Champion
    {

        public MasterYi()
        {
            ARAMSimulator.farmRange = 750;
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                {
                    new ConditionalItem(ItemId.Blade_of_the_Ruined_King),
                    new ConditionalItem(ItemId.Berserkers_Greaves,ItemId.Mercurys_Treads,ItemCondition.ENEMY_LOSING),
                    new ConditionalItem(ItemId.Phantom_Dancer),
                    new ConditionalItem(ItemId.Youmuus_Ghostblade,ItemId.Warmogs_Armor,ItemCondition.ENEMY_LOSING),
                    new ConditionalItem(ItemId.The_Black_Cleaver),
                    new ConditionalItem(ItemId.Infinity_Edge, ItemId.Frozen_Mallet,ItemCondition.ENEMY_LOSING),
                },
                startingItems = new List<ItemId>
                {
                    ItemId.Vampiric_Scepter, ItemId.Boots_of_Speed
                }
            };
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.IsReady() || target == null)
                return;
            if (safeGap(target))
                Q.CastOnUnit(target);
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady())
                return;
            if (player.HealthPercent < 35 && !Q.IsReady())
                W.Cast();
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady() || target == null)
                return;
            E.Cast();
        }


        public override void useR(Obj_AI_Base target)
        {
            if (!R.IsReady() || target == null)
                return;
            if(R.Cast())
                Aggresivity.addAgresiveMove(new AgresiveMove(565,6000,true));
        }

        public override void useSpells()
        {
            var tar = ARAMTargetSelector.getBestTarget(Q.Range);
            if (tar != null) useQ(tar);
            tar = ARAMTargetSelector.getBestTarget(W.Range);
            if (tar != null) useW(tar);
            tar = ARAMTargetSelector.getBestTarget(E.Range);
            if (tar != null) useE(tar);
            tar = ARAMTargetSelector.getBestTarget(R.Range);
            if (tar != null) useR(tar);
        }

        public override void setUpSpells()
        {
            //Create the spells
            Q = new Spell(SpellSlot.Q, 600);
            W = new Spell(SpellSlot.W, 500);
            E = new Spell(SpellSlot.E, 200);
            R = new Spell(SpellSlot.R, 250);
        }

        public override void farm()
        {
            if (!Q.IsReady())
                return;
            if (player.ManaPercent < 45)
                return;

            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range);

            foreach (var minion in minions.Where(minion => minion.IsValidTarget(Q.Range) && minion.Health < Q.GetDamage(minion)))
            {
                Q.Cast(minion);
                return;
            }
        }
    }
}
