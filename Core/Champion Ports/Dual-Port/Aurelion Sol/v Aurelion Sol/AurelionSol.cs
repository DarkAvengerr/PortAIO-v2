using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SPrediction;
using System.Drawing;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace vAurelionSol
{
    public class AurelionSol
    {
        public static Menu Config;
        public static string menuName = "vAurelionSol";
        public static Spell Q, W, W2, E, R;
        public static float Outerlayer = 600f;
        public static Orbwalking.Orbwalker Orbwalker;
        private static Obj_AI_Base Player = ObjectManager.Player;

        public static string[] HitchanceNameArray = { "Low", "Medium", "High", "Very High", "Only Immobile" };
        public static HitChance[] HitchanceArray = { HitChance.Low, HitChance.Medium, HitChance.High, HitChance.VeryHigh, HitChance.Immobile };

        public static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName != "AurelionSol")
            {
                return;
            }

            Q = new Spell(SpellSlot.Q, 600f);
            W = new Spell(SpellSlot.W, 600f);
            E = new Spell(SpellSlot.E, 3000f);
            R = new Spell(SpellSlot.R, 1600f);

            Q.SetSkillshot(0.25f, 110f, 850f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.30f, 120f, 4500f, false, SkillshotType.SkillshotLine);

            Config = new Menu(menuName, menuName, true);
            {
                Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker Settings"));

                var combo = new Menu(":: Combo Settings", ":: Combo Settings");
                {
                    combo.AddItem(new MenuItem("combo.q", "Use Q").SetValue(true));
                    combo.AddItem(new MenuItem("combo.w", "Use W when out of Range").SetValue(true));
                    combo.AddItem(new MenuItem("combo.r", "Use R").SetValue(true));
                    // combo.AddItem(new MenuItem("combo.r.min", "Min. Enemies to use R").SetValue(new Slider(2, 1, 5)));

                    Config.AddSubMenu(combo);
                }

                var lane = new Menu(":: Lane Settings", ":: Lane Settings");
                {
                    lane.AddItem(new MenuItem("lane.q", "Use Q").SetValue(true));
                    lane.AddItem(new MenuItem("lane.min.minions", "Min. Minions to Q").SetValue(new Slider(3, 1, 8)));
                    lane.AddItem(new MenuItem("lane.mana", "Min. Mana Percent").SetValue(new Slider(30, 1, 99)));

                    Config.AddSubMenu(lane);
                }

                var jungle = new Menu(":: Jungle Settings", ":: Jungle Settings");
                {
                    jungle.AddItem(new MenuItem("jungle.q", "Use Q").SetValue(true));
                    jungle.AddItem(new MenuItem("jungle.mana", "Min. Mana Percent").SetValue(new Slider(30, 1, 99)));

                    Config.AddSubMenu(jungle);
                }

                /*
                var lasthit = new Menu(":: Lasthit Settings", ":: Lasthit Settings");
                {
                    lasthit.AddItem(new MenuItem("lasthit.q", "Use Q").SetValue(true));
                    lasthit.AddItem(new MenuItem("lasthit.mana", "Min. Mana Percent").SetValue(new Slider(30, 1, 99)));

                    Config.AddSubMenu(lasthit);
                }
                */

                var killsteal = new Menu(":: KS Settings", ":: KS Settings");
                {
                    killsteal.AddItem(new MenuItem("ks.q", "Use Q").SetValue(true));
                    killsteal.AddItem(new MenuItem("ks.r", "Use R").SetValue(true));

                    Config.AddSubMenu(killsteal);
                }

                var misc = new Menu(":: Misc Settings", ":: Misc Settings");
                {
                    misc.AddItem(new MenuItem("inter.q", "Interrupt (Q)").SetValue(true));
                    misc.AddItem(new MenuItem("gap.q", "Gapclose (Q)").SetValue(true));

                    Config.AddSubMenu(misc);
                }

                var draw = new Menu(":: Draw Settings", ":: Draw Settings");
                {
                    draw.AddItem(new MenuItem("draw.q", "Q Range").SetValue(new Circle(true, Color.Chartreuse)));
                    draw.AddItem(new MenuItem("draw.w", "W Range").SetValue(new Circle(true, Color.Yellow)));
                    draw.AddItem(new MenuItem("draw.e", "E Range").SetValue(new Circle(true, Color.White)));
                    draw.AddItem(new MenuItem("draw.r", "R Range").SetValue(new Circle(true, Color.SandyBrown)));

                    Config.AddSubMenu(draw);
                }

                Config.AddItem(new MenuItem("sol.hitchance", "Skillshot Hitchance").SetValue(new StringList(HitchanceNameArray, 2)));

                var debug = new Menu(":: Debugging", ":: Debugging");
                {
                    debug.AddItem(new MenuItem("debug", "Debug Mode").SetValue(false));
                    debug.AddItem(new MenuItem("debugmouse", "Debug Mode [Mouse]").SetValue(false));
                    debug.AddItem(new MenuItem("warning", "WARNING: May cause FPS issues"));

                    Config.AddSubMenu(debug);
                }
            }

            SPrediction.Prediction.Initialize(Config, ":: Prediction Settings");
            Config.AddToMainMenu();

            Game.OnUpdate += Game_OnUpdate;
            Interrupter2.OnInterruptableTarget += SolInterrupter;
            AntiGapcloser.OnEnemyGapcloser += SolGapcloser;
            Drawing.OnDraw += SolDraw;

            Obj_AI_Base.OnSpellCast += OnProcessSpellCast;

            Chat.Print("[00:00] vAurelionSol");
            Chat.Print("[00:00] The first Aurelion Sol assembly available!");
        }

        private static void SolInterrupter(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (Q.IsReady() && sender.IsValidTarget(Q.Range) && MenuCheck("inter.q", Config))
            {
                Q.SPredictionCast(sender, SpellHitChance(Config, "sol.hitchance"));
            }
        }

        private static void SolGapcloser(ActiveGapcloser gapcloser)
        {
            if (Q.IsReady() && gapcloser.Sender.IsValidTarget(Q.Range) && MenuCheck("gap.q", Config))
            {
                Q.SPredictionCast(gapcloser.Sender, SpellHitChance(Config, "sol.hitchance"));
            }
            else if (!Q.IsReady() && R.IsReady() && gapcloser.Sender.IsValidTarget(R.Range) && MenuCheck("gap.r", Config))
            {
                R.SPredictionCast(gapcloser.Sender, SpellHitChance(Config, "sol.hitchance"));
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Laneclear();
                    Jungleclear();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    // Lasthit();
                    Harass();
                    break;
            }
        }

        public static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs arg)
        {
            if (MenuCheck("debug", Config))
            {
                if (sender.IsMe)
                {
                    Chat.Print("Spell: " + arg.SData.Name);
                    Chat.Print("Delay: " + arg.SData.DelayTotalTimePercent.ToString());
                    Chat.Print("Cast Range: " + arg.SData.CastRange.ToString());
                    Chat.Print("Line Width: " + arg.SData.LineWidth.ToString());
                    Chat.Print("Missle Speed: " + arg.SData.MissileSpeed.ToString());
                    Chat.Print("-");
                }
            }
        }

        private static void Combo()
        {
            if (MenuCheck("combo.q", Config) && Q.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range)))
                {
                    Q.Cast(enemy);
                }
            }

            if (MenuCheck("combo.w", Config) && W.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(1000)))
                {
                    if (enemy.Distance(Player.Position) > 320 && !Player.HasBuff("aurelionsolwactive"))
                    {
                        W.Cast();
                    }
                    else if (enemy.Distance(Player.Position) > 900 && Player.HasBuff("aurelionsolwactive"))
                    {
                        W.Cast();
                    }
                    else if (enemy.Distance(Player.Position) < 400 && Player.HasBuff("aurelionsolwactive"))
                    {
                        W.Cast();
                    }
                }
            }

            if (MenuCheck("combo.r", Config) && R.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range) &&
                                    !x.IsDead && !x.IsZombie && !x.HasBuffOfType(BuffType.SpellShield) && !x.HasBuffOfType(BuffType.SpellImmunity)))
                {
                    R.Cast(enemy);
                }
            }
        }

        private static void Laneclear()
        {
            var qMin = MinionManager.GetMinions(Player.ServerPosition, Q.Range + Q.Width);
            var qPos = Q.GetCircularFarmLocation(qMin, Q.Width);

            if (Player.ManaPercent <= SliderCheck("lane.mana", Config))
            {
                return;
            }
            
            if (MenuCheck("lane.q", Config) && Q.IsReady())
            {
                if (qPos.MinionsHit >= 2)
                {
                    Q.Cast(qPos.Position);
                }
            }
        }

        private static void Jungleclear()
        {
            var camp = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            var pred = Q.GetCircularFarmLocation(camp);

            if (Player.ManaPercent <= SliderCheck("jungle.mana", Config))
            {
                return;
            }

            if (MenuCheck("jungle.q", Config) && Q.IsReady())
            {
                if (pred.MinionsHit >= 1)
                {
                    Q.Cast(pred.Position);
                }
            }
        }

        /*
        private static void Lasthit()
        {
            // Need to finish this 
        }
        */

        private static void Harass()
        {
            if (MenuCheck("harass.q", Config) && Q.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range)))
                {
                    Q.Cast(enemy);
                }
            }

            if (MenuCheck("harass.w", Config) && W.IsReady() && !MenuCheck("auto.w", Config))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(1000)))
                {
                    if (enemy.Distance(Player.Position) > 400 && !Player.HasBuff("aurelionsolwactive"))
                    {
                        W.Cast();
                    }
                    else if (enemy.Distance(Player.Position) > 900 && Player.HasBuff("aurelionsolwactive"))
                    {
                        W.Cast();
                    }
                    else if (enemy.Distance(Player.Position) < 400 && Player.HasBuff("aurelionsolwactive"))
                    {
                        W.Cast();
                    }
                }
            }
        }

        private static void Killsteal()
        {
            foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range) &&
                    !x.IsDead && !x.IsZombie && !x.HasBuffOfType(BuffType.SpellShield) && !x.HasBuffOfType(BuffType.SpellImmunity)))
            {
                if (MenuCheck("ks.q", Config) && Q.IsReady() && enemy.Health < Q.GetDamage(enemy) && enemy.IsValidTarget(Q.Range))
                {
                    Q.SPredictionCast(enemy, SpellHitChance(Config, "sol.hitchance"));
                }

                if (MenuCheck("ks.r", Config) && R.IsReady() && enemy.Health < R.GetDamage(enemy) && enemy.IsValidTarget(R.Range))
                {
                    R.Cast(enemy);
                }
            }
        }

        private static void SolDraw(EventArgs args)
        {
            if (Q.IsReady() && ActiveCheck("draw.q", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, GetColor("draw.q", Config));
            }

            if (W.IsReady() && MenuCheck("draw.w", Config))
            {
                Render.Circle.DrawCircle(Player.Position, W.Range, GetColor("draw.w", Config));
            }

            if (E.IsReady() && MenuCheck("draw.e", Config))
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, GetColor("draw.e", Config));
            }

            if (R.IsReady() && MenuCheck("draw.r", Config))
            {
                Render.Circle.DrawCircle(Player.Position, R.Range, GetColor("draw.r", Config));
            }
        }

        public static HitChance SpellHitChance(Menu menu, string menuname)
        {
            return HitchanceArray[menu.Item(menuname).GetValue<StringList>().SelectedIndex];
        }

        public static bool MenuCheck(string menuName, Menu menu)
        {
            return menu.Item(menuName).GetValue<bool>();
        }

        public static int SliderCheck(string menuName, Menu menu)
        {
            return menu.Item(menuName).GetValue<Slider>().Value;
        }

        public static bool ActiveCheck(string menuName, Menu menu)
        {
            return menu.Item(menuName).GetValue<Circle>().Active;
        }

        public static Color GetColor(string menuName, Menu menu)
        {
            return menu.Item(menuName).GetValue<Circle>().Color;
        }
    }
}
