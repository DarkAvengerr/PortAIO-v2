using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SCommon;
using SCommon.Database;
using SCommon.PluginBase;
using SCommon.Orbwalking;
using SCommon.Prediction;
using SUtility.Drawings;
using SharpDX;
//typedefs
using TargetSelector = SCommon.TS.TargetSelector;
using EloBuddy;

namespace SAutoCarry.Champions
{
    public class Viktor : SCommon.PluginBase.Champion
    {
        private int m_laserLenght = 500;
        private int m_lastFollowTick = 0;

        public Viktor()
            : base ("Viktor", "SAutoCarry - Viktor")
        {
            OnUpdate += BeforeOrbwalk;
            OnCombo += Combo;
            OnHarass += Harass;
            OnLaneClear += LaneClear;
        }

        public override void CreateConfigMenu()
        {
            Menu combo = new Menu("Combo", "SAutoCarry.Viktor.Combo");
            combo.AddItem(new MenuItem("SAutoCarry.Viktor.Combo.UseQ", "Use Q").SetValue(true)).ValueChanged += (s, ar) => ConfigMenu.Item("SAutoCarry.Viktor.Combo.UseQOnlyRange").Show(ar.GetNewValue<bool>());
            combo.AddItem(new MenuItem("SAutoCarry.Viktor.Combo.UseQOnlyRange", "Use Q Only When In AA Range (faster Q->AA burst)").SetValue(new KeyBind('G', KeyBindType.Toggle, true))).Show(combo.Item("SAutoCarry.Viktor.Combo.UseQ").GetValue<bool>());
            combo.AddItem(new MenuItem("SAutoCarry.Viktor.Combo.UseE", "Use E").SetValue(true));
            combo.AddItem(new MenuItem("SAutoCarry.Viktor.Combo.UseR", "Use R").SetValue(true)).ValueChanged += (s, ar) => ConfigMenu.Item("SAutoCarry.Viktor.Combo.UseRMin").Show(ar.GetNewValue<bool>());
            combo.AddItem(new MenuItem("SAutoCarry.Viktor.Combo.UseRMin", "Use R Min. Hit").SetValue(new Slider(1, 1, 5))).Show(combo.Item("SAutoCarry.Viktor.Combo.UseR").GetValue<bool>());

            Menu harass = new Menu("Harass", "SAutoCarry.Viktor.Harass");
            harass.AddItem(new MenuItem("SAutoCarry.Viktor.Harass.UseQ", "Use Q").SetValue(true));
            harass.AddItem(new MenuItem("SAutoCarry.Viktor.Harass.UseE", "Use E").SetValue(true));
            harass.AddItem(new MenuItem("SAutoCarry.Viktor.Harass.MinMana", "Min. Mana").SetValue(new Slider(60, 0, 100)));
            harass.AddItem(new MenuItem("SAutoCarry.Viktor.Harass.Toggle", "Toggle Harass").SetValue(new KeyBind('Z', KeyBindType.Toggle)));

            Menu laneclear = new Menu("LaneClear", "SAutoCarry.Viktor.LaneClear");
            laneclear.AddItem(new MenuItem("SAutoCarry.Viktor.LaneClear.UseQ", "Use Q").SetValue(false));
            laneclear.AddItem(new MenuItem("SAutoCarry.Viktor.LaneClear.UseE", "Use E").SetValue(true)).ValueChanged += (s, ar) => ConfigMenu.Item("SAutoCarry.Viktor.LaneClear.UseEMin").Show(ar.GetNewValue<bool>());
            laneclear.AddItem(new MenuItem("SAutoCarry.Viktor.LaneClear.UseEMin", "Use E Min. Minions").SetValue(new Slider(3, 1, 6))).Show(laneclear.Item("SAutoCarry.Viktor.LaneClear.UseE").GetValue<bool>());
            laneclear.AddItem(new MenuItem("SAutoCarry.Viktor.LaneClear.Toggle", "Enable Spellfarm").SetValue(new KeyBind('T', KeyBindType.Toggle, true)));
            laneclear.AddItem(new MenuItem("SAutoCarry.Viktor.LaneClear.MinMana", "Min. Mana %").SetValue(new Slider(40, 0, 100)));

            Menu misc = new Menu("Misc", "SAutoCarry.Viktor.Misc");
            misc.AddItem(new MenuItem("SAutoCarry.Viktor.Misc.AutoFollowR", "Auto Follow R").SetValue(true));
            misc.AddItem(new MenuItem("SAutoCarry.Viktor.Misc.InterruptR", "Interrupt Channeling Spells With R").SetValue(true));
            misc.AddItem(new MenuItem("SAutoCarry.Viktor.Misc.InterruptW", "Interrupt Spells With W").SetValue(true));
            misc.AddItem(new MenuItem("SAutoCarry.Viktor.Misc.AntiGapW", "Anti Gap Closer With W").SetValue(true));
            misc.AddItem(new MenuItem("SAutoCarry.Viktor.Misc.ImmobileW", "Auto W To Immobile Target").SetValue(true));

            DamageIndicator.Initialize((t) => (float)CalculateComboDamage(t), misc);

            ConfigMenu.AddSubMenu(combo);
            ConfigMenu.AddSubMenu(harass);
            ConfigMenu.AddSubMenu(laneclear);
            ConfigMenu.AddSubMenu(misc);
            ConfigMenu.AddToMainMenu();
        }

