using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;

namespace AmumuSharp
{
    internal class Amumu
    {
        readonly Menu _menu;

        private static readonly AIHeroClient Player = ObjectManager.Player;
        private readonly Spell _spellQ;
        private readonly Spell _spellW;
        private readonly Spell _spellE;
        private readonly Spell _spellR;

        private bool _comboW;

        private static Orbwalking.Orbwalker _orbwalker;

        public Amumu() //add Q near mouse (range), 
        {
            if (Player.ChampionName != "Amumu")
                return;

            (_menu = new Menu("AmumuSharp", "AmumuSharp", true)).AddToMainMenu();

            var targetSelectorMenu = new Menu("Target Selector", "TargetSelector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            _menu.AddSubMenu(targetSelectorMenu);

            _orbwalker = new Orbwalking.Orbwalker(_menu.AddSubMenu(new Menu("Orbwalking", "Orbwalking")));

            var comboMenu = _menu.AddSubMenu(new Menu("Combo", "Combo"));
            comboMenu.AddItem(new MenuItem("comboQ" + Player.ChampionName, "Use Q").SetValue(new StringList(new[] { "No", "Always", "If out of range" }, 1)));
            comboMenu.AddItem(new MenuItem("comboW" + Player.ChampionName, "Use W").SetValue(true));
            comboMenu.AddItem(new MenuItem("comboE" + Player.ChampionName, "Use E").SetValue(true));
            comboMenu.AddItem(new MenuItem("comboR" + Player.ChampionName, "Auto Use R").SetValue(new Slider(3, 0, 5)));
            comboMenu.AddItem(new MenuItem("comboWPercent" + Player.ChampionName, "Use W until Mana %").SetValue(new Slider(10)));

            var farmMenu = _menu.AddSubMenu(new Menu("Farming", "Farming"));
            farmMenu.AddItem(new MenuItem("farmQ" + Player.ChampionName, "Use Q").SetValue(new StringList(new[] { "No", "Always", "If out of range" }, 2)));
            farmMenu.AddItem(new MenuItem("farmW" + Player.ChampionName, "Use W").SetValue(true));
            farmMenu.AddItem(new MenuItem("farmWignoremana" + Player.ChampionName, "Always Use W if got blue buff").SetValue(true));
            farmMenu.AddItem(new MenuItem("farmE" + Player.ChampionName, "Use E").SetValue(true));
            farmMenu.AddItem(new MenuItem("farmWPercent" + Player.ChampionName, "Use W until Mana %").SetValue(new Slider(20)));

            var drawMenu = _menu.AddSubMenu(new Menu("Drawing", "Drawing"));
            drawMenu.AddItem(new MenuItem("drawQ" + Player.ChampionName, "Draw Q range").SetValue(new Circle(true, System.Drawing.Color.FromArgb(125, 0, 255, 0))));
            drawMenu.AddItem(new MenuItem("drawW" + Player.ChampionName, "Draw W range").SetValue(new Circle(false, System.Drawing.Color.FromArgb(125, 0, 255, 0))));
            drawMenu.AddItem(new MenuItem("drawE" + Player.ChampionName, "Draw E range").SetValue(new Circle(false, System.Drawing.Color.FromArgb(125, 0, 255, 0))));
            drawMenu.AddItem(new MenuItem("drawR" + Player.ChampionName, "Draw R range").SetValue(new Circle(false, System.Drawing.Color.FromArgb(125, 0, 255, 0))));

            var miscMenu = _menu.AddSubMenu(new Menu("Misc", "Misc"));
            miscMenu.AddItem(new MenuItem("aimQ" + Player.ChampionName, "Q near mouse").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
            miscMenu.AddItem(new MenuItem("packetCast", "Packet Cast").SetValue(true));

            _spellQ = new Spell(SpellSlot.Q, 1080);
            _spellW = new Spell(SpellSlot.W, 300);
            _spellE = new Spell(SpellSlot.E, 350);
            _spellR = new Spell(SpellSlot.R, 550);

            _spellQ.SetSkillshot(.25f, 90, 2000, true, SkillshotType.SkillshotLine);  //check delay
            _spellW.SetSkillshot(0f, _spellW.Range, float.MaxValue, false, SkillshotType.SkillshotCircle); //correct
            _spellE.SetSkillshot(.5f, _spellE.Range, float.MaxValue, false, SkillshotType.SkillshotCircle); //check delay
            _spellR.SetSkillshot(.25f, _spellR.Range, float.MaxValue, false, SkillshotType.SkillshotCircle); //check delay

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;

            Chat.Print("<font color=\"#1eff00\">AmumuSharp by Beaving</font> - <font color=\"#00BFFF\">Loaded</font>");
        }

        void Game_OnUpdate(EventArgs args)
        {
            AutoUlt();

            if (_menu.Item("aimQ" + Player.ChampionName).GetValue<KeyBind>().Active)
                CastQ(Program.Helper.EnemyTeam.Where(x => x.LSIsValidTarget(_spellQ.Range) && x.LSDistance(Game.CursorPos) < 400).OrderBy(x => x.LSDistance(Game.CursorPos)).FirstOrDefault());

            switch (_orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    break;
                default:
                    RegulateWState();
                    break;
            }
        }

        void AutoUlt()
        {
            var comboR = _menu.Item("comboR" + Player.ChampionName).GetValue<Slider>().Value;

            if (comboR > 0 && _spellR.LSIsReady())
            {
                int enemiesHit = 0;
                int killableHits = 0;

                foreach (AIHeroClient enemy in Program.Helper.EnemyTeam.Where(x => x.LSIsValidTarget(_spellR.Range)))
                {
                    var prediction = Prediction.GetPrediction(enemy, _spellR.Delay);

                    if (prediction != null && prediction.UnitPosition.LSDistance(Player.ServerPosition) <= _spellR.Range)
                    {
                        enemiesHit++;

                        if (Player.LSGetSpellDamage(enemy, SpellSlot.W) >= enemy.Health)
                            killableHits++;
                    }
                }

                if (enemiesHit >= comboR || (killableHits >= 1 && Player.Health / Player.MaxHealth <= 0.1))
                    CastR();
            }
        }

        void CastE(Obj_AI_Base target)
        {
            if (!_spellE.LSIsReady() || target == null || !target.LSIsValidTarget())
                return;

            if (_spellE.GetPrediction(target).UnitPosition.LSDistance(Player.ServerPosition) <= _spellE.Range)
                _spellE.CastOnUnit(Player);
        }

        public float GetManaPercent()
        {
            return (Player.Mana / Player.MaxMana) * 100f;
        }

        public bool PacketsNoLel()
        {
            return _menu.Item("packetCast").GetValue<bool>();
        }

        void Combo()
        {
            var comboQ = _menu.Item("comboQ" + Player.ChampionName).GetValue<StringList>().SelectedIndex;
            var comboW = _menu.Item("comboW" + Player.ChampionName).GetValue<bool>();
            var comboE = _menu.Item("comboE" + Player.ChampionName).GetValue<bool>();
            var comboR = _menu.Item("comboR" + Player.ChampionName).GetValue<Slider>().Value;

            if (comboQ > 0 && _spellQ.LSIsReady())
            {
                if (_spellR.LSIsReady() && comboR > 0) //search unit that provides most targets hit by ult. prioritize hero target unit
                {
                    int maxTargetsHit = 0;
                    Obj_AI_Base unitMostTargetsHit = null;

                    foreach (Obj_AI_Base unit in ObjectManager.Get<Obj_AI_Base>().Where(x => x.LSIsValidTarget(_spellQ.Range) && _spellQ.GetPrediction(x).Hitchance >= HitChance.High)) //causes troubles?
                    {
                        int targetsHit = unit.LSCountEnemiesInRange((int)_spellR.Range); //unitposition might not reflect where you land with Q

                        if (targetsHit > maxTargetsHit || (unitMostTargetsHit != null && targetsHit >= maxTargetsHit && unit.Type == GameObjectType.AIHeroClient))
                        {
                            maxTargetsHit = targetsHit;
                            unitMostTargetsHit = unit;
                        }
                    }

                    if (maxTargetsHit >= comboR)
                        CastQ(unitMostTargetsHit);
                }

                Obj_AI_Base target = TargetSelector.GetTarget(_spellQ.Range, TargetSelector.DamageType.Magical);

                if (target != null)
                    if (comboQ == 1 || (comboQ == 2 && !Orbwalking.InAutoAttackRange(target)))
                        CastQ(target);
            }

            if (comboW && _spellW.LSIsReady())
            {
                var target = TargetSelector.GetTarget(_spellW.Range, TargetSelector.DamageType.Magical);

                if (target != null)
                {
                    var enoughMana = GetManaPercent() >= _menu.Item("comboWPercent" + Player.ChampionName).GetValue<Slider>().Value;

                    if (Player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 1)
                    {
                        if (Player.LSDistance(target.ServerPosition) <= _spellW.Range && enoughMana)
                        {
                            _comboW = true;
                            _spellW.Cast();
                        }
                    }
                    else if (!enoughMana)
                        RegulateWState(true);
                }
                else
                    RegulateWState();
            }

            if (comboE && _spellE.LSIsReady())
                CastE(Program.Helper.EnemyTeam.OrderBy(x => x.LSDistance(Player)).FirstOrDefault());
        }

        void LaneClear()
        {
            var farmQ = _menu.Item("farmQ" + Player.ChampionName).GetValue<StringList>().SelectedIndex;
            var farmW = _menu.Item("farmW" + Player.ChampionName).GetValue<bool>();
            var farmE = _menu.Item("farmE" + Player.ChampionName).GetValue<bool>();

            List<Obj_AI_Base> minions;

            if (farmQ > 0 && _spellQ.LSIsReady())
            {
                Obj_AI_Base minion = MinionManager.GetMinions(Player.ServerPosition, _spellQ.Range, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.MaxHealth).FirstOrDefault(x => _spellQ.GetPrediction(x).Hitchance >= HitChance.Medium);

                if (minion != null)
                    if (farmQ == 1 || (farmQ == 2 && !Orbwalking.InAutoAttackRange(minion)))
                        CastQ(minion, HitChance.Medium);
            }

            if (farmE && _spellE.LSIsReady())
            {
                minions = MinionManager.GetMinions(Player.ServerPosition, _spellE.Range, MinionTypes.All, MinionTeam.NotAlly);
                CastE(minions.OrderBy(x => x.LSDistance(Player)).FirstOrDefault());
            }

            if (!farmW || !_spellW.LSIsReady())
                return;
            _comboW = false;

            minions = MinionManager.GetMinions(Player.ServerPosition, _spellW.Range, MinionTypes.All, MinionTeam.NotAlly);

            bool anyJungleMobs = minions.Any(x => x.Team == GameObjectTeam.Neutral);

            var enoughMana = GetManaPercent() > _menu.Item("farmWPercent" + Player.ChampionName).GetValue<Slider>().Value;

            if (enoughMana || (Player.HasBuff("CrestoftheAncientGolem") && _menu.Item("farmWignoremana" + Player.ChampionName).GetValue<bool>()) && ((minions.Count >= 3 || anyJungleMobs) && Player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 1))
                _spellW.Cast();
            else if (!enoughMana || ((minions.Count <= 2 && !anyJungleMobs) && Player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 2))
                RegulateWState(!enoughMana);
        }

        void RegulateWState(bool ignoreTargetChecks = false)
        {
            if (!_spellW.LSIsReady() || Player.Spellbook.GetSpell(SpellSlot.W).ToggleState != 2)
                return;

            var target = TargetSelector.GetTarget(_spellW.Range, TargetSelector.DamageType.Magical);
            var minions = MinionManager.GetMinions(Player.ServerPosition, _spellW.Range, MinionTypes.All, MinionTeam.NotAlly);

            if (!ignoreTargetChecks && (target != null || (!_comboW && minions.Count != 0)))
                return;

            _spellW.Cast();
            _comboW = false;
        }

        void CastQ(Obj_AI_Base target, HitChance hitChance = HitChance.High)
        {
            if (!_spellQ.LSIsReady())
                return;
            if (target == null || !target.LSIsValidTarget())
                return;

            _spellQ.CastIfHitchanceEquals(target, hitChance);
        }

        void CastR()
        {
            if (!_spellR.LSIsReady())
                return;
            _spellR.Cast();
        }

        void Drawing_OnDraw(EventArgs args)
        {
            if (!Player.IsDead)
            {
                var drawQ = _menu.Item("drawQ" + Player.ChampionName).GetValue<Circle>();
                var drawW = _menu.Item("drawW" + Player.ChampionName).GetValue<Circle>();
                var drawE = _menu.Item("drawE" + Player.ChampionName).GetValue<Circle>();
                var drawR = _menu.Item("drawR" + Player.ChampionName).GetValue<Circle>();

                if (drawQ.Active)
                    Render.Circle.DrawCircle(Player.Position, _spellQ.Range, drawQ.Color);

                if (drawW.Active)
                    Render.Circle.DrawCircle(Player.Position, _spellW.Range, drawW.Color);

                if (drawE.Active)
                    Render.Circle.DrawCircle(Player.Position, _spellE.Range, drawE.Color);

                if (drawR.Active)
                    Render.Circle.DrawCircle(Player.Position, _spellR.Range, drawR.Color);
            }
        }
    }
}
