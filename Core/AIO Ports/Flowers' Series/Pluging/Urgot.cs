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
    

    internal class Urgot : Logic
    {
        public Urgot()
        {
            Q = new Spell(SpellSlot.Q, 1000f);
            QExtend = new Spell(SpellSlot.Q, 1200f);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 900f);
            R = new Spell(SpellSlot.R, 550f);

            Q.SetSkillshot(0.25f, 60f, 1600f, true, SkillshotType.SkillshotLine);
            QExtend.SetSkillshot(0.25f, 60f, 1600f, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 120f, 1500f, false, SkillshotType.SkillshotCircle);


            var comboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                comboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboWAlways", "Use W| Always Use?", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboWBuff", "Use W| If target have E buff", true).SetValue(true));
                comboMenu.AddItem(
                    new MenuItem("ComboWLowHp", "Use W| If Player HealthPercent <= x%", true).SetValue(new Slider(50)));
                comboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboFirstE", "First Cast E -> Q Combo", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboR", "Use R", true).SetValue(true));
            }

            var harassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                harassMenu.AddItem(new MenuItem("HarassQ", "Use Q", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassW", "Use W", true).SetValue(false));
                harassMenu.AddItem(new MenuItem("HarassE", "Use E", true).SetValue(true));
                harassMenu.AddItem(
                    new MenuItem("HarassMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var clearMenu = Menu.AddSubMenu(new Menu("Clear", "Clear"));
            {
                var laneClearMenu = clearMenu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
                {
                    laneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q", true).SetValue(true));
                    laneClearMenu.AddItem(new MenuItem("LaneClearE", "Use E", true).SetValue(true));
                    laneClearMenu.AddItem(
                        new MenuItem("LaneClearECount", "If E CanHit Counts >= x", true).SetValue(new Slider(3, 1, 5)));
                    laneClearMenu.AddItem(
                        new MenuItem("LaneClearMana", "If Player ManaPercent >= %", true).SetValue(new Slider(60)));
                }

                var jungleClearMenu = clearMenu.AddSubMenu(new Menu("JungleClear", "JungleClear"));
                {
                    jungleClearMenu.AddItem(new MenuItem("JungleClearQ", "Use Q", true).SetValue(true));
                    jungleClearMenu.AddItem(new MenuItem("JungleClearW", "Use W", true).SetValue(true));
                    jungleClearMenu.AddItem(new MenuItem("JungleClearE", "Use E", true).SetValue(true));
                    jungleClearMenu.AddItem(
                        new MenuItem("JungleClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(30)));
                }

                clearMenu.AddItem(new MenuItem("asdqweqwe", " ", true));
                ManaManager.AddSpellFarm(clearMenu);
            }

            var killStealMenu = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            {
                killStealMenu.AddItem(new MenuItem("KillStealQ", "Use Q", true).SetValue(true));
                killStealMenu.AddItem(new MenuItem("KillStealE", "Use E", true).SetValue(true));
            }

            var miscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                var rMenu = miscMenu.AddSubMenu(new Menu("R Settings", "R Settings"));
                {
                    rMenu.AddItem(
                        new MenuItem("RSwap", "If After Swap Enemies Count <= x", true).SetValue(new Slider(3, 1, 5)));
                    rMenu.AddItem(new MenuItem("RAlly", "If Target Under Ally Turret", true).SetValue(true));
                    rMenu.AddItem(new MenuItem("RSafe", "Dont Cast In Enemy Turret", true).SetValue(true));
                    rMenu.AddItem(new MenuItem("RKill", "If Target Can Kill", true).SetValue(true));
                    rMenu.AddItem(new MenuItem("DontrList", "Dont R List: ", true));
                    foreach (var target in HeroManager.Enemies)
                    {
                        rMenu.AddItem(
                            new MenuItem("Dontr" + target.ChampionName.ToLower(), target.ChampionName, true).SetValue(true));
                    }
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

                var itemsMenu = utilityMenu.AddSubMenu(new Menu("Items", "Items"));
                {
                    ItemsManager.AddToMenu(itemsMenu);
                }
            }

            var drawMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            {
                drawMenu.AddItem(new MenuItem("DrawQ", "Draw Q Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawE", "Draw E Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawR", "Draw R Range", true).SetValue(false));
                ManaManager.AddDrawFarm(drawMenu);
                DamageIndicator.AddToMenu(drawMenu);
            }

            Obj_AI_Base.OnSpellCast += OnSpellCast;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
        }

        private void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (!sender.IsMe || !Orbwalking.IsAutoAttack(Args.SData.Name))
            {
                return;
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                var target = (AIHeroClient)Args.Target;

                if (target != null && !target.IsDead && !target.IsZombie)
                {
                    if (Menu.GetBool("ComboE") && E.IsReady() && (Q.IsReady() || target.Health < E.GetDamage(target)))
                    {
                        SpellManager.PredCast(E, target, true);
                    }

                    if (Menu.GetBool("ComboW") && W.IsReady())
                    {
                        if (target.DistanceToPlayer() <= Orbwalking.GetRealAutoAttackRange(Me))
                        {
                            if (Menu.GetBool("ComboWAlways"))
                            {
                                W.Cast();
                            }

                            if (Me.HealthPercent <= Menu.GetSlider("ComboWLowHp"))
                            {
                                W.Cast();
                            }
                        }

                        if (Menu.GetBool("ComboWBuff") && HaveEBuff(target) && Q.IsReady())
                        {
                            W.Cast();
                        }
                    }
                    else if (Menu.GetBool("ComboQ") && Q.IsReady() && target.IsValidTarget(QExtend.Range))
                    {
                        if (!HaveEBuff(target) && target.IsValidTarget(Q.Range))
                        {
                            if (Menu.GetBool("ComboFirstE") && E.IsReady() && Menu.GetBool("ComboE") && target.IsValidTarget(E.Range))
                            {
                                SpellManager.PredCast(E, target, true);
                            }
                            else
                            {
                                SpellManager.PredCast(Q, target);
                            }
                        }
                        else if (target.IsValidTarget(QExtend.Range) && HaveEBuff(target))
                        {
                            QExtend.Cast(target);
                        }
                    }

                }
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                if (ManaManager.HasEnoughMana(Menu.GetSlider("JungleClearMana")) && ManaManager.SpellFarm)
                {
                    var mobs = MinionManager.GetMinions(Me.Position, R.Range, MinionTypes.All, MinionTeam.Neutral,
                        MinionOrderTypes.MaxHealth);

                    if (mobs.Any())
                    {
                        var mob = mobs.FirstOrDefault();
                        var bigmob = mobs.FirstOrDefault(x => !x.Name.ToLower().Contains("mini"));

                        if (Menu.Item("JungleClearW", true).GetValue<bool>() && W.IsReady() && bigmob != null &&
                            bigmob.IsValidTarget(W.Range))
                        {
                            W.Cast();
                        }
                        else if (Menu.Item("JungleClearQ", true).GetValue<bool>() && Q.IsReady() && mob != null &&
                            mob.IsValidTarget(Q.Range))
                        {
                            Q.Cast(mob);
                        }
                    }
                }
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (Me.IsDead)
            {
                return;
            }

            if (R.Level > 0)
            {
                R.Range = new[] { 550f, 700f, 850f }[R.Level - 1];
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
                    LaneClear();
                    JungleClear();
                    break;
            }
        }

        private void KillSteal()
        {
            if (Menu.GetBool("KillStealQ") && Q.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range) && x.Health < Q.GetDamage(x)))
                {
                    SpellManager.PredCast(Q, target);
                    return;
                }
            }

            if (Menu.GetBool("KillStealE") && E.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(E.Range) && x.Health < E.GetDamage(x)))
                {
                    SpellManager.PredCast(E, target, true);
                    return;
                }
            }
        }

        private void Combo()
        {
            var target = TargetSelector.GetSelectedTarget() ??
                         TargetSelector.GetTarget(QExtend.Range, TargetSelector.DamageType.Physical);

            if (target.Check(QExtend.Range))
            {
                if (Menu.GetBool("ComboR") && R.IsReady() && target.IsValidTarget(R.Range))
                {
                    if (Menu.GetBool("RSafe") && !Me.UnderTurret(true))
                    {
                        foreach (
                            var rTarget in
                            HeroManager.Enemies.Where(
                                    x =>
                                        x.IsValidTarget(R.Range) &&
                                        !Menu.GetBool("Dontr" + target.ChampionName.ToLower()))
                                .OrderByDescending(x => E.IsReady() ? E.GetDamage(x) : 0 + Q.GetDamage(x)*2))
                        {
                            if (rTarget.CountEnemiesInRange(R.Range) <= Menu.GetSlider("RSwap"))
                            {
                                if (Menu.GetBool("RAlly") && Me.UnderAllyTurret() && rTarget.DistanceToPlayer() <= 350)
                                {
                                    R.CastOnUnit(rTarget);
                                }

                                if (Menu.GetBool("RKill") && target.Health < DamageCalculate.GetComboDamage(target))
                                {
                                    R.CastOnUnit(rTarget);
                                }
                            }
                        }
                    }
                }

                if (Menu.GetBool("ComboE") && E.IsReady() && target.IsValidTarget(E.Range) &&
                    (Q.IsReady() || target.Health < E.GetDamage(target)))
                {
                    SpellManager.PredCast(E, target, true);
                }

                if (Menu.GetBool("ComboW") && W.IsReady())
                {
                    if (target.DistanceToPlayer() <= Orbwalking.GetRealAutoAttackRange(Me))
                    {
                        if (Menu.GetBool("ComboWAlways"))
                        {
                            W.Cast();
                        }

                        if (Me.HealthPercent <= Menu.GetSlider("ComboWLowHp"))
                        {
                            W.Cast();
                        }
                    }

                    if (Menu.GetBool("ComboWBuff") && HaveEBuff(target) && Q.IsReady())
                    {
                        W.Cast();
                    }
                }

                if (Menu.GetBool("ComboQ") && Q.IsReady() && target.IsValidTarget(QExtend.Range))
                {
                    if (!HaveEBuff(target) && target.IsValidTarget(Q.Range))
                    {
                        if (Menu.GetBool("ComboFirstE") && E.IsReady() && Menu.GetBool("ComboE") && target.IsValidTarget(E.Range))
                        {
                            SpellManager.PredCast(E, target, true);
                        }
                        else
                        {
                            SpellManager.PredCast(Q, target);
                        }
                    }
                    else if (target.IsValidTarget(QExtend.Range) && HaveEBuff(target))
                    {
                        QExtend.Cast(target);
                    }
                }
            }
        }

        private void Harass()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("HarassMana")))
            {
                var target = TargetSelector.GetTarget(QExtend.Range, TargetSelector.DamageType.Physical);

                if (target.Check(QExtend.Range))
                {
                    if (Menu.GetBool("HarassE") && E.IsReady() && target.IsValidTarget(E.Range))
                    {
                        SpellManager.PredCast(E, target, true);
                    }

                    if (Menu.GetBool("HarassW") && W.IsReady())
                    {
                        if (HaveEBuff(target) && Q.IsReady())
                        {
                            W.Cast();
                        }
                    }

                    if (Menu.GetBool("HarassQ") && Q.IsReady() && target.IsValidTarget(QExtend.Range))
                    {
                        if (!HaveEBuff(target) && target.IsValidTarget(Q.Range))
                        {
                            SpellManager.PredCast(Q, target);
                        }
                        else if (target.IsValidTarget(QExtend.Range) && HaveEBuff(target))
                        {
                            QExtend.Cast(target);
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

        private void LaneClear()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("LaneClearMana")) && ManaManager.SpellFarm)
            {
                var minions = MinionManager.GetMinions(Me.Position, R.Range);

                if (minions.Any())
                {
                    if (Menu.GetBool("LaneClearE") && E.IsReady())
                    {
                        var eMinions = MinionManager.GetMinions(Me.Position, E.Range);
                        var eFarm =
                            MinionManager.GetBestLineFarmLocation(eMinions.Select(x => x.Position.To2D()).ToList(),
                                E.Width, E.Range);

                        if (eFarm.MinionsHit >= Menu.GetSlider("LaneClearECount"))
                        {
                            E.Cast(eFarm.Position);
                        }
                    }

                    if (Menu.GetBool("LaneClearQ") && Q.IsReady())
                    {
                        var qMinion =
                            MinionManager
                                .GetMinions(
                                    Me.Position, Q.Range)
                                .FirstOrDefault(
                                    x =>
                                        x.Health < Q.GetDamage(x) &&
                                        HealthPrediction.GetHealthPrediction(x, 250) > 0 &&
                                        x.Health > Me.GetAutoAttackDamage(x));

                        if (qMinion != null)
                        {
                            Q.Cast(qMinion);
                        }
                    }
                }
            }
        }

        private void JungleClear()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("JungleClearMana")) && ManaManager.SpellFarm)
            {
                var mobs = MinionManager.GetMinions(Me.Position, R.Range, MinionTypes.All, MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth);

                if (mobs.Any())
                {
                    var mob = mobs.FirstOrDefault();
                    var bigmob = mobs.FirstOrDefault(x => !x.Name.ToLower().Contains("mini"));

                    if (Menu.GetBool("JungleClearE") && E.IsReady())
                    {
                        if (bigmob != null && bigmob.IsValidTarget(E.Range))
                        {
                            E.Cast(bigmob);
                        }
                        else
                        {
                            var eMobs = MinionManager.GetMinions(Me.Position, E.Range, MinionTypes.All, MinionTeam.Neutral,
                                MinionOrderTypes.MaxHealth);

                            var eFarm =
                                MinionManager.GetBestLineFarmLocation(eMobs.Select(x => x.Position.To2D()).ToList(),
                                    E.Width, E.Range);

                            if (eFarm.MinionsHit >= 1)
                            {
                                E.Cast(eFarm.Position);
                            }
                        }
                    }

                    if (Menu.GetBool("JungleClearQ") && Q.IsReady() && mob != null && mob.IsValidTarget(Q.Range))
                    {
                        Q.Cast(mob);
                    }
                }
            }
        }

        private void OnDraw(EventArgs Args)
        {
            if (!Me.IsDead && !Shop.IsOpen && !MenuGUI.IsChatOpen  )
            {
                if (Menu.GetBool("DrawQ") && Q.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, Q.Range, Color.Green, 1);
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

        private static bool HaveEBuff(AIHeroClient target)
        {
            return target.HasBuff("urgotcorrosivedebuff");
        }
    }
}
