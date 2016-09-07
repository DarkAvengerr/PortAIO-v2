using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Flowers_Series.Plugings
{
    using Common;
    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;
    using SharpDX;
    using System;
    using System.Linq;
    using static Common.Manager;

    public class Graves
    {
        private static Spell Q;
        private static Spell W;
        private static Spell E;
        private static Spell R;
        private static HpBarDraw HpBarDraw = new HpBarDraw();
        private static float SearchERange;

        private static Menu Menu => Program.Menu;
        private static AIHeroClient Me => Program.Me;

        public static void Init()
        {
            Q = new Spell(SpellSlot.Q, 800f);
            W = new Spell(SpellSlot.W, 900f);
            E = new Spell(SpellSlot.E, 425f);
            R = new Spell(SpellSlot.R, 1050f);

            Q.SetSkillshot(0.25f, 40f, 3000f, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.25f, 250f, 1000f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.25f, 100f, 2100f, false, SkillshotType.SkillshotLine);

            SearchERange = E.Range + Me.GetRealAutoAttackRange() - 100;

            var ComboMenu = Menu.Add(new Menu("Graves_Combo", "Combo"));
            {
                ComboMenu.Add(new MenuBool("Q", "Use Q", true));
                ComboMenu.Add(new MenuBool("W", "Use W", true));
                ComboMenu.Add(new MenuBool("E", "Use E", true));
                ComboMenu.Add(new MenuBool("ETower", "Use E|Safe Check", true));
                ComboMenu.Add(new MenuBool("R", "Use R", false));
                ComboMenu.Add(new MenuSlider("RHit", "R Min Hit Enemies Count >= ", 4, 1, 6));
            }

            var HarassMenu = Menu.Add(new Menu("Graves_Harass", "Harass"));
            {
                HarassMenu.Add(new MenuBool("Q", "Use Q", true));
                HarassMenu.Add(new MenuSlider("Mana", "When Player ManaPercent >= %", 20));
            }

            var LaneClearMenu = Menu.Add(new Menu("Graves_LaneClear", "LaneClear"));
            {
                LaneClearMenu.Add(new MenuBool("Q", "Use Q", true));
                LaneClearMenu.Add(new MenuSlider("Hit", "Min Q Hit Counts >=", 3, 1, 5));
                LaneClearMenu.Add(new MenuSlider("Mana", "When Player ManaPercent >= %", 20));
            }

            var JungleClearMenu = Menu.Add(new Menu("Graves_JungleClear", "JungleClear"));
            {
                JungleClearMenu.Add(new MenuBool("Q", "Use Q", true));
                JungleClearMenu.Add(new MenuBool("E", "Use E", true));
                JungleClearMenu.Add(new MenuSlider("Mana", "When Player ManaPercent >= %", 20));
            }

            var AutoMenu = Menu.Add(new Menu("Graves_Auto", "Auto"));
            {
                AutoMenu.Add(new MenuBool("Q", "Use Q", true));
                AutoMenu.Add(new MenuBool("R", "Use R", true));
            }

            var RList = Menu.Add(new Menu("Graves_RList", "R BlackList"));
            {
                if (GameObjects.EnemyHeroes.Any())
                {
                    GameObjects.EnemyHeroes.ForEach(i => RList.Add(new MenuBool(i.ChampionName.ToLower(), i.ChampionName, !AutoEnableList.Contains(i.ChampionName))));
                }
            }

            var MiscMenu = Menu.Add(new Menu("Graves_Misc", "Misc"));
            {
                MiscMenu.Add(new MenuBool("GapCloser", "Use W|Anti GapCloser", true));
            }

            var DrawMenu = Menu.Add(new Menu("Graves_Draw", "Drawing"));
            {
                DrawMenu.Add(new MenuBool("Q", "Q"));
                DrawMenu.Add(new MenuBool("W", "W"));
                DrawMenu.Add(new MenuBool("E", "E"));
                DrawMenu.Add(new MenuBool("R", "R"));
                DrawMenu.Add(new MenuBool("DrawDamage", "Draw Combo Damage", true));
            }

            WriteConsole(GameObjects.Player.ChampionName + " Inject!");

            Game.OnUpdate += OnUpdate;
            Events.OnGapCloser += OnGapCloser;
            Variables.Orbwalker.OnAction += OnAction;
            Obj_AI_Base.OnSpellCast += OnSpellCast;
            Drawing.OnDraw += OnDraw;
        }


        internal static void OnUpdate(EventArgs args)
        {
            if (Me.IsDead || Me.IsRecalling())
            {
                return;
            }

            if (InCombo)
            {
                ComboLogic();
            }

            if (InHarass)
            {
                HarassLogic();
            }

            if (InClear)
            {
                LaneLogic();
                JungleLogic();
            }

            AutoLogic();
        }

        private static void ComboLogic()
        {
            var target = GetTarget(R);

            if (CheckTarget(target))
            {
                if (Menu["Graves_Combo"]["Q"] && Q.IsReady() && Q.IsInRange(target))
                {
                    var QPred = Q.GetPrediction(target, true);

                    if (QPred.Hitchance >= HitChance.High)
                    {
                        Q.Cast(QPred.CastPosition);
                    }
                }

                if (Menu["Graves_Combo"]["E"] && E.IsReady())
                {
                    if (CanCaseE(target, Game.CursorPos))
                    {
                        E.Cast(Game.CursorPos);
                        Variables.Orbwalker.ResetSwingTimer();
                    }
                }

                if (Menu["Graves_Combo"]["W"] && W.IsReady() && W.IsInRange(target))
                {
                    var WPred = W.GetPrediction(target);

                    if (WPred.Hitchance >= HitChance.VeryHigh)
                    {
                        W.Cast(WPred.CastPosition);
                    }
                }

                if (Menu["Graves_Combo"]["R"] && R.IsReady() && !Q.IsReady() && R.IsInRange(target))
                {
                    R.CastIfWillHit(target, Menu["Graves_Combo"]["RHit"].GetValue<MenuSlider>().Value);
                }
            }
        }

        private static void HarassLogic()
        {
            var target = GetTarget(Q);

            if (CheckTarget(target) && Menu["Graves_Harass"]["Q"] && 
                Me.ManaPercent >= Menu["Graves_Harass"]["Mana"].GetValue<MenuSlider>().Value && 
                Q.IsReady() && target.IsValidTarget(Q.Range))
            {
                var QPred = Q.GetPrediction(target, true);

                if (QPred.Hitchance >= HitChance.High)
                {
                    Q.Cast(QPred.CastPosition);
                }
            }
        }

        private static void LaneLogic()
        {
            var Minions = GetMinions(Me.Position, Q.Range);

            if (Menu["Graves_LaneClear"]["Q"] && Me.ManaPercent >= Menu["Graves_LaneClear"]["Mana"].GetValue<MenuSlider>().Value &&
                Q.IsReady() && Minions.Count() > 0)
            {
                var QFarm = Q.GetLineFarmLocation(Minions, Q.Width);

                if (QFarm.MinionsHit >= Menu["Graves_LaneClear"]["Hit"].GetValue<MenuSlider>().Value)
                {
                    Q.Cast(QFarm.Position);
                }
            }
        }

        private static void JungleLogic()
        {
            var Mobs = GetMobs(Me.Position, Q.Range, true);

            if (Mobs.Count() > 0)
            {
                if (Menu["Graves_JungleClear"]["Q"] && Me.ManaPercent >= Menu["Graves_JungleClear"]["Mana"].GetValue<MenuSlider>().Value && Q.IsReady())
                {
                    Q.Cast(Mobs[0].Position);
                }
            }
        }

        private static void AutoLogic()
        {
            if (Menu["Graves_Auto"]["Q"] && Q.IsReady())
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(t => !t.IsDead && !t.IsZombie && t.IsValidTarget(Q.Range) && Q.IsInRange(t)))
                {
                    if (CheckTarget(target))
                    {
                        if (!CanMove(target))
                        {
                            Q.Cast(target);
                        }

                        if (target.Health < Q.GetDamage(target))
                        {
                            Q.Cast(target);
                        }
                    }
                }
            }

            if (Menu["Graves_Auto"]["R"] && R.IsReady())
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(t => !t.IsDead && !t.IsZombie && t.IsValidTarget(R.Range) && R.IsInRange(t)))
                {
                    if (CheckTarget(target))
                    {
                        if (target.Health < R.GetDamage(target) && target.DistanceToPlayer() > GetAttackRange(Me) + 100 && !Menu["Graves_RList"][target.ChampionName.ToLower()])
                        {
                            R.Cast(target);
                        }
                    }
                }
            }
        }

        private static void OnGapCloser(object sender, Events.GapCloserEventArgs args)
        {
            if (args.IsDirectedToPlayer)
            {
                if (Menu["Graves_Misc"]["GapCloser"] && W.IsReady() && args.Sender.IsValidTarget(W.Range) && args.End.DistanceToPlayer() <= 200)
                {
                    W.Cast(args.End);
                }
            }
        }

        private static void OnAction(object sender, OrbwalkingActionArgs args)
        {
            if (args.Type == OrbwalkingType.AfterAttack)
            {
                if (args.Target is Obj_AI_Minion && InClear)
                {
                    var Mobs = GetMobs(Me.Position, GetAttackRange(Me), true);

                    if (Mobs.Count() > 0 && Menu["Graves_JungleClear"]["E"] && Me.ManaPercent >= Menu["Graves_JungleClear"]["Mana"].GetValue<MenuSlider>().Value && E.IsReady())
                    {
                        if (CanCaseE(Mobs[0], Game.CursorPos) && Me.Spellbook.IsAutoAttacking && !Me.Spellbook.IsCastingSpell)
                        {
                            E.Cast(Game.CursorPos);
                            Variables.Orbwalker.ResetSwingTimer();
                        }
                    }
                }
            }
        }

        private static void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (args.Target is AIHeroClient && InCombo)
            {
                var t = args.Target as AIHeroClient;

                if (E.IsReady() && t.DistanceToPlayer() <= E.Range)
                {
                    if (Menu["Graves_Combo"]["E"] && E.IsReady())
                    {
                        if (CanCaseE(t, Game.CursorPos))
                        {
                            E.Cast(Game.CursorPos);
                            Variables.Orbwalker.ResetSwingTimer();
                        }
                    }
                }
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (Me.IsDead)
                return;

            if (Menu["Graves_Draw"]["Q"] && Q.IsReady())
            {
                Render.Circle.DrawCircle(Me.Position, Q.Range, System.Drawing.Color.Red);
            }

            if (Menu["Graves_Draw"]["W"] && W.IsReady())
            {
                Render.Circle.DrawCircle(Me.Position, W.Range, System.Drawing.Color.Orange);
            }

            if (Menu["Graves_Draw"]["E"] && E.IsReady())
            {
                Render.Circle.DrawCircle(Me.Position, E.Range, System.Drawing.Color.LightSeaGreen);
            }

            if (Menu["Graves_Draw"]["R"] && R.IsReady())
            {
                Render.Circle.DrawCircle(Me.Position, R.Range, System.Drawing.Color.YellowGreen);
            }

            if (Menu["Draw"]["DrawDamage"])
            {
                foreach (var e in ObjectManager.Get<AIHeroClient>().Where(e => e.IsValidTarget() && e.IsValid && !e.IsDead && !e.IsZombie))
                {
                    HpBarDraw.Unit = e;
                    HpBarDraw.DrawDmg((float)GetDamage(e), new SharpDX.ColorBGRA(255, 204, 0, 170));
                }
            }
        }


        private static bool CanCaseE(Obj_AI_Base target, Vector3 Pos)
        {
            if (E.IsReady() && target.IsValidTarget(SearchERange) && !Me.IsUnderEnemyTurret())
            {
                var EndPos = Me.ServerPosition.Extend(Pos, E.Range);

                if (!EndPos.IsWall())
                {
                    if (EndPos.IsUnderEnemyTurret() && Menu["Graves_Combo"]["ETower"])
                    {
                        return false;
                    }

                    if (EndPos.CountEnemyHeroesInRange(E.Range) >= 3 && Me.HealthPercent >= 80)
                    {
                        return true;
                    }

                    if (EndPos.CountEnemyHeroesInRange(E.Range) < 3)
                    {
                        return true;
                    }

                    if (target.Distance(EndPos) < Me.GetRealAutoAttackRange())
                    {
                        return true;
                    }

                    if (!target.IsValidTarget(E.Range) && target.IsValidTarget(SearchERange) && Me.MoveSpeed > target.MoveSpeed)
                    {
                        return true;
                    }

                    if (!Me.HasBuff("gravesbasicattackammo2") && Me.HasBuff("gravesbasicattackammo1") && target.IsValidTarget(Me.GetRealAutoAttackRange()))
                    {
                        return true;
                    }
                }
                else
                    return false;
            }

            return false;
        }
    }
}
