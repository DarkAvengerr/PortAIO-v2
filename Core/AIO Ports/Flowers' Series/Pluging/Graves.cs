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
    

    internal class Graves : Logic
    {
        private bool canE;
        private readonly float SearchERange = 875f;

        public Graves()
        {
            Q = new Spell(SpellSlot.Q, 800f);
            W = new Spell(SpellSlot.W, 900f);
            E = new Spell(SpellSlot.E, 425f);
            R = new Spell(SpellSlot.R, 1050f);

            Q.SetSkillshot(0.25f, 40f, 3000f, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.25f, 250f, 1000f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.25f, 100f, 2100f, false, SkillshotType.SkillshotLine);

            var comboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                comboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboEAA", "Use E| Reset Attack", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboECheck", "Use E| Safe Check", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboR", "Use R", true).SetValue(false));
                comboMenu.AddItem(
                    new MenuItem("ComboRCount", "Use R| Min Hit Count >= x", true).SetValue(new Slider(4, 1, 5)));
            }

            var harassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                harassMenu.AddItem(new MenuItem("HarassQ", "Use Q", true).SetValue(true));
                harassMenu.AddItem(
                    new MenuItem("HarassMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var clearMenu = Menu.AddSubMenu(new Menu("Clear", "Clear"));
            {
                var laneClearMenu = clearMenu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
                {
                    laneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q", true).SetValue(true));
                    laneClearMenu.AddItem(
                        new MenuItem("LaneClearQCount", "Use Q| Min Hit Count >= x", true).SetValue(new Slider(3, 1, 5)));
                    laneClearMenu.AddItem(
                        new MenuItem("LaneClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
                }

                var jungleClearMenu = clearMenu.AddSubMenu(new Menu("JungleClear", "JungleClear"));
                {
                    jungleClearMenu.AddItem(new MenuItem("JungleClearQ", "Use Q", true).SetValue(true));
                    jungleClearMenu.AddItem(new MenuItem("JungleClearE", "Use E", true).SetValue(true));
                    jungleClearMenu.AddItem(
                        new MenuItem("JungleClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(20)));
                }

                clearMenu.AddItem(new MenuItem("asdqweqwe", " ", true));
                ManaManager.AddSpellFarm(clearMenu);
            }

            var killStealMenu = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            {
                killStealMenu.AddItem(new MenuItem("KillStealQ", "KillSteal Q", true).SetValue(true));
                killStealMenu.AddItem(new MenuItem("KillStealW", "KillSteal W", true).SetValue(true));
                killStealMenu.AddItem(new MenuItem("KillStealR", "KillSteal R", true).SetValue(true));
                foreach (var target in HeroManager.Enemies)
                {
                    killStealMenu.AddItem(new MenuItem("KillStealR" + target.ChampionName.ToLower(),
                        "Kill: " + target.ChampionName, true).SetValue(true));
                }
            }

            var burstMenu = Menu.AddSubMenu(new Menu("Burst", "Burst"));
            {
                burstMenu.AddItem(new MenuItem("BurstKeys", "Burst Key -> Please Check The Orbwalker Key!", true));
                burstMenu.AddItem(new MenuItem("Bursttarget", "Burst Target -> Left Click to Lock!", true));
                burstMenu.AddItem(new MenuItem("Burstranges",
                    "How to Burst -> Lock the target and then just press Burst Key!", true));
                burstMenu.AddItem(new MenuItem("BurstER", "Burst Mode -> Enabled E->R?", true).SetValue(false))
                    .SetTooltip("if you dont enabled is RE Burst Mode");
            }
            
            var miscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                miscMenu.AddItem(new MenuItem("GapW", "Use W| Anti GapCloser", true).SetValue(true));
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
                drawMenu.AddItem(new MenuItem("DrawW", "Draw W Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawE", "Draw E Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawR", "Draw R Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawBurst", "Draw Burst Range", true).SetValue(false));
                ManaManager.AddDrawFarm(drawMenu);
                DamageIndicator.AddToMenu(drawMenu);
            }

            Obj_AI_Base.OnPlayAnimation += OnPlayAnimation;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnSpellCast += OnSpellCast;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Drawing.OnDraw += OnDraw;
        }

        private void OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs Args)
        {
            if (!sender.IsMe || Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Burst)
            {
                return;
            }

            if (Args.Animation == "Spell3")
            {
                Orbwalking.ResetAutoAttackTimer();
                canE = false;

                if (Menu.GetBool("BurstER") && TargetSelector.GetSelectedTarget() != null && R.IsReady())
                {
                    var target = TargetSelector.GetSelectedTarget();

                    if (target != null)
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(Game.Ping, () => R.Cast(target.ServerPosition, true));
                    }
                }
            }

            if ((Args.Animation == "Spell4" || Args.Animation == "785121b3") && TargetSelector.GetSelectedTarget() != null && 
                !Menu.GetBool("BurstER") && E.IsReady())
            {
                E.Cast(TargetSelector.GetSelectedTarget().Position, true);
            }
        }

        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if (Args.SData.Name.Contains("GravesMove"))
            {
                Orbwalking.ResetAutoAttackTimer();
                canE = false;
            }
        }

        private void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead || Me.IsRecalling())
            {
                return;
            }

            KillSteal();

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Burst:
                    Burst();
                    break;
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
                foreach (var target in HeroManager.Enemies.Where(x => x.Check(Q.Range) && x.Health < Q.GetDamage(x)))
                {
                    SpellManager.PredCast(Q, target);
                }
            }

            if (Menu.GetBool("KillStealW") && W.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.Check(W.Range) && x.Health < W.GetDamage(x)))
                {
                    SpellManager.PredCast(W, target);
                }
            }

            if (Menu.GetBool("KillStealR") && R.IsReady())
            {
                foreach (
                    var target in
                    HeroManager.Enemies.Where(
                        x => x.Check(R.Range) && 
                        Menu.GetBool("KillStealR" + x.ChampionName.ToLower()) && x.Health < R.GetDamage(x) &&
                        x.DistanceToPlayer() > Orbwalking.GetRealAutoAttackRange(Me)))
                {
                    SpellManager.PredCast(R, target);
                }
            }
        }

        private void Burst()
        {
            var target = TargetSelector.GetSelectedTarget();

            if (target.Check(800f))
            {
                var pos = Me.Position.Extend(target.Position, E.Range);

                if (R.IsReady())
                {
                    if (!Menu.GetBool("BurstER"))
                    {
                        if (E.IsReady() && target.IsValidTarget(600f))
                        {
                            if (R.CanCast(target))
                            {
                                R.Cast(target, true);
                            }
                        }
                    }
                    else
                    {
                        if (E.IsReady() && target.IsValidTarget(600f))
                        {
                            E.Cast(target.Position, true);
                        }
                    }
                }
                else
                {
                    if (Q.IsReady() && target.IsValidTarget(Q.Range))
                    {
                        SpellManager.PredCast(Q, target);
                    }

                    if (W.IsReady() && target.IsValidTarget(W.Range) &&
                             (target.DistanceToPlayer() <= target.AttackRange + 70 ||
                              (target.DistanceToPlayer() >= Orbwalking.GetRealAutoAttackRange(Me) + 80)))
                    {
                        SpellManager.PredCast(W, target);
                    }

                    if (E.IsReady() && !R.IsReady())
                    {
                        ELogic(target);
                    }
                }
            }
        }

        private void Combo()
        {
            var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

            if (target.Check(R.Range))
            {
                if (Menu.GetBool("ComboQ") && Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    SpellManager.PredCast(Q, target);
                }

                if (Menu.GetBool("ComboE") && E.IsReady() && target.IsValidTarget(SearchERange))
                {
                    ELogic(target);
                }

                if (Menu.GetBool("ComboW") && W.IsReady() && target.IsValidTarget(W.Range) &&
                    (target.DistanceToPlayer() <= target.AttackRange + 70 ||
                     (target.DistanceToPlayer() >= Orbwalking.GetRealAutoAttackRange(Me) + 80)))
                {
                    SpellManager.PredCast(W, target);
                }

                if (Menu.GetBool("ComboR") && R.IsReady() && target.IsValidTarget(R.Range))
                {
                    R.CastIfWillHit(target, Menu.GetSlider("ComboRCount"));
                }
            }
        }

        private void Harass()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("HarassMana")))
            {
                var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

                if (Menu.GetBool("HarassQ") && Q.IsReady() && target.Check(Q.Range))
                {
                    SpellManager.PredCast(Q, target);
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
                    var Minions = MinionManager.GetMinions(Me.Position, Q.Range);

                    if (Minions.Any())
                    {
                        var QFarm =
                            MinionManager.GetBestLineFarmLocation(Minions.Select(x => x.Position.To2D()).ToList(),
                                Q.Width, Q.Range);

                        if (QFarm.MinionsHit >= Menu.GetSlider("LaneClearQCount"))
                        {
                            Q.Cast(QFarm.Position);
                        }
                    }
                }
            }
        }

        private void JungleClear()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("JungleClearMana")) && ManaManager.SpellFarm)
            {
                if (Menu.GetBool("JungleClearQ") && Q.IsReady())
                {
                    var mobs = MinionManager.GetMinions(Me.Position, Q.Range, MinionTypes.All, MinionTeam.Neutral,
                        MinionOrderTypes.MaxHealth);

                    if (mobs.Any())
                    {
                        var QFarm =
                            MinionManager.GetBestLineFarmLocation(mobs.Select(x => x.Position.To2D()).ToList(),
                                Q.Width, Q.Range);

                        if (QFarm.MinionsHit >= 1)
                        {
                            Q.Cast(QFarm.Position);
                        }
                    }
                }
            }
        }

        private void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if (Args.SData.Name.Contains("GravesChargeShot"))
            {
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Burst &&
                    TargetSelector.GetSelectedTarget() != null && E.IsReady())
                {
                    var target = TargetSelector.GetSelectedTarget();
                    var pos = Me.Position.Extend(target.Position, E.Range);
                    E.Cast(pos);
                }
            }

            if (Orbwalking.IsAutoAttack(Args.SData.Name))
            {
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                {
                    var target = Args.Target as AIHeroClient;

                    if (target != null && !target.IsDead && !target.IsZombie)
                    {
                        if (Menu.GetBool("ComboEAA") && E.IsReady())
                        {
                            ELogic(target);
                        }
                    }
                }

                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                {
                    var target = Args.Target as Obj_AI_Minion;

                    if (target != null && !target.IsDead)
                    {
                        if (ManaManager.HasEnoughMana(Menu.GetSlider("JungleClearMana")) && ManaManager.SpellFarm)
                        {
                            if (Menu.GetBool("JungleClearE") && E.IsReady())
                            {
                                var mobs =
                                    MinionManager.GetMinions(Me.Position, W.Range, MinionTypes.All, MinionTeam.Neutral,
                                    MinionOrderTypes.MaxHealth).Where(x => !x.Name.ToLower().Contains("mini"));

                                if (mobs.FirstOrDefault() != null)
                                {
                                    if (!Me.Spellbook.IsCastingSpell)
                                    {
                                        ELogic(mobs.FirstOrDefault());
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void OnEnemyGapcloser(ActiveGapcloser Args)
        {
            if (Menu.GetBool("GapW") && W.IsReady() && Args.Sender.IsValidTarget(W.Range) && Args.End.DistanceToPlayer() <= 200)
            {
                W.Cast(Args.End);
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

                if (Menu.GetBool("DrawBurst"))
                {
                    Render.Circle.DrawCircle(Me.Position, Orbwalking.GetRealAutoAttackRange(Me), Color.FromArgb(90, 255, 255), 1);
                }
            }
        }

        private void ELogic(Obj_AI_Base target)
        {
            if (!E.IsReady())
            {
                return;
            }

            var ePosition = Me.ServerPosition.Extend(Game.CursorPos, E.Range);
            var targetDisE = target.ServerPosition.Distance(ePosition);

            if (ePosition.UnderTurret(true) && Me.HealthPercent <= 60)
            {
                canE = false;
            }

            if (Menu.GetBool("ComboECheck"))
            {
                if (ePosition.CountEnemiesInRange(350f) >= 3)
                {
                    canE = false;
                }

                //Catilyn W
                if (ObjectManager
                        .Get<Obj_GeneralParticleEmitter>()
                        .FirstOrDefault(
                            x =>
                                x != null && x.IsValid &&
                                x.Name.ToLower().Contains("yordletrap_idle_red.troy") &&
                                x.Position.Distance(ePosition) <= 100) != null)
                {
                    canE = false;
                }

                //Jinx E
                if (ObjectManager.Get<Obj_AI_Minion>()
                        .FirstOrDefault(x => x.IsValid && x.IsEnemy && x.Name == "k" &&
                                             x.Position.Distance(ePosition) <= 100) != null)
                {
                    canE = false;
                }

                //Teemo R
                if (ObjectManager.Get<Obj_AI_Minion>()
                        .FirstOrDefault(x => x.IsValid && x.IsEnemy && x.Name == "Noxious Trap" &&
                                             x.Position.Distance(ePosition) <= 100) != null)
                {
                    canE = false;
                }

                if (ePosition.CountEnemiesInRange(350) >= 3)
                {
                    canE = false;
                }
            }

            if (target.Distance(ePosition) > Orbwalking.GetRealAutoAttackRange(Me))
            {
                canE = false;
            }

            if (target.Health < Me.GetAutoAttackDamage(target, true)*2 && 
                target.Distance(ePosition) <= Orbwalking.GetRealAutoAttackRange(Me) && Me.CanAttack)
            {
                canE = true;
            }

            if (!Me.HasBuff("GravesBasicAttackAmmo2") && Me.HasBuff("GravesBasicAttackAmmo1") &&
                target.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Me)) &&
                target.Distance(ePosition) <= Orbwalking.GetRealAutoAttackRange(Me))
            {
                canE = true;
            }

            if (!Me.HasBuff("GravesBasicAttackAmmo2") && !Me.HasBuff("GravesBasicAttackAmmo1") && 
                target.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Me)))
            {
                canE = true;
            }

            if (canE)
            {
                E.Cast(Game.CursorPos, true);
                canE = false;
            }
        }
    }
}
