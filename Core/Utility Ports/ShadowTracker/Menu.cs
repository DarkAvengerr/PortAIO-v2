using System;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;


using EloBuddy; namespace ShadowTracker
{
    class MainMenu
    {
        public static Menu _MainMenu;        
        public static void Menu()
        {
            try // try start
            {
                _MainMenu = new Menu("ShadowTracker", "ShadowTracker", true);
                _MainMenu.AddToMainMenu();

                var Draw = new Menu("Draw", "Draw");
                {
                    Draw.AddItem(new MenuItem("Skill", "Skill").SetValue(true));
                    Draw.AddItem(new MenuItem("Spell", "Spell").SetValue(true));
                    Draw.AddItem(new MenuItem("Item", "Item").SetValue(true));
                }
                _MainMenu.AddSubMenu(Draw);                
            } // try end     
            catch (Exception e)
            {
                Console.Write(e);
                Chat.Print("ShadowTracker is not working. plz send message by KorFresh (Code 1)");
            }           
            
        }
    }
}
