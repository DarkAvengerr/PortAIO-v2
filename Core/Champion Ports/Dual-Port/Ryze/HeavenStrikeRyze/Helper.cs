using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using ItemData = LeagueSharp.Common.Data.ItemData;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HeavenStrikeRyze
{
    public static class Helper
    {
        private static AIHeroClient Player { get { return ObjectManager.Player; } }
        private static string QBuffName = "";
        private static string EBuffName = "RyzeE";
        public static double BonusMana
        {
           get
            {
                return Player.MaxMana - new double[] { 392, 430, 469, 510, 553, 598, 644, 693, 743, 795, 849, 904, 962, 1021, 1082, 1145, 1210, 1276 }[Player.Level - 1];
            }
        }
        public static double Qdamage(Obj_AI_Base target, bool hasbuffe = false)
        {
            if (Program._e.Level == 0) hasbuffe = false;
            if (Program._q.Level == 0) return 0;
            double damage = new double[] { 60, 85, 110, 135, 160, 185 }[Program._q.Level - 1]
                + 0.45 * Player.TotalMagicalDamage
                + 0.3 * BonusMana;
            double bonus = (HasEBuff(target) || hasbuffe) ? new double[] { 0.4, 0.55, 0.7, 0.85, 1 }[Program._e.Level - 1] : 0;
            return Player.CalcDamage(target, Damage.DamageType.Magical, damage * (1 + bonus));
        }
        public static double Wdamge(Obj_AI_Base target)
        {
            if (Program._w.Level == 0) return 0;
            var damage = new double[] { 80, 100, 120, 140, 160 }[Program._w.Level - 1]
                + 0.2 * Player.TotalMagicalDamage
                + 0.1 * BonusMana;
            return Player.CalcDamage(target, Damage.DamageType.Magical, damage);
        }
        public static double Edamge(Obj_AI_Base target)
        {
            if (Program._e.Level == 0) return 0;
            var damage = new double[] { 50, 75, 100, 125, 150 }[Program._e.Level - 1]
                + 0.3 * Player.TotalMagicalDamage
                + 0.2 * BonusMana;
            return Player.CalcDamage(target, Damage.DamageType.Magical, damage);
        }
        public static int RRAnge()
        {
            return Program._r.Level == 2 ? 3000 : 1500;
        }
        public static bool HasEBuff(Obj_AI_Base target)
        {
            return target.HasBuff(EBuffName) ;
        }
        public static int Qstack()
        {
            if (Player.HasBuff("ryzeqiconnocharge"))
                return 0;
            if (Player.HasBuff("ryzeqiconhalfcharge"))
                return 1;
            if (Player.HasBuff("ryzeqiconfullcharge"))
                return 2;
            return 0;
        }
        public static bool CanShield()
        {
            if (Player.Mana < Program._q.ManaCost + Program._w.ManaCost + Program._e.ManaCost)
                return false;
            if (Qstack() == 0)
                return false;
            int x = 0;
            if (Program._w.IsReady())
                x += 1;
            if (Program._e.IsReady())
                x += 1;
            x += Qstack();
            if (x >= 2)
                return true;
            return false; 
        }
        public static void CastQTarget(Obj_AI_Base target, bool forceQ = false)
        {
            if (!target.IsValidTarget(Program._q.Range) || target.IsZombie || !Program._q.IsReady())
                return;
            var pred = Program._q.GetPrediction(target);
            if (pred.Hitchance >= HitChance.Low)
            {
                Program._q.Cast(pred.CastPosition);
            }
            else
            {
                if (HasEBuff(target))
                {
                    foreach (var tar in ObjectManager.Get<Obj_AI_Base>()
                        .Where(x =>!x.IsAlly && HasEBuff(x) && !x.IsDead
                            && GetchainedTarget(x).Any(y => y.NetworkId == target.NetworkId)))
                    {
                        var pred1 = Program._q.GetPrediction(tar);
                        if (pred1.Hitchance >= HitChance.Low)
                        {
                            Program._q.Cast(pred1.CastPosition);
                        }
                    }
                }
            }
            if (forceQ)
            {
                var predF = Program._q2.GetPrediction(target);
                if (predF.Hitchance >= HitChance.Low)
                {
                    Program._q2.Cast(pred.CastPosition);
                }
            }
        }
        public static List<Obj_AI_Base> GetchainedTarget(Obj_AI_Base target)
        {
            var targets = new List<Obj_AI_Base>();
            if (!HasEBuff(target))
                return targets;
            targets.Add(target);
            var hasbuff = ObjectManager.Get<Obj_AI_Base>().Where(x => !x.IsAlly && HasEBuff(x) && ! x.IsDead && x.Distance(target) <= 1000);
            int count = 1;
            while (count == 1)
            {
                count = 2;
                foreach (var tar in hasbuff)
                {
                     if ( HasEBuff(tar) && !tar.IsDead && targets.Any(x => x.Distance(tar.Position) <= 300) && !targets.Any(x => x.NetworkId == tar.NetworkId))
                    {
                        targets.Add(tar);
                        count = 1;
                    }
                }
            }
            return targets;
        }
    }
}
