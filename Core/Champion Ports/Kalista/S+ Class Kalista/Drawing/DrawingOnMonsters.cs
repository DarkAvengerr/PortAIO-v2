using System;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;
using Damage = S_Plus_Class_Kalista.Libaries.Damage;
using EloBuddy;

namespace S_Plus_Class_Kalista.Drawing
{
    internal class DrawingOnMonsters : Core
    {
        private const string _MenuNameBase = ".Monster Menu";
        private const string _MenuItemBase = ".Monster.";

        public static Menu DrawingMonsterMenu()
        {
            var menu = new Menu(_MenuNameBase, "monsterMenu");
            var epicMenu = new Menu(".Epics", "epicMenu");
            epicMenu.AddItem(
                new MenuItem(_MenuItemBase + "Boolean.DrawOnEpics", "Draw Damage On epics(Dragon/Baron)").SetValue(true));
            epicMenu.AddItem(
                new MenuItem(_MenuItemBase + "Boolean.DrawOnEpics.InnerColor", "Inner Market Color").SetValue(
                    new Circle(true, Color.DeepSkyBlue)));
            epicMenu.AddItem(
                new MenuItem(_MenuItemBase + "Boolean.DrawOnEpics.OuterColor", "Outer Marker Color").SetValue(
                    new Circle(true, Color.Crimson)));

            var monsterMenu = new Menu(".NormalMonsters", "normalMonsterMenu");
            monsterMenu.AddItem( new MenuItem(_MenuItemBase + "Boolean.DrawOnMonsters", "Draw Damage On Monsters").SetValue(true));
            monsterMenu.AddItem(new MenuItem(_MenuItemBase + "Boolean.DrawOnMonsters.Fill", "Fill Damage Color").SetValue(
                    new Circle(true, Color.DarkGray)));

            monsterMenu.AddItem(new MenuItem(_MenuItemBase + "Boolean.DrawOnMonsters.NonFill", "NonFill Damage Color").SetValue(new Circle(false, Color.LightSlateGray)));

            menu.AddSubMenu(epicMenu);
            menu.AddSubMenu(monsterMenu);

            menu.AddItem(new MenuItem(_MenuItemBase + "Boolean.DrawOnMonsters.KillableColor", "Killable Text").SetValue( new Circle(true, Color.Red)));

            return menu;
        }

        public static void OnDrawMonster(EventArgs args)
        {
        }
    }
}