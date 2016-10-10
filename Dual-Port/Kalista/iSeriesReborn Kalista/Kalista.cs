using System;
using System.Collections.Generic;
using System.Linq;
using iSeriesDZLib.Logging;
using iSeriesReborn.Champions.Kalista.Modules;
using iSeriesReborn.Champions.Kalista.Skills;
using iSeriesReborn.Utility;
using iSeriesReborn.Utility.Drawings;
using iSeriesReborn.Utility.MenuUtility;
using iSeriesReborn.Utility.ModuleHelper;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.Kalista
{
    /// <summary>
    /// Kalista champion main class
    /// </summary>
    class Kalista : ChampionBase
    {
        //TODO Damage Bar indicator.
        //Orbwalk minions to reach target

        private Dictionary<SpellSlot, Spell> spells = new Dictionary<SpellSlot, Spell>()
        {
            { SpellSlot.Q, new Spell(SpellSlot.Q, 1150) }, 
            { SpellSlot.W, new Spell(SpellSlot.W, 5200) }, 
            { SpellSlot.E, new Spell(SpellSlot.E, 950) }, 
            { SpellSlot.R, new Spell(SpellSlot.R, 1200) }
        };

        protected override void OnChampLoad()
        {
            spells[SpellSlot.Q].SetSkillshot(0.25f, 40f, 1200f, true, SkillshotType.SkillshotLine);
            spells[SpellSlot.R].SetSkillshot(0.50f, 1500, float.MaxValue, false, SkillshotType.SkillshotCircle);

            DamageIndicator.DamageToUnit = KalistaE.GetRendDamage;
            DamageIndicator.Enabled = true;

            EloBuddy.Player.OnIssueOrder += KalistaHooks.OnIssueOrder;
            Obj_AI_Base.OnSpellCast += KalistaHooks.OnProcessSpellCast;
            Orbwalking.OnNonKillableMinion += KalistaHooks.OnNonKillableMinion;
            AntiGapcloser.OnEnemyGapcloser += KalistaAGP.OnGapclose;
        }

        protected override void LoadMenu()
        {
            
            var defaultMenu = Variables.Menu;

            var comboMenu = defaultMenu.AddModeMenu(Orbwalking.OrbwalkingMode.Combo);
            {
                comboMenu.AddSkill(SpellSlot.Q, Orbwalking.OrbwalkingMode.Combo, true, 15);
                comboMenu.AddSkill(SpellSlot.E, Orbwalking.OrbwalkingMode.Combo, true, 10);
                comboMenu.AddSlider("iseriesr.kalista.combo.e.minstacks", "Min. Stacks for E (Leave/Expire)", 9, 1, 15).SetTooltip("The min number of stacks to use E when target is about to leave the range or the rend buff is about to expire.");
            }

            var mixedMenu = defaultMenu.AddModeMenu(Orbwalking.OrbwalkingMode.Mixed);
            {
                mixedMenu.AddSkill(SpellSlot.Q, Orbwalking.OrbwalkingMode.Mixed, true, 15);
                mixedMenu.AddSkill(SpellSlot.E, Orbwalking.OrbwalkingMode.Mixed, true, 10);
                mixedMenu.AddSlider("iseriesr.kalista.mixed.e.minstacks", "Min. Stacks for E (Leave/Expire)", 9, 1, 15).SetTooltip("The min number of stacks to use E when target is about to leave the range or the rend buff is about to expire.");
            }

            var laneclearMenu = defaultMenu.AddModeMenu(Orbwalking.OrbwalkingMode.LaneClear);
            {
                laneclearMenu.AddSkill(SpellSlot.Q, Orbwalking.OrbwalkingMode.LaneClear, true, 50);
                laneclearMenu.AddSkill(SpellSlot.E, Orbwalking.OrbwalkingMode.LaneClear, true, 50);
            }

            var miscMenu = defaultMenu.AddSubMenu(new Menu("[iSR] Misc", "iseriesr.kalista.misc"));
            {
                miscMenu.AddBool("iseriesr.kalista.misc.steale", "Steal Drake / Baron with E", true).SetTooltip("Will use E to secure Dragon / Baron.");
                miscMenu.AddBool("iseriesr.kalista.misc.kse", "KS With E", true).SetTooltip("Will use E to KS enemies.");
                miscMenu.AddBool("iseriesr.kalista.misc.lhassit", "Last Hit Assist", true).SetTooltip("Will use E to secure minions you can't secure normally.");
                miscMenu.AddBool("iseriesr.kalista.misc.edeath", "Use E Before Death", true).SetTooltip("Will use E just before death to assure you will not die in vain :roto2:.");
                miscMenu.AddBool("iseriesr.kalista.misc.useeslow", "Use E for slow", true).SetTooltip("Will kill enemy minions with E to slow enemy when it's possible to reset E");
                miscMenu.AddBool("iseriesr.kalista.misc.savesoulbound", "Save Soulbound with R", true).SetTooltip("Will use R to save soulbound.");
                miscMenu.AddKeybind("iseriesr.kalista.misc.walljump", "Walljump", new Tuple<uint, KeyBindType>('Z', KeyBindType.Press)).SetTooltip("Will flee to a position and use Q to walljump. Position near the wall and magic will happen.");
            }

        }

        protected override void OnTick()
        {
            if (KalistaHooks.SoulBound == null)
            {
                KalistaHooks.SoulBound =
                    HeroManager.Allies.Find(
                        h => h.Buffs.Any(b => b.Caster.IsMe && b.Name.Contains("kalistacoopstrikeally")));
            }
        }

        protected override void OnCombo()
        {
            KalistaQ.ExecuteComboLogic();
            KalistaE.ExecuteComboLogic();
        }

        protected override void OnMixed()
        {
            KalistaQ.ExecuteComboLogic();
            KalistaE.ExecuteComboLogic();
        }

        protected override void OnLastHit(){ }

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
                new KalistaMobStealer(),
                new KalistaEKs(),
                new KalistaESlow(),
                new KalistaEDeath(),
                new KalistaWalljump(),
                new KalistaSoulboundSaver()
            };
        }
    }
}
