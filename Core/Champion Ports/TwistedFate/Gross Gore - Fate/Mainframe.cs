#region Use
using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX; 
#endregion

using EloBuddy; 
using LeagueSharp.Common; 
 namespace GrossGoreTwistedFate
{
    using GrossGoreTwistedFate.Modes;

    internal static class Mainframe
    {
        #region Properties

        internal static SebbyLib.Orbwalking.Orbwalker Orbwalker { get; set; }

        #endregion

        #region Methods

        internal static void Init()
        {
            Game.OnUpdate += OnUpdate;

            Obj_AI_Base.OnProcessSpellCast += Computed.OnProcessSpellCast;
            Obj_AI_Base.OnProcessSpellCast += Computed.YellowIntoQ;
            Obj_AI_Base.OnProcessSpellCast += Computed.RedIntoQ;
            SebbyLib.Orbwalking.BeforeAttack += Computed.OnBeforeAttack;
            AntiGapcloser.OnEnemyGapcloser += Computed.Gapcloser_OnGapCloser;
            Interrupter2.OnInterruptableTarget += Computed.InterruptableSpell_OnInterruptableTarget;
        }

        private static void OnUpdate(EventArgs args)
        {
            if (!ObjectManager.Player.IsDead)
            {
                switch(Orbwalker.ActiveMode)
                {
                    case SebbyLib.Orbwalking.OrbwalkingMode.Combo:
                    {
                            ComboMode.Execute();
                            break;
                    }
                    case SebbyLib.Orbwalking.OrbwalkingMode.Mixed:
                    {
                            MixedMode.Execute();
                            break;
                    }
                    case SebbyLib.Orbwalking.OrbwalkingMode.LaneClear:
                    {
                            Clear.Execute();
                            break;
                    }
                }

                ManualCards.Execute(); Automated.Execute(); QWaveClear.Execute(); QChampions.Execute();
            }
        }

        #endregion
    }
}