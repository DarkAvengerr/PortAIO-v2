#region

using System;
using System.Linq;
using Arcane_Ryze.Draw;
using Arcane_Ryze.Handler;
using Arcane_Ryze.Main;
using Arcane_Ryze.Modes;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using SharpDX;

#endregion

using EloBuddy;
using LeagueSharp.SDK;
namespace Arcane_Ryze
{
    internal class Program : Core
    {
        Random Random = new Random();
        internal static float Timer;
        internal static float LastQ;

        public static void Main()
        {
            Bootstrap.Init(null);
            Load();
        }

        private static void Load()
        {

            if (GameObjects.Player.ChampionName != "Ryze")
            {
                return;
            }
            Chat.Print("<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#00e5e5\">Arcane Ryze</font></b><b><font color=\"#FFFFFF\">]</font></b><b><font color=\"#FFFFFF\"> Version: 1</font></b>");
            Chat.Print("<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#00e5e5\">Update</font></b><b><font color=\"#FFFFFF\">]</font></b><b><font color=\"#FFFFFF\"> Release</font></b>");

            Spells.Load();
            MenuConfig.Load();
            Orbwalker.OnAction += BeforeAA.OnAction;

            Game.OnUpdate += OnUpdate;
            Drawing.OnEndScene += Drawing_OnEndScene;
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead || Player.IsRecalling())
            {
                return;
            }
            //  Console.WriteLine("Buffs: {0}", string.Join(" | ", Player.Buffs.Where(b => b.Caster.NetworkId == Player.NetworkId).Select(b => b.DisplayName)));


            // Useless Code.
            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkingMode.Combo:
                    Combo.ComboLogic();
                    break;
                case OrbwalkingMode.Hybrid:
                    Harass.HarassLogic();
                    break;
                case OrbwalkingMode.LaneClear:
                    Jungle.JungleLogic();
                    Lane.LaneLogic();
                    break;
                case OrbwalkingMode.LastHit:
                    LastHit.LastHitLogic();
                    break;
            }
            KillSteal.Killsteal();

        }
        private static void Drawing_OnEndScene(EventArgs args)
        {
            
        }
    }
}
