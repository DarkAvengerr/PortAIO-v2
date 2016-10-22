namespace xSaliceResurrected_Rework
{
    using LeagueSharp;
    using LeagueSharp.Common;
    using Pluging;
    using EloBuddy;
    public class PluginLoader
    {
        private static bool _loaded;

        public PluginLoader()
        {
            if (!_loaded)
            {
                switch (ObjectManager.Player.ChampionName.ToLower())
                {
                    case "ahri":
                        var ahri = new Ahri();
                        _loaded = true;
                        break;
                    case "akali":
                        var akali = new Akali();
                        _loaded = true;
                        break;
                    case "anivia":
                        var anivia = new Anivia();
                        break;
                    case "cassiopeia":
                        var cassiopeia = new Cassiopeia();
                        _loaded = true;
                        break;
                    case "ashe":
                        var ashe = new Ashe();
                        _loaded = true;
                        break;
                    case "chogath":
                        var chogath = new Chogath();
                        _loaded = true;
                        break;
                    case "corki":
                        var corki = new Corki();
                        _loaded = true;
                        break;
                    case "ekko":
                        var ekko = new Ekko();
                        _loaded = true;
                        break;
                    case "ezreal":
                        var ezreal = new Ezreal();
                        _loaded = true;
                        break;
                    case "irelia":
                        var irelia = new Irelia();
                        _loaded = true;
                        break;
                    case "jinx":
                        var jinx = new Jinx();
                        _loaded = true;
                        break;
                    case "karthus":
                        var karthus = new Karthus();
                        _loaded = true;
                        break;
                    case "katarina":
                        var katarina = new Katarina();
                        _loaded = true;
                        break;
                    case "kogmaw":
                        var kogMaw = new KogMaw();
                        _loaded = true;
                        break;
                    case "lissandra":
                        var lissandra = new Lissandra();
                        _loaded = true;
                        break;
                    case "lucian":
                        var lucian = new Lucian();
                        _loaded = true;
                        break;
                    case "jayce":
                        var jayce = new Jayce();
                        _loaded = true;
                        break;
                    case "orianna":
                        var orianna = new Orianna();
                        _loaded = true;
                        break;
                    case "rumble":
                        var rumble = new Rumble();
                        _loaded = true;
                        break;
                    case "syndra":
                        var syndra = new Syndra();
                        _loaded = true;
                        break;
                    case "vayne":
                        var vayne = new Vayne();
                        _loaded = true;
                        break;
                    case "viktor":
                        var viktor = new Viktor();
                        _loaded = true;
                        break;
                    case "vladimir":
                        var vladimir = new Vladimir();
                        _loaded = true;
                        break;
                    case "urgot":
                        var urgot = new Urgot();
                        _loaded = true;
                        break;
                    case "zyra":
                        var zyra = new Zyra();
                        _loaded = true;
                        break;
                    default:
                        Notifications.AddNotification(ObjectManager.Player.ChampionName + " not supported!!", 10000);
                        break;
                }
            }
        }
    }
}