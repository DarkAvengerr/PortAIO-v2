/*
 Copyright 2015 - 2015 ShineCommon
 Summoner.cs is part of ShineCommon
 
 ShineCommon is free software: you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation, either version 3 of the License, or
 (at your option) any later version.
 
 ShineCommon is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 GNU General Public License for more details.
 
 You should have received a copy of the GNU General Public License
 along with ShineCommon. If not, see <http://www.gnu.org/licenses/>.
*/

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
 namespace SSHCommon.Activator
{
    public abstract class Summoner
    {
        public Menu summoners;
        public SpellSlot Slot;
        public float Range;

        public Summoner()
        {
            Slot = SpellSlot.Unknown;
            Game.OnUpdate += Game_OnUpdate;
        }

        public virtual void Game_OnUpdate(EventArgs args)
        {
            //
        }

        public virtual void SetSlot()
        {
            //
        }

        public virtual int GetDamage()
        {
            //
            return 0;
        }

        public bool IsReady()
        {
            return ObjectManager.Player.Spellbook.CanUseSpell(Slot) == SpellState.Ready;
        }

        public void Cast()
        {
            if (Slot != SpellSlot.Unknown && ObjectManager.Player.Spellbook.CanUseSpell(Slot) == SpellState.Ready)
                ObjectManager.Player.Spellbook.CastSpell(Slot);
        }

        public void Cast(Obj_AI_Base t)
        {
            if (Slot != SpellSlot.Unknown && ObjectManager.Player.Spellbook.CanUseSpell(Slot) == SpellState.Ready && t.Distance(ObjectManager.Player.ServerPosition) < Range)
                ObjectManager.Player.Spellbook.CastSpell(Slot, t);
        }
    }
}
