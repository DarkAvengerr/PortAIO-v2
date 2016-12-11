using EloBuddy; 
using LeagueSharp.Common; 
namespace NechritoRiven.Event.Interrupters_Etc
{
    #region

    using LeagueSharp;
    using LeagueSharp.Common;

    using NechritoRiven.Core;

    #endregion

    internal class ProcessSpell : Core
    {
        #region Public Methods and Operators

        public static void OnProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsEnemy || !sender.IsValidTarget(1000))
            {
                return;
            }

            if (Spells.E.IsReady())
            {
                if (BackgroundData.AntigapclosingSpells.Contains(args.SData.Name) || (BackgroundData.TargetedSpells.Contains(args.SData.Name) && args.Target.IsMe))
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(120, ()=> Spells.E.Cast(Game.CursorPos));
                }
            }

            if (!BackgroundData.InterrupterSpell.Contains(args.SData.Name) || !Spells.W.IsReady() || !BackgroundData.InRange(sender))
            {
                return;
            }

            BackgroundData.CastW(sender);
        }

        #endregion
    }
}