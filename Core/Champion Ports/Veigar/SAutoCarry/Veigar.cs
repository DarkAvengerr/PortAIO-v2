using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SCommon;
using SCommon.Maths;
using SCommon.Database;
using SCommon.PluginBase;
using SPrediction;
using SUtility.Drawings;
using SharpDX;
//typedefs
using TargetSelector = SCommon.TS.TargetSelector;
using EloBuddy;

namespace SAutoCarry.Champions
{
    public class Veigar : SCommon.PluginBase.Champion
    {
        public Veigar()
            : base("Veigar", "SAutoCarry - Veigar")
        {
            OnUpdate += BeforeOrbwalk;
            OnCombo += Combo;
            OnHarass += Harass;
            OnLaneClear += LaneClear;
        }

        public override void CreateConfigMenu()
        {
            Menu combo = new Menu("Combo", "SAutoCarry.Veigar.Combo");
            combo.AddItem(new MenuItem("SAutoCarry.Veigar.Combo.UseQ", "Use Q").SetValue(true));
            combo.AddItem(new MenuItem("SAutoCarry.Veigar.Combo.UseW", "Use W").SetValue(true));
            combo.AddItem(new MenuItem("SAutoCarry.Veigar.Combo.UseE", "Use E").SetValue(true));
            combo.AddItem(new MenuItem("SAutoCarry.Veigar.Combo.EMode", "E Mode").SetValue(new StringList(new string[] { "Stun enemy", "Zone enemy" })));
            combo.AddItem(new MenuItem("SAutoCarry.Veigar.Combo.UseR", "Use R").SetValue(true));

            Menu harass = new Menu("Harass", "SAutoCarry.Veigar.Harass");
            harass.AddItem(new MenuItem("SAutoCarry.Veigar.Harass.UseQ", "Use Q").SetValue(true));
            harass.AddItem(new MenuItem("SAutoCarry.Veigar.Harass.UseW", "Use W").SetValue(true));
            harass.AddItem(new MenuItem("SAutoCarry.Veigar.Harass.UseE", "Use E").SetValue(true));
            harass.AddItem(new MenuItem("SAutoCarry.Veigar.Harass.EMode", "E Mode").SetValue(new StringList(new string[] { "Stun enemy", "Zone enemy" })));
            harass.AddItem(new MenuItem("SAutoCarry.Veigar.Harass.Mana", "Min. Mana Percent").SetValue(new Slider(60, 0, 100)));

            Menu laneclear = new Menu("Lane/Jungle Clear", "SAutoCarry.Veigar.LaneClear");
            laneclear.AddItem(new MenuItem("SAutoCarry.Veigar.LaneClear.UseQ", "Use Q").SetValue(true));
            laneclear.AddItem(new MenuItem("SAutoCarry.Veigar.LaneClear.UseW", "Use W").SetValue(true));
            laneclear.AddItem(new MenuItem("SAutoCarry.Veigar.LaneClear.MinW", "Min. Minions To W In Range").SetValue(new Slider(4, 1, 12)));
            laneclear.AddItem(new MenuItem("SAutoCarry.Veigar.LaneClear.Mana", "Min. Mana Percent").SetValue(new Slider(10, 0, 100)));

            Menu misc = new Menu("Misc", "SAutoCarry.Veigar.Misc");
            misc.AddItem(new MenuItem("SAutoCarry.Veigar.Misc.AutoQLastHit", "Auto Q Last Hit").SetValue(new KeyBind('T', KeyBindType.Toggle)));
            misc.AddItem(new MenuItem("SAutoCarry.Veigar.Misc.AntiGapcloseE", "Anti Gap Closer With E").SetValue(true));
            misc.AddItem(new MenuItem("SAutoCarry.Veigar.Misc.AutoWImmobile", "Auto W Immobile Target").SetValue(true));
            //auto r
            Menu autoUlt = new Menu("Auto Ult Settings (Killable)", "SAutoCarry.Veigar.AutoR");
            foreach (AIHeroClient enemy in HeroManager.Enemies)
                autoUlt.AddItem(new MenuItem("SAutoCarry.Veigar.AutoR.DontUlt" + enemy.ChampionName, string.Format("Dont Auto Ult {0}", enemy.ChampionName)).SetValue(false));
            autoUlt.AddItem(new MenuItem("SAutoCarry.Veigar.AutoR.Enabled", "Enabled").SetValue(true));
            //
            misc.AddSubMenu(autoUlt);

            ConfigMenu.AddSubMenu(combo);
            ConfigMenu.AddSubMenu(harass);
            ConfigMenu.AddSubMenu(laneclear);
            ConfigMenu.AddSubMenu(misc);
            ConfigMenu.AddToMainMenu();

            DamageIndicator.Initialize((t) => (float)CalculateComboDamage(t), misc);
        }

