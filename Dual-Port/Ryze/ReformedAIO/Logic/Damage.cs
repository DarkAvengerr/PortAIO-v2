using EloBuddy; namespace ReformedAIO.Champions.Ryze.Logic
{
    #region Using Directives

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Diana;

    #endregion

    internal class Damage
    {
        #region Public Methods and Operators

        public float ComboDmg(Obj_AI_Base x)
        {
            var dmg = 0f;

            if (Variable.Spells[SpellSlot.Q].LSIsReady()) dmg = dmg + Variables.Spells[SpellSlot.Q].GetDamage(x) * 2;

            if (Variable.Spells[SpellSlot.W].LSIsReady()) dmg = dmg + Variables.Spells[SpellSlot.W].GetDamage(x);

            if (Variable.Spells[SpellSlot.E].LSIsReady()) dmg = dmg + Variables.Spells[SpellSlot.E].GetDamage(x);

            if (Variable.Spells[SpellSlot.R].LSIsReady()) dmg = dmg + Variables.Spells[SpellSlot.R].GetDamage(x);

            if (Variable.Player.LSHasBuff("SummonerExhaust")) dmg = dmg * 0.6f;

            return dmg;
        }

        #endregion
    }
}