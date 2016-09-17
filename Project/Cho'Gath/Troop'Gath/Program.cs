using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace TroopChogath
{
    using System.ComponentModel.Design;

    class Program
    {
        public const string ChampionName = "Chogath";

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

        private static Items.Item hextech;

        private static Obj_AI_Base target;

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.ChampionName != "Chogath") return;

            Q = new Spell(SpellSlot.Q, 950);
            W = new Spell(SpellSlot.W, 700);
            E = new Spell(SpellSlot.E, 500);
            R = new Spell(SpellSlot.R, 175, TargetSelector.DamageType.True);

            Menu = new Menu(Player.ChampionName, Player.ChampionName, true);
            Menu orbwalkerMenu = Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            //SpellMenu
            Menu spellMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            spellMenu.AddItem(new MenuItem("useQ", "Use Q").SetValue(true));
            spellMenu.AddItem(new MenuItem("useW", "Use W").SetValue(true));
            spellMenu.AddItem(new MenuItem("useR", "Use R").SetValue(true));
            //Harass Menu
            var harass = new Menu("Harass", "Harass");
            Menu.AddSubMenu(harass);
            harass.AddItem(new MenuItem("harassQ", "Use Q to harass").SetValue(true));
            harass.AddItem(new MenuItem("harassW", "Use W to harass").SetValue(true));
            //LaneClear Menu
            Menu.AddSubMenu(new Menu("laneclear", "laneclear"));
            Menu.SubMenu("laneclear").AddItem(new MenuItem("jungleclearQ", "use Q to Laneclear").SetValue(true));
            Menu.SubMenu("laneclear").AddItem(new MenuItem("jungleclearW", "use W to Laneclear").SetValue(true));
            Menu.SubMenu("laneclear").AddItem(new MenuItem("jungleclearR", "use R to get stacks").SetValue(true));
            //JungleClear Menu
            var jungle = new Menu("JungleClear", "JungleClear");
            Menu.AddSubMenu(jungle);
            jungle.AddItem(new MenuItem("jungleclearQ2", "Use Q to JungleClear").SetValue(true));
            jungle.AddItem(new MenuItem("jungleclearW2", "Use W to JungleClear").SetValue(true));

            cutlass = new Items.Item(3144, 450);
            botrk = new Items.Item(3153, 450);
            hextech = new Items.Item(3146, 700);
            Menu.AddToMainMenu();

            Game.OnUpdate += OnUpdate;
            Chat.Print("Chogath v1.5 totally overwritten");
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Menu.Item("useR").GetValue<bool>())
            {
                Killsecure();
            }
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
                Lane();
                Jungle();
            }
        }

        private static void Combo()
        {
            var useQ = (Menu.Item("useQ").GetValue<bool>());
            var useW = (Menu.Item("useW").GetValue<bool>());
            var m = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
            //Itemusage
            if (m != null && Player.Distance(m) <= botrk.Range)
            {
                botrk.Cast(m);
            }
            if (m != null && Player.Distance(m) <= cutlass.Range)
            {
                cutlass.Cast(m);
            }
            if (m != null && Player.Distance(m) <= hextech.Range)
            {
                hextech.Cast(m);
            }
            //combo
            if (useQ && (Q.IsReady()))
            {
                Q.Cast(m);
            }
            if (useW && W.IsReady())
            {
                W.CastOnBestTarget();
            }
        }

        private static void Killsecure()
        {
            var useR = (Menu.Item("useR").GetValue<bool>());
            var m = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
            if (m != null && m.Health < R.GetDamage(m) && useR)
            {
                R.CastOnUnit(m);
            }
        }

        private static void Harass()
        {
            var o = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
            if (Menu.Item("harassW").GetValue<bool>())
            {
                if (Player.Distance(o.Position) > 125 && (Q.IsReady()))
                {
                    W.Cast(o);
                }
            }
            if (Menu.Item("harassQ").GetValue<bool>())
            {
                Q.CastIfHitchanceEquals(target, HitChance.High);
            }
        }

        private static void Lane()
        {
            Obj_AI_Base minion = MinionManager.GetMinions(Player.Position, 500).FirstOrDefault();
            if (Menu.Item("jungleclearW").GetValue<bool>()) W.Cast(minion);
            if (Menu.Item("jungleclearQ").GetValue<bool>() && !W.IsReady()) Q.Cast(minion);
            if (Menu.Item("jungleclearR").GetValue<bool>()) R.Cast(minion);
        }


        private static void Jungle()
        {
            var allMinions = MinionManager.GetMinions(
                ObjectManager.Player.ServerPosition,
                Q.Range,
                MinionTypes.All,
                MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);
            if (Menu.Item("jungleclearQ2").GetValue<bool>() && Q.IsReady())
            {
                foreach (var minion in allMinions)
                {
                    if (minion.IsValidTarget())
                    {
                        Q.Cast();
                    }
                }
            }
        }
    }
}
