using EloBuddy; 
using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Ashe.Logic
{
    #region Using Directives

    using LeagueSharp;
    using LeagueSharp.Common;

    #endregion

    internal class QLogic
    {
        #region Public Methods and Operators
        private readonly Orbwalking.Orbwalker orbwalker;
        public void Kite(Obj_AI_Base x)
        {
            if (x == null || x.HasBuffOfType(BuffType.PhysicalImmunity)) return;

            this.orbwalker.ForceTarget(x);
        }

        public int QCount()
        {
            return Variable.Player.GetBuffCount("AsheQ");
        }

        #endregion
    }
}