        public override void SetSpells()
        {
            Spells[Q] = new Spell(SpellSlot.Q, 600f);

            Spells[W] = new Spell(SpellSlot.W, 700f);

            Spells[E] = new Spell(SpellSlot.E, 525f);
            Spells[E].SetSkillshot(0f, 80f, 1200f, false, SkillshotType.SkillshotLine);

            Spells[R] = new Spell(SpellSlot.R, 700f);
            Spells[R].SetSkillshot(0.25f, 450f, 0, false, SkillshotType.SkillshotCircle);
        }

        public void BeforeOrbwalk()
        {
            if (AutoFollowR)
                FollowR();

            if (ImmobileW)
                AutoImmobileW();

            if (HarassToggle)
                Harass();
        }

        public void Combo()
        {
            if (Spells[E].LSIsReady() && ComboUseE)
            {
                var t = TargetSelector.GetTarget(Spells[E].Range + m_laserLenght, LeagueSharp.Common.TargetSelector.DamageType.Magical);
                if (t != null)
                {
                    if (SCommon.Orbwalking.Utility.InAARange(t) && ObjectManager.Player.HasBuff("viktorpowertransferreturn") && Orbwalker.CanAttack(250))
                        return;

                    Spells[E].SPredictionCastVector(t, m_laserLenght, HitChance.High);
                }
            }

            if (Spells[R].LSIsReady() && ComboUseR)
            {
                if (ComboUseRMin == 1)
                {
                    var t = TargetSelector.GetTarget(Spells[R].Range, LeagueSharp.Common.TargetSelector.DamageType.Magical);
                    if (t != null && t.Health - CalculateComboDamage(t) - (Spells[Q].LSIsReady(1000) ? CalculateViktorPassiveAADamage(t) : 0) < 200)
                    {
                        if (SCommon.Orbwalking.Utility.InAARange(t) && ObjectManager.Player.HasBuff("viktorpowertransferreturn") && Orbwalker.CanAttack(250))
                            return;
                        Spells[R].SPredictionCast(t, HitChance.High);
                    }
                }
                else
                    Spells[R].SPredictionCastAoe(ComboUseRMin);
            }

            if(Spells[Q].LSIsReady() && ComboUseQ && !ComboUseQOnlyAA)
            {
                var t = TargetSelector.GetTarget(Spells[Q].Range, LeagueSharp.Common.TargetSelector.DamageType.Magical);
                if (t != null)
                    Spells[Q].CastOnUnit(t);
            }
        }

        public void Harass()
        {
            if (ObjectManager.Player.ManaPercent < HarassMinMana)
                return;

            if (Spells[E].LSIsReady() && HarassUseE)
            {
                var t = TargetSelector.GetTarget(Spells[E].Range + m_laserLenght, LeagueSharp.Common.TargetSelector.DamageType.Magical);
                if (t != null)
                {
                    if (SCommon.Orbwalking.Utility.InAARange(t) && ObjectManager.Player.HasBuff("viktorpowertransferreturn") && Orbwalker.CanAttack(250))
                        return;

                    Spells[E].SPredictionCastVector(t, m_laserLenght, HitChance.High);
                }
            }
        }

