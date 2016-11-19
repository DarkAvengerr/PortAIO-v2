using EloBuddy; 
using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Gragas.Logic
{
    #region Using Directives

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using SPrediction;

    #endregion

    internal class RLogic
    {
        #region Public Methods and Operators

        public Vector3 RPred(AIHeroClient target)
        {
            var pos =
                Variable.Spells[SpellSlot.R].GetVectorSPrediction(target, 1150)
                    .CastTargetPosition.Extend(Variable.Player.Position.To2D(), 65);

            return pos.To3D() + RDelay(target);
        }

        #endregion

        #region Methods

        private float RDelay(Obj_AI_Base target)
        {
            var time = target.Distance(Variable.Player) / Variable.Spells[SpellSlot.R].Speed;

            return time + Variable.Spells[SpellSlot.R].Delay;
        }

        #endregion
    }
}