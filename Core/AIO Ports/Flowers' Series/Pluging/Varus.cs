using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_ADC_Series.Pluging
{
    using ADCCOMMON;
    using System;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using Color = System.Drawing.Color;
    using Orbwalking = ADCCOMMON.Orbwalking;

    internal class Varus : Logic
    {
        private const float qRange = 1600f;

        public Varus()
        {
            Q = new Spell(SpellSlot.Q, 925f);
            W = new Spell(SpellSlot.W, 0);
            E = new Spell(SpellSlot.E, 975f);
            R = new Spell(SpellSlot.R, 1050f);

            Q.SetSkillshot(0.25f, 70f, 1650f, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.35f, 120f, 1500f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.25f, 120f, 1950f, false, SkillshotType.SkillshotLine);

            Q.SetCharged(925, 1600, 1.5f);

            var comboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                comboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboR", "Use R", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboRSolo", "Use R|Logic?", true).SetValue(true));
                comboMenu.AddItem(
                    new MenuItem("ComboRCount", "Use R|Counts Enemies In Range >= x", true).SetValue(new Slider(3, 1, 5)));
                comboMenu.AddItem(
                    new MenuItem("ComboPassive", "Use Spell|When target Have x Passive", true).SetValue(new Slider(3, 0, 3)));
            }

            var harassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                harassMenu.AddItem(new MenuItem("HarassQ", "Use Q", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassE", "Use E", true).SetValue(false));
                harassMenu.AddItem(
                    new MenuItem("AutoHarass", "Auto Harass?", true).SetValue(new KeyBind('G', KeyBindType.Toggle)));
                harassMenu.AddItem(
                    new MenuItem("HarassMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var clearMenu = Menu.AddSubMenu(new Menu("Clear", "Clear"));
            {
                var laneClearMenu = clearMenu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
                {
                    laneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q", true).SetValue(true));
                    laneClearMenu.AddItem(
                        new MenuItem("LaneClearQCount", "If Q CanHit Counts >= x", true).SetValue(new Slider(3, 1, 5)));
                    laneClearMenu.AddItem(new MenuItem("LaneClearE", "Use E", true).SetValue(true));
                    laneClearMenu.AddItem(
                        new MenuItem("LaneClearECount", "If E CanHit Counts >= x", true).SetValue(new Slider(3, 1, 5)));
                    laneClearMenu.AddItem(
                        new MenuItem("LaneClearMana", "If Player ManaPercent >= %", true).SetValue(new Slider(60)));
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
                killStealMenu.AddItem(new MenuItem("KillStealQ", "Use Q", true).SetValue(true));
                killStealMenu.AddItem(new MenuItem("KillStealE", "Use E", true).SetValue(true));
            }

            var miscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                miscMenu.AddItem(
                    new MenuItem("SemiR", "Semi-manual R Key", true).SetValue(new KeyBind('T', KeyBindType.Press)));
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

            Orbwalking.BeforeAttack += BeforeAttack;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
        }

        private void BeforeAttack(Orbwalking.BeforeAttackEventArgs Args)
        {
            if (Me.IsDead)
            {
                return;
            }

            Args.Process = !Q.IsCharging;
        }

        private void OnUpdate(EventArgs args)
        {
            if (Me.IsDead || Me.IsRecalling())
            {
                return;
            }

            SemiRLogic();
            AutoHarass();
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

        private void SemiRLogic()
        {
            if (Menu.GetKey("SemiR") && R.IsReady())
            {
                var target = TargetSelector.GetSelectedTarget() ??
                             TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

                if (target.Check(R.Range))
                {
                    SpellManager.PredCast(R, target);
                }
            }
        }

        private void AutoHarass()
        {
            if (Me.UnderTurret(true))
            {
                return;
            }

            if (Menu.GetKey("AutoHarass") && !Orbwalking.isCombo && !Orbwalking.isHarass && !Me.IsRecalling())
            {
                Harass();
            }
        }

        private void KillSteal()
        {
            if (Menu.GetBool("KillStealQ") && Q.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(qRange) && x.Health < Q.GetDamage(x)))
                {
                    CastQ(target);
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
                         TargetSelector.GetTarget(qRange, TargetSelector.DamageType.Physical);

            if (target.Check(qRange))
            {
                if (Menu.GetBool("ComboR") && R.IsReady() && target.IsValidTarget(R.Range))
                {
                    if (Menu.GetBool("ComboRSolo") && Me.CountEnemiesInRange(1000) <= 2)
                    {
                        if (target.Health + target.HPRegenRate * 2 <
                            R.GetDamage(target) + W.GetDamage(target) + (E.IsReady() ? E.GetDamage(target) : 0) +
                            (Q.IsReady() ? Q.GetDamage(target) : 0) + Me.GetAutoAttackDamage(target) * 3)
                        {
                            SpellManager.PredCast(R, target);
                        }
                    }

                    var rPred = R.GetPrediction(target, true);

                    if (rPred.AoeTargetsHitCount >= Menu.GetSlider("ComboRCount") ||
                        Me.CountEnemiesInRange(R.Range) >= Menu.GetSlider("ComboRCount"))
                    {
                        SpellManager.PredCast(R, target);
                    }
                }

                if (Menu.GetBool("ComboQ") && target.IsValidTarget(qRange))
                {
                    if (target.DistanceToPlayer() >= Orbwalking.GetRealAutoAttackRange(Me) + 200 ||
                        GetPassiveCount(target) >= Menu.GetSlider("ComboPassive") ||
                        W.Level == 0 || target.Health < Q.GetDamage(target))
                    {
                        CastQ(target);
                    }
                }

                if (Menu.GetBool("ComboE") && E.IsReady() && target.IsValidTarget(E.Range))
                {
                    if (GetPassiveCount(target) >= Menu.GetSlider("ComboPassive") ||
                        W.Level == 0 || target.Health < E.GetDamage(target))
                    {
                        SpellManager.PredCast(E, target, true);
                    }
                }
            }
        }

        private void Harass()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("HarassMana")))
            {
                var target = TargetSelector.GetTarget(qRange, TargetSelector.DamageType.Physical);

                if (target.Check(qRange))
                {
                    if (Menu.GetBool("HarassQ") && Q.IsReady() && target.IsValidTarget(qRange))
                    {
                        CastQ(target);
                    }

                    if (Menu.GetBool("HarassE") && E.IsReady() && target.IsValidTarget(E.Range))
                    {
                        SpellManager.PredCast(E, target, true);
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
                if (Menu.GetBool("LaneClearQ") && Q.IsReady())
                {
                    var qMinions = MinionManager.GetMinions(Me.Position, Q.Range);

                    if (qMinions.Any())
                    {
                        var qFarm =
                            MinionManager.GetBestLineFarmLocation(qMinions.Select(x => x.Position.To2D()).ToList(),
                                Q.Width, Q.Range);

                        if (qFarm.MinionsHit >= Menu.GetSlider("LaneClearQCount"))
                        {
                            CastQ(null, qFarm.Position.To3D());
                        }
                    }
                }

                if (Menu.GetBool("LaneClearE") && E.IsReady())
                {
                    var eMinions = MinionManager.GetMinions(Me.Position, E.Range);

                    if (eMinions.Any())
                    {
                        var eFarm = 
                            MinionManager.GetBestCircularFarmLocation(eMinions.Select(x => x.Position.To2D()).ToList(),
                                E.Width, E.Range);

                        if (eFarm.MinionsHit >= Menu.GetSlider("LaneClearECount"))
                        {
                            E.Cast(eFarm.Position);
                        }
                    }
                }
            }
        }

        private void JungleClear()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("JungleClearMana")) && ManaManager.SpellFarm)
            {
                var mobs = MinionManager.GetMinions(Me.Position, qRange, MinionTypes.All, MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth);

                if (mobs.Any())
                {
                    var mob = mobs.FirstOrDefault(x => !x.Name.Contains("mini"));

                    if (mob != null)
                    {
                        if (Menu.GetBool("JungleClearQ") && Q.IsReady() && mob.IsValidTarget(qRange))
                        {
                            CastQ(mob);
                        }

                        if (Menu.GetBool("JungleClearE") && E.IsReady() && mob.IsValidTarget(E.Range))
                        {
                            E.Cast(mob.Position);
                        }
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

        private void CastQ(Obj_AI_Base target = null, Vector3 pos = default(Vector3))
        {
            if (Q.IsCharging)
            {
                if (target != null && target.IsValidTarget(qRange))
                {
                    SpellManager.PredCast(Q, target, true);
                }
                
                if (pos != default(Vector3))
                {
                    Q.Cast(pos, true);
                }

                if (target == null && pos == default(Vector3))
                {
                    foreach (
                        var t in
                        HeroManager.Enemies.Where(x => !x.IsDead && !x.IsZombie && x.IsValidTarget(qRange))
                            .OrderBy(x => x.Health))
                    {
                        SpellManager.PredCast(Q, t, true);
                    }
                }
            }
            else
            {
                Q.StartCharging();
            }
        }

        private static int GetPassiveCount(Obj_AI_Base target)
        {
            return target.HasBuff("varuswdebuff") ? target.GetBuffCount("varuswdebuff") : 0;
        }
    }
}
