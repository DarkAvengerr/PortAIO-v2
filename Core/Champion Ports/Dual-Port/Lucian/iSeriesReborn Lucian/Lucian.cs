using System.Collections.Generic;
using iSeriesDZLib.Logging;
using iSeriesReborn.Champions.Lucian;
using iSeriesReborn.Utility;
using iSeriesReborn.Utility.MenuUtility;
using iSeriesReborn.Utility.ModuleHelper;
using LeagueSharp;
using LeagueSharp.Common;
using iSeriesReborn.Champions.Lucian.Skills;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.Kalista
{
    /// <summary>
    /// Lucian champion main class
    /// </summary>
    class Lucian : ChampionBase
    {
        //TODO Damage Bar indicator.

        private Dictionary<SpellSlot, Spell> spells = new Dictionary<SpellSlot, Spell>()
        {
            { SpellSlot.Q, new Spell(SpellSlot.Q, 675f) },
            { SpellSlot.W, new Spell(SpellSlot.W, 1000f) },
            { SpellSlot.E, new Spell(SpellSlot.E, 425f) },
            { SpellSlot.R, new Spell(SpellSlot.R, 1400f) }
        };

        public Spell qExtended = new Spell(SpellSlot.Q, 1100);

        protected override void OnChampLoad()
        {
            spells[SpellSlot.Q].SetTargetted(0.25f, float.MaxValue);
            qExtended = new Spell(SpellSlot.Q, 1100);
            qExtended.SetSkillshot(0.25f, 5f, float.MaxValue, true, SkillshotType.SkillshotLine);
            spells[SpellSlot.W].SetSkillshot(0.3f, 80, 1600, true, SkillshotType.SkillshotLine);
            spells[SpellSlot.E].SetSkillshot(.25f, 1f, float.MaxValue, false, SkillshotType.SkillshotLine);
            spells[SpellSlot.R].SetSkillshot(.1f, 110, 2800, true, SkillshotType.SkillshotLine);
            Spellbook.OnCastSpell += LucianHooks.OnCastSpell;
            Obj_AI_Base.OnSpellCast += LucianHooks.OnSpellCast;
        }

        protected override void LoadMenu()
        {

            var defaultMenu = Variables.Menu;

            var comboMenu = defaultMenu.AddModeMenu(Orbwalking.OrbwalkingMode.Combo);
            {
                comboMenu.AddSkill(SpellSlot.Q, Orbwalking.OrbwalkingMode.Combo, true, 10);
                comboMenu.AddSkill(SpellSlot.W, Orbwalking.OrbwalkingMode.Combo, true, 25);
                comboMenu.AddSkill(SpellSlot.E, Orbwalking.OrbwalkingMode.Combo, true, 5);
                //comboMenu.AddSkill(SpellSlot.R, Orbwalking.OrbwalkingMode.Combo, true, 10);
            }

            var mixedMenu = defaultMenu.AddModeMenu(Orbwalking.OrbwalkingMode.Mixed);
            {
                mixedMenu.AddSkill(SpellSlot.Q, Orbwalking.OrbwalkingMode.Combo, true, 15);
                mixedMenu.AddSkill(SpellSlot.W, Orbwalking.OrbwalkingMode.Mixed, true, 15);
                mixedMenu.AddSkill(SpellSlot.E, Orbwalking.OrbwalkingMode.Mixed, true, 15);
            }

            var laneclearMenu = defaultMenu.AddModeMenu(Orbwalking.OrbwalkingMode.LaneClear);
            {
                laneclearMenu.AddSkill(SpellSlot.Q, Orbwalking.OrbwalkingMode.LaneClear, true, 50);
            }

            var miscMenu = defaultMenu.AddSubMenu(new Menu("[iSR] Misc", "iseriesr.lucian.misc"));
            {
                miscMenu.AddBool("iseriesr.lucian.misc.eks", "E KS");
            }

        }

        protected override void OnTick()
        {

        }

        protected override void OnCombo()
        {
            
        }

        protected override void OnMixed()
        {

        }

        protected override void OnLastHit() { }

        protected override void OnLaneClear()
        {
            Farm.ExecuteLogic();
        }

        protected override void OnAfterAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            LucianHooks.OnAfterAA(sender, args);
        }

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
