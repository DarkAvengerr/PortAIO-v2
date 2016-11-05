using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace DZAIO_Reborn.Plugins.Champions.Orianna.BallManager
{
    public class CommandQueue
    {
        private static int GameTickCount => (int)(Game.Time * 1000);
        private static int lastSendTime = 0;
        public static bool CanRun => !IsRunning;

        public static bool IsRunning
        {
            get
            {
                IsRunning = lastSendTime > 0 && lastSendTime + Game.Ping + 200 - GameTickCount > 0 || IsPlayerBusy();
                return lastSendTime > 0 && lastSendTime + Game.Ping + 200 - GameTickCount > 0 || IsPlayerBusy();
            }
            private set
            {
                if (!value)
                {
                    lastSendTime = 0;
                }
            }
        }

        public static void InitEvents()
        {
            Obj_AI_Base.OnSpellCast += OnProcessSCast;
            Spellbook.OnStopCast += OnStopCast;
            Spellbook.OnCastSpell += OnCast;
        }
        
        private static void OnProcessSCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && !args.SData.IsAutoAttack())
            {
                IsRunning = false;
            }
        }

        private static void OnStopCast(Obj_AI_Base sender, SpellbookStopCastEventArgs args)
        {
            if (sender.IsMe)
            {
                IsRunning = false;
            }
        }

        private static void OnCast(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (sender.Owner.IsMe)
            {
                switch (args.Slot)
                {
                    case SpellSlot.Q:
                    case SpellSlot.W:
                    case SpellSlot.E:
                    case SpellSlot.R:
                        if (CanRun)
                        {
                            lastSendTime = GameTickCount;
                        }
                        else
                        {
                            args.Process = false;
                        }
                        break;
                }
            }
        }

        private static bool IsPlayerBusy()
        {
            return ObjectManager.Player.Spellbook.IsCastingSpell || ObjectManager.Player.Spellbook.IsChanneling ||
                   ObjectManager.Player.Spellbook.IsCharging;
        }

    }
}
