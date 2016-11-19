using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Core
{
    using ExorAIO.Champions.Akali;
    using ExorAIO.Champions.Amumu;
    using ExorAIO.Champions.Anivia;
    using ExorAIO.Champions.Ashe;
    using ExorAIO.Champions.Caitlyn;
    using ExorAIO.Champions.Cassiopeia;
    using ExorAIO.Champions.Corki;
    using ExorAIO.Champions.Darius;
    using ExorAIO.Champions.Diana;
    using ExorAIO.Champions.Draven;
    using ExorAIO.Champions.DrMundo;
    using ExorAIO.Champions.Evelynn;
    using ExorAIO.Champions.Ezreal;
    using ExorAIO.Champions.Graves;
    using ExorAIO.Champions.Jax;
    using ExorAIO.Champions.Jhin;
    using ExorAIO.Champions.Jinx;
    using ExorAIO.Champions.Kalista;
    using ExorAIO.Champions.Karma;
    using ExorAIO.Champions.Karthus;
    using ExorAIO.Champions.KogMaw;
    using ExorAIO.Champions.Lucian;
    using ExorAIO.Champions.Lux;
    using ExorAIO.Champions.MissFortune;
    using ExorAIO.Champions.Nautilus;
    using ExorAIO.Champions.Nocturne;
    using ExorAIO.Champions.Nunu;
    using ExorAIO.Champions.Olaf;
    using ExorAIO.Champions.Orianna;
    using ExorAIO.Champions.Pantheon;
    using ExorAIO.Champions.Quinn;
    using ExorAIO.Champions.Renekton;
    using ExorAIO.Champions.Ryze;
    using ExorAIO.Champions.Sivir;
    using ExorAIO.Champions.Sona;
    using ExorAIO.Champions.Taliyah;
    using ExorAIO.Champions.Tristana;
    using ExorAIO.Champions.Tryndamere;
    using ExorAIO.Champions.Twitch;
    using ExorAIO.Champions.Udyr;
    using ExorAIO.Champions.Vayne;
    using ExorAIO.Champions.Veigar;
    using ExorAIO.Champions.Warwick;
    using ExorAIO.Utilities;

    using LeagueSharp.SDK;

    /// <summary>
    ///     The bootstrap class.
    /// </summary>
    internal class Bootstrap
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Tries to load the champion which is being currently played.
        /// </summary>
        public static void LoadChampion()
        {
            switch (GameObjects.Player.ChampionName)
            {
                case "Akali":
                    new Akali().OnLoad();
                    break;
                case "Amumu":
                    new Amumu().OnLoad();
                    break;
                case "Anivia":
                    new Anivia().OnLoad();
                    break;
                case "Ashe":
                    new Ashe().OnLoad();
                    break;
                case "Caitlyn":
                    new Caitlyn().OnLoad();
                    break;
                case "Cassiopeia":
                    new Cassiopeia().OnLoad();
                    break;
                case "Corki":
                    new Corki().OnLoad();
                    break;
                case "Darius":
                    new Darius().OnLoad();
                    break;
                case "Diana":
                    new Diana().OnLoad();
                    break;
                case "Draven":
                    new Draven().OnLoad();
                    break;
                case "DrMundo":
                    new DrMundo().OnLoad();
                    break;
                case "Evelynn":
                    new Evelynn().OnLoad();
                    break;
                case "Ezreal":
                    new Ezreal().OnLoad();
                    break;
                case "Graves":
                    new Graves().OnLoad();
                    break;
                case "Jax":
                    new Jax().OnLoad();
                    break;
                case "Jhin":
                    new Jhin().OnLoad();
                    break;
                case "Jinx":
                    new Jinx().OnLoad();
                    break;
                case "Kalista":
                    new Kalista().OnLoad();
                    break;
                case "Karma":
                    new Karma().OnLoad();
                    break;
                case "Karthus":
                    new Karthus().OnLoad();
                    break;
                case "KogMaw":
                    new KogMaw().OnLoad();
                    break;
                case "Lucian":
                    new Lucian().OnLoad();
                    break;
                case "Lux":
                    new Lux().OnLoad();
                    break;
                case "MissFortune":
                    new MissFortune().OnLoad();
                    break;
                case "Nautilus":
                    new Nautilus().OnLoad();
                    break;
                case "Nocturne":
                    new Nocturne().OnLoad();
                    break;
                case "Nunu":
                    new Nunu().OnLoad();
                    break;
                case "Olaf":
                    new Olaf().OnLoad();
                    break;
                case "Orianna":
                    new Orianna().OnLoad();
                    break;
                case "Pantheon":
                    new Pantheon().OnLoad();
                    break;
                case "Quinn":
                    new Quinn().OnLoad();
                    break;
                case "Renekton":
                    new Renekton().OnLoad();
                    break;
                case "Ryze":
                    new Ryze().OnLoad();
                    break;
                case "Sivir":
                    new Sivir().OnLoad();
                    break;
                case "Sona":
                    new Sona().OnLoad();
                    break;
                case "Taliyah":
                    new Taliyah().OnLoad();
                    break;
                case "Tristana":
                    new Tristana().OnLoad();
                    break;
                case "Tryndamere":
                    new Tryndamere().OnLoad();
                    break;
                case "Twitch":
                    new Twitch().OnLoad();
                    break;
                case "Udyr":
                    new Udyr().OnLoad();
                    break;
                case "Vayne":
                    new Vayne().OnLoad();
                    break;
                case "Veigar":
                    new Veigar().OnLoad();
                    break;
                case "Warwick":
                    new Warwick().OnLoad();
                    break;
                default:
                    Vars.IsLoaded = false;
                    break;
            }
        }

        #endregion
    }
}