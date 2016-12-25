using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
namespace e.Motion_Gangplank
{
    class DelayManager
    {
        private Spell _spellToUse;
        private int _expireTime;
        private int _lastuse;
        private Obj_AI_Base target;
        
        public DelayManager(Spell spell, int expireTicks)
        {
            _spellToUse = spell;
            _expireTime = expireTicks;
        }

        public void Delay(Obj_AI_Base enemy)
        {
            _lastuse = Utils.TickCount;
            target = enemy;
        }

        public bool Active()
        {
            return (target != null && _lastuse + _expireTime >= Utils.TickCount);
        }

        public void CheckEachTick()
        {
            if (target != null && _lastuse + _expireTime >= Utils.TickCount && _spellToUse.IsReady() && _spellToUse.Range >= Program.Player.Distance(target))
            {
                _spellToUse.Cast(target);
                target = null;
                //Chat.Print("Casted with DelayManager(TM)");
            }
        }
    }
}
