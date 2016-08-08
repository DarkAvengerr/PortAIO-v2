using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhin___The_Virtuoso.Extensions;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Jhin___The_Virtuoso.Modes
{
    static class None
    {
        public static void ImmobileExecute()
        {
            if (Spells.E.LSIsReady() && Menus.Config.Item("auto.e.immobile").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(Spells.E.Range) && x.IsEnemyImmobile()))
                {
                    Spells.E.Cast(enemy);
                }
            }
        }

        public static void KillSteal()
        {
            if (Spells.Q.LSIsReady() && Menus.Config.Item("q.ks").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(Spells.Q.Range) &&
                    x.Health < Spells.Q.GetDamage(x)))
                {
                    Spells.Q.CastOnUnit(enemy);
                }
            }
            if (Spells.W.LSIsReady() && Menus.Config.Item("w.ks").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.LSDistance(ObjectManager.Player) < Menus.Config.Item("w.combo.max.distance").GetValue<Slider>().Value
                        && x.LSDistance(ObjectManager.Player) > Menus.Config.Item("w.combo.min.distance").GetValue<Slider>().Value
                        && x.IsValid && Spells.W.GetPrediction(x).Hitchance >= Menus.Config.HikiChance("w.hit.chance")
                        && x.Health < Spells.W.GetDamage(x) && !x.IsDead && !x.IsZombie && x.IsValid))
                {
                    Spells.W.Cast(enemy);
                }
            }
        }

        public static void TeleportE()
        {
            if (Spells.E.LSIsReady() && Menus.Config.Item("e.combo").GetValue<bool>() && Menus.Config.Item("e.combo.teleport").GetValue<bool>())
            {
                foreach (var obj in ObjectManager.Get<Obj_AI_Base>().Where(x => x.Team != ObjectManager.Player.Team && x.LSDistance(ObjectManager.Player) < Spells.E.Range
                    && x.HasBuff("teleport_target") && !x.IsDead && !x.IsZombie))
                {
                    Spells.E.Cast(obj);
                }
            }
        }
    }
}
