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

    internal class Ashe : Logic
    {
        public Ashe()
        {
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 1255f);
            E = new Spell(SpellSlot.E, 5000f);
            R = new Spell(SpellSlot.R, 2000f);

            W.SetSkillshot(0.25f, 60f, 2000f, true, SkillshotType.SkillshotCone);
            E.SetSkillshot(0.25f, 300f, 1400f, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.25f, 130f, 1600f, true, SkillshotType.SkillshotLine);

            var comboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                comboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboSaveMana", "Save Mana To Cast Q", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboR", "Use R", true).SetValue(true));
            }

            var harassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                harassMenu.AddItem(new MenuItem("HarassW", "Use W", true).SetValue(true));
                harassMenu.AddItem(
                    new MenuItem("HarassWMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var clearMenu = Menu.AddSubMenu(new Menu("Clear", "Clear"));
            {
                var laneClearMenu = clearMenu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
                {
                    laneClearMenu.AddItem(new MenuItem("LaneClearW", "Use W", true).SetValue(true));
                    laneClearMenu.AddItem(
                        new MenuItem("LaneClearWCount", "If W CanHit Counts >= ", true).SetValue(new Slider(3, 1, 5)));
                    laneClearMenu.AddItem(
                        new MenuItem("LaneClearWMana", "If Player ManaPercent >= %", true).SetValue(new Slider(60)));
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
                killStealMenu.AddItem(new MenuItem("KillStealW", "KillSteal W", true).SetValue(true));
                killStealMenu.AddItem(new MenuItem("KillStealR", "KillSteal R", true).SetValue(true));
                foreach (var target in HeroManager.Enemies)
                {
                    killStealMenu.AddItem(new MenuItem("KillStealR" + target.ChampionName.ToLower(), 
                        "Kill: " + target.ChampionName, true).SetValue(true));
                }
            }

            var miscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                var rMenu = miscMenu.AddSubMenu(new Menu("R Settings", "R Settings"));
                {
                    rMenu.AddItem(new MenuItem("AutoR", "Auto R?", true).SetValue(true));
                    rMenu.AddItem(new MenuItem("Interrupt", "Interrupt Danger Spells", true).SetValue(true));
                    rMenu.AddItem(
                        new MenuItem("SemiR", "Semi-manual R Key", true).SetValue(new KeyBind('T', KeyBindType.Press)));
                }

                var antiGapcloserMenu = miscMenu.AddSubMenu(new Menu("Anti Gapcloser", "Anti Gapcloser"));
                {
                    antiGapcloserMenu.AddItem(new MenuItem("AntiGapCloser", "Enabled", true).SetValue(true));
                    antiGapcloserMenu.AddItem(
                        new MenuItem("AntiGapCloserHp", "AntiGapCloser |When Player HealthPercent <= x%", true).SetValue(
                            new Slider(30)));
                    antiGapcloserMenu.AddItem(new MenuItem("AntiGapCloserRList", "AntiGapCloser R List:"));
                    foreach (var target in HeroManager.Enemies)
                    {
                        antiGapcloserMenu.AddItem(new MenuItem("AntiGapCloserR" + target.ChampionName.ToLower(),
                            "GapCloser: " + target.ChampionName, true).SetValue(true));
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
                drawMenu.AddItem(new MenuItem("DrawR", "Draw R Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawRMin", "Draw R Range(MinMap)", true).SetValue(false));
                ManaManager.AddDrawFarm(drawMenu);
                DamageIndicator.AddToMenu(drawMenu);
            }

            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += OnInterruptableTarget;
            Obj_AI_Base.OnSpellCast += OnSpellCast;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Drawing.OnEndScene += OnEndScene;
        }

        private void OnEnemyGapcloser(ActiveGapcloser Args)
        {
            if (!Args.Sender.IsEnemy || !R.IsReady() || Menu.GetBool("AntiGapCloser") || 
                Me.HealthPercent > Menu.GetSlider("AntiGapCloserHp"))
            {
                return;
            }

            if (Menu.GetBool("AntiGapCloserR" + Args.Sender.ChampionName.ToLower()) && Args.End.DistanceToPlayer() <= 300)
            {
                SpellManager.PredCast(R, Args.Sender);
            }
        }

        private void OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs Args)
        {
            if (Menu.GetBool("Interrupt") && R.IsReady())
            {
                if (!sender.IsEnemy || Args.DangerLevel < Interrupter2.DangerLevel.High || !sender.IsValidTarget(R.Range))
                {
                    return;
                }

                SpellManager.PredCast(R, sender);
            }
        }

        private void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (!sender.IsMe || !Orbwalking.IsAutoAttack(Args.SData.Name))
            {
                return;
            }

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    if (Menu.GetBool("ComboQ"))
                    {
                        var target = (AIHeroClient)Args.Target;

                        if (target != null && !target.IsDead && !target.IsZombie)
                        {
                            if (Me.HasBuff("asheqcastready"))
                            {
                                Q.Cast();
                                Orbwalking.ResetAutoAttackTimer();
                            }
                        }
                    }
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    if (ManaManager.HasEnoughMana(Menu.GetSlider("JungleClearMana")) && ManaManager.SpellFarm)
                    {
                        if (Menu.GetBool("JungleClearQ") && Args.Target is Obj_AI_Minion)
                        {
                            var mobs = MinionManager.GetMinions(Me.Position, Orbwalking.GetRealAutoAttackRange(Me),
                                MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

                            if (mobs.Any())
                            {
                                foreach (var mob in mobs)
                                {
                                    if (!mob.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Me)) ||
                                        !(mob.Health > Me.GetAutoAttackDamage(mob) * 2))
                                    {
                                        continue;
                                    }

                                    if (Me.HasBuff("asheqcastready"))
                                    {
                                        Q.Cast();
                                        Orbwalking.ResetAutoAttackTimer();
                                    }
                                }
                            }
                        }
                    }
                    break;
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (Me.IsDead || Me.IsRecalling())
            {
                return;
            }

            if (Menu.GetKey("SemiR"))
            {
                OneKeyR();
            }

            AutoRLogic();
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

        private void AutoRLogic()
        {
            if (Menu.GetBool("AutoR") && R.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range) && x.Check(R.Range)))
                {
                    if (!(target.DistanceToPlayer() > Orbwalking.GetRealAutoAttackRange(Me)) ||
                        !(target.DistanceToPlayer() <= 700) ||
                        !(target.Health > Me.GetAutoAttackDamage(target)) ||
                        !(target.Health < R.GetDamage(target) + Me.GetAutoAttackDamage(target)*3) ||
                        target.HasBuffOfType(BuffType.SpellShield))
                    {
                        continue;
                    }

                    SpellManager.PredCast(R, target);
                    return;
                }
            }
        }

        private void KillSteal()
        {
            if (Menu.GetBool("KillStealW") && W.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.Check(W.Range)))
                {
                    if (!target.IsValidTarget(W.Range) || !(target.Health < W.GetDamage(target)))
                        continue;

                    if (target.DistanceToPlayer() <= Orbwalking.GetRealAutoAttackRange(Me) && 
                        Me.HasBuff("AsheQAttack"))
                    {
                        continue;
                    }

                    SpellManager.PredCast(W, target);
                    return;
                }
            }

            if (!Menu.GetBool("KillStealR") || !R.IsReady())
            {
                return;
            }

            foreach (
                var target in
                HeroManager.Enemies.Where(
                    x =>
                        x.Check(2000) &&
                        Menu.GetBool("KillStealR" + x.ChampionName.ToLower())))
            {
                if (!(target.DistanceToPlayer() > 800) || !(target.Health < R.GetDamage(target)) ||
                    target.HasBuffOfType(BuffType.SpellShield))
                {
                    continue;
                }

                SpellManager.PredCast(R, target);
                return;
            }
        }

        private void Combo()
        {
            if (Menu.GetBool("ComboR") && R.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.Check(1200)))
                {
                    if (target.IsValidTarget(600) && Me.CountEnemiesInRange(600) >= 3 && target.CountAlliesInRange(200) <= 2)
                    {
                        SpellManager.PredCast(R, target);
                    }

                    if (Me.CountEnemiesInRange(800) == 1 &&
                        target.DistanceToPlayer() > Orbwalking.GetRealAutoAttackRange(Me) &&
                        target.DistanceToPlayer() <= 700 &&
                        target.Health > Me.GetAutoAttackDamage(target) &&
                        target.Health < R.GetDamage(target) + Me.GetAutoAttackDamage(target)*3 &&
                        !target.HasBuffOfType(BuffType.SpellShield))
                    {
                        SpellManager.PredCast(R, target);
                    }

                    if (target.DistanceToPlayer() <= 1000 &&
                        (!target.CanMove || target.HasBuffOfType(BuffType.Stun) ||
                        R.GetPrediction(target).Hitchance == HitChance.Immobile))
                    {
                        SpellManager.PredCast(R, target);
                    }
                }
            }

            if (Menu.GetBool("ComboW") && W.IsReady() && !Me.HasBuff("AsheQAttack"))
            {
                if ((Menu.GetBool("ComboSaveMana") &&
                     Me.Mana > (R.IsReady() ? R.Instance.SData.Mana : 0) + W.Instance.SData.Mana + Q.Instance.SData.Mana) ||
                    !Menu.GetBool("ComboSaveMana"))
                {
                    var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);

                    if (target.Check(W.Range))
                    {
                        SpellManager.PredCast(W, target);
                    }
                }
            }

            if (Menu.GetBool("ComboE") && E.IsReady())
            {
                var target = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Physical);

                if (target.Check(1000))
                {
                    var EPred = E.GetPrediction(target);

                    if ((NavMesh.GetCollisionFlags(EPred.CastPosition) == CollisionFlags.Grass ||
                         NavMesh.IsWallOfGrass(target.ServerPosition, 20)) && !target.IsVisible)
                    {
                        E.Cast(EPred.CastPosition);
                    }
                }
            }
        }

        private void Harass()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("HarassWMana")))
            {
                if (Menu.GetBool("HarassW") && W.IsReady() && !Me.HasBuff("AsheQAttack"))
                {
                    var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);

                    if (target.Check(W.Range))
                    {
                        SpellManager.PredCast(W, target);
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
            if (ManaManager.HasEnoughMana(Menu.GetSlider("LaneClearWMana")) && ManaManager.SpellFarm)
            {
                if (Menu.GetBool("LaneClearW") && W.IsReady())
                {
                    var minions = MinionManager.GetMinions(Me.Position, W.Range);

                    if (minions.Any())
                    {
                        var wFarm = MinionManager.GetBestCircularFarmLocation(minions.Select(x => x.Position.To2D()).ToList(),
                            W.Width, W.Range);

                        if (wFarm.MinionsHit >= Menu.GetSlider("LaneClearWCount"))
                        {
                            W.Cast(wFarm.Position);
                        }
                    }
                }
            }
        }

        private void JungleClear()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("JungleClearMana")))
            {
                if (Menu.GetBool("JungleClearW") && !Me.HasBuff("AsheQAttack"))
                {
                    var mobs = MinionManager.GetMinions(Me.Position, W.Range, MinionTypes.All, MinionTeam.Neutral,
                        MinionOrderTypes.MaxHealth);

                    if (mobs.Any())
                    {
                        var mob = mobs.FirstOrDefault();

                        if (mob != null)
                        {
                            W.Cast(mob.Position);
                        }
                    }
                }
            }
        }

        private void OneKeyR()
        {
            Orbwalking.MoveTo(Game.CursorPos);

            if (R.IsReady())
            {
                var select = TargetSelector.GetSelectedTarget();
                var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);

                if (select != null && !target.HasBuffOfType(BuffType.SpellShield) && target.IsValidTarget(R.Range))
                {
                    SpellManager.PredCast(R, target);
                }
                else if (select == null && target != null && !target.HasBuffOfType(BuffType.SpellShield) &&
                    target.IsValidTarget(R.Range))
                {
                    SpellManager.PredCast(R, target);
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

                if (Menu.GetBool("DrawR") && R.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, R.Range, Color.FromArgb(19, 130, 234), 1);
                }
            }
        }

        private void OnEndScene(EventArgs Args)
        {
            if (!Me.IsDead && !Shop.IsOpen && !MenuGUI.IsChatOpen  )
            {
#pragma warning disable 618
                if (Menu.GetBool("DrawRMin") && R.IsReady())
                {
                    LeagueSharp.Common.Utility.DrawCircle(Me.Position, R.Range, Color.FromArgb(14, 194, 255), 1, 30, true);
                }
#pragma warning restore 618
            }
        }
    }
}