        public override void SetSpells()
        {
            Spells[Q] = new Spell(SpellSlot.Q, 650 + 150);
            Spells[Q].SetSkillshot(0.25f, 70f, 2000f, false, SkillshotType.SkillshotLine);

            Spells[W] = new Spell(SpellSlot.W, 900 + 150);
            Spells[W].SetSkillshot(1.25f, 200f, 0, false, SkillshotType.SkillshotCircle);

            Spells[E] = new Spell(SpellSlot.E, 700);
            Spells[E].SetSkillshot(0.5f, 350f, 0, false, SkillshotType.SkillshotCircle);

            Spells[R] = new Spell(SpellSlot.R, 650);
        }

        public void BeforeOrbwalk()
        {
            bool autoQ = ConfigMenu.Item("SAutoCarry.Veigar.Misc.AutoQLastHit").GetValue<KeyBind>().Active;
            bool autoW = ConfigMenu.Item("SAutoCarry.Veigar.Misc.AutoWImmobile").GetValue<bool>();
            bool autoR = ConfigMenu.Item("SAutoCarry.Veigar.AutoR.Enabled").GetValue<bool>();

            foreach (var enemy in HeroManager.Enemies)
            {
                if (enemy.IsValidTarget(Spells[W].Range - 150) && enemy.IsImmobilized() && autoW)
                    Spells[W].Cast(enemy.ServerPosition);

                if (enemy.IsValidTarget(Spells[R].Range) && autoR && !ConfigMenu.Item("SAutoCarry.Veigar.AutoR.DontUlt" + enemy.ChampionName).GetValue<bool>())
                {
                    if (CalculateDamageR(enemy) >= enemy.Health)
                        Spells[R].CastOnUnit(enemy);
                }
            }

            if (autoQ && Orbwalker.ActiveMode != SCommon.Orbwalking.Orbwalker.Mode.Combo)
                StackQ();
        }

        public void Combo()
        {
            bool waitforE = false;
            if (Spells[E].IsReady() && ConfigMenu.Item("SAutoCarry.Veigar.Combo.UseE").GetValue<bool>())
            {
                var t = TargetSelector.GetTarget(1000, LeagueSharp.Common.TargetSelector.DamageType.Magical);
                if (t != null)
                {
                    Spells[E].SPredictionCastRing(t, 80, HitChance.High, ConfigMenu.Item("SAutoCarry.Veigar.Combo.EMode").GetValue<StringList>().SelectedIndex == 0);
                    waitforE = true;
                }
            }

            if (Spells[W].IsReady() && ConfigMenu.Item("SAutoCarry.Veigar.Combo.UseW").GetValue<bool>() && !waitforE)
            {
                var t = TargetSelector.GetTarget(Spells[W].Range, LeagueSharp.Common.TargetSelector.DamageType.Magical);
                if (t != null)
                    Spells[W].SPredictionCast(t, HitChance.High);
            }

            if (Spells[R].IsReady() && ConfigMenu.Item("SAutoCarry.Veigar.Combo.UseR").GetValue<bool>())
            {
                var t = TargetSelector.GetTarget(Spells[R].Range, LeagueSharp.Common.TargetSelector.DamageType.Magical);
                if (t != null && CalculateComboDamage(t, 4) >= t.Health)
                    Spells[R].CastOnUnit(t);
            }

            if (Spells[Q].IsReady() && ConfigMenu.Item("SAutoCarry.Veigar.Combo.UseQ").GetValue<bool>())
            {
                var t = TargetSelector.GetTarget(Spells[Q].Range, LeagueSharp.Common.TargetSelector.DamageType.Magical);
                if (t != null)
                    Spells[Q].SPredictionCast(t, HitChance.High);
            }
        }

