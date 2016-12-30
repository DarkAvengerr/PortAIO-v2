using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SebbyLib;
using SharpDX;
using DXColor = SharpDX.Color;
using Color = System.Drawing.Color;
using OKTWPrediction = SebbyLib.Prediction.Prediction;
using Orbwalking = LeagueSharp.Common.Orbwalking;
using FS = System.Drawing.FontStyle;
using SharpDX.Direct3D9;
using CNLib;

using EloBuddy;
using LeagueSharp.Common;
namespace Jhin_As_The_Virtuoso
{

    class Jhin
    {
        public static Menu Config { get; set; }
        public static AIHeroClient Player => HeroManager.Player;
        public static Orbwalking.Orbwalker Orbwalker { get; private set; }
        public static Spell Q { get; set; }
        public static Spell W { get; set; }
        public static Spell E { get; set; }
        public static int lastwarded { get; set; }
        public static Spell R { get; set; }
        public static bool IsCastingR => R.Instance.Name == "JhinRShot";
        public static Vector3 REndPos { get; private set; }
        public static Dictionary<int, float> PingList { get; set; } = new Dictionary<int, float>();
        public static List<AIHeroClient> KillableList { get; set; } = new List<AIHeroClient>();
        public static int[] delay => new[] {
                Config.Item("第一次延迟").GetValue<Slider>().Value,
                Config.Item("第二次延迟").GetValue<Slider>().Value,
                Config.Item("第三次延迟").GetValue<Slider>().Value
        };

        public static Items.Item BlueTrinket = new Items.Item(3342, 3500f);
        public static Items.Item ScryingOrb = new Items.Item(3363, 3500f);

        public static Font KillTextFont = new Font(Drawing.Direct3DDevice, new FontDescription
        {
            Height = 28,
            FaceName = "Microsoft YaHei",
        });

        public static Storage storage { get; set; } = new Storage("Jhin As The Virtuoso 1");
        public static bool IsChinese { get; set; } = CNLib.MultiLanguage.IsCN;

        internal static void OnLoad()
        {
            if (Player.ChampionName != "Jhin") { return; }

            LoadSpell();
            LoadMenu();
            LoadEvents();
            LastPosition.Load();
            //初始化ping时间
            foreach (var enemy in HeroManager.Enemies)
            {
                PingList.Add(enemy.NetworkId, 0);
            }

            DamageIndicator.DamageToUnit = GetRDmg;

            if (IsChinese)
            {
                Chat.Print("戏命师—烬　".ToHtml(25) + "此刻,大美将致!".ToHtml(Color.PowderBlue, FontStlye.Cite));
            }
            else
            {
                Chat.Print("Jhin As The Virtuoso　".ToHtml(25) + "Art requires a certain cruelty!".ToHtml(Color.Purple, FontStlye.Cite));
            }
        }

        private static void LoadEvents()
        {
            //
            Obj_AI_Base.OnSpellCast += Obj_AI_Base_OnSpellCast;
            //
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnWndProc += Game_OnWndProc;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Obj_AI_Base.OnLevelUp += Obj_AI_Base_OnLevelUp;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
            Orbwalking.OnNonKillableMinion += Orbwalking_OnNonKillableMinion;
            EloBuddy.Player.OnIssueOrder += Obj_AI_Base_OnIssueOrder;
            CustomEvents.Unit.OnDash += Unit_OnDash;
        }

        private static void Orbwalking_OnNonKillableMinion(AttackableUnit minion)
        {
            if (Q.IsReady() && Config.Item("补刀Q").GetValue<bool>())
            {
                Q.Cast(minion as Obj_AI_Base);
            }
        }

        private static void Unit_OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            if (sender.IsEnemy)
            {
                if (Config.Item("位移E").GetValue<bool>() && Config.GetBool("EList" + sender.NetworkId) && E.IsReady() && args.EndPos.Distance(Player) < E.Range && NavMesh.IsWallOfGrass(args.EndPos.To3D(), 10))
                {
                    E.Cast(args.EndPos);
                }

                if (Config.Item("位移W").GetValue<bool>() && Config.GetBool("WList" + sender.NetworkId) && W.IsReady() && (sender as AIHeroClient).HasWBuff() && args.EndPos.Distance(Player) < W.Range && (!E.IsReady() || args.EndPos.Distance(Player) > E.Range))
                {
                    W.Cast(args.EndPos);
                }
            }
        }

        private static void Obj_AI_Base_OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (sender.IsMe
                && IsCastingR && Config.Item("禁止移动").GetValue<bool>()
                && Player.CountEnemiesInRange(Config.Item("禁止距离").GetValue<Slider>().Value) == 0
                && HeroManager.Enemies.Any(e => e.InRCone() && !e.IsDead && e.IsValid && e.IsVisible)
            )
            {
                args.Process = false;
            }
        }

        private static void Obj_AI_Base_OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            //if (sender.IsMe)
            //{
            //	if (args.SData.Name == "JhinRShotMis")
            //	{

            //		RCharge.Index++;
            //		RCharge.CastT = Game.Time;
            //	}
            //	if (args.SData.Name == "JhinRShotMis4")
            //	{

            //		RCharge.Index = 0;
            //		RCharge.CastT = Game.Time;
            //		RCharge.Target = null;
            //	}
            //}

            if (sender.IsMe && !Orbwalking.IsAutoAttack(Args.SData.Name) && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (Args.Target is AIHeroClient && Config.Item("ComboW").GetValue<bool>() && W.IsReady())
                {
                    var target = (AIHeroClient)Args.Target;

                    if (!target.IsDead && !target.IsZombie && target.IsValidTarget(W.Range) && target.IsHPBarRendered)
                    {
                        if (target.HasWBuff() && Config.Item("标记W").GetValue<bool>())
                        {
                            W.CastSpell(target);
                            return;
                        }
                        else
                        {
                            var WPred = W.GetPrediction(target);

                            if (WPred.Hitchance >= HitChance.VeryHigh)
                            {
                                W.Cast(WPred.UnitPosition, true);
                                return;
                            }
                        }
                    }
                }
            }
        }

        private static void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo
                && !target.IsDead && target.IsValidTarget(Q.Range) && Q.IsReady())
            {
                Q.Cast(target as Obj_AI_Base);
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed
                && target?.Type == GameObjectType.AIHeroClient
                && !target.IsDead && target.IsValidTarget(Q.Range) && Q.IsReady()
                && Config.Item("消耗Q").GetValue<bool>())
            {
                Q.Cast(target as Obj_AI_Base);
            }
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {

            if (Config.Item("打断E").GetValue<bool>() && sender.IsEnemy && E.CanCast(sender))
            {
                if (sender.ChampionName == "Thresh")
                {
                    return;
                }
                E.Cast(sender);
            }
        }

        private static void Obj_AI_Base_OnLevelUp(Obj_AI_Base sender, EventArgs args)
        {
            if (!sender.IsMe) return;

            if (Config.Item("自动加点").GetValue<bool>() && Player.Level >= Config.Item("加点等级").GetValue<Slider>().Value)
            {
                int Delay = Config.Item("加点延迟").GetValue<Slider>().Value;

                if (Player.Level == 6 || Player.Level == 11 || Player.Level == 16)
                {
                    Player.Spellbook.LevelSpell(SpellSlot.R);
                }

                if (Q.Level == 0)
                {
                    Player.Spellbook.LevelSpell(SpellSlot.Q);
                }
                else if (W.Level == 0)
                {
                    Player.Spellbook.LevelSpell(SpellSlot.W);
                }
                else if (E.Level == 0)
                {
                    Player.Spellbook.LevelSpell(SpellSlot.E);
                }

                if (Config.Item("加点方案").GetValue<StringList>().SelectedIndex == 0)//主Q副W
                {
                    DelayLevels(Delay, SpellSlot.Q);
                    DelayLevels(Delay + 50, SpellSlot.W);
                    DelayLevels(Delay + 100, SpellSlot.E);
                }
                else if (Config.Item("加点方案").GetValue<StringList>().SelectedIndex == 1)//主Q副E
                {
                    DelayLevels(Delay, SpellSlot.Q);
                    DelayLevels(Delay + 50, SpellSlot.E);
                    DelayLevels(Delay + 100, SpellSlot.W);
                }
                else if (Config.Item("加点方案").GetValue<StringList>().SelectedIndex == 2)//主W副Q
                {
                    DelayLevels(Delay, SpellSlot.W);
                    DelayLevels(Delay + 50, SpellSlot.Q);
                    DelayLevels(Delay + 100, SpellSlot.E);
                }
                else if (Config.Item("加点方案").GetValue<StringList>().SelectedIndex == 3)//主W副E
                {
                    DelayLevels(Delay, SpellSlot.W);
                    DelayLevels(Delay + 50, SpellSlot.E);
                    DelayLevels(Delay + 100, SpellSlot.Q);
                }
                else if (Config.Item("加点方案").GetValue<StringList>().SelectedIndex == 4)//主E副Q
                {
                    DelayLevels(Delay, SpellSlot.E);
                    DelayLevels(Delay + 50, SpellSlot.Q);
                    DelayLevels(Delay + 100, SpellSlot.W);
                }
                else if (Config.Item("加点方案").GetValue<StringList>().SelectedIndex == 5)//主E副W
                {
                    DelayLevels(Delay, SpellSlot.E);
                    DelayLevels(Delay + 50, SpellSlot.W);
                    DelayLevels(Delay + 100, SpellSlot.Q);
                }
            }

            if (!Config.Item("自动加点").GetValue<bool>() && Config.Item("自动点大").GetValue<bool>()
                && (Player.Level == 6 || Player.Level == 11 || Player.Level == 16))
            {
                Player.Spellbook.LevelSpell(SpellSlot.R);
            }
        }

        public static void DelayLevels(int time, SpellSlot QWER)
        {
            LeagueSharp.Common.Utility.DelayAction.Add(time, () => { Player.Spellbook.LevelSpell(QWER); });
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Config.Item("防突E").GetValue<bool>() && Config.GetBool("EList" + gapcloser.Sender.NetworkId))
            {
                E.Cast(gapcloser.End);
            }
            if (Config.Item("防突W").GetValue<bool>() && Config.GetBool("WList" + gapcloser.Sender.NetworkId) && gapcloser.Sender.HasWBuff())
            {
                W.CastSpell(gapcloser.Sender);
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name == "JhinRShotMis")
                {

                    RCharge.Index++;
                    RCharge.CastT = Game.Time;

                    if (Config.GetBool("调试"))
                    {
                        DeBug.Debug("[OnProcessSpellCast]", $"使用技能R。是否正在放R{IsCastingR}");
                    }
                }
                if (args.SData.Name == "JhinRShotMis4")
                {

                    RCharge.Index = 0;
                    RCharge.CastT = Game.Time;
                    RCharge.Target = null;

                    if (Config.GetBool("调试"))
                    {
                        DeBug.Debug("[OnProcessSpellCast]", $"使用技能R。是否正在放R{IsCastingR}");
                    }
                }
            }

            if (sender.IsMe && args.SData.Name == "JhinR")
            {
                REndPos = args.End;
                if (Config.Item("R放眼").GetValue<bool>()
                    && (ScryingOrb.IsReady())
                    && HeroManager.Enemies.All(e => !e.InRCone() || !e.IsValid || e.IsDead))
                {
                    var bushList = VectorHelper.GetBushInRCone();
                    var lpl = VectorHelper.GetLastPositionInRCone();
                    if (bushList?.Count > 0)
                    {
                        if (lpl?.Count > 0)
                        {
                            var lp = lpl.First(p => Game.Time - p.LastSeen > 2 * 1000);
                            if (lp != null)
                            {
                                var bush = VectorHelper.GetBushNearPosotion(lp.LastPosition, bushList);
                                ScryingOrb.Cast(bush);
                            }

                        }
                        else
                        {
                            var bush = VectorHelper.GetBushNearPosotion(REndPos, bushList);
                            ScryingOrb.Cast(bush);
                        }

                    }
                    else if (lpl?.Count > 0)
                    {
                        ScryingOrb.Cast(lpl.First().LastPosition);
                    }
                }
            }

            if (sender.IsMe && args.Slot == SpellSlot.W && Config.GetBool("调试"))
            {
                DeBug.Debug("[OnProcessSpellCast]", $"使用技能W。是否正在放R{IsCastingR}");
            }
        }

        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (args.WParam == Config.Item("半手动R自动").GetValue<KeyBind>().Key && IsCastingR)
            {
                args.Process = false;
            }

            if (!MenuGUI.IsChatOpen && args.WParam == Config.Item("半手动R自动").GetValue<KeyBind>().Key && !IsCastingR && R.IsReady() && RCharge.Target == null)
            {
                var t = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);
                if (t != null && t.IsValid && R.CastSpell(t))
                {
                    args.Process = false;
                    RCharge.Target = t;
                }

            }

        }

        private static AIHeroClient GetTargetInR()
        {
            var ignoredList = new List<AIHeroClient>();
            foreach (var enemy in HeroManager.Enemies.Where(e => !e.IsValid || e.IsDead || !e.InRCone()))
            {
                ignoredList.Add(enemy);
            }
            var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical, true, ignoredList);
            if (target != null && target.IsValid && !target.IsDead)
            {
                return target;
            }
            return null;
        }

        private static void Game_OnUpdate(EventArgs args)
        {

            #region 击杀列表 及 击杀信号提示
            foreach (var enemy in HeroManager.Enemies)
            {
                if (R.CanCast(enemy) && !enemy.IsDead && enemy.IsValid && GetRDmg(enemy) >= enemy.Health)
                {
                    if (!KillableList.Contains(enemy))
                    {
                        KillableList.Add(enemy);
                    }

                    if (Config.Item("击杀信号提示").GetValue<bool>() && Game.Time - PingList[enemy.NetworkId] > 10 * 1000)
                    {
                        TacticalMap.ShowPing(PingCategory.AssistMe, enemy, true);
                        TacticalMap.ShowPing(PingCategory.AssistMe, enemy, true);
                        TacticalMap.ShowPing(PingCategory.AssistMe, enemy, true);
                        PingList[enemy.NetworkId] = Game.Time;
                    }
                }
                else
                {
                    if (KillableList.Contains(enemy))
                    {
                        KillableList.Remove(enemy);
                    }
                }
            }
            #endregion

            #region 其它设置，买蓝眼

            if (Config.Item("买蓝眼").GetValue<bool>() && !ScryingOrb.IsOwned() && (Player.InShop() || Player.InFountain()) && Player.Level >= 9)
            {
                Shop.BuyItem(ItemId.Farsight_Alteration);
            }
            #endregion

            #region 提前结束R时 重置大招次数及目标
            if (!IsCastingR && !R.IsReady())
            {
                RCharge.Index = 0;
                RCharge.Target = null;
            }
            #endregion

            if (!IsCastingR && RCharge.Index == 0)
            {
                QLogic();
                WLogic();
                ELogic();
            }
            RLogic();
        }

        private static void ELogic()
        {
            #region E逻辑
            foreach (var enemy in HeroManager.Enemies)
            {
                #region 硬控E
                if (enemy.IsValidTarget(E.Range + 30) && Config.Item("硬控E").GetValue<bool>() && !OktwCommon.CanMove(enemy))
                {
                    E.CastSpell(enemy);
                }
                #endregion

                #region 探草E
                if (enemy.IsDead) continue;
                var path = enemy.Path.ToList().LastOrDefault();
                if (!NavMesh.IsWallOfGrass(path, 1)) continue;
                if (enemy.Distance(path) > 200) continue;
                if (NavMesh.IsWallOfGrass(HeroManager.Player.Position, 1) && HeroManager.Player.Distance(path) < 200) continue;

                if (Environment.TickCount - lastwarded > 1000)
                {
                    if (E.IsReady() && HeroManager.Player.Distance(path) < E.Range)
                    {
                        E.Cast(path);
                        lastwarded = Environment.TickCount;
                    }
                }
                #endregion
            }
            #endregion
            //清兵
            if (Config.Item("清兵E").GetValue<bool>() && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                var minions = MinionManager.GetMinions(E.Range + E.Width, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth);
                if (minions?.Count > 5)
                {
                    var eClear = E.GetCircularFarmLocation(minions, E.Width);
                    if (eClear.MinionsHit >= 3)
                    {
                        E.Cast(eClear.Position);
                    }
                }

            }
        }

        private static void WLogic()
        {
            #region W逻辑
            foreach (var enemy in HeroManager.Enemies.Where(e => e.IsValid && !e.IsDead && e.Distance(Player) < W.Range).OrderByDescending(k => k.Distance(Player)).OrderByDescending(k => k.Health))
            {
                if (Config.Item("标记W").GetValue<bool>()
                    && Config.GetBool("WList" + enemy.NetworkId)
                    && (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                    && enemy.CountAlliesInRange(650) > 0
                    && enemy.HasWBuff())
                {
                    W.CastSpell(enemy);
                }

                if (Config.Item("硬控W").GetValue<bool>() && !OktwCommon.CanMove(enemy) && enemy.HasWBuff())
                {
                    W.CastSpell(enemy);
                }

                if (Config.Item("抢人头W").GetValue<bool>()
                    && Config.GetBool("WList" + enemy.NetworkId)
                    && Player.CountEnemiesInRange(Player.AttackRange + 100) == 0 && enemy.Health < OktwCommon.GetIncomingDamage(enemy) + W.GetDmg(enemy)
                    && !Q.CanCast(enemy) && !(Orbwalking.CanAttack() && Orbwalking.InAutoAttackRange(enemy)))
                {
                    W.CastSpell(enemy);
                }

            }
            #endregion
        }

        private static void QLogic()
        {
            #region Q逻辑
            //Q消耗
            if (Config.Item("消耗Q兵").GetValue<bool>() && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                var ms = MinionManager.GetMinions(Q.Range);
                if (ms != null && ms.Count > 0)
                {
                    var t = ms.Find(m => Q.GetDmg(m) > m.Health && m.CountEnemiesInRange(200) > 0);
                    if (t != null)
                    {
                        Q.Cast(t);
                    }
                }
            }

            if (Config.Item("清兵Q").GetValue<bool>() && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                var ms = MinionManager.GetMinions(Q.Range);
                if (ms != null && ms.Count >= 3)
                {
                    var t = ms.Find(m => Q.GetDmg(m) > m.Health);
                    if (t != null)
                    {
                        Q.Cast(t);
                    }
                }
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                var target = Orbwalker.GetTarget() as AIHeroClient;
                if (target != null && target.IsValid && !Orbwalking.CanAttack() && !Player.Spellbook.IsAutoAttacking && target.Health < Q.GetDmg(target) + W.GetDmg(target))
                {
                    Q.Cast(target);
                }
            }

            //Q抢人头
            foreach (var enemy in HeroManager.Enemies.Where(e => e.IsValidTarget(Q.Range) && Q.GetDmg(e) + OktwCommon.GetIncomingDamage(e) > e.Health))
            {
                Q.Cast(enemy);
            }

            #endregion
        }

        private static void RLogic()
        {
            if (!IsCastingR && Config.GetBool("自动R"))
            {
                var target = TargetSelector.GetTarget(R.Range - 400, TargetSelector.DamageType.Physical);
                if (target != null && target.IsValid && !target.IsDead
                    && GetRDmg(target) > target.Health
                    && OktwCommon.ValidUlt(target) && !OktwCommon.IsSpellHeroCollision(target, R)
                    && target.CountAlliesInRange(600) == 0
                    && LastPosition.GetLastPositionsInRange(Player, 1000, 4000).Count == 0
                    && !Player.UnderTurret(true)
                    && !target.InFountain())
                {
                    if (R.CastSpell(target))
                    {
                        RCharge.Target = target;
                    }
                }
            }

            #region 自动R逻辑

            if (IsCastingR)
            {
                if (Config.Item("R放眼").GetValue<bool>() && ScryingOrb.IsReady())
                {
                    var pistionList = VectorHelper.GetLastPositionInRCone().Where(m => !m.Hero.IsVisible && !m.Hero.IsDead && Game.Time - m.LastSeen < 7 * 1000).OrderByDescending(m => m.LastSeen);

                    if (RCharge.Target == null && pistionList.Count() > 0)
                    {
                        var MissPosition = pistionList.First();
                        var MostNearBush = VectorHelper.GetBushNearPosotion(MissPosition.LastPosition);
                        if (MostNearBush != Vector3.Zero && MostNearBush.Distance(MissPosition.LastPosition) < 500)
                        {
                            ScryingOrb.Cast(MostNearBush);
                        }
                        else
                        {
                            ScryingOrb.Cast(MissPosition.LastPosition);
                        }
                    }
                    else if (RCharge.Target != null && !RCharge.Target.IsVisible && !RCharge.Target.IsDead)
                    {
                        var RTargetLastPosition = pistionList?.Find(m => m.Hero == RCharge.Target && Game.Time - m.LastSeen < 3 * 1000);
                        if (RTargetLastPosition != null)
                        {
                            var MostNearBush = VectorHelper.GetBushNearPosotion(RTargetLastPosition.LastPosition);
                            if (MostNearBush.Distance(RTargetLastPosition.LastPosition) < 500)
                            {
                                ScryingOrb.Cast(MostNearBush);
                            }
                            else
                            {
                                ScryingOrb.Cast(RTargetLastPosition.LastPosition);
                            }
                        }
                    }
                }

                var target = GetTargetInR();
                if (target != null)
                {
                    #region 使用R，并记录R目标和施放时间
                    if (RCharge.Index == 0)
                    {
                        if (R.CastSpell(target))
                        {
                            RCharge.Target = target;
                        }
                    }
                    else
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(delay[RCharge.Index - 1], () =>
                        {
                            if (R.CastSpell(target))
                            {
                                RCharge.Target = target;
                            }
                        });
                    }
                    #endregion
                }
            }

            #endregion
        }

        private static float GetRDmg(Obj_AI_Base target)
        {
            var damage = (-25 + 75 * R.Level + 0.2 * Player.FlatPhysicalDamageMod) * (1 + (100 - target.HealthPercent) * 0.02);
            return (IsCastingR || R.IsReady()) ? (5 - RCharge.Index) * (float)damage : 0;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            #region 范围显示
            var ShowW = Config.Item("W范围").GetValue<Circle>();
            var ShowE = Config.Item("E范围").GetValue<Circle>();
            var ShowR = Config.Item("R范围").GetValue<Circle>();
            var ShowWM = Config.Item("小地图W范围").GetValue<bool>();
            var ShowRM = Config.Item("小地图R范围").GetValue<bool>();

            if (W.IsReady() && ShowW.Active)
            {
                Render.Circle.DrawCircle(Player.Position, W.Range, ShowW.Color, 2);
            }
            if (W.IsReady() && ShowWM)
            {
                LeagueSharp.Common.Utility.DrawCircle(Player.Position, W.Range, ShowW.Color, 2, 30, true);
            }

            if (R.IsReady() && ShowR.Active)
            {
                Render.Circle.DrawCircle(Player.Position, R.Range, ShowR.Color, 2);
            }
            if (R.IsReady() && ShowRM)
            {
                LeagueSharp.Common.Utility.DrawCircle(Player.Position, R.Range, ShowR.Color, 2, 30, true);
            }

            if (E.IsReady() && ShowE.Active)
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, ShowE.Color, 2);
            }
            #endregion

            var ShowD = Config.Item("大招伤害").GetValue<Circle>();
            DamageIndicator.Enabled = ShowD.Active;
            DamageIndicator.Color = ShowD.Color;

            var ShowT = Config.Item("击杀文本提示").GetValue<Circle>();
            if (ShowT.Active && KillableList?.Count > 0)
            {
                var killname = "R击杀名单\n";
                foreach (var k in KillableList)
                {
                    killname += (k.Name + "　").ToGBK() + $"({k.ChampionName.ToCN(IsChinese)})\n";
                }

                var KillTextColor = new ColorBGRA
                {
                    A = Config.Item("击杀文本提示").GetValue<Circle>().Color.A,
                    B = Config.Item("击杀文本提示").GetValue<Circle>().Color.B,
                    G = Config.Item("击杀文本提示").GetValue<Circle>().Color.G,
                    R = Config.Item("击杀文本提示").GetValue<Circle>().Color.R,
                };

                KillTextFont.DrawText(null, killname,
                    (int)(Drawing.Width * ((float)Config.Item("击杀文本X").GetValue<Slider>().Value / 100)),
                    (int)(Drawing.Height * ((float)Config.Item("击杀文本Y").GetValue<Slider>().Value / 100)),
                    KillTextColor);
            }

        }

        private static void LoadSpell()
        {
            Q = new Spell(SpellSlot.Q, 600);
            W = new Spell(SpellSlot.W, 2500);
            E = new Spell(SpellSlot.E, 760);
            R = new Spell(SpellSlot.R, 3500);

            W.SetSkillshot(0.75f, 40, float.MaxValue, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(1.3f, 200, 1600, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.2f, 80, 5000, false, SkillshotType.SkillshotLine);
        }

        private static void LoadMenu()
        {

            Config = new Menu(IsChinese ? "戏命师 - 烬" : "Jhin As The Virtuoso", "JhinAsTheVirtuoso", true);
            Config.AddToMainMenu();

            Config.AddBool("调试", "调试");
            Config.AddSeparator();
            Config.AddLabel("你付了钱，就好好看戏吧").SetFontStyle(FS.Bold, DXColor.PapayaWhip);

            var OMenu = Config.AddMenu("走砍设置", "走砍设置");
            Orbwalker = new Orbwalking.Orbwalker(OMenu);

            //Q菜单
            var QMenu = Config.AddMenu("Q设置", "Q设置");
            QMenu.AddBool("消耗Q兵", "可Q死小兵时消耗", true);
            QMenu.AddBool("消耗Q", "一直用Q消耗", true);
            QMenu.AddBool("清兵Q", "使用Q清兵", true);
            QMenu.AddBool("补刀Q", "使用Q补刀", false);
            QMenu.AddBool("抢人头Q", "Q抢人头", true);

            //W菜单
            var WMenu = Config.AddMenu("W设置", "W设置");
            WMenu.AddBool("ComboW", "Use W In Combo", true);
            WMenu.AddBool("硬控W", "自动W硬控敌人", true);
            WMenu.AddBool("标记W", "W有标记的敌人", true);
            WMenu.AddBool("抢人头W", "W抢人头", true);
            WMenu.AddBool("防突W", "W有标记的突进", true);
            WMenu.AddBool("位移W", "敌人位移W", true);
            var WListMenu = WMenu.AddMenu("W名单", "W名单");
            foreach (var enemy in HeroManager.Enemies)
            {
                //设置中英名
                WListMenu.AddBool("WList" + enemy.NetworkId, enemy.ChampionName.ToCN(IsChinese), true);
            }

            //E菜单
            var EMenu = Config.AddMenu("E设置", "E设置");
            EMenu.AddBool("硬控E", "自动E硬控敌人", true);
            EMenu.AddBool("防突E", "自动E防突进", true);
            EMenu.AddBool("打断E", "自动E持续技能敌人", true);
            EMenu.AddBool("探草E", "敌人进草自动E", true);
            EMenu.AddBool("位移E", "敌人位移到看不到的地方E", true);
            EMenu.AddBool("清兵E", "使用E清兵", true);

            var EListMenu = EMenu.AddMenu("E名单", "E名单");
            EListMenu.AddLabel("E名单只适用于防突进/位移");
            foreach (var enemy in HeroManager.Enemies)
            {
                //设置中英名
                EListMenu.AddBool("EList" + enemy.NetworkId, enemy.ChampionName.ToCN(IsChinese), true);
            }

            //R菜单
            var RMenu = Config.AddMenu("R设置", "R设置");
            RMenu.AddLabel("移动设置").SetFontStyle(FS.Bold, DXColor.Orange);
            RMenu.AddBool("禁止移动", "R时禁止移动和攻击", true);
            RMenu.AddSlider("禁止距离", "当?码敌人靠近解除禁止", 700, 0, (int)R.Range);
            RMenu.AddSeparator();

            RMenu.AddLabel("击杀提示设置").SetFontStyle(FS.Bold, DXColor.Orange);
            RMenu.AddCircle("击杀文本提示", "文字提示R可击杀目标", true, Color.Orange);
            RMenu.AddSlider("击杀文本X", "文字提示横向位置", 71);
            RMenu.AddSlider("击杀文本Y", "文字提示纵向位置", 86);
            RMenu.AddBool("击杀信号提示", "信号提示R可击杀目标(本地)", true);
            RMenu.AddSeparator();

            RMenu.AddLabel("自动R设置").SetFontStyle(FS.Bold, DXColor.Orange);
            RMenu.AddBool("自动R", "自动R", true);
            RMenu.AddSeparator();

            RMenu.AddLabel("半手动R设置").SetFontStyle(FS.Bold, DXColor.Orange);
            RMenu.AddKeyBind("半手动R自动", "半手动R(自动R)", 'R', KeyBindType.Press);
            RMenu.AddSlider("第一次延迟", "第一次R后延迟(毫秒)", 0, 0, 1000);
            RMenu.AddSlider("第二次延迟", "第二次R后延迟(毫秒)", 0, 0, 1000);
            RMenu.AddSlider("第三次延迟", "第三次R后延迟(毫秒)", 0, 0, 1000);
            RMenu.AddSeparator();

            RMenu.AddBool("R放眼", "R时无视野放蓝眼", true);

            //其它菜单
            var MMenu = Config.AddMenu("其它设置", "其它设置");
            MMenu.AddLabel("自动加点设置").SetFontStyle(FS.Bold, DXColor.Orange);
            MMenu.AddBool("自动点大", "只自动学大");
            MMenu.AddBool("自动加点", "自动加点", true);
            MMenu.AddSlider("加点等级", "从几级开始加点", 2, 1, 6);
            MMenu.AddSlider("加点延迟", "加点延迟", 700, 0, 2000);
            MMenu.AddStringList("加点方案", "加点方案", new[] { "主Q副W", "主Q副E", "主W副Q", "主W副E", "主E副Q", "主E副W" });

            MMenu.AddSeparator();
            MMenu.AddBool("买蓝眼", "9级时自动买蓝眼", true);

            //显示菜单
            var DMenu = Config.AddMenu("显示设置", "显示设置");
            DMenu.AddLabel("范围显示").SetFontStyle(FS.Bold, DXColor.Orange);
            DMenu.AddCircle("W范围", "显示W范围", true, Color.Blue);
            DMenu.AddBool("小地图W范围", "小地图显示W范围", true);
            DMenu.AddCircle("E范围", "显示E范围", true, Color.Yellow);
            DMenu.AddCircle("R范围", "显示R范围", true, Color.YellowGreen);
            DMenu.AddBool("小地图R范围", "小地图显示R范围", true);
            DMenu.AddSeparator();

            DMenu.AddLabel("伤害提示").SetFontStyle(FS.Bold, DXColor.Orange);
            DMenu.AddCircle("大招伤害", "显示四次大招后伤害", true, Color.Red);

        }

    }
}
