using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using VayneHunter_Reborn.Skills.Tumble.VHRQ;
using VayneHunter_Reborn.Utility;
using VayneHunter_Reborn.Utility.Helpers;
using VayneHunter_Reborn.Utility.MenuUtility;
using EloBuddy;

namespace VayneHunter_Reborn.Skills.Tumble
{
    class TumbleMethods
    {
        private static Spell Q
        {
            get { return Variables.spells[SpellSlot.Q]; }
        }

        private static QProvider Provider = new QProvider();

        private static readonly string[] MobNames =
        {
            "SRU_Red", "SRU_Blue", "SRU_Gromp", "SRU_Murkwolf",
            "SRU_Razorbeak", "SRU_Krug", "Sru_Crab",
            "TT_Spiderboss", "TTNGolem", "TTNWolf",
            "TTNWraith"
        };

        public static void PreCastTumble(Obj_AI_Base target)
        {
            if (!target.LSIsValidTarget(ObjectManager.Player.AttackRange + 65f + 65f + 300f))
            {
                return;
            }

            var menuOption =
                Variables.Menu.Item(
                    string.Format("dz191.vhr.{0}.q.2wstacks", Variables.Orbwalker.ActiveMode.ToString().ToLower()));
    
            var TwoWQ = menuOption != null ? menuOption.GetValue<bool>() : false;

            if (target is AIHeroClient)
            {
                var tg = target as AIHeroClient;
                //TargetSelector.SetTarget(tg); //<---- TODO

                if (TwoWQ && (tg.GetWBuff() != null && tg.GetWBuff().Count < 1) && Variables.spells[SpellSlot.W].Level > 0)
                {
                    return;
                }
            }
            var smartQPosition = TumblePositioning.GetSmartQPosition();
            var smartQCheck =  smartQPosition != Vector3.Zero;
            var QPosition = smartQCheck ? smartQPosition : Game.CursorPos;

            OnCastTumble(target, QPosition);
        }

        public static void HandleFarmTumble(Obj_AI_Base target)
        {
            if (MobNames.Contains(target.CharData.BaseSkinName) && MenuExtensions.GetItemValue<bool>("dz191.vhr.farm.qjungle"))
            {
                DefaultQCast(Game.CursorPos, target);
                return;
            }
            
            if (!Variables.spells[SpellSlot.Q].IsEnabledAndReady(Variables.Orbwalker.ActiveMode))
            {
                return;
            }

            var minionsInRange = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, ObjectManager.Player.AttackRange + 65)
                .Where(m => m.Health <= ObjectManager.Player.LSGetAutoAttackDamage(m) + Variables.spells[SpellSlot.Q].GetDamage(m))
                .ToList();

            if (minionsInRange.Count() > 1)
            {
                var firstMinion = minionsInRange.OrderBy(m => m.HealthPercent).First();
                var afterTumblePosition = PlayerHelper.GetAfterTumblePosition(Game.CursorPos);
                if (afterTumblePosition.LSDistance(firstMinion.ServerPosition) <= Orbwalking.GetRealAutoAttackRange(null))
                {
                    DefaultQCast(Game.CursorPos, firstMinion);
                    Variables.Orbwalker.ForceTarget(firstMinion);
                }
            }

        }

        private static void OnCastTumble(Obj_AI_Base target, Vector3 position)
        {
            var afterTumblePosition = ObjectManager.Player.ServerPosition.LSExtend(position, 300f);
            var distanceToTarget = afterTumblePosition.LSDistance(target.ServerPosition, true);
            if ((distanceToTarget < Math.Pow(ObjectManager.Player.AttackRange + 65, 2) && distanceToTarget > 110*110)
                || MenuExtensions.GetItemValue<bool>("dz191.vhr.misc.tumble.qspam"))
            {
                switch (MenuExtensions.GetItemValue<StringList>("dz191.vhr.misc.condemn.qlogic").SelectedIndex)
                {
                    case 0:
                        /**
                        var smartQPosition = TumblePositioning.GetSmartQPosition();
                        var smartQCheck =  smartQPosition != Vector3.Zero;
                        var QPosition = smartQCheck ? smartQPosition : Game.CursorPos;
                        var QPosition2 = Provider.GetQPosition() != Vector3.Zero ? Provider.GetQPosition() : QPosition;
                        

                        DefaultQCast

                        if (!QPosition2.UnderTurret(true) || (QPosition2.UnderTurret(true) && ObjectManager.Player.UnderTurret(true)))
                        {
                             CastQ(QPosition2);
                        }
                         * */

                        if (Variables.MeleeEnemiesTowardsMe.Any() &&
                            !Variables.MeleeEnemiesTowardsMe.All(m => m.HealthPercent <= 15))
                        {
                            var Closest =
                                Variables.MeleeEnemiesTowardsMe.OrderBy(m => m.LSDistance(ObjectManager.Player)).First();
                            var whereToQ = Closest.ServerPosition.LSExtend(
                                ObjectManager.Player.ServerPosition, Closest.LSDistance(ObjectManager.Player) + 300f);

                            if (whereToQ.IsSafe())
                            {
                                CastQ(whereToQ);
                            }
                        }
                        else
                        {
                            DefaultQCast(position, target);
                        }

                        break;
                    case 1:
                        //To mouse
                        DefaultQCast(position, target);
                        break;

                    case 2:
                        //Away from melee enemies
                        if (Variables.MeleeEnemiesTowardsMe.Any() &&
                            !Variables.MeleeEnemiesTowardsMe.All(m => m.HealthPercent <= 15))
                        {
                            var Closest =
                                Variables.MeleeEnemiesTowardsMe.OrderBy(m => m.LSDistance(ObjectManager.Player)).First();
                            var whereToQ = Closest.ServerPosition.LSExtend(
                                ObjectManager.Player.ServerPosition, Closest.LSDistance(ObjectManager.Player) + 300f);

                            if (whereToQ.IsSafe())
                            {
                                CastQ(whereToQ);
                            }
                        }
                        else
                        {
                            DefaultQCast(position, target);
                        }
                        break;
                    case 3:
                        //Credits to Kurisu's Graves!
                        var range = Orbwalking.GetRealAutoAttackRange(target);
                        var path = LeagueSharp.Common.Geometry.LSCircleCircleIntersection(ObjectManager.Player.ServerPosition.LSTo2D(),
                            Prediction.GetPrediction(target, 0.25f).UnitPosition.LSTo2D(), 300f, range);

                        if (path.Count() > 0)
                        {
                            var TumblePosition = path.MinOrDefault(x => x.LSDistance(Game.CursorPos)).To3D();
                            if (!TumblePosition.IsSafe(true))
                            {
                                CastQ(TumblePosition);
                            }
                        }
                        else
                        {
                            DefaultQCast(position, target);
                        }
                        break;
                }
            }
        }

        private static void DefaultQCast(Vector3 position, Obj_AI_Base Target)
        {
            var afterTumblePosition = PlayerHelper.GetAfterTumblePosition(Game.CursorPos);
            var CursorPos = Game.CursorPos;
            var EnemyPoints = TumblePositioning.GetEnemyPoints();
            if (afterTumblePosition.IsSafe(true) || (!EnemyPoints.Contains(Game.CursorPos.LSTo2D())) || (Variables.EnemiesClose.Count() == 1))
            {
                if (afterTumblePosition.LSDistance(Target.ServerPosition) <= Orbwalking.GetRealAutoAttackRange(Target))
                {
                    CastQ(position);
                }
            }
        }

        private static void CastQ(Vector3 Position)
        {
            var endPosition = Position;

            if (MenuExtensions.GetItemValue<bool>("dz191.vhr.mixed.mirinQ"))
            {
                var qBurstModePosition = GetQBurstModePosition();
                if (qBurstModePosition != null)
                {
                    endPosition = (Vector3)qBurstModePosition;
                }
            }
            
            if (Variables.spells[SpellSlot.R].IsEnabledAndReady(Orbwalking.OrbwalkingMode.Combo) && Variables.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (ObjectManager.Player.LSCountEnemiesInRange(750f) >=
                    MenuExtensions.GetItemValue<Slider>("dz191.vhr.combo.r.minenemies").Value)
                {
                    Variables.spells[SpellSlot.R].Cast();
                }
            }

            Q.Cast(endPosition);
        }

        private static Vector3? GetQBurstModePosition()
        {
            var positions =
                GetWallQPositions(70).ToList().OrderBy(pos => pos.LSDistance(ObjectManager.Player.ServerPosition, true));

            foreach (var position in positions)
            {
                if (position.LSIsWall() && position.IsSafe(true))
                {
                    return position;
                }
            }
            
            return null;
        }

        private static Vector3[] GetWallQPositions(float Range)
        {
            Vector3[] vList =
            {
                (ObjectManager.Player.ServerPosition.LSTo2D() + Range * ObjectManager.Player.Direction.LSTo2D()).To3D(),
                (ObjectManager.Player.ServerPosition.LSTo2D() - Range * ObjectManager.Player.Direction.LSTo2D()).To3D()

            };
            
            return vList;
        }
    }
}
