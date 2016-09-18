using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoKingdom.BadaoChampion.BadaoElise
{
    public static class BadaoEliseSpellsManager
    {
        private static AIHeroClient Player { get{ return ObjectManager.Player; } }
        public static int LastQT, LastQ2T, LastWT, LastW2T, LastET, LastE2T;
        public static void BadaoActivate()
        {
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            //Chat.Print(Player.PercentCooldownMod.ToString());
        }

        // EliseHumanW EliseHumanE EliseSpiderW EliseHumanQ EliseSpiderQCast EliseSPiderEInitial EliseR, EliseRSpider
        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
                return;
            switch (args.SData.Name)
            {
                case "EliseHumanQ":
                    LastQT = Environment.TickCount;
                    break;
                case "EliseHumanW":
                    LastWT = Environment.TickCount;
                    break;
                case "EliseHumanE":
                    LastET = Environment.TickCount;
                    break;
                case "EliseSpiderQCast":
                    LastQ2T = Environment.TickCount;
                    break;
                case "EliseSpiderW":
                    LastW2T = Environment.TickCount;
                    break;
                case "EliseSPiderEInitial":
                    LastE2T = Environment.TickCount;
                    break;
                default:
                    break;
            }
        }
        public static bool QHumanReady
        {
            get
            {
                return Player.Mana >= new int[] { 80, 85, 90, 95, 100 }[BadaoMainVariables.Q.Instance.Level - 1]
                    && Environment.TickCount - LastQT >= 1000f * 6f * (1f + Player.PercentCooldownMod);
            }
        }
        public static bool WHumanReady
        {
            get
            {
                return Player.Mana >= new int[] { 60, 70, 80, 90, 100 }[BadaoMainVariables.W.Instance.Level - 1]
                    && Environment.TickCount - LastWT >= 1000 * 12 * (1 + Player.PercentCooldownMod);
            }
        }
        public static bool EHumanReady
        {
            get
            {
                return Player.Mana >= new int[] { 50, 50, 50, 50, 50 }[BadaoMainVariables.E.Instance.Level - 1]
                    && Environment.TickCount - LastET >= 1000 *
                    new int[] { 14, 13, 12, 11, 10 }[BadaoMainVariables.E.Instance.Level - 1] * (1 + Player.PercentCooldownMod);
            }
        }
        public static bool QSpiderReady
        {
            get
            {
                return 
                    Environment.TickCount - LastQ2T >= 1000 * 6 * (1 + Player.PercentCooldownMod);
            }
        }
        public static bool WSpiderReady
        {
            get
            {
                return
                    Environment.TickCount - LastW2T >= 1000 * 12 * (1 + Player.PercentCooldownMod);
            }
        }
        public static bool ESpiderReady
        {
            get
            {
                return
                    Environment.TickCount - LastE2T >= 1000 *
                    new int[] { 26, 23, 20, 17, 14 }[BadaoMainVariables.E.Instance.Level - 1] * (1 + Player.PercentCooldownMod);
            }
        }
        public static bool IsHuman
        {
            get
            {
                return
                    BadaoMainVariables.R.Instance.Name == "EliseR";
            }
        }
    }
}
