using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using System.IO;
using SharpDX;
using Collision = LeagueSharp.Common.Collision;
using EloBuddy; 
 using LeagueSharp.Common; 
 namespace GenerationJinx
{
    class Program
    {
        public const string ChampionName = "Jinx";

        //Orbwalker instance
        public static Orbwalking.Orbwalker Orbwalker;

        //Spells
        public static List<Spell> SpellList = new List<Spell>();
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static Spell R1;
        //ManaMenager
        public static float QMANA;
        public static float WMANA;
        public static float EMANA;
        public static float RMANA;
        public static bool attackNow = true;
        public static double lag = 0;
        public static double WCastTime = 0;
        public static double QCastTime = 0;
        public static float DragonDmg = 0;
        public static double DragonTime = 0;
        //AutoPotion
        public static Items.Item Potion = new Items.Item(2003, 0);
        public static Items.Item ManaPotion = new Items.Item(2004, 0);
        public static Items.Item Youmuu = new Items.Item(3142, 0);

        //Menu
        public static Menu Config;

        private static AIHeroClient Player;

        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            Player = ObjectManager.Player;
            if (Player.ChampionName != ChampionName) return;
            
            Q = new Spell(SpellSlot.Q, float.MaxValue);
            W = new Spell(SpellSlot.W, 1500f);
            E = new Spell(SpellSlot.E, 900f);
            R = new Spell(SpellSlot.R, 2500f);
            R1 = new Spell(SpellSlot.R, 2500f);
            
