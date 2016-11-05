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
 namespace TrooplonRewritten
{
    class Program
    {
        public const string ChampionName = "Talon";

        public static AIHeroClient Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        public static Orbwalking.Orbwalker Orbwalker;

        //Menu
        public static Menu Menu;

        //Spells
        public static List<Spell> SpellList = new List<Spell>();

        public static Spell Q;

        public static Spell W;

        public static Spell E;

        public static Spell R;

        private static Items.Item cutlass;

        private static Items.Item botrk;

        private static Items.Item Tiamat;

        private static Items.Item Hydra;

        private static Items.Item Youmu;

        private static Obj_AI_Base target;

        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Talon") return;

            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 700f);
            E = new Spell(SpellSlot.E, 720f);
            R = new Spell(SpellSlot.R, 600f);

            Menu = new Menu(Player.ChampionName, Player.ChampionName, true);
            Menu orbwalkerMenu = Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            //Spells
            Menu spellMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            spellMenu.AddItem(new MenuItem("useQ", "Use Q").SetValue(true));
            spellMenu.AddItem(new MenuItem("useW", "Use W").SetValue(true));
            spellMenu.AddItem(new MenuItem("useE", "Use E").SetValue(true));
            spellMenu.AddItem(new MenuItem("useR", "Use R").SetValue(true));
            //Harass
            var harass = new Menu("Harass", "Harass");
            Menu.AddSubMenu(harass);
            harass.AddItem(new MenuItem("harassQ", "Use Q to harass").SetValue(true));
            harass.AddItem(new MenuItem("harassW1", "Use W to harass").SetValue(true));
            harass.AddItem(new MenuItem("harassE", "Use E to harass").SetValue(true));
            //LaneClear
            Menu.AddSubMenu(new Menu("Laneclear", "laneclear"));
            Menu.SubMenu("laneclear").AddItem(new MenuItem("laneW", "use W+Q to Laneclear").SetValue(true));
            //Drawings
            Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            Menu.SubMenu("Drawings").AddItem(new MenuItem("Draw_Disabled", "Disable all Drawings").SetValue(false));
            Menu.SubMenu("Drawings").AddItem(new MenuItem("Qdraw", "Draw Q Range").SetValue(true));
            Menu.SubMenu("Drawings").AddItem(new MenuItem("Wdraw", "Draw W Range").SetValue(true));
            Menu.SubMenu("Drawings").AddItem(new MenuItem("Edraw", "Draw E Range").SetValue(true));
            Menu.SubMenu("Drawings").AddItem(new MenuItem("Rdraw", "Draw R Range").SetValue(true));
            //KS
            Menu.AddSubMenu(new Menu("Misc", "Misc"));
            Menu.SubMenu("Misc").AddItem(new MenuItem("useWks", "use W to Ks [works perfectly]").SetValue(true));
            //Credits
            Menu.AddItem(new MenuItem("Credits", "Assembly created by trooperhdx"));
            //Youmus fixed

            cutlass = new Items.Item(3144, 450);
            botrk = new Items.Item(3153, 450);
            Tiamat = new Items.Item(3077, 185);
            Hydra = new Items.Item(3074, 185);
            Youmu = new Items.Item(3142, 900);

            Menu.AddToMainMenu();

            Drawing.OnDraw += OnDraw;
            Game.OnUpdate += OnUpdate;
            Orbwalking.OnAttack += OnAa;
            Orbwalking.AfterAttack += AfterAa;
            Chat.Print(
                "<font color='#00CC83'>trooperhdx:</font> <font color='#B6250B'>" + Player.ChampionName
                + " Loaded<font color='#00B4D2'> Dont forget to Upvote this Assembly on the Assembly Database! </font>");
        }

        private static void OnDraw(EventArgs args)
        {
            var Target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);
            if (Menu.Item("Draw_Disabled").GetValue<bool>()) return;

            if (Menu.Item("Qdraw").GetValue<bool>()) Render.Circle.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.CadetBlue, 3);
            if (Menu.Item("Wdraw").GetValue<bool>()) Render.Circle.DrawCircle(Player.Position, W.Range, System.Drawing.Color.IndianRed, 3);
            if (Menu.Item("Edraw").GetValue<bool>()) Render.Circle.DrawCircle(Player.Position, E.Range, System.Drawing.Color.DarkSeaGreen, 3);
            if (Menu.Item("Rdraw").GetValue<bool>()) Render.Circle.DrawCircle(Player.Position, R.Range, System.Drawing.Color.BurlyWood, 3);
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Combo();
            }
            if (Menu.Item("useWks").GetValue<bool>())
            {
                Killsecure();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                Harass();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                Lane();
            }
        }

        private static void Killsecure()
        {
            var useW = (Menu.Item("useWks").GetValue<bool>());
            var y = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
            if (y != null && y.Health < W.GetDamage(y) && useW)
            {
                W.CastOnUnit(y);
            }
        }

        private static void Combo()
        {
            var e = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
            var useE = (Menu.Item("useE").GetValue<bool>());
            var useQ = (Menu.Item("useQ").GetValue<bool>());
            var useW = (Menu.Item("useW").GetValue<bool>());
            var useR = (Menu.Item("useR").GetValue<bool>());



            //Itemusage
            if (e != null && Player.Distance(e) <= botrk.Range)
            {
                botrk.Cast(e);
            }
            if (e != null && Player.Distance(e) <= cutlass.Range)
            {
                cutlass.Cast(e);
            }
            if (e != null && Player.Distance(e) <= Tiamat.Range)
            {
                Tiamat.Cast(e);
            }
            if (e != null && Player.Distance(e) <= Hydra.Range)
            {
                Hydra.Cast(e);
            }
            if (e != null && Player.Distance(e) <= Youmu.Range)
            {
                Youmu.Cast(e);
            }

            //combo
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (useE && E.IsReady())
                {
                    E.CastOnBestTarget();
                }
                if (useW && W.IsReady() && !Q.IsReady())  //W
                {
                    W.CastOnBestTarget();
                }
                if (useR && R.IsReady() && !W.IsReady()) //Q
                {
                    R.CastOnBestTarget();
                }

            }
        }

        private static void Harass()
        {
            var c = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                if (Menu.Item("harassW1").GetValue<bool>())
                {
                    {
                        W.Cast(c);
                    }
                    if (Menu.Item("harassE").GetValue<bool>())
                    {
                        {
                            E.Cast(c);
                        }
                        if (Menu.Item("harassQ").GetValue<bool>())
                        {
                            {
                                Q.Cast(c);
                            }
                        }
                    }
                }
        }

        private static void AfterAa(AttackableUnit unit, AttackableUnit attackableUnit)
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (Menu.Item("useQ").GetValue<bool>() && E.IsReady()) ;
                Q.Cast();
            }
        }

        private static void OnAa(AttackableUnit unit, AttackableUnit attackableUnit)
        {
            {
            }
        }


        private static void Lane()
        {
            var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, 700f);
            {
                if (Menu.Item("laneW").GetValue<bool>() && W.IsReady() && Q.IsReady())
                {
                    foreach (var minion in allMinions)
                    {
                        if (minion.IsValidTarget())
                        {
                            W.Cast(minion);
                        }
                        allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, 125f);
                        {
                            if (minion.IsValidTarget())
                            {
                                Q.Cast(minion);

                            }
                        }
                    }
                }
            }
        }
    }
}

