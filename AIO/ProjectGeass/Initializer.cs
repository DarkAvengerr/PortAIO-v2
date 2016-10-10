using LeagueSharp.Common;
using _Project_Geass.Data.Champions;
using _Project_Geass.Functions;
using _Project_Geass.Humanizer.TickTock;
using _Project_Geass.Module.Champions.Core;
using _Project_Geass.Module.Core.Items.Menus;
using _Project_Geass.Module.Core.Mana.Menus;
using _Project_Geass.Module.Core.OnLevel.Menus;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace _Project_Geass
{

    internal class Initializer
    {
        #region Public Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Initializer" /> class.
        /// </summary>
        public Initializer()
        {
            StaticObjects.ProjectLogger.WriteLog("Loading...");
            if (!Bootloader.ChampionBundled.ContainsKey(StaticObjects.Player.ChampionName))
                return;
            if (!Bootloader.ChampionBundled[StaticObjects.Player.ChampionName])
                return;

            StaticObjects.ProjectLogger.WriteLog("Load Delays...");
            Handler.Load(true);
            // ReSharper disable once UnusedVariable
            var initializerMenu=new SettingsMenuGenerater();
            StaticObjects.SettingsMenu.AddToMainMenu();

            var championSettings=new Settings();

            if (!StaticObjects.SettingsMenu.Item($"{Names.Menu.BaseItem}{StaticObjects.Player.ChampionName}.Enable").GetValue<bool>())
                return;

            var coreMenu=new Menu("Core Modules", "CoreModulesMenu");

            var drawingEnabled=StaticObjects.SettingsMenu.Item($"{Names.Menu.BaseItem}{StaticObjects.Player.ChampionName}.DrawingMenu").GetValue<bool>();
            var manaEnabled=StaticObjects.SettingsMenu.Item($"{Names.Menu.BaseItem}{StaticObjects.Player.ChampionName}.ManaMenu").GetValue<bool>();
            var itemEnabled=StaticObjects.SettingsMenu.Item($"{Names.Menu.BaseItem}{StaticObjects.Player.ChampionName}.ItemMenu").GetValue<bool>();
            var autoLevelEnabled=StaticObjects.SettingsMenu.Item($"{Names.Menu.BaseItem}{StaticObjects.Player.ChampionName}.OnLevelMenu").GetValue<bool>();
            var trinketEnabled=StaticObjects.SettingsMenu.Item($"{Names.Menu.BaseItem}{StaticObjects.Player.ChampionName}.TrinketMenu").GetValue<bool>();

            StaticObjects.ProjectLogger.WriteLog("Load Base Menu's...");
            var orbWalker=new Orbwalking.Orbwalker(StaticObjects.ProjectMenu.SubMenu(nameof(Orbwalking.Orbwalker)));
            // ReSharper disable once UnusedVariable
            var manaMenu=new ManaMenu(coreMenu, championSettings.ManaSettings, manaEnabled);
            // ReSharper disable once UnusedVariable
            var drawingMeun=new Module.Core.Drawing.Menus.Drawing(coreMenu, championSettings.DrawingSettings, drawingEnabled);
            // ReSharper disable once UnusedVariable
            var itemMenu=new Item(coreMenu, itemEnabled, orbWalker);
            // ReSharper disable once UnusedVariable
            var autoLevelMenu=new Abilities(coreMenu, championSettings.AbilitieSettings, autoLevelEnabled);
            // ReSharper disable once UnusedVariable
            var trinketMenu=new Trinket(coreMenu, trinketEnabled);

            StaticObjects.ProjectMenu.AddSubMenu(coreMenu);
            StaticObjects.ProjectMenu.AddToMainMenu();
            Bootloader.Load(manaEnabled, orbWalker);
        }

        #endregion Public Constructors
    }

}