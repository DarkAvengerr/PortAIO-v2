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
    

    internal class Quinn : Logic
    {
        public Quinn()
        {
            Q = new Spell(SpellSlot.Q, 1000f);
            W = new Spell(SpellSlot.W, 2000f);
            E = new Spell(SpellSlot.E, 700f);
            R = new Spell(SpellSlot.R, 550f);

            Q.SetSkillshot(0.25f, 90f, 1550f, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.25f, 2000f, 1400f, false, SkillshotType.SkillshotCircle);
            E.SetTargetted(0.25f, 2000f);

            var comboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                comboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
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
                        new MenuItem("LaneClearQCount", "If Q CanHit Counts >= ", true).SetValue(new Slider(3, 1, 5)));
                    laneClearMenu.AddItem(
                        new MenuItem("LaneClearMana", "When Player ManaPercent >= %", true).SetValue(new Slider(60)));
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
                killStealMenu.AddItem(new MenuItem("KillStealQ", "KillSteal Q", true).SetValue(true));
            }

            var miscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                var eMenu = miscMenu.AddSubMenu(new Menu("E Settings", "E Settings"));
                {
                    eMenu.AddItem(new MenuItem("Interrupt", "Interrupt Danger Spells", true).SetValue(true));
                    eMenu.AddItem(new MenuItem("Gapcloser", "Anti Gapcloser", true).SetValue(true));
                    eMenu.AddItem(new MenuItem("AntiAlistar", "Anti Alistar", true).SetValue(true));
                    eMenu.AddItem(new MenuItem("AntiRengar", "Anti Rengar", true).SetValue(true));
                    eMenu.AddItem(new MenuItem("AntiKhazix", "Anti Khazix", true).SetValue(true));
                }

                var rMenu = miscMenu.AddSubMenu(new Menu("R Settings", "R Settings"));
                {
                    rMenu.AddItem(new MenuItem("AutoR", "Auto R?", true).SetValue(true));
                }

                miscMenu.AddItem(new MenuItem("Forcus", "Forcus Attack Passive Target", true).SetValue(true));
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
                ManaManager.AddDrawFarm(drawMenu);
                //DamageIndicator.AddToMenu(drawMenu);
            }

            GameObject.OnCreate += OnCreate;
            Orbwalking.BeforeAttack += BeforeAttack;
            Obj_AI_Base.OnSpellCast += OnSpellCast;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += OnInterruptableTarget;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
        }

        private void OnCreate(GameObject sender, EventArgs Args)
        {
            var Rengar = HeroManager.Enemies.Find(heros => heros.ChampionName.Equals("Rengar"));
            var Khazix = HeroManager.Enemies.Find(heros => heros.ChampionName.Equals("Khazix"));

            if (Rengar != null && Menu.GetBool("AntiRengar"))
            {
                if (sender.Name == "Rengar_LeapSound.troy" && sender.Position.Distance(Me.Position) < E.Range)
                {
                    E.CastOnUnit(Rengar);
                }
            }

            if (Khazix != null && Menu.GetBool("AntiKhazix"))
            {
                if (sender.Name == "Khazix_Base_E_Tar.troy" && sender.Position.Distance(Me.Position) <= 300)
                {
                    E.CastOnUnit(Khazix);
                }
            }
        }

        private void BeforeAttack(Orbwalking.BeforeAttackEventArgs Args)
        {
            if (Menu.GetBool("Forcus"))
            {
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo ||
                    Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x => !x.IsDead && !x.IsZombie && HavePassive(x)))
                    {
                        Orbwalker.ForceTarget(enemy);
                    }
                }

                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                {
                    var all = MinionManager.GetMinions(Me.Position, Orbwalking.GetRealAutoAttackRange(Me),
                        MinionTypes.All, MinionTeam.NotAlly).Where(HavePassive);

                    if (all.Any())
                    {
                        Orbwalker.ForceTarget(all.FirstOrDefault());
                    }
                }
            }
        }

        private void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (sender.IsMe && Orbwalking.IsAutoAttack(Args.SData.Name))
            {
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                {
                    var target = Args.Target as AIHeroClient;

                    if (target != null && target.Check())
                    {
                        if (Menu.GetBool("ComboE") && E.IsReady())
                        {
                            E.CastOnUnit(target, true);
                        }
                        else if (Menu.GetBool("ComboQ") && Q.IsReady() && !Me.HasBuff("QuinnR"))
                        {
                            SpellManager.PredCast(Q, target, true);
                        }
                    }
                }

                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                {
                    if (ManaManager.HasEnoughMana(Menu.GetSlider("JungleClearMana")) && ManaManager.SpellFarm)
                    {
                        var mobs = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Neutral,
                            MinionOrderTypes.MaxHealth);

                        if (mobs.Any())
                        {
                            var mob = mobs.FirstOrDefault();

                            if (Menu.GetBool("JungleClearE") && E.IsReady() && mob.IsValidTarget(E.Range))
                            {
                                E.CastOnUnit(mob, true);
                            }
                            else if (Menu.GetBool("JungleClearQ") && Q.IsReady() && mob.IsValidTarget(Q.Range) &&
                                !Me.HasBuff("QuinnR"))
                            {
                                Q.Cast(mob, true);
                            }
                        }
                    }
                }
            }
        }

        private void OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs Args)
        {
            if (Menu.GetBool("Interrupt") && E.IsReady() && sender.IsEnemy &&
                sender.IsValidTarget(E.Range))
            {
                if (sender.IsCastingInterruptableSpell())
                {
                    E.CastOnUnit(sender, true);
                }

                if (Args.DangerLevel >= Interrupter2.DangerLevel.High)
                {
                    E.CastOnUnit(sender, true);
                }
            }
        }

        private void OnEnemyGapcloser(ActiveGapcloser Args)
        {
            if (E.IsReady())
            {
                if (Menu.GetBool("AntiAlistar") && Args.Sender.ChampionName == "Alistar" && Args.SkillType == GapcloserType.Targeted)
                {
                    E.CastOnUnit(Args.Sender, true);
                }

                if (Menu.GetBool("Gapcloser"))
                {
                    if (Args.End.DistanceToPlayer() <= 250 && Args.Sender.IsValid)
                    {
                        E.CastOnUnit(Args.Sender, true);
                    }
                }
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (Me.IsDead || Me.IsRecalling())
            {
                return;
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
                case Orbwalking.OrbwalkingMode.None:
                    AutoR();
                    break;
            }
        }

        private void KillSteal()
        {
            if (Menu.GetBool("KillStealQ") && Q.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range) && x.Health < Q.GetDamage(x)))
                {
                    if (target.Check(Q.Range))
                    {
                        SpellManager.PredCast(Q, target, true);
                        return;
                    }
                }
            }
        }

        private void Combo()
        {
            var target = TargetSelector.GetSelectedTarget() ??
                TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);

            if (target.Check(Q.Range))
            {
                if (Menu.GetBool("ComboE") && E.IsReady() && Me.HasBuff("QuinnR"))
                {
                    E.CastOnUnit(target);
                }

                if (Menu.GetBool("ComboQ") && Q.IsReady() && !Me.HasBuff("QuinnR"))
                {
                    if (target.DistanceToPlayer() <= Orbwalking.GetRealAutoAttackRange(Me) && HavePassive(target))
                    {
                        return;
                    }

                    SpellManager.PredCast(Q, target, true);
                }

                if (Menu.GetBool("ComboW") && W.IsReady())
                {
                    var WPred = W.GetPrediction(target);

                    if ((NavMesh.GetCollisionFlags(WPred.CastPosition) == CollisionFlags.Grass ||
                         NavMesh.IsWallOfGrass(target.ServerPosition, 20)) && !target.IsVisible)
                    {
                        W.Cast();
                    }
                }
            }
        }

        private void Harass()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("HarassMana")))
            {
                if (Menu.GetBool("HarassQ"))
                {
                    var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);

                    if (target.Check(Q.Range))
                    {
                        SpellManager.PredCast(Q, target, true);
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
                    var minions = MinionManager.GetMinions(Me.Position, Q.Range);

                    if (minions.Any())
                    {
                        var QFarm =
                            MinionManager.GetBestCircularFarmLocation(minions.Select(x => x.Position.To2D()).ToList(),
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
                var mobs = MinionManager.GetMinions(Me.Position, Q.Range, MinionTypes.All, MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth);

                if (mobs.Any())
                {
                    if (Menu.GetBool("JungleClearQ") && Q.IsReady())
                    {
                        var QFarm =
                            MinionManager.GetBestCircularFarmLocation(mobs.Select(x => x.Position.To2D()).ToList(),
                                Q.Width, Q.Range);

                        if (QFarm.MinionsHit >= 1)
                        {
                            Q.Cast(QFarm.Position);
                        }

                    }

                    if (Menu.GetBool("JungleClearE") && E.IsReady())
                    {
                        var mob =
                            mobs.FirstOrDefault(x => !x.Name.ToLower().Contains("mini") && x.Health >= E.GetDamage(x));

                        if (mob != null)
                        {
                            E.CastOnUnit(mob);
                        }
                    }
                }
            }
        }

        private void AutoR()
        {
            if (Menu.GetBool("AutoR") && R.IsReady() && R.Instance.Name == "QuinnR")
            {
                if (!Me.IsDead && !Shop.IsOpen && !MenuGUI.IsChatOpen   &&
                    Me.InFountain())
                {
                    R.Cast();
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
            }
        }

        private static bool HavePassive(Obj_AI_Base target)
        {
            return target.HasBuff("quinnw");
        }
    }
}
