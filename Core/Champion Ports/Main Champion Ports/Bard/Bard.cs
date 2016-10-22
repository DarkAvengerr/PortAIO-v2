using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy;

namespace DZBard
{
    class Bard
    {
        public static Menu BardMenu;

        public static Orbwalking.Orbwalker BardOrbwalker { get; set; }
        
        //DO YOU HAVE A MOMENT TO TALK ABOUT DIKTIONARIESS!=!=!=!==??!?!? -Everance 2k15
        public static Dictionary<SpellSlot, Spell> spells = new Dictionary<SpellSlot, Spell>()
        {
            {SpellSlot.Q, new Spell(SpellSlot.Q, 950f)},
            {SpellSlot.W, new Spell(SpellSlot.W, 945f)},
            {SpellSlot.E, new Spell(SpellSlot.E, float.MaxValue)}
        };

        public static float LastMoveC;
        public static int TunnelNetworkID;
        public static Vector3 TunnelEntrance = Vector3.Zero;
        public static Vector3 TunnelExit = Vector3.Zero;


        internal static void OnLoad()
        {
            LoadEvents();
            LoadSpells();
        }

        private static void LoadSpells()
        {
            spells[SpellSlot.Q].SetSkillshot(0.25f, 65f, 1600f, true, SkillshotType.SkillshotLine);
        }

        private static void LoadEvents()
        {
            Game.OnUpdate += Game_OnUpdate;
            GameObject.OnCreate += OnCreate;
            GameObject.OnDelete += OnDelete;
            Orbwalking.BeforeAttack += OnBeforeAttack;
        }

        private static void OnBeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (args.Target.Type == GameObjectType.obj_AI_Minion
                && (BardOrbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                && GetItemValue<bool>("dz191.bard.misc.attackMinions"))
            {
                if (ObjectManager.Player.CountAlliesInRange(GetItemValue<Slider>("dz191.bard.misc.attackMinionRange").Value) > 0)
                {
                    args.Process = false;
                }
            }
        }

        private static void OnDelete(GameObject sender, EventArgs args)
        {
            if (sender.Name.Contains("BardDoor_EntranceMinion") && sender.NetworkId == TunnelNetworkID)
            {
                TunnelNetworkID = -1;
                TunnelEntrance = Vector3.Zero;
                TunnelExit = Vector3.Zero;
            }
        }

        private static void OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name.Contains("BardDoor_EntranceMinion"))
            {
                TunnelNetworkID = sender.NetworkId;
                TunnelEntrance = sender.Position;
            }