        public void Harass()
        {
            if (ObjectManager.Player.ManaPercent < ConfigMenu.Item("SAutoCarry.Veigar.Harass.Mana").GetValue<Slider>().Value)
                return;

            bool waitforE = false;

            if (Spells[E].IsReady() && ConfigMenu.Item("SAutoCarry.Veigar.Harass.UseE").GetValue<bool>())
            {
                var t = TargetSelector.GetTarget(1000, LeagueSharp.Common.TargetSelector.DamageType.Magical);
                if (t != null)
                {
                    Spells[E].SPredictionCastRing(t, 80, HitChance.High, ConfigMenu.Item("SAutoCarry.Veigar.Harass.EMode").GetValue<StringList>().SelectedIndex == 0);
                    waitforE = true;
                }
            }

            if (Spells[W].IsReady() && ConfigMenu.Item("SAutoCarry.Veigar.Harass.UseW").GetValue<bool>() && !waitforE)
            {
                var t = TargetSelector.GetTarget(Spells[W].Range, LeagueSharp.Common.TargetSelector.DamageType.Magical);
                if (t != null)
                    Spells[W].SPredictionCast(t, HitChance.High);
            }

            if (Spells[Q].IsReady() && ConfigMenu.Item("SAutoCarry.Veigar.Harass.UseQ").GetValue<bool>())
            {
                var t = TargetSelector.GetTarget(Spells[Q].Range, LeagueSharp.Common.TargetSelector.DamageType.Magical);
                if (t != null)
                    Spells[Q].SPredictionCast(t, HitChance.High);
            }
        }

        public void LaneClear()
        {
            if (ObjectManager.Player.ManaPercent < ConfigMenu.Item("SAutoCarry.Veigar.LaneClear.Mana").GetValue<Slider>().Value)
                return;

            if (ConfigMenu.Item("SAutoCarry.Veigar.LaneClear.UseQ").GetValue<bool>())
                StackQ();

            if (ConfigMenu.Item("SAutoCarry.Veigar.LaneClear.UseW").GetValue<bool>())
            {
                var farmLocation = MinionManager.GetBestCircularFarmLocation(MinionManager.GetMinions(Spells[W].Range, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.MaxHealth).Select(q => q.ServerPosition.To2D()).ToList(), Spells[W].Width, Spells[W].Range);
                if (farmLocation.MinionsHit >= ConfigMenu.Item("SAutoCarry.Veigar.LaneClear.MinW").GetValue<Slider>().Value)
                    Spells[W].Cast(farmLocation.Position);
            }
        }

        private void StackQ()
        {
            if (Spells[Q].IsReady())
            {
                var farmLocation = MinionManager.GetBestLineFarmLocation(MinionManager.GetMinions(Spells[Q].Range, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.MaxHealth).Where(p => ObjectManager.Player.GetSpellDamage(p, SpellSlot.Q) >= p.Health).Select(q => q.ServerPosition.To2D()).ToList(), Spells[Q].Width, Spells[Q].Range);
                if (farmLocation.MinionsHit > 0)
                    Spells[Q].Cast(farmLocation.Position);
            }
        }

        protected override void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (ConfigMenu.Item("SAutoCarry.Veigar.Misc.AntiGapcloseE").GetValue<bool>())
            {
                if (gapcloser.End.Distance(ObjectManager.Player.ServerPosition) < 350)
                    Spells[E].Cast(ObjectManager.Player.ServerPosition);
            }
        }

        public override double CalculateDamageR(AIHeroClient target)
        {
            if (!Spells[R].IsReady())
                return 0;

            return ObjectManager.Player.CalcDamage(target, Damage.DamageType.Magical, new int[] { 250, 375, 500 }[Spells[R].Level - 1] + ObjectManager.Player.FlatMagicDamageMod + target.FlatMagicDamageMod * 0.8);
        }
    }
}
