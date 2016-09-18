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
 namespace BadaoKingdom.BadaoChampion.BadaoJhin
{
    public static class BadaoJhinHelper
    {
        public static bool DrawWMiniMap()
        {
            return BadaoJhinVariables.DrawWMiniMap.GetValue<bool>();
        }
        public static uint AutoRTapKey()
        {
            return BadaoJhinVariables.AutoRTapKey.GetValue<KeyBind>().Key;
        }
        public static bool AutoRTargetAuto()
        {
            return BadaoJhinVariables.AutoRTarget.GetValue<StringList>().SelectedIndex == 2;
        }
        public static bool AutoRTargetNearMouse()
        {
            return BadaoJhinVariables.AutoRTarget.GetValue<StringList>().SelectedIndex == 1;
        }
        public static bool AutoRTargetSelected()
        {
            return BadaoJhinVariables.AutoRTarget.GetValue<StringList>().SelectedIndex == 0;
        }
        public static bool AutoRModeAuto()
        {
            return BadaoJhinVariables.AutoRMode.GetValue<StringList>().SelectedIndex == 0;
        }
        public static bool AutoRModeOnTap()
        {
            return BadaoJhinVariables.AutoRMode.GetValue<StringList>().SelectedIndex == 1 ;
        }
        public static bool UseAutoPingKillable()
        {
            return BadaoJhinVariables.AutoPingKillable.GetValue<bool>();
        }
        public static bool UseAutoKS()
        {
            return BadaoJhinVariables.AutoKS.GetValue<bool>();
        }
        public static bool UseWAutoTrap()
        {
            return BadaoMainVariables.W.IsReady() && BadaoJhinVariables.AutoWTrap.GetValue<bool>();
        }
        public static bool UseWAuto()
        {
            return BadaoMainVariables.W.IsReady() && BadaoJhinVariables.AutoW.GetValue<bool>();
        }
        public static bool UseRAuto()
        {
            return BadaoMainVariables.R.IsReady() && BadaoJhinVariables.AutoR.GetValue<bool>();
        }
        public static bool UseQJungle()
        {
            return BadaoMainVariables.Q.IsReady() && BadaoJhinVariables.JungleClearQ.GetValue<bool>();
        }
        public static bool UseWJungle()
        {
            return BadaoMainVariables.W.IsReady() && BadaoJhinVariables.JungleClearW.GetValue<bool>();
        }
        public static bool UseEJungle()
        {
            return BadaoMainVariables.E.IsReady() && BadaoJhinVariables.JungleClearE.GetValue<bool>();
        }
        public static bool UseQLane()
        {
            return BadaoMainVariables.Q.IsReady() && BadaoJhinVariables.LaneClearQ.GetValue<bool>();
        }
        public static bool UseQHarass()
        {
            return BadaoMainVariables.Q.IsReady() && BadaoJhinVariables.HarassQ.GetValue<bool>();
        }
        public static bool UseWHarass()
        {
            return BadaoMainVariables.W.IsReady() && BadaoJhinVariables.HarassW.GetValue<bool>();
        }
        public static bool UseEHarass()
        {
            return BadaoMainVariables.E.IsReady() && BadaoJhinVariables.HarassE.GetValue<bool>();
        }
        public static bool UseECombo()
        {
            return BadaoMainVariables.E.IsReady() && BadaoJhinVariables.ComboE.GetValue<bool>();
        }
        public static bool UseWCombo()
        {
            return BadaoMainVariables.W.IsReady() && BadaoJhinVariables.ComboW.GetValue<bool>();
        }
        public static bool UseWOnlySnareCombo()
        {
            return BadaoMainVariables.W.IsReady() && BadaoJhinVariables.ComboWOnlySnare.GetValue<bool>();
        }
        public static bool UseQCombo()
        {
            return BadaoMainVariables.Q.IsReady() && BadaoJhinVariables.ComboQ.GetValue<bool>();
        }
        public static bool CanHarassMana()
        {
            return ObjectManager.Player.Mana / ObjectManager.Player.MaxMana * 100 >= BadaoJhinVariables.HarassMana.GetValue<Slider>().Value;
        }
        public static bool CanLaneClearMana()
        {
            return ObjectManager.Player.Mana / ObjectManager.Player.MaxMana * 100 >= BadaoJhinVariables.LaneClearMana.GetValue<Slider>().Value;
        }
        public static bool CanJungleClearMana()
        {
            return ObjectManager.Player.Mana / ObjectManager.Player.MaxMana * 100 >= BadaoJhinVariables.JungleClearMana.GetValue<Slider>().Value;
        }
        public static bool CanAutoMana()
        {
            return ObjectManager.Player.Mana / ObjectManager.Player.MaxMana * 100 >= BadaoJhinVariables.AutoMana.GetValue<Slider>().Value;
        }
        public static bool HasJhinPassive(AIHeroClient target)
        {
            return BadaoJhinPassive.JhinPassive.Any(x => Geometry.Distance(x.Position.To2D(),(target.Position.To2D())) <= 50);
        }

