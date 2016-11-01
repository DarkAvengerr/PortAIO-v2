using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NechritoRiven.Event
{
    #region

    using Core;

    using LeagueSharp;
    using LeagueSharp.Common;

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
                if (EAntiSpell.Contains(args.SData.Name) || (TargetedAntiSpell.Contains(args.SData.Name) && args.Target.IsMe))
                {
                    Spells.E.Cast(Game.CursorPos);
                }
            }

            if (!WAntiSpell.Contains(args.SData.Name) || !Spells.W.IsReady() || !InRange(sender))
            {
                return;
            }

            CastW(sender);
        }

        #endregion
    }
}