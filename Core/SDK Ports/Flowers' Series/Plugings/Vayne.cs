using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Flowers_Series.Plugings
{
    using Common;
    using LeagueSharp;
    using LeagueSharp.Data.Enumerations;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;
    using System;
    using System.Linq;
    using static Common.Manager;

    public static class Vayne
    {
        private static Spell Q;
        private static Spell W;
        private static Spell E;
        private static Spell R;
        private static HpBarDraw HpBarDraw = new HpBarDraw();

        private static Menu Menu => Program.Menu;
        private static AIHeroClient Me => Program.Me;

        public static void Init()
        {
            Q = new Spell(SpellSlot.Q, 800f);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 650f);
            R = new Spell(SpellSlot.R);

            E.SetTargetted(0.25f, 1600f);

            var ComboMenu = Menu.Add(new Menu("Vayne_Combo", "Combo"));
            {
                ComboMenu.Add(new MenuSeparator("QLogic", "Q Logic"));
                ComboMenu.Add(new MenuBool("Q", "Use Q", true));
                ComboMenu.Add(new MenuBool("AQA", "A-Q-A", true));
                ComboMenu.Add(new MenuBool("SafeCheck", "Safe Q Check", true));
                ComboMenu.Add(new MenuBool("QTurret", "Dont Cast In Turret", true));
                ComboMenu.Add(new MenuSeparator("ELogic", "E Logic"));
                ComboMenu.Add(new MenuBool("E", "Use E", true));
                ComboMenu.Add(new MenuSeparator("RLogic", "R Logic"));
                ComboMenu.Add(new MenuBool("R", "Use R", true));
                ComboMenu.Add(new MenuSlider("RCount", "When Enemies Counts >= ", 2, 1, 5));
                ComboMenu.Add(new MenuSlider("RHp", "Or Player HealthPercent <= %", 45));
            }

            var HarassMenu = Menu.Add(new Menu("Vayne_Harass", "Harass"));
            {
                HarassMenu.Add(new MenuBool("Q", "Use Q", true));
                HarassMenu.Add(new MenuBool("SafeCheck", "Safe Q Check", true));
                HarassMenu.Add(new MenuBool("QTurret", "Dont Cast In Turret", true));
                HarassMenu.Add(new MenuBool("E", "Use E | Only Target have 2 Passive"));
            }

            var LaneClearMenu = Menu.Add(new Menu("Vayne_LaneClear", "LaneClear"));
            {
                LaneClearMenu.Add(new MenuBool("Q", "Use Q", true));
                LaneClearMenu.Add(new MenuBool("QTurret", "Use Q To Attack Tower", true));
                LaneClearMenu.Add(new MenuSlider("Mana", "Min LaneClear Mana >= %", 50));
            }

            var JungleClearMenu = Menu.Add(new Menu("Vayne_JungleClear", "JungleClear"));
            {
                JungleClearMenu.Add(new MenuBool("Q", "Use Q", true));
                JungleClearMenu.Add(new MenuBool("E", "Use E", true));
                JungleClearMenu.Add(new MenuSlider("Mana", "Min JungleClear Mana >= %", 30));
            }

            var AutoMenu = Menu.Add(new Menu("Vayne_Auto", "Auto"));
            {
                AutoMenu.Add(new MenuSeparator("ELogic", "E Logic"));
                AutoMenu.Add(new MenuBool("E", "Use E"));
                if (GameObjects.EnemyHeroes.Any())
                {
                    GameObjects.EnemyHeroes.ForEach(i => AutoMenu.Add(new MenuBool("CastE" + i.ChampionName, "Cast To :" + i.ChampionName, AutoEnableList.Contains(i.ChampionName))));
                }
                AutoMenu.Add(new MenuSeparator("RLogic", "R Logic"));
                AutoMenu.Add(new MenuBool("R", "Use R", true));
                AutoMenu.Add(new MenuSlider("RCount", "When Enemies Counts >= ", 3, 1, 5));
                AutoMenu.Add(new MenuSlider("RRange", "Search Enemies Range ", 600, 500, 1200));
            }

            var EMenu = Menu.Add(new Menu("Vayne_E", "E Settings"));
            {
                EMenu.Add(new MenuBool("Gapcloser", "Anti Gapcloser", true));
                EMenu.Add(new MenuBool("AntiAlistar", "Anti Alistar", true));
                EMenu.Add(new MenuBool("AntiRengar", "Anti Rengar", true));
                EMenu.Add(new MenuBool("AntiKhazix", "Anti Khazix", true));
                EMenu.Add(new MenuBool("Interrupt", "Interrupt Danger Spells", true));
                EMenu.Add(new MenuBool("Under", "Dont Cast In Turret", true));
                EMenu.Add(new MenuSlider("Push", "Push Tolerance", -30, -100, 100));
            }

            var Draw = Menu.Add(new Menu("Vayne_Draw", "Draw"));
            {
                Draw.Add(new MenuBool("E", "Draw E Range"));
                Draw.Add(new MenuBool("Damage", "Draw Combo Damage", true));
            }

            WriteConsole(GameObjects.Player.ChampionName + " Inject!");

            GameObject.OnCreate += OnCreate;
            Game.OnUpdate += OnUpdate;
            Variables.Orbwalker.OnAction += OnAction;
            Events.OnInterruptableTarget += OnInterruptableTarget;
            Events.OnGapCloser += OnGapCloser;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Drawing.OnDraw += OnDraw;
        }

        private static void OnCreate(GameObject sender, EventArgs Args)
        {
            if (Menu["Vayne_E"]["Gapcloser"] && E.IsReady())
            {
                var Rengar = GameObjects.EnemyHeroes.Find(heros => heros.ChampionName.Equals("Rengar"));
                var Khazix = GameObjects.EnemyHeroes.Find(heros => heros.ChampionName.Equals("Khazix"));

                if (Rengar != null && Menu["Vayne_E"]["AntiRengar"])
                {
                    if (sender.Name == ("Rengar_LeapSound.troy") && sender.Distance(Me) < E.Range)
                        E.CastOnUnit(Rengar);
                }

                if (Khazix != null && Menu["Vayne_E"]["AntiKhazix"])
                {
                    if (sender.Name == ("Khazix_Base_E_Tar.troy") && sender.Distance(Me) <= 300)
                        E.CastOnUnit(Khazix);
                }
            }
        }

        private static void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead)
                return;

            if (!E.IsReady())
            {
                return;
            }

            if (InCombo && Menu["Vayne_Combo"]["E"] && E.IsReady())
            {
                var target = GetTarget(E.Range);

                if (CheckTarget(target))
                {
                    ELogic(target);
                }
            }

            if (InHarass && Menu["Vayne_Harass"]["E"] && E.IsReady())
            {
                var target = GetTarget(E.Range);

                if (CheckTarget(target) && GetPassive(target) == 2)
                {
                    E.CastOnUnit(target);
                }
            }

            if (InClear && Menu["Vayne_JungleClear"]["E"] && E.IsReady() && 
                Me.ManaPercent >= Menu["Vayne_JungleClear"]["Mana"].GetValue<MenuSlider>().Value)
            {
                var mob = GetMobs(Me.Position, E.Range, true).FirstOrDefault();

                if (mob != null && mob.IsValidTarget(E.Range))
                {
                    E.CastOnUnit(mob);
                }
            }

            if (Menu["Vayne_Auto"]["E"] && !InCombo)
            {
                var target = GetTarget(E.Range);

                if (CheckTarget(target) && Menu["Vayne_Auto"]["CastE" + target.ChampionName])
                {
                    ELogic(target);
                }
            }
        }

        private static void OnAction(object obj, OrbwalkingActionArgs Args)
        {
            if (!Q.IsReady())
            {
                return;
            }

            if (Args.Type == OrbwalkingType.AfterAttack)
            {
                AfterQLogic(Args);
            }

            if (Args.Type == OrbwalkingType.BeforeAttack)
            {
                BeforeQLogic(Args);
            }
        }

        private static void AfterQLogic(OrbwalkingActionArgs Args)
        {
            if (InCombo && Args.Target is AIHeroClient && Menu["Vayne_Combo"]["Q"] && Menu["Vayne_Combo"]["AQA"])
            {
                var target = Args.Target as AIHeroClient;

                if (CheckTarget(target) && target.IsValidTarget(Q.Range) && Q.IsReady())
                {
                    var AfterQPosition = Me.ServerPosition + (Game.CursorPos - Me.ServerPosition).Normalized() * 250;
                    var Distance = target.ServerPosition.Distance(AfterQPosition);

                    if (Menu["Vayne_Combo"]["QTurret"] && AfterQPosition.IsUnderEnemyTurret())
                    {
                        return;
                    }

                    if (Menu["Vayne_Combo"]["SafeCheck"] && AfterQPosition.CountEnemyHeroesInRange(300) >= 3)
                    {
                        return;
                    }

                    if (Distance <= 650 && Distance >= 300)
                    {
                        Q.Cast(Game.CursorPos);
                        return;
                    }
                }
            }
            else if (InClear)
            {
                if (Args.Target is Obj_AI_Turret && Menu["Vayne_LaneClear"]["Q"]
                    && Me.ManaPercent >= Menu["Vayne_LaneClear"]["Mana"].GetValue<MenuSlider>().Value &&
                    Menu["Vayne_LaneClear"]["QTurret"] && Me.CountEnemyHeroesInRange(900) == 0 && Q.IsReady())
                {
                    Q.Cast(Game.CursorPos);
                }

                if (Args.Target is Obj_AI_Minion)
                {
                    LaneClearQ(Args);
                    JungleQ(Args);
                }
            }
        }

        private static void LaneClearQ(OrbwalkingActionArgs Args)
        {
            if (Menu["Vayne_LaneClear"]["Q"] && Me.ManaPercent >= Menu["Vayne_LaneClear"]["Mana"].GetValue<MenuSlider>().Value)
            {
                var minions = GetMinions(Me.Position, GetAttackRange(Me) + 175).Where(m => m.Health < (Q.GetDamage(m) + Me.GetAutoAttackDamage(m)));

                if (minions.Count() > 0 && Args.Target.NetworkId != minions.FirstOrDefault().NetworkId)
                {
                    Q.Cast(Game.CursorPos);
                    Variables.Orbwalker.ForceTarget = minions.FirstOrDefault();
                }
            }
        }

        private static void JungleQ(OrbwalkingActionArgs Args)
        {
            if (Menu["Vayne_JungleClear"]["Q"] && Me.ManaPercent >= Menu["Vayne_JungleClear"]["Mana"].GetValue<MenuSlider>().Value)
            {
                var mobs = GetMobs(Me.Position, GetAttackRange(Me), true);

                if (mobs.Count() > 0 && Q.IsReady())
                {
                    Q.Cast(Game.CursorPos);
                }
            }
        }

        private static void BeforeQLogic(OrbwalkingActionArgs Args)
        {
            if (!(Args.Target is AIHeroClient))
            {
                return;
            }

            var target = Args.Target as AIHeroClient;

            if (InCombo && Args.Target is AIHeroClient && Menu["Vayne_Combo"]["Q"])
            {
                if (CheckTarget(target) && target.IsValidTarget(Q.Range) && Q.IsReady())
                {
                    var AfterQPosition = Me.ServerPosition + (Game.CursorPos - Me.ServerPosition).Normalized() * 250;
                    var Distance = target.ServerPosition.Distance(AfterQPosition);

                    if (Menu["Vayne_Combo"]["QTurret"] && AfterQPosition.IsUnderEnemyTurret())
                    {
                        return;
                    }

                    if (Menu["Vayne_Combo"]["SafeCheck"] && AfterQPosition.CountEnemyHeroesInRange(300) >= 3)
                    {
                        return;
                    }

                    if (target.DistanceToPlayer() >= 600 && Distance <= 600)
                    {
                        Q.Cast(Game.CursorPos);
                        return;
                    }
                }
            }

            if (InHarass && Args.Target is AIHeroClient && Menu["Vayne_Harass"]["Q"])
            {
                if (CheckTarget(target) && target.IsValidTarget(800) && Q.IsReady())
                {
                    var AfterQPosition = Me.ServerPosition + (Game.CursorPos - Me.ServerPosition).Normalized() * 250;
                    var Distance = target.ServerPosition.Distance(AfterQPosition);

                    if (Menu["Vayne_Harass"]["QTurret"] && AfterQPosition.IsUnderEnemyTurret())
                    {
                        return;
                    }

                    if (Menu["Vayne_Harass"]["SafeCheck"] && AfterQPosition.CountEnemyHeroesInRange(300) >= 2)
                    {
                        return;
                    }

                    if (target.DistanceToPlayer() >= 600 && Distance <= 600)
                    {
                        Q.Cast(Game.CursorPos);
                        return;
                    }
                }
            }
        }

        private static void OnInterruptableTarget(object obj, Events.InterruptableTargetEventArgs Args)
        {
            if (Menu["Vayne_E"]["Interrupt"] && E.IsReady() && Args.Sender.IsValidTarget(E.Range))
            {
                if (Args.Sender.IsCastingInterruptableSpell())
                {
                    E.CastOnUnit(Args.Sender);
                }

                if (Args.DangerLevel >= DangerLevel.Medium)
                {
                    E.CastOnUnit(Args.Sender);
                }
            }
        }

        private static void OnGapCloser(object obj, Events.GapCloserEventArgs Args)
        {
            if (Args.IsDirectedToPlayer)
            {
                if (Menu["Vayne_E"]["Gapcloser"] && E.IsReady())
                {
                    if (Menu["Vayne_E"]["AntiAlistar"] && Args.Sender.ChampionName == "Alistar" && Args.SkillType == GapcloserType.Targeted)
                    {
                        E.CastOnUnit(Args.Sender);
                    }
                    else if (Args.End.DistanceToPlayer() <= 250 && Args.Target.IsValid) 
                    {
                        E.CastOnUnit(Args.Sender);
                    }
                }
            }
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (!R.IsReady())
            {
                return;
            }

            if (InCombo && Menu["Vayne_Combo"]["R"])
            {
                if (Me.CountEnemyHeroesInRange(800) >= Menu["Vayne_Combo"]["RCount"].GetValue<MenuSlider>().Value)
                {
                    R.Cast();
                }

                if (Me.CountEnemyHeroesInRange(GetAttackRange(Me)) >= 1 && Me.HealthPercent <= Menu["Vayne_Combo"]["RHp"].GetValue<MenuSlider>().Value)
                {
                    R.Cast();
                }
            }

            if (Menu["Vayne_Auto"]["R"] &&
                Me.CountEnemyHeroesInRange(Menu["Vayne_Auto"]["RRange"].GetValue<MenuSlider>().Value) >= 
                Menu["Vayne_Auto"]["RCount"].GetValue<MenuSlider>().Value)
            {
                R.Cast();
            }
        }

        private static void OnDraw(EventArgs Args)
        {
            if (Me.IsDead)
                return;

            if (Menu["Vayne_Draw"]["E"] && E.IsReady())
            {
                Render.Circle.DrawCircle(Me.Position, E.Range, System.Drawing.Color.AliceBlue);
            }

            if (Menu["Vayne_Draw"]["Damage"])
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget() && !x.IsDead && !x.IsZombie))
                {
                    if (target != null)
                    {
                        HpBarDraw.Unit = target;

                        HpBarDraw.DrawDmg((float)GetDamage(target), new SharpDX.ColorBGRA(255, 200, 0, 170));
                    }
                }
            }
        }

        private static void ELogic(Obj_AI_Base target)
        {
            if (Menu["Vayne_E"]["Under"] && Me.IsUnderEnemyTurret())
            {
                return;
            }

            if (target != null && target.IsHPBarRendered)
            {
                var EPred = E.GetPrediction(target);
                var PD = 425 + Menu["Vayne_E"]["Push"].GetValue<MenuSlider>().Value;
                var PP = EPred.UnitPosition.Extend(Me.Position, -PD);

                for (int i = 1; i < PD; i += (int)target.BoundingRadius)
                {
                    var VL = EPred.UnitPosition.Extend(Me.Position, -i);
                    var J4 = ObjectManager.Get<Obj_AI_Base>().Any(f => f.Distance(PP) <= target.BoundingRadius && f.Name.ToLower() == "beacon");
                    var CF = NavMesh.GetCollisionFlags(VL);

                    if (CF.HasFlag(CollisionFlags.Wall) || CF.HasFlag(CollisionFlags.Building) || J4)
                    {
                        E.CastOnUnit(target);
                        return;
                    }
                }
            }
        }

        private static int GetPassive(Obj_AI_Base target)
        {
            int counts = 0;

            if (target != null && target.IsValidTarget())
            {
                foreach (var buff in target.Buffs)
                {
                    if (buff.Name.ToLower() == "vaynesilvereddebuff")
                    {
                        counts = buff.Count;
                    }
                }
            }

            return counts;
        }
    }
}
