using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Ashe.Logic
{
    #region Using Directives

    using LeagueSharp;
    using LeagueSharp.Common;

    #endregion

    internal class RLogic
    {
        #region Public Methods and Operators

        public float ComboDamage(AIHeroClient target)
        {
            var dmg = 0f;

            if (Variable.Spells[SpellSlot.Q].IsReady()) dmg = dmg + Variable.Spells[SpellSlot.Q].GetDamage(target);

            if (Variable.Spells[SpellSlot.W].IsReady()) dmg = dmg + Variable.Spells[SpellSlot.W].GetDamage(target);

            if (Variable.Spells[SpellSlot.R].IsReady()) dmg = dmg + Variable.Spells[SpellSlot.R].GetDamage(target);

            dmg = dmg * 1.17f; // Calcs are always a bit wrong, 17% could secure a kill.

            if (Variable.Player.HasBuff("SummonerExhaust")) dmg = dmg * 0.6f;

            return dmg;
        }

        public bool Killable(AIHeroClient target)
        {
            return RDmg(target) > target.Health && target.Distance(Variable.Player) < 1500;
        }

        public float RDmg(AIHeroClient target)
        {
            var dmg = 0f;

            if (!Variable.Spells[SpellSlot.R].IsReady()) return 0f;

            if (Variable.Spells[SpellSlot.Q].IsReady() || QCount() >= 3)
                dmg = dmg + (float)Variable.Player.GetAutoAttackDamage(target) * 5
                      + Variable.Spells[SpellSlot.Q].GetDamage(target);

            else
            {
                dmg = dmg + (float)Variable.Player.GetAutoAttackDamage(target) * 2;
            }

            dmg = dmg + Variable.Spells[SpellSlot.R].GetDamage(target);

            return dmg;
        }

        public bool SafeR(AIHeroClient target)
        {
            bool safe;

            if (target.Distance(Variable.Player) < 1500) safe = true;

            if (target.CountAlliesInRange(1500) > target.CountEnemiesInRange(1500)
                || Variable.Player.Health > target.Health || ComboDamage(target) > target.Health) ;
            safe = true;
            // This will count for more allies than enemies in 1500 units or if player health is more than targets health, can be improved.

            if (target.HasBuffOfType(BuffType.SpellShield) || target.UnderTurret()
                || target.HasBuffOfType(BuffType.PhysicalImmunity) || target.HasBuff("kindredrnodeathbuff")
                || target.HasBuff("Chrono Shift")) safe = false;

            return safe;
        }

        #endregion

        #region Methods

        private int QCount()
        {
            return Variable.Player.GetBuffCount("AsheQ");
        }

        #endregion
    }
}