using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;



using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NoobUdyr
{
    class Program
    {
        public const string ChampionName = "Udyr";

        //adding a skill list
        public static List<Spell> SpellList = new List<Spell>();
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        //orbwalker & menu
        static Orbwalking.Orbwalker orbwalker;
        static Menu menu;
        static AIHeroClient Player { get { return ObjectManager.Player; } }
        static AIHeroClient Target = null;

        //not needed atm
        /*private static bool IsRUsed
        {
            get { return Player.HasBuff("udyrphoenixstance"); }
        }*/
        //items
        private static Items.Item tiamat;
        private static Items.Item hydra;
        private static Items.Item cutlass;
        private static Items.Item botrk;
        private static Items.Item titanic;
        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            //Checks wether its udyr or not
            if (Player.ChampionName != "Udyr")
            {
                return;
            }
            //declaring skills
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 600);
            R = new Spell(SpellSlot.R);

            menu = new Menu(Player.ChampionName, Player.ChampionName, true);
            Menu orbwalkerMenu = menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);

            //Combo
            var combo = new Menu("Combo", "Combo");
            menu.AddSubMenu(combo);
            combo.AddItem(new MenuItem("useQ", "Use Q").SetValue(true));
            combo.AddItem(new MenuItem("useW", "Use W").SetValue(true));
            combo.AddItem(new MenuItem("useE", "Use E").SetValue(true));
            combo.AddItem(new MenuItem("useR", "Use R").SetValue(true));

            //LaneClear
            var lc = new Menu("Laneclear", "Laneclear");
            menu.AddSubMenu(lc);
            lc.AddItem(new MenuItem("laneclearQ", "Use Q to LaneClear").SetValue(true));
            lc.AddItem(new MenuItem("laneclearW", "Use W to LaneClear").SetValue(true));
            lc.AddItem(new MenuItem("laneclearR", "Use R to LaneClear").SetValue(true));

            //Jungle Clear
            var jungle = new Menu("JungleClear", "JungleClear");
            menu.AddSubMenu(jungle);
            jungle.AddItem(new MenuItem("jungleclearQ", "Use Q to JungleClear").SetValue(true));
            jungle.AddItem(new MenuItem("jungleclearW", "Use W to JungleClear").SetValue(true));
            jungle.AddItem(new MenuItem("jungleclearE", "Use E to JungleClear").SetValue(true));
            jungle.AddItem(new MenuItem("jungleclearR", "Use R to JungleClear").SetValue(true));

            //soon :(
            /*var flee = new Menu("Flee", "Flee");
            menu.AddSubMenu(flee);
            flee.AddItem(new MenuItem("FleeE", "Use E to Flee").SetValue(true));*/

            //item declaration
            hydra = new Items.Item(3074, 185);
            tiamat = new Items.Item(3077, 185);
            cutlass = new Items.Item(3144, 450);
            botrk = new Items.Item(3153, 450);
            titanic = new Items.Item(3748, 450);
            menu.AddToMainMenu();

            //Events
            Game.OnUpdate += OnUpdate;
            Orbwalking.AfterAttack += AfterAa;
            Chat.Print("NoobUdyr by 1Shinigamix3");
        }
        private static void OnUpdate(EventArgs args)
        {
            //If Spacebar then use following :
            if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                AIHeroClient m = TargetSelector.GetTarget(600/*right here i decided that the range should be 600*/, TargetSelector.DamageType.Physical);
                //Itemusage of ranged stuff
                if (Player.Distance(m) <= botrk.Range)
                {
                    botrk.Cast(m);
                }
                if (Player.Distance(m) <= cutlass.Range)
                {
                    cutlass.Cast(m);
                }
                //If target is in 600 range Cast E when E is ready
                if (/*The useE is a question wether the value of the menu is true or not*/menu.Item("useE").GetValue<bool>() && E.IsReady() /*&& !IsRUsed*/)
                        E.Cast(m);
            }
            //If LaneClear button then use
            if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                //I declared what Lane and Jungle is (scroll down)
                Lane();
                Jungle();
            }
        }
        //This is an event and uses what in it after an AA
        private static void AfterAa(AttackableUnit unit, AttackableUnit target)
        {
            if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (menu.Item("useR").GetValue<bool>() && R.IsReady())
                    R.Cast();
                //Hydra usage
                if (hydra.IsOwned() && Player.Distance(target) < hydra.Range && hydra.IsReady())
                    hydra.Cast();
                if (tiamat.IsOwned() && Player.Distance(target) < tiamat.Range && tiamat.IsReady())
                    tiamat.Cast();
                if (titanic.IsOwned() && Player.Distance(target) < titanic.Range && titanic.IsReady())
                    titanic.Cast();
                if (menu.Item("useQ").GetValue<bool>() && Q.IsReady())
                    Q.Cast();
                if (menu.Item("useW").GetValue<bool>() && W.IsReady())
                    W.Cast();
            }
        }
        //Lane&JungleClear
        private static void Lane()
        {
            var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, 600);
            if (menu.Item("laneclearW").GetValue<bool>() && W.IsReady())
            {
                foreach (var minion in allMinions)
                {
                    if (minion.IsValidTarget())
                    {
                        W.Cast();
                    }
                }
            }
            if (menu.Item("laneclearQ").GetValue<bool>() && Q.IsReady())
            {
                foreach (var minion in allMinions)
                {
                    if (minion.IsValidTarget())
                    {
                        Q.Cast();
                    }
                }
            }

            if (menu.Item("laneclearR").GetValue<bool>() && R.IsReady())
            {
                foreach (var minion in allMinions)
                {
                    if (minion.IsValidTarget())
                    {
                        R.Cast();
                    }
                }
            }
        }

        private static void Jungle()
        {
            var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, 600, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            if (menu.Item("jungleclearE").GetValue<bool>() && E.IsReady())
            {
                foreach (var minion in allMinions)
                {
                    if (minion.IsValidTarget())
                    {
                        E.Cast();
                    }
                }
            }

            if (menu.Item("jungleclearW").GetValue<bool>() && W.IsReady())
            {
                foreach (var minion in allMinions)
                {
                    if (minion.IsValidTarget())
                    {
                        W.Cast();
                    }
                }
            }
            if (menu.Item("jungleclearQ").GetValue<bool>() && Q.IsReady())
            {
                foreach (var minion in allMinions)
                {
                    if (minion.IsValidTarget())
                    {
                        Q.Cast();
                    }
                }
            }
            if (menu.Item("jungleclearR").GetValue<bool>() && R.IsReady())
            {
                foreach (var minion in allMinions)
                {
                    if (minion.IsValidTarget())
                    {
                        R.Cast();
                    }
                }
            }

        }
    }
}
