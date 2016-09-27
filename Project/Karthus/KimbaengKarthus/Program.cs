using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Kimbaeng_KarThus
{
    using SharpDX.Direct3D9;

    internal class Program
    {
        public static Menu _menu;

        private static AIHeroClient Player;

        private static Orbwalking.Orbwalker _orbwalker;

        public static Spell Q;

        public static Spell W;

        public static Spell E;

        public static Spell R;

        private static bool _comboE;

        private static Vector2 PingLocation;

        private static int LastPingT = 0;

        private const float SpellQWidth = 160f;

        public static SpellSlot IgniteSlot;

        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName != "Karthus") return;


            Player = ObjectManager.Player;
            IgniteSlot = ObjectManager.Player.GetSpellSlot("SummonerDot");
            Q = new Spell(SpellSlot.Q, 875f);
            W = new Spell(SpellSlot.W, 990f);
            E = new Spell(SpellSlot.E, 425f);
            R = new Spell(SpellSlot.R, float.MaxValue);

            Q.SetSkillshot(1f, 150f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            W.SetSkillshot(0.25f, 5f, float.MaxValue, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(1f, 520f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(3f, float.MaxValue, float.MaxValue, false, SkillshotType.SkillshotCircle);


            (_menu = new Menu("Kimbaeng Karthus", "kimbaengkarthus", true)).AddToMainMenu();

            var targetSelectorMenu = new Menu("Target Selector", "TargetSelector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            _menu.AddSubMenu(targetSelectorMenu);

            _orbwalker = new Orbwalking.Orbwalker(_menu.AddSubMenu(new Menu("Orbwalking", "Orbwalking")));
            _orbwalker.SetAttack(true);

            var HitchanceMenu = _menu.AddSubMenu(new Menu("Hitchance", "Hitchance"));
            HitchanceMenu.AddItem(
                new MenuItem("Hitchance", "Hitchance").SetValue(
                    new StringList(new[] { "Low", "Medium", "High", "VeryHigh", "Impossible" }, 3)));

            var comboMenu = _menu.AddSubMenu(new Menu("combo", "Combo"));
            comboMenu.AddItem(new MenuItem("useQ", "Use Q").SetValue(true));
            comboMenu.AddItem(new MenuItem("useW", "Use W").SetValue(true));
            comboMenu.AddItem(new MenuItem("useE", "Use E").SetValue(true));
            comboMenu.AddItem(new MenuItem("comboAA", "Use AA").SetValue(false));
            comboMenu.AddItem(new MenuItem("string", "if No Mana(100↓), Allow Use AA"));
            comboMenu.AddItem(new MenuItem("UseI", "Use Ignite").SetValue(true));

            var harassMenu = _menu.AddSubMenu(new Menu("Harass", "Harass"));
            harassMenu.AddItem(new MenuItem("useQHarass", "UseQ").SetValue(true));
            harassMenu.AddItem(new MenuItem("useEHarass", "UseE").SetValue(true));
            harassMenu.AddItem(new MenuItem("harassAA", "Use AA").SetValue(false));
            harassMenu.AddItem(new MenuItem("autoqh", "Auto Q Harass").SetValue(false));
            harassMenu.AddItem(new MenuItem("harassmana", "Mana %").SetValue(new Slider(50)));

            var LastHitMenu = _menu.AddSubMenu(new Menu("LastHit", "LastHit"));
            LastHitMenu.AddItem(new MenuItem("useqlasthit", "Use Q").SetValue(true));

            var MiscMenu = _menu.AddSubMenu(new Menu("Misc", "Misc"));
            var ultMenu = MiscMenu.AddSubMenu(new Menu("Ult", "Ult"));
            ultMenu.AddItem(new MenuItem("NotifyUlt", "Notify Ult Text").SetValue(true));
            ultMenu.AddItem(new MenuItem("NotifyPing", "Notify Ult Ping").SetValue(true));

            MiscMenu.AddItem(new MenuItem("estate", "Auto E if No Target").SetValue(true));

            var DrawMenu = _menu.AddSubMenu(new Menu("Draw", "drawing"));
            DrawMenu.AddItem(new MenuItem("noDraw", "Disable Drawing").SetValue(true));
            DrawMenu.AddItem(new MenuItem("drawQ", "DrawQ").SetValue(new Circle(true, System.Drawing.Color.Goldenrod)));
            DrawMenu.AddItem(new MenuItem("drawW", "DrawW").SetValue(new Circle(false, System.Drawing.Color.Goldenrod)));
            DrawMenu.AddItem(new MenuItem("drawE", "DrawE").SetValue(new Circle(false, System.Drawing.Color.Goldenrod)));
            DrawMenu.SubMenu("Draw Damage").AddItem(new MenuItem("damagedraw", "Draw Combo Damage").SetValue(true));
            DrawMenu.SubMenu("Draw Damage").AddItem(new MenuItem("damagedrawfill", "Fill Color").SetValue(new Circle(true, System.Drawing.Color.Gold)));

            damageindicator.DamageToUnit = TotalDamage;
            damageindicator.Enabled = DrawMenu.SubMenu("Draw Damage").Item("damagedraw").GetValue<bool>();
            damageindicator.Fill = DrawMenu.SubMenu("Draw Damage").Item("damagedrawfill").GetValue<Circle>().Active;
            damageindicator.FillColor = DrawMenu.SubMenu("Draw Damage").Item("damagedrawfill").GetValue<Circle>().Color;


            Drawing.OnDraw += Drawing_Ondraw;
            Game.OnUpdate += Game_OnUpdate;
            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;

            Chat.Print("Kimbaeng<font color=\"#030066\">Karthus</font> Loaded");
            Chat.Print("If You like this Assembly plz <font color=\"#1DDB16\">Upvote</font> XD ");
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (_menu.Item("NotifyUlt").GetValue<bool>())
            {
                AutoUlt();
            }

            if (_menu.Item("NotifyPing").GetValue<bool>())
            {
                NotifyPing();
            }

            if (_menu.Item("autoqh").GetValue<bool>())
            {
                Harass();
            }
            if (_orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                _orbwalker.SetAttack(_menu.Item("comboAA").GetValue<bool>() || ObjectManager.Player.Mana < 100);
                Combo();
            }


            if (_orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                _orbwalker.SetAttack(_menu.Item("harassAA").GetValue<bool>());
                Harass();
                LastHit();
            }

            if (_orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                LaneClear();
            }
            if (_orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit)
            {
                LastHit();
            }
            RegulateEState();
            PassiveForm();
        }

        private static void Drawing_Ondraw(EventArgs args)
        {
            if (_menu.Item("noDraw").GetValue<bool>())
            {
                return;
            }

            var qValue = _menu.Item("drawQ").GetValue<Circle>();
            var wValue = _menu.Item("drawW").GetValue<Circle>();
            var eValue = _menu.Item("drawE").GetValue<Circle>();

            if (qValue.Active)
            {
                if (Q.Instance.Level != 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, qValue.Color);
            }

            if (wValue.Active)
            {
                if (W.Instance.Level != 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, wValue.Color);
            }

            if (eValue.Active)
            {
                if (E.Instance.Level != 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, eValue.Color);
            }

        }

        private static void AutoUlt()
        {
            if (!R.IsReady()) return;
            var pos = Drawing.WorldToScreen(ObjectManager.Player.Position)[1] + 20;
            foreach (var hero in
                    ObjectManager.Get<AIHeroClient>()
                        .Where(
                            x => ObjectManager.Player.GetSpellDamage(x, SpellSlot.R) >= x.Health && x.IsValidTarget()))
            {
                Drawing.DrawText(
                    Drawing.WorldToScreen(Player.Position)[0] - 30,
                    pos,
                    System.Drawing.Color.Gold,
                    "Can Kill : " + hero.ChampionName);
                pos += 20;
            }
        }

        private static void NotifyPing()
        {
            if (R.Instance.Level == 0)
            {
                return;
            }
            else
                foreach (var enemy in
                    HeroManager.Enemies.Where(
                        t =>
                        ObjectManager.Player.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Ready && t.IsValidTarget()
                        && R.GetDamage(t) > t.Health && t.Distance(ObjectManager.Player.Position) > Q.Range))
                {
                    Ping(enemy.Position.To2D());
                }
        }



        private static void LastHit()
        {
            if (!Orbwalking.CanMove(40)) return;

            var minions = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly);
            minions.RemoveAll(x => x.MaxHealth <= 5);

            foreach (var minion in
                minions.Where(
                    x => ObjectManager.Player.GetSpellDamage(x, SpellSlot.Q, 1) >=
                    HealthPrediction.GetHealthPrediction(x, (int)(Q.Delay * 1000))))
            {
                Q.Cast(minion);
            }
        }

        private static void LaneClear()
        {
            var minions = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health);

            if (!minions.Any())
                return;

            var minion = minions.First();

            if (Q.IsReady() && minion.IsValidTarget(Q.Range))
            {
                Q.Cast(minion.ServerPosition);
            }
        }
    
        private static void RegulateEState(bool ignoreTargetChecks = false)
        {
            if (_menu.Item("estate").GetValue<bool>())
            {
                if (!E.IsReady() || IsInPassiveForm()
                    || ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).ToggleState != 2) return;
                var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
                var minions = MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition,
                    E.Range,
                    MinionTypes.All,
                    MinionTeam.NotAlly);

                if (!ignoreTargetChecks && (target != null || (!_comboE && minions.Count != 0))) return;
                E.CastOnUnit(ObjectManager.Player);
                _comboE = false;
            }
        }

        private static bool IsInPassiveForm()
        {
            return ObjectManager.Player.IsZombie;
        }

        private static void PassiveForm()
        {
            if (Player.IsZombie)
            {
                var Target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

                if (Target != null)
                {
                    Combo();
                }
                else
                {
                    LaneClear();
                }
            }

        }

        private static void Ping(Vector2 position)
        {
            if (LeagueSharp.Common.Utils.TickCount - LastPingT < 30 * 1000)
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
        private static float TotalDamage(AIHeroClient hero)
        {
            var damage = 0d;
            if (Q.IsReady())
            {
                damage += Q.GetDamage(hero);
            }
            if (W.IsReady())
            {
                damage += W.GetDamage(hero);
            }
            if (E.IsReady())
            {
                damage += W.GetDamage(hero);
            }
            if (R.IsReady())
            {
                damage += R.GetDamage(hero);
            }
            return (float)damage;
        }

        private static void Combo()
        {
            var wTarget = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
            var eTarget = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            var UseQ = _menu.Item("useQ").GetValue<bool>();
            var UseW = _menu.Item("useW").GetValue<bool>();
            var UseE = _menu.Item("useE").GetValue<bool>();

            if (wTarget != null && UseW && W.IsReady())
            {
                W.Cast(wTarget, false, true);
            }

            if (eTarget != null && UseE && E.IsReady() && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).ToggleState == 1)
            {
                if (ObjectManager.Player.Distance(eTarget.ServerPosition) <= E.Range)
                {
                    _comboE = true;
                    E.Cast();
                }
            }

            else if (eTarget == null && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).ToggleState != 1)
            {
                E.Cast();
            }
            if (UseQ && Q.IsReady()) 
            {
                var HC = HitChance.VeryHigh;
                switch (_menu.Item("Hitchance").GetValue<StringList>().SelectedIndex)
                {
                    case 0: //Low
                        HC = HitChance.Low;
                        break;
                    case 1: //Medium
                        HC = HitChance.Medium;
                        break;
                    case 2: //High
                        HC = HitChance.High;
                        break;
                    case 3: //Very High
                        HC = HitChance.VeryHigh;
                        break;
                    case 4: //impossable
                        HC = HitChance.Impossible;
                        break;
                }
                var qTarget = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
                Q.CastIfHitchanceEquals(qTarget, HC, true);
            }

            if (IgniteSlot != SpellSlot.Unknown &&
    ObjectManager.Player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready &&
    ObjectManager.Player.Distance(wTarget.ServerPosition) < 600 &&
    Player.GetSummonerSpellDamage(wTarget, Damage.SummonerSpell.Ignite) > wTarget.Health)
            {
                ObjectManager.Player.Spellbook.CastSpell(IgniteSlot, wTarget);
            }

        }

        private static void Harass()
        {
            var eTarget = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            var UseQ = _menu.Item("useQHarass").GetValue<bool>();
            var UseE = _menu.Item("useEHarass").GetValue<bool>();

            var HC = HitChance.VeryHigh;
            if (UseQ && Q.IsReady())
            {
                switch (_menu.Item("Hitchance").GetValue<StringList>().SelectedIndex)
                {
                    case 0: //Low
                        HC = HitChance.Low;
                        break;
                    case 1: //Medium
                        HC = HitChance.Medium;
                        break;
                    case 2: //High
                        HC = HitChance.High;
                        break;
                    case 3: //Very High
                        HC = HitChance.VeryHigh;
                        break;
                    case 4: //impossable
                        HC = HitChance.Impossible;
                        break;
                }
                var qTarget = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
                Q.CastIfHitchanceEquals(qTarget, HC, true);

            }

            if (eTarget != null && UseE && E.IsReady() && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).ToggleState == 1)
            {
                E.Cast();
            }
            else if (eTarget == null && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).ToggleState != 1)
            {
                E.Cast();
            }
        }

        private static void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (_orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                args.Process = !Q.IsReady();
            }
            else if (_orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit)
            {
                var farmQ = _menu.Item("useqlasthit").GetValue<bool>();
                args.Process =
                    !(farmQ && Q.IsReady());
            }
        }

    }
}
