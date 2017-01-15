using EloBuddy; 
using LeagueSharp.Common; 
namespace StreamingAnnouncer
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Color = System.Drawing.Color;

    class Program
    {
        /// <summary>
        ///     The menu.
        /// </summary>
        public static Menu Menu;

        /// <summary>
        ///     Announcer box.
        /// </summary>
        public static bool HideAnnouncer = false;

        /// <summary>
        ///     The last announcer notification.
        /// </summary>
        private static float lastAnnouncer;

        /// <summary>
        ///     The Check Interval
        /// </summary>
        private const float CheckInterval = 5000f;

        /// <summary>
        ///     Gets the top offset of the HUD elements
        /// </summary>
        private static int HudOffsetTop => Menu.Item("Stream.OffsetTop").GetValue<Slider>().Value;

        /// <summary>
        ///     Gets the right offset of the announcer hider.
        /// </summary>
        private static int HudOffsetRight => Menu.Item("Stream.OffsetRight").GetValue<Slider>().Value;

        /// <summary>
        ///     Gets the right offset of the announcer hider.
        /// </summary>
        private static int HudBarWidth => Menu.Item("Stream.Width").GetValue<Slider>().Value;

        /// <summary>
        ///     Gets the right offset of the announcer hider.
        /// </summary>
        private static int HudBarHeight => Menu.Item("Stream.Height").GetValue<Slider>().Value;

        /// <summary>
        ///     Entry point.
        /// </summary>
        /// <param name="args"></param>
        public static void Main()
        {
            MenuInitializer();
            Game.OnNotify += OnNotify;
            Drawing.OnDraw += Drawing_OnEndScene;
            Game.OnProcessPacket += Game_OnGameProcessPacket;
        }

        /// <summary>
        ///     On packet process.
        /// </summary>
        /// <param name="args"></param>
        private static void Game_OnGameProcessPacket(GamePacketEventArgs args)
        {
            Disconnect(args);
            Reconnect(args);
        }

        /// <summary>
        ///     Disconnect packet process.
        /// </summary>
        /// <param name="args"></param>
        private static void Disconnect(GamePacketEventArgs args)
        {
            var reader = new BinaryReader(new MemoryStream(args.PacketData));
            byte packetId = reader.ReadByte();
            int packet = -1;

            if (Game.Version.Contains("6.24"))
            {
                packet = 13;
            }

            if (packetId != packet || args.PacketData.Length != 12)
            {
                return;
            }

            HideAnnouncer = true;
            lastAnnouncer = Environment.TickCount;
            Console.WriteLine("[Streaming {0:HH:mm:ss}] Hiding Disconect", DateTime.Now);
        }

        /// <summary>
        ///     Reconnect packet process.
        /// </summary>
        /// <param name="args"></param>
        private static  void Reconnect(GamePacketEventArgs args)
        {
            var reader = new BinaryReader(new MemoryStream(args.PacketData));
            byte packetId = reader.ReadByte();
            int packet = -1;

            if (Game.Version.Contains("6.24"))
            {
                packet = 65;
            }

            if (packetId != packet)
            {
                return;
            }

            HideAnnouncer = true;
            lastAnnouncer = Environment.TickCount;
            Console.WriteLine("[Streaming {0:HH:mm:ss}] Hiding Reconnect", DateTime.Now);
        }
        /// <summary>
        ///     The Menu.
        /// </summary>
        private static void MenuInitializer()
        {
            Menu = new Menu("ElStreaming", "Stream", true).SetFontStyle(FontStyle.Bold, SharpDX.Color.GreenYellow);

            Menu.AddItem(new MenuItem("Activate-Stream", "Hide Announcer").SetValue(true));
            Menu.AddItem(new MenuItem("Stream.OffsetTop", "Offset Right").SetValue(new Slider(600, 0, 1500)));
            Menu.AddItem(new MenuItem("Stream.OffsetRight", "Offset Top").SetValue(new Slider(140, 0, 1500)));
            Menu.AddItem(new MenuItem("Stream.Width", "Bar Width").SetValue(new Slider(700, 0, 1500)));
            Menu.AddItem(new MenuItem("Stream.Height", "Bar Height").SetValue(new Slider(45)));
            Menu.AddItem(new MenuItem("Stream.Color", "Bar Color").SetValue(new Circle(true, System.Drawing.Color.DeepPink)));
            Menu.AddItem(new MenuItem("empty-line-3000", string.Empty));
            Menu.AddItem(new MenuItem("AddTestCard", "Draw Test Bar").SetValue(false).DontSave());

            Menu.Item("AddTestCard").ValueChanged += (sender, args) =>
            {
                args.Process = false;
                lastAnnouncer = Environment.TickCount;
                HideAnnouncer = true;
            };

            Menu.AddToMainMenu();
        }

        /// <summary>
        ///     Called on notification.
        /// </summary>
        /// <param name="args"></param>
        private static void OnNotify(GameNotifyEventArgs args)
        {
            if (args.EventId == GameEventId.OnChampionDoubleKill 
                || args.EventId == GameEventId.OnChampionTripleKill
                || args.EventId == GameEventId.OnChampionQuadraKill 
                || args.EventId == GameEventId.OnChampionPentaKill
                || args.EventId == GameEventId.OnChampionUnrealKill
                || args.EventId == GameEventId.OnChampionDie
                || args.EventId == GameEventId.OnAce
                || args.EventId == GameEventId.OnFirstBlood 
                || args.EventId == GameEventId.OnTurretDie
                || args.EventId == GameEventId.OnTurretKill 
                || args.EventId == GameEventId.OnChampionKill
                || args.EventId == GameEventId.OnKillingSpree 
                || args.EventId == GameEventId.OnKillingSpreeSet1
                || args.EventId == GameEventId.OnKillingSpreeSet2 
                || args.EventId == GameEventId.OnKillingSpreeSet3
                || args.EventId == GameEventId.OnKillingSpreeSet4 
                || args.EventId == GameEventId.OnKillingSpreeSet5
                || args.EventId == GameEventId.OnKillingSpreeSet6 
                || args.EventId == GameEventId.OnKillWorm
                || args.EventId == GameEventId.OnKillWormSteal
                || args.EventId == GameEventId.OnKillDragon 
                //|| args.EventId == GameEventId.rift
                || args.EventId == GameEventId.OnChampionSingleKill 
                || args.EventId == GameEventId.OnKillDragonSteal
                || args.EventId == GameEventId.OnReconnect 
                || args.EventId == GameEventId.OnQuit
                //|| args.EventId == GameEventId.OnShutdown 
                || args.EventId == GameEventId.OnLeave
                || args.EventId == GameEventId.OnTurretDie)
            {
                HideAnnouncer = true;
                lastAnnouncer = Environment.TickCount;
                Console.WriteLine("[Streaming {0:HH:mm:ss}] Hiding {1}", DateTime.Now, args.EventId);

                if (HideAnnouncer && lastAnnouncer > Environment.TickCount)
                {
                    lastAnnouncer = Environment.TickCount + 3000;
                    Console.WriteLine("[Streaming EXTEND  {0:HH:mm:ss}] Hiding {1}", DateTime.Now, args.EventId);
                }
            }
        }

        /// <summary>
        ///     The drawings.
        /// </summary>
        /// <param name="args"></param>
        private static void Drawing_OnEndScene(EventArgs args)
        {
            if (HideAnnouncer && Menu.Item("Activate-Stream").GetValue<bool>())
            {
                DrawRect(HudOffsetTop, HudOffsetRight, HudBarWidth, HudBarHeight, 1, Menu.Item("Stream.Color").GetValue<Circle>().Color);

                if (lastAnnouncer + CheckInterval > Environment.TickCount)
                {
                    return;
                }

                HideAnnouncer = false;
            }
        }

        /// <summary>
        ///     Draws a rectangle
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="thickness"></param>
        /// <param name="color"></param>
        private static void DrawRect(float x, float y, int width, float height, float thickness, Color color)
        {
            for (var i = 0; i < height; i++)
            {
                Drawing.DrawLine(x, y + i, x + width, y + i, thickness, color);
            }
        }
    }
}
