using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using SharpDX;
using Color = System.Drawing.Color;
using LeagueSharp.Common;


using EloBuddy; 
 using LeagueSharp.Common; 
 using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoActionsLimiter
{
    public static class MovementBlock
    {
        public static AIHeroClient Player { get { return ObjectManager.Player; } }
        public static int MovementBlockCount;
        public static int LastMovement;
        public static void BadaoActivate()
        {
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            EloBuddy.Player.OnIssueOrder += Obj_AI_Base_OnIssueOrder;
        }

        private static void Obj_AI_Base_OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (!sender.IsMe)
                return;
            if (args.Order != GameObjectOrder.MoveTo)
                return;
            if (Environment.TickCount - LastMovement < 100)
            {
                args.Process = false;
                MovementBlockCount += 1;
            }
            else
            {
                LastMovement = Environment.TickCount;
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
                return;
            if (!args.SData.IsAutoAttack())
                return;
            
            LastMovement = 0;
        }
    }
}
