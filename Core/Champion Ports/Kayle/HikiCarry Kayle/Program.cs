using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HikiCarry_Kayle
{
    class Program
    {
        public const string ChampionName = "Kayle";
        public static Orbwalking.Orbwalker Orbwalker;
        public static List<Spell> SpellList = new List<Spell>();
        static List<Spells> SpellListt = new List<Spells>();
        static int Delay = 0;

        public static AIHeroClient tar;

        public static Menu Config;

        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static SpellSlot Ignite;
        public static SpellSlot smiteSlot = SpellSlot.Unknown;
        public static readonly int[] Smites = { 1039, 3713, 3726, 3725, 3726, 3723, 3711, 3722,
                                                  3721, 3720, 3719, 3715, 3718, 3717, 3716, 
                                                  3714, 3706, 3710, 3709, 3708, 3707 }; //smite id's


        private static AIHeroClient Player;
        public struct Spells
        {
            public string ChampionName;
            public string SpellName;
            public SpellSlot slot;
        }
        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            Player = ObjectManager.Player;
            if (Player.BaseSkinName != ChampionName) return;


            Q = new Spell(SpellSlot.Q, 650f);
            W = new Spell(SpellSlot.W, 900f);
            E = new Spell(SpellSlot.E, 625f);
            R = new Spell(SpellSlot.R, 900f);

            Ignite = Player.GetSpellSlot("summonerdot");

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            SpellListt.Add(new Spells { ChampionName = "akali", SpellName = "akalismokebomb", slot = SpellSlot.W });   //Akali W
            SpellListt.Add(new Spells { ChampionName = "shaco", SpellName = "deceive", slot = SpellSlot.Q }); //Shaco Q
            SpellListt.Add(new Spells { ChampionName = "khazix", SpellName = "khazixr", slot = SpellSlot.R }); //Khazix R
            SpellListt.Add(new Spells { ChampionName = "khazix", SpellName = "khazixrlong", slot = SpellSlot.R }); //Khazix R Evolved
            SpellListt.Add(new Spells { ChampionName = "talon", SpellName = "talonshadowassault", slot = SpellSlot.R }); //Talon R
            SpellListt.Add(new Spells { ChampionName = "monkeyking", SpellName = "monkeykingdecoy", slot = SpellSlot.W }); //Wukong W

            //MENU
            Config = new Menu("HikiCarry - Kayle", "HikiCarry - Kayle", true);

            //TARGET SELECTOR
            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            //ORBWALKER
            Config.AddSubMenu(new Menu("Orbwalker Settings", "Orbwalker Settings"));
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker Settings"));

            //INFO
            Config.AddSubMenu(new Menu("Information", "Information"));
            Config.SubMenu("Information").AddItem(new MenuItem("Author", "@Hikigaya"));
            Config.SubMenu("Information").AddItem(new MenuItem("AuthorX2", "DONT RAPE ALEX KAPPAHD"));

            //COMBO
            Config.AddSubMenu(new Menu("Combo Settings", "Combo Settings"));
            Config.SubMenu("Combo Settings").AddItem(new MenuItem("qCombo", "Use Q").SetValue(true));
            Config.SubMenu("Combo Settings").AddItem(new MenuItem("wCombo", "Use W").SetValue(true));
            Config.SubMenu("Combo Settings").AddItem(new MenuItem("eCombo", "Use E").SetValue(true));
            Config.SubMenu("Combo Settings").AddItem(new MenuItem("rCombo", "Use R").SetValue(true));

            Config.AddSubMenu(new Menu("Harass Settings", "Harass Settings"));
            Config.SubMenu("Harass Settings").AddItem(new MenuItem("qHarass", "Use Q").SetValue(true));
            Config.SubMenu("Harass Settings").AddItem(new MenuItem("eHarass", "Use E").SetValue(true));
            Config.SubMenu("Harass Settings").AddItem(new MenuItem("manaHarass", "Harass Mana Percent").SetValue(new Slider(30, 1, 100)));

            Config.AddSubMenu(new Menu("Clear Settings", "Clear Settings"));
            Config.SubMenu("Clear Settings").AddItem(new MenuItem("siegeminionstoQ", "Use Q for Siege Minions").SetValue(true));
            Config.SubMenu("Clear Settings").AddItem(new MenuItem("eClear", "Use E").SetValue(true));
            Config.SubMenu("Clear Settings").AddItem(new MenuItem("manaClear", "Clear Mana Percent").SetValue(new Slider(30, 1, 100)));

            Config.AddSubMenu(new Menu("Jungle Settings", "Jungle Settings"));
            Config.SubMenu("Jungle Settings").AddItem(new MenuItem("qJungle", "Use Q").SetValue(true));
            Config.SubMenu("Jungle Settings").AddItem(new MenuItem("eJungle", "Use E").SetValue(true));
            Config.SubMenu("Jungle Settings").AddItem(new MenuItem("manaJungle", "Jungle Mana Percent").SetValue(new Slider(30, 1, 100)));
            Config.SubMenu("Jungle Settings").AddItem(new MenuItem("JungCheck", "                 Ulti Settings"));
            Config.SubMenu("Jungle Settings").AddItem(new MenuItem("rJungle", "Use R").SetValue(true));
            Config.SubMenu("Jungle Settings").AddItem(new MenuItem("rJungleHp", "Min Percentage of HP for R").SetValue(new Slider(2, 1, 100)));

            Config.AddSubMenu(new Menu("Heal and Ulti Settings", "Heal and Ulti Settings"));
            
            Config.SubMenu("Heal and Ulti Settings").AddItem(new MenuItem("kayleset", "                 Kayle Settings"));
            Config.SubMenu("Heal and Ulti Settings").AddItem(new MenuItem("wHeal", "Use W").SetValue(true));
            Config.SubMenu("Heal and Ulti Settings").AddItem(new MenuItem("wHealMePercent", "Min Percentage of HP for W").SetValue(new Slider(30, 1, 100)));
            Config.SubMenu("Heal and Ulti Settings").AddItem(new MenuItem("rMe", "Use R").SetValue(true));
            Config.SubMenu("Heal and Ulti Settings").AddItem(new MenuItem("rMinHpMe", "Min Percentage of HP for R").SetValue(new Slider(20, 1, 100)));
            
            Config.SubMenu("Heal and Ulti Settings").AddItem(new MenuItem("allyset", "                 Ally Settings"));
            Config.SubMenu("Heal and Ulti Settings").AddItem(new MenuItem("wAllyHeal", "Use W for Ally").SetValue(true));
            Config.SubMenu("Heal and Ulti Settings").AddItem(new MenuItem("wHealAllyPercent", "Ally Min Percentage of HP").SetValue(new Slider(10, 1, 100)));
            Config.SubMenu("Heal and Ulti Settings").AddItem(new MenuItem("rAlly", "Use R for Ally").SetValue(true));
            Config.SubMenu("Heal and Ulti Settings").AddItem(new MenuItem("rAllyHP", "Min Percentage of HP for R").SetValue(new Slider(10, 1, 100)));


            Config.AddSubMenu(new Menu("Invisible Kicker", "Invisiblez"));
            Config.SubMenu("Invisiblez").AddItem(new MenuItem("Use", "Use Vision Ward On Combo").SetValue(new KeyBind(32, KeyBindType.Press)));

            {
                foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy))
                {
                    foreach (var spell in SpellListt.Where(x => x.ChampionName.ToLower() == hero.ChampionName.ToLower()))
                    {
                        Config.SubMenu("Invisiblez").AddItem(new MenuItem(hero.ChampionName.ToLower() + spell.slot.ToString(), hero.ChampionName + " - " + spell.slot.ToString()).SetValue(true));
                    }
                }

                if (HeroManager.Enemies.Any(x => x.ChampionName.ToLower() == "rengar"))
                {
                    Config.SubMenu("Invisiblez").AddItem(new MenuItem("RengarR", "Rengar R").SetValue(true));
                }


            }

            Config.AddSubMenu(new Menu("Drawing Settings", "Drawing Settings"));
            
            Config.SubMenu("Drawing Settings").AddItem(new MenuItem("RushQRange", "Q Range").SetValue(new Circle(true, Color.SpringGreen)));
            Config.SubMenu("Drawing Settings").AddItem(new MenuItem("RushWRange", "W Range").SetValue(new Circle(true, Color.Red)));
            Config.SubMenu("Drawing Settings").AddItem(new MenuItem("RushERange", "E Range").SetValue(new Circle(true, Color.Crimson)));
            Config.SubMenu("Drawing Settings").AddItem(new MenuItem("RushRRange", "R Range").SetValue(new Circle(true, Color.Purple)));

            var drawDamageMenu = new MenuItem("RushDrawEDamage", "E Damage").SetValue(true);
            var drawFill = new MenuItem("RushDrawEDamageFill", "E Damage Fill").SetValue(new Circle(true, Color.Yellow));
            Config.SubMenu("Drawing Settings").AddItem(drawDamageMenu);
            Config.SubMenu("Drawing Settings").AddItem(drawFill);

            DamageIndicator.DamageToUnit = GetComboDamage;
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

            Config.AddItem(new MenuItem("summonerz", "            Summoner Spell Settings"));
            Config.AddItem(new MenuItem("signite", "Use [IGNITE]").SetValue(true));
            Config.AddToMainMenu();
            Game.OnUpdate += Game_OnGameUpdate;
            AIHeroClient.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
            Obj_AI_Base.OnCreate += Obj_AI_Base_OnCreate;
            
            Drawing.OnDraw += Drawing_OnDraw;
        }
        private static void Game_OnGameUpdate(EventArgs args)
        {
            Orbwalker.SetAttack(true);
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Combo();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                cLear();
                JungleClear();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                haraSS();
            }

            if (W.IsReady() && Config.Item("wHeal").GetValue<bool>()) // W HEAL TO KAYLE
            {
                if (Player.HealthPercent <= Config.Item("wHealMePercent").GetValue<Slider>().Value && !Player.IsRecalling())
                {
                    W.Cast(Player);
                }
            }
            if (Player.InventoryItems.Any(item => Smites.Any(t => t == (int)(item.Id)))) //smite id check
            {
                if (R.IsReady() && Config.Item("rJungle").GetValue<bool>()) // R TO KAYLE jungle
                {
                    var yx = Drawing.WorldToScreen(Player.Position);
                    
                    if (Player.HealthPercent <= Config.Item("rJungleHp").GetValue<Slider>().Value && !Player.IsRecalling())
                    {
                        R.Cast(Player);
                    }
                }
            }
            else
            {
                if (R.IsReady() && Config.Item("rMe").GetValue<bool>()) // definetly not jungle
                {
                    var yx = Drawing.WorldToScreen(Player.Position);
                    
                    if (Player.HealthPercent <= Config.Item("rMinHpMe").GetValue<Slider>().Value && !Player.IsRecalling())
                    {
                        R.Cast(Player);
                    }
                }
            }
            if (W.IsReady() && Config.Item("wAllyHeal").GetValue<bool>())
            {
                foreach (var ally in HeroManager.Allies)
                {
                    if (!ally.IsMe && ally.HealthPercent <= Config.Item("wHealAllyPercent").GetValue<Slider>().Value
                        && !ally.IsRecalling() && !ally.IsDead)
                    {
                        W.Cast(ally);
                    }
                }
            }
            if (R.IsReady() && Config.Item("rAlly").GetValue<bool>())
            {
                foreach (var allyx in HeroManager.Allies)
                {
                    if (!allyx.IsMe && allyx.HealthPercent <= Config.Item("rAllyHP").GetValue<Slider>().Value
                        && !allyx.IsRecalling() && !allyx.IsDead)
                    {
                       R.Cast(allyx);
                    }
                }
            }
        }
        private static void haraSS()
        {
            if (ObjectManager.Player.ManaPercent > Config.Item("manaHarass").GetValue<Slider>().Value)
            {
                if (Q.IsReady() && Config.Item("qHarass").GetValue<bool>())
                {
                    foreach (
                       var en in
                           HeroManager.Enemies.Where(
                               hero =>
                                   hero.IsValidTarget(Q.Range)))
                    {
                        if (Q.CanCast(en) && !Player.Spellbook.IsAutoAttacking && !Player.IsDashing())

                            Q.Cast(en);
                    }
                }

                if (E.IsReady() && Config.Item("eHarass").GetValue<bool>())
                {
                    foreach (
                       var en in
                           HeroManager.Enemies.Where(
                               hero =>
                                   hero.IsValidTarget(E.Range)))
                    {
                        E.Cast();
                    }
                }
            }
        }
        private static void cLear()
        {
            var qMinion = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Enemy);
            var eMinion = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range + 150, MinionTypes.All, MinionTeam.Enemy);

            if (ObjectManager.Player.ManaPercent > Config.Item("manaClear").GetValue<Slider>().Value)
            {
                if (Q.IsReady() && Config.Item("siegeminionstoQ").GetValue<bool>())
                {
                    foreach (var minyon in qMinion)
                    {
                        if (minyon.CharData.BaseSkinName.Contains("MinionSiege"))
                        {
                            if (Q.IsKillable(minyon))
                            {
                                Q.CastOnUnit(minyon);
                            }
                        }
                    }
                }
                if (E.IsReady() && Config.Item("eClear").GetValue<bool>())
                {
                    foreach (var minyon in eMinion)
                    {
                        if (eMinion.Count >= 3)
                        {
                            E.Cast();
                        }
                    }
                }
            }
        }
        private static void JungleClear()
        {
            if (ObjectManager.Player.ManaPercent > Config.Item("manaJungle").GetValue<Slider>().Value)
            {
                var mobs = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
                if (mobs == null || (mobs != null && mobs.Count == 0))
                {
                    return;
                }
                if (Q.IsReady() && Config.Item("qJungle").GetValue<bool>())
                {
                    Q.Cast(mobs[0]);
                }
                if (E.IsReady() && Config.Item("eJungle").GetValue<bool>())
                {
                    E.Cast();
                }
            }
        }
        private static void Combo()
        {
            AIHeroClient target = TargetSelector.GetTarget(600, TargetSelector.DamageType.Magical);
            if (Q.IsReady() && Config.Item("qCombo").GetValue<bool>())
            {
                foreach (
                   var en in
                       HeroManager.Enemies.Where(
                           hero =>
                               hero.IsValidTarget(Q.Range)))
                {
                    if (Q.CanCast(en) && !Player.Spellbook.IsAutoAttacking && !Player.IsDashing())

                        Q.Cast(en);
                }
            }
            if (E.IsReady() && Config.Item("eCombo").GetValue<bool>())
            {
                foreach (
                   var en in
                       HeroManager.Enemies.Where(
                           hero =>
                               hero.IsValidTarget(E.Range)))
                {
                        E.Cast();
                }
            }
            

            if (Ignite.IsReady() && Config.Item("signite").GetValue<bool>())
            {
                if (GetComboDamage(target) > target.Health - 100)
                {
                    Player.Spellbook.CastSpell(Ignite, target);
                }
            }

            

        }
        private static void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!Config.Item("Use").GetValue<KeyBind>().Active)
                return;

            if (!sender.IsEnemy || sender.IsDead || !(sender is AIHeroClient))
                return;

            if (SpellListt.Exists(x => x.SpellName.Contains(args.SData.Name.ToLower())))
            {
                var _sender = sender as AIHeroClient;

                if (!Config.Item(_sender.ChampionName.ToLower() + _sender.GetSpellSlot(args.SData.Name).ToString()).GetValue<bool>())
                    return;

                if (CheckSlot() == SpellSlot.Unknown)
                    return;

                if (CheckWard())
                    return;

                if (ObjectManager.Player.Distance(sender.Position) > 700)
                    return;

                if (Environment.TickCount - Delay > 1500 || Delay == 0)
                {
                    var pos = ObjectManager.Player.Distance(args.End) > 600 ? ObjectManager.Player.Position : args.End;
                    ObjectManager.Player.Spellbook.CastSpell(CheckSlot(), pos);
                    Delay = Environment.TickCount;
                }
            }
        }
        private static void Obj_AI_Base_OnCreate(GameObject sender, EventArgs args)
        {
            if (!Config.Item("Use").GetValue<KeyBind>().Active)
                return;

            var Rengar = HeroManager.Enemies.Find(x => x.ChampionName.ToLower() == "rengar");

            if (Rengar == null)
                return;

            if (!Config.Item("RengarR").GetValue<bool>())
                return;

            if (ObjectManager.Player.Distance(sender.Position) < 1500)
            {
                Console.WriteLine("Sender : " + sender.Name);
            }

            if (sender.IsEnemy && sender.Name.Contains("Rengar_Base_R_Alert"))
            {
                if (ObjectManager.Player.HasBuff("rengarralertsound") &&
                !CheckWard() &&
                !Rengar.IsVisible &&
                !Rengar.IsDead &&
                    CheckSlot() != SpellSlot.Unknown)
                {
                    ObjectManager.Player.Spellbook.CastSpell(CheckSlot(), ObjectManager.Player.Position);
                }
            }
        }
        static SpellSlot CheckSlot()
        {
            SpellSlot slot = SpellSlot.Unknown;

            if (Items.CanUseItem(3362) && Items.HasItem(3362, ObjectManager.Player))
            {
                slot = SpellSlot.Trinket;
            }
            else if (Items.CanUseItem(2043) && Items.HasItem(2043, ObjectManager.Player))
            {
                slot = ObjectManager.Player.GetSpellSlot("VisionWard");
            }
            return slot;
        }
        static bool CheckWard()
        {
            var status = false;

            foreach (var a in ObjectManager.Get<Obj_AI_Minion>().Where(x => x.Name == "VisionWard"))
            {
                if (ObjectManager.Player.Distance(a.Position) < 450)
                {
                    status = true;
                }
            }

            return status;
        }
        private static void Drawing_OnDraw(EventArgs args)
        {
            var menuItem1 = Config.Item("RushQRange").GetValue<Circle>();
            var menuItem2 = Config.Item("RushWRange").GetValue<Circle>();
            var menuItem3 = Config.Item("RushERange").GetValue<Circle>();
            var menuItem4 = Config.Item("RushRRange").GetValue<Circle>();

            if (menuItem1.Active && Q.IsReady())
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range, Color.SpringGreen);
            }
            if (menuItem2.Active && W.IsReady())
            {
                Render.Circle.DrawCircle(Player.Position, W.Range, Color.Red);
            }
            if (menuItem3.Active && E.IsReady())
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, Color.Crimson);
            }
            if (menuItem4.Active && R.IsReady())
            {
                Render.Circle.DrawCircle(Player.Position, R.Range, Color.Purple);
            }
        }
        private static float GetComboDamage(AIHeroClient enemy)
        {
            float damage = 0;

            if (Q.IsReady())
                damage += Q.GetDamage(enemy);
            if (E.IsReady())
                damage += E.GetDamage(enemy);

            return damage;
        }
    }
}