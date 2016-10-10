using LeagueSharp.Common;
using _Project_Geass.Functions;
using Prediction = _Project_Geass.Functions.Prediction;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace _Project_Geass.Module.Champions.Heroes.Menus
{

    internal class Caitlyn
    {
        #region Private Fields

        private readonly string _baseName=Names.ProjectName+StaticObjects.Player.ChampionName+".";

        #endregion Private Fields

        #region Public Constructors

        public Caitlyn()
        {
            StaticObjects.ProjectMenu.AddSubMenu(Combo());
            StaticObjects.ProjectMenu.AddSubMenu(Mixed());
            StaticObjects.ProjectMenu.AddSubMenu(Clear());
            StaticObjects.ProjectMenu.AddSubMenu(Auto());
        }

        #endregion Public Constructors

        #region Private Methods

        private Menu Auto()
        {
            var basename=_baseName+"Auto.";

            var mainMenu=new Menu(nameof(Auto), basename);

            mainMenu.AddItem(new MenuItem($"{basename}.UseW.OnImmobile ", "Use W on Immobile enemies").SetValue(true));

            mainMenu.AddItem(new MenuItem($"{basename}.UseE.OnGapClose", "Use E on gapclose").SetValue(true));
            var eMenu=new Menu("GapClose E Settings", basename+"GapCloseE");

            foreach (var enemy in Functions.Objects.Heroes.GetEnemies())
                eMenu.AddItem(new MenuItem($"{basename}.UseE.OnGapClose.{enemy.ChampionName}", $"On {enemy.ChampionName}").SetValue(true));

            mainMenu.AddItem(new MenuItem($"{basename}.UseW.OnGapClose", "Use W on gapclose").SetValue(true));
            var wMenu=new Menu("GapClose W Settings", basename+"GapCloseW");

            foreach (var enemy in Functions.Objects.Heroes.GetEnemies())
                wMenu.AddItem(new MenuItem($"{basename}.UseW.OnGapClose.{enemy.ChampionName}", $"On {enemy.ChampionName}").SetValue(true));

            mainMenu.AddItem(new MenuItem($"{basename}.UseR.OnKillable", "Use R on Killable").SetValue(true));
            var rMenu=new Menu("Killable R Settings", basename+"KillableR");

            foreach (var enemy in Functions.Objects.Heroes.GetEnemies())
                rMenu.AddItem(new MenuItem($"{basename}.UseR.OnKillable.{enemy.ChampionName}", $"On {enemy.ChampionName}").SetValue(true));

            mainMenu.AddSubMenu(eMenu);
            mainMenu.AddSubMenu(wMenu);
            mainMenu.AddSubMenu(rMenu);

            return mainMenu;
        }

        private Menu Clear()
        {
            var basename=_baseName+"Clear.";
            var mainMenu=new Menu(nameof(Clear), basename);

            mainMenu.AddItem(new MenuItem($"{basename}.UseQ", "Use Q").SetValue(false));
            mainMenu.AddItem(new MenuItem($"{basename}.UseQ.Minions", "Minons Hit").SetValue(new Slider(4, 3, 10)));

            return mainMenu;
        }

        private Menu Combo()
        {
            var basename=_baseName+"Combo.";

            var mainMenu=new Menu(nameof(Combo), basename);
            mainMenu.AddItem(new MenuItem($"{basename}.UseQ", "Use Q").SetValue(true));

            mainMenu.AddItem(new MenuItem($"{basename}.UseW", "Use W").SetValue(true));

            mainMenu.AddItem(new MenuItem($"{basename}.UseQ.Prediction", "Q Prediction").SetValue(new StringList(Prediction.GetHitChanceNames())));

            var wMenu=new Menu("W Settings", basename+"W Settings");

            foreach (var enemy in Functions.Objects.Heroes.GetEnemies())
                wMenu.AddItem(new MenuItem($"{basename}.UseW.On.{enemy.ChampionName}", $"{enemy.ChampionName}.Enable").SetValue(true));

            mainMenu.AddSubMenu(wMenu);

            return mainMenu;
        }

        private Menu Mixed()
        {
            var basename=_baseName+"Mixed.";
            var mainMenu=new Menu(nameof(Mixed), basename);

            mainMenu.AddItem(new MenuItem($"{basename}.UseQ", "Use Q").SetValue(true));
            mainMenu.AddItem(new MenuItem($"{basename}.UseQ.Prediction", "Q Prediction").SetValue(new StringList(Prediction.GetHitChanceNames())));

            var wMenu=new Menu("Q Settings", basename+"Q Settings");

            foreach (var enemy in Functions.Objects.Heroes.GetEnemies())
                wMenu.AddItem(new MenuItem($"{basename}.UseQ.On.{enemy.ChampionName}", $"{enemy.ChampionName}.Enable").SetValue(true));

            mainMenu.AddSubMenu(wMenu);
            return mainMenu;
        }

        #endregion Private Methods
    }

}