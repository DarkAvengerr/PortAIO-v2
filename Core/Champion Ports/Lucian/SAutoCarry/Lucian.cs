using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SCommon;
using SCommon.PluginBase;
using SCommon.Prediction;
using SCommon.Maths;
using SharpDX;
//typedefs
using TargetSelector = SCommon.TS.TargetSelector;
using EloBuddy;

namespace SAutoCarry.Champions
{
    public class Lucian : SCommon.PluginBase.Champion
    {
        private bool m_spellCasting;
        public Lucian()
            : base("Lucian", "SAutoCarry - Lucian")
        {
            OnDraw += BeforeDraw;
            OnUpdate += BeforeOrbwalk;
            OnCombo += Combo;
            OnHarass += Harass;
            OnLaneClear += LaneClear;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            Spellbook.OnStopCast += Spellbook_OnStopCast;
            SCommon.Prediction.Prediction.predMenu.Item("SPREDDRAWINGS").SetValue(false);
        }

        public override void CreateConfigMenu()
        {
            Menu combo = new Menu("Combo", "SAutoCarry.Lucian.Combo");
            combo.AddItem(new MenuItem("SAutoCarry.Lucian.Combo.UseQ", "Use Q").SetValue(true));
            combo.AddItem(new MenuItem("SAutoCarry.Lucian.Combo.UseQEx", "Use Extended Q").SetValue(true));
            combo.AddItem(new MenuItem("SAutoCarry.Lucian.Combo.UseW", "Use W").SetValue(true));
            combo.AddItem(new MenuItem("SAutoCarry.Lucian.Combo.UseE", "Use E").SetValue(true)).ValueChanged += (s, ar) => combo.Item("SAutoCarry.Lucian.Combo.EMode").Show(ar.GetNewValue<bool>());
            combo.AddItem(new MenuItem("SAutoCarry.Lucian.Combo.EMode", "E Mode").SetValue(new StringList(new[] { "Auto Pos", "Side Pos", "Cursor Pos" }))).Show(combo.Item("SAutoCarry.Lucian.Combo.UseE").GetValue<bool>());

            Menu harass = new Menu("Harass", "SAutoCarry.Lucian.Harass");
            harass.AddItem(new MenuItem("SAutoCarry.Lucian.Harass.UseQEx", "Use Extended Q").SetValue(true));
            harass.AddItem(new MenuItem("SAutoCarry.Lucian.Harass.Toggle", "Toggle Harass").SetValue(new KeyBind('A', KeyBindType.Toggle)));
            harass.AddItem(new MenuItem("SAutoCarry.Lucian.Harass.MinMana", "Min. Mana %").SetValue(new Slider(0, 40, 100)));

            Menu misc = new Menu("Misc", "SAutoCarry.Lucian.Misc");
            misc.AddItem(new MenuItem("SAutoCarry.Lucian.Misc.CheckPassive", "Check Passive").SetValue(true));
            misc.AddItem(new MenuItem("SAutoCarry.Lucian.Misc.LockR", "Lock R Selected Target").SetValue(new KeyBind('T', KeyBindType.Press)));
            misc.AddItem(new MenuItem("SAutoCarry.Lucian.Misc.AAInd", "Draw AA Indicator").SetValue(true));

            ConfigMenu.AddSubMenu(combo);
            ConfigMenu.AddSubMenu(harass);
            ConfigMenu.AddSubMenu(misc);
            ConfigMenu.AddToMainMenu();
        }

        public override void SetSpells()
        {
            Spells[Q] = new Spell(SpellSlot.Q, 675f);
            Spells[Q].SetTargetted(0.35f, float.MaxValue);

            Spells[W] = new Spell(SpellSlot.W, 1100f);
            Spells[W].SetSkillshot(0.25f, 150f, 1600f, false, SkillshotType.SkillshotLine);

            Spells[E] = new Spell(SpellSlot.E, 425f);

            Spells[R] = new Spell(SpellSlot.R, 1400f);
        }

        public void BeforeDraw()
        {
            if (DrawAAIndicator)
            {
                foreach (var enemy in HeroManager.Enemies)
                {
                    if (enemy.IsValidTarget(1200))
                    {
                        float autoAttackDamage = SCommon.Damage.AutoAttack.GetDamage(enemy) * 1.5f;
                        int aaCount = (int)Math.Ceiling(Math.Max(1, enemy.Health - CalculateComboDamage(enemy, 0)) / autoAttackDamage);
                        Text.DrawText(null, aaCount.ToString() + " x AA + Combo", (int)enemy.HPBarPosition.X, (int)enemy.HPBarPosition.Y, Color.Gold);
                    }
                }
            }
        }

