using System;
using System.Collections.Generic;
using iSeriesDZLib.Logging;
using iSeriesReborn.Champions.Jinx.Modules;
using iSeriesReborn.Champions.Jinx.Skills;
using iSeriesReborn.Utility;
using iSeriesReborn.Utility.MenuUtility;
using iSeriesReborn.Utility.ModuleHelper;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.Jinx
{
    /// <summary>
    /// Jinx champion main class
    /// </summary>
    class Jinx : ChampionBase
    {
        private readonly Dictionary<SpellSlot, Spell> spells = new Dictionary<SpellSlot, Spell>()
        {
            { SpellSlot.Q, new Spell(SpellSlot.Q) },
            { SpellSlot.W, new Spell(SpellSlot.W, 1450f) },
            { SpellSlot.E, new Spell(SpellSlot.E, 900f) },
            { SpellSlot.R, new Spell(SpellSlot.R, 2000f) }
        };

        protected override void OnChampLoad()
        {
            spells[SpellSlot.W].SetSkillshot(0.6f, 60f, 3300f, true, SkillshotType.SkillshotLine);
            spells[SpellSlot.E].SetSkillshot(1.2f, 1f, 1750f, false, SkillshotType.SkillshotCircle);
            spells[SpellSlot.R].SetSkillshot(0.6f, 140f, 1700f, false, SkillshotType.SkillshotLine);
            Orbwalking.BeforeAttack += JinxQ.BeforeAttack;
            Obj_AI_Base.OnSpellCast += JinxE.OnProcessSpellCast;
            Spellbook.OnCastSpell += JinxHumanizer.OnCastSpell;
            AntiGapcloser.OnEnemyGapcloser += JinxE.OnGapcloser;
        }

        protected override void LoadMenu()
        {

            var defaultMenu = Variables.Menu;

            var comboMenu = defaultMenu.AddModeMenu(Orbwalking.OrbwalkingMode.Combo);
            {
                comboMenu.AddSkill(SpellSlot.Q, Orbwalking.OrbwalkingMode.Combo, true, 15);
                comboMenu.AddSkill(SpellSlot.W, Orbwalking.OrbwalkingMode.Combo, true, 10);
                comboMenu.AddSkill(SpellSlot.E, Orbwalking.OrbwalkingMode.Combo, true, 25);
                comboMenu.AddSkill(SpellSlot.R, Orbwalking.OrbwalkingMode.Combo, true, 10);
            }

            var mixedMenu = defaultMenu.AddModeMenu(Orbwalking.OrbwalkingMode.Mixed);
            {
                mixedMenu.AddSkill(SpellSlot.Q, Orbwalking.OrbwalkingMode.Mixed, true, 15);
                mixedMenu.AddSkill(SpellSlot.W, Orbwalking.OrbwalkingMode.Mixed, true, 10);
            }

            var laneclearMenu = defaultMenu.AddModeMenu(Orbwalking.OrbwalkingMode.LaneClear);
            {
                laneclearMenu.AddSkill(SpellSlot.Q, Orbwalking.OrbwalkingMode.LaneClear, true, 50);
            }

            var miscMenu = defaultMenu.AddSubMenu(new Menu("[iSR] Misc", "iseriesr.jinx.misc"));
            {
                miscMenu.AddBool("iseriesr.jinx.q.switch.lhlc", "Always Switch to Minigun in Lasthit/Laneclear");
                miscMenu.AddBool("iseriesr.jinx.q.switch.noenemies", "Switch to Minigun if no enemies in range");
                miscMenu.AddBool("iseriesr.jinx.q.humanize", "Humanize Q swap").SetTooltip("Will not switch Q super rapidly to prevent you from getting called out.");
                miscMenu.AddBool("iseriesr.jinx.w.auto", "Auto W Slow/Immobile", true);
                miscMenu.AddBool("iseriesr.jinx.e.auto", "Auto E Slow/Immobile", true);
                miscMenu.AddBool("iseriesr.jinx.e.ops", "E On Process Spell", true).SetTooltip("Will use E on spells such as Thresh Q, Malz R, etc for best hitchance.");
                miscMenu.AddBool("iseriesr.jinx.e.agp", "Antigapcloser", true).SetTooltip("Will use E as an antigapcloser.");
                miscMenu.AddKeybind("iseriesr.jinx.r.manual", "Manual R",
                    new Tuple<uint, KeyBindType>('T', KeyBindType.Press));

            }

        }

        protected override void OnTick()
        {
        }

        protected override void OnCombo()
        {
            JinxQ.HandleQLogic();
            JinxW.HandleWLogic();
            JinxE.HandleELogic();
            JinxR.HandleRLogic();
        }

        protected override void OnMixed()
        {
            JinxQ.HandleQLogic();
            JinxW.HandleWLogic();
        }

        protected override void OnLastHit() { }

        protected override void OnLaneClear()
        {
            JinxQ.QSwapLC();
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
                new JinxAutoE(),
                new JinxAutoW(),
                new JinxManualR()
            };
        }
    }
}
