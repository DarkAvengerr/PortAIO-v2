using System;
using System.Drawing;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.SDK.Utils;
using Color = SharpDX.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace TiltSharp
{
    internal class Program
    {
        public static Menu Menu;

        public static void Main()
        {
            CustomEvents.Game.OnGameLoad += Load;
            Obj_AI_Base.OnSpellCast += OnSpellCast;
        }

        public static void Load(EventArgs args)
        {
            Menu = new Menu(" :: TiltSharp :^)", "tiltsharpmenu", true).SetFontStyle(FontStyle.Regular, Color.Pink);
            Menu.AddItem(
                new MenuItem("info", "Assembly Version 4 - Built 4.8.2016 for Patch 6.15").SetFontStyle(FontStyle.Bold));
            Menu.AddItem(new MenuItem("seperator1", ""));
            Menu.AddItem(new MenuItem("aaLaugh", "Laugh on AA").SetValue(true));
            Menu.AddItem(new MenuItem("aaBadge", "Mastery on AA").SetValue(true));
            Menu.AddItem(new MenuItem("badgeEvents", "Mastery on Events (Coming soon)"));
            Menu.AddItem(new MenuItem("seperator2", ""));
            Menu.AddItem(new MenuItem("tyvmExory", "Exory is gay for providing c/p code tyvm <3"));
            Menu.AddToMainMenu();
        }

        public static void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe &&
                AutoAttack.IsAutoAttack(args.SData.Name))
            {
                if (Menu.Item("aaLaugh").GetValue<bool>())
                {
                    Chat.Say("/l");
                }

                if (Menu.Item("aaBadge").GetValue<bool>())
                {
                    Chat.Say("/masterybadge");
                }
            }
        }
    }
}
