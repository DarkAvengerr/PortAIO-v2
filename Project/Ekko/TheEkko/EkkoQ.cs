using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using TheEkko.ComboSystem;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace TheEkko
{
    class EkkoQ : Skill
    {
        private readonly float[] _damage1 = { 60, 75, 90, 105, 120 };
        private readonly float[] _damage2 = { 60, 85, 110, 135, 160 };
        public int MinFarm = 4;

        public EkkoQ(Spell spell)
            : base(spell)
        {
            spell.SetSkillshot(0.5f, 60, 1200, false, SkillshotType.SkillshotLine);
            //Console.WriteLine(spell.Instance.SData.LineWidth + " lw");
            //Console.WriteLine(spell.Instance.SData.MissileSpeed + " cast");
        }


        public override void Cast(AIHeroClient target, bool force = false, HitChance minChance = HitChance.Low)
        {
            if (HasBeenSafeCast() || target == null) return;
            var prediction = Spell.GetPrediction(target);
            if (prediction.Hitchance < minChance) return;
            SafeCast(() => Spell.Cast(prediction.CastPosition));

        }

        //public override void LaneClear(IMainContext context, ComboProvider combo, AIHeroClient target)
        //{
            
        //    var minions = MinionManager.GetMinions(1000);
        //    if (minions.Count == 0) return;
        //    var farmLocation = Spell.GetLineFarmLocation(minions, 60f); //Todo: use line farm location. seems buggy?
        //    Console.WriteLine(farmLocation.Position);
        //    SafeCast(() =>
        //    {
        //        Spell.Cast(farmLocation.Position.To3D2());
        //    }
        //    );
        //}

        public override float GetDamage(AIHeroClient enemy)
        {
            if (Spell.Level == 0) return 0;
            return (float)ObjectManager.Player.CalcDamage(enemy, Damage.DamageType.Magical, _damage1[Spell.Level - 1] + ObjectManager.Player.TotalMagicalDamage * 0.2f + _damage2[Spell.Level - 1] + ObjectManager.Player.TotalMagicalDamage * 0.6f);
        }

        public override int GetPriority()
        {
            return 1;
        }
    }
}
