using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HikiCarry_Elise
{
    class Program
    {
        public const string ChampionName = "Elise";
        public static Orbwalking.Orbwalker Orbwalker;
        public static List<Spell> SpellList = new List<Spell>();
        public static Menu Config;
        private static AIHeroClient Player = ObjectManager.Player;
        private static SpellSlot Flash;

        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static Spell SpiderQ;
        public static Spell SpiderW;
        public static Spell SpiderE;

        public static float rangeQ;
        public static float rangeW;
        public static float rangeE;

        private static Items.Item IronSolari; 
        private static Items.Item Zhonya;
        private static Items.Item Randuin;

        private static readonly int[] Smites = { 3713, 3726, 3725, 3724, 3723, 3933,
                                                   3711, 3722, 3721, 3720, 3719, 3932, 3715,  3718, 
                                                   3717, 3716, 3714, 3931, 3706, 3710, 3709, 3708, 3707, 3930 };

        private static bool humansexygirl;
        private static bool spidergirl;

        public static void Game_OnGameLoad()
        {
            if (Player.BaseSkinName != ChampionName) return;

            Q = new Spell(SpellSlot.Q, 625f);
            W = new Spell(SpellSlot.W, 950f);
            E = new Spell(SpellSlot.E, 1075f);
            R = new Spell(SpellSlot.R,0);
            SpiderQ = new Spell(SpellSlot.Q, 475f);
            SpiderW = new Spell(SpellSlot.W);
            SpiderE = new Spell(SpellSlot.E, 750f);

            W.SetSkillshot(0.25f, 100f, 1000, true, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 55f, 1300, true, SkillshotType.SkillshotLine);

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);
            SpellList.Add(SpiderQ);
            SpellList.Add(SpiderW);
            SpellList.Add(SpiderE);

            Zhonya = new Items.Item(3157, 10);
            IronSolari = new Items.Item(3190, 590f);
            Randuin = new Items.Item(3143, 490f);

            Config = new Menu("HikiCarry - Elise", "HikiCarry - Elise", true);

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            Config.AddSubMenu(new Menu("Orbwalker Settings", "Orbwalker Settings"));
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker Settings"));

            Config.AddSubMenu(new Menu("Combo Settings", "Combo Settings"));
            Config.SubMenu("Combo Settings").AddItem(new MenuItem("qCombo", "Use Q [Human]").SetValue(true));
            Config.SubMenu("Combo Settings").AddItem(new MenuItem("wCombo", "Use W [Human]").SetValue(true));
            Config.SubMenu("Combo Settings").AddItem(new MenuItem("eCombo", "Use E [Human]").SetValue(true));
            Config.SubMenu("Combo Settings").AddItem(new MenuItem("rCombo", "Auto Switch Form").SetValue(true));
            Config.SubMenu("Combo Settings").AddItem(new MenuItem("qCombo.Spider", "Use Q [Spider]").SetValue(true));
            Config.SubMenu("Combo Settings").AddItem(new MenuItem("wCombo.Spider", "Use W [Spider]").SetValue(true));
            Config.SubMenu("Combo Settings").AddItem(new MenuItem("eCombo.Spider", "Use E [Spider]").SetValue(true));


            Config.AddSubMenu(new Menu("Harass Settings", "Harass Settings"));
            Config.SubMenu("Harass Settings").AddItem(new MenuItem("qHarass", "Use Q [Human]").SetValue(true));
            Config.SubMenu("Harass Settings").AddItem(new MenuItem("wHarass", "Use W [Human]").SetValue(true));
            Config.SubMenu("Harass Settings").AddItem(new MenuItem("manaHarass", "Harass Mana Percent").SetValue(new Slider(30, 1, 100)));

            Config.AddSubMenu(new Menu("Jungle Settings", "Jungle Settings"));
            Config.SubMenu("Jungle Settings").AddItem(new MenuItem("stealset", "            Steal Settings"));
            Config.SubMenu("Jungle Settings").AddItem(new MenuItem("jungsteal", "Jungle Steal").SetValue(true));
            Config.SubMenu("Jungle Settings").AddItem(new MenuItem("baronsteal", "Baron Steal [Q]").SetValue(true));
            Config.SubMenu("Jungle Settings").AddItem(new MenuItem("drakesteal", "Dragon Steal [Q]").SetValue(true));
            Config.SubMenu("Jungle Settings").AddItem(new MenuItem("redsteal", "Red Steal [Q]").SetValue(true));
            Config.SubMenu("Jungle Settings").AddItem(new MenuItem("bluesteal", "Blue Steal [Q]").SetValue(true));
            //clear
            Config.SubMenu("Jungle Settings").AddItem(new MenuItem("clearset", "            Clear Settings"));
            Config.SubMenu("Jungle Settings").AddItem(new MenuItem("qjClear", "Use Q [Human]").SetValue(true));
            Config.SubMenu("Jungle Settings").AddItem(new MenuItem("wjClear", "Use W [Human]").SetValue(true));
            Config.SubMenu("Jungle Settings").AddItem(new MenuItem("qjClear.Spider", "Use Q [Spider]").SetValue(true));
            Config.SubMenu("Jungle Settings").AddItem(new MenuItem("wjClear.Spider", "Use W [Spider]").SetValue(true));
            Config.SubMenu("Jungle Settings").AddItem(new MenuItem("autoswitch", "Auto Switch").SetValue(true));
            
            Config.AddSubMenu(new Menu("Misc Settings", "Misc Settings"));
            Config.SubMenu("Misc Settings").AddItem(new MenuItem("agapcloser", "Anti-Gapcloser [Human E]").SetValue(true));
            Config.SubMenu("Misc Settings").AddItem(new MenuItem("agapcloser2", "Anti-Gapcloser [Spider E]").SetValue(true));
            Config.SubMenu("Misc Settings").AddItem(new MenuItem("ainterrupt", "Auto Interrupt Active! [Human E]").SetValue(true));
            Config.SubMenu("Misc Settings").AddItem(new MenuItem("ainterrupt2", "Auto Interrupt Active! [Spider E]").SetValue(true));

            Config.AddSubMenu(new Menu("KillSteal Settings", "KillSteal Settings"));
            Config.SubMenu("KillSteal Settings").AddItem(new MenuItem("humanKSQ", "Killsteal Q [Human]").SetValue(true));
            Config.SubMenu("KillSteal Settings").AddItem(new MenuItem("spiderKSQ", "Killsteal Q [Spider]").SetValue(true));
            Config.SubMenu("KillSteal Settings").AddItem(new MenuItem("humanKSW", "Killsteal W [Human]").SetValue(true));
            
            Config.AddSubMenu(new Menu("Items Settings", "Items Settings"));
            //////////
            Config.SubMenu("Items Settings").AddSubMenu(new Menu("Randuin Omen Settings", "Randuin Omen Settings"));
            Config.SubMenu("Items Settings").SubMenu("Randuin Omen Settings").AddItem(new MenuItem("useRanduin", "Use Randuin").SetValue(true));
            Config.SubMenu("Items Settings").SubMenu("Randuin Omen Settings").AddItem(new MenuItem("randuinCount", "If Enemy Count >=").SetValue(new Slider(2, 1, 5)));
            /////////
            Config.SubMenu("Items Settings").AddSubMenu(new Menu("Zhonya Settings", "Zhonya Settings"));
            Config.SubMenu("Items Settings").SubMenu("Zhonya Settings").AddItem(new MenuItem("useZhonya", "Use Zhonya").SetValue(true));
            Config.SubMenu("Items Settings").SubMenu("Zhonya Settings").AddItem(new MenuItem("zhonyaMyHp", "If My Hp >= %").SetValue(new Slider(10, 0, 100)));
            ///////////
            Config.SubMenu("Items Settings").AddSubMenu(new Menu("Iron Solari Settings", "Iron Solari Settings"));
            Config.SubMenu("Items Settings").SubMenu("Iron Solari Settings").AddItem(new MenuItem("useSolari", "Use Iron Solari").SetValue(true));
            Config.SubMenu("Items Settings").SubMenu("Iron Solari Settings").AddItem(new MenuItem("ironsolariAllyHp", "If Ally Hp >= %").SetValue(new Slider(20, 0, 100)));

            Config.AddSubMenu(new Menu("Draw Settings", "Draw Settings"));
            Config.SubMenu("Draw Settings").AddItem(new MenuItem("qDraw", "Q Range").SetValue(new Circle(true, Color.White)));
            Config.SubMenu("Draw Settings").AddItem(new MenuItem("wDraw", "W Range").SetValue(new Circle(true, Color.White)));
            Config.SubMenu("Draw Settings").AddItem(new MenuItem("eDraw", "E Range").SetValue(new Circle(true, Color.White)));

            Config.AddToMainMenu();

            Game.OnUpdate += Game_OnGameUpdate;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (humansexygirl)
            {
                if (Config.Item("ainterrupt").GetValue<bool>())
                {
                    if (sender.LSIsValidTarget(1000))
                    {
                        Render.Circle.DrawCircle(sender.Position, sender.BoundingRadius, Color.Gold, 5);
                        var targetpos = Drawing.WorldToScreen(sender.Position);
                        Drawing.DrawText(targetpos[0] - 40, targetpos[1] + 20, Color.Gold, "Interrupt");
                    }
                    if (E.CanCast(sender))
                    {
                        E.Cast(sender);
                    }
                }
            }
            if (spidergirl)
            {
                if (Config.Item("ainterrupt2").GetValue<bool>())
                {
                    if (sender.LSIsValidTarget(1000))
                    {
                        Render.Circle.DrawCircle(sender.Position, sender.BoundingRadius, Color.Gold, 5);
                        var targetpos = Drawing.WorldToScreen(sender.Position);
                        Drawing.DrawText(targetpos[0] - 40, targetpos[1] + 20, Color.Gold, "Interrupt");
                    }
                    if (SpiderE.CanCast(sender))
                    {
                        SpiderE.Cast(sender);
                    }
                }
            }
        }
        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (humansexygirl)
            {
                if (Config.Item("agapcloser").GetValue<bool>())
                {
                    if (gapcloser.Sender.LSIsValidTarget(1000))
                    {
                        Render.Circle.DrawCircle(gapcloser.Sender.Position, gapcloser.Sender.BoundingRadius, Color.Gold, 5);
                        var targetpos = Drawing.WorldToScreen(gapcloser.Sender.Position);
                        Drawing.DrawText(targetpos[0] - 40, targetpos[1] + 20, Color.Gold, "Gapcloser");
                    }
                    if (E.CanCast(gapcloser.Sender))
                    {
                        E.Cast(gapcloser.Sender);
                    }
                }
            }
            if (spidergirl)
            {
                if (Config.Item("agapcloser2").GetValue<bool>())
                {
                    if (gapcloser.Sender.LSIsValidTarget(1000))
                    {
                        Render.Circle.DrawCircle(gapcloser.Sender.Position, gapcloser.Sender.BoundingRadius, Color.Gold, 5);
                        var targetpos = Drawing.WorldToScreen(gapcloser.Sender.Position);
                        Drawing.DrawText(targetpos[0] - 40, targetpos[1] + 20, Color.Gold, "Gapcloser");
                    }
                    if (E.CanCast(gapcloser.Sender))
                    {
                        E.Cast(gapcloser.Sender);
                    }
                } 
            }
        }
        private static void Game_OnGameUpdate(EventArgs args)
        {
            spiderCheck();
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Combo();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                Harass();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                Jungle();
            }
            Items();
            KillSteal();
        }
        private static void KillSteal()
        {
            var spiderQKS = Config.Item("spiderKSQ").GetValue<bool>();
            var humanQKS = Config.Item("humanKSQ").GetValue<bool>();
            var humanWKS = Config.Item("humanKSW").GetValue<bool>();

            foreach (var target in HeroManager.Enemies.OrderByDescending(x => x.Health))
            {
                if (spidergirl)
                {
                    if (SpiderQ.LSIsReady() && spiderQKS && SpiderQ.CanCast(target) && SpiderQ.IsKillable(target))
                    {
                        SpiderQ.Cast(target);
                    }   
                }
                if (humansexygirl)
                {
                    if (Q.LSIsReady() && humanQKS && Q.CanCast(target) && Q.IsKillable(target))
                    {
                        Q.Cast(target);
                    }
                    if (W.LSIsReady() && humanWKS && Q.CanCast(target) && W.IsKillable(target))
                    {
                        W.Cast(target);
                    }
                }
            }
        }
        private static void Items()
        {
            if (Config.Item("useRanduin").GetValue<bool>())
            {
                if (Player.LSCountEnemiesInRange(400) >= Config.Item("randuinCount").GetValue<Slider>().Value)
                {
                    Randuin.Cast();
                }
            }
            if (Config.Item("useZhonya").GetValue<bool>())
            {
                if (Player.HealthPercent <= Config.Item("zhonyaMyHp").GetValue<Slider>().Value)
                {
                    Zhonya.Cast();
                }
            }
            if (Config.Item("useSolari").GetValue<bool>())
            {
                foreach (var ally in HeroManager.Allies)
                {
                    if (!ally.IsMe && !ally.LSIsRecalling() && ally.HealthPercent <= Config.Item("ironsolariAllyHp").GetValue<Slider>().Value)
                    {
                        IronSolari.Cast();
                    }
                }
            }
        }
        private static void Jungle()
        {
            var mobs = MinionManager.GetMinions(Player.ServerPosition, Orbwalking.GetRealAutoAttackRange(Player) + 100, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            if (mobs == null || (mobs != null && mobs.Count == 0))
            {
                return;
            }
            if (humansexygirl)
            {
                    if (Q.LSIsReady() && Config.Item("qjClear").GetValue<bool>())
                    {
                        Q.Cast(mobs[0]);
                    }
                    if (W.LSIsReady() && Config.Item("wjClear").GetValue<bool>())
                    {
                        W.Cast(mobs[0]);
                    }
                    if (!Q.LSIsReady() && !W.LSIsReady() && Config.Item("autoswitch").GetValue<bool>())
                    {
                        R.Cast();
                    }
            }
            if (spidergirl)
            {
                     if (W.LSIsReady() && Config.Item("wjClear.Spider").GetValue<bool>())
                     {
                         W.Cast();
                     }
                     if (Q.LSIsReady() && Config.Item("qjClear.Spider").GetValue<bool>())
                     {
                         Q.Cast(mobs[0]);
                     }
                     if (!SpiderQ.LSIsReady() && !SpiderW.LSIsReady() && Config.Item("autoswitch").GetValue<bool>())
                     {
                         R.Cast();
                     }
            }
            if (Config.Item("junglesteal").GetValue<bool>()) //jung steal check
            {
                foreach (var minyon in mobs)
                {

                    if (minyon.CharData.BaseSkinName.Contains("SRU_Dragon") && Config.Item("drakesteal").GetValue<bool>()) // dragon steal with spider Q and human Q
                    {
                        if (spidergirl)
                        {
                            if (Q.LSIsReady() && Q.IsKillable(minyon))
                            {
                                SpiderQ.Cast(minyon);
                            }
                        }
                        if (humansexygirl)
                        {
                            if (Q.LSIsReady() && Q.IsKillable(minyon))
                            {
                                Q.Cast(minyon);
                            }
                        }
                    }

                    if (minyon.CharData.BaseSkinName.Contains("SRU_Baron") && Config.Item("baronsteal").GetValue<bool>()) // baron steal with spider Q and human Q
                    {
                        if (spidergirl)
                        {
                            if (Q.LSIsReady() && Q.IsKillable(minyon))
                            {
                                SpiderQ.Cast(minyon);
                            }
                        }
                        if (humansexygirl)
                        {
                            if (Q.LSIsReady() && Q.IsKillable(minyon))
                            {
                                Q.Cast(minyon);
                            }
                        }
                    }
                    if (minyon.CharData.BaseSkinName.Contains("SRU_Red") && Config.Item("redsteal").GetValue<bool>()) // red steal with spider Q and human Q
                    {
                        if (spidergirl)
                        {
                            if (Q.LSIsReady() && Q.IsKillable(minyon))
                            {
                                SpiderQ.Cast(minyon);
                            }
                        }
                        if (humansexygirl)
                        {
                            if (Q.LSIsReady() && Q.IsKillable(minyon))
                            {
                                Q.Cast(minyon);
                            }
                        }
                    }
                    if (minyon.CharData.BaseSkinName.Contains("SRU_Blue") && Config.Item("bluesteal").GetValue<bool>()) // blue steal with spider Q and human Q
                    {
                        if (spidergirl)
                        {
                            if (Q.LSIsReady() && Q.IsKillable(minyon))
                            {
                                SpiderQ.Cast(minyon);
                            }
                        }
                        if (humansexygirl)
                        {
                            if (Q.LSIsReady() && Q.IsKillable(minyon))
                            {
                                Q.Cast(minyon);
                            }
                        }
                    }
                }
            }
        }
        private static void Combo()
        {
            if (humansexygirl)
            {
                if (E.LSIsReady() && Config.Item("eCombo").GetValue<bool>())
                {
                    foreach (var en in HeroManager.Enemies.Where(hero => hero.LSIsValidTarget(E.Range)))
                    {
                        if (E.CanCast(en) && E.GetPrediction(en).Hitchance >= HitChance.Medium && !Player.Spellbook.IsAutoAttacking && !Player.LSIsDashing())
                        {
                            E.Cast(en);
                        }
                    }
                }
                if (Q.LSIsReady() && Config.Item("qCombo").GetValue<bool>())
                {
                    foreach (var en in HeroManager.Enemies.Where(hero => hero.LSIsValidTarget(Q.Range)))
                    {
                        if (Q.CanCast(en))
                        {
                            Q.Cast(en);
                        }
                    }
                }
                if (W.LSIsReady() && Config.Item("wCombo").GetValue<bool>())
                {
                    foreach (var en in HeroManager.Enemies.Where(hero => hero.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(Player))))
                    {
                        if (W.CanCast(en))
                        {
                            W.Cast(en);
                        }
                    }
                }
                if (!Q.LSIsReady() && !W.LSIsReady() && !E.LSIsReady() && R.LSIsReady() &&
                    Config.Item("rCombo").GetValue<bool>())
                {
                    R.Cast();
                }
            }
            if (spidergirl)
            {
                if (SpiderW.LSIsReady() && Config.Item("wCombo.Spider").GetValue<bool>())
                {
                    foreach (var en in HeroManager.Enemies.Where(hero => hero.LSIsValidTarget(Q.Range)))
                    {
                        W.Cast();
                    }
                }
                if (SpiderQ.LSIsReady() && Config.Item("qCombo.Spider").GetValue<bool>())
                {
                    foreach (var en in HeroManager.Enemies.Where(hero => hero.LSIsValidTarget(Q.Range)))
                    {
                        Q.Cast(en);
                    }
                }
                if (SpiderE.LSIsReady() && Config.Item("qCombo.Spider").GetValue<bool>())
                {
                    foreach (var en in HeroManager.Enemies.Where(hero => hero.LSIsValidTarget(E.Range)))
	                {
                        if (Player.LSDistance(en, true) <= SpiderE.Range && Player.LSDistance(en, true) > SpiderQ.Range &&
                        Config.Item("eCombo.Spider").GetValue<bool>() && SpiderE.LSIsReady())
                        {
                            E.Cast(en);
                        }
                        if (Player.LSDistance(en, true) <= SpiderE.Range && Player.LSDistance(en, true) > SpiderQ.Range &&
                        Config.Item("eCombo.Spider").GetValue<bool>() && SpiderE.LSIsReady() && Player.LSCountAlliesInRange(E.Range) == 1 && en.HealthPercent < 5)
                        {
                            E.Cast(en);
                        }
	                }
                }
                if (!SpiderQ.LSIsReady() && !SpiderW.LSIsReady() && R.LSIsReady() &&
                    Config.Item("rCombo").GetValue<bool>())
                {
                    R.Cast();
                }
            }
            
        }
        private static void Harass()
        {
            if (humansexygirl)
            {
                if (ObjectManager.Player.ManaPercent > Config.Item("manaHarass").GetValue<Slider>().Value)
                {
                    if (Q.LSIsReady() && Config.Item("qHarass").GetValue<bool>())
                    {
                        foreach (AIHeroClient qTarget in HeroManager.Enemies.Where(x => x.LSIsValidTarget(Q.Range)))
                        {
                            Q.Cast(qTarget);
                        }
                    }
                    if (W.LSIsReady() && Config.Item("wHarass").GetValue<bool>())
	                {
                        foreach (AIHeroClient wTarget in HeroManager.Enemies.Where(x => x.LSIsValidTarget(W.Range)))
	                    {
                            if (W.CanCast(wTarget) && W.GetPrediction(wTarget).Hitchance >= HitChance.High && !Player.Spellbook.IsAutoAttacking && !Player.LSIsDashing())
                            {
                                W.Cast(wTarget);
                            }
	                    }
	                }
                }
            }
        }
        private static void spiderCheck()
        {
            if (Player.Spellbook.GetSpell(SpellSlot.Q).Name == "EliseHumanQ" ||
                Player.Spellbook.GetSpell(SpellSlot.W).Name == "EliseHumanW" ||
                Player.Spellbook.GetSpell(SpellSlot.E).Name == "EliseHumanE")
            {
                spidergirl    = false;
                humansexygirl = true;
                rangeQ = Q.Range;
                rangeW = W.Range;
                rangeE = E.Range;
                
            }
            if (Player.Spellbook.GetSpell(SpellSlot.Q).Name == "EliseSpiderQCast" ||
                Player.Spellbook.GetSpell(SpellSlot.W).Name == "EliseSpiderW"     ||
                Player.Spellbook.GetSpell(SpellSlot.E).Name == "EliseSpiderEInitial")
            {
                spidergirl    = true;
                humansexygirl = false;
                rangeQ = SpiderQ.Range;
                rangeW = SpiderW.Range;
                rangeE = SpiderE.Range;
            }
        }
        private static void Drawing_OnDraw(EventArgs args)
        {
            var menuItem1 = Config.Item("qDraw").GetValue<Circle>();
            var menuItem2 = Config.Item("wDraw").GetValue<Circle>();
            var menuItem3 = Config.Item("eDraw").GetValue<Circle>();

            if (menuItem1.Active && Q.LSIsReady())
            {
                Render.Circle.DrawCircle(Player.Position, rangeQ, Color.SpringGreen);
            }
            if (menuItem2.Active && W.LSIsReady())
            {
                Render.Circle.DrawCircle(Player.Position, rangeW , Color.White);
            }
            if (menuItem3.Active && E.LSIsReady())
            {
                Render.Circle.DrawCircle(Player.Position, rangeW, Color.White);
            }
        }
    }
}
