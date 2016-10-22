using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using TheKalista.Commons.ComboSystem;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace TheGaren
{
    class GarenR : Skill
    {
        public bool Killsteal;

        public GarenR(SpellSlot spell)
            : base(spell) { }

        public override void Execute(AIHeroClient target)
        {
            if (Killsteal)
            {
                foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(375)))
                {
                    if (IsKillable(enemy))
                        Cast(enemy);
                }
            }
            else if (IsKillable(target) && HealthPrediction.GetHealthPrediction(target, 1000) > ObjectManager.Player.GetAutoAttackDamage(target) && !Provider.ShouldBeDead(target))
            {
                Cast(target);
            }
        }

        public override int GetPriority()
        {
            return 4;
        }
    }
}
