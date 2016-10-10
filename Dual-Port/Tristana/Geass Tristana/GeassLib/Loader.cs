using System;
using GeassLib.Functions.Logging;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace GeassLib
{
    public class Loader
    {
        // ReSharper disable once NotAccessedField.Local
        private  Menus.OnLevel _onLevelMenu;
        // ReSharper disable once NotAccessedField.Local
        private Menus.Trinket _trinket;
        // ReSharper disable once NotAccessedField.Local
        private Menus.Items _items;
        // ReSharper disable once NotAccessedField.Local
        private Menus.Drawing _drawing;

        public Loader(string ext,bool useTrinket = false,bool useOnLevel = false, int[] abiSeq = null,bool useDrawing=false,bool useItems=false)
        {
            Globals.Variables.AssemblyName = $"GeassLib.{ext}";

            Globals.Objects.Logger = new Logger(Globals.Variables.AssemblyName);
            Globals.Objects.GeassLibMenu = new Menu(Globals.Variables.AssemblyName, Globals.Variables.AssemblyName,true);
            Globals.Objects.AssemblyLoadTime = DateTime.Now;
            Globals.Objects.Player = ObjectManager.Player;




            //Create Menus
            if (useTrinket)
                _trinket = new Menus.Trinket();

            if (useDrawing)
                _drawing = new Menus.Drawing();

            if (useItems)
                _items = new Menus.Items();

            if (useOnLevel)
            {
                if (abiSeq != null && abiSeq.Length > 0)
                    _onLevelMenu = new Menus.OnLevel(abiSeq);
            }


            Globals.Objects.GeassLibMenu.AddToMainMenu(); // Add Menu's to main menu

        }
    }
}