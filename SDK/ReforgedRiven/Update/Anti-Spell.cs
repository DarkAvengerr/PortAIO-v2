using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using Reforged_Riven.Extras;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Reforged_Riven.Update
{
    internal class AntiSpell : Core
    {
        public static void OnCasting(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsEnemy || sender.Type != Player.Type) return;

            var epos = Player.ServerPosition + (Player.ServerPosition - sender.ServerPosition).Normalized()*300;

            if (!(Player.Distance(sender.ServerPosition) <= args.SData.CastRange)) return;

            if (Spells.E.IsReady())
            {
                if (Logic.EAntiSpell.Contains(args.SData.Name) || (Logic.TargetedAntiSpell.Contains(args.SData.Name) && args.Target.IsMe))
                {
                    Spells.E.Cast(epos);
                }
            }

            if (!Logic.WAntiSpell.Contains(args.SData.Name) || !Spells.W.IsReady()) return;

               Spells.W.Cast();
        }
    }
}