using EloBuddy; 
using LeagueSharp.Common; 
 namespace Flowers_ADC_Series
{
    using Common;
    using LeagueSharp;
    using LeagueSharp.Common;
    using System.Linq;

    internal class Logic
    {
        protected static Spell Q;
        protected static Spell W;
        protected static Spell E;
        protected static Spell R;
        protected static Spell EQ;
        protected static Spell QExtend;
        protected static Menu Championmenu;
        protected static Menu Utilitymenu;
        protected static AIHeroClient Me;
        protected static Orbwalking.Orbwalker Orbwalker;
        private static Menu Menu;
        public static HpBarDraw HpBarDraw = new HpBarDraw();

        private static readonly string[] SupportList =
        {
            "Ashe", "Caitlyn", "Corki", "Draven", "Ezreal", "Graves", "Jhin", "Jinx", "Kalista", "Kindred", "KogMaw",
            "Lucian", "Missfortune", "Quinn", "Sivir", "Tristana", "Twitch", "Urgot", "Varus", "Vayne"
        };

        public static int spellHitChance
        {
            get
            {
                if (Menu.Item("SetHitchance", true).GetValue<StringList>().SelectedIndex == 0)
                {
                    return 0;
                }
                if (Menu.Item("SetHitchance", true).GetValue<StringList>().SelectedIndex == 1)
                {
                    return 1;
                }
                if (Menu.Item("SetHitchance", true).GetValue<StringList>().SelectedIndex == 2)
                {
                    return 2;
                }
                return 3;
            }
        }

        public static int SelectPred => Menu.Item("SelectPred", true).GetValue<StringList>().SelectedIndex;

        public static void InitAIO()
        {
            Me = ObjectManager.Player;

            Menu = new Menu("Flowers' ADC Series", "Flowers' ADC Series", true);

            var OrbMenu = Menu.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            {
                Orbwalker = new Orbwalking.Orbwalker(OrbMenu);
            }

            var PredMenu = Menu.AddSubMenu(new Menu("Prediction", "Prediction"));
            {
                PredMenu.AddItem(new MenuItem("SelectPred", "Select Prediction: ", true).SetValue(new StringList(new[]
                {
                    "Common Prediction", "OKTW Prediction", "SDK Prediction"
                }, 1)));
                PredMenu.AddItem(
                    new MenuItem("SetHitchance", "HitChance: ", true).SetValue(
                        new StringList(new[] { "VeryHigh", "High", "Medium", "Low" })));
            }

            Utilitymenu = Menu.AddSubMenu(new Menu("Utility", "Utility"));
            {
                var EnabledMenu = Utilitymenu.AddSubMenu(new Menu("Utility Enabled", "Utility Enabled"));
                {
                    EnabledMenu.AddItem(new MenuItem("Inject AutoLevels", "Inject AutoLevels").SetValue(true));
                    EnabledMenu.AddItem(new MenuItem("Inject AutoWard", "Inject AutoWard").SetValue(true));
                    EnabledMenu.AddItem(new MenuItem("Inject TurnAround", "Inject TurnAround").SetValue(true));
                    EnabledMenu.AddItem(new MenuItem("Inject SkinChange", "Inject SkinChange").SetValue(true));
                    EnabledMenu.AddItem(new MenuItem("Inject Items", "Inject Items").SetValue(true));
                    EnabledMenu.AddItem(new MenuItem("Inject Cleanese", "Inject Cleanese").SetValue(true));
                    EnabledMenu.AddItem(new MenuItem("Inject Humanizer", "Inject Humanizer").SetValue(true));
                }

                if (Menu.Item("Inject AutoLevels").GetValue<bool>())
                {
                    Utility.AutoLevels.Init();
                }

                if (Menu.Item("Inject AutoWard").GetValue<bool>())
                {
                    Utility.AutoWard.Init();
                }

                if (Menu.Item("Inject TurnAround").GetValue<bool>())
                {
                    Utility.TurnAround.Init();
                }

                if (Menu.Item("Inject SkinChange").GetValue<bool>())
                {
                    //Utility.SkinChange.Init();
                }

                if (Menu.Item("Inject Items").GetValue<bool>())
                {
                    Utility.Items.Init();
                }

                if (Menu.Item("Inject Cleanese").GetValue<bool>())
                {
                    Utility.Cleaness.Init();
                }

                if (Menu.Item("Inject Humanizer").GetValue<bool>())
                {
                    Utility.Humanizer.Init();
                }
            }

            Championmenu = Menu.AddSubMenu(new Menu("Pluging: " + Me.ChampionName, "Pluging: " + Me.ChampionName));
            {
                switch (Me.ChampionName)
                {
                    case "Ashe":
                        var ashe = new Pluging.Ashe();
                        break;
                    case "Caitlyn":
                        var caitlyn = new Pluging.Caitlyn();
                        break;
                    case "Corki":
                        var corki = new Pluging.Corki();
                        break;
                    case "Draven":
                        var draven = new Pluging.Draven();
                        break;
                    case "Ezreal":
                        var ezreal = new Pluging.Ezreal();
                        break;
                    case "Graves":
                        var graves = new Pluging.Graves();
                        break;
                    case "Jhin":
                        var jhin = new Pluging.Jhin();
                        break;
                    case "Jinx":
                        var jinx = new Pluging.Jinx();
                        break;
                    case "Kalista":
                        var kalista = new Pluging.Kalista();
                        break;
                    case "Kindred":
                        var kindred = new Pluging.Kindred();
                        break;
                    case "KogMaw":
                        var kogMaw = new Pluging.KogMaw();
                        break;
                    case "Lucian":
                        var lucian = new Pluging.Lucian();
                        break;
                    case "MissFortune":
                        var missFortune = new Pluging.MissFortune();
                        break;
                    case "Quinn":
                        var quinn = new Pluging.Quinn();
                        break;
                    case "Sivir":
                        var sivir = new Pluging.Sivir();
                        break;
                    case "Tristana":
                        var tristana = new Pluging.Tristana();
                        break;
                    case "Twitch":
                        var twitch = new Pluging.Twitch();
                        break;
                    case "Urgot":
                        var urgot = new Pluging.Urgot();
                        break;
                    case "Varus":
                        var varus = new Pluging.Varus();
                        break;
                    case "Vayne":
                        var vayne = new Pluging.Vayne();
                        break;
                    default:
                        Menu.AddItem(new MenuItem("Not Support!", Me.ChampionName + ": Not Support!", true));
                        break;
                }
            }

            Menu.AddItem(new MenuItem("SpaceBar1", "   ", true));
            Menu.AddItem(new MenuItem("Credit", "Credit: NightMoon", true));

            Menu.AddToMainMenu();

            Chat.Print("Flowers' ADC Series: " + Me.ChampionName + (SupportList.Contains(Me.ChampionName)
                               ? " Load! Credit: NightMoon"
                               : " Not Support!"));
        }
    }
}
