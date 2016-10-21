using LeagueSharp.Common;
using globals = KAPPALISTAXD.Core.KappalistaGlobals;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace KAPPALISTAXD.Core
{
    class KappalistaMenu
    {
        public KappalistaMenu()
        {
            globals.mainMenu = new Menu("KAPPALISTA XD", "main", true);

            Menu tsMenu = new Menu("Target selector", "ts", false);
            tsMenu.AddItem(new MenuItem("ts-info", "Target selector is now in the Common menu", false));
            globals.mainMenu.AddSubMenu(tsMenu);

            Menu orbwalkerMenu = new Menu("Orbwalker", "orbwalk", false);
            globals.orbwalkerInstance = new Orbwalking.Orbwalker(orbwalkerMenu);
            globals.mainMenu.AddSubMenu(orbwalkerMenu);

            Menu drawingMenu = new Menu("Drawing", "drawing", false);
            drawingMenu.AddItem(new MenuItem("drawing-q", "Draw Q", false).SetValue(true));
            drawingMenu.AddItem(new MenuItem("drawing-w", "Draw W", false).SetValue(false));
            drawingMenu.AddItem(new MenuItem("drawing-e", "Draw E", false).SetValue(true));
            drawingMenu.AddItem(new MenuItem("drawing-r", "Draw R", false).SetValue(false));
            globals.mainMenu.AddSubMenu(drawingMenu);
            
            Menu comboMenu = new Menu("Combo", "combo", false);
            comboMenu.AddItem(new MenuItem("combo-q", "Use Q", false).SetValue(true));
            comboMenu.AddItem(new MenuItem("combo-e", "Use E", false).SetValue(true));
            globals.mainMenu.AddSubMenu(comboMenu);

            Menu harassMenu = new Menu("Harass", "harass", false);
            harassMenu.AddItem(new MenuItem("harass-q", "Use Q", false).SetValue(true));
            globals.mainMenu.AddSubMenu(harassMenu);

            Menu jungClearMenu = new Menu("Lane clear", "jgclear", false);
            jungClearMenu.AddItem(new MenuItem("jgclear-e", "Use E", false).SetValue(true));
            jungClearMenu.AddItem(new MenuItem("jgclear-e-pro", "Use E when number hitted", false).SetValue(new Slider(3, 1, 7)));
            globals.mainMenu.AddSubMenu(jungClearMenu);

            Menu killStealMenu = new Menu("Killsteal", "ks", false);
            killStealMenu.AddItem(new MenuItem("ks-e", "Killsteal champions with E", false).SetValue(true));
            killStealMenu.AddItem(new MenuItem("ks-db", "Killsteal jungle with E", false).SetValue(true));
            globals.mainMenu.AddSubMenu(killStealMenu);

            Menu miscMenu = new Menu("Misc", "misc", false);
            miscMenu.AddItem(new MenuItem("misc-prevent-e", "Prevent E on Spellshields & Invulnerable", false).SetValue(true));
            miscMenu.AddItem(new MenuItem("misc-lasthit-e", "Lasthit minions with E", false).SetValue(true));

            miscMenu.AddItem(new MenuItem("misc-dying-e", "E before dying", false).SetValue(true));
            miscMenu.AddItem(new MenuItem("misc-dying-e-pro", "E dying on %", false).SetValue(new Slider(10, 1, 50)));

            miscMenu.AddItem(new MenuItem("misc-leaving-e", "E when leaving range", false).SetValue(true));
            miscMenu.AddItem(new MenuItem("misc-leaving-e-pro", "E leaving stacks", false).SetValue(new Slider(5, 1, 10)));

            miscMenu.AddItem(new MenuItem("misc-use-r", "Save soulbound with R", false).SetValue(true));
            miscMenu.AddItem(new MenuItem("misc-use-r-pro", "Soulbound % health ally", false).SetValue(new Slider(20, 1, 50)));

            miscMenu.AddItem(new MenuItem("misc-ward-trick", "Sentinel trick baron/drag", false).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));

            globals.mainMenu.AddSubMenu(miscMenu);

            globals.mainMenu.AddToMainMenu();
        }
    }
}
