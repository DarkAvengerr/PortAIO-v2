#region

using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using Reforged_Riven.Extras;
using Reforged_Riven.Menu;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Reforged_Riven.Update
{
    internal class KillSteal : Core
    {
        public static void Update(EventArgs args)
        {
            var target = Variables.TargetSelector.GetTarget(Spells.R.Range, DamageType.Physical);

            if (target == null) return;

            if (target.HasBuff("kindrednodeathbuff") 
                || target.HasBuff("Undying Rage") 
                || target.HasBuff("JudicatorIntervention"))
            {
                return;
            }

            //if (Spells.Q.IsReady())
            //{
            //    if (target.Health < Spells.Q.GetDamage(target) && Logic.InQRange(target))
            //    {
            //        Spells.Q.Cast(target);
            //    }
            //}

            if (Spells.W.IsReady())
            {
                if (target.Health < Spells.W.GetDamage(target) && Logic.InWRange(target))
                {
                    Spells.W.Cast();
                }
            }

            if (Spells.R.IsReady() && Spells.R.Instance.Name == IsSecondR)
            {
                if (target.Health < Dmg.RDmg(target))
                {
                    var pred = Spells.R.GetPrediction(target);
                    if (pred.Hitchance > HitChance.High)
                    {
                        if (Spells.E.IsReady())
                        {
                            Spells.E.Cast(target.ServerPosition);
                        }
                        Spells.R.Cast(pred.CastPosition);
                    }
                }
            }

            if (!Spells.Ignite.IsReady() || !MenuConfig.Ignite) return;

            foreach (var x in GameObjects.EnemyHeroes.Where(t => t.IsValidTarget(600f) && t.Health < Dmg.IgniteDmg))
            {
                GameObjects.Player.Spellbook.CastSpell(Spells.Ignite, x);
            }
        }
    }
}