        public void LaneClear()
        {
            if (ObjectManager.Player.ManaPercent < LaneClearMinMana || !LaneClearToggle)
                return;

            if (LaneClearUseQ)
            {
                var minion = MinionManager.GetMinions(Spells[Q].Range).Where(p => p.IsSiegeMinion() || p.CharData.BaseSkinName.Contains("Siege")).FirstOrDefault();
                if (minion != null)
                {
                    if ((minion.CharData.BaseSkinName.Contains("Siege") && Spells[Q].IsKillable(minion)) || minion.IsSiegeMinion())
                        Spells[Q].CastOnUnit(minion);
                }
            }

            if (LaneClearUseE)
            {
                var fromPosition = GetLaneClearLaserStart();
                if (fromPosition.LSIsValid())
                {
                    var farmLocation = GetBestLaserFarmLocation(fromPosition, MinionManager.GetMinionsPredictedPositions(MinionManager.GetMinions(fromPosition.To3D(), 700), 0, 80, 1200f, fromPosition.To3D(), 700, false, SkillshotType.SkillshotLine), 80, 700);

                    if (farmLocation.MinionsHit >= LaneClearUseEMin)
                        Spells[E].Cast(fromPosition, farmLocation.Position);
                }
            }
        }

        private static Vector2 GetLaneClearLaserStart()
        {
            int hitNum = 0;
            Vector2 startPos = new Vector2(0, 0);
            Vector2 endPos = new Vector2(0, 0);
            foreach (var minion in MinionManager.GetMinions(ObjectManager.Player.ServerPosition, 700))
            {
                var farmLocation = GetBestLaserFarmLocation(minion.Position.LSTo2D(), (from mnion in MinionManager.GetMinions(minion.Position, 700) select mnion.Position.LSTo2D()).ToList<Vector2>(), 80, 700);
                if (farmLocation.MinionsHit > hitNum)
                {
                    hitNum = farmLocation.MinionsHit;
                    startPos = minion.Position.LSTo2D();
                    endPos = farmLocation.Position;
                }
            }
            return startPos;
        }


        private static MinionManager.FarmLocation GetBestLaserFarmLocation(Vector2 sourcepos, List<Vector2> minionPositions, float width, float range)
        {
            var result = new Vector2();
            var minionCount = 0;
            var startPos = sourcepos;

            var max = minionPositions.Count;
            for (var i = 0; i < max; i++)
            {
                for (var j = 0; j < max; j++)
                {
                    if (minionPositions[j] != minionPositions[i])
                    {
                        minionPositions.Add((minionPositions[j] + minionPositions[i]) / 2);
                    }
                }
            }

            foreach (var pos in minionPositions)
            {
                if (pos.LSDistance(startPos, true) <= range * range)
                {
                    var endPos = startPos + range * (pos - startPos).LSNormalized();

                    var count =
                        minionPositions.Count(pos2 => pos2.LSDistance(startPos, endPos, true, true) <= width * width);

                    if (count >= minionCount)
                    {
                        result = endPos;
                        minionCount = count;
                    }
                }
            }

            return new MinionManager.FarmLocation(result, minionCount);
        }


        private void FollowR()
        {
            var t = TargetSelector.GetTarget(1100f, LeagueSharp.Common.TargetSelector.DamageType.Magical);
            if(t != null)
            {
                if(Spells[R].Instance.Name != "ViktorChaosStorm" && Utils.TickCount - m_lastFollowTick > 500)
                {
                    Spells[R].Cast(t.ServerPosition);
                    m_lastFollowTick = Utils.TickCount;
                }
            }
        }

        private void AutoImmobileW()
        {
            var target = HeroManager.Enemies.Where(p => p.Buffs.Any(q => Data.IsImmobilizeBuff(q.Type)) && p.LSIsValidTarget(Spells[W].Range)).FirstOrDefault();
            if (target != null)
                Spells[W].Cast(target.ServerPosition);
        }

        private double CalculateViktorPassiveAADamage(AIHeroClient target)
        {
            return ObjectManager.Player.CalcDamage(target, LeagueSharp.Common.Damage.DamageType.Magical, 0.5f * ObjectManager.Player.TotalMagicalDamage + new float[] { 20, 25, 30, 35, 40, 45, 50, 55, 60, 70, 80, 90, 110, 130, 150, 170, 190, 210 }[ObjectManager.Player.Level - 1]);
        }

        protected override void OrbwalkingEvents_BeforeAttack(BeforeAttackArgs args)
        {
            if (args.Target.Type == GameObjectType.AIHeroClient)
            {
                if (Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.Combo)
                {
                    if (Spells[Q].LSIsReady() && ComboUseQ)
                    {
                        Spells[Q].CastOnUnit(args.Target as AIHeroClient);
                        args.Process = false;
                    }
                }
                else if (Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.Mixed)
                {
                    if (Spells[Q].LSIsReady() && HarassUseQ)
                    {
                        Spells[Q].CastOnUnit(args.Target as AIHeroClient);
                        args.Process = false;
                    }
                }
            }
        }

