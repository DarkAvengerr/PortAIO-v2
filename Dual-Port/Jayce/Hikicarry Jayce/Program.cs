using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;
using LeagueSharp;
using SharpDX;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HikiCarry_Jayce___Hammer_of_Justice
{
    class Program
    {
        public static Menu Config;
        public static Spell CannonQ, CannonQExt, CannonW, CannonE, R, HammerQ, HammerW, HammerE;
        public static float RangeQ,RangeQExt, RangeW, RangeE, RangeR;
        public static readonly AIHeroClient Jayce = ObjectManager.Player;
        public static Orbwalking.Orbwalker Orbwalker;
        public static bool Hammer,Cannon;
        //public static int Stage; // 1-CANNONQ / 2-CANNONW / 3-CANNONE / 4-HAMMERQ / 5-HAMMERW / 6-HAMMERE
        public static int Stage = 0;

        public static void OnGameLoad()
        {
            if (Jayce.CharData.BaseSkinName != "Jayce")
            {
                return;
            }
            
            CannonQ = new Spell(SpellSlot.Q, 1050);
            CannonQExt = new Spell(SpellSlot.Q, 1650);
            HammerQ = new Spell(SpellSlot.Q, 600);
            CannonW = new Spell(SpellSlot.W);
            HammerW = new Spell(SpellSlot.W, 350);
            CannonE = new Spell(SpellSlot.E, 650);
            HammerE = new Spell(SpellSlot.E, 240);
            R = new Spell(SpellSlot.R);

            CannonQ.SetSkillshot(0.25f, 79, 1200, true, SkillshotType.SkillshotLine);
            CannonQExt.SetSkillshot(0.35f, 98, 1600, true, SkillshotType.SkillshotLine);
            HammerQ.SetTargetted(0.25f, float.MaxValue);
            CannonE.SetSkillshot(0.1f, 120, float.MaxValue, false, SkillshotType.SkillshotCircle);
            HammerE.SetTargetted(.25f, float.MaxValue);

            Config = new Menu("HikiCarry - Jayce", "HikiCarry - Jayce", true);
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker Settings"));

            var comboMenu = new Menu("Combo Settings", "Combo Settings");
            {
                comboMenu.AddItem(new MenuItem("q.cannon", "Cannon (Q)").SetValue(true));
                comboMenu.AddItem(new MenuItem("w.cannon", "Cannon (W)").SetValue(true));
                comboMenu.AddItem(new MenuItem("e.cannon", "Cannon (E)").SetValue(true));
                comboMenu.AddItem(new MenuItem("combo.switch", "Auto Switch").SetValue(true));
                comboMenu.AddItem(new MenuItem("q.hammer", "Hammer (Q)").SetValue(true));
                comboMenu.AddItem(new MenuItem("w.hammer", "Hammer (W)").SetValue(true));
                comboMenu.AddItem(new MenuItem("e.hammer", "Hammer (E)").SetValue(true));
                comboMenu.AddItem(new MenuItem("combo", "Combo!", true).SetValue(new KeyBind(32, KeyBindType.Press)));
                Config.AddSubMenu(comboMenu);
            }
            /*var burstMenu = new Menu("Burst Settings", "Burst Settings");
            {
                burstMenu.AddItem(new MenuItem("burst.style", "Burst Style").SetValue(new StringList(new[] { "Cannon Burst 1", "Cannon Burst 2", "Hammer Burst 1","Hammer Burst 2" })));
                burstMenu.AddItem(new MenuItem("burst", "Burst!", true).SetValue(new KeyBind("K".ToCharArray()[0], KeyBindType.Press)));
                Config.AddSubMenu(burstMenu);
            }*/
            var harassMenu = new Menu("Harass Settings", "Harass Settings");
            {
                harassMenu.AddItem(new MenuItem("q.cannon.harass", "Cannon (Q)").SetValue(true));
                harassMenu.AddItem(new MenuItem("e.cannon.harass", "Cannon (E)").SetValue(true));
                harassMenu.AddItem(new MenuItem("harass.mana", "Min. Mana Percent").SetValue(new Slider(20, 0, 100)));
                Config.AddSubMenu(harassMenu);
            }
            var clearMenu = new Menu("Clear Settings", "Clear Settings");
            {
                clearMenu.AddItem(new MenuItem("q.cannon.clear", "Cannon (Q)").SetValue(true));
                clearMenu.AddItem(new MenuItem("e.cannon.clear", "Cannon (E)").SetValue(true));
                clearMenu.AddItem(new MenuItem("clear.switch", "Auto Switch").SetValue(true));
                clearMenu.AddItem(new MenuItem("clear.minion.count", "Min. Minion Count").SetValue(new Slider(3, 1, 5)));
                clearMenu.AddItem(new MenuItem("clear.mana", "Min. Mana Percent").SetValue(new Slider(20, 0, 100)));
                Config.AddSubMenu(clearMenu);
            }
            var jungleMenu = new Menu("Jungle Settings", "Jungle Settings");
            {
                jungleMenu.AddItem(new MenuItem("q.cannon.jungle", "Cannon (Q)").SetValue(true));
                jungleMenu.AddItem(new MenuItem("w.cannon.jungle", "Cannon (W)").SetValue(true));
                jungleMenu.AddItem(new MenuItem("e.cannon.jungle", "Cannon (E)").SetValue(true));
                jungleMenu.AddItem(new MenuItem("jungle.switch", "Auto Switch").SetValue(true));
                jungleMenu.AddItem(new MenuItem("q.hammer.jungle", "Hammer (Q)").SetValue(true));
                jungleMenu.AddItem(new MenuItem("w.hammer.jungle", "Hammer (W)").SetValue(true));
                jungleMenu.AddItem(new MenuItem("e.hammer.jungle", "Hammer (E)").SetValue(true));
                jungleMenu.AddItem(new MenuItem("jungle.mana", "Min. Mana Percent").SetValue(new Slider(20, 0, 100)));
                Config.AddSubMenu(jungleMenu);
            }
            var killSteal = new Menu("KillSteal Settings", "KillSteal Settings");
            {
                killSteal.AddItem(new MenuItem("q.cannon.ks", "Cannon (Q)").SetValue(true));
                killSteal.AddItem(new MenuItem("e.cannon.ks", "Cannon (E)").SetValue(true));
                killSteal.AddItem(new MenuItem("q.hammer.ks", "Hammer Q (Safe)").SetValue(true));
                Config.AddSubMenu(killSteal);
            }
            var miscMenu = new Menu("Miscellaneous", "Miscellaneous");
            {
                miscMenu.AddItem(new MenuItem("interrupt.hammer.e", "Interrupter (Hammer E)").SetValue(true));
                miscMenu.AddItem(new MenuItem("gapcloser.hammer.e", "Gapcloser (Hammer E)").SetValue(true));
                Config.AddSubMenu(miscMenu);
            }
            var drawMenu = new Menu("Draw Settings", "Draw Settings");
            {
                drawMenu.AddItem(new MenuItem("draw.q", "(Q) Range").SetValue(new Circle(true, Color.LightCoral)));
                drawMenu.AddItem(new MenuItem("draw.w", "(W) Range").SetValue(new Circle(true, Color.LightCoral)));
                drawMenu.AddItem(new MenuItem("draw.e", "(E) Range").SetValue(new Circle(true, Color.LightCoral)));
                Config.AddSubMenu(drawMenu);
            }
            var drawDamageMenu = new MenuItem("RushDrawEDamage", "Combo Damage").SetValue(true);
            var drawFill = new MenuItem("RushDrawEDamageFill", "Combo Damage Fill").SetValue(new Circle(true, Color.Gold));

            drawMenu.SubMenu("Damage Draws").AddItem(drawDamageMenu);
            drawMenu.SubMenu("Damage Draws").AddItem(drawFill);
            DamageIndicator.DamageToUnit = Helper.TotalDamage;
            DamageIndicator.Enabled = drawDamageMenu.GetValue<bool>();
            DamageIndicator.Fill = drawFill.GetValue<Circle>().Active;
            DamageIndicator.FillColor = drawFill.GetValue<Circle>().Color;

            drawDamageMenu.ValueChanged +=
            delegate(object sender, OnValueChangeEventArgs eventArgs)
            {
                DamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
            };

            drawFill.ValueChanged +=
            delegate(object sender, OnValueChangeEventArgs eventArgs)
            {
                DamageIndicator.Fill = eventArgs.GetNewValue<Circle>().Active;
                DamageIndicator.FillColor = eventArgs.GetNewValue<Circle>().Color;
            };

            Config.AddToMainMenu();
            Chat.Print("<font color='#ff3232'>HikiCarry Jayce - Hammer of Justice: </font> <font color='#d4d4d4'>If you like this assembly feel free to upvote on Assembly DB</font>");
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Game.OnUpdate += OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (Hammer)
            {
                if (Config.Item("ainterrupt").GetValue<bool>())
                {
                    if (sender.IsValidTarget(1000))
                    {
                        Render.Circle.DrawCircle(sender.Position, sender.BoundingRadius, Color.Gold, 5);
                        var targetpos = Drawing.WorldToScreen(sender.Position);
                        Drawing.DrawText(targetpos[0] - 40, targetpos[1] + 20, Color.Gold, "Interrupt");
                    }
                    if (HammerE.CanCast(sender))
                    {
                        HammerE.Cast(sender);
                    }
                }
            }
        }
        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Hammer)
            {
                if (Config.Item("agapcloser").GetValue<bool>())
                {
                    if (gapcloser.Sender.IsValidTarget(1000))
                    {
                        Render.Circle.DrawCircle(gapcloser.Sender.Position, gapcloser.Sender.BoundingRadius, Color.Gold, 5);
                        var targetpos = Drawing.WorldToScreen(gapcloser.Sender.Position);
                        Drawing.DrawText(targetpos[0] - 40, targetpos[1] + 20, Color.Gold, "Gapcloser");
                    }
                    if (HammerE.CanCast(gapcloser.Sender))
                    {
                        HammerE.Cast(gapcloser.Sender);
                    }
                }
            }
        }
        private static void OnGameUpdate(EventArgs args)
        {
            Helper.CheckForm();
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;

                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;

                case Orbwalking.OrbwalkingMode.LaneClear:
                    Clear();
                    Jungle();
                    break;
            }

            /*if (Config.Item("burst", true).GetValue<KeyBind>().Active)
            {
                Helper.BurstCombo();
            }
            */
        }
        private static void Combo()
        {
            if (Cannon || !Jayce.IsMelee)
            {
                if (Helper.MenuCheck("q.cannon") && Helper.MenuCheck("e.cannon") && CannonQ.IsReady() && CannonE.IsReady())
                {
                    Helper.Ext();
                }

                if (Helper.MenuCheck("q.cannon") && CannonQ.IsReady() && !CannonE.IsReady())
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(CannonQ.Range)))
                    {
                        if (CannonQ.GetPrediction(enemy).Hitchance >= HitChance.VeryHigh)
                        {
                            CannonQ.Cast(enemy);
                        }
                    }
                }

                if (Helper.MenuCheck("w.cannon") && CannonW.IsReady()) 
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(600)))
                    {
                        CannonW.Cast();
                    }
                }

                if (Helper.MenuCheck("combo.switch") && !CannonQ.IsReady() && !CannonE.IsReady() && !CannonW.IsReady())
                {
                    R.Cast();
                }
            }
            if (Hammer || Jayce.IsMelee)
            {
                if (Helper.MenuCheck("q.hammer") && HammerQ.IsReady())
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(HammerQ.Range)))
                    {
                        HammerQ.Cast(enemy);
                    }
                }
                if (Helper.MenuCheck("w.hammer") && HammerW.IsReady())
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(600)))
                    {
                        HammerW.Cast();
                    }
                }
                if (Helper.MenuCheck("e.hammer") && HammerE.IsReady())
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(HammerE.Range)))
                    {
                        HammerE.Cast(enemy);
                    }
                }
                if (Helper.MenuCheck("combo.switch") && !HammerQ.IsReady() && !HammerW.IsReady() && !HammerE.IsReady())
                {
                    R.Cast();
                }
            } 

        }
        private static void Harass()
        {
            if (Hammer || Jayce.IsMelee && Jayce.ManaPercent < Helper.CountCheckerino("harass.mana"))
            {
                return;
            }

            if (Cannon || !Jayce.IsMelee)
            {
                if (Helper.MenuCheck("q.cannon.harass") && Helper.MenuCheck("e.cannon.harass") && CannonQ.IsReady() &&
                    CannonE.IsReady())
                {
                    Helper.Ext();
                }
                if (Helper.MenuCheck("q.cannon.harass") && CannonQ.IsReady() && !CannonE.IsReady())
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(CannonQ.Range)))
                    {
                        if (CannonQ.GetPrediction(enemy).Hitchance >= HitChance.VeryHigh)
                        {
                            CannonQ.Cast(enemy);
                        }
                    }
                }
            }
        }
        private static void Clear()
        {
            if (Hammer || Jayce.IsMelee && Jayce.ManaPercent < Helper.CountCheckerino("clear.mana"))
            {
                return;
            }
            if (Cannon || !Jayce.IsMelee)
            {
                var minionQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, CannonQ.Range);
                var lineLocation = CannonQ.GetCircularFarmLocation(minionQ);
                if (lineLocation.MinionsHit <= 0)
                {
                    return;
                }
                if (lineLocation.MinionsHit >= Helper.CountCheckerino("clear.minion.count") && Helper.MenuCheck("q.cannon.clear"))
                {
                    CannonQ.Cast(lineLocation.Position);
                }
            }
        }
        private static void Jungle()
        {
            if (Hammer || Jayce.IsMelee)
            {
                var mob = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
                if (mob == null || (mob.Count == 0))
                {
                    return;
                }
                if (HammerQ.CanCast(mob[0]) && Helper.MenuCheck("q.hammer.jungle"))
                {
                    HammerQ.CastOnUnit(mob[0]);
                }
                if (Jayce.Distance(mob[0].Position) < 600 && Helper.MenuCheck("w.hammer.jungle"))
                {
                    HammerW.Cast();
                }
                if (HammerE.CanCast(mob[0]) && Helper.MenuCheck("e.hammer.jungle"))
                {
                    HammerE.CastOnUnit(mob[0]);
                }
                if (!HammerQ.IsReady() && !HammerW.IsReady() && !HammerE.IsReady() && Helper.MenuCheck("jungle.switch"))
                {
                    R.Cast();
                }
            }
            if (Cannon || !Jayce.IsMelee)
            {
                var mob = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
                if (mob == null || (mob.Count == 0))
                {
                    return;
                }
                if (CannonQ.IsReady() && CannonE.IsReady() && Helper.MenuCheck("q.cannon.jungle") && Helper.MenuCheck("e.cannon.jungle"))
                {
                    Helper.JungleExt();
                }
                if (CannonQ.IsReady() && !CannonE.IsReady() && CannonQ.CanCast(mob[0]) && Helper.MenuCheck("q.cannon.jungle"))
                {
                    CannonQ.CastOnUnit(mob[0]);
                }
                if (Jayce.Distance(mob[0].Position) < 600 && Helper.MenuCheck("w.cannon.jungle"))
                {
                    CannonW.Cast();
                }
                if (!CannonQ.IsReady() && !CannonW.IsReady() && !CannonE.IsReady() && Helper.MenuCheck("jungle.switch"))
                {
                    R.Cast();
                }
            }
        }
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Config.Item("q.draw").GetValue<Circle>().Active && CannonQ.IsReady() || HammerQ.IsReady())
            {
                Helper.SkillDraw(RangeQ, Config.Item("q.draw").GetValue<Circle>().Color, 5);
                if (Cannon)
                {
                    Helper.SkillDraw(RangeQExt, Config.Item("q.draw").GetValue<Circle>().Color, 5);
                }
            }
            if (Config.Item("w.draw").GetValue<Circle>().Active && CannonW.IsReady() || HammerW.IsReady())
            {
                Helper.SkillDraw(RangeW, Config.Item("w.draw").GetValue<Circle>().Color, 5);
            }
            if (Config.Item("e.draw").GetValue<Circle>().Active && CannonE.IsReady() || HammerE.IsReady())
            {
                Helper.SkillDraw(RangeQ, Config.Item("e.draw").GetValue<Circle>().Color, 5);
            }
        }
    }
}
