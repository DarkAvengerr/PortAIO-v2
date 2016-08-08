using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DZAIO_Reborn.Core;
using DZAIO_Reborn.Helpers;
using DZAIO_Reborn.Helpers.Entity;
using DZAIO_Reborn.Helpers.Modules;
using DZAIO_Reborn.Plugins.Champions.Sivir.Modules;
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
 namespace DZAIO_Reborn.Plugins.Champions.Sivir
{
    class Sivir : IChampion
    {
        public void OnLoad(Menu menu)
        {
            var comboMenu = new Menu(ObjectManager.Player.ChampionName + ": Combo", "dzaio.champion.sivir.combo");
            {
                comboMenu.AddModeMenu(ModesMenuExtensions.Mode.Combo, new[] { SpellSlot.Q, SpellSlot.W }, new[] { true, true });
                menu.AddSubMenu(comboMenu);
            }

            var mixedMenu = new Menu(ObjectManager.Player.ChampionName + ": Mixed", "dzaio.champion.sivir.harrass");
            {
                mixedMenu.AddModeMenu(ModesMenuExtensions.Mode.Harrass, new[] { SpellSlot.Q, SpellSlot.W}, new[] { true, true });
                mixedMenu.AddSlider("dzaio.champion.sivir.mixed.mana", "Min Mana % for Harass", 30, 0, 100);
                menu.AddSubMenu(mixedMenu);
            }

            var farmMenu = new Menu(ObjectManager.Player.ChampionName + ": Farm", "dzaio.champion.sivir.farm");
            {
                farmMenu.AddModeMenu(ModesMenuExtensions.Mode.Laneclear, new[] { SpellSlot.Q, SpellSlot.W }, new[] { true, true });
                farmMenu.AddSlider("dzaio.champion.sivir.farm.q.min", "Min Minions for Q", 4, 1, 6);
                farmMenu.AddSlider("dzaio.champion.sivir.farm.mana", "Min Mana % for Farm", 30, 0, 100);
                menu.AddSubMenu(farmMenu);
            }

            var extraMenu = new Menu(ObjectManager.Player.ChampionName + ": Extra", "dzaio.champion.sivir.extra");
            {
                extraMenu.AddBool("dzaio.champion.sivir.extra.autoE", "E Shield", true);
                extraMenu.AddSlider("dzaio.champion.sivir.extra.autoE.Delay", "E Shield Delay", 120, 0, 350);
                extraMenu.AddBool("dzaio.champion.sivir.extra.autoQKS", "Q KS", true);
                extraMenu.AddBool("dzaio.champion.sivir.extra.autoQRoot", "Q Root/Slow/Dash", true);
            }

            Variables.Spells[SpellSlot.Q].SetSkillshot(0.25f, 90f, 1350f, false, SkillshotType.SkillshotLine);
        }

        public void RegisterEvents()
        {
            DZInterrupter.OnInterruptableTarget += OnInterrupter;
            DZAntigapcloser.OnEnemyGapcloser += OnGapcloser;
            Orbwalking.AfterAttack += AfterAttack;
            Obj_AI_Base.OnProcessSpellCast += OnSpellCast;
        }

