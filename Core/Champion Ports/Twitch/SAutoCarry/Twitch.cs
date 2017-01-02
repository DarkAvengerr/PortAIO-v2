using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SCommon;
using SCommon.PluginBase;
using SPrediction;
using SCommon.Orbwalking;
using SCommon.Database;

using SharpDX;
//typedefs
using TargetSelector = SCommon.TS.TargetSelector;
using EloBuddy;

namespace SAutoCarry.Champions
{
    public class Twitch : SCommon.PluginBase.Champion
    {
        public Twitch()
            : base("Twitch", "SAutoCarry - Twitch")
        {
            OnCombo += Combo;
            OnHarass += Harass;
            OnLaneClear += LaneClear;
            OnUpdate += BeforeOrbwalk;
        }

        public override void CreateConfigMenu()
        {
            Menu combo = new Menu("Combo", "SAutoCarry.Twitch.Combo");
            combo.AddItem(new MenuItem("SAutoCarry.Twitch.Combo.UseQ", "Use Q").SetValue(true));
            combo.AddItem(new MenuItem("SAutoCarry.Twitch.Combo.UseW", "Use W").SetValue(true));
            combo.AddItem(new MenuItem("SAutoCarry.Twitch.Combo.UseE", "Use E").SetTooltip("Uses when killable with E or passive stacks = 6").SetValue(true));
            ///
            Menu whitelist = new Menu("E WhiteList", "SAutoCarry.Twitch.Combo.EWhiteList");
            foreach (var enemy in HeroManager.Enemies)
                whitelist.AddItem(new MenuItem("SAutoCarry.Twitch.Combo.EWhiteList." + enemy.ChampionName, "Use E For " + enemy.ChampionName).SetValue(true));
            combo.AddSubMenu(whitelist);
            //
            Menu harass = new Menu("Harass", "SAutoCarry.Twitch.Harass");
            harass.AddItem(new MenuItem("SAutoCarry.Twitch.Harass.UseW", "Use W").SetValue(true));
            harass.AddItem(new MenuItem("SAutoCarry.Twitch.Harass.UseE", "Use E").SetValue(true));


            Menu laneclear = new Menu("LaneClear/JungleClear", "SAutoCarry.Twitch.LaneClear");
            laneclear.AddItem(new MenuItem("SAutoCarry.Twitch.LaneClear.UseW", "Use W").SetValue(true));
            laneclear.AddItem(new MenuItem("SAutoCarry.Twitch.LaneClear.UseE", "Use E").SetValue(true));

            Menu misc = new Menu("Misc", "SAutoCarry.Twitch.Misc");
            misc.AddItem(new MenuItem("SAutoCarry.Twitch.Misc.SaveManaE", "Save Mana for E").SetValue(true));
            misc.AddItem(new MenuItem("SAutoCarry.Twitch.Misc.RecallQ", "Use Q When Recalling").SetValue(new KeyBind('G', KeyBindType.Press)));

            //DamageIndicator.Initialize((t) => (float)CalculateDamageE(t), misc);

            ConfigMenu.AddSubMenu(combo);
            ConfigMenu.AddSubMenu(harass);
            ConfigMenu.AddSubMenu(laneclear);
            ConfigMenu.AddSubMenu(misc);

            ConfigMenu.AddToMainMenu();
        }

        public override void SetSpells()
        {
            Spells[Q] = new Spell(SpellSlot.Q);

            Spells[W] = new Spell(SpellSlot.W, 950f);
            Spells[W].SetSkillshot(0.25f, 100f, 1400f, false, SkillshotType.SkillshotCircle);

            Spells[E] = new Spell(SpellSlot.E, 1200f);

            Spells[R] = new Spell(SpellSlot.R);
        }

        public void Combo()
        {
            if (Spells[W].IsReady() && ComboUseW)
            {
                var t = TargetSelector.GetTarget(Spells[W].Range, LeagueSharp.Common.TargetSelector.DamageType.Physical);
                if (t != null)
                    Spells[W].SPredictionCast(t, HitChance.High);
            }

            if (Spells[E].IsReady() && ComboUseE)
            {
                if (HeroManager.Enemies.Any(x => x.IsValidTarget(Spells[E].Range) && !x.IsInvulnerable && (x.GetBuffCount("twitchdeadlyvenom") >= 6 || Spells[E].IsKillable(x)) && IsWhitelisted(x)))
                    Spells[E].Cast();
            }
        }


