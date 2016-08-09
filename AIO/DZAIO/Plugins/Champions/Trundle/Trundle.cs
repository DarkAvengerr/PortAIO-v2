using System;
using System.Collections.Generic;
using System.Linq;
using DZAIO_Reborn.Core;
using DZAIO_Reborn.Helpers;
using DZAIO_Reborn.Helpers.Entity;
using DZAIO_Reborn.Helpers.Modules;
using DZAIO_Reborn.Plugins.Champions.Trundle.Modules;
using DZAIO_Reborn.Plugins.Interface;
using DZLib.Core;
using DZLib.Menu;
using DZLib.MenuExtensions;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace DZAIO_Reborn.Plugins.Champions.Trundle
{
    class Trundle : IChampion
    {
        public void OnLoad(Menu menu)
        {
            var comboMenu = new Menu(ObjectManager.Player.ChampionName + ": Combo", "dzaio.champion.trundle.combo");
            {
                comboMenu.AddModeMenu(ModesMenuExtensions.Mode.Combo, new[] { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R }, new[] { true, true, true, true });
                //comboMenu.AddNoUltiMenu(false);
                menu.AddSubMenu(comboMenu);
            }

            var mixedMenu = new Menu(ObjectManager.Player.ChampionName + ": Mixed", "dzaio.champion.trundle.harrass");
            {
                mixedMenu.AddModeMenu(ModesMenuExtensions.Mode.Harrass, new[] { SpellSlot.Q, SpellSlot.W, SpellSlot.E}, new[] { true, true, true });
                mixedMenu.AddSlider("dzaio.champion.trundle.mixed.mana", "Min Mana % for Harass", 30, 0, 100);
                menu.AddSubMenu(mixedMenu);
            }

            var farmMenu = new Menu(ObjectManager.Player.ChampionName + ": Farm", "dzaio.champion.trundle.farm");
            {
                farmMenu.AddModeMenu(ModesMenuExtensions.Mode.Laneclear, new[] { SpellSlot.Q, }, new[] { true });
                farmMenu.AddBool("dzaio.champion.trundle.jungleclear.q", "Use Q Jungle", true);
                farmMenu.AddBool("dzaio.champion.trundle.jungleclear.w", "Use W Jungle", true);

                farmMenu.AddSlider("dzaio.champion.trundle.farm.mana", "Min Mana % for Farm", 30, 0, 100);
                menu.AddSubMenu(farmMenu);
            }

            var extraMenu = new Menu(ObjectManager.Player.ChampionName + ": Extra", "dzaio.champion.trundle.extra");
            {
                extraMenu.AddBool("dzaio.champion.trundle.extra.interrupter", "Interrupter (E)", true);
                extraMenu.AddBool("dzaio.champion.trundle.extra.antigapcloser", "Antigapcloser (E)", true);
                extraMenu.AddBool("dzaio.champion.trundle.extra.qks", "Q For KS", true);
            }
        }

        public void RegisterEvents()
        {
            DZInterrupter.OnInterruptableTarget += OnInterrupter;
            DZAntigapcloser.OnEnemyGapcloser += OnGapcloser;
        }

        private void OnGapcloser(DZLib.Core.ActiveGapcloser gapcloser)
        {
            if (Variables.AssemblyMenu.GetItemValue<bool>("dzaio.champion.trundle.extra.antigapcloser")
                && ObjectManager.Player.ManaPercent > 20
                && gapcloser.End.Distance(ObjectManager.Player.ServerPosition) < 400
                && gapcloser.Sender.IsValidTarget(Variables.Spells[SpellSlot.E].Range)
                && Variables.Spells[SpellSlot.E].IsReady())
            {
                Variables.Spells[SpellSlot.E].Cast(gapcloser.Sender.ServerPosition);
            }
        }

        private void OnInterrupter(AIHeroClient sender, DZInterrupter.InterruptableTargetEventArgs args)
        {
            if (Variables.AssemblyMenu.GetItemValue<bool>("dzaio.champion.trundle.extra.interrupter")
                && ObjectManager.Player.ManaPercent > 20
                && args.DangerLevel > DZInterrupter.DangerLevel.Medium
                && sender.IsValidTarget(Variables.Spells[SpellSlot.E].Range)
                && Variables.Spells[SpellSlot.E].IsReady())
            {
                Variables.Spells[SpellSlot.E].Cast(sender.ServerPosition);
            }
        }

        public Dictionary<SpellSlot, Spell> GetSpells()
        {
           return new Dictionary<SpellSlot, Spell>
                      {
                                    { SpellSlot.Q, new Spell(SpellSlot.Q, 550f) },
                                    { SpellSlot.W, new Spell(SpellSlot.W, 900f) },
                                    { SpellSlot.E, new Spell(SpellSlot.E, 1000f) },
                                    { SpellSlot.R, new Spell(SpellSlot.R, 700f) }
                      };
        }

        public List<IModule> GetModules()
        {
            return new List<IModule>()
            {
                new TrundleQKS()
            };
        }

        public void OnTick()
        {
        }

        public void OnCombo()
        {
            var target = TargetSelector.GetTarget(Variables.Spells[SpellSlot.E].Range,
                TargetSelector.DamageType.Physical);
            if (target.IsValidTarget())
            {

                if (Variables.Spells[SpellSlot.Q].IsEnabledAndReady(ModesMenuExtensions.Mode.Combo)
                    && target.IsValidTarget(Variables.Spells[SpellSlot.Q].Range))
                {
                    Variables.Spells[SpellSlot.Q].Cast();
                }

                if (Variables.Spells[SpellSlot.W].IsEnabledAndReady(ModesMenuExtensions.Mode.Combo)
                    && target.IsValidTarget(Variables.Spells[SpellSlot.W].Range))
                {
                    var optimalWPosition = ObjectManager.Player.ServerPosition.Extend(target.Position,
                        ObjectManager.Player.Distance(target) / 2f);

                    Variables.Spells[SpellSlot.W].Cast(optimalWPosition);
                }

                if (Variables.Spells[SpellSlot.E].IsEnabledAndReady(ModesMenuExtensions.Mode.Combo))
                {
                    Variables.Spells[SpellSlot.E].Cast(GetPillarOptimalPosition(target));
                }

                if (Variables.Spells[SpellSlot.R].IsEnabledAndReady(ModesMenuExtensions.Mode.Combo))
                {
                    var bestRTarget = Variables.Orbwalker.GetTarget().IsValid<AIHeroClient>() && Variables.Orbwalker.GetTarget().IsValidTarget(Variables.Spells[SpellSlot.R].Range)
                        ? Variables.Orbwalker.GetTarget() as AIHeroClient
                        : GetBestRTarget();
                    if (bestRTarget.IsValidTarget())
                    {
                        Variables.Spells[SpellSlot.R].Cast(bestRTarget);
                    }
                }
            }
        }

        public void OnMixed()
        {
            if (ObjectManager.Player.ManaPercent <
                Variables.AssemblyMenu.GetItemValue<Slider>("dzaio.champion.trundle.mixed.mana").Value)
            {
                return;
            }

            var target = TargetSelector.GetTarget(Variables.Spells[SpellSlot.E].Range,
                TargetSelector.DamageType.Physical);
            if (target.IsValidTarget())
            {
                if (Variables.Spells[SpellSlot.Q].IsEnabledAndReady(ModesMenuExtensions.Mode.Harrass)
                    && target.IsValidTarget(Variables.Spells[SpellSlot.Q].Range))
                {
                    Variables.Spells[SpellSlot.Q].Cast();
                }

                if (Variables.Spells[SpellSlot.W].IsEnabledAndReady(ModesMenuExtensions.Mode.Harrass)
                    && target.IsValidTarget(Variables.Spells[SpellSlot.W].Range))
                {
                    var optimalWPosition = ObjectManager.Player.ServerPosition.Extend(target.Position,
                        ObjectManager.Player.Distance(target)/2f);

                    Variables.Spells[SpellSlot.W].Cast(optimalWPosition);
                }

                if (Variables.Spells[SpellSlot.E].IsEnabledAndReady(ModesMenuExtensions.Mode.Harrass))
                {
                    Variables.Spells[SpellSlot.E].Cast(GetPillarOptimalPosition(target));
                }
            }
        }

        public void OnLastHit()
        {
            throw new NotImplementedException();
        }

        public void OnLaneclear()
        {
            if (ObjectManager.Player.ManaPercent <
                Variables.AssemblyMenu.GetItemValue<Slider>("dzaio.champion.trundle.farm.mana").Value)
            {
                return;
            }

            if (EntityHelper.PlayerIsClearingJungle())
            {
                OnJungleClear();
            }
            else
            {
                //Laneclear
            }
        }

        private void OnJungleClear()
        {
            var Minion =
                MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition,
                    Variables.Spells[SpellSlot.W].Range,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth).FirstOrDefault();
            var target = Minion ?? PlayerMonitor.GetLastTarget();

            if (Variables.AssemblyMenu.GetItemValue<bool>("dzaio.champion.trundle.jungleclear.w")
                && Variables.Spells[SpellSlot.W].IsReady() && target.IsValidTarget(675f))
            {
                Variables.Spells[SpellSlot.W].Cast(ObjectManager.Player.ServerPosition.Extend(target.Position, ObjectManager.Player.Distance(target) / 2f));
            }

            if (Variables.AssemblyMenu.GetItemValue<bool>("dzaio.champion.trundle.jungleclear.q")
                && Variables.Spells[SpellSlot.Q].IsReady() && target.IsValidTarget(Variables.Spells[SpellSlot.Q].Range))
            {
                Variables.Spells[SpellSlot.Q].Cast();
            }
        }

        private AIHeroClient GetBestRTarget()
        {
            return
                ObjectManager.Player.GetEnemiesInRange(Variables.Spells[SpellSlot.R].Range).OrderBy(m => m.PercentArmorMod)
                    .ThenBy(m => m.PercentMagicReduction)
                    .FirstOrDefault();
        }

        private Vector3 GetPillarOptimalPosition(AIHeroClient target)
        {
            var targetPrediction = Prediction.GetPrediction(target, 0.25f);
            var extendedVector = targetPrediction.UnitPosition.Extend(ObjectManager.Player.ServerPosition, -310f);
            return extendedVector;
        }
    }
}
