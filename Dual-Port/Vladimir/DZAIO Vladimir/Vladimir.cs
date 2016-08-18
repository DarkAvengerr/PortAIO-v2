using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DZAIO_Reborn.Core;
using DZAIO_Reborn.Helpers;
using DZAIO_Reborn.Helpers.Entity;
using DZAIO_Reborn.Helpers.Modules;
using DZAIO_Reborn.Plugins.Champions.Vladimir.Modules;
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
 namespace DZAIO_Reborn.Plugins.Champions.Vladimir
{
    class Vladimir : IChampion
    {
        public void OnLoad(Menu menu)
        {
            var comboMenu = new Menu(ObjectManager.Player.ChampionName + ": Combo", "dzaio.champion.vladimir.combo");
            {
                comboMenu.AddModeMenu(ModesMenuExtensions.Mode.Combo, new[] { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R }, new[] { true, true, true, true });
                comboMenu.AddSlider("dzaio.champion.vladimir.combo.r.min", "Min Enemies for R", 2, 1, 5);
                menu.AddSubMenu(comboMenu);
            }

            var mixedMenu = new Menu(ObjectManager.Player.ChampionName + ": Mixed", "dzaio.champion.vladimir.harrass");
            {
                mixedMenu.AddModeMenu(ModesMenuExtensions.Mode.Harrass, new[] { SpellSlot.Q, SpellSlot.E }, new[] { true, true });
                mixedMenu.AddSlider("dzaio.champion.vladimir.mixed.mana", "Min Mana % for Harass", 30, 0, 100);
                menu.AddSubMenu(mixedMenu);
            }

            var farmMenu = new Menu(ObjectManager.Player.ChampionName + ": Farm", "dzaio.champion.vladimir.farm");
            {
                farmMenu.AddModeMenu(ModesMenuExtensions.Mode.Laneclear, new[] { SpellSlot.Q }, new[] { true });
                farmMenu.AddSlider("dzaio.champion.vladimir.farm.mana", "Min Mana % for Farm", 30, 0, 100);
                menu.AddSubMenu(farmMenu);
            }

            var extraMenu = new Menu(ObjectManager.Player.ChampionName + ": Extra", "dzaio.champion.vladimir.extra");
            {
                extraMenu.AddBool("dzaio.champion.vladimir.extra.antigapcloser", "W Antigapcloser (Important Gapclosers)", true);
                extraMenu.AddSlider("dzaio.champion.vladimir.extra.w.antigpdelay", "W Antigapcloser Delay", 120, 0, 350);
                extraMenu.AddBool("dzaio.champion.vladimir.extra.autoQKS", "Q KS", true);
            }

            Variables.Spells[SpellSlot.Q].SetTargetted(0.25f, 2000f);
            Variables.Spells[SpellSlot.R].SetSkillshot(0.25f, 175, 700, false, SkillshotType.SkillshotCircle);

        }

        public void RegisterEvents()
        {
            DZInterrupter.OnInterruptableTarget += OnInterrupter;
            DZAntigapcloser.OnEnemyGapcloser += OnGapcloser;
            Orbwalking.AfterAttack += AfterAttack;
        }


        private void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            
        }

        private void OnGapcloser(DZLib.Core.ActiveGapcloser gapcloser)
        {
            if (!Variables.AssemblyMenu.GetItemValue<bool>("dzaio.champion.vladimir.extra.antigapcloser"))
            {
                return;
            }

            if (!gapcloser.Sender.IsValidTarget() ||
                !(gapcloser.End.Distance(ObjectManager.Player.ServerPosition) <= 350f) || !(ObjectManager.Player.HealthPercent > 25))
            {
                return;
            }
            LeagueSharp.Common.Utility.DelayAction.Add(Variables.AssemblyMenu.GetItemValue<Slider>("dzaio.champion.vladimir.extra.w.antigpdelay").Value,
                () =>
                {
                    var extendedPosition = ObjectManager.Player.ServerPosition.Extend(Game.CursorPos, Variables.Spells[SpellSlot.E].Range);
                    Variables.Spells[SpellSlot.W].Cast(extendedPosition);
                });
        }

