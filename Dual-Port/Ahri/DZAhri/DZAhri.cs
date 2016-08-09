using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace DZAhri
{
    class DZAhri
    {
        public static Menu Menu;
        public static Orbwalking.Orbwalker Orbwalker;
        public static readonly Dictionary<SpellSlot, Spell> _spells = new Dictionary<SpellSlot, Spell>
        {
            { SpellSlot.Q, new Spell(SpellSlot.Q, 925f) },
            { SpellSlot.W, new Spell(SpellSlot.W, 700f) },
            { SpellSlot.E, new Spell(SpellSlot.E, 875f) },
            { SpellSlot.R, new Spell(SpellSlot.R, 400f) }
        };
        private delegate void OnOrbwalkingMode();
        private static Dictionary<Orbwalking.OrbwalkingMode, OnOrbwalkingMode> _orbwalkingModesDictionary;

        public static void OnLoad()
        {
            if (ObjectManager.Player.ChampionName != "Ahri")
            {
                return;
            }
            _orbwalkingModesDictionary = new Dictionary<Orbwalking.OrbwalkingMode, OnOrbwalkingMode>
            {
                { Orbwalking.OrbwalkingMode.Combo, Combo },
                { Orbwalking.OrbwalkingMode.Mixed, Harass },
                { Orbwalking.OrbwalkingMode.LastHit, LastHit },
                { Orbwalking.OrbwalkingMode.LaneClear, Laneclear },
                { Orbwalking.OrbwalkingMode.None, () => { } }
            };
            SetUpMenu();
            SetUpSpells();
            SetUpEvents();
        }

        #region Modes Menu
        private static void Combo()
        {
            if (ObjectManager.Player.ManaPercent < Menu.Item("dz191.ahri.combo.mana").GetValue<Slider>().Value || ObjectManager.Player.IsDead)
            {
                return;
            }
            var comboTarget = TargetSelector.GetTarget(_spells[SpellSlot.E].Range, TargetSelector.DamageType.Magical);
            var charmedUnit = HeroManager.Enemies.Find(h => h.HasBuffOfType(BuffType.Charm) && h.IsValidTarget(_spells[SpellSlot.Q].Range));
            AIHeroClient target = comboTarget;
            if (charmedUnit != null)
            {
                target = charmedUnit;
            }
            if (target.IsValidTarget())
            {
                switch (Menu.Item("dz191.ahri.combo.mode").GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        if (!target.IsCharmed() && Helpers.IsMenuEnabled("dz191.ahri.combo.usee") && _spells[SpellSlot.E].IsReady() && _spells[SpellSlot.Q].IsReady())
                        {
                            _spells[SpellSlot.E].CastIfHitchanceEquals(target, HitChance.High);
                        }
                        if (Helpers.IsMenuEnabled("dz191.ahri.combo.useq") && _spells[SpellSlot.Q].IsReady() && (!_spells[SpellSlot.E].IsReady() || ObjectManager.Player.ManaPercent <= 25))
                        {
                            _spells[SpellSlot.Q].CastIfHitchanceEquals(target, HitChance.High);
                        }
                        if (Helpers.IsMenuEnabled("dz191.ahri.combo.usew") && _spells[SpellSlot.W].IsReady() && ObjectManager.Player.Distance(target) <= _spells[SpellSlot.W].Range && (target.IsCharmed() || (_spells[SpellSlot.W].GetDamage(target) + _spells[SpellSlot.Q].GetDamage(target) > target.Health + 25)))
                        {
                            _spells[SpellSlot.W].Cast();
                        }
                        break;
                    case 1:
                        if (!target.IsCharmed() && Helpers.IsMenuEnabled("dz191.ahri.combo.usee") && _spells[SpellSlot.E].IsReady() && _spells[SpellSlot.Q].IsReady())
                        {
                            _spells[SpellSlot.E].CastIfHitchanceEquals(target, HitChance.High);
                        }
                        if (Helpers.IsMenuEnabled("dz191.ahri.combo.useq") && _spells[SpellSlot.Q].IsReady())
                        {
                            _spells[SpellSlot.Q].CastIfHitchanceEquals(target, HitChance.High);
                        }
                        if (Helpers.IsMenuEnabled("dz191.ahri.combo.usew") && _spells[SpellSlot.W].IsReady() && ObjectManager.Player.Distance(target) <= _spells[SpellSlot.W].Range && ((_spells[SpellSlot.W].GetDamage(target) + _spells[SpellSlot.Q].GetDamage(target) > target.Health + 25)))
                        {
                            _spells[SpellSlot.W].Cast();
                        }
                        break;

                }
                HandleRCombo(target);
            }
        }

        private static void Harass()
        {
            if (ObjectManager.Player.ManaPercent < Menu.Item("dz191.ahri.harass.mana").GetValue<Slider>().Value || ObjectManager.Player.IsDead)
            {
                return;
            }
            var comboTarget = TargetSelector.GetTarget(_spells[SpellSlot.E].Range, TargetSelector.DamageType.Magical);
            var charmedUnit = HeroManager.Enemies.Find(h => h.IsCharmed() && h.IsValidTarget(_spells[SpellSlot.Q].Range));
            AIHeroClient target = comboTarget;
            if (charmedUnit != null)
            {
                target = charmedUnit;
            }
            if (target.IsValidTarget())
            {
                if (!target.IsCharmed() && Helpers.IsMenuEnabled("dz191.ahri.harass.usee") && _spells[SpellSlot.E].IsReady() && _spells[SpellSlot.Q].IsReady())
                {
                    _spells[SpellSlot.E].CastIfHitchanceEquals(target, HitChance.High);
                }
                if (Helpers.IsMenuEnabled("dz191.ahri.harass.useq") && _spells[SpellSlot.Q].IsReady())
                {
                    if (Helpers.IsMenuEnabled("dz191.ahri.harass.onlyqcharm") && !target.IsCharmed())
                    {
                        return;
                    }
                    _spells[SpellSlot.Q].CastIfHitchanceEquals(target, HitChance.High);
                }
                if (Helpers.IsMenuEnabled("dz191.ahri.harass.usew") && _spells[SpellSlot.W].IsReady() && ObjectManager.Player.Distance(target) <= _spells[SpellSlot.W].Range)
                {
                    _spells[SpellSlot.W].Cast();
                }
            }
        }

        private static void LastHit()
        {
            if (ObjectManager.Player.ManaPercent < Menu.Item("dz191.ahri.farm.mana").GetValue<Slider>().Value)
            {
                return;
            }
            if (Helpers.IsMenuEnabled("dz191.ahri.farm.qlh"))
            {
                var minionInQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _spells[SpellSlot.Q].Range).FindAll(m => _spells[SpellSlot.Q].GetDamage(m) >= m.Health).ToList();
                var killableMinions = _spells[SpellSlot.Q].GetLineFarmLocation(minionInQ);
                if (killableMinions.MinionsHit > 0)
                {
                    _spells[SpellSlot.Q].Cast(killableMinions.Position);
                }
            }
        }

        private static void Laneclear()
        {
            if (ObjectManager.Player.ManaPercent < Menu.Item("dz191.ahri.farm.mana").GetValue<Slider>().Value)
            {
                return;
            }
            if (Helpers.IsMenuEnabled("dz191.ahri.farm.qlc"))
            {
                var minionInQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _spells[SpellSlot.Q].Range);
                var killableMinions = _spells[SpellSlot.Q].GetLineFarmLocation(minionInQ);
                if (killableMinions.MinionsHit >= 3)
                {
                    _spells[SpellSlot.Q].Cast(killableMinions.Position);
                }
            }
            if (Helpers.IsMenuEnabled("dz191.ahri.farm.wlc"))
            {
                var minionInW = MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition, _spells[SpellSlot.W].Range);
                if (minionInW.Count > 0)
                {
                    _spells[SpellSlot.W].Cast();
                }
            }
        }
        private static void HandleRCombo(AIHeroClient target)
        {
            if (_spells[SpellSlot.R].IsReady() && Helpers.IsMenuEnabled("dz191.ahri.combo.user"))
            {
                //User chose not to initiate with R.
                if (Helpers.IsMenuEnabled("dz191.ahri.combo.initr"))
                {
                    return;
                }
                //Neither Q or E are ready in <= 2 seconds and we can't kill the enemy with 1 R stack. Don't use R
                if ((!_spells[SpellSlot.Q].IsReady(2) && !_spells[SpellSlot.E].IsReady(2)) || !(Helpers.GetComboDamage(target) >= target.Health + 20))
                {
                    return;
                }
                //Set the test position to the Cursor Position
                var testPosition = Game.CursorPos;
                //Extend from out position towards there
                var extendedPosition = ObjectManager.Player.Position.Extend(testPosition, 500f);
                //Safety checks
                if (extendedPosition.IsSafe())
                {
                    _spells[SpellSlot.R].Cast(extendedPosition);
                }
            }
        }
        #endregion

        #region Event delegates
        static void Game_OnUpdate(EventArgs args)
        {
            _orbwalkingModesDictionary[Orbwalker.ActiveMode]();
            if (Helpers.IsMenuEnabled("dz191.ahri.misc.userexpire"))
            {
                var rBuff = ObjectManager.Player.Buffs.Find(buff => buff.Name == "AhriTumble");
                if (rBuff != null)
                {
                    //This tryhard tho
                    if (rBuff.EndTime - Game.Time <= 1.0f + (Game.Ping / (2f * 1000f)))
                    {
                        var extendedPosition = ObjectManager.Player.Position.Extend(Game.CursorPos, _spells[SpellSlot.R].Range);
                        if (extendedPosition.IsSafe())
                        {
                            _spells[SpellSlot.R].Cast(extendedPosition);
                        }
                    }
                }
            }
            if (Helpers.IsMenuEnabled("dz191.ahri.misc.autoq"))
            {
                var charmedUnit = HeroManager.Enemies.Find(h => h.IsCharmed() && h.IsValidTarget(_spells[SpellSlot.Q].Range));
                if (charmedUnit != null)
                {
                    _spells[SpellSlot.Q].Cast(charmedUnit);
                }
            }
            if (Helpers.IsMenuEnabled("dz191.ahri.combo.autoq2"))
            {
                var qMana = Menu.Item("dz191.ahri.combo.autoq2mana").GetValue<Slider>().Value;
                if (ObjectManager.Player.ManaPercent >= qMana && _spells[SpellSlot.Q].IsReady())
                {
                    var target = TargetSelector.GetTarget(_spells[SpellSlot.Q].Range, TargetSelector.DamageType.Magical);
                    if (target != null && ObjectManager.Player.Distance(target) >= _spells[SpellSlot.Q].Range * 0.7f)
                    {
                        _spells[SpellSlot.Q].CastIfHitchanceEquals(target, HitChance.High);
                    }
                }
            }
            if (Menu.Item("dz191.ahri.misc.instacharm").GetValue<KeyBind>().Active && _spells[SpellSlot.E].IsReady())
            {
                var target = TargetSelector.GetTarget(_spells[SpellSlot.E].Range, TargetSelector.DamageType.Magical);
                if (target != null)
                {
                    var prediction = _spells[SpellSlot.E].GetPrediction(target);
                    _spells[SpellSlot.E].Cast(prediction.CastPosition);
                }
            }

        }
        static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Helpers.IsMenuEnabled("dz191.ahri.misc.egp") && gapcloser.Sender.IsValidTarget(_spells[SpellSlot.E].Range) && _spells[SpellSlot.E].IsReady())
            {
                _spells[SpellSlot.E].Cast(gapcloser.Sender);
            }
            if (Helpers.IsMenuEnabled("dz191.ahri.misc.rgap") && !_spells[SpellSlot.E].IsReady() &&
                _spells[SpellSlot.R].IsReady())
            {
                _spells[SpellSlot.R].Cast(ObjectManager.Player.ServerPosition.Extend(gapcloser.End, -400f));
            }
        }
        static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (Helpers.IsMenuEnabled("dz191.ahri.misc.eint") && args.DangerLevel >= Interrupter2.DangerLevel.Medium && _spells[SpellSlot.E].IsReady())
            {
                _spells[SpellSlot.E].Cast(sender.ServerPosition);
            }
        }
        static void Drawing_OnDraw(EventArgs args)
        {
            var qItem = Menu.Item("dz191.ahri.drawings.q").GetValue<Circle>();
            var eItem = Menu.Item("dz191.ahri.drawings.e").GetValue<Circle>();
            if (qItem.Active)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position,_spells[SpellSlot.Q].Range,qItem.Color);
            }
            if (eItem.Active)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _spells[SpellSlot.E].Range, eItem.Color);
            }
        }

        #endregion

        #region Events, Spells, Menu Init
        private static void SetUpEvents()
        {
            Game.OnUpdate += Game_OnUpdate;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void SetUpSpells()
        {
            _spells[SpellSlot.Q].SetSkillshot(0.25f, 100, 1600, false, SkillshotType.SkillshotLine);
            _spells[SpellSlot.E].SetSkillshot(0.25f, 60, 1200, true, SkillshotType.SkillshotLine);
        }

        private static void SetUpMenu()
        {
            Menu = new Menu("DZAhri", "dz191.ahri", true);
            var orbMenu = new Menu("[Ahri] Orbwalker", "dz191.ahri.orbwalker");
            Orbwalker = new Orbwalking.Orbwalker(orbMenu);
            Menu.AddSubMenu(orbMenu);
            var tsMenu = new Menu("[Ahri] Target Selector", "dz191.ahri.ts");
            TargetSelector.AddToMenu(tsMenu);
            Menu.AddSubMenu(tsMenu);
            var comboMenu = new Menu("[Ahri] Combo", "dz191.ahri.combo"); //Done
            {
                comboMenu.AddItem(new MenuItem("dz191.ahri.combo.useq", "Use Q Combo").SetValue(true));
                comboMenu.AddItem(new MenuItem("dz191.ahri.combo.usew", "Use W Combo").SetValue(true));
                comboMenu.AddItem(new MenuItem("dz191.ahri.combo.usee", "Use E Combo").SetValue(true));
                comboMenu.AddItem(new MenuItem("dz191.ahri.combo.user", "Use R Combo").SetValue(true));
                comboMenu.AddItem(new MenuItem("dz191.ahri.combo.initr", "Don't Initiate with R").SetValue(false)); //Done
                comboMenu.AddItem(new MenuItem("dz191.ahri.combo.mana", "Min Combo Mana").SetValue(new Slider(20)));
                comboMenu.AddItem(new MenuItem("dz191.ahri.combo.mode", "Combo Mode").SetValue(new StringList(new []{"Wait for Charm","Don't Wait for Charm"})));
            }
            Menu.AddSubMenu(comboMenu);

            var harassMenu = new Menu("[Ahri] Harass", "dz191.ahri.harass");
            {
                harassMenu.AddItem(new MenuItem("dz191.ahri.harass.useq", "Use Q Harass").SetValue(true));
                harassMenu.AddItem(new MenuItem("dz191.ahri.harass.usew", "Use W Harass").SetValue(true));
                harassMenu.AddItem(new MenuItem("dz191.ahri.harass.usee", "Use E Harass").SetValue(true));
                harassMenu.AddItem(new MenuItem("dz191.ahri.harass.onlyqcharm", "Use Q Only when charmed").SetValue(true));
                harassMenu.AddItem(new MenuItem("dz191.ahri.harass.mana", "Min Harass Mana").SetValue(new Slider(20)));
            }
            Menu.AddSubMenu(harassMenu);

            var miscMenu = new Menu("[Ahri] Misc", "dz191.ahri.misc");
            {
                miscMenu.AddItem(new MenuItem("dz191.ahri.misc.egp", "Auto E Gapclosers").SetValue(true)); //Done
                miscMenu.AddItem(new MenuItem("dz191.ahri.misc.eint", "Auto E Interrupter").SetValue(true)); //Done
                miscMenu.AddItem(new MenuItem("dz191.ahri.misc.rgap", "R away gapclosers if E on CD").SetValue(false)); //Done
                miscMenu.AddItem(new MenuItem("dz191.ahri.misc.autoq", "Auto Q Charmed targets").SetValue(false)); //Done
                miscMenu.AddItem(new MenuItem("dz191.ahri.misc.userexpire", "Use R when about to expire").SetValue(false)); //Done
                miscMenu.AddItem(new MenuItem("dz191.ahri.misc.autoq2", "Auto Q poke (Long range)").SetValue(false)); //Done
                miscMenu.AddItem(new MenuItem("dz191.ahri.misc.autoq2mana", "Auto Q mana").SetValue(new Slider(25))); //Done
                miscMenu.AddItem(new MenuItem("dz191.ahri.misc.instacharm", "Instacharm").SetValue(new KeyBind("T".ToCharArray()[0],KeyBindType.Press))); //Done
            }
            Menu.AddSubMenu(miscMenu);

            var farmMenu = new Menu("[Ahri] Farm", "dz191.ahri.farm");
            {
                farmMenu.AddItem(new MenuItem("dz191.ahri.farm.qlh", "Use Q LastHit").SetValue(false));
                farmMenu.AddItem(new MenuItem("dz191.ahri.farm.qlc", "Use Q Laneclear").SetValue(false));
                farmMenu.AddItem(new MenuItem("dz191.ahri.farm.wlc", "Use W Laneclear").SetValue(false));
                farmMenu.AddItem(new MenuItem("dz191.ahri.farm.mana", "Min Farm Mana").SetValue(new Slider(20)));
            }
            Menu.AddSubMenu(farmMenu);

            var drawMenu = new Menu("[Ahri] Drawing", "dz191.ahri.drawings");
            {
                drawMenu.AddItem(new MenuItem("dz191.ahri.drawings.q", "Draw Q").SetValue(new Circle(true,Color.Aqua)));
                drawMenu.AddItem(new MenuItem("dz191.ahri.drawings.e", "Draw E").SetValue(new Circle(true, Color.Aqua)));
            }
            Menu.AddSubMenu(drawMenu);

            Menu.AddToMainMenu();
        }
        #endregion

    }
}
