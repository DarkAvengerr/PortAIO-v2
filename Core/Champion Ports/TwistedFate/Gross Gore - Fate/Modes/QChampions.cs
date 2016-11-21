#region Use
using System;
using System.Windows.Input;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common; 
#endregion

using EloBuddy; 
using LeagueSharp.Common; 
 namespace GrossGoreTwistedFate.Modes
{
    using Config = GrossGoreTwistedFate.Config;

    internal static class QChampions
    {

        #region Prop

        internal static int CastQTick;

        #endregion

        #region Methods

        internal static void Execute()
        {
            var qMana = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).SData.Mana;

            if (Config.UseQEnemy)
            {
                if(Spells._q.IsReadyPerfectly())
                {
                    if(ObjectManager.Player.Mana >= qMana)
                    {
                        CastQTick = Utils.TickCount;
                    }
                }
            }

            if (Utils.TickCount - CastQTick < 500)
            {
                var qTarget = TargetSelector.GetTarget(Spells._q.Range, Spells._q.DamageType);

                if (qTarget.IsValidTarget(Spells._q.Range))
                {
                    var qPred = Spells._q.GetPrediction(qTarget);

                    if (qPred.Hitchance >= Spells._q.MinHitChance)
                    {
                        Spells._q.Cast(qPred.CastPosition);
                    }
                }
            }
        }

        #endregion
    }
}