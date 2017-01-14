using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;

using EloBuddy; 
using LeagueSharp.Common; 
namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Ziggs : Base
    {
        public static Spell Q2, Q3;
        GameObject ZiggsW = null;

        public float DragonDmg = 0;
        public double DragonTime = 0;

        public Ziggs()
        {
            Q1 = new Spell(SpellSlot.Q, 850f);
            Q2 = new Spell(SpellSlot.Q, 1115f);
            Q3 = new Spell(SpellSlot.Q, 1390f);

            W = new Spell(SpellSlot.W, 1000f);
            E = new Spell(SpellSlot.E, 900f);
            R = new Spell(SpellSlot.R, 5300f);

            Q1.SetSkillshot(0.26f, 95f, 1650f, false, SkillshotType.SkillshotCircle);
            Q2.SetSkillshot(0.4f, 50f, 1650f, false, SkillshotType.SkillshotCircle);
            Q3.SetSkillshot(0.6f, 50f, 1650f, false, SkillshotType.SkillshotCircle);

            W.SetSkillshot(0.25f, 275f, 1750f, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(1f, 150f, 1750f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.7f, 340f , 1500f, false, SkillshotType.SkillshotCircle);

            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("noti", "Show notification", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("onlyRdy", "Draw only ready spells", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("qRange", "Q range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("wRange", "W range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("eRange", "E range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("rRange", "R range", true).SetValue(false));

            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("farmQout", "Last hit Q minion out range AA", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("farmQ", "Lane clear Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("farmE", "Lane clear E", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("jungleQ", "Jungle clear Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("jungleW", "Jungle clear W", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("jungleE", "Jungle clear E", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("autoQ", "Auto Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("harassQ", "Harass Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("QHarassMana", "Harass Mana", true).SetValue(new Slider(30, 100, 0)));

            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("autoW", "Auto W", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("harassW", "Harass W", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("interupterW", "Interrupter W", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("turretW", "auto destroy turrets", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("useW", "dash W hotkey", true).SetValue(new KeyBind("Y".ToCharArray()[0], KeyBindType.Press))); //32 == space

            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("autoE", "Auto E on CC", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("comboE", "Auto E in Combo BETA", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("AGC", "Anti Gapcloser E", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("opsE", "OnProcessSpellCastE", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("telE", "Auto E teleport", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("autoR", "Auto R", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").SubMenu("Combo").AddItem(new MenuItem("minHpSpecial", "Min main target HP %", true).SetValue(new Slider(50, 100, 0)));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").SubMenu("Combo").AddItem(new MenuItem("Rcc", "R cc", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").SubMenu("Combo").AddItem(new MenuItem("Raoe", "R AOE", true).SetValue(new Slider(3, 5, 0)));

            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("useR", "Semi-manual cast R key", true).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press))); //32 == space
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("MaxRangeR", "Max R ks range", true).SetValue(new Slider(3000, 5000, 0)));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("MinRangeR", "Min R ks range", true).SetValue(new Slider(900, 5000, 0)));

            Config.SubMenu(Player.ChampionName).SubMenu("R Config").SubMenu("R Jungle stealer").AddItem(new MenuItem("Rjungle", "R Jungle stealer", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").SubMenu("R Jungle stealer").AddItem(new MenuItem("Rdragon", "Dragon", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").SubMenu("R Jungle stealer").AddItem(new MenuItem("Rbaron", "Baron", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").SubMenu("R Jungle stealer").AddItem(new MenuItem("Rred", "Red", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").SubMenu("R Jungle stealer").AddItem(new MenuItem("Rblue", "Blue", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").SubMenu("R Jungle stealer").AddItem(new MenuItem("Rally", "Ally stealer", true).SetValue(false));


            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if(Config.Item("AGC", true).GetValue<bool>() && Player.Mana > RMANA + EMANA && gapcloser.Sender.IsValidTarget(E.Range))
            {
                E.Cast(gapcloser.End);
            }
        }

        private void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (Config.Item("interupterW", true).GetValue<bool>() && sender.IsValidTarget(W.Range))
            {
                W.Cast(sender.Position);
            }
        }

        private void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (sender.IsValid && sender.Name.Contains("Ziggs_Base_W_aoe_green"))
                ZiggsW = sender;
        }

        private void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name.Contains("Ziggs_Base_W_aoe_green"))
                ZiggsW = null;
        }

        private void Game_OnGameUpdate(EventArgs args)
        {

            if (Program.LagFree(0))
            {
                SetMana();
                Jungle();
            }
            if (Program.LagFree(1) && E.IsReady())
                LogicE();
            if (Program.LagFree(2) && Q1.IsReady() && Config.Item("autoQ", true).GetValue<bool>())
                LogicQ();
            if (Program.LagFree(3) && W.IsReady() && Config.Item("autoW", true).GetValue<bool>())
                LogicW();
            

            if (R.IsReady())
            {
                if (Program.LagFree(4) && Config.Item("autoR", true).GetValue<bool>())
                    LogicR();

                if (Config.Item("useR", true).GetValue<KeyBind>().Active)
                {
                    var t = TargetSelector.GetTarget(2500, TargetSelector.DamageType.Magical);
                    if (t.IsValidTarget())
                        R.Cast(t, true, true);
                }
                if (Config.Item("Rjungle", true).GetValue<bool>())
                {
                    KsJungle();
                }
            }
            else
                DragonTime = 0;
        }

        private void KsJungle()
        {
            var mobs = Cache.GetMinions(Player.ServerPosition, float.MaxValue, MinionTeam.Neutral);
            foreach (var mob in mobs)
            {
                if (mob.Health == mob.MaxHealth)
                    continue;
                if (((mob.BaseSkinName.ToLower().Contains("dragon") && Config.Item("Rdragon", true).GetValue<bool>())
                    || (mob.BaseSkinName == "SRU_Baron" && Config.Item("Rbaron", true).GetValue<bool>())
                    || (mob.BaseSkinName == "SRU_Red" && Config.Item("Rred", true).GetValue<bool>())
                    || (mob.BaseSkinName == "SRU_Blue" && Config.Item("Rblue", true).GetValue<bool>()))
                    && (mob.CountAlliesInRange(1000) == 0 || Config.Item("Rally", true).GetValue<bool>())
                    && mob.Distance(Player.Position) > 1000
                    )
                {
                    if (DragonDmg == 0)
                        DragonDmg = mob.Health;

                    if (Game.Time - DragonTime > 3)
                    {
                        if (DragonDmg - mob.Health > 0)
                        {
                            DragonDmg = mob.Health;
                        }
                        DragonTime = Game.Time;
                    }
                    else
                    {
                        var DmgSec = (DragonDmg - mob.Health) * (Math.Abs(DragonTime - Game.Time) / 3);
                        //Program.debug("DS  " + DmgSec);
                        if (DragonDmg - mob.Health > 0)
                        {

                            var timeTravel = GetUltTravelTime(Player, R.Speed, R.Delay, mob.Position);
                            var timeR = (mob.Health - R.GetDamage(mob)) / (DmgSec / 3);
                            //Program.debug("timeTravel " + timeTravel + "timeR " + timeR + "d " + R.GetDamage(mob));
                            if (timeTravel > timeR)
                                R.Cast(mob.Position);
                        }
                        else
                            DragonDmg = mob.Health;
                        //Program.debug("" + GetUltTravelTime(ObjectManager.Player, R.Speed, R.Delay, mob.Position));
                    }
                }
            }
        }
        private float GetUltTravelTime(AIHeroClient source, float speed, float delay, Vector3 targetpos)
        {
            float distance = Vector3.Distance(source.ServerPosition, targetpos);
            float missilespeed = speed;

            return (distance / missilespeed + delay);
        }


        private void Jungle()
        {
            if (Program.LaneClear && Player.Mana > RMANA + QMANA)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, 650, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];
                    if (Q1.IsReady() && Config.Item("jungleQ", true).GetValue<bool>())
                    {
                        Q1.Cast(mob.ServerPosition);
                        return;
                    }
                    else if (E.IsReady() && Config.Item("jungleE", true).GetValue<bool>())
                    {
                        E.Cast(mob.ServerPosition);
                        return;
                    }
                    else if (W.IsReady() && Config.Item("jungleW", true).GetValue<bool>())
                    {
                        W.Cast();
                        return;
                    }
                }
            }
        }

        private void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q3.Range, TargetSelector.DamageType.Magical);
            if (t.IsValidTarget())
            {
                if (Program.Combo && Player.Mana > RMANA + QMANA + EMANA)
                    CastQ(t);
                else if (Program.Harass && Config.Item("harassQ", true).GetValue<bool>() && Config.Item("Harass" + t.ChampionName).GetValue<bool>() && Player.ManaPercent > Config.Item("QHarassMana", true).GetValue<Slider>().Value && OktwCommon.CanHarras())
                    CastQ(t);
                else if (OktwCommon.GetKsDamage(t, Q1) > t.Health)
                    CastQ(t);
                else if (Player.Mana > RMANA + QMANA)
                {
                    foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(Q3.Range) && !OktwCommon.CanMove(enemy)))
                        CastQ(t);
                }
            }

            if (ObjectManager.Player.Spellbook.IsAutoAttacking)
                return;

            if (!Program.None && !Program.Combo)
            {
                var allMinions = Cache.GetMinions(Player.ServerPosition, Q1.Range);

                if (Config.Item("farmQout", true).GetValue<bool>() && Player.Mana > RMANA + QMANA + EMANA + WMANA)
                {
                    foreach (var minion in allMinions.Where(minion => minion.IsValidTarget(Q1.Range) && (!Orbwalker.InAutoAttackRange(minion) || (!minion.UnderTurret(true) && minion.UnderTurret()))))
                    {
                        var hpPred = SebbyLib.HealthPrediction.GetHealthPrediction(minion, 600);
                        if (hpPred < Q1.GetDamage(minion) && hpPred > minion.Health - hpPred * 2)
                        {
                            Q1.Cast(minion);
                            return;
                        }
                    }
                }
                if (FarmSpells && Config.Item("farmQ", true).GetValue<bool>())
                {
                    var farmPos = Q1.GetCircularFarmLocation(allMinions, Q1.Width);
                    if (farmPos.MinionsHit >= FarmMinions)
                        Q1.Cast(farmPos.Position);
                }
            }
        }
        private void LogicW()
        {
            if (!W.Instance.Name.Contains("oggle"))
            {
                if (Config.Item("useW", true).GetValue<KeyBind>().Active)
                    W.Cast(Player.Position.Extend(Game.CursorPos, -100));

                    var t = TargetSelector.GetTarget(W.Range - 250, TargetSelector.DamageType.Magical);
                if (t.IsValidTarget())
                {
                    var close = HeroManager.Enemies.FirstOrDefault(x => x.IsMelee && x.IsValidTarget(350) && x.IsFacing(Player));
                    if (close != null)
                    {
                        W.Cast(Prediction.GetPrediction(Player, 0.2f).CastPosition.Extend(Prediction.GetPrediction(close, 0.2f).CastPosition, 50));
                    }
                    var pred = Prediction.GetPrediction(t, 0.3f);

                    var castPos = pred.CastPosition;
                    var tP = Player.Distance(t);
                    var tC = Player.Distance(castPos);
                    if (Program.Combo && Player.Mana > RMANA + WMANA)
                    {
                        if (tP < tC && tP > 500)
                        {
                            Console.WriteLine("pull");
                            W.Cast(Player.Position.Extend(castPos, tP + 250));
                        }
                        else if (tP > tC && tP < 500)
                        {
                            Console.WriteLine("push");
                            W.Cast(Player.Position.Extend(castPos, tP - 250));
                        }
                    }
                }

                if (Config.Item("turretW", true).GetValue<bool>())
                {
                    var turret = Cache.TurretList.FirstOrDefault(x => x.IsValidTarget(W.Range) && x.HealthPercent < 22.5 + W.Level * 2.5 );
                    if (turret != null)
                        W.Cast(turret);     
                }
            }
            else
            {
                W.Cast();
            }
        }

        private void LogicE()
        {
            if (Player.Mana > RMANA + EMANA && Config.Item("autoE", true).GetValue<bool>() )
            {
                var close = HeroManager.Enemies.FirstOrDefault(x => x.IsMelee && x.IsValidTarget(400) && x.IsFacing(Player));
                if (close != null)
                {
                    E.Cast(close);
                }

                foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(E.Range + 50) && !OktwCommon.CanMove(enemy)))
                {
                    E.Cast(enemy);
                    return;
                }

                if (Config.Item("telE", true).GetValue<bool>())
                {
                    var trapPos = OktwCommon.GetTrapPos(E.Range);
                    if (!trapPos.IsZero)
                        E.Cast(trapPos);
                }

                if (Program.Combo && Player.IsMoving && Config.Item("comboE", true).GetValue<bool>() && Player.Mana > RMANA + EMANA + WMANA)
                {
                    var t = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
                    if (t.IsValidTarget(E.Range))
                    {
                        E.CastIfWillHit(t, 2);
                        if (t.HasBuffOfType(BuffType.Slow))
                            Program.CastSpell(E, t);
                        
                        if (OktwCommon.IsMovingInSameDirection(Player, t))
                            Program.CastSpell(E, t);
                    }
                }
                else if (FarmSpells && Config.Item("farmE", true).GetValue<bool>())
                {
                    var allMinions = Cache.GetMinions(Player.ServerPosition, E.Range);
                    var farmPos = E.GetCircularFarmLocation(allMinions, E.Width);
                    if (farmPos.MinionsHit >= FarmMinions + 1)
                        E.Cast(farmPos.Position);
                }
            }
        }

        private void LogicR()
        {

            if (Config.Item("autoR", true).GetValue<bool>())
            {
                R.Range = Config.Item("MaxRangeR", true).GetValue<Slider>().Value;
                var minHpCombo = Config.Item("minHpSpecial", true).GetValue<Slider>().Value;
                foreach (var target in HeroManager.Enemies.Where(target => target.IsValidTarget(R.Range) && OktwCommon.ValidUlt(target)))
                {
                    double predictedHealth = target.Health - OktwCommon.GetIncomingDamage(target);
                    double Rdmg = R.GetDamage(target);

                    if (Rdmg > predictedHealth)
                    {
                        var incom = OktwCommon.GetIncomingDamage(Player);
                        if (incom > 0 && Player.Health - incom * Player.CountEnemiesInRange(500) < Player.Level * 12)
                            R.Cast(target,true,true);
                    }

                    Rdmg = Rdmg * 0.66;

                    if (Rdmg > predictedHealth && target.CountAlliesInRange(500) == 0 && Player.Distance(target) > Config.Item("MinRangeR", true).GetValue<Slider>().Value)
                        Program.CastSpell(R, target);

                    if (Program.Combo && target.HealthPercent < minHpCombo)
                    {
                        R.CastIfWillHit(target, Config.Item("Raoe", true).GetValue<Slider>().Value, true);

                        if (Config.Item("Rcc", true).GetValue<bool>() && !OktwCommon.CanMove(target))
                            R.Cast(target, true, true);
                    }
                }
            }
        }
        private void SetMana()
        {
            if ((Config.Item("manaDisable", true).GetValue<bool>() && Program.Combo) || Player.HealthPercent < 20)
            {
                QMANA = 0;
                WMANA = 0;
                EMANA = 0;
                RMANA = 0;
                return;
            }

            QMANA = Q1.Instance.SData.Mana;
            WMANA = W.Instance.SData.Mana;
            EMANA = E.Instance.SData.Mana;

            if (!R.IsReady())
                RMANA = QMANA - Player.PARRegenRate * Q1.Instance.Cooldown;
            else
                RMANA = R.Instance.SData.Mana;
        }
        private static void CastQ(Obj_AI_Base target)
        {
            SebbyLib.Prediction.PredictionOutput prediction = GetPred(Q1,target);

            Vector3 pos1 = Vector3.Zero, pos2 = Vector3.Zero, pos3 = Vector3.Zero;

            SebbyLib.Prediction.HitChance hitchance = SebbyLib.Prediction.HitChance.Low;

            if (Config.Item("QHitChance", true).GetValue<StringList>().SelectedIndex == 0)
                hitchance = SebbyLib.Prediction.HitChance.VeryHigh;
            else if (Config.Item("QHitChance", true).GetValue<StringList>().SelectedIndex == 1)
                hitchance = SebbyLib.Prediction.HitChance.High;
            else if (Config.Item("QHitChance", true).GetValue<StringList>().SelectedIndex == 2)
                hitchance = SebbyLib.Prediction.HitChance.Medium;
            
            if (prediction.Hitchance == SebbyLib.Prediction.HitChance.OutOfRange)
            {
                prediction = GetPred(Q2, target);
                pos1 = Player.Position.Extend(prediction.CastPosition, Q1.Range);
                if (Cache.GetMinions(pos1, 280).Any())
                    return;
                if (OktwCommon.CirclePoints(10, 150, pos1).Any(x => x.IsWall()))
                    return;
                if (prediction.Hitchance == SebbyLib.Prediction.HitChance.OutOfRange)
                {
                    prediction = GetPred(Q3, target);
                    pos2 = Player.Position.Extend(prediction.CastPosition, Q2.Range);
                    if (Cache.GetMinions(pos2, 280).Any())
                        return;
                    if (OktwCommon.CirclePoints(10, 150, pos2).Any(x => x.IsWall()))
                        return;
                    if (prediction.Hitchance == SebbyLib.Prediction.HitChance.OutOfRange)
                    {
                        return;
                    }
                    else if (prediction.Hitchance >= hitchance )
                    {
                        Q3.Cast(prediction.CastPosition);
                    }
                }
                else if (prediction.Hitchance >= hitchance )
                {
                    Q2.Cast(prediction.CastPosition);
                }
            }
            else if (prediction.Hitchance >= hitchance)
            {
                Q1.Cast(prediction.CastPosition);
            }

        }

        private static SebbyLib.Prediction.PredictionOutput GetPred(Spell QWER , Obj_AI_Base target)
        {
           
            SebbyLib.Prediction.SkillshotType CoreType2 = SebbyLib.Prediction.SkillshotType.SkillshotLine;
            bool aoe2 = false;

            if (QWER.Type == SkillshotType.SkillshotCircle)
            {
                CoreType2 = SebbyLib.Prediction.SkillshotType.SkillshotCircle;
                aoe2 = true;
            }

            if (QWER.Width > 80 && !QWER.Collision)
                aoe2 = true;

            var predInput2 = new SebbyLib.Prediction.PredictionInput
            {
                Aoe = aoe2,
                Collision = QWER.Collision,
                Speed = QWER.Speed,
                Delay = QWER.Delay,
                Range = QWER.Range,
                From = Player.ServerPosition,
                Radius = QWER.Width,
                Unit = target,
                Type = CoreType2
            };
            var poutput2 = SebbyLib.Prediction.Prediction.GetPrediction(predInput2);

            //var poutput2 = QWER.GetPrediction(target);

            if (QWER.Speed != float.MaxValue && OktwCommon.CollisionYasuo(Player.ServerPosition, poutput2.CastPosition))
                return null;

            return poutput2;
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (Config.Item("qRange", true).GetValue<bool>())
            {
                if (Config.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (Q3.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(Player.Position, Q3.Range, System.Drawing.Color.Cyan, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(Player.Position, Q3.Range, System.Drawing.Color.Cyan, 1, 1);
            }
            if (Config.Item("wRange", true).GetValue<bool>())
            {
                if (Config.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (W.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.Orange, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.Orange, 1, 1);
            }
            if (Config.Item("eRange", true).GetValue<bool>())
            {
                if (Config.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (E.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(Player.Position, E.Range, System.Drawing.Color.Yellow, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(Player.Position, E.Range, System.Drawing.Color.Yellow, 1, 1);
            }
            if (Config.Item("rRange", true).GetValue<bool>())
            {
                if (Config.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (R.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(Player.Position, R.Range, System.Drawing.Color.Gray, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(Player.Position, R.Range, System.Drawing.Color.Gray, 1, 1);
            }
        }
    }
}
