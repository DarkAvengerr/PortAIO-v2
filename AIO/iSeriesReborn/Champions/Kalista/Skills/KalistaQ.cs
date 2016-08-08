using System;
using iSeriesReborn.Utility;
using iSeriesReborn.Utility.MenuUtility;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.Kalista.Skills
{
    class KalistaQ
    {
        private static float LastCastTick = 0f;

        public static void ExecuteComboLogic()
        {
            var spells = Variables.CurrentChampion.GetSpells();

            if (spells[SpellSlot.Q].IsEnabledAndReady())
            {
                var TargetHero = TargetSelector.GetTarget(spells[SpellSlot.Q].Range * 0.75f, TargetSelector.DamageType.Physical);
                if (TargetHero.LSIsValidTarget(spells[SpellSlot.Q].Range * 0.75f))
                {
                    var dashEndPos = ObjectManager.Player.GetDashInfo().EndPos;
                    var QNormalPrediction = spells[SpellSlot.Q].GetPrediction(TargetHero);
                    if (dashEndPos != Vector2.Zero)
                    {
                        spells[SpellSlot.Q].UpdateSourcePosition(dashEndPos.To3D());
                        QNormalPrediction = spells[SpellSlot.Q].GetPrediction(TargetHero);
                        spells[SpellSlot.Q].UpdateSourcePosition(ObjectManager.Player.ServerPosition);
                    }

                    if (QNormalPrediction.Hitchance >= HitChance.High)
                    {
                        if (!ObjectManager.Player.LSIsDashing() && (!ObjectManager.Player.Spellbook.IsAutoAttacking || ObjectManager.Player.Spellbook.IsAutoAttacking) && (Environment.TickCount - LastCastTick > 500))
                        {
                            spells[SpellSlot.Q].Cast(QNormalPrediction.CastPosition);
                            LastCastTick = Environment.TickCount;
                        }
                    }else if (!TargetHero.LSIsValidTarget(spells[SpellSlot.E].Range)
                        && TargetHero.LSIsValidTarget(spells[SpellSlot.E].Range + 285f) 
                        && KalistaE.CanBeRendKilled(TargetHero))
                    {
                        if (!ObjectManager.Player.LSIsDashing() && (!ObjectManager.Player.Spellbook.IsAutoAttacking || ObjectManager.Player.Spellbook.IsAutoAttacking) && (Environment.TickCount - LastCastTick > 500))
                        {
                            spells[SpellSlot.Q].Cast(QNormalPrediction.CastPosition);
                            LastCastTick = Environment.TickCount;
                        }
                    }
                }
            }
        }
    }
}
