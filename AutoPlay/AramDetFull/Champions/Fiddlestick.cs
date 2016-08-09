using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;using DetuksSharp;
using LeagueSharp.Common;

using EloBuddy; namespace ARAMDetFull.Champions
{
    class Fiddlestick : Champion
    {

        public int drainStart = 0;


        public bool justUsedDrain
        {
            get { return drainStart + 500 > DeathWalker.now; }
        }

        public Fiddlestick()
        {
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.Zhonyas_Hourglass),
                            new ConditionalItem(ItemId.Ionian_Boots_of_Lucidity),
                            new ConditionalItem(ItemId.Morellonomicon),
                            new ConditionalItem(ItemId.Void_Staff),
                            new ConditionalItem(ItemId.Abyssal_Scepter),
                            new ConditionalItem(ItemId.Rabadons_Deathcap),
                        },
                startingItems = new List<ItemId>
                        {
                            ItemId.Needlessly_Large_Rod
                        }
            };
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (target == null || !Q.IsReady())
                return;
            Q.CastOnUnit(target);
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady() || target == null || Q.IsReady() || E.IsReady())
                return;
            W.CastOnUnit(target);
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady() || target == null)
            {
                return;
            }

            if (E.CastOnUnit(target))
                drainStart = DeathWalker.now;
        }

        public override void useR(Obj_AI_Base target)
        {
            if (!R.IsReady())
                return;
            if (!Sector.inTowerRange(target.Position.To2D()) &&  (MapControl.fightIsOn() != null ) && player.HealthPercent>45)
                R.Cast(target.Position);
        }

        public override void useSpells()
        {
            if (player.HasBuff("Drain") || justUsedDrain)
            {
                DeathWalker.setMovement(false);
                DeathWalker.setAttack(false);
            }
            if (justUsedDrain)
                return;
            if (!player.HasBuff("Drain"))
            {
                DeathWalker.setMovement(true);
                DeathWalker.setAttack(true);
            }

            var tar = ARAMTargetSelector.getBestTarget(Q.Range);
            if (tar != null) useQ(tar);
            tar = ARAMTargetSelector.getBestTarget(E.Range);
            if (tar != null) useE(tar);
            tar = ARAMTargetSelector.getBestTarget(W.Range);
            if (tar != null) useW(tar);
            tar = ARAMTargetSelector.getBestTarget(R.Range);
            if (tar != null) useR(tar);
        }

        public override void setUpSpells()
        {
            Q = new Spell(SpellSlot.Q, 575);
            W = new Spell(SpellSlot.W, 575);
            E = new Spell(SpellSlot.E, 750);
            R = new Spell(SpellSlot.R, 800);
        }
    }
}
