// Copyright 2014 - 2014 Esk0r
// Program.cs is part of Evade.
// 
// Evade is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Evade is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Evade. If not, see <http://www.gnu.org/licenses/>.

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Flowers_Riven.Evade
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using Color = System.Drawing.Color;
    using GamePath = System.Collections.Generic.List<SharpDX.Vector2>;

    internal class Program
    {
        public static SpellList<Skillshot> DetectedSkillshots = new SpellList<Skillshot>();
        private static bool _evading;
        public static bool NoSolutionFound;
        public static Vector2 EvadeToPoint;
        public static int LastWardJumpAttempt;
        public static Vector2 PreviousTickPosition;
        public static Vector2 PlayerPosition;
        public static string PlayerChampionName;
        private static readonly Random RandomN = new Random();
        public static bool ForcePathFollowing;

        public static bool Evading
        {
            get
            {
                return _evading;
            }
            set
            {
                if (value)
                {
                    ForcePathFollowing = true;
                }

                _evading = value;
            }
        }

        public static Vector2 EvadePoint { get; set; }

        public static void InjectEvade()
        {
            PlayerChampionName = ObjectManager.Player.ChampionName;

            Config.CreateMenu();

            Game.OnUpdate += Game_OnOnGameUpdate;
            EloBuddy.Player.OnIssueOrder += ObjAiHeroOnOnIssueOrder;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            SkillshotDetector.OnDetectSkillshot += OnDetectSkillshot;
            SkillshotDetector.OnDeleteMissile += SkillshotDetectorOnOnDeleteMissile;
            Drawing.OnDraw += Drawing_OnDraw;
            CustomEvents.Unit.OnDash += UnitOnOnDash;
            DetectedSkillshots.OnAdd += DetectedSkillshots_OnAdd;
            Collision.Init();
        }

        private static void DetectedSkillshots_OnAdd(object sender, EventArgs e)
        {
            Evading = false;
        }

        private static void SkillshotDetectorOnOnDeleteMissile(Skillshot skillshot, MissileClient missile)
        {
            if (skillshot.SpellData.SpellName == "VelkozQ")
            {
                var spellData = SpellDatabase.GetByName("VelkozQSplit");
                var direction = skillshot.Direction.Perpendicular();

                if (DetectedSkillshots.Count(s => s.SpellData.SpellName == "VelkozQSplit") == 0)
                {
                    for (var i = -1; i <= 1; i = i + 2)
                    {
                        var skillshotToAdd = new Skillshot(
                            DetectionType.ProcessSpell, spellData, Utils.TickCount, missile.Position.To2D(),
                            missile.Position.To2D() + i * direction * spellData.Range, skillshot.Unit);

                        DetectedSkillshots.Add(skillshotToAdd);
                    }
                }
            }
        }

        private static void OnDetectSkillshot(Skillshot skillshot)
        {
            var alreadyAdded = false;

            if (Config.EvadeMenu.Item("DisableFow").GetValue<bool>() && !skillshot.Unit.IsHPBarRendered)
            {
                return;
            }
                
            foreach (var item in DetectedSkillshots)
            {
                if (item.SpellData.SpellName == skillshot.SpellData.SpellName &&
                    item.Unit.NetworkId == skillshot.Unit.NetworkId && 
                    skillshot.Direction.AngleBetween(item.Direction) < 5 &&
                    (skillshot.Start.Distance(item.Start) < 100 || skillshot.SpellData.FromObjects.Length == 0))
                {
                    alreadyAdded = true;
                }
            }

            if (skillshot.Unit.Team == ObjectManager.Player.Team)
            {
                return;
            }

            if (skillshot.Start.Distance(PlayerPosition) >
                (skillshot.SpellData.Range + skillshot.SpellData.Radius + 1000) * 1.5)
            {
                return;
            }

            if (!alreadyAdded || skillshot.SpellData.DontCheckForDuplicates)
            {
                if (skillshot.DetectionType == DetectionType.ProcessSpell)
                {
                    if (skillshot.SpellData.MultipleNumber != -1)
                    {
                        var originalDirection = skillshot.Direction;

                        for (var i = -(skillshot.SpellData.MultipleNumber - 1) / 2;
                            i <= (skillshot.SpellData.MultipleNumber - 1) / 2;
                            i++)
                        {
                            var end = skillshot.Start +
                                      skillshot.SpellData.Range *
                                      originalDirection.Rotated(skillshot.SpellData.MultipleAngle * i);
                            var skillshotToAdd = new Skillshot(
                                skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, skillshot.Start, end,
                                skillshot.Unit);

                            DetectedSkillshots.Add(skillshotToAdd);
                        }

                        return;
                    }

                    if (skillshot.SpellData.SpellName == "UFSlash")
                    {
                        skillshot.SpellData.MissileSpeed = 1600 + (int) skillshot.Unit.MoveSpeed;
                    }

                    if (skillshot.SpellData.SpellName == "SionR")
                    {
                        skillshot.SpellData.MissileSpeed = (int)skillshot.Unit.MoveSpeed;
                    }

                    if (skillshot.SpellData.Invert)
                    {
                        var newDirection = -(skillshot.End - skillshot.Start).Normalized();
                        var end = skillshot.Start + newDirection * skillshot.Start.Distance(skillshot.End);
                        var skillshotToAdd = new Skillshot(
                            skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, skillshot.Start, end,
                            skillshot.Unit);
                        DetectedSkillshots.Add(skillshotToAdd);
                        return;
                    }

                    if (skillshot.SpellData.Centered)
                    {
                        var start = skillshot.Start - skillshot.Direction * skillshot.SpellData.Range;
                        var end = skillshot.Start + skillshot.Direction * skillshot.SpellData.Range;
                        var skillshotToAdd = new Skillshot(
                            skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, start, end,
                            skillshot.Unit);
                        DetectedSkillshots.Add(skillshotToAdd);
                        return;
                    }

                    var objAiHero = skillshot.Unit as AIHeroClient;

                    if (objAiHero != null && skillshot.SpellData.SpellName == "TaricE" && objAiHero.ChampionName == "Taric")
                    {
                        var target = HeroManager.AllHeroes
                            .FirstOrDefault(h => h.Team == skillshot.Unit.Team &&
                            h.IsHPBarRendered && h.HasBuff("taricwleashactive"));

                        if (target != null)
                        {
                            var start = target.ServerPosition.To2D();
                            var direction = (skillshot.OriginalEnd - start).Normalized();
                            var end = start + direction * skillshot.SpellData.Range;
                            var skillshotToAdd = new Skillshot(
                                    skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick,
                                    start, end, target)
                            {
                                OriginalEnd = skillshot.OriginalEnd
                            };

                            DetectedSkillshots.Add(skillshotToAdd);
                        }
                    }

                    switch (skillshot.SpellData.SpellName)
                    {
                        case "SyndraE":
                        case "syndrae5":
                            var angle = 60;
                            var edge1 =
                                (skillshot.End - skillshot.Unit.ServerPosition.To2D()).Rotated(
                                    -angle / 2 * (float) Math.PI / 180);
                            var edge2 = edge1.Rotated(angle * (float) Math.PI / 180);
                            var positions = new List<Vector2>();
                            var explodingQ = DetectedSkillshots.FirstOrDefault(s => s.SpellData.SpellName == "SyndraQ");

                            if (explodingQ != null)
                            {
                                positions.Add(explodingQ.End);
                            }

                            foreach (var minion in ObjectManager.Get<Obj_AI_Minion>())
                            {
                                if (minion.Name == "Seed" && !minion.IsDead && (minion.Team != ObjectManager.Player.Team))
                                {
                                    positions.Add(minion.ServerPosition.To2D());
                                }
                            }

                            foreach (var position in positions)
                            {
                                var v = position - skillshot.Unit.ServerPosition.To2D();
                                if (edge1.CrossProduct(v) > 0 && v.CrossProduct(edge2) > 0 &&
                                    position.Distance(skillshot.Unit) < 800 )
                                {
                                    var start = position;
                                    var end = skillshot.Unit.ServerPosition.To2D()
                                        .Extend(
                                            position,
                                            skillshot.Unit.Distance(position) > 200 ? 1300 : 1000);

                                    var startTime = skillshot.StartTick;

                                    startTime += (int)(150 + skillshot.Unit.Distance(position) / 2.5f);
                                    var skillshotToAdd = new Skillshot(
                                        skillshot.DetectionType, skillshot.SpellData, startTime, start, end,
                                        skillshot.Unit);

                                    DetectedSkillshots.Add(skillshotToAdd);
                                }
                            }
                            return;
                        case "MalzaharQ":
                        {
                            var start = skillshot.End - skillshot.Direction.Perpendicular() * 400;
                            var end = skillshot.End + skillshot.Direction.Perpendicular() * 400;
                            var skillshotToAdd = new Skillshot(
                                skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, start, end,
                                skillshot.Unit);

                            DetectedSkillshots.Add(skillshotToAdd);
                            return;
                        }
                        case "ZyraQ":
                        {
                            var start = skillshot.End - skillshot.Direction.Perpendicular() * 450;
                            var end = skillshot.End + skillshot.Direction.Perpendicular() * 450;
                            var skillshotToAdd = new Skillshot(
                                skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, start, end,
                                skillshot.Unit);

                            DetectedSkillshots.Add(skillshotToAdd);
                            return;
                        }
                        case "DianaArc":
                        {
                            var skillshotToAdd = new Skillshot(
                                skillshot.DetectionType, SpellDatabase.GetByName("DianaArcArc"), skillshot.StartTick, skillshot.Start, skillshot.End,
                                skillshot.Unit);

                            DetectedSkillshots.Add(skillshotToAdd);
                        }
                            break;
                    }

                    if (skillshot.SpellData.SpellName == "ZiggsQ")
                    {
                        var d1 = skillshot.Start.Distance(skillshot.End);
                        var d2 = d1 * 0.4f;
                        var d3 = d2 * 0.69f;
                        var bounce1SpellData = SpellDatabase.GetByName("ZiggsQBounce1");
                        var bounce2SpellData = SpellDatabase.GetByName("ZiggsQBounce2");
                        var bounce1Pos = skillshot.End + skillshot.Direction * d2;
                        var bounce2Pos = bounce1Pos + skillshot.Direction * d3;

                        bounce1SpellData.Delay =
                            (int) (skillshot.SpellData.Delay + d1 * 1000f / skillshot.SpellData.MissileSpeed + 500);
                        bounce2SpellData.Delay =
                            (int) (bounce1SpellData.Delay + d2 * 1000f / bounce1SpellData.MissileSpeed + 500);

                        var bounce1 = new Skillshot(
                            skillshot.DetectionType, bounce1SpellData, skillshot.StartTick, skillshot.End, bounce1Pos,
                            skillshot.Unit);
                        var bounce2 = new Skillshot(
                            skillshot.DetectionType, bounce2SpellData, skillshot.StartTick, bounce1Pos, bounce2Pos,
                            skillshot.Unit);

                        DetectedSkillshots.Add(bounce1);
                        DetectedSkillshots.Add(bounce2);
                    }

                    if (skillshot.SpellData.SpellName == "ZiggsR")
                    {
                        skillshot.SpellData.Delay =
                            (int) (1500 + 1500 * skillshot.End.Distance(skillshot.Start) / skillshot.SpellData.Range);
                    }

                    if (skillshot.SpellData.SpellName == "JarvanIVDragonStrike")
                    {
                        var endPos = new Vector2();

                        foreach (var s in DetectedSkillshots)
                        {
                            if (s.Unit.NetworkId == skillshot.Unit.NetworkId && s.SpellData.Slot == SpellSlot.E)
                            {
                                var extendedE = new Skillshot(
                                    skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, skillshot.Start,
                                    skillshot.End + skillshot.Direction * 100, skillshot.Unit);
                                if (!extendedE.IsSafe(s.End))
                                {
                                    endPos = s.End;
                                }
                                break;
                            }
                        }

                        foreach (var m in ObjectManager.Get<Obj_AI_Minion>())
                        {
                            if (m.CharData.BaseSkinName == "jarvanivstandard" && m.Team == skillshot.Unit.Team)
                            {                    
                                var extendedE = new Skillshot(
                                    skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, skillshot.Start,
                                    skillshot.End + skillshot.Direction * 100, skillshot.Unit);

                                if (!extendedE.IsSafe(m.Position.To2D()))
                                {
                                    endPos = m.Position.To2D();
                                }
                                break;
                            }
                        }

                        if (endPos.IsValid())
                        {
                            skillshot = new Skillshot(DetectionType.ProcessSpell, SpellDatabase.GetByName("JarvanIVEQ"), Utils.TickCount, skillshot.Start, endPos, skillshot.Unit);
                            skillshot.End = endPos + 200 * (endPos - skillshot.Start).Normalized();
                            skillshot.Direction = (skillshot.End - skillshot.Start).Normalized();
                        }
                    }
                }

                if (skillshot.SpellData.SpellName == "OriannasQ")
                {
                    var skillshotToAdd = new Skillshot(
                        skillshot.DetectionType, SpellDatabase.GetByName("OriannaQend"), skillshot.StartTick, skillshot.Start, skillshot.End,
                        skillshot.Unit);

                    DetectedSkillshots.Add(skillshotToAdd);
                }

                if (skillshot.SpellData.DisableFowDetection && skillshot.DetectionType == DetectionType.RecvPacket)
                {
                    return;
                }

                DetectedSkillshots.Add(skillshot);
            }
        }

        private static void Game_OnOnGameUpdate(EventArgs args)
        {
            PlayerPosition = ObjectManager.Player.ServerPosition.To2D();

            if (PreviousTickPosition.IsValid() &&
                PlayerPosition.Distance(PreviousTickPosition) > 200)
            {
                Evading = false;
                EvadeToPoint = Vector2.Zero;
            }

            PreviousTickPosition = PlayerPosition;

            DetectedSkillshots.RemoveAll(skillshot => !skillshot.IsActive());

            foreach (var skillshot in DetectedSkillshots)
            {
                skillshot.Game_OnGameUpdate();
            }

            if (!Config.EvadeMenu.Item("Enabled").GetValue<KeyBind>().Active)
            {
                Evading = false;
                return;
            }

            if (PlayerChampionName == "Olaf" && Config.EvadeMenu.Item("DisableEvadeForOlafR").GetValue<bool>() && ObjectManager.Player.HasBuff("OlafRagnarok"))
            {
                Evading = false;
                return;
            }

            if (ObjectManager.Player.IsDead)
            {
                Evading = false;
                EvadeToPoint = Vector2.Zero;
                return;
            }

            if (ObjectManager.Player.IsCastingInterruptableSpell(true))
            {
                Evading = false;
                EvadeToPoint = Vector2.Zero;
                return;
            }

            if (ObjectManager.Player.Spellbook.IsAutoAttacking && !Orbwalking.IsAutoAttack(ObjectManager.Player.LastCastedSpellName()))
            {
                Evading = false;
                return;
            }

            if (Utils.ImmobileTime(ObjectManager.Player) - Utils.TickCount > Game.Ping / 2 + 70)
            {
                Evading = false;
                return;
            }

            if (ObjectManager.Player.IsDashing())
            {
                Evading = false;
                return;
            }

            if (PlayerChampionName == "Sion" && ObjectManager.Player.HasBuff("SionR"))
            {
                return;
            }

            foreach (var ally in ObjectManager.Get<AIHeroClient>())
            {
                if (ally.IsValidTarget(1000, false))
                {
                    var shieldAlly = Config.EvadeMenu.Item("shield" + ally.ChampionName);
                    if (shieldAlly != null && shieldAlly.GetValue<bool>())
                    {
                        var allySafeResult = IsSafe(ally.ServerPosition.To2D());

                        if (!allySafeResult.IsSafe)
                        {
                            var dangerLevel = 0;

                            foreach (var skillshot in allySafeResult.SkillshotList)
                            {
                                dangerLevel = Math.Max(dangerLevel, skillshot.GetValue<Slider>("DangerLevel").Value);
                            }

                            foreach (var evadeSpell in EvadeSpellDatabase.Spells)
                            {
                                if (evadeSpell.IsShield && evadeSpell.CanShieldAllies &&
                                    ally.Distance(ObjectManager.Player.ServerPosition) < evadeSpell.MaxRange &&
                                    dangerLevel >= evadeSpell.DangerLevel &&
                                    ObjectManager.Player.Spellbook.CanUseSpell(evadeSpell.Slot) == SpellState.Ready &&
                                    IsAboutToHit(ally, evadeSpell.Delay))
                                {
                                    ObjectManager.Player.Spellbook.CastSpell(evadeSpell.Slot, ally);
                                }
                            }
                        }
                    }
                }
            }

            var currentPath = ObjectManager.Player.GetWaypoints();
            var safeResult = IsSafe(PlayerPosition);
            var safePath = IsSafePath(currentPath, 100);
            
            NoSolutionFound = false;
            
            if (Evading && IsSafe(EvadePoint).IsSafe)
            {
                if (safeResult.IsSafe)
                {
                    Evading = false;
                }
            }
            else if (Evading)
            {
                Evading = false;
            }

            if (!safePath.IsSafe)
            {
                if (!safeResult.IsSafe)
                {
                    TryToEvade(safeResult.SkillshotList, EvadeToPoint.IsValid() ? EvadeToPoint : Game.CursorPos.To2D());
                }
            }
        }

        public static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (sender.Owner.IsValid && sender.Owner.IsMe)
            {
                if (args.Slot == SpellSlot.Recall)
                {
                    EvadeToPoint = new Vector2();
                }

                if (Evading)
                {
                    var blockLevel = Config.EvadeMenu.Item("BlockSpells").GetValue<StringList>().SelectedIndex;

                    if (blockLevel == 0)
                    {
                        return;
                    }

                    var isDangerous = false;

                    foreach (var skillshot in DetectedSkillshots)
                    {
                        if (skillshot.Evade() && skillshot.IsDanger(PlayerPosition))
                        {
                            isDangerous = skillshot.GetValue<bool>("IsDangerous");
                            if (isDangerous)
                            {
                                break;
                            }
                        }
                    }

                    if (blockLevel == 1 && !isDangerous)
                    {
                        return;
                    }

                    args.Process = !SpellBlocker.ShouldBlock(args.Slot);
                }
            }
        }

        private static void ObjAiHeroOnOnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if (args.Order == GameObjectOrder.MoveTo || args.Order == GameObjectOrder.AttackTo)
            {
                EvadeToPoint.X = args.TargetPosition.X;
                EvadeToPoint.Y = args.TargetPosition.Y;
            }
            else
            {
                EvadeToPoint = Vector2.Zero;
            }

            if (DetectedSkillshots.Count == 0)
            {
                ForcePathFollowing = false;
            }
            
            if (NoSolutionFound)
            {
                return;
            }

            if (!Config.EvadeMenu.Item("Enabled").GetValue<KeyBind>().Active)
            {
                return;
            }

            if (EvadeSpellDatabase.Spells.Any(evadeSpell => evadeSpell.Name == "Walking" && !evadeSpell.Enabled))
            {
                return;
            }

            var myPath =
                ObjectManager.Player.GetPath(
                    new Vector3(args.TargetPosition.X, args.TargetPosition.Y, ObjectManager.Player.ServerPosition.Z)).To2DList();
            var safeResult = IsSafe(PlayerPosition);

            if (Evading || !safeResult.IsSafe)
            {
                var rcSafePath = IsSafePath(myPath, Config.EvadingRouteChangeTimeOffset);
                if (args.Order == GameObjectOrder.MoveTo)
                {
                    var willMove = false;

                    if (Evading &&
                        Utils.TickCount - Config.LastEvadePointChangeT > Config.EvadePointChangeInterval)
                    {
                        var points = Evader.GetEvadePoints(-1, 0, false, true);

                        if (points.Count > 0)
                        {
                            var to = new Vector2(args.TargetPosition.X, args.TargetPosition.Y);
                            EvadePoint = to.Closest(points);
                            Evading = true;
                            Config.LastEvadePointChangeT = Utils.TickCount;
                            willMove = true;
                        }
                    }

                    if (rcSafePath.IsSafe && IsSafe(myPath[myPath.Count - 1]).IsSafe && args.Order == GameObjectOrder.MoveTo)
                    {
                        EvadePoint = myPath[myPath.Count - 1];
                        Evading = true;
                        willMove = true;
                    }

                    if (!willMove)
                    {
                        ForcePathFollowing = true;
                    }
                }

                args.Process = false;
                return;
            }

            var safePath = IsSafePath(myPath, Config.CrossingTimeOffset);

            if (!safePath.IsSafe && args.Order != GameObjectOrder.AttackUnit)
            {
                if (safePath.Intersection.Valid)
                {
                    if (ObjectManager.Player.Distance(safePath.Intersection.Point) > 75)
                    {
                        ForcePathFollowing = true;
                    }
                }

                ForcePathFollowing = true;
                args.Process = false;
            }

            if (!safePath.IsSafe && args.Order == GameObjectOrder.AttackUnit)
            {
                var target = args.Target;

                if (target != null && target.IsValid<Obj_AI_Base>() && target.IsVisible)
                {
                    if (PlayerPosition.Distance(((Obj_AI_Base)target).ServerPosition) >
                        ObjectManager.Player.AttackRange + ObjectManager.Player.BoundingRadius +
                        target.BoundingRadius)
                    {
                        args.Process = false;
                    }
                }
            }
        }

        private static void UnitOnOnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            if (sender.IsMe)
            {
                EvadeToPoint = args.EndPos;
            }
        }

        public static IsSafeResult IsSafe(Vector2 point)
        {
            var result = new IsSafeResult {SkillshotList = new List<Skillshot>()};

            foreach (var skillshot in DetectedSkillshots)
            {
                if (skillshot.Evade() && skillshot.IsDanger(point))
                {
                    result.SkillshotList.Add(skillshot);
                }
            }

            result.IsSafe = (result.SkillshotList.Count == 0);

            return result;
        }

        public static SafePathResult IsSafePath(GamePath path, int timeOffset, int speed = -1, int delay = 0, Obj_AI_Base unit = null)
        {
            var IsSafe = true;
            var intersections = new List<FoundIntersection>();
            var intersection = new FoundIntersection();

            foreach (var skillshot in DetectedSkillshots)
            {
                if (skillshot.Evade())
                {
                    var sResult = skillshot.IsSafePath(path, timeOffset, speed, delay, unit);
                    IsSafe = IsSafe && sResult.IsSafe;

                    if (sResult.Intersection.Valid)
                    {
                        intersections.Add(sResult.Intersection);
                    }
                }
            }

            if (!IsSafe)
            {
                var intersetion = intersections.MinOrDefault(o => o.Distance);

                return new SafePathResult(false, intersetion.Valid ? intersetion : intersection);
            }

            return new SafePathResult(true, intersection);
        }

        public static bool IsSafeToBlink(Vector2 point, int timeOffset, int delay)
        {
            foreach (var skillshot in DetectedSkillshots)
            {
                if (skillshot.Evade())
                {
                    if (!skillshot.IsSafeToBlink(point, timeOffset, delay))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool IsAboutToHit(Obj_AI_Base unit, int time)
        {
            time += 150;

            foreach (var skillshot in DetectedSkillshots)
            {
                if (skillshot.Evade())
                {
                    if (skillshot.IsAboutToHit(time, unit))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static void TryToEvade(List<Skillshot> HitBy, Vector2 to)
        {
            var dangerLevel = 0;

            foreach (var skillshot in HitBy)
            {
                dangerLevel = Math.Max(dangerLevel, skillshot.GetValue<Slider>("DangerLevel").Value);
            }

            foreach (var evadeSpell in EvadeSpellDatabase.Spells)
            {
                if (evadeSpell.Enabled && evadeSpell.DangerLevel <= dangerLevel)
                {
                    if (evadeSpell.IsSpellShield &&
                        ObjectManager.Player.Spellbook.CanUseSpell(evadeSpell.Slot) == SpellState.Ready)
                    {
                        if (IsAboutToHit(ObjectManager.Player, evadeSpell.Delay))
                        {
                            ObjectManager.Player.Spellbook.CastSpell(evadeSpell.Slot, ObjectManager.Player);
                        }

                        NoSolutionFound = true;
                        return;
                    }

                    if (evadeSpell.IsReady())
                    {
                        if (evadeSpell.IsMovementSpeedBuff)
                        {
                            var points = Evader.GetEvadePoints((int) evadeSpell.MoveSpeedTotalAmount());

                            if (points.Count > 0)
                            {
                                EvadePoint = to.Closest(points);
                                Evading = true;

                                if (evadeSpell.IsSummonerSpell)
                                {
                                    ObjectManager.Player.Spellbook.CastSpell(
                                        evadeSpell.Slot, ObjectManager.Player);
                                }
                                else
                                {
                                    ObjectManager.Player.Spellbook.CastSpell(evadeSpell.Slot, ObjectManager.Player);
                                }

                                return;
                            }
                        }

                        if (evadeSpell.IsDash)
                        {
                            if (evadeSpell.IsTargetted)
                            {
                                var targets = Evader.GetEvadeTargets(
                                    evadeSpell.ValidTargets, evadeSpell.Speed, evadeSpell.Delay, evadeSpell.MaxRange);

                                if (targets.Count > 0)
                                {
                                    var closestTarget = Utils.Closest(targets, to);
                                    EvadePoint = closestTarget.ServerPosition.To2D();
                                    Evading = true;

                                    if (evadeSpell.IsSummonerSpell)
                                    {
                                        ObjectManager.Player.Spellbook.CastSpell(evadeSpell.Slot, closestTarget);
                                    }
                                    else
                                    {
                                        ObjectManager.Player.Spellbook.CastSpell(evadeSpell.Slot, closestTarget);
                                    }

                                    return;
                                }
                                if (Utils.TickCount - LastWardJumpAttempt < 250)
                                {
                                    NoSolutionFound = true;
                                    return;
                                }
                            }
                            else
                            {
                                var points = Evader.GetEvadePoints(evadeSpell.Speed, evadeSpell.Delay);

                                points.RemoveAll(
                                    item => item.Distance(ObjectManager.Player.ServerPosition) > evadeSpell.MaxRange);

                                if (evadeSpell.FixedRange)
                                {
                                    for (var i = 0; i < points.Count; i++)
                                    {
                                        points[i] = PlayerPosition
                                            .Extend(points[i], evadeSpell.MaxRange);
                                    }

                                    for (var i = points.Count - 1; i > 0; i--)
                                    {
                                        if (!IsSafe(points[i]).IsSafe)
                                        {
                                            points.RemoveAt(i);
                                        }
                                    }
                                }
                                else
                                {
                                    for (var i = 0; i < points.Count; i++)
                                    {
                                        var k =
                                            (int)
                                                (evadeSpell.MaxRange -
                                                 PlayerPosition.Distance(points[i]));
                                        k -= Math.Max(RandomN.Next(k) - 100, 0);
                                        var extended = points[i] +
                                                       k *
                                                       (points[i] - PlayerPosition)
                                                           .Normalized();
                                        if (IsSafe(extended).IsSafe)
                                        {
                                            points[i] = extended;
                                        }
                                    }
                                }

                                if (points.Count > 0)
                                {
                                    EvadePoint = to.Closest(points);
                                    Evading = true;

                                    if (!evadeSpell.Invert)
                                    {
                                        if (evadeSpell.RequiresPreMove)
                                        {
                                            var theSpell = evadeSpell;
                                            LeagueSharp.Common.Utility.DelayAction.Add(
                                                Game.Ping / 2 + 100,
                                                delegate
                                                {
                                                    ObjectManager.Player.Spellbook.CastSpell(
                                                        theSpell.Slot, EvadePoint.To3D());
                                                });
                                        }
                                        else
                                        {
                                            ObjectManager.Player.Spellbook.CastSpell(evadeSpell.Slot, EvadePoint.To3D());
                                        }
                                    }
                                    else
                                    {
                                        var castPoint = PlayerPosition -
                                                        (EvadePoint - PlayerPosition);
                                        ObjectManager.Player.Spellbook.CastSpell(evadeSpell.Slot, castPoint.To3D());
                                    }

                                    return;
                                }
                            }
                        }

                        if (evadeSpell.IsBlink)
                        {
                            if (evadeSpell.IsTargetted)
                            {
                                var targets = Evader.GetEvadeTargets(
                                    evadeSpell.ValidTargets, int.MaxValue, evadeSpell.Delay, evadeSpell.MaxRange, true);

                                if (targets.Count > 0)
                                {
                                    if (IsAboutToHit(ObjectManager.Player, evadeSpell.Delay))
                                    {
                                        var closestTarget = Utils.Closest(targets, to);
                                        EvadePoint = closestTarget.ServerPosition.To2D();
                                        Evading = true;

                                        if (evadeSpell.IsSummonerSpell)
                                        {
                                            ObjectManager.Player.Spellbook.CastSpell(
                                                evadeSpell.Slot, closestTarget);
                                        }
                                        else
                                        {
                                            ObjectManager.Player.Spellbook.CastSpell(evadeSpell.Slot, closestTarget);
                                        }
                                    }

                                    NoSolutionFound = true;
                                    return;
                                }

                                if (Utils.TickCount - LastWardJumpAttempt < 250)
                                {
                                    NoSolutionFound = true;
                                    return;
                                }
                            }
                            else
                            {
                                var points = Evader.GetEvadePoints(int.MaxValue, evadeSpell.Delay, true);

                                points.RemoveAll(
                                    item => item.Distance(ObjectManager.Player.ServerPosition) > evadeSpell.MaxRange);


                                for (var i = 0; i < points.Count; i++)
                                {
                                    var k =
                                        (int)
                                            (evadeSpell.MaxRange -
                                             PlayerPosition.Distance(points[i]));

                                    k = k - new Random(Utils.TickCount).Next(k);
                                    var extended = points[i] +
                                                   k *
                                                   (points[i] - PlayerPosition).Normalized();
                                    if (IsSafe(extended).IsSafe)
                                    {
                                        points[i] = extended;
                                    }
                                }

                                if (points.Count > 0)
                                {
                                    if (IsAboutToHit(ObjectManager.Player, evadeSpell.Delay))
                                    {
                                        EvadePoint = to.Closest(points);
                                        Evading = true;
                                        if (evadeSpell.IsSummonerSpell)
                                        {
                                            ObjectManager.Player.Spellbook.CastSpell(
                                                evadeSpell.Slot, EvadePoint.To3D());
                                        }
                                        else
                                        {
                                            ObjectManager.Player.Spellbook.CastSpell(evadeSpell.Slot, EvadePoint.To3D());
                                        }
                                    }

                                    NoSolutionFound = true;
                                    return;
                                }
                            }
                        }

                        if (evadeSpell.IsInvulnerability)
                        {
                            if (evadeSpell.IsTargetted)
                            {
                                var targets = Evader.GetEvadeTargets(
                                    evadeSpell.ValidTargets, int.MaxValue, 0, evadeSpell.MaxRange, true, false, true);

                                if (targets.Count > 0)
                                {
                                    if (IsAboutToHit(ObjectManager.Player, evadeSpell.Delay))
                                    {
                                        var closestTarget = Utils.Closest(targets, to);
                                        EvadePoint = closestTarget.ServerPosition.To2D();
                                        Evading = true;
                                        ObjectManager.Player.Spellbook.CastSpell(evadeSpell.Slot, closestTarget);
                                    }

                                    NoSolutionFound = true;
                                    return;
                                }
                            }
                            else
                            {
                                if (IsAboutToHit(ObjectManager.Player, evadeSpell.Delay))
                                {
                                    if (evadeSpell.SelfCast)
                                    {
                                        ObjectManager.Player.Spellbook.CastSpell(evadeSpell.Slot);
                                    }
                                    else
                                    {
                                        ObjectManager.Player.Spellbook.CastSpell(
                                            evadeSpell.Slot, ObjectManager.Player.ServerPosition);
                                    }
                                }
                            }

                            NoSolutionFound = true;
                            return;
                        }
                    }

                    if (evadeSpell.IsShield &&
                        ObjectManager.Player.Spellbook.CanUseSpell(evadeSpell.Slot) == SpellState.Ready)
                    {
                        if (IsAboutToHit(ObjectManager.Player, evadeSpell.Delay))
                        {
                            ObjectManager.Player.Spellbook.CastSpell(evadeSpell.Slot, ObjectManager.Player);
                        }

                        NoSolutionFound = true;
                        return;
                    }
                }
            }

            NoSolutionFound = true;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!Config.EvadeMenu.Item("EnableDrawings").GetValue<bool>())
            {
                return;
            }

            if (Config.EvadeMenu.Item("ShowEvadeStatus").GetValue<bool>())
            {
                var heropos = Drawing.WorldToScreen(ObjectManager.Player.Position);

                Drawing.DrawText(heropos.X - 30, heropos.Y + 70, Config.EvadeMenu.Item("Enabled").GetValue<KeyBind>().Active ? Color.Lime : Color.Yellow, "Evade: " + (Config.EvadeMenu.Item("Enabled").GetValue<KeyBind>().Active ? "ON" : "Off"));
            }

            var Border = Config.EvadeMenu.Item("Border").GetValue<Slider>().Value;
            var missileColor = Config.EvadeMenu.Item("MissileColor").GetValue<Color>();

            foreach (var skillshot in DetectedSkillshots)
            {
                skillshot.Draw(
                    skillshot.Evade() && Config.EvadeMenu.Item("Enabled").GetValue<KeyBind>().Active
                        ? Config.EvadeMenu.Item("EnabledColor").GetValue<Color>()
                        : Config.EvadeMenu.Item("DisabledColor").GetValue<Color>(), missileColor, Border);
            }
        }

        public struct IsSafeResult
        {
            public bool IsSafe;
            public List<Skillshot> SkillshotList;
        }
    }
}
