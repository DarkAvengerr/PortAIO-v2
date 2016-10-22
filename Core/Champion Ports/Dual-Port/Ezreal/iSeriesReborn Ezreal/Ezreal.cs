using System.Collections.Generic;
using iSeriesDZLib.Logging;
using iSeriesReborn.Champions.Ezreal.Modules;
using iSeriesReborn.Champions.Ezreal.Skills;
using iSeriesReborn.Utility;
using iSeriesReborn.Utility.MenuUtility;
using iSeriesReborn.Utility.ModuleHelper;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.Kalista
{
    /// <summary>
    /// Ezreal champion main class
    /// </summary>
    class Ezreal : ChampionBase
    {
        //TODO Damage Bar indicator.

        private Dictionary<SpellSlot, Spell> spells = new Dictionary<SpellSlot, Spell>()
        {
            { SpellSlot.Q, new Spell(SpellSlot.Q, 1150) },
            { SpellSlot.W, new Spell(SpellSlot.W, 1000) },
            { SpellSlot.E, new Spell(SpellSlot.E, 475) },
            { SpellSlot.R, new Spell(SpellSlot.R, 3000f) }
        };

        protected override void OnChampLoad()
        {
            spells[SpellSlot.Q].SetSkillshot(0.25f, 60f, 2000f, true, SkillshotType.SkillshotLine);
            spells[SpellSlot.W].SetSkillshot(0.25f, 80f, 2000f, false, SkillshotType.SkillshotLine);
            spells[SpellSlot.R].SetSkillshot(1f, 160f, 2000f, false, SkillshotType.SkillshotLine);
        }

        protected override void LoadMenu()
        {

            var defaultMenu = Variables.Menu;

            var comboMenu = defaultMenu.AddModeMenu(Orbwalking.OrbwalkingMode.Combo);
            {
                comboMenu.AddSkill(SpellSlot.Q, Orbwalking.OrbwalkingMode.Combo, true, 15);
                comboMenu.AddSkill(SpellSlot.W, Orbwalking.OrbwalkingMode.Combo, true, 10);
                comboMenu.AddSkill(SpellSlot.R, Orbwalking.OrbwalkingMode.Combo, true, 10);
            }

            var mixedMenu = defaultMenu.AddModeMenu(Orbwalking.OrbwalkingMode.Mixed);
            {
                mixedMenu.AddSkill(SpellSlot.Q, Orbwalking.OrbwalkingMode.Combo, true, 15);
                mixedMenu.AddSkill(SpellSlot.W, Orbwalking.OrbwalkingMode.Mixed, true, 15);
            }

            var laneclearMenu = defaultMenu.AddModeMenu(Orbwalking.OrbwalkingMode.LaneClear);
            {
                laneclearMenu.AddSkill(SpellSlot.Q, Orbwalking.OrbwalkingMode.LaneClear, true, 50);
            }

            var miscMenu = defaultMenu.AddSubMenu(new Menu("[iSR] Misc", "iseriesr.ezreal.misc"));
            {
                miscMenu.AddBool("iseriesr.ezreal.misc.autoq.immobile", "Auto Q Immobile / KS");
            }

        }

        protected override void OnTick()
        {

        }

        protected override void OnCombo()
        {
            EzrealQ.ExecuteLogic();
            EzrealW.ExecuteLogic();
            EzrealR.ExecuteLogic();
        }

        protected override void OnMixed()
        {
            EzrealQ.ExecuteLogic();
            EzrealW.ExecuteLogic();
        }

        protected override void OnLastHit() { }

        protected override void OnLaneClear()
        {
            EzrealQ.ExecuteFarmLogic();
        }

        protected override void OnAfterAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {

        }

        public override Dictionary<SpellSlot, Spell> GetSpells()
        {
            return spells;
        }

        public override List<IModule> GetModules()
        {
            return new List<IModule>()
            {
                new EzrealAutoQ()
            };
        }
    }
}
