using System;
using LeagueSharp;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace GeassLib.Events
{
    public class OnLevel
    {
  
         readonly int[] _abilitySequences;
         private int _lastLevel;

        public OnLevel(int[] sequence)
        {
            if(!DelayHandler.Loaded)DelayHandler.Load();

            _abilitySequences = sequence;
            Game.OnUpdate += OnUpdate;
        }


        void OnUpdate(EventArgs args)
        {
            if (DelayHandler.CheckOnLevel())
            {
                if (!Globals.Objects.GeassLibMenu.Item(Menus.Names.LevelItemBase + "Boolean.AutoLevelUp").GetValue<bool>()) return;
                DelayHandler.UseOnLevel();

                if (_lastLevel != Globals.Objects.Player.Level)
                {
                    _lastLevel = Globals.Objects.Player.Level;
                    LevelUpSpells();
                }
            }
        }

        void LevelUpSpells()
        {
            var qL = Globals.Objects.Player.Spellbook.GetSpell(SpellSlot.Q).Level + _qOff;
            var wL = Globals.Objects.Player.Spellbook.GetSpell(SpellSlot.W).Level + _wOff;
            var eL = Globals.Objects.Player.Spellbook.GetSpell(SpellSlot.E).Level + _eOff;
            var rL = Globals.Objects.Player.Spellbook.GetSpell(SpellSlot.R).Level + _rOff;


            if (qL + wL + eL + rL >= Globals.Objects.Player.Level) return;

            int[] level = { 0, 0, 0, 0 };

            for (var i = 0; i < Globals.Objects.Player.Level; i++)
                level[_abilitySequences[i] - 1] = level[_abilitySequences[i] - 1] + 1;

            if (qL < level[0]) Globals.Objects.Player.Spellbook.LevelSpell(SpellSlot.Q);
            if (wL < level[1]) Globals.Objects.Player.Spellbook.LevelSpell(SpellSlot.W);
            if (eL < level[2]) Globals.Objects.Player.Spellbook.LevelSpell(SpellSlot.E);
            if (rL < level[3]) Globals.Objects.Player.Spellbook.LevelSpell(SpellSlot.R);
        }

#pragma warning disable RECS0122 // Initializing field with default value is redundant
        readonly int _qOff = 0;
        readonly int _wOff = 0;
        readonly int _eOff = 0;
        readonly int _rOff = 0;
#pragma warning restore RECS0122 // Initializing field with default value is redundant

    }
}
