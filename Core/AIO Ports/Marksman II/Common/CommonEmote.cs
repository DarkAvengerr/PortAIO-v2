using System;
using System.Drawing;
using LeagueSharp;
using LeagueSharp.Common;
using Color = SharpDX.Color;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Marksman.Common
{
    using System.Linq;
    using System.Security.AccessControl;

    public static class CommonEmote
    {
        public static Menu LocalMenu;

        public static int[] SpellLevels;

        public static void Init(Menu nParentMenu)
        {

            LocalMenu = new Menu("Emote", "Common.Emote").SetFontStyle(FontStyle.Regular, Color.IndianRed);
            LocalMenu.AddItem(new MenuItem("Emote.Kill", "Kill:").SetValue(new StringList(new []{ "Off", "Master Badge", "Laugh", "Taunt", "Joke", "Dance"})));
            LocalMenu.AddItem(new MenuItem("Emote.Assist", "Assist:").SetValue(new StringList(new[] { "Off", "Master Badge", "Laugh", "Taunt", "Joke", "Dance" })));
            LocalMenu.AddItem(new MenuItem("Emote.Victory", "Victory:").SetValue(new StringList(new[] { "Off", "Master Badge", "Laugh", "Taunt", "Joke", "Dance" })));
            LocalMenu.AddItem(new MenuItem("Emote.Enable", "Enable:").SetValue(true)).SetFontStyle(FontStyle.Regular, Color.GreenYellow);
            nParentMenu.AddSubMenu(LocalMenu);

            Game.OnNotify += GameOnOnNotify;
            
        }

        static void ExecuteEmote(int nEmote)
        {
            switch (nEmote)
            {
                case 1:
                    Chat.Say("/masterybadge");
                    break;
                case 2:
                    Chat.Say("/l");
                    break;
                case 3:
                    Chat.Say("/t");
                    break;
                case 4:
                    Chat.Say("/j");
                    break;
                case 5:
                    Chat.Say("/d");
                    break;
            }
        }

        private static void GameOnOnNotify(GameNotifyEventArgs args)
        {
            if (!LocalMenu.Item("Emote.Enable").GetValue<bool>())
            {
                return;
            }

            var nEmoteKill = LocalMenu.Item("Emote.Kill").GetValue<StringList>().SelectedIndex;
            if (nEmoteKill != 0 && args.EventId == GameEventId.OnChampionKill)
            {
                ExecuteEmote(nEmoteKill);
            }

            nEmoteKill = LocalMenu.Item("Emote.Assist").GetValue<StringList>().SelectedIndex;
            if (nEmoteKill != 0 && args.EventId == GameEventId.OnDeathAssist)
            {
                ExecuteEmote(nEmoteKill);
            }

            nEmoteKill = LocalMenu.Item("Emote.Victory").GetValue<StringList>().SelectedIndex;
            if (nEmoteKill != 0 && args.EventId == GameEventId.OnVictoryPointThreshold1)
            {
                ExecuteEmote(nEmoteKill);
            }
        }
    }
}
