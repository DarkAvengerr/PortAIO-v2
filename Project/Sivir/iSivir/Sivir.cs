using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using DZLib.MenuExtensions;

using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSivir
{
    class Sivir
    {
        #region Static Fields

        /// <summary>
        ///     The dictionary to call the Spell slot and the Spell Class
        /// </summary>
        public static readonly Dictionary<SpellSlot, Spell> Spells = new Dictionary<SpellSlot, Spell>
                                                                         {
                                                                             {
                                                                                 SpellSlot.Q, 
                                                                                 new Spell(SpellSlot.Q, 1100f)
                                                                             }, 
                                                                             { SpellSlot.W, new Spell(SpellSlot.W) }, 
                                                                             { SpellSlot.E, new Spell(SpellSlot.E) }, 
                                                                         };

        #endregion

        #region Fields

        private Menu menu;

        private Orbwalking.Orbwalker orbwalker;

        private static readonly List<DangerousSpell> DangerousSpells = new List<DangerousSpell>
                                                                           {
                                                                               new DangerousSpell
                                                                                   {
                                                                                       ChampionName = "Vayne", 
                                                                                       Delay = 0.25f, 
                                                                                       SpellName = "VayneCondemn", 
                                                                                       Slot = SpellSlot.E, 
                                                                                       IsTargetMissle = false
                                                                                   }, 
                                                                               new DangerousSpell
                                                                                   {
                                                                                       ChampionName = "Galio", 
                                                                                       Delay = 0.25f, SpellName = "GalioR", 
                                                                                       Slot = SpellSlot.R, 
                                                                                       IsTargetMissle = false
                                                                                   }, 
                                                                               new DangerousSpell
                                                                                   {
                                                                                       ChampionName = "Amumu", 
                                                                                       Delay = 0.25f, SpellName = "AmumuR", 
                                                                                       Slot = SpellSlot.R, 
                                                                                       IsTargetMissle = false
                                                                                   }, 
                                                                               new DangerousSpell
                                                                                   {
                                                                                       ChampionName = "Rammus", 
                                                                                       Delay = 0.25f, 
                                                                                       SpellName = "rammusE", 
                                                                                       Slot = SpellSlot.E, 
                                                                                       IsTargetMissle = false
                                                                                   }, 
                                                                               new DangerousSpell
                                                                                   {
                                                                                       ChampionName = "Skarner", 
                                                                                       Delay = 0.25f, 
                                                                                       SpellName = "skarnerR", 
                                                                                       Slot = SpellSlot.R, 
                                                                                       IsTargetMissle = false
                                                                                   }, 
                                                                               new DangerousSpell
                                                                                   {
                                                                                       ChampionName = "Caitlyn", 
                                                                                       Delay = 0.25f, 
                                                                                       SpellName =
                                                                                           "CaitlynAceintheHoleMissile", 
                                                                                       Slot = SpellSlot.R, 
                                                                                       IsTargetMissle = true
                                                                                   }, 
                                                                               new DangerousSpell
                                                                                   {
                                                                                       ChampionName = "Tristana", 
                                                                                       Delay = 0.25f, 
                                                                                       SpellName = "TristanaR", 
                                                                                       Slot = SpellSlot.R, 
                                                                                       IsTargetMissle = false
                                                                                   }, 
                                                                               new DangerousSpell
                                                                                   {
                                                                                       ChampionName = "Syndra", 
                                                                                       Delay = 0.25f, 
                                                                                       SpellName = "SyndraR", 
                                                                                       Slot = SpellSlot.R, 
                                                                                       IsTargetMissle = false
                                                                                   }, 
                                                                               new DangerousSpell
                                                                                   {
                                                                                       ChampionName = "Alistar", 
                                                                                       Delay = 0f, SpellName = "Pulverize", 
                                                                                       Slot = SpellSlot.Q, 
                                                                                       IsTargetMissle = false
                                                                                   }, 
                                                                               new DangerousSpell
                                                                                   {
                                                                                       ChampionName = "Nocturne", 
                                                                                       Delay = 500f, 
                                                                                       SpellName =
                                                                                           "NocturneUnspeakableHorror", 
                                                                                       Slot = SpellSlot.E, 
                                                                                       IsTargetMissle = false
                                                                                   }
                                                                           };

        #endregion

        internal void OnLoad()
        {
            if (ObjectManager.Player.ChampionName != "Sivir") return;

            LoadSpells();
            LoadMenu();

            Game.OnUpdate += OnUpdate;
            Orbwalking.AfterAttack += OnAfterAttack;
            Obj_AI_Base.OnProcessSpellCast += OnSpellCast;
            GameObject.OnCreate += OnCreateObject;
        }

        private void OnCreateObject(GameObject sender, EventArgs arguments)
        {
            if (!(sender is MissileClient) || !sender.IsValid) return;
            var args = (MissileClient)sender;

            if (args.SData.Name != "CaitlynAceintheHoleMissile" || !args.Target.IsMe) return;

            if (Spells[SpellSlot.E].IsReady() && menu.Item("com.isivir.combo.useE").GetValue<bool>()
                && menu.Item("CaitlynAceintheHoleMissile").GetValue<bool>())
            {
                LeagueSharp.Common.Utility.DelayAction.Add(
                    (int)(args.StartPosition.Distance(ObjectManager.Player.Position) / 2000f + Game.Ping / 2f), 
                    () => Spells[SpellSlot.E].Cast());
            }
        }

        private void OnSpellCast(Obj_AI_Base sender1, GameObjectProcessSpellCastEventArgs args)
        {
            var sender = sender1 as AIHeroClient;

            if (sender == null || sender.IsMe || sender.IsAlly || !args.Target.IsMe || !Spells[SpellSlot.E].IsReady()
                || !menu.Item("com.isivir.combo.useE").GetValue<bool>() || args.SData.IsAutoAttack()
                || ObjectManager.Player.IsInvulnerable) return;

            if (sender.GetSpellDamage(ObjectManager.Player, args.Slot) >= ObjectManager.Player.Health
                && args.SData.TargettingType == SpellDataTargetType.Self)
            {
                LeagueSharp.Common.Utility.DelayAction.Add((int)0.25f, () => Spells[SpellSlot.E].Cast());
            }

            foreach (var spell in DangerousSpells)
            {
                if (sender.ChampionName == spell.ChampionName && args.SData.Name == spell.SpellName
                    && args.Slot == spell.Slot && !spell.IsTargetMissle)
                {
                    LeagueSharp.Common.Utility.DelayAction.Add((int)spell.Delay, () => Spells[SpellSlot.E].Cast());
                }
            }
        }

        private void OnUpdate(EventArgs args)
        {
            switch (orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Laneclear();
                    break;
            }

            foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells[SpellSlot.Q].Range)))
            {
                if (menu.Item("com.isivir.misc.qImmobile").GetValue<bool>() && Spells[SpellSlot.Q].IsReady())
                {
                    Spells[SpellSlot.Q].CastIfHitchanceEquals(target, HitChance.Immobile);
                }
            }
        }

        private void OnAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (!unit.IsMe || !unit.IsValid) return;

            switch (orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    if (menu.Item("com.isivir.combo.useW").GetValue<bool>() && target.IsValid<AIHeroClient>())
                    {
                        if (ObjectManager.Player.GetAutoAttackDamage((AIHeroClient)target, true)
                            * menu.Item("com.isivir.miscc.noW").GetValue<Slider>().Value > target.Health) return;

                        Spells[SpellSlot.W].Cast();
                    }

                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    if (menu.Item("com.isivir.harass.useW").GetValue<bool>() && target.IsValid<AIHeroClient>()
                        && ObjectManager.Player.ManaPercent
                        >= menu.Item("com.isivir.harass.mana").GetValue<Slider>().Value)
                    {
                        Spells[SpellSlot.W].Cast();
                    }

                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    if (menu.Item("com.isivir.laneclear.useW").GetValue<bool>() && target.IsValid<Obj_AI_Minion>()
                        && ObjectManager.Player.ManaPercent
                        >= menu.Item("com.isivir.laneclear.mana").GetValue<Slider>().Value)
                    {
                        Spells[SpellSlot.W].Cast();
                    }

                    break;
            }
        }

        private void Combo()
        {
            var useQ = menu.Item("com.isivir.combo.useQ").GetValue<bool>();
            var target = TargetSelector.GetTarget(Spells[SpellSlot.Q].Range, TargetSelector.DamageType.Physical);

            if (!Spells[SpellSlot.Q].IsReady() || !useQ) return;

            var prediction = Spells[SpellSlot.Q].GetPrediction(target);
            if (prediction.Hitchance >= HitChance.VeryHigh)
            {
                Spells[SpellSlot.Q].Cast(prediction.CastPosition);
            }
        }

        private void Harass()
        {
            var useQ = menu.Item("com.isivir.harass.useQ").GetValue<bool>();
            var target = TargetSelector.GetTarget(Spells[SpellSlot.Q].Range, TargetSelector.DamageType.Physical);

            if (!Spells[SpellSlot.Q].IsReady() || !useQ
                || ObjectManager.Player.ManaPercent < menu.Item("com.isivir.harass.mana").GetValue<Slider>().Value) return;

            var prediction = Spells[SpellSlot.Q].GetPrediction(target);
            if (prediction.Hitchance >= HitChance.High)
            {
                Spells[SpellSlot.Q].Cast(prediction.CastPosition);
            }
        }

        private void Laneclear()
        {
            if (!Spells[SpellSlot.Q].IsReady() || !menu.Item("com.isivir.laneclear.useQ").GetValue<bool>()
                || ObjectManager.Player.ManaPercent < menu.Item("com.isivir.laneclear.mana").GetValue<Slider>().Value) return;

            var minions = MinionManager.GetMinions(ObjectManager.Player.Position, Spells[SpellSlot.Q].Range);
            var farmLocation = Spells[SpellSlot.Q].GetLineFarmLocation(minions);

            if (farmLocation.MinionsHit >= menu.Item("com.isivir.laneclear.qMin").GetValue<Slider>().Value)
            {
                Spells[SpellSlot.Q].Cast(farmLocation.Position);
            }
        }

        private void LoadMenu()
        {
            menu = new Menu("iSivir", "com.isivir", true).SetFontStyle(FontStyle.Bold, SharpDX.Color.AliceBlue);

            var owMenu = new Menu(":: iSivir - Orbwalker", "com.isivir.orbwalker");
            {
                orbwalker = new Orbwalking.Orbwalker(owMenu);
                menu.AddSubMenu(owMenu);
            }

            var comboMenu = new Menu(":: iSivir - Combo Options", "com.isivir.combo");
            {
                comboMenu.AddBool("com.isivir.combo.useQ", "Use Q", true);
                comboMenu.AddBool("com.isivir.combo.useW", "Use W", true);
                comboMenu.AddBool("com.isivir.combo.useE", "Use E for targetted spells", true);
                var autoShield = new Menu(":: Auto Shield", "com.isivir.combo.autoShield");
                {
                    foreach (var spell in DangerousSpells)
                    {
                        if (HeroManager.Enemies.Any(x => x.ChampionName == spell.ChampionName))
                        {
                            autoShield.AddBool(
                                spell.SpellName, 
                                spell.ChampionName + ": " + spell.SpellName + " (" + spell.Slot + ")", 
                                true);
                        }
                    }

                    comboMenu.AddSubMenu(autoShield);
                }

                menu.AddSubMenu(comboMenu);
            }

            var harassMenu = new Menu(":: iSivir - Harass Options", "com.isivir.harass");
            {
                harassMenu.AddBool("com.isivir.harass.useQ", "Use Q", true);
                harassMenu.AddBool("com.isivir.harass.useW", "Use W", true);
                harassMenu.AddSlider("com.isivir.harass.mana", "Min Mana %", 70, 0, 100);
                menu.AddSubMenu(harassMenu);
            }

            var laneclearMenu = new Menu(":: iSivir - Laneclear Options", "com.isivir.laneclear");
            {
                laneclearMenu.AddBool("com.isivir.laneclear.useQ", "Use Q", true);
                laneclearMenu.AddSlider("com.isivir.laneclear.qMin", "Min Minions for Q", 4, 0, 10);
                laneclearMenu.AddBool("com.isivir.laneclear.useW", "Use W", true);
                laneclearMenu.AddSlider("com.isivir.laneclear.mana", "Min Mana %", 70, 0, 100);
                menu.AddSubMenu(laneclearMenu);
            }

            var miscMenu = new Menu(":: iSivir - Misc Options", "com.isivir.misc");
            {
                miscMenu.AddBool("com.isivir.misc.qImmobile", "Auto Q Immobile", true);
                miscMenu.AddSlider("com.isivir.miscc.noW", "No W if x aa can kill", 1, 0, 10);
                menu.AddSubMenu(miscMenu);
            }

            var drawingMenu = new Menu(":: iSivir - Drawing Options", "com.isivir.drawing");
            {
                menu.AddSubMenu(drawingMenu);
            }

            menu.AddToMainMenu();
        }

        private void LoadSpells()
        {
            Spells[SpellSlot.Q].SetSkillshot(0.25f, 90f, 1350f, false, SkillshotType.SkillshotLine);
        }
    }

    internal class DangerousSpell
    {
        public string ChampionName { get; set; }

        public string SpellName { get; set; }

        public float Delay { get; set; }

        public SpellSlot Slot { get; set; }

        public bool IsTargetMissle { get; set; }
    }
}