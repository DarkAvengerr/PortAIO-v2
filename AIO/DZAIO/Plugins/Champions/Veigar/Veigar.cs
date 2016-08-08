using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DZAIO_Reborn.Core;
using DZAIO_Reborn.Helpers;
using DZAIO_Reborn.Helpers.Entity;
using DZAIO_Reborn.Helpers.Modules;
using DZAIO_Reborn.Plugins.Champions.Veigar.Modules;
using DZAIO_Reborn.Plugins.Interface;
using DZLib.Core;
using DZLib.Menu;
using DZLib.MenuExtensions;
using LeagueSharp;
using LeagueSharp.Common;
using SPrediction;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace DZAIO_Reborn.Plugins.Champions.Veigar
{
    class Veigar : IChampion
    {
        public void OnLoad(Menu menu)
        {
            var comboMenu = new Menu(ObjectManager.Player.ChampionName + ": Combo", "dzaio.champion.veigar.combo");
            {
                comboMenu.AddModeMenu(ModesMenuExtensions.Mode.Combo, new[] { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R }, new[] { true, true, true, true });
                //comboMenu.AddNoUltiMenu(false);
                menu.AddSubMenu(comboMenu);
            }

            var mixedMenu = new Menu(ObjectManager.Player.ChampionName + ": Mixed", "dzaio.champion.veigar.harrass");
            {
                mixedMenu.AddModeMenu(ModesMenuExtensions.Mode.Harrass, new[] { SpellSlot.Q, SpellSlot.W, SpellSlot.E }, new[] { true, true, true });
                mixedMenu.AddSlider("dzaio.champion.veigar.mixed.mana", "Min Mana % for Harass", 30, 0, 100);
                menu.AddSubMenu(mixedMenu);
            }

            var farmMenu = new Menu(ObjectManager.Player.ChampionName + ": Farm", "dzaio.champion.veigar.farm");
            {
                farmMenu.AddModeMenu(ModesMenuExtensions.Mode.Laneclear, new[] { SpellSlot.Q, SpellSlot.W }, new[] { true, true});

                farmMenu.AddBool("dzaio.champion.veigar.farm.w.kill", "Only Use W to kill Minions", false);
                farmMenu.AddSlider("dzaio.champion.veigar.farm.w.min", "Min Minions for W", 2, 1, 6);
                farmMenu.AddSlider("dzaio.champion.veigar.farm.mana", "Min Mana % for Farm", 30, 0, 100);
                menu.AddSubMenu(farmMenu);
            }

            var extraMenu = new Menu(ObjectManager.Player.ChampionName + ": Extra", "dzaio.champion.veigar.extra");
            {
                extraMenu.AddBool("dzaio.champion.veigar.extra.interrupter", "Interrupter (E)", true);
                extraMenu.AddBool("dzaio.champion.veigar.extra.antigapcloser", "Antigapcloser (E)", true);
                extraMenu.AddBool("dzaio.champion.veigar.extra.farmQ", "Auto Q Farm", true);
                extraMenu.AddBool("dzaio.champion.veigar.extra.autoW", "Auto W Stunned / Rooted", true);
                extraMenu.AddBool("dzaio.champion.veigar.extra.autoRKS", "Auto R KS", true);
            }

            Variables.Spells[SpellSlot.Q].SetSkillshot(0.25f, 65f, 1900f, false, SkillshotType.SkillshotLine);
            Variables.Spells[SpellSlot.W].SetSkillshot(1.25f, 190f, 0, false, SkillshotType.SkillshotCircle);
            Variables.Spells[SpellSlot.E].SetSkillshot(0.5f, 335f, 0, false, SkillshotType.SkillshotCircle);
        }

        public void RegisterEvents()
        {
            DZInterrupter.OnInterruptableTarget += OnInterrupter;
            DZAntigapcloser.OnEnemyGapcloser += OnGapcloser;
        }

        private void OnGapcloser(DZLib.Core.ActiveGapcloser gapcloser)
        {
            if (Variables.AssemblyMenu.GetItemValue<bool>("dzaio.champion.veigar.extra.antigapcloser")
                && ObjectManager.Player.ManaPercent > 20
                && gapcloser.End.LSDistance(ObjectManager.Player.ServerPosition) < 400
                && gapcloser.Sender.LSIsValidTarget(Variables.Spells[SpellSlot.E].Range)
                && Variables.Spells[SpellSlot.E].LSIsReady())
            {
                Variables.Spells[SpellSlot.E].SPredictionCastRing(gapcloser.Sender, 80f, HitChance.High);
            }
        }

        private void OnInterrupter(AIHeroClient sender, DZInterrupter.InterruptableTargetEventArgs args)
        {
            if (Variables.AssemblyMenu.GetItemValue<bool>("dzaio.champion.veigar.extra.interrupter")
                && ObjectManager.Player.ManaPercent > 20
                && args.DangerLevel > DZInterrupter.DangerLevel.Medium
                && sender.LSIsValidTarget(Variables.Spells[SpellSlot.E].Range)
                && Variables.Spells[SpellSlot.E].LSIsReady())
            {
                Variables.Spells[SpellSlot.E].SPredictionCastRing(sender, 80f, HitChance.Medium);
            }
        }
        public Dictionary<SpellSlot, Spell> GetSpells()
        {
            return new Dictionary<SpellSlot, Spell>
                      {
                                    { SpellSlot.Q, new Spell(SpellSlot.Q, 800f) },
                                    { SpellSlot.W, new Spell(SpellSlot.W, 1000f) },
                                    { SpellSlot.E, new Spell(SpellSlot.E, 700f) },
                                    { SpellSlot.R, new Spell(SpellSlot.R, 640f) }
                      };
        }

        public List<IModule> GetModules()
        {
            return new List<IModule>()
            {
                new VeigarAutoQ(),
                new VeigarAutoW(),
                new VeigarAutoRKS()
            };
        }

        public void OnTick()
        {
            
        }

        public void OnCombo()
        {
            if (Variables.Spells[SpellSlot.E].IsEnabledAndReady(ModesMenuExtensions.Mode.Combo))
            {
                var target = Variables.Spells[SpellSlot.E].GetTarget(250f);
                if (target.LSIsValidTarget())
                {
                    Variables.Spells[SpellSlot.E].SPredictionCastRing(target, 80f, HitChance.High);
                }
            }

            if (Variables.Spells[SpellSlot.W].IsEnabledAndReady(ModesMenuExtensions.Mode.Combo))
            {
                var target = EntityHelper.GetStunnedTarget(Variables.Spells[SpellSlot.W].Range) ??
                             TargetSelector.GetTarget(Variables.Spells[SpellSlot.W].Range, TargetSelector.DamageType.Magical);
                if (target.LSIsValidTarget())
                {
                    var targetIsStunned = target.HasBuffOfType(BuffType.Stun);
                    if (targetIsStunned)
                    {
                        var stunBuff = target.Buffs.Where(b => b.Type == BuffType.Stun)
                            .OrderByDescending(m => m.EndTime - Game.Time)
                            .FirstOrDefault();
                        if (stunBuff != null)
                        {
                            var actualStunDuration = stunBuff.EndTime - Game.Time;
                            if (actualStunDuration > 1.300f)
                            {
                                //Might need to revise this
                                Variables.Spells[SpellSlot.W].CastIfHitchanceEquals(target, HitChance.High);
                            }
                        }
                        
                    }
                }
            }

            if (Variables.Spells[SpellSlot.Q].IsEnabledAndReady(ModesMenuExtensions.Mode.Combo))
            {
                var target = TargetSelector.GetTarget(Variables.Spells[SpellSlot.Q].Range,
                    TargetSelector.DamageType.Magical);
                if (target.LSIsValidTarget())
                {
                    Variables.Spells[SpellSlot.Q].CastIfHitchanceEquals(target, HitChance.High);
                }
            }

            if (Variables.Spells[SpellSlot.R].IsEnabledAndReady(ModesMenuExtensions.Mode.Combo))
            {
                var target = TargetSelector.GetTarget(Variables.Spells[SpellSlot.Q].Range,
                    TargetSelector.DamageType.Magical);
                if (target.LSIsValidTarget() 
                    && Variables.Spells[SpellSlot.R].IsKillable(target))
                {
                    //TODO Check Zhonya and stuff like that if it is an enemy mage.
                    Variables.Spells[SpellSlot.R].Cast(target);
                }
            }
        }

        public void OnMixed()
        {
            if (ObjectManager.Player.ManaPercent <
                Variables.AssemblyMenu.GetItemValue<Slider>("dzaio.champion.veigar.mixed.mana").Value)
            {
                return;
            }

            if (Variables.Spells[SpellSlot.E].IsEnabledAndReady(ModesMenuExtensions.Mode.Harrass))
            {
                var target = Variables.Spells[SpellSlot.E].GetTarget(250f);
                if (target.LSIsValidTarget())
                {
                    Variables.Spells[SpellSlot.E].SPredictionCastRing(target, 80f, HitChance.VeryHigh);
                }
            }

            if (Variables.Spells[SpellSlot.W].IsEnabledAndReady(ModesMenuExtensions.Mode.Harrass))
            {
                var target = EntityHelper.GetStunnedTarget(Variables.Spells[SpellSlot.W].Range) ??
                             TargetSelector.GetTarget(Variables.Spells[SpellSlot.W].Range, TargetSelector.DamageType.Magical);
                if (target.LSIsValidTarget())
                {
                    var targetIsStunned = target.HasBuffOfType(BuffType.Stun);
                    if (targetIsStunned)
                    {
                        var stunBuff = target.Buffs.Where(b => b.Type == BuffType.Stun)
                            .OrderByDescending(m => m.EndTime - Game.Time)
                            .FirstOrDefault();
                        if (stunBuff != null)
                        {
                            var actualStunDuration = stunBuff.EndTime - Game.Time;
                            if (actualStunDuration > 1.300f)
                            {
                                //Might need to revise this
                                Variables.Spells[SpellSlot.W].CastIfHitchanceEquals(target, HitChance.VeryHigh);
                            }
                        }

                    }
                }
            }

            if (Variables.Spells[SpellSlot.Q].IsEnabledAndReady(ModesMenuExtensions.Mode.Harrass))
            {
                var target = TargetSelector.GetTarget(Variables.Spells[SpellSlot.Q].Range,
                    TargetSelector.DamageType.Magical);
                if (target.LSIsValidTarget())
                {
                    Variables.Spells[SpellSlot.Q].CastIfHitchanceEquals(target, HitChance.High);
                }
            }
        }

        public void OnLastHit()
        {}

        public void OnLaneclear()
        {
            if (ObjectManager.Player.ManaPercent <
                Variables.AssemblyMenu.GetItemValue<Slider>("dzaio.champion.veigar.farm.mana").Value)
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

                if (lineFarmLocation.MinionsHit > 0)
                {
                    Variables.Spells[SpellSlot.Q].Cast(lineFarmLocation.Position);
                }
            }

            if (Variables.Spells[SpellSlot.W].IsEnabledAndReady(ModesMenuExtensions.Mode.Laneclear))
            {
                var minMinions = Variables.AssemblyMenu.GetItemValue<Slider>("dzaio.champion.veigar.farm.w.min").Value;
                var positions = Variables.AssemblyMenu.GetItemValue<bool>("dzaio.champion.veigar.farm.w.kill") ?
                    MinionManager.GetMinions(Variables.Spells[SpellSlot.W].Range,
                    MinionTypes.All, MinionTeam.NotAlly,
                    MinionOrderTypes.MaxHealth)
                    .Where(m => Variables.Spells[SpellSlot.W].GetDamage(m) >= m.Health + 5)
                    .Select(m => m.ServerPosition.LSTo2D()).ToList()

                    :

                     MinionManager.GetMinions(Variables.Spells[SpellSlot.W].Range,
                    MinionTypes.All, MinionTeam.NotAlly,
                    MinionOrderTypes.MaxHealth)
                    .Select(m => m.ServerPosition.LSTo2D()).ToList()
                    ;

                var circularFarmLocation = Variables.Spells[SpellSlot.W].GetCircularFarmLocation(positions);
                if (circularFarmLocation.MinionsHit >= minMinions)
                {
                    Variables.Spells[SpellSlot.W].Cast(circularFarmLocation.Position);
                }
            }
        }
    }
}
