using System;
using System.Linq;
using DZLib.Logging;
using iSeriesReborn.Utility;
using iSeriesReborn.Utility.Entities;
using iSeriesReborn.Utility.MenuUtility;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.Jinx.Skills
{
    class JinxQ
    {

        public static void HandleQLogic()
        {
            if (!Orbwalking.CanMove(80))
            {
                return;
            }

            if (ObjectManager.Player.Spellbook.IsAutoAttacking || ObjectManager.Player.Spellbook.IsAutoAttacking)
            {
                return;
            }
            
            if (Variables.spells[SpellSlot.Q].IsEnabledAndReady(false))
            {
                var maxAaRange = JinxUtility.GetMinigunRange(null) + JinxUtility.GetFishboneRange() + 25f;
                var selectedTarget = TargetSelector.GetTarget(maxAaRange, TargetSelector.DamageType.Physical);
                var jinxBaseRange = JinxUtility.GetMinigunRange(selectedTarget);

                if (selectedTarget.LSIsValidTarget())
                {
                    var manaItem = Variables.Menu.Item($"iseriesr.{ObjectManager.Player.ChampionName.ToLower()}.{Variables.Orbwalker.ActiveMode.ToString().ToLower()}.mm.{SpellSlot.Q.ToString().ToLower()}");
                    var manaCondition = (manaItem != null && ObjectManager.Player.ManaPercent >= manaItem.GetValue<Slider>().Value);

                    if (JinxUtility.IsFishBone())
                    {
                         //If we can kill the target within 3 AAs then don't switch.
                         //Jinx Q uses 20 mana for each AA. So check if we can do the 3 AAs.
                         //And also check if the target is not about to escape our AA range.
                            if (selectedTarget.Health + 5 <= ObjectManager.Player.LSGetAutoAttackDamage(selectedTarget) * 3 
                                && (ObjectManager.Player.Mana - 20 * 3 > 0)
                                && !(selectedTarget.LSDistance(ObjectManager.Player) > JinxUtility.GetFishboneRange() * 0.9f 
                                && (ObjectManager.Player.ServerPosition.LSDistance((selectedTarget.ServerPosition.LSTo2D() + selectedTarget.Direction.LSTo2D().LSPerpendicular() * (300 + 65f)).To3D()) > ObjectManager.Player.LSDistance(selectedTarget.ServerPosition))
                                ))
                            {
                                return;
                            }

                        //We don't have more mana then set in the menu.
                        if (!manaCondition)
                        {
                            //Swap to minigun
                            Variables.spells[SpellSlot.Q].Cast();
                            return;
                        }

                        //If the distance from the selected target is less than the minigun base range. And it has no enemies in 150 (AOE) range within it.
                        //Swap to minigun.
                        if (ObjectManager.Player.LSDistance(selectedTarget) < jinxBaseRange && !(selectedTarget.ServerPosition.LSCountEnemiesInRange(150) >= 2))
                        {
                            Variables.spells[SpellSlot.Q].Cast();
                        }
                    }
                    else
                    {
                        //If the distance is greater than our current AA range or the selected target has enemies in AOE range.
                        //Swap to fishbone
                        if (ObjectManager.Player.LSDistance(selectedTarget) > jinxBaseRange || (selectedTarget.ServerPosition.LSCountEnemiesInRange(150) >= 2))
                        {
                            Variables.spells[SpellSlot.Q].Cast();
                        }
                    }
                }
            }
        }

        public static void QSwapNoEnemies()
        {
            if (Variables.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo
                && JinxUtility.IsFishBone()
                && MenuExtensions.GetItemValue<bool>("iseriesr.jinx.q.switch.noenemies")
                && ObjectManager.Player.LSCountEnemiesInRange(1500) < 1)
            {
                Variables.spells[SpellSlot.Q].Cast();
            }
        }

        public static void QSwapLC()
        {
            //If we are in laneclear or lasthit
            if (Variables.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear ||
                Variables.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit)
            {
                if (Variables.spells[SpellSlot.Q].IsEnabledAndReady())
                {
                    //If there are no minions in our AA range.
                    if (
                        !MinionManager.GetMinions(ObjectManager.Player.ServerPosition,
                            Orbwalking.GetRealAutoAttackRange(null)).Any())
                    {
                        if (JinxUtility.IsFishBone())
                        {
                            Variables.spells[SpellSlot.Q].Cast();
                        }
                    }
                }
                else
                {
                    if (JinxUtility.IsFishBone())
                    {
                        Variables.spells[SpellSlot.Q].Cast();
                    }
                }
                
            }
        }
    
        internal static void BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (Variables.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear ||
                Variables.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit)
            {
                //If we have the switch to always go minigun in LC or LH mode then switch and return.
                    if (MenuExtensions.GetItemValue<bool>("iseriesr.jinx.q.switch.lhlc"))
                    {
                        if (JinxUtility.IsFishBone())
                        {
                            Variables.spells[SpellSlot.Q].Cast();
                            return;
                        }
                    }
                //If it's enabled and ready.
                    if (Variables.spells[SpellSlot.Q].IsEnabledAndReady() && ObjectManager.Player.ManaPercent > 30)
                    {
                        var target = args.Target;
                        if (target is Obj_AI_Minion)
                        {
                            //If our target is a minion.
                            var tgMinion = target as Obj_AI_Minion;
                            //If there are minions near the one we are AAing and they are at least 3 killable minions.
                            if (GameObjects.EnemyMinions.Count(minion => minion.LSDistance(tgMinion) < 150 
                                && minion.Health <= (ObjectManager.Player.LSGetAutoAttackDamage(minion) * 1.1f) * 3) >= 3)
                            {
                                if (!JinxUtility.IsFishBone())
                                {
                                    Variables.spells[SpellSlot.Q].Cast();
                                }
                            }
                            else
                            {
                                //Otherwise we go to minigun.
                                if (JinxUtility.IsFishBone())
                                {
                                    Variables.spells[SpellSlot.Q].Cast();
                                }
                            }
                        }
                    }
            }
        }
    }
}
