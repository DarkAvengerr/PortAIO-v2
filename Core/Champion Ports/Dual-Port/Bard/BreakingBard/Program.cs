using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy;
using LeagueSharp.Common;
namespace BreakingBard
{
    class Program
    {
        static Vector2 wallVec = new Vector2(0, 0);
        static Vector2 flashVec = new Vector2(0, 0);
        static Orbwalking.Orbwalker Orbwalker;

        static Menu config = new Menu("BardExtension", "asf", true);
        static Spell R = new Spell(SpellSlot.R) { Speed = 2.1f /*(units/ms)*/, Range = 3400, Delay = 500 };
        static int R_radius = 350;

        static Spell Q = new Spell(SpellSlot.Q, 900) { Speed = 1.6f, Delay = 250 };
        static Spell W = new Spell(SpellSlot.W, 900) { Delay = 250, Speed = int.MaxValue };
        static int Q_radius = 60;

        static List<Vector2> predictions = new List<Vector2>();
        static List<AIHeroClient> enemiesGettingHit = new List<AIHeroClient>();
        static List<Vector2> allyPredictions = new List<Vector2>();
        static Vector2 circleCenter;
        static float circleRadius;

        static Vector2 betterCircleCenter;
        static float betterCircleRadius;

        static bool draw = false;

        public static void Main()
        {
            Game_OnGameLoad();
            Interrupter.OnPossibleToInterrupt += Interrupter_OnPossibleToInterrupt;
        }

        private static void Interrupter_OnPossibleToInterrupt(AIHeroClient unit, InterruptableSpell spell)
        {
            if (config.SubMenu("ult").SubMenu("misc").Item("ultInterrupt").GetValue<bool>() &&
                R.IsReady() && unit.Distance(ObjectManager.Player) <= R.Range + 600 &&
                !(spell.DangerLevel >= InterruptableDangerLevel.High && unit.ChampionName == "Fiddlesticks"))
            {
                float flyTime = ObjectManager.Player.Distance(unit) / R.Speed;

                //if (Environment.TickCount + flyTime / 1000 < spell.EndTime)
                //{
                var senderPred = Prediction.GetPrediction(unit, R.Delay + flyTime);

                if (senderPred.Hitchance >= HitChance.High &&
                    unit.Distance(ObjectManager.Player) <= R.Range)
                    R.Cast(senderPred.CastPosition);
                //}
            }
        }

        static void Game_OnGameLoad()
        {
            var TargetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(TargetSelectorMenu);
            config.AddSubMenu(TargetSelectorMenu);


            config.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(config.SubMenu("Orbwalker"));

            config.AddSubMenu(new Menu("Combo", "combo"));
            config.SubMenu("combo").AddItem(new MenuItem("comboKey", "Active"))
                .SetValue(new KeyBind(32, KeyBindType.Press));
            config.SubMenu("combo").AddItem(new MenuItem("useQToStun", "Use Q to stun only in combo"))
                .SetValue(true);
            config.SubMenu("combo").AddItem(new MenuItem("useQ", "Use Q always in combo"))
                .SetValue(true);
            config.SubMenu("combo").AddItem(new MenuItem("useW", "Use W on ally"))
                .SetValue(true);

            config.AddSubMenu(new Menu("Harass", "harass"));
            config.SubMenu("harass").AddItem(new MenuItem("useQHarassKey", "Use Q"))
                .SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press));
            config.SubMenu("harass").AddItem(new MenuItem("useQHarassToggle", "Use Q Toggle"))
                .SetValue(new KeyBind("L".ToCharArray()[0], KeyBindType.Toggle));
            config.SubMenu("harass").AddItem(new MenuItem("mana", "Use Q till X % of max mana"))
                .SetValue(new Slider(40, 0, 100));

            config.AddSubMenu(new Menu("Extra (Beta)", "extra"));
            config.SubMenu("extra").AddItem(new MenuItem("flashQ", "Use Flash -> Q to stun enemy at wall"))
                .SetValue(new KeyBind("H".ToCharArray()[0], KeyBindType.Press));
            config.SubMenu("extra").AddItem(new MenuItem("unitsToExtend", "Min distance between player and enemy:"))
                .SetValue(new Slider(100, 0, 950));
            config.SubMenu("extra").AddItem(new MenuItem("qDeviation", "Q range deviation"))
                .SetValue(new Slider(200, 0, 500));

            config.AddSubMenu(new Menu("Ultimate", "ult"));
            config.SubMenu("ult").AddItem(new MenuItem("ultEnemy", "Auto ult if hit X enemies"))
                .SetValue(new KeyBind(32, KeyBindType.Press));
            config.SubMenu("ult").AddItem(new MenuItem("ultEnemyCount", "X enemies:")).
                SetValue(new Slider(3, 1, 5));
            config.SubMenu("ult").AddItem(new MenuItem("ultLowAlly", "Auto ult if ally low")).
                SetValue(true);
            config.SubMenu("ult").AddSubMenu(new Menu("Allies to ult", "allies"));
            config.SubMenu("ult").AddSubMenu(new Menu("Enemies to ult", "enemies"));
            config.SubMenu("ult").AddItem(new MenuItem("ultAllyAtXHp", "Ult ally at X hp")).
                SetValue(new Slider(300, 2, 1500));
            config.SubMenu("ult").AddSubMenu(new Menu("Misc", "misc"));

            config.SubMenu("ult").SubMenu("misc").AddItem(new MenuItem("radiusDeviation", "Radius Deviation")).
                SetValue(new Slider(130, 0, 300));
            config.SubMenu("ult").SubMenu("misc").AddItem(new MenuItem("useBetterPred", "Only ult if undogeable (with walking)")).
                SetValue(false);
            config.SubMenu("ult").SubMenu("misc").AddItem(new MenuItem("info1", "not recommended"));
            config.SubMenu("ult").SubMenu("misc").AddItem(new MenuItem("ultInterrupt", "Use ult to interrupt")).
                SetValue(true);

            foreach (var ally in ObjectManager.Get<AIHeroClient>().Where(x => x.IsAlly))
            {
                config.SubMenu("ult").SubMenu("allies").AddItem(new MenuItem(ally.ChampionName, ally.ChampionName)).
                    SetValue(true);
            }

            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy))
            {
                config.SubMenu("ult").SubMenu("enemies").AddItem(new MenuItem(enemy.ChampionName, enemy.ChampionName)).
                    SetValue(false);
            }

            config.AddToMainMenu();
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            if (draw)
                Render.Circle.DrawCircle(betterCircleCenter.To3D(), betterCircleRadius,
                    System.Drawing.Color.White, 50);

            if (wallVec != new Vector2(0, 0))
                Render.Circle.DrawCircle(wallVec.To3D(), 100,
                    System.Drawing.Color.Yellow, 50);

            if (flashVec != new Vector2(0, 0))
                Render.Circle.DrawCircle(flashVec.To3D(), 100,
                    System.Drawing.Color.Red, 50);

        }

        static void CalcUlt()
        {
            if (config.SubMenu("ult").Item("ultEnemy").GetValue<KeyBind>().Active)
            {
                predictions.Clear();
                enemiesGettingHit.Clear();
                AddPredictions();

                if (predictions.Count >= config.SubMenu("ult").Item("ultEnemyCount").GetValue<Slider>().Value)
                {
                    MEC.FindMinimalBoundingCircle(predictions, out circleCenter, out circleRadius);

                    FindFarPoints();

                    if (predictions.Count >= config.SubMenu("ult").Item("ultEnemyCount").GetValue<Slider>().Value)
                    {

                        MEC.FindMinimalBoundingCircle(predictions, out betterCircleCenter, out betterCircleRadius);

                        if (predictions.Count >= config.SubMenu("ult").Item("ultEnemyCount").GetValue<Slider>().Value &&
                            betterCircleCenter.Distance(ObjectManager.Player.Position) <= R.Range)
                        {
                            draw = true;
                            if (config.SubMenu("ult").SubMenu("misc").Item("useBetterPred").GetValue<bool>() &&
                                BetterPredition() >=
                                config.SubMenu("ult").Item("ultEnemyCount").GetValue<Slider>().Value)
                                Ult();
                            else if (!config.SubMenu("ult").SubMenu("misc").Item("useBetterPred").GetValue<bool>())
                                Ult();
                        }
                        else
                        {
                            draw = false;
                            //Console.WriteLine("Less points: " + predictions.Count.ToString());
                        }
                    }
                }
            }

            if (R.IsReady())
            {
                foreach (var ally in ObjectManager.Get<AIHeroClient>().Where(x => x.IsAlly && !x.IsDead))
                {
                    if (config.SubMenu("ult").SubMenu("allies").Item(ally.ChampionName).GetValue<bool>() &&
                        !ally.IsRecalling())
                    {
                        float flyTime = ObjectManager.Player.Distance(ally) / R.Speed;
                        float healthPrediction = HealthPrediction.GetHealthPrediction(ally, (int)(R.Delay + flyTime));

                        if (healthPrediction >= 1 &&
                            healthPrediction <= config.SubMenu("ult").Item("ultAllyAtXHp").GetValue<Slider>().Value &&
                            ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy && !x.IsDead).Count() > 0)
                        {
                            var allyPosPrediction =
                                Prediction.GetPrediction(ally, R.Delay + flyTime);
                            if (allyPosPrediction.CastPosition.Distance(ObjectManager.Player.Position) <= R.Range &&
                                allyPosPrediction.Hitchance >= HitChance.Medium)
                                R.Cast(allyPosPrediction.CastPosition);
                        }
                    }
                }
            }
        }

        static void Game_OnUpdate(EventArgs args)
        {
            CalcUlt();

            CastQStunOnly();

            CastQNormal();

            if (config.SubMenu("combo").Item("useW").GetValue<bool>() &&
                config.SubMenu("combo").Item("comboKey").GetValue<KeyBind>().Active &&
                W.IsReady())
                CastW();

            Extra();
        }

        private static void Extra()
        {
            if (config.SubMenu("extra").Item("flashQ").GetValue<KeyBind>().Active)
            {
                foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy && !x.IsDead &&
                x.Distance(ObjectManager.Player.Position) <= 2000))
                {
                    var enemyPos = TargetSelector.GetSelectedTarget() == null ?
                        enemy.Position.To2D() :
                        TargetSelector.GetSelectedTarget().Position.To2D();

                    List<Vector2> dirVecs = Math.GetAllDirectionVectorsOfCircle(enemyPos, Q.Range);
                    List<Vector2> dirVecsReachingWall = new List<Vector2>();

                    foreach (var dirVec in dirVecs)
                    {
                        if (LeagueSharp.Common.Utility.IsWall(dirVec))
                            dirVecsReachingWall.Add(dirVec);
                    }

                    //getting farest wallVector
                    float farestDirVecDistToPlayer = 0;
                    Vector2 farestVector = new Vector2(0, 0);

                    if (dirVecsReachingWall.Count > 0)
                    {
                        foreach (var dirVecReachingWall in dirVecsReachingWall)
                        {
                            if (farestDirVecDistToPlayer == 0 || dirVecReachingWall.Distance(ObjectManager.Player.Position) >
                                farestDirVecDistToPlayer)
                            {
                                farestDirVecDistToPlayer = dirVecReachingWall.Distance(ObjectManager.Player.Position);
                                farestVector = dirVecReachingWall;
                            }
                        }
                    }

                    if (farestVector != new Vector2(0, 0))
                    {
                        Vector2 enemyPred_farestVector = farestVector - enemyPos;

                        wallVec = Math.IsWall_DownScaleVector(enemyPos, enemyPred_farestVector);

                        Vector2 dirVecToExtend = enemyPos - wallVec;
                        //stretch vector x units

                        //origin vec from enemy to wall (lenght reaches wall)

                        flashVec = Math.ExtendVectorToX_CheckDist(wallVec, dirVecToExtend, Q.Range,
                            config.SubMenu("extra").Item("unitsToExtend").GetValue<Slider>().Value);


                        if (flashVec != new Vector2(0, 0) &&
                            flashVec.Distance(ObjectManager.Player) <= 450 &&
                            wallVec.Distance(flashVec) <= Q.Range -
                            config.SubMenu("extra").Item("qDeviation").GetValue<Slider>().Value) //flash range
                        {
                            float qFlyTime = flashVec.Distance(enemy) / Q.Speed;
                            var enemyPredPos = Prediction.GetPrediction(enemy, qFlyTime);

                            Vector2 Vec_flashVec_enemyPredPos = enemyPredPos.CastPosition.To2D() - flashVec;
                            Vec_flashVec_enemyPredPos = Vector2.Multiply(Vec_flashVec_enemyPredPos,
                                (Q.Range - config.SubMenu("extra").Item("qDeviation").GetValue<Slider>().Value) /
                                Vec_flashVec_enemyPredPos.Length());
                            Vec_flashVec_enemyPredPos = flashVec + Vec_flashVec_enemyPredPos;

                            Spell Flash = new Spell(ObjectManager.Player.GetSpellSlot("SummonerFlash"), 450);

                            if (Flash.IsReady() && enemyPredPos.Hitchance >= HitChance.High &&
                                LeagueSharp.Common.Utility.IsWall(Vec_flashVec_enemyPredPos))
                            {
                                Flash.Cast(enemyPredPos.CastPosition);
                                Q.Cast(enemyPos);
                            }
                        }
                    }

                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static void CastW()
        {
            foreach (var ally in ObjectManager.Get<AIHeroClient>().Where(x => x.IsAlly && !x.IsDead && !x.IsMe).OrderBy(x => x.CombatType ==
            GameObjectCombatType.Ranged))
            {
                var allyPred = W.GetPrediction(ally);

                if (allyPred.Hitchance >= HitChance.Medium)
                    W.Cast(allyPred.CastPosition);
            }
        }
        private static void CastQNormal()
        {
            if ((config.SubMenu("combo").Item("comboKey").GetValue<KeyBind>().Active &&
                config.SubMenu("combo").Item("useQ").GetValue<bool>() &&
                !config.SubMenu("combo").Item("useQToStun").GetValue<bool>())
                ||
                (
                (config.SubMenu("harass").Item("useQHarassKey").GetValue<KeyBind>().Active ||
                config.SubMenu("harass").Item("useQHarassToggle").GetValue<KeyBind>().Active) &&
                ObjectManager.Player.Mana >= ObjectManager.Player.MaxMana *
                (config.SubMenu("harass").Item("mana").GetValue<Slider>().Value / 100)
                )
                && Q.IsReady())
            {
                var target = (TargetSelector.GetTarget(1500, TargetSelector.DamageType.Physical) ??
               TargetSelector.GetTarget(1500, TargetSelector.DamageType.Magical)) ??
               TargetSelector.GetTarget(1500, TargetSelector.DamageType.True);

                target = TargetSelector.GetSelectedTarget() ?? target;

                var predPos = Q.GetPrediction(target);

                if (predPos.Hitchance >= HitChance.High)
                    Q.Cast(predPos.CastPosition);
            }
        }

        private static void CastQStunOnly()
        {
            if (config.SubMenu("combo").Item("comboKey").GetValue<KeyBind>().Active &&
                config.SubMenu("combo").Item("useQ").GetValue<bool>() &&
                config.SubMenu("combo").Item("useQToStun").GetValue<bool>() && Q.IsReady())
            {
                foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy && !x.IsDead
                    && x.Distance(ObjectManager.Player.Position) <= Q.Range + 1000))
                {
                    var predPos = Q.GetPrediction(enemy);
                    var unitPos = predPos.UnitPosition.To2D();
                    Vector2 dirVec1 = unitPos - ObjectManager.Player.Position.To2D();

                    dirVec1 = Vector2.Multiply(dirVec1, Q.Range / dirVec1.Length());
                    dirVec1 = ObjectManager.Player.Position.To2D() + dirVec1;

                    if (LeagueSharp.Common.Utility.IsWall(dirVec1) && predPos.Hitchance >= HitChance.High &&
                        predPos.CastPosition.Distance(ObjectManager.Player.Position) <= Q.Range)
                        Q.Cast(predPos.CastPosition);

                    if (Q.IsReady())
                    {
                        List<Vector2> to = new List<Vector2> { predPos.UnitPosition.To2D() };

                        if (Q.GetCollision(ObjectManager.Player.Position.To2D(),
                            to).Count == 1 && predPos.Hitchance >= HitChance.High)
                        {
                            Q.Cast(predPos.CastPosition);
                        }
                    }
                    else
                        break;
                }
            }
        }

        private static void Ult()
        {
            //if (config.SubMenu("ult").SubMenu("misc").Item("checkAllyHit").GetValue<bool>() && !CheckIfAllyGetsHit())
            //    R.Cast(betterCircleCenter);
            //else if (!config.SubMenu("ult").SubMenu("misc").Item("checkAllyHit").GetValue<bool>())
            R.Cast(betterCircleCenter);
        }

        private static int BetterPredition()
        {
            int i = 0;
            float flyTime = ObjectManager.Player.Distance(betterCircleCenter) / R.Speed;

            foreach (var enemy in enemiesGettingHit)
            {
                var dodgeDist = R_radius - enemy.Distance(betterCircleCenter);
                var dodgeTime = dodgeDist / enemy.MoveSpeed;

                if (dodgeTime > flyTime)
                    i++;
            }

            return i;
        }

        private static bool CheckIfAllyGetsHit()
        {
            foreach (var ally in ObjectManager.Get<AIHeroClient>().Where(x => x.IsAlly && !x.IsDead))
            {
                float flyTime = ObjectManager.Player.Distance(ally, false) / R.Speed;
                var allyPred = Prediction.GetPrediction(ally, R.Delay + flyTime);
                if (allyPred.Hitchance == HitChance.VeryHigh || allyPred.Hitchance == HitChance.Immobile)
                    allyPredictions.Add(allyPred.UnitPosition.To2D());
            }

            foreach (var allyPred in allyPredictions)
            {
                if (betterCircleCenter.Distance(allyPred) <= R_radius)
                    return true;
            }

            return false;

        }

        private static void FindFarPoints()
        {
            for (int j = 0; j < 5; j++)
            {
                float maxDist = 0;
                int removeIndex = -1;

                int i = 0;
                foreach (var prediction in predictions)
                {
                    if (circleCenter.Distance(prediction) > R_radius -
                        config.SubMenu("ult").SubMenu("misc").Item("radiusDeviation").GetValue<Slider>().Value &&
                        (maxDist == 0 || circleCenter.Distance(prediction) > maxDist))
                    {
                        maxDist = circleCenter.Distance(prediction);
                        removeIndex = i;
                    }
                    i++;
                }

                if (removeIndex != -1)
                    predictions.RemoveAt(removeIndex);
                else
                    return;

                MEC.FindMinimalBoundingCircle(predictions, out circleCenter, out circleRadius);
            }
        }

        private static void AddPredictions()
        {
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy && !x.IsDead))
            {
                if (config.SubMenu("ult").SubMenu("enemies").Item(enemy.ChampionName).GetValue<bool>())
                {
                    float flyTime = ObjectManager.Player.Distance(enemy, false) / R.Speed;
                    if (Prediction.GetPrediction(enemy, R.Delay + flyTime).Hitchance >= HitChance.High)
                    {
                        predictions.Add(Prediction.GetPrediction(enemy, R.Delay + flyTime).UnitPosition.To2D());
                        enemiesGettingHit.Add(enemy);
                    }
                }
            }
        }
    }
}