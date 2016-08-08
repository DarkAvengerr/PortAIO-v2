using System;
using System.Collections.Generic;
using System.Drawing;
using DZLib.Logging;
using iSeriesReborn.Champions.Tristana.Skills;
using iSeriesReborn.Utility;
using iSeriesReborn.Utility.MenuUtility;
using iSeriesReborn.Utility.ModuleHelper;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.Tristana
{
    class Tristana : ChampionBase
    {
        private readonly Dictionary<SpellSlot, Spell> spells = new Dictionary<SpellSlot, Spell>()
        {
            { SpellSlot.Q, new Spell(SpellSlot.Q) },
            { SpellSlot.E, new Spell(SpellSlot.E, 550) },
            { SpellSlot.R, new Spell(SpellSlot.R, 550) }
        };

        protected override void OnChampLoad()
        {
            AntiGapcloser.OnEnemyGapcloser += TristanaHooks.OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += TristanaHooks.OnInterruptableTarget;
        }

        protected override void LoadMenu()
        {
            var defaultMenu = Variables.Menu;

            var comboMenu = defaultMenu.AddModeMenu(Orbwalking.OrbwalkingMode.Combo);
            {
                var useEOnMenu = new Menu("Use E On", "iseriesr.tristana.combo.eon");
                {
                    useEOnMenu.AddChampMenu(true);
                    comboMenu.AddSubMenu(useEOnMenu);
                }

                comboMenu.AddSkill(SpellSlot.Q, Orbwalking.OrbwalkingMode.Combo, true, 15);
                comboMenu.AddSkill(SpellSlot.E, Orbwalking.OrbwalkingMode.Combo, true, 5);
                comboMenu.AddSkill(SpellSlot.R, Orbwalking.OrbwalkingMode.Combo, true, 10);
            }

            var mixedMenu = defaultMenu.AddModeMenu(Orbwalking.OrbwalkingMode.Mixed);
            {
                mixedMenu.AddSkill(SpellSlot.Q, Orbwalking.OrbwalkingMode.Mixed, true, 15);
                mixedMenu.AddSkill(SpellSlot.E, Orbwalking.OrbwalkingMode.Mixed, true, 5);
            }

            var laneclearMenu = defaultMenu.AddModeMenu(Orbwalking.OrbwalkingMode.LaneClear);
            {
                laneclearMenu.AddSkill(SpellSlot.E, Orbwalking.OrbwalkingMode.LaneClear, true, 50);
            }

            var miscMenu = defaultMenu.AddSubMenu(new Menu("[iSR] Misc", "iseriesr.tristana.misc"));
            {
                miscMenu.AddBool("iseriesr.tristana.misc.antigp", "Anti Gapcloser", true).SetTooltip("Uses R to stop gapclosers");
                miscMenu.AddBool("iseriesr.tristana.misc.interrupter", "Interrupter", true).SetTooltip("Uses R to interrupt skills.");
            }
            spells[SpellSlot.E].SetTargetted(0.25f, 2400);
        }

        protected override void OnTick()
        {

        }

        protected override void OnCombo()
        {
            TristanaQ.HandleLogic();
            TristanaE.HandleLogic();
            TristanaR.HandleLogic();
        }

        protected override void OnMixed()
        {
            TristanaQ.HandleLogic();
            TristanaE.HandleLogic();
        }

        protected override void OnLastHit() { }

        protected override void OnLaneClear()
        {
            TristanaE.HandleLaneclear();
        }

        protected override void OnAfterAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args){}

        public override Dictionary<SpellSlot, Spell> GetSpells()
        {
            return spells;
        }

        public override List<IModule> GetModules()
        {
            return new List<IModule>()
            {

            };
        }
    }
}
