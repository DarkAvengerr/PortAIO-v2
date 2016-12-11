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
        public Ziggs()
        {
            Q1 = new Spell(SpellSlot.Q, 850f);
            Q2 = new Spell(SpellSlot.Q, 1115f);
            Q3 = new Spell(SpellSlot.Q, 1390f);

            W = new Spell(SpellSlot.W, 1000f);
            E = new Spell(SpellSlot.E, 900f);
            R = new Spell(SpellSlot.R, 5300f);

            Q1.SetSkillshot(0.25f, 100f, 1700f, false, SkillshotType.SkillshotCircle);
            Q2.SetSkillshot(0.4f, 50f, 1650f, false, SkillshotType.SkillshotCircle);
            Q3.SetSkillshot(0.6f, 50f, 1650f, false, SkillshotType.SkillshotCircle);

            W.SetSkillshot(0.25f, 275f, 1750f, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(1f, 150f, 1750f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.7f, 350f , 1500f, false, SkillshotType.SkillshotCircle);

            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("autoQ", "Auto Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("harassQ", "Harass Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("QHarassMana", "Harass Mana", true).SetValue(new Slider(30, 100, 0)));

            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("autoW", "Auto W", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("harassW", "Harass W", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("interupterW", "Interrupter W", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("autoE", "Auto E on CC", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("comboE", "Auto E in Combo BETA", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("AGC", "Anti Gapcloser E", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("opsE", "OnProcessSpellCastE", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("telE", "Auto E teleport", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("farmQout", "Last hit Q minion out range AA", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("farmQ", "Lane clear Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("farmE", "Lane clear E", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("farmW", "Lane clear W", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("jungleE", "Jungle clear E", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("jungleQ", "Jungle clear Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("jungleW", "Jungle clear W", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("autoR", "Auto R", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("Rcc", "R cc", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("Raoe", "R AOE", true).SetValue(new Slider(3, 5, 0)));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").SubMenu("R Jungle stealer").AddItem(new MenuItem("Rjungle", "R Jungle stealer", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").SubMenu("R Jungle stealer").AddItem(new MenuItem("Rdragon", "Dragon", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").SubMenu("R Jungle stealer").AddItem(new MenuItem("Rbaron", "Baron", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").SubMenu("R Jungle stealer").AddItem(new MenuItem("Rred", "Red", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").SubMenu("R Jungle stealer").AddItem(new MenuItem("Rblue", "Blue", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").SubMenu("R Jungle stealer").AddItem(new MenuItem("Rally", "Ally stealer", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("useR", "Semi-manual cast R key", true).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press))); //32 == space
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("Rturrent", "Don't R under turret", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("MaxRangeR", "Max R range", true).SetValue(new Slider(3000, 5000, 0)));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("MinRangeR", "Min R range", true).SetValue(new Slider(900, 5000, 0)));

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

        private void Drawing_OnDraw(EventArgs args)
        {
            //Ziggs_Base_W_Countdown
        }

        private void Game_OnGameUpdate(EventArgs args)
        {

            if (Program.LagFree(0))
            {
                SetMana();
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
                var t = TargetSelector.GetTarget(W.Range - 250, TargetSelector.DamageType.Magical);
                if (t.IsValidTarget())
                {
                    var close = HeroManager.Enemies.FirstOrDefault(x => x.IsMelee && x.IsValidTarget(350) && x.IsFacing(Player));
                    if(close != null)
                    {
                        W.Cast(Prediction.GetPrediction(Player, 0.2f).CastPosition.Extend(Prediction.GetPrediction(close,0.2f).CastPosition, 50));
                    }
                    var pred = Prediction.GetPrediction(t,0.3f);

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
                        {
                            Program.CastSpell(E, t);
                        }
                        if (OktwCommon.IsMovingInSameDirection(Player, t))
                            Program.CastSpell(E, t);
                    }
                }
            }
            
        }


        private void LogicR()
        {

            if (Config.Item("autoR", true).GetValue<bool>() && Player.CountEnemiesInRange(800) == 0 )
            {
                R.Range = Config.Item("MaxRangeR", true).GetValue<Slider>().Value;
                foreach (var target in HeroManager.Enemies.Where(target => target.IsValidTarget(R.Range) && OktwCommon.ValidUlt(target)))
                {
                    double predictedHealth = target.Health - OktwCommon.GetIncomingDamage(target);
                    double Rdmg = R.GetDamage(target);

                    if (Rdmg > predictedHealth)
                    {
                        var incom = OktwCommon.GetIncomingDamage(Player);
                        if (incom > 0 && Player.Health - incom < Player.Level * 12)
                            R.Cast(target);
                    }

                    if (Config.Item("Rcc", true).GetValue<bool>()&& Rdmg > predictedHealth && !OktwCommon.CanMove(target))
                    {
                        R.Cast(target);
                    }
                    Rdmg = Rdmg * 0.66;


                    if (Rdmg > predictedHealth && target.CountAlliesInRange(500) == 0 && Player.Distance(target) > Config.Item("MinRangeR", true).GetValue<Slider>().Value)
                    {
                        Program.CastSpell(R, target);
                        Program.debug("R normal");
                    }
                    if (Program.Combo )
                    {
                        R.CastIfWillHit(target, Config.Item("Raoe", true).GetValue<Slider>().Value, true);
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
    }
}
