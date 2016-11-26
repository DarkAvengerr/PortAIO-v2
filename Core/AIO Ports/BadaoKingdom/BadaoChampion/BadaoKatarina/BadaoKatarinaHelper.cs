using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace BadaoKingdom.BadaoChampion.BadaoKatarina
{
    using static BadaoMainVariables;
    using static BadaoKatarinaVariables;
    public static class BadaoKatarinaHelper
    {
        public static bool IsDaggerFixed (Obj_AI_Base target)
        {
            return
                PickableDaggers.Any(x => x.Dagger.Position.Distance(target.Position) <= 450);
        }
        public static KatarinaDagger GetFixedDagger (Obj_AI_Base target)
        {
            KatarinaDagger FixedDagger = new KatarinaDagger {Dagger = null, CreationTime = 0 };
            var FixedDaggerA = PickableDaggers.Where(x => x.Dagger.Position.Distance(target.Position) <= 450).MinOrDefault(x => x.Dagger.Position.Distance(target.Position));
            if (FixedDaggerA != null)
                FixedDagger = FixedDaggerA;
            return FixedDagger;
        }
        public static void CastEFixedDagger(KatarinaDagger dagger,Obj_AI_Base target)
        {
            var distance = dagger.Dagger.Position.Distance(target.Position);
            Vector2 castpos = new Vector2();
            if (distance > 150)
                castpos = dagger.Dagger.Position.To2D().Extend(target.Position.To2D(), 150);
            else
                castpos = target.Position.To2D().Extend(dagger.Dagger.Position.To2D(), 10);
            E.Cast(castpos);
        }
        public static double GetPassiveDamage (Obj_AI_Base target)
        {
            int nhantu = Player.Level < 6 ? 0 :
                (Player.Level < 11 ? 1 :
                (Player.Level < 16 ? 2 : 3));
            var raw = new double[] { 75, 78, 83, 88, 95, 103, 112, 122, 133, 145, 159, 173, 189, 206, 224, 243, 264, 245 }[Player.Level - 1]
                + Player.FlatPhysicalDamageMod
                + new double[] { 0.55, 0.70, 0.85, 1 }[nhantu] * Player.TotalMagicalDamage ;
            return Player.CalcDamage(target, Damage.DamageType.Magical, raw);

        }
        public static double GetQDamage(Obj_AI_Base target)
        {
            if (Q.Level == 0)
                return 0;
            var raw = new double[] { 75, 105, 135, 165, 195 }[Q.Level - 1] + 0.3 * Player.TotalMagicalDamage;
            return Player.CalcDamage(target, Damage.DamageType.Magical, raw);
        }
        public static double GetEDamage(Obj_AI_Base target)
        {
            if (E.Level == 0)
                return 0;
            var raw = new double[] { 30, 45, 60, 75, 90 }[E.Level - 1] + 0.25 * Player.TotalMagicalDamage + 0.65 * Player.TotalAttackDamage;
            return Player.CalcDamage(target, Damage.DamageType.Magical, raw);
        }
        public static double GetRDamge(Obj_AI_Base target)
        {
            if (R.Level == 0)
                return 0;
            var raw = new double[] {375, 562.5,750}[R.Level - 1] + 2.85 * Player.TotalMagicalDamage + 3.30 * Player.TotalAttackDamage;
            return Player.CalcDamage(target, Damage.DamageType.Magical, raw);
        }
        public static List <GameObject> GetEVinasun()
        {
            List<GameObject> Vinasun = new List<GameObject>();
            Vinasun.AddRange(MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.All).Where(x => !((x is Obj_AI_Minion) && MinionManager.IsWard(x as Obj_AI_Minion))));
            Vinasun.AddRange(HeroManager.AllHeroes.Where(unit => unit != null && unit.IsValid && !unit.IsDead && unit.IsTargetable && Player.Distance(unit) <= E.Range));
            Vinasun.AddRange(Daggers.Select(x => x.Dagger).ToList());
            return Vinasun;
        }
        public static List<Obj_AI_Base> GetQVinasun()
        {
            List<Obj_AI_Base> Vinasun = new List<Obj_AI_Base>();
            Vinasun.AddRange(MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.All).Where(x => !((x is Obj_AI_Minion) && MinionManager.IsWard(x as Obj_AI_Minion))));
            Vinasun.AddRange(HeroManager.AllHeroes.Where(unit => unit != null && unit.IsValid && !unit.IsDead && unit.IsTargetable && Player.Distance(unit) <= Q.Range));
            return Vinasun;
        }
    }
}
