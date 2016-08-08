using System;
using System.Collections.Generic;
using System.Linq;
using DZAIO_Reborn.Core;
using DZAIO_Reborn.Helpers;
using DZAIO_Reborn.Helpers.Modules;
using DZAIO_Reborn.Helpers.Positioning;
using DZAIO_Reborn.Plugins.Champions.Bard.Modules;
using DZAIO_Reborn.Plugins.Interface;
using DZLib.Core;
using DZLib.Menu;
using DZLib.MenuExtensions;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace DZAIO_Reborn.Plugins.Champions.Bard
{
    class Bard : IChampion
    {
        public static int TunnelObjectNetworkID;
        public static Vector3 TunnelEntrancePosition = Vector3.Zero;
        public static Vector3 TunnelExitPosition = Vector3.Zero;

        public void OnLoad(Menu menu)
        {
            var comboMenu = new Menu(ObjectManager.Player.ChampionName + ": Combo", "dzaio.champion.bard.combo");
            {
                comboMenu.AddModeMenu(ModesMenuExtensions.Mode.Combo, new[] { SpellSlot.Q, SpellSlot.R }, new[] { true, true });
                comboMenu.AddSlider("dzaio.champion.bard.combo.r.min", "Min Enemies for R", 2, 1, 5);

                menu.AddSubMenu(comboMenu);
            }

            var mixedMenu = new Menu(ObjectManager.Player.ChampionName + ": Mixed", "dzaio.champion.bard.harrass");
            {
                mixedMenu.AddModeMenu(ModesMenuExtensions.Mode.Harrass, new[] { SpellSlot.Q}, new[] { true });
                mixedMenu.AddSlider("dzaio.champion.bard.mixed.mana", "Min Mana % for Harass", 30, 0, 100);
                menu.AddSubMenu(mixedMenu);
            }

            var extraMenu = new Menu(ObjectManager.Player.ChampionName + ": Extra", "dzaio.champion.bard.extra");
            {
                extraMenu.AddBool("dzaio.champion.bard.extra.interrupter", "Interrupter (Q)", true);
                extraMenu.AddBool("dzaio.champion.bard.extra.antigapcloser", "Antigapcloser (Q)", true);
                extraMenu.AddBool("dzaio.champion.bard.extra.autoQ", "Auto Q Stunned / Rooted", true);
                extraMenu.AddBool("dzaio.champion.bard.extra.autoQKS", "Auto Q KS", true);
                extraMenu.AddBool("dzaio.champion.bard.extra.supportmode", "Support Mode", true);
                extraMenu.AddKeybind("dzaio.champion.bard.extra.fleemode", "Flee Mode", new Tuple<uint, KeyBindType>('Z', KeyBindType.Press));
            }

            Variables.Spells[SpellSlot.Q].SetSkillshot(0.25f, 100, 1600, false, SkillshotType.SkillshotLine);
            Variables.Spells[SpellSlot.E].SetSkillshot(0.25f, 60, 1200, true, SkillshotType.SkillshotLine);
        }

        public void RegisterEvents()
        {
            DZInterrupter.OnInterruptableTarget += OnInterrupter;
            DZAntigapcloser.OnEnemyGapcloser += OnGapcloser;
            Orbwalking.BeforeAttack += BeforeAttack;
            GameObject.OnCreate += OnCreateObject;
            GameObject.OnDelete += OnDeleteObject;
        }

        private void OnCreateObject(GameObject sender, System.EventArgs args)
        {
            if (sender.Name.Contains("BardDoor_EntranceMinion"))
            {
                TunnelObjectNetworkID = sender.NetworkId;
                TunnelEntrancePosition = sender.Position;
            }

            if (sender.Name.Contains("BardDoor_ExitMinion"))
            {
                TunnelExitPosition = sender.Position;
            }
        }

        private void OnDeleteObject(GameObject sender, System.EventArgs args)
        {
            if (sender.Name.Contains("BardDoor_EntranceMinion") && sender.NetworkId == TunnelObjectNetworkID)
            {
                TunnelObjectNetworkID = -1;
                TunnelEntrancePosition = Vector3.Zero;
                TunnelExitPosition = Vector3.Zero;
            }
        }

        private void BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {

            if (args.Target.Type == GameObjectType.obj_AI_Minion
            && (Variables.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            && Variables.AssemblyMenu.GetItemValue<bool>("dzaio.champion.bard.extra.supportmode"))
            {
                if (ObjectManager.Player.LSCountAlliesInRange(Variables.AssemblyMenu.GetItemValue<Slider>("dz191.bard.misc.attackMinionRange").Value) > 0)
                {
                    args.Process = false;
                }
            }
        }

        private void OnGapcloser(DZLib.Core.ActiveGapcloser gapcloser)
        {
            if (Variables.AssemblyMenu.GetItemValue<bool>("dzaio.champion.bard.extra.antigapcloser") 
                && Variables.Spells[SpellSlot.Q].LSIsReady()
                && gapcloser.Sender.LSIsValidTarget()
                && gapcloser.End.LSDistance(ObjectManager.Player.ServerPosition) < gapcloser.Start.LSDistance(ObjectManager.Player.ServerPosition))
            {
                HandleQ(gapcloser.Sender);
            }
        }

        private void OnInterrupter(AIHeroClient sender, DZInterrupter.InterruptableTargetEventArgs args)
        {
            if (Variables.AssemblyMenu.GetItemValue<bool>("dzaio.champion.bard.extra.interrupter") 
                && Variables.Spells[SpellSlot.Q].LSIsReady()
                && sender.LSIsValidTarget()
                && args.DangerLevel >= DZInterrupter.DangerLevel.High)
            {
                HandleQ(sender);
            }
        }

        public Dictionary<SpellSlot, Spell> GetSpells()
        {
            return new Dictionary<SpellSlot, Spell>
                      {
                            {SpellSlot.Q, new Spell(SpellSlot.Q, 950f)},
                            {SpellSlot.W, new Spell(SpellSlot.W, 945f)},
                            {SpellSlot.E, new Spell(SpellSlot.E, float.MaxValue)}
                      };
        }

        public List<IModule> GetModules()
        {
            return new List<IModule>()
            {
                new BardAutoQ()
            };
        }

        public void OnTick()
        {
            if (Variables.AssemblyMenu.GetItemValue<KeyBind>("dzaio.champion.bard.extra.fleemode").Active)
            {
                DoFlee();
            }
        }

        private void DoFlee()
        {
            if (!Variables.Spells[SpellSlot.E].LSIsReady() || TunnelObjectNetworkID == -1)
            {
                Orbwalking.MoveTo(Game.CursorPos);
            }

            if (PositioningHelper.DoPositionsCrossWall(ObjectManager.Player.ServerPosition, Game.CursorPos)
                && PositioningHelper.GetWallLength(ObjectManager.Player.ServerPosition,Game.CursorPos) >= 200f)
            {
                Orbwalking.MoveTo(PositioningHelper.GetFirstWallPoint(ObjectManager.Player.ServerPosition, Game.CursorPos));
            }

            //Q

            var ComboTarget = TargetSelector.GetTarget(Variables.Spells[SpellSlot.Q].Range / 1.3f, TargetSelector.DamageType.Magical);

            if (Variables.Spells[SpellSlot.Q].LSIsReady() && ComboTarget.LSIsValidTarget())
            {
                HandleQ(ComboTarget);
            }

            //E

            var dir = ObjectManager.Player.ServerPosition.LSTo2D() + ObjectManager.Player.Direction.LSTo2D().LSPerpendicular() * (ObjectManager.Player.BoundingRadius * 2.5f);
            var Extended = Game.CursorPos;

            if (dir.LSIsWall() 
                 && PositioningHelper.GetWallLength(ObjectManager.Player.ServerPosition, Extended) >= 200f 
                 && PositioningHelper.DoPositionsCrossWall(ObjectManager.Player.ServerPosition, Extended))
            {
                 Variables.Spells[SpellSlot.E].Cast(Extended);
            }
        }

        public void OnCombo()
        {
            var ComboTarget = TargetSelector.GetTarget(Variables.Spells[SpellSlot.Q].Range / 1.3f, TargetSelector.DamageType.Magical);

            if (Variables.Spells[SpellSlot.Q].IsEnabledAndReady(ModesMenuExtensions.Mode.Combo) && ComboTarget.LSIsValidTarget())
            {
                HandleQ(ComboTarget);
            }

            if (Variables.Spells[SpellSlot.W].IsEnabledAndReady(ModesMenuExtensions.Mode.Combo))
            {
                HandleW();
            }
        }

        public void OnMixed()
        {
            var ComboTarget = TargetSelector.GetTarget(Variables.Spells[SpellSlot.Q].Range / 1.3f, TargetSelector.DamageType.Magical);

            if (Variables.Spells[SpellSlot.Q].IsEnabledAndReady(ModesMenuExtensions.Mode.Harrass) && ComboTarget.LSIsValidTarget())
            {
                HandleQ(ComboTarget);
            }

            if (Variables.Spells[SpellSlot.W].IsEnabledAndReady(ModesMenuExtensions.Mode.Harrass))
            {
                HandleW();
            }
        }

        public void OnLastHit()
        { }

        public void OnLaneclear()
        {

        }
        private static void HandleW()
        {
            var HealthPercent = 30f;

            if (ObjectManager.Player.LSIsRecalling() || ObjectManager.Player.LSInShop() || !Variables.Spells[SpellSlot.W].LSIsReady())
            {
                return;
            }

            if (ObjectManager.Player.HealthPercent <= HealthPercent)
            {
                var castPosition = ObjectManager.Player.ServerPosition.LSExtend(Game.CursorPos, ObjectManager.Player.BoundingRadius * 1.3f);
                Variables.Spells[SpellSlot.W].Cast(castPosition);
                return;
            }

            var LHAlly = HeroManager.Allies
                .Where(ally => ally.LSIsValidTarget(Variables.Spells[SpellSlot.W].Range, false)
                    && ally.HealthPercent <= HealthPercent
                    && TargetSelector.GetPriority(ally) > 3)
                .OrderBy(ally => ally.Health)
                .FirstOrDefault();

            if (LHAlly != null)
            {
                var movementPrediction = Prediction.GetPrediction(LHAlly, 0.35f);
                Variables.Spells[SpellSlot.W].Cast(movementPrediction.UnitPosition);
            }
        }

        public void HandleQ(AIHeroClient comboTarget)
        {
            var QPrediction = Variables.Spells[SpellSlot.Q].GetPrediction(comboTarget);

            if (QPrediction.Hitchance >= HitChance.High)
            {
                var QPushDistance = 250;
                var QAccuracy = 20;
                var PlayerPosition = ObjectManager.Player.ServerPosition;

                var BeamStartPositions = new List<Vector3>()
                    {
                        QPrediction.CastPosition,
                        QPrediction.UnitPosition,
                        comboTarget.ServerPosition,
                        comboTarget.Position
                    };

                if (comboTarget.LSIsDashing())
                {
                    BeamStartPositions.Add(comboTarget.GetDashInfo().EndPos.To3D());
                }

                var PositionsList = new List<Vector3>();
                var CollisionPositions = new List<Vector3>();

                foreach (var position in BeamStartPositions)
                {
                    var collisionableObjects = Variables.Spells[SpellSlot.Q].GetCollision(position.LSTo2D(),
                        new List<Vector2>() { position.LSExtend(PlayerPosition, -QPushDistance).LSTo2D() });

                    if (collisionableObjects.Any())
                    {
                        if (collisionableObjects.Any(h => h is AIHeroClient) &&
                            (collisionableObjects.All(h => h.LSIsValidTarget())))
                        {
                            Variables.Spells[SpellSlot.Q].Cast(QPrediction.CastPosition);
                            break;
                        }

                        for (var i = 0; i < QPushDistance; i += (int)comboTarget.BoundingRadius)
                        {
                            CollisionPositions.Add(position.LSExtend(PlayerPosition, -i));
                        }
                    }

                    for (var i = 0; i < QPushDistance; i += (int)comboTarget.BoundingRadius)
                    {
                        PositionsList.Add(position.LSExtend(PlayerPosition, -i));
                    }
                }

                if (PositionsList.Any())
                {
                    //We don't want to divide by 0 Kappa
                    var WallNumber = PositionsList.Count(p => p.LSIsWall()) * 1.3f;
                    var CollisionPositionCount = CollisionPositions.Count;
                    var Percent = (WallNumber + CollisionPositionCount) / PositionsList.Count;
                    var AccuracyEx = QAccuracy / 100f;
                    if (Percent >= AccuracyEx)
                    {
                        Variables.Spells[SpellSlot.Q].Cast(QPrediction.CastPosition);
                    }

                }
            }
            else if (QPrediction.Hitchance == HitChance.Collision)
            {
                var QCollision = QPrediction.CollisionObjects;
                if (QCollision.Count == 1)
                {
                    Variables.Spells[SpellSlot.Q].Cast(QPrediction.CastPosition);
                }
            }
        }
    }
}