        private void OnInterrupter(AIHeroClient sender, DZInterrupter.InterruptableTargetEventArgs args)
        {

        }
        public Dictionary<SpellSlot, Spell> GetSpells()
        {
            return new Dictionary<SpellSlot, Spell>
                      {
                                    { SpellSlot.Q, new Spell(SpellSlot.Q, 600f) },
                                    { SpellSlot.W, new Spell(SpellSlot.W) },
                                    { SpellSlot.E, new Spell(SpellSlot.E, 600f) },
                                    { SpellSlot.R, new Spell(SpellSlot.R, 680f) }
                      };
        }

        public List<IModule> GetModules()
        {
            return new List<IModule>()
            {
                new VladimirQKS()
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
                        Variables.Spells[SpellSlot.Q].CastOnUnit(qTarget);
                }
            }

            if (Variables.Spells[SpellSlot.E].IsEnabledAndReady(ModesMenuExtensions.Mode.Combo))
            {
                var eTarget = Variables.Spells[SpellSlot.E].GetTarget();
                if (eTarget.IsValidTarget() && ObjectManager.Player.Distance(eTarget) >= Variables.Spells[SpellSlot.E].Range / 2f)
                {
                    Variables.Spells[SpellSlot.E].CastOnUnit(eTarget);
                }
            }

            if (Variables.Spells[SpellSlot.R].IsEnabledAndReady(ModesMenuExtensions.Mode.Combo))
            {
                var qTarget = Variables.Spells[SpellSlot.Q].GetTarget();

                if (Variables.Spells[SpellSlot.Q].IsEnabledAndReady(ModesMenuExtensions.Mode.Combo) && qTarget.IsValidTarget()
                       && Variables.Spells[SpellSlot.Q].IsKillable(qTarget))
                {
                    Variables.Spells[SpellSlot.Q].CastOnUnit(qTarget);
                    return;
                }

                if (Variables.Spells[SpellSlot.R].GetDamage(qTarget) >= qTarget.Health && qTarget.IsValidTarget(Variables.Spells[SpellSlot.R].Range))
                {
                    var prediction = Variables.Spells[SpellSlot.R].GetPrediction(qTarget);
                    if (prediction.Hitchance >= HitChance.VeryHigh)
                    {
                        Variables.Spells[SpellSlot.R].Cast(prediction.CastPosition);
                    }
                }
            }
        }

        public void OnMixed()
        {
            if (ObjectManager.Player.ManaPercent <
                Variables.AssemblyMenu.GetItemValue<Slider>("dzaio.champion.vladimir.mixed.mana").Value)
            {
                return;
            }

            if (Variables.Spells[SpellSlot.Q].IsEnabledAndReady(ModesMenuExtensions.Mode.Harrass))
            {
                var qTarget = Variables.Spells[SpellSlot.Q].GetTarget();
                if (qTarget.IsValidTarget())
                {
                    Variables.Spells[SpellSlot.Q].CastOnUnit(qTarget);
                }
            }

            if (Variables.Spells[SpellSlot.E].IsEnabledAndReady(ModesMenuExtensions.Mode.Harrass))
            {
                var eTarget = Variables.Spells[SpellSlot.E].GetTarget();
                if (eTarget.IsValidTarget() && ObjectManager.Player.Distance(eTarget) >= Variables.Spells[SpellSlot.E].Range / 2f)
                {
                    Variables.Spells[SpellSlot.E].CastOnUnit(eTarget);
                }
            }
        }

        public void OnLastHit()
        { }

        public void OnLaneclear()
        {
            if (ObjectManager.Player.ManaPercent <
                Variables.AssemblyMenu.GetItemValue<Slider>("dzaio.champion.vladimir.farm.mana").Value)
            {
                return;
            }

            var minionTarget =
                MinionManager
                    .GetMinions(Variables.Spells[SpellSlot.Q].Range, MinionTypes.All).FirstOrDefault(m => Variables.Spells[SpellSlot.Q].IsKillable(m));
            if (minionTarget.IsValidTarget(Variables.Spells[SpellSlot.Q].Range))
            {
                Variables.Spells[SpellSlot.Q].Cast(minionTarget);
            }

        }

    }
}
