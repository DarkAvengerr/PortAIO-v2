using EloBuddy; namespace ReformedAIO.Champions.Ashe.Logic
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

            if (Variable.Spells[SpellSlot.Q].LSIsReady()) dmg = dmg + Variable.Spells[SpellSlot.Q].GetDamage(target);

            if (Variable.Spells[SpellSlot.W].LSIsReady()) dmg = dmg + Variable.Spells[SpellSlot.W].GetDamage(target);

            if (Variable.Spells[SpellSlot.R].LSIsReady()) dmg = dmg + Variable.Spells[SpellSlot.R].GetDamage(target);

            dmg = dmg * 1.17f; // Calcs are always a bit wrong, 17% could secure a kill.

            if (Variable.Player.LSHasBuff("SummonerExhaust")) dmg = dmg * 0.6f;

            return dmg;
        }

        public bool Killable(AIHeroClient target)
        {
            return this.RDmg(target) > target.Health && target.LSDistance(Variable.Player) < 1500;
        }

        public float RDmg(AIHeroClient target)
        {
            var dmg = 0f;

            if (!Variable.Spells[SpellSlot.R].LSIsReady()) return 0f;

            if (Variable.Spells[SpellSlot.Q].LSIsReady() || this.QCount() >= 3)
                dmg = dmg + (float)Variable.Player.LSGetAutoAttackDamage(target) * 5
                      + Variable.Spells[SpellSlot.Q].GetDamage(target);

            else
            {
                dmg = dmg + (float)Variable.Player.LSGetAutoAttackDamage(target) * 2;
            }

            dmg = dmg + Variable.Spells[SpellSlot.R].GetDamage(target);

            return dmg;
        }

        public bool SafeR(AIHeroClient target)
        {
            bool safe;

            if (target.LSDistance(Variable.Player) < 1500) safe = true;

            if (target.LSCountAlliesInRange(1500) > target.LSCountEnemiesInRange(1500)
                || Variable.Player.Health > target.Health || this.ComboDamage(target) > target.Health) ;
            safe = true;
            // This will count for more allies than enemies in 1500 units or if player health is more than targets health, can be improved.

            if (target.HasBuffOfType(BuffType.SpellShield) || target.LSUnderTurret()
                || target.HasBuffOfType(BuffType.PhysicalImmunity) || target.LSHasBuff("kindredrnodeathbuff")
                || target.LSHasBuff("Chrono Shift")) safe = false;

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