using System;
using System.Collections.Generic;
using System.Drawing;
using DZLib.Logging;
using iSeriesReborn.Champions.Vayne.Modules;
using iSeriesReborn.Champions.Vayne.Skills;
using iSeriesReborn.Champions.Vayne.Utility;
using iSeriesReborn.Utility;
using iSeriesReborn.Utility.MenuUtility;
using iSeriesReborn.Utility.ModuleHelper;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.Vayne
{
    class Vayne : ChampionBase
    {
        private readonly Dictionary<SpellSlot, Spell> spells = new Dictionary<SpellSlot, Spell>()
        {
            { SpellSlot.Q, new Spell(SpellSlot.Q) },
            { SpellSlot.W, new Spell(SpellSlot.W) },
            { SpellSlot.E, new Spell(SpellSlot.E, 590f) },
            { SpellSlot.R, new Spell(SpellSlot.R) }
        };

        protected override void OnChampLoad()
        {
            spells[SpellSlot.E].SetTargetted(0.25f, 1250f);
            AntiGapcloser.OnEnemyGapcloser += VayneHooks.OnGapCloser;
            Interrupter2.OnInterruptableTarget += VayneHooks.OnInterrupt;
            Orbwalking.BeforeAttack += VayneHooks.BeforeAttack;
            Orbwalking.OnNonKillableMinion += VayneHooks.OnNonKillableMinion;
        }

        protected override void LoadMenu()
        {
            var defaultMenu = Variables.Menu;

            var comboMenu = defaultMenu.AddModeMenu(Orbwalking.OrbwalkingMode.Combo);
            {
                comboMenu.AddSkill(SpellSlot.Q, Orbwalking.OrbwalkingMode.Combo, true, 15);
                comboMenu.AddSkill(SpellSlot.E, Orbwalking.OrbwalkingMode.Combo, true, 5);
                comboMenu.AddSkill(SpellSlot.R, Orbwalking.OrbwalkingMode.Combo, false, 10);
                comboMenu.AddSlider("iseriesr.vayne.combo.r.minen", "Min. Enemies for R", 2, 1, 5);
            }

            var mixedMenu = defaultMenu.AddModeMenu(Orbwalking.OrbwalkingMode.Mixed);
            {
                mixedMenu.AddSkill(SpellSlot.Q, Orbwalking.OrbwalkingMode.Mixed, true, 15);
                mixedMenu.AddSkill(SpellSlot.E, Orbwalking.OrbwalkingMode.Mixed, true, 5);
            }

            var laneclearMenu = defaultMenu.AddModeMenu(Orbwalking.OrbwalkingMode.LaneClear);
            {
                laneclearMenu.AddSkill(SpellSlot.Q, Orbwalking.OrbwalkingMode.LaneClear, true, 50);
            }

            var miscMenu = defaultMenu.AddSubMenu(new Menu("[iSR] Misc", "iseriesr.vayne.misc"));
            {
                miscMenu.AddKeybind("iseriesr.vayne.misc.noaastealthex", "Don't AA while stealthed", new Tuple<uint, KeyBindType>('K', KeyBindType.Toggle)).SetTooltip("Will not AA while you are in Ult+Q"); //Done
                miscMenu.AddBool("iseriesr.vayne.misc.qinrange", "Q For KS", true).SetTooltip("Uses Q to KS by Qing in range if you can kill with Q + AA"); //Done
                miscMenu.AddBool("iseriesr.vayne.misc.qe", "QE", true).SetTooltip("Uses Q to get in position for E"); //Done
                miscMenu.AddItem(new MenuItem("aaaaasep1", "E Settings").SetFontStyle(FontStyle.Bold, SharpDX.Color.Red));
                miscMenu.AddSlider("iseriesr.vayne.misc.condemn.acc", "Accuracy", 45, 1, 60);
                miscMenu.AddSlider("iseriesr.vayne.misc.condemn.pushdist", "Push Distance", 390, 370, 475);
                miscMenu.AddBool("iseriesr.vayne.misc.condemn.autoe", "Auto E").SetTooltip("Uses E whenever possible"); //Done
                miscMenu.AddBool("iseriesr.vayne.misc.condemn.wardbush", "Ward Bush on Condemn").SetTooltip("Ward Bush when target ends there."); //Done
                miscMenu.AddItem(new MenuItem("aaaaasep2", "Misc Settings").SetFontStyle(FontStyle.Bold, SharpDX.Color.Red));
                miscMenu.AddBool("iseriesr.vayne.misc.general.antigp", "Anti Gapcloser", true).SetTooltip("Uses E to stop gapclosers");
                miscMenu.AddBool("iseriesr.vayne.misc.general.interrupter", "Interrupter", true).SetTooltip("Uses E to interrupt skills.");
                miscMenu.AddBool("iseriesr.vayne.misc.general.focus2w", "Focus 2W Stacks", true).SetTooltip("Focus Targets with 2W marks");
            }
        }

        protected override void OnTick()
        {
        }

        protected override void OnCombo()
        {
            VayneE.HandleELogic();
        }

        protected override void OnMixed()
        {

        }

        protected override void OnLastHit(){ }

        protected override void OnLaneClear()
        {

        }

        protected override void OnAfterAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            switch (Variables.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                case Orbwalking.OrbwalkingMode.Mixed:
                    VayneQ.HandleQLogic(args.Target as Obj_AI_Base);
                    break;
            }
        }

        public override Dictionary<SpellSlot, Spell> GetSpells()
        {
            return spells;
        }

        public override List<IModule> GetModules()
        {
            return new List<IModule>()
            {
                new VayneFocus2W(),
                new VayneAutoE(),
                new VayneQKS()
            };
        }
    }
}
