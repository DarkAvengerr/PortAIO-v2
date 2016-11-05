using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace S_Plus_Class_Kalista.Handlers
{
    internal class Humanizer : Core
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Random _random = new Random();
        private static float _lastAttackTick, _lastMoveTick;
        private static float _currentAttackDelay = 500f;
        private static float _currentMoveDelay = 500f;
        public static long BlockedCommands;

        private const string MenuNameBase = ".Humanizer Menu";
        private const string MenuItemBase = ".Humanizer.";

        private const string DelayMenuNameBase = ".Delays.";
        public const string DelayItemBase = MenuItemBase + "Delays.";



        public static void Load()
        {
            // Obj_AI_Base.OnIssueOrder += OnIssueOrder;
            SMenu.AddSubMenu(_Menu());
        }

        private static Menu _Menu()
        {
            var menu = new Menu(MenuNameBase, "humanMenu");

            var subMenuDelay = new Menu(DelayMenuNameBase, "delayMenu");

            subMenuDelay.AddItem(
                new MenuItem(DelayItemBase + "Slider.RendDelay", "Rend Delay").SetValue(new Slider(300, 200, 1250)));
            subMenuDelay.AddItem(
                new MenuItem(DelayItemBase + "Slider.NonKillableDelay", "NonKillable Delay").SetValue(new Slider(500,
                    250, 1500)));
            subMenuDelay.AddItem(
                new MenuItem(DelayItemBase + "Slider.LevelDelay", "Level Delay").SetValue(new Slider(500, 50, 1000)));
            subMenuDelay.AddItem(
                new MenuItem(DelayItemBase + "Slider.EventDelay", "Check Delay").SetValue(new Slider(100, 50, 200)));
            subMenuDelay.AddItem(
                new MenuItem(DelayItemBase + "Slider.SoulBoundDelay", "SoulBound Delay").SetValue(new Slider(250, 50,
                    1000)));
            subMenuDelay.AddItem(
                new MenuItem(DelayItemBase + "Slider.ItemDelay", "Item Delay").SetValue(new Slider(200, 50, 1000)));
            subMenuDelay.AddItem(
                new MenuItem(DelayItemBase + "Slider.TrinketDelay", "Trinket Delay").SetValue(new Slider(500, 50, 2000)));
            subMenuDelay.AddItem(
                new MenuItem(DelayItemBase + "Slider.MinSeedDelay", "Minimum Random Delay").SetValue(new Slider(0, 0,
                    500)));
            subMenuDelay.AddItem(
                new MenuItem(DelayItemBase + "Slider.MaxSeedDelay", "Maximum Random Delay").SetValue(new Slider(50, 0,
                    500)));

            menu.AddSubMenu(subMenuDelay);


            return menu;
        }

        public class Limiter
        {
            private static readonly Random Rand = new Random();
            public static readonly Dictionary<string, NewLevelShit> Delays = new Dictionary<string, NewLevelShit>();

            private static float _fMin = 0f, _fMax = 250f;

            public struct NewLevelShit
            {
                public readonly float Delay;
                public readonly float LastTick;

                public NewLevelShit(float delay, float lastTick)
                {
                    Delay = delay;
                    LastTick = lastTick;
                }
            }


            private static string[] _sDelays =
            {
                "RendDelay", "NonKillableDelay", "LevelDelay", "EventDelay",
                "SoulBoundDelay", "ItemDelay", "TrinketDelay"
            };

            private static void LoadDelays()
            {

                try
                {
                    foreach (var sDelay in _sDelays.Where(sDelay => !Delays.ContainsKey(sDelay)))
                    {
                        Delays.Add($"{Humanizer.DelayItemBase}Slider.{sDelay}",
                            new NewLevelShit(
                                SMenu.Item($"{Humanizer.DelayItemBase}Slider.{sDelay}").GetValue<Slider>().Value, 0f));
                    }
                }
                catch
                {
                    //Fuck her right in the pussy
                }
                finally
                {
                    _fMin = SMenu.Item($"{Humanizer.DelayItemBase}Slider.MinSeedDelay").GetValue<Slider>().Value;
                    _fMax = SMenu.Item($"{Humanizer.DelayItemBase}Slider.MaxSeedDelay").GetValue<Slider>().Value;
                }

            }

            public static bool CheckDelay(string key)
            {
                if (Delays.ContainsKey(key))
                    return Delays[key].LastTick - Core.Time.TickCount < Delays[key].Delay;

                LoadDelays();

                return false;
            }

            public static void UseTick(string key)
            {
                Delays[key] = new NewLevelShit(Delays[key].Delay,
                    Time.TickCount + Rand.NextFloat(_fMin, _fMax)); //Randomize delay
            }
        }
    }
}



    

