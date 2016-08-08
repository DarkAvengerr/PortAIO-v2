using EloBuddy; namespace ReformedAIO.Champions.Diana.Logic
{
    #region Using Directives

    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    #endregion

    internal class PaleCascadeLogic
    {
        #region Fields

        protected AIHeroClient Target;

        #endregion

        #region Public Methods and Operators

        public bool Buff(Obj_AI_Base x)
        {
            return x.Buffs.Any(a => a.Name.ToLower().Contains("dianamoonlight"));
        }

        public float GetDmg(Obj_AI_Base x)
        {
            if (x == null)
            {
                return 0;
            }

            var dmg = 0f;

            if (Variables.Player.LSHasBuff("SummonerExhaust")) dmg = dmg * 0.6f;

            if (Variables.Spells[SpellSlot.Q].LSIsReady()) dmg = dmg + Variables.Spells[SpellSlot.R].GetDamage(x);

            return dmg;
        }

        #endregion
    }
}