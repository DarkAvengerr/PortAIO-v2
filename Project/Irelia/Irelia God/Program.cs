using System;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Channels;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace IreliaGod
{
    internal class Program
    {
        private static readonly AIHeroClient Player = ObjectManager.Player;
        public static Orbwalking.Orbwalker Orbwalker;
        private static int lastsheenproc;
        private static int rcount;

        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            // Only load on Irelia, silly
            if (Player.CharData.BaseSkinName != "Irelia") return;

            // Say hello
            Chat.Print("[Irelia<font color='#79BAEC'>God</font>]: <font color='#FFFFFF'>" + "Loaded!</font>");

            // Initialize our menu
            IreliaMenu.Initialize();

            // Initialize our spells
            Spells.Initialize();

            // Subscribe to our events
            Game.OnUpdate += OnUpdate;
            Orbwalking.BeforeAttack += BeforeAttack;
            Drawing.OnDraw += OnDraw;
            Obj_AI_Base.OnBuffLose += OnBuffLose; // Sheen buff workaround
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += OnInterruptableTarget;
            //Obj_AI_Base.OnAggro += AIHeroClient_OnAggro;
            Obj_AI_Base.OnProcessSpellCast += (sender, eventArgs) =>
            {
                if (sender.IsMe && eventArgs.SData.Name == Spells.E.Instance.SData.Name)
                    LeagueSharp.Common.Utility.DelayAction.Add(260, Orbwalking.ResetAutoAttackTimer);

                if (sender.IsMe && eventArgs.SData.Name == Spells.Q.Instance.SData.Name)
                    LeagueSharp.Common.Utility.DelayAction.Add(260, Orbwalking.ResetAutoAttackTimer);
            };
            Orbwalking.AfterAttack += (unit, target) =>
            {
                if (IreliaMenu.Config.Item("combo.items").GetValue<bool>() && unit.IsMe && target != null && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                {
                    if (Spells.Tiamat.IsReady())
                        Spells.Tiamat.Cast();

                    if (Spells.Hydra.IsReady())
                        Spells.Hydra.Cast();
                }
            };
        }

        static void AIHeroClient_OnAggro(Obj_AI_Base sender, Obj_AI_Base args)
        {
            if (!IreliaMenu.Config.Item("misc.stunundertower").GetValue<bool>()) return;
            if (!Spells.E.IsReady()) return;
            if (!sender.Name.Contains("Turret")) return;

            foreach (
                var enemy in
                    HeroManager.Enemies.Where(
                        enemy => enemy.NetworkId == args.NetworkId && Player.HealthPercent <= enemy.HealthPercent))
            {
                if (Player.Distance(enemy) <= Spells.E.Range)
                    Spells.E.CastOnUnit(enemy);

                else if (Player.Distance(enemy) <= Spells.Q.Range && Spells.Q.IsReady())
                {
                    var qminion =  MinionManager
                        .GetMinions(Spells.Q.Range + 350, MinionTypes.All, MinionTeam.NotAlly)
                        .Where(
                            m =>
                                m.Distance(Player) <= Spells.Q.Range &&
                                m.Health <= QDamage(m) + ExtraWDamage(m) + SheenDamage(m) - 30 && m.IsValidTarget())
                        .OrderBy(m => m.Distance(enemy.Position) <= Spells.Q.Range + 350)
                        .FirstOrDefault();

                    if (qminion != null && qminion.Distance(enemy) <= Spells.E.Range)
                    {
                        var qtraveltime = Player.Distance(qminion)/Spells.Q.Speed + Spells.Q.Delay;
                        var enemy1 = enemy;
                        Spells.Q.CastOnUnit(qminion);
                        LeagueSharp.Common.Utility.DelayAction.Add((int) qtraveltime, () => Spells.E.CastOnUnit(enemy1));
                    }
                }
            }
        }

        private static bool Selected()
        {
            if (!IreliaMenu.Config.Item("force.target").GetValue<bool>()) return false;

            var target = TargetSelector.GetSelectedTarget();
            float range = IreliaMenu.Config.Item("force.target.range").GetValue<Slider>().Value;
            if (target == null || target.IsDead || target.IsZombie) return false;

            return !(Player.Distance(target.Position) > range);
        }

        private static Obj_AI_Base GetTarget(float range)
        {
            return Selected() ? TargetSelector.GetSelectedTarget() : TargetSelector.GetTarget(range, TargetSelector.DamageType.Physical);
        }

        private static void OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender == null) return;
            if (IreliaMenu.Config.Item("misc.interrupt").GetValue<bool>() && sender.IsValidTarget(Spells.E.Range) &&
                Spells.E.IsReady() && Player.HealthPercent <= sender.HealthPercent)
                Spells.E.CastOnUnit(sender);
        }

        private static void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender == null) return;
            if (IreliaMenu.Config.Item("misc.age").GetValue<bool>() && Spells.E.IsReady() &&
                gapcloser.Sender.IsValidTarget())
            {
                Spells.E.Cast(gapcloser.Sender);
            }
        }

        public static float ComboDamage(AIHeroClient hero) // Thanks honda
        {
            var result = 0d;

            if (Spells.Q.IsReady())
            {
                result += QDamage(hero) + ExtraWDamage(hero) + SheenDamage(hero);
            }
            if (Spells.W.IsReady() || Player.HasBuff("ireliahitenstylecharged"))
            {
                result += (ExtraWDamage(hero) +
                           Player.CalcDamage(hero, Damage.DamageType.Physical, Player.TotalAttackDamage))*3; // 3 autos
            }
            if (Spells.E.IsReady())
            {
                result += Spells.E.GetDamage(hero);
            }
            if (Spells.R.IsReady())
            {
                result += Spells.R.GetDamage(hero)*rcount;
            }

            return (float) result;
        }

        private static void OnBuffLose(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            if (sender.IsMe && args.Buff.Name == "sheen")
                lastsheenproc = Utils.TickCount;
        }

        private static void RCount()
        {
            if (rcount == 0 && Spells.R.IsReady())
                rcount = 4;

            if (!Spells.R.IsReady() & rcount != 0)
                rcount = 0;

            foreach (
                var buff in
                    Player.Buffs.Where(b => b.Name == "ireliatranscendentbladesspell" && b.IsValid))
            {
                rcount = buff.Count;
            }
        }

        private static bool UnderTheirTower(Obj_AI_Base target)
        {
            var tower =
                ObjectManager
                    .Get<Obj_AI_Turret>()
                    .FirstOrDefault(turret => turret != null && turret.Distance(target) <= 775 && turret.IsValid && turret.Health > 0 && !turret.IsAlly);

            return tower != null;
        }

        private static void OnUpdate(EventArgs args)
        {
            Killsteal();
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Laneclear();
                    Jungleclear();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    break;
                case Orbwalking.OrbwalkingMode.None:
                    if (IreliaMenu.Config.Item("flee").GetValue<KeyBind>().Active)
                    {
                        if (Spells.E.IsReady() && IreliaMenu.Config.Item("flee.e").GetValue<bool>())
                        {
                            var etarget =
                                HeroManager.Enemies
                                    .FindAll(
                                        enemy =>
                                            enemy.IsValidTarget() && Player.Distance(enemy.Position) <= Spells.E.Range)
                                    .OrderBy(e => e.Distance(Player));

                            if (etarget.FirstOrDefault() != null)
                                Spells.E.CastOnUnit(etarget.FirstOrDefault());
                        }

                        if (Spells.R.IsReady() && IreliaMenu.Config.Item("flee.r").GetValue<bool>())
                        {
                            var rtarget =
                                HeroManager.Enemies
                                    .FindAll(
                                        enemy =>
                                            enemy.IsValidTarget() && Player.Distance(enemy.Position) <= Spells.R.Range)
                                    .OrderBy(e => e.Distance(Player));
                            if (rtarget.FirstOrDefault() == null) goto WALK;
                            var rprediction = Prediction.GetPrediction(rtarget.FirstOrDefault(), Spells.R.Delay,
                                Spells.R.Width, Spells.R.Speed);

                            Spells.R.Cast(rprediction.CastPosition);
                        }

                        if (Spells.Q.IsReady() && IreliaMenu.Config.Item("flee.q").GetValue<bool>())
                        {
                            var target =
                                HeroManager.Enemies
                                    .FindAll(
                                        enemy =>
                                            enemy.IsValidTarget() && Player.Distance(enemy.Position) <= Spells.R.Range)
                                    .MinOrDefault(e => e.Distance(Player) <= Spells.R.Range);

                            if (target == null) goto WALK;

                            var qminion =
                                MinionManager
                                    .GetMinions(Spells.Q.Range, MinionTypes.All, MinionTeam.NotAlly)
                                    .Where(
                                        m => 
                                            m.IsValidTarget(Spells.Q.Range) &&
                                            m.Distance(target) > Player.Distance(target))
                                    .MaxOrDefault(m => m.Distance(target));

                            if (qminion != null)
                                Spells.Q.CastOnUnit(qminion);
                        }

                        WALK:
                        Orbwalking.MoveTo(Game.CursorPos);
                    }
                    break;
            }
            RCount();
        }

        private static void Combo()
        {
            var gctarget = GetTarget(Spells.Q.Range*2.5f);
            var target = GetTarget(Spells.Q.Range);
            if (gctarget == null) return;

            var qminion =
                MinionManager
                    .GetMinions(Spells.Q.Range + 350, MinionTypes.All, MinionTeam.NotAlly) //added 350 range, bad?
                    .Where(
                        m =>
                            m.Distance(Player) <= Spells.Q.Range &&
                            m.Health <= QDamage(m) + ExtraWDamage(m) + SheenDamage(m) - 30 && m.IsValidTarget())
                    .OrderBy(m => m.Distance(gctarget.Position) <= Spells.Q.Range + 350)
                    .FirstOrDefault();


            if (Spells.Q.IsReady())
            {
                if (IreliaMenu.Config.Item("combo.q.gc").GetValue<bool>() &&
                    gctarget.Distance(Player.Position) >= Orbwalking.GetRealAutoAttackRange(gctarget) && qminion != null &&
                    qminion.Distance(gctarget.Position) <= Player.Distance(gctarget.Position) &&
                    qminion.Distance(Player.Position) <= Spells.Q.Range)
                {
                    Spells.Q.CastOnUnit(qminion);
                }

                if (IreliaMenu.Config.Item("combo.q").GetValue<bool>() && target != null &&
                    target.Distance(Player.Position) <= Spells.Q.Range &&
                    target.Distance(Player.Position) >=
                    IreliaMenu.Config.Item("combo.q.minrange").GetValue<Slider>().Value)
                {
                    if (UnderTheirTower(target))
                        if (target.HealthPercent >=
                            IreliaMenu.Config.Item("combo.q.undertower").GetValue<Slider>().Value) return;

                    if (IreliaMenu.Config.Item("combo.w").GetValue<bool>())
                        Spells.W.Cast();

                    Spells.Q.CastOnUnit(target);
                }

                if (IreliaMenu.Config.Item("combo.q").GetValue<bool>() &&
                    IreliaMenu.Config.Item("combo.q.lastsecond").GetValue<bool>() && target != null)
                {
                    var buff = Player.Buffs.FirstOrDefault(b => b.Name == "ireliahitenstylecharged" && b.IsValid);
                    if (buff != null && buff.EndTime - Game.Time <= (Player.Distance(target) / Spells.Q.Speed + Spells.Q.Delay + .500 + Player.AttackCastDelay) && !Player.Spellbook.IsAutoAttacking)
                    {
                        if (UnderTheirTower(target))
                            if (target.HealthPercent >=
                                IreliaMenu.Config.Item("combo.q.undertower").GetValue<Slider>().Value) return;

                        Spells.Q.Cast(target);
                    }
                }
            }

            if (Spells.E.IsReady() && IreliaMenu.Config.Item("combo.e").GetValue<bool>() && target != null)
            {
                if (IreliaMenu.Config.Item("combo.e.logic").GetValue<bool>() &&
                    target.Distance(Player.Position) <= Spells.E.Range)
                {
                    if (target.HealthPercent >= Player.HealthPercent)
                    {
                        Spells.E.CastOnUnit(target);
                    }
                    else if (!target.IsFacing(Player) && target.Distance(Player.Position) >= Spells.E.Range/2)
                    {
                        Spells.E.CastOnUnit(target);
                    }
                }
                else if (target.Distance(Player.Position) <= Spells.E.Range)
                {
                    Spells.E.CastOnUnit(target);
                }
            }

            if (Spells.R.IsReady() && IreliaMenu.Config.Item("combo.r").GetValue<bool>() && !IreliaMenu.Config.Item("combo.r.selfactivated").GetValue<bool>())
            {
                if (IreliaMenu.Config.Item("combo.r.weave").GetValue<bool>())
                {
                    if (target != null && !Player.HasBuff("sheen") &&
                        target.Distance(Player.Position) <= Spells.R.Range &&
                        Utils.TickCount - lastsheenproc >= 1500)
                    {
                        Spells.R.Cast(target, false, true);
                    }
                }
                else
                {
                    Spells.R.Cast(target, false, true);
                        // Set to Q range because we are already going to combo them at this point most likely, no stupid long range R initiations
                }
            }
            else if (Spells.R.IsReady() && IreliaMenu.Config.Item("combo.r").GetValue<bool>() && IreliaMenu.Config.Item("combo.r.selfactivated").GetValue<bool>() && rcount <= 3)
            {
                if (IreliaMenu.Config.Item("combo.r.weave").GetValue<bool>())
                {
                    if (target != null && !Player.HasBuff("sheen") &&
                        target.Distance(Player.Position) <= Spells.R.Range &&
                        Utils.TickCount - lastsheenproc >= 1500)
                    {
                        Spells.R.Cast(target, false, true);
                    }
                }
                else
                {
                    Spells.R.Cast(target, false, true);
                    // Set to Q range because we are already going to combo them at this point most likely, no stupid long range R initiations
                }
            }

            if (IreliaMenu.Config.Item("combo.items").GetValue<bool>() && target != null)
            {
                if (Player.Distance(target.ServerPosition) <= 600 && ComboDamage((AIHeroClient) target) >= target.Health &&
                    IreliaMenu.Config.Item("combo.ignite").GetValue<bool>())
                {
                    Player.Spellbook.CastSpell(Spells.Ignite, target);
                }

                if (Spells.Youmuu.IsReady() && target.IsValidTarget(Spells.Q.Range))
                {
                    Spells.Youmuu.Cast();
                }

                if (Player.Distance(target.ServerPosition) <= 450 && Spells.Cutlass.IsReady())
                {
                    Spells.Cutlass.Cast(target);
                }

                if (Player.Distance(target.ServerPosition) <= 450 && Spells.Blade.IsReady())
                {
                    Spells.Blade.Cast(target);
                }
            }
        }

        private static void Harass()
        {
            var gctarget = TargetSelector.GetTarget(Spells.Q.Range * 2.5f, TargetSelector.DamageType.Physical);
            var target = TargetSelector.GetTarget(Spells.Q.Range, TargetSelector.DamageType.Physical);
            if (gctarget == null) return;
            if (Player.ManaPercent <= IreliaMenu.Config.Item("harass.mana").GetValue<Slider>().Value && Player.HasBuff("ireliatranscendentbladesspell") && rcount >= 1) goto castr;
            if (Player.ManaPercent <= IreliaMenu.Config.Item("harass.mana").GetValue<Slider>().Value) return;

            var qminion =
                MinionManager
                    .GetMinions(Spells.Q.Range, MinionTypes.All, MinionTeam.NotAlly)
                    .Where(
                        m =>
                            m.Distance(Player) <= Spells.Q.Range && m.Health <= Spells.Q.GetDamage(m) &&
                            m.IsValidTarget())
                    .MinOrDefault(m => m.Distance(target) <= Spells.Q.Range);

            if (Spells.Q.IsReady())
            {
                if (IreliaMenu.Config.Item("harass.q.gc").GetValue<bool>() && qminion != null &&
                    qminion.Distance(target) <= Player.Distance(target))
                {
                    Spells.Q.CastOnUnit(qminion);
                }

                if (IreliaMenu.Config.Item("harass.q").GetValue<bool>() && target != null &&
                    target.Distance(Player.Position) <= Spells.Q.Range &&
                    target.Distance(Player.Position) >=
                    IreliaMenu.Config.Item("harass.q.minrange").GetValue<Slider>().Value)
                {
                    if (UnderTheirTower(target))
                        if (target.HealthPercent >=
                            IreliaMenu.Config.Item("harass.q.undertower").GetValue<Slider>().Value) return;

                    Spells.Q.CastOnUnit(target);
                }

                if (IreliaMenu.Config.Item("harass.q").GetValue<bool>() &&
                    IreliaMenu.Config.Item("harass.q.lastsecond").GetValue<bool>() && target != null)
                {
                    var buff = Player.Buffs.FirstOrDefault(b => b.Name == "ireliahitenstylecharged" && b.IsValid);
                    if (buff != null && buff.EndTime - Game.Time <= (Player.Distance(target) / Spells.Q.Speed + Spells.Q.Delay + .500 + Player.AttackCastDelay) && !Player.Spellbook.IsAutoAttacking)
                    {
                        if (UnderTheirTower(target))
                            if (target.HealthPercent >=
                                IreliaMenu.Config.Item("harass.q.undertower").GetValue<Slider>().Value) return;

                        Spells.Q.Cast(target);
                    }
                }
            }

            if (Spells.E.IsReady() && IreliaMenu.Config.Item("harass.e").GetValue<bool>() && target != null)
            {
                if (IreliaMenu.Config.Item("harass.e.logic").GetValue<bool>() &&
                    target.Distance(Player.Position) <= Spells.E.Range)
                {
                    if (target.HealthPercent >= Player.HealthPercent)
                    {
                        Spells.E.CastOnUnit(target);
                    }
                    else if (!target.IsFacing(Player) && target.Distance(Player.Position) >= Spells.E.Range/2)
                    {
                        Spells.E.CastOnUnit(target);
                    }
                }
                else if (target.Distance(Player.Position) <= Spells.E.Range)
                {
                    Spells.E.CastOnUnit(target);
                }
            }

            castr:
            if (Spells.R.IsReady() && IreliaMenu.Config.Item("harass.r").GetValue<bool>())
            {
                if (IreliaMenu.Config.Item("harass.r.weave").GetValue<bool>())
                {
                    if (IreliaMenu.Config.Item("harass.r.weave").GetValue<bool>())
                    {
                        if (target != null && !Player.HasBuff("sheen") &&
                            target.Distance(Player.Position) <= Spells.R.Range &&
                            Utils.TickCount - lastsheenproc >= 1500)
                        {
                            Spells.R.Cast(target, false, true);
                        }
                    }
                }
                else
                {
                    Spells.R.Cast(target, false, true);
                        // Set to Q range because we are already going to combo them at this point most likely, no stupid long range R initiations
                }
            }
        }

        private static void Killsteal()
        {
            foreach (
                var enemy in
                    HeroManager.Enemies.Where(e => e.Distance(Player.Position) <= Spells.R.Range && e.IsValidTarget()))
            {
                if (enemy == null) return;

                if (Spells.Q.IsReady() && IreliaMenu.Config.Item("misc.ks.q").GetValue<bool>() &&
                    Spells.E.IsReady() && IreliaMenu.Config.Item("misc.ks.e").GetValue<bool>() &&
                    Spells.E.GetDamage(enemy) + QDamage(enemy) + ExtraWDamage(enemy) + SheenDamage(enemy) >=
                            enemy.Health)
                {
                    if (enemy.Distance(Player.Position) <= Spells.Q.Range && enemy.Distance(Player.Position) > Spells.E.Range)
                    {
                        Spells.Q.Cast(enemy);
                        var enemy1 = enemy;
                        LeagueSharp.Common.Utility.DelayAction.Add((int)(1000 * Player.Distance(enemy) / Spells.Q.Speed + Spells.Q.Delay), () => Spells.E.Cast(enemy1));
                    }
                    else if (enemy.Distance(Player.Position) <= Spells.Q.Range)
                    {
                        Spells.E.Cast(enemy);
                        var enemy1 = enemy;
                        LeagueSharp.Common.Utility.DelayAction.Add(250, () => Spells.Q.Cast(enemy1));
                    }

                }

                if (IreliaMenu.Config.Item("misc.ks.q").GetValue<bool>() && Spells.Q.IsReady() &&
                    QDamage(enemy) + ExtraWDamage(enemy) + SheenDamage(enemy) >= enemy.Health &&
                    enemy.Distance(Player.Position) <= Spells.Q.Range)
                {
                    Spells.Q.CastOnUnit(enemy);
                    return;
                }

                if (IreliaMenu.Config.Item("misc.ks.e").GetValue<bool>() && Spells.E.IsReady() &&
                    Spells.E.GetDamage(enemy) >= enemy.Health && enemy.Distance(Player.Position) <= Spells.E.Range)
                {
                    Spells.E.CastOnUnit(enemy);
                    return;
                }

                if (IreliaMenu.Config.Item("misc.ks.r").GetValue<bool>() && Spells.R.IsReady() &&
                    Spells.R.GetDamage(enemy)*rcount >= enemy.Health)
                {
                    Spells.R.Cast(enemy, false, true);
                }

            }
        }

        private static void Laneclear()
        {
            if (Player.ManaPercent <= IreliaMenu.Config.Item("laneclear.mana").GetValue<Slider>().Value) return;

            var qminion =
                MinionManager
                    .GetMinions(
                        Spells.Q.Range, MinionTypes.All, MinionTeam.NotAlly)
                    .FirstOrDefault(
                        m =>
                            m.Distance(Player) <= Spells.Q.Range &&
                            m.Health <= QDamage(m) + ExtraWDamage(m) + SheenDamage(m) - 10 &&
                            m.IsValidTarget());


            if (Spells.Q.IsReady() && IreliaMenu.Config.Item("laneclear.q").GetValue<bool>() && qminion != null)
            {
                Spells.Q.CastOnUnit(qminion);
            }

            var rminions = MinionManager.GetMinions(Player.Position, Spells.R.Range);
            if (Spells.R.IsReady() && IreliaMenu.Config.Item("laneclear.r").GetValue<bool>() && rminions.Count != 0)
            {
                var location = Spells.R.GetLineFarmLocation(rminions);

                if (location.MinionsHit >=
                    IreliaMenu.Config.Item("laneclear.r.minimum").GetValue<Slider>().Value)
                    Spells.R.Cast(location.Position);
            }
        }

        private static void Jungleclear()
        {
        }

        private static void BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (IreliaMenu.Config.Item("combo.w").GetValue<bool>() &&
                Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo &&
                args.Target != null &&
                args.Target.Type == GameObjectType.AIHeroClient &&
                args.Target.IsValidTarget() ||
                IreliaMenu.Config.Item("harass.w").GetValue<bool>() &&
                Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed &&
                args.Target != null &&
                args.Target.Type == GameObjectType.AIHeroClient &&
                args.Target.IsValidTarget())
                Spells.W.Cast();
        }

        private static void OnDraw(EventArgs args)
        {
            if (Player.IsDead) return;
            if (IreliaMenu.Config.Item("drawings.q").GetValue<bool>())
            {
                if (Spells.Q.IsReady())
                    Drawing.DrawCircle(Player.Position, Spells.Q.Range, Color.LightCoral);
                else
                    Drawing.DrawCircle(Player.Position, Spells.Q.Range, Color.Maroon);
            }
            if (IreliaMenu.Config.Item("drawings.e").GetValue<bool>())
            {
                if (Spells.E.IsReady())
                    Drawing.DrawCircle(Player.Position, Spells.E.Range, Color.LightCoral);
                else
                    Drawing.DrawCircle(Player.Position, Spells.E.Range, Color.Maroon);
            }
            if (IreliaMenu.Config.Item("drawings.r").GetValue<bool>())
            {
                if (Spells.R.IsReady())
                    Drawing.DrawCircle(Player.Position, Spells.R.Range, Color.LightCoral);
                else
                    Drawing.DrawCircle(Player.Position, Spells.R.Range, Color.Maroon);
            }
        }

        private static double SheenDamage(Obj_AI_Base target) // Thanks princer007 for the basic idea
        {
            var result = 0d;
            foreach (var item in Player.InventoryItems)
                switch ((int) item.Id)
                {
                    case 3057: //Sheen
                        if (Utils.TickCount - lastsheenproc >= 1750 + Game.Ping)
                            result += Player.CalcDamage(target, Damage.DamageType.Physical, Player.BaseAttackDamage);
                        break;
                    case 3078: //Triforce
                        if (Utils.TickCount - lastsheenproc >= 1750 + Game.Ping)
                            result += Player.CalcDamage(target, Damage.DamageType.Physical, Player.BaseAttackDamage * 2);
                        break;
                }
            return result;
        }

        private static double ExtraWDamage(Obj_AI_Base target)
        {
            // tried some stuff with if buff == null but the damage will be enough then cast W and it worked.. but meh, idk will look at later

            var extra = 0d;
            var buff = Player.Buffs.FirstOrDefault(b => b.Name == "ireliahitenstylecharged" && b.IsValid);
            if (buff != null && buff.EndTime < (1000*Player.Distance(target)/Spells.Q.Speed + Spells.Q.Delay))
                extra += new double[] {15, 30, 45, 60, 75}[Spells.W.Level - 1];

            return extra;
        }

        private static double QDamage(Obj_AI_Base target)
        {
            return Spells.Q.IsReady()
                ? Player.CalcDamage(
                    target,
                    Damage.DamageType.Physical,
                    new double[] {20, 50, 80, 110, 140}[Spells.Q.Level - 1]
                    + Player.TotalAttackDamage)
                //- 25) Safety net, for some reason the damage is never exact ): why?
                : 0d;
        }
    }
}