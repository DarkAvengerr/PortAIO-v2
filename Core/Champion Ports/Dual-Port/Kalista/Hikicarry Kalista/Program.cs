using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DetuksSharp;
using LeagueSharp.Common;
using LeagueSharp;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HikiCarry_Kalista
{
    class Program
    {
        public static Menu Config;
        public static Spell Q, W, E, R;
        private static AIHeroClient Kalista = ObjectManager.Player;
        public static Orbwalking.Orbwalker Orbwalker;

        public static void Game_OnGameLoad()
        {
            if (Kalista.CharData.BaseSkinName != "Kalista")
            {
                return;
            }

            Q = new Spell(SpellSlot.Q, 1150f);
            W = new Spell(SpellSlot.W, 5000f);
            E = new Spell(SpellSlot.E, 1000f);
            R = new Spell(SpellSlot.R, 1500f);

            Q.SetSkillshot(0.25f, 40f, 1200f, true, SkillshotType.SkillshotLine);

            Config = new Menu("HikiCarry - Kalista", "HikiCarry - Kalista", true);
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker Settings"));

            var comboMenu = new Menu("Combo Settings", "Combo Settings");
            {
                comboMenu.AddItem(new MenuItem("qCombo", "Use Q").SetValue(true));
                comboMenu.AddItem(new MenuItem("eCombo", "Use E").SetValue(true));
                comboMenu.AddItem(new MenuItem("combo", "Combo!", true).SetValue(new KeyBind(32, KeyBindType.Press)));
                Config.AddSubMenu(comboMenu);
            }

            var harassMenu = new Menu("Harass Settings", "Harass Settings");
            {
                harassMenu.AddItem(new MenuItem("qHarass", "Use Q").SetValue(true));
                harassMenu.AddItem(new MenuItem("eHarass", "Use E").SetValue(true));
                harassMenu.AddItem(new MenuItem("eSpearCount", "If Enemy Spear Count >= ").SetValue(new Slider(3, 0, 10)));
                harassMenu.AddItem(new MenuItem("manaHarass", "Harass Mana Manager").SetValue(new Slider(20, 0, 100)));
                Config.AddSubMenu(harassMenu);
            }

            var laneMenu = new Menu("LaneClear Settings", "LaneClear Settings");
            {
                laneMenu.AddItem(new MenuItem("eClear", "Use E").SetValue(true));
                laneMenu.AddItem(new MenuItem("eClearCount", "If Can Kill Minion >= ").SetValue(new Slider(2, 1, 5)));
                laneMenu.AddItem(new MenuItem("manaClear", "Clear Mana Manager").SetValue(new Slider(20, 0, 100)));
                Config.AddSubMenu(laneMenu);
            }

            var jungMenu = new Menu("JungleClear Settings", "JungleClear Settings");
            {
                jungMenu.AddItem(new MenuItem("qJungle", "Use Q").SetValue(true));
                jungMenu.AddItem(new MenuItem("eJungle", "Use E").SetValue(true));
                jungMenu.AddItem(new MenuItem("manaJungle", "Jungle Mana Manager").SetValue(new Slider(20, 0, 100)));
                Config.AddSubMenu(jungMenu);
            }

            var ksMenu = new Menu("KillSteal Settings", "KillSteal Settings");
            {
                ksMenu.AddItem(new MenuItem("qKS", "Use Q").SetValue(true));
                ksMenu.AddItem(new MenuItem("eKS", "Use E").SetValue(true));
                Config.AddSubMenu(ksMenu);
            }

            var itemMenu = new Menu("Item Settings", "Item Settings");
            {
                var qssMenu = new Menu("QSS Settings", "QSS Settings");
                {
                    qssMenu.AddItem(new MenuItem("use.qss", "Use QSS").SetValue(true));
                    qssMenu.AddItem(new MenuItem("clear.ignite", "Clear Ignite").SetValue(true));
                    qssMenu.AddItem(new MenuItem("clear.exhaust", "Clear Exhaust").SetValue(true));
                    qssMenu.AddItem(new MenuItem("clear.zedult", "Clear Zed R").SetValue(true));
                    qssMenu.AddItem(new MenuItem("clear.fizzult", "Clear Fizz R").SetValue(true));
                    qssMenu.AddItem(new MenuItem("clear.malzaharult", "Clear Malzahar R").SetValue(true));
                    qssMenu.AddItem(new MenuItem("clear.vladulti", "Clear Vladimir R").SetValue(true));
                    itemMenu.AddSubMenu(qssMenu);
                }

                var botrk = new Menu("BOTRK Settings", "BOTRK Settings");
                {
                    botrk.AddItem(new MenuItem("useBOTRK", "Use BOTRK").SetValue(true));
                    botrk.AddItem(new MenuItem("myhp", "Use if my HP < %").SetValue(new Slider(20, 0, 100)));
                    botrk.AddItem(new MenuItem("theirhp", "Use if enemy HP < %").SetValue(new Slider(20, 0, 100)));
                    itemMenu.AddSubMenu(botrk);
                }

                var ghostBlade = new Menu("GhostBlade Settings", "GhostBlade Settings");
                {
                    ghostBlade.AddItem(new MenuItem("gBlade", "Use GhostBlade").SetValue(true));
                    itemMenu.AddSubMenu(ghostBlade);
                }

                var bilgewater = new Menu("Bilgewater Settings", "Bilgewater Settings");
                {
                    bilgewater.AddItem(new MenuItem("useBilge", "Use BOTRK").SetValue(true));
                    bilgewater.AddItem(new MenuItem("myhpbilge", "Use if my HP < %").SetValue(new Slider(20, 0, 100)));
                    bilgewater.AddItem(new MenuItem("theirhpbilge", "Use if enemy HP < %").SetValue(new Slider(20, 0, 100)));
                    itemMenu.AddSubMenu(bilgewater);
                }
                Config.AddSubMenu(itemMenu);
            }

            var miscMenu = new Menu("Miscellaneous", "Miscellaneous");
            {
                var lastJoke = new Menu("Last Joke Settings", "Last Joke Settings");
                {
                    lastJoke.AddItem(new MenuItem("last.joke", "Last Joke").SetValue(true));
                    lastJoke.AddItem(new MenuItem("last.joke.hp", "Kalista HP Percent").SetValue(new Slider(2, 1, 99)));
                    miscMenu.AddSubMenu(lastJoke);
                }

                var orbSet = new Menu("Scrying Orb Settings", "Scrying Orb Settings");
                {
                    orbSet.AddItem(new MenuItem("bT", "Auto Scrying Orb Buy!").SetValue(true));
                    orbSet.AddItem(new MenuItem("bluetrinketlevel", "Scrying Orb Buy Level").SetValue(new Slider(6, 0, 18)));
                    miscMenu.AddSubMenu(orbSet);
                }
                miscMenu.AddItem(new MenuItem("qImmobile", "Auto Q to Immobile Target").SetValue(true));
                Config.AddSubMenu(miscMenu);
            }
            var wCombo = new Menu("Wombo Combo with R", "Wombo Combo with R"); // beta
            {
                var balista = new Menu("Balista", "Balista");
                {
                    balista.AddItem(new MenuItem("use.balista", "Balista Active").SetValue(true));
                    balista.AddItem(new MenuItem("balista.maxrange", "Balista Max Range").SetValue(new Slider(700, 100, 1500)));
                    balista.AddItem(new MenuItem("balista.minrange", "Balista Min Range").SetValue(new Slider(700, 100, 1500)));
                    wCombo.AddSubMenu(balista);
                }
                var skalista = new Menu("Skalista", "Skalista");
                {
                    skalista.AddItem(new MenuItem("use.skalista", "SKalista Active").SetValue(true));
                    skalista.AddItem(new MenuItem("skalista.maxrange", "SKalista Max Range").SetValue(new Slider(700, 100, 1500)));
                    skalista.AddItem(new MenuItem("skalista.minrange", "SKalista Min Range").SetValue(new Slider(700, 100, 1500)));
                    wCombo.AddSubMenu(skalista);
                }
            }
            Config.AddSubMenu(wCombo);

            var drawMenu = new Menu("Draw Settings", "Draw Settings");
            {
                drawMenu.AddItem(new MenuItem("qDraw", "Q Range").SetValue(new Circle(true, Color.White)));
                drawMenu.AddItem(new MenuItem("wDraw", "W Range").SetValue(new Circle(true, Color.Silver)));
                drawMenu.AddItem(new MenuItem("eDraw", "E Range").SetValue(new Circle(true, Color.Yellow)));
                drawMenu.AddItem(new MenuItem("rDraw", "R Range").SetValue(new Circle(true, Color.Gold)));
                drawMenu.AddItem(new MenuItem("ePercent", "E % On Enemy").SetValue(new Circle(true, Color.Gold)));
                drawMenu.AddItem(new MenuItem("e.percent.jungle.mobs", "E % On Jungle Mobs").SetValue(new Circle(true, Color.Chartreuse)));
                drawMenu.AddItem(new MenuItem("signal", "Support Signal").SetValue(true));
                drawMenu.AddItem(new MenuItem("circleSupport", "Draw Support on Circle").SetValue(true));
                Config.AddSubMenu(drawMenu);
            }

            Config.AddItem(new MenuItem("saveSupport", "Save Support [R]").SetValue(true));
            Config.AddItem(new MenuItem("savePercent", "Save Support Health Percent").SetValue(new Slider(10, 1, 99)));
            Config.AddItem(new MenuItem("calculator", "E Damage Calculator").SetValue(new StringList(new[] { "Custom Calculator", "Common Calculator" }))); 

            var drawDamageMenu = new MenuItem("RushDrawEDamage", "E Damage").SetValue(true);
            var drawFill = new MenuItem("RushDrawEDamageFill", "E Damage Fill").SetValue(new Circle(true, Color.Gold));

            drawMenu.SubMenu("Damage Draws").AddItem(drawDamageMenu);
            drawMenu.SubMenu("Damage Draws").AddItem(drawFill);

            DamageIndicator.DamageToUnit = Calculators.ChampionTotalDamage;
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
            Chat.Print("<font color='#ff3232'>HikiCarry Kalista 1.3.3.7: </font> <font color='#d4d4d4'>If you like this assembly feel free to upvote on Assembly DB</font>");
            Orbwalking.AfterAttack += AfterAttack;
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;

        }

        private static void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (ObjectManager.Player.IsDead && ObjectManager.Player.IsZombie)
            {
                return;
            }
            if (Config.Item("gBlade").GetValue<bool>() && Config.Item("combo", true).GetValue<KeyBind>().Active)
            {
                Activator.Youmuu();
            }
            if (Config.Item("useBOTRK").GetValue<bool>())
            {
                Activator.Blade(target,Config.Item("theirhp").GetValue<Slider>().Value,Config.Item("myhp").GetValue<Slider>().Value);
            }
            if (Config.Item("useBilge").GetValue<bool>())
            {
                Activator.Blade(target, Config.Item("theirhpbilge").GetValue<Slider>().Value, Config.Item("myhpbilge").GetValue<Slider>().Value);
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
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
            if (Config.Item("use.balista").GetValue<bool>()) 
            {
                Helper.Balista(Config.Item("balista.minrange").GetValue<Slider>().Value, Config.Item("balista.maxrange").GetValue<Slider>().Value,R);
            }
            if (Config.Item("use.skalista").GetValue<bool>())
            {
                Helper.SKalista(Config.Item("skalista.minrange").GetValue<Slider>().Value, Config.Item("skalista.maxrange").GetValue<Slider>().Value, R);
            }
            if (Config.Item("qKS").GetValue<bool>())
            {
                Helper.KillStealWithPierce(HitChance.High);
            }
            if (Config.Item("eKS").GetValue<bool>())
            {
                Helper.KillStealWithRend();
            }
            if (Config.Item("qImmobile").GetValue<bool>())
            {
                Helper.ImmobilePierce(HitChance.High, 0);
            }
            if (Config.Item("bT").GetValue<bool>())
            {
                Helper.BlueOrb(Config.Item("bluetrinketlevel").GetValue<Slider>().Value);
            }
            if (Config.Item("use.qss").GetValue<bool>())
            {
                Activator.QuickSilver("clear.ignite", "clear.exhaust", "clear.zedult", "clear.fizzult", "clear.malzaharult", "clear.vladulti");
            }
            if (Config.Item("saveSupport").GetValue<bool>())
            {
                Helper.SupportProtector(R);
            }
        }

        private static void Combo()
        {
            if (Q.IsReady() && Config.Item("qCombo").GetValue<bool>())
            {
                Helper.PierceCombo(0,HitChance.VeryHigh);
            }
            if (E.IsReady() && Config.Item("eCombo").GetValue<bool>())
            {
                Helper.RendCombo();
            }
        }

        private static void Harass()
        {
            if (Kalista.ManaPercent < Config.Item("manaHarass").GetValue<Slider>().Value)
            {
                return;
            }
            if (Q.IsReady() && Config.Item("qHarass").GetValue<bool>())
            {
                Helper.PierceCombo(0, HitChance.VeryHigh);
            }
            if (E.IsReady() && Config.Item("eHarass").GetValue<bool>())
            {
                Helper.RendHarass(Config.Item("eSpearCount").GetValue<Slider>().Value);
            }
        }

        private static void Clear()
        {
            if (Kalista.ManaPercent < Config.Item("manaClear").GetValue<Slider>().Value)
            {
                return;
            }
            if (E.IsReady() && Config.Item("eClear").GetValue<bool>())
            {
                Helper.RendClear(Config.Item("eClearCount").GetValue<Slider>().Value);
            }
        }

        private static void Jungle()
        {
            if (Kalista.ManaPercent < Config.Item("manaJungle").GetValue<Slider>().Value)
            {
                return;
            }
            if (Q.IsReady() && Config.Item("qJungle").GetValue<bool>())
            {
                Helper.PierceJungleClear(Q,HitChance.VeryHigh);
            }
            if (E.IsReady() && Config.Item("eJungle").GetValue<bool>())
            {
                Helper.RendJungleClear();
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Config.Item("qDraw").GetValue<Circle>().Active && Q.IsReady())
            {
                Draws.SkillDraw(Q, Config.Item("qDraw").GetValue<Circle>().Color,5);
            }
            if (Config.Item("wDraw").GetValue<Circle>().Active && W.IsReady())
            {
                Draws.SkillDraw(W, Config.Item("wDraw").GetValue<Circle>().Color, 5);
            }
            if (Config.Item("eDraw").GetValue<Circle>().Active && E.IsReady())
            {
                Draws.SkillDraw(E, Config.Item("eDraw").GetValue<Circle>().Color, 5);
            }
            if (Config.Item("rDraw").GetValue<Circle>().Active && R.IsReady())
            {
                Draws.SkillDraw(R, Config.Item("rDraw").GetValue<Circle>().Color, 5);
            }
            if (Config.Item("ePercent").GetValue<Circle>().Active && E.IsReady())
            {
                Draws.EPercentOnEnemy(Config.Item("ePercent").GetValue<Circle>().Color);
            }
            if (Config.Item("e.percent.jungle.mobs").GetValue<Circle>().Active && E.IsReady())
            {
                Draws.EPercentOnJungleMobs(Config.Item("e.percent.jungle.mobs").GetValue<Circle>().Color);
            }
            if (Config.Item("signal").GetValue<Circle>().Active)
            {
                Draws.ConnectionSignal(Config.Item("signal").GetValue<Circle>().Color);
            }
            if (Config.Item("circleSupport").GetValue<Circle>().Active)
            {
                Draws.CircleOnSupport(Config.Item("circleSupport").GetValue<Circle>().Color);
            }
        }
    }
}
