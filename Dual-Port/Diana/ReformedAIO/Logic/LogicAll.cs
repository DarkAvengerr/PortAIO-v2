using EloBuddy; namespace ReformedAIO.Champions.Diana.Logic
{
    #region Using Directives

    using LeagueSharp;
    using LeagueSharp.Common;

    #endregion

    internal class LogicAll
    {
        #region Public Methods and Operators

        public float ComboDmg(Obj_AI_Base x)
        {
            var dmg = 0f;

            if (Variables.Spells[SpellSlot.Q].LSIsReady()) dmg = dmg + Variables.Spells[SpellSlot.Q].GetDamage(x);

            if (Variables.Spells[SpellSlot.W].LSIsReady()) dmg = dmg + Variables.Spells[SpellSlot.W].GetDamage(x);

            if (Variables.Spells[SpellSlot.E].LSIsReady()) dmg = dmg + Variables.Spells[SpellSlot.E].GetDamage(x);

            if (Variables.Spells[SpellSlot.R].LSIsReady()) dmg = dmg + Variables.Spells[SpellSlot.R].GetDamage(x);

            if (Variables.Player.LSHasBuff("SummonerExhaust")) dmg = dmg * 0.6f;

            return dmg;
        }

        #endregion
    }
}