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
    internal class Tristana
    {
        internal static Spell Q;
        internal static Spell W;
        internal static Spell E;
        internal static Spell R;

        public Tristana()
        {

            Q = new Spell(SpellSlot.Q, 585);
            W = new Spell(SpellSlot.W, 900);
            E = new Spell(SpellSlot.E, 630);
            R = new Spell(SpellSlot.R, 630);

            W.SetSkillshot(0.25f, 150, 1200, false, SkillshotType.SkillshotCircle);

            var comboMenu = new Menu("Combo Settings", "Combo Settings");
            {
                comboMenu.AddItem(new MenuItem("q.combo", "Use (Q)", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("q.combo.debuff", "Use (Q) If Enemy Debuffed", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("e.combo", "Use (E)", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("r.combo", "Use (R)", true).SetValue(true));
                Initializer.Config.AddSubMenu(comboMenu);
            }

            var esettings = new Menu("(E) Settings", "(E) Settings");
            {
                foreach (var enemy in HeroManager.Enemies)
                {
                    esettings.AddItem(new MenuItem("e." +enemy.ChampionName, "(E): "+ enemy.ChampionName, true).SetValue(true));
                }
                Initializer.Config.AddSubMenu(esettings);
            }

            var harassMenu = new Menu("Harass Settings", "Harass Settings");
            {
                harassMenu.AddItem(new MenuItem("q.harass", "Use (Q)", true).SetValue(false));
                harassMenu.AddItem(new MenuItem("e.harass", "Use (E)", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("harass.mana", "Harass Mana Percent", true).SetValue(new Slider(30, 1, 99)));
                Initializer.Config.AddSubMenu(harassMenu);
            }

            var clearmenu = new Menu("LaneClear Settings","LaneClear Settings");
            {
                clearmenu.AddItem(new MenuItem("q.laneclear", "Use (Q)", true).SetValue(true));
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
            Game.OnUpdate += TristanaOnUpdate;

        }

        private float TotalDamage(AIHeroClient hero)
        {
            var damage = 0d;
 
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

        private void TristanaOnUpdate(EventArgs args)
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
            var target = TargetSelector.GetTarget(800, TargetSelector.DamageType.Physical);
            if (target != null)
            {
                if (Q.IsReady() && Utilities.Enabled("q.combo") && target.IsValidTarget(Q.Range))
                {
                    if (Utilities.Enabled("q.combo.debuff") && target.HasBuff("tristanaecharge"))
                    {
                        Q.Cast();
                    }

                    if (!Utilities.Enabled("q.combo.debuff"))
                    {
                        Q.Cast();
                    }

                }

                if (E.IsReady() && Utilities.Enabled("e.combo") && target.IsValidTarget(E.Range))
                {
                    if (Utilities.Enabled("e."+target.ChampionName))
                    {
                        E.CastOnUnit(target);
                    }
                }

                if (R.IsReady() && Utilities.Enabled("r.combo"))
                {
                    if (target.IsValidTarget(R.Range) && R.GetDamage(target) > target.Health)
                    {
                        R.CastOnUnit(target);
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

            var target = TargetSelector.GetTarget(800, TargetSelector.DamageType.Physical);
            if (target != null)
            {
                if (Q.IsReady() && Utilities.Enabled("q.harass") && target.IsValidTarget(Q.Range))
                {
                    Q.Cast();
                }

                if (E.IsReady() && Utilities.Enabled("e.harass") && Utilities.Enabled("e." + target.ChampionName)
                    && target.IsValidTarget(E.Range))
                {
                    E.CastOnUnit(target);
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
                if (minionlist != null && minionlist.Count() >= Utilities.Slider("q.minion.count"))
                {
                    Q.Cast();
                }
            }

            if (E.IsReady() && Utilities.Enabled("e.laneclear"))
            {
                var minionlist = MinionManager.GetMinions(ObjectManager.Player.Position, E.Range);
                if (minionlist != null && minionlist.Count() >= Utilities.Slider("e.minion.count"))
                {
                    E.CastOnUnit(minionlist[0]);
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
                var target = MinionManager.GetMinions(ObjectManager.Player.AttackRange, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth)
                    .FirstOrDefault(x => x.IsValidTarget(Q.Range));

                if (target != null)
                {
                    Q.Cast();
                }
            }

            if (E.IsReady() && Utilities.Enabled("e.jungle"))
            {
                var target = MinionManager.GetMinions(ObjectManager.Player.AttackRange, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth)
                    .FirstOrDefault(x => x.IsValidTarget(E.Range));

                if (target != null)
                {
                    E.CastOnUnit(target);
                }
            }

        }
    }
}