        protected override void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (AntiGapW & gapcloser.End.LSDistance(ObjectManager.Player.ServerPosition) < Spells[W].Range)
                Spells[W].Cast(gapcloser.End);
        }

        protected override void Interrupter_OnPossibleToInterrupt(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (InterruptR && args.MovementInterrupts && args.DangerLevel == Interrupter2.DangerLevel.High)
            {
                if (sender.LSIsValidTarget(Spells[R].Range))
                    Spells[R].Cast(sender.ServerPosition);
            }
            else if (InterruptW && args.MovementInterrupts)
            {
                if (sender.LSIsValidTarget(Spells[W].Range))
                    Spells[W].Cast(sender.ServerPosition);
            }
        }

        public override double CalculateAADamage(AIHeroClient target, int aacount = 2)
        {
            var qaaDmg = new [] { 20, 25, 30, 35, 40, 45, 50, 55, 60, 70, 80, 90, 110, 130, 150, 170, 190, 210 };
            if (ObjectManager.Player.HasBuff("viktorpowertransferreturn"))
                return CalculateViktorPassiveAADamage(target) + base.CalculateAADamage(target) * (aacount - 1);
            else
                return base.CalculateAADamage(target, aacount);
        }

        public bool ComboUseQ
        {
            get { return ConfigMenu.Item("SAutoCarry.Viktor.Combo.UseQ").GetValue<bool>(); }
        }

        public bool ComboUseQOnlyAA
        {
            get { return ConfigMenu.Item("SAutoCarry.Viktor.Combo.UseQOnlyRange").GetValue<KeyBind>().Active; }
        }

        public bool ComboUseE
        {
            get { return ConfigMenu.Item("SAutoCarry.Viktor.Combo.UseE").GetValue<bool>(); }
        }

        public bool ComboUseR
        {
            get { return ConfigMenu.Item("SAutoCarry.Viktor.Combo.UseR").GetValue<bool>(); }
        }

        public int ComboUseRMin
        {
            get { return ConfigMenu.Item("SAutoCarry.Viktor.Combo.UseRMin").GetValue<Slider>().Value; }
        }

        public bool HarassUseQ
        {
            get { return ConfigMenu.Item("SAutoCarry.Viktor.Harass.UseQ").GetValue<bool>(); }
        }

        public bool HarassUseE
        {
            get { return ConfigMenu.Item("SAutoCarry.Viktor.Harass.UseE").GetValue<bool>(); }
        }

        public int HarassMinMana
        {
            get { return ConfigMenu.Item("SAutoCarry.Viktor.Harass.MinMana").GetValue<Slider>().Value; }
        }

        public bool HarassToggle
        {
            get { return ConfigMenu.Item("SAutoCarry.Viktor.Harass.Toggle").GetValue<KeyBind>().Active; }
        }

        public bool LaneClearUseQ
        {
            get { return ConfigMenu.Item("SAutoCarry.Viktor.LaneClear.UseQ").GetValue<bool>(); }
        }

        public bool LaneClearUseE
        {
            get { return ConfigMenu.Item("SAutoCarry.Viktor.LaneClear.UseE").GetValue<bool>(); }
        }

        public int LaneClearUseEMin
        {
            get { return ConfigMenu.Item("SAutoCarry.Viktor.LaneClear.UseEMin").GetValue<Slider>().Value; }
        }

        public bool LaneClearToggle
        {
            get { return ConfigMenu.Item("SAutoCarry.Viktor.LaneClear.Toggle").GetValue<KeyBind>().Active; }
        }

        public int LaneClearMinMana
        {
            get { return ConfigMenu.Item("SAutoCarry.Viktor.LaneClear.MinMana").GetValue<Slider>().Value; }
        }

        public bool AutoFollowR
        {
            get { return ConfigMenu.Item("SAutoCarry.Viktor.Misc.AutoFollowR").GetValue<bool>(); }
        }

        public bool InterruptR
        {
            get { return ConfigMenu.Item("SAutoCarry.Viktor.Misc.InterruptR").GetValue<bool>(); }
        }

        public bool InterruptW
        {
            get { return ConfigMenu.Item("SAutoCarry.Viktor.Misc.InterruptW").GetValue<bool>(); }
        }

        public bool AntiGapW
        {
            get { return ConfigMenu.Item("SAutoCarry.Viktor.Misc.AntiGapW").GetValue<bool>(); }
        }

        public bool ImmobileW
        {
            get { return ConfigMenu.Item("SAutoCarry.Viktor.Misc.ImmobileW").GetValue<bool>(); }
        }
    }
}
