using System;
using System.Collections.Generic;
using System.Linq;

using LeagueSharp;
using LeagueSharp.Common;

using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BlackFeeder
{

    using Color = System.Drawing.Color;
    using Menu = LeagueSharp.Common.Menu;

    internal class Entry
    {
        #region Static Fields

        #region SpellList

        public static List<ChampWrapper> ListChamp = new List<ChampWrapper>()
                                                         {
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Blitzcrank",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.W }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Bard",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.W }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "DrMundo",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.R }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Draven",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.W }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Evelynn",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.W }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Garen",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.Q }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Hecarim",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.E }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Karma",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.E }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Kayle",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.W }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Kennen",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.E }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Lulu",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.W }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "MasterYi",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.R }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Nunu",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.W }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Olaf",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.R }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Orianna",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.W }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Poppy",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.W }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Quinn",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.R }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Rammus",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.Q }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Rumble",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.W }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Ryze",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.R }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Shyvana",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.W }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Singed",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.R }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Sivir",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.R }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Skarner",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.W }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Sona",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.E }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Teemo",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.W }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Trundle",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.W }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Twitch",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.Q }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Udyr",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.E }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Volibear",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.Q }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Zilean",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.W, SpellSlot.E }
                                                                 }
                                                         };
        #endregion

        public static Menu Menu;

        private static string[] deaths;

        private static AIHeroClient player;
        private static SpellSlot ghostSlot, healSlot;

        private static readonly Vector3 TopVector3 = new Vector3(2122, 12558, 53);
        private static readonly Vector3 BotVector3 = new Vector3(12608, 2380, 52);
        private static readonly Vector3 PurpleSpawn = new Vector3(14286f, 14382f, 172f);
        private static readonly Vector3 BlueSpawn = new Vector3(416f, 468f, 182f);

        public static bool TopVectorReached;
        public static bool BotVectorReached;

        private static int globalRand;

        private static bool surrenderActive;
        private static int surrenderTime;
        private static float realTime;

        private static int lastLaugh;
        private static double lastTouchdown;
        private static double timeDead;

        #endregion

        #region OnLoad

        public static void OnLoad()
        {
            try
            {
                deaths = new[]
                         {
                             "/all XD", "kek", "sorry lag", "/all gg", "help pls", "nooob wtf", "team???", "/all gg my team sucks",
                             "/all matchmaking sucks", "i can't carry dis", "wtf how?", "wow rito nerf pls",
                             "/all report enemys for drophacks", "tilidin y u do dis", "kappa", "amk", "/all einfach mal leben genießen amk"
                         };

                player = ObjectManager.Player;
                ghostSlot = player.GetSpellSlot("SummonerHaste");
                healSlot = player.GetSpellSlot("SummonerHeal");

                if (player.Gold >= 0)
                {
                    realTime = Game.Time;
                }

                ShowNotification("BlackFeeder by blacky - Loaded", Color.Crimson, 10000);

                InitializeMenu.Load();
                SRShopAI.Main.Init(); //Credits to Insensitivity for his amazing "ARAMShopAI" assembly
                Game.OnUpdate += OnUpdate;
                Game.OnEnd += OnEnd;
                EloBuddy.Player.OnIssueOrder += OnIssueOrder;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion

        #region OnUpdate

        private static void OnUpdate(EventArgs args)
        {
            if (Menu.Item("Feeding.Activated").GetValue<bool>())
            {
                Feed();
            }

            if (Menu.Item("Surrender.Activated").GetValue<bool>())
            {
                Surrender(); 
            }

            if (player.IsDead || player.InFountain())
            {
                TopVectorReached = false;
                BotVectorReached = false;
            }
            else
            {
                if (player.Distance(BotVector3) <= 300)
                {
                    BotVectorReached = true;
                }

                if (player.Distance(TopVector3) <= 300)
                {
                    TopVectorReached = true;
                }
            }
        }

        #endregion

        #region OnIssueOrder

        private static void OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (!Menu.Item("Attacks.Disabled").GetValue<bool>())
            {
                return;
            }

            if (sender.IsMe && args.Order == GameObjectOrder.AttackTo)
            {
                args.Process = false;
            }
        }

        #endregion

        #region OnEnd

        private static void OnEnd(EventArgs args)
        {
            Chat.Say("/all Good game guys, well played.");

            if (Menu.Item("Quit.Activated").GetValue<bool>())
            {
                LeagueSharp.Common.Utility.DelayAction.Add(3000, () => Game.QuitGame());
            }
        }

        #endregion

        #region Feed

        private static void Feed()
        {
            var feedingMode = Menu.Item("Feeding.FeedMode").GetValue<StringList>().SelectedIndex;

            if (feedingMode == 3 && globalRand == -1)
            {
                var rnd = new Random();
                globalRand = rnd.Next(0, 3);
            }

            if (feedingMode != 3)
            {
                globalRand = -1;
            }

            if (player.IsDead)
            {
                globalRand = -1;
            }

            if (globalRand != -1)
            {
                feedingMode = globalRand;
            }

            switch (feedingMode)
            {
                case 0:
                    {
                        if (player.Team == GameObjectTeam.Order)
                        {
                            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, PurpleSpawn);
                        }
                        else
                        {
                            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, BlueSpawn);
                        }
                    }
                    break;
                case 1:
                    {
                        if (player.Team == GameObjectTeam.Order)
                        {
                            if (!BotVectorReached)
                                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, BotVector3);
                            else if (BotVectorReached)
                                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, PurpleSpawn);
                        }
                        else
                        {
                            if (!BotVectorReached)
                                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, BotVector3);
                            else if (BotVectorReached)
                                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, BlueSpawn);
                        }
                    }
                    break;
                case 2:
                    {
                        if (player.Team == GameObjectTeam.Order)
                        {
                            if (!TopVectorReached)
                                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, TopVector3);
                            else if (TopVectorReached)
                                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, PurpleSpawn);
                        }
                        else
                        {
                            if (!TopVectorReached)
                                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, TopVector3);
                            else if (TopVectorReached)
                                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, BlueSpawn);
                        }
                    }
                    break;
            }

            if (Menu.Item("Spells.Activated").GetValue<bool>())
            {
                Spells();
            }

            if (Menu.Item("Messages.Activated").GetValue<bool>())
            {
                Messages();
            }

            if (Menu.Item("Laugh.Activated").GetValue<bool>())
            {
                Laughing();
            }
        }

        #endregion  

        #region Spells

        private static void Spells()
        {
            if (player.Distance(PurpleSpawn) < 600 | player.Distance(BlueSpawn) < 600)
            {
                return;
            }

            if (ghostSlot != SpellSlot.Unknown && player.Spellbook.CanUseSpell(ghostSlot) == SpellState.Ready)
            {
                player.Spellbook.CastSpell(ghostSlot);
            }

            if (healSlot != SpellSlot.Unknown && player.Spellbook.CanUseSpell(healSlot) == SpellState.Ready)
            {
                player.Spellbook.CastSpell(healSlot);
            }

            var entry = ListChamp.FirstOrDefault(h => h.Name == ObjectManager.Player.ChampionName);

            if (entry == null)
            {
                return;
            }

            var slots = entry.SpellSlots;

            foreach (var slot in slots)
            {
                player.Spellbook.LevelSpell(slot);
                if (player.Spellbook.CanUseSpell(slot) == SpellState.Ready)
                {
                    player.Spellbook.CastSpell(slot, player);
                }
            }
        }

        #endregion

        #region Laughing

        private static void Laughing()
        {
            if (Environment.TickCount <= lastLaugh + 2500)
            {
                return;
            }

            Chat.Say("/l");
            lastLaugh = Environment.TickCount;
        }

        #endregion

        #region Messages

        private static void Messages()
        {
            if (player.IsDead && Game.Time - timeDead > 80)
            {
                var r = new Random();
                Chat.Say(deaths[r.Next(0,17)]);
                timeDead = Game.Time;
            }

            if (player.Team == GameObjectTeam.Chaos && player.Distance(BlueSpawn) < 600)
            {
                if (Game.Time - lastTouchdown > 80)
                {
                    Chat.Say("/all TOUCHDOWN!");
                    lastTouchdown = Game.Time;
                }
            }

            if (player.Team == GameObjectTeam.Order && player.Distance(PurpleSpawn) < 600)
            {
                if (Game.Time - lastTouchdown > 80)
                {
                    Chat.Say("/all TOUCHDOWN!");
                    lastTouchdown = Game.Time;
                }
            }
        }

        #endregion

        #region Surrender

        private static void Surrender()
        {
            if (Game.Time - realTime >= 1200 && !surrenderActive)
            {
                Chat.Say("/ff");
                surrenderActive = true;
                surrenderTime = Environment.TickCount;
            }

            if (surrenderActive)
            {
                if (Environment.TickCount - surrenderTime > 180000)
                {
                    surrenderActive = false;
                }
            }
        }

        #endregion

        #region Notifications Credits to Beaving.

        public static Notification ShowNotification(string message, Color color, int duration = -1, bool dispose = true)
        {
            var notif = new Notification(message).SetTextColor(color);
            Notifications.AddNotification(notif);
            if (dispose)
            {
                LeagueSharp.Common.Utility.DelayAction.Add(duration, () => notif.Dispose());
            }
            return notif;
        }

        #endregion
    }
}