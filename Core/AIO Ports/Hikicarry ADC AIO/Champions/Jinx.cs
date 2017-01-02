using System;
using System.Collections.Generic;
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
    internal class Jinx
    {
        internal static Spell Q;
        internal static Spell W;
        internal static Spell E;
        internal static Spell R;

        
        public static bool IsMinigun => !ObjectManager.Player.HasBuff("JinxQ");
        public static float MinigunRange => 575f;
        public static bool IsFishBone => ObjectManager.Player.HasBuff("JinxQ");
        public static float FishBoneRange => 75f + 25f * Q.Level;
        public static int FishBoneAoeRadius => 200;
        

        public Jinx()
        {
            Q = new Spell(SpellSlot.Q, ObjectManager.Player.AttackRange);
            W = new Spell(SpellSlot.W, 1490f);
            E = new Spell(SpellSlot.E, 900f);
            R = new Spell(SpellSlot.R, 2500f);

            W.SetSkillshot(0.6f, 75f, 3300f, true, SkillshotType.SkillshotLine);
            E.SetSkillshot(1.2f, 1f, 1750f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.7f, 140f, 1500f, false, SkillshotType.SkillshotLine);

            var comboMenu = new Menu("Combo Settings", "Combo Settings");
            {
                comboMenu.AddItem(new MenuItem("q.combo", "Use (Q)", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("w.combo", "Use (W)", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("e.combo", "Use (E)", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("r.combo", "Use (R)", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ult.distance", "Ult Max Distance", true).SetValue(new Slider(1500, 1, 2000)));
                Initializer.Config.AddSubMenu(comboMenu);
            }

            var harassMenu = new Menu("Harass Settings", "Harass Settings");
            {
                harassMenu.AddItem(new MenuItem("q.harass", "Use (Q)", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("w.harass", "Use (W)", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("harass.mana", "Harass Mana Percent", true).SetValue(new Slider(30, 1, 99)));
                Initializer.Config.AddSubMenu(harassMenu);
            }

            var clearmenu = new Menu("LaneClear Settings", "LaneClear Settings");
            {
                clearmenu.AddItem(new MenuItem("q.laneclear", "Use (Q)", true).SetValue(true));
                clearmenu.AddItem(new MenuItem("q.minion.count", "(Q) Min. Minion Count", true).SetValue(new Slider(3, 1, 5)));
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

            var miscMenu = new Menu("Misc Settings", "Misc Settings");
            {
                var dashinterrupter = new Menu("Dash Interrupter", "Dash Interrupter");
                {
                    dashinterrupter.AddItem(new MenuItem("dash.block", "Use (E) for Block Dash!", true).SetValue(true));
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

                //DamageIndicator.DamageToUnit = TotalDamage;
                //DamageIndicator.Enabled = drawDamageMenu.GetValue<bool>();
                //DamageIndicator.Fill = drawFill.GetValue<Circle>().Active;
                //DamageIndicator.FillColor = drawFill.GetValue<Circle>().Color;

                drawDamageMenu.ValueChanged +=
                delegate (object sender, OnValueChangeEventArgs eventArgs)
                {
                    //DamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
                };

                drawFill.ValueChanged +=
                delegate (object sender, OnValueChangeEventArgs eventArgs)
                {
                    //DamageIndicator.Fill = eventArgs.GetNewValue<Circle>().Active;
                    //DamageIndicator.FillColor = eventArgs.GetNewValue<Circle>().Color;
                };
                Initializer.Config.AddSubMenu(drawMenu);
            }

            Game.OnUpdate += JinxOnUpdate;
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
                && sender.IsValidTarget(W.Range) && W.IsReady())
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
                        W.Cast(endpos);
                    }
                }
            }
        }
        private void JinxOnUpdate(EventArgs args)
        {
            switch (Initializer.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    OnCombo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    OnHarass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    OnClear();
                    break;
                    case Orbwalking.OrbwalkingMode.LastHit:
                    OnLastHit();
                    break;
            }

            ImmobileTarget();
        }

        private void OnLastHit()
        {
            if (Q.IsReady())
            {
                if (IsFishBone)
                {
                    Q.Cast();
                }
            }
        }

        private void OnCombo()
        {
            var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);
            if (target != null)
            {
                if (Q.IsReady() && Utilities.Enabled("q.combo"))
                {
                    if (IsMinigun && target.Distance(ObjectManager.Player.Position) >= MinigunRange)
                    {
                        Q.Cast();
                    }
                    if (IsFishBone && target.Distance(ObjectManager.Player.Position) <= MinigunRange)
                    {
                        Q.Cast();
                    }
                    if (IsFishBone && target.Distance(ObjectManager.Player.Position) > FishBoneRange)
                    {
                        Q.Cast();
                    }
                }

                if (W.IsReady() && Utilities.Enabled("w.combo"))
                {
                    W.Do(target,Utilities.HikiChance("hitchance"));
                }

                if (R.IsReady() && Utilities.Enabled("r.combo") && 
                    target.IsValidTarget(Utilities.Slider("ult.distance")) && target.Health < R.GetDamage(target))
                {
                    R.Do(target,Utilities.HikiChance("hitchance"));
                }
            }
            
        }
        private void OnHarass()
        {
            if (ObjectManager.Player.ManaPercent < Utilities.Slider("harass.mana"))
            {
                return;
            }

            var target = TargetSelector.GetTarget(W.Range,TargetSelector.DamageType.Physical);
            if (target != null)
            {
                
                if (Q.IsReady() && Utilities.Enabled("q.harass"))
                {
                    if (IsMinigun && target.Distance(ObjectManager.Player.Position) >= MinigunRange)
                    {
                        Q.Cast();
                    }
                    if (IsFishBone && target.Distance(ObjectManager.Player.Position) <= MinigunRange)
                    {
                        Q.Cast();
                    }
                    if (IsFishBone && target.Distance(ObjectManager.Player.Position) > FishBoneRange)
                    {
                        Q.Cast();
                    }
                }

                if (W.IsReady() && target.IsValidTarget(W.Range) && Utilities.Enabled("w.harass"))
                {
                    W.Do(target,Utilities.HikiChance("hitchance"));
                }
            }
        }
        private void OnClear()
        {
            if (ObjectManager.Player.ManaPercent < Utilities.Slider("clear.mana"))
            {
                return;
            }

            if (Q.IsReady() && Utilities.Enabled("q.laneclear"))
            {
                var minion = MinionManager.GetMinions(Q.Range);
                if (minion != null && minion.Count() >= Utilities.Slider("q.minion.count"))
                {
                    if (IsMinigun && minion[0].Distance(ObjectManager.Player.Position) >= MinigunRange)
                    {
                        Q.Cast();
                    }
                    if (IsFishBone && minion[0].Distance(ObjectManager.Player.Position) <= MinigunRange)
                    {
                        Q.Cast();
                    }
                }
            }
        }

        private static void ImmobileTarget()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
            if (target != null)
            {
                if (E.IsReady() && Utilities.Enabled("e.combo") && (target.HasBuffOfType(BuffType.Slow)
                                                                    || target.HasBuffOfType(BuffType.Charm) ||
                                                                    target.HasBuffOfType(BuffType.Fear) ||
                                                                    target.HasBuffOfType(BuffType.Stun) ||
                                                                    target.HasBuffOfType(BuffType.Taunt)
                                                                    || target.HasBuffOfType(BuffType.Snare)))
                {
                    E.Cast(target.Position);
                }
               
            }
        }
    }
}
