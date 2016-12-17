using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using hCamille.Extensions;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Utilities = hCamille.Extensions.Utilities;

using EloBuddy; 
using LeagueSharp.Common; 
namespace hCamille.Champions
{
    class Camille
    {
        public static string WallBuff => "camilleedashtoggle";
        public static string DashName => "camilleedash";
        public static bool OnWall => ObjectManager.Player.HasBuff(WallBuff) || Spells.E.Instance.Name == "CamilleEDash2";
        public static bool HasQBuff => ObjectManager.Player.HasBuff("CamilleQ");
        public static string UltimateEmitterName => "Indicator_Edge.troy";
        /*
         * TODO
         * Camille_Base_R_cas_sound.troy
         * Camille_Base_R_buf.troy
         * Camille_Base_R_tar.troy
         * Camille_Base_R_tar_tether.troy
         * Camille_Base_R_Indicator_Edge.troy
         * Camille_Base_R_cas.troy      
         */

        public Camille()
        {
            Spells.Initializer();
            Menus.Initializer();

            Game.OnUpdate += CamilleOnUpdate;
            Obj_AI_Base.OnSpellCast += CamilleOnSpellCast;
            EloBuddy.Player.OnIssueOrder += OnIssueOrder;
            Drawing.OnDraw += OnDraw;
            Obj_AI_Base.OnProcessSpellCast += OnProcess;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += OnInterrupt;

        }

        private void OnInterrupt(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender.IsEnemy && args.DangerLevel > Interrupter2.DangerLevel.Medium && Utilities.Enabled("e.interrupt"))
            {
                var result = ObjectManager.Player;
                var rng = Utilities.Slider("wall.search.range");
                var listPoint = new List<Tuple<Vector2, float>>();
                for (var i = 0; i <= 360; i += 1)
                {
                    var cosX = Math.Cos(i * Math.PI / 180);
                    var sinX = Math.Sin(i * Math.PI / 180);
                    var pos1 = new Vector3(
                        (float)(result.Position.X + rng * cosX), (float)(result.Position.Y + rng * sinX),
                        ObjectManager.Player.Position.Z);
                    var time = Utils.TickCount;
                    for (int j = 0; j < rng; j += 100)
                    {
                        var pos = new Vector3(
                            (float)(result.Position.X + j * cosX), (float)(result.Position.Y + j * sinX),
                            ObjectManager.Player.Position.Z);
                        if (NavMesh.GetCollisionFlags(pos).HasFlag(CollisionFlags.Wall) || NavMesh.GetCollisionFlags(pos).HasFlag(CollisionFlags.Building))
                        {
                            if (j != 0)
                            {
                                int left = j - 99, right = j;
                                do
                                {
                                    var middle = (left + right) / 2;
                                    pos = new Vector3(
                                        (float)(result.Position.X + middle * cosX), (float)(result.Position.Y + middle * sinX),
                                        ObjectManager.Player.Position.Z);
                                    if (NavMesh.GetCollisionFlags(pos).HasFlag(CollisionFlags.Wall) || NavMesh.GetCollisionFlags(pos).HasFlag(CollisionFlags.Building))
                                    {
                                        right = middle;
                                    }
                                    else
                                    {
                                        left = middle + 1;
                                    }
                                } while (left < right);
                            }
                            pos1 = pos;
                            time = Utils.TickCount;
                            break;
                        }
                    }

                    listPoint.Add(new Tuple<Vector2, float>(pos1.To2D(), time));
                }
                var target = sender;
                if (!OnWall)
                {
                    for (int i = 0; i < listPoint.Count - 1; i++)
                    {
                        if (listPoint[i].Item1.IsWall() && listPoint[i].Item1.Distance(ObjectManager.Player.Position) < Utilities.Slider("wall.distance.to.enemy")
                             && listPoint[i].Item1.Distance(target.Position) < 500 && target.IsCastingInterruptableSpell(false) && !listPoint[i].Item1.To3D().UnderTurret(true))
                        {
                            Spells.E.Cast(listPoint[i].Item1);
                        }
                    }
                }
            }
        }

