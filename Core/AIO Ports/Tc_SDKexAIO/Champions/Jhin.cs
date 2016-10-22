using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Tc_SDKexAIO.Champions
{

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;

    using LeagueSharp.SDK.Utils;
    using LeagueSharp.SDK.UI;

    using System;
    using System.Linq;
    using System.Windows.Forms;

    using SharpDX;

    using Common;
    using Config;
    using static Common.Manager;

    using Menu = LeagueSharp.SDK.UI.Menu;

    internal static class Jhin
    {

        private static Menu Menu => PlaySharp.ChampionMenu;
        private static AIHeroClient Player => PlaySharp.Player;
        private static HpBarDraw HpBarDraw = new HpBarDraw();
        private static float LasPing = Variables.TickCount;
        private static string StartR = "JhinR";
        private static string IsCastingR = "JhinR";
        private static Spell Q, W, E, R;

        internal static void Init()
        {

            Q = new Spell(SpellSlot.Q, 550);
            W = new Spell(SpellSlot.W, 2500).SetSkillshot(0.75f, 40, float.MaxValue, false, SkillshotType.SkillshotLine);
            E = new Spell(SpellSlot.E, 750).SetSkillshot(0.5f, 120, 1600, false, SkillshotType.SkillshotCircle);
            R = new Spell(SpellSlot.R, 3500).SetSkillshot(0.21f, 80, 5000, false, SkillshotType.SkillshotLine);


            var QMenu = Menu.Add(new Menu("Q", "Q.Set"));
            {
                QMenu.GetSeparator("Q: Always On");
                QMenu.Add(new MenuBool("ComboQ", "Comno Q", true));
                QMenu.Add(new MenuBool("HarassQ", "Harass Q", true));
                QMenu.Add(new MenuBool("LaneClearQ", "LaneClear Q", true));
                QMenu.Add(new MenuBool("JungleQ", "Jungle Q", true));
                QMenu.Add(new MenuBool("KillStealQ", "KillSteal Q", true));
            }

            var WMenu = Menu.Add(new Menu("W", "W.Set"));
            {
                WMenu.Add(new MenuBool("ComboW", "Comno W", true));
                WMenu.Add(new MenuBool("KSW", "Killsteal W", true));
                WMenu.Add(new MenuBool("HarassW", "Harass W", true));
                WMenu.Add(new MenuBool("LaneClearW", "LaneClear W", true));
                WMenu.Add(new MenuBool("WMO", "W Only Marked Target", true));
                WMenu.Add(new MenuKeyBind("WTap", "W Fire On Tap", Keys.G, KeyBindType.Press));
                WMenu.Add(new MenuKeyBind("AutoW", "Use W Auto (Toggle)", Keys.Y, KeyBindType.Toggle));
                WMenu.Add(new MenuSlider("HarassWMana", "Harass W Min Mana > =", 60));
            }

            var EMenu = Menu.Add(new Menu("E", "E.Set"));
            {
                EMenu.GetSeparator("E: Mobe");
                EMenu.Add(new MenuBool("ComboE", "Combo E", true));
                EMenu.Add(new MenuBool("LaneClearE", "LaneClear E", true));
                EMenu.Add(new MenuSlider("LaneClearEMana", "LaneClear E Min Mana", 40, 0, 100));
                EMenu.Add(new MenuSlider("LCminions", "LaneClear Min minion", 5, 3, 8));
                EMenu.GetSeparator("E: Gapcloser | Melee Modes");
                EMenu.Add(new MenuBool("Gapcloser", "Gapcloser E", true));
                EMenu.GetSeparator("Auto E Always");
                EMenu.Add(new MenuKeyBind("ETap", "Force E", Keys.H, KeyBindType.Press));
            }

            var RMenu = Menu.Add(new Menu("R", "R.Set"));
            {
                RMenu.Add(new MenuKeyBind("RTap", "R Fire On Tap", Keys.S, KeyBindType.Press));
                RMenu.Add(new MenuBool("Ping", "Ping Who Can Killable(Every 3 Seconds)", true));
            }

            var DrawMenu = Menu.Add(new Menu("Draw", "Draw"));
            {
                DrawMenu.Add(new MenuBool("Q", "Q Range"));
                DrawMenu.Add(new MenuBool("W", "W Range"));
                DrawMenu.Add(new MenuBool("E", "E Range"));
                DrawMenu.Add(new MenuBool("R", "R Range"));
                DrawMenu.Add(new MenuBool("RDKs", "Draw Who Can Killable With R (3 Fire)", true));
                DrawMenu.Add(new MenuBool("RDind", "Draw R Damage Indicator (3 Fire)", true));
            }

            Menu.Add(new MenuBool("ComboY", "Combo Use Youmoo", true));

            PlaySharp.Write(GameObjects.Player.ChampionName + "OK! :)");


            Obj_AI_Base.OnSpellCast += OnSpellCast;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Events.OnGapCloser += OnGapCloser;
            Drawing.OnDraw += OnDraw;
            Drawing.OnEndScene += OnEndScene;
            Game.OnUpdate += OnUpdate;

        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {

            try
            {
                if (!sender.IsMe && !AutoAttack.IsAutoAttack(args.SData.Name) || !args.Target.IsEnemy
                    || !args.Target.IsValid || !(args.Target is AIHeroClient)) return;
                if (Combo && Menu["ComboY"].GetValue<MenuBool>().Value)
                {
                    CastYoumoo();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error In On ProcessSpellCast" + ex);
            }
        }

        private static void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            try
            {

                if (!sender.IsMe || !AutoAttack.IsAutoAttack(args.SData.Name)) return;

                if (Combo)
                {
                    if (args.Target is AIHeroClient)
                    {
                        var target = (AIHeroClient)args.Target;
                        if (!target.IsDead)
                        {
                            if (Menu["W"]["ComboW"].GetValue<MenuBool>() && W.IsReady())
                            {
                                W.Cast(W.GetPrediction(target).UnitPosition);
                                return;
                            }
                            if (Q.IsReady() && Menu["Q"]["ComboQ"].GetValue<MenuBool>() && Player.Distance(target) <= 550)
                            {
                                Q.Cast(target);
                            }
                        }
                    }
                }
                if (Harass)
                {
                    if (args.Target is AIHeroClient)
                    {
                        var target = (AIHeroClient)args.Target;
                        if (!target.IsDead)
                        {
                            if (Menu["W"]["HarassW"].GetValue<MenuBool>() && W.IsReady())
                            {
                                W.Cast(W.GetPrediction(target).UnitPosition);
                                return;
                            }
                            if (Q.IsReady() && Menu["Q"]["HarassQ"].GetValue<MenuBool>() && Player.Distance(target) <= 550)
                            {
                                Q.Cast(target);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error In On DoCast" + ex);
            }
        }

        private static void OnGapCloser(object sender, Events.GapCloserEventArgs args)
        {
            try
            {
                if (E.IsReady() && !Invulnerable.Check(args.Sender) && args.Sender.IsValidTarget(E.Range))
                {
                    if (Menu["E"]["Gapcloser"].GetValue<MenuBool>().Value)
                    {
                        E.Cast(args.End);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error In On GapCloser" + ex);
            }
        }

        private static void OnUpdate(EventArgs args)
        {

                if (Player.IsDead)
                    return;

                ComboLogic(args);

                HarassLogic(args);

                LaneClearLogic(args);

                KSLogic(args);
        }

        static void CastYoumoo()
        {
            if (Items.CanUseItem(3142))
                Items.UseItem(3142);
        }

        private static void KSLogic(EventArgs args)
        {

            if (Menu["W"]["WTap"].GetValue<MenuKeyBind>().Active)
            {
                if (W.IsReady())
                {
                    var WTarget = GetTarget(W.Range, W.DamageType);

                    if (W.GetPrediction(WTarget).Hitchance >= HitChance.High)
                    {
                        W.Cast(W.GetPrediction(WTarget, true).UnitPosition);
                    }
                }
            }
            if (Menu["Q"]["KillStealQ"].GetValue<MenuBool>())
            {
                if (Q.IsReady())
                {
                    var QTarget = GetTarget(Q.Range, Q.DamageType);

                    if (QTarget.Health <= Q.GetDamage(QTarget))
                    {
                        Q.Cast(QTarget);
                    }
                }
            }
            if (Menu["W"]["KSW"].GetValue<MenuBool>())
            {
                if (W.IsReady())
                {
                    var WTarget = GetTarget(W.Range, W.DamageType);

                    if (WTarget.Health <= W.GetDamage(WTarget) && W.GetPrediction(WTarget).Hitchance >= HitChance.VeryHigh)
                    {
                        W.Cast(W.GetPrediction(WTarget, true).UnitPosition);
                    }
                }
            }
            foreach (var e in GameObjects.Get<AIHeroClient>().Where(x => x.IsValidTarget() && x.Health
                <= R.GetDamage(x) * 3 && !x.IsZombie && !x.IsDead && !x.IsDead))
            {
                if (LasPing <= Variables.TickCount && Menu["R"]["Ping"])
                {
                    LasPing = Variables.TickCount + 3000;
                    TacticalMap.SendPing(PingCategory.Danger, e);
                }
            }
            if (Menu["E"]["ETap"].GetValue<MenuKeyBind>().Active)
            {
                if (E.IsReady())
                {
                    var ETarget = GetTarget(E.Range, E.DamageType);

                    if (!ETarget.IsDead && R.GetPrediction(ETarget).Hitchance >= HitChance.High)
                    {
                        E.Cast(R.GetPrediction(ETarget, true).UnitPosition);
                    }
                }
            }
            if (Menu["R"]["RTap"].GetValue<MenuKeyBind>().Active)
            {
                if (R.IsReady() && R.Instance.Name == StartR)
                {
                    var RTarget = GetTarget(R.Range, R.DamageType);

                    if (RTarget.Health <= R.GetDamage(RTarget) * 3 && !RTarget.IsZombie && !RTarget.IsDead
                        && R.GetPrediction(RTarget).Hitchance >= HitChance.VeryHigh)
                    {
                        if (Items.CanUseItem(3363))
                        {
                            Items.UseItem(3363, RTarget.Position);
                        }
                        R.Cast(R.GetPrediction(RTarget, true).UnitPosition);
                    }
                }
            }
            if (Menu["R"]["RTap"].GetValue<MenuKeyBind>().Active)
            {
                if (Q.IsReady() && R.Instance.Name == IsCastingR)
                {
                    var RTarget = GetTarget(R.Range, R.DamageType);

                    if (Items.CanUseItem(3363))
                    {
                        Items.UseItem(3363, RTarget.Position);
                    }
                    R.Cast(R.GetPrediction(RTarget, true).UnitPosition);
                }
            }
        }


        private static void ComboLogic(EventArgs args)
        {
            if (Combo)
            {
                if (Menu["W"]["ComboW"].GetValue<MenuBool>())
                {
                    if (W.IsReady())
                    {
                        var WTarget = GetTarget(2500, W.DamageType);

                        var WMO = Menu["W"]["WMO"].GetValue<MenuBool>();

                        if (W.GetPrediction(WTarget).Hitchance >= HitChance.VeryHigh
                            && ((WTarget.HasBuff("jhinespotteddebuff") && WMO) || !WMO))
                        {
                            W.Cast(W.GetPrediction(WTarget).UnitPosition);
                        }
                    }
                }
                if (Menu["Q"]["ComboQ"].GetValue<MenuBool>())
                {
                    var QTarget = GetTarget(550, Q.DamageType);

                    if (Q.IsReady() && !Player.Spellbook.IsAutoAttacking && !Variables.Orbwalker.CanAttack)
                    {
                        Q.Cast(QTarget);
                    }
                }
                if (Menu["E"]["ComboE"].GetValue<MenuBool>())
                {
                    var ETarget = Variables.Orbwalker.GetTarget();

                    if (E.IsReady() && ETarget.IsValidTarget())
                    {
                        E.Cast(E.GetPrediction((Obj_AI_Base)ETarget).UnitPosition);
                    }
                }
                if (Menu["W"]["AutoW"].GetValue<MenuKeyBind>().Active)
                {
                    if (Player.ManaPercent >= Menu["W"]["HarassWMana"].GetValue<MenuSlider>().Value)
                    {
                        var WTarget = GetTarget(2500, W.DamageType);

                        var WMO = Menu["W"]["WMO"].GetValue<MenuBool>();

                        if (W.GetPrediction(WTarget).Hitchance >= HitChance.VeryHigh
                            && W.IsReady() && ((WTarget.HasBuff("jhinespotteddebuff")
                            && WMO) && !WMO))
                        {
                            W.Cast(W.GetPrediction(WTarget).UnitPosition);
                        }
                    }
                }
            }
        }

        private static void HarassLogic(EventArgs args)
        {
            if (Harass)
            {
                if (Menu["W"]["HarassW"].GetValue<MenuBool>())
                {
                    var WTarget = GetTarget(2500, W.DamageType);

                    var WMO = Menu["W"]["WMO"].GetValue<MenuBool>();

                    if (W.GetPrediction(WTarget).Hitchance >= HitChance.VeryHigh
                        && W.IsReady() && ((WTarget.HasBuff("jhinespotteddebuff") && WMO) || !WMO))
                    {
                        W.Cast(W.GetPrediction(WTarget).UnitPosition);
                    }
                }
                if (Menu["Q"]["HarassQ"].GetValue<MenuBool>())
                {
                    var QTarget = GetTarget(550, Q.DamageType);

                    if (Q.IsReady() && !Player.Spellbook.IsAutoAttacking && !Variables.Orbwalker.CanAttack)
                    {
                        Q.Cast(QTarget);
                    }
                }
            }
        }

        private static void LaneClearLogic(EventArgs args)
        {
            if (LaneClear)
            {
                if (Menu["Q"]["LaneClearQ"].GetValue<MenuBool>())
                {
                    var minionQ = GameObjects.EnemyMinions.Where(x => x.IsValidTarget(Q.Range)).MinOrDefault(x => x.Health);

                    if (minionQ != null)
                    {
                        Q.Cast(minionQ);
                    }
                }

                if (Menu["Q"]["JungleQ"].GetValue<MenuBool>())
                {
                    var JungleQ = GameObjects.JungleLarge.Where(x => x.IsValidTarget(Q.Range)).MinOrDefault(x => x.Health);
                    if (JungleQ != null)
                    {
                        Q.Cast(JungleQ);
                    }
                }
                if (Menu["W"]["LaneClearW"].GetValue<MenuBool>())
                {
                    var minionW = GameObjects.EnemyMinions.Where(x => x.IsValidTarget(W.Range)).MinOrDefault(x => x.Health);

                    if (minionW != null)
                    {
                        W.Cast(minionW);
                    }
                }
                if (Menu["E"]["LaneClearE"].GetValue<MenuBool>())
                {
                    var minionE = GetMinions(Player.ServerPosition, E.Range);

                    var farmPosition = E.GetCircularFarmLocation(minionE, W.Width);

                    if (Player.ManaPercent > Menu["E"]["LaneClearEMana"].GetValue<MenuSlider>().Value)
                    {
                        if (farmPosition.MinionsHit >= Menu["E"]["LCminions"].GetValue<MenuSlider>().Value)
                            E.Cast(farmPosition.Position);
                    }
                }
            }
        }

        private static void OnEndScene(EventArgs args)
        {
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(x => x.IsValidTarget() && x.IsEnemy))
            {
                if (Menu["Draw"]["RDind"].GetValue<MenuBool>() && R.Level >= 1)
                {
                    HpBarDraw.Unit = enemy;
                    HpBarDraw.DrawDmg(R.GetDamage(enemy) * 3, new ColorBGRA(0, 100, 200, 150));
                }
            }
        }

        private static void OnDraw(EventArgs args)
        {
            try
            {
                if (Menu["Draw"]["RDKs"].GetValue<MenuBool>() && R.IsReady() && R.Level >= 1)
                {
                    var spos = Drawing.WorldToScreen(Player.Position);
                    var target = ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy && x.Health <= R.GetDamage(x) * 3
                    && !x.IsZombie && !x.IsDead);
                    int addpos = 0;
                    foreach (var killable in target)
                    {
                        Drawing.DrawText(spos.X - 50, spos.Y + 35 + addpos, System.Drawing.Color.Red, killable.ChampionName + "Is Killable !!!");
                        addpos = addpos + 15;
                    }
                }
                if (Menu["Draw"]["R"].GetValue<MenuBool>() && R.Level >= 1)
                {
                    Drawing.DrawCircle(Player.Position, 3500, R.IsReady() ? System.Drawing.Color.Cyan : System.Drawing.Color.DarkRed);
                }
                if (Menu["Draw"]["W"].GetValue<MenuBool>() && W.Level >= 1)
                {
                    Drawing.DrawCircle(Player.Position, 2500, W.IsReady() ? System.Drawing.Color.Cyan : System.Drawing.Color.DarkRed);
                }
                if (Menu["Draw"]["E"].GetValue<MenuBool>() && E.Level >= 1)
                {
                    Drawing.DrawCircle(Player.Position, 750, E.IsReady() ? System.Drawing.Color.Cyan : System.Drawing.Color.DarkRed);
                }
                if (Menu["Draw"]["Q"].GetValue<MenuBool>() && Q.Level >= 1)
                {
                    Drawing.DrawCircle(Player.Position, 550 + Player.BoundingRadius, Q.IsReady() ? System.Drawing.Color.Cyan : System.Drawing.Color.DarkRed);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error In On Draw" + ex);
            }
        }
    }
}