using EloBuddy; namespace ReformedAIO.Champions.Ryze.Logic
{
    #region Using Directives

    using LeagueSharp;

    #endregion

    internal class ELogic
    {
        #region Public Methods and Operators

        public bool RyzeE(Obj_AI_Base x)
        {
            return x.HasBuff("RyzeE");
        }

        public bool StageFirst()
        {
            return Variable.Player.HasBuff("ryzeqiconnocharge");
        }

        public bool StageFull()
        {
            return Variable.Player.HasBuff("ryzeqiconfullcharge");
        }

        public bool StageHalf()
        {
            return Variable.Player.HasBuff("ryzeqiconhalfcharge");
        }

        #endregion
    }
}