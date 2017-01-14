using EloBuddy; 
using LeagueSharp.Common; 
namespace MoonRiven
{
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Riven : Logic
    {
        internal static void LoadAssembly()
        {
            SpellInit.Init();
            MenuInit.Init();
            OnUpdateEvents.Init();
            AfterAttackEvents.Init();
            SpellCastEvents.Init();
            OnSpellCastEvents.Init();
            GapcloserEvents.Init();
            InterruptEvents.Init();
            OnDrawEvents.Init();
        }

        internal static bool UseItem()
        {
            if (Items.HasItem(3077) && Items.CanUseItem(3077))
            {
                return Items.UseItem(3077);
            }

            if (Items.HasItem(3074) && Items.CanUseItem(3074))
            {
                return Items.UseItem(3074);
            }

            if (Items.HasItem(3053) && Items.CanUseItem(3053))
            {
                return Items.UseItem(3053);
            }

            return false;
        }

        internal static bool HaveShield(AIHeroClient target)
        {
            if (target.HasBuff("SivirE")) // Sivir E
            {
                return true;
            }

            if (target.HasBuff("BlackShield")) // Morgana E
            {
                return true;
            }

            if (target.HasBuff("NocturneShit")) // Noc E
            {
                return true;
            }

            //if (target.HasBuffOfType(BuffType.SpellShield)) // Others Shield??? maybe i will add it
            //{
            //    return true;
            //}

            return false;
        }

        internal static bool CastQ(Obj_AI_Base target)
        {
            if (target == null || target.IsDead || !target.IsValidTarget() || Q.Level == 0 || !Q.IsReady())
            {
                return false;
            }

            switch (MenuInit.QMode)
            {
                case 0:
                    return Q.Cast(target.Position, true);
                case 1:
                    return  Q.Cast(Game.CursorPos, true);
                default:
                    return false;
            }
        }

        internal static bool R1Logic(AIHeroClient target)
        {
            if (!target.IsValidTarget() || R.Instance.Name != "RivenFengShuiEngine" || !MenuInit.ComboR)
            {
                return false;
            }

            return R.Cast(true);
        }

        internal static bool R2Logic(AIHeroClient target)
        {
            if (!target.IsValidTarget() || R.Instance.Name == "RivenFengShuiEngine" || MenuInit.ComboR2 == 3)
            {
                return false;
            }

            switch (MenuInit.ComboR2)
            {
                case 0:
                    if (target.HealthPercent < 20 ||
                        (target.Health > DamageCalculate.GetRDamage(target) + Me.GetAutoAttackDamage(target) * 2 &&
                         target.HealthPercent < 40) ||
                        (target.Health <= DamageCalculate.GetRDamage(target)) ||
                        (target.Health <= DamageCalculate.GetComboDamage(target) * 1.3))
                    {
                        var pred = R.GetPrediction(target, true);

                        if (pred.Hitchance >= HitChance.VeryHigh)
                        {
                            return R.Cast(pred.CastPosition, true);
                        }
                    }
                    break;
                case 1:
                    if (DamageCalculate.GetRDamage(target) > target.Health && target.DistanceToPlayer() < 600)
                    {
                        var pred = R.GetPrediction(target, true);

                        if (pred.Hitchance >= HitChance.VeryHigh)
                        {
                            return R.Cast(pred.CastPosition, true);
                        }
                    }
                    break;
                case 2:
                    if (target.DistanceToPlayer() < 600)
                    {
                        var pred = R.GetPrediction(target, true);

                        if (pred.Hitchance >= HitChance.VeryHigh)
                        {
                            return R.Cast(pred.CastPosition, true);
                        }
                    }
                    break;
            }

            return false;
        }
    }
}