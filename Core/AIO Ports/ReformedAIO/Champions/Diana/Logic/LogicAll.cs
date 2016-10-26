namespace ReformedAIO.Champions.Diana.Logic
{
    #region Using Directives

    using EloBuddy;
    using LeagueSharp.Common;

    #endregion

    internal class LogicAll
    {
        #region Public Methods and Operators

        public float ComboDmg(Obj_AI_Base x)
        {
            var dmg = 0f;

            if (Variables.Spells[SpellSlot.Q].IsReady()) dmg = dmg + Variables.Spells[SpellSlot.Q].GetDamage(x);

            if (Variables.Spells[SpellSlot.W].IsReady()) dmg = dmg + Variables.Spells[SpellSlot.W].GetDamage(x);

            if (Variables.Spells[SpellSlot.E].IsReady()) dmg = dmg + Variables.Spells[SpellSlot.E].GetDamage(x);

            if (Variables.Spells[SpellSlot.R].IsReady()) dmg = dmg + Variables.Spells[SpellSlot.R].GetDamage(x);

            if (Variables.Player.HasBuff("SummonerExhaust")) dmg = dmg * 0.6f;

            return dmg;
        }

        #endregion
    }
}