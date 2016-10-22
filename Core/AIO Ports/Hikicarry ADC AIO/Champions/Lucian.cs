using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using HikiCarry.Core.Plugins;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Utilities = HikiCarry.Core.Utilities.Utilities;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HikiCarry.Champions
{
    class Lucian
    {
        internal static Spell Q;
        internal static Spell Q2;
        internal static Spell W;
        internal static Spell E;
        internal static Spell R;
        public static int ERange => Utilities.Slider("lucian.e.range");

        public Lucian()
        {
            Q = new Spell(SpellSlot.Q, 675);
            Q2 = new Spell(SpellSlot.Q, 900);
            W = new Spell(SpellSlot.W, 1000);
            E = new Spell(SpellSlot.E, 475);
            R = new Spell(SpellSlot.R, 1400);

            Q.SetTargetted(0.25f, float.MaxValue);
            Q2.SetSkillshot(0.55f, 50f, float.MaxValue, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.4f, 150f, 1600, true, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.25f, 1f, float.MaxValue, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.2f, 110f, 2500, true, SkillshotType.SkillshotLine);

            var comboMenu = new Menu(":: Combo Settings", ":: Combo Settings");
            {
                comboMenu.AddItem(new MenuItem("lucian.q.combo", "Use Q", true).SetValue(true)).SetTooltip("Uses Q in Combo", SharpDX.Color.GreenYellow);
                comboMenu.AddItem(new MenuItem("lucian.e.combo", "Use E", true).SetValue(true)).SetTooltip("Uses E in Combo", SharpDX.Color.GreenYellow);
                comboMenu.AddItem(new MenuItem("lucian.e.mode", "E Type", true).SetValue(new StringList(new[] { "Safe", "Cursor Position" })));
                comboMenu.AddItem(new MenuItem("lucian.w.combo", "Use W", true).SetValue(true)).SetTooltip("Uses W in Combo", SharpDX.Color.GreenYellow);
                comboMenu.AddItem(new MenuItem("lucian.disable.w.prediction, true", "Disable W Prediction").SetValue(true)).SetTooltip("10/10 for speed combo!", SharpDX.Color.GreenYellow);
                comboMenu.AddItem(new MenuItem("lucian.r.combo", "Use R", true).SetValue(true)).SetTooltip("Uses R in Combo (Only Casting If Enemy Killable)", SharpDX.Color.GreenYellow);
                comboMenu.AddItem(new MenuItem("lucian.combo.start.e", "Start Combo With E", true).SetValue(true)).SetTooltip("Starting Combo With E", SharpDX.Color.GreenYellow);
                comboMenu.AddItem(new MenuItem("lucian.e.range", "(E) Range").SetValue(new Slider(475,1,475))).SetTooltip("If you wanna do short dash just set that slider to 1");
                Initializer.Config.AddSubMenu(comboMenu);
            }

            var harassMenu = new Menu(":: Harass Settings", ":: Harass Settings");
            {
                harassMenu.AddItem(new MenuItem("lucian.q.harass", "Use Q", true).SetValue(true)).SetTooltip("Uses Q in Harass", SharpDX.Color.GreenYellow);
                harassMenu.AddItem(new MenuItem("lucian.q.type", "Harass Type", true).SetValue(new StringList(new[] { "Extended", "Normal" })));
                harassMenu.AddItem(new MenuItem("lucian.w.harass", "Use W", true).SetValue(true)).SetTooltip("Uses W in Harass", SharpDX.Color.GreenYellow);
                harassMenu.AddItem(new MenuItem("lucian.harass.mana", "Min. Mana", true).SetValue(new Slider(50, 1, 99))).SetTooltip("Manage your Mana!", SharpDX.Color.GreenYellow);
                var qToggleMenu = new Menu(":: Q Whitelist (Extended)", ":: Q Whitelist (Extended)");
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValid))
                    {
                        qToggleMenu.AddItem(new MenuItem("lucian.white" + enemy.ChampionName, "(Q) " + enemy.ChampionName, true).SetValue(true));
                    }
                    harassMenu.AddSubMenu(qToggleMenu);
                }

                Initializer.Config.AddSubMenu(harassMenu);
            }

            var clearMenu = new Menu(":: Clear Settings", ":: Clear Settings");
            {
                clearMenu.AddItem(new MenuItem("lucian.q.clear", "Use Q", true).SetValue(true)).SetTooltip("Uses Q in Clear", SharpDX.Color.GreenYellow);
                clearMenu.AddItem(new MenuItem("lucian.q.harass.in.laneclear", "Use Extended (Q) Harass Enemy", true).SetValue(true)).SetTooltip("Uses Q in Clear", SharpDX.Color.GreenYellow);
                clearMenu.AddItem(new MenuItem("lucian.w.clear", "Use W", true).SetValue(true)).SetTooltip("Uses W in Clear", SharpDX.Color.GreenYellow);
                clearMenu.AddItem(new MenuItem("lucian.q.minion.hit.count", "(Q) Min. Minion Hit", true).SetValue(new Slider(3, 1, 5))).SetTooltip("Minimum minion count for Q", SharpDX.Color.GreenYellow);
                clearMenu.AddItem(new MenuItem("lucian.w.minion.hit.count", "(W) Min. Minion Hit", true).SetValue(new Slider(3, 1, 5))).SetTooltip("Minimum minion count for W", SharpDX.Color.GreenYellow);
                clearMenu.AddItem(new MenuItem("lucian.clear.mana", "Min. Mana", true).SetValue(new Slider(50, 1, 99))).SetTooltip("Manage your Mana!", SharpDX.Color.GreenYellow);
                Initializer.Config.AddSubMenu(clearMenu);
            }

            var jungleMenu = new Menu(":: Jungle Settings", ":: Jungle Settings");
            {
                jungleMenu.AddItem(new MenuItem("lucian.q.jungle", "Use Q", true).SetValue(true)).SetTooltip("Uses Q in Jungle", SharpDX.Color.GreenYellow);
                jungleMenu.AddItem(new MenuItem("lucian.w.jungle", "Use W", true).SetValue(true)).SetTooltip("Uses W in Jungle", SharpDX.Color.GreenYellow);
                jungleMenu.AddItem(new MenuItem("lucian.e.jungle", "Use E", true).SetValue(true)).SetTooltip("Uses E in Jungle (Using Mouse Position)", SharpDX.Color.GreenYellow);
                jungleMenu.AddItem(new MenuItem("lucian.jungle.mana", "Min. Mana", true).SetValue(new Slider(50, 1, 99))).SetTooltip("Manage your Mana!", SharpDX.Color.GreenYellow);
                Initializer.Config.AddSubMenu(jungleMenu);
            }

            var killStealMenu = new Menu(":: KillSteal Settings", ":: KillSteal Settings");
            {
                killStealMenu.AddItem(new MenuItem("lucian.q.ks", "Use Q", true).SetValue(true)).SetTooltip("Uses Q if Enemy Killable", SharpDX.Color.GreenYellow);
                killStealMenu.AddItem(new MenuItem("lucian.w.ks", "Use W", true).SetValue(true)).SetTooltip("Uses W if Enemy Killable", SharpDX.Color.GreenYellow);
                Initializer.Config.AddSubMenu(killStealMenu);
            }

            var eqMenu = new Menu(":: E+Q KS Settings", ":: E+Q KS Settings").SetFontStyle(FontStyle.Bold,SharpDX.Color.Crimson);
            {
                eqMenu.AddItem(new MenuItem("use.eq", "Use E+Q", true).SetValue(true));
                eqMenu.AddItem(new MenuItem("eq.safety.check", "Safety Check?", true).SetValue(true));
                eqMenu.AddItem(new MenuItem("eq.safety.range", "Safety Range", true).SetValue(new Slider(1150, 1, 1150)));
                eqMenu.AddItem(new MenuItem("eq.min.enemy.count.range", "Min Enemy Count", true).SetValue(new Slider(1, 1, 5)));
                Initializer.Config.AddSubMenu(eqMenu);
            }

            var miscMenu = new Menu(":: Miscellaneous", ":: Miscellaneous");
            {
                miscMenu.AddItem(new MenuItem("dodge.jarvan.ult", "Dodge JarvanIV Ult ?", true)).SetValue(true);
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
            Game.OnUpdate += LucianOnUpdate;
            Obj_AI_Base.OnSpellCast += LucianOnSpellCast;
            Drawing.OnDraw += LucianOnDraw;
            Obj_AI_Base.OnProcessSpellCast += OnProcess;
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

        private void LucianOnUpdate(EventArgs args)
        {
            switch (Initializer.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Clear();
                    break;
            }

            if (Initializer.Config.Item("lucian.semi.manual.ult").GetValue<KeyBind>().Active)
            {
                SemiManual();
            }

            if (UltActive && Initializer.Config.Item("lucian.semi.manual.ult").GetValue<KeyBind>().Active)
            {
                Initializer.Orbwalker.SetAttack(false);
            }

            if (!UltActive || !Initializer.Config.Item("lucian.semi.manual.ult").GetValue<KeyBind>().Active)
            {
                Initializer.Orbwalker.SetAttack(true);
            }

            if (!UltActive && Utilities.Enabled("use.eq"))
            {
                if (E.IsReady() &&
                    ObjectManager.Player.CountEnemiesInRange(Utilities.Slider("eq.safety.range")) <= Utilities.Slider("eq.min.enemy.count.range"))
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range + E.Range - 100)))
                    {
                        var aadamage = ObjectManager.Player.GetAutoAttackDamage(enemy);
                        var dmg = ObjectManager.Player.CalcDamage(enemy, Damage.DamageType.Physical,
                            Q.GetDamage(enemy));
                        var combodamage = aadamage + dmg;

                        if (enemy.Health < combodamage)
                        {
                            E.Cast(ObjectManager.Player.Position.Extend(enemy.Position, ERange));
                        }
                    }

                    if (Q.IsReady() && ObjectManager.Player.CountEnemiesInRange(Utilities.Slider("eq.safety.range")) <= Utilities.Slider("eq.min.enemy.count.range"))
                    {
                        foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range)))
                        {
                            var dmg = ObjectManager.Player.CalcDamage(enemy, Damage.DamageType.Physical,
                                Q.GetDamage(enemy));
                            var aadamage = ObjectManager.Player.GetAutoAttackDamage(enemy);

                            var combodamage = aadamage + dmg;

                            if (enemy.Health < combodamage)
                            {
                                Q.CastOnUnit(enemy);
                            }
                        }
                    }
                }
            }
            if (!UltActive && Utilities.Enabled("lucian.q.ks") && Q.IsReady())
            {
                ExtendedQKillSteal();
            }

            if (!UltActive && Utilities.Enabled("lucian.w.ks") && W.IsReady())
            {
                KillstealW();
            }

        }
        private static void SemiManual()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range) &&
                R.GetPrediction(x).CollisionObjects.Count < 2))
            {
                R.Cast(enemy);
            }

        }
        private static void Harass()
        {
            if (ObjectManager.Player.ManaPercent < Utilities.Slider("lucian.harass.mana"))
            {
                return;
            }
            if (Q.IsReady() || Q2.IsReady() && Utilities.Enabled("lucian.q.harass") && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
            {
                HarassQCast();
            }
            if (W.IsReady() && Utilities.Enabled("lucian.w.harass") && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(W.Range) && W.GetPrediction(x).Hitchance >= HitChance.Medium))
                {
                    W.Cast(enemy);
                }
            }
        }
        private static void HarassQCast()
        {
            switch (Initializer.Config.Item("lucian.q.type",true).GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    var minions = ObjectManager.Get<Obj_AI_Minion>().Where(o => o.IsValidTarget(Q.Range));
                    var target = ObjectManager.Get<AIHeroClient>().Where(x => x.IsValidTarget(Q2.Range)).FirstOrDefault(x => Initializer.Config.Item("lucian.white" + x.ChampionName,true).GetValue<bool>());
                    if (target.Distance(ObjectManager.Player.Position) > Q.Range && target.CountEnemiesInRange(Q2.Range) > 0)
                    {
                        foreach (var minion in minions)
                        {
                            if (Q2.WillHit(target, ObjectManager.Player.ServerPosition.Extend(minion.ServerPosition, Q2.Range), 0, HitChance.VeryHigh))
                            {
                                Q2.CastOnUnit(minion);
                            }
                        }
                    }
                    break;
                case 1:
                    foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range)))
                    {
                        Q.CastOnUnit(enemy);
                    }
                    break;
            }
        }
        private static void ExtendedQKillSteal()
        {
            var minions = ObjectManager.Get<Obj_AI_Minion>().Where(o => o.IsValidTarget(Q.Range));
            var target = HeroManager.Enemies.FirstOrDefault(x => x.IsValidTarget(Q2.Range));
            
            if (target != null && (target.Distance(ObjectManager.Player.Position) > Q.Range &&
                                   target.Distance(ObjectManager.Player.Position) < Q2.Range && 
                                   target.CountEnemiesInRange(Q2.Range) >= 1 && target.Health < Q.GetDamage(target) && !target.IsDead))
            {
                foreach (var minion in minions)
                {
                    if (Q2.WillHit(target, ObjectManager.Player.ServerPosition.Extend(minion.ServerPosition, Q2.Range),0,HitChance.VeryHigh))
                    {
                        Q2.CastOnUnit(minion);
                    }
                }
            }
        }
        private static void KillstealW()
        {
            var target = HeroManager.Enemies.Where(x => x.IsValidTarget(W.Range)).
                FirstOrDefault(x=> x.Health < W.GetDamage(x));

            var pred = W.GetPrediction(target);

            if (target != null && pred.Hitchance >= HitChance.High)
            {
                W.Cast(pred.CastPosition);
            }
        }
        private static void Clear()
        {
            if (ObjectManager.Player.ManaPercent < Utilities.Slider("lucian.clear.mana"))
            {
                return;
            }
            if (Q.IsReady() && Utilities.Enabled("lucian.q.clear") && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
            {
                foreach (var minion in MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All,
                MinionTeam.NotAlly))
                {
                    var prediction = Prediction.GetPrediction(minion, Q.Delay,
                        ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).SData.CastRadius);

                    var collision = Q.GetCollision(ObjectManager.Player.Position.To2D(),
                        new List<Vector2> { prediction.UnitPosition.To2D() });

                    foreach (var cs in collision)
                    {
                        if (collision.Count >= Utilities.Slider("lucian.q.minion.hit.count"))
                        {
                            if (collision.Last().Distance(ObjectManager.Player) -
                                collision[0].Distance(ObjectManager.Player) <= 600
                                && collision[0].Distance(ObjectManager.Player) <= 500)
                            {
                                Q.Cast(cs);
                            }
                        }
                    }

                }
            }

            if (W.IsReady() && Utilities.Enabled("lucian.w.clear") && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
            {
                if (W.GetCircularFarmLocation(MinionManager.GetMinions(ObjectManager.Player.Position, Q.Range, MinionTypes.All, MinionTeam.NotAlly)).MinionsHit >= Utilities.Slider("lucian.w.minion.hit.count"))
                {
                    W.Cast(W.GetCircularFarmLocation(MinionManager.GetMinions(ObjectManager.Player.Position, Q.Range, MinionTypes.All, MinionTeam.NotAlly)).Position);
                }
            }

            if (Q.IsReady() && Utilities.Enabled("lucian.q.harass.in.laneclear") &&
                ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
            {
                var minions = ObjectManager.Get<Obj_AI_Minion>().Where(o => o.IsValidTarget(Q.Range));
                var target = ObjectManager.Get<AIHeroClient>().Where(x => x.IsValidTarget(Q2.Range)).FirstOrDefault(x => Initializer.Config.Item("lucian.white" + x.ChampionName,true).GetValue<bool>());
                if (target.Distance(ObjectManager.Player.Position) > Q.Range && target.CountEnemiesInRange(Q2.Range) > 0)
                {
                    foreach (var minion in minions)
                    {
                        if (Q2.WillHit(target, ObjectManager.Player.ServerPosition.Extend(minion.ServerPosition, Q2.Range), 0, HitChance.VeryHigh))
                        {
                            Q2.CastOnUnit(minion);
                        }
                    }
                }
            }
        }
        private void LucianOnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && Orbwalking.IsAutoAttack(args.SData.Name) && args.Target is AIHeroClient && args.Target.IsValid)
            {
                if (Utilities.Enabled("lucian.combo.start.e"))
                {
                    if (!E.IsReady() && Q.IsReady() && Utilities.Enabled("lucian.q.combo") &&
                    ObjectManager.Player.Distance(args.Target.Position) < Q.Range &&
                    Initializer.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
                    {
                        Q.CastOnUnit(((AIHeroClient)args.Target));
                    }
                    
                    if (!E.IsReady() && W.IsReady() && Utilities.Enabled("lucian.w.combo") &&
                        ObjectManager.Player.Distance(args.Target.Position) < W.Range &&
                        Initializer.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
                    {
                        if (Utilities.Enabled("lucian.disable.w.prediction"))
                        {
                            W.Cast(((AIHeroClient)args.Target).Position);
                        }
                        else
                        {
                            if (W.GetPrediction(((AIHeroClient)args.Target)).Hitchance >= HitChance.Medium)
                            {
                                W.Cast(((AIHeroClient)args.Target).Position);
                            }
                        }
                       
                    }
                    if (E.IsReady() && Utilities.Enabled("lucian.e.combo") &&
                        ObjectManager.Player.Distance(args.Target.Position) < Q2.Range &&
                        Initializer.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
                    {
                        switch (Initializer.Config.Item("lucian.e.mode",true).GetValue<StringList>().SelectedIndex)
                        {
                            case 0:
                                Utilities.ECast(((AIHeroClient)args.Target), E);
                                break;
                            case 1:
                                E.Cast(Game.CursorPos);
                                break;
                        }
                        
                    }
                }
                else
                {
                    if (Q.IsReady() && Utilities.Enabled("lucian.q.combo") &&
                    ObjectManager.Player.Distance(args.Target.Position) < Q.Range &&
                    Initializer.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
                    {
                        Q.CastOnUnit(((AIHeroClient)args.Target));
                    }
                    if (W.IsReady() && Utilities.Enabled("lucian.w.combo") &&
                        ObjectManager.Player.Distance(args.Target.Position) < W.Range &&
                        Initializer.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff")
                        && W.GetPrediction(((AIHeroClient)args.Target)).Hitchance >= HitChance.Medium)
                    {
                        W.Cast(((AIHeroClient)args.Target).Position);
                    }
                    if (E.IsReady() && Utilities.Enabled("lucian.e.combo") &&
                        ObjectManager.Player.Distance(args.Target.Position) < Q2.Range &&
                        Initializer.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
                    {
                        switch (Initializer.Config.Item("lucian.e.mode",true).GetValue<StringList>().SelectedIndex)
                        {
                            case 0:
                                Utilities.ECast(((AIHeroClient)args.Target),E);
                                break;
                            case 1:
                                E.Cast(Game.CursorPos);
                                break;
                        }
                    }
                }
                
            }
            else if (sender.IsMe && Orbwalking.IsAutoAttack(args.SData.Name) && args.Target is Obj_AI_Minion && args.Target.IsValid && ((Obj_AI_Minion)args.Target).Team == GameObjectTeam.Neutral
                && ObjectManager.Player.ManaPercent > Utilities.Slider("lucian.clear.mana"))
            {
                if (Q.IsReady() && Utilities.Enabled("lucian.q.jungle") &&
                    ObjectManager.Player.Distance(args.Target.Position) < Q.Range &&
                    Initializer.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
                {
                    Q.CastOnUnit(((Obj_AI_Minion)args.Target));
                }
                if (W.IsReady() && Utilities.Enabled("lucian.w.jungle") &&
                    ObjectManager.Player.Distance(args.Target.Position) < W.Range &&
                    Initializer.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
                {
                    W.Cast(((Obj_AI_Minion)args.Target).Position);
                }
                if (E.IsReady() && Utilities.Enabled("lucian.e.jungle") &&
                   ((Obj_AI_Minion)args.Target).IsValidTarget(1000) &&
                    Initializer.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
                {
                    E.Cast(Game.CursorPos);
                }

            }
        }

        private void LucianOnDraw(EventArgs args)
        {
            throw new NotImplementedException();
        }
        public static bool UltActive => ObjectManager.Player.HasBuff("LucianR");
        private void OnProcess(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsEnemy && sender is AIHeroClient &&
                args.End.Distance(ObjectManager.Player.Position) < 100 &&
                args.SData.Name == "JarvanIVCataclysm" && args.Slot == SpellSlot.R
                && Utilities.Enabled("dodge.jarvan.ult") &&
                (!ObjectManager.Player.Position.Extend(args.End, -E.Range).IsWall() ||
                !ObjectManager.Player.Position.Extend(args.End, -E.Range).UnderTurret(true)))
            {
                var extpos = ObjectManager.Player.Position.Extend(args.End, -E.Range);
                E.Cast(extpos);
            }
        }
    }
}
