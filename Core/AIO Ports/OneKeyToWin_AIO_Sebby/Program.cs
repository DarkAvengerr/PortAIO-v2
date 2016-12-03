using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SPrediction;
using SebbyLib;
using SharpDX.Direct3D9;

using EloBuddy;
using LeagueSharp.Common;
namespace OneKeyToWin_AIO_Sebby
{
    internal class Program
    {
        private static string OktNews = "NEW champion - TEEMO, Sivir E impove";

        public static Menu Config;

        public static AIHeroClient Player { get { return ObjectManager.Player; } }
        public static SebbyLib.Orbwalking.Orbwalker Orbwalker;

        public static Spell Q, W, E, R, Q1, W1, E1, R1;
        public static float QMANA = 0, WMANA = 0, EMANA = 0, RMANA = 0;

        public static float JungleTime, DrawSpellTime = 0;
        public static AIHeroClient jungler = ObjectManager.Player;
        public static int timer, HitChanceNum = 4, tickNum = 4, tickIndex = 0;
        public static Obj_SpawnPoint enemySpawn;
        public static SebbyLib.Prediction.PredictionOutput DrawSpellPos;

        public static bool SPredictionLoad = false;
        public static int AIOmode = 0;
        private static float dodgeRange = 420;
        private static float dodgeTime = Game.Time;
        private static float spellFarmTimer = 0;
        private static Font TextBold;

        public static void GameOnOnGameLoad()
        {
            TextBold = new Font(Drawing.Direct3DDevice, new FontDescription
            { FaceName = "Impact", Height = 30, Weight = FontWeight.Normal, OutputPrecision = FontPrecision.Default, Quality = FontQuality.ClearType });

            enemySpawn = ObjectManager.Get<Obj_SpawnPoint>().FirstOrDefault(x => x.IsEnemy);
            Q = new Spell(SpellSlot.Q);
            E = new Spell(SpellSlot.E);
            W = new Spell(SpellSlot.W);
            R = new Spell(SpellSlot.R);

            Config = new Menu("OneKeyToWin AIO", "OneKeyToWin_AIO" + ObjectManager.Player.ChampionName, true).SetFontStyle(System.Drawing.FontStyle.Bold, Color.DeepSkyBlue);

            Config.AddItem(new MenuItem("AIOmode", "AIO mode", true).SetValue(new StringList(new[] { "Utility and champion", "Only Champion", "Only Utility" }, 1)));

            AIOmode = Config.Item("AIOmode", true).GetValue<StringList>().SelectedIndex;

            if (AIOmode != 2)
            {
                Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
                Orbwalker = new SebbyLib.Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));
            }


            #region LOAD CHAMPIONS

            switch (Player.ChampionName)
            {
                case "Jinx":
                    new Champions.Jinx();
                    break;
                case "Sivir":
                    new Champions.Sivir();
                    break;
                case "Ezreal":
                    new Champions.Ezreal();
                    break;
                case "KogMaw":
                    new Champions.KogMaw();
                    break;
                case "Annie":
                    new Champions.Annie();
                    break;
                case "Ashe":
                    new Champions.Ashe();
                    break;
                case "MissFortune":
                    new Champions.MissFortune();
                    break;
                case "Quinn":
                    new Champions.Quinn();
                    break;
                case "Kalista":
                    new Champions.Kalista();
                    break;
                case "Caitlyn":
                    new Champions.Caitlyn();
                    break;
                case "Graves":
                    new Champions.Graves();
                    break;
                case "Urgot":
                    new Champions.Urgot();
                    break;
                case "Anivia":
                    new Champions.Anivia();
                    break;
                case "Orianna":
                    new Champions.Orianna();
                    break;
                case "Ekko":
                    new Champions.Ekko();
                    break;
                case "Vayne":
                    new Champions.Vayne();
                    break;
                case "Lucian":
                    new Champions.Lucian();
                    break;
                case "Darius":
                    new Champions.Darius();
                    break;
                case "Blitzcrank":
                    new Champions.Blitzcrank();
                    break;
                case "Corki":
                    new Champions.Corki();
                    break;
                case "Varus":
                    new Champions.Varus();
                    break;
                case "Twitch":
                    new Champions.Twitch();
                    break;
                case "Tristana":
                    new Champions.Tristana();
                    break;
                case "Xerath":
                    new Champions.Xerath();
                    break;
                case "Jayce":
                    new Champions.Jayce();
                    break;
                case "Kayle":
                    new Champions.Kayle();
                    break;
                case "Thresh":
                    new Champions.Thresh();
                    break;
                case "Draven":
                    new Champions.Draven();
                    break;
                case "Evelynn":
                    new Champions.Evelynn();
                    break;
                case "Ahri":
                    new Champions.Ahri();
                    break;
                case "Brand":
                    new Champions.Brand();
                    break;
                case "Morgana":
                    new Champions.Morgana();
                    break;
                case "Lux":
                    new Champions.Lux();
                    break;
                case "Malzahar":
                    new Champions.Malzahar();
                    break;
                case "Karthus":
                    new Champions.Karthus();
                    break;
                case "Swain":
                    new Champions.Swain();
                    break;
                case "TwistedFate":
                    new Champions.TwistedFate();
                    break;
                case "Syndra":
                    new Champions.Syndra();
                    break;
                case "Velkoz":
                    new Champions.Velkoz();
                    break;
                case "Jhin":
                    new Champions.Jhin();
                    break;
                case "Kindred":
                    new Champions.Kindred();
                    break;
                case "Braum":
                    new Champions.Braum();
                    break;
                case "Teemo":
                    new Champions.Teemo();
                    break;
                case "Ziggs":
                    new Champions.Ziggs();
                    break;
            }
            #endregion


            Config.SubMenu("Prediction MODE").AddItem(new MenuItem("Qpred", "Q Prediction MODE", true).SetValue(new StringList(new[] { "Common prediction", "OKTW© PREDICTION", "SPediction press F5 if not loaded", "SDK", "Exory prediction" }, 0)));
            Config.SubMenu("Prediction MODE").AddItem(new MenuItem("QHitChance", "Q Hit Chance", true).SetValue(new StringList(new[] { "Very High", "High", "Medium" }, 0)));
            Config.SubMenu("Prediction MODE").AddItem(new MenuItem("Wpred", "W Prediction MODE", true).SetValue(new StringList(new[] { "Common prediction", "OKTW© PREDICTION", "SPediction press F5 if not loaded", "SDK", "Exory prediction" }, 0)));
            Config.SubMenu("Prediction MODE").AddItem(new MenuItem("WHitChance", "W Hit Chance", true).SetValue(new StringList(new[] { "Very High", "High", "Medium" }, 0)));
            Config.SubMenu("Prediction MODE").AddItem(new MenuItem("Epred", "E Prediction MODE", true).SetValue(new StringList(new[] { "Common prediction", "OKTW© PREDICTION", "SPediction press F5 if not loaded", "SDK", "Exory prediction" }, 0)));
            Config.SubMenu("Prediction MODE").AddItem(new MenuItem("EHitChance", "E Hit Chance", true).SetValue(new StringList(new[] { "Very High", "High", "Medium" }, 0)));
            Config.SubMenu("Prediction MODE").AddItem(new MenuItem("Rpred", "R Prediction MODE", true).SetValue(new StringList(new[] { "Common prediction", "OKTW© PREDICTION", "SPediction press F5 if not loaded", "SDK", "Exory prediction" }, 0)));
            Config.SubMenu("Prediction MODE").AddItem(new MenuItem("RHitChance", "R Hit Chance", true).SetValue(new StringList(new[] { "Very High", "High", "Medium" }, 0)));
            
            Config.SubMenu("Prediction MODE").AddItem(new MenuItem("debugPred", "Draw Aiming OKTW© PREDICTION").SetValue(false));

            if (Config.Item("Qpred", true).GetValue<StringList>().SelectedIndex == 2 || Config.Item("Wpred", true).GetValue<StringList>().SelectedIndex == 2
                || Config.Item("Epred", true).GetValue<StringList>().SelectedIndex == 2 || Config.Item("Rpred", true).GetValue<StringList>().SelectedIndex == 2)
            {
                SPrediction.Prediction.Initialize(Config.SubMenu("Prediction MODE"));
                SPredictionLoad = true;
                Config.SubMenu("Prediction MODE").AddItem(new MenuItem("322", "SPREDICTION LOADED"));
            }
            else
                Config.SubMenu("Prediction MODE").AddItem(new MenuItem("322", "SPREDICTION NOT LOADED"));

            Config.AddItem(new MenuItem("aiomodes", "!!! PRESS F5 TO RELOAD MODE !!!"));


            Config.AddToMainMenu();
            Game.OnUpdate += OnUpdate;
            SebbyLib.Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;
            Drawing.OnDraw += OnDraw;
        }

        private static void PositionHelper()
        {
            if (Player.ChampionName == "Draven" || !Config.Item("positioningAssistant").GetValue<bool>() || AIOmode == 2)
                return;

            if (Player.IsMelee)
            {
                Orbwalker.SetOrbwalkingPoint(new Vector3());
                return;
            }

            foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsMelee && enemy.IsValidTarget(dodgeRange) && enemy.IsFacing(Player) && Config.Item("posAssistant" + enemy.ChampionName).GetValue<bool>()))
            {
                var points = OktwCommon.CirclePoints(20, 250, Player.Position);

                if (Player.FlatMagicDamageMod > Player.FlatPhysicalDamageMod)
                    OktwCommon.blockAttack = true;


                Vector3 bestPoint = Vector3.Zero;

                foreach (var point in points)
                {
                    if (point.IsWall() || point.UnderTurret(true))
                    {
                        Orbwalker.SetOrbwalkingPoint(new Vector3());
                        return;
                    }

                    if (enemy.Distance(point) > dodgeRange && (bestPoint == Vector3.Zero || Game.CursorPos.Distance(point) < Game.CursorPos.Distance(bestPoint)))
                    {
                        bestPoint = point;
                    }
                }

                if (enemy.Distance(bestPoint) > dodgeRange)
                {
                    Orbwalker.SetOrbwalkingPoint(bestPoint);
                }
                else
                {
                    var fastPoint = enemy.ServerPosition.Extend(Player.ServerPosition, dodgeRange);
                    if (fastPoint.CountEnemiesInRange(dodgeRange) <= Player.CountEnemiesInRange(dodgeRange))
                    {
                        Orbwalker.SetOrbwalkingPoint(fastPoint);
                    }
                }

                dodgeTime = Game.Time;
                return;
            }

            Orbwalker.SetOrbwalkingPoint(new Vector3());
            if (OktwCommon.blockAttack == true)
                OktwCommon.blockAttack = false;
        }

        private static void Orbwalking_BeforeAttack(SebbyLib.Orbwalking.BeforeAttackEventArgs args)
        {
            if (AIOmode == 2)
                return;

            if (Combo && Config.Item("comboDisableMode", true).GetValue<bool>())
            {
                var t = (AIHeroClient)args.Target;
                if (4 * Player.GetAutoAttackDamage(t) < t.Health - OktwCommon.GetIncomingDamage(t) && !t.HasBuff("luxilluminatingfraulein") && !Player.HasBuff("sheen") && !Player.HasBuff("Mastery6261"))
                    args.Process = false;
            }

            if (!Player.IsMelee && OktwCommon.CollisionYasuo(Player.ServerPosition, args.Target.Position) && Config.Item("collAA", true).GetValue<bool>())
            {
                args.Process = false;
            }

            if (Orbwalker.ActiveMode == SebbyLib.Orbwalking.OrbwalkingMode.Mixed && Config.Item("supportMode", true).GetValue<bool>())
            {
                if (args.Target.Type == GameObjectType.obj_AI_Minion) args.Process = false;
            }
        }

        public static bool LaneClear = false, None = false, Harass = false, Combo = false, Farm = false;

        private static void OnUpdate(EventArgs args)
        {
            if (AIOmode == 2)
            {
                if (Player.IsMoving)
                    Combo = true;
                else
                    Combo = false;
            }
            else
            {
                Combo = Orbwalker.ActiveMode == SebbyLib.Orbwalking.OrbwalkingMode.Combo;

                if (Config.Item("harassMixed").GetValue<bool>())
                    Harass = Orbwalker.ActiveMode == SebbyLib.Orbwalking.OrbwalkingMode.Mixed;
                else
                    Harass = Orbwalker.ActiveMode == SebbyLib.Orbwalking.OrbwalkingMode.LaneClear || Orbwalker.ActiveMode == SebbyLib.Orbwalking.OrbwalkingMode.Mixed || Orbwalker.ActiveMode == SebbyLib.Orbwalking.OrbwalkingMode.Freeze;

                Farm = Orbwalker.ActiveMode == SebbyLib.Orbwalking.OrbwalkingMode.LaneClear || Orbwalker.ActiveMode == SebbyLib.Orbwalking.OrbwalkingMode.Mixed || Orbwalker.ActiveMode == SebbyLib.Orbwalking.OrbwalkingMode.Freeze;
                None = Orbwalker.ActiveMode == SebbyLib.Orbwalking.OrbwalkingMode.None;
                LaneClear = Orbwalker.ActiveMode == SebbyLib.Orbwalking.OrbwalkingMode.LaneClear;
            }

            tickIndex++;

            if (tickIndex > 4)
                tickIndex = 0;

            if (!LagFree(0))
                return;

            JunglerTimer();
        }

        public static void JunglerTimer()
        {
            if (AIOmode != 1 && Config.Item("timer").GetValue<bool>() && jungler != null && jungler.IsValid)
            {
                if (jungler.IsDead)
                {
                    timer = (int)(enemySpawn.Position.Distance(Player.Position) / 370);
                }
                else if (jungler.IsVisible)
                {
                    float Way = 0;
                    var JunglerPath = Player.GetPath(Player.Position, jungler.Position);
                    var PointStart = Player.Position;
                    if (JunglerPath == null)
                        return;
                    foreach (var point in JunglerPath)
                    {
                        var PSDistance = PointStart.Distance(point);
                        if (PSDistance > 0)
                        {
                            Way += PSDistance;
                            PointStart = point;
                        }
                    }
                    timer = (int)(Way / jungler.MoveSpeed);
                }
            }
        }

        public static bool LagFree(int offset)
        {
            if (tickIndex == offset)
                return true;
            else
                return false;
        }

        private static bool IsJungler(AIHeroClient hero) { return hero.Spellbook.Spells.Any(spell => spell.Name.ToLower().Contains("smite")); }

        public static void CastSpell(Spell QWER, Obj_AI_Base target)
        {
            int predIndex = 0;
            HitChance hitchance = HitChance.Low;

            if (QWER.Slot == SpellSlot.Q)
            {
                predIndex = Config.Item("Qpred", true).GetValue<StringList>().SelectedIndex;
                if (Config.Item("QHitChance", true).GetValue<StringList>().SelectedIndex == 0)
                    hitchance = HitChance.VeryHigh;
                else if (Config.Item("QHitChance", true).GetValue<StringList>().SelectedIndex == 1)
                    hitchance = HitChance.High;
                else if (Config.Item("QHitChance", true).GetValue<StringList>().SelectedIndex == 2)
                    hitchance = HitChance.Medium;
            }
            else if (QWER.Slot == SpellSlot.W)
            {
                predIndex = Config.Item("Wpred", true).GetValue<StringList>().SelectedIndex;
                if (Config.Item("WHitChance", true).GetValue<StringList>().SelectedIndex == 0)
                    hitchance = HitChance.VeryHigh;
                else if (Config.Item("WHitChance", true).GetValue<StringList>().SelectedIndex == 1)
                    hitchance = HitChance.High;
                else if (Config.Item("WHitChance", true).GetValue<StringList>().SelectedIndex == 2)
                    hitchance = HitChance.Medium;
            }
            else if (QWER.Slot == SpellSlot.E)
            {
                predIndex = Config.Item("Epred", true).GetValue<StringList>().SelectedIndex;
                if (Config.Item("EHitChance", true).GetValue<StringList>().SelectedIndex == 0)
                    hitchance = HitChance.VeryHigh;
                else if (Config.Item("EHitChance", true).GetValue<StringList>().SelectedIndex == 1)
                    hitchance = HitChance.High;
                else if (Config.Item("EHitChance", true).GetValue<StringList>().SelectedIndex == 2)
                    hitchance = HitChance.Medium;
            }
            else if (QWER.Slot == SpellSlot.R)
            {
                predIndex = Config.Item("Rpred", true).GetValue<StringList>().SelectedIndex;
                if (Config.Item("RHitChance", true).GetValue<StringList>().SelectedIndex == 0)
                    hitchance = HitChance.VeryHigh;
                else if (Config.Item("RHitChance", true).GetValue<StringList>().SelectedIndex == 1)
                    hitchance = HitChance.High;
                else if (Config.Item("RHitChance", true).GetValue<StringList>().SelectedIndex == 2)
                    hitchance = HitChance.Medium;
            }

            if (predIndex == 3)
            {
                SebbyLib.Movement.SkillshotType CoreType2 = SebbyLib.Movement.SkillshotType.SkillshotLine;
                bool aoe2 = false;

                if (QWER.Type == SkillshotType.SkillshotCircle)
                {
                    //CoreType2 = SebbyLib.Movement.SkillshotType.SkillshotCircle;
                    //aoe2 = true;
                }

                if (QWER.Width > 80 && !QWER.Collision)
                    aoe2 = true;

                var predInput2 = new SebbyLib.Movement.PredictionInput
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
                var poutput2 = SebbyLib.Movement.Prediction.GetPrediction(predInput2);

                //var poutput2 = QWER.GetPrediction(target);

                if (QWER.Speed != float.MaxValue && OktwCommon.CollisionYasuo(Player.ServerPosition, poutput2.CastPosition))
                    return;

                if ((int)hitchance == 6)
                {
                    if (poutput2.Hitchance >= SebbyLib.Movement.HitChance.VeryHigh)
                        QWER.Cast(poutput2.CastPosition);
                    else if (predInput2.Aoe && poutput2.AoeTargetsHitCount > 1 && poutput2.Hitchance >= SebbyLib.Movement.HitChance.High)
                    {
                        QWER.Cast(poutput2.CastPosition);
                    }

                }
                else if ((int)hitchance == 5)
                {
                    if (poutput2.Hitchance >= SebbyLib.Movement.HitChance.High)
                        QWER.Cast(poutput2.CastPosition);

                }
                else if ((int)hitchance == 4)
                {
                    if (poutput2.Hitchance >= SebbyLib.Movement.HitChance.Medium)
                        QWER.Cast(poutput2.CastPosition);
                }
            }
            else if (predIndex == 1)
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
                    return;

                if ((int)hitchance == 6)
                {
                    if (poutput2.Hitchance >= SebbyLib.Prediction.HitChance.VeryHigh)
                        QWER.Cast(poutput2.CastPosition);
                    else if (predInput2.Aoe && poutput2.AoeTargetsHitCount > 1 && poutput2.Hitchance >= SebbyLib.Prediction.HitChance.High)
                    {
                        QWER.Cast(poutput2.CastPosition);
                    }

                }
                else if ((int)hitchance == 5)
                {
                    if (poutput2.Hitchance >= SebbyLib.Prediction.HitChance.High)
                        QWER.Cast(poutput2.CastPosition);

                }
                else if ((int)hitchance == 4)
                {
                    if (poutput2.Hitchance >= SebbyLib.Prediction.HitChance.Medium)
                        QWER.Cast(poutput2.CastPosition);
                }
                if (Game.Time - DrawSpellTime > 0.5)
                {
                    DrawSpell = QWER;
                    DrawSpellTime = Game.Time;

                }
                DrawSpellPos = poutput2;
            }
            else if (predIndex == 0)
            {
                QWER.CastIfHitchanceEquals(target, hitchance);
            }
            else if (predIndex == 2)
            {
                if (target is AIHeroClient && target.IsValid)
                {
                    var t = target as AIHeroClient;
                    QWER.SPredictionCast(t, hitchance);
                }
                else
                {
                    QWER.CastIfHitchanceEquals(target, HitChance.High);
                }
            }
        }

        public static Spell DrawSpell;

        public static void drawText(string msg, Vector3 Hero, System.Drawing.Color color, int weight = 0)
        {
            var wts = Drawing.WorldToScreen(Hero);
            Drawing.DrawText(wts[0] - (msg.Length) * 5, wts[1] + weight, color, msg);
        }

        public static void debug(string msg)
        {
            if (Config.Item("debug").GetValue<bool>())
            {
                Console.WriteLine(msg);
            }
            if (Config.Item("debugChat").GetValue<bool>())
            {
                Chat.Print(msg);
            }
        }

        private static void DrawFontTextScreen(Font vFont, string vText, float vPosX, float vPosY, ColorBGRA vColor)
        {
            vFont.DrawText(null, vText, (int)vPosX, (int)vPosY, vColor);
        }

        private static void OnDraw(EventArgs args)
        {
            if (!SPredictionLoad && (int)Game.Time % 2 == 0 && (Config.Item("Qpred", true).GetValue<StringList>().SelectedIndex == 2 || Config.Item("Wpred", true).GetValue<StringList>().SelectedIndex == 2
                || Config.Item("Epred", true).GetValue<StringList>().SelectedIndex == 2 || Config.Item("Rpred", true).GetValue<StringList>().SelectedIndex == 2))
                drawText("PRESS F5 TO LOAD SPREDICTION", Player.Position, System.Drawing.Color.Yellow, -300);
        }
    }
}