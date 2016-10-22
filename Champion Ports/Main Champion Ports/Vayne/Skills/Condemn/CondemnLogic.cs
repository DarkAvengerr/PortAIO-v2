using System;
using System.Linq;
using iSeriesReborn.Utility.Positioning;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using VayneHunter_Reborn.External.Evade;
using VayneHunter_Reborn.Skills.Condemn.Methods;
using VayneHunter_Reborn.Utility;
using VayneHunter_Reborn.Utility.MenuUtility;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace VayneHunter_Reborn.Skills.Condemn
{
    class CondemnLogic
    {
        private static Spell E
        {
            get { return Variables.spells[SpellSlot.E]; }
        }

        private static readonly Spell TrinketSpell = new Spell(SpellSlot.Trinket);

        public static void OnLoad()
        {
            Variables.spells[SpellSlot.E].SetTargetted(0.375f, float.MaxValue);
            InterrupterGapcloser.OnLoad();
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            Obj_AI_Base.OnProcessSpellCast += WindWall.OnProcessSpellCast;

        }

        public static void Execute(EventArgs args)
        {

            if (!E.IsEnabledAndReady(Variables.Orbwalker.ActiveMode))
            {
                return;
            }

            /**
            var CondemnTarget = GetCondemnTarget(ObjectManager.Player.ServerPosition);
            if (CondemnTarget.IsValidTarget())
            {
               // var AAForE = MenuExtensions.GetItemValue<Slider>("dz191.vhr.misc.condemn.noeaa").Value;

               // if (CondemnTarget.Health / ObjectManager.Player.GetAutoAttackDamage(CondemnTarget, true) < AAForE)
               // {
               //     return;
               // }
                var targetPosition = CondemnTarget.ServerPosition;
                var myPosition = ObjectManager.Player.ServerPosition;
                if (WindWall.CollidesWithWall(myPosition, targetPosition))
                {
                    return;
                }

                E.CastOnUnit(CondemnTarget);
            }*/

            var pushDistance = MenuExtensions.GetItemValue<Slider>("dz191.vhr.misc.condemn.pushdistance").Value - 25;

             if (pushDistance >= 410)
            {
                var PushEx = pushDistance;
                pushDistance -= (10 + (PushEx - 410)/2);
            }
            foreach (var target in HeroManager.Enemies.Where(en => en.IsValidTarget(E.Range) && !en.IsDashing()))
            {
                //Yasuo Windwall check
                if (WindWall.CollidesWithWall(ObjectManager.Player.ServerPosition, target.ServerPosition))
                {
                    continue;
                }

                if (MenuExtensions.GetItemValue<bool>("dz191.vhr.misc.condemn.onlystuncurrent") && Variables.Orbwalker.GetTarget() != null &&
                            !target.NetworkId.Equals(Variables.Orbwalker.GetTarget().NetworkId))
                {
                    continue;
                }

                if (target.Health + 10 <=
                    ObjectManager.Player.GetAutoAttackDamage(target) *
                    MenuExtensions.GetItemValue<Slider>("dz191.vhr.misc.condemn.noeaa").Value)
                {
                    continue;
                }
                var Prediction = Variables.spells[SpellSlot.E].GetPrediction(target);
                var endPosition = Prediction.UnitPosition.Extend(ObjectManager.Player.ServerPosition, -pushDistance);

                if (Prediction.Hitchance >= HitChance.VeryHigh)
                {
                    if (endPosition.IsWall())
                    {
                        var condemnRectangle = new VHRPolygon(VHRPolygon.Rectangle(target.ServerPosition.To2D(), endPosition.To2D(), target.BoundingRadius));

                        if (condemnRectangle.Points
                            .Count(point => NavMesh.GetCollisionFlags(point.X, point.Y).HasFlag(CollisionFlags.Wall)) >= condemnRectangle.Points.Count() * (20 / 100f))
                        {
                            E.CastOnUnit(target);
                            TrinketBush(target.ServerPosition.Extend(ObjectManager.Player.ServerPosition, -450f));
                        }
                    }
                    else
                    {
                        //It's not a wall.
                        var step = pushDistance / 5f;
                        for (float i = 0; i < pushDistance; i += step)
                        {
                            var endPositionEx = Prediction.UnitPosition.Extend(ObjectManager.Player.ServerPosition, -i);
                            if (endPositionEx.IsWall())
                            {
                                var condemnRectangle = new VHRPolygon(VHRPolygon.Rectangle(target.ServerPosition.To2D(), endPosition.To2D(), target.BoundingRadius));
                                
                                if (condemnRectangle.Points
                                    .Count(point => NavMesh.GetCollisionFlags(point.X, point.Y).HasFlag(CollisionFlags.Wall)) >= condemnRectangle.Points.Count() * (20 / 100f))
                                {
                                        E.CastOnUnit(target);
                                        TrinketBush(target.ServerPosition.Extend(ObjectManager.Player.ServerPosition, -450f));

                                }
                                return;
                            }
                        }
                    }
                }
            }
        }

        private static void TrinketBush(Vector3 endPosition)
        {
            if (TrinketSpell.IsReady())
            {
                var extended = ObjectManager.Player.ServerPosition.Extend(endPosition, 400f);
                if (NavMesh.IsWallOfGrass(extended, 130f) && !NavMesh.IsWallOfGrass(ObjectManager.Player.ServerPosition, 65f))
                {
                    LeagueSharp.Common.Utility.DelayAction.Add((int)(Game.Ping / 2f + 250), () =>
                    {
                        TrinketSpell.Cast(extended);
                    });
                }
            }
        }

        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            return;
            
        }

        public static Obj_AI_Base GetCondemnTarget(Vector3 fromPosition)
        {
            switch (MenuExtensions.GetItemValue<StringList>("dz191.vhr.misc.condemn.condemnmethod").SelectedIndex)
            {
                case 0:
                    //VH Revolution
                    return VHRevolution.GetTarget(fromPosition);
                case 1:
                    //VH Reborn
                    return VHReborn.GetTarget(fromPosition);
                case 2:
                    //Marksman / Gosu
                    return Condemn.Methods.Marksman.GetTarget(fromPosition);
                case 3:
                    //Shine#
                    return Shine.GetTarget(fromPosition);
            }
            return null;
        }
    }
}
