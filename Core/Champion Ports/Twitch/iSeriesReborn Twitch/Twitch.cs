using System;
using System.Collections.Generic;
using System.Linq;
using iSeriesDZLib.Logging;
using iSeriesReborn.Champions.Kalista.Modules;
using iSeriesReborn.Champions.Kalista.Skills;
using iSeriesReborn.Champions.Twitch.Skills;
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
    /// Twitch champion main class
    /// </summary>
    class Twitch : ChampionBase
    {
        //TODO Damage Bar indicator.

        private Dictionary<SpellSlot, Spell> spells = new Dictionary<SpellSlot, Spell>()
        {
            { SpellSlot.Q, new Spell(SpellSlot.Q) }, 
            { SpellSlot.W, new Spell(SpellSlot.W, 950) }, 
            { SpellSlot.E, new Spell(SpellSlot.E, 1200) }, 
            { SpellSlot.R, new Spell(SpellSlot.R) }
        };

        protected override void OnChampLoad()
        {
            spells[SpellSlot.W].SetSkillshot(0.25f, 120f, 1400f, false, SkillshotType.SkillshotCircle);

        }

        protected override void LoadMenu()
        {

            var defaultMenu = Variables.Menu;

            var comboMenu = defaultMenu.AddModeMenu(Orbwalking.OrbwalkingMode.Combo);
            {
                comboMenu.AddSkill(SpellSlot.W, Orbwalking.OrbwalkingMode.Combo, true, 15);
                comboMenu.AddSkill(SpellSlot.E, Orbwalking.OrbwalkingMode.Combo, true, 10);
            }

            var mixedMenu = defaultMenu.AddModeMenu(Orbwalking.OrbwalkingMode.Mixed);
            {
                mixedMenu.AddSkill(SpellSlot.W, Orbwalking.OrbwalkingMode.Mixed, true, 15);
                mixedMenu.AddSkill(SpellSlot.E, Orbwalking.OrbwalkingMode.Mixed, true, 10);
            }

            var laneclearMenu = defaultMenu.AddModeMenu(Orbwalking.OrbwalkingMode.LaneClear);
            {
                laneclearMenu.AddSkill(SpellSlot.E, Orbwalking.OrbwalkingMode.LaneClear, true, 50);
            }

            var miscMenu = defaultMenu.AddSubMenu(new Menu("[iSR] Misc", "iseriesr.twitch.misc"));
            {
                miscMenu.AddBool("iseriesr.twitch.misc.steale", "Steal Drake / Baron with E", true).SetTooltip("Will use E to secure Dragon / Baron.");
                miscMenu.AddBool("iseriesr.twitch.misc.kse", "KS With E", true).SetTooltip("Will use E to KS enemies.");
            }

        }

        protected override void OnTick()
        {
            
        }

        protected override void OnCombo()
        {
            TwitchE.ExecuteLogic();
            TwitchW.OnExecute();
        }

        protected override void OnMixed()
        {
            TwitchE.ExecuteLogic();
            TwitchW.OnExecute();
        }

        protected override void OnLastHit() { }

        protected override void OnLaneClear()
        {

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
                new TwitchEKS(),
                new TwitchESteal()
            };
        }
    }
}
