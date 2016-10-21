using LeagueSharp.Common;
using _Project_Geass.Functions;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace _Project_Geass.Module.Champions.Heroes.Menus
{

    internal class Tristana
    {
        #region Private Fields

        private readonly string _baseName=Names.ProjectName+StaticObjects.Player.ChampionName+".";

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Tristana" /> class.
        /// </summary>
        public Tristana()
        {
            StaticObjects.ProjectMenu.AddSubMenu(Drawing());
            StaticObjects.ProjectMenu.AddSubMenu(Combo());
            StaticObjects.ProjectMenu.AddSubMenu(Mixed());
            StaticObjects.ProjectMenu.AddSubMenu(Clear());
            StaticObjects.ProjectMenu.AddSubMenu(Auto());
        }

        #endregion Public Constructors

        #region Private Methods

        /// <summary>
        ///     Automative events
        /// </summary>
        /// <returns>
        /// </returns>
        private Menu Auto()
        {
            var basename=_baseName+"Auto.";

            var mainMenu=new Menu(nameof(Auto), basename);
            mainMenu.AddItem(new MenuItem($"{basename}.UseR", "Use R").SetValue(true));

            var rMenu=new Menu("R Settings", basename+"RSettings");

            foreach (var enemy in Functions.Objects.Heroes.GetEnemies())
                rMenu.AddItem(new MenuItem($"{basename}.UseR.{enemy.ChampionName}", $"{enemy.ChampionName}").SetValue(true));

            mainMenu.AddSubMenu(rMenu);

            return mainMenu;
        }

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
            mainMenu.AddItem(new MenuItem($"{basename}.UseQ.OnMinions", "Use Q On Minons").SetValue(true));
            mainMenu.AddItem(new MenuItem($"{basename}.UseQ.OnTurrets", "Use Q On Turrets").SetValue(true));
            mainMenu.AddItem(new MenuItem($"{basename}.UseQ.OnJungle", "Use Q On Monsters").SetValue(true));

            mainMenu.AddItem(new MenuItem($"{basename}.UseE", "Use E").SetValue(true));
            mainMenu.AddItem(new MenuItem($"{basename}.UseE.OnMinions", "Use E On Minons").SetValue(true));
            mainMenu.AddItem(new MenuItem($"{basename}.UseE.OnTurrets", "Use E On Turrets").SetValue(true));
            mainMenu.AddItem(new MenuItem($"{basename}.UseE.OnJungle", "Use E On Monsters").SetValue(true));

            mainMenu.AddItem(new MenuItem($"{basename}.MinionsInRange", "Minons In Range").SetValue(new Slider(4, 3, 10)));
            mainMenu.AddItem(new MenuItem($"{basename}.UseE.Focus", "Focus E target").SetValue(true));

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

            mainMenu.AddItem(new MenuItem($"{basename}.UseE", "Use E").SetValue(true));

            var eMenu=new Menu("E Settings", basename+"E Settings");

            foreach (var enemy in Functions.Objects.Heroes.GetEnemies())
                eMenu.AddItem(new MenuItem($"{basename}.UseE.On.{enemy.ChampionName}", $"{enemy.ChampionName}").SetValue(true));

            mainMenu.AddItem(new MenuItem($"{basename}.UseR", "Use R (kill)").SetValue(true));

            var rMenu=new Menu("R Settings", basename+"R Settings");

            foreach (var enemy in Functions.Objects.Heroes.GetEnemies())
                rMenu.AddItem(new MenuItem($"{basename}.UseR.On.{enemy.ChampionName}", $"{enemy.ChampionName}").SetValue(true));

            mainMenu.AddSubMenu(eMenu);
            mainMenu.AddSubMenu(rMenu);

            return mainMenu;
        }

        /// <summary>
        ///     Drawings this instance.
        /// </summary>
        /// <returns>
        /// </returns>
        private Menu Drawing()
        {
            var basename=_baseName+"Drawing.";

            var mainMenu=new Menu(nameof(basename), basename);
            mainMenu.AddItem(new MenuItem($"{basename}.DrawEStacks", "Draw E Stacks").SetValue(true));

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

            var eMenu=new Menu("E Settings", basename+"E Settings");

            foreach (var enemy in Functions.Objects.Heroes.GetEnemies())
                eMenu.AddItem(new MenuItem($"{basename}.UseE.On.{enemy.ChampionName}", $"{enemy.ChampionName}").SetValue(true));

            mainMenu.AddSubMenu(eMenu);
            return mainMenu;
        }

        #endregion Private Methods
    }

}