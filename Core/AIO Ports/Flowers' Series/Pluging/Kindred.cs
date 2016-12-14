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
    

    internal class Kindred : Logic
    {
        public Kindred()
        {
            Q = new Spell(SpellSlot.Q, 340f);
            W = new Spell(SpellSlot.W, 800f);
            E = new Spell(SpellSlot.E, 550f);
            R = new Spell(SpellSlot.R, 550f);

            var comboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                comboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboAQA", "Use AA-Q-AA Logic", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
            }

            var harassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                harassMenu.AddItem(new MenuItem("HarassQ", "Use Q", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassW", "Use W", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassE", "Use E", true).SetValue(true));
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
                    jungleClearMenu.AddItem(new MenuItem("JungleClearE", "Use E", true).SetValue(true));
                    jungleClearMenu.AddItem(
                        new MenuItem("JungleClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(20)));
                }

                clearMenu.AddItem(new MenuItem("asdqweqwe", " ", true));
                ManaManager.AddSpellFarm(clearMenu);
            }

            var killStealMenu = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            {
                killStealMenu.AddItem(new MenuItem("KillStealQ", "Use Q", true).SetValue(true));
            }

            var fleeMenu = Menu.AddSubMenu(new Menu("Flee", "Flee"));
            {
                fleeMenu.AddItem(new MenuItem("FleeQ", "Use Q", true).SetValue(true));
                fleeMenu.AddItem(new MenuItem("FleeKey", "Flee Key", true).SetValue(new KeyBind('Z', KeyBindType.Press)));
            }

            var miscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                var qMenu = miscMenu.AddSubMenu(new Menu("Q Settings", "Q Settings"));
                {
                    qMenu.AddItem(new MenuItem("QCheck", "Use Q|Safe Check?", true).SetValue(true));
                    qMenu.AddItem(new MenuItem("QTurret", "Use Q|Dont Cast To Turret", true).SetValue(true));
                    qMenu.AddItem(new MenuItem("QMelee", "Use Q|Anti Melee", true).SetValue(true));
                }

                var rMenu = miscMenu.AddSubMenu(new Menu("R Settings", "R Settings"));
                {
                    rMenu.AddItem(new MenuItem("AutoR", "Auto R Save Myself?", true).SetValue(true));
                    rMenu.AddItem(
                        new MenuItem("AutoRHp", "Auto R| When Player Health Percent <= x%", true).SetValue(new Slider(15)));
                    rMenu.AddItem(new MenuItem("AutoSave", "Auto R Save Ally?", true).SetValue(true));
                    rMenu.AddItem(
                        new MenuItem("AutoSaveHp", "Auto R| When Ally Health Percent <= x%", true).SetValue(new Slider(20)));
                }

                miscMenu.AddItem(new MenuItem("Forcus", "Forcus Attack Passive Target", true).SetValue(true));
                miscMenu.AddItem(new MenuItem("ForcusE", "Forcus Attack E Mark Target", true).SetValue(true));
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
                DamageIndicator.AddToMenu(drawMenu);
            }

            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Game.OnUpdate += OnUpdate;
            Orbwalking.BeforeAttack += BeforeAttack;
            Orbwalking.AfterAttack += AfterAttack;
            Drawing.OnDraw += OnDraw;
        }

        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (sender == null || !sender.IsEnemy || Args.Target == null || Me.IsDead || Me.InFountain())
            {
                return;
            }

            switch (sender.Type)
            {
                case GameObjectType.AIHeroClient:
                    if (Args.Target.IsMe)
                    {
                        if (sender.IsMelee && Q.IsReady() && Menu.GetBool("QMelee"))
                        {
                            Q.Cast(Me.Position.Extend(sender.Position, -Q.Range));
                        }

                        if (R.IsReady() && Menu.GetBool("AutoR"))
                        {
                            if (Me.HealthPercent <= Menu.GetSlider("AutoRHp"))
                            {
                                R.Cast();
                            }

                            if (Orbwalking.IsAutoAttack(Args.SData.Name))
                            {
                                if (sender.GetAutoAttackDamage(Me, true) >= Me.Health)
                                {
                                    R.Cast();
                                }
                            }
                            else
                            {
                                var target = (AIHeroClient)Args.Target;

                                if (target.GetSpellSlot(Args.SData.Name) != SpellSlot.Unknown)
                                {
                                    if (target.GetSpellDamage(Me, Args.SData.Name) > Me.Health)
                                    {
                                        if (Args.End.DistanceToPlayer() < 150 + Me.BoundingRadius)
                                        {
                                            R.Cast();
                                        }

                                        if (target.DistanceToPlayer() < 150 + Me.BoundingRadius)
                                        {
                                            R.Cast();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (Args.Target.IsAlly && Args.Target.Type == GameObjectType.AIHeroClient && !Args.Target.IsDead)
                    {
                        var ally = (AIHeroClient) Args.Target;

                        if (R.IsReady() && Menu.GetBool("AutoSave") && ally.DistanceToPlayer() <= R.Range)
                        {
                            if (ally.HealthPercent <= Menu.GetSlider("AutoSaveHp"))
                            {
                                R.Cast();
                            }

                            if (Orbwalking.IsAutoAttack(Args.SData.Name))
                            {
                                if (sender.GetAutoAttackDamage(ally, true) >= ally.Health)
                                {
                                    R.Cast();
                                }
                            }
                            else
                            {
                                var target = (AIHeroClient)Args.Target;

                                if (target.GetSpellSlot(Args.SData.Name) != SpellSlot.Unknown)
                                {
                                    if (target.GetSpellDamage(Me, Args.SData.Name) > Me.Health)
                                    {
                                        if (Args.End.DistanceToPlayer() < 150 + ally.BoundingRadius)
                                        {
                                            R.Cast();
                                        }

                                        if (target.DistanceToPlayer() < 150 + ally.BoundingRadius)
                                        {
                                            R.Cast();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;
                case GameObjectType.obj_AI_Turret:
                    if (Args.Target.IsMe)
                    {
                        if (sender.IsMelee && Q.IsReady() && Menu.GetBool("QMelee"))
                        {
                            Q.Cast(Me.Position.Extend(sender.Position, -Q.Range));
                        }

                        if (R.IsReady() && Menu.GetBool("AutoR"))
                        {
                            if (sender.TotalAttackDamage > Me.Health)
                            {
                                R.Cast();
                            }
                        }
                    }
                    else if (Args.Target.IsAlly && Args.Target.Type == GameObjectType.AIHeroClient && !Args.Target.IsDead)
                    {
                        var ally = (AIHeroClient)Args.Target;

                        if (R.IsReady() && Menu.GetBool("AutoSave") && ally.DistanceToPlayer() <= R.Range)
                        {
                            if (sender.TotalAttackDamage > ally.Health)
                            {
                                R.Cast();
                            }
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
                    QLogic(target);
                }
            }
        }

        private void Combo()
        {
            var target = TargetSelector.GetSelectedTarget() ??
                         TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

            if (target.IsValidTarget(E.Range) && !target.IsDead && !target.IsZombie)
            {
                if (Menu.GetBool("ComboE") && E.IsReady() && target.IsValidTarget(E.Range))
                {
                    E.Cast(target);
                }

                if (Menu.GetBool("ComboW") && W.IsReady() && target.IsValidTarget(W.Range))
                {
                    W.Cast();
                }

                if (Menu.GetBool("ComboQ") && Q.IsReady())
                {
                    QLogic(target);
                }
            }
        }

        private void Harass()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("HarassMana")))
            {
                var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

                if (target.IsValidTarget(E.Range) && !target.IsDead && !target.IsZombie)
                {
                    if (Menu.GetBool("HarassE") && E.IsReady() && target.IsValidTarget(E.Range))
                    {
                        E.Cast(target);
                    }

                    if (Menu.GetBool("HarassW") && W.IsReady() && target.IsValidTarget(W.Range))
                    {
                        W.Cast();
                    }

                    if (Menu.GetBool("HarassQ") && Q.IsReady())
                    {
                        QLogic(target);
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
                        if (minions.Count >= Menu.GetSlider("LaneClearQCount"))
                        {
                            Q.Cast(Game.CursorPos);
                        }
                    }
                }
            }
        }

        private void JungleClear()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("JungleClearMana")) && ManaManager.SpellFarm)
            {
                var mobs = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth);

                if (mobs.Any())
                {
                    var mob = mobs.FirstOrDefault();

                    if (Menu.GetBool("JungleClearE") && E.IsReady())
                    {
                        E.Cast(mob, true);
                    }

                    if (Menu.GetBool("JungleClearW") && W.IsReady())
                    {
                        W.Cast(mob, true);
                    }

                    if (Menu.GetBool("JungleClearQ") && Q.IsReady())
                    {
                        Q.Cast(mob, true);
                    }
                }
            }
        }

        private void Flee()
        {
            Orbwalking.MoveTo(Game.CursorPos);

            if (Menu.GetBool("FleeQ") && Q.IsReady())
            {
                Q.Cast(Me.ServerPosition.Extend(Game.CursorPos, Q.Range), true);
            }
        }

        private void BeforeAttack(Orbwalking.BeforeAttackEventArgs Args)
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                var ForcusETarget =
                    HeroManager.Enemies.FirstOrDefault(
                        x =>
                            x.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Me)) &&
                            x.HasBuff("kindredecharge"));

                var ForcusTarget =
                    HeroManager.Enemies.FirstOrDefault(
                        x =>
                            x.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Me)) &&
                            x.HasBuff("kindredhittracker"));

                if (ForcusETarget.Check(Orbwalking.GetRealAutoAttackRange(Me)) &&
                    Menu.GetBool("ForcusE"))
                {
                    Orbwalker.ForceTarget(ForcusETarget);
                }
                else if (Menu.GetBool("Forcus") &&
                         ForcusTarget.Check(Orbwalking.GetRealAutoAttackRange(Me)))
                {
                    Orbwalker.ForceTarget(ForcusTarget);
                }
                else
                {
                    Orbwalker.ForceTarget(null);
                }
            }
        }

        private void AfterAttack(AttackableUnit sender, AttackableUnit t)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (Menu.GetBool("ComboAQA"))
                {
                    var target = t as AIHeroClient;

                    if (target != null && !target.IsDead && !target.IsZombie && Q.IsReady())
                    {
                        QLogic(target);
                    }
                }
            }
        }

        private void OnDraw(EventArgs Args)
        {
            if (!Me.IsDead && !Shop.IsOpen && !MenuGUI.IsChatOpen  )
            {
                if (Menu.GetBool("DrawW") && (W.IsReady() || Me.HasBuff("kindredwclonebuffvisible")))
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

        private void QLogic(Obj_AI_Base target)
        {
            if (!Q.IsReady())
            {
                return;
            }

            var qPosition = Me.ServerPosition.Extend(Game.CursorPos, Q.Range);
            var targetDisQ = target.ServerPosition.Distance(qPosition);
            var canQ = false;

            if (Menu.GetBool("QTurret") && qPosition.UnderTurret(true))
            {
                canQ = false;
            }

            if (Menu.GetBool("QCheck"))
            {
                if (qPosition.CountEnemiesInRange(300f) >= 3)
                {
                    canQ = false;
                }

                //Catilyn W
                if (ObjectManager
                        .Get<Obj_GeneralParticleEmitter>()
                        .FirstOrDefault(
                            x =>
                                x != null && x.IsValid &&
                                x.Name.ToLower().Contains("yordletrap_idle_red.troy") &&
                                x.Position.Distance(qPosition) <= 100) != null)
                {
                    canQ = false;
                }

                //Jinx E
                if (ObjectManager.Get<Obj_AI_Minion>()
                        .FirstOrDefault(x => x.IsValid && x.IsEnemy && x.Name == "k" &&
                                             x.Position.Distance(qPosition) <= 100) != null)
                {
                    canQ = false;
                }

                //Teemo R
                if (ObjectManager.Get<Obj_AI_Minion>()
                        .FirstOrDefault(x => x.IsValid && x.IsEnemy && x.Name == "Noxious Trap" &&
                                             x.Position.Distance(qPosition) <= 100) != null)
                {
                    canQ = false;
                }
            }

            if (targetDisQ >= Q.Range && targetDisQ <= Q.Range * 2)
            {
                canQ = true;
            }

            if (canQ)
            {
                Q.Cast(Game.CursorPos, true);
                canQ = false;
            }
        }
    }
}