        private void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender is AIHeroClient && !args.SData.LSIsAutoAttack() && sender.IsEnemy &&
                (args.Target != null && args.Target.IsMe) && Variables.AssemblyMenu.GetItemValue<bool>("dzaio.champion.sivir.extra.autoE"))
            {
                LeagueSharp.Common.Utility.DelayAction.Add(Variables.AssemblyMenu.GetItemValue<Slider>("dzaio.champion.sivir.extra.autoE.Delay").Value,
                    () =>
                    { Variables.Spells[SpellSlot.E].Cast(); });
            }
        }

        private void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (!unit.IsMe || !unit.IsValid<Obj_AI_Base>())
            {
                return;
            }

            if (Variables.Spells[SpellSlot.W].IsEnabledAndReady(ModesMenuExtensions.Mode.Combo) 
                || (Variables.Spells[SpellSlot.W].IsEnabledAndReady(ModesMenuExtensions.Mode.Harrass) 
                && ObjectManager.Player.ManaPercent >= Variables.AssemblyMenu.GetItemValue<Slider>("dzaio.champion.sivir.mixed.mana").Value))
            {
                if (target.IsValid<AIHeroClient>() && target.LSIsValidTarget())
                {
                    Variables.Spells[SpellSlot.W].Cast();
                }
            }
        }

        private void OnGapcloser(DZLib.Core.ActiveGapcloser gapcloser)
        {
            
        }

        private void OnInterrupter(AIHeroClient sender, DZInterrupter.InterruptableTargetEventArgs args)
        {
            
        }
        public Dictionary<SpellSlot, Spell> GetSpells()
        {
            return new Dictionary<SpellSlot, Spell>
                      {
                                    { SpellSlot.Q, new Spell(SpellSlot.Q, 1250f) },
                                    { SpellSlot.W, new Spell(SpellSlot.W) },
                                    { SpellSlot.E, new Spell(SpellSlot.E) },
                                    { SpellSlot.R, new Spell(SpellSlot.R) }
                      };
        }

        public List<IModule> GetModules()
        {
            return new List<IModule>()
            {
                new SivirQKS(),
                new SivirQRoot()
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
                if (qTarget.LSIsValidTarget())
                {
                    var qPrediction = Variables.Spells[SpellSlot.Q].GetPrediction(qTarget);
                    if (qPrediction.Hitchance >= HitChance.High)
                    {
                        Variables.Spells[SpellSlot.Q].Cast(qPrediction.CastPosition);
                    }
                }
            }
        }

        public void OnMixed()
        {
            if (ObjectManager.Player.ManaPercent <
                Variables.AssemblyMenu.GetItemValue<Slider>("dzaio.champion.sivir.mixed.mana").Value)
            {
                return;
            }

            if (Variables.Spells[SpellSlot.Q].IsEnabledAndReady(ModesMenuExtensions.Mode.Harrass))
            {
                var qTarget = Variables.Spells[SpellSlot.Q].GetTarget();
                if (qTarget.LSIsValidTarget())
                {
                    var qPrediction = Variables.Spells[SpellSlot.Q].GetPrediction(qTarget);
                    if (qPrediction.Hitchance >= HitChance.High)
                    {
                        Variables.Spells[SpellSlot.Q].Cast(qPrediction.CastPosition);
                    }
                }
            }
        }

        public void OnLastHit()
        { }

        public void OnLaneclear()
        {
            if (ObjectManager.Player.ManaPercent <
                Variables.AssemblyMenu.GetItemValue<Slider>("dzaio.champion.sivir.farm.mana").Value)
            {
                return;
            }
            var positions = MinionManager.GetMinions(Variables.Spells[SpellSlot.Q].Range,
                    MinionTypes.All, MinionTeam.NotAlly,
                    MinionOrderTypes.MaxHealth)
                    .Select(m => m.ServerPosition.LSTo2D()).ToList();


            if (Variables.Spells[SpellSlot.Q].IsEnabledAndReady(ModesMenuExtensions.Mode.Farm))
            {
                
                var lineFarmLocation = Variables.Spells[SpellSlot.Q].GetLineFarmLocation(positions);

                if (lineFarmLocation.MinionsHit >= Variables.AssemblyMenu.GetItemValue<Slider>("dzaio.champion.sivir.farm.q.min").Value)
                {
                    Variables.Spells[SpellSlot.Q].Cast(lineFarmLocation.Position);
                }
            }

            if (Variables.Spells[SpellSlot.W].IsEnabledAndReady(ModesMenuExtensions.Mode.Farm))
            {
                if (positions.Count() >= Variables.AssemblyMenu.GetItemValue<Slider>("dzaio.champion.sivir.farm.q.min").Value)
                {
                    Variables.Spells[SpellSlot.W].Cast();
                }
            }
            
        }
    }
}
