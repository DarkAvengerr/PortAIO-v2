using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Gragas.Logic
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

            if (Variable.Spells[SpellSlot.Q].IsReady()) dmg = dmg + Variable.Spells[SpellSlot.Q].GetDamage(x);

            if (Variable.Spells[SpellSlot.W].IsReady()) dmg = dmg + Variable.Spells[SpellSlot.W].GetDamage(x) + (float)Variable.Player.GetAutoAttackDamage(x);

            if (Variable.Spells[SpellSlot.E].IsReady()) dmg = dmg + Variable.Spells[SpellSlot.E].GetDamage(x);

            if (Variable.Spells[SpellSlot.R].IsReady()) dmg = dmg + Variable.Spells[SpellSlot.R].GetDamage(x);

            if (Variable.Player.HasBuff("SummonerExhaust")) dmg = dmg * 0.6f;

            return dmg;
        }

        #endregion
    }
}