using System;
using System.Drawing;
using System.Linq;
using HikiCarry.Core.Plugins;
using HikiCarry.Core.Predictions;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Utilities = HikiCarry.Core.Utilities.Utilities;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HikiCarry.Champions
{
    internal class Ashe
    {
        internal static Spell Q;
        internal static Spell W;
        internal static Spell E;
        internal static Spell R;

        public Ashe()
        {
            
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 1200);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R, 2500);

            W.SetSkillshot(0.5f, 100, 902, true, SkillshotType.SkillshotCone);
            R.SetSkillshot(0.25f, 130f, 1600f, false, SkillshotType.SkillshotLine);

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
                harassMenu.AddItem(new MenuItem("w.harass", "Use (W)", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("harass.mana", "Harass Mana Percent", true).SetValue(new Slider(30, 1, 99)));
                Initializer.Config.AddSubMenu(harassMenu);
            }

            var clearmenu = new Menu("LaneClear Settings","LaneClear Settings");
            {
                clearmenu.AddItem(new MenuItem("w.laneclear", "Use (W)", true).SetValue(true));
                clearmenu.AddItem(new MenuItem("w.minion.count", "(W) Min. Minion Count", true).SetValue(new Slider(3, 1, 5)));
                clearmenu.AddItem(new MenuItem("clear.mana", "Clear Mana Percent", true).SetValue(new Slider(30, 1, 99)));
                Initializer.Config.AddSubMenu(clearmenu);
            }

            var junglemenu = new Menu("Jungle Settings", "Jungle Settings");
            {
                junglemenu.AddItem(new MenuItem("q.jungle", "Use (Q)", true).SetValue(true));
                junglemenu.AddItem(new MenuItem("w.jungle", "Use (W)", true).SetValue(true));
                junglemenu.AddItem(new MenuItem("jungle.mana", "Jungle Mana Percent", true).SetValue(new Slider(30, 1, 99)));
                Initializer.Config.AddSubMenu(junglemenu);
            }

            var miscMenu = new Menu("Misc Settings", "Misc Settings");
            {
                var dashinterrupter = new Menu("Dash Interrupter", "Dash Interrupter");
                {
                    dashinterrupter.AddItem(new MenuItem("dash.block", "Use (R) for Block Dash!", true).SetValue(true));
                    miscMenu.AddSubMenu(dashinterrupter);
                }
                Initializer.Config.AddSubMenu(miscMenu);
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

            Game.OnUpdate += AsheOnUpdate;
            Obj_AI_Base.OnSpellCast += AsheOnSpellCast;
            Obj_AI_Base.OnNewPath += ObjAiHeroOnOnNewPath;

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

        private void ObjAiHeroOnOnNewPath(Obj_AI_Base sender, GameObjectNewPathEventArgs args)
        {
            if (sender.IsEnemy && sender is AIHeroClient && args.IsDash && Utilities.Enabled("dash.block")
                && sender.IsValidTarget(1000) && R.IsReady())
            {
                var starttick = Utils.TickCount;
                var speed = args.Speed;
                var startpos = sender.ServerPosition.To2D();
                var path = args.Path;
                var forch = args.Path.OrderBy(x => starttick + (int)(1000 * (new Vector3(x.X, x.Y, x.Z).
                    Distance(startpos.To3D()) / speed))).FirstOrDefault();
                {
                    var endpos = new Vector3(forch.X, forch.Y, forch.Z);
                    var endtick = starttick + (int)(1000 * (endpos.Distance(startpos.To3D())
                        / speed));
                    var duration = endtick - starttick;

                    if (duration < starttick)
                    {
                        R.Cast(endpos);
                    }
                }
            }
        }

        private void AsheOnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.Target is AIHeroClient && args.SData.IsAutoAttack())
            {
                if (Q.IsReady() && Utilities.Enabled("q.combo") && ((AIHeroClient)args.Target).IsValidTarget(ObjectManager.Player.AttackRange) &&
                    sender.HasBuff("asheqcastready") && Initializer.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                {
                    Q.Cast();
                }

                if (Q.IsReady() && Utilities.Enabled("q.harass") && ((AIHeroClient)args.Target).IsValidTarget(ObjectManager.Player.AttackRange) &&
                    sender.HasBuff("asheqcastready") && Initializer.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                {
                    Q.Cast();
                }
            }
        }

        private void AsheOnUpdate(EventArgs args)
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
                if (W.IsReady() && Utilities.Enabled("w.combo") && target.IsValidTarget(W.Range))
                {
                    W.Do(target,Utilities.HikiChance("hitchance"));
                }

                if (R.IsReady() && Utilities.Enabled("r.combo"))
                {
                    if (target.IsValidTarget(R.Range) && R.GetDamage(target) > target.Health)
                    {
                        R.Do(target,Utilities.HikiChance("hitchance"));
                    }

                    if (target.IsValidTarget(R.Range / 6))
                    {
                        R.Do(target,Utilities.HikiChance("hitchance"));
                    }

                    if (target.IsValidTarget(ObjectManager.Player.AttackRange) && 
                        target.HasBuffOfType(BuffType.Slow))
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

            var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
            if (target != null)
            {
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

            if (W.IsReady() && Utilities.Enabled("w.laneclear"))
            {
                var minionlist = MinionManager.GetMinions(ObjectManager.Player.Position, W.Range);
                var whitlist = W.GetCircularFarmLocation(minionlist);
                if (whitlist.MinionsHit >= Utilities.Slider("w.minion.count"))
                {
                    W.Cast(whitlist.Position);
                }
            }
        }

        private static void OnJungle()
        {
            if (ObjectManager.Player.ManaPercent < Utilities.Slider("jungle.mana"))
            {
                return;
            }

            if (W.IsReady() && Utilities.Enabled("w.jungle"))
            {
                var target = MinionManager.GetMinions(ObjectManager.Player.AttackRange, MinionTypes.All,MinionTeam.Neutral, MinionOrderTypes.MaxHealth)
                    .FirstOrDefault(x => x.IsValidTarget(W.Range));

                if (target != null)
                {
                    W.Do(target,HitChance.High);
                }

            }
        }
    }
}
