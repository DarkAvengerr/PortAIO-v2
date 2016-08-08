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
    class Quinn : Champion
    {

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.LSIsReady())
                return;
            if (IsValorMode() && player.LSDistance(target,true) <= 375*375)
            {
                Q.Cast();
            }
            if (!IsValorMode())
            {
                Q.Cast(target);
            }

            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.The_Bloodthirster),
                            new ConditionalItem(ItemId.Berserkers_Greaves),
                            new ConditionalItem(ItemId.Statikk_Shiv),
                            new ConditionalItem(ItemId.Frozen_Mallet),
                            new ConditionalItem(ItemId.Blade_of_the_Ruined_King),
                            new ConditionalItem(ItemId.Banshees_Veil),
                        },
                startingItems = new List<ItemId>
                        {
                            ItemId.Vampiric_Scepter,ItemId.Boots_of_Speed
                        }
            };
        }

        public override void useW(Obj_AI_Base target)
        {
            //if (MapControl.balanceAroundPoint(target.Position.LSTo2D(), 400) <= 1)
             //   W.CastOnUnit(target);
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.LSIsReady())
                return;
            if (!Sector.inTowerRange(target.Position.LSTo2D()) &&
                (MapControl.balanceAroundPoint(target.Position.LSTo2D(), 700) >= -1 ||
                 (MapControl.fightIsOn() != null && MapControl.fightIsOn().NetworkId == target.NetworkId)))
            {

                var passive = player.CalcDamage(target, Damage.DamageType.Physical,
                    15 + (player.Level*10) + (player.FlatPhysicalDamageMod*0.5));

                if (IsValorMode())
                {
                    if (R.LSIsReady() && target.LSDistance(player) < R.Range)
                    {
                        var ultdamage = player.CalcDamage(target, Damage.DamageType.Physical,
                            (75 + (R.Level*55) + (player.FlatPhysicalDamageMod*0.5))*
                            (2 - (target.Health/target.MaxHealth)));

                        if ((ultdamage + passive) > target.Health)
                        {
                            R.Cast();
                            E.CastOnUnit(target);
                            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                        }
                    }
                    else
                    {
                        E.CastOnUnit(target);
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                    }
                }
                else // human form
                {
                    if (MapControl.balanceAroundPoint(target.Position.LSTo2D(), 600) >= -1)
                    {
                        E.CastOnUnit(target, true);
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                    }
                }
            }
        }

        public override void useR(Obj_AI_Base target)
        {
            return;
            if (!R.LSIsReady())
                return;
            if (player.Path.Length > 0 && player.Path[player.Path.Length - 1].LSDistance(player.Position) > 2500)
            {
                R.Cast(player.Path[player.Path.Length - 1]);
            }


        }

        public override void setUpSpells()
        {
            Q = new Spell(SpellSlot.Q, 1010);
            E = new Spell(SpellSlot.E, 800);
            R = new Spell(SpellSlot.R, 550);

            Q.SetSkillshot(0.25f, 80f, 1150f, true, SkillshotType.SkillshotLine);
            E.SetTargetted(0.25f, 2000f);
        }

        public override void useSpells()
        {
            var tar = ARAMTargetSelector.getBestTarget(1010);
            if (tar == null)
                return;
            useQ(tar);
            useE(tar);
            useR(tar);
        }

        public bool IsValorMode()
        {
            return ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Name == "QuinnRFinale";
        }
    }
}
