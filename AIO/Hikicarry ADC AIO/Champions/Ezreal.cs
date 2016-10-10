using System;
using System.Drawing;
using System.Linq;
using HikiCarry.Core.Plugins;
using HikiCarry.Core.Predictions;
using HikiCarry.Core.Utilities;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HikiCarry.Champions
{
    internal class Ezreal
    {
        internal static Spell Q;
        internal static Spell W;
        internal static Spell E;
        internal static Spell R;

        public Ezreal()
        {

            Q = new Spell(SpellSlot.Q, 1150f);
            W = new Spell(SpellSlot.W, 1000f);
            E = new Spell(SpellSlot.E, 475f);
            R = new Spell(SpellSlot.R, 3000f);

            Q.SetSkillshot(0.25f, 60f, 2000f, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.25f, 80f, 1600f, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(1.0f, 160f, 2000f, false, SkillshotType.SkillshotLine);

            var comboMenu = new Menu("Combo Settings", "Combo Settings");
            {
                comboMenu.AddItem(new MenuItem("q.combo", "Use (Q)", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("w.combo", "Use (W)", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("r.combo", "Use (R)", true).SetValue(true));
                Initializer.Config.AddSubMenu(comboMenu);
            }
            
            var harassMenu = new Menu("Harass Settings", "Harass Settings");
            {
                harassMenu.AddItem(new MenuItem("q.harass", "Use (Q)", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("w.harass", "Use (W)", true).SetValue(false));
                harassMenu.AddItem(new MenuItem("harass.mana", "Harass Mana Percent", true).SetValue(new Slider(30, 1, 99)));
                Initializer.Config.AddSubMenu(harassMenu);
            }

            var clearmenu = new Menu("LaneClear Settings","LaneClear Settings");
            {
                clearmenu.AddItem(new MenuItem("q.laneclear", "Use (Q)", true).SetValue(true));
                clearmenu.AddItem(new MenuItem("clear.mana", "Clear Mana Percent", true).SetValue(new Slider(30, 1, 99)));
                Initializer.Config.AddSubMenu(clearmenu);
            }

            var junglemenu = new Menu("Jungle Settings", "Jungle Settings");
            {
                junglemenu.AddItem(new MenuItem("q.jungle", "Use (Q)", true).SetValue(true));
                junglemenu.AddItem(new MenuItem("jungle.mana", "Jungle Mana Percent", true).SetValue(new Slider(30, 1, 99)));
                Initializer.Config.AddSubMenu(junglemenu);
            }
            var drawMenu = new Menu("Draw Settings", "Draw Settings");
            {
                var drawDamageMenu = new MenuItem("RushDrawEDamage", "Combo Damage").SetValue(true);
                var drawFill = new MenuItem("RushDrawEDamageFill", "Combo Damage Fill").SetValue(new Circle(true, System.Drawing.Color.Gold));

                drawMenu.SubMenu("Damage Draws").AddItem(drawDamageMenu);
                drawMenu.SubMenu("Damage Draws").AddItem(drawFill);

                DamageIndicator.DamageToUnit = TotalDamage;
                DamageIndicator.Enabled = drawDamageMenu.GetValue<bool>();
                DamageIndicator.Fill = drawFill.GetValue<Circle>().Active;
                DamageIndicator.FillColor = drawFill.GetValue<Circle>().Color;

                drawDamageMenu.ValueChanged +=
                delegate (object sender, OnValueChangeEventArgs eventArgs)
                {
                    DamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
                };

                drawFill.ValueChanged +=
                delegate (object sender, OnValueChangeEventArgs eventArgs)
                {
                    DamageIndicator.Fill = eventArgs.GetNewValue<Circle>().Active;
                    DamageIndicator.FillColor = eventArgs.GetNewValue<Circle>().Color;
                };
                Initializer.Config.AddSubMenu(drawMenu);
            }
            Game.OnUpdate += EzrealOnUpdate;

        }

        private float TotalDamage(AIHeroClient hero)
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

        private void EzrealOnUpdate(EventArgs args)
        {
            switch (Initializer.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    OnCombo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    OnMixed();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    OnClear();
                    OnJungle();
                    break;
            }
        }

        private static void OnCombo()
        {
            var target = TargetSelector.GetTarget(2000, TargetSelector.DamageType.Physical);
            if (target != null)
            {
                if (Q.IsReady() && Utilities.Enabled("q.combo") && target.IsValidTarget(Q.Range))
                {
                    Q.Do(target, Utilities.HikiChance("hitchance"));
                }

                if (W.IsReady() && Utilities.Enabled("w.combo") && target.IsValidTarget(W.Range))
                {
                    W.Do(target,Utilities.HikiChance("hitchance"));
                }

                if (R.IsReady() && Utilities.Enabled("r.combo"))
                {
                    if (target.IsValidTarget(R.Range - 1000) && R.GetDamage(target) > target.Health)
                    {
                        R.Do(target,Utilities.HikiChance("hitchance"));
                    }
                }
            }

        }

        private static void OnMixed()
        {
            if (ObjectManager.Player.ManaPercent < Utilities.Slider("harass.mana"))
            {
                return;
            }

            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            if (target != null)
            {
                if (Q.IsReady() && Utilities.Enabled("q.harass") && target.IsValidTarget(Q.Range))
                {
                    Q.Do(target, Utilities.HikiChance("hitchance"));
                }

                if (W.IsReady() && Utilities.Enabled("w.harass") && target.IsValidTarget(W.Range))
                {
                    W.Do(target, Utilities.HikiChance("hitchance"));
                }
            }
        }

        private static void OnClear()
        {
            if (ObjectManager.Player.ManaPercent < Utilities.Slider("clear.mana"))
            {
                return;
            }

            if (Q.IsReady() && Utilities.Enabled("q.laneclear"))
            {
                foreach (var minion in MinionManager.GetMinions(Q.Range).Where(x=> x.IsValidTarget(Q.Range) && x.Health < Q.GetDamage(x)))
                {
                    Q.Do(minion,HitChance.High);
                }
            }
        }

        private static void OnJungle()
        {
            if (ObjectManager.Player.ManaPercent < Utilities.Slider("jungle.mana"))
            {
                return;
            }

            if (Q.IsReady() && Utilities.Enabled("q.jungle"))
            {
                var target = MinionManager.GetMinions(ObjectManager.Player.AttackRange, MinionTypes.All,MinionTeam.Neutral, MinionOrderTypes.MaxHealth)
                    .FirstOrDefault(x => x.IsValidTarget(Q.Range));

                if (target != null)
                {
                    Q.Do(target,HitChance.High);
                }

            }
        }
    }
}
