using EloBuddy; namespace ReformedAIO.Champions.Ashe.Logic
{
    #region Using Directives

    using LeagueSharp;

    #endregion

    internal class QLogic
    {
        #region Public Methods and Operators

        public void Kite(Obj_AI_Base x)
        {
            if (x == null || x.HasBuffOfType(BuffType.PhysicalImmunity)) return;

            Variable.Orbwalker.ForceTarget(x);
        }

        public int QCount()
        {
            return Variable.Player.GetBuffCount("AsheQ");
        }

        #endregion
    }
}