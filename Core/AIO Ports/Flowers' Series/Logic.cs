using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_ADC_Series
{
    using LeagueSharp;
    using LeagueSharp.Common;
    using System.Linq;
    using Orbwalking = ADCCOMMON.Orbwalking;

    internal class Logic
    {
        protected static Spell Q;
        protected static Spell W;
        protected static Spell E;
        protected static Spell R;
        protected static Spell EQ;
        protected static Spell QExtend;
        protected static AIHeroClient Me;
        protected static Orbwalking.Orbwalker Orbwalker;
        protected static Menu Menu;

        private static readonly string[] SupportList =
        {
            "Ashe", "Caitlyn", "Corki", "Draven", "Ezreal", "Graves", "Jhin", "Jinx", "Kalista", "Kindred", "KogMaw",
            "Lucian", "Missfortune", "Quinn", "Sivir", "Tristana", "Twitch", "Urgot", "Varus", "Vayne"
        };

        public static void InitAIO()
        {
            Me = ObjectManager.Player;

            Menu = new Menu("Flowers' ADC Series", "Flowers' ADC Series", true);

            var targetSelectMenu = Menu.AddSubMenu(new Menu("Target Selector", "Target Selector"));
            {
                TargetSelector.AddToMenu(targetSelectMenu);
            }

            var orbMenu = Menu.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            {
                Orbwalker = new Orbwalking.Orbwalker(orbMenu);
            }

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

            Menu.AddItem(new MenuItem("SpaceBar1", "   ", true));
            Menu.AddItem(new MenuItem("Credit", "Credit: NightMoon", true));

            Menu.AddToMainMenu();

            Chat.Print("Flowers' ADC Series: " + Me.ChampionName + (SupportList.Contains(Me.ChampionName)
                               ? " Load! Credit: NightMoon"
                               : " Not Support!"));
        }
    }
}
