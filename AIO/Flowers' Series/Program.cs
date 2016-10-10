using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Flowers_ADC_Series
{
    using System;
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Program
    {
        public static Menu Menu;
        public static Menu Championmenu;
        public static AIHeroClient Me;
        public static Orbwalking.Orbwalker Orbwalker;

        public static string[] DangerSpellName =
        {
            "KatarinaR", "GalioIdolOfDurand", "GragasE", "Crowstorm", "BandageToss", "LissandraE",
            "AbsoluteZero", "AlZaharNetherGrasp", "FallenOne", "PantheonRJump", "CaitlynAceintheHole",
            "MissFortuneBulletTime", "InfiniteDuress", "ThreshQ", "RocketGrab", "AatroxQ", "AkaliShadowDance",
            "Headbutt", "DianaTeleport", "AlZaharNetherGrasp", "JaxLeapStrike", "KatarinaE", "KhazixE",
            "LeonaZenithBlade",
            "MaokaiTrunkLine", "MonkeyKingNimbus", "PantheonW", "PoppyHeroicCharge", "ShenShadowDash",
            "SejuaniArcticAssault", "RenektonSliceAndDice", "Slash", "XenZhaoSweep", "RocketJump"
        };

        public static string[] AutoEnableList =
        {
             "Annie", "Ahri", "Akali", "Anivia", "Annie", "Brand", "Cassiopeia", "Diana", "Evelynn", "FiddleSticks", "Fizz", "Gragas", "Heimerdinger", "Karthus",
             "Kassadin", "Katarina", "Kayle", "Kennen", "Leblanc", "Lissandra", "Lux", "Malzahar", "Mordekaiser", "Morgana", "Nidalee", "Orianna",
             "Ryze", "Sion", "Swain", "Syndra", "Teemo", "TwistedFate", "Veigar", "Viktor", "Vladimir", "Xerath", "Ziggs", "Zyra", "Velkoz", "Azir", "Ekko",
             "Ashe", "Caitlyn", "Corki", "Draven", "Ezreal", "Graves", "Jayce", "Jinx", "KogMaw", "Lucian", "MasterYi", "MissFortune", "Quinn", "Shaco", "Sivir",
             "Talon", "Tristana", "Twitch", "Urgot", "Varus", "Vayne", "Yasuo", "Zed", "Kindred", "AurelionSol"
        };

        public static void Main()
        {
            OnGameLoad();
        }

        private static void OnGameLoad()
        {
            Me = ObjectManager.Player;

            Menu = new Menu("Flowers' ADC Series", "Flowers' ADC Series", true);

            var OrbMenu = Menu.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(OrbMenu);

            var PredMenu = Menu.AddSubMenu(new Menu("Prediction", "Prediction"));
            {
                PredMenu.AddItem(new MenuItem("SelectPred", "Select Prediction: ", true).SetValue(new StringList(new[]
                {
                    "Common Prediction", "OKTW Prediction", "SDK Prediction", "SPrediction(Need F5 Reload)",
                    "xcsoft AIO Prediction"
                }, 2)));
                PredMenu.AddItem(
                    new MenuItem("SetHitchance", "HitChance: ", true).SetValue(
                        new StringList(new[] { "VeryHigh", "High", "Medium", "Low" })));
                PredMenu.AddItem(new MenuItem("AboutCommonPred", "Common Prediction -> LeagueSharp.Commmon Prediction"));
                PredMenu.AddItem(new MenuItem("AboutOKTWPred", "OKTW Prediction -> Sebby' Prediction"));
                PredMenu.AddItem(new MenuItem("AboutSDKPred", "SDK Prediction -> LeagueSharp.SDKEx Prediction"));
                PredMenu.AddItem(new MenuItem("AboutSPred", "SPrediction -> Shine' Prediction"));
                PredMenu.AddItem(new MenuItem("AboutxcsoftAIOPred", "xcsoft AIO Prediction -> xcsoft ALL In One Prediction"));
            }

            Utility.AutoLevels.Init();
            Utility.SkinChange.Init();
            Utility.Offensive.Init();
            Utility.Defensive.Init();

            Championmenu = Menu.AddSubMenu(new Menu("Pluging: " + Me.ChampionName, "Pluging: " + Me.ChampionName));
            {
                switch (Me.ChampionName)
                {
                    case "Ashe":
                        var ashe = new Pluging.Ashe();
                        Chat.Print("Flowers' ADC Series: " + Me.ChampionName + " Load Succeed! Credit: NightMoon");
                        break;
                    case "Caitlyn":
                        var caitlyn = new Pluging.Caitlyn();
                        Chat.Print("Flowers' ADC Series: " + Me.ChampionName + " Load Succeed! Credit: NightMoon");
                        break;
                    case "Corki":
                        var corki = new Pluging.Corki();
                        Chat.Print("Flowers' ADC Series: " + Me.ChampionName + " Load Succeed! Credit: NightMoon");
                        break;
                    //case "Draven":
                    //    var draven = new Pluging.Draven();
                    //    Chat.Print("Flowers' ADC Series: " + Me.ChampionName + " Load Succeed! Credit: NightMoon");
                    //    break;
                    case "Ezreal":
                        var ezreal = new Pluging.Ezreal();
                        Chat.Print("Flowers' ADC Series: " + Me.ChampionName + " Load Succeed! Credit: NightMoon");
                        break;
                    case "Graves":
                        var graves = new Pluging.Graves();
                        Chat.Print("Flowers' ADC Series: " + Me.ChampionName + " Load Succeed! Credit: NightMoon");
                        break;
                    case "Jhin":
                        var jhin = new Pluging.Jhin();
                        Chat.Print("Flowers' ADC Series: " + Me.ChampionName + " Load Succeed! Credit: NightMoon");
                        break;
                    //case "Jinx":
                    //    var jinx = new Pluging.Jinx();
                    //    Chat.Print("Flowers' ADC Series: " + Me.ChampionName + " Load Succeed! Credit: NightMoon");
                    //    break;
                    case "Kalista":
                        var kalista = new Pluging.Kalista();
                        Chat.Print("Flowers' ADC Series: " + Me.ChampionName + " Load Succeed! Credit: NightMoon");
                        break;
                    case "KogMaw":
                        var kogMaw = new Pluging.KogMaw();
                        Chat.Print("Flowers' ADC Series: " + Me.ChampionName + " Load Succeed! Credit: NightMoon");
                        break;
                    case "Lucian":
                        var lucian = new Pluging.Lucian();
                        Chat.Print("Flowers' ADC Series: " + Me.ChampionName + " Load Succeed! Credit: NightMoon");
                        break;
                    case "MissFortune":
                        var missFortune = new Pluging.MissFortune();
                        Chat.Print("Flowers' ADC Series: " + Me.ChampionName + " Load Succeed! Credit: NightMoon");
                        break;
                    case "Quinn":
                        var quinn = new Pluging.Quinn();
                        Chat.Print("Flowers' ADC Series: " + Me.ChampionName + " Load Succeed! Credit: NightMoon");
                        break;
                    case "Sivir":
                        var sivir = new Pluging.Sivir();
                        Chat.Print("Flowers' ADC Series: " + Me.ChampionName + " Load Succeed! Credit: NightMoon");
                        break;
                    case "Tristana":
                        var tristana = new Pluging.Tristana();
                        Chat.Print("Flowers' ADC Series: " + Me.ChampionName + " Load Succeed! Credit: NightMoon");
                        break;
                    case "Twitch":
                        var twitch = new Pluging.Twitch();
                        Chat.Print("Flowers' ADC Series: " + Me.ChampionName + " Load Succeed! Credit: NightMoon");
                        break;
                    case "Urgot":
                        var urgot = new Pluging.Urgot();
                        Chat.Print("Flowers' ADC Series: " + Me.ChampionName + " Load Succeed! Credit: NightMoon");
                        break;
                    case "Varus":
                        var varus = new Pluging.Varus();
                        Chat.Print("Flowers' ADC Series: " + Me.ChampionName + " Load Succeed! Credit: NightMoon");
                        break;
                    case "Vayne":
                        var vayne = new Pluging.Vayne();
                        Chat.Print("Flowers' ADC Series: " + Me.ChampionName + " Load Succeed! Credit: NightMoon");
                        break;
                    default:
                        Menu.AddItem(new MenuItem("Not Support!", Me.ChampionName + ": Not Support!", true));
                        Chat.Print("Flowers' ADC Series: " + Me.ChampionName + " Not Support! Credit: NightMoon");
                        break;
                }
            }

            Menu.AddItem(new MenuItem("SpaceBar", "   ", true));
            Menu.AddItem(new MenuItem("Credit", "Credit: NightMoon", true));

            if (Menu.Item("SelectPred", true).GetValue<StringList>().SelectedIndex == 3)
            {
                SPrediction.Prediction.Initialize(PredMenu);
            }

            Menu.AddToMainMenu();
        }
    }
}
