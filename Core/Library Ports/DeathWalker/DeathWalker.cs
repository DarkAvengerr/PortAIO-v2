using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DetuksSharp.Prediction;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;
using EloBuddy;

namespace DetuksSharp
{

    public class DeathWalker
    {

        public static bool azir = false;

        //Spells that reset the attack timer.
        private static readonly string[] AttackResets =
        {
            "dariusnoxiantacticsonh", "fioraflurry", "garenq",
            "hecarimrapidslash", "jaxempowertwo", "jaycehypercharge", "leonashieldofdaybreak", "luciane", "lucianw", "lucianq",
            "monkeykingdoubleattack", "mordekaisermaceofspades", "nasusq", "nautiluspiercinggaze", "netherblade",
            "parley", "poppydevastatingblow", "powerfist", "renektonpreexecute", "rengarq", "shyvanadoubleattack",
            "sivirw", "takedown", "talonnoxiandiplomacy", "trundletrollsmash", "vaynetumble", "vie", "volibearq",
            "xenzhaocombotarget", "yorickspectral", "reksaiq"
        };

        //Spells that are not attacks even if they have the "attack" word in their name.
        private static readonly string[] NoAttacks =
        {
            "jarvanivcataclysmattack", "monkeykingdoubleattack",
            "shyvanadoubleattack", "shyvanadoubleattackdragon", "zyragraspingplantattack", "zyragraspingplantattack2",
            "zyragraspingplantattackfire", "zyragraspingplantattack2fire", "viktorpowertransfer"
        };

        //Spells that are attacks even if they dont have the "attack" word in their name.
        private static readonly string[] Attacks =
        {
            "caitlynheadshotmissile", "frostarrow", "garenslash2",
            "kennenmegaproc", "lucianpassiveattack", "masteryidoublestrike", "quinnwenhanced", "renektonexecute",
            "renektonsuperexecute", "rengarnewpassivebuffdash", "trundleq", "xenzhaothrust", "xenzhaothrust2",
            "xenzhaothrust3", "viktorqbuff"
        };

        //cant cancel attacks
        private static readonly string[] AttacksCantCancel =
        {
            "azirbasicattacksoldier",
        };

        public static int now
        {
            get { return (int)DateTime.Now.TimeOfDay.TotalMilliseconds; }
        }

        public static Menu menu;
        public enum Mode
        {
            Combo,
            Harass,
            LaneClear,
            LaneFreeze,
            Lasthit,
            Flee,
            None,
        }

        public static Mode CurrentMode
        {
            get
            {
                if (BottingMode)
                    return Mode.LaneClear;
                if (menu.Item("Combo_Key").GetValue<KeyBind>().Active)
                    return Mode.Combo;
                if (menu.Item("Harass_Key").GetValue<KeyBind>().Active)
                    return Mode.Harass;
                if (menu.Item("LaneClear_Key").GetValue<KeyBind>().Active)
                    return Mode.LaneClear;
                if (menu.Item("LastHit_Key").GetValue<KeyBind>().Active)
                    return Mode.Lasthit;
                return Mode.None;
            }
        }


        public delegate void BeforeAttackEvenH(BeforeAttackEventArgs args);
        public delegate void AfterAttackEvenH(AttackableUnit unit, AttackableUnit target);
        public delegate void OnUnkillableEvenH(AttackableUnit unit, AttackableUnit target, int msTillDead);

        public static event BeforeAttackEvenH BeforeAttack;
        public static event AfterAttackEvenH AfterAttack;
        public static event OnUnkillableEvenH OnUnkillable;
        public static AIHeroClient player = ObjectManager.Player;

        public static int lastDmg = HealthDeath.now;

        private static int previousAttack = 0;

        private static bool isTryingToAttack = false;
        private static int lastAutoAttack = 0;
        private static int lastAutoAttackMove = 0;
        private static int lastmove = 0;

        private static int cantMoveTill = 0;

        private static bool attack = true;
        private static bool move = true;

        private static bool disableNextAttack = false;

        private static bool playerStoped = false;

        private static AttackableUnit killUnit = null;

        public static Obj_AI_Base ForcedTarget = null;


        public static AttackableUnit lastAutoAttackUnit = null;


        public static List<AIHeroClient> AllEnemys = new List<AIHeroClient>();
        public static List<AIHeroClient> AllAllys = new List<AIHeroClient>();

        public static List<Obj_AI_Turret> EnemyTowers = new List<Obj_AI_Turret>();

        public static List<Obj_BarracksDampener> EnemyBarracs = new List<Obj_BarracksDampener>();


        public static List<Obj_AI_Base> enemiesAround = new List<Obj_AI_Base>();

        public static List<Obj_HQ> EnemyHQ = new List<Obj_HQ>();

        public static List<AttackableUnit> EnemyObjectives = new List<AttackableUnit>();

        //For botting use mostly
        public static bool CustomOrbwalkMode = false;

        public static int CustomAttackDelay = 0;

        public static bool IgnoreMinions = false;

        public static bool BottingMode = false;

        public static int CustomRunCS = 0;

        public static int CustomMoveDelay = 0;
        public static int CustomMoveDelayTemp = 0;

        private static DeathDraw dDraw;

        private static void init()
        {
            dDraw = new DeathDraw();
            //While testing menu
            AllEnemys = ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsEnemy).ToList();
            AllAllys = ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsAlly).ToList();

            EnemyTowers = ObjectManager.Get<Obj_AI_Turret>().Where(tow => tow.IsEnemy).ToList();

            EnemyBarracs = ObjectManager.Get<Obj_BarracksDampener>().Where(tow => tow.IsEnemy).ToList();

            EnemyHQ = ObjectManager.Get<Obj_HQ>().Where(tow => tow.IsEnemy).ToList();

            EnemyObjectives.AddRange(EnemyTowers);
            EnemyObjectives.AddRange(EnemyBarracs);
            EnemyObjectives.AddRange(EnemyHQ);

        }

        private static void afterAttack(Obj_AI_Base sender, AttackableUnit target)
        {
            isTryingToAttack = false;
            Console.WriteLine("Hit is rdy " + player.AttackDelay * 1000);
            lastAutoAttackMove = 0;
            FireAfterAttack(sender, target);
        }

        private static void onDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (isAutoAttackReset(args.SData.Name))
                {
                    if (player.ChampionName == "Lucian")
                        LeagueSharp.Common.Utility.DelayAction.Add((int)350, delegate { resetAutoAttackTimer(); });
                    else
                        resetAutoAttackTimer();
                }
                var spell = player.Spellbook.GetSpell(args.Slot);
                if (spell != null)
                {
                    if (spell.IsAutoAttack() || args.SData.IsAutoAttack())
                    {
                        afterAttack(sender, (AttackableUnit)args.Target);
                    }
                }
            }
        }

        private static void onStopAutoAttack(Obj_AI_Base sender, SpellbookStopCastEventArgs args)
        {
            if (sender.IsValid && sender.IsMe && EloBuddy.SDK.Orbwalker.IsRanged && (args.DestroyMissile && args.StopAnimation))
            {
                Console.WriteLine("Cancel auto");
                var resetTo = (menu.Item("betaStut").GetValue<bool>()) ? previousAttack : 0;
                lastAutoAttack = resetTo;
                lastAutoAttackMove = resetTo;
            }
        }

        private static void onStartAutoAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
                return;


            if (IsAutoAttack(args.SData.Name))
            {
                previousAttack = lastAutoAttack;
                lastAutoAttack = now;
                lastAutoAttackMove = now;
            }
            if (IsCantCancel(args.SData.Name))
            {
                lastAutoAttackMove -= 100;
            }
        }

        public static void disableMovementFor(int ms)
        {
            cantMoveTill = now + ms;
        }

        private static void onDraw(EventArgs args)
        {
            LeagueSharp.Common.Utility.DrawCircle(player.Position, player.AttackRange + player.BoundingRadius, Color.Green);
        }

        private static void OnUpdate(EventArgs args)
        {
            if (BottingMode)
                return;
            try
            {
                deathWalkTarget(Game.CursorPos, CurrentMode != Mode.None ? getBestTarget() : ((azir && player.HealthPercent > 30) ? getBestTarget(azir) : null), CurrentMode == Mode.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static void doAttack(AttackableUnit target)
        {
            FireBeforeAttack(target);
            if (EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target))
            {
                playerStoped = false;
                previousAttack = lastAutoAttack;
                lastAutoAttack = now;
                lastAutoAttackMove = now;
                lastAutoAttackUnit = target;
            }
        }

        public static void deathWalk(Vector3 goalPosition, bool onlyChamps = false, bool delayMovement = false)
        {
            if ((CurrentMode == Mode.None || BottingMode) && !CustomOrbwalkMode)
                deathWalkTarget(goalPosition, getBestTarget(false, onlyChamps), false, delayMovement);
        }

        public static void deathWalkTarget(Vector3 goalPosition, AttackableUnit target = null, bool noMove = false, bool delayMovement = false)
        {
            if (target != null && canAttack() && inAutoAttackRange(target))
            {
                doAttack(target);
            }
            CustomMoveDelayTemp = delayMovement ? 400 : 0;
            if (canMove() && !noMove)
            {
                if (target != null && (CurrentMode == Mode.Lasthit || CurrentMode == Mode.Harass))
                    killUnit = target;
                if (killUnit != null && !(killUnit is AIHeroClient) && killUnit.IsValid && !killUnit.IsDead && killUnit.Position.Distance(player.Position) > getRealAutoAttackRange(killUnit) - 30)//Get in range
                    moveTo(killUnit.Position);
                moveTo(goalPosition);
            }
        }

        public static AttackableUnit getBestTarget(bool onlySolider = false, bool targetOnlyChampions = false)
        {
            bool soliderHit = false;

            /* turrets / inhibitors / nexus */
            if (BottingMode)
            {
                /* turrets */
                foreach (var turret in
                    ObjectManager.Get<Obj_AI_Turret>().Where(t => t.IsValidTarget() && inAutoAttackRange(t) && !t.IsDead))
                {
                    return turret;
                }

                /* inhibitor */
                foreach (var turret in
                    ObjectManager.Get<Obj_BarracksDampener>()
                        .Where(t => t.IsValidTarget() && inAutoAttackRange(t) && !t.IsDead))
                {
                    return turret;
                }

                /* nexus */
                foreach (var nexus in
                    ObjectManager.Get<Obj_HQ>().Where(t => t.IsValidTarget() && inAutoAttackRange(t) && !t.IsDead))
                {
                    return nexus;
                }
            }

            if (targetOnlyChampions)
                return GetBestHeroTarget(out soliderHit);

            if (ForcedTarget != null && !onlySolider)
            {
                if (inAutoAttackRange(ForcedTarget) && !ForcedTarget.IsDead && ForcedTarget.IsHPBarRendered && ForcedTarget.IsVisible)
                    return ForcedTarget;
                ForcedTarget = null;
            }

            if (azir)
            {
                enemiesAround = ObjectManager.Get<Obj_AI_Base>()
                .Where(targ => targ.IsValid && inAutoAttackRange(targ) && targ.IsEnemy && targ.IsHPBarRendered).ToList();
            }
            else
            {
                enemiesAround = ObjectManager.Get<Obj_AI_Base>()
                    .Where(targ => targ.IsValidTarget(getTargetSearchDist()) && targ.IsEnemy && targ.IsHPBarRendered && targ.IsTargetable && !targ.IsDead).ToList();
            }

            Obj_AI_Base best = null;

            //Lat hit
            float bestPredHp = float.MaxValue;

            if (azir)
            {
                var hero1 = GetBestHeroTarget(out soliderHit);

                if (hero1 != null && (enemyInAzirRange(hero1) || hero1 is Obj_AI_Minion) && (!onlySolider || soliderHit))
                    return hero1;
            }

            if (!onlySolider)
            {
                //check motherfuckers that are attacked by tower
                if (CurrentMode == Mode.Harass || CurrentMode == Mode.Lasthit || CurrentMode == Mode.LaneClear)
                {

                    foreach (var targ in enemiesAround)
                    {
                        var towerShot = HealthDeath.attackedByTurret(targ);
                        if (towerShot == null) continue;
                        var hpOnDmgPred = HealthDeath.getLaneClearPred(targ, towerShot.hitOn + 10 - now);

                        var aa = getRealAADmg(targ);
                        if (hpOnDmgPred > aa && hpOnDmgPred <= aa * 2f)
                        {
                            return targ;
                        }
                    }
                }
            }

            if (!onlySolider)
            {
                if (CurrentMode == Mode.Harass || CurrentMode == Mode.Lasthit || CurrentMode == Mode.LaneClear)
                {
                    //Last hit
                    foreach (var targ in enemiesAround.OrderByDescending(min => HealthDeath.getLastHitPredPeriodic(min, timeTillDamageOn(min))))
                    {
                        var hpOnDmgPred = HealthDeath.getLastHitPred(targ, timeTillDamageOn(targ));
                        if (hpOnDmgPred <= 0 && (lastAutoAttackUnit == null || lastAutoAttackUnit.NetworkId != targ.NetworkId))
                            FireOnUnkillable(player, targ, HealthDeath.getTimeTillDeath(targ));
                        if (hpOnDmgPred <= 0 || hpOnDmgPred > (int)getRealAADmg(targ))
                            continue;
                        var cannonBonus = (targ.BaseSkinName == "SRU_ChaosMinionSiege") ? 100 : 0;
                        if (best == null || hpOnDmgPred - cannonBonus < bestPredHp)
                        {
                            best = targ;
                            bestPredHp = hpOnDmgPred;
                        }
                    }
                    if (best != null)
                        return best;
                }
            }

            var hero = GetBestHeroTarget(out soliderHit);

            if (hero != null && (!onlySolider || soliderHit))
                return hero;

            if (!onlySolider)
                if (ShouldWaitAllTogether())
                    return null;

            /* turrets / inhibitors / nexus */
            if (CurrentMode == Mode.LaneClear)
            {
                /* turrets */
                foreach (var turret in
                    ObjectManager.Get<Obj_AI_Turret>().Where(t => t.IsValidTarget() && inAutoAttackRange(t) && !t.IsDead))
                {
                    return turret;
                }

                /* inhibitor */
                foreach (var turret in
                    ObjectManager.Get<Obj_BarracksDampener>()
                        .Where(t => t.IsValidTarget() && inAutoAttackRange(t) && !t.IsDead))
                {
                    return turret;
                }

                /* nexus */
                foreach (var nexus in
                    ObjectManager.Get<Obj_HQ>().Where(t => t.IsValidTarget() && inAutoAttackRange(t) && !t.IsDead))
                {
                    return nexus;
                }
            }

            if (!onlySolider)
                //Laneclear
                if (CurrentMode == Mode.LaneClear)
                {
                    best = enemiesAround.Where(min => !ShouldWaitMinion(min))
                        .OrderByDescending(targ => targ.Health + (enemyInAzirRange(targ) ? 1000 : 0)).FirstOrDefault();
                }


            return best;
        }

        private static Obj_AI_Base GetBestHeroTarget(out bool soliderHit)
        {
            soliderHit = false;
            var target = TargetSelector.GetTarget(-1, TargetSelector.DamageType.Physical);
            if (target != null && target.IsValidTarget() && ObjectManager.Player.Distance(target) <= Player.Instance.AttackRange)
            {
                return target;
            }
            return null;
        }


        private static double CountKillhits(AIHeroClient enemy)
        {
            return enemy.Health / getRealAADmg(enemy);
        }

        private static bool ShouldWaitAllTogether()
        {
            foreach (var minion in MinionManager.GetMinions(getTargetSearchDist(), MinionTypes.All))
            {
                if (minion.IsValidTarget() && minion != null)
                {
                    var dmgAt = timeTillDamageOn(minion);
                    var hp = HealthDeath.getLaneClearPred(minion, (int)((player.AttackDelay * 1000) * 1.26f));
                    if (hp <= getRealAADmg(minion))
                        return true;
                }
            }
            return false;
        }

        private static bool ShouldWaitMinion(Obj_AI_Base minion)
        {
            if (minion == null)
            {
                return false;
            }
            var hp = HealthDeath.getLaneClearPred(minion, (int)((player.AttackDelay * 1000) * 2.26f));
            if (hp <= getRealAADmg(minion))
                return true;
            return false;
        }


        public static float getTargetSearchDist()
        {
            return player.AttackRange + player.BoundingRadius + menu.Item("runCS").GetValue<Slider>().Value + CustomRunCS;
        }

        public static int timeTillDamageOn(Obj_AI_Base unit)
        {
            var dist = unit.ServerPosition.Distance(player.ServerPosition);
            int addTime = -menu.Item("farmDelay").GetValue<Slider>().Value - ((azir) ? 100 : 0);//some farm delay
            if (!inAutoAttackRange(unit))//+ check if want to move to killabel minion and range it wants to
            {
                var realDist = realDistanceTill(unit);
                var aaRange = getRealAutoAttackRange(unit);

                addTime += (int)(((realDist - aaRange) * 1000) / player.MoveSpeed);
            }

            if (player.IsMelee || azir)
            {
                return (int)(canAttackAfter() + player.AttackCastDelay * 1000) + addTime;
            }
            else
            {
                var misDist = dist;
                return (int)(canAttackAfter() + player.AttackCastDelay * 1000 + (misDist * 1000) / player.BasicAttack.MissileSpeed) + addTime;
            }
        }

        public static bool inAutoAttackRange(AttackableUnit unit)
        {
            if (!unit.IsValidTarget() || unit == null)
            {
                return false;
            }

            if (azir && unit is Obj_AI_Base && enemyInAzirRange((Obj_AI_Base)unit))
            {
                return true;
            }

            var myRange = getRealAutoAttackRange(unit);
            return
                Vector2.DistanceSquared(
                    (unit is Obj_AI_Base) ? ((Obj_AI_Base)unit).ServerPosition.To2D() : unit.Position.To2D(),
                    player.ServerPosition.To2D()) <= myRange * myRange;
        }

        public static bool isAutoAttackReset(string name)
        {
            return AttackResets.Contains(name.ToLower());
        }

        public static bool IsAutoAttack(string name)
        {
            return (name.ToLower().Contains("attack") && !NoAttacks.Contains(name.ToLower())) ||
            Attacks.Contains(name.ToLower());
        }

        public static bool IsCantCancel(string name)
        {
            return AttacksCantCancel.Contains(name.ToLower());
        }

        public static bool inAutoAttackRange(Obj_AI_Base source, AttackableUnit target)
        {
            if (!target.IsValidTarget() || target == null)
            {
                return false;
            }

            if (source.IsMe && azir && target is Obj_AI_Base && enemyInAzirRange((Obj_AI_Base)target))
            {
                return true;
            }

            var myRange = getRealAutoAttackRange(source, target);
            return
                Vector2.DistanceSquared(
                    target.Position.To2D(),
                    source.ServerPosition.To2D()) <= myRange * myRange;
        }

        public static float getRealAutoAttackRange(AttackableUnit unit)
        {
            return getRealAutoAttackRange(player, unit);
        }

        public static float getRealAutoAttackRange(Obj_AI_Base source, AttackableUnit target)
        {
            var result = source.AttackRange + source.BoundingRadius;
            if (target.IsValidTarget() && target != null)
            {
                return result + target.BoundingRadius;
            }
            return result;
        }

        public static void moveTo(Vector3 goalPosition)
        {
            try
            {
                if (now - lastmove < CustomMoveDelay + CustomMoveDelayTemp)//Humanizer
                    return;
                if (player.ServerPosition.Distance(goalPosition) < 100)
                {
                    if (!playerStoped)
                    {
                        //Player.IssueOrder(GameObjectOrder.Stop, player.ServerPosition);
                        playerStoped = true;
                    }
                    return;
                }
                playerStoped = false;
                if (Player.IssueOrder(GameObjectOrder.MoveTo, goalPosition))
                {
                    lastAutoAttackMove = now;
                    lastmove = now;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void setAttack(bool val)
        {
            attack = val;
        }

        public static void setMovement(bool val)
        {
            move = val;
        }

        public static bool getAttack()
        {
            return attack;
        }

        public static bool getMovement()
        {
            return move;
        }

        public static bool canAttack()
        {
            return canAttackAfter() == 0 && attack && cantMoveTill < now;
        }

        public static int canAttackAfter()
        {
            int after = (int)(player.AttackDelay * 1000) + lastAutoAttack - now + menu.Item("AttDelay").GetValue<Slider>().Value + CustomAttackDelay;
            return (after > 0 ? after : 0);
        }

        public static bool canMove()
        {
            return canMoveAfter() == 0 && cantMoveTill < now && move;
        }

        private static bool hyperCharged()
        {
            return player.Buffs.Any(buffs => buffs.Name == "jaycehypercharge");
        }

        public static int canMoveAfter()
        {
            var after = lastAutoAttackMove + player.AttackCastDelay * 1000 - now + menu.Item("MovDelay").GetValue<Slider>().Value + ((hyperCharged()) ? 150 : 0);
            var aaBefore = (isTryingToAttack) ? (lastAutoAttack + 350) - now : 0;
            return (int)(after > 0 ? after : (aaBefore > 0) ? aaBefore : 0);
        }

        public static void resetAutoAttackTimer(bool safe = false)
        {
            if (!safe || canMove())
                lastAutoAttack = 0;
        }

        public static float realDistanceTill(AttackableUnit unit)
        {
            return realDistanceTill(player, unit);
        }

        public static float realDistanceTill(Obj_AI_Base source, AttackableUnit target)
        {
            float dist = 0;
            var dists = source.GetPath(target.Position);
            if (dists.Count() == 0)
                return 0;
            Vector3 from = dists[0];
            foreach (var to in dists)
            {
                dist += Vector3.Distance(from, to);
                from = to;
            }
            return dist;
        }

        public class BeforeAttackEventArgs
        {
            public AttackableUnit Target;
            public Obj_AI_Base Unit = ObjectManager.Player;
            private bool _process = true;
            public bool Process
            {
                get
                {
                    return _process;
                }
                set
                {
                    disableNextAttack = !value;
                    _process = value;
                }
            }
        }

        private static void FireBeforeAttack(AttackableUnit target)
        {
            isTryingToAttack = true;
            if (BeforeAttack != null)
            {
                BeforeAttack(new BeforeAttackEventArgs
                {
                    Target = target
                });
            }
            else
            {
                disableNextAttack = false;
            }
        }

        private static void FireAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            lastAutoAttackMove = 0;
            //set can move
            if (AfterAttack != null)
            {
                AfterAttack(unit, target);
            }
        }

        private static void FireOnUnkillable(AttackableUnit unit, AttackableUnit target, int msTillDead)
        {
            //set can move
            if (OnUnkillable != null)
            {
                OnUnkillable(unit, target, msTillDead);
            }
        }

        public static void AddToMenu(Menu menuIn)
        {
            menuIn.AddItem(new MenuItem("Combo_Key", "Combo Key").SetValue(new KeyBind(32, KeyBindType.Press)));
            menuIn.AddItem(new MenuItem("Harass_Key", "harass Key").SetValue(new KeyBind('C', KeyBindType.Press)));
            menuIn.AddItem(new MenuItem("LaneClear_Key", "LaneClear Key").SetValue(new KeyBind('V', KeyBindType.Press)));
            menuIn.AddItem(new MenuItem("LastHit_Key", "LastHir Key").SetValue(new KeyBind('X', KeyBindType.Press)));
            menuIn.AddItem(new MenuItem("AttDelay", "Attack delay").SetValue(new Slider(0, -100, 250)));
            menuIn.AddItem(new MenuItem("MovDelay", "Move delay").SetValue(new Slider(0, -100, 250)));
            menuIn.AddItem(new MenuItem("farmDelay", "Farm delay").SetValue(new Slider(25, -100, 250)));
            menuIn.AddItem(new MenuItem("runCS", "Run CS distance").SetValue(new Slider(25, 0, 500)));
            menuIn.AddItem(new MenuItem("betaStut", "Beta anti stut").SetValue(true));
            menuIn.AddItem(new MenuItem("nobar", "Disable greenBar").SetValue(false));

            menu = menuIn;

            init();

            Obj_AI_Base.OnBasicAttack += onStartAutoAttack;
            Spellbook.OnStopCast += onStopAutoAttack;
            Obj_AI_Base.OnSpellCast += onDoCast;
        }


        public static float getRealAADmg(Obj_AI_Base targ)
        {
            if (targ == null)
            {
                return 0f;
            }

            if (!azir)
                return (float)player.GetAutoAttackDamage(targ, true);
            var solAround = solidersAroundEnemy(targ);

            if (solAround == 0)
                return (float)player.GetAutoAttackDamage(targ);
            int[] solBaseDmg = { 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100, 110, 120, 130, 140, 150, 160, 170 };

            var solDmg = solBaseDmg[player.Level - 1] + player.FlatMagicDamageMod * 0.6f;

            return (float)player.CalcDamage(targ, Damage.DamageType.Magical, solDmg + (solAround - 1) * solDmg * 0.25f);

        }


        //Azir stuff

        public static int azirSoliderReach = 375;
        public static int azirSoliderRange = 325;
        //Tnx Kortatu ;)
        private static Dictionary<int, string> Animations = new Dictionary<int, string>();

        public static List<Obj_AI_Minion> azirSoldiers = new List<Obj_AI_Minion>();

        public static List<Obj_AI_Minion> getActiveSoliders()
        {
            return azirSoldiers.Where(s => s.IsValid && !s.IsMoving && !s.IsDead && !s.IsMoving && s.ServerPosition.Distance(player.Position, true) <= 875 * 875 /*(!Animations.ContainsKey(s.NetworkId) || Animations[s.NetworkId] != "Inactive")*/).ToList();
        }

        public static bool enemyInAzirRange(Obj_AI_Base ene)
        {
            if (ene == null)
            {
                return false;
            }

            var solis = getActiveSoliders();

            return !ene.IsDead && solis.Count != 0 && solis.Where(sol => !sol.IsMoving && !sol.IsDashing()).Any(sol => ene.Distance(sol, true) < azirSoliderRange * azirSoliderRange);
        }

        public static int solidersAroundEnemy(Obj_AI_Base ene)
        {
            if (ene == null)
            {
                return 0;
            }
            var solis = getActiveSoliders();

            return solis.Count(sol => ene.Distance(sol, true) < azirSoliderRange * azirSoliderRange);
        }
    }
}