        // damage calculation
        // 60 / 85 / 110 / 135 / 160 (+ 30 / 35 / 40 / 45 / 50% AD) (+ 60% AP)
        public static float GetQDamage(Obj_AI_Base target)
        {
            float rawDamage = new float[] { 60, 85, 110, 135, 160 }[BadaoMainVariables.Q.Level - 1]
                + new float[] { 0.3f, 0.35f, 0.4f, 0.45f, 0.5f }[BadaoMainVariables.Q.Level - 1] * GetTotalAttackDamage()
                + 0.6f * ObjectManager.Player.TotalMagicalDamage;
            return (float)ObjectManager.Player.CalcDamage(target, Damage.DamageType.Physical, rawDamage);

        }
        public static float GetWDamage(Obj_AI_Base target)
        {
            float rawDamage = new float[] { 50, 85, 120, 155, 190 }[BadaoMainVariables.W.Level - 1]
                + 0.7f * GetTotalAttackDamage();
            return (float)ObjectManager.Player.CalcDamage(target, Damage.DamageType.Physical, rawDamage);
        }
        public static float GetRdamage(Obj_AI_Base target)
        {
            float health = target.Health;
            float damage1 = BasicRdamage(target, health, 1);
            if (damage1 >= health)
                return target.MaxHealth + 100;
            health -= damage1;
            float damage2 = BasicRdamage(target, health, 1);
            if (damage2 >= health)
                return target.MaxHealth + 100;
            health -= damage2;
            float damage3 = BasicRdamage(target, health, 1);
            if (damage3 >= health)
                return target.MaxHealth + 100;
            health -= damage3;
            float damage4 = BasicRdamage(target, health, 2);
            if (damage4 >= health)
                return target.MaxHealth + 100;
            health -= damage4;
            return target.Health - health;

        }
        public static float BasicRdamage(Obj_AI_Base target, float Health, int state)
        {
            float missingHealth = (target.MaxHealth - Health) / target.MaxHealth;
            float rawDamage = new float[] { 50, 125, 200 }[BadaoMainVariables.R.Level - 1]
                + 0.25f * GetTotalAttackDamage();
            float calcDamage = (float)ObjectManager.Player.CalcDamage(target, Damage.DamageType.Physical, rawDamage);
            if (state == 1)
            {
                return calcDamage * (1 + missingHealth * 2);
            }
            return calcDamage * 2 * (1 + missingHealth * 2);
        }
        public static float GetTotalAttackDamage()
        {
            return
                ObjectManager.Player.TotalAttackDamage +
                (new float[] { 2, 3, 4, 5, 6, 7, 8, 10, 12, 14, 16, 18, 20, 24, 28, 32, 36, 40 }[ObjectManager.Player.Level - 1] / 100f
                 + ObjectManager.Player.Crit * 0.4f + (ObjectManager.Player.AttackSpeedMod - 1f) * 0.25f)
                 * ObjectManager.Player.TotalAttackDamage; 
        }

