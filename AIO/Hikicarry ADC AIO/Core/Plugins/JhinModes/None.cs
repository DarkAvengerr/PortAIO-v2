using System.Linq;
using HikiCarry.Champions;
using HikiCarry.Core.Predictions;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HikiCarry.Core.Plugins.JhinModes
{
    static class None
    {
        public static void ImmobileExecute()
        {
            if (Jhin.E.IsReady() && Initializer.Config.Item("auto.e.immobile",true).GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Jhin.E.Range) && Utilities.Utilities.IsImmobile(x)))
                {
                    Jhin.E.Do(enemy, Utilities.Utilities.HikiChance("hitchance"));
                }
            }
        }

        public static void KillSteal()
        {
            if (Jhin.Q.IsReady() && Initializer.Config.Item("q.ks",true).GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Jhin.Q.Range) &&
                    x.Health < Jhin.Q.GetDamage(x)))
                {
                    Jhin.Q.CastOnUnit(enemy);
                }
            }
            if (Jhin.W.IsReady() && Initializer.Config.Item("w.ks",true).GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.Distance(ObjectManager.Player) < Initializer.Config.Item("w.combo.max.distance",true).GetValue<Slider>().Value
                        && x.Distance(ObjectManager.Player) > Initializer.Config.Item("w.combo.min.distance",true).GetValue<Slider>().Value
                        && x.IsValid && x.Health < Jhin.W.GetDamage(x) && !x.IsDead && !x.IsZombie && x.IsValid))
                {
                    Jhin.W.Do(enemy,Utilities.Utilities.HikiChance("hitchance"));
                }
            }
        }

       
    }
}
