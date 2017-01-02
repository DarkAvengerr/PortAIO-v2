using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
namespace BasicChatBlock
{
    class Program
    {

        public static Menu main;

        public static MenuItem enabledItem;
        public static bool isEnabled => enabledItem.GetValue<bool>();

        public static void Main()
        {
            Game_OnGameLoad(new EventArgs());
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            main = new Menu("BasicChatBlocker", "com.chatblocker", true);
            enabledItem = new MenuItem("com.chatblocker.enabled", "Enabled?", true).SetValue(true);
            main.AddItem(enabledItem);
            main.AddToMainMenu();

            Chat.Say("/mute all");

            Chat.OnMessage += Game_OnChat;
        }

        private static void Game_OnChat(AIHeroClient sender, ChatMessageEventArgs args)
        {
            if (!isEnabled) return;
            if (!sender.IsMe) return;
            
            args.Process = false;
        }
    }
}
