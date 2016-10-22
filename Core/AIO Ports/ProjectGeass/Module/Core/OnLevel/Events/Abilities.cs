using System;
using LeagueSharp;
using LeagueSharp.Common;
using _Project_Geass.Data.Champions;
using _Project_Geass.Functions;
using _Project_Geass.Humanizer.TickTock;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace _Project_Geass.Module.Core.OnLevel.Events
{

    internal class Abilities : SettingsBase
    {
        #region Public Constructors

        public Abilities(int[] sequence)
        {
            _abilitySequences=sequence;
            _rng=new Random();
            Obj_AI_Base.OnLevelUp+=Obj_AI_Base_OnLevelUp;
            Game.OnUpdate+=OnUpdate;
        }

        #endregion Public Constructors

        #region Public Methods

        public void OnUpdate(EventArgs args)
        {
            if (Handler.CheckOnLevel())
                if (StaticObjects.Player.Level==1)
                {
                    if (StaticObjects.ProjectMenu.Item(Names.Menu.LevelItemBase+"Boolean.AutoLevelUp").GetValue<bool>())
                        LevelUpSpells();

                    Game.OnUpdate-=OnUpdate;
                }
            Handler.UseOnLevel();
        }

        #endregion Public Methods

        #region Private Fields

        private readonly int[] _abilitySequences;
        private readonly Random _rng;
        private int _lastLevel;

        #endregion Private Fields

        #region Private Methods

        private void LevelUpSpells()
        {
            while (_lastLevel!=StaticObjects.Player.Level)
            {
                var ability=_abilitySequences[_lastLevel];
                if (ability.Equals(Q))
                    StaticObjects.Player.Spellbook.LevelUpSpell(SpellSlot.Q);
                else if (ability.Equals(W))
                    StaticObjects.Player.Spellbook.LevelUpSpell(SpellSlot.W);
                else if (ability.Equals(E))
                    StaticObjects.Player.Spellbook.LevelUpSpell(SpellSlot.E);
                else if (ability.Equals(R))
                    StaticObjects.Player.Spellbook.LevelUpSpell(SpellSlot.R);

                _lastLevel++;
            }
        }

        private void Obj_AI_Base_OnLevelUp(Obj_AI_Base sender, EventArgs args)
        {
            if (!sender.IsMe||!StaticObjects.ProjectMenu.Item(Names.Menu.LevelItemBase+"Boolean.AutoLevelUp").GetValue<bool>())
                return;

            LeagueSharp.Common.Utility.DelayAction.Add(_rng.Next(50, 200)-Game.Ping, LevelUpSpells);
        }

        #endregion Private Methods
    }

}