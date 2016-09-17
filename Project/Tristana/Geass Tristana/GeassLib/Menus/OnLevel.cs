using GeassLib.Events;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace GeassLib.Menus
{
    public class OnLevel
    {
        // ReSharper disable once ArrangeTypeMemberModifiers
        // ReSharper disable once NotAccessedField.Local
        Events.OnLevel _onLevel;

        Menu GetMenu()
        {
            var menu = new Menu(Names.LevelNameBase, "levelMenu");
            menu.AddItem(new MenuItem(Names.LevelItemBase + "Boolean.AutoLevelUp", "Auto level-up abilities").SetValue(true));
            return menu;
        }

        public OnLevel(int[] abiSeq)
        {
            if (!DelayHandler.Loaded) DelayHandler.Load();
            Globals.Objects.Logger.WriteLog("Create OnLevel Menu");
            Globals.Objects.GeassLibMenu.AddSubMenu(GetMenu());
            _onLevel = new Events.OnLevel(abiSeq);

        }
    }
}