            W.SetSkillshot(0.6f, 75f, 3300f, true, SkillshotType.SkillshotLine);
            E.SetSkillshot(1.2f, 1f, 1750f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.7f, 140f, 1500f, false, SkillshotType.SkillshotLine);
            R1.SetSkillshot(0.7f, 200f, 1500f, false, SkillshotType.SkillshotCircle);

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);
            SpellList.Add(R1);
            Config = new Menu(ChampionName, ChampionName, true);
            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);
            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));
            Config.AddToMainMenu();
            #region E

            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != Player.Team))
                Config.SubMenu("W Config").SubMenu("Haras").AddItem(new MenuItem("haras" + enemy.ChampionName, enemy.ChampionName).SetValue(true));
            Config.SubMenu("W Config").AddItem(new MenuItem("humanzier", "Humanizer W (0.5 delay)").SetValue(false));
            #endregion
            #region E
            Config.SubMenu("E Config").AddItem(new MenuItem("autoE", "Auto E").SetValue(true));
            Config.SubMenu("E Config").AddItem(new MenuItem("comboE", "Auto E in Combo BETA").SetValue(true));
            Config.SubMenu("E Config").AddItem(new MenuItem("AGC", "AntiGapcloserE").SetValue(true));
            Config.SubMenu("E Config").AddItem(new MenuItem("opsE", "OnProcessSpellCastE").SetValue(true));
            Config.SubMenu("E Config").AddItem(new MenuItem("telE", "Auto E teleport").SetValue(true));
            #endregion
            #region R
            Config.SubMenu("R Config").AddItem(new MenuItem("autoR", "Auto R").SetValue(true));
            Config.SubMenu("R Config").SubMenu("R Jungle stealer").AddItem(new MenuItem("Rjungle", "R Jungle stealer").SetValue(true));
            Config.SubMenu("R Config").SubMenu("R Jungle stealer").AddItem(new MenuItem("Rdragon", "Dragon").SetValue(true));
            Config.SubMenu("R Config").SubMenu("R Jungle stealer").AddItem(new MenuItem("Rbaron", "Baron").SetValue(true));
            Config.SubMenu("R Config").AddItem(new MenuItem("hitchanceR", "VeryHighHitChanceR").SetValue(true));
            Config.SubMenu("R Config").AddItem(new MenuItem("useR", "Semi-manual cast R key").SetValue(new KeyBind('t', KeyBindType.Press))); //32 == space

            #endregion
            Config.SubMenu("Draw").SubMenu("Draw AAcirlce P00kz style").AddItem(new MenuItem("OrbDraw", "Draw AAcirlce P00kz style").SetValue(false));
            Config.SubMenu("Draw").SubMenu("Draw AAcirlce P00kz style").AddItem(new MenuItem("1", "pls disable Orbwalking > Drawing > AAcirlce"));
            Config.SubMenu("Draw").SubMenu("Draw AAcirlce P00kz style").AddItem(new MenuItem("2", "My HP: 0-30 red, 30-60 orange,60-100 green"));
            Config.SubMenu("Draw").AddItem(new MenuItem("noti", "Show notification").SetValue(false));
            Config.SubMenu("Draw").AddItem(new MenuItem("qRange", "Q range").SetValue(false));
            Config.SubMenu("Draw").AddItem(new MenuItem("wRange", "W range").SetValue(false));
            Config.SubMenu("Draw").AddItem(new MenuItem("eRange", "E range").SetValue(false));
            Config.SubMenu("Draw").AddItem(new MenuItem("rRange", "R range").SetValue(false));
            Config.SubMenu("Draw").AddItem(new MenuItem("onlyRdy", "Draw only ready spells").SetValue(true));
            Config.SubMenu("Draw").AddItem(new MenuItem("orb", "Orbwalker target P00kz style").SetValue(true));
            Config.SubMenu("Draw").AddItem(new MenuItem("semi", "Semi-manual R target").SetValue(false));

            Config.AddItem(new MenuItem("Hit", "Hit Chance SkillShot").SetValue(new Slider(3, 4, 0)));
            Config.AddItem(new MenuItem("farmQ", "Q farm").SetValue(true));
            Config.AddItem(new MenuItem("pots", "Use pots").SetValue(true));
            Config.AddItem(new MenuItem("debug", "Debug").SetValue(false));

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Orbwalking.BeforeAttack += BeforeAttack;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Chat.Print("<font color=\"#ff00d8\">J</font>inx full automatic AI ver beta 0.1 <font color=\"#000000\">by Khelia</font> - <font color=\"#00BFFF\">Loaded</font>");
            Chat.Print("<font color=\"#ff00d8\">THX to Sebastion1 for use some Parts of the Code</font> - <font color=\"#00BFFF\">Loaded</font>");
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (E.IsReady() && ObjectManager.Player.Mana > RMANA + EMANA && Config.Item("autoE").GetValue<bool>())
            {
                if (Config.Item("telE").GetValue<bool>())
                {
                    foreach (var Object in ObjectManager.Get<Obj_AI_Base>().Where(Obj => Obj.Distance(Player.ServerPosition) < E.Range && E.IsReady() && Obj.Team != Player.Team && (Obj.HasBuff("teleport_target", true) || Obj.HasBuff("Pantheon_GrandSkyfall_Jump", true))))
                    {
                        E.Cast(Object.Position, true);
                    }
                }
                foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsValidTarget(E.Range) && !enemy.HasBuff("rocketgrab2") && E.IsReady()))
                {
                    if (enemy.HasBuffOfType(BuffType.Stun) || enemy.HasBuffOfType(BuffType.Snare) ||
                         enemy.HasBuffOfType(BuffType.Charm) || enemy.HasBuffOfType(BuffType.Fear) ||
                         enemy.HasBuffOfType(BuffType.Taunt) || enemy.HasBuffOfType(BuffType.Suppression) ||
                         enemy.IsStunned || enemy.HasBuff("Recall"))
                        E.Cast(enemy, true);
                    else
                        E.CastIfHitchanceEquals(enemy, HitChance.Immobile, true);
                }

                var ta = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
                if (ObjectManager.Player.IsMoving && ta.IsValidTarget(E.Range) && E.GetPrediction(ta).CastPosition.Distance(ta.Position) > 200 && (int)E.GetPrediction(ta).Hitchance == 5 && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && E.IsReady() && Config.Item("comboE").GetValue<bool>() && ObjectManager.Player.Mana > RMANA + EMANA + WMANA && ta.Path.Count() == 1)
                {
                    if (ta.HasBuffOfType(BuffType.Slow))
                        E.CastIfHitchanceEquals(ta, HitChance.VeryHigh, true);
                    else if (ta.CountEnemiesInRange(250) > 2)
                        E.CastIfHitchanceEquals(ta, HitChance.VeryHigh, true);
                    else
                    {
                        if (ObjectManager.Player.Position.Distance(ta.ServerPosition) > ObjectManager.Player.Position.Distance(ta.Position))
                        {
                            if (ta.Position.Distance(ObjectManager.Player.ServerPosition) < ta.Position.Distance(ObjectManager.Player.Position))
                            {
                                CastSpell(E, ta, Config.Item("Hit").GetValue<Slider>().Value);
                                debug("E run");
                            }
                        }
                        else
                        {
                            if (ta.Position.Distance(ObjectManager.Player.ServerPosition) > ta.Position.Distance(ObjectManager.Player.Position))
                            {
                                CastSpell(E, ta, Config.Item("Hit").GetValue<Slider>().Value);
                                debug("E escape");
                            }
                        }
                    }
                }
            }

            if (Q.IsReady())
            {
                ManaMenager();
                if (Farm && Config.Item("farmQ").GetValue<bool>() && (Game.Time - lag > 0.1) && ObjectManager.Player.Mana > RMANA + WMANA + EMANA + 10 && !FishBoneActive)
                {
                    farmQ();
                    lag = Game.Time;
                }
                var t = TargetSelector.GetTarget(bonusRange() + 60, TargetSelector.DamageType.Physical);
                if (t.IsValidTarget())
                {
                    var distance = GetRealDistance(t);
                    var powPowRange = GetRealPowPowRange(t);
                    if (!FishBoneActive && !Orbwalking.InAutoAttackRange(t))
                    {
                        if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && (ObjectManager.Player.Mana > RMANA + WMANA + 20 || ObjectManager.Player.GetAutoAttackDamage(t) * 2 > t.Health))
                            Q.Cast();
                        else if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed && Orbwalker.GetTarget() == null && ObjectManager.Player.Mana > RMANA + WMANA + EMANA + 20 && distance < bonusRange() + t.BoundingRadius + ObjectManager.Player.BoundingRadius)
                            Q.Cast();
                        else if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && !ObjectManager.Player.UnderTurret(true) && ObjectManager.Player.Mana > RMANA + WMANA + EMANA + WMANA + 20 && distance < bonusRange())
                            Q.Cast();
                    }
                }
                else if (!FishBoneActive && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && ObjectManager.Player.Mana > RMANA + WMANA + 20 && ObjectManager.Player.CountEnemiesInRange(2000) > 0)
                    Q.Cast();
                else if (FishBoneActive && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && ObjectManager.Player.Mana < RMANA + WMANA + 20)
                    Q.Cast();
                else if (FishBoneActive && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && ObjectManager.Player.CountEnemiesInRange(2000) == 0)
                    Q.Cast();
                else if (FishBoneActive && Farm)
                    Q.Cast();
            }

            if (W.IsReady() && (Game.Time - QCastTime > 0.6))
            {
                bool cast = false;
                bool wait = false;

                foreach (var target in ObjectManager.Get<AIHeroClient>())
                {
                    if (target.IsValidTarget(W.Range) &&
                        !target.HasBuffOfType(BuffType.PhysicalImmunity) && !target.HasBuffOfType(BuffType.SpellImmunity) && !target.HasBuffOfType(BuffType.SpellShield))
                    {
                        float predictedHealth = HealthPrediction.GetHealthPrediction(target, (int)(W.Delay + (Player.Distance(target.ServerPosition) / W.Speed) * 1000));
                        var Wdmg = W.GetDamage(target);
                        if (Wdmg > predictedHealth)
                        {
                            cast = true;
                            wait = true;
                            PredictionOutput output = R.GetPrediction(target);
                            Vector2 direction = output.CastPosition.To2D() - Player.Position.To2D();
                            direction.Normalize();
                            List<AIHeroClient> enemies = ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy && x.IsValidTarget()).ToList();
                            foreach (var enemy in enemies)
                            {
                                if (enemy.BaseSkinName == target.BaseSkinName || !cast)
                                    continue;
                                PredictionOutput prediction = R.GetPrediction(enemy);
                                Vector3 predictedPosition = prediction.CastPosition;
                                Vector3 v = output.CastPosition - Player.ServerPosition;
                                Vector3 w = predictedPosition - Player.ServerPosition;
                                double c1 = Vector3.Dot(w, v);
                                double c2 = Vector3.Dot(v, v);
                                double b = c1 / c2;
                                Vector3 pb = Player.ServerPosition + ((float)b * v);
                                float length = Vector3.Distance(predictedPosition, pb);
                                if (length < (W.Width + enemy.BoundingRadius) && Player.Distance(predictedPosition) < Player.Distance(target.ServerPosition))
                                    cast = false;
                            }
                            if (!Orbwalking.InAutoAttackRange(target) && cast && target.IsValidTarget(W.Range) && ObjectManager.Player.CountEnemiesInRange(400) == 0 && target.Path.Count() < 2)
                            {
                                W.CastIfHitchanceEquals(target, HitChance.VeryHigh, true);
                                debug("W ks");
                            }
                        }
                    }
                }

                var t = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
                if (t.IsValidTarget() && !wait)
                {

                    if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && ObjectManager.Player.Mana > RMANA + WMANA + 10 && ObjectManager.Player.CountEnemiesInRange(GetRealPowPowRange(t)) == 0)
                    {
                        if (Config.Item("humanzier").GetValue<bool>())
                            LeagueSharp.Common.Utility.DelayAction.Add(500, () => CastSpell(W, t, Config.Item("Hit").GetValue<Slider>().Value));
                        else
                            CastSpell(W, t, Config.Item("Hit").GetValue<Slider>().Value);
                    }
                    else if ((Farm && ObjectManager.Player.Mana > RMANA + EMANA + WMANA + WMANA + 40) && Config.Item("haras" + t.CharData).GetValue<bool>() && !ObjectManager.Player.UnderTurret(true) && ObjectManager.Player.CountEnemiesInRange(bonusRange()) == 0)
                    {

                        if (Config.Item("humanzier").GetValue<bool>())
                            LeagueSharp.Common.Utility.DelayAction.Add(500, () => CastSpell(W, t, Config.Item("Hit").GetValue<Slider>().Value));
                        else
                            CastSpell(W, t, Config.Item("Hit").GetValue<Slider>().Value);
                    }
                    else if ((Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo || Farm) && ObjectManager.Player.Mana > RMANA + WMANA && ObjectManager.Player.CountEnemiesInRange(GetRealPowPowRange(t)) == 0)
                    {
                        foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsValidTarget(W.Range)))
                        {
                            if (enemy.HasBuffOfType(BuffType.Stun) || enemy.HasBuffOfType(BuffType.Snare) ||
                             enemy.HasBuffOfType(BuffType.Charm) || enemy.HasBuffOfType(BuffType.Fear) ||
                             enemy.HasBuffOfType(BuffType.Taunt) || enemy.HasBuffOfType(BuffType.Slow) || enemy.HasBuff("Recall"))
                            {
                                W.Cast(enemy, true);
                            }
                        }
                    }
                }
            }

            if (R.IsReady())
            {
                if (Config.Item("useR").GetValue<KeyBind>().Active)
                {
                    var t = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);
                    if (t.IsValidTarget())
                        R1.Cast(t, true, true);
                }
                if (Config.Item("Rjungle").GetValue<bool>())
                {
                    KsJungle();
                }
            }

            if (R.IsReady() && Config.Item("autoR").GetValue<bool>())
            {
                bool cast = false;
                foreach (var target in ObjectManager.Get<AIHeroClient>())
                {
                    if (target.IsValidTarget(R.Range) && (Game.Time - WCastTime > 0.8) && ValidUlt(target))
                    {
                        float predictedHealth = HealthPrediction.GetHealthPrediction(target, (int)(R.Delay + (Player.Distance(target.ServerPosition) / R.Speed) * 1000));
                        var Rdmg = R.GetDamage(target,1);
                        if (Rdmg > predictedHealth)
                        {
                            cast = true;
                            PredictionOutput output = R.GetPrediction(target);
                            Vector2 direction = output.CastPosition.To2D() - Player.Position.To2D();
                            direction.Normalize();
                            List<AIHeroClient> enemies = ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy && x.IsValidTarget()).ToList();
                            foreach (var enemy in enemies)
                            {
                                if (enemy.BaseSkinName == target.BaseSkinName || !cast)
                                    continue;
                                PredictionOutput prediction = R.GetPrediction(enemy);
                                Vector3 predictedPosition = prediction.CastPosition;
                                Vector3 v = output.CastPosition - Player.ServerPosition;
                                Vector3 w = predictedPosition - Player.ServerPosition;
                                double c1 = Vector3.Dot(w, v);
                                double c2 = Vector3.Dot(v, v);
                                double b = c1 / c2;
                                Vector3 pb = Player.ServerPosition + ((float)b * v);
                                float length = Vector3.Distance(predictedPosition, pb);
                                if (length < (R.Width + 150 + enemy.BoundingRadius / 2) && Player.Distance(predictedPosition) < Player.Distance(target.ServerPosition))
                                    cast = false;
                            }

                            if (cast && GetRealDistance(target) > bonusRange() + 300 + target.BoundingRadius && target.CountAlliesInRange(600) == 0 && ObjectManager.Player.CountEnemiesInRange(400) == 0)
                            {
                                castR(target);
                                debug("R normal High");

                            }
                            else if (cast && target.CountEnemiesInRange(200) > 2 && GetRealDistance(target) > bonusRange() + 200 + target.BoundingRadius)
                            {
                                R1.Cast(target, true, true);
                                debug("R aoe 1");
                            }
                        }
                    }
                }
            }
            
        }

        private static bool Farm
        {
            get { return (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear) || (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed) || (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit); }
        }

        private static void KsJungle()
        {
            var mobs = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, float.MaxValue, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            foreach (var mob in mobs)
            {
                //debug(mob.BaseSkinName);
                if (((mob.BaseSkinName == "SRU_Dragon" && Config.Item("Rdragon").GetValue<bool>())
                    || (mob.BaseSkinName == "SRU_Baron" && Config.Item("Rbaron").GetValue<bool>()))
                    && mob.CountAlliesInRange(1000) == 0
                    && mob.Health < mob.MaxHealth
                    && mob.Distance(Player.Position) > 1000)
                {
                    if (DragonDmg == 0)
                        DragonDmg = mob.Health;

                    if (Game.Time - DragonTime > 4)
                    {
                        if (DragonDmg - mob.Health > 0)
                        {
                            DragonDmg = mob.Health;
                        }
                        DragonTime = Game.Time;
                    }

                    else
                    {
                        var DmgSec = (DragonDmg - mob.Health) * (Math.Abs(DragonTime - Game.Time) / 4);
                        //debug("DS  " + DmgSec);
                        if (DragonDmg - mob.Health > 0)
                        {
                            var timeTravel = GetUltTravelTime(ObjectManager.Player, R.Speed, R.Delay, mob.Position);
                            var timeR = (mob.Health - ObjectManager.Player.CalcDamage(mob, Damage.DamageType.Physical, (250 + (100 * R.Level)) + ObjectManager.Player.FlatPhysicalDamageMod + 300)) / (DmgSec / 4);
                            //debug("timeTravel " + timeTravel + "timeR " + timeR + "d " + ((150 + (100 * R.Level + 200) + ObjectManager.Player.FlatPhysicalDamageMod)));
                            if (timeTravel > timeR)
                                R.Cast(mob.Position);
                        }
                        else
                        {
                            DragonDmg = mob.Health;
                        }
                        //debug("" + GetUltTravelTime(ObjectManager.Player, R.Speed, R.Delay, mob.Position));
                    }
                }
            }
        }

        private static float GetUltTravelTime(AIHeroClient source, float speed, float delay, Vector3 targetpos)
        {
            float distance = Vector3.Distance(source.ServerPosition, targetpos);
            float missilespeed = speed;
            if (source.ChampionName == "Jinx" && distance > 1350)
            {
                const float accelerationrate = 0.3f; //= (1500f - 1350f) / (2200 - speed), 1 unit = 0.3units/second
                var acceldifference = distance - 1350f;
                if (acceldifference > 150f) //it only accelerates 150 units
                    acceldifference = 150f;
                var difference = distance - 1500f;
                missilespeed = (1350f * speed + acceldifference * (speed + accelerationrate * acceldifference) + difference * 2200f) / distance;
            }
            return (distance / missilespeed + delay);
        }
        private static bool ValidUlt(AIHeroClient target)
        {
            if (target.HasBuffOfType(BuffType.PhysicalImmunity)
            || target.HasBuffOfType(BuffType.SpellImmunity)
            || target.IsZombie
            || target.HasBuffOfType(BuffType.Invulnerability)
            || target.HasBuffOfType(BuffType.SpellShield)
            )
                return false;
            else
                return true;
        }

        private static void castR(AIHeroClient target)
        {
            if (Config.Item("hitchanceR").GetValue<bool>())
            {
                List<Vector2> waypoints = target.GetWaypoints();
                if ((ObjectManager.Player.Distance(waypoints.Last<Vector2>().To3D()) - ObjectManager.Player.Distance(target.Position)) > 400)
                {
                    R.Cast(target, true);
                }
            }
            else
                R.Cast(target, true);
        }

        static void BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            var t = TargetSelector.GetTarget(bonusRange() + 60, TargetSelector.DamageType.Physical);
            if (Q.IsReady() && FishBoneActive && t.IsValidTarget())
            {

                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && GetRealDistance(t) < GetRealPowPowRange(t) && (ObjectManager.Player.Mana < RMANA + WMANA + 20 || ObjectManager.Player.GetAutoAttackDamage(t) * 2 < t.Health))
                    Q.Cast();
                else if (Farm && (GetRealDistance(t) > bonusRange() || GetRealDistance(t) < GetRealPowPowRange(t) || ObjectManager.Player.Mana < RMANA + EMANA + WMANA + WMANA))
                    Q.Cast();
            }
            if (Q.IsReady() && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && !FishBoneActive && ObjectManager.Player.Mana < RMANA + EMANA + WMANA + 30)
            {
                var allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, bonusRange() + 30, MinionTypes.All);
                foreach (var minion in allMinionsQ)
                {
                    if (Orbwalking.InAutoAttackRange(minion) && minion.Health < ObjectManager.Player.GetAutoAttackDamage(minion))
                    {
                        foreach (var minion2 in allMinionsQ)
                        {
                            if (minion2.Health < ObjectManager.Player.GetAutoAttackDamage(minion2) && minion.ServerPosition.Distance(minion2.Position) < 150 && minion2.Position != minion.Position)
                            {
                                Q.Cast();
                            }
                        }
                    }
                }
            }
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Config.Item("AGC").GetValue<bool>() && E.IsReady() && ObjectManager.Player.Mana > RMANA + EMANA)
            {
                var Target = (AIHeroClient)gapcloser.Sender;
                if (Target.IsValidTarget(E.Range))
                {
                    E.Cast(ObjectManager.Player.ServerPosition, true);
                    debug("E agc");
                }
                return;
            }
            return;
        }

        public static void farmQ()
        {
            var allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, bonusRange() + 30, MinionTypes.All);
            foreach (var minion in allMinionsQ)
            {
                if (!Orbwalking.InAutoAttackRange(minion) && minion.Health < ObjectManager.Player.GetAutoAttackDamage(minion) && GetRealPowPowRange(minion) < GetRealDistance(minion) && bonusRange() < GetRealDistance(minion))
                {
                    Q.Cast();
                    return;
                }
            }
        }

        public static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs args)
        {

            if (Config.Item("opsE").GetValue<bool>() && unit.Team != ObjectManager.Player.Team && ShouldUseE(args.SData.Name) && unit.IsValidTarget(E.Range))
            {
                E.Cast(unit.ServerPosition, true);
                debug("E ope");
            }
            if (unit.IsMe && args.SData.Name == "JinxW")
            {
                WCastTime = Game.Time;
            }
        }

        public static bool ShouldUseE(string SpellName)
        {
            if (SpellName == "ThreshQ")
                return true;
            if (SpellName == "KatarinaR")
                return true;
            if (SpellName == "AlZaharNetherGrasp")
                return true;
            if (SpellName == "GalioIdolOfDurand")
                return true;
            if (SpellName == "LuxMaliceCannon")
                return true;
            if (SpellName == "MissFortuneBulletTime")
                return true;
            if (SpellName == "RocketGrabMissile")
                return true;
            if (SpellName == "CaitlynPiltoverPeacemaker")
                return true;
            if (SpellName == "EzrealTrueshotBarrage")
                return true;
            if (SpellName == "InfiniteDuress")
                return true;
            if (SpellName == "VelkozR")
                return true;
            return false;
        }

        public static float bonusRange()
        {
            return 620f + ObjectManager.Player.BoundingRadius + 50 + 25 * ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level;
        }

        private static bool FishBoneActive
        {
            get { return Math.Abs(ObjectManager.Player.AttackRange - 525f) > float.Epsilon; }
        }

        private static float GetRealPowPowRange(GameObject target)
        {
            return 630f + ObjectManager.Player.BoundingRadius + target.BoundingRadius;
        }

        private static float GetRealDistance(Obj_AI_Base target)
        {
            return ObjectManager.Player.ServerPosition.Distance(target.ServerPosition) + ObjectManager.Player.BoundingRadius +
                   target.BoundingRadius;
        }

        public static void ManaMenager()
        {
            QMANA = 10;
            WMANA = W.Instance.SData.Mana;
            EMANA = E.Instance.SData.Mana;
            if (!R.IsReady())
                RMANA = WMANA - ObjectManager.Player.PARRegenRate * W.Instance.Cooldown;
            else
                RMANA = R.Instance.SData.Mana; ;

            if (ObjectManager.Player.Health < ObjectManager.Player.MaxHealth * 0.2)
            {
                QMANA = 0;
                WMANA = 0;
                EMANA = 0;
                RMANA = 0;
            }
        }


        public static void debug(string msg)
        {
            if (Config.Item("debug").GetValue<bool>())
                Chat.Print(msg);
        }

        private static void CastSpell(Spell QWER, AIHeroClient target, int HitChanceNum)
        {
            //HitChance 0 - 2
            // example CastSpell(Q, ts, 2);
            var poutput = QWER.GetPrediction(target);
            var col = poutput.CollisionObjects.Count(ColObj => ColObj.IsEnemy && ColObj.IsMinion && !ColObj.IsDead);
            if (QWER.Collision && col > 0)
                return;
            if (HitChanceNum == 0)
                QWER.Cast(target, true);
            else if (HitChanceNum == 1)
            {
                if ((int)poutput.Hitchance > 4)
                    QWER.Cast(poutput.CastPosition);
            }
            else if (HitChanceNum == 2)
            {
                if ((target.IsFacing(ObjectManager.Player) && (int)poutput.Hitchance == 5) || (target.Path.Count() == 0 && target.Position == target.ServerPosition))
                {
                    if (ObjectManager.Player.Distance(target.Position) < QWER.Range - ((target.MoveSpeed * QWER.Delay) + (Player.Distance(target.Position) / QWER.Speed) + (target.BoundingRadius * 2)))
                    {
                        QWER.Cast(poutput.CastPosition);
                    }
                }
                else if ((int)poutput.Hitchance == 5)
                {
                    QWER.Cast(poutput.CastPosition);
                }
            }
            else if (HitChanceNum == 3)
            {
                List<Vector2> waypoints = target.GetWaypoints();
                float SiteToSite = ((target.MoveSpeed * QWER.Delay) + (Player.Distance(target.ServerPosition) / QWER.Speed)) * 6 - QWER.Width;
                float BackToFront = ((target.MoveSpeed * QWER.Delay) + (Player.Distance(target.ServerPosition) / QWER.Speed));
                if (ObjectManager.Player.Distance(waypoints.Last<Vector2>().To3D()) < SiteToSite || ObjectManager.Player.Distance(target.Position) < SiteToSite)
                    QWER.CastIfHitchanceEquals(target, HitChance.High, true);
                else if (target.Path.Count() < 2
                    && (target.ServerPosition.Distance(waypoints.Last<Vector2>().To3D()) > SiteToSite
                    || Math.Abs(ObjectManager.Player.Distance(waypoints.Last<Vector2>().To3D()) - ObjectManager.Player.Distance(target.Position)) > BackToFront
                    || target.HasBuffOfType(BuffType.Slow) || target.HasBuff("Recall")
                    || (target.Path.Count() == 0 && target.Position == target.ServerPosition)
                    ))
                {
                    if (target.IsFacing(ObjectManager.Player) || target.Path.Count() == 0)
                    {
                        if (ObjectManager.Player.Distance(target.Position) < QWER.Range - ((target.MoveSpeed * QWER.Delay) + (Player.Distance(target.Position) / QWER.Speed) + (target.BoundingRadius * 2)))
                            QWER.CastIfHitchanceEquals(target, HitChance.High, true);
                    }
                    else
                    {
                        QWER.CastIfHitchanceEquals(target, HitChance.High, true);
                    }
                }
            }
            else if (HitChanceNum == 4 && (int)poutput.Hitchance > 4)
            {
                List<Vector2> waypoints = target.GetWaypoints();
                float SiteToSite = ((target.MoveSpeed * QWER.Delay) + (Player.Distance(target.ServerPosition) / QWER.Speed)) * 6 - QWER.Width;
                float BackToFront = ((target.MoveSpeed * QWER.Delay) + (Player.Distance(target.ServerPosition) / QWER.Speed));
                if (ObjectManager.Player.Distance(waypoints.Last<Vector2>().To3D()) < SiteToSite || ObjectManager.Player.Distance(target.Position) < SiteToSite)
                    QWER.CastIfHitchanceEquals(target, HitChance.High, true);
                else if (target.Path.Count() < 2
                    && (target.ServerPosition.Distance(waypoints.Last<Vector2>().To3D()) > SiteToSite
                    || Math.Abs(ObjectManager.Player.Distance(waypoints.Last<Vector2>().To3D()) - ObjectManager.Player.Distance(target.Position)) > BackToFront
                    || target.HasBuffOfType(BuffType.Slow) || target.HasBuff("Recall")
                    || (target.Path.Count() == 0 && target.Position == target.ServerPosition)
                    ))
                {
                    if (target.IsFacing(ObjectManager.Player) || target.Path.Count() == 0)
                    {
                        if (ObjectManager.Player.Distance(target.Position) < QWER.Range - ((target.MoveSpeed * QWER.Delay) + (Player.Distance(target.Position) / QWER.Speed) + (target.BoundingRadius * 2)))
                            QWER.CastIfHitchanceEquals(target, HitChance.High, true);
                    }
                    else
                    {
                        QWER.CastIfHitchanceEquals(target, HitChance.High, true);
                    }
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {

            if (Config.Item("OrbDraw").GetValue<bool>())
            {
                if (ObjectManager.Player.HealthPercentage() > 60)
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, ObjectManager.Player.AttackRange + ObjectManager.Player.BoundingRadius * 2, System.Drawing.Color.GreenYellow, 2, 1);
                else if (ObjectManager.Player.HealthPercentage() > 30)
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, ObjectManager.Player.AttackRange + ObjectManager.Player.BoundingRadius * 2, System.Drawing.Color.Orange, 3, 1);
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, ObjectManager.Player.AttackRange + ObjectManager.Player.BoundingRadius * 2, System.Drawing.Color.Red, 4, 1);
            }
            if (Config.Item("qRange").GetValue<bool>())
            {
                if (FishBoneActive)
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, 590f + ObjectManager.Player.BoundingRadius, System.Drawing.Color.DeepPink, 1, 1);
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, bonusRange() - 40, System.Drawing.Color.DeepPink, 1, 1);
            }
            if (Config.Item("wRange").GetValue<bool>())
            {
                if (Config.Item("onlyRdy").GetValue<bool>())
                {
                    if (W.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.Cyan, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.Cyan, 1, 1);
            }
            if (Config.Item("eRange").GetValue<bool>())
            {
                if (Config.Item("onlyRdy").GetValue<bool>())
                {
                    if (E.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.Gray, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.Gray, 1, 1);
            }
            if (Config.Item("rRange").GetValue<bool>())
            {
                if (Config.Item("onlyRdy").GetValue<bool>())
                {
                    if (R.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.Gray, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.Gray, 1, 1);
            }

            if (Config.Item("orb").GetValue<bool>())
            {
                var orbT = Orbwalker.GetTarget();

                if (orbT.IsValidTarget())
                {
                    if (orbT.Health > orbT.MaxHealth * 0.6)
                        LeagueSharp.Common.Utility.DrawCircle(orbT.Position, orbT.BoundingRadius, System.Drawing.Color.GreenYellow, 5, 1);
                    else if (orbT.Health > orbT.MaxHealth * 0.3)
                        LeagueSharp.Common.Utility.DrawCircle(orbT.Position, orbT.BoundingRadius, System.Drawing.Color.Orange, 10, 1);
                    else
                        LeagueSharp.Common.Utility.DrawCircle(orbT.Position, orbT.BoundingRadius, System.Drawing.Color.Red, 10, 1);
                }
            }

            if (Config.Item("noti").GetValue<bool>())
            {
                var t = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

                if (t.IsValidTarget() && R.IsReady())
                {
                    float predictedHealth = HealthPrediction.GetHealthPrediction(t, (int)(R.Delay + (Player.Distance(t.ServerPosition) / R.Speed) * 1000));
                    var rDamage = R.GetDamage(t);
                    if (rDamage > predictedHealth)
                    {
                        Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.5f, System.Drawing.Color.Red, "Ult can kill: " + t.ChampionName + " have: " + t.Health + "hp");
                        Render.Circle.DrawCircle(t.ServerPosition, 200, System.Drawing.Color.Red);
                    }
                    if (Config.Item("semi").GetValue<bool>())
                    {
                        Render.Circle.DrawCircle(t.Position, 100, System.Drawing.Color.Red);
                    }
                }
                var tw = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
                if (tw.IsValidTarget())
                {
                    if (Config.Item("debug").GetValue<bool>())
                        Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.4f, System.Drawing.Color.GreenYellow, "Pred: " + (int)R.GetPrediction(tw).Hitchance);

                    var wDmg = W.GetDamage(tw);
                    if (wDmg > tw.Health)
                    {
                        Render.Circle.DrawCircle(ObjectManager.Player.ServerPosition, W.Range, System.Drawing.Color.Red);
                        Render.Circle.DrawCircle(tw.ServerPosition, 200, System.Drawing.Color.Red);
                        Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.4f, System.Drawing.Color.Red, "W can kill: " + t.ChampionName + " have: " + t.Health + "hp");
                    }
                }
            }
        }
    }
}