        private void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender != null && gapcloser.Sender.IsEnemy && gapcloser.End.Distance(ObjectManager.Player.Position) < 300
                && Utilities.Enabled("e.anti"))
            {
                var result = ObjectManager.Player;
                var rng = Utilities.Slider("wall.search.range");
                var listPoint = new List<Tuple<Vector2, float>>();
                for (var i = 0; i <= 360; i += 1)
                {
                    var cosX = Math.Cos(i * Math.PI / 180);
                    var sinX = Math.Sin(i * Math.PI / 180);
                    var pos1 = new Vector3(
                        (float)(result.Position.X + rng * cosX), (float)(result.Position.Y + rng * sinX),
                        ObjectManager.Player.Position.Z);
                    var time = Utils.TickCount;
                    for (int j = 0; j < rng; j += 100)
                    {
                        var pos = new Vector3(
                            (float)(result.Position.X + j * cosX), (float)(result.Position.Y + j * sinX),
                            ObjectManager.Player.Position.Z);
                        if (NavMesh.GetCollisionFlags(pos).HasFlag(CollisionFlags.Wall) || NavMesh.GetCollisionFlags(pos).HasFlag(CollisionFlags.Building))
                        {
                            if (j != 0)
                            {
                                int left = j - 99, right = j;
                                do
                                {
                                    var middle = (left + right) / 2;
                                    pos = new Vector3(
                                        (float)(result.Position.X + middle * cosX), (float)(result.Position.Y + middle * sinX),
                                        ObjectManager.Player.Position.Z);
                                    if (NavMesh.GetCollisionFlags(pos).HasFlag(CollisionFlags.Wall) || NavMesh.GetCollisionFlags(pos).HasFlag(CollisionFlags.Building))
                                    {
                                        right = middle;
                                    }
                                    else
                                    {
                                        left = middle + 1;
                                    }
                                } while (left < right);
                            }
                            pos1 = pos;
                            time = Utils.TickCount;
                            break;
                        }
                    }

                    listPoint.Add(new Tuple<Vector2, float>(pos1.To2D(), time));
                }
                var target = gapcloser.Sender;
                if (!OnWall)
                {
                    for (int i = 0; i < listPoint.Count - 1; i++)
                    {
                        if (listPoint[i].Item1.IsWall() && listPoint[i].Item1.Distance(ObjectManager.Player.Position) < Utilities.Slider("wall.distance.to.enemy")
                             && listPoint[i].Item1.Distance(gapcloser.Sender.Position) > 400 && !listPoint[i].Item1.To3D().UnderTurret(true))
                        {
                            var i1 = i;
                            var starttick = listPoint[i1].Item2;
                            var startpos = target.ServerPosition.To2D();
                            var speed = target.GetSpell(gapcloser.Slot).SData.MissileMaxSpeed;
                            var pathshit = target.Path.OrderBy(x => starttick + (int)(1000 * (new Vector3(x.X, x.Y, x.Z).
                            Distance(startpos.To3D()) / speed))).FirstOrDefault();

                            var endpos = new Vector3(pathshit.X, pathshit.Y, pathshit.Z);
                            var endtick = starttick + (int)(1000 * (endpos.Distance(startpos.To3D())
                                / speed));
                            var camilleendtic = starttick + (int)(1000 * (listPoint[i].Item1.Distance(ObjectManager.Player.Position)
                                / Spells.E.Speed));

                            if (listPoint[i].Item1.Distance(endpos) < 500 && camilleendtic > endtick)
                            {
                                Spells.E.Cast(listPoint[i].Item1);
                            }
                        }
                    }
                }
            }
        }

        private void OnProcess(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
               
                if (args.SData.Name == "CamilleW")
                {
                    Spells.W.LastCastAttemptT = Environment.TickCount;
                }

                if (args.SData.Name == "CamilleE" || args.SData.Name == "CamilleEDash2")
                {
                    Spells.E.LastCastAttemptT = Environment.TickCount;
                }

                if (args.SData.Name == "CamilleR")
                {
                    Spells.R.LastCastAttemptT = Environment.TickCount;
                }


            }
        }

        private void OnDraw(EventArgs args)
        {
            if (Menus.Config.Item("q.draw").GetValue<Circle>().Active && Spells.Q.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells.Q.Range, Menus.Config.Item("q.draw").GetValue<Circle>().Color);
            }
            if (Menus.Config.Item("w.draw").GetValue<Circle>().Active && Spells.W.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells.W.Range, Menus.Config.Item("w.draw").GetValue<Circle>().Color);
            }
            if (Menus.Config.Item("e.draw").GetValue<Circle>().Active && Spells.E.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells.E.Range, Menus.Config.Item("e.draw").GetValue<Circle>().Color);
            }
            if (Menus.Config.Item("r.draw").GetValue<Circle>().Active && Spells.R.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells.R.Range, Menus.Config.Item("r.draw").GetValue<Circle>().Color);
            }
        }

        private void OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (sender.IsMe && OnWall && Menus.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && 
                Spells.E.IsReady() && args.Order == GameObjectOrder.MoveTo)
            {
                var target = TargetSelector.GetTarget(Utilities.Slider("enemy.search.range"), TargetSelector.DamageType.Physical);
                if (target != null)
                {
                    args.Process = false;
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, target.ServerPosition, false);
                }
                else
                {
                    args.Process = true;
                }
            }
        }

        private void CamilleOnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.IsAutoAttack() && Menus.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo
                && Menus.Config.Item("q.mode").GetValue<StringList>().SelectedIndex == 0 && Spells.Q.IsReady() && Utilities.Enabled("q.combo"))
            {
                Spells.Q.Cast();
            }
        }

        private void CamilleOnUpdate(EventArgs args)
        {
            switch (Menus.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    OnCombo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    OnMixed();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    OnJungle();
                    OnClear();
                    break;
            }

            if (Menus.Config.Item("flee").GetValue<KeyBind>().Active)
            {
                Orbwalking.MoveTo(Game.CursorPos);
                if (Spells.E.IsReady())
                {
                    FleeE();
                }
            }
        }

        private static void OnCombo()
        {
            var target = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Physical);
            if (target != null)
            {
                if (Spells.Q.IsReady() && Utilities.Enabled("q.combo") && target.IsValidTarget(ObjectManager.Player.AttackRange)
                    && Menus.Config.Item("q.mode").GetValue<StringList>().SelectedIndex == 1)
                {
                    Spells.Q.Cast();
                }

                if (Spells.W.IsReady() && Utilities.Enabled("w.combo") && target.IsValidTarget(Spells.W.Range) && 
                    Environment.TickCount - Spells.E.LastCastAttemptT > 1200)
                {
                    switch (Menus.Config.Item("w.mode").GetValue<StringList>().SelectedIndex)
                    {
                        case 0:
                            if (OnWall)
                            {
                                var pred = Spells.W.GetPrediction(target);
                                if (pred.Hitchance >= HitChance.Medium)
                                {
                                    Spells.W.Cast(pred.CastPosition);
                                }
                            }
                            break;
                        case 1:
                            var predx = Spells.W.GetPrediction(target);
                            if (predx.Hitchance >= HitChance.Medium)
                            {
                                Spells.W.Cast(predx.CastPosition);
                            }
                            break;
                    }
                    
                }

                if (Spells.E.IsReady() && Utilities.Enabled("e.combo") && target.IsValidTarget(Utilities.Slider("enemy.search.range")))
                {
                    if (ObjectManager.Player.CountEnemiesInRange(Utilities.Slider("enemy.search.range")) <= Utilities.Slider("max.enemy.count")
                        && Environment.TickCount - Spells.R.LastCastAttemptT > 4000)
                    {
                        UseE();
                    }
                }

                if (Spells.R.IsReady() && Utilities.Enabled("r.combo"))
                {
                    switch (Menus.Config.Item("r.mode").GetValue<StringList>().SelectedIndex)
                    {
                        case 0:
                            if (target.IsValidTarget(Spells.R.Range) && target.HealthPercent < Utilities.Slider("enemy.health.percent") && Utilities.Enabled("r." + target.ChampionName))
                            {
                                Spells.R.CastOnUnit(target);
                            }
                            break;
                        case 1:
                            var selectedtarget = TargetSelector.SelectedTarget;
                            if (selectedtarget != null && selectedtarget.IsValidTarget(Spells.R.Range) && selectedtarget.HealthPercent < Utilities.Slider("enemy.health.percent") && Utilities.Enabled("r." + selectedtarget.ChampionName))
                            {
                                Spells.R.CastOnUnit(selectedtarget);
                            }
                            break;
                    }
                    
                }
            }
            
        }

        private static void UseE()
        {
            var result = ObjectManager.Player;
            var rng = Utilities.Slider("wall.search.range");
            var listPoint = new List<Tuple<Vector2, float>>();
            for (var i = 0; i <= 360; i += 1)
            {
                var cosX = Math.Cos(i * Math.PI / 180);
                var sinX = Math.Sin(i * Math.PI / 180);
                var pos1 = new Vector3(
                    (float)(result.Position.X + rng * cosX), (float)(result.Position.Y + rng * sinX),
                    ObjectManager.Player.Position.Z);
                var time = Utils.TickCount;
                for (int j = 0; j < rng; j += 100)
                {
                    var pos = new Vector3(
                        (float)(result.Position.X + j * cosX), (float)(result.Position.Y + j * sinX),
                        ObjectManager.Player.Position.Z);
                    if (NavMesh.GetCollisionFlags(pos).HasFlag(CollisionFlags.Wall))
                    {
                        if (j != 0)
                        {
                            int left = j - 99, right = j;
                            do
                            {
                                var middle = (left + right) / 2;
                                pos = new Vector3(
                                    (float)(result.Position.X + middle * cosX), (float)(result.Position.Y + middle * sinX),
                                    ObjectManager.Player.Position.Z);
                                if (NavMesh.GetCollisionFlags(pos).HasFlag(CollisionFlags.Wall))
                                {
                                    right = middle;
                                }
                                else
                                {
                                    left = middle + 1;
                                }
                            } while (left < right);
                        }
                        pos1 = pos;
                        time = Utils.TickCount;
                        break;
                    }
                }

                listPoint.Add(new Tuple<Vector2, float>(pos1.To2D(), time));
            }
            var target = TargetSelector.GetTarget(Utilities.Slider("enemy.search.range"), TargetSelector.DamageType.Physical);
            if (target != null && !OnWall)
            {
                for (int i = 0; i < listPoint.Count - 1; i++)
                {
                    if (listPoint[i].Item1.IsWall() && listPoint[i].Item1.Distance(ObjectManager.Player.Position) < Utilities.Slider("wall.distance.to.enemy"))
                    {
                        var i1 = i;
                        var starttick = listPoint[i1].Item2;
                        var startpos = target.ServerPosition.To2D();
                        var speed = target.MoveSpeed;
                        var pathshit = target.Path.OrderBy(x => starttick + (int)(1000 * (new Vector3(x.X, x.Y, x.Z).
                        Distance(startpos.To3D()) / speed))).FirstOrDefault();

                        var endpos = new Vector3(pathshit.X, pathshit.Y, pathshit.Z);
                        var endtick = starttick + (int)(1000 * (endpos.Distance(startpos.To3D())
                            / speed));
                        var camilleendtic = starttick + (int)(1000 * (listPoint[i].Item1.Distance(ObjectManager.Player.Position)
                            / Spells.E.Speed));

                        if (listPoint[i].Item1.Distance(endpos) < 500 && camilleendtic > endtick)
                        {
                            Spells.E.Cast(listPoint[i].Item1);
                        }
                    }
                }
            }
        }

        private static void FleeE()
        {
            var result = ObjectManager.Player;
            var rng = Utilities.Slider("wall.search.range");
            var listPoint = new List<Tuple<Vector2, float>>();
            for (var i = 0; i <= 360; i += 1)
            {
                var cosX = Math.Cos(i * Math.PI / 180);
                var sinX = Math.Sin(i * Math.PI / 180);
                var pos1 = new Vector3(
                    (float)(result.Position.X + rng * cosX), (float)(result.Position.Y + rng * sinX),
                    ObjectManager.Player.Position.Z);
                var time = Utils.TickCount;
                for (int j = 0; j < rng; j += 100)
                {
                    var pos = new Vector3(
                        (float)(result.Position.X + j * cosX), (float)(result.Position.Y + j * sinX),
                        ObjectManager.Player.Position.Z);
                    if (NavMesh.GetCollisionFlags(pos).HasFlag(CollisionFlags.Wall))
                    {
                        if (j != 0)
                        {
                            int left = j - 99, right = j;
                            do
                            {
                                var middle = (left + right) / 2;
                                pos = new Vector3(
                                    (float)(result.Position.X + middle * cosX), (float)(result.Position.Y + middle * sinX),
                                    ObjectManager.Player.Position.Z);
                                if (NavMesh.GetCollisionFlags(pos).HasFlag(CollisionFlags.Wall))
                                {
                                    right = middle;
                                }
                                else
                                {
                                    left = middle + 1;
                                }
                            } while (left < right);
                        }
                        pos1 = pos;
                        time = Utils.TickCount;
                        break;
                    }
                }

                listPoint.Add(new Tuple<Vector2, float>(pos1.To2D(), time));
            }

            if (!OnWall)
            {
                for (int i = 0; i < listPoint.Count - 1; i++)
                {
                    var rectangle = new Geometry.Polygon.Rectangle(ObjectManager.Player.Position, ObjectManager.Player.Position.Extend(Game.CursorPos, Spells.E.Range), Spells.E.Width);
                    if (listPoint[i].Item1.IsWall() && listPoint[i].Item1.Distance(ObjectManager.Player.Position) < Utilities.Slider("wall.distance.to.enemy") && rectangle.IsInside(listPoint[i].Item1))
                    {
                        Spells.E.Cast(listPoint[i].Item1);
                    }
                }
            }

        }

        private static void OnMixed()
        {
            if (ObjectManager.Player.ManaPercent < Utilities.Slider("harass.mana"))
            {
                return;
            }

            var target = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Physical);
            if (target != null)
            {
                if (Spells.Q.IsReady() && Utilities.Enabled("q.combo") && target.IsValidTarget(ObjectManager.Player.AttackRange))
                {
                    Spells.Q.Cast();
                }

                if (Spells.W.IsReady() && Utilities.Enabled("w.combo") && target.IsValidTarget(Spells.W.Range))
                {
                    var pred = Spells.W.GetPrediction(target);
                    if (pred.Hitchance >= HitChance.High)
                    {
                        Spells.W.Cast(pred.CastPosition);
                    }
                }
            }
        }

        private static void OnJungle()
        {
            if (ObjectManager.Player.ManaPercent < Utilities.Slider("jungle.mana"))
            {
                return;
            }

            var mob = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            if (mob == null || mob.Count == 0)
            {
                return;
            }

            if (Spells.Q.IsReady() && Utilities.Enabled("q.jungle") && Spells.Q.IsInRange(mob[0]))
            {
                Spells.Q.Cast();
            }

            if (Spells.W.IsReady() && Utilities.Enabled("w.jungle"))
            {
                Spells.W.Cast(mob[0].Position);
            }
            
        }

        private static void OnClear()
        {
            if (ObjectManager.Player.ManaPercent < Utilities.Slider("clear.mana"))
            {
                return;
            }

            if (Spells.Q.IsReady() && Utilities.Enabled("q.clear"))
            {
                var minions = MinionManager.GetMinions(ObjectManager.Player.Position, 500, MinionTypes.All,
                MinionTeam.Enemy).Count;
                if (minions > 4)
                {
                    Spells.Q.Cast();
                }
            }

            if (Spells.Q.IsReady() && Calculation.HasProtocolOneBuff && Utilities.Enabled("q.clear"))
            {
                var minion = MinionManager.GetMinions(ObjectManager.Player.Position, 500, MinionTypes.All,
                    MinionTeam.Enemy).FirstOrDefault();
                if (minion != null && minion.Health < minion.ProtocolDamage())
                {
                    Spells.Q.Cast();
                }
            }

            if (Spells.Q.IsReady() && Calculation.HasProtocolTwoBuff && Utilities.Enabled("q.clear"))
            {
                var minion = MinionManager.GetMinions(ObjectManager.Player.Position, 500, MinionTypes.All,
                    MinionTeam.Enemy).FirstOrDefault();
                if (minion != null && minion.Health < minion.ProtocolTwoDamage())
                {
                    Spells.Q.Cast();
                }
            }

            if (Spells.W.IsReady() && Utilities.Enabled("w.clear"))
            {
                var minions = MinionManager.GetMinions(ObjectManager.Player.Position, Spells.W.Range, MinionTypes.All,
                MinionTeam.NotAlly);

                var minioncount = Spells.W.GetLineFarmLocation(minions);
                if (minions == null || minions.Count == 0)
                {
                    return;
                }

                if (minioncount.MinionsHit >= Utilities.Slider("min.count"))
                {
                    Spells.W.Cast(minioncount.Position);
                }
            }
        }
    }
}
