#region

using LeagueSharp;
using LeagueSharp.Common;

#endregion

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NechritoRiven.Event
{
    using Core;

    internal class OnCasted : Core
    {
        public static void OnCasting(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsEnemy || sender.Type != Player.Type) return;

            var epos = Player.ServerPosition + (Player.ServerPosition - sender.ServerPosition).Normalized() * 300;

            if (!(Player.Distance(sender.ServerPosition) <= args.SData.CastRange)) return;

            if (Spells.E.IsReady())
            {
                if (EAntiSpell.Contains(args.SData.Name) || (TargetedAntiSpell.Contains(args.SData.Name) && args.Target.IsMe))
                {
                    Spells.E.Cast(epos);
                }
            }

            if (!WAntiSpell.Contains(args.SData.Name) || !Spells.W.IsReady()) return;

            Spells.W.Cast();
        }
    }
}
