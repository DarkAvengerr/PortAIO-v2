using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using VeigShineCommon;
using SPrediction;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace PlaceboVeigar
{
    public class Veigar : BaseChamp
    {
        public Veigar()
            : base("Veigar")
        {
            
        }

        public override void CreateConfigMenu()
        {
            combo = new Menu("Combo", "Combo");
            combo.AddItem(new MenuItem("CUSEQ", "Use Q").SetValue(true));
            combo.AddItem(new MenuItem("CUSEW", "Use W").SetValue(true));
            combo.AddItem(new MenuItem("CUSEE", "Use E").SetValue(true));
            combo.AddItem(new MenuItem("CMODEE", "E Mode").SetValue(new StringList(new string[] { "Stun enemy", "Zone enemy" })));
            combo.AddItem(new MenuItem("CUSER", "Use R").SetValue(true));

            harass = new Menu("Harass", "Harass");
            harass.AddItem(new MenuItem("HUSEQ", "Use Q").SetValue(true));
            harass.AddItem(new MenuItem("HUSEW", "Use W").SetValue(true));
            harass.AddItem(new MenuItem("HUSEE", "Use E").SetValue(true));
            harass.AddItem(new MenuItem("HMODEE", "E Mode").SetValue(new StringList(new string[] { "Stun enemy", "Zone enemy" })));
            harass.AddItem(new MenuItem("HMANA", "Min. Mana Percent").SetValue(new Slider(60, 0, 100)));

            laneclear = new Menu("Lane/Jungle Clear", "LaneClear");
            laneclear.AddItem(new MenuItem("LUSEQ", "Use Q").SetValue(true));
            laneclear.AddItem(new MenuItem("LUSEW", "Use W").SetValue(true));
            laneclear.AddItem(new MenuItem("LMINW", "Min. Minions To W In Range").SetValue(new Slider(4, 1, 12)));
            laneclear.AddItem(new MenuItem("LMANA", "Min. Mana Percent").SetValue(new Slider(10, 0, 100)));

            misc = new Menu("Misc", "Misc");
            misc.AddItem(new MenuItem("MAUTOQLH", "Auto Q Last Hit").SetValue(new KeyBind('T', KeyBindType.Toggle)));
            misc.AddItem(new MenuItem("MANTIGAPE", "Anti Gap Closer With E").SetValue(true));
            misc.AddItem(new MenuItem("MAUTOWIMMO", "Auto W Immobile Target").SetValue(true));
            //auto r
            Menu autoUlt = new Menu("Auto Ult Settings (Killable)", "AutoR");
            foreach (AIHeroClient enemy in HeroManager.Enemies)
                autoUlt.AddItem(new MenuItem("noautor" + enemy.ChampionName, string.Format("Dont Auto Ult {0}", enemy.ChampionName)).SetValue(false));
            autoUlt.AddItem(new MenuItem("autorenable", "Enabled").SetValue(true));
            //
            misc.AddSubMenu(autoUlt);

            Config.AddSubMenu(combo);
            Config.AddSubMenu(harass);
            Config.AddSubMenu(laneclear);
            Config.AddSubMenu(misc);
            Config.AddToMainMenu();

            DamageIndicator.DamageToUnit = (t) => (float)CalculateComboDamage(t);

            BeforeOrbWalking += BeforeOrbwalk;
            OrbwalkingFunctions[OrbwalkingComboMode] += Combo;
            OrbwalkingFunctions[OrbwalkingHarassMode] += Harass;
            OrbwalkingFunctions[OrbwalkingLaneClearMode] += LaneClear;
        }

        public override void SetSpells()
        {
            Spells[Q] = new Spell(SpellSlot.Q, 650 + 150);
            Spells[Q].SetSkillshot(0.25f, 70f, 2000f, false, SkillshotType.SkillshotLine);

            Spells[W] = new Spell(SpellSlot.W, 900 + 150);
            Spells[W].SetSkillshot(1.25f, 200f, 0, false, SkillshotType.SkillshotCircle);

            Spells[E] = new Spell(SpellSlot.E, 700, TargetSelector.DamageType.Magical);
            Spells[E].SetSkillshot(0.5f, 350f, 0, false, SkillshotType.SkillshotCircle);

            Spells[R] = new Spell(SpellSlot.R, 650);
        }

        public void BeforeOrbwalk()
        {
            bool autoQ = Config.Item("MAUTOQLH").GetValue<KeyBind>().Active;
            bool autoW = Config.Item("MAUTOWIMMO").GetValue<bool>();
            bool autoR = Config.Item("autorenable").GetValue<bool>();
            
            foreach (var enemy in HeroManager.Enemies)
            {
                if (enemy.IsValidTarget(Spells[W].Range - 150) && ShineCommon.Utility.IsImmobileTarget(enemy) && autoW)
                    Spells[W].Cast(enemy.ServerPosition);

                if (enemy.IsValidTarget(Spells[R].Range) && autoR && !Config.Item("noautor" + enemy.ChampionName).GetValue<bool>())
                {
                    if (CalculateDamageR(enemy) >= enemy.Health)
                        Spells[R].CastOnUnit(enemy);
                }
            }

            if (autoQ && OrbwalkingActiveMode != OrbwalkingComboMode)
                StackQ();
        }

        public void Combo()
        {
            bool waitforE = false;
            if (Spells[E].IsReady() && Config.Item("CUSEE").GetValue<bool>())
            {
                var t = Target.Get(1000);
                if (t != null)
                {
                    Spells[E].SPredictionCastRing(t, 80, HitChance.High, Config.Item("CMODEE").GetValue<StringList>().SelectedIndex == 0);
                    waitforE = true;
                }
            }

            if (Spells[W].IsReady() && Config.Item("CUSEW").GetValue<bool>() && !waitforE)
            {
                var t = Target.Get(Spells[W].Range, true);
                if (t != null)
                    Spells[W].SPredictionCast(t, HitChance.High);
            }

            if (Spells[R].IsReady() && Config.Item("CUSER").GetValue<bool>())
            {
                var t = Target.Get(Spells[R].Range, true);
                if (t != null && CalculateComboDamage(t, 4) >= t.Health)
                    Spells[R].CastOnUnit(t);
            }

            if (Spells[Q].IsReady() && Config.Item("CUSEQ").GetValue<bool>())
            {
                var t = Target.Get(Spells[Q].Range, true);
                if (t != null)
                    Spells[Q].SPredictionCast(t, HitChance.High);
            }
        }

        public void Harass()
        {
            if (ObjectManager.Player.ManaPercent < Config.Item("HMANA").GetValue<Slider>().Value)
                return;

            bool waitforE = false;

            if (Spells[E].IsReady() && Config.Item("HUSEE").GetValue<bool>())
            {
                var t = Target.Get(1000);
                if (t != null)
                {
                    Spells[E].SPredictionCastRing(t, 80, HitChance.High, Config.Item("HMODEE").GetValue<StringList>().SelectedIndex == 0);
                    waitforE = true;
                }
            }

            if (Spells[W].IsReady() && Config.Item("HUSEW").GetValue<bool>() && !waitforE)
            {
                var t = Target.Get(Spells[W].Range, true);
                if (t != null)
                    Spells[W].SPredictionCast(t, HitChance.High);
            }

            if (Spells[Q].IsReady() && Config.Item("HUSEQ").GetValue<bool>())
            {
                var t = Target.Get(Spells[Q].Range, true);
                if (t != null)
                    Spells[Q].SPredictionCast(t, HitChance.High);
            }
        }

        public void LaneClear()
        {
            if (ObjectManager.Player.ManaPercent < Config.Item("LMANA").GetValue<Slider>().Value)
                return;

            if (Config.Item("LUSEQ").GetValue<bool>())
                StackQ();

            if (Config.Item("LUSEW").GetValue<bool>())
            {
                var farmLocation = MinionManager.GetBestCircularFarmLocation(MinionManager.GetMinions(Spells[W].Range, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.MaxHealth).Select(q => q.ServerPosition.To2D()).ToList(), Spells[W].Width, Spells[W].Range);
                if (farmLocation.MinionsHit >= Config.Item("LMINW").GetValue<Slider>().Value)
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

        public override void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Config.Item("MANTIGAPE").GetValue<bool>())
            {
                if (gapcloser.End.Distance(ObjectManager.Player.ServerPosition) < 350)
                    Spells[E].Cast(ObjectManager.Player.ServerPosition);
            }
        }

        public override double CalculateDamageR(AIHeroClient target)
        {
            if(!Spells[R].IsReady())
                return 0;

            return ObjectManager.Player.CalcDamage(target, Damage.DamageType.Magical, new int[] { 250, 375, 500 }[Spells[R].Level - 1] + ObjectManager.Player.FlatMagicDamageMod + target.FlatMagicDamageMod * 0.8);
        }
    }
}
