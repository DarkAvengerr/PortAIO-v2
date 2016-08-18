using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DZAIO_Reborn.Core;
using DZAIO_Reborn.Helpers;
using DZAIO_Reborn.Helpers.Entity;
using DZAIO_Reborn.Helpers.Modules;
using DZAIO_Reborn.Helpers.Positioning;
using DZAIO_Reborn.Plugins.Champions.Warwick.Modules;
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
 namespace DZAIO_Reborn.Plugins.Champions.Warwick
{
    class Warwick : IChampion
    {
        private float currentELevel = 0;

        public void OnLoad(Menu menu)
        {
            var comboMenu = new Menu(ObjectManager.Player.ChampionName + ": Combo", "dzaio.champion.warwick.combo");
            {
                comboMenu.AddModeMenu(ModesMenuExtensions.Mode.Combo, new[] { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R }, new[] { true, true, true, true });
                comboMenu.AddSlider("dzaio.champion.warwick.combo.r.min", "Min Enemies for R", 2, 1, 5);
                menu.AddSubMenu(comboMenu);
            }

            var mixedMenu = new Menu(ObjectManager.Player.ChampionName + ": Mixed", "dzaio.champion.warwick.harrass");
            {
                mixedMenu.AddModeMenu(ModesMenuExtensions.Mode.Harrass, new[] { SpellSlot.Q, SpellSlot.W }, new[] { true, true });
                mixedMenu.AddSlider("dzaio.champion.warwick.mixed.mana", "Min Mana % for Harass", 30, 0, 100);
                menu.AddSubMenu(mixedMenu);
            }

            var farmMenu = new Menu(ObjectManager.Player.ChampionName + ": Farm", "dzaio.champion.warwick.farm");
            {
                farmMenu.AddModeMenu(ModesMenuExtensions.Mode.Laneclear, new[] { SpellSlot.Q, SpellSlot.W }, new[] { true, true });
                farmMenu.AddSlider("dzaio.champion.warwick.farm.mana", "Min Mana % for Farm", 30, 0, 100);
                menu.AddSubMenu(farmMenu);
            }

            var extraMenu = new Menu(ObjectManager.Player.ChampionName + ": Extra", "dzaio.champion.warwick.extra");
            {
                extraMenu.AddBool("dzaio.champion.warwick.extra.interruptR", "R Interrupt", true);
                extraMenu.AddBool("dzaio.champion.warwick.extra.autoQKS", "Q KS", true);
                extraMenu.AddBool("dzaio.champion.warwick.extra.QUnderTurret", "Q Under Turret", true);
                extraMenu.AddBool("dzaio.champion.warwick.extra.smiteR", "Use Smite Before R", true);
                extraMenu.AddBool("dzaio.champion.warwick.extra.rPeel", "Use R To Peel", true);
            }

            Variables.Spells[SpellSlot.Q].SetTargetted(0.25f, 2000f);
            Variables.Spells[SpellSlot.R].SetSkillshot(0.25f, 175, 700, false, SkillshotType.SkillshotCircle);

        }

        public void RegisterEvents()
        {
            DZInterrupter.OnInterruptableTarget += OnInterrupter;
            DZAntigapcloser.OnEnemyGapcloser += OnGapcloser;
            Orbwalking.AfterAttack += AfterAttack;
            Obj_AI_Base.OnSpellCast += OnSpellCast;
        }

        private void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.Slot == SpellSlot.R && args.Target != null && args.Target.IsValid<AIHeroClient>())
            {
                var smiteSlot = ObjectManager.Player.GetSmiteSlot();
                if (smiteSlot != SpellSlot.Unknown)
                {
                    var smiteSpell = ObjectManager.Player.GetSpell(smiteSlot);
                    if (smiteSpell.IsReady() && args.Target.Position.Distance(ObjectManager.Player.ServerPosition) < 550f)
                    {
                        ObjectManager.Player.Spellbook.CastSpell(smiteSlot, args.Target);
                    }
                }
            }
        }


        private void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {

        }

        private void OnGapcloser(DZLib.Core.ActiveGapcloser gapcloser)
        {

        }

        private void OnInterrupter(AIHeroClient sender, DZInterrupter.InterruptableTargetEventArgs args)
        {
            if (Variables.Spells[SpellSlot.R].IsReady() &&
                Variables.AssemblyMenu.GetItemValue<bool>("dzaio.champion.warwick.extra.interruptR") &&
                args.DangerLevel >= DZInterrupter.DangerLevel.High)
            {
                var interruptPosition = sender.ServerPosition;
                if (interruptPosition.IsSafe() && sender.IsValidTarget(Variables.Spells[SpellSlot.R].Range))
                {
                    Variables.Spells[SpellSlot.R].CastOnUnit(sender);
                }
            }
        }
        public Dictionary<SpellSlot, Spell> GetSpells()
        {
            return new Dictionary<SpellSlot, Spell>
                      {
                                    { SpellSlot.Q, new Spell(SpellSlot.Q, 400f) },
                                    { SpellSlot.W, new Spell(SpellSlot.W, 1250f) },
                                    { SpellSlot.E, new Spell(SpellSlot.E, 670f + (770f * ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level)) },
                                    { SpellSlot.R, new Spell(SpellSlot.R, 700f) }
                      };
        }

        public List<IModule> GetModules()
        {
            return new List<IModule>()
            {
                new WarwickQKS()
            };
        }

        public void OnTick()
        {
            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level > currentELevel)
            {
                Variables.Spells[SpellSlot.E].Range = 670f +
                                                  (770f * ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level);
                currentELevel = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level;
            }
        }

        public void OnCombo()
        {
            if (Variables.Spells[SpellSlot.Q].IsEnabledAndReady(ModesMenuExtensions.Mode.Combo))
            {
                if (ObjectManager.Player.UnderTurret(true) &&
                    !Variables.AssemblyMenu.GetItemValue<bool>("dzaio.champion.warwick.extra.QUnderTurret"))
                {
                    return;
                }

                var qTarget = Variables.Spells[SpellSlot.Q].GetTarget();
                if (qTarget.IsValidTarget())
                {
                    Variables.Spells[SpellSlot.Q].CastOnUnit(qTarget);
                }
            }

            if (Variables.Spells[SpellSlot.W].IsEnabledAndReady(ModesMenuExtensions.Mode.Combo) && ObjectManager.Player.CountEnemiesInRange(Variables.Spells[SpellSlot.W].Range) > 0)
            {
                    Variables.Spells[SpellSlot.W].Cast();
            }

            if (Variables.Spells[SpellSlot.R].IsEnabledAndReady(ModesMenuExtensions.Mode.Combo))
            {
                var rTarget = Variables.Spells[SpellSlot.R].GetTarget();
                if (rTarget.IsValidTarget())
                {
                    var rPosition = rTarget.ServerPosition;
                    var targetPriority = TargetSelector.GetPriority(rTarget);
                    var enemiesAroundPosition = rPosition.GetEnemiesInRange(550f).Count();
                    var alliesAroundPosition = rPosition.GetAlliesInRange(550f).Count();
                    if (!rPosition.UnderTurret(true) && ObjectManager.Player.HealthPercent >= 15 &&
                        ((targetPriority > 4 && enemiesAroundPosition < 3 && alliesAroundPosition > 0) ||
                         (enemiesAroundPosition < 3 && alliesAroundPosition > 0 &&
                          Variables.Spells[SpellSlot.R].IsKillable(rTarget))))
                    {
                        Variables.Spells[SpellSlot.R].Cast(rTarget);
                    }
                   
                }
            }
        }

        public void OnMixed()
        {
            if (ObjectManager.Player.ManaPercent <
                Variables.AssemblyMenu.GetItemValue<Slider>("dzaio.champion.warwick.mixed.mana").Value)
            {
                return;
            }

            if (Variables.Spells[SpellSlot.Q].IsEnabledAndReady(ModesMenuExtensions.Mode.Harrass))
            {
                if (ObjectManager.Player.UnderTurret(true) &&
                    !Variables.AssemblyMenu.GetItemValue<bool>("dzaio.champion.warwick.extra.QUnderTurret"))
                {
                    return;
                }

                var qTarget = Variables.Spells[SpellSlot.Q].GetTarget();
                if (qTarget.IsValidTarget())
                {
                    Variables.Spells[SpellSlot.Q].CastOnUnit(qTarget);
                }
            }

            if (Variables.Spells[SpellSlot.W].IsEnabledAndReady(ModesMenuExtensions.Mode.Harrass) 
                && ObjectManager.Player.CountEnemiesInRange(Variables.Spells[SpellSlot.W].Range) > 0)
            {
                Variables.Spells[SpellSlot.W].Cast();
            }
        }

        public void OnLastHit()
        { }

        public void OnLaneclear()
        {
            if (ObjectManager.Player.ManaPercent <
                Variables.AssemblyMenu.GetItemValue<Slider>("dzaio.champion.warwick.farm.mana").Value)
            {
                return;
            }

           
        }

    }
}
