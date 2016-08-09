using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; namespace ADCPackage.Plugins
{
    static class Corki
    {
        private static Spell Q, W, E, R, R2;

        private static AIHeroClient Player => ObjectManager.Player;

        public static void Load()
        {
            Chat.Print("[<font color='#F8F46D'>ADC Package</font>] by <font color='#79BAEC'>God</font> - <font color='#FFFFFF'>Corki</font> loaded (incomplete)");
            Game.OnUpdate += PermaActive;

            InitSpells();
            InitMenu();
        }

        private static void PermaActive(EventArgs args)
        {
            if (Menu.Config.Item("ks.q").IsActive())
            {
                foreach (var enemy in HeroManager.Enemies.Where(e => Q.CanCast(e) && Q.IsKillable(e)))
                {
                    Q.Cast(enemy);
                    return;
                }
            }

            if (Menu.Config.Item("ks.r").IsActive())
            {
                var r2 = Player.HasBuff("corkimissilebarragecounterbig");

                if (r2)
                {
                    foreach (var enemy in HeroManager.Enemies.Where(e => R.CanCast(e) && R.IsKillable(e)))
                    {
                        R.Cast(enemy);
                        return;
                    }
                }
                else
                {
                    foreach (var enemy in HeroManager.Enemies.Where(e => R2.CanCast(e) && R2.IsKillable(e)))
                    {
                        R2.Cast(enemy);
                        return;
                    }
                }
            }
        }

        private static void InitSpells()
        {
            Q = new Spell(SpellSlot.Q, 825);
            W = new Spell(SpellSlot.W, 800);
            E = new Spell(SpellSlot.E, 600);
            R = new Spell(SpellSlot.R, 1300);
            R2 = new Spell(SpellSlot.R, 1500);

            Q.SetSkillshot(.50f, 250f, 1075, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(.20f, 40f, 2000f, true, SkillshotType.SkillshotLine);
            R2.SetSkillshot(.20f, 40f, 2000f, true, SkillshotType.SkillshotLine);

        }

        public static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);

            if (target != null)
            {
                if (Menu.Config.Item("q.combo").IsActive())
                {
                    if (Q.CanCast(target))
                    {
                        if (target.IsMoving && target.GetWaypoints().Count >= 2)
                        {
                            //var qpred = Q.GetPrediction(target);
                            var x = target.Position.Extend(Prediction.GetPrediction(target, Q.Delay).UnitPosition, 300);

                            if (Q.IsInRange(x))
                            {
                                Q.Cast(x);
                                return;
                            }
                        }

                        var qpred = Q.GetPrediction(target);
                        if (qpred.Hitchance >= HitChance.Dashing)
                        {
                            Q.Cast(qpred.CastPosition);
                        }
                    }
                }

                if (Menu.Config.Item("e.combo").IsActive())
                {
                    if (E.IsReady() && Player.IsFacing(target))
                    {
                        E.Cast(target);
                    }
                }
            }

            if (Menu.Config.Item("r.combo").IsActive() && Menu.Config.Item("r.combo.stacks").GetValue<Slider>().Value < R.Instance.Ammo)
            {
                if (R.IsReady())
                {
                    var r2 = Player.HasBuff("corkimissilebarragecounterbig");
                    if (!r2)
                    {
                        var rtarget = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
                        if (rtarget == null) return;
                        var pred = R.GetPrediction(rtarget);
                        if (R.CanCast(rtarget))
                        {
                            switch (pred.Hitchance)
                            {
                                case HitChance.High:
                                case HitChance.VeryHigh:
                                case HitChance.Immobile:
                                    R.Cast(pred.CastPosition);
                                    break;
                                case HitChance.Collision:
                                    var colliding =
                                        pred.CollisionObjects.OrderBy(o => o.Distance(Player.ServerPosition)).ToList();
                                    if (colliding[0].Distance(rtarget.ServerPosition) <= 200 + colliding[0].BoundingRadius)
                                    {
                                        R.Cast(pred.CastPosition);
                                    }
                                    break;
                            }
                        }
                    }
                    else
                    {
                        var rtarget = TargetSelector.GetTarget(R2.Range, TargetSelector.DamageType.Magical);
                        if (rtarget == null) return;
                        var pred = R2.GetPrediction(rtarget);
                        if (R2.CanCast(rtarget))
                        {
                            switch (pred.Hitchance)
                            {
                                case HitChance.High:
                                case HitChance.VeryHigh:
                                case HitChance.Immobile:
                                    R2.Cast(pred.CastPosition);
                                    break;

                                case HitChance.Collision:
                                    var colliding =
                                        pred.CollisionObjects.OrderBy(o => o.Distance(Player.ServerPosition)).ToList();
                                    if (colliding[0].Distance(rtarget.ServerPosition) <= 200 + colliding[0].BoundingRadius)
                                    {
                                        R.Cast(pred.CastPosition);
                                    }
                                    break;
                            }
                        }
                    }
                }
            }


        }

        public static void Harass()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);

            if (target != null)
            {
                if (Menu.Config.Item("q.harass").IsActive())
                {
                    if (Q.CanCast(target))
                    {
                        if (target.IsMoving)
                        {
                            //var qpred = Q.GetPrediction(target);
                            var x = target.Position.Extend(Prediction.GetPrediction(target, Q.Delay).UnitPosition, 300);

                            if (Q.IsInRange(x))
                            {
                                Q.Cast(x);
                                return;
                            }
                        }

                        var qpred = Q.GetPrediction(target);
                        if (qpred.Hitchance >= HitChance.Dashing)
                        {
                            Q.Cast(qpred.CastPosition);
                        }
                    }
                }

                if (Menu.Config.Item("e.harass").IsActive())
                {
                    if (E.IsReady() && Player.IsFacing(target))
                    {
                        E.Cast(target);
                    }
                }
            }

            if (Menu.Config.Item("r.harass").IsActive() && Menu.Config.Item("r.harass.stacks").GetValue<Slider>().Value < R.Instance.Ammo)
            {
                if (R.IsReady())
                {
                    var r2 = Player.HasBuff("corkimissilebarragecounterbig");
                    if (!r2)
                    {
                        var rtarget = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
                        if (rtarget == null) return;
                        var pred = R.GetPrediction(rtarget);
                        if (R.CanCast(rtarget))
                        {
                            switch (pred.Hitchance)
                            {
                                case HitChance.High:
                                case HitChance.VeryHigh:
                                case HitChance.Immobile:
                                    R.Cast(pred.CastPosition);
                                    break;
                                case HitChance.Collision:
                                    var colliding =
                                        pred.CollisionObjects.OrderBy(o => o.Distance(Player.ServerPosition)).ToList();
                                    if (colliding[0].Distance(rtarget.ServerPosition) <= 200 + colliding[0].BoundingRadius)
                                    {
                                        R.Cast(pred.CastPosition);
                                    }
                                    break;
                            }
                        }
                    }
                    else
                    {
                        var rtarget = TargetSelector.GetTarget(R2.Range, TargetSelector.DamageType.Magical);
                        if (rtarget == null) return;
                        var pred = R2.GetPrediction(rtarget);
                        if (R2.CanCast(rtarget))
                        {
                            switch (pred.Hitchance)
                            {
                                case HitChance.High:
                                case HitChance.VeryHigh:
                                case HitChance.Immobile:
                                    R2.Cast(pred.CastPosition);
                                    break;

                                case HitChance.Collision:
                                    var colliding =
                                        pred.CollisionObjects.OrderBy(o => o.Distance(Player.ServerPosition)).ToList();
                                    if (colliding[0].Distance(rtarget.ServerPosition) <= 200 + colliding[0].BoundingRadius)
                                    {
                                        R2.Cast(pred.CastPosition);
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }

        public static void LaneClear()
        {
            if (Q.IsReady() && Menu.Config.Item("q.laneclear").IsActive())
            {
                var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range);
                var qfarm = Q.GetCircularFarmLocation(minions);

                if (qfarm.MinionsHit >= Menu.Config.Item("q.laneclear.minions").GetValue<Slider>().Value)
                {
                    Q.Cast(qfarm.Position);
                }
            }

            if (E.IsReady() && Menu.Config.Item("e.laneclear").IsActive())
            {
                var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range);

                if (minions.Count >= Menu.Config.Item("e.laneclear.minions").GetValue<Slider>().Value)
                {
                    if (Player.IsFacing(minions.FirstOrDefault()))
                        E.Cast(Game.CursorPos);
                }
            }

            if (R.IsReady() && Menu.Config.Item("r.laneclear").IsActive() && Menu.Config.Item("r.laneclear.stacks").GetValue<Slider>().Value < R.Instance.Ammo)
            {
                var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, R.Range);
                var rfarm = R.GetCircularFarmLocation(minions, 150);

                if (rfarm.MinionsHit >= 2)
                {
                    R.Cast(rfarm.Position);
                }
            }
        }

        private static void InitMenu()
        {
            Menu.Config.AddSubMenu(new LeagueSharp.Common.Menu("Corki", "adcpackage.corki"));

            var comboMenu =
                Menu.Config.SubMenu("adcpackage.corki")
                    .AddSubMenu(new LeagueSharp.Common.Menu("Combo Menu", "combo"));
            {
                comboMenu.Color = SharpDX.Color.MediumVioletRed;
            }

            comboMenu.AddItem(new MenuItem("combo.settings", "Combo Settings"))
                .SetFontStyle(FontStyle.Regular, SharpDX.Color.Yellow);
            comboMenu.AddItem(new MenuItem("q.combo", "Use Q").SetValue(true));
            comboMenu.AddItem(new MenuItem("e.combo", "Use E").SetValue(true));
            comboMenu.AddItem(new MenuItem("r.combo", "Use R").SetValue(true));
            comboMenu.AddItem(new MenuItem("r.combo.stacks", "   Save X missiles:").SetValue(new Slider(0, 0, 7)));

            /* TODO: HARASS */

            var harassMenu =
                Menu.Config.SubMenu("adcpackage.corki")
                    .AddSubMenu(new LeagueSharp.Common.Menu("Harass Menu", "harass"));
            {
                harassMenu.Color = SharpDX.Color.MediumVioletRed;
            }

            harassMenu.AddItem(new MenuItem("harass.settings", "Harass Settings"))
                .SetFontStyle(FontStyle.Regular, SharpDX.Color.Yellow);
            harassMenu.AddItem(new MenuItem("q.harass", "Use Q").SetValue(true));
            harassMenu.AddItem(new MenuItem("e.harass", "Use E").SetValue(true));
            harassMenu.AddItem(new MenuItem("r.harass", "Use R").SetValue(true));
            harassMenu.AddItem(new MenuItem("r.harass.stacks", "   Save X missiles:").SetValue(new Slider(2, 0, 7)));

            /* TODO: LANECLEAR */

            var laneclearMenu =
                Menu.Config.SubMenu("adcpackage.corki")
                    .AddSubMenu(new LeagueSharp.Common.Menu("Laneclear Menu", "laneclear"));
            {
                laneclearMenu.Color = SharpDX.Color.LightGoldenrodYellow;
            }


            laneclearMenu.AddItem(new MenuItem("laneclearMenu.settings", "Laneclear Settings"))
                .SetFontStyle(FontStyle.Regular, SharpDX.Color.Yellow);
            laneclearMenu.AddItem(new MenuItem("q.laneclear", "Use Q").SetValue(true));
            laneclearMenu.AddItem(new MenuItem("q.laneclear.minions", "   Hit X minions:").SetValue(new Slider(3, 0, 7)));
            laneclearMenu.AddItem(new MenuItem("e.laneclear", "Use E").SetValue(true));
            laneclearMenu.AddItem(new MenuItem("e.laneclear.minions", "   X minions in range:").SetValue(new Slider(3, 0, 7)));
            laneclearMenu.AddItem(new MenuItem("r.laneclear", "Use R").SetValue(true));
            laneclearMenu.AddItem(new MenuItem("r.laneclear.stacks", "   Save X missiles:").SetValue(new Slider(2, 0, 7)));

            /* TODO: EXTRAS */

            var extrasMenu =
                Menu.Config.SubMenu("adcpackage.corki")
                    .AddSubMenu(new LeagueSharp.Common.Menu("Extras Menu", "extras"));
            {
                extrasMenu.Color = SharpDX.Color.Aquamarine;
            }
            extrasMenu.AddItem(new MenuItem("ks.settings", "Killsteal settings")).SetFontStyle(FontStyle.Regular, SharpDX.Color.Yellow);
            extrasMenu.AddItem(new MenuItem("ks.q", "   Killsteal Q").SetValue(true));
            extrasMenu.AddItem(new MenuItem("ks.r", "   Killsteal R").SetValue(true));
            extrasMenu.AddItem(new MenuItem("auto.settings", "Auto spell settings")).SetFontStyle(FontStyle.Regular, SharpDX.Color.Yellow);
            extrasMenu.AddItem(new MenuItem("immobile.q", "   Q on CC'd - soon (lazy kek)").SetValue(false));
            extrasMenu.AddItem(new MenuItem("immobile.r", "   R on CC'd - soon (lazy kek)").SetValue(false));

        }
    }
}