        public void BeforeOrbwalk()
        {
            if (LockR && TargetSelector.SelectedTarget != null && IsUltActive && TargetSelector.SelectedTarget.Path.Length > 0)
                Orbwalker.Orbwalk(null, TargetSelector.SelectedTarget.Path.Last());

            if (HarassToggle && Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.None)
                Harass();
        }

        public void Combo()
        {
            if (CheckPassive && HasPassive)
                return;

            var t = TargetSelector.GetTarget(Spells[Q].Range, LeagueSharp.Common.TargetSelector.DamageType.Physical);
            if (t != null)
            {
                if (Spells[Q].IsReady() && ComboUseQ)
                    Spells[Q].CastOnUnit(t);

                if (!CheckPassive && Spells[W].IsReady())
                    Spells[W].SPredictionCast(t, HitChance.High);
            }
            else
            {
                if (ComboUseQExtended)
                    ExtendedQ();
            }
        }

        public void Harass()
        {
            if (ObjectManager.Player.ManaPercent < HarassManaPercent)
                return;

            if (HarassUseQExtended)
                ExtendedQ();
        }

        public void LaneClear()
        {
            var jungleMob = MinionManager.GetMinions(Spells[Q].Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();
            if (jungleMob != null && !HasPassive)
                Spells[Q].CastOnUnit(jungleMob);
        }

        public void ExtendedQ()
        {
            var t = TargetSelector.GetTarget(1200f, LeagueSharp.Common.TargetSelector.DamageType.Physical);
            if (t != null)
            {
                var enemyHitBox = ClipperWrapper.DefineCircle(SCommon.Prediction.Prediction.GetFastUnitPosition(t, 0.35f), t.BoundingRadius);
                var minions = MinionManager.GetMinions(Spells[Q].Range, MinionTypes.All, MinionTeam.NotAlly);
                foreach (var minion in minions)
                {
                    var spellHitBox = ClipperWrapper.DefineRectangle(ObjectManager.Player.ServerPosition.To2D(), ObjectManager.Player.ServerPosition.To2D() + (minion.ServerPosition.To2D() - ObjectManager.Player.ServerPosition.To2D()).Normalized() * 1200f, 60f);
                    if (ClipperWrapper.IsIntersects(ClipperWrapper.MakePaths(enemyHitBox), ClipperWrapper.MakePaths(spellHitBox)))
                    {
                        Spells[Q].CastOnUnit(minion);
                        return;
                    }
                }
            }
        }

        public Vector3 FindDashPosition(AIHeroClient target)
        {
            if (ComboEMode == 0)
            {
                Vector3 vec = target.ServerPosition;

                if (target.Path.Length > 0)
                {
                    if (ObjectManager.Player.Distance(vec) < ObjectManager.Player.Distance(target.Path.Last()))
                        return IsSafe(target, Game.CursorPos);
                    else
                        return IsSafe(target, Game.CursorPos.To2D().Rotated(LeagueSharp.Common.Geometry.DegreeToRadian((vec - ObjectManager.Player.ServerPosition).To2D().AngleBetween((Game.CursorPos - ObjectManager.Player.ServerPosition).To2D()) % 90)).To3D());
                }
                else
                {
                    if (target.IsMelee)
                        return IsSafe(target, Game.CursorPos);
                }

                return IsSafe(target, ObjectManager.Player.ServerPosition + (target.ServerPosition - ObjectManager.Player.ServerPosition).Normalized().To2D().Rotated(LeagueSharp.Common.Geometry.DegreeToRadian(90 - (vec - ObjectManager.Player.ServerPosition).To2D().AngleBetween((Game.CursorPos - ObjectManager.Player.ServerPosition).To2D()))).To3D() * 300f);
            }
            else if(ComboEMode == 1) //side e idea, credits hoola
            {
                return SCommon.Maths.Geometry.Deviation(ObjectManager.Player.ServerPosition.To2D(), target.ServerPosition.To2D(), 65).To3D();
            }
            else if (ComboEMode == 2)
            {
                return Game.CursorPos;
            }

            return Vector3.Zero;
        }

        public static Vector3 IsSafe(AIHeroClient target, Vector3 vec)
        {
            if (target.ServerPosition.To2D().Distance(vec) <= target.AttackRange && vec.CountEnemiesInRange(1000) > 1)
                return Vector3.Zero;

            if (HeroManager.Enemies.Any(p => p.NetworkId != target.NetworkId && p.ServerPosition.To2D().Distance(vec) <= p.AttackRange) || vec.UnderTurret(true))
                return Vector3.Zero;

            return vec;
        }

        protected override void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.Slot == SpellSlot.Q || args.Slot == SpellSlot.W || args.Slot == SpellSlot.E || args.Slot == SpellSlot.R)
                    HasPassive = true;
            }
        }

        protected override void OrbwalkingEvents_BeforeAttack(SCommon.Orbwalking.BeforeAttackArgs args)
        {
            if (!HasPassive && args.Target != null && args.Target is AIHeroClient && Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.Combo)
            {
                if (Spells[Q].IsReady() && ComboUseQ)
                {
                    Spells[Q].CastOnUnit(args.Target as Obj_AI_Base);
                    args.Process = false;
                }
            }
        }

        protected override void OrbwalkingEvents_AfterAttack(SCommon.Orbwalking.AfterAttackArgs args)
        {
            HasPassive = false;
            if (args.Target != null && args.Target is AIHeroClient && Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.Combo)
            {
                var t = args.Target as AIHeroClient;
                if (Spells[E].IsReady() && ComboUseE)
                {
                    var pos = FindDashPosition(t);
                    if (pos.IsValid())
                    {
                        Spells[E].Cast(pos);
                        return;
                    }
                }

                if (Spells[W].IsReady() && ComboUseW)
                    Spells[W].SPredictionCast(t, HitChance.Low);
            }
            else if (args.Target != null && args.Target is Obj_AI_Base && Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.LaneClear)
            {
                var jungleMob = MinionManager.GetMinions(Spells[Q].Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();
                if (jungleMob != null)
                {   
                    if (Spells[E].IsReady())
                    {
                        Spells[E].Cast(Game.CursorPos);
                        return;
                    }

                    if (Spells[W].IsReady())
                        Spells[W].Cast(jungleMob.ServerPosition);
                }
            }
        }

        private void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {

            if (sender.Owner.IsMe && (args.Slot == SpellSlot.Q || args.Slot == SpellSlot.W || args.Slot == SpellSlot.E || args.Slot == SpellSlot.R))
            {
                if (m_spellCasting)
                    return;

                m_spellCasting = true;
                if (IsUltActive && (args.Slot != SpellSlot.R && args.Slot != SpellSlot.E))
                    args.Process = false;
            }
        }

        private void Spellbook_OnStopCast(Obj_AI_Base sender, SpellbookStopCastEventArgs args)
        {
            if (sender.IsMe)
                m_spellCasting = false;
        }

        public bool IsUltActive
        {
            get { return ObjectManager.Player.HasBuff("LucianR"); }
        }

        public bool HasPassive
        {
            get;
            set;
        }

        public bool ComboUseQ
        {
            get { return ConfigMenu.Item("SAutoCarry.Lucian.Combo.UseQ").GetValue<bool>(); }
        }

        public bool ComboUseQExtended
        {
            get { return ConfigMenu.Item("SAutoCarry.Lucian.Combo.UseQEx").GetValue<bool>(); }
        }

        public bool ComboUseW
        {
            get { return ConfigMenu.Item("SAutoCarry.Lucian.Combo.UseW").GetValue<bool>(); }
        }

        public bool ComboUseE
        {
            get { return ConfigMenu.Item("SAutoCarry.Lucian.Combo.UseE").GetValue<bool>(); }
        }

        public int ComboEMode
        {
            get { return ConfigMenu.Item("SAutoCarry.Lucian.Combo.EMode").GetValue<StringList>().SelectedIndex; }
        }

        public bool HarassUseQExtended
        {
            get { return ConfigMenu.Item("SAutoCarry.Lucian.Harass.UseQEx").GetValue<bool>(); }
        }

        public bool HarassToggle
        {
            get { return ConfigMenu.Item("SAutoCarry.Lucian.Harass.Toggle").GetValue<KeyBind>().Active; }
        }

        public int HarassManaPercent
        {
            get { return ConfigMenu.Item("SAutoCarry.Lucian.Harass.MinMana").GetValue<Slider>().Value; }
        }

        public bool CheckPassive
        {
            get { return ConfigMenu.Item("SAutoCarry.Lucian.Misc.CheckPassive").GetValue<bool>(); }
        }

        public bool LockR
        {
            get { return ConfigMenu.Item("SAutoCarry.Lucian.Misc.LockR").GetValue<KeyBind>().Active; }
        }

        public bool DrawAAIndicator
        {
            get { return ConfigMenu.Item("SAutoCarry.Lucian.Misc.AAInd").GetValue<bool>(); }
        }
    }
}
