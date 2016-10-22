using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace myWorld.Library.SummonerManager
{
    class Summoner
    {
        SpellSlot slot;
        float range;
        public float Range
        {
            get
            {
                return range;
            }
        }

        public Summoner(string name, float range)
        {
            this.slot = ObjectManager.Player.GetSpellSlot(name);
            this.range = range;
        }

        public bool IsReady()
        {
            return slot != SpellSlot.Unknown && ObjectManager.Player.GetSpell(slot).State == SpellState.Ready;
        }

        public bool IsExist()
        {
            return slot != SpellSlot.Unknown;
        }

        public bool IsInRange(Obj_AI_Base target)
        {
            return ObjectManager.Player.Position.Distance(target.Position) <= range;
        }

        public void Cast()
        {
            ObjectManager.Player.Spellbook.CastSpell(slot);
        }

        public void Cast(Obj_AI_Base target)
        {
            ObjectManager.Player.Spellbook.CastSpell(slot, target);
        }

        public void Cast(Vector3 pos)
        {
            ObjectManager.Player.Spellbook.CastSpell(slot, pos);
        }
    }
}
