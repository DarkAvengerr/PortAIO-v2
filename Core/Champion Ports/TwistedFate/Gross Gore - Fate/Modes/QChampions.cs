using EloBuddy; 
using LeagueSharp.Common; 
 namespace GrossGoreTwistedFate.Modes
{
    using System;
    using System.Windows.Input;

    using LeagueSharp;
    using LeagueSharp.Common;
    using System.Linq;

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
            var qTarget = TargetSelector.GetTarget(Spells.Q.Range, TargetSelector.DamageType.Magical);

            if (Config.IsKeyPressed("qEnemy")
                    && (!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.LeftCtrl)
                        && !Keyboard.IsKeyDown(Key.LeftAlt)) && ObjectManager.Player.Mana >= qMana && Spells.Q.IsReady())
            {
                CastQTick = Utils.TickCount;
            }

            if (Utils.TickCount - CastQTick < 500)
            {
                if (qTarget.IsValidTarget(Spells.Q.Range))
                {
                    var qPred = Spells.Q.GetPrediction(qTarget);

                    if (qPred.Hitchance >= HitChance.High)
                    {
                        Spells.Q.Cast(qPred.CastPosition);
                    }
                }
            }
        }

        #endregion
    }
}