using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Tc_SDKexAIO.Champions
{

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;
    using LeagueSharp.SDK.Enumerations;

    using Common;
    using Config;

    using Menu = LeagueSharp.SDK.UI.Menu;

    using SharpDX;

    using System;
    using System.Linq;
    using System.Windows.Forms;

    using static Common.Manager;

    internal static class Teemo
    {

        private static Menu Menu => PlaySharp.ChampionMenu;

        private static AIHeroClient Player => PlaySharp.Player;
        private static HpBarDraw HpBarDraw = new HpBarDraw();
        private static Spell Q, W, E, R;
        private static Vector3 Position;
        private static int RCast;
        private static float RRange => 300 * R.Level;
        private static bool Trap => GameObjects.Get<Obj_AI_Base>().Where(x => x.Name == "Noxious Trap").Any(x => Position.Distance(x.Position) <= 250);

        internal static void Init()
        {

            Q = new Spell(SpellSlot.Q, 680).SetTargetted(0.5f, 1500f);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R, 300).SetSkillshot(0.5f, 120f, 1000f, false, SkillshotType.SkillshotCircle);

            var QMenu = Menu.Add(new Menu("Q", "Q.Set"));
            {
                QMenu.GetBool("ComboQ", "Comno Q");
                QMenu.GetBool("HarassQ", "Harass Q");
                QMenu.GetBool("JCQ", "JungleClear Q", false);
                QMenu.GetSlider("ManaQ", "JungleClear Mana", 40, 0, 80);
                QMenu.GetBool("ADQ", "Use Q AD");
                QMenu.GetBool("KSQ", "KillStel Q");
                QMenu.GetBool("CheckAA", "Check AA", false);
            }

            var WMenu = Menu.Add(new Menu("W", "W.Set"));
            {
                WMenu.GetBool("ComboW", "Combo W", false);
                WMenu.GetBool("WRange", "Use W if enemy is in range only", false);
                WMenu.GetKeyBind("FleeKey", "Flee Use W Key", Keys.Z, KeyBindType.Press);
            }
            var RMenu = Menu.Add(new Menu("R", "R.Set"));
            {
                RMenu.GetSlider("Charge", "Charges of R before using R :)", 2, 1, 3);
                RMenu.Add(new MenuKeyBind("AutoR", "Auto R Key", Keys.T, KeyBindType.Toggle));
                RMenu.GetSlider("RCount", "R Count >=", 1, 1, 5);
                RMenu.GetBool("Gapcloser", "Gapcloser R");
                RMenu.GetBool("LCR", "LaneClear R", false);
                RMenu.GetBool("JCR", "JungleClear R", false);
                RMenu.GetSlider("MinionR", "R Min Minion Count >=", 3, 2, 8);
            }

            var DrawMenu = Menu.Add(new Menu("Draw", "Draw"));
            {
                DrawMenu.GetBool("Q", "Q Range");
                DrawMenu.GetBool("R", "R Range");
                DrawMenu.GetBool("DrawDamge", "Draw Damge", false);
            }

            PlaySharp.Write(GameObjects.Player.ChampionName + "OK! :)");

            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Events.OnGapCloser += OnGapCloser;
            Drawing.OnDraw += OnDraw;
            Variables.Orbwalker.OnAction += OnAction;
        }

        private static void OnAction(object sender, OrbwalkingActionArgs args)
        {
            try
            {
                var ComboQ = Menu["Q"]["ComboQ"].GetValue<MenuBool>();
                var HarassQ = Menu["Q"]["HarassQ"].GetValue<MenuBool>();
                var ADQ = Menu["Q"]["ADQ"].GetValue<MenuBool>();
                var CheckQA = Menu["Q"]["CheckAA"].GetValue<MenuBool>();

                if (args.Type == OrbwalkingType.AfterAttack)
                {
                    var QTarget = GetTarget(680, Q.DamageType);

                    var Attack = GetAttackRange(QTarget);

                    if (QTarget != null && Combo && ComboQ)
                    {
                        if (CheckQA)
                        {
                            if (ADQ && Marksman.Contains(QTarget.CharData.BaseSkinName) && Q.IsReady() && Q.IsInRange(QTarget, -170))
                            {
                                Q.Cast(QTarget);
                            }
                            else if (Q.IsReady() && Q.IsInRange(QTarget, -100))
                            {
                                Q.Cast(QTarget);
                            }
                        }
                        else if (ADQ && Marksman.Contains(QTarget.CharData.BaseSkinName) && Q.IsReady() && Q.IsInRange(QTarget))
                        {
                            Q.Cast(QTarget);
                        }
                        else if (ADQ && Q.IsReady() && Q.IsInRange(QTarget))
                        {
                            Q.Cast(QTarget);
                        }
                    }
                    if (QTarget != null && Harass && HarassQ)
                    {
                        if (CheckQA)
                        {
                            if (Q.IsReady() && Q.IsInRange(QTarget, -70))
                            {
                                Q.Cast(QTarget);
                            }
                        }
                        else if (Q.IsReady() && Q.IsReady() && Q.IsInRange(QTarget))
                        {
                            Q.Cast(QTarget);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error In OnAction" + ex);
            }
        }

        private static void OnDraw(EventArgs args)
        {
            try
            {                
                if (Menu["Draw"]["Q"].GetValue<MenuBool>() && Q.Level >= 1)
                {
                    Render.Circle.DrawCircle(Player.Position, Q.Range + Player.BoundingRadius, Q.IsReady() ? System.Drawing.Color.Cyan : System.Drawing.Color.DarkRed);
                }
                if (Menu["Draw"]["R"].GetValue<MenuBool>() && R.Level >= 1)
                {
                    Render.Circle.DrawCircle(Player.Position, R.Range + Player.BoundingRadius, R.IsReady() ? System.Drawing.Color.AliceBlue : System.Drawing.Color.Beige);
                }
                if (Menu["Draw"]["DrawDamge"])
                {
                    foreach (var t in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget() && !x.IsDead && !x.IsZombie))
                    {
                        if (t != null)
                        {
                            HpBarDraw.Unit = t;
                            HpBarDraw.DrawDmg((float)GetDamage(t), new ColorBGRA(255, 200, 0, 170));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error In OnDraw" + ex);
            }
        }

        private static void OnGapCloser(object sender, Events.GapCloserEventArgs args)
        {
            if (!R.IsReady()) return;
                
            if (Menu["R"]["Gapcloser"].GetValue<MenuBool>())
            {
                if (args.Sender.IsValidTarget() && args.Sender.IsFacing(Player) && args.Sender.IsTargetable)
                {
                    R.Cast(args.Sender.Position);
                }
            }
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name == "TeemoRCast")
            {
                RCast = Variables.TickCount;
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            try
            {

                R = new Spell(SpellSlot.R, RRange);

                if (Player.IsDead)
                    return;

                ComboLogic(args);

                LaneClearLogic(args);

                JungleClear(args);

                if (None && Menu["W"]["FleeKey"].GetValue<MenuKeyBind>().Active)
                {
                    if (EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos))
                    {
                        if (W.IsReady())
                        {
                            W.Cast(Player);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error In OnUpdate" + ex);
            }
        }

        private static void ComboLogic(EventArgs args)
        {
            try
            {
                var ComboW = Menu["W"]["ComboW"].GetValue<MenuBool>().Value;
                var ComboR = Menu["R"]["AutoR"].GetValue<MenuKeyBind>();
                var Charge = Menu["R"]["Charge"].GetValue<MenuSlider>();
                var enemy = GameObjects.EnemyHeroes.FirstOrDefault(t => t.IsValidTarget()
                && InAutoAttackRange(t));
                {
                    if (Combo && W.IsReady() && ComboW && !Menu["W"]["WRange"].GetValue<MenuBool>())
                    {
                        W.Cast();
                    }

                    if (enemy == null) return;

                    if (ComboW && Menu["W"]["WRange"].GetValue<MenuBool>())
                    {
                        if (W.IsReady())
                            W.Cast();
                    }
                    if (Combo && ComboR.Active && R.IsReady())
                    {
                        var RTarget = GetTarget(300, R.DamageType);

                        if (R.IsInRange(RTarget) && Charge <= Player.Spellbook.GetSpell(SpellSlot.R).Ammo
                            && RTarget.IsValidTarget() && !Trap)
                        {
                            R.CastIfHitchanceEquals(RTarget, HitChance.High);
                        }
                        else if (R.IsReady() && ComboR.Active && Charge <= Player.Spellbook.GetSpell(SpellSlot.R).Ammo)
                        {
                            var RS = GameObjects.Get<Obj_AI_Base>().FirstOrDefault(t =>
                            t.Name == "Noxious Trap");

                            if (RS != null)
                            {
                                var Position = RS.Position;
                                var predictionPosition = Position.Extend(RTarget.Position, Player.CharData.AcquisitionRange * R.Level + 2);

                                if (R.IsInRange(RTarget, Player.CharData.AcquisitionRange * R.Level + 2) && Trap)
                                {
                                    R.Cast(predictionPosition);
                                }
                            }
                        }
                    }
                }
                if (Menu["Q"]["KSQ"].GetValue<MenuBool>() && Combo)
                {
                    var QTarget = GameObjects.EnemyHeroes.Where(x => x.IsValidTarget() && Q.IsInRange(x) && GetDamage(x) >= x.Health).OrderBy(t => t.Health).FirstOrDefault();

                    if (QTarget != null)
                    {
                        if (Q.IsReady())
                        {
                            Q.Cast(QTarget);
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error In ComboLogic" + ex);
            }
        }

        private static void LaneClearLogic(EventArgs args)
        {
            try
            {
                var LCR = Menu["R"]["LCR"].GetValue<MenuBool>().Value;
                var allMinionsR = GetMinions(Player.Position, R.Range);
                var RLocation = R.GetCircularFarmLocation(allMinionsR, R.Range);
                var MinionR = Menu["R"]["MinionR"].GetValue<MenuSlider>().Value;

                if (LaneClear && MinionR <= RLocation.MinionsHit && LCR)
                {
                    foreach (var minion in allMinionsR)
                    {
                        if (minion.Health <= Player.GetSpellDamage(minion, SpellSlot.R)
                            && R.IsReady() && R.IsInRange(RLocation.Position.ToVector3()) && !Trap && MinionR <= RLocation.MinionsHit)
                        {
                            R.Cast(RLocation.Position);
                            return;
                        }
                    }
                }
                else if (R.IsReady() && R.IsInRange(RLocation.Position.ToVector3())
                    && !Trap && MinionR <= RLocation.MinionsHit)
                {
                    R.Cast(RLocation.Position);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error In LaneClearLogic" + ex);
            }
        }

        private static void JungleClear(EventArgs args)
        {
            try
            {
                var JCQ = Menu["Q"]["JCQ"].GetValue<MenuBool>().Value;
                var JCR = Menu["R"]["JCR"].GetValue<MenuBool>().Value;
                var ManaQ = Menu["Q"]["ManaQ"].GetValue<MenuSlider>().Value;

                if (LaneClear)
                {
                    var JungleMinionQ = GameObjects.JungleLarge.Where(
                        x => Q.IsInRange(x) && x.Team == GameObjectTeam.Neutral && x.IsValidTarget()).OrderBy(t => t.MaxHealth).FirstOrDefault();
                    var JungleMinionR = GameObjects.JungleLarge.Where(
                        x => R.IsInRange(x) && x.Team == GameObjectTeam.Neutral && x.IsValidTarget()).OrderBy(t => t.MaxHealth).FirstOrDefault();

                    if (JCQ && JungleMinionQ != null)
                    {
                        if (Q.IsReady() && ManaQ <= (int)Player.ManaPercent)
                        {
                            Q.CastOnUnit(JungleMinionQ);
                        }
                    }
                    if (JCR && JungleMinionR != null)
                    {
                        if (R.IsReady() && Player.Spellbook.GetSpell(SpellSlot.R).Ammo >= 1)
                        {
                            R.Cast(JungleMinionR.Position);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error In JungleClear" + ex);
            }
        }

        private static string[] Marksman =
        {
            "Ashe", "Caitlyn", "Corki", "Draven", "Ezreal", "Jinx", "Kalista",
            "KogMaw", "Lucian", "MissFortune", "Quinn", "Sivir", "Teemo", "Tristana", "Twitch", "Urgot", "Varus",
            "Vayne"
        };
    }
}