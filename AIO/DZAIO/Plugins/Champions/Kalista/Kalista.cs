using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DZAIO_Reborn.Core;
using DZAIO_Reborn.Helpers;
using DZAIO_Reborn.Helpers.Entity;
using DZAIO_Reborn.Helpers.Modules;
using DZAIO_Reborn.Plugins.Champions.Kalista.Modules;
using DZAIO_Reborn.Plugins.Champions.Veigar.Modules;
using DZAIO_Reborn.Plugins.Interface;
using DZLib.Core;
using DZLib.Menu;
using DZLib.MenuExtensions;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SPrediction;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace DZAIO_Reborn.Plugins.Champions.Kalista
{
    class Kalista : IChampion
    {
        private static float LastQCastTick = 0f;
        private static float LastECastTick = 0f;

        public void OnLoad(Menu menu)
        {
            var comboMenu = new Menu(ObjectManager.Player.ChampionName + ": Combo", "dzaio.champion.kalista.combo");
            {
                comboMenu.AddModeMenu(ModesMenuExtensions.Mode.Combo, new[] { SpellSlot.Q, SpellSlot.E, SpellSlot.R }, new[] { true, true, true });
                comboMenu.AddSlider("dzaio.champion.kalista.combo.e.stacks", "Min E Stacks", 6, 1, 10);
                //comboMenu.AddNoUltiMenu(false);
                menu.AddSubMenu(comboMenu);
            }

            var mixedMenu = new Menu(ObjectManager.Player.ChampionName + ": Mixed", "dzaio.champion.kalista.harrass");
            {
                mixedMenu.AddModeMenu(ModesMenuExtensions.Mode.Harrass, new[] { SpellSlot.Q, SpellSlot.E }, new[] { true, true });
                mixedMenu.AddSlider("dzaio.champion.kalista.mixed.mana", "Min Mana % for Harass", 30, 0, 100);
                menu.AddSubMenu(mixedMenu);
            }

            var farmMenu = new Menu(ObjectManager.Player.ChampionName + ": Farm", "dzaio.champion.kalista.farm");
            {
                farmMenu.AddModeMenu(ModesMenuExtensions.Mode.Laneclear, new[] { SpellSlot.Q, SpellSlot.E }, new[] { true, true });

                farmMenu.AddSlider("dzaio.champion.kalista.farm.e.min", "Min Minions for E", 3, 1, 6);
                farmMenu.AddSlider("dzaio.champion.kalista.farm.mana", "Min Mana % for Farm", 30, 0, 100);
                menu.AddSubMenu(farmMenu);
            }

            var extraMenu = new Menu(ObjectManager.Player.ChampionName + ": Extra", "dzaio.champion.kalista.extra");
            {
                extraMenu.AddBool("dzaio.champion.kalista.extra.antigapcloser", "Antigapcloser (Q)", true);
                extraMenu.AddBool("dzaio.champion.kalista.extra.autoQ", "Auto Q Stunned / Rooted", true);
                extraMenu.AddBool("dzaio.champion.kalista.kalista.autoEKS", "Auto E KS", true);
                extraMenu.AddBool("dzaio.champion.kalista.kalista.autoESlow", "Auto E for Slow (With Reset on Minion)", true);
                extraMenu.AddBool("dzaio.champion.kalista.kalista.autoEDeath", "Auto E when about to die", true);
                extraMenu.AddBool("dzaio.champion.kalista.kalista.autoESteal", "Auto E to steal Drake and Baron", true);
                extraMenu.AddBool("dzaio.champion.kalista.kalista.autoRSoul", "Auto R to save Soulbound", true);
                extraMenu.AddKeybind("dzaio.champion.kalista.kalista.wallJump", "Wall Jump", new Tuple<uint, KeyBindType>('Z', KeyBindType.Press));
            }

            Variables.Spells[SpellSlot.Q].SetSkillshot(0.25f, 40f, 1200f, true, SkillshotType.SkillshotLine);
            Variables.Spells[SpellSlot.R].SetSkillshot(0.50f, 1500, float.MaxValue, false, SkillshotType.SkillshotCircle);

        }

        public void RegisterEvents()
        {
            DZInterrupter.OnInterruptableTarget += OnInterrupter;
            DZAntigapcloser.OnEnemyGapcloser += OnGapcloser;
        }

        private void OnGapcloser(DZLib.Core.ActiveGapcloser gapcloser)
        {
            if (Variables.AssemblyMenu.GetItemValue<bool>("dzaio.champion.kalista.extra.antigapcloser") 
                && Variables.Spells[SpellSlot.Q].LSIsReady())
            {
                Variables.Spells[SpellSlot.Q].Cast(gapcloser.End);
                LeagueSharp.Common.Utility.DelayAction.Add((int)(250f + Game.Ping / 2f + 30), () =>
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                });
            }
        }

        private void OnInterrupter(AIHeroClient sender, DZInterrupter.InterruptableTargetEventArgs args)
        {
            
        }

        public Dictionary<SpellSlot, Spell> GetSpells()
        {
            return new Dictionary<SpellSlot, Spell>
                      {
                            { SpellSlot.Q, new Spell(SpellSlot.Q, 1150) }, 
                            { SpellSlot.W, new Spell(SpellSlot.W, 5200) }, 
                            { SpellSlot.E, new Spell(SpellSlot.E, 950) }, 
                            { SpellSlot.R, new Spell(SpellSlot.R, 1200) }
                      };
        }

        public List<IModule> GetModules()
        {
            return new List<IModule>()
            {
                new KalistaEKS(),
                new KalistaESlow(),
                new KalistaEDeath(),
                new KalistaEDrakeBaron(),
                new KalistaSoulboundSaver(),
                new KalistaWallJump()
            };
        }

        public void OnTick()
        {

        }

        public void OnCombo()
        {

            /** Q Logic */

            if (Variables.Spells[SpellSlot.Q].IsEnabledAndReady(ModesMenuExtensions.Mode.Combo))
            {
                var qRangeReduction = 0.75f;
                var qJumpRange = 280f;
                var qTickInterval = 500f;


                var target = TargetSelector.GetTarget(
                    Variables.Spells[SpellSlot.Q].Range * qRangeReduction, TargetSelector.DamageType.Physical);

                if (target.LSIsValidTarget(Variables.Spells[SpellSlot.Q].Range * qRangeReduction))
                {
                    //Calculate the dash End Position and update the prediction accordingly
                    var dashEndPos = ObjectManager.Player.GetDashInfo().EndPos;
                    var QPrediction = Variables.Spells[SpellSlot.Q].GetPrediction(target);

                    if (dashEndPos != Vector2.Zero)
                    {
                        Variables.Spells[SpellSlot.Q].UpdateSourcePosition(dashEndPos.To3D());
                        QPrediction = Variables.Spells[SpellSlot.Q].GetPrediction(target);
                        Variables.Spells[SpellSlot.Q].UpdateSourcePosition(ObjectManager.Player.ServerPosition);
                    }

                    if (QPrediction.Hitchance >= HitChance.High)
                    {
                        if (IsPlayerNotBusy() && (Environment.TickCount - LastQCastTick >= qTickInterval))
                        {
                            Variables.Spells[SpellSlot.Q].Cast(QPrediction.CastPosition);
                            LastQCastTick = Environment.TickCount;
                        }
                    }
                    else if (target.CanBeRendKilled() && !target.LSIsValidTarget(Variables.Spells[SpellSlot.E].Range)
                      && target.LSIsValidTarget(Variables.Spells[SpellSlot.E].Range + qJumpRange))
                    {
                        if (IsPlayerNotBusy() && (Environment.TickCount - LastQCastTick >= qTickInterval))
                        {
                            Variables.Spells[SpellSlot.Q].Cast(QPrediction.CastPosition);
                            LastQCastTick = Environment.TickCount;
                        }
                    }
                }
            }

            /** E Logic */
            if (Variables.Spells[SpellSlot.E].IsEnabledAndReady(ModesMenuExtensions.Mode.Combo))
            {
                foreach (
                var hero in
                    HeroManager.Enemies.Where(
                        h =>  h.HasRend() && h.LSIsValidTarget(Variables.Spells[SpellSlot.E].Range)))
                {
                    if (hero.CanBeRendKilled() 
                        || hero.GetRendBuff().Count >= Variables.AssemblyMenu.GetItemValue<Slider>("dzaio.champion.kalista.combo.e.stacks").Value)
                    {
                        Variables.Spells[SpellSlot.E].Cast();
                    }
                }
            }
        }

        public bool IsPlayerNotBusy()
        {
            return !ObjectManager.Player.LSIsDashing() &&
                   (!ObjectManager.Player.Spellbook.IsAutoAttacking || ObjectManager.Player.Spellbook.IsAutoAttacking);
        }

        public void OnMixed()
        {

        }

        public void OnLastHit()
        { }

        public void OnLaneclear()
        {
            if (ObjectManager.Player.ManaPercent <
                Variables.AssemblyMenu.GetItemValue<Slider>("dzaio.champion.kalista.farm.mana").Value)
            {
                return;
            }

        }
    }
}
