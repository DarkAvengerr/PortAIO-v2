using LeagueSharp.Common;
using _Project_Geass.Functions;
using Prediction = _Project_Geass.Functions.Prediction;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace _Project_Geass.Module.Champions.Heroes.Menus
{

    internal class Ezreal
    {
        #region Private Fields

        private readonly string _baseName=Names.ProjectName+StaticObjects.Player.ChampionName+".";

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Ezreal" /> class.
        /// </summary>
        public Ezreal()
        {
            StaticObjects.ProjectMenu.AddSubMenu(Misc());
            StaticObjects.ProjectMenu.AddSubMenu(Combo());
            StaticObjects.ProjectMenu.AddSubMenu(Mixed());
            StaticObjects.ProjectMenu.AddSubMenu(Clear());
        }

        #endregion Public Constructors

        #region Private Methods

        /// <summary>
        ///     On Clear
        /// </summary>
        /// <returns>
        /// </returns>
        private Menu Clear()
        {
            var basename=_baseName+"Clear.";
            var mainMenu=new Menu(nameof(Clear), basename);

            mainMenu.AddItem(new MenuItem($"{basename}.UseQ", "Use Q").SetValue(true));
            mainMenu.AddItem(new MenuItem($"{basename}.UseQ.Minon.LastHit", "Use Q To Last Hit Minons").SetValue(true));
            mainMenu.AddItem(new MenuItem($"{basename}.UseQ.OnJungle", "Use Q on mosters").SetValue(true));

            return mainMenu;
        }

        /// <summary>
        ///     On Combo
        /// </summary>
        /// <returns>
        /// </returns>
        private Menu Combo()
        {
            var basename=_baseName+"Combo.";

            var mainMenu=new Menu(nameof(Combo), basename);
            mainMenu.AddItem(new MenuItem($"{basename}.UseQ", "Use Q").SetValue(true));

            mainMenu.AddItem(new MenuItem($"{basename}.UseW", "Use W").SetValue(true));

            mainMenu.AddItem(new MenuItem($"{basename}.UseR", "Use R").SetValue(true));

            mainMenu.AddItem(new MenuItem($"{basename}.UseQ.Prediction", "Q Prediction").SetValue(new StringList(Prediction.GetHitChanceNames())));

            mainMenu.AddItem(new MenuItem($"{basename}.UseW.Prediction", "W Prediction").SetValue(new StringList(Prediction.GetHitChanceNames())));

            mainMenu.AddItem(new MenuItem($"{basename}.UseR.Prediction", "R Prediction").SetValue(new StringList(Prediction.GetHitChanceNames())));

            var qMenu=new Menu("Q Settings", basename+"Q Settings");

            foreach (var enemy in Functions.Objects.Heroes.GetEnemies())
                qMenu.AddItem(new MenuItem($"{basename}.UseQ.On.{enemy.ChampionName}", $"{enemy.ChampionName}").SetValue(true));

            var wMenu=new Menu("W Settings", basename+"W Settings");

            foreach (var enemy in Functions.Objects.Heroes.GetEnemies())
                wMenu.AddItem(new MenuItem($"{basename}.UseW.On.{enemy.ChampionName}", $"{enemy.ChampionName}").SetValue(true));

            mainMenu.AddItem(new MenuItem($"{basename}.UseR.Range", "R Range").SetValue(new Slider(1000, 500, 1750)));

            var rMenu=new Menu("R KS Settings", basename+"R Settings");

            foreach (var enemy in Functions.Objects.Heroes.GetEnemies())
                rMenu.AddItem(new MenuItem($"{basename}.UseR.On.{enemy.ChampionName}", $"{enemy.ChampionName}").SetValue(true));

            mainMenu.AddSubMenu(qMenu);
            mainMenu.AddSubMenu(wMenu);
            mainMenu.AddSubMenu(rMenu);

            return mainMenu;
        }

        /// <summary>
        ///     Miscs
        /// </summary>
        /// <returns>
        /// </returns>
        private Menu Misc()
        {
            var basename=_baseName+"Misc.";

            var mainMenu=new Menu(nameof(Misc), basename);
            mainMenu.AddItem(new MenuItem($"{basename}.UseQ.TearStack", "Use Q to tear stack (when no enemy in range)").SetValue(true));
            mainMenu.AddItem(new MenuItem($"{basename}.UseQ.TearStack.MinMana", "Min Mana%").SetValue(new Slider(70)));
            return mainMenu;
        }

        /// <summary>
        ///     On Mixed
        /// </summary>
        /// <returns>
        /// </returns>
        private Menu Mixed()
        {
            var basename=_baseName+"Mixed.";
            var mainMenu=new Menu(nameof(Mixed), basename);

            mainMenu.AddItem(new MenuItem($"{basename}.UseQ", "Use Q").SetValue(true));

            mainMenu.AddItem(new MenuItem($"{basename}.UseW", "Use W").SetValue(true));

            mainMenu.AddItem(new MenuItem($"{basename}.UseQ.Prediction", "Q Prediction").SetValue(new StringList(Prediction.GetHitChanceNames())));

            mainMenu.AddItem(new MenuItem($"{basename}.UseW.Prediction", "W Prediction").SetValue(new StringList(Prediction.GetHitChanceNames())));

            var qMenu=new Menu("Q Settings", basename+"Q Settings");

            foreach (var enemy in Functions.Objects.Heroes.GetEnemies())
                qMenu.AddItem(new MenuItem($"{basename}.UseQ.On.{enemy.ChampionName}", $"{enemy.ChampionName}").SetValue(true));

            var wMenu=new Menu("W Settings", basename+"W Settings");

            foreach (var enemy in Functions.Objects.Heroes.GetEnemies())
                wMenu.AddItem(new MenuItem($"{basename}.UseW.On.{enemy.ChampionName}", $"{enemy.ChampionName}").SetValue(true));

            mainMenu.AddSubMenu(qMenu);
            mainMenu.AddSubMenu(wMenu);

            return mainMenu;
        }

        #endregion Private Methods
    }

}