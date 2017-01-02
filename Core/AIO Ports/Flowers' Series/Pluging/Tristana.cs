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
    

    internal class Tristana : Logic
    {
        public Tristana()
        {
            Q = new Spell(SpellSlot.Q, 0f);
            W = new Spell(SpellSlot.W, 900f);
            E = new Spell(SpellSlot.E, 700f);
            R = new Spell(SpellSlot.R, 700f);

            W.SetSkillshot(0.50f, 250f, 1400f, false, SkillshotType.SkillshotCircle);

            var comboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                comboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboQOnlyPassive", "Use Q | Only target Have E Buff", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboEOnlyAfterAA", "Use E| Only After Attack", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboR", "Use R| Save MySelf", true).SetValue(true));
                comboMenu.AddItem(
                    new MenuItem("ComboRHp", "Use R| When Player HealthPercent <= x%", true).SetValue(new Slider(20)));
            }

            var harassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                harassMenu.AddItem(new MenuItem("HarassE", "Use E", true).SetValue(false));
                harassMenu.AddItem(new MenuItem("HarassEToMinion", "Use E| To Minion", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var clearMenu = Menu.AddSubMenu(new Menu("Clear", "Clear"));
            {
                var laneClearMenu = clearMenu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
                {
                    laneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q", true).SetValue(true));
                    laneClearMenu.AddItem(new MenuItem("LaneClearE", "Use E", true).SetValue(true));
                    laneClearMenu.AddItem(
                        new MenuItem("LaneClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(30)));
                }

                var jungleClearMenu = clearMenu.AddSubMenu(new Menu("JungleClear", "JungleClear"));
                {
                    jungleClearMenu.AddItem(new MenuItem("JungleClearQ", "Use Q", true).SetValue(true));
                    jungleClearMenu.AddItem(new MenuItem("JungleClearE", "Use E", true).SetValue(true));
                    jungleClearMenu.AddItem(
                        new MenuItem("JungleClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(30)));
                }

                clearMenu.AddItem(new MenuItem("asdqweqwe", " ", true));
                ManaManager.AddSpellFarm(clearMenu);
            }

            var killStealMenu = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            {
                killStealMenu.AddItem(new MenuItem("KillStealE", "Use E", true).SetValue(true));
                killStealMenu.AddItem(new MenuItem("KillStealR", "Use R", true).SetValue(true));
                foreach (var target in HeroManager.Enemies)
                {
                    killStealMenu.AddItem(new MenuItem("KillStealR" + target.ChampionName.ToLower(),
                        "Kill: " + target.ChampionName, true).SetValue(true));
                }
            }

            var fleeMenu = Menu.AddSubMenu(new Menu("Flee", "Flee"));
            {
                fleeMenu.AddItem(new MenuItem("FleeW", "Use W", true).SetValue(true));
                fleeMenu.AddItem(new MenuItem("FleeKey", "Flee Key", true).SetValue(new KeyBind('Z', KeyBindType.Press)));
            }

            var miscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                var eMenu = miscMenu.AddSubMenu(new Menu("E Settings", "E Settings"));
                {
                    eMenu.AddItem(
                        new MenuItem("SemiE", "Semi-manual E Key", true).SetValue(new KeyBind('E', KeyBindType.Press)));
                    foreach (var target in HeroManager.Enemies)
                    {
                        eMenu.AddItem(
                            new MenuItem("Semi" + target.ChampionName.ToLower(), "E target: " + target.ChampionName, true)
                                .SetValue(true));
                    }
                }

                var rMenu = miscMenu.AddSubMenu(new Menu("R Settings", "R Settings"));
                {
                    rMenu.AddItem(new MenuItem("InterruptR", "Use R Interrupt Spell", true).SetValue(true));
                    rMenu.AddItem(new MenuItem("AntiR", "Use R Anti Gapcloser", true).SetValue(false));
                    rMenu.AddItem(new MenuItem("AntiRengar", "Use R Anti Rengar", true).SetValue(true));
                    rMenu.AddItem(new MenuItem("AntiKhazix", "Use R Anti Khazix", true).SetValue(true));
                }

                miscMenu.AddItem(new MenuItem("Forcustarget", "Forcus Attack Passive Target", true).SetValue(true));
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
                drawMenu.AddItem(new MenuItem("DrawR", "Draw R Range", true).SetValue(false));
                ManaManager.AddDrawFarm(drawMenu);
                //DamageIndicator.AddToMenu(drawMenu);
            }

            Game.OnUpdate += OnUpdate;
            GameObject.OnCreate += OnCreate;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += OnInterruptableTarget;
            Orbwalking.BeforeAttack += BeforeAttack;
            Orbwalking.AfterAttack += AfterAttack;
            Drawing.OnDraw += OnDraw;
        }

        private void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead || Me.IsRecalling())
            {
                return;
            }

            if (E.Level > 0)
            {
                E.Range = 630 + 7 * (Me.Level - 1);
            }

            if (R.Level > 0)
            {
                R.Range = 630 + 7 * (Me.Level - 1);
            }

            if (Menu.GetKey("SemiE") && E.IsReady())
            {
                OneKeyCastE();
            }

            if (Menu.GetKey("FleeKey"))
            {
                Flee();
            }

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

        private void KillSteal()
        {
            if (Menu.GetBool("KillStealE") && E.IsReady())
            {

                foreach (
                    var target in
                    HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range) && x.Health <
                                                   Me.GetSpellDamage(x, SpellSlot.E)*
                                                   (x.GetBuffCount("TristanaECharge")*0.30) +
                                                   Me.GetSpellDamage(x, SpellSlot.E)))
                {
                    if (target.Check(E.Range))
                    {
                        E.CastOnUnit(target, true);
                    }
                }
            }

            if (Menu.GetBool("KillStealR") && R.IsReady())
            {
                if (Menu.GetBool("KillStealE") && E.IsReady())
                {
                    foreach (
                        var target in
                        from x in HeroManager.Enemies.Where(x => x.IsValidTarget(E.Range))
                        let etargetstacks = x.Buffs.Find(buff => buff.Name == "TristanaECharge")
                        where
                        R.GetDamage(x) + E.GetDamage(x) + etargetstacks?.Count*0.30*E.GetDamage(x) >=
                        x.Health
                        select x)
                    {
                        if (target.Check(R.Range))
                        {
                            R.CastOnUnit(target);
                            return;
                        }
                    }
                }
                else
                {
                    var target = HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range))
                        .OrderByDescending(x => x.Health).FirstOrDefault(x => x.Health < R.GetDamage(x));

                    if (target.Check(R.Range))
                    {
                        R.CastOnUnit(target);
                    }
                }
            }
        }

        private void Combo()
        {
            var target = TargetSelector.GetSelectedTarget() ??
                TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

            if (target.Check(E.Range))
            {
                if (Menu.GetBool("ComboQ") && Q.IsReady())
                {
                    if (Menu.GetBool("ComboQOnlyPassive"))
                    {
                        if (!E.IsReady() && target.HasBuff("TristanaECharge"))
                        {
                            Q.Cast();
                        }
                        else if (!E.IsReady() && !target.HasBuff("TristanaECharge") && E.Cooldown > 4)
                        {
                            Q.Cast();
                        }
                    }
                    else
                    {
                        Q.Cast();
                    }
                }

                if (Menu.GetBool("ComboE") && E.IsReady() && !Menu.GetBool("ComboEOnlyAfterAA") && target.IsValidTarget(E.Range))
                {
                    E.CastOnUnit(target, true);
                }

                if (Menu.GetBool("ComboR") && R.IsReady() && Me.HealthPercent <= Menu.GetSlider("ComboRHp"))
                {
                    var dangerenemy = HeroManager.Enemies.Where(e => e.IsValidTarget(R.Range)).
                        OrderBy(enemy => enemy.Distance(Me)).FirstOrDefault();

                    if (dangerenemy != null)
                    {
                        R.CastOnUnit(dangerenemy, true);
                    }
                }
            }
        }

        private void Harass()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("HarassMana")) )
            {
                if (E.IsReady())
                {
                    if (Menu.GetBool("HarassE"))
                    {
                        foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(E.Range)))
                        {
                            E.CastOnUnit(target, true);
                        }
                    }

                    if (Menu.GetBool("HarassEToMinion"))
                    {
                        foreach (var minion in MinionManager.GetMinions(E.Range).Where(m =>
                        m.Health < Me.GetAutoAttackDamage(m) && m.CountEnemiesInRange(m.BoundingRadius + 150) >= 1))
                        {
                            var etarget = E.GetTarget();

                            if (etarget != null)
                            {
                                return;
                            }

                            E.CastOnUnit(minion, true);
                            Orbwalker.ForceTarget(minion);
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
                var mobs =
                    MinionManager.GetMinions(Me.Position, E.Range, MinionTypes.All, MinionTeam.Neutral,
                        MinionOrderTypes.MaxHealth).Where(x => !x.Name.ToLower().Contains("mini"));

                if (mobs.Any())
                {
                    var mob = mobs.FirstOrDefault();

                    if (Menu.GetBool("JungleClearE") && E.IsReady())
                    {
                        E.CastOnUnit(mob, true);
                    }

                    if (Menu.GetBool("JungleClearQ") && Q.IsReady() && !E.IsReady())
                    {
                        Q.Cast();
                    }
                }
            }
        }

        private void Flee()
        {
            Orbwalking.MoveTo(Game.CursorPos);

            if (Menu.GetBool("FleeW") && W.IsReady())
            {
                W.Cast(Game.CursorPos); 
            }
        }

        private void OneKeyCastE()
        {
            foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(E.Range)))
            {
                if (target.Check(E.Range))
                {
                    if (target.Health <
                        Me.GetSpellDamage(target, SpellSlot.E)*(target.GetBuffCount("TristanaECharge")*0.30) +
                        Me.GetSpellDamage(target, SpellSlot.E))
                    {
                        E.CastOnUnit(target, true);
                    }

                    if (Me.CountEnemiesInRange(1200) == 1)
                    {
                        if (Me.HealthPercent >= target.HealthPercent && Me.Level + 1 >= target.Level)
                        {
                            E.CastOnUnit(target);
                        }
                        else if (Me.HealthPercent + 20 >= target.HealthPercent &&
                            Me.HealthPercent >= 40 && Me.Level + 2 >= target.Level)
                        {
                            E.CastOnUnit(target);
                        }
                    }

                    if (Menu.GetBool("Semi" + target.ChampionName.ToLower()))
                    {
                        E.CastOnUnit(target, true);
                    }
                }
            }
        }

        private void OnCreate(GameObject sender, EventArgs Args)
        {
            if (R.IsReady())
            {
                var Rengar = HeroManager.Enemies.Find(heros => heros.ChampionName.Equals("Rengar"));
                var Khazix = HeroManager.Enemies.Find(heros => heros.ChampionName.Equals("Khazix"));

                if (Menu.GetBool("AntiRengar") && Rengar != null)
                {
                    if (sender.Name == "Rengar_LeapSound.troy" && sender.Position.Distance(Me.Position) < R.Range)
                    {
                        R.CastOnUnit(Rengar, true);
                    }
                }

                if (Menu.GetBool("AntiKhazix") && Khazix != null)
                {
                    if (sender.Name == "Khazix_Base_E_Tar.troy" && sender.Position.Distance(Me.Position) <= 300)
                    {
                        R.CastOnUnit(Khazix, true);
                    }
                }
            }
        }

        private void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Menu.GetBool("AntiR") && R.IsReady())
            {
                if (gapcloser.End.Distance(Me.Position) <= 200 && gapcloser.Sender.IsValidTarget(R.Range))
                {
                    R.CastOnUnit(gapcloser.Sender);
                }
            }
        }

        private void OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs Args)
        {
            if (Menu.GetBool("InterruptR") && R.IsReady())
            {
                if (Args.DangerLevel >= Interrupter2.DangerLevel.High && sender.IsValidTarget(R.Range))
                {
                    R.CastOnUnit(sender);
                }
            }
        }

        private void BeforeAttack(Orbwalking.BeforeAttackEventArgs Args)
        {
            if (Menu.GetBool("Forcustarget"))
            {
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo ||
                    Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                {
                    foreach (
                        var enemy in
                        HeroManager.Enemies.Where(
                            enemy => Orbwalking.InAutoAttackRange(enemy) && enemy.HasBuff("TristanaEChargeSound")))
                    {
                        Orbwalker.ForceTarget(enemy);
                    }
                }
            }

            if (Args.Unit.IsMe && Orbwalking.InAutoAttackRange(Args.Target))
            {
                switch (Orbwalker.ActiveMode)
                {
                    case Orbwalking.OrbwalkingMode.Combo:
                        {
                            if (Menu.GetBool("ComboQ") && Q.IsReady())
                            {
                                if (Menu.GetBool("ComboQOnlyPassive"))
                                {
                                    var Target = Args.Target.Type == GameObjectType.AIHeroClient
                                        ? (AIHeroClient) Args.Target
                                        : null;

                                    if (Target != null &&
                                        (Target.HasBuff("TristanaEChargeSound") || Target.HasBuff("TristanaECharge")))
                                    {
                                        Q.Cast();
                                    }
                                }
                                else
                                {
                                    Q.Cast();
                                }
                            }
                            break;
                        }
                    case Orbwalking.OrbwalkingMode.LaneClear:
                        {
                            if (ManaManager.HasEnoughMana(Menu.GetSlider("JungleClearMana")) && ManaManager.SpellFarm)
                            {
                                if (Menu.GetBool("JungleClearQ") && Q.IsReady())
                                {
                                    var minion =
                                        MinionManager.GetMinions(Orbwalking.GetRealAutoAttackRange(ObjectManager.Player),
                                                MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

                                    if (minion.Any(x => x.NetworkId == Args.Target.NetworkId))
                                    {
                                        Q.Cast();
                                    }
                                }
                            }
                            break;
                        }
                }
            }
        }

        private void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (!unit.IsMe)
            {
                return;
            }

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    if (Menu.GetBool("ComboE") && E.IsReady() && Menu.GetBool("ComboEOnlyAfterAA"))
                    {
                        var t = target as AIHeroClient;

                        if (t != null && t.IsValidTarget(E.Range))
                        {
                            E.CastOnUnit(t, true);
                        }
                    }
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    if (ManaManager.HasEnoughMana(Menu.GetSlider("LaneClearMana")) && ManaManager.SpellFarm)
                    {
                        if (target != null &&
                            (target.Type == GameObjectType.obj_AI_Turret || target.Type == GameObjectType.obj_Turret))
                        {
                            if (Menu.GetBool("LaneClearE") && E.IsReady())
                            {
                                E.CastOnUnit(target as Obj_AI_Base, true);

                                if (!Me.Spellbook.IsAutoAttacking && Me.CountEnemiesInRange(1000) == 0 &&
                                    Menu.GetBool("LaneClearQ"))
                                {
                                    Q.Cast();
                                }
                            }
                        }
                    }
                    break;
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

                if (Menu.GetBool("DrawR") && R.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, R.Range, Color.FromArgb(19, 130, 234), 1);
                }
            }
        }
    }
}
