using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DZAIO_Reborn.Core;
using DZAIO_Reborn.Helpers;
using DZAIO_Reborn.Helpers.Entity;
using DZAIO_Reborn.Helpers.Modules;
using DZAIO_Reborn.Plugins.Champions.Ahri.Modules;
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
 namespace DZAIO_Reborn.Plugins.Champions.Ahri
{
    class Ahri : IChampion
    {
        public void OnLoad(Menu menu)
        {
            var comboMenu = new Menu(ObjectManager.Player.ChampionName + ": Combo", "dzaio.champion.ahri.combo");
            {
                comboMenu.AddModeMenu(ModesMenuExtensions.Mode.Combo, new[] { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R }, new[] { true, true, true, true });
                comboMenu.AddBool("dzaio.champion.ahri.combo.waitforE", "Wait for charm", true);
                comboMenu.AddBool("dzaio.champion.ahri.combo.onlyInitR", "Only use First R (Only to initiate)", true);

                menu.AddSubMenu(comboMenu);
            }

            var mixedMenu = new Menu(ObjectManager.Player.ChampionName + ": Mixed", "dzaio.champion.ahri.harrass");
            {
                mixedMenu.AddModeMenu(ModesMenuExtensions.Mode.Harrass, new[] { SpellSlot.Q, SpellSlot.W }, new[] { true, true });
                mixedMenu.AddSlider("dzaio.champion.ahri.mixed.mana", "Min Mana % for Harass", 30, 0, 100);
                menu.AddSubMenu(mixedMenu);
            }

            var farmMenu = new Menu(ObjectManager.Player.ChampionName + ": Farm", "dzaio.champion.ahri.farm");
            {
                farmMenu.AddModeMenu(ModesMenuExtensions.Mode.Laneclear, new[] { SpellSlot.Q, SpellSlot.W }, new[] { true, true });

                farmMenu.AddSlider("dzaio.champion.ahri.farm.w.min", "Min Minions for W", 2, 1, 6);
                farmMenu.AddSlider("dzaio.champion.ahri.farm.mana", "Min Mana % for Farm", 30, 0, 100);
                menu.AddSubMenu(farmMenu);
            }

            var extraMenu = new Menu(ObjectManager.Player.ChampionName + ": Extra", "dzaio.champion.ahri.extra");
            {
                extraMenu.AddBool("dzaio.champion.ahri.extra.interrupter", "Interrupter (E)", true);
                extraMenu.AddBool("dzaio.champion.ahri.extra.antigapcloser", "Antigapcloser (E)", true);
                extraMenu.AddBool("dzaio.champion.ahri.extra.autoQ", "Auto Q Stunned / Rooted", true);
                extraMenu.AddBool("dzaio.champion.ahri.extra.autoQKS", "Auto Q KS", true);
            }

            Variables.Spells[SpellSlot.Q].SetSkillshot(0.25f, 100, 1600, false, SkillshotType.SkillshotLine);
            Variables.Spells[SpellSlot.E].SetSkillshot(0.25f, 60, 1200, true, SkillshotType.SkillshotLine);
        }

        public void RegisterEvents()
        {
            DZInterrupter.OnInterruptableTarget += OnInterrupter;
            DZAntigapcloser.OnEnemyGapcloser += OnGapcloser;
        }

        private void OnGapcloser(DZLib.Core.ActiveGapcloser gapcloser)
        {
            if (Variables.AssemblyMenu.GetItemValue<bool>("dzaio.champion.ahri.extra.antigapcloser")
                && ObjectManager.Player.ManaPercent > 20
                && gapcloser.End.LSDistance(ObjectManager.Player.ServerPosition) < 400
                && gapcloser.Sender.LSIsValidTarget(Variables.Spells[SpellSlot.E].Range)
                && Variables.Spells[SpellSlot.E].LSIsReady())
            {
                Variables.Spells[SpellSlot.E].CastIfHitchanceEquals(gapcloser.Sender, HitChance.Medium);
            }
        }

        private void OnInterrupter(AIHeroClient sender, DZInterrupter.InterruptableTargetEventArgs args)
        {
            if (Variables.AssemblyMenu.GetItemValue<bool>("dzaio.champion.ahri.extra.interrupter")
                && ObjectManager.Player.ManaPercent > 20
                && args.DangerLevel > DZInterrupter.DangerLevel.Medium
                && sender.LSIsValidTarget(Variables.Spells[SpellSlot.E].Range)
                && Variables.Spells[SpellSlot.E].LSIsReady())
            {
                Variables.Spells[SpellSlot.E].CastIfHitchanceEquals(sender, HitChance.High);
            }
        }
        public Dictionary<SpellSlot, Spell> GetSpells()
        {
            return new Dictionary<SpellSlot, Spell>
                      {
                            { SpellSlot.Q, new Spell(SpellSlot.Q, 925f) },
                            { SpellSlot.W, new Spell(SpellSlot.W, 700f) },
                            { SpellSlot.E, new Spell(SpellSlot.E, 875f) },
                            { SpellSlot.R, new Spell(SpellSlot.R, 400f) }
                      };
        }

        public List<IModule> GetModules()
        {
            return new List<IModule>()
            {
               new AhriQKS(),
               new AhriQRootStun()
            };
        }

        public void OnTick()
        {

        }

        public void OnCombo()
        {
            var comboTarget = TargetSelector.GetTarget(Variables.Spells[SpellSlot.E].Range, TargetSelector.DamageType.Magical);
            var charmedUnit = HeroManager.Enemies.Find(h => h.HasBuffOfType(BuffType.Charm) && h.LSIsValidTarget(Variables.Spells[SpellSlot.Q].Range));

            var finalTarget = charmedUnit ?? comboTarget;

            if (finalTarget.LSIsValidTarget())
            {
                if (Variables.AssemblyMenu.GetItemValue<bool>("dzaio.champion.ahri.combo.waitforE"))
                {
                    //Wait for Charm Combo
                    if (Variables.Spells[SpellSlot.E].IsEnabledAndReady(ModesMenuExtensions.Mode.Combo) &&
                        Variables.Spells[SpellSlot.Q].IsEnabledAndReady(ModesMenuExtensions.Mode.Combo) &&
                        !finalTarget.IsCharmed)
                    {
                        Variables.Spells[SpellSlot.E].CastIfHitchanceEquals(finalTarget, HitChance.High);
                    }

                    if (Variables.Spells[SpellSlot.Q].IsEnabledAndReady(ModesMenuExtensions.Mode.Combo) &&
                        (!Variables.Spells[SpellSlot.E].IsEnabledAndReady(ModesMenuExtensions.Mode.Combo) ||
                         ObjectManager.Player.ManaPercent <= 25))
                    {
                        Variables.Spells[SpellSlot.Q].CastIfHitchanceEquals(finalTarget, HitChance.High);
                    }

                    if (Variables.Spells[SpellSlot.W].IsEnabledAndReady(ModesMenuExtensions.Mode.Combo) &&
                        finalTarget.LSIsValidTarget(Variables.Spells[SpellSlot.W].Range) &&
                        (finalTarget.IsCharmed ||
                         (Variables.Spells[SpellSlot.W].GetDamage(finalTarget) +
                          Variables.Spells[SpellSlot.Q].GetDamage(finalTarget) > finalTarget.Health + 25)))
                    {
                        Variables.Spells[SpellSlot.W].Cast();
                    }
                }
                else
                {
                    if (Variables.Spells[SpellSlot.E].IsEnabledAndReady(ModesMenuExtensions.Mode.Combo) &&
                        Variables.Spells[SpellSlot.Q].IsEnabledAndReady(ModesMenuExtensions.Mode.Combo) &&
                        !finalTarget.IsCharmed)
                    {
                        Variables.Spells[SpellSlot.E].CastIfHitchanceEquals(finalTarget, HitChance.High);
                    }

                    if (Variables.Spells[SpellSlot.Q].IsEnabledAndReady(ModesMenuExtensions.Mode.Combo))
                    {
                        Variables.Spells[SpellSlot.Q].CastIfHitchanceEquals(finalTarget, HitChance.High);
                    }

                    if (Variables.Spells[SpellSlot.W].IsEnabledAndReady(ModesMenuExtensions.Mode.Combo) &&
                        finalTarget.LSIsValidTarget(Variables.Spells[SpellSlot.W].Range) &&
                        ((Variables.Spells[SpellSlot.W].GetDamage(finalTarget) +
                          Variables.Spells[SpellSlot.Q].GetDamage(finalTarget) > finalTarget.Health + 25)))
                    {
                        Variables.Spells[SpellSlot.W].Cast();
                    }
                }

                if (Variables.Spells[SpellSlot.R].IsEnabledAndReady(ModesMenuExtensions.Mode.Combo))
                {
                    HandleRLogic();
                }
                
            }
        }

        private void HandleRLogic()
        {
            if (CanUltiSafely())
            {
                var mousePosition = Game.CursorPos;
                var extendedPosition = ObjectManager.Player.ServerPosition.LSExtend(mousePosition, 450f);

                Variables.Spells[SpellSlot.R].Cast(extendedPosition);
            }
        }

        private bool CanUltiSafely()
        {
            if (ObjectManager.Player.LSCountEnemiesInRange(550f) > 0)
            {
                return true;
            }

            if (ObjectManager.Player.LSHasBuff("AhriTumble") &&
                Variables.AssemblyMenu.GetItemValue<bool>("dzaio.champion.ahri.combo.onlyInitR"))
            {
                return false;
            }

            var mousePosition = Game.CursorPos;
            var extendedPosition = ObjectManager.Player.ServerPosition.LSExtend(mousePosition, 450f);

            if (extendedPosition.LSGetEnemiesInRange(625f).Any())
            {
                if (ObjectManager.Player.LSHasBuff("AhriTumble"))
                {
                    return true;
                }

                var manaCheck = ObjectManager.Player.Mana >
                                Variables.Spells[SpellSlot.Q].ManaCost + Variables.Spells[SpellSlot.E].ManaCost +
                                Variables.Spells[SpellSlot.R].ManaCost;

                if (manaCheck && Variables.Spells[SpellSlot.Q].IsEnabledAndReady(ModesMenuExtensions.Mode.Combo) &&
                    Variables.Spells[SpellSlot.E].IsEnabledAndReady(ModesMenuExtensions.Mode.Combo))
                {
                    if (extendedPosition.IsSafe(450f))
                    {
                        return true;
                    }
                }

            }

            return false;
        }
        public void OnMixed()
        {
            if (ObjectManager.Player.ManaPercent <
                Variables.AssemblyMenu.GetItemValue<Slider>("dzaio.champion.ahri.mixed.mana").Value)
            {
                return;
            }
            var comboTarget = TargetSelector.GetTarget(Variables.Spells[SpellSlot.E].Range, TargetSelector.DamageType.Magical);
            var charmedUnit = HeroManager.Enemies.Find(h => h.HasBuffOfType(BuffType.Charm) && h.LSIsValidTarget(Variables.Spells[SpellSlot.Q].Range));

            var finalTarget = charmedUnit ?? comboTarget;

            if (Variables.Spells[SpellSlot.Q].IsEnabledAndReady(ModesMenuExtensions.Mode.Combo))
            {
                Variables.Spells[SpellSlot.Q].CastIfHitchanceEquals(finalTarget, HitChance.High);
            }

            if (Variables.Spells[SpellSlot.W].IsEnabledAndReady(ModesMenuExtensions.Mode.Combo) &&
                finalTarget.LSIsValidTarget(Variables.Spells[SpellSlot.W].Range) &&
                ((Variables.Spells[SpellSlot.W].GetDamage(finalTarget) +
                  Variables.Spells[SpellSlot.Q].GetDamage(finalTarget) > finalTarget.Health + 25)))
            {
                Variables.Spells[SpellSlot.W].Cast();
            }
        }

        public void OnLastHit()
        { }

        public void OnLaneclear()
        {
            if (ObjectManager.Player.ManaPercent <
                Variables.AssemblyMenu.GetItemValue<Slider>("dzaio.champion.ahri.farm.mana").Value)
            {
                return;
            }

            if (Variables.Spells[SpellSlot.Q].IsEnabledAndReady(ModesMenuExtensions.Mode.Laneclear))
            {
                var positions = MinionManager.GetMinions(Variables.Spells[SpellSlot.Q].Range,
                    MinionTypes.All, MinionTeam.NotAlly,
                    MinionOrderTypes.MaxHealth)
                    .Where(m => Variables.Spells[SpellSlot.Q].GetDamage(m) >= m.Health + 5)
                    .Select(m => m.ServerPosition.LSTo2D()).ToList();

                var lineFarmLocation = Variables.Spells[SpellSlot.Q].GetLineFarmLocation(positions);

                if (lineFarmLocation.MinionsHit >= 6)
                {
                    Variables.Spells[SpellSlot.Q].Cast(lineFarmLocation.Position);
                }
            }

            if (Variables.Spells[SpellSlot.Q].IsEnabledAndReady(ModesMenuExtensions.Mode.Laneclear))
            {
                var positions = MinionManager.GetMinions(Variables.Spells[SpellSlot.W].Range,
                    MinionTypes.All, MinionTeam.NotAlly,
                    MinionOrderTypes.MaxHealth)
                    .Where(m => Variables.Spells[SpellSlot.W].GetDamage(m) >= m.Health + 5)
                    .Select(m => m.ServerPosition.LSTo2D()).ToList();
                if (positions.Count() >=
                    Variables.AssemblyMenu.GetItemValue<Slider>("dzaio.champion.ahri.farm.w.min").Value)
                {
                    Variables.Spells[SpellSlot.W].Cast();
                }
            }

        }
    }
}
