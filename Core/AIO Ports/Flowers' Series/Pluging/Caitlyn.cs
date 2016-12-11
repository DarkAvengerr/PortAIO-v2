using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_ADC_Series.Pluging
{
    using ADCCOMMON;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using Color = System.Drawing.Color;
    using Orbwalking = ADCCOMMON.Orbwalking;

    internal class Caitlyn : Logic
    {
        private int LastQTime;
        private int LastWTime;

        public Caitlyn()
        {
            Q = new Spell(SpellSlot.Q, 1250f);
            W = new Spell(SpellSlot.W, 800f);
            E = new Spell(SpellSlot.E, 750f);
            R = new Spell(SpellSlot.R, 2000f);

            Q.SetSkillshot(0.50f, 50f, 2000f, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 60f, 1600f, true, SkillshotType.SkillshotLine);

            var comboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                comboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                comboMenu.AddItem(
                    new MenuItem("ComboQCount", "Use Q| Min Hit Count >= x", true).SetValue(new Slider(2, 1, 5)));
                comboMenu.AddItem(
                    new MenuItem("ComboQRange", "Use Q| Min Range >= x", true).SetValue(new Slider(750, 500, 1000)));
                comboMenu.AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
                comboMenu.AddItem(
                    new MenuItem("ComboWCount", "Use W| Min Count >= x", true).SetValue(new Slider(1, 1, 3)));
                comboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboR", "Use R", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboRSafe", "Use R| Safe Check?", true).SetValue(true));
                comboMenu.AddItem(
                    new MenuItem("ComboRRange", "Use R| Min Range >= x", true).SetValue(new Slider(900, 500, 1500)));
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
                    jungleClearMenu.AddItem(
                        new MenuItem("JungleClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(30)));
                }

                clearMenu.AddItem(new MenuItem("asdqweqwe", " ", true));
                ManaManager.AddSpellFarm(clearMenu);
            }

            var fleeMenu = Menu.AddSubMenu(new Menu("Flee", "Flee"));
            {
                fleeMenu.AddItem(new MenuItem("FleeE", "Use E", true).SetValue(true));
                fleeMenu.AddItem(new MenuItem("FleeKey", "Flee Key", true).SetValue(new KeyBind('Z', KeyBindType.Press)));
            }

            var killStealMenu = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            {
                killStealMenu.AddItem(new MenuItem("KillStealQ", "Use Q", true).SetValue(true));
            }

            var miscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                var qSettings = miscMenu.AddSubMenu(new Menu("Q Settings", "Q Settings"));
                {
                    qSettings.AddItem(new MenuItem("AutoQ", "Auto Q?", true).SetValue(true));
                }

                var wSettings = miscMenu.AddSubMenu(new Menu("W Settings", "W Settings"));
                {
                    wSettings.AddItem(new MenuItem("AutoWCC", "Auto W|CC", true).SetValue(true));
                    wSettings.AddItem(new MenuItem("AutoWTP", "Auto W|Teleport", true).SetValue(true));
                }

                var eSettings = miscMenu.AddSubMenu(new Menu("E Settings", "E Settings"));
                {
                    var interruptMenu = eSettings.AddSubMenu(new Menu("Interrupt Settings", "Interrupt Settings"));
                    {
                        interruptMenu.AddItem(new MenuItem("Interrupt", "Interrupt Danger Spells", true).SetValue(true));
                        interruptMenu.AddItem(new MenuItem("AntiAlistar", "Interrupt Alistar W", true).SetValue(true));
                        interruptMenu.AddItem(new MenuItem("AntiRengar", "Interrupt Rengar Jump", true).SetValue(true));
                        interruptMenu.AddItem(new MenuItem("AntiKhazix", "Interrupt Khazix R", true).SetValue(true));
                    }

                    var antigapcloserMenu =
                        eSettings.AddSubMenu(new Menu("AntiGapcloser Settings", "AntiGapcloser Settings"));
                    {
                        antigapcloserMenu.AddItem(new MenuItem("Gapcloser", "Anti Gapcloser", true).SetValue(false));
                        foreach (var target in HeroManager.Enemies)
                        {
                            antigapcloserMenu.AddItem(
                                new MenuItem("AntiGapcloser" + target.ChampionName.ToLower(), target.ChampionName, true)
                                    .SetValue(false));
                        }
                    }
                }

                var rSettings = miscMenu.AddSubMenu(new Menu("R Settings", "R Settings"));
                {
                    rSettings.AddItem(
                        new MenuItem("SemiR", "Semi-manual R Key", true).SetValue(new KeyBind('T', KeyBindType.Press)));
                }

                miscMenu.AddItem(
                    new MenuItem("EQKey", "One Key EQ target", true).SetValue(new KeyBind('G', KeyBindType.Press)));
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
                drawMenu.AddItem(new MenuItem("DrawRMin", "Draw R Range(MinMap)", true).SetValue(false));
                ManaManager.AddDrawFarm(drawMenu);
                DamageIndicator.AddToMenu(drawMenu);
            }

            GameObject.OnCreate += OnCreate;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Drawing.OnEndScene += OnEndScene;
        }

        private void OnCreate(GameObject sender, EventArgs Args)
        {
            var Rengar = HeroManager.Enemies.Find(heros => heros.ChampionName.Equals("Rengar"));
            var Khazix = HeroManager.Enemies.Find(heros => heros.ChampionName.Equals("Khazix"));

            if (Rengar != null && Menu.GetBool("AntiRengar"))
            {
                if (sender.Name == "Rengar_LeapSound.troy" && sender.Position.Distance(Me.Position) < E.Range)
                {
                    E.Cast(Rengar.Position, true);
                }
            }

            if (Khazix != null && Menu.GetBool("AntiKhazix"))
            {
                if (sender.Name == "Khazix_Base_E_Tar.troy" && sender.Position.Distance(Me.Position) <= 300)
                {
                    E.Cast(Khazix.Position, true);
                }
            }
        }

        private void OnEnemyGapcloser(ActiveGapcloser Args)
        {
            if (E.IsReady())
            {
                if (Menu.GetBool("AntiAlistar") && Args.Sender.ChampionName == "Alistar" && Args.SkillType == GapcloserType.Targeted)
                {
                    E.Cast(Args.Sender.Position, true);
                }

                if (Menu.GetBool("Gapcloser") && Menu.GetBool("AntiGapcloser" + Args.Sender.ChampionName.ToLower()))
                {
                    if (Args.Sender.DistanceToPlayer() <= 200 && Args.Sender.IsValid)
                    {
                        E.Cast(Args.Sender.Position, true);
                    }
                }
            }
        }

        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if (Args.Slot == SpellSlot.Q)
            {
                LastQTime = Utils.TickCount;
            }

            if (Args.Slot == SpellSlot.W)
            {
                LastWTime = Utils.TickCount;
            }
        }

        private void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead || Me.IsRecalling())
            {
                return;
            }

            R.Range = 500 * R.Level + 1500;

            if (Menu.GetKey("EQKey"))
            {
                OneKeyEQ();
            }

            if (Menu.GetKey("SemiR") && R.IsReady())
            {
                OneKeyCastR();
            }

            if (Menu.GetKey("FleeKey"))
            {
                Flee();
            }

            Auto();
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

        private void OneKeyCastR()
        {
            var select = TargetSelector.GetSelectedTarget();
            var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

            if (select != null && target.IsValidTarget(R.Range))
            {
                R.CastOnUnit(select);
            }
            else if (select == null && target != null && target.IsValidTarget(R.Range))
            {
                R.CastOnUnit(target);
            }
        }

        private void Auto()
        {
            if (Menu.GetBool("AutoQ") && Q.IsReady() && !Orbwalking.isCombo && !Orbwalking.isHarass)
            {
                var target = TargetSelector.GetTarget(Q.Range - 30, TargetSelector.DamageType.Physical);

                if (target.Check(Q.Range) && !target.CanMoveMent())
                {
                    SpellManager.PredCast(Q, target);
                }
            }

            if (W.IsReady())
            {
                if (Menu.GetBool("AutoWCC"))
                {
                    foreach (
                        var target in
                        HeroManager.Enemies.Where(
                            x => x.IsValidTarget(W.Range) && !x.CanMoveMent() && !x.HasBuff("caitlynyordletrapinternal")))
                    {
                        if (Utils.TickCount - LastWTime > 1500)
                        {
                            W.Cast(target.Position, true);
                        }
                    }
                }

                if (Menu.GetBool("AutoWTP"))
                {
                    var obj =
                        ObjectManager
                            .Get<Obj_AI_Base>()
                            .FirstOrDefault(x => !x.IsAlly && !x.IsMe && x.DistanceToPlayer() <= W.Range &&
                                                 x.Buffs.Any(
                                                     a =>
                                                         a.Name.ToLower().Contains("teleport") || // tp
                                                         a.Name.ToLower().Contains("gate")) && // tf r
                                                 !ObjectManager.Get<Obj_AI_Base>()
                                                     .Any(b => b.Name.ToLower().Contains("trap") && b.Distance(x) <= 150));

                    if (obj != null)
                    {
                        if (Utils.TickCount - LastWTime > 1500)
                        {
                            W.Cast(obj.Position, true);
                        }
                    }
                }
            }
        }

        private void KillSteal()
        {
            if (Menu.GetBool("KillStealQ") && Q.IsReady())
            {
                foreach (
                    var target in
                    HeroManager.Enemies.Where(
                        x => x.Check(Q.Range) && x.Health < Q.GetDamage(x)))
                {
                    if (Orbwalking.InAutoAttackRange(target) && target.Health <= Me.GetAutoAttackDamage(target, true))
                    {
                        continue;
                    }

                    SpellManager.PredCast(Q, target);
                }
            }
        }

        private void Combo()
        {
            var target = TargetSelector.GetSelectedTarget() ??
                         TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

            if (target.Check(R.Range))
            {
                if (Menu.GetBool("ComboE") && E.IsReady() && target.IsValidTarget(700))
                {
                    var ePred = E.GetPrediction(target);

                    if (ePred.CollisionObjects.Count == 0 || ePred.Hitchance >= HitChance.VeryHigh)
                    {
                        if (Menu.GetBool("ComboQ") && Q.IsReady())
                        {
                            if (E.Cast(target).IsCasted())
                            {
                                Q.Cast(target);
                            }
                        }
                        else
                        {
                            E.Cast(target);
                        }
                    }
                    else
                    {
                        if (Menu.GetBool("ComboQ") && Q.IsReady() && target.IsValidTarget(Q.Range))
                        {
                            if (target.DistanceToPlayer() >= Menu.GetSlider("ComboQRange"))
                            {
                                SpellManager.PredCast(Q, target);

                                if (Me.CountEnemiesInRange(Q.Range) >= Menu.GetSlider("ComboQCount"))
                                {
                                    Q.CastIfWillHit(target, Menu.GetSlider("ComboQCount"), true);
                                }
                            }
                        }
                    }
                }

                if (Menu.GetBool("ComboQ") && Q.IsReady() && !E.IsReady() && target.IsValidTarget(Q.Range) &&
                    target.DistanceToPlayer() >= Menu.GetSlider("ComboQRange"))
                {
                    if (target.DistanceToPlayer() >= Menu.GetSlider("ComboQRange"))
                    {
                        SpellManager.PredCast(Q, target);

                        if (Me.CountEnemiesInRange(Q.Range) >= Menu.GetSlider("ComboQCount"))
                        {
                            Q.CastIfWillHit(target, Menu.GetSlider("ComboQCount"), true);
                        }
                    }
                }

                if (Menu.GetBool("ComboW") && W.IsReady() && target.IsValidTarget(W.Range) && 
                    W.Instance.Ammo >= Menu.GetSlider("ComboWCount"))
                {
                    if (Utils.TickCount - LastWTime > 1500)
                    {
                        if (target.IsFacing(Me))
                        {
                            if (target.IsMelee && target.DistanceToPlayer() < target.AttackRange + 100)
                            {
                                W.Cast(Me.Position);
                            }
                            else
                            {
                                var wPred = W.GetPrediction(target);

                                if (wPred.Hitchance >= HitChance.VeryHigh && target.IsValidTarget(W.Range))
                                {
                                    W.Cast(wPred.CastPosition);
                                }
                            }
                        }
                        else
                        {
                            var wPred = W.GetPrediction(target);

                            if (wPred.Hitchance >= HitChance.VeryHigh && target.IsValidTarget(W.Range))
                            {
                                W.Cast(wPred.CastPosition +
                                       Vector3.Normalize(target.ServerPosition - Me.ServerPosition)*100);
                            }
                        }
                    }
                }

                if (Menu.GetBool("ComboR") && R.IsReady() && Utils.TickCount - LastQTime > 2500)
                {
                    if (Menu.GetBool("ComboRSafe") && (Me.UnderTurret(true) || Me.CountEnemiesInRange(1000) > 2))
                    {
                        return;
                    }

                    if (!target.IsValidTarget(R.Range))
                    {
                        return;
                    }

                    if (target.DistanceToPlayer() < Menu.GetSlider("ComboRRange"))
                    {
                        return;
                    }

                    if (target.Health + target.HPRegenRate * 3 > R.GetDamage(target))
                    {
                        return;
                    }

                    var RCollision =
                        LeagueSharp.Common.Collision
                            .GetCollision(new List<Vector3> {target.ServerPosition},
                                new PredictionInput
                                {
                                    Delay = R.Delay,
                                    Radius = R.Width,
                                    Speed = R.Speed,
                                    Unit = Me,
                                    UseBoundingRadius = true,
                                    Collision = true,
                                    CollisionObjects = new[] {CollisionableObjects.Heroes, CollisionableObjects.YasuoWall}
                                })
                            .Any(x => x.NetworkId != target.NetworkId);

                    if (RCollision)
                    {
                        return;
                    }

                    R.CastOnUnit(target, true);
                }
            }
        }

        private void Harass()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("HarassMana")))
            {
                if (Menu.GetBool("HarassQ") && Q.IsReady())
                {
                    var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);

                    if (target.Check(Q.Range))
                    {
                        SpellManager.PredCast(Q, target);
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
                            MinionManager.GetBestLineFarmLocation(minions.Select(x => x.Position.To2D()).ToList(),
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
                        Q.Cast(mobs.FirstOrDefault(), true);
                    }
                }
            }
        }

        private void Flee()
        {
            Orbwalking.MoveTo(Game.CursorPos);

            if (Menu.GetBool("FleeE") && E.IsReady())
            {
                E.Cast(Me.Position - (Game.CursorPos - Me.Position));
            }
        }

        private void OneKeyEQ()
        {
            Orbwalking.MoveTo(Game.CursorPos);

            if (E.IsReady() && Q.IsReady())
            {
                var target = TargetSelector.GetSelectedTarget() ??
                    TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

                if (target.Check(E.Range))
                {
                    if (E.GetPrediction(target).CollisionObjects.Count == 0 && E.CanCast(target))
                    {
                        E.Cast(target);
                        SpellManager.PredCast(Q, target);
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
