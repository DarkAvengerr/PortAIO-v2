using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp.Common;
using LeagueSharp;
using LeagueSharp.Common.Data;

using EloBuddy;
using LeagueSharp.Common;
namespace SethLulu
{
    class Program
    {
        public const string ChampionName = "Lulu";

        public static Menu _root;
        public static Spell _q, _w, _e, _r;
        public static Orbwalking.Orbwalker Orbwalker;

        public static List<Spell> _spells = new List<Spell>();

        private static SpellSlot _igniteSlot;
        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.BaseSkinName != ChampionName) return;

            _q = new Spell(SpellSlot.Q, 925f);
            _q.SetSkillshot(0.25f, 60, 1450, false, SkillshotType.SkillshotLine);

            _w = new Spell(SpellSlot.W, 650f);
            _e = new Spell(SpellSlot.E, 650f);
            _r = new Spell(SpellSlot.R, 900f);

            _igniteSlot = ObjectManager.Player.GetSpellSlot("SummonerDot");

            _spells.Add(_q);
            _spells.Add(_w);
            _spells.Add(_e);
            _spells.Add(_r);

            _root = new Menu("SethLulu", "Lulu", true);
            _root.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));

            Orbwalker = new Orbwalking.Orbwalker(_root.SubMenu("Orbwalking"));

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            _root.AddSubMenu(targetSelectorMenu);

            _root.AddSubMenu(new Menu("Spell Handler", "SpellsM"));
            _root.SubMenu("SpellsM").AddItem(new MenuItem("Combo", "Combo"));
            _root.SubMenu("Combo").AddItem(new MenuItem("UseQCombo", "Use Q").SetValue(true));
            _root.SubMenu("Combo").AddItem(new MenuItem("UseWCombo", "Use W").SetValue(true));
            _root.SubMenu("Combo").AddItem(new MenuItem("UseECombo", "Use E").SetValue(true));
            _root.SubMenu("Combo").AddItem(new MenuItem("UseIgniteCombo", "Use Ignite").SetValue(true));
            _root.SubMenu("Combo")
                .AddItem(
                    new MenuItem("ComboActive", "Combo!").SetValue(
                        new KeyBind(_root.Item("Orbwalk").GetValue<KeyBind>().Key, KeyBindType.Press)));
            _root.SubMenu("SpellsM").AddItem(new MenuItem("Harass", "Harass"));
            _root.SubMenu("Harass").AddItem(new MenuItem("UseQHarass", "Use Q").SetValue(true));
            _root.SubMenu("Harass").AddItem(new MenuItem("UseWHarass", "Use W").SetValue(true));
            _root.SubMenu("Harass").AddItem(new MenuItem("UseEHarass", "Use E").SetValue(true));
            _root.SubMenu("Harass")
                .AddItem(
                    new MenuItem("HarassActive", "Harass (toggle)!").SetValue(
                        new KeyBind("Y".ToCharArray()[0], KeyBindType.Toggle)));
            _root.SubMenu("SpellsM").AddItem(new MenuItem("Farm", "Farm"));
            _root.SubMenu("Farm").AddItem(new MenuItem("UseQFarm", "Use Q").SetValue(true));
            _root.SubMenu("Farm").AddItem(new MenuItem("UseEFarm", "Use E").SetValue(true));
            _root.SubMenu("Farm")
                .AddItem(
                new MenuItem("LaneClearActive", "Farm!").SetValue(
                    new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));
            _root.SubMenu("SpellsM").AddItem(new MenuItem("Killsteal", "Killsteal"));
            _root.SubMenu("Killsteal").AddItem(new MenuItem("StealActive", "Enabled").SetValue(true));
            if (_root.Item("StealActive").GetValue<bool>() == true)
            {
                _root.SubMenu("Killsteal").AddItem(new MenuItem("UseQSteal", "Use Q").SetValue(true));
                _root.SubMenu("Killsteal").AddItem(new MenuItem("UseESteal", "Use E").SetValue(true));
            }

            _root.AddSubMenu(new Menu("Defensive Spells", "AllyM"));
            _root.SubMenu("AllyM").AddItem(new MenuItem("DefActive", "Enable Defensive Spells").SetValue(true));
            if (_root.Item("DefActive").GetValue<bool>() == true)
            {
                _root.SubMenu("AllyM").AddItem(new MenuItem("ESpell", "Spell E"));
                _root.SubMenu("ESpell").AddItem(new MenuItem("AutoE", "% HP to use E").SetValue(new Slider(50, 100, 0)));
                _root.SubMenu("ESpell").AddItem(new MenuItem("AutoEAlly", "% HP to use E Ally").SetValue(new Slider(50, 100, 0)));
                _root.SubMenu("AllyM").AddItem(new MenuItem("RSpell", "Spell R"));
                _root.SubMenu("RSpell").AddItem(new MenuItem("AutoR", "% HP to use R").SetValue(new Slider(50, 100, 0)));
                _root.SubMenu("RSpell").AddItem(new MenuItem("AutoRAlly", "% HP to use R Ally").SetValue(new Slider(50, 100, 0)));
            }

            _root.AddSubMenu(new Menu("Drawing Manager", "SharpDrawer"));
            _root.SubMenu("SharpDrawer").AddItem(new MenuItem("DrawQ", "Draw Q Range").SetValue(new Circle(true, System.Drawing.Color.FromArgb(100, 58, 90, 179))));
            _root.SubMenu("SharpDrawer").AddItem(new MenuItem("DrawW", "Draw W Range").SetValue(new Circle(true, System.Drawing.Color.FromArgb(100, 58, 90, 179))));
            _root.SubMenu("SharpDrawer").AddItem(new MenuItem("DrawE", "Draw E Range").SetValue(new Circle(true, System.Drawing.Color.FromArgb(100, 58, 90, 179))));
            _root.SubMenu("SharpDrawer").AddItem(new MenuItem("DrawR", "Draw R Range").SetValue(new Circle(true, System.Drawing.Color.FromArgb(100, 58, 90, 179))));

            _root.AddSubMenu(new Menu("Misc Manager", "Misc"));
            _root.SubMenu("Misc").AddItem(new MenuItem("InterruptSpells", "Interrupt Spells").SetValue(false));
            _root.SubMenu("InterruptSpells").AddItem(new MenuItem("RInterrupt", "Use R").SetValue(false));
            _root.SubMenu("InterruptSpells").AddItem(new MenuItem("WInterrupt", "Use W").SetValue(false));
            _root.SubMenu("Misc").AddItem(new MenuItem("WEGapcloser", "W > E on GapClosers").SetValue(true));

            _root.AddToMainMenu();


            var dmgAfterComboItem = new MenuItem("DamageAfterCombo", "Draw damage after combo").SetValue(true);
            _root.SubMenu("SharpDrawer").AddItem(dmgAfterComboItem);
            //LeagueSharp.Common.Utility.HpBar//DamageIndicator.DamageToUnit = GetComboDamage;
            //LeagueSharp.Common.Utility.HpBar//DamageIndicator.Enabled = dmgAfterComboItem.GetValue<bool>();
            dmgAfterComboItem.ValueChanged += delegate (object sender, OnValueChangeEventArgs eventArgs)
            {
                //LeagueSharp.Common.Utility.HpBar//DamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
            };
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloserOnOnEnemyGapcloser;
        }

        private static float GetComboDamage(Obj_AI_Base t)
        {
            var fComboDamage = 0d;
            if (_q.IsReady())
                fComboDamage += ObjectManager.Player.GetSpellDamage(t, SpellSlot.Q);
            if (_r.IsReady())
                fComboDamage += ObjectManager.Player.GetSpellDamage(t, SpellSlot.R);
            if (_w.IsReady())
                fComboDamage += ObjectManager.Player.GetSpellDamage(t, SpellSlot.W);
            if (_e.IsReady())
                fComboDamage += ObjectManager.Player.GetSpellDamage(t, SpellSlot.E);
            if (_igniteSlot != SpellSlot.Unknown &&
            ObjectManager.Player.Spellbook.CanUseSpell(_igniteSlot) == SpellState.Ready)
                fComboDamage += ObjectManager.Player.GetSummonerSpellDamage(t, Damage.SummonerSpell.Ignite);

            return (float)fComboDamage;
        }

        private static void AntiGapcloserOnOnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (_root.Item("WEGapcloser").GetValue<bool>())
            {
                if (ObjectManager.Player.Distance(gapcloser.Sender) < 650f)
                {
                    if (_w.IsReady() && _e.IsReady())
                    {
                        _w.CastOnUnit(gapcloser.Sender);
                        _e.CastOnUnit(ObjectManager.Player);
                    }
                }

                else
                {
                    if (_e.IsReady() && ObjectManager.Player.Distance(gapcloser.Sender) < _e.Range)
                    { _e.CastOnUnit(gapcloser.Sender); }
                }

                if (_w.IsReady() && ObjectManager.Player.Distance(gapcloser.Sender) < _w.Range)
                { _w.CastOnUnit(gapcloser.Sender); }
            }
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (_root.Item("InterruptSpells").GetValue<bool>())
            {
                if (_root.Item("RInterrupt").GetValue<bool>())
                {
                    var allys = HeroManager.Allies;
                    if (ObjectManager.Player.Distance(sender) < 100f && _r.IsReady())
                    { _r.CastOnUnit(ObjectManager.Player); }

                    foreach (var ally in allys)
                    {
                        if (ObjectManager.Player.Distance(sender) > _r.Range)
                        {
                            if (ally.Distance(sender) < 100f && ObjectManager.Player.Distance(ally) < _r.Range && _r.IsReady())
                            { _r.CastOnUnit(ally); }
                        }
                    }
                }

                if (_root.Item("WInterrupt").GetValue<bool>())
                {
                    if (ObjectManager.Player.Distance(sender) < _w.Range && _w.IsReady())
                    { _w.CastOnUnit(sender); }
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (_root.Item("DrawQ").GetValue<Circle>().Active)
            { Render.Circle.DrawCircle(ObjectManager.Player.Position, _q.Range, _root.Item("DrawQ").GetValue<Circle>().Color); }
            if (_root.Item("DrawW").GetValue<Circle>().Active)
            { Render.Circle.DrawCircle(ObjectManager.Player.Position, _w.Range, _root.Item("DrawW").GetValue<Circle>().Color); }
            if (_root.Item("DrawE").GetValue<Circle>().Active)
            { Render.Circle.DrawCircle(ObjectManager.Player.Position, _e.Range, _root.Item("DrawE").GetValue<Circle>().Color); }
            if (_root.Item("DrawR").GetValue<Circle>().Active)
            { Render.Circle.DrawCircle(ObjectManager.Player.Position, _r.Range, _root.Item("DrawR").GetValue<Circle>().Color); }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (_root.Item("ComboActive").GetValue<KeyBind>().Active)
            {
                var target = TargetSelector.GetTarget(1000f, TargetSelector.DamageType.Magical);
                if (target != null)
                {
                    if (_root.Item("UseQCombo").GetValue<bool>())
                    {
                        if (ObjectManager.Player.Distance(target) < _q.Range && _q.IsReady())
                        { _q.Cast(target, true); }
                        else
                        {
                            if (ObjectManager.Player.Distance(target) > _q.Range && _q.IsReady() && _e.IsReady())
                            {
                                var minions = MinionManager.GetMinions(_e.Range, MinionTypes.All, MinionTeam.NotAlly);

                                foreach (var minion in minions)
                                {
                                    if (ObjectManager.Player.Distance(minion) < _e.Range && ObjectManager.Player.Distance(target) < 650f + 925f)
                                    {
                                        _e.CastOnUnit(minion);
                                        _q.Cast(target, true);
                                    }
                                }
                            }
                        }
                    }
                    if (_root.Item("UseECombo").GetValue<bool>())
                    {
                        if (ObjectManager.Player.Distance(target) < _e.Range && _e.IsReady())
                        { _e.CastOnUnit(target); }
                    }

                    if (_root.Item("UseIgniteCombo").GetValue<bool>())
                    {
                        if (_igniteSlot != SpellSlot.Unknown && ObjectManager.Player.Spellbook.CanUseSpell(_igniteSlot) == SpellState.Ready)
                        { ObjectManager.Player.Spellbook.CastSpell(_igniteSlot, target); }
                    }

                    if (_root.Item("UseWCombo").GetValue<bool>())
                    {
                        if (ObjectManager.Player.Distance(target) < _w.Range && _w.IsReady())
                        { _w.CastOnUnit(target); }
                    }
                }
            }

            if (_root.Item("HarassActive").GetValue<KeyBind>().Active)
            {
                var hTarget = TargetSelector.GetTarget(1000f, TargetSelector.DamageType.Magical);
                if (hTarget != null)
                {
                    if (ObjectManager.Player.Distance(hTarget) < ObjectManager.Player.AttackRange)
                    { EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, hTarget); }

                    if (_root.Item("UseQHarass").GetValue<bool>())
                    {
                        if (ObjectManager.Player.Distance(hTarget) < _q.Range && _q.IsReady())
                        { _q.Cast(hTarget); }
                        else
                        {
                            if (ObjectManager.Player.Distance(hTarget) > _q.Range && _q.IsReady() && _e.IsReady())
                            {
                                var minions = MinionManager.GetMinions(_e.Range, MinionTypes.All, MinionTeam.NotAlly);

                                foreach (var minion in minions)
                                {
                                    if (ObjectManager.Player.Distance(minion) < _e.Range && ObjectManager.Player.Distance(hTarget) < 650f + 925f)
                                    {
                                        _e.CastOnUnit(minion);
                                        _q.Cast(hTarget, true);
                                    }
                                }
                            }
                        }
                    }

                    if (_root.Item("UseEHarass").GetValue<bool>())
                    {
                        if (ObjectManager.Player.Distance(hTarget) < _e.Range && _e.IsReady())
                        { _e.CastOnUnit(hTarget); }
                    }

                    if (_root.Item("UseWHarass").GetValue<bool>())
                    {
                        if (ObjectManager.Player.Distance(hTarget) < _w.Range && _w.IsReady())
                        { _w.CastOnUnit(hTarget); }
                    }
                }
            }

            if (_root.Item("DefActive").GetValue<bool>())
            {
                // E spell
                var AllyE =
                    ObjectManager.Get<Obj_AI_Base>()
                        .Where(x => !x.IsEnemy)
                        .Where(x => !x.IsDead)
                        .Where(x => x.Distance(ObjectManager.Player.Position) <= _e.Range)
                        .Where(x => x.Health <= x.MaxHealth * (_root.Item("AutoEAlly").GetValue<Slider>().Value / 100));

                var PlayerE = ObjectManager.Player.MaxHealth / 100 * _root.Item("AutoE").GetValue<Slider>().Value;
                if (ObjectManager.Player.Health <= PlayerE)
                { _e.CastOnUnit(ObjectManager.Player); }
                else
                {
                    if (AllyE != null)
                    { _e.CastOnUnit((Obj_AI_Base)AllyE); }
                }

                // R Spell
                var AllyR =
                    ObjectManager.Get<Obj_AI_Base>()
                        .Where(x => !x.IsEnemy)
                        .Where(x => !x.IsDead)
                        .Where(x => x.Distance(ObjectManager.Player.Position) <= _r.Range)
                        .Where(x => x.Health <= x.MaxHealth * (_root.Item("AutoRAlly").GetValue<Slider>().Value / 100));
                var PlayerR = ObjectManager.Player.MaxHealth / 100 * _root.Item("AutoR").GetValue<Slider>().Value;
                if (ObjectManager.Player.Health <= PlayerR)
                { _r.CastOnUnit(ObjectManager.Player); }
                else
                {
                    if (AllyR != null)
                    { _r.CastOnUnit((Obj_AI_Base)AllyR); }
                }
            }

            if (_root.Item("StealActive").GetValue<bool>())
            {
                if (_root.Item("UseQSteal").GetValue<bool>())
                {
                    var target = TargetSelector.GetTarget(1000f, TargetSelector.DamageType.Magical);
                    var dmgQ = ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q);

                    if (ObjectManager.Player.Distance(target) <= _q.Range && _q.IsReady())
                    {
                        if (dmgQ >= target.Health + 35)
                        { _q.Cast(target); }
                    }
                    else
                    {
                        if (ObjectManager.Player.Distance(target) <= _q.Range + _e.Range && _q.IsReady() && _e.IsReady())
                        {
                            var minions = MinionManager.GetMinions(_e.Range, MinionTypes.All, MinionTeam.NotAlly);
                            foreach (var minion in minions)
                            {
                                _e.CastOnUnit(minion);
                                _q.Cast(target, true);
                            }
                        }
                    }
                }

                if (_root.Item("UseESteal").GetValue<bool>())
                {
                    var target = TargetSelector.GetTarget(_e.Range, TargetSelector.DamageType.Magical);
                    var dmgE = ObjectManager.Player.GetSpellDamage(target, SpellSlot.E);
                    if (ObjectManager.Player.Distance(target) <= _e.Range && _e.IsReady())
                    {
                        if (dmgE >= target.Health + 35)
                        { _e.CastOnUnit(target); }
                    }

                }
            }

            if (_root.Item("LaneClearActive").GetValue<KeyBind>().Active)
            {
                foreach (
                    var minion in
                        ObjectManager.Get<Obj_AI_Minion>()
                            .Where(minion => minion.Team != ObjectManager.Player.Team)
                    )
                {
                    if (_root.Item("UseQFarm").GetValue<bool>())
                    {
                        if (ObjectManager.Player.Distance(minion) <= _q.Range && _q.IsReady())
                        { _q.Cast(minion); }
                    }
                    if (_root.Item("UseEFarm").GetValue<bool>())
                    {
                        if (ObjectManager.Player.Distance(minion) <= _e.Range && _e.IsReady())
                        { _e.CastOnUnit(minion); }
                    }
                }
            }
        }
    }
}
