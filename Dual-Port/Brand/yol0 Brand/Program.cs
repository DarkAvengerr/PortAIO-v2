using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace yol0Brand
{
    internal class Program
    {
        private static AIHeroClient Player { get { return ObjectManager.Player; } }
        private static Spell _Q = new Spell(SpellSlot.Q, 1050);
        private static Spell _W = new Spell(SpellSlot.W, 900);
        private static Spell _E = new Spell(SpellSlot.E, 625);
        private static Spell _R = new Spell(SpellSlot.R, 750);

        private static Spell _Ignite = new Spell(SpellSlot.Unknown, 600);
        
        private static Menu Config;
        private static Orbwalking.Orbwalker _orbwalker;
        private static AIHeroClient comboTarget;
        public static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Brand")
                return;

            Config = new Menu("yol0 Brand", "yol0Brand", true);
            Config.AddSubMenu(new Menu("Target Selector", "Target Selector"));
            Config.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.AddSubMenu(new Menu("Harass", "Harass"));
            Config.AddSubMenu(new Menu("Lane Clear", "Farm"));
            Config.AddSubMenu(new Menu("KS", "KS"));
            Config.AddSubMenu(new Menu("Misc", "Misc"));
            Config.AddSubMenu(new Menu("Drawing", "Drawing"));

            TargetSelector.AddToMenu(Config.SubMenu("Target Selector"));
            _orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker"));

            Config.SubMenu("Combo").AddItem(new MenuItem("useQ", "Use Q").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("useW", "Use W").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("useE", "Use E").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("useR", "Use R").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("blaze", "Use E before Q").SetValue(false));
            Config.SubMenu("Combo").AddItem(new MenuItem("useI", "Use Ignite").SetValue(true));

            Config.SubMenu("Harass").AddItem(new MenuItem("useQ", "Use Q").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("useW", "Use W").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("useE", "Use E").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("mana", "Mana Manager").SetValue(new Slider(40, 0, 100)));

            Config.SubMenu("Farm").AddItem(new MenuItem("useQ", "Use Q").SetValue(false));
            Config.SubMenu("Farm").AddItem(new MenuItem("useW", "Use W").SetValue(true));
            Config.SubMenu("Farm").AddItem(new MenuItem("useE", "Use E").SetValue(false));

            Config.SubMenu("KS").AddItem(new MenuItem("ksQ", "KS with Q").SetValue(true));
            Config.SubMenu("KS").AddItem(new MenuItem("ksW", "KS with W").SetValue(true));
            Config.SubMenu("KS").AddItem(new MenuItem("ksE", "KS with E").SetValue(true));
            Config.SubMenu("KS").AddItem(new MenuItem("ksR", "KS with R").SetValue(true));

            Config.SubMenu("Misc").AddItem(new MenuItem("gapclose", "Auto Stun Gapclosers").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("interrupt", "Auto Interrupt").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("ignite", "Auto Ignite").SetValue(true));

            Config.SubMenu("Drawing").AddItem(new MenuItem("drawQ", "Draw Q Range").SetValue(new Circle(true, Color.Green)));
            Config.SubMenu("Drawing").AddItem(new MenuItem("drawW", "Draw W Range").SetValue(new Circle(true, Color.Green)));
            Config.SubMenu("Drawing").AddItem(new MenuItem("drawE", "Draw E Range").SetValue(new Circle(true, Color.Green)));
            Config.SubMenu("Drawing").AddItem(new MenuItem("drawR", "Draw R Range").SetValue(new Circle(true, Color.Green)));
            Config.SubMenu("Drawing").AddItem(new MenuItem("drawDamage", "Draw Healthbar Damage").SetValue(true));
            Config.SubMenu("Drawing").AddItem(new MenuItem("drawKill", "Draw Killable").SetValue(true));

            Config.AddToMainMenu();

            LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnit = delegate(AIHeroClient enemy)
            {
                return (float)GetDamage(enemy);
            };
            
            _Q.SetSkillshot(0.625f, 50f, 1600f, true, SkillshotType.SkillshotLine);
            _W.SetSkillshot(1.0f, 240f, int.MaxValue, false, SkillshotType.SkillshotCircle);

            var ignite = Player.Spellbook.Spells.FirstOrDefault(spell => spell.Name == "summonerdot");
            if (ignite != null)
                _Ignite.Slot = ignite.Slot;
            

            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Interrupter2.OnInterruptableTarget += OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
        }

        private static void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!Config.SubMenu("Misc").Item("gapcloser").GetValue<bool>())
                return;

            if (gapcloser.Sender.HasBuff("brandablaze") && _Q.IsReady())
            {
                _Q.Cast(gapcloser.Sender);
            }
            else
            {
                if (_E.IsReady() && _Q.IsReady())
                {
                    _E.CastOnUnit(gapcloser.Sender);
                }
            }
        }

        private static void OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!Config.SubMenu("Misc").Item("interrupt").GetValue<bool>())
                return;

            if (sender.HasBuff("brandablaze") && _Q.IsReady())
            {
                _Q.Cast(sender);
            }
            else
            {
                if (_E.IsReady() && _Q.IsReady())
                {
                    _E.CastOnUnit(sender);
                }
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (Config.SubMenu("Drawing").Item("drawQ").GetValue<Circle>().Active)
            {
                Render.Circle.DrawCircle(Player.Position, _Q.Range, Config.SubMenu("Drawing").Item("drawQ").GetValue<Circle>().Color);
            }
            if (Config.SubMenu("Drawing").Item("drawW").GetValue<Circle>().Active)
            {
                Render.Circle.DrawCircle(Player.Position, _W.Range, Config.SubMenu("Drawing").Item("drawW").GetValue<Circle>().Color);
            }
            if (Config.SubMenu("Drawing").Item("drawE").GetValue<Circle>().Active)
            {
                Render.Circle.DrawCircle(Player.Position, _E.Range, Config.SubMenu("Drawing").Item("drawE").GetValue<Circle>().Color);
            }
            if (Config.SubMenu("Drawing").Item("drawR").GetValue<Circle>().Active)
            {
                Render.Circle.DrawCircle(Player.Position, _R.Range, Config.SubMenu("Drawing").Item("drawR").GetValue<Circle>().Color);
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = Config.SubMenu("Drawing").Item("drawDamage").GetValue<bool>();

            if (_orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                comboTarget = TargetSelector.GetTarget(_Q.Range, TargetSelector.DamageType.Magical);
                if (comboTarget.IsValid && comboTarget.IsValidTarget())
                {
                    Combo(comboTarget);
                }
            }
            else if (_orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                if (Config.SubMenu("Farm").Item("useW").GetValue<bool>())
                    CastWFarm();

                if (Config.SubMenu("Farm").Item("useQ").GetValue<bool>())
                    CastQFarm();

                if (Config.SubMenu("Farm").Item("useE").GetValue<bool>())
                    CastEFarm();
            }
            else if (_orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                comboTarget = TargetSelector.GetTarget(_Q.Range, TargetSelector.DamageType.Magical);
                if (comboTarget.IsValid && comboTarget.IsValidTarget())
                {
                    Harass(comboTarget);
                }
            }
            KS();
        }

        private static void Harass(AIHeroClient target)
        {
            if (Config.SubMenu("Harass").Item("useE").GetValue<bool>() && CheckMana(_E))
                CastE(target);
            if (Config.SubMenu("Harass").Item("useQ").GetValue<bool>() && CheckMana(_Q))
                CastQ(target);
            if (Config.SubMenu("Harass").Item("useW").GetValue<bool>() && CheckMana(_W))
                CastW(target);
        }

        private static bool CheckMana(Spell spell)
        {
            return (Player.Mana - spell.Instance.SData.Mana) / Player.MaxMana >= Config.SubMenu("Harass").Item("mana").GetValue<Slider>().Value / 100;
        }

        private static void Combo(AIHeroClient target)
        {
            if (Config.SubMenu("Combo").Item("useE").GetValue<bool>())
                CastE(target);  
            if (Config.SubMenu("Combo").Item("useQ").GetValue<bool>())
                CastQ(target);
            if (Config.SubMenu("Combo").Item("useW").GetValue<bool>())
                CastW(target);
            if (Config.SubMenu("Combo").Item("useR").GetValue<bool>())
                CastR(target);
        }

        private static void KS()
        {
            foreach (var enemy in HeroManager.Enemies)
            {
                if (_Ignite.Slot != SpellSlot.Unknown && Config.SubMenu("Misc").Item("ignite").GetValue<bool>() && Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite) > enemy.Health && enemy.IsValidTarget(_Ignite.Range))
                {
                    _Ignite.CastOnUnit(enemy);
                }
                else if (Config.SubMenu("KS").Item("ksQ").GetValue<bool>() && Player.GetSpellDamage(enemy, SpellSlot.Q) > enemy.Health && _Q.IsReady() && enemy.IsValidTarget(_Q.Range))
                {
                    _Q.Cast(enemy);
                }
                else if (Config.SubMenu("KS").Item("ksW").GetValue<bool>() && Player.GetSpellDamage(enemy, SpellSlot.W) > enemy.Health && _W.IsReady() && enemy.IsValidTarget(_W.Range))
                {
                    _W.Cast(enemy);
                }
                else if (Config.SubMenu("KS").Item("ksE").GetValue<bool>() && Player.GetSpellDamage(enemy, SpellSlot.E) > enemy.Health && _E.IsReady() && enemy.IsValidTarget(_E.Range))
                {
                    _E.Cast(enemy);
                }
                else if (Config.SubMenu("KS").Item("ksR").GetValue<bool>() && Player.GetSpellDamage(enemy, SpellSlot.R) > enemy.Health && _R.IsReady() && enemy.IsValidTarget(_R.Range))
                {
                    _R.Cast(enemy);
                }
            }
        }

        private static void CastQ(Obj_AI_Base target)
        {
            if (_orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && target.HasBuff("brandablaze") && Config.SubMenu("Combo").Item("blaze").GetValue<bool>())
            {
                if (_Q.IsReady() && target.IsValidTarget(_Q.Range))
                {
                    _Q.Cast(target);
                }
            }
            else
            {
                if (_Q.IsReady() && target.IsValidTarget(_Q.Range))
                {
                    _Q.Cast(target);
                }
            }
        }

        private static void CastW(Obj_AI_Base target)
        {
            if (_W.IsReady() && target.IsValidTarget(_W.Range))
            {
                _W.Cast(target, aoe: true);
            }
        }

        private static void CastE(Obj_AI_Base target)
        {
            if (_E.IsReady() && target.IsValidTarget(_E.Range))
            {
                _E.CastOnUnit(target);
            }
        }

        private static void CastR(Obj_AI_Base target)
        {
            if (_R.IsReady() && target.IsValidTarget(_R.Range))
            {
                _R.CastOnUnit(target);
            }
        }

        private static void CastWFarm()
        {
            if (!_W.IsReady())
                return;

            var minions = MinionManager.GetMinions(_W.Range);
            var positions = new List<Vector2>();
            foreach(var minion in minions)
            {
                positions.Add(minion.ServerPosition.To2D());
            }

            var location = MinionManager.GetBestCircularFarmLocation(positions, 240, _W.Range);
            if (location.MinionsHit >= 3)
            {
                _W.Cast(location.Position);
            }
        }

        private static void CastQFarm()
        {
            if (!_Q.IsReady())
                return;

            var minions = MinionManager.GetMinions(_Q.Range);
            foreach (var minion in minions)
            {
                if (Player.GetSpellDamage(minion, SpellSlot.Q) > minion.Health)
                {
                    _Q.Cast(minion);
                }
            }
        }

        private static void CastEFarm()
        {
            if (!_E.IsReady())
                return;

            var minions = MinionManager.GetMinions(_E.Range);
            foreach (var minion in minions)
            {
                if (Player.GetSpellDamage(minion, SpellSlot.E) > minion.Health)
                {
                    _E.CastOnUnit(minion);
                }
            }
        }

        private static double GetDamage(AIHeroClient target)
        {
            var pDamage = Damage.CalcDamage(Player, target, Damage.DamageType.Magical, (.08) * target.MaxHealth);
            var qDamage = Damage.GetSpellDamage(Player, target, SpellSlot.Q);
            var wDamage = Damage.GetSpellDamage(Player, target, SpellSlot.W);
            var eDamage = Damage.GetSpellDamage(Player, target, SpellSlot.E);
            var rDamage = Damage.GetSpellDamage(Player, target, SpellSlot.R);
            var iDamage = Damage.GetSummonerSpellDamage(Player, target, Damage.SummonerSpell.Ignite);
            var totalDamage = 0.0;

            var myMana = Player.Mana;
            var qMana = _Q.Instance.SData.Mana;
            var wMana = _W.Instance.SData.Mana;
            var eMana = _E.Instance.SData.Mana;
            var rMana = _R.Instance.SData.Mana;
            var totalMana = 0.0;

            if (!_Q.IsReady())
                qDamage = 0.0;
            if (!_W.IsReady())
                wDamage = 0.0;
            if (!_E.IsReady())
                eDamage = 0.0;
            if (!_R.IsReady())
                rDamage = 0.0;
            if (_Ignite.Slot == SpellSlot.Unknown)
                iDamage = 0.0;

            if (myMana >= eMana && myMana >= totalMana)
            {
                totalMana += eMana;
                totalDamage += eDamage;
            }

            if (myMana >= qMana && myMana >= totalMana)
            {
                totalMana += qMana;
                totalDamage += qDamage;
            }

            if (myMana >= wMana && myMana >= totalMana)
            {
                totalMana += wMana;
                totalDamage += wDamage;
            }

            if (myMana >= rMana && myMana >= totalMana)
            {
                totalMana += rMana;
                totalDamage += rDamage;
            }

            totalDamage += pDamage;
            totalDamage += iDamage;
            return totalDamage;
        }
    }
}
