using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = SharpDX.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace KarthusSharp
{
    /*
    * LaneClear:
    * - allow AA on Tower, Ward (don't Q wards)
    * - improve somehow
    * - https://github.com/trus/L-/blob/master/TRUSt%20in%20my%20Karthus/Program.cs
    * 
    * Ult KS:
    * - don't KS anymore if enemy is recalling and would arrive base before ult went through (have to include BaseUlt functionality)
    * - It also ulted while taking hits from enemy tower.
    * 
    * Misc:
    * - add don't use spells until lvl x etc.
    * - Recode
    * - Onspellcast if q farm enabled, disable AA in beforeattack and start timer that lasts casttime

    *  10.9.2015
    * - Added Ulti damage indicator
    * - Added toggle harass Q + permashow status
    * - Added ping notification
    * - Added ulti ks permashow status
    * */

    internal class Karthus
    {
        private readonly Menu _menu;

        private readonly Spell _spellQ;
        private readonly Spell _spellW;
        private readonly Spell _spellE;
        private readonly Spell _spellR;

        private const float SpellQWidth = 160f;
        private const float SpellWWidth = 160f;

        private bool _comboE;
        private static Vector2 PingLocation;
        private static int LastPingT = 0;

        private static Orbwalking.Orbwalker _orbwalker;

        public Karthus()
        {
            if (ObjectManager.Player.ChampionName != "Karthus")
                return;

            (_menu = new Menu("KarthusSharp", "KarthusSharp", true)).AddToMainMenu();

            var targetSelectorMenu = new Menu("Target Selector", "TargetSelector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            _menu.AddSubMenu(targetSelectorMenu);

            _orbwalker = new Orbwalking.Orbwalker(_menu.AddSubMenu(new Menu("Orbwalking", "Orbwalking")));

            var comboMenu = _menu.AddSubMenu(new Menu("Combo", "Combo"))
                .SetFontStyle(FontStyle.Regular, Color.GreenYellow);
            comboMenu.AddItem(new MenuItem("comboQ", "Use Q").SetValue(true));
            comboMenu.AddItem(new MenuItem("comboW", "Use W").SetValue(true));
            comboMenu.AddItem(new MenuItem("comboE", "Use E").SetValue(true));
            comboMenu.AddItem(new MenuItem("comboAA", "Use AA").SetValue(false));
            comboMenu.AddItem(new MenuItem("comboWPercent", "Use W until Mana %").SetValue(new Slider(10)));
            comboMenu.AddItem(new MenuItem("comboEPercent", "Use E until Mana %").SetValue(new Slider(15)));
            comboMenu.AddItem(new MenuItem("comboMove", "Orbwalk/Move").SetValue(true));

            var harassMenu = _menu.AddSubMenu(new Menu("Harass", "Harass"));
            harassMenu.AddItem(new MenuItem("harassQ", "Use Q").SetValue(true));
            harassMenu.AddItem(new MenuItem("harassQPercent", "Use Q until Mana %").SetValue(new Slider(15)));
            harassMenu.AddItem(new MenuItem("harassQLasthit", "Prioritize Last Hit").SetValue(true));
            harassMenu.AddItem(
                new MenuItem("harassQToggle", "Toggle Q").SetValue(new KeyBind("H".ToCharArray()[0], KeyBindType.Toggle)))
                .Permashow(true, "Karthus| Toggle Q");
            harassMenu.AddItem(new MenuItem("harassMove", "Orbwalk/Move").SetValue(true));

            var farmMenu = _menu.AddSubMenu(new Menu("Farming", "Farming"));
            farmMenu.AddItem(
                new MenuItem("farmQ", "Use Q").SetValue(new StringList(new[] {"Last Hit", "Lane Clear", "Both", "No"}, 1)));
            farmMenu.AddItem(new MenuItem("farmE", "Use E in Lane Clear").SetValue(true));
            farmMenu.AddItem(new MenuItem("farmAA", "Use AA in Lane Clear").SetValue(false));
            farmMenu.AddItem(new MenuItem("farmQPercent", "Use Q until Mana %").SetValue(new Slider(10)));
            farmMenu.AddItem(new MenuItem("farmEPercent", "Use E until Mana %").SetValue(new Slider(20)));
            farmMenu.AddItem(new MenuItem("farmMove", "Orbwalk/Move").SetValue(true));

            var notifyMenu = _menu.AddSubMenu(new Menu("Notify on R killable enemies", "Notify"));
            notifyMenu.AddItem(new MenuItem("notifyR", "Text Notify").SetValue(true));
            notifyMenu.AddItem(new MenuItem("notifyPing", "Ping Notify").SetValue(false));

            var drawMenu = _menu.AddSubMenu(new Menu("Drawing", "Drawing"));
            drawMenu.AddItem(
                new MenuItem("drawQ", "Draw Q range").SetValue(new Circle(true,
                    System.Drawing.Color.FromArgb(125, 0, 255, 0))));

            var miscMenu = _menu.AddSubMenu(new Menu("Misc", "Misc"));
            miscMenu.AddItem(new MenuItem("ultKS", "Ultimate KS").SetValue(true))
                .Permashow(true, "Karthus| Ultimate KS");
            ;
            miscMenu.AddItem(new MenuItem("autoCast", "Auto Combo/LaneClear if dead").SetValue(false));
            miscMenu.AddItem(new MenuItem("packetCast", "Packet Cast").SetValue(true));

            _spellQ = new Spell(SpellSlot.Q, 875);
            _spellW = new Spell(SpellSlot.W, 1000);
            _spellE = new Spell(SpellSlot.E, 505);
            _spellR = new Spell(SpellSlot.R, 20000f);

            _spellQ.SetSkillshot(1f, 160, float.MaxValue, false, SkillshotType.SkillshotCircle);
            _spellW.SetSkillshot(.5f, 70, float.MaxValue, false, SkillshotType.SkillshotCircle);
            _spellE.SetSkillshot(1f, 505, float.MaxValue, false, SkillshotType.SkillshotCircle);
            _spellR.SetSkillshot(3f, float.MaxValue, float.MaxValue, false, SkillshotType.SkillshotCircle);

            var dmgAfterComboItem = new MenuItem("DamageAfterCombo", "Damage After Combo").SetValue(true);
            _menu.SubMenu("Drawing").AddItem(dmgAfterComboItem);

            LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnit = ComboDamage;
            LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = dmgAfterComboItem.GetValue<bool>();
            dmgAfterComboItem.ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
            {
                LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
            };

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;

            Chat.Print(
                "<font color=\"#1eff00\">KarthusSharp by Beaving</font> - <font color=\"#00BFFF\">Loaded</font>");
        }

        private void Game_OnUpdate(EventArgs args)
        {
            if (_menu.Item("ultKS").GetValue<bool>())
                UltKs();

            if (_menu.Item("notifyPing").GetValue<bool>())
                foreach (
                    var enemy in
                        HeroManager.Enemies.Where(
                            t =>
                                ObjectManager.Player.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Ready &&
                                t.IsValidTarget() && _spellR.GetDamage(t) > t.Health &&
                                t.Distance(ObjectManager.Player.Position) > _spellQ.Range))
                {
                    Ping(enemy.Position.To2D());
                }


            if (_spellQ.IsReady() && _menu.Item("harassQToggle").GetValue<KeyBind>().Active &&
                _orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
            {
                CastQ(TargetSelector.GetTarget(_spellQ.Range, TargetSelector.DamageType.Magical),
                    _menu.Item("harassQPercent").GetValue<Slider>().Value);
            }

            switch (_orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    _orbwalker.SetAttack(_menu.Item("comboAA").GetValue<bool>() || ObjectManager.Player.Mana < 100);
                        //if no mana, allow auto attacks!
                    _orbwalker.SetMovement(_menu.Item("comboMove").GetValue<bool>());
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    _orbwalker.SetAttack(true);
                    _orbwalker.SetMovement(_menu.Item("harassMove").GetValue<bool>());
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    _orbwalker.SetAttack(_menu.Item("farmAA").GetValue<bool>() || ObjectManager.Player.Mana < 100);
                    _orbwalker.SetMovement(_menu.Item("farmMove").GetValue<bool>());
                    LaneClear();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    _orbwalker.SetAttack(true);
                    _orbwalker.SetMovement(_menu.Item("farmMove").GetValue<bool>());
                    LastHit();
                    break;
                default:
                    _orbwalker.SetAttack(true);
                    _orbwalker.SetMovement(true);
                    RegulateEState();

                    if (_menu.Item("autoCast").GetValue<bool>())
                        if (IsInPassiveForm())
                            if (!Combo())
                                LaneClear(true);

                    break;
            }
        }

        private void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (_orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                args.Process = !_spellQ.IsReady();
            }
            else if (_orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit)
            {
                bool farmQ = _menu.Item("farmQ").GetValue<StringList>().SelectedIndex == 0 ||
                             _menu.Item("farmQ").GetValue<StringList>().SelectedIndex == 2;
                args.Process =
                    !(farmQ && _spellQ.IsReady() &&
                      GetManaPercent() >= _menu.Item("farmQPercent").GetValue<Slider>().Value);
            }
        }

        public float GetManaPercent()
        {
            return (ObjectManager.Player.Mana/ObjectManager.Player.MaxMana)*100f;
        }

        public bool PacketsNoLel()
        {
            return _menu.Item("packetCast").GetValue<bool>();
        }

        private bool Combo()
        {
            bool anyQTarget = false;

            if (_menu.Item("comboW").GetValue<bool>())
                CastW(TargetSelector.GetTarget(_spellW.Range, TargetSelector.DamageType.Magical),
                    _menu.Item("comboWPercent").GetValue<Slider>().Value);

            if (_menu.Item("comboE").GetValue<bool>() && _spellE.IsReady() && !IsInPassiveForm())
            {
                var target = TargetSelector.GetTarget(_spellE.Range, TargetSelector.DamageType.Magical);

                if (target != null)
                {
                    var enoughMana = GetManaPercent() >= _menu.Item("comboEPercent").GetValue<Slider>().Value;

                    if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).ToggleState == 1)
                    {
                        if (ObjectManager.Player.Distance(target.ServerPosition) <= _spellE.Range && enoughMana)
                        {
                            _comboE = true;
                            _spellE.Cast();
                        }
                    }
                    else if (!enoughMana)
                        RegulateEState(true);
                }
                else
                    RegulateEState();
            }

            if (_menu.Item("comboQ").GetValue<bool>() && _spellQ.IsReady())
            {
                var target = TargetSelector.GetTarget(_spellQ.Range, TargetSelector.DamageType.Magical);

                if (target != null)
                {
                    anyQTarget = true;
                    CastQ(target);
                }
            }

            return anyQTarget;
        }

        private void Harass()
        {
            if (_menu.Item("harassQLasthit").GetValue<bool>())
                LastHit();

            if (_menu.Item("harassQ").GetValue<bool>())
                CastQ(TargetSelector.GetTarget(_spellQ.Range, TargetSelector.DamageType.Magical),
                    _menu.Item("harassQPercent").GetValue<Slider>().Value);
        }

        private void LaneClear(bool ignoreConfig = false)
        {
            var farmQ = ignoreConfig || _menu.Item("farmQ").GetValue<StringList>().SelectedIndex == 1 ||
                        _menu.Item("farmQ").GetValue<StringList>().SelectedIndex == 2;
            var farmE = ignoreConfig || _menu.Item("farmE").GetValue<bool>();

            List<Obj_AI_Base> minions;

            bool jungleMobs;
            if (farmQ && _spellQ.IsReady())
            {
                minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _spellQ.Range, MinionTypes.All,
                    MinionTeam.NotAlly);
                minions.RemoveAll(x => x.MaxHealth <= 5); //filter wards the ghetto method lel

                jungleMobs = minions.Any(x => x.Team == GameObjectTeam.Neutral);

                _spellQ.Width = SpellQWidth;
                var farmInfo = _spellQ.GetCircularFarmLocation(minions, _spellQ.Width);

                if (farmInfo.MinionsHit >= 1)
                    CastQ(farmInfo.Position, jungleMobs ? 0 : _menu.Item("farmQPercent").GetValue<Slider>().Value);
            }

            if (!farmE || !_spellE.IsReady() || IsInPassiveForm())
                return;
            _comboE = false;

            minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _spellE.Range, MinionTypes.All,
                MinionTeam.NotAlly);
            minions.RemoveAll(x => x.MaxHealth <= 5); //filter wards the ghetto method lel

            jungleMobs = minions.Any(x => x.Team == GameObjectTeam.Neutral);

            var enoughMana = GetManaPercent() > _menu.Item("farmEPercent").GetValue<Slider>().Value;

            if (enoughMana &&
                ((minions.Count >= 3 || jungleMobs) &&
                 ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).ToggleState == 1))
                _spellE.CastOnUnit(ObjectManager.Player);
            else if (!enoughMana ||
                     ((minions.Count <= 2 && !jungleMobs) &&
                      ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).ToggleState == 2))
                RegulateEState(!enoughMana);
        }

        private void LastHit()
        {
            var farmQ = _menu.Item("farmQ").GetValue<StringList>().SelectedIndex == 0 ||
                        _menu.Item("farmQ").GetValue<StringList>().SelectedIndex == 2;

            if (!farmQ || !_spellQ.IsReady())
                return;

            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _spellQ.Range, MinionTypes.All,
                MinionTeam.NotAlly);
            minions.RemoveAll(x => x.MaxHealth <= 5); //filter wards the ghetto method lel

            foreach (
                var minion in
                    minions.Where(
                        x =>
                            ObjectManager.Player.GetSpellDamage(x, SpellSlot.Q, 1) >=
                            //FirstDamage = multitarget hit, differentiate! (check radius around mob predicted pos)
                            HealthPrediction.GetHealthPrediction(x, (int) (_spellQ.Delay*1000))))
            {
                CastQ(minion, _menu.Item("farmQPercent").GetValue<Slider>().Value);
            }
        }

        private void UltKs()
        {
            if (!_spellR.IsReady())
                return;
            var time = Utils.TickCount;

            List<AIHeroClient> ultTargets = new List<AIHeroClient>();

            foreach (
                var target in
                    Program.Helper.EnemyInfo.Where(
                        x => //need to check if recently recalled (for cases when no mana for baseult)
                            x.Player.IsValid &&
                            !x.Player.IsDead &&
                            x.Player.IsEnemy &&
                            //!(x.RecallInfo.Recall.Status == Packet.S2C.Recall.RecallStatus.RecallStarted && x.RecallInfo.GetRecallCountdown() < 3100) && //let BaseUlt handle this one
                            ((!x.Player.IsVisible && time - x.LastSeen < 10000) ||
                             (x.Player.IsVisible && x.Player.IsValidTarget())) &&
                            ObjectManager.Player.GetSpellDamage(x.Player, SpellSlot.R) >=
                            Program.Helper.GetTargetHealth(x, (int) (_spellR.Delay*1000f))))
            {
                if (target.Player.IsVisible || (!target.Player.IsVisible && time - target.LastSeen < 2750))
                    //allies still attacking target? prevent overkill
                    if (Program.Helper.OwnTeam.Any(x => !x.IsMe && x.Distance(target.Player) < 1600))
                        continue;

                if (IsInPassiveForm() ||
                    !Program.Helper.EnemyTeam.Any(
                        x =>
                            x.IsValid && !x.IsDead &&
                            (x.IsVisible || (!x.IsVisible && time - Program.Helper.GetPlayerInfo(x).LastSeen < 2750)) &&
                            ObjectManager.Player.Distance(x) < 1600))
                    //any other enemies around? dont ult unless in passive form
                    ultTargets.Add(target.Player);
            }

            int targets = ultTargets.Count();

            if (targets > 0)
            {
                //dont ult if Zilean is nearby the target/is the target and his ult is up
                var zilean =
                    Program.Helper.EnemyTeam.FirstOrDefault(
                        x =>
                            x.ChampionName == "Zilean" &&
                            (x.IsVisible || (!x.IsVisible && time - Program.Helper.GetPlayerInfo(x).LastSeen < 3000)) &&
                            (x.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Ready ||
                             (x.Spellbook.GetSpell(SpellSlot.R).Level > 0 &&
                              x.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Surpressed &&
                              x.Mana >= x.Spellbook.GetSpell(SpellSlot.R).SData.Mana)));

                if (zilean != null)
                {
                    int inZileanRange = ultTargets.Count(x => x.Distance(zilean) < 2500);
                        //if multiple, shoot regardless

                    if (inZileanRange > 0)
                        targets--; //remove one target, because zilean can save one
                }

                if (targets > 0)
                    _spellR.Cast();
            }
        }

        private void RegulateEState(bool ignoreTargetChecks = false)
        {
            if (!_spellE.IsReady() || IsInPassiveForm() ||
                ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).ToggleState != 2)
                return;
            var target = TargetSelector.GetTarget(_spellE.Range, TargetSelector.DamageType.Magical);
            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _spellE.Range, MinionTypes.All,
                MinionTeam.NotAlly);

            if (!ignoreTargetChecks && (target != null || (!_comboE && minions.Count != 0)))
                return;
            _spellE.CastOnUnit(ObjectManager.Player);
            _comboE = false;
        }

        private void CastQ(Obj_AI_Base target, int minManaPercent = 0)
        {
            if (!_spellQ.IsReady() || !(GetManaPercent() >= minManaPercent))
                return;
            if (target == null)
                return;
            _spellQ.Width = GetDynamicQWidth(target);
            _spellQ.Cast(target);

        }

        private void CastQ(Vector2 pos, int minManaPercent = 0)
        {
            if (!_spellQ.IsReady())
                return;
            if (GetManaPercent() >= minManaPercent)
                _spellQ.Cast(pos);
        }

        private void CastW(Obj_AI_Base target, int minManaPercent = 0)
        {
            if (!_spellW.IsReady() || !(GetManaPercent() >= minManaPercent))
                return;
            if (target == null)
                return;
            _spellW.Width = GetDynamicWWidth(target);
            _spellW.Cast(target);
        }

        private float GetDynamicWWidth(Obj_AI_Base target)
        {
            return Math.Max(70, (1f - (ObjectManager.Player.Distance(target)/_spellW.Range))*SpellWWidth);
        }

        private float GetDynamicQWidth(Obj_AI_Base target)
        {
            return Math.Max(30, (1f - (ObjectManager.Player.Distance(target)/_spellQ.Range))*SpellQWidth);
        }

        private static bool IsInPassiveForm()
        {
            return ObjectManager.Player.IsZombie; //!ObjectManager.Player.IsHPBarRendered;
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (!ObjectManager.Player.IsDead)
            {
                var drawQ = _menu.Item("drawQ").GetValue<Circle>();

                if (drawQ.Active)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, _spellQ.Range, drawQ.Color);
            }

            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Level > 0)
            {
                var time = Utils.TickCount;

                var victims =
                    Program.Helper.EnemyInfo.Where(
                        x =>
                            x.Player.IsValid && !x.Player.IsDead && x.Player.IsEnemy &&
                            ((!x.Player.IsVisible && time - x.LastSeen < 10000) ||
                             (x.Player.IsVisible && x.Player.IsValidTarget())) &&
                            ObjectManager.Player.GetSpellDamage(x.Player, SpellSlot.R) >=
                            Program.Helper.GetTargetHealth(x, (int) (_spellR.Delay*1000f)))
                        .Aggregate("", (current, target) => current + (target.Player.ChampionName + " "));

                if (victims != "" && _spellR.IsReady())
                {
                    if (_menu.Item("notifyR").GetValue<bool>())
                    {
                        Drawing.DrawText(Drawing.Width*0.44f, Drawing.Height*0.7f, System.Drawing.Color.GreenYellow,
                            "Ult can kill: " + victims);

                        //use when pos works
//                        var x = new Render.Text((int) (Drawing.Width*0.44f), (int) (Drawing.Height*0.7f), "Ult can kill: " + victims, 30, SharpDX.Color.Red); //.Add()


                    }
                }
            }
        }

        private static void Ping(Vector2 position)
        {
            if (LeagueSharp.Common.Utils.TickCount - LastPingT < 30*1000)
            {
                return;
            }

            LastPingT = LeagueSharp.Common.Utils.TickCount;
            PingLocation = position;
            SimplePing();

            LeagueSharp.Common.Utility.DelayAction.Add(150, SimplePing);
            LeagueSharp.Common.Utility.DelayAction.Add(300, SimplePing);
            LeagueSharp.Common.Utility.DelayAction.Add(400, SimplePing);
            LeagueSharp.Common.Utility.DelayAction.Add(800, SimplePing);
        }

        private static void SimplePing()
        {
            TacticalMap.ShowPing(PingCategory.Fallback, PingLocation, true);
        }

        private float ComboDamage(AIHeroClient t)
        {
            return _spellR.GetDamage(t);
        }
    }
}
