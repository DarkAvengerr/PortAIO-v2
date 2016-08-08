using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;
using EloBuddy;

namespace OneKeyToWin_AIO_Sebby
{
    class Vayne
    {
        private Menu Config = Program.Config;
        public static SebbyLib.Orbwalking.Orbwalker Orbwalker = Program.Orbwalker;
        private Spell E, Q, R, W;
        private float QMANA = 0, WMANA = 0, EMANA = 0, RMANA = 0;
        public AIHeroClient Player { get { return ObjectManager.Player; } }
        public static Core.OKTWdash Dash;

        public void LoadOKTW()
        {
            Q = new Spell(SpellSlot.Q, 300);
            E = new Spell(SpellSlot.E, 670);
            W = new Spell(SpellSlot.E, 670);
            R = new Spell(SpellSlot.R, 3000);

            E.SetTargetted(0.25f, 2200f);

            LoadMenuOKTW();

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            SebbyLib.Orbwalking.BeforeAttack += BeforeAttack;
            SebbyLib.Orbwalking.AfterAttack += afterAttack;
            Interrupter2.OnInterruptableTarget +=Interrupter2_OnInterruptableTarget;
            //Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        private void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (E.LSIsReady() && sender.LSIsValidTarget(E.Range))
                E.Cast(sender);
        }
        private void LoadMenuOKTW()
        {
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("onlyRdy", "Draw only ready spells", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("qRange", "Q range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("eRange2", "E push position", true).SetValue(false));

            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("autoQ", "Auto Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("Qstack", "Q at X stack", true).SetValue(new Slider(2, 2, 1)));
            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("QE", "try Q + E ", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("Qonly", "Q only after AA", true).SetValue(false));
            Dash = new Core.OKTWdash(Q);

            Config.SubMenu(Player.ChampionName).SubMenu("E Config").SubMenu("GapCloser").AddItem(new MenuItem("gapE", "Enable", true).SetValue(true));
            foreach (var enemy in HeroManager.Enemies)
                Config.SubMenu(Player.ChampionName).SubMenu("E Config").SubMenu("GapCloser").AddItem(new MenuItem("gap" + enemy.ChampionName, enemy.ChampionName).SetValue(true));
            foreach (var enemy in HeroManager.Enemies)
                Config.SubMenu(Player.ChampionName).SubMenu("E Config").SubMenu("Use E ").AddItem(new MenuItem("stun" + enemy.ChampionName, enemy.ChampionName).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("useE", "OneKeyToCast E closest person", true).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press))); //32 == space
            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("Eks", "E KS", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("autoR", "Auto R", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("visibleR", "Unvisable block AA ", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("autoQR", "Auto Q when R active ", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("farmQ", "Q farm helper", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("farmQjungle", "Q jungle", true).SetValue(true));

        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var target = gapcloser.Sender;

            if (E.LSIsReady() && target.LSIsValidTarget(E.Range) && Config.Item("gapE", true).GetValue<bool>() && Config.Item("gap" + target.ChampionName).GetValue<bool>())
                E.Cast(target);
        }

        private void BeforeAttack(SebbyLib.Orbwalking.BeforeAttackEventArgs args)
        {
            if (Config.Item("visibleR", true).GetValue<bool>() && Player.HasBuff("vaynetumblefade") && Player.LSCountEnemiesInRange(800) > 1)
                args.Process = false;

            if (args.Target.Type != GameObjectType.AIHeroClient)
                return;

            var t = args.Target as AIHeroClient;

            if (GetWStacks(t) < 2 && args.Target.Health > 5 * Player.LSGetAutoAttackDamage(t))
            {
                foreach (var target in HeroManager.Enemies.Where(target => target.LSIsValidTarget(800) && GetWStacks(target) == 2))
                {
                    if (SebbyLib.Orbwalking.InAutoAttackRange(target) && args.Target.Health > 3 * Player.LSGetAutoAttackDamage(target))
                    {
                        args.Process = false;
                        Orbwalker.ForceTarget(target);
                    }
                }
            }
        }

        private void afterAttack(AttackableUnit unit, AttackableUnit target)
        {
            var t = target as AIHeroClient;
            if (t != null)
            {
                if (E.LSIsReady() && Config.Item("Eks", true).GetValue<bool>())
                {
                    var incomingDMG = OktwCommon.GetIncomingDamage(t, 0.3f, false);
                    if (incomingDMG > t.Health)
                        return;

                    var dmgE = E.GetDamage(t) + incomingDMG;

                    if (GetWStacks(t) == 1)
                        dmgE += Wdmg(t);

                    if (dmgE > t.Health)
                    {
                        E.Cast(t);
                    }
                }

                if (Q.LSIsReady() && !Program.None && Config.Item("autoQ", true).GetValue<bool>() && (GetWStacks(t) == Config.Item("Qstack", true).GetValue<Slider>().Value - 1 || Player.HasBuff("vayneinquisition")))
                {
                    var dashPos = Dash.CastDash(true);
                    if (!dashPos.IsZero)
                    {
                        Q.Cast(dashPos);
                    }
                }
            }

            var m = target as Obj_AI_Minion;

            if (m != null && Q.LSIsReady() && Program.Farm && Config.Item("farmQ", true).GetValue<bool>())
            {
                var dashPosition = Player.Position.LSExtend(Game.CursorPos, Q.Range);
                if (!Dash.IsGoodPosition(dashPosition))
                    return;
                
                if (Config.Item("farmQjungle", true).GetValue<bool>() && m.Team == GameObjectTeam.Neutral)
                {
                    Q.Cast(dashPosition, true);
                }

                if (Config.Item("farmQ", true).GetValue<bool>())
                {
                    foreach (var minion in Cache.GetMinions(dashPosition, 0).Where(minion => m.NetworkId != minion.NetworkId))
                    {
                        var time = (int)(Player.AttackCastDelay * 1000) + Game.Ping / 2 + 1000 * (int)Math.Max(0, Player.LSDistance(minion) - Player.BoundingRadius) / (int)Player.BasicAttack.MissileSpeed;
                        var predHealth = SebbyLib.HealthPrediction.GetHealthPrediction(minion, time);
                        if (predHealth < Player.LSGetAutoAttackDamage(minion) + Q.GetDamage(minion) && predHealth > 0)
                            Q.Cast(dashPosition, true);
                    }
                }
            }
        }

        private double Wdmg(Obj_AI_Base target)
        {
            return target.MaxHealth * (4.5 + W.Level * 1.5) * 0.01;
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            var dashPosition = Player.Position.LSExtend(Game.CursorPos, Q.Range);

            if (E.LSIsReady())
            {
                var ksTarget = Player;
                foreach (var target in HeroManager.Enemies.Where(target => target.LSIsValidTarget(E.Range) && target.Path.Count() < 2 ))
                {
                    if (CondemnCheck(Player.ServerPosition, target) && Config.Item("stun" + target.ChampionName).GetValue<bool>() )
                        E.Cast(target);
                    else if (Q.LSIsReady() && Dash.IsGoodPosition(dashPosition) && Config.Item("QE", true).GetValue<bool>() && CondemnCheck(dashPosition, target))
                    {
                        Q.Cast(dashPosition);
                        Program.debug("Q + E");
                    }
                }
            }

            if (Program.LagFree(1) && Q.LSIsReady())
            {
                if (Config.Item("autoQR", true).GetValue<bool>() && Player.HasBuff("vayneinquisition")  && Player.LSCountEnemiesInRange(1500) > 0 && Player.LSCountEnemiesInRange(670) != 1)
                {
                    var dashPos = Dash.CastDash();
                    if (!dashPos.IsZero)
                    {
                        Q.Cast(dashPos);
                    }
                }
                if (Program.Combo && Config.Item("autoQ", true).GetValue<bool>() && !Config.Item("Qonly", true).GetValue<bool>())
                {
                    var t = TargetSelector.GetTarget(900, TargetSelector.DamageType.Physical);

                    if (t.LSIsValidTarget() && !SebbyLib.Orbwalking.InAutoAttackRange(t) && t.Position.LSDistance(Game.CursorPos) < t.Position.LSDistance(Player.Position) &&  !t.LSIsFacing(Player))
                    {
                        var dashPos = Dash.CastDash();
                        if (!dashPos.IsZero)
                        {
                            Q.Cast(dashPos);
                        }
                    }
                }
            }

            if (Program.LagFree(2))
            {
                AIHeroClient bestEnemy = null;
                foreach (var target in HeroManager.Enemies.Where(target => target.LSIsValidTarget(E.Range)))
                {
                    if (target.LSIsValidTarget(250) && target.IsMelee)
                    {
                        if (Q.LSIsReady() && Config.Item("autoQ", true).GetValue<bool>())
                        {
                            var dashPos = Dash.CastDash(true);
                            if (!dashPos.IsZero)
                            {
                                Q.Cast(dashPos);
                            }
                        }
                        else if (E.LSIsReady() && Player.Health < Player.MaxHealth * 0.4)
                        {
                            E.Cast(target);
                            Program.debug("push");
                        }
                    }
                    if (bestEnemy == null)
                        bestEnemy = target;
                    else if (Player.LSDistance(target.Position) < Player.LSDistance(bestEnemy.Position))
                        bestEnemy = target;
                }
                if (Config.Item("useE", true).GetValue<KeyBind>().Active && bestEnemy != null)
                {
                    E.Cast(bestEnemy);
                }
            }

            if (Program.LagFree(3) && R.LSIsReady() )
            {
                if ( Config.Item("autoR", true).GetValue<bool>())
                {
                    if (Player.LSCountEnemiesInRange(700) > 2)
                        R.Cast();
                    else if (Program.Combo && Player.LSCountEnemiesInRange(600) > 1)
                        R.Cast();
                    else if (Player.Health < Player.MaxHealth * 0.5 && Player.LSCountEnemiesInRange(500) > 0)
                        R.Cast();
                }
            }
        }

        private bool CondemnCheck(Vector3 fromPosition, AIHeroClient target)
        {
            var prepos = E.GetPrediction(target);

            float pushDistance = 470;

            if (Player.ServerPosition != fromPosition)
                pushDistance = 410 ;

            int radius = 250;
            var start2 = target.ServerPosition;
            var end2 = prepos.CastPosition.LSExtend(fromPosition, -pushDistance);

            Vector2 start = start2.LSTo2D();
            Vector2 end = end2.LSTo2D();
            var dir = (end - start).LSNormalized();
            var pDir = dir.LSPerpendicular();

            var rightEndPos = end + pDir * radius;
            var leftEndPos = end - pDir * radius;


            var rEndPos = new Vector3(rightEndPos.X, rightEndPos.Y, ObjectManager.Player.Position.Z);
            var lEndPos = new Vector3(leftEndPos.X, leftEndPos.Y, ObjectManager.Player.Position.Z);


            var step = start2.LSDistance(rEndPos) / 10;
            for (var i = 0; i < 10; i++)
            {
                var pr = start2.LSExtend(rEndPos, step * i);
                var pl = start2.LSExtend(lEndPos, step * i);
                if (pr.LSIsWall() && pl.LSIsWall())
                    return true;
            }

            return false;
        }

        private int GetWStacks(Obj_AI_Base target)
        {
            foreach (var buff in target.Buffs)
            {
                if (buff.Name.ToLower() == "vaynesilvereddebuff")
                    return buff.Count;
            }
            return 0;
        }

        private List<Vector3> CirclePoint(float CircleLineSegmentN, float radius, Vector3 position)
        {
            List<Vector3> points = new List<Vector3>();
            for (var i = 1; i <= CircleLineSegmentN; i++)
            {
                var angle = i * 2 * Math.PI / CircleLineSegmentN;
                var point = new Vector3(position.X + radius * (float)Math.Cos(angle), position.Y + radius * (float)Math.Sin(angle), position.Z);
                points.Add(point);
            }
            return points;
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (Config.Item("qRange", true).GetValue<bool>())
            {
                if (Config.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (Q.LSIsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, Q.Range + E.Range, System.Drawing.Color.Cyan, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, Q.Range + E.Range, System.Drawing.Color.Cyan, 1, 1);
            }

            if (E.LSIsReady() && Config.Item("eRange2", true).GetValue<bool>())
            {
                foreach (var target in HeroManager.Enemies.Where(target => target.LSIsValidTarget(800)))
                {
                    var poutput = E.GetPrediction(target);

                    var pushDistance = 460;

                    var finalPosition = poutput.CastPosition.LSExtend(Player.ServerPosition, -pushDistance);
                    if (finalPosition.LSIsWall())
                        Render.Circle.DrawCircle(finalPosition, 100, System.Drawing.Color.Red);
                    else
                        Render.Circle.DrawCircle(finalPosition, 100, System.Drawing.Color.YellowGreen);
                }
            }
        }
    }
}