            if (sender.Name.Contains("BardDoor_ExitMinion"))
            {
                TunnelExit = sender.Position;
            }
        }

        static void Game_OnUpdate(EventArgs args)
        {
            var ComboTarget = TargetSelector.GetTarget(spells[SpellSlot.Q].Range / 1.3f, TargetSelector.DamageType.Magical);
            switch (BardOrbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    if (spells[SpellSlot.Q].IsReady() && GetItemValue<bool>(string.Format("dz191.bard.{0}.useq", BardOrbwalker.ActiveMode.ToString().ToLower())) &&
                        ComboTarget.IsValidTarget())
                    {
                        HandleQ(ComboTarget);
                    }

                    if (GetItemValue<bool>(string.Format("dz191.bard.{0}.usew", BardOrbwalker.ActiveMode.ToString().ToLower())))
                    {
                        HandleW();
                    }

                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    if (spells[SpellSlot.Q].IsReady() && GetItemValue<bool>(string.Format("dz191.bard.{0}.useq", BardOrbwalker.ActiveMode.ToString().ToLower())) &&
                        ComboTarget.IsValidTarget() && GetItemValue<bool>(string.Format("dz191.bard.qtarget.{0}", ComboTarget.ChampionName.ToLower())))
                    {
                        HandleQ(ComboTarget);
                    }
                    break;
            }

            if (GetItemValue<KeyBind>("dz191.bard.flee.flee").Active)
            {
                DoFlee();
            }
        }

        private static void DoFlee()
        {
            if ((IsOverWall(ObjectManager.Player.ServerPosition, Game.CursorPos) 
                && GetWallLength(ObjectManager.Player.ServerPosition, Game.CursorPos) >= 250f) && (spells[SpellSlot.E].IsReady() 
                || (TunnelNetworkID != -1 
                && (ObjectManager.Player.ServerPosition.Distance(TunnelEntrance) < 250f))))
            {
                MoveToLimited(GetFirstWallPoint(ObjectManager.Player.ServerPosition, Game.CursorPos));
            }
            else
            {
                MoveToLimited(Game.CursorPos);
            }

            if (GetItemValue<bool>("dz191.bard.flee.q"))
            {
                var ComboTarget = TargetSelector.GetTarget(spells[SpellSlot.Q].Range/1.3f,
                    TargetSelector.DamageType.Magical);

                if (spells[SpellSlot.Q].IsReady() &&
                    ComboTarget.IsValidTarget())
                {
                    HandleQ(ComboTarget);
                }
            }

            if (GetItemValue<bool>("dz191.bard.flee.w"))
            {
                if (ObjectManager.Player.CountAlliesInRange(1000f) - 1 < ObjectManager.Player.CountEnemiesInRange(1000f)
                    || (ObjectManager.Player.HealthPercent <= GetItemValue<Slider>("dz191.bard.wtarget.healthpercent").Value && ObjectManager.Player.CountEnemiesInRange(900f) >= 1))
                {
                    var castPosition = ObjectManager.Player.ServerPosition.Extend(Game.CursorPos, 65);
                    spells[SpellSlot.W].Cast(castPosition);
                }
            }

            if (GetItemValue<bool>("dz191.bard.flee.e"))
            {
                var dir = ObjectManager.Player.ServerPosition.To2D() + ObjectManager.Player.Direction.To2D().Perpendicular() * (ObjectManager.Player.BoundingRadius * 2.5f);
                var Extended = Game.CursorPos;
                if (dir.IsWall() && IsOverWall(ObjectManager.Player.ServerPosition, Extended) 
                    && spells[SpellSlot.E].IsReady()
                    && GetWallLength(ObjectManager.Player.ServerPosition, Extended) >= 250f)
                {
                    spells[SpellSlot.E].Cast(Extended);
                }
            }
        }

        private static void HandleQ(AIHeroClient comboTarget)
        {
                var QPrediction = spells[SpellSlot.Q].GetPrediction(comboTarget);
                
                if (QPrediction.Hitchance >= HitChance.High)
                {
                    if (spells[SpellSlot.Q].GetDamage(comboTarget) > comboTarget.Health + 15 && GetItemValue<bool>("dz191.bard.combo.qks"))
                    {
                        spells[SpellSlot.Q].Cast(QPrediction.CastPosition);
                        return;
                    }

                    var QPushDistance = GetItemValue<Slider>("dz191.bard.misc.distance").Value;
                    var QAccuracy = GetItemValue<Slider>("dz191.bard.misc.accuracy").Value;
                    var PlayerPosition = ObjectManager.Player.ServerPosition;

                    var BeamStartPositions = new List<Vector3>()
                    {
                        QPrediction.CastPosition,
                        QPrediction.UnitPosition,
                        comboTarget.ServerPosition,
                        comboTarget.Position
                    };

                    if (comboTarget.IsDashing())
                    {
                        BeamStartPositions.Add(comboTarget.GetDashInfo().EndPos.To3D());
                    }

                    var PositionsList = new List<Vector3>();
                    var CollisionPositions = new List<Vector3>();

                    foreach (var position in BeamStartPositions)
                    {
                        var collisionableObjects = spells[SpellSlot.Q].GetCollision(position.To2D(),
                            new List<Vector2>() {position.Extend(PlayerPosition, -QPushDistance).To2D()});

                        if (collisionableObjects.Any())
                        {
                            if (collisionableObjects.Any(h => h is AIHeroClient) &&
                                (collisionableObjects.All(h => h.IsValidTarget())))
                            {
                                spells[SpellSlot.Q].Cast(QPrediction.CastPosition);
                                break;
                            }

                            for (var i = 0; i < QPushDistance; i += (int) comboTarget.BoundingRadius)
                            {
                                CollisionPositions.Add(position.Extend(PlayerPosition, -i));
                            }
                        }

                        for (var i = 0; i < QPushDistance; i += (int) comboTarget.BoundingRadius)
                        {
                            PositionsList.Add(position.Extend(PlayerPosition, -i));
                        }
                    }

                    if (PositionsList.Any())
                    {
                        //We don't want to divide by 0 Kappa
                        var WallNumber = PositionsList.Count(p => p.IsWall())*1.3f;
                        var CollisionPositionCount = CollisionPositions.Count;
                        var Percent = (WallNumber + CollisionPositionCount)/PositionsList.Count;
                        var AccuracyEx = QAccuracy/100f;
                        if (Percent >= AccuracyEx)
                        {
                            spells[SpellSlot.Q].Cast(QPrediction.CastPosition);
                        }
                        
                    }
                }else if (QPrediction.Hitchance == HitChance.Collision)
                {
                    var QCollision = QPrediction.CollisionObjects;
                    if (QCollision.Count == 1)
                    {
                        spells[SpellSlot.Q].Cast(QPrediction.CastPosition);
                    }
                }
        }


        private static void HandleW()
        {
            if (ObjectManager.Player.IsRecalling() || Shop.IsOpen || !spells[SpellSlot.W].IsReady())
            {
                return;
            }

            if (ObjectManager.Player.HealthPercent <= GetItemValue<Slider>("dz191.bard.wtarget.healthpercent").Value)
            {
                var castPosition = ObjectManager.Player.ServerPosition.Extend(Game.CursorPos, 65);
                spells[SpellSlot.W].Cast(castPosition);
                return;
            }

            var LowHealthAlly = HeroManager.Allies
                .Where(ally => ally.IsValidTarget(spells[SpellSlot.W].Range, false)
                    && ally.HealthPercent <= GetItemValue<Slider>("dz191.bard.wtarget.healthpercent").Value
                    && GetItemValue<bool>(string.Format("dz191.bard.wtarget.{0}", ally.ChampionName.ToLower())))
                //.OrderBy(TargetSelector.GetPriority)
                .OrderBy(ally => ally.Health)
                .FirstOrDefault();

            if (LowHealthAlly != null)
            {
                var movementPrediction = Prediction.GetPrediction(LowHealthAlly, 0.25f);
                spells[SpellSlot.W].Cast(movementPrediction.UnitPosition);
            }
        }


        private static T GetItemValue<T>(string item)
        {
            return BardMenu.Item(item).GetValue<T>();
        }

        private static bool IsOverWall(Vector3 start, Vector3 end)
        {
            double distance = Vector3.Distance(start, end);
            for (uint i = 0; i < distance; i += 10)
            {
                var tempPosition = start.Extend(end, i).To2D();
                if (tempPosition.IsWall())
                {
                    return true;
                }
            }

            return false;
        }

        private static Vector3 GetFirstWallPoint(Vector3 start, Vector3 end)
        {
            double distance = Vector3.Distance(start, end);
            for (uint i = 0; i < distance; i += 10)
            {
                var tempPosition = start.Extend(end, i);
                if (tempPosition.IsWall())
                {
                    return tempPosition.Extend(start, -35);
                }
            }

            return Vector3.Zero;
        }

        private static float GetWallLength(Vector3 start, Vector3 end)
        {
            double distance = Vector3.Distance(start, end);
            var firstPosition = Vector3.Zero;
            var lastPosition = Vector3.Zero;

            for (uint i = 0; i < distance; i += 10)
            {
                var tempPosition = start.Extend(end, i);
                if (tempPosition.IsWall() && firstPosition == Vector3.Zero)
                {
                    firstPosition = tempPosition;
                }
                lastPosition = tempPosition;
                if (!lastPosition.IsWall() && firstPosition != Vector3.Zero)
                {
                    break;
                }
            }

            return Vector3.Distance(firstPosition, lastPosition);
        }

        public static void MoveToLimited(Vector3 where)
        {
            if (Environment.TickCount - LastMoveC < 80)
            {
                return;
            }

            LastMoveC = Environment.TickCount;

            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, where);
        }
    }
}
