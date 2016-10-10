using System.Drawing;
using LeagueSharp.Common;
using _Project_Geass.Functions;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace _Project_Geass.Module.Core.Drawing.Menus
{

    internal sealed class Drawing
    {
        #region Public Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Drawing" /> class.
        /// </summary>
        /// <param name="menu">
        ///     The menu.
        /// </param>
        /// <param name="drawingOptions">
        ///     The drawing options.
        /// </param>
        /// <param name="enabled">
        ///     if set to <c> true </c> [enabled].
        /// </param>
        public Drawing(Menu menu, bool[] drawingOptions, bool enabled)
        {
            if (!enabled)
                return;

            menu.AddSubMenu(Menu(drawingOptions));
            // ReSharper disable once UnusedVariable
            var helper=new Events.Drawing();

            StaticObjects.ProjectLogger.WriteLog("Drawing Menu and events loaded.");
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        ///     Menus the specified drawing options.
        /// </summary>
        /// <param name="drawingOptions">
        ///     The drawing options.
        /// </param>
        /// <returns>
        /// </returns>
        public Menu Menu(bool[] drawingOptions)
        {
            var menu=new Menu(nameof(Drawing), Names.Menu.DrawingNameBase);

            var enemyMenu=new Menu("Enemys", Names.Menu.DrawingItemBase+"Enemy");
            enemyMenu.AddItem(new MenuItem(Names.Menu.DrawingItemBase+StaticObjects.Player.ChampionName+".Boolean.DrawOnEnemy.ComboDamage", "Combo Damage Fill").SetValue(new Circle(true, Color.DarkGray)));
            enemyMenu.AddItem(new MenuItem(Names.Menu.DrawingItemBase+StaticObjects.Player.ChampionName+".Boolean.DrawOnEnemy.DrawRange", "Draw Enemy AA Range").SetValue(new Circle(true, Color.Red)));

            var selfMenu=new Menu("Self", Names.Menu.DrawingItemBase+"Self");
            //selfMenu.AddItem(new MenuItem(Names.Menu.DrawingItemBase + StaticObjects.Player.ChampionName + ".Boolean.DrawOnSelf", "Draw On Self").SetValue(true));

            if (drawingOptions[0])
                selfMenu.AddItem(new MenuItem(Names.Menu.DrawingItemBase+StaticObjects.Player.ChampionName+".Boolean.DrawOnSelf.QColor", "Q Range").SetValue(new Circle(true, Color.LightBlue)));
            if (drawingOptions[1])
                selfMenu.AddItem(new MenuItem(Names.Menu.DrawingItemBase+StaticObjects.Player.ChampionName+".Boolean.DrawOnSelf.WColor", "W Range").SetValue(new Circle(true, Color.LightGreen)));
            if (drawingOptions[2])
                selfMenu.AddItem(new MenuItem(Names.Menu.DrawingItemBase+StaticObjects.Player.ChampionName+".Boolean.DrawOnSelf.EColor", "E Range").SetValue(new Circle(true, Color.LightCoral)));
            if (drawingOptions[03])
                selfMenu.AddItem(new MenuItem(Names.Menu.DrawingItemBase+StaticObjects.Player.ChampionName+".Boolean.DrawOnSelf.RColor", "R Range").SetValue(new Circle(true, Color.LightSlateGray)));

            var lastHitMenu=new Menu("Minions", Names.Menu.DrawingItemBase+"Minions");

            lastHitMenu.AddItem(new MenuItem(Names.Menu.DrawingItemBase+".Minion."+"Circle.LastHitHelper", "LastHit Helper").SetValue(new Circle(true, Color.LightGray)));

            menu.AddSubMenu(lastHitMenu);
            menu.AddSubMenu(enemyMenu);
            menu.AddSubMenu(selfMenu);

            return menu;
        }

        #endregion Public Methods
    }

}