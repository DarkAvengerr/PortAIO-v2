using GeassLib.Events.Drawing.Minon;
using LeagueSharp.Common;
using Color = System.Drawing.Color;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace GeassLib.Menus
{
    class Drawing
    {
        // ReSharper disable once NotAccessedField.Local
        LastHitHelper _helper;

        public Menu MinonMenu()
        {
            var menu = new Menu(Names.DrawingNameBase + "Minons", "minionMenu");
            menu.AddItem(
                new MenuItem(Names.DrawingItemBase + ".Minion." + "Boolean.LastHitHelper", "LastHit Helper").SetValue(
                    false));

            menu.AddItem(
                new MenuItem(Names.DrawingItemBase + ".Minion." + "Circle.KillableColor", "Killable Color").SetValue(
                    new Circle(true, Color.Green)));
            menu.AddItem(
                new MenuItem(Names.DrawingItemBase + ".Minion." + "Slider.RenderDistance", "Render Distance").SetValue(
                    new Slider(1000, 500, 2500)));
            return menu;
        }

        public Drawing()
        {
            Globals.Objects.Logger.WriteLog("Create Drawing Menu");
            Globals.Objects.GeassLibMenu.AddSubMenu(MinonMenu());
            _helper = new LastHitHelper();
        }
    }
}
