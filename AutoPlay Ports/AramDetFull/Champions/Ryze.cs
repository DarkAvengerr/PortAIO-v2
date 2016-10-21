using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;using DetuksSharp;
using LeagueSharp.Common;

using EloBuddy; namespace ARAMDetFull.Champions
{
    class Ryze : Champion
    {

        public Ryze()
        {
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.Rod_of_Ages),
                            new ConditionalItem(ItemId.Sorcerers_Shoes),
                            new ConditionalItem(ItemId.Archangels_Staff),
                            new ConditionalItem(ItemId.Spirit_Visage,ItemId.Frozen_Heart,ItemCondition.ENEMY_AP),
                            new ConditionalItem(ItemId.Rabadons_Deathcap),
                            new ConditionalItem(ItemId.Banshees_Veil),
                        },
                startingItems = new List<ItemId>
                        {
                            ItemId.Catalyst_of_Aeons
                        }
            };
        }


        public override void useQ(Obj_AI_Base target)
        {
            if(Q.IsReady())
                Q.Cast(target);
        }

        public override void useW(Obj_AI_Base target)
        {
            if (W.IsReady())
                W.Cast(target);
        }

        public override void useE(Obj_AI_Base target)
        {
            if (E.IsReady())
                E.CastOnUnit(target);
        }


        public override void useR(Obj_AI_Base target)
        {
            if (!R.IsReady() || target == null)
                return;
            R.Cast();
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
            Q = new Spell(SpellSlot.Q, 865);
            W = new Spell(SpellSlot.W, 585);
            E = new Spell(SpellSlot.E, 585);
            R = new Spell(SpellSlot.R);
            Q.SetSkillshot(0.25f, 50f, 1700f, true, SkillshotType.SkillshotLine);
        }

        public override void farm()
        {
            if (player.ManaPercent <= 60)
                return;
            var minionCount = MinionManager.GetMinions(player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health);
            {
                foreach (var minion in minionCount)
                {
                    if ( Q.IsReady()
                        && minion.IsValidTarget(Q.Range)
                        && minion.Health > Q.GetDamage(minion))
                    {
                        Q.Cast(minion);
                    }

                    if (W.IsReady()
                        && minion.IsValidTarget(W.Range)
                        && minion.Health > W.GetDamage(minion))
                    {
                        W.CastOnUnit(minion);
                    }

                    if (E.IsReady()
                        && minion.IsValidTarget(E.Range)
                        && minion.Health > E.GetDamage(minion))
                    {
                        E.CastOnUnit(minion);
                    }

                    if (Q.IsReady()
                        && minion.IsValidTarget(Q.Range))
                    {
                        Q.Cast(minion);
                    }

                    if (E.IsReady()
                        && minion.IsValidTarget(E.Range))
                    {
                        E.CastOnUnit(minion);
                    }
                    if (W.IsReady()
                        && minion.IsValidTarget(W.Range))
                    {
                        W.CastOnUnit(minion);
                    }
                }
            }
        }
    }
}