        // get Q info
        public static List<QInfo> GetQInfo()
        {
            List<QInfo> QInfo = new List<QInfo>();
            List<Obj_AI_Base> Targets = new List<Obj_AI_Base>();
            var heroes = HeroManager.Enemies.Where(x => x != null && x.IsValid && !x.IsDead);
            var laneMinions = MinionManager.GetMinions(ObjectManager.Player.Position, 2100);
            var jungleMinions = MinionManager.GetMinions(ObjectManager.Player.Position, 2100, MinionTypes.All, MinionTeam.Neutral);
            Targets.AddRange(heroes);
            Targets.AddRange(laneMinions);
            Targets.AddRange(jungleMinions);
            var TargetsQ = Targets.Where(x => x.Distance(ObjectManager.Player.Position) <= 600);
            foreach (var target in TargetsQ)
            {
                List<int> usedID = new List<int>();
                int deathCount = 0;
                int deathCount1 = 0;
                int deathCount2 = 0;
                int deathCount3 = 0;
                int deathCount4 = 0;
                usedID.Add(target.NetworkId);
                if (GetQDamage(target) * (1f + deathCount * 0.35f) > target.Health)
                    deathCount += 1;
                deathCount1 = deathCount;
                //target2
                var target2 = Targets.OrderBy(x => x.Distance(target))
                    .Where(x => !usedID.Contains(x.NetworkId) && x.Distance(target) <= 500).FirstOrDefault();
                if (target2 ==  null)
                {
                    QInfo.Add(new QInfo()
                    {
                        QTarget = target,
                        BounceTargets = new List<TargetInfo>()
                            {
                                new TargetInfo() { Target = target, DeathCount = 0}
                            },
                        DeathCount = deathCount
                    });
                    continue;
                }
                usedID.Add(target2.NetworkId);
                if (GetQDamage(target2) * (1f + deathCount * 0.35f) > target2.Health)
                    deathCount += 1;
                deathCount2 = deathCount;
                //target3
                var target3 = Targets.OrderBy(x => x.Distance(target))
                    .Where(x => !usedID.Contains(x.NetworkId) && x.Distance(target2) <= 500).FirstOrDefault();
                if (target3 == null)
                {
                    QInfo.Add(new QInfo()
                    {
                        QTarget = target,
                        BounceTargets = new List<TargetInfo>()
                            {
                                new TargetInfo() { Target = target, DeathCount = 0 },
                                new TargetInfo() { Target = target2, DeathCount = deathCount1 }
                            },
                        DeathCount = deathCount
                    });
                    continue;
                }
                usedID.Add(target3.NetworkId);
                if (GetQDamage(target3) * (1f + deathCount * 0.35f) > target3.Health)
                    deathCount += 1;
                deathCount3 = deathCount;
                //target4
                var target4 = Targets.OrderBy(x => x.Distance(target))
                    .Where(x => !usedID.Contains(x.NetworkId) && x.Distance(target3) <= 500).FirstOrDefault();
                if (target4 == null)
                {
                    QInfo.Add(new QInfo()
                    {
                        QTarget = target,
                        BounceTargets = new List<TargetInfo>()
                            {
                                new TargetInfo() { Target = target, DeathCount = 0 },
                                new TargetInfo() { Target = target2, DeathCount = deathCount1 },
                                new TargetInfo() { Target = target3, DeathCount = deathCount2 }
                            },
                        DeathCount = deathCount
                    });
                    continue;
                }
                if (GetQDamage(target4) * (1f + deathCount * 0.35f) > target4.Health)
                    deathCount += 1;
                deathCount4 = deathCount;
                QInfo.Add(new QInfo()
                {
                    QTarget = target,
                    BounceTargets = new List<TargetInfo>()
                        {
                            new TargetInfo() { Target = target, DeathCount = 0 },
                            new TargetInfo() { Target = target2, DeathCount = deathCount1 },
                            new TargetInfo() { Target = target3, DeathCount = deathCount2 },
                            new TargetInfo() { Target = target4, DeathCount = deathCount3 }
                        },
                    DeathCount = deathCount
                });
            }
            return QInfo;
        }
        public class QInfo
        {
            public Obj_AI_Base QTarget = null;
            public List<TargetInfo> BounceTargets = new List<TargetInfo>();
            public int DeathCount = 0;
        }
        public class TargetInfo
        {
            public Obj_AI_Base Target = null;
            public int DeathCount = 0;

        }
    }
}
