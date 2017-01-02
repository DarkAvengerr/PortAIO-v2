using System;
using System.Drawing;
using System.Linq;
using HikiCarry.Core.Plugins;
using HikiCarry.Core.Predictions;
using HikiCarry.Core.Utilities;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Utilities = HikiCarry.Core.Utilities.Utilities;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HikiCarry.Champions
{
    internal class Jhin
    {
        internal static Spell Q;
        internal static Spell W;
        internal static Spell E;
        internal static Spell R;

        public Jhin()
        {
            Q = new Spell(SpellSlot.Q, 550);
            W = new Spell(SpellSlot.W, 2500);
            E = new Spell(SpellSlot.E, 2000);
            R = new Spell(SpellSlot.R, 3500);

            W.SetSkillshot(0.75f, 40, float.MaxValue, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.23f, 120, 1600, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.21f, 80, 5000, false, SkillshotType.SkillshotLine);

            var comboMenu = new Menu("Combo Settings", ":: Combo Settings");
            {

                comboMenu.AddItem(new MenuItem("q.combo", "Use (Q)", true).SetValue(true));

                comboMenu.AddItem(new MenuItem("w.combo", "Use (W)", true).SetValue(true));
                comboMenu.AddItem(
                    new MenuItem("w.combo.min.distance", "Min. Distance", true).SetValue(new Slider(400, 1, 2500)));
                comboMenu.AddItem(
                    new MenuItem("w.combo.max.distance", "Max. Distance", true).SetValue(new Slider(1000, 1, 2500)));
                comboMenu.AddItem(new MenuItem("w.passive.combo", "Use (W) If Enemy Is Marked", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("e.combo", "Use (E)", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("e.to.dash.end", "Use (E) to Dash End", true).SetValue(true));
                Initializer.Config.AddSubMenu(comboMenu);
            }

            var harassMenu = new Menu("Harass Settings", ":: Harass Settings");
            {
                harassMenu.AddItem(new MenuItem("q.harass", "Use (Q)", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("w.harass", "Use (W)",true).SetValue(true));
                harassMenu.AddItem(
                    new MenuItem("harass.mana", "Min. Mana Percentage",true).SetValue(new Slider(50, 1, 99)));
                Initializer.Config.AddSubMenu(harassMenu);
            }

            var clearMenu = new Menu("Clear Settings", ":: Clear Settings");
            {
                var laneclearMenu = new Menu("Wave Clear", ":: Wave Clear");
                {
                    laneclearMenu.AddItem(
                        new MenuItem("keysinfo1", "                  (Q) Settings").SetTooltip("Q Settings"));
                    laneclearMenu.AddItem(new MenuItem("q.clear", "Use (Q)", true).SetValue(true));
                    laneclearMenu.AddItem(
                        new MenuItem("keysinfo2", "                  (W) Settings").SetTooltip("W Settings"));
                    laneclearMenu.AddItem(new MenuItem("w.clear", "Use (W)", true).SetValue(true));
                    laneclearMenu.AddItem(new MenuItem("w.hit.x.minion", "Min. Minion", true).SetValue(new Slider(4, 1, 5)));
                    clearMenu.AddSubMenu(laneclearMenu);
                }


                var jungleClear = new Menu("Jungle Clear", ":: Jungle Clear");
                {
                    jungleClear.AddItem(
                            new MenuItem("keysinfo1X", "                  (Q) Settings").SetTooltip("Q Settings"));
                    jungleClear.AddItem(new MenuItem("q.jungle", "Use (Q)", true).SetValue(true));
                    jungleClear.AddItem(
                        new MenuItem("keysinfo2X", "                  (W) Settings").SetTooltip("W Settings"));
                    jungleClear.AddItem(new MenuItem("w.jungle", "Use (W)", true).SetValue(true));
                    clearMenu.AddSubMenu(jungleClear);
                }

                clearMenu.AddItem(
                        new MenuItem("clear.mana", "LaneClear Min. Mana Percentage", true).SetValue(new Slider(50, 1, 99)));
                clearMenu.AddItem(
                    new MenuItem("jungle.mana", "Jungle Min. Mana Percentage", true).SetValue(new Slider(50, 1, 99)));
                Initializer.Config.AddSubMenu(clearMenu);

            }

            var ksMenu = new Menu("Kill Steal", ":: Kill Steal");
            {
                ksMenu.AddItem(new MenuItem("q.ks", "Use (Q)", true).SetValue(true));
                ksMenu.AddItem(new MenuItem("w.ks", "Use (W)", true).SetValue(true));
                Initializer.Config.AddSubMenu(ksMenu);
            }

            var miscMenu = new Menu("Miscellaneous", ":: Miscellaneous");
            {
                miscMenu.AddItem(new MenuItem("auto.e.immobile", "Auto Cast (E) Immobile Target", true).SetValue(true));
                Initializer.Config.AddSubMenu(miscMenu);
            }



            var rComboMenu = new Menu("Ultimate Settings", ":: Ultimate Settings").SetFontStyle(FontStyle.Bold,
                    SharpDX.Color.Yellow);
            {
                var rComboWhiteMenu = new Menu(":: R - Whitelist", ":: R - Whitelist");
                {
                    foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValid))
                    {
                        rComboWhiteMenu.AddItem(
                            new MenuItem("r.combo." + enemy.ChampionName, "(R): " + enemy.ChampionName, true).SetValue(
                                true));
                    }
                    rComboMenu.AddSubMenu(rComboWhiteMenu);
                }
                rComboMenu.AddItem(new MenuItem("r.combo", "Use (R)", true).SetValue(true));
                rComboMenu.AddItem(
                    new MenuItem("auto.shoot.bullets", "If Jhin Casting (R) Auto Cast Bullets", true).SetValue(true));
                Initializer.Config.AddSubMenu(rComboMenu);
            }

            Initializer.Config.AddItem(
                    new MenuItem("semi.manual.ult", "Semi-Manual (R)!", true).SetValue(new KeyBind("A".ToCharArray()[0],
                        KeyBindType.Press)));
            Initializer.Config.AddItem(new MenuItem("use.combo", "Combo (Active)", true).SetValue(new KeyBind(32, KeyBindType.Press)));

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

            Game.OnUpdate += JhinOnUpdate;
            Obj_AI_Base.OnNewPath += ObjAiHeroOnOnNewPath;

        }

        private void ObjAiHeroOnOnNewPath(Obj_AI_Base sender, GameObjectNewPathEventArgs args)
        {
            if (sender.IsEnemy && sender is AIHeroClient && args.IsDash && Utilities.Enabled("e.to.dash.end")
                && sender.IsValidTarget(E.Range) && E.IsReady())
            {
                var starttick = Utils.TickCount;
                var speed = args.Speed;
                var startpos = sender.ServerPosition.To2D();
                var forch = args.Path.OrderBy(x => starttick + (int)(1000 * (new Vector3(x.X, x.Y, x.Z).
                    Distance(startpos.To3D()) / speed))).FirstOrDefault();
                {
                    var endpos = new Vector3(forch.X, forch.Y, forch.Z);
                    var endtick = starttick + (int)(1000 * (endpos.Distance(startpos.To3D())
                        / speed));
                    var duration = endtick - starttick;

                    if (duration < starttick)
                    {
                        E.Cast(endpos);
                    }
                }
            }
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
        private void JhinOnUpdate(EventArgs args)
        {
            #region Orbwalker & Modes 

            switch (Initializer.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    OnCombo();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    OnClear();
                    OnJungle();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    OnHarass();
                    break;
                case Orbwalking.OrbwalkingMode.None:
                    OnKillSteal();
                    break;
            }

            #endregion

            if (ObjectManager.Player.IsActive(R))
            {
                Initializer.Orbwalker.SetAttack(false);
                Initializer.Orbwalker.SetMovement(false);
            }
            else
            {
                Initializer.Orbwalker.SetAttack(true);
                Initializer.Orbwalker.SetMovement(true);
            }

            if (Initializer.Config.Item("semi.manual.ult", true).GetValue<KeyBind>().Active &&
                R.IsReady() && Utilities.Enabled("r.combo") && !Utilities.Enabled("auto.shoot.bullets"))
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range) &&
                                                                      Utilities.Enabled("r.combo." + x.ChampionName)))
                {
                    R.Do(target, Utilities.HikiChance("hitchance"));
                }
            }

            if (ObjectManager.Player.IsActive(R) && Utilities.Enabled("auto.shoot.bullets") && R.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range) &&
                                                                      Utilities.Enabled("r.combo." + x.ChampionName)))
                {
                    R.Do(target, Utilities.HikiChance("hitchance"));
                }
            }
        }

        private void OnKillSteal()
        {
            if (Q.IsReady() && Utilities.Enabled("q.ks"))
            {
                var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
                if (target != null && target.IsValidTarget(Q.Range) && target.Health < Q.GetDamage(target))
                {
                    Q.CastOnUnit(target);
                }
            }
            if (W.IsReady() && Utilities.Enabled("w.ks"))
            {
                var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
                if (target != null && target.IsValidTarget(W.Range) && target.Health < W.GetDamage(target))
                {
                    W.Do(target,Utilities.HikiChance("hitchance"));
                }
            }
        }
        private void OnCombo()
        {
            if (Q.IsReady() && Utilities.Enabled("q.combo"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x=> x.IsValidTarget(Q.Range)))
                {
                    Q.CastOnUnit(enemy);
                }
            }
            if (W.IsReady() && Utilities.Enabled("w.combo"))
            {
                if (Utilities.Enabled("w.passive.combo"))
                {
                    var target = TargetSelector.GetTarget(Utilities.Slider("w.combo.max.distance"), TargetSelector.DamageType.Physical);
                    if (target != null && target.IsValidTarget(Utilities.Slider("w.combo.max.distance")) && 
                        target.HasBuff("jhinespotteddebuff") && target.Distance(ObjectManager.Player.Position) > Utilities.Slider("w.combo.min.distance"))
                    {
                        W.Do(target,Utilities.HikiChance("hitchance"));
                    }
                }
                else
                {
                    var target = TargetSelector.GetTarget(Utilities.Slider("w.combo.max.distance"), TargetSelector.DamageType.Physical);
                    if (target != null && target.IsValidTarget(Utilities.Slider("w.combo.max.distance")) 
                        && target.Distance(ObjectManager.Player.Position) > Utilities.Slider("w.combo.min.distance"))
                    {
                        W.Do(target, Utilities.HikiChance("hitchance"));
                    }
                }
            }

            if (E.IsReady() && Utilities.Enabled("e.combo"))
            {
                var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
                if (target != null && target.IsValidTarget(E.Range) && Utilities.IsImmobile(target))
                {
                    E.Do(target,Utilities.HikiChance("hitchance"));
                }
            }

        }
        private void OnHarass()
        {
            if (ObjectManager.Player.ManaPercent < Utilities.Slider("harass.mana"))
            {
                return;
            }
            if (Q.IsReady() && Utilities.Enabled("q.harass"))
            {
                var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
                if (target != null && target.IsValidTarget(Q.Range))
                {
                    Q.CastOnUnit(target);
                }
            }
            if (W.IsReady() && Utilities.Enabled("w.harass"))
            {
                var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
                if (target != null && target.IsValidTarget(W.Range))
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
            if (Q.IsReady() && Utilities.Enabled("q.clear"))
            {
                var min = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range).MinOrDefault(x => x.Health);
                if (min != null)
                {
                    Q.CastOnUnit(min);
                }
            }

            if (W.IsReady() && Utilities.Enabled("w.clear"))
            {
                var min = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range);
                if (min != null && W.GetLineFarmLocation(min).MinionsHit >= Utilities.Slider("w.hit.x.minion"))
                {
                    W.Cast(W.GetLineFarmLocation(min).Position);
                }
            }
        }
        private void OnJungle()
        {
            if (ObjectManager.Player.ManaPercent < Utilities.Slider("jungle.mana"))
            {
                return;
            }
            var mobs = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Jhin.Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (mobs == null || (mobs.Count == 0))
            {
                return;
            }

            if (Q.IsReady() && Utilities.Enabled("q.jungle"))
            {
                Q.Cast(mobs[0]);
            }

            if (W.IsReady() && Utilities.Enabled("w.jungle"))
            {
                W.Cast(mobs[0]);
            }
        }

    }
}
