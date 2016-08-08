using EloBuddy; namespace ReformedAIO.Champions.Ryze.Logic
{
    #region Using Directives

    using LeagueSharp;

    using SharpDX;

    #endregion

    internal class QLogic
    {
        #region Public Methods and Operators

        public Vector3 QPred(Obj_AI_Base x)
        {
            var pos = Variable.Spells[SpellSlot.Q].GetPrediction(x);

            return pos.CastPosition;
        }

        #endregion
    }
}