using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hikiMarksmanRework.Core.Drawings;
using hikiMarksmanRework.Core.Menus;
using hikiMarksmanRework.Core.Spells;
using hikiMarksmanRework.Core.Utilitys;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace hikiMarksmanRework.Champions
{
    class Lucian
    {
        public Lucian()
        {
            LucianOnLoad();
        }
        private static readonly Render.Sprite HikiSprite = new Render.Sprite(PortAIO.Properties.Resources.logo, new Vector2((Drawing.Width / 2) - 500, (Drawing.Height / 2) - 350));
        private static void LucianOnLoad()
        {
            LucianMenu.Config =
                new Menu("hikiMarksman:AIO - Lucian", "hikiMarksman:AIO - Lucian", true).SetFontStyle(System.Drawing.FontStyle.Bold,
                    SharpDX.Color.Gold);
            {
                LucianSpells.Init();
                LucianMenu.OrbwalkerInit();
                LucianMenu.MenuInit();
            }

            Chat.Print("<font color='#ff3232'>hikiMarksman:AIO - Lucian: </font><font color='#00FF00'>loaded! You can rekt everyone with this assembly</font>", ObjectManager.Player.ChampionName);
            Chat.Print(string.Format("<font color='#ff3232'>hikiMarksman:AIO - </font><font color='#00FF00'>Assembly Version: </font><font color='#ff3232'><b>{0}</b></font> ", typeof(Program).Assembly.GetName().Version));
            Chat.Print("<font color='#ff3232'>If you like this assembly feel free to upvote on Assembly Database</font>");


            HikiSprite.Add(0);
            HikiSprite.OnDraw();
            LeagueSharp.Common.Utility.DelayAction.Add(8000, () => HikiSprite.Remove());

            Notifications.AddNotification("hikiMarksman:AIO", 4000);
            Notifications.AddNotification(String.Format("{0} Loaded", ObjectManager.Player.ChampionName), 5000);
            Notifications.AddNotification("Gift From Hikigaya", 6000);

            Game.OnUpdate += LucianOnUpdate;
            Spellbook.OnCastSpell += LucianOnCastSpell;
            Obj_AI_Base.OnSpellCast += LucianOnSpellCast;
            Drawing.OnDraw += LucianOnDraw;
        }
        public static bool UltActive
        {
            get { return ObjectManager.Player.LSHasBuff("LucianR"); }
        }

        private static void LucianOnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            
            if (UltActive && (args.Slot == SpellSlot.R) && sender.Owner.IsMe)
            {
                args.Process = false;
                LucianMenu.Orbwalker.SetAttack(false);
            }
            else
            {
                LucianMenu.Orbwalker.SetAttack(true);
                args.Process = true;
            }
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
        private static void LucianOnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && Orbwalking.IsAutoAttack(args.SData.Name) && args.Target is AIHeroClient && args.Target.IsValid)
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
                    ECast(((AIHeroClient)args.Target));
                }
            }
            else if (sender.IsMe && Orbwalking.IsAutoAttack(args.SData.Name) && args.Target is Obj_AI_Minion && args.Target.IsValid && ((Obj_AI_Minion)args.Target).Team == GameObjectTeam.Neutral
                && ObjectManager.Player.ManaPercent > Helper.LSlider("lucian.clear.mana"))
            {
                if (LucianSpells.Q.LSIsReady() && Helper.LEnabled("lucian.q.combo") &&
                    ObjectManager.Player.LSDistance(args.Target.Position) < LucianSpells.Q.Range &&
                    LucianMenu.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
                {
                    LucianSpells.Q.CastOnUnit(((Obj_AI_Minion)args.Target));
                }
                if (LucianSpells.W.LSIsReady() && Helper.LEnabled("lucian.w.combo") &&
                    ObjectManager.Player.LSDistance(args.Target.Position) < LucianSpells.W.Range &&
                    LucianMenu.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
                {
                    LucianSpells.W.Cast(((Obj_AI_Minion)args.Target).Position);
                }
                if (LucianSpells.E.LSIsReady() && Helper.LEnabled("lucian.e.combo") &&
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
        }

        private static void SemiManual()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            foreach (var enemy in HeroManager.Enemies.Where(x=> x.LSIsValidTarget(LucianSpells.Q.Range) && 
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
                foreach (var enemy in HeroManager.Enemies.Where(x=> x.LSIsValidTarget(LucianSpells.W.Range) && LucianSpells.W.GetPrediction(x).Hitchance >= HitChance.Medium))
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