        public void Harass()
        {
            if (Spells[W].IsReady() && HarassUseW)
            {
                var t = TargetSelector.GetTarget(Spells[W].Range, LeagueSharp.Common.TargetSelector.DamageType.Physical);
                if (t != null)
                    Spells[W].SPredictionCast(t, HitChance.High);
            }

            if (Spells[E].IsReady() && HarassUseE)
            {
                 if (HeroManager.Enemies.Any(x => x.IsValidTarget(Spells[E].Range) && !x.IsInvulnerable && (x.GetBuffCount("twitchdeadlyvenom") >= 6 || Spells[E].IsKillable(x)) && IsWhitelisted(x)))
                        Spells[E].Cast();
            }
        }

        public void LaneClear()
        {
            var minion = MinionManager.GetMinions(Spells[E].Range, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.MaxHealth).FirstOrDefault();
            if (minion != null)
            {
                if (Spells[W].IsReady() && minion.IsValidTarget(Spells[W].Range) && LaneClearW)
                    Spells[W].Cast(minion.ServerPosition);

                if (Spells[E].IsReady() && LaneClearE)
                {
                    if ((minion.CharData.BaseSkinName.Contains("Dragon") || minion.CharData.BaseSkinName.Contains("Baron") || minion.CharData.BaseSkinName.Contains("Siege") || minion.IsJungleMinion()) && Spells[E].IsKillable(minion))
                        Spells[E].Cast();
                }
            }
        }
        
        public void RecallStealthQ()
        {
            if (Spells[Q].IsReady() && RecallQ && !ObjectManager.Player.IsRecalling())
            {
                Spells[Q].Cast();
                ObjectManager.Player.Spellbook.CastSpell(SpellSlot.Recall);
            }
        }

        public void BeforeOrbwalk()
        {
            if (RecallQ)
                RecallStealthQ();
        }

        public void SaveManaExpunge()
        {
            if (SaveManaE)
            {
                if (ObjectManager.Player.Mana <= Spells[E].ManaCost + Spells[W].ManaCost)
                    return;
            }
        }

        public bool IsWhitelisted(AIHeroClient enemy)
        {
            return ConfigMenu.Item("SAutoCarry.Twitch.Combo.EWhiteList." + enemy.ChampionName).GetValue<bool>();
        }

        protected override void OrbwalkingEvents_BeforeAttack(BeforeAttackArgs args)
        {
            if (Spells[Q].IsReady() && ComboUseQ && args.Target.Type == GameObjectType.AIHeroClient && Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.Combo)
            {
                Spells[Q].Cast();
                args.Process = false;
            }
        }

        public bool ComboUseQ
        {
            get { return ConfigMenu.Item("SAutoCarry.Twitch.Combo.UseQ").GetValue<bool>(); }
        }

        public bool ComboUseW
        {
            get { return ConfigMenu.Item("SAutoCarry.Twitch.Combo.UseW").GetValue<bool>(); }
        }

        public bool ComboUseE
        {
            get { return ConfigMenu.Item("SAutoCarry.Twitch.Combo.UseE").GetValue<bool>(); }
        }

        public bool HarassUseW
        {
            get { return ConfigMenu.Item("SAutoCarry.Twitch.Harass.UseW").GetValue<bool>(); }
        }

        public bool HarassUseE
        {
            get { return ConfigMenu.Item("SAutoCarry.Twitch.Harass.UseE").GetValue<bool>(); }
        }

        public bool LaneClearW
        {
            get { return ConfigMenu.Item("SAutoCarry.Twitch.LaneClear.UseW").GetValue<bool>(); }
        }

        public bool LaneClearE
        {
            get { return ConfigMenu.Item("SAutoCarry.Twitch.LaneClear.UseE").GetValue<bool>(); }
        }
      
        public bool RecallQ
        {
            get { return ConfigMenu.Item("SAutoCarry.Twitch.Misc.RecallQ").GetValue<KeyBind>().Active; }
        }

        public bool SaveManaE
        {
            get { return ConfigMenu.Item("SAutoCarry.Twitch.Misc.SaveManaE").GetValue<bool>(); }
        }
    }
}
