using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DZAIO_Reborn.Core;
using DZAIO_Reborn.Helpers;
using DZAIO_Reborn.Helpers.Entity;
using DZAIO_Reborn.Helpers.Modules;
using DZAIO_Reborn.Plugins.Champions.Ezreal.Modules;
using DZAIO_Reborn.Plugins.Champions.Sivir.Modules;
using DZAIO_Reborn.Plugins.Champions.Veigar.Modules;
using DZAIO_Reborn.Plugins.Interface;
using DZLib.Core;
using DZLib.Menu;
using DZLib.MenuExtensions;
using DZLib.Positioning;
using LeagueSharp;
using LeagueSharp.Common;
using SPrediction;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace DZAIO_Reborn.Plugins.Champions.Ezreal
{
    class Ezreal : IChampion
    {
        public void OnLoad(Menu menu)
        {
            var comboMenu = new Menu(ObjectManager.Player.ChampionName + ": Combo", "dzaio.champion.ezreal.combo");
            {
                comboMenu.AddModeMenu(ModesMenuExtensions.Mode.Combo, new[] { SpellSlot.Q, SpellSlot.W, SpellSlot.R }, new[] { true, true, true });
                comboMenu.AddSlider("dzaio.champion.ezreal.combo.r.min", "Min Enemies for R", 2, 1, 5);
                menu.AddSubMenu(comboMenu);
            }

            var mixedMenu = new Menu(ObjectManager.Player.ChampionName + ": Mixed", "dzaio.champion.ezreal.harrass");
            {
                mixedMenu.AddModeMenu(ModesMenuExtensions.Mode.Harrass, new[] { SpellSlot.Q, SpellSlot.W }, new[] { true, true });
                mixedMenu.AddSlider("dzaio.champion.ezreal.mixed.mana", "Min Mana % for Harass", 30, 0, 100);
                menu.AddSubMenu(mixedMenu);
            }

            var farmMenu = new Menu(ObjectManager.Player.ChampionName + ": Farm", "dzaio.champion.ezreal.farm");
            {
                farmMenu.AddModeMenu(ModesMenuExtensions.Mode.Laneclear, new[] { SpellSlot.Q }, new[] { true });
                farmMenu.AddSlider("dzaio.champion.ezreal.farm.mana", "Min Mana % for Farm", 30, 0, 100);
                menu.AddSubMenu(farmMenu);
            }

            var extraMenu = new Menu(ObjectManager.Player.ChampionName + ": Extra", "dzaio.champion.ezreal.extra");
            {
                extraMenu.AddBool("dzaio.champion.ezreal.extra.antigapcloser", "E Antigapcloser", true);
                extraMenu.AddSlider("dzaio.champion.ezreal.extra.e.antigpdelay", "E Antigapcloser Delay", 120, 0, 350);
                extraMenu.AddBool("dzaio.champion.ezreal.extra.autoQKS", "Q KS", true);
                extraMenu.AddBool("dzaio.champion.ezreal.extra.autoQRoot", "Q Root/Slow/Dash", false);
                extraMenu.AddBool("dzaio.champion.ezreal.extra.autoRKS", "R KS", true);
            }

            Variables.Spells[SpellSlot.Q].SetSkillshot(0.25f, 60f, 2000f, true, SkillshotType.SkillshotLine);
            Variables.Spells[SpellSlot.W].SetSkillshot(0.25f, 80f, 1600f, false, SkillshotType.SkillshotLine);
            Variables.Spells[SpellSlot.R].SetSkillshot(1f, 160f, 2000f, false, SkillshotType.SkillshotLine);

        }

        public void RegisterEvents()
        {
            DZInterrupter.OnInterruptableTarget += OnInterrupter;
            DZAntigapcloser.OnEnemyGapcloser += OnGapcloser;
            Orbwalking.AfterAttack += AfterAttack;
        }


        private void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (!unit.IsMe || !unit.IsValid<Obj_AI_Base>())
            {
                return;
            }

            if (Variables.Spells[SpellSlot.Q].IsEnabledAndReady(ModesMenuExtensions.Mode.Combo)
                || (Variables.Spells[SpellSlot.Q].IsEnabledAndReady(ModesMenuExtensions.Mode.Harrass)
                && ObjectManager.Player.ManaPercent >= Variables.AssemblyMenu.GetItemValue<Slider>("dzaio.champion.extra.mixed.mana").Value))
            {
                if (target.IsValid<AIHeroClient>() && target.IsValidTarget())
                {
                    Variables.Spells[SpellSlot.Q].CastIfHitchanceEquals(target as AIHeroClient, HitChance.High);
                }
            }

            if (Variables.Spells[SpellSlot.W].IsEnabledAndReady(ModesMenuExtensions.Mode.Combo)
                || (Variables.Spells[SpellSlot.W].IsEnabledAndReady(ModesMenuExtensions.Mode.Harrass)
                && ObjectManager.Player.ManaPercent >= Variables.AssemblyMenu.GetItemValue<Slider>("dzaio.champion.extra.mixed.mana").Value))
            {
                if (target.IsValid<AIHeroClient>() && target.IsValidTarget())
                {
                    Variables.Spells[SpellSlot.W].CastIfHitchanceEquals(target as AIHeroClient, HitChance.High);
                }
            }
        }

        private void OnGapcloser(DZLib.Core.ActiveGapcloser gapcloser)
        {
            if (!Variables.AssemblyMenu.GetItemValue<bool>("dzaio.champion.ezreal.extra.antigapcloser"))
            {
                return;
            }

            if (!gapcloser.Sender.IsValidTarget() ||
                !(gapcloser.End.Distance(ObjectManager.Player.ServerPosition) <= 350f))
            {
                return;
            }
            LeagueSharp.Common.Utility.DelayAction.Add(Variables.AssemblyMenu.GetItemValue<Slider>("dzaio.champion.ezreal.extra.e.antigpdelay").Value,
                () =>
                {
                    var extendedPosition = ObjectManager.Player.ServerPosition.Extend(Game.CursorPos, Variables.Spells[SpellSlot.E].Range);

                    if (extendedPosition.IsSafe(Variables.Spells[SpellSlot.E].Range) &&
                        extendedPosition.CountAlliesInRange(650f) >= 0)
                    {
                        Variables.Spells[SpellSlot.E].Cast(extendedPosition);
                    }
                });
            
        }

        private void OnInterrupter(AIHeroClient sender, DZInterrupter.InterruptableTargetEventArgs args)
        {

        }
        public Dictionary<SpellSlot, Spell> GetSpells()
        {
            return new Dictionary<SpellSlot, Spell>
                      {
                                    { SpellSlot.Q, new Spell(SpellSlot.Q, 1180f) },
                                    { SpellSlot.W, new Spell(SpellSlot.W, 850f) },
                                    { SpellSlot.E, new Spell(SpellSlot.E, 475f) },
                                    { SpellSlot.R, new Spell(SpellSlot.R, 2500f) }
                      };
        }

        public List<IModule> GetModules()
        {
            return new List<IModule>()
            {
                new EzrealQKS(),
                new EzrealQRoot(),
                new EzrealRKS()
            };
        }

        public void OnTick()
        {

        }

        public void OnCombo()
        {
            if (Variables.Spells[SpellSlot.Q].IsEnabledAndReady(ModesMenuExtensions.Mode.Combo))
            {
                var qTarget = Variables.Spells[SpellSlot.Q].GetTarget();
                if (qTarget.IsValidTarget())
                {
                    var qPrediction = Variables.Spells[SpellSlot.Q].GetPrediction(qTarget);
                    if (qPrediction.Hitchance >= HitChance.High)
                    {
                        Variables.Spells[SpellSlot.Q].Cast(qPrediction.CastPosition);
                    }
                }
            }

            if (Variables.Spells[SpellSlot.W].IsEnabledAndReady(ModesMenuExtensions.Mode.Combo) && ObjectManager.Player.ManaPercent > 45)
            {
                var wTarget = Variables.Spells[SpellSlot.W].GetTarget();
                if (wTarget.IsValidTarget())
                {
                    Variables.Spells[SpellSlot.W].CastIfHitchanceEquals(wTarget, HitChance.High);
                }
            }

            if (Variables.Spells[SpellSlot.R].IsEnabledAndReady(ModesMenuExtensions.Mode.Combo))
            {
                var target = TargetSelector.GetTarget(2300f, TargetSelector.DamageType.Physical);

                if (target.IsValidTarget(Variables.Spells[SpellSlot.R].Range)
                    && ObjectManager.Player.Distance(target) >= Orbwalking.GetRealAutoAttackRange(null) * 0.85f
                    && !(target.Health + 5 <
                      ObjectManager.Player.GetAutoAttackDamage(target) * 2f +
                      Variables.Spells[SpellSlot.Q].GetDamage(target))
                    && HeroManager.Enemies.Count(m => m.Distance(target.ServerPosition) < 200f) >= Variables.AssemblyMenu.Item("dzaio.champion.ezreal.combo.r.min").GetValue<Slider>().Value
                    && CanExecuteTarget(target))
                {
                    Variables.Spells[SpellSlot.R].CastIfHitchanceEquals(
                        target, target.IsMoving ? HitChance.VeryHigh : HitChance.High);
                }
            }
        }

        public void OnMixed()
        {
            if (ObjectManager.Player.ManaPercent <
                Variables.AssemblyMenu.GetItemValue<Slider>("dzaio.champion.ezreal.mixed.mana").Value)
            {
                return;
            }

            if (Variables.Spells[SpellSlot.Q].IsEnabledAndReady(ModesMenuExtensions.Mode.Harrass))
            {
                var qTarget = Variables.Spells[SpellSlot.Q].GetTarget();
                if (qTarget.IsValidTarget())
                {
                    var qPrediction = Variables.Spells[SpellSlot.Q].GetPrediction(qTarget);
                    if (qPrediction.Hitchance >= HitChance.High)
                    {
                        Variables.Spells[SpellSlot.Q].Cast(qPrediction.CastPosition);
                    }
                }
            }

            if (Variables.Spells[SpellSlot.W].IsEnabledAndReady(ModesMenuExtensions.Mode.Harrass) && ObjectManager.Player.ManaPercent > 45)
            {
                var wTarget = Variables.Spells[SpellSlot.W].GetTarget();
                if (wTarget.IsValidTarget())
                {
                    Variables.Spells[SpellSlot.W].CastIfHitchanceEquals(wTarget, HitChance.High);
                }
            }
        }

        public void OnLastHit()
        { }

        public void OnLaneclear()
        {
            if (ObjectManager.Player.ManaPercent <
                Variables.AssemblyMenu.GetItemValue<Slider>("dzaio.champion.ezreal.farm.mana").Value)
            {
                return;
            }

        }

        private bool CanExecuteTarget(Obj_AI_Base target)
        {
            double damage = 1f;

            var prediction = Variables.Spells[SpellSlot.R].GetPrediction(target);
            var count = prediction.CollisionObjects.Count;

            damage += ObjectManager.Player.GetSpellDamage(target, SpellSlot.R);

            if (count >= 7)
            {
                damage = damage * .3f;
            }
            else if (count != 0)
            {
                damage = damage * (10 - count / 10f);
            }

            return damage >= target.Health + 10f;
        }
    }
}
