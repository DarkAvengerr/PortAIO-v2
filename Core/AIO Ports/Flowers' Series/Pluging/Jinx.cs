using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_ADC_Series.Pluging
{
    using ADCCOMMON;
    using System;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using Color = System.Drawing.Color;
    using Orbwalking = ADCCOMMON.Orbwalking;

    internal class Jinx : Logic
    {
        private float bigGunRange;
        private float rCoolDown;

        public Jinx()
        {
            Q = new Spell(SpellSlot.Q, 525f);
            W = new Spell(SpellSlot.W, 1500f);
            E = new Spell(SpellSlot.E, 920f);
            R = new Spell(SpellSlot.R, 3000f);

            W.SetSkillshot(0.6f, 60f, 3300f, true, SkillshotType.SkillshotLine);
            E.SetSkillshot(1.2f, 100f, 1750f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.7f, 140f, 1500f, false, SkillshotType.SkillshotLine);

            var comboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                comboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboR", "Use R", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboRTeam", "Use R|Team Fight", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboRSolo", "Use R|Solo Mode", true).SetValue(true));
            }

            var harassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                harassMenu.AddItem(new MenuItem("HarassQ", "Use Q", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassW", "Use W", true).SetValue(true));
                harassMenu.AddItem(
                    new MenuItem("HarassMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var clearMenu = Menu.AddSubMenu(new Menu("Clear", "Clear"));
            {
                var laneClearMenu = clearMenu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
                {
                    laneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q", true).SetValue(true));
                    laneClearMenu.AddItem(
                        new MenuItem("LaneClearQCount", "If Q CanHit Counts >= ", true).SetValue(new Slider(3, 1, 5)));
                    laneClearMenu.AddItem(
                        new MenuItem("LaneClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
                }

                var jungleClearMenu = clearMenu.AddSubMenu(new Menu("JungleClear", "JungleClear"));
                {
                    jungleClearMenu.AddItem(new MenuItem("JungleClearQ", "Use Q", true).SetValue(true));
                    jungleClearMenu.AddItem(new MenuItem("JungleClearW", "Use W", true).SetValue(true));
                    jungleClearMenu.AddItem(
                        new MenuItem("JungleClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(30)));
                }

                clearMenu.AddItem(new MenuItem("asdqweqwe", " ", true));
                ManaManager.AddSpellFarm(clearMenu);
            }

            var killStealMenu = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            {
                killStealMenu.AddItem(new MenuItem("KillStealW", "Use W", true).SetValue(true));
                killStealMenu.AddItem(new MenuItem("KillStealR", "Use R", true).SetValue(true));
                foreach (var target in HeroManager.Enemies)
                {
                    killStealMenu.AddItem(new MenuItem("KillStealR" + target.ChampionName.ToLower(),
                        "Kill: " + target.ChampionName, true).SetValue(true));
                }
            }

            var miscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                var wMenu = miscMenu.AddSubMenu(new Menu("W Settings", "W Settings"));
                {
                    wMenu.AddItem(new MenuItem("AutoW", "Auto W| When target Cant Move", true).SetValue(true));
                }

                var eMenu = miscMenu.AddSubMenu(new Menu("E Settings", "E Settings"));
                {
                    eMenu.AddItem(new MenuItem("AutoE", "Auto E| When target Cant Move", true).SetValue(true));
                    eMenu.AddItem(new MenuItem("AutoETP", "Auto E| Teleport", true).SetValue(true));
                    eMenu.AddItem(new MenuItem("GapE", "Anti GapCloser E", true).SetValue(true));
                }

                var rMenu = miscMenu.AddSubMenu(new Menu("R Settings", "R Settings"));
                {
                    rMenu.AddItem(
                        new MenuItem("rMenuSemi", "Semi R Key", true).SetValue(
                            new KeyBind('T', KeyBindType.Press)));
                    rMenu.AddItem(
                        new MenuItem("rMenuMin", "Use R| Min Range >= x", true).SetValue(new Slider(1000, 500, 2500)));
                    rMenu.AddItem(
                        new MenuItem("rMenuMax", "Use R| Man Range <= x", true).SetValue(new Slider(3000, 1500, 3500)));
                }
            }

            var utilityMenu = Menu.AddSubMenu(new Menu("Utility", "Utility"));
            {
                var skinMenu = utilityMenu.AddSubMenu(new Menu("Skin Change", "Skin Change"));
                {
                    SkinManager.AddToMenu(skinMenu);
                }

                var autoLevelMenu = utilityMenu.AddSubMenu(new Menu("Auto Levels", "Auto Levels"));
                {
                    LevelsManager.AddToMenu(autoLevelMenu);
                }

                var humainzerMenu = utilityMenu.AddSubMenu(new Menu("Humanier", "Humanizer"));
                {
                    HumanizerManager.AddToMenu(humainzerMenu);
                }

                var itemsMenu = utilityMenu.AddSubMenu(new Menu("Items", "Items"));
                {
                    ItemsManager.AddToMenu(itemsMenu);
                }
            }

            var drawMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            {
                drawMenu.AddItem(new MenuItem("DrawW", "Draw W Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawE", "Draw E Range", true).SetValue(false));
                ManaManager.AddDrawFarm(drawMenu);
                DamageIndicator.AddToMenu(drawMenu);
            }

            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Orbwalking.BeforeAttack += BeforeAttack;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
        }

        private void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var target = gapcloser.Sender;

            if (target.IsValidTarget(E.Range) && (gapcloser.End.DistanceToPlayer() <= 300 || target.DistanceToPlayer() <= 300))
            {
                if (Menu.GetBool("GapE") && E.IsReady())
                {
                    SpellManager.PredCast(E, target, true);
                }
            }
        }

        private void BeforeAttack(Orbwalking.BeforeAttackEventArgs Args)
        {
            if (!Q.IsReady())
            {
                return;
            }

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    if (Menu.GetBool("ComboQ"))
                    {
                        var target = Args.Target as AIHeroClient;

                        if (target != null && !target.IsDead && !target.IsZombie)
                        {
                            if (Me.HasBuff("JinxQ"))
                            {
                                if (target.Health < Me.GetAutoAttackDamage(target) * 3 &&
                                    target.DistanceToPlayer() <= Q.Range + 60)
                                {
                                    Q.Cast();
                                }
                                else if (Me.Mana < (rCoolDown == -1 ? 100 : (rCoolDown > 10 ? 130 : 150)))
                                {
                                    Q.Cast();
                                }
                                else if (target.IsValidTarget(Q.Range))
                                {
                                    Q.Cast();
                                }
                            }
                            else
                            {
                                if (target.CountEnemiesInRange(150) >= 2 &&
                                    Me.Mana > R.ManaCost + Q.ManaCost * 2 + W.ManaCost && target.DistanceToPlayer() > Q.Range)
                                {
                                    Q.Cast();
                                }
                            }
                        }
                    }
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    if (ManaManager.HasEnoughMana(Menu.GetSlider("HarassMana")))
                    {
                        if (Menu.GetBool("HarassQ"))
                        {
                            var target = Args.Target as AIHeroClient;

                            if (target != null && !target.IsDead && !target.IsZombie)
                            {
                                if (Me.HasBuff("JinxQ"))
                                {
                                    if (target.DistanceToPlayer() >= bigGunRange)
                                    {
                                        Q.Cast();
                                    }
                                }
                                else
                                {
                                    if (target.CountEnemiesInRange(150) >= 2 &&
                                        Me.Mana > R.ManaCost + Q.ManaCost*2 + W.ManaCost &&
                                        target.DistanceToPlayer() > Q.Range)
                                    {
                                        Q.Cast();
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (Me.HasBuff("JinxQ") && Q.IsReady())
                        {
                            Q.Cast();
                        }
                    }
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    if (ManaManager.HasEnoughMana(Menu.GetSlider("LaneClearMana")) && ManaManager.SpellFarm)
                    {
                        if (Menu.GetBool("LaneClearQ"))
                        {
                            var min = Args.Target as Obj_AI_Base;
                            var minions = MinionManager.GetMinions(Me.Position, bigGunRange);

                            if (minions.Any() && min != null)
                            {
                                foreach (var minion in minions.Where(x => x.NetworkId != min.NetworkId))
                                {
                                    var count = ObjectManager.Get<Obj_AI_Minion>().Count(x => x.Distance(minion) <= 150);

                                    if (minion.DistanceToPlayer() <= bigGunRange)
                                    {
                                        if (Me.HasBuff("JinxQ"))
                                        {
                                            if (Menu.GetSlider("LaneClearQCount") > count)
                                            {
                                                Q.Cast();
                                            }
                                            else if (min.Health > Me.GetAutoAttackDamage(min) * 1.1f)
                                            {
                                                Q.Cast();
                                            }
                                        }
                                        else if (!Me.HasBuff("JinxQ"))
                                        {
                                            if (Menu.GetSlider("LaneClearQCount") <= count)
                                            {
                                                Q.Cast();
                                            }
                                            else if (min.Health < Me.GetAutoAttackDamage(min) * 1.1f &&
                                                     min.DistanceToPlayer() > Q.Range)
                                            {
                                                Q.Cast();
                                            }
                                        }
                                    }
                                }

                                if (minions.Count <= 2 && Me.HasBuff("JinxQ"))
                                {
                                    Q.Cast();
                                }
                            }
                        }
                    }
                    else
                    {
                        if (Me.HasBuff("JinxQ") && Q.IsReady())
                        {
                            Q.Cast();
                        }
                    }
                    break;
            }
        }

        private void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead || Me.IsRecalling())
            {
                return;
            }

            if (Q.Level > 0)
            {
                bigGunRange = Q.Range + new[] {75, 100, 125, 150, 175}[Q.Level - 1];
            }

            if (R.Level > 0)
            {
                R.Range = Menu.GetSlider("rMenuMax");
            }

            rCoolDown = R.Level > 0
                ? (R.Instance.CooldownExpires - Game.Time < 0 ? 0 : R.Instance.CooldownExpires - Game.Time)
                : -1;

            AutoLogic();
            SemiRLogic();
            KillSteal();

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    FarmHarass();
                    JungleClear();
                    break;
            }
        }

        private void AutoLogic()
        {
            if (Menu.GetBool("AutoW") && W.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(W.Range) && !x.CanMoveMent()))
                {
                    W.Cast(target);
                }
            }

            if (Menu.GetBool("AutoE") && E.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(E.Range) && !x.CanMoveMent()))
                {
                    E.Cast(target);
                }
            }

            if (Menu.GetBool("AutoETP") && E.IsReady())
            {
                foreach (
                    var obj in
                    ObjectManager.Get<Obj_AI_Base>()
                        .Where(
                            x =>
                                x.IsEnemy && x.DistanceToPlayer() < E.Range &&
                                (x.HasBuff("teleport_target") || x.HasBuff("Pantheon_GrandSkyfall_Jump"))))
                {
                    E.Cast(obj.Position);
                }
            }
        }

        private void SemiRLogic()
        {
            if (Menu.GetKey("rMenuSemi") && R.IsReady())
            {
                var target = TargetSelector.GetSelectedTarget() ??
                             TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

                if (target.Check(R.Range))
                {
                    SpellManager.PredCast(R, target, true);
                }
            }
        }

        private void KillSteal()
        {
            if (Menu.GetBool("KillStealW") && W.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(W.Range) && x.Health < W.GetDamage(x)))
                {
                    if (Orbwalking.InAutoAttackRange(target) && target.Health <= Me.GetAutoAttackDamage(target, true))
                    {
                        continue;
                    }

                    SpellManager.PredCast(W, target);
                    return;
                }
            }

            if (Menu.GetBool("KillStealR") && R.IsReady())
            {
                foreach (
                    var target in
                    HeroManager.Enemies.Where(
                        x =>
                            x.IsValidTarget(R.Range) && x.DistanceToPlayer() > Menu.GetSlider("rMenuMin") &&
                            Menu.GetBool("KillStealR" + x.ChampionName.ToLower()) && x.Health < R.GetDamage(x)))
                {
                    SpellManager.PredCast(R, target, true);
                    return;
                }
            }
        }

        private void Combo()
        {
            if (Menu.GetBool("ComboW") && W.IsReady())
            {
                var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);

                if (target.Check(W.Range) && target.DistanceToPlayer() > Q.Range
                    && Me.CountEnemiesInRange(W.Range - 300) <= 3)
                {
                    SpellManager.PredCast(W, target);
                }
            }

            if (Menu.GetBool("ComboE") && E.IsReady())
            {
                var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

                if (target.Check(E.Range))
                {
                    if (!target.CanMoveMent())
                    {
                        E.Cast(target);
                    }
                    else
                    {
                        if (E.GetPrediction(target).Hitchance >= HitChance.VeryHigh)
                        {
                            SpellManager.PredCast(E, target);
                        }
                    }
                }
            }

            if (Menu.GetBool("ComboQ") && Q.IsReady())
            {
                var target = TargetSelector.GetTarget(bigGunRange, TargetSelector.DamageType.Physical);

                if (Me.HasBuff("JinxQ"))
                {
                    if (Me.Mana < (rCoolDown == -1 ? 100 : (rCoolDown > 10 ? 130 : 150)))
                    {
                        Q.Cast();
                    }

                    if (Me.CountEnemiesInRange(1500) == 0)
                    {
                        Q.Cast();
                    }

                    if (target == null)
                    {
                        Q.Cast();
                    }
                    else if (target.Check(bigGunRange))
                    {
                        if (target.Health < Me.GetAutoAttackDamage(target) * 3 &&
                            target.DistanceToPlayer() <= Q.Range + 60)
                        {
                            Q.Cast();
                        }
                    }
                }
                else
                {
                    if (target.Check(bigGunRange))
                    {
                        if (Me.CountEnemiesInRange(Q.Range) == 0 && Me.CountEnemiesInRange(bigGunRange) > 0 &&
                            Me.Mana > R.ManaCost + W.ManaCost + Q.ManaCost*2)
                        {
                            Q.Cast();
                        }

                        if (target.CountEnemiesInRange(150) >= 2 &&
                            Me.Mana > R.ManaCost + Q.ManaCost * 2 + W.ManaCost && target.DistanceToPlayer() > Q.Range)
                        {
                            Q.Cast();
                        }
                    }
                }
            }

            if (Menu.GetBool("ComboR") && R.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.Check(1200)))
                {
                    if (Menu.GetBool("ComboRTeam") && target.IsValidTarget(600) && Me.CountEnemiesInRange(600) >= 2 &&
                        target.CountAlliesInRange(200) <= 3 && target.HealthPercent < 50)
                    {
                        SpellManager.PredCast(R, target, true);
                    }

                    if (Menu.GetBool("ComboRSolo") && Me.CountEnemiesInRange(1500) <= 2 && target.DistanceToPlayer() > Q.Range &&
                        target.DistanceToPlayer() < bigGunRange && target.Health > Me.GetAutoAttackDamage(target) &&
                        target.Health < R.GetDamage(target) + Me.GetAutoAttackDamage(target)*3)
                    {
                        SpellManager.PredCast(R, target, true);
                    }
                }
            }
        }

        private void Harass()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("HarassMana")))
            {
                if (Menu.GetBool("HarassW") && W.IsReady())
                {
                    var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);

                    if (target.Check(W.Range) && target.DistanceToPlayer() > Q.Range)
                    {
                        SpellManager.PredCast(W, target);
                    }
                }

                if (Menu.GetBool("HarassQ") && Q.IsReady())
                {
                    var target = TargetSelector.GetTarget(bigGunRange, TargetSelector.DamageType.Physical);

                    if (target.IsValidTarget(bigGunRange) && Orbwalking.CanAttack())
                    {
                        if (target.CountEnemiesInRange(150) >= 2 &&
                            Me.Mana > R.ManaCost + Q.ManaCost * 2 + W.ManaCost && target.DistanceToPlayer() > Q.Range)
                        {
                            Q.Cast();
                        }

                        if (target.DistanceToPlayer() > Q.Range && Me.Mana > R.ManaCost + Q.ManaCost * 2 + W.ManaCost)
                        {
                            Q.Cast();
                        }
                    }
                    else
                    {
                        if (Me.HasBuff("JinxQ") && Q.IsReady())
                        {
                            Q.Cast();
                        }
                    }
                }
            }
        }

        private void FarmHarass()
        {
            if (ManaManager.SpellHarass)
            {
                Harass();
            }
        }

        private void JungleClear()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("JungleClearMana")) && ManaManager.SpellFarm)
            {
                var mobs = MinionManager.GetMinions(Me.Position, bigGunRange, MinionTypes.All, MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth);

                if (mobs.Any())
                {
                    if (Menu.GetBool("JungleClearW") && W.IsReady())
                    {
                        W.Cast(mobs.FirstOrDefault(x => !x.Name.ToLower().Contains("mini")));
                    }

                    if (Menu.GetBool("JungleClearQ") && Q.IsReady())
                    {
                        if (Me.HasBuff("JinxQ"))
                        {
                            foreach (var mob in mobs)
                            {
                                var count = ObjectManager.Get<Obj_AI_Minion>().Count(x => x.Distance(mob) <= 150);

                                if (mob.DistanceToPlayer() <= bigGunRange)
                                {
                                    if (count < 2)
                                    {
                                        Q.Cast();
                                    }
                                    else if (mob.Health > Me.GetAutoAttackDamage(mob) * 1.1f)
                                    {
                                        Q.Cast();
                                    }
                                }
                            }

                            if (mobs.Count < 2)
                            {
                                Q.Cast();
                            }
                        }
                        else
                        {
                            foreach (var mob in mobs)
                            {
                                var count = ObjectManager.Get<Obj_AI_Minion>().Count(x => x.Distance(mob) <= 150);

                                if (mob.DistanceToPlayer() <= bigGunRange)
                                {
                                    if (count >= 2)
                                    {
                                        Q.Cast();
                                    }
                                    else if (mob.Health < Me.GetAutoAttackDamage(mob) * 1.1f &&
                                             mob.DistanceToPlayer() > Q.Range)
                                    {
                                        Q.Cast();
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (Me.HasBuff("JinxQ") && Q.IsReady())
                    {
                        Q.Cast();
                    }
                }
            }
            else
            {
                if (Me.HasBuff("JinxQ") && Q.IsReady())
                {
                    Q.Cast();
                }
            }
        }

        private void OnDraw(EventArgs Args)
        {
            if (!Me.IsDead && !Shop.IsOpen && !MenuGUI.IsChatOpen  )
            {
                if (Menu.GetBool("DrawW") && W.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, W.Range, Color.FromArgb(9, 253, 242), 1);
                }

                if (Menu.GetBool("DrawE") && E.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, E.Range, Color.FromArgb(188, 6, 248), 1);
                }
            }
        }
    }
}
