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
    internal class Varus
    {
        internal static Spell Q;
        internal static Spell W;
        internal static Spell E;
        internal static Spell R;

        public Varus()
        {

            Q = new Spell(SpellSlot.Q, 1625);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 925);
            R = new Spell(SpellSlot.R, 1075);

            Q.SetCharged("VarusQ", "VarusQ", 250, 1600, 1.2f);

            Q.SetSkillshot(0.25f, 70f, 1500f, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(.50f, 250, 1400, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(.25f, 120, 1950, false, SkillshotType.SkillshotLine);

            var comboMenu = new Menu("Combo Settings", "Combo Settings");
            {
                comboMenu.AddItem(new MenuItem("q.combo", "Use (Q)", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("q.combo.charge", "(Q) Minimum Charge", true).SetValue(new Slider(700, 1, 1600)));
                comboMenu.AddItem(new MenuItem("e.combo", "Use (E)", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("r.combo", "Use (R)", true).SetValue(true));
                Initializer.Config.AddSubMenu(comboMenu);
            }
            
            var harassMenu = new Menu("Harass Settings", "Harass Settings");
            {
                harassMenu.AddItem(new MenuItem("q.harass", "Use (Q)", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("q.harass.charge", "(Q) Minimum Charge", true).SetValue(new Slider(700, 1, 1600)));
                harassMenu.AddItem(new MenuItem("e.harass", "Use (E)", true).SetValue(false));
                harassMenu.AddItem(new MenuItem("harass.mana", "Harass Mana Percent", true).SetValue(new Slider(30, 1, 99)));
                Initializer.Config.AddSubMenu(harassMenu);
            }

            var clearmenu = new Menu("LaneClear Settings","LaneClear Settings");
            {
                clearmenu.AddItem(new MenuItem("q.laneclear", "Use (Q)", true).SetValue(true));
                clearmenu.AddItem(new MenuItem("q.clear.charge", "(Q) Minimum Charge", true).SetValue(new Slider(700, 1, 1600)));
                clearmenu.AddItem(new MenuItem("q.minion.count", "(Q) Min. Minion Count", true).SetValue(new Slider(3, 1, 5)));
                clearmenu.AddItem(new MenuItem("e.laneclear", "Use (E)", true).SetValue(true));
                clearmenu.AddItem(new MenuItem("e.minion.count", "(E) Min. Minion Count", true).SetValue(new Slider(3, 1, 5)));
                clearmenu.AddItem(new MenuItem("clear.mana", "Clear Mana Percent", true).SetValue(new Slider(30, 1, 99)));
                Initializer.Config.AddSubMenu(clearmenu);
            }

            var junglemenu = new Menu("Jungle Settings", "Jungle Settings");
            {
                junglemenu.AddItem(new MenuItem("q.jungle", "Use (Q)", true).SetValue(true));
                junglemenu.AddItem(new MenuItem("e.jungle", "Use (E)", true).SetValue(true));
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
            Game.OnUpdate += VarusOnUpdate;

        }

        private float TotalDamage(AIHeroClient hero)
        {
            var damage = 0d;
            if (Q.IsReady())
            {
                damage += Q.GetDamage(hero);
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

        private void VarusOnUpdate(EventArgs args)
        {
            Initializer.Orbwalker.SetAttack(!Q.IsCharging);

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
                    Q.StartCharging();
                    if (Q.IsCharging)
                    {
                        if (Q.Range >= Utilities.Slider("q.combo.charge"))
                        {
                            Q.Do(target,Utilities.HikiChance("hitchance"));
                        }
                    }
                    
                }
                if (E.IsReady() && Utilities.Enabled("e.combo") && target.IsValidTarget(E.Range))
                {
                    E.Do(target,Utilities.HikiChance("hitchance"));
                }

                if (R.IsReady() && Utilities.Enabled("r.combo"))
                {
                    if (target.IsValidTarget(R.Range - 1000) && R.GetDamage(target) > target.Health)
                    {
                        R.Do(target,Utilities.HikiChance("hitchance"));
                    }

                    if (target.IsValidTarget(ObjectManager.Player.AttackRange) && Q.IsReady() && 
                        target.Health < R.GetDamage(target) + Q.GetDamage(target))
                    {
                        R.Do(target, Utilities.HikiChance("hitchance"));
                    }

                    if (target.IsValidTarget(ObjectManager.Player.AttackRange) && Q.IsReady() &&
                        target.Health < R.GetDamage(target) / 2 && Utilities.IsImmobile(target))
                    {
                        R.Do(target, Utilities.HikiChance("hitchance"));
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
                    Q.StartCharging();
                    if (Q.IsCharging)
                    {
                        if (Q.Range >= Utilities.Slider("q.combo.charge"))
                        {
                            Q.Do(target, Utilities.HikiChance("hitchance"));
                        }
                    }
                }

                if (E.IsReady() && Utilities.Enabled("e.harass") && target.IsValidTarget(E.Range))
                {
                    E.Do(target, Utilities.HikiChance("hitchance"));
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
                var minionlist = MinionManager.GetMinions(ObjectManager.Player.Position, Q.Range);
                if (minionlist.Count() >= Utilities.Slider("q.minion.count"))
                {
                    Q.StartCharging();
                    var whitlist = W.GetLineFarmLocation(minionlist);
                    if (Q.IsCharging && Q.Range >= Utilities.Slider("q.clear.charge")
                        && whitlist.MinionsHit >= Utilities.Slider("q.minion.count"))
                    {
                        Q.Cast(whitlist.Position);
                    }
                }
            }

            if (E.IsReady() && Utilities.Enabled("e.laneclear"))
            {
                var minionlist = MinionManager.GetMinions(ObjectManager.Player.Position, Q.Range);
                var whitlist = W.GetCircularFarmLocation(minionlist);
                if (whitlist.MinionsHit >= Utilities.Slider("e.minion.count"))
                {
                    E.Cast(whitlist.Position);
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

            if (E.IsReady() && Utilities.Enabled("e.jungle"))
            {
                var target = MinionManager.GetMinions(ObjectManager.Player.AttackRange, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth)
                    .FirstOrDefault(x => x.IsValidTarget(E.Range));

                if (target != null)
                {
                    E.Do(target, HitChance.High);
                }

            }
        }
    }
}
