using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NoobMalzahar
{
    class Program
    {
        public const string ChampionName = "Malzahar";
        public static AIHeroClient Player => ObjectManager.Player;

        public static Orbwalking.Orbwalker Orbwalker;
        //Menu
        public static Menu Menu;
        //Spells
        public static List<Spell> SpellList = new List<Spell>();
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }
        private static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.ChampionName != "Malzahar") return;

            Q = new Spell(SpellSlot.Q, 900);
            W = new Spell(SpellSlot.W, 800);
            E = new Spell(SpellSlot.E, 650);
            R = new Spell(SpellSlot.R, 700);

            Q.SetSkillshot(0.5f, 100, float.MaxValue, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.5f, 240, 20, false, SkillshotType.SkillshotCircle);

            Menu = new Menu(Player.ChampionName, Player.ChampionName, true);
            var orbwalkerMenu = Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            //Combo Menu
            var combo = new Menu("Combo", "Combo");
            Menu.AddSubMenu(combo);
            combo.AddItem(new MenuItem("Combo", "Combo"));
            combo.AddItem(new MenuItem("useQ", "Use Q").SetValue(true));
            combo.AddItem(new MenuItem("useW", "Use W").SetValue(true));
            combo.AddItem(new MenuItem("useE", "Use E").SetValue(true));
            combo.AddItem(new MenuItem("useR", "Use R").SetValue(true));
            //Harass Menu
            var harass = new Menu("Harass", "Harass");
            Menu.AddSubMenu(harass);
            harass.AddItem(new MenuItem("autoharass", "Auto Harrass with E").SetValue(false));
            //LaneClear Menu
            var lc = new Menu("Laneclear", "Laneclear");
            Menu.AddSubMenu(lc);
            lc.AddItem(new MenuItem("laneclearE", "Use E to LaneClear").SetValue(true));

            var miscMenu = new Menu("Misc", "Misc");
            Menu.AddSubMenu(miscMenu);
            miscMenu.AddItem(new MenuItem("drawQ", "Draw Q range").SetValue(false));
            miscMenu.AddItem(new MenuItem("oneshot", "Burst Combo").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)).SetTooltip("It will cast Q+E+W+R on enemy when enemy is in E range."));
            Menu.AddToMainMenu();

            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Chat.Print("NoobMalzahar by 1Shinigamix3");
        }
        private static void OnDraw(EventArgs args)
        {
            if (Menu.Item("drawQ").GetValue<bool>())
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.DarkRed, 3);               
            }
            if ((Menu.Item("oneshot").GetValue<KeyBind>().Active))
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, System.Drawing.Color.BurlyWood, 3);
            }
        }
        private static void OnUpdate(EventArgs args)
        {
            //Combo
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Combo();
            }
            //Burst
            if (Menu.Item("oneshot").GetValue<KeyBind>().Active)
            {
                Oneshot();
            }
            //Lane
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                Lane();
            }
            //AutoHarass
            AutoHarass();
        }
        private static void AutoHarass()
        {
            var m = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            if (Menu.Item("autoharass").GetValue<bool>())
                    E.CastOnUnit(m);
        }
        static bool HasRBuff()
        {
            return (Player.IsChannelingImportantSpell() || Player.HasBuff("AiZaharNetherGrasp"));
        }
        //Combo
        private static void Combo()
        {
            if (HasRBuff())
                return;
            var useQ = (Menu.Item("useQ").GetValue<bool>());
            var useW = (Menu.Item("useW").GetValue<bool>());
            var useE = (Menu.Item("useE").GetValue<bool>());
            var useR = (Menu.Item("useR").GetValue<bool>());
            var m = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (!m.IsValidTarget())
            {
                return;
            }
            if (useQ && Q.IsReady()) Q.Cast(m);
            if (useE && E.IsReady()) E.CastOnUnit(m);
            if (useW && W.IsReady()) W.Cast(m);           
            if (useR && R.IsReady() && m != null && m.Health < R.GetDamage(m)) R.CastOnUnit(m);
        }
        //Burst
        public static void Oneshot()
        {
            if (HasRBuff())
                return;
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            var m = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
                if (Q.IsReady()) Q.CastOnUnit(m);                
                if (E.IsReady()) E.CastOnUnit(m);
                if (W.IsReady()) W.CastOnUnit(m);
                if (R.IsReady() && !E.IsReady() && !W.IsReady()) R.CastOnUnit(m);                         
        }
        //Lane
        private static void Lane()
        {
            var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range);
            if (Menu.Item("laneclearE").GetValue<bool>() && E.IsReady())
            {
                foreach (var minion in allMinions)
                {
                    if (minion.IsValidTarget())
                    {
                        E.CastOnUnit(minion);
                    }
                }
            }
        }
    }
}