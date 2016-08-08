using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy;

namespace LCS_Lucian
{
    class Program
    {
        public static void OnLoad()
        {
            if (ObjectManager.Player.ChampionName != "Lucian")
            {
                return;
            }

            LucianMenu.Config =
                new Menu("LCS Series: Lucian", "LCS Series: Lucian", true).SetFontStyle(System.Drawing.FontStyle.Bold,
                    Color.Gold);
            {
                LucianSpells.Init();
                LucianMenu.OrbwalkerInit();
                LucianMenu.MenuInit();
            }

            Chat.Print("<font color='#99FFFF'>LCS Series - Lucian loaded! </font><font color='#99FF00'> Be Rekkles ! Its Possible. Enjoy GODSPEED Spell + Passive Usage </font>");
            Chat.Print("<font color='##FFCC00'>LCS Series totally improved LCS player style.</font>");

            Game.OnUpdate += LucianOnUpdate;
            Obj_AI_Base.OnSpellCast += LucianOnDoCast;
            Drawing.OnDraw += LucianOnDraw;
        }
        public static bool UltActive
        {
            get { return ObjectManager.Player.HasBuff("LucianR"); }
        }
        private static void ECast(AIHeroClient enemy)
        {
            var range = Orbwalking.GetRealAutoAttackRange(enemy);
            var path = Geometry.LSCircleCircleIntersection(ObjectManager.Player.ServerPosition.LSTo2D(),
                Prediction.GetPrediction(enemy, 0.25f).UnitPosition.LSTo2D(), LucianSpells.E.Range, range);

            if (path.Count() > 0)
            {
                var epos = path.MinOrDefault(x => x.LSDistance(Game.CursorPos));
                if (epos.To3D().LSUnderTurret(true) || epos.To3D().LSIsWall())
                {
                    return;
                }

                if (epos.To3D().LSCountEnemiesInRange(LucianSpells.E.Range - 100) > 0)
                {
                    return;
                }
                LucianSpells.E.Cast(epos);
            }
            if (path.Count() == 0)
            {
                var epos = ObjectManager.Player.ServerPosition.LSExtend(enemy.ServerPosition, -LucianSpells.E.Range);
                if (epos.LSUnderTurret(true) || epos.LSIsWall())
                {
                    return;
                }

                // no intersection or target to close
                LucianSpells.E.Cast(ObjectManager.Player.ServerPosition.LSExtend(enemy.ServerPosition, -LucianSpells.E.Range));
            }
        }
        private static void LucianOnDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && Orbwalking.IsAutoAttack(args.SData.Name) && args.Target is AIHeroClient && args.Target.IsValid)
            {
                if (Helper.LEnabled("lucian.combo.start.e"))
                {
                    if (!LucianSpells.E.LSIsReady() && LucianSpells.Q.LSIsReady() && Helper.LEnabled("lucian.q.combo") &&
                    ObjectManager.Player.LSDistance(args.Target.Position) < LucianSpells.Q.Range &&
                    LucianMenu.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
                    {
                        LucianSpells.Q.CastOnUnit(((AIHeroClient)args.Target));
                    }
                    
                    if (!LucianSpells.E.LSIsReady() && LucianSpells.W.LSIsReady() && Helper.LEnabled("lucian.w.combo") &&
                        ObjectManager.Player.LSDistance(args.Target.Position) < LucianSpells.W.Range &&
                        LucianMenu.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
                    {
                        if (Helper.LEnabled("lucian.disable.w.prediction"))
                        {
                            LucianSpells.W.Cast(((AIHeroClient)args.Target).Position);
                        }
                        else
                        {
                            if (LucianSpells.W.GetPrediction(((AIHeroClient)args.Target)).Hitchance >= HitChance.Medium)
                            {
                                LucianSpells.W.Cast(((AIHeroClient)args.Target).Position);
                            }
                        }
                       
                    }
                    if (LucianSpells.E.LSIsReady() && Helper.LEnabled("lucian.e.combo") &&
                        ObjectManager.Player.LSDistance(args.Target.Position) < LucianSpells.Q2.Range &&
                        LucianMenu.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
                    {
                        switch (LucianMenu.Config.Item("lucian.e.mode").GetValue<StringList>().SelectedIndex)
                        {
                            case 0:
                                ECast(((AIHeroClient)args.Target));
                                break;
                            case 1:
                                LucianSpells.E.Cast(Game.CursorPos);
                                break;
                        }
                        
                    }
                }
                else
                {
                    if (LucianSpells.Q.LSIsReady() && Helper.LEnabled("lucian.q.combo") &&
                    ObjectManager.Player.LSDistance(args.Target.Position) < LucianSpells.Q.Range &&
                    LucianMenu.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
                    {
                        LucianSpells.Q.CastOnUnit(((AIHeroClient)args.Target));
                    }
                    if (LucianSpells.W.LSIsReady() && Helper.LEnabled("lucian.w.combo") &&
                        ObjectManager.Player.LSDistance(args.Target.Position) < LucianSpells.W.Range &&
                        LucianMenu.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff")
                        && LucianSpells.W.GetPrediction(((AIHeroClient)args.Target)).Hitchance >= HitChance.Medium)
                    {
                        LucianSpells.W.Cast(((AIHeroClient)args.Target).Position);
                    }
                    if (LucianSpells.E.LSIsReady() && Helper.LEnabled("lucian.e.combo") &&
                        ObjectManager.Player.LSDistance(args.Target.Position) < LucianSpells.Q2.Range &&
                        LucianMenu.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
                    {
                        switch (LucianMenu.Config.Item("lucian.e.mode").GetValue<StringList>().SelectedIndex)
                        {
                            case 0:
                                ECast(((AIHeroClient)args.Target));
                                break;
                            case 1:
                                LucianSpells.E.Cast(Game.CursorPos);
                                break;
                        }
                    }
                }
                
            }
            else if (sender.IsMe && Orbwalking.IsAutoAttack(args.SData.Name) && args.Target is Obj_AI_Minion && args.Target.IsValid && ((Obj_AI_Minion)args.Target).Team == GameObjectTeam.Neutral
                && ObjectManager.Player.ManaPercent > Helper.LSlider("lucian.clear.mana"))
            {
                if (LucianSpells.Q.LSIsReady() && Helper.LEnabled("lucian.q.jungle") &&
                    ObjectManager.Player.LSDistance(args.Target.Position) < LucianSpells.Q.Range &&
                    LucianMenu.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
                {
                    LucianSpells.Q.CastOnUnit(((Obj_AI_Minion)args.Target));
                }
                if (LucianSpells.W.LSIsReady() && Helper.LEnabled("lucian.w.jungle") &&
                    ObjectManager.Player.LSDistance(args.Target.Position) < LucianSpells.W.Range &&
                    LucianMenu.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
                {
                    LucianSpells.W.Cast(((Obj_AI_Minion)args.Target).Position);
                }
                if (LucianSpells.E.LSIsReady() && Helper.LEnabled("lucian.e.jungle") &&
                   ((Obj_AI_Minion)args.Target).LSIsValidTarget(1000) &&
                    LucianMenu.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
                {
                    LucianSpells.E.Cast(Game.CursorPos);
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

            if (UltActive && LucianMenu.Config.Item("lucian.semi.manual.ult").GetValue<KeyBind>().Active)
            {
                LucianMenu.Orbwalker.SetAttack(false);
            }

            if (!UltActive || !LucianMenu.Config.Item("lucian.semi.manual.ult").GetValue<KeyBind>().Active)
            {
                LucianMenu.Orbwalker.SetAttack(true);
            }

            if (Helper.LEnabled("lucian.q.ks") && LucianSpells.Q.LSIsReady())
            {
                ExtendedQKillSteal();
            }

            if (Helper.LEnabled("lucian.w.ks") && LucianSpells.W.LSIsReady())
            {
                KillstealW();
            }


        }
        private static void SemiManual()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            foreach (var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(LucianSpells.R.Range) &&
                LucianSpells.R.GetPrediction(x).CollisionObjects.Count == 0))
            {
                LucianSpells.R.Cast(enemy);
            }
        }
        private static void Harass()
        {
            if (ObjectManager.Player.ManaPercent < Helper.LSlider("lucian.harass.mana"))
            {
                return;
            }
            if (LucianSpells.Q.LSIsReady() || LucianSpells.Q2.LSIsReady() && Helper.LEnabled("lucian.q.harass") && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
            {
                HarassQCast();
            }
            if (LucianSpells.W.LSIsReady() && Helper.LEnabled("lucian.w.harass") && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(LucianSpells.W.Range) && LucianSpells.W.GetPrediction(x).Hitchance >= HitChance.Medium))
                {
                    LucianSpells.W.Cast(enemy);
                }
            }
        }
        private static void HarassQCast()
        {
            switch (LucianMenu.Config.Item("lucian.q.type").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    var minions = ObjectManager.Get<Obj_AI_Minion>().Where(o => o.LSIsValidTarget(LucianSpells.Q.Range));
                    var target = ObjectManager.Get<AIHeroClient>().Where(x => x.LSIsValidTarget(LucianSpells.Q2.Range)).FirstOrDefault(x => LucianMenu.Config.Item("lucian.white" + x.ChampionName).GetValue<bool>());
                    if (target.LSDistance(ObjectManager.Player.Position) > LucianSpells.Q.Range && target.LSCountEnemiesInRange(LucianSpells.Q2.Range) > 0)
                    {
                        foreach (var minion in minions)
                        {
                            if (LucianSpells.Q2.WillHit(target, ObjectManager.Player.ServerPosition.LSExtend(minion.ServerPosition, LucianSpells.Q2.Range), 0, HitChance.VeryHigh))
                            {
                                LucianSpells.Q2.CastOnUnit(minion);
                            }
                        }
                    }
                    break;
                case 1:
                    foreach (var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(LucianSpells.Q.Range)))
                    {
                        LucianSpells.Q.CastOnUnit(enemy);
                    }
                    break;
            }
        }
        private static void ExtendedQKillSteal()
        {
            var minions = ObjectManager.Get<Obj_AI_Minion>().Where(o => o.LSIsValidTarget(LucianSpells.Q.Range));
            var target = HeroManager.Enemies.FirstOrDefault(x => x.LSIsValidTarget(LucianSpells.Q2.Range));
            
            if (target.LSDistance(ObjectManager.Player.Position) > LucianSpells.Q.Range &&
                target.LSDistance(ObjectManager.Player.Position) < LucianSpells.Q2.Range && 
                target.LSCountEnemiesInRange(LucianSpells.Q2.Range) >= 1 && target.Health < LucianSpells.Q.GetDamage(target) && !target.IsDead)
            {
                foreach (var minion in minions)
                {
                    if (LucianSpells.Q2.WillHit(target, ObjectManager.Player.ServerPosition.LSExtend(minion.ServerPosition, LucianSpells.Q2.Range),0,HitChance.VeryHigh))
                    {
                        LucianSpells.Q2.CastOnUnit(minion);
                    }
                }
            }
        }
        private static void KillstealW()
        {
            var target = HeroManager.Enemies.Where(x => x.LSIsValidTarget(LucianSpells.W.Range)).
                FirstOrDefault(x=> x.Health < LucianSpells.W.GetDamage(x));

            var pred = LucianSpells.W.GetPrediction(target);

            if (target != null && pred.Hitchance >= HitChance.High)
            {
                LucianSpells.W.Cast(pred.CastPosition);
            }
        }
        private static void Clear()
        {
            if (ObjectManager.Player.ManaPercent < Helper.LSlider("lucian.clear.mana"))
            {
                return;
            }
            if (LucianSpells.Q.LSIsReady() && Helper.LEnabled("lucian.q.clear") && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
            {
                foreach (var minion in MinionManager.GetMinions(ObjectManager.Player.ServerPosition, LucianSpells.Q.Range, MinionTypes.All,
                MinionTeam.NotAlly))
                {
                    var prediction = Prediction.GetPrediction(minion, LucianSpells.Q.Delay,
                        ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).SData.CastRadius);

                    var collision = LucianSpells.Q.GetCollision(ObjectManager.Player.Position.LSTo2D(),
                        new List<Vector2> { prediction.UnitPosition.LSTo2D() });

                    foreach (var cs in collision)
                    {
                        if (collision.Count >= Helper.LSlider("lucian.q.minion.hit.count"))
                        {
                            if (collision.Last().LSDistance(ObjectManager.Player) -
                                collision[0].LSDistance(ObjectManager.Player) <= 600
                                && collision[0].LSDistance(ObjectManager.Player) <= 500)
                            {
                                LucianSpells.Q.Cast(cs);
                            }
                        }
                    }

                }
            }
            if (LucianSpells.W.LSIsReady() && Helper.LEnabled("lucian.w.clear") && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
            {
                if (LucianSpells.W.GetCircularFarmLocation(MinionManager.GetMinions(ObjectManager.Player.Position, LucianSpells.Q.Range, MinionTypes.All, MinionTeam.NotAlly)).MinionsHit >= Helper.LSlider("lucian.w.minion.hit.count"))
                {
                    LucianSpells.W.Cast(LucianSpells.W.GetCircularFarmLocation(MinionManager.GetMinions(ObjectManager.Player.Position, LucianSpells.Q.Range, MinionTypes.All, MinionTeam.NotAlly)).Position);
                }
            }
        }
        private static void LucianOnDraw(EventArgs args)
        {
            LucianDrawing.Init();
        }
    }
}
