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
    

    internal class KogMaw : Logic
    {
        public KogMaw()
        {
            Q = new Spell(SpellSlot.Q, 980f);
            W = new Spell(SpellSlot.W, Me.AttackRange);
            E = new Spell(SpellSlot.E, 1200f);
            R = new Spell(SpellSlot.R, 1800f);

            Q.SetSkillshot(0.25f, 50f, 2000f, true, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 120f, 1400f, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(1.2f, 120f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            var comboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                comboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboR", "Use R", true).SetValue(true));
                comboMenu.AddItem(
                    new MenuItem("ComboRLimit", "Use R|Limit Stack >= x", true).SetValue(new Slider(3, 0, 10)));
            }

            var harassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                harassMenu.AddItem(new MenuItem("HarassQ", "Use Q", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassE", "Use E", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassR", "Use R", true).SetValue(true));
                harassMenu.AddItem(
                    new MenuItem("HarassRLimit", "Use R|Limit Stack >= x", true).SetValue(new Slider(5, 0, 10)));
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
                    laneClearMenu.AddItem(new MenuItem("LaneClearR", "Use R", true).SetValue(true));
                    laneClearMenu.AddItem(
                        new MenuItem("LaneClearRLimit", "Use R|Limit Stack >= x", true).SetValue(new Slider(4, 0, 10)));
                    laneClearMenu.AddItem(
                        new MenuItem("LaneClearMana", "If Player ManaPercent >= %", true).SetValue(new Slider(60)));
                }

                var jungleClearMenu = clearMenu.AddSubMenu(new Menu("JungleClear", "JungleClear"));
                {
                    jungleClearMenu.AddItem(new MenuItem("JungleClearQ", "Use Q", true).SetValue(true));
                    jungleClearMenu.AddItem(new MenuItem("JungleClearW", "Use W", true).SetValue(true));
                    jungleClearMenu.AddItem(new MenuItem("JungleClearE", "Use E", true).SetValue(true));
                    jungleClearMenu.AddItem(new MenuItem("JungleClearR", "Use R", true).SetValue(true));
                    jungleClearMenu.AddItem(
                        new MenuItem("JungleClearRLimit", "Use R|Limit Stack >= x", true).SetValue(new Slider(5, 0, 10)));
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
                killStealMenu.AddItem(new MenuItem("KillStealR", "Use R", true).SetValue(true));
            }

            var miscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                miscMenu.AddItem(new MenuItem("GapE", "Anti GapCloser E", true).SetValue(true));
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
                drawMenu.AddItem(new MenuItem("DrawQ", "Draw Q Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawW", "Draw W Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawE", "Draw E Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawR", "Draw R Range", true).SetValue(false));
                ManaManager.AddDrawFarm(drawMenu);
                DamageIndicator.AddToMenu(drawMenu);
            }

            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Obj_AI_Base.OnSpellCast += OnSpellCast;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
        }

        private void OnEnemyGapcloser(ActiveGapcloser Args)
        {
            if (Menu.GetBool("GapE") && E.IsReady() && Args.Sender.IsValidTarget(E.Range))
            {
                SpellManager.PredCast(E, Args.Sender, true);
            }
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
                    if (Menu.GetBool("ComboW") && W.IsReady() && target.IsValidTarget(W.Range))
                    {
                        W.Cast();
                    }
                    else if (Menu.GetBool("ComboR") && R.IsReady() && Menu.GetSlider("ComboRLimit") >= GetRCount &&
                        target.IsValidTarget(R.Range))
                    {
                        SpellManager.PredCast(R, target, true);
                    }
                    else if (Menu.GetBool("ComboQ") && Q.IsReady() && target.IsValidTarget(Q.Range))
                    {
                        SpellManager.PredCast(Q, target);
                    }
                    else if (Menu.GetBool("ComboE") && E.IsReady() && target.IsValidTarget(E.Range))
                    {
                        SpellManager.PredCast(E, target, true);
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

                        if (Menu.GetBool("JungleClearW") && W.IsReady() && bigmob != null && bigmob.IsValidTarget(W.Range))
                        {
                            W.Cast();
                        }
                        else if (Menu.GetBool("JungleClearR") && R.IsReady() && Menu.GetSlider("JungleClearRLimit") >= GetRCount && 
                            bigmob != null)
                        {
                            R.Cast(bigmob);
                        }
                        else if (Menu.GetBool("JungleClearE") && E.IsReady())
                        {
                            if (bigmob != null && bigmob.IsValidTarget(E.Range))
                            {
                                E.Cast(bigmob);
                            }
                            else
                            {
                                var eMobs = MinionManager.GetMinions(Me.Position, E.Range, MinionTypes.All, MinionTeam.Neutral,
                                    MinionOrderTypes.MaxHealth);
                                var eFarm = E.GetLineFarmLocation(eMobs, E.Width);

                                if (eFarm.MinionsHit >= 2)
                                {
                                    E.Cast(eFarm.Position);
                                }
                            }
                        }
                        else if (Menu.GetBool("JungleClearQ") && Q.IsReady() && mob != null && mob.IsValidTarget(Q.Range))
                        {
                            Q.Cast(mob);
                        }
                    }
                }
            }
        }

        private void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead || Me.IsRecalling())
            {
                return;
            }

            if (W.Level > 0)
            {
                W.Range = Me.AttackRange + new[] { 130, 150, 170, 190, 210 }[W.Level - 1];
            }

            if (R.Level > 0)
            {
                R.Range = 1200 + 300*R.Level - 1;
            }

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
                    SpellManager.PredCast(R, target, true);
                }
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

            if (Menu.GetBool("KillStealR") && R.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range) && x.Health < R.GetDamage(x)))
                {
                    SpellManager.PredCast(R, target, true);
                    return;
                }
            }
        }

        private void Combo()
        {
            var target = TargetSelector.GetSelectedTarget() ??
                         TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

            if (target.Check(R.Range))
            {
                if (Menu.GetBool("ComboR") && R.IsReady() &&
                    Menu.GetSlider("ComboRLimit") >= GetRCount &&
                    target.IsValidTarget(R.Range))
                {
                    SpellManager.PredCast(R, target, true);
                }

                if (Menu.GetBool("ComboQ") && Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    SpellManager.PredCast(Q, target, true);
                }

                if (Menu.GetBool("ComboE") && E.IsReady() && target.IsValidTarget(E.Range))
                {
                    SpellManager.PredCast(E, target);
                }

                if (Menu.GetBool("ComboW") && W.IsReady() && target.IsValidTarget(W.Range) &&
                    target.DistanceToPlayer() > Orbwalking.GetRealAutoAttackRange(Me) && Me.CanAttack)
                {
                    W.Cast();
                }
            }
        }

        private void Harass()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("HarassMana")))
            {
                var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);

                if (target.Check(R.Range))
                {
                    if (Menu.GetBool("HarassR") && R.IsReady() && Menu.GetSlider("HarassRLimit") >= GetRCount &&
                        target.IsValidTarget(R.Range))
                    {
                        SpellManager.PredCast(R, target, true);
                    }

                    if (Menu.GetBool("HarassQ") && Q.IsReady() && target.IsValidTarget(Q.Range))
                    {
                        SpellManager.PredCast(Q, target);
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
                var minions = MinionManager.GetMinions(Me.Position, R.Range);

                if (minions.Any())
                {
                    if (Menu.GetBool("LaneClearR") && R.IsReady() && Menu.GetSlider("LaneClearRLimit") >= GetRCount)
                    {
                        var rMinion =
                            minions.FirstOrDefault(x => x.DistanceToPlayer() > Orbwalking.GetRealAutoAttackRange(Me));

                        if (rMinion != null && HealthPrediction.GetHealthPrediction(rMinion, 250) > 0)
                        {
                            R.Cast(rMinion);
                        }
                    }

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

                    if (Menu.GetBool("JungleClearW") && W.IsReady() && bigmob != null && bigmob.IsValidTarget(W.Range))
                    {
                        W.Cast();
                    }

                    if (Menu.GetBool("JungleClearR") && R.IsReady() && Menu.GetSlider("JungleClearRLimit") >= GetRCount && 
                        bigmob != null)
                    {
                        R.Cast(bigmob);
                    }

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

                            if (eFarm.MinionsHit >= 2)
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

        private static int GetRCount
            => Me.HasBuff("kogmawlivingartillerycost") ? Me.GetBuffCount("kogmawlivingartillerycost") : 0;
    }
}
