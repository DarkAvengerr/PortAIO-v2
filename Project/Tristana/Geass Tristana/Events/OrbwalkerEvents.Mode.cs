using LeagueSharp;
using LeagueSharp.Common;
using System.Linq;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Geass_Tristana.Events
{
    internal partial class OrbwalkerEvents
    {
        private void Combo()
        {
            GeassLib.Globals.Variables.InCombo = true;
            if (!SMenu.Item(MenuNameBase + "Combo.Boolean.UseQ").GetValue<bool>() &&
                !SMenu.Item(MenuNameBase + "Combo.Boolean.UseE").GetValue<bool>() &&
                !SMenu.Item(MenuNameBase + "Combo.Boolean.UseR").GetValue<bool>()) return;

            if (SMenu.Item(MenuNameBase + "Combo.Boolean.UseE").GetValue<bool>() && Champion.GetSpellE.IsReady() && ComboUseE())
            {
                foreach (var enemy in (ObjectManager.Get<AIHeroClient>().Where(e => e.IsValidTarget(Champion.GetSpellE.Range)).OrderBy(hp => hp.Health)))
                {
                    if (!SMenu.Item(MenuNameBase + "Combo.Boolean.UseE.On." + enemy.ChampionName).GetValue<bool>()) continue;
                     Logger.WriteLog($"Combo Use E on {enemy}");
                    Champion.GetSpellE.Cast(enemy);
                    break;
                }
            }

            if (SMenu.Item(MenuNameBase + "Combo.Boolean.UseQ").GetValue<bool>() && Champion.GetSpellQ.IsReady() && ComboUseQ())
            {
                foreach (var enemy in (ObjectManager.Get<AIHeroClient>().Where(e => e.IsValidTarget(Champion.GetSpellQ.Range)).OrderBy(hp => hp.Health)))
                {
                     Logger.WriteLog($"Combo Use Q on {enemy}");
                    Champion.GetSpellQ.Cast();
                    CommonOrbwalker.ForceTarget(enemy);
                    break;
                }
            }

            if (SMenu.Item(MenuNameBase + "Combo.Boolean.FocusETarget").GetValue<bool>())
            {
                foreach (var enemy in (ObjectManager.Get<AIHeroClient>().Where(e => e.IsValidTarget(Champion.GetSpellQ.Range) && e.HasBuff("TristanaECharge")).OrderBy(hp => hp.Health)))
                {
                     Logger.WriteLog($"Force target {enemy}");
                    CommonOrbwalker.ForceTarget(enemy);
                    break;
                }
            }

            if (SMenu.Item(MenuNameBase + "Combo.Boolean.UseR").GetValue<bool>() && Champion.GetSpellR.IsReady() && ComboUseR())
            {
                foreach (var enemy in (ObjectManager.Get<AIHeroClient>().Where(e => e.IsValidTarget(Champion.GetSpellR.Range)).OrderBy(hp => hp.Health)))
                {
                    if (!SMenu.Item(MenuNameBase + "Combo.Boolean.UseR.On." + enemy.ChampionName).GetValue<bool>()) continue;
                    if (_damageLib.CalcDamage(enemy) < enemy.Health) continue;
                     Logger.WriteLog($"Combo Use R on {enemy}");
                    Champion.GetSpellR.Cast(enemy);
                    return;
                }
            }
        }

        private Result JungleClear()
        {
            if (!SMenu.Item(MenuNameBase + "Clear.Boolean.UseQ.Monsters").GetValue<bool>() &&
                !SMenu.Item(MenuNameBase + "Clear.Boolean.UseE.Monsters").GetValue<bool>()) return Result.Invalid;

            var validMonsters = MinionManager.GetMinions(Champion.GetSpellQ.Range, MinionTypes.All, MinionTeam.Neutral);

            if (validMonsters.Count <= 0) return Result.Failure;

            foreach (var monster in validMonsters.Where(name => !name.Name.ToLower().Contains("mini") && !name.BaseSkinName.ToLower().Contains("mini") && name.IsValidTarget(Champion.GetSpellE.Range)).OrderBy(hp => hp.Health))
            {
                if (SMenu.Item(MenuNameBase + "Clear.Boolean.UseE.Monsters").GetValue<bool>() && ClearUseE())
                {
                     Logger.WriteLog($"Jungle Use E on {monster.Name}");
                    Champion.GetSpellE.Cast(monster);
                    CommonOrbwalker.ForceTarget(monster);
                }
                if (SMenu.Item(MenuNameBase + "Clear.Boolean.UseQ.Monsters").GetValue<bool>() && ClearUseQ())
                {
                     Logger.WriteLog($"Jungle Use Q on {monster.Name}");
                    Champion.GetSpellQ.Cast();
                    CommonOrbwalker.ForceTarget(monster);
                    return Result.Success;
                }
            }

            return Result.Success;
        }

        private void LaneClear()
        {
            GeassLib.Globals.Variables.InCombo = false;
            if (!Champion.GetSpellE.IsReady() && !Champion.GetSpellQ.IsReady()) return;

            if (TurretClear() == Result.Success) { }
            else if (JungleClear() == Result.Success) { }
            else LaneClearE();
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private Result LaneClearE()
        {
            if (!SMenu.Item(MenuNameBase + "Clear.Boolean.UseE.Minons").GetValue<bool>()
                && !SMenu.Item(MenuNameBase + "Clear.Boolean.UseQ.Minons").GetValue<bool>()) return Result.Invalid;

            var validMinons = MinionManager.GetMinions(Champion.Player.Position, Champion.GetSpellQ.Range - 50, MinionTypes.All, MinionTeam.NotAlly);
            if (validMinons.Count < SMenu.Item(MenuNameBase + "Clear.Minons.Slider.MinMinons").GetValue<Slider>().Value) return Result.Failure;

            if (Champion.GetSpellE.IsReady() && SMenu.Item(MenuNameBase + "Clear.Boolean.UseE.Minons").GetValue<bool>() && ClearUseE())
            {
                Obj_AI_Base target = null;
                var bestInRange = 0;
                foreach (var minon in validMinons.Where(minon => minon.IsValidTarget(Champion.GetSpellE.Range)))
                {
                    var inRange = 1 + validMinons.Count(minon2 => minon.Distance(minon) < 125);
                    if (inRange <= bestInRange) continue;
                    bestInRange = inRange;
                    target = minon;
                }
                if (target != null && bestInRange >= SMenu.Item(MenuNameBase + "Clear.Minons.Slider.MinMinons").GetValue<Slider>().Value)
                {
                     Logger.WriteLog($"Laneclear Use E on {target.Name} in range {bestInRange}");
                    Champion.GetSpellE.Cast(target);
                    CommonOrbwalker.ForceTarget(target);
                }
            }

            if (SMenu.Item(MenuNameBase + "Clear.Boolean.UseQ.Minons").GetValue<bool>() && Champion.GetSpellQ.IsReady() && ClearUseQ())
            {
                foreach (
                    var minion in
                        validMinons.Where(
                            charge =>
                                charge.HasBuff("TristanaECharge") && 
                                charge.IsValidTarget(Champion.GetSpellQ.Range)).OrderBy(hp => hp.Health))
                {
                     Logger.WriteLog($"Force target {minion.Name}");
                    Champion.GetSpellQ.Cast();
                    CommonOrbwalker.ForceTarget(minion);
                    return Result.Success;
                }
            }

            return Result.Success;
        }

        private void LastHit()
        {
            GeassLib.Globals.Variables.InCombo = false;
        }

        private void Mixed()
        {
            GeassLib.Globals.Variables.InCombo = false;
            var minValue = SMenu.Item(MenuNameBase + "Mixed.Slider.MaxDistance").GetValue<Slider>().Value;
            if (SMenu.Item(MenuNameBase + "Mixed.Boolean.UseE").GetValue<bool>() && Champion.GetSpellE.IsReady() && MixedUseE())
            {

                foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(e => e.IsValidTarget(Champion.GetSpellE.Range - minValue)).OrderBy(hp => hp.HealthPercent))
                {
                    if (!SMenu.Item(MenuNameBase + "Mixed.Boolean.UseE.On." + enemy.ChampionName).GetValue<bool>()) continue;

                     Logger.WriteLog($"Mixed Use E on {enemy.Name}");
                    Champion.GetSpellE.Cast(enemy);
                    CommonOrbwalker.ForceTarget(enemy);

                    if (SMenu.Item(MenuNameBase + "Mixed.Boolean.UseQ").GetValue<bool>() && MixedUseQ())
                    {
                        if (Champion.GetSpellQ.IsReady())
                        {
                             Logger.WriteLog($"Mixed Use Q on {enemy.Name}");
                            Champion.GetSpellQ.Cast();

}
                    }
                    return;
                }
            }
            else if (SMenu.Item(MenuNameBase + "Mixed.Boolean.UseQ").GetValue<bool>() && Champion.GetSpellQ.IsReady() && MixedUseQ())
            {
                foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(e => e.IsValidTarget(Champion.GetSpellQ.Range - minValue)).OrderBy(hp => hp.Health))
                {
                     Logger.WriteLog($"Mixed Use Q on {enemy.Name}");
                    Champion.GetSpellQ.Cast();
                    CommonOrbwalker.ForceTarget(enemy);
                    return;
                }
            }

            //if (SMenu.Item(MenuNameBase + "Mixed.Boolean.UseR").GetValue<bool>() && Champion.GetSpellR.IsReady() && MixedUseR())
            //{
            //    foreach (var enemy in ObjectManager.Get<AIHeroClient>().OrderBy(hp => hp.Health))
            //    {
            //        if (enemy.IsDead) continue;
            //        if (!enemy.IsEnemy) continue;
            //        if (!SMenu.Item(MenuNameBase + "Mixed.Boolean.UseR.On." + enemy.ChampionName).GetValue<bool>()) continue;
            //        if (!enemy.IsValidTarget(Champion.GetSpellR.Range)) continue;
            //        if (_damageLib.CalculateDamage(enemy) < enemy.Health) continue;
            //        Champion.GetSpellR.Cast(enemy);
            //        break;
            //    }
            //}
        }

        private Result TurretClear()
        {
            if (!SMenu.Item(MenuNameBase + "Clear.Boolean.UseQ.Turret").GetValue<bool>() &&
                !SMenu.Item(MenuNameBase + "Clear.Boolean.UseE.Turret").GetValue<bool>()) return Result.Invalid;

            var validTurets = ObjectManager.Get<Obj_AI_Turret>().OrderBy(dis => dis.ServerPosition.Distance(Champion.Player.ServerPosition));
            
            var target = validTurets.Where(turret => turret.IsEnemy).Where(turret => !turret.IsDead).FirstOrDefault(turret => turret.IsValidTarget(Champion.GetSpellQ.Range));
            
            if (target == null)
            {
               //  Logger.WriteLog($"No vlaid turret");
                return Result.Failure;
            }

            if (SMenu.Item(MenuNameBase + "Clear.Boolean.UseE.Turret").GetValue<bool>() && ClearUseE())
            {
                 Logger.WriteLog($"Turret Clear Use E on {target.Name}");
                Champion.GetSpellE.Cast(target);
                CommonOrbwalker.ForceTarget(target);
            }

            if (SMenu.Item(MenuNameBase + "Clear.Boolean.UseQ.Turret").GetValue<bool>() && ClearUseQ())
            {
                 Logger.WriteLog($"Turret Clear Use Q on {target.Name}");
                Champion.GetSpellQ.Cast();
                CommonOrbwalker.ForceTarget(target);
            }

            return Result.Success;
        }
    }

    public enum Result
    {
        Success,
        Failure,
        Invalid
    }
}