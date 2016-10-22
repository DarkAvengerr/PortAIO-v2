using EloBuddy;
using LeagueSharp.SDK;
namespace Tc_SDKexAIO.Champions
{
    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;
    using LeagueSharp.SDK.TSModes;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.Data.Enumerations;


    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Keys = System.Windows.Forms.Keys;

    using SharpDX;

    using Common;

    using Common.Evade;

    using Config;

    using Color = System.Drawing.Color;
    using static Common.Manager;

    internal static class LeeSin
    {
        private const int FlashRange = 425, IgniteRange = 600, SmiteRange = 570, RKickRange = 750, WardRange = 600;
        private const int DistWard = 230, DistFlash = 130;
        private static SpellSlot Flash, Ignite, Smite;
        private static Menu Menu => PlaySharp.ChampionMenu;
        private static Spell Q, Q2, W, E, E2, R, R2, R3;
        private static AIHeroClient Player => PlaySharp.Player;
        private static Obj_AI_Base objQ, lastObjQ;
        private static HpBarDraw HpBarDraw = new HpBarDraw();
        private static Vector3 posBubbaKush, posBubbaKushFlash, posBubbaKushJump;
        private static Vector3 lastEndPos, lastFlashPos, lastPlacePos;
        private static bool isDashing, IsWardFlash;
        private static int lastW, lastW2, lastE2, lastR, cPassive, LastRFlashTime;
        private static int lastInsecTime, lastMoveTime, lastFlashRTime, lastPlaceTime;
        private static int LastInsecWardTime, LastInsecJumpTme;
        private static readonly List<string> SpecialPet = new List<string> { "jarvanivstandard", "teemomushroom", "illaoiminion" };


        internal static bool IsDashing => (lastW > 0 && Variables.TickCount - lastW <= 100) || Player.IsDashing();

        internal static bool IsEOne => E.Instance.SData.Name.ToLower().Contains("one");

        internal static bool IsQOne => Q.Instance.SData.Name.ToLower().Contains("one");

        internal static bool IsRecentR => Variables.TickCount - lastR < 2500;

        internal static bool IsWOne => W.Instance.SData.Name.ToLower().Contains("one");

        internal static bool CanInsec => (CanWardJump || (Menu["Insec"]["Flash"] && Flash.IsReady()) || IsRecent) && R.IsReady();

        internal static bool CanWardFlash => Menu["Insec"]["Flash"] && Menu["Insec"]["FlashJump"] && CanWardJump && Flash.IsReady();

        internal static bool IsRecent => IsRecentWardJump || (Menu["Insec"]["Flash"] && Variables.TickCount - lastFlashRTime < 5000);

        internal static bool IsRecentWardJump => Variables.TickCount - LastInsecWardTime < 5000 || Variables.TickCount - LastInsecJumpTme < 5000;

        internal static float RangeNormal => CanWardJump || IsRecentWardJump ? WardRange - DistWard : FlashRange - DistFlash;

        internal static float RangeWardFlash => WardRange + R.Range - 100;

        internal static bool CanWardJump => CanCastWard && W.IsReady() && IsWOne;

        internal static bool CanCastWard => Variables.TickCount - lastPlaceTime > 1250 && Items.GetWardSlot() != null;

        internal static bool IsTryingToJump => lastPlacePos.IsValid() && Variables.TickCount - lastPlaceTime < 1250;

        internal static void Init()
        {
            var smiteName = Player.Spellbook.Spells.Where(i => (i.Slot == SpellSlot.Summoner1 || i.Slot == SpellSlot.Summoner2) && i.Name.ToLower().Contains("smite")).Select(i => i.Name).FirstOrDefault();

            if (!string.IsNullOrEmpty(smiteName))
            {
                Smite = Player.GetSpellSlot(smiteName);
            }

            Ignite = Player.GetSpellSlot("SummonerDot");
            Flash = Player.GetSpellSlot("SummonerFlash");

            Q = new Spell(SpellSlot.Q, 1100).SetSkillshot(0.25f, 60, 1800, true, SkillshotType.SkillshotLine);
            Q2 = new Spell(Q.Slot, 1300);
            W = new Spell(SpellSlot.W, 700);
            E = new Spell(SpellSlot.E, 425).SetTargetted(0.25f, float.MaxValue);
            E2 = new Spell(E.Slot, 570);
            R = new Spell(SpellSlot.R, 375).SetTargetted(0.275f, float.MaxValue);
            R2 = new Spell(R.Slot, RKickRange).SetSkillshot(0.325f, 0, 950, false, SkillshotType.SkillshotLine);
            R3 = new Spell(R.Slot, R.Range).SetSkillshot(R2.Delay, 0, R2.Speed, false, R2.Type);
            Q.DamageType = Q2.DamageType = W.DamageType = R.DamageType = DamageType.Physical;
            E.DamageType = DamageType.Magical;
            Q.MinHitChance = R2.MinHitChance = HitChance.VeryHigh;

            var KeyMenu = Menu.Add(new Menu("Key", "????"));
            {
                KeyMenu.GetKeyBind("Star", "???????", Keys.T);
                KeyMenu.GetKeyBind("FleeW", "???? ", Keys.X);
                KeyMenu.GetKeyBind("RAuto", "??R??", Keys.L, KeyBindType.Toggle);
                KeyMenu.GetKeyBind("RFlash", "R????????", Keys.Y);
                KeyMenu.GetKeyBind("Insec", "?????", Keys.R);
                KeyMenu.GetKeyBind("RAdv", "??R??", Keys.A);
            }

            var QMenu = Menu.Add(new Menu("QMenu", "Q ????"));
            {
                QMenu.GetSeparator("?? ??");
                QMenu.GetBool("ComboQ", "??Q1");
                QMenu.GetBool("ComboQ2", "??Q2");
                QMenu.GetBool("ComboQ2Obj", "?Q2 ????????", false);
                QMenu.GetBool("ComboQCol", "?? + Q");
                QMenu.GetBool("ComboStarKill", "???????????????? Q", false);
                QMenu.GetSeparator("?? ??");
                QMenu.GetBool("JungleQ", "?? Q !");
                QMenu.GetBool("JungleQBig", "??????????");
                QMenu.GetSeparator("?? ??");
                QMenu.GetBool("LastHitQ", "??Q !");
                QMenu.GetSeparator("?? ??");
                QMenu.GetBool("KillStealQ", "??Q !");
                QMenu.GetBool("KillStealQ2", "?? Q2!");
                QMenu.GetSeparator("??? ??");
                QMenu.GetBool("InsecQ", "??Q !");
                QMenu.GetBool("InsecQCol", "?? +Q");
                QMenu.GetBool("InsecQObj", " Q ???????");
            }

            var WMenu = Menu.Add(new Menu("WMenu", "W ????"));
            {
                WMenu.GetSeparator("?? ??");
                WMenu.GetBool("ComboW", "?? W !", false);
                WMenu.GetBool("ComboW2", "???? W2!", false);
                WMenu.GetBool("StarKillWJ", "????????", false);
                WMenu.GetBool("LaneJungleW", "????W !", false);
            }

            var EMenu = Menu.Add(new Menu("EMenu", "E ????"));
            {
                EMenu.GetSeparator("?? ??");
                EMenu.GetBool("ComboE", "?? E !");
                EMenu.GetBool("ComboE2", "??E2 ??");
                EMenu.GetSeparator("?? ??");
                EMenu.GetBool("LaneJungleE", "??E ?? !");
                EMenu.GetSeparator("?? ??");
                EMenu.GetBool("KSE", "?? !");
            }

            var RMenu = Menu.Add(new Menu("RMenu", "R ????"));
            {
                RMenu.GetBool("KSR", " ??R ??");
                RMenu.GetBool("RAutoKill", "???R????????");
                RMenu.GetSlider("RAutoCountA", "??????? >=", 1, 1, 4);
                RMenu.Add(new MenuSeparator("RLSITST", "R????"));
                if (GameObjects.EnemyHeroes.Any())
                {
                    GameObjects.EnemyHeroes.ForEach(i => RMenu.GetBool("RCast" + i.ChampionName, i.ChampionName, AutoEnableList.Contains(i.ChampionName)));
                }
            }

            var Insec = Menu.Add(new Menu("Insec", "??????"));
            {
                Insec.GetBool("TargetSelect", "??????????", false);
                Insec.GetList("Mode", "?????", new[] { "??/?? ??", "????" });
                Insec.GetBool("Flash", "????");
                Insec.GetList("FlashMode", "????", new[] { "R > ?", "? > R", "??" });
                Insec.GetBool("FlashJump", "???????? R ???");
            }

            var drawMenu = Menu.Add(new Menu("Draw", "????"));
            {
                drawMenu.GetBool("Q", "Q ??", false);
                drawMenu.GetBool("W", "W ??", false);
                drawMenu.GetBool("E", "E ??", false);
                drawMenu.GetBool("R", "R ??", false);
                drawMenu.GetBool("KnockUp", "??R ????");
                drawMenu.GetBool("DLine", "?????");
                drawMenu.GetBool("DWardFlash", " ?????");
            }

            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Drawing.OnEndScene += OnEndScene;
            Game.OnUpdate += delegate
            {
                if (cPassive == 0 || cPassive == 1)
                {
                    return;
                }

                var count = Player.GetBuffCount("BlindMonkFlurry");

                if (count < cPassive)
                {
                    cPassive = count;
                }

                if (lastInsecTime > 0 && Variables.TickCount - lastInsecTime > 5000)
                {
                    CleanData();
                }

                if (lastMoveTime > 0 && Variables.TickCount - lastMoveTime > 1000 && !R.IsReady())
                {
                    lastMoveTime = 0;
                }

                if (lastPlacePos.IsValid() && Variables.TickCount - lastPlaceTime > 1500)
                {
                    lastPlacePos = new Vector3();
                }

                if (Player.IsDead)
                {
                    return;
                }

                if (IsTryingToJump)
                {
                    Jump(lastPlacePos);
                }
            };
            Obj_AI_Base.OnSpellCast += OnSpellCast;
            Obj_AI_Base.OnBuffGain += OnBuffAdd;
            Obj_AI_Base.OnBuffLose += OnBuffLose;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;

            Obj_AI_Base.OnProcessSpellCast += (sender, args) =>
            {
                if (!sender.IsMe || !Menu["Key"]["Insec"].GetValue<MenuKeyBind>().Active || !lastFlashPos.IsValid() || args.SData.Name != "SummonerFlash" || !Menu["Insec"]["Flash"] || Variables.TickCount - lastFlashRTime > 1250 || args.End.Distance(lastFlashPos) > 150)
                {
                    return;
                }

                lastFlashRTime = Variables.TickCount;

                var target = Variables.TargetSelector.GetSelectedTarget();

                if (target.IsValidTarget())
                {
                    DelayAction.Add(5, () => R.CastOnUnit(target));
                }
            };

            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;

            Obj_AI_Base.OnBuffGain += (sender, args) =>
            {
                if (!sender.IsEnemy || args.Buff.DisplayName != "BlindMonkSonicWave")
                {
                    return;
                }
                lastObjQ = sender;
            };

            Obj_AI_Base.OnBuffUpdate += OnBuffUpdateCount;
            GameObjectNotifier<Obj_AI_Minion>.OnCreate += OnCreate;
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!lastPlacePos.IsValid() || !sender.IsMe || args.Slot != SpellSlot.W || !args.SData.Name.ToLower().Contains("one"))
            {
                return;
            }
            var ward = args.Target as Obj_AI_Minion;

            if (ward == null || !ward.IsValid() || ward.Distance(lastPlacePos) > 150)
            {
                return;
            }
            var tick = Variables.TickCount;

            if (tick - LastInsecJumpTme < 1250)
            {
                LastInsecJumpTme = tick;
            }

            IsWardFlash = false;
            lastPlacePos = new Vector3();
        }

        private static void OnCreate(object sender, Obj_AI_Minion minion)
        {
            if (!lastPlacePos.IsValid() || minion.Distance(lastPlacePos) > 150 || !minion.IsAlly || !minion.IsWard() || !W.IsInRange(minion))
            {
                return;
            }

            var tick = Variables.TickCount;

            if (tick - LastInsecWardTime < 1250)
            {
                LastInsecWardTime = tick;
            }

            if (tick - lastPlaceTime < 1250 && W.IsReady() && IsWOne && W.CastOnUnit(minion))
            {
                lastW = tick;
            }
        }

        private static void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe || args.Slot != SpellSlot.R)
            {
                return;
            }
            CleanData();
        }

        private static void OnEndScene(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }
            if (Menu["Draw"]["Q"] && Q.Level > 0)
            {
                Render.Circle.DrawCircle(
                    Player.Position,
                    (IsQOne ? Q : Q2).Range,
                    Q.IsReady() ? Color.LimeGreen : Color.IndianRed);
            }
            if (Menu["Draw"]["W"] && W.Level > 0 && IsWOne)
            {
                Render.Circle.DrawCircle(Player.Position, W.Range, W.IsReady() ? Color.LimeGreen : Color.IndianRed);
            }
            if (Menu["Draw"]["E"] && E.Level > 0)
            {
                Render.Circle.DrawCircle(
                    Player.Position,
                    (IsEOne ? E : E2).Range,
                    E.IsReady() ? Color.LimeGreen : Color.IndianRed);
            }
            if (Menu["Draw"]["R"] && R.Level > 0)
            {
                Render.Circle.DrawCircle(Player.Position, R.Range, R.IsReady() ? Color.LimeGreen : Color.IndianRed);
            }
        }

        private static void OnBuffUpdateCount(Obj_AI_Base sender, Obj_AI_BaseBuffUpdateEventArgs args)
        {
            if (!sender.IsMe || args.Buff.DisplayName != "BlindMonkFlurry")
            {
                return;
            }
            cPassive = args.Buff.Count;
        }

        internal static void OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (sender.IsMe)
            {
                switch (args.Buff.DisplayName)
                {
                    case "BlindMonkFlurry":
                        cPassive = 2;
                        break;
                    case "BlindMonkQTwoDash":
                        isDashing = true;
                        break;
                }
            }
            else if (sender.IsEnemy)
            {
                if (args.Buff.DisplayName == "BlindMonkSonicWave")
                {
                    objQ = sender;
                }
                else if (args.Buff.Name == "blindmonkrroot" && Flash.IsReady())
                {
                    CastRFlash(sender);
                }
            }
        }

        internal static void OnBuffLose(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            if (sender.IsMe)
            {
                switch (args.Buff.DisplayName)
                {
                    case "BlindMonkFlurry":
                        cPassive = 0;
                        break;
                    case "BlindMonkQTwoDash":
                        isDashing = false;
                        break;
                }

            }
            else if (sender.IsEnemy && args.Buff.DisplayName == "BlindMonkSonicWave")
            {
                objQ = null;
            }
        }

        internal static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe || args.Slot != SpellSlot.R)
            {
                return;
            }

            lastR = Variables.TickCount;
        }

        internal static void AutoKnockUp()
        {
            if (!R.IsReady() || !Menu["Key"]["RAuto"].GetValue<MenuKeyBind>().Active)
            {
                return;
            }

            var multiR = GetMultiR();

            if (multiR.Item1 != null && (multiR.Item2 == -1 || multiR.Item2 >= Menu["RMenu"]["RAutoCountA"] + 1))
            {
                R.CastOnUnit(multiR.Item1);
            }
        }

        internal static bool CanE2(Obj_AI_Base target)
        {
            var buff = target.GetBuff("BlindMonkTempest");

            return buff != null && buff.EndTime - Game.Time < 0.25 * (buff.EndTime - buff.StartTime);
        }

        internal static bool CanQ2(Obj_AI_Base target)
        {
            var buff = target.GetBuff("BlindMonkSonicWave");

            return buff != null && buff.EndTime - Game.Time < 0.25 * (buff.EndTime - buff.StartTime);
        }

        internal static bool CanR(AIHeroClient target)
        {
            var buff = target.GetBuff("BlindMonkDragonsRage");

            return buff != null && buff.EndTime - Game.Time <= 0.75 * (buff.EndTime - buff.StartTime);
        }

        private static void CastE(List<Obj_AI_Minion> minions = null)
        {
            if (!E.IsReady() || isDashing || Variables.TickCount - lastW <= 150 || Variables.TickCount - lastW2 <= 100)
            {
                return;
            }
            if (minions == null)
            {
                CastECombo();
            }
            else
            {
                CastELaneClear(minions);
            }
        }

        internal static void CastECombo()
        {

            if (IsEOne)
            {
                var target = Variables.TargetSelector.GetTargets(E.Range + 20, E.DamageType).Where(i => E.CanHitCircle(i)).ToList();

                if (target.Count == 0)
                {
                    return;
                }

                if ((cPassive == 0 && Player.Mana >= 70) || target.Count > 2 || (Variables.Orbwalker.GetTarget() == null ? target.Any(i => i.DistanceToPlayer() > Player.GetRealAutoAttackRange() + 100) : cPassive < 2))
                {
                    E.Cast();
                }
            }
            else if (Menu["EMenu"]["ComboE2"])
            {
                var target = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(E2.Range) && HaveE(i)).ToList();

                if (target.Count == 0)
                {
                    return;
                }

                if ((cPassive == 0 || target.Count > 2 || target.Any(i => CanE2(i) || i.DistanceToPlayer() > i.GetRealAutoAttackRange() + 50)) && E2.Cast())
                {
                    lastE2 = Variables.TickCount;
                }
            }
        }

        internal static void CastELaneClear(List<Obj_AI_Minion> minions)
        {
            if (IsEOne)
            {
                if (cPassive > 0)
                {
                    return;
                }

                var count = minions.Count(i => i.IsValidTarget(E.Range));

                if (count > 0 && (Player.Mana >= 70 || count > 2))
                {
                    E.Cast();
                }
            }
            else
            {
                var minion = minions.Where(i => i.IsValidTarget(E2.Range) && HaveE(i)).ToList();

                if (minion.Count > 0 && (cPassive == 0 || minion.Any(CanE2)) && E2.Cast())
                {
                    if (E2.Cast())
                    {
                        lastE2 = Variables.TickCount;
                    }
                }
            }
        }

        internal static void CastQSmite(AIHeroClient target)
        {
            var pred = Q.GetPrediction(target, false, -1, CollisionableObjects.YasuoWall);

            if (pred.Hitchance < Q.MinHitChance)
            {
                return;
            }

            var col = Q.GetCollision(target, new List<Vector3> { pred.UnitPosition, target.Position });

            if (col.Count == 0 || (Menu["QMenu"]["ComboQCol"] && CastSmiteKillCollision(col)))
            {
                Q.Cast(pred.CastPosition);
            }
        }

        internal static void CastRFlash(Obj_AI_Base target)
        {
            var targetSelect = Variables.TargetSelector.GetSelectedTarget();

            if (!targetSelect.IsValidTarget() || !targetSelect.Compare(target) || target.Health + target.AttackShield <= R.GetDamage(target))
            {
                return;
            }
            var pos = new Vector3();

            if (Menu["Key"]["RFlash"].GetValue<MenuKeyBind>().Active)
            {
                pos = Game.CursorPos;
            }
            else if (Menu["Key"]["RAdv"].GetValue<MenuKeyBind>().Active)
            {
                pos = posBubbaKush;
            }
            else if (Menu["Key"]["Insec"].GetValue<MenuKeyBind>().Active && Variables.TickCount - LastRFlashTime < 5000)
            {
                pos = GetPositionKickTo((AIHeroClient)target);
            }

            if (!pos.IsValid())
            {
                return;
            }

            Player.Spellbook.CastSpell(Flash, target.ServerPosition.Extend(pos, -(150 + target.BoundingRadius / 2)));
        }

        internal static void CastW(List<Obj_AI_Minion> minions = null)
        {
            if (!W.IsReady() || Variables.TickCount - lastW <= 300 || isDashing || Variables.TickCount - lastE2 <= 100)
            {
                return;
            }
            var hero = Variables.Orbwalker.GetTarget() as AIHeroClient;
            Obj_AI_Minion minion = null;
            if (minions != null && minions.Count > 0)
            {
                minion = minions.FirstOrDefault(i => i.InAutoAttackRange());
            }
            if (hero == null && minion == null)
            {
                return;
            }
            if (hero != null && !IsWOne && !Menu["WMenu"]["ComboW2"])
            {
                return;
            }

            if (hero != null && Player.HealthPercent < hero.HealthPercent && Player.HealthPercent < 30)
            {
                if (IsWOne)
                {
                    if (W.Cast())
                    {
                        lastW = Variables.TickCount;
                        return;
                    }
                }
                else if (W.Cast())
                {
                    lastW2 = Variables.TickCount;
                    return;
                }
            }
            if (Player.HealthPercent < (minions == null ? 8 : 5) || (!IsWOne && Variables.TickCount - lastW > 2600)
                || cPassive == 0
                || (minion != null && minion.Team == GameObjectTeam.Neutral
                    && minion.GetJungleType() != JungleType.Small && Player.HealthPercent < 40 && IsWOne))
            {
                if (IsWOne)
                {
                    if (W.Cast())
                    {
                        lastW = Variables.TickCount;
                    }
                }
                else if (W.Cast())
                {
                    lastW2 = Variables.TickCount;
                }
            }
        }

        internal static void Combo()
        {
            if (R.IsReady() && Menu["QMenu"]["ComboStarKill"] && Q.IsReady() && !IsQOne && Menu["QMenu"]["ComboQ"] && Menu["QMenu"]["ComboQ2"])
            {
                var target = Variables.TargetSelector.GetTargets(Q2.Range, Q2.DamageType).FirstOrDefault(HaveQ);

                if (target != null && target.Health + target.AttackShield > Q.GetDamage(target, DamageStage.SecondCast) + Player.GetAutoAttackDamage(target) && target.Health + target.AttackShield <= GetQ2Dmg(target, R.GetDamage(target)) + Player.GetAutoAttackDamage(target))
                {
                    if (R.CastOnUnit(target))
                    {
                        return;
                    }

                    if (Menu["WMenu"]["StarKillWJ"] && !R.IsInRange(target) && target.DistanceToPlayer() < WardRange + R.Range - 50 && Player.Mana >= 80 && !isDashing)
                    {
                        Flee(target.ServerPosition, true);
                    }
                }
            }

            if (Menu["QMenu"]["ComboQ"] && Q.IsReady())
            {
                if (IsQOne)
                {
                    var target = Q.GetTarget(Q.Width / 2);

                    if (target != null)
                    {
                        CastQSmite(target);
                    }
                }
                else if (Menu["QMenu"]["ComboQ2"] && !IsDashing && objQ.IsValidTarget(Q2.Range))
                {
                    var target = objQ as AIHeroClient;

                    if (target != null)
                    {
                        if ((CanQ2(target) || (!R.IsReady() && IsRecentR && CanR(target)) || target.Health + target.AttackShield <= Q.GetDamage(target, DamageStage.SecondCast) + Player.GetAutoAttackDamage(target) || ((R.IsReady() || (!target.HasBuff("BlindMonkDragonsRage") && Variables.TickCount - lastR > 1000)) && target.DistanceToPlayer() > target.GetRealAutoAttackRange() + 100) || cPassive == 0) && Q2.Cast())
                        {
                            isDashing = true;
                            return;
                        }
                    }
                    else if (Menu["QMenu"]["ComboQ2Obj"])
                    {
                        var targetQ2 = Q2.GetTarget(200);

                        if (targetQ2 != null && objQ.Distance(targetQ2) < targetQ2.DistanceToPlayer() && !targetQ2.InAutoAttackRange() && Q2.Cast())
                        {
                            isDashing = true;
                            return;
                        }
                    }
                }
            }

            if (Menu["EMenu"]["ComboE"])
            {
                CastE();
            }

            if (Menu["WMenu"]["ComboW"])
            {
                CastW();
            }

            var subTarget = W.GetTarget();

            //UseItem(subTarget);

            if (subTarget != null && Ignite.IsReady() && subTarget.HealthPercent < 30 && subTarget.DistanceToPlayer() <= IgniteRange)
            {
                Player.Spellbook.CastSpell(Ignite, subTarget);
            }
        }

        internal static void Flee(Vector3 pos, bool isStar = false)
        {
            if (!W.IsReady() || !IsWOne || Variables.TickCount - lastW <= 500)
            {
                return;
            }

            var posPlayer = Player.ServerPosition;
            var posJump = pos.Distance(posPlayer) < W.Range ? pos : posPlayer.Extend(pos, W.Range);
            var objJumps = new List<Obj_AI_Base>();
            objJumps.AddRange(GameObjects.AllyHeroes.Where(i => !i.IsMe));
            objJumps.AddRange(GameObjects.AllyWards.Where(i => i.IsWard()));
            objJumps.AddRange(
                GameObjects.AllyMinions.Where(
                    i => i.IsMinion() || i.IsPet() || SpecialPet.Contains(i.CharData.BaseSkinName.ToLower())));
            var objJump =
                objJumps.Where(
                    i => i.IsValidTarget(W.Range, false) && i.Distance(posJump) < (isStar ? R.Range - 50 : 200))
                    .MinOrDefault(i => i.Distance(posJump));
            if (objJump != null)
            {
                if (W.CastOnUnit(objJump))
                {
                    lastW = Variables.TickCount;
                }
            }
            else
            {
                Place(posJump);
            }
        }

        private static Tuple<AIHeroClient, Vector3, Vector3> GetBubbaKush()
        {
            var bestHit = 0;
            AIHeroClient bestTarget = null;
            Vector3 bestPos = new Vector3(), startPos = new Vector3();
            var targetKicks =
                GameObjects.EnemyHeroes.Where(
                    i =>
                    i.IsValidTarget(R.Range) && i.Health + i.AttackShield > R.GetDamage(i)
                    && !i.HasBuffOfType(BuffType.SpellShield) && !i.HasBuffOfType(BuffType.SpellImmunity))
                    .OrderByDescending(i => i.AllShield)
                    .ToList();
            foreach (var targetKick in targetKicks)
            {
                var posTarget = targetKick.ServerPosition;
                R3.Width = targetKick.BoundingRadius;
                R3.Range = RKickRange + R3.Width / 2;
                R3.UpdateSourcePosition(posTarget, posTarget);
                var targetHits =
                    GameObjects.EnemyHeroes.Where(
                        i => i.IsValidTarget(R3.Range + R3.Width / 2, true, R3.From) && !i.Compare(targetKick)).ToList();
                if (targetHits.Count == 0)
                {
                    continue;
                }
                var cHit = 1;
                var pos = new Vector3();
                foreach (var targetHit in targetHits)
                {
                    var pred = R3.GetPrediction(targetHit);
                    if (pred.Hitchance < HitChance.High)
                    {
                        continue;
                    }
                    cHit++;
                    pos = pred.CastPosition;
                    var dmgR = GetRColDmg(targetKick, targetHit);
                    if (targetHit.Health + targetHit.AttackShield <= dmgR
                        && !Invulnerable.Check(targetHit, R.DamageType, true, dmgR))
                    {
                        return new Tuple<AIHeroClient, Vector3, Vector3>(targetKick, pos, posTarget);
                    }
                }
                if (bestHit == 0 || bestHit < cHit)
                {
                    bestHit = cHit;
                    bestTarget = targetKick;
                    bestPos = pos;
                    startPos = posTarget;
                }
            }
            return new Tuple<AIHeroClient, Vector3, Vector3>(bestTarget, bestPos, startPos);
        }

        private static Tuple<AIHeroClient, int> GetMultiR()
        {
            var bestHit = 0;
            AIHeroClient bestTarget = null;
            var targetKicks =
                GameObjects.EnemyHeroes.Where(
                    i =>
                    i.IsValidTarget(R.Range) && i.Health + i.AttackShield > R.GetDamage(i)
                    && !i.HasBuffOfType(BuffType.SpellShield) && !i.HasBuffOfType(BuffType.SpellImmunity))
                    .OrderByDescending(i => i.AllShield)
                    .ToList();
            foreach (var targetKick in targetKicks)
            {
                var posTarget = targetKick.ServerPosition;
                R2.Width = targetKick.BoundingRadius;
                R2.Range = RKickRange + R2.Width / 2;
                R2.UpdateSourcePosition(posTarget, posTarget);
                var targetHits =
                    GameObjects.EnemyHeroes.Where(
                        i => i.IsValidTarget(R2.Range + R2.Width / 2, true, R2.From) && !i.Compare(targetKick)).ToList();
                if (targetHits.Count == 0)
                {
                    continue;
                }
                var cHit = 1;
                foreach (var targetHit in targetHits)
                {
                    var pred = R2.GetPrediction(targetHit);
                    if (pred.Hitchance < HitChance.High)
                    {
                        continue;
                    }
                    cHit++;
                    if (Menu["RMenu"]["RAutoKill"])
                    {
                        var dmgR = GetRColDmg(targetKick, targetHit);
                        if (targetHit.Health + targetHit.AttackShield <= dmgR
                            && !Invulnerable.Check(targetHit, R.DamageType, true, dmgR))
                        {
                            return new Tuple<AIHeroClient, int>(targetKick, -1);
                        }
                    }
                }
                if (bestHit == 0 || bestHit < cHit)
                {
                    bestHit = cHit;
                    bestTarget = targetKick;
                }
            }
            return new Tuple<AIHeroClient, int>(bestTarget, bestHit);
        }

        internal static double GetQ2Dmg(Obj_AI_Base target, double subHp)
        {
            var dmg = new[] { 50, 80, 110, 140, 170 }[Q.Level - 1] + 0.9 * Player.FlatPhysicalDamageMod + 0.08 * (target.MaxHealth - (target.Health - subHp));

            return Player.CalculateDamage(target, DamageType.Physical, target is Obj_AI_Minion ? Math.Min(dmg, 400) : dmg) + subHp;
        }

        internal static float GetRColDmg(AIHeroClient kickTarget, AIHeroClient hitTarget)
        {
            return R.GetDamage(hitTarget) + (float)Player.CalculateDamage(hitTarget, DamageType.Physical, new[] { 0.12, 0.15, 0.18 }[R.Level - 1] * kickTarget.AllShield);
        }

        internal static bool HaveE(Obj_AI_Base target)
        {
            return target.HasBuff("BlindMonkTempest");
        }

        internal static bool HaveQ(Obj_AI_Base target)
        {
            return target.HasBuff("BlindMonkSonicWave");
        }

        internal static void KillSteal()
        {
            if (Menu["QMenu"]["KillStealQ"] && Q.IsReady())
            {
                if (IsQOne)
                {
                    var target = Q.GetTarget(Q.Width / 2);

                    if (target != null && (target.Health + target.AttackShield <= Q.GetDamage(target) || (Menu["QMenu"]["KillStealQ2"] && target.Health + target.AttackShield <= GetQ2Dmg(target, Q.GetDamage(target)) + Player.GetAutoAttackDamage(target) && Player.Mana - Q.Instance.SData.Mana >= 30)) && Q.Casting(target, false, CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall).IsCasted())
                    {
                        return;
                    }
                }
                else if (Menu["QMenu"]["KillStealQ2"] && !IsDashing)
                {
                    var target = objQ as AIHeroClient;

                    if (target != null && target.Health + target.AttackShield <= Q.GetDamage(target, DamageStage.SecondCast) + Player.GetAutoAttackDamage(target) && Q2.Cast())
                    {
                        isDashing = true;
                        return;
                    }
                }
            }
            if (Menu["EMenu"]["KSE"] && E.IsReady() && IsEOne && Variables.TargetSelector.GetTargets(E.Range, E.DamageType).Any(i => E.CanHitCircle(i) && i.Health + i.MagicShield <= E.GetDamage(i)) && E.Cast())
            {
                return;
            }

            if (Menu["RMenu"]["KSR"] && R.IsReady())
            {
                var targetList = Variables.TargetSelector.GetTargets(R.Range, R.DamageType, false).Where(i => Menu["RMenu"]["RCast" + i.ChampionName]).ToList();

                if (targetList.Count > 0)
                {
                    var targetR = targetList.FirstOrDefault(i => i.Health + i.AttackShield <= R.GetDamage(i));

                    if (targetR != null)
                    {
                        R.CastOnUnit(targetR);
                    }
                    else if (Menu["QMenu"]["KillStealQ"] && Menu["QMenu"]["KillStealQ2"] && Q.IsReady() && !IsQOne)
                    {
                        var targetQ2R = targetList.FirstOrDefault(i => HaveQ(i) && i.Health + i.AttackShield <= GetQ2Dmg(i, R.GetDamage(i)) + Player.GetAutoAttackDamage(i));

                        if (targetQ2R != null)
                        {
                            R.CastOnUnit(targetQ2R);
                        }
                    }
                }
            }
        }

        internal static void LaneClear()
        {
            var minions = ListMinions().Where(i => i.IsValidTarget(Q2.Range)).OrderByDescending(i => i.MaxHealth).ToList();

            if (minions.Count == 0)
            {
                return;
            }

            if (Menu["EMenu"]["LaneJungleE"])
            {
                CastE(minions);
            }

            if (Menu["WMenu"]["LaneJungleW"])
            {
                CastW(minions);
            }

            if (Menu["QMenu"]["JungleQ"] && Q.IsReady())
            {
                if (IsQOne)
                {
                    if (cPassive < 2)
                    {
                        var minionQ = minions.Where(i => i.DistanceToPlayer() < Q.Range - 10).ToList();

                        if (minionQ.Count > 0)
                        {
                            var minionJungle = minionQ.Where(i => i.Team == GameObjectTeam.Neutral).OrderByDescending(i => i.MaxHealth).ThenBy(i => i.DistanceToPlayer()).ToList();

                            if (Menu["QMenu"]["JungleQBig"] && minionJungle.Count > 0 && Player.Health > 100)
                            {
                                minionJungle = minionJungle.Where(i => i.GetJungleType() == JungleType.Legendary || i.GetJungleType() == JungleType.Large || i.Name.Contains("Crab")).ToList();
                            }

                            if (minionJungle.Count > 0)
                            {
                                minionJungle.ForEach(i => Q.Casting(i));
                            }
                            else
                            {
                                var minionLane = minionQ.Where(i => i.Team != GameObjectTeam.Neutral).OrderByDescending(i => i.GetMinionType().HasFlag(MinionTypes.Siege)).ThenBy(i => i.GetMinionType().HasFlag(MinionTypes.Super)).ThenBy(i => i.Health).ThenByDescending(i => i.MaxHealth).ToList();

                                if (minionLane.Count == 0)
                                {
                                    return;
                                }

                                foreach (var minion in minionLane)
                                {
                                    if (minion.InAutoAttackRange())
                                    {
                                        if (Q.GetHealthPrediction(minion) > Q.GetDamage(minion) && Q.Casting(minion).IsCasted())
                                        {
                                            return;
                                        }
                                    }
                                    else if ((Variables.Orbwalker.GetTarget() != null ? Q.CanLastHit(minion, Q.GetDamage(minion)) : Q.GetHealthPrediction(minion) > Q.GetDamage(minion)) && Q.Casting(minion).IsCasted())
                                    {
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (!IsDashing)
                {
                    var q2Minion = objQ;

                    if (q2Minion.IsValidTarget(Q2.Range) && (CanQ2(q2Minion) || q2Minion.Health <= Q.GetDamage(q2Minion, DamageStage.SecondCast) || q2Minion.DistanceToPlayer() > q2Minion.GetRealAutoAttackRange() + 100 || cPassive == 0) && Q2.Cast())
                    {
                        isDashing = true;
                    }
                }
            }
        }

        internal static void LastHit()
        {
            if (!Menu["QMenu"]["LastHitQ"] || !Q.IsReady() || !IsQOne || Player.Spellbook.IsAutoAttacking)
            {
                return;
            }

            var minions = GameObjects.EnemyMinions.Where(i => (i.IsMinion() || i.IsPet(false)) && i.IsValidTarget(Q.Range) && Q.CanLastHit(i, Q.GetDamage(i))).OrderByDescending(i => i.MaxHealth).ToList();

            if (minions.Count == 0)
            {
                return;
            }

            minions.ForEach(i => Q.Casting(i, false, CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall));
        }

        internal static void OnDraw(EventArgs args)
        {
            if (Player.IsDead || R.Level == 0 || !CanInsec)
            {
                return;
            }
            if (Menu["Draw"]["KnockUp"] && R.Level > 0)
            {

                var menu = Menu["Key"]["RAuto"].GetValue<MenuKeyBind>();
                var text = $"Auto KnockUp: {(menu.Active ? "On" : "Off")} <{Menu["RMenu"]["RAutoCountA"].GetValue<MenuSlider>().Value}> [{menu.Key}]";
                var pos = Drawing.WorldToScreen(Player.Position);

                Drawing.DrawText(
                    pos.X - (float)Drawing.GetTextEntent((text), 15)
                    .Width / 2,
                    pos.Y + 20, menu.Active ? Color.White : Color.Gray, text);
            }

            if (Menu["Draw"]["DLine"])
            {
                var target = GetTarget;

                if (target != null)
                {
                    Render.Circle.DrawCircle(target.Position, target.BoundingRadius * 1.35f, Color.BlueViolet);
                    Render.Circle.DrawCircle(GetPositionBehind(target), target.BoundingRadius * 1.35f, Color.BlueViolet);
                    Drawing.DrawLine(Drawing.WorldToScreen(target.Position), Drawing.WorldToScreen(GetPositionKickTo(target)), 1, Color.BlueViolet);
                }
            }

            if (Menu["Draw"]["DWardFlash"] && CanWardFlash)
            {
                Render.Circle.DrawCircle(Player.Position, RangeWardFlash, Color.Orange);
            }
        }

        internal static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead || MenuGUI.IsChatOpen || Shop.IsOpen || Player.IsRecalling())
            {
                return;
            }

            if (Menu["Key"]["FleeW"].GetValue<MenuKeyBind>().Active)
            {
                Variables.Orbwalker.Move(Game.CursorPos);
                Flee(Game.CursorPos);
            }

            KillSteal();

            Variables.Orbwalker.AttackState = (!Menu["Key"]["Insec"].GetValue<MenuKeyBind>().Active);

            switch (Variables.Orbwalker.ActiveMode)
            {
                case OrbwalkingMode.Combo:
                    Combo();
                    break;
                case OrbwalkingMode.LaneClear:
                    LaneClear();
                    break;
                case OrbwalkingMode.LastHit:
                    LastHit();
                    break;
                case OrbwalkingMode.None:
                    if (Menu["Key"]["RFlash"].GetValue<MenuKeyBind>().Active)
                    {
                        Variables.Orbwalker.Move(Game.CursorPos);

                        if (R.IsReady() && Flash.IsReady())
                        {
                            var target = Variables.TargetSelector.GetTargets(R.Range, R.DamageType).Where(i => i.Health + i.AttackShield > R.GetDamage(i)).MaxOrDefault(i => new Priority().GetPriority(i));

                            if (target != null && R.CastOnUnit(target))
                            {
                                Variables.TargetSelector.SetTarget(target);
                            }
                        }
                    }
                    else if (Menu["Key"]["RAdv"].GetValue<MenuKeyBind>().Active)
                    {
                        Variables.Orbwalker.Move(Game.CursorPos);
                        if (R.IsReady() && Flash.IsReady())
                        {
                            var bubbaKush = GetBubbaKush();
                            if (bubbaKush.Item1 != null && bubbaKush.Item2.IsValid() && R.CastOnUnit(bubbaKush.Item1))
                            {
                                posBubbaKush = bubbaKush.Item2;
                                Variables.TargetSelector.SetTarget(bubbaKush.Item1);
                            }
                        }
                    }
                    else if (Menu["Key"]["Star"].GetValue<MenuKeyBind>().Active)
                    {
                        StarCombo();
                    }
                    else if (Menu["Key"]["Insec"].GetValue<MenuKeyBind>().Active)
                    {
                        Start(GetTarget);
                    }
                    break;
            }

            if (!Menu["Key"]["Insec"].GetValue<MenuKeyBind>().Active)
            {
                AutoKnockUp();
            }
        }

        internal static void StarCombo()
        {
            var target = Q.GetTarget(Q.Width / 2);

            if (!IsQOne)
            {
                target = objQ as AIHeroClient;
            }

            if (!Q.IsReady())
            {
                target = W.GetTarget();
            }

            Variables.Orbwalker.Orbwalk(target);

            if (target == null)
            {
                return;
            }

            if (Q.IsReady())
            {
                if (IsQOne)
                {
                    CastQSmite(target);
                }
                else if (!IsDashing && HaveQ(target) && (target.Health + target.AttackShield <= Q.GetDamage(target, DamageStage.SecondCast) + Player.GetAutoAttackDamage(target) || (!R.IsReady() && IsRecentR && CanR(target))) && Q2.Cast())
                {
                    isDashing = true;
                    return;
                }
            }

            if (E.IsReady() && IsEOne && E.CanHitCircle(target) && (!HaveQ(target) || Player.Mana >= 70) && E.Cast())
            {
                return;
            }

            if (!R.IsReady() || !Q.IsReady() || IsQOne || !HaveQ(target))
            {
                return;
            }

            if (R.IsInRange(target))
            {
                R.CastOnUnit(target);
            }
            else if (target.DistanceToPlayer() < WardRange + R.Range - 50 && Player.Mana >= 70 && !isDashing)
            {
                Flee(target.ServerPosition, true);
            }
        }

        /*
        internal static void UseItem(AIHeroClient target)
        {
            if (target != null && (target.HealthPercent < 40 || Player.HealthPercent < 50))
            {
                if (Bilgewater.IsReady)
                {
                    Bilgewater.Cast(target);
                }
                if (BotRuinedKing.IsReady)
                {
                    BotRuinedKing.Cast(target);
                }
            }

            if (Youmuu.IsReady && Player.CountEnemyHeroesInRange(W.Range + E.Range) > 0)
            {
                Youmuu.Cast();
            }

            if (Tiamat.IsReady && Player.CountEnemyHeroesInRange(Tiamat.Range) > 0)
            {
                Tiamat.Cast();
            }

            if (Hydra.IsReady && Player.CountEnemyHeroesInRange(Hydra.Range) > 0)
            {
                Hydra.Cast();
            }

            if (Titanic.IsReady && !Player.Spellbook.IsAutoAttacking && Variables.Orbwalker.GetTarget() != null)
            {
                Titanic.Cast();
            }
        }
        */

        internal static void Start(AIHeroClient target)
        {
            if (Variables.Orbwalker.CanMove && Variables.TickCount - lastMoveTime > 250)
            {
                if (target != null && lastMoveTime > 0 && CanInsec)
                {
                    var posEnd = GetPositionKickTo(target);

                    Variables.Orbwalker.Move(posEnd.DistanceToPlayer() > target.Distance(posEnd) ? GetPositionBehind(target) : Game.CursorPos);
                }
                else
                {
                    Variables.Orbwalker.Move(Game.CursorPos);
                }
            }

            if (target == null || !CanInsec)
            {
                return;
            }

            if (R.IsInRange(target))
            {
                var posEnd = GetPositionKickTo(target);
                var posTarget = target.Position;
                var posPlayer = Player.Position;

                if (posPlayer.Distance(posEnd) > posTarget.Distance(posEnd))
                {
                    var project = posTarget.Extend(posPlayer, -RKickRange).ProjectOn(posTarget, posEnd.Extend(posTarget, -(RKickRange * 0.5f)));

                    if (project.IsOnSegment && project.SegmentPoint.Distance(posEnd) <= RKickRange * 0.5f && R.CastOnUnit(target))
                    {
                        return;
                    }
                }
            }

            if (!IsRecent)
            {
                if (!IsWardFlash)
                {
                    var checkJump = GapCheck(target);

                    if (checkJump.Item2)
                    {
                        GapByWardJump(target, checkJump.Item1);
                    }
                    else
                    {
                        var checkFlash = GapCheck(target, true);

                        if (checkFlash.Item2)
                        {
                            GapByFlash(target, checkFlash.Item1);
                        }
                        else if (CanWardFlash)
                        {
                            var posTarget = target.ServerPosition;

                            if (posTarget.DistanceToPlayer() < RangeWardFlash && (!isDashing || (!lastObjQ.Compare(target) && lastObjQ.Distance(posTarget) > RangeNormal)))
                            {
                                IsWardFlash = true;
                                return;
                            }
                        }
                    }
                }
                else if (Place(target.ServerPosition))
                {
                    Variables.TargetSelector.SetTarget(target);
                    return;
                }
            }

            if (!IsDashing && (!CanWardFlash || !IsWardFlash))
            {
                GapByQ(target);
            }
        }

        internal static void CleanData()
        {
            lastEndPos = lastFlashPos = new Vector3();
            lastInsecTime = 0;
            IsWardFlash = false;
            Variables.TargetSelector.SetTarget(null);
        }

        internal static void GapByFlash(AIHeroClient target, Vector3 posBehind)
        {
            switch (Menu["Insec"]["FlashMode"].GetValue<MenuList>().Index)
            {
                case 0:
                    GapByRFlash(target);
                    break;
                case 1:
                    GapByFlashR(target, posBehind);
                    break;
                case 2:
                    if (!posBehind.IsValid())
                    {
                        GapByRFlash(target);
                    }
                    else
                    {
                        GapByFlashR(target, posBehind);
                    }
                    break;
            }
        }

        internal static void GapByFlashR(AIHeroClient target, Vector3 posBehind)
        {
            if (!Player.Spellbook.CastSpell(Flash, posBehind))
            {
                return;
            }

            if (Player.CanMove)
            {
                lastMoveTime = Variables.TickCount;
                Variables.Orbwalker.Move(posBehind.Extend(GetPositionKickTo(target), -(DistFlash + Player.BoundingRadius / 2)));
            }

            lastFlashPos = posBehind;
            lastEndPos = GetPositionAfterKick(target);
            lastInsecTime = lastFlashRTime = Variables.TickCount;
            Variables.TargetSelector.SetTarget(target);
        }

        internal static void GapByQ(AIHeroClient target)
        {
            if (!Menu["QMenu"]["InsecQ"] || !Q.IsReady())
            {
                return;
            }

            if (CanWardFlash && IsQOne && Player.Mana < 50 + 80)
            {
                return;
            }

            var minDist = CanWardFlash ? RangeWardFlash : RangeNormal;

            if (IsQOne)
            {
                var pred = Q.GetPrediction(target, false, -1, CollisionableObjects.YasuoWall);

                if (pred.Hitchance >= Q.MinHitChance)
                {
                    var col = Q.GetCollision(target, new List<Vector3> { pred.UnitPosition, target.Position });
                    if ((col.Count == 0 || (Menu["QMenu"]["InsecQCol"] && CastSmiteKillCollision(col))) && Q.Cast(pred.CastPosition))
                    {
                        return;
                    }
                }

                if (!Menu["QMenu"]["InsecQObj"])
                {
                    return;
                }

                var nearObj =
                    ListEnemies(true)
                        .Where(
                            i =>
                            !i.Compare(target) && i.IsValidTarget(Q.Range)
                            && Q.GetHealthPrediction(i) > Q.GetDamage(i) && i.Distance(target) < minDist - 100)
                        .OrderBy(i => i.Distance(target))
                        .ThenByDescending(i => i.Health)
                        .ToList();
                if (nearObj.Count == 0)
                {
                    return;
                }

                nearObj.ForEach(i => Q.Casting(i));
            }
            else if (target.DistanceToPlayer() > minDist && (HaveQ(target) || (objQ.IsValidTarget(Q2.Range) && target.Distance(objQ) < minDist - 100)) && ((CanWardJump && Player.Mana >= 80) || (Menu["Insec"]["Flash"] && Flash.IsReady())) && Q2.Cast())
            {
                isDashing = true;
                Variables.TargetSelector.SetTarget(target);
            }
        }

        internal static void GapByRFlash(AIHeroClient target)
        {
            if (!R.CastOnUnit(target))
            {
                return;
            }

            lastEndPos = GetPositionAfterKick(target);
            lastInsecTime = LastRFlashTime = Variables.TickCount;
            Variables.TargetSelector.SetTarget(target);
        }

        internal static void GapByWardJump(AIHeroClient target, Vector3 posBehind)
        {
            if (!Place(posBehind, 1))
            {
                return;
            }

            if (Player.CanMove)
            {
                lastMoveTime = Variables.TickCount;
                Variables.Orbwalker.Move(posBehind.Extend(GetPositionKickTo(target), -(DistWard + Player.BoundingRadius / 2)));
            }

            lastEndPos = GetPositionAfterKick(target);
            lastInsecTime = LastInsecWardTime = LastInsecJumpTme = Variables.TickCount;

            Variables.TargetSelector.SetTarget(target);
        }

        internal static Tuple<Vector3, bool> GapCheck(AIHeroClient target, bool useFlash = false)
        {
            if (!useFlash ? !CanWardJump : !Menu["Insec"]["Flash"] || !Flash.IsReady())
            {
                return new Tuple<Vector3, bool>(new Vector3(), false);
            }

            var posEnd = GetPositionKickTo(target);
            var posTarget = target.ServerPosition;
            var posPlayer = Player.ServerPosition;

            if (!useFlash)
            {
                var posBehind = posTarget.Extend(posEnd, -DistWard);

                if (posPlayer.Distance(posBehind) < WardRange && posTarget.Distance(posBehind) < posEnd.Distance(posBehind))
                {
                    return new Tuple<Vector3, bool>(posBehind, true);
                }
            }
            else
            {
                var flashMode = Menu["Insec"]["FlashMode"].GetValue<MenuList>().Index;

                if (flashMode != 1 && posPlayer.Distance(posTarget) < R.Range)
                {
                    return new Tuple<Vector3, bool>(new Vector3(), true);
                }

                if (flashMode > 0)
                {
                    var posBehind = posTarget.Extend(posEnd, -DistFlash);

                    if (posPlayer.Distance(posBehind) < FlashRange && posTarget.Distance(posBehind) < posEnd.Distance(posBehind))
                    {
                        return new Tuple<Vector3, bool>(posBehind, true);
                    }
                }
            }

            return new Tuple<Vector3, bool>(new Vector3(), false);
        }

        internal static Vector3 GetPositionAfterKick(AIHeroClient target)
        {
            return target.ServerPosition.Extend(GetPositionKickTo(target), RKickRange);
        }

        internal static Vector3 GetPositionKickTo(AIHeroClient target)
        {
            if (lastEndPos.IsValid() && target.Distance(lastEndPos) <= RKickRange + 700)
            {
                return lastEndPos;
            }

            var pos = Player.ServerPosition;

            switch (Menu["Insec"]["Mode"].GetValue<MenuList>().Index)
            {
                case 0:
                    var turret = GameObjects.AllyTurrets.Where(i => !i.IsDead && target.Distance(i) <= RKickRange + 500 && i.Distance(target) - RKickRange <= 950 && i.Distance(target) > 400).MinOrDefault(i => i.DistanceToPlayer());

                    if (turret != null)
                    {
                        pos = turret.ServerPosition;
                    }
                    else
                    {
                        var hero = GameObjects.AllyHeroes.Where(i => i.IsValidTarget(RKickRange + 700, false, target.ServerPosition) && !i.IsMe && i.HealthPercent > 10 && i.Distance(target) > 350).MaxOrDefault(i => new Priority().GetDefaultPriority(i));

                        if (hero != null)
                        {
                            pos = hero.ServerPosition;
                        }
                    }
                    break;
                case 1:
                    pos = Game.CursorPos;
                    break;
            }

            return pos;
        }

        internal static Vector3 GetPositionBehind(AIHeroClient target)
        {
            return target.ServerPosition.Extend(GetPositionKickTo(target), -(CanWardJump ? DistWard : DistFlash));
        }

        internal static AIHeroClient GetTarget
        {
            get
            {
                AIHeroClient target = null;

                if (Menu["Insec"]["TargetSelect"])
                {
                    var sub = Variables.TargetSelector.GetSelectedTarget();

                    if (sub.IsValidTarget())
                    {
                        target = sub;
                    }
                }
                else
                {
                    target = Q.GetTarget(-100);

                    if ((Menu["QMenu"]["InsecQ"] && Q.IsReady()) || objQ.IsValidTarget(Q2.Range))
                    {
                        target = Q2.GetTarget(FlashRange);
                    }
                }

                return target;
            }
        }

        internal static bool Place(Vector3 pos, int type = 0)
        {
            if (!CanWardJump)
                return false;

            var ward = Items.GetWardSlot();

            if (ward == null)
                return false;

            var posPlayer = Player.ServerPosition;
            var posPlace = pos.Distance(posPlayer) < WardRange ? pos : posPlayer.Extend(pos, WardRange);

            if (!Player.Spellbook.CastSpell(ward.SpellSlot, posPlace))
                return false;

            if (type == 0)
            {
                lastPlaceTime = Variables.TickCount + 1100;
            }
            else if (type == 1)
                lastPlaceTime = LastInsecWardTime = LastInsecJumpTme = Variables.TickCount;


            lastPlacePos = posPlace;

            return true;
        }

        internal static void Jump(Vector3 pos)
        {
            if (!W.IsReady() || !IsWOne || Variables.TickCount - lastW <= 500)
                return;

            var wardObj = GameObjects.AllyWards.Where(i => i.IsValidTarget(W.Range, false) && i.IsWard() && i.Distance(pos) < 200).MinOrDefault(i => i.Distance(pos));

            if (wardObj != null && W.CastOnUnit(wardObj))
                lastW = Variables.TickCount;
        }

        internal static bool CastSmiteKillCollision(List<Obj_AI_Base> col)
        {
            if (col.Count > 1 || !Smite.IsReady())
            {
                return false;
            }

            var obj = col.First();

            return obj.Health <= GetSmiteDmg && obj.DistanceToPlayer() < SmiteRange && Player.Spellbook.CastSpell(Smite, obj);
        }
    }
}