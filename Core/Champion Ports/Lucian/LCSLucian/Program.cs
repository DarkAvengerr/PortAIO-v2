    using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Spells = LCS_Lucian.LucianSpells;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace LCS_Lucian
{
    internal class Program
    {
        // ReSharper disable once UnusedParameter.Local
        public static void Main()
        {
            OnLoad(new EventArgs());
        }

        private static void OnLoad(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != "Lucian")
            {
                return;
            }

            LucianMenu.Config =
                new Menu("LCS Series: Lucian", "LCS Series: Lucian", true).SetFontStyle(System.Drawing.FontStyle.Bold,
                    Color.Gold);
            {
                Spells.Init();
                LucianMenu.OrbwalkerInit();
                LucianMenu.MenuInit();
            }

            Chat.Print("<font color='#99FFFF'>LCS Series - Lucian loaded! </font><font color='#99FF00'> Be Rekkles ! Its Possible. Enjoy GODSPEED Spell + Passive Usage </font>");
            Chat.Print("<font color='##FFCC00'>LCS Series totally improved LCS player style.</font>");

            Game.OnUpdate += LucianOnUpdate;
            Obj_AI_Base.OnSpellCast += LucianOnSpellCast;
            Drawing.OnDraw += LucianOnDraw;
            Obj_AI_Base.OnProcessSpellCast += OnProcess;
        }

        private static void OnProcess(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsEnemy && sender is AIHeroClient && 
                args.End.Distance(ObjectManager.Player.Position) < 100 &&
                args.SData.Name == "JarvanIVCataclysm" && args.Slot == SpellSlot.R
                && Helper.LEnabled("dodge.jarvan.ult") && 
                (!ObjectManager.Player.Position.Extend(args.End, -Spells.E.Range).IsWall() || 
                !ObjectManager.Player.Position.Extend(args.End, -Spells.E.Range).UnderTurret(true)))
            {
                var extpos = ObjectManager.Player.Position.Extend(args.End, - Spells.E.Range);
                Spells.E.Cast(extpos);
            }
        }

        private static void ECast(AIHeroClient enemy)
        {
            var range = Orbwalking.GetRealAutoAttackRange(enemy);
            var path = Geometry.CircleCircleIntersection(ObjectManager.Player.ServerPosition.To2D(),
                Prediction.GetPrediction(enemy, 0.25f).UnitPosition.To2D(),  Helper.LSlider("lucian.e.range"), range);

            if (path.Length > 0)
            {
                var epos = path.MinOrDefault(x => x.Distance(Game.CursorPos));
                if (epos.To3D().UnderTurret(true) || epos.To3D().IsWall())
                {
                    return;
                }

                if (epos.To3D().CountEnemiesInRange( Helper.LSlider("lucian.e.range") - 100) > 0)
                {
                    return;
                }
                Spells.E.Cast(epos);
            }
            if (path.Length == 0)
            {
                var epos = ObjectManager.Player.ServerPosition.Extend(enemy.ServerPosition, - Helper.LSlider("lucian.e.range"));
                if (epos.UnderTurret(true) || epos.IsWall())
                {
                    return;
                }

                // no intersection or target to close
                Spells.E.Cast(ObjectManager.Player.ServerPosition.Extend(enemy.ServerPosition, - Helper.LSlider("lucian.e.range")));
            }
        }
        private static void LucianOnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && Orbwalking.IsAutoAttack(args.SData.Name) && args.Target is AIHeroClient && args.Target.IsValid)
            {
                if (Helper.LEnabled("lucian.combo.start.e"))
                {
                    if (!Spells.E.IsReady() && Spells.Q.IsReady() && Helper.LEnabled("lucian.q.combo") &&
                    ObjectManager.Player.Distance(args.Target.Position) < Spells.Q.Range &&
                    LucianMenu.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
                    {
                        Spells.Q.CastOnUnit(((AIHeroClient)args.Target));
                    }
                    
                    if (!Spells.E.IsReady() && Spells.W.IsReady() && Helper.LEnabled("lucian.w.combo") &&
                        ObjectManager.Player.Distance(args.Target.Position) < Spells.W.Range &&
                        LucianMenu.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
                    {
                        if (Helper.LEnabled("lucian.disable.w.prediction"))
                        {
                            Spells.W.Cast(((AIHeroClient)args.Target).Position);
                        }
                        else
                        {
                            if (Spells.W.GetPrediction(((AIHeroClient)args.Target)).Hitchance >= HitChance.Medium)
                            {
                                Spells.W.Cast(((AIHeroClient)args.Target).Position);
                            }
                        }
                       
                    }
                    if (Spells.E.IsReady() && Helper.LEnabled("lucian.e.combo") &&
                        ObjectManager.Player.Distance(args.Target.Position) < Spells.Q2.Range &&
                        LucianMenu.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
                    {
                        switch (LucianMenu.Config.Item("lucian.e.mode").GetValue<StringList>().SelectedIndex)
                        {
                            case 0:
                                ECast(((AIHeroClient)args.Target));
                                break;
                            case 1:
                                Spells.E.Cast(Game.CursorPos);
                                break;
                        }
                        
                    }
                }
                else
                {
                    if (Spells.Q.IsReady() && Helper.LEnabled("lucian.q.combo") &&
                    ObjectManager.Player.Distance(args.Target.Position) < Spells.Q.Range &&
                    LucianMenu.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
                    {
                        Spells.Q.CastOnUnit(((AIHeroClient)args.Target));
                    }
                    if (Spells.W.IsReady() && Helper.LEnabled("lucian.w.combo") &&
                        ObjectManager.Player.Distance(args.Target.Position) < Spells.W.Range &&
                        LucianMenu.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff")
                        && Spells.W.GetPrediction(((AIHeroClient)args.Target)).Hitchance >= HitChance.Medium)
                    {
                        Spells.W.Cast(((AIHeroClient)args.Target).Position);
                    }
                    if (Spells.E.IsReady() && Helper.LEnabled("lucian.e.combo") &&
                        ObjectManager.Player.Distance(args.Target.Position) < Spells.Q2.Range &&
                        LucianMenu.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
                    {
                        switch (LucianMenu.Config.Item("lucian.e.mode").GetValue<StringList>().SelectedIndex)
                        {
                            case 0:
                                ECast(((AIHeroClient)args.Target));
                                break;
                            case 1:
                                Spells.E.Cast(Game.CursorPos);
                                break;
                        }
                    }
                }
                
            }
            else if (sender.IsMe && Orbwalking.IsAutoAttack(args.SData.Name) && args.Target is Obj_AI_Minion && args.Target.IsValid && ((Obj_AI_Minion)args.Target).Team == GameObjectTeam.Neutral
                && ObjectManager.Player.ManaPercent > Helper.LSlider("lucian.clear.mana"))
            {
                if (Spells.Q.IsReady() && Helper.LEnabled("lucian.q.jungle") &&
                    ObjectManager.Player.Distance(args.Target.Position) < Spells.Q.Range &&
                    LucianMenu.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
                {
                    Spells.Q.CastOnUnit(((Obj_AI_Minion)args.Target));
                }
                if (Spells.W.IsReady() && Helper.LEnabled("lucian.w.jungle") &&
                    ObjectManager.Player.Distance(args.Target.Position) < Spells.W.Range &&
                    LucianMenu.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
                {
                    Spells.W.Cast(((Obj_AI_Minion)args.Target).Position);
                }
                if (Spells.E.IsReady() && Helper.LEnabled("lucian.e.jungle") &&
                   ((Obj_AI_Minion)args.Target).IsValidTarget(1000) &&
                    LucianMenu.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
                {
                    Spells.E.Cast(Game.CursorPos);
                }

            }
        }
        private static void LucianOnUpdate(EventArgs args)
        {
            switch (LucianMenu.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Clear();
                    break;
            }

            if (LucianMenu.Config.Item("lucian.semi.manual.ult").GetValue<KeyBind>().Active)
            {
                SemiManual();
            }

            if (ObjectManager.Player.HasBuff("LucianR") && LucianMenu.Config.Item("lucian.semi.manual.ult").GetValue<KeyBind>().Active)
            {
                LucianMenu.Orbwalker.SetAttack(false);
            }

            if (!ObjectManager.Player.HasBuff("LucianR") || !LucianMenu.Config.Item("lucian.semi.manual.ult").GetValue<KeyBind>().Active)
            {
                LucianMenu.Orbwalker.SetAttack(true);
            }

            if (!ObjectManager.Player.HasBuff("LucianR") && Helper.LEnabled("use.eq"))
            {
                if (Spells.E.IsReady() && 
                    ObjectManager.Player.CountEnemiesInRange(Helper.LSlider("eq.safety.range")) <= Helper.LSlider("eq.min.enemy.count.range"))
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x=> x.IsValidTarget(Spells.Q.Range + Spells.E.Range - 100 )))
                    {
                        var aadamage = ObjectManager.Player.GetAutoAttackDamage(enemy);
                        var dmg = ObjectManager.Player.CalcDamage(enemy, Damage.DamageType.Physical,
                            Spells.Q.GetDamage(enemy));
                        var combodamage = aadamage + dmg;

                        if (enemy.Health < combodamage - 100)
                        {
                            Spells.E.Cast(ObjectManager.Player.Position.Extend(enemy.Position,  Helper.LSlider("lucian.e.range")));
                        }
                    }
                    
                    if (Spells.Q.IsReady() && ObjectManager.Player.CountEnemiesInRange(Helper.LSlider("eq.safety.range")) <= Helper.LSlider("eq.min.enemy.count.range"))
                    {
                        foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.Q.Range)))
                        {
                            var dmg = ObjectManager.Player.CalcDamage(enemy, Damage.DamageType.Physical,
                                Spells.Q.GetDamage(enemy));
                            var aadamage = ObjectManager.Player.GetAutoAttackDamage(enemy);

                            var combodamage = aadamage + dmg - 100;

                            if (enemy.Health < combodamage)
                            {
                                Spells.Q.CastOnUnit(enemy);
                            }
                        }
                    }
                }
            }
            if (!ObjectManager.Player.HasBuff("LucianR") && Helper.LEnabled("lucian.q.ks") && Spells.Q.IsReady())
            {
                ExtendedQKillSteal();
            }

            if (!ObjectManager.Player.HasBuff("LucianR") && Helper.LEnabled("lucian.w.ks") && Spells.W.IsReady())
            {
                KillstealW();
            }


        }
        private static void SemiManual()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.R.Range) &&
                Spells.R.GetPrediction(x).CollisionObjects.Count < 2))
            {
                Spells.R.Cast(enemy);
            }

        }
        private static void Harass()
        {
            if (ObjectManager.Player.ManaPercent < Helper.LSlider("lucian.harass.mana"))
            {
                return;
            }
            if (Spells.Q.IsReady() || Spells.Q2.IsReady() && Helper.LEnabled("lucian.q.harass") && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
            {
                HarassQCast();
            }
            if (Spells.W.IsReady() && Helper.LEnabled("lucian.w.harass") && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.W.Range) && Spells.W.GetPrediction(x).Hitchance >= HitChance.Medium))
                {
                    Spells.W.Cast(enemy);
                }
            }
        }
        private static void HarassQCast()
        {
            switch (LucianMenu.Config.Item("lucian.q.type").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    var minions = ObjectManager.Get<Obj_AI_Minion>().Where(o => o.IsValidTarget(Spells.Q.Range));
                    var target = ObjectManager.Get<AIHeroClient>().Where(x => x.IsValidTarget(Spells.Q2.Range)).FirstOrDefault(x => LucianMenu.Config.Item("lucian.white" + x.ChampionName).GetValue<bool>());
                    if (target.Distance(ObjectManager.Player.Position) > Spells.Q.Range && target.CountEnemiesInRange(Spells.Q2.Range) > 0)
                    {
                        foreach (var minion in minions)
                        {
                            if (Spells.Q2.WillHit(target, ObjectManager.Player.ServerPosition.Extend(minion.ServerPosition, Spells.Q2.Range), 0, HitChance.VeryHigh))
                            {
                                Spells.Q2.CastOnUnit(minion);
                            }
                        }
                    }
                    break;
                case 1:
                    foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.Q.Range)))
                    {
                        Spells.Q.CastOnUnit(enemy);
                    }
                    break;
            }
        }
        private static void ExtendedQKillSteal()
        {
            var minions = ObjectManager.Get<Obj_AI_Minion>().Where(o => o.IsValidTarget(Spells.Q.Range));
            var target = HeroManager.Enemies.FirstOrDefault(x => x.IsValidTarget(Spells.Q2.Range));
            
            if (target != null && (target.Distance(ObjectManager.Player.Position) > Spells.Q.Range &&
                                   target.Distance(ObjectManager.Player.Position) < Spells.Q2.Range &&
                                   target.CountEnemiesInRange(Spells.Q2.Range) >= 1 && target.Health < Spells.Q.GetDamage(target) + (ObjectManager.Player.AttackRange * 2) && !target.IsDead))
            {
                foreach (var minion in minions)
                {
                    if (Spells.Q2.WillHit(target, ObjectManager.Player.ServerPosition.Extend(minion.ServerPosition, Spells.Q2.Range),0,HitChance.VeryHigh))
                    {
                        Spells.Q2.CastOnUnit(minion);
                    }
                }
            }
        }
        private static void KillstealW()
        {
            var target = HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.W.Range)).
                FirstOrDefault(x => x.Health < Spells.W.GetDamage(x) + (ObjectManager.Player.AttackRange * 2));

            if (target != null)
            {
                var pred = Spells.W.GetPrediction(target);

                if (target != null && pred.Hitchance >= HitChance.High)
                {
                    Spells.W.Cast(pred.CastPosition);
                }
            }
        }
        private static void Clear()
        {
            if (ObjectManager.Player.ManaPercent < Helper.LSlider("lucian.clear.mana"))
            {
                return;
            }
            if (Spells.Q.IsReady() && Helper.LEnabled("lucian.q.clear") && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
            {
                foreach (var minion in MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Spells.Q.Range, MinionTypes.All,
                MinionTeam.NotAlly))
                {
                    var prediction = Prediction.GetPrediction(minion, Spells.Q.Delay,
                        ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).SData.CastRadius);

                    var collision = Spells.Q.GetCollision(ObjectManager.Player.Position.To2D(),
                        new List<Vector2> { prediction.UnitPosition.To2D() });

                    foreach (var cs in collision)
                    {
                        if (collision.Count >= Helper.LSlider("lucian.q.minion.hit.count"))
                        {
                            if (collision.Last().Distance(ObjectManager.Player) -
                                collision[0].Distance(ObjectManager.Player) <= 600
                                && collision[0].Distance(ObjectManager.Player) <= 500)
                            {
                                Spells.Q.Cast(cs);
                            }
                        }
                    }

                }
            }

            if (Spells.W.IsReady() && Helper.LEnabled("lucian.w.clear") && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
            {
                if (Spells.W.GetCircularFarmLocation(MinionManager.GetMinions(ObjectManager.Player.Position, Spells.Q.Range, MinionTypes.All, MinionTeam.NotAlly)).MinionsHit >= Helper.LSlider("lucian.w.minion.hit.count"))
                {
                    Spells.W.Cast(Spells.W.GetCircularFarmLocation(MinionManager.GetMinions(ObjectManager.Player.Position, Spells.Q.Range, MinionTypes.All, MinionTeam.NotAlly)).Position);
                }
            }

        }
        private static void LucianOnDraw(EventArgs args)
        {
            LucianDrawing.Init();
        }
    }
}
