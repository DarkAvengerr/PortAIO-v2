using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Flowers__AurelionSol
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using LeagueSharp;
    using LeagueSharp.Common;
    using Color = System.Drawing.Color;
    using SharpDX;
    using SharpDX.Direct3D9;

    class Program
    {
        internal static AIHeroClient Me;
        internal static Spell Q, W, E, R;
        internal static Menu Menu;
        internal static Orbwalking.Orbwalker Orbwalker;
        internal static MissileClient qqq;
        public static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName != "AurelionSol")
                return;

            Me = ObjectManager.Player;

            Q = new Spell(SpellSlot.Q, 700f);
            W = new Spell(SpellSlot.W, 675f);
            E = new Spell(SpellSlot.E, 400f);
            R = new Spell(SpellSlot.R, 1550f);

            Q.SetSkillshot(0.40f, 180, 800, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.25f, 180, 1750, false, SkillshotType.SkillshotLine);

            Menu = new Menu("Flowers' AurelionSol", "NightMoon", true);

            Menu.AddSubMenu(new Menu("[FL] Orbwalking", "nightmoon.Orbwalking.Menu"));
            Orbwalker = new Orbwalking.Orbwalker(Menu.SubMenu("nightmoon.Orbwalking.Menu"));

            Menu.AddSubMenu(new Menu("[FL] Combo Menu", "nightmoon.Combo.Menu"));
            Menu.SubMenu("nightmoon.Combo.Menu").AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
            Menu.SubMenu("nightmoon.Combo.Menu").AddItem(new MenuItem("ComboQFollow", "Auto Follow Q", true).SetValue(true));
            Menu.SubMenu("nightmoon.Combo.Menu").AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
            Menu.SubMenu("nightmoon.Combo.Menu").AddItem(new MenuItem("ComboR", "Use R", true).SetValue(true));
            Menu.SubMenu("nightmoon.Combo.Menu").AddItem(new MenuItem("ComboRHit", "R Min HitChance Counts", true).SetValue(new Slider(2, 1, 5)));

            Menu.AddSubMenu(new Menu("[FL] Harass Menu", "nightmoon.Harass.Menu"));
            Menu.SubMenu("nightmoon.Harass.Menu").AddItem(new MenuItem("HarassQ", "Use Q", true).SetValue(true));
            Menu.SubMenu("nightmoon.Harass.Menu").AddItem(new MenuItem("HarassW", "Use W", true).SetValue(true));

            Menu.AddSubMenu(new Menu("[FL] Clear Menu", "nightmoon.Clear.Menu"));
            Menu.SubMenu("nightmoon.Clear.Menu").AddItem(new MenuItem("LaneClear", "    LaneClear Setting", true));
            Menu.SubMenu("nightmoon.Clear.Menu").AddItem(new MenuItem("LaneClearQ", "Use Q", true).SetValue(true));
            Menu.SubMenu("nightmoon.Clear.Menu").AddItem(new MenuItem("LaneClearW", "Use W", true).SetValue(true));
            Menu.SubMenu("nightmoon.Clear.Menu").AddItem(new MenuItem("JungleClear", "    JungleClear Setting", true));
            Menu.SubMenu("nightmoon.Clear.Menu").AddItem(new MenuItem("JungleClearQ", "Use Q", true).SetValue(true));
            Menu.SubMenu("nightmoon.Clear.Menu").AddItem(new MenuItem("JungleClearW", "Use W", true).SetValue(true));

            Menu.AddSubMenu(new Menu("[FL] Mana Control", "nightmoon.Mana.Menu"));
            Menu.SubMenu("nightmoon.Mana.Menu").AddItem(new MenuItem("HarassMana", "Min Harass Mana < %", true).SetValue(new Slider(40)));
            Menu.SubMenu("nightmoon.Mana.Menu").AddItem(new MenuItem("LaneClearMana", "Min LaneClear Mana < %", true).SetValue(new Slider(40)));
            Menu.SubMenu("nightmoon.Mana.Menu").AddItem(new MenuItem("JungleClearMana", "Min JungleClear Mana < %", true).SetValue(new Slider(40)));

            Menu.AddSubMenu(new Menu("[FL] Misc Menu", "nightmoon.Misc.Menu"));
            Menu.SubMenu("nightmoon.Misc.Menu").AddItem(new MenuItem("Packet", "         Packet", true));
            Menu.SubMenu("nightmoon.Misc.Menu").AddItem(new MenuItem("UsePacket", "Use Packet", true).SetValue(true));
            Menu.SubMenu("nightmoon.Misc.Menu").AddItem(new MenuItem("KillSteal", "          KillSteal", true));
            Menu.SubMenu("nightmoon.Misc.Menu").AddItem(new MenuItem("KillStealQ", "Use Q", true).SetValue(true));
            Menu.SubMenu("nightmoon.Misc.Menu").AddItem(new MenuItem("KillStealR", "Use R", true).SetValue(true));
            Menu.SubMenu("nightmoon.Misc.Menu").AddItem(new MenuItem("Interrupt", "          Interrupt", true));
            Menu.SubMenu("nightmoon.Misc.Menu").AddItem(new MenuItem("InterruptQ", "Use Q", true).SetValue(true));
            Menu.SubMenu("nightmoon.Misc.Menu").AddItem(new MenuItem("InterruptR", "Use R", true).SetValue(false));
            Menu.SubMenu("nightmoon.Misc.Menu").AddItem(new MenuItem("GapCloser", "          AntiGapCloser", true));
            Menu.SubMenu("nightmoon.Misc.Menu").AddItem(new MenuItem("GapCloserQ", "Use Q", true).SetValue(true));
            Menu.SubMenu("nightmoon.Misc.Menu").AddItem(new MenuItem("GapCloserR", "Use R", true).SetValue(false));

            Menu.AddSubMenu(new Menu("[FL] Draw Menu", "nightmoon.Draw.Menu"));
            Menu.SubMenu("nightmoon.Draw.Menu").AddItem(new MenuItem("DrawQ", "Q Range", true).SetValue(new Circle(false, Color.Azure)));
            Menu.SubMenu("nightmoon.Draw.Menu").AddItem(new MenuItem("DrawW", "W Range", true).SetValue(new Circle(false, Color.Blue)));
            Menu.SubMenu("nightmoon.Draw.Menu").AddItem(new MenuItem("DrawE", "E Range", true).SetValue(new Circle(false, Color.DarkSalmon)));
            Menu.SubMenu("nightmoon.Draw.Menu").AddItem(new MenuItem("DrawR", "R Range", true).SetValue(new Circle(false, Color.Red)));

            Menu.AddToMainMenu();

            Chat.Print("<font color='#2848c9'>Flowers' AurelionSol</font> --> <font color='#b756c5'>Load! </font> <font size='30'><font color='#d949d4'>Good Luck!</font></font>");


            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Game.OnUpdate += OnUpdate;
            GameObject.OnCreate += OnCreate;
            GameObject.OnDelete += OnDelete;
            Drawing.OnDraw += OnDraw;
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var sender = gapcloser.Sender;

            if (!sender.IsEnemy)
                return;

            if (Q.IsReady() && Menu.GetBool("GapCloserQ"))
            {
                var QPred = Q.GetPrediction(sender);

                if (sender.IsValidTarget(Q.Range))
                {
                    if (QPred.Hitchance >= HitChance.VeryHigh)
                        Q.Cast(QPred.CastPosition, UsePacket);
                }
            }

            if (R.IsReady() && Menu.GetBool("GapCloserR"))
            {
                var RPred = R.GetPrediction(sender);

                if (sender.IsValidTarget(R.Range))
                {
                    if (RPred.Hitchance >= HitChance.VeryHigh)
                        R.Cast(RPred.CastPosition, UsePacket);
                }
            }
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender.IsMe || sender.IsAlly)
                return;

            if (args.DangerLevel >= Interrupter2.DangerLevel.Medium)
            {
                if (Q.IsReady() && Menu.GetBool("GapCloserQ"))
                {
                    var QPred = Q.GetPrediction(sender);

                    if (sender.IsValidTarget(Q.Range))
                    {
                        if (QPred.Hitchance >= HitChance.VeryHigh)
                            Q.Cast(QPred.CastPosition, UsePacket);
                    }
                }

                if (args.DangerLevel == Interrupter2.DangerLevel.High)
                {
                    if (R.IsReady() && Menu.GetBool("GapCloserR"))
                    {
                        var RPred = R.GetPrediction(sender);

                        if (sender.IsValidTarget(R.Range))
                        {
                            if (RPred.Hitchance >= HitChance.VeryHigh)
                                R.Cast(RPred.CastPosition, UsePacket);
                        }
                    }
                }
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Me.IsDead || Me.IsRecalling())
                return;

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    ComboLogic();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    HarassLogic();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClearLogic();
                    JungleClearLogic();
                    break;
            }

            AutoKillStealLogic();
        }

        private static void AutoKillStealLogic()
        {
            foreach(var e in HeroManager.Enemies.Where(em => em.IsValidTarget() && !em.IsZombie && !em.IsDead))
            {
                if (Q.IsReady() && Menu.GetBool("KillStealQ"))
                {

                    if (e.Health + e.MagicShield + 50 < GetQDamage(e))
                    {
                        var QPred = Q.GetPrediction(e);

                        if (QPred.Hitchance >= HitChance.VeryHigh)
                            Q.Cast(QPred.CastPosition, UsePacket);
                    }
                }

                if (R.IsReady() && Menu.GetBool("KillStealR"))
                {
                    if (e.Health + e.MagicShield + 50 < GetRDamage(e))
                    {
                        R.Cast(e, UsePacket);
                    }
                }
            }
        }

        private static void JungleClearLogic()
        {
            if (Me.ManaPercent > Menu.GetSlider("JungleClearMana"))
            {
                if (Q.IsReady() && Menu.GetBool("JungleClearQ"))
                {
                    var QMob = MinionManager.GetMinions(Me.Position, Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

                    if (QMob != null)
                    {
                        foreach(var mob in QMob)
                        {
                            if (mob.IsValidTarget(Q.Range))
                                Q.Cast(mob, UsePacket);
                        }
                    }
                }

                if (HavePassive && Menu.GetBool("JungleClearW"))
                {
                    var WMob = MinionManager.GetMinions(Me.Position, W.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

                    if (WMob != null)
                    {
                        foreach (var mob in WMob)
                        {
                            if (mob.IsValidTarget(W.Range) && !mob.IsValidTarget(420) && !IsWActive)
                            {
                                W.Cast(UsePacket);
                            }
                            else if (IsWActive && mob.IsValidTarget(420))
                            {
                                W.Cast(UsePacket);
                            }
                        }
                    }
                }
            }
        }

        private static void LaneClearLogic()
        {
            if (Me.ManaPercent > Menu.GetSlider("LaneClearMana"))
            {
                if (Menu.GetBool("LaneClearQ") && Q.IsReady())
                {
                    var QMin = MinionManager.GetMinions(Me.Position, Q.Range);
                    var FarmLocation = Q.GetCircularFarmLocation(QMin, Q.Width);

                    if (QMin != null)
                    {
                        if (FarmLocation.MinionsHit >= 3)
                            Q.Cast(FarmLocation.Position, UsePacket);
                    }
                }

                if (Menu.GetBool("LaneClearW") && HavePassive)
                {
                    var WMin = MinionManager.GetMinions(Me.Position, W.Range);

                    if (WMin != null)
                    {
                        if (WMin.Count() >= 2)
                            W.Cast(UsePacket);
                    }
                }
            }
        }

        private static void HarassLogic()
        {
            if (Me.ManaPercent > Menu.GetSlider("HarassMana"))
            {
                if (Menu.GetBool("HarassQ"))
                {
                    var QTTT = TargetSelector.GetTarget(Q.Range * 2, TargetSelector.DamageType.Magical);
                    var QPred = Q.GetPrediction(QTTT);

                    if (Q.IsReady() && QTTT.IsValidTarget(Q.Range) && !SecondQ)
                    {
                        if (QPred.Hitchance >= HitChance.High)
                            Q.Cast(QPred.CastPosition, UsePacket);
                    }

                    if (qqq != null && SecondQ)
                    {
                        var QSize = qqq.StartPosition.Distance(qqq.Position);
                        var QRange = ((QSize + Q.Width) / 15) * ((QSize + Q.Width) / 15);

                        if (TargetInqqq(QTTT.ServerPosition, QRange))
                            Q.Cast(UsePacket);
                    }
                }

                if (Menu.GetBool("HarassW") && HavePassive && W.IsReady())
                {
                    var WTTT = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);

                    if (!IsWActive)
                    {
                        if (WTTT != null && WTTT.IsValidTarget(W.Range) && !WTTT.IsValidTarget(420))
                        {
                            W.Cast(UsePacket);
                        }
                    }
                    else if (IsWActive)
                    {
                        if (!(WTTT.Distance(Me.ServerPosition) < 840) || WTTT.IsValidTarget(420))
                        {
                            W.Cast(UsePacket);
                        }
                    }
                }
            }
        }

        private static void ComboLogic()
        {
            if (Menu.GetBool("ComboQ"))
            {
                var QTTT = TargetSelector.GetTarget(Q.Range * 2, TargetSelector.DamageType.Magical);

                if (Q.IsReady() && QTTT.IsValidTarget(Q.Range))
                {
                    Q.Cast(QTTT, UsePacket);
                }
            }

            if (Menu.GetBool("ComboQFollow"))
            {
                if (SecondQ && qqq != null)
                {
                    Orbwalker.SetMovement(false);
                    Orbwalker.SetAttack(false);
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, qqq.Position);
                }
                else if (!SecondQ && qqq == null)
                {
                    Orbwalker.SetAttack(true);
                    Orbwalker.SetMovement(true);
                }
            }

            if (Menu.GetBool("ComboW") && HavePassive && W.IsReady())
            {
                var WTTT = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);

                if (!IsWActive)
                {
                    if (WTTT != null && WTTT.IsValidTarget(W.Range) && !WTTT.IsValidTarget(420))
                    {
                        W.Cast(UsePacket);
                    }
                }
                else if (IsWActive)
                {
                    if (!WTTT.IsValidTarget(800) || WTTT.IsValidTarget(420))
                    {
                        W.Cast(UsePacket);
                    }
                }
            }
            
            if (Menu.GetBool("ComboR") && R.IsReady())
            {
                var RTTT = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);

                if (RTTT != null && RTTT.IsValidTarget(R.Range))
                {
                    foreach (var enemy in from enemy in HeroManager.Enemies
                                          let startPos = enemy.ServerPosition
                                          let endPos = Me.ServerPosition.Extend(startPos, Me.Distance(enemy) + R.Range)
                                          let rectangle = new Geometry.Polygon.Rectangle(startPos, endPos, R.Width)
                                          where HeroManager.Enemies.Count(x => rectangle.IsInside(x)) >= Menu.GetSlider("ComboRHit")
                                          select enemy)
                    {
                        R.Cast(enemy.Position, UsePacket);
                    }
                }
            }
        }

        public static bool TargetInqqq(Vector3 TargetPos, float range)
        {
            return qqq.Position.To2D().Distance(TargetPos.To2D(), true) < range;
        }


        private static void OnCreate(GameObject sender, EventArgs args)
        {
            var SpellQ = sender as MissileClient;

            if (SpellQ != null)
            {
                if (SpellQ.IsValid && SpellQ.SpellCaster.IsMe && SpellQ.SpellCaster.IsValid && SpellQ.SData.Name.Contains("AurelionSolQMissile"))
                {
                    qqq = SpellQ;
                }
            }
        }

        private static void OnDelete(GameObject sender, EventArgs args)
        {
            var SpellQ = sender as MissileClient;

            if (SpellQ != null)
            {
                if (SpellQ.IsValid && SpellQ.SpellCaster is AIHeroClient)
                    if (SpellQ.SpellCaster.IsMe && SpellQ.SpellCaster.IsValid && SpellQ.SData.Name.Contains("AurelionSolQMissile"))
                    {
                        qqq = null;
                    }
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (Me.IsDead)
                return;

            if (Menu.GetDraw("DrawQ") && Q.IsReady())
                Render.Circle.DrawCircle(Me.Position, Q.Range, Menu.GetColor("DrawQ"));

            if (Menu.GetDraw("DrawW") && HavePassive)
                Render.Circle.DrawCircle(Me.Position, IsWActive ? 675 : 420, Menu.GetColor("DrawW"));

            if (Menu.GetDraw("DrawE") && E.IsReady())
                Render.Circle.DrawCircle(Me.Position, E.Range, Menu.GetColor("DrawE"));

            if (Menu.GetDraw("DrawR") && R.IsReady())
                Render.Circle.DrawCircle(Me.Position, R.Range, Menu.GetColor("DrawR"));
        }

        internal static double GetQDamage(Obj_AI_Base t)
        {
            return Me.CalcDamage
                (t, Damage.DamageType.Magical, 
                (float)new double[] { 70, 110, 150, 190, 230 }[Q.Level - 1] + 0.65f *
                Me.TotalMagicalDamage);
        }

        internal static double GetRDamage(Obj_AI_Base t)
        {
            return Me.CalcDamage
                (t, Damage.DamageType.Magical,
                (float)new double[] { 200, 400, 600 }[R.Level - 1] + 0.70f *
                Me.TotalMagicalDamage);
        }

        internal static bool IsWActive
        {
            get
            {
                return Me.HasBuff("AurelionSolWActive");
            }
        }

        internal static bool HavePassive
        {
            get
            {
                return Me.HasBuff("AurelionSolPassive");
            }
        }

        internal static bool SecondQ
        {
            get
            {
                return Me.HasBuff("AurelionSolQHaste");
            }
        }

        internal static bool UsePacket
        {
            get
            {
                return Menu.Item("UsePacket", true).GetValue<bool>();
            }
        }
    }
}
