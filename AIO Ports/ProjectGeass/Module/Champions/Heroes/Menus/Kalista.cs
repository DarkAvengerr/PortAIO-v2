using LeagueSharp.Common;
using _Project_Geass.Functions;
using Prediction = _Project_Geass.Functions.Prediction;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace _Project_Geass.Module.Champions.Heroes.Menus
{

    internal class Kalista
    {
        #region Private Fields

        private readonly string _baseName=Names.ProjectName+StaticObjects.Player.ChampionName+".";

        #endregion Private Fields

        #region Public Constructors

        public Kalista()
        {
            StaticObjects.ProjectMenu.AddSubMenu(Combo());
            StaticObjects.ProjectMenu.AddSubMenu(Mixed());
            StaticObjects.ProjectMenu.AddSubMenu(Clear());
            StaticObjects.ProjectMenu.AddSubMenu(Auto());
        }

        #endregion Public Constructors

        #region Private Methods

        //Auto Not DONE
        private Menu Auto()
        {
            var basename=_baseName+"Auto.";

            var mainMenu=new Menu(basename, "Auto Events");

            var eMenu=new Menu($"{basename}EMenu", "E Menu");

            var eAutoKill=new Menu($"{basename}EMenu.AutoKill", "Auto Kill");
            eAutoKill.AddItem(new MenuItem($"{basename}.UseE", "Enable").SetValue(true));
            eAutoKill.AddItem(new MenuItem($"{basename}.UseE.NonKillable", "NonKillable Minons").SetValue(true));
            eAutoKill.AddItem(new MenuItem($"{basename}.UseE.Epics", "Epic Monsters").SetValue(true));
            eAutoKill.AddItem(new MenuItem($"{basename}.UseE.Enemies", "Enemies").SetValue(true));
            eAutoKill.AddItem(new MenuItem($"{basename}.UseE.Buffs", $"Buffs").SetValue(true));

            var eOther=new Menu($"{basename}EMenu.Other", "Other");

            eMenu.AddSubMenu(eAutoKill);
            eMenu.AddSubMenu(eOther);

            var rMenu=new Menu($"{basename}.RMenu", "R Menu");
            rMenu.AddItem(new MenuItem($"{basename}.UseR", "Enable").SetValue(true));

            mainMenu.AddSubMenu(rMenu);

            return mainMenu;
        }

        private Menu Clear()
        {
            var basename=_baseName+"Clear.";
            var mainMenu=new Menu(basename, nameof(Clear));

            mainMenu.AddItem(new MenuItem($"{basename}.UseE", "Use E").SetValue(true));
            mainMenu.AddItem(new MenuItem($"{basename}.UseE.OnMinions", "Use E On Minons").SetValue(true));
            mainMenu.AddItem(new MenuItem($"{basename}.UseE.OnTurrets", "Use E On Turrets").SetValue(true));
            mainMenu.AddItem(new MenuItem($"{basename}.UseE.OnJungle", "Use E On Monsters").SetValue(true));

            return mainMenu;
        }

        private Menu Combo()
        {
            var basename=_baseName+"Combo.";

            var mainMenu=new Menu(basename, nameof(Mixed));
            mainMenu.AddItem(new MenuItem($"{basename}.UseQ", "Use Q").SetValue(true));
            mainMenu.AddItem(new MenuItem($"{basename}.UseQ.Exploit", "Use Q AA Reset").SetValue(true));
            mainMenu.AddItem(new MenuItem($"{basename}.UseQ.Prediction", "Q Prediction").SetValue(new StringList(Prediction.GetHitChanceNames())));

            mainMenu.AddItem(new MenuItem($"{basename}.UseE", "Use E").SetValue(true));
            mainMenu.AddItem(new MenuItem($"{basename}.UseE.OnKillable", "Use E OnKillable").SetValue(true));

            var qMenu=new Menu("Q Settings", basename+"Q Settings");

            foreach (var enemy in Functions.Objects.Heroes.GetEnemies())
                qMenu.AddItem(new MenuItem($"{basename}.UseQ.On.{enemy.ChampionName}", $"{enemy.ChampionName}.Enable").SetValue(true));

            mainMenu.AddSubMenu(qMenu);
            return mainMenu;
        }

        private Menu Mixed()
        {
            var basename=_baseName+"Mixed.";
            var mainMenu=new Menu(basename, nameof(Mixed));
            mainMenu.AddItem(new MenuItem($"{basename}.UseQ", "Use Q").SetValue(true));
            mainMenu.AddItem(new MenuItem($"{basename}.UseQ.Exploit", "Use Q AA Reset").SetValue(true));
            mainMenu.AddItem(new MenuItem($"{basename}.UseQ.Prediction", "Q Prediction").SetValue(new StringList(Prediction.GetHitChanceNames())));

            mainMenu.AddItem(new MenuItem($"{basename}.UseE", "Use E").SetValue(true));
            mainMenu.AddItem(new MenuItem($"{basename}.UseE.Stacks", "E Stacks").SetValue(new Slider(4, 1, 10)));

            var qMenu=new Menu("Q Settings", basename+"Q Settings");

            foreach (var enemy in Functions.Objects.Heroes.GetEnemies())
                qMenu.AddItem(new MenuItem($"{basename}.UseQ.On.{enemy.ChampionName}", $"{enemy.ChampionName}.Enable").SetValue(true));

            mainMenu.AddSubMenu(qMenu);
            return mainMenu;
        }

        #endregion Private Methods
    }

}