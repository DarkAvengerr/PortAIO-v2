using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;
using EloBuddy;

namespace OneKeyToWin_AIO_Sebby
{
    class Orianna
    {
        private Menu Config = Program.Config;
        public static SebbyLib.Orbwalking.Orbwalker Orbwalker = Program.Orbwalker;
        private Spell E, Q, R, W, QR;
        private float QMANA = 0, WMANA = 0, EMANA = 0, RMANA = 0;
        private AIHeroClient Player { get { return ObjectManager.Player; } }

        private float RCastTime = 0;
        private Vector3 BallPos;
        private int FarmId;
        private bool Rsmart = false;
        private AIHeroClient best;

        public void LoadOKTW()
        {
            Q = new Spell(SpellSlot.Q, 800);
            W = new Spell(SpellSlot.W, 210);
            E = new Spell(SpellSlot.E, 1095);
            R = new Spell(SpellSlot.R, 360);
            QR = new Spell(SpellSlot.Q, 825);

            Q.SetSkillshot(0.05f, 70f, 1150f, false, SkillshotType.SkillshotCircle);
            W.SetSkillshot(0.25f, 210f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.25f, 100f, 1700f, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.4f, 370f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            QR.SetSkillshot(0.5f, 400f, 100f, false, SkillshotType.SkillshotCircle);

            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("qRange", "Q range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("wRange", "W range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("eRange", "E range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("rRange", "R range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("onlyRdy", "Draw only ready spells", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("E Shield Config").AddItem(new MenuItem("autoW", "Auto E", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("E Shield Config").AddItem(new MenuItem("hadrCC", "Auto E hard CC", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("E Shield Config").AddItem(new MenuItem("poison", "Auto E poison", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("E Shield Config").AddItem(new MenuItem("Wdmg", "E dmg % hp", true).SetValue(new Slider(10, 100, 0)));
            Config.SubMenu(Player.ChampionName).SubMenu("E Shield Config").AddItem(new MenuItem("AGC", "AntiGapcloserE", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("farmQout", "Farm Q out range aa minion", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("Mana", "LaneClear Mana", true).SetValue(new Slider(60, 100, 0)));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("LCminions", "LaneClear minimum minions", true).SetValue(new Slider(2, 10, 0)));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("farmQ", "LaneClear Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("farmW", "LaneClear W", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("farmE", "LaneClear E", true).SetValue(false));

            Config.SubMenu(Player.ChampionName).SubMenu("R config").AddItem(new MenuItem("rCount", "Auto R x enemies", true).SetValue(new Slider(3, 0, 5)));
            Config.SubMenu(Player.ChampionName).SubMenu("R config").AddItem(new MenuItem("smartR", "Semi-manual cast R key", true).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
            Config.SubMenu(Player.ChampionName).SubMenu("R config").AddItem(new MenuItem("OPTI", "OnPossibleToInterrupt R", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R config").AddItem(new MenuItem("Rturrent", "auto R under turrent", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R config").AddItem(new MenuItem("Rks", "R ks", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R config").AddItem(new MenuItem("Rlifesaver", "auto R life saver", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R config").AddItem(new MenuItem("Rblock", "Block R if 0 hit ", true).SetValue(true));
            foreach (var enemy in HeroManager.Enemies)
                Config.SubMenu(Player.ChampionName).SubMenu("R config").SubMenu("Always R").AddItem(new MenuItem("Ralways" + enemy.ChampionName, enemy.ChampionName,true).SetValue(false));

            Config.SubMenu(Player.ChampionName).AddItem(new MenuItem("W", "Auto W SpeedUp logic", true).SetValue(false));
            Game.OnUpdate += Game_OnGameUpdate;
            GameObject.OnCreate += Obj_AI_Base_OnCreate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget +=Interrupter2_OnInterruptableTarget;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!Config.Item("OPTI", true).GetValue<bool>())
                return;
            if (R.LSIsReady() && sender.LSDistance(BallPos) < R.Range)
            {
                R.Cast();
                Program.debug("interupt");
            }
            else if (Q.LSIsReady() && Player.Mana > RMANA + QMANA && sender.LSIsValidTarget(Q.Range))
                Q.Cast(sender.ServerPosition);
        }


        private void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.R && Config.Item("Rblock", true).GetValue<bool>() &&  CountEnemiesInRangeDeley(BallPos, R.Width, R.Delay) == 0)
                args.Process = false;
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var Target = gapcloser.Sender;
            if (Config.Item("AGC", true).GetValue<bool>() && E.LSIsReady() && Target.LSIsValidTarget(800) && Player.Mana > RMANA + EMANA)
                E.CastOnUnit(Player);
            return;
        }
        
        private void Game_OnGameUpdate(EventArgs args)
        {
            //Program.debug(""+BallPos.LSDistance(Player.Position));
            if (Player.HasBuff("Recall") || Player.IsDead)
                return;

            if (R.LSIsReady())
                LogicR();

            bool hadrCC = true, poison = true;
            if (Program.LagFree(0))
            {
                SetMana();
                hadrCC = Config.Item("hadrCC", true).GetValue<bool>();
                poison = Config.Item("poison", true).GetValue<bool>();
            }

            best = Player;

            foreach (var ally in HeroManager.Allies.Where(ally => ally.IsValid && !ally.IsDead))
            {
                if (ally.HasBuff("orianaghostself") || ally.HasBuff("orianaghost"))
                    BallPos = ally.ServerPosition;

                if (Program.LagFree(3) )
                {
                    if (E.LSIsReady() && Player.Mana > RMANA + EMANA && ally.LSDistance(Player.Position) < E.Range)
                    {
                        var countEnemy = ally.LSCountEnemiesInRange(800);
                        if (ally.Health < countEnemy * ally.Level * 25)
                        {
                            E.CastOnUnit(ally);
                        }
                        else if (HardCC(ally) && hadrCC && countEnemy > 0)
                        {
                            E.CastOnUnit(ally);
                        }
                        else if (ally.HasBuffOfType(BuffType.Poison))
                        {
                            E.CastOnUnit(ally);
                        }
                    }
                    if (W.LSIsReady() && Player.Mana > RMANA + WMANA && BallPos.LSDistance(ally.ServerPosition) < 240 && ally.Health < ally.LSCountEnemiesInRange(600) * ally.Level * 20)
                        W.Cast();

                    if ((ally.Health < best.Health || ally.LSCountEnemiesInRange(300) > 0) && ally.LSDistance(Player.Position) < E.Range && ally.LSCountEnemiesInRange(700) > 0)
                        best = ally;
                }
                if (Program.LagFree(1) && E.LSIsReady() && Player.Mana > RMANA + EMANA && ally.LSDistance(Player.Position) < E.Range && ally.LSCountEnemiesInRange(R.Width) >= Config.Item("rCount", true).GetValue<Slider>().Value && 0 != Config.Item("rCount", true).GetValue<Slider>().Value)
                {
                    E.CastOnUnit(ally);
                }
            }
            /*
            foreach (var ally in HeroManager.Allies.Where(ally => ally.IsValid && ally.LSDistance(Player.Position) < 1000))
            {
                foreach (var buff in ally.Buffs)
                {
                        Program.debug(buff.Name);
                }

            }
            */
            if ((Config.Item("smartR", true).GetValue<KeyBind>().Active || Rsmart) && R.LSIsReady())
            {
                Rsmart = true;
                var target = TargetSelector.GetTarget(Q.Range + 100, TargetSelector.DamageType.Magical);
                if (target.LSIsValidTarget())
                {
                    if (CountEnemiesInRangeDeley(BallPos, R.Width, R.Delay) > 1)
                        R.Cast();
                    else if (Q.LSIsReady())
                        QR.Cast(target, true, true);
                    else if (CountEnemiesInRangeDeley(BallPos, R.Width, R.Delay) > 0)
                        R.Cast();
                }
                else
                    Rsmart = false;
            }
            else
                Rsmart = false;

            if (Program.LagFree(1))
            {
                LogicQ();
                LogicFarm();
            }

            if (Program.LagFree(2) && W.LSIsReady() )
                LogicW();

            if (Program.LagFree(4) && E.LSIsReady())
                LogicE(best); 
        }

        private void LogicE(AIHeroClient best)
        {
            var ta = TargetSelector.GetTarget(1300, TargetSelector.DamageType.Magical);

            if (Program.Combo && ta.LSIsValidTarget() && !W.LSIsReady() && Player.Mana > RMANA + EMANA)
            {
                if (CountEnemiesInRangeDeley(BallPos, 100, 0.1f) > 0)
                    E.CastOnUnit(best);
                var castArea = ta.LSDistance(best.ServerPosition) * (best.ServerPosition - ta.ServerPosition).LSNormalized() + ta.ServerPosition;
                if (castArea.LSDistance(ta.ServerPosition) < ta.BoundingRadius / 2)
                    E.CastOnUnit(best);
            }
        }

        private void LogicR()
        {            
            foreach (var t in HeroManager.Enemies.Where(t => t.LSIsValidTarget() && BallPos.LSDistance(Prediction.GetPrediction(t, R.Delay).CastPosition) < R.Width && BallPos.LSDistance(t.ServerPosition) < R.Width))
            {
                if (Program.Combo && Config.Item("Ralways" + t.ChampionName, true).GetValue<bool>())
                {
                    R.Cast();
                }

                if (Config.Item("Rks", true).GetValue<bool>())
                {
                    var comboDmg = OktwCommon.GetKsDamage(t, R);

                    if (t.LSIsValidTarget(Q.Range))
                        comboDmg += Q.GetDamage(t);
                    if (W.LSIsReady())
                        comboDmg += W.GetDamage(t);
                    if (Orbwalker.InAutoAttackRange(t))
                        comboDmg += (float)Player.LSGetAutoAttackDamage(t) * 2;
                    if (t.Health < comboDmg)
                        R.Cast();
                    Program.debug("ks");
                }
                if (Config.Item("Rturrent", true).GetValue<bool>() && BallPos.LSUnderTurret(false) && !BallPos.LSUnderTurret(true))
                {
                    R.Cast();
                    Program.debug("Rturrent");
                }
                if (Config.Item("Rlifesaver", true).GetValue<bool>() && Player.Health < Player.LSCountEnemiesInRange(800) * Player.Level * 20 && Player.LSDistance(BallPos) > t.LSDistance(Player.Position))
                {
                    R.Cast();
                    Program.debug("ls");
                }
            }

            int countEnemies=CountEnemiesInRangeDeley(BallPos, R.Width, R.Delay);

            if (countEnemies >= Config.Item("rCount", true).GetValue<Slider>().Value && BallPos.LSCountEnemiesInRange(R.Width) == countEnemies)
                R.Cast();
        }

        private void LogicW()
        {
            foreach (var t in HeroManager.Enemies.Where(t => t.LSIsValidTarget() && BallPos.LSDistance(t.ServerPosition) < 250 && t.Health < W.GetDamage(t)))
            {
                W.Cast();
                return;
            }
            if (CountEnemiesInRangeDeley(BallPos, W.Width, 0f) > 0 && Player.Mana > RMANA + WMANA)
            {
                W.Cast();
                return;
            }
            if (Config.Item("W", true).GetValue<bool>() && !Program.Farm && !Program.Combo && ObjectManager.Player.Mana > Player.MaxMana * 0.95 && Player.HasBuff("orianaghostself"))
                W.Cast();
        }

        private void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (t.LSIsValidTarget() && Q.LSIsReady())
            {
                if (Q.GetDamage(t) + W.GetDamage(t) > t.Health)
                    CastQ(t);
                else if (Program.Combo && Player.Mana > RMANA + QMANA - 10)
                    CastQ(t);
                else if (Program.Farm && Player.Mana > RMANA + QMANA + WMANA + EMANA)
                    CastQ(t);
            }
            if (Config.Item("W", true).GetValue<bool>() && !t.LSIsValidTarget() && Program.Combo && Player.Mana > RMANA + 3 * QMANA + WMANA + EMANA + WMANA)
            {
                if (W.LSIsReady() && Player.HasBuff("orianaghostself"))
                {
                    W.Cast();
                }
                else if (E.LSIsReady() && !Player.HasBuff("orianaghostself"))
                {
                    E.CastOnUnit(Player);
                }
            }
        }

        private void LogicFarm()
        {
            if (!Program.Farm)
                return;

            var allMinions = Cache.GetMinions(Player.ServerPosition, Q.Range);
            if (Config.Item("farmQout", true).GetValue<bool>() && Player.Mana > RMANA + QMANA + WMANA + EMANA)
            {
                foreach (var minion in allMinions.Where(minion => minion.LSIsValidTarget(Q.Range) && !Orbwalker.InAutoAttackRange(minion) && minion.Health < Q.GetDamage(minion) && minion.Health > minion.FlatPhysicalDamageMod))
                {
                    Q.Cast(minion);
                }
            }

            if (!Program.LaneClear || Player.Mana < RMANA + QMANA)
                return;

            var mobs = Cache.GetMinions(Player.ServerPosition, 800, MinionTeam.Neutral);
            if (mobs.Count > 0)
            {
                var mob = mobs[0];
                if (Q.LSIsReady())
                    Q.Cast(mob.Position);
                if (W.LSIsReady() && BallPos.LSDistance(mob.Position) < W.Width)
                    W.Cast();
                else if (E.LSIsReady())
                    E.CastOnUnit(best);
                return;
            }
            

            if ((Player.ManaPercent > Config.Item("Mana", true).GetValue<Slider>().Value || (Player.LSUnderTurret(false) && !Player.LSUnderTurret(true))))
            {
                var Qfarm = Q.GetCircularFarmLocation(allMinions, 100);
                var QWfarm = Q.GetCircularFarmLocation(allMinions, W.Width);

                if (Qfarm.MinionsHit + QWfarm.MinionsHit == 0)
                    return;
                if (Config.Item("farmQ", true).GetValue<bool>())
                {
                    if (Qfarm.MinionsHit > Config.Item("LCminions", true).GetValue<Slider>().Value && !W.LSIsReady() && Q.LSIsReady())
                    {
                        Q.Cast(Qfarm.Position);
                    }
                    else if (QWfarm.MinionsHit > 2 && Q.LSIsReady())
                        Q.Cast(QWfarm.Position);
                }

                foreach (var minion in allMinions)
                {
                    if (W.LSIsReady() && minion.LSDistance(BallPos) < W.Range && minion.Health < W.GetDamage(minion) && Config.Item("farmW", true).GetValue<bool>())
                        W.Cast();
                    if (!W.LSIsReady() && E.LSIsReady() && minion.LSDistance(BallPos) < E.Width && Config.Item("farmE", true).GetValue<bool>())
                        E.CastOnUnit(Player);
                }
            }
        }

        private void CastQ(AIHeroClient target)
        {
            float distance = Vector3.Distance(BallPos, target.ServerPosition);
            

            if (E.LSIsReady() && Player.Mana > RMANA + QMANA + WMANA + EMANA && distance > Player.LSDistance(target.ServerPosition) + 300)
            {
                E.CastOnUnit(Player);
                return;
            }

            if (Config.Item("PredictionMODE", true).GetValue<StringList>().SelectedIndex == 1)
            {
                //var prepos5 = Core.Prediction.GetPrediction(target, delay, Q.Width);

                var predInput2 = new SebbyLib.Prediction.PredictionInput
                {
                    Aoe = true,
                    Collision = Q.Collision,
                    Speed = Q.Speed,
                    Delay = Q.Delay,
                    Range = float.MaxValue,
                    From = BallPos,
                    Radius = Q.Width,
                    Unit = target,
                    Type = SebbyLib.Prediction.SkillshotType.SkillshotCircle
                };
                var prepos5 = SebbyLib.Prediction.Prediction.GetPrediction(predInput2);

                if ((int)prepos5.Hitchance > 5 - Config.Item("HitChance", true).GetValue<StringList>().SelectedIndex)
                {
                    if (prepos5.CastPosition.LSDistance(prepos5.CastPosition) < Q.Range)
                    {
                        Q.Cast(prepos5.CastPosition);
                    }
                }
            }
            else
            {
                float delay = (distance / Q.Speed + Q.Delay);
                var prepos = Prediction.GetPrediction(target, delay, Q.Width);

                if ((int)prepos.Hitchance > 5 - Config.Item("HitChance", true).GetValue<StringList>().SelectedIndex)
                {
                    if (prepos.CastPosition.LSDistance(prepos.CastPosition) < Q.Range)
                    {
                        Q.Cast(prepos.CastPosition);
                    }
                }
            }
        }

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {

            if (sender.IsMe && args.SData.Name == "OrianaIzunaCommand")
                BallPos = args.End;

             if (!E.LSIsReady() || !sender.IsEnemy || !Config.Item("autoW", true).GetValue<bool>() || Player.Mana < EMANA + RMANA || sender.LSDistance(Player.Position) > 1600)
                return;

            foreach (var ally in HeroManager.Allies.Where(ally => ally.IsValid && !ally.IsDead && Player.LSDistance(ally.ServerPosition) < E.Range))
            {
                double dmg = 0;
                if (args.Target != null && args.Target.NetworkId == ally.NetworkId)
                {
                    dmg = dmg + sender.LSGetSpellDamage(ally, args.SData.Name);
                }
                else
                {
                    var castArea = ally.LSDistance(args.End) * (args.End - ally.ServerPosition).LSNormalized() + ally.ServerPosition;
                    if (castArea.LSDistance(ally.ServerPosition) < ally.BoundingRadius / 2)
                        dmg = dmg + sender.LSGetSpellDamage(ally, args.SData.Name);
                    else
                        continue;
                }

                double HpLeft = ally.Health - dmg;
                double HpPercentage = (dmg * 100) / ally.Health;
                double shieldValue = 60 + E.Level * 40 + 0.4 * Player.FlatMagicDamageMod;

                if (HpPercentage >= Config.Item("Wdmg", true).GetValue<Slider>().Value || dmg > shieldValue)
                    E.CastOnUnit(ally);
            }   
        }

        private int CountEnemiesInRangeDeley(Vector3 position, float range, float delay)
        {
            int count = 0;
            foreach (var t in HeroManager.Enemies.Where(t => t.LSIsValidTarget()))
            {
                Vector3 prepos = Prediction.GetPrediction(t, delay).CastPosition;
                if (position.LSDistance(prepos) < range)
                    count++;
            }
            return count;
        }

        private void Obj_AI_Base_OnCreate(GameObject obj, EventArgs args)
        {
            if (obj.IsValid && obj.IsAlly && obj.Name == "TheDoomBall")
            {
                BallPos = obj.Position;
            }
        }

        private bool HardCC(AIHeroClient target)
        {
            if (target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Snare) || target.HasBuffOfType(BuffType.Knockup) ||
                target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Fear) || target.HasBuffOfType(BuffType.Knockback) ||
                target.HasBuffOfType(BuffType.Taunt) || target.HasBuffOfType(BuffType.Suppression) ||
                target.IsStunned )
            {
                return true;

            }
            else
                return false;
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

            QMANA = Q.Instance.SData.Mana;
            WMANA = W.Instance.SData.Mana;
            EMANA = E.Instance.SData.Mana;

            if (!R.LSIsReady())
                RMANA = QMANA - Player.PARRegenRate * Q.Instance.Cooldown;
            else
                RMANA = R.Instance.SData.Mana;
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (BallPos.LSIsValid())
            {
                if (Config.Item("wRange", true).GetValue<bool>())
                {
                    if (Config.Item("onlyRdy", true).GetValue<bool>())
                    {
                        if (W.LSIsReady())
                            LeagueSharp.Common.Utility.DrawCircle(BallPos, W.Range, System.Drawing.Color.Orange, 1, 1);
                    }
                    else
                        LeagueSharp.Common.Utility.DrawCircle(BallPos, W.Range, System.Drawing.Color.Orange, 1, 1);
                }

                if (Config.Item("rRange", true).GetValue<bool>())
                {
                    if (Config.Item("onlyRdy", true).GetValue<bool>())
                    {
                        if (R.LSIsReady())
                            LeagueSharp.Common.Utility.DrawCircle(BallPos, R.Range, System.Drawing.Color.Gray, 1, 1);
                    }
                    else
                        LeagueSharp.Common.Utility.DrawCircle(BallPos, R.Range, System.Drawing.Color.Gray, 1, 1);
                }
            }

            if (Config.Item("qRange", true).GetValue<bool>())
            {
                if (Config.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (Q.LSIsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
            }
            
            if (Config.Item("eRange", true).GetValue<bool>())
            {
                if (Config.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (E.LSIsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.Yellow, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.Yellow, 1, 1);
            }
        }
    }
}
