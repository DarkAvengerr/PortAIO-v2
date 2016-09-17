using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using System.Threading;
using SharpDX;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Original_Gragas
{
    class Program
    {
        public static Orbwalking.Orbwalker orbwalker;
        public static Menu menu;
        public static Menu R_Insec, R_Barrel,  Q_LANECLEAR, Q_Auto, E_Menu, barrel, Ult, W_Menu;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
        }

        static void OnLoad(EventArgs args)
        {
            
            menu = new Menu("Original Gragas", "menu", true);

            Menu TargetMenu = menu.AddSubMenu(new Menu("Target Selector", "ts"));
            TargetMenu.AddItem(new MenuItem("DamageType", "AD Gragas or AP Gragas").SetValue(new StringList(new[] { "Physical Damage", "Magical Damage" }, 1)));

            Menu OrbwalkerMenu = menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            orbwalker = new Orbwalking.Orbwalker(OrbwalkerMenu);

            List<string> list = new List<string> { "Target Selector" };
            int i = 1;
            foreach (AIHeroClient unit in HeroManager.Enemies)
            {
                list.Add(unit.ChampionName);
                i = i + 1;
            }

            W_Menu = menu.AddSubMenu(new Menu("Gragas W", "W"));

            W_Menu.AddItem(new MenuItem("useW", "Let script use gragas W").SetValue(true));
            W_Menu.AddItem(new MenuItem("jungleW", "Clear jungle with W").SetValue(true));
            W_Menu.AddItem(new MenuItem("laneclearW", "Laneclear with w").SetValue(true));
            W_Menu.AddItem(new MenuItem("num_w", "Number of minions to W").SetValue(new Slider(2, 1, 4)));
            W_Menu.AddItem(new MenuItem("comboW", "Use W in combo").SetValue(true));

            E_Menu = menu.AddSubMenu(new Menu("Gragas E", "E"));

            E_Menu.AddItem(new MenuItem("useE", "Let script use gragas E").SetValue(true));
            E_Menu.AddItem(new MenuItem("draw", "Draw Gragas E range").SetValue(true));
            E_Menu.AddItem(new MenuItem("focus_unit", "Force bellyslam unit").SetValue(new StringList(list.ToArray(), 0)));
            E_Menu.AddItem(new MenuItem("jungleE", "Clear jungle with E").SetValue(true));

            barrel = menu.AddSubMenu(new Menu("Gragas Q", "Q"));

            barrel.AddItem(new MenuItem("draw", "Draw Q range").SetValue(true));

            Q_LANECLEAR = barrel.AddSubMenu(new Menu("Q Laneclear settings", "QLaneClear"));
            Q_LANECLEAR.AddItem(new MenuItem("useQ", "Use Q in lane and jungle clear").SetValue(true));
            Q_LANECLEAR.AddItem(new MenuItem("min_num", "Only Q x minions").SetValue(new Slider(3, 1, 7)));

            Q_Auto = barrel.AddSubMenu(new Menu("Auto Detonate Q", "QSecondCast"));
            Q_Auto.AddItem(new MenuItem("useQ", "Automatically detonate Q").SetValue(true));
            Q_Auto.AddItem(new MenuItem("combo", "Auto detonate in combo").SetValue(true));
            Q_Auto.AddItem(new MenuItem("farm", "Auto detonate Q in farming").SetValue(true));
            Q_Auto.AddItem(new MenuItem("num_farm", "Number of minions to farm lasthit").SetValue(new Slider(2, 1, 7)));
            Q_Auto.AddItem(new MenuItem("num_clear", "Number of minions to hit clear").SetValue(new Slider(5, 1, 7)));

            Ult = menu.AddSubMenu(new Menu("Gragas ult", "R"));

            Ult.AddItem(new MenuItem("draw", "Draw ult range").SetValue(true));

            R_Insec = Ult.AddSubMenu(new Menu("Insec settings", "R_Insec"));
            R_Insec.AddItem(new MenuItem("min_num", "Only ult x enemies").SetValue(new Slider(2, 1, 5)));
            R_Insec.AddItem(new MenuItem("force_key", "Force insec key").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
            R_Insec.AddItem(new MenuItem("focus_unit", "Force insec unit").SetValue(new StringList(list.ToArray(), 0)));
            R_Insec.AddItem(new MenuItem("barrel_use", "Do ult barrel in combo").SetValue(true));
            
            R_Barrel = Ult.AddSubMenu(new Menu("Q -> R combo settings", "R_Barrel"));
            R_Barrel.AddItem(new MenuItem("focus_unit", "Force windygragas combo unit").SetValue(new StringList(list.ToArray(), 0)));
            R_Barrel.AddItem(new MenuItem("auto_combo", "Automatically perform combo if will kill").SetValue(true));
            
            menu.AddToMainMenu();

            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            GameObject.OnCreate += OnCreateObj;
            GameObject.OnDelete += OnDeleteObj;

            E.bellyslam.SetSkillshot(0, E.hitradius, 1300, true, SkillshotType.SkillshotLine);
            Q.barrel.SetSkillshot(0, 300, 550, false, SkillshotType.SkillshotCircle);
        }

        private static void OnUpdate(EventArgs args)
        {
            GragasSkills.Flabslap();
            GragasSkills.InsecUlt();
            GragasSkills.WindyUlt();
            GragasSkills.ThrowQ();
            GragasSkills.DetonateQ();
            GragasSkills.UseW();
        }

        private static void OnCreateObj(GameObject sender, EventArgs args)
        {
            if (!sender.IsValid || sender == null)
            {
                return;
            }

            var missile = sender as MissileClient;
            
            if (!missile.IsValid || missile == null || missile.SpellCaster == null)
            {
                return;
            }

            var unit = missile.SpellCaster as AIHeroClient;

            if (unit == null || missile.SData == null || !unit.IsValid)
            {
                return;
            }

            if (unit.IsValid && (unit.IsMe) && missile.SData.Name == "GragasQMissile")
            {
                Q.qPosition = missile.EndPosition;
            }
        }

        private static void OnDeleteObj(GameObject sender, EventArgs args)
        {

            if (sender.Position.Distance(HeroManager.Player.Position) <= 1000)
            {
                if (sender.Name == "Gragas_Base_Q_Ally.troy")
                {
                    Q.qPosition = new Vector3(0, 0, 0);
                }
            }
        }

        public static void OnDraw(EventArgs args)
        {
            if (E_Menu.Item("draw").GetValue<bool>())
            {
                Drawing.DrawCircle(HeroManager.Player.Position, E.bellyslam.Range, System.Drawing.Color.Yellow);
            }
            if (barrel.Item("draw").GetValue<bool>())
            {
                Drawing.DrawCircle(HeroManager.Player.Position, Q.barrel.Range, System.Drawing.Color.Orange);
                Drawing.DrawCircle(Q.qPosition, 300, System.Drawing.Color.Orange);
            }
            if (Ult.Item("draw").GetValue<bool>())
            {
                Drawing.DrawCircle(HeroManager.Player.Position, R.ult.Range, System.Drawing.Color.Red);
            }
        }
    }
}
