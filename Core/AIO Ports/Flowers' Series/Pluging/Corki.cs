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
    

    internal class Corki : Logic
    {
        public Corki()
        {
            Q = new Spell(SpellSlot.Q, 825f);
            W = new Spell(SpellSlot.W, 800f);
            E = new Spell(SpellSlot.E, 600f);
            R = new Spell(SpellSlot.R, 1300f);

            Q.SetSkillshot(0.3f, 200f, 1000f, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.1f, (float)(45 * Math.PI / 180), 1500f, false, SkillshotType.SkillshotCone);
            R.SetSkillshot(0.2f, 40f, 2000f, true, SkillshotType.SkillshotLine);

            var comboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                comboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboR", "Use R", true).SetValue(true));
                comboMenu.AddItem(
                    new MenuItem("ComboRLimit", "Use R|Limit Stack >= x", true).SetValue(new Slider(0, 0, 7)));
            }

            var harassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                harassMenu.AddItem(new MenuItem("HarassQ", "Use Q", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassE", "Use E", true).SetValue(false));
                harassMenu.AddItem(new MenuItem("HarassR", "Use R", true).SetValue(true));
                harassMenu.AddItem(
                    new MenuItem("HarassRLimit", "Use R|Limit Stack >= x", true).SetValue(new Slider(4, 0, 7)));
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
                    laneClearMenu.AddItem(new MenuItem("LaneClearR", "Use R", true).SetValue(true));
                    laneClearMenu.AddItem(
                        new MenuItem("LaneClearRLimit", "Use R|Limit Stack >= x", true).SetValue(new Slider(4, 0, 7)));
                    laneClearMenu.AddItem(
                        new MenuItem("LaneClearRCount", "If R CanHit Counts >= x", true).SetValue(new Slider(3, 1, 5)));
                    laneClearMenu.AddItem(
                        new MenuItem("LaneClearMana", "If Player ManaPercent >= %", true).SetValue(new Slider(60)));
                }

                var jungleClearMenu = clearMenu.AddSubMenu(new Menu("JungleClear", "JungleClear"));
                {
                    jungleClearMenu.AddItem(new MenuItem("JungleClearQ", "Use Q", true).SetValue(true));
                    jungleClearMenu.AddItem(new MenuItem("JungleClearE", "Use E", true).SetValue(true));
                    jungleClearMenu.AddItem(new MenuItem("JungleClearR", "Use R", true).SetValue(true));
                    jungleClearMenu.AddItem(
                        new MenuItem("JungleClearRLimit", "Use R|Limit Stack >= x", true).SetValue(new Slider(0, 0, 7)));
                    jungleClearMenu.AddItem(
                        new MenuItem("JungleClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(30)));
                }

                clearMenu.AddItem(new MenuItem("asdqweqwe", " ", true));
                ManaManager.AddSpellFarm(clearMenu);
            }

            var killStealMenu = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            {
                killStealMenu.AddItem(new MenuItem("KillStealQ", "Use Q", true).SetValue(true));
                killStealMenu.AddItem(new MenuItem("KillStealR", "Use R", true).SetValue(true));
            }

            var fleeMenu = Menu.AddSubMenu(new Menu("Flee", "Flee"));
            {
                fleeMenu.AddItem(new MenuItem("FleeW", "Use W", true).SetValue(true));
                fleeMenu.AddItem(new MenuItem("FleeKey", "Flee Key", true).SetValue(new KeyBind('Z', KeyBindType.Press)));
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
                drawMenu.AddItem(new MenuItem("DrawW", "Draw W Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawE", "Draw E Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawR", "Draw R Range", true).SetValue(false));
                ManaManager.AddDrawFarm(drawMenu);
                //DamageIndicator.AddToMenu(drawMenu);
            }

            Obj_AI_Base.OnSpellCast += OnSpellCast;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
        }

        private void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (sender.IsMe && Orbwalking.IsAutoAttack(Args.SData.Name))
            {
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                {
                    var target = Args.Target as AIHeroClient;

                    if (target != null)
                    {
                        if (Menu.GetBool("ComboR") && R.IsReady() && R.Instance.Ammo >= Menu.GetSlider("ComboRLimit"))
                        {
                            SpellManager.PredCast(R, target, true);
                        }
                        else if (Menu.GetBool("ComboQ") && Q.IsReady())
                        {
                            SpellManager.PredCast(Q, target, true);
                        }
                        else if (Menu.GetBool("ComboE") && E.IsReady())
                        {
                            E.Cast();
                        }
                    }
                }

                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                {
                    if (ManaManager.HasEnoughMana(Menu.GetSlider("JungleClearMana")) && ManaManager.SpellFarm)
                    {
                        var mobs = MinionManager.GetMinions(R.Range, MinionTypes.All, MinionTeam.Neutral,
                            MinionOrderTypes.MaxHealth);

                        if (mobs.Any())
                        {
                            var mob = mobs.FirstOrDefault();

                            if (Menu.GetBool("JungleClearR") && R.IsReady() && R.Instance.Ammo >= Menu.GetSlider("JungleClearRLimit"))
                            {
                                R.Cast(mob, true);
                            }
                            else if (Menu.GetBool("JungleClearQ") && Q.IsReady())
                            {
                                Q.Cast(mob, true);
                            }
                            else if (Menu.GetBool("JungleClearE") && E.IsReady())
                            {
                                E.Cast();
                            }
                        }
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

            if (R.Level > 0)
            {
                R.Range = Me.HasBuff("CorkiMissileBarrageCounterBig") ? 1500f : 1300f;
            }

            if (Menu.GetKey("FleeKey"))
            {
                Flee();
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
                    SpellManager.PredCast(R, target, true);
                }
            }
        }

        private void AutoHarass()
        {
            if (Me.UnderTurret(true))
            {
                return;
            }

            if (Menu.GetKey("AutoHarass") && Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo && Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Mixed && !Me.IsRecalling())
            {
                Harass();
            }
        }

        private void KillSteal()
        {
            if (Menu.GetBool("KillStealQ") && Q.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range) && x.Health < Q.GetDamage(x)))
                {
                    SpellManager.PredCast(R, target, true);
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
            var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

            if (target.Check(R.Range))
            {
                if (Menu.GetBool("ComboR") && R.IsReady() && R.Instance.Ammo >= Menu.GetSlider("ComboRLimit") &&
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
                    E.Cast();
                }
            }
        }

        private void Harass()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("HarassMana")))
            {
                var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

                if (target.Check(R.Range))
                {
                    if (Menu.GetBool("HarassR") && R.IsReady() && R.Instance.Ammo >= Menu.GetSlider("HarassRLimit") &&
                        target.IsValidTarget(R.Range))
                    {
                        SpellManager.PredCast(R, target, true);
                    }

                    if (Menu.GetBool("HarassQ") && Q.IsReady() && target.IsValidTarget(Q.Range))
                    {
                        SpellManager.PredCast(Q, target, true);
                    }

                    if (Menu.GetBool("HarassE") && E.IsReady() && target.IsValidTarget(E.Range))
                    {
                        E.Cast();
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
                        var QFram =
                            MinionManager.GetBestCircularFarmLocation(minions.Select(x => x.Position.To2D()).ToList(),
                                Q.Width, Q.Range);

                        if (QFram.MinionsHit >= Menu.GetSlider("LaneClearQCount"))
                        {
                            Q.Cast(QFram.Position, true);
                        }
                    }
                }

                if (Menu.GetBool("LaneClearE") && E.IsReady())
                {
                    var eMinions = MinionManager.GetMinions(Me.Position, E.Range);

                    if (eMinions.Any())
                    {
                        if (eMinions.Count >= Menu.GetSlider("LaneClearECount"))
                        {
                            E.Cast();
                        }
                    }
                }

                if (Menu.GetBool("LaneClearR") && R.IsReady() && R.Instance.Ammo >= Menu.GetSlider("LaneClearRLimit"))
                {
                    var rMinions = MinionManager.GetMinions(Me.Position, R.Range);

                    if (rMinions.Any())
                    {
                        var RFarm =
                            MinionManager.GetBestLineFarmLocation(rMinions.Select(x => x.Position.To2D()).ToList(),
                                R.Width, R.Range);

                        if (RFarm.MinionsHit >= Menu.GetSlider("LaneClearRCount"))
                        {
                            R.Cast(RFarm.Position, true);
                        }
                    }
                }
            }
        }

        private void JungleClear()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("JungleClearMana")) && ManaManager.SpellFarm)
            {
                var mobs = MinionManager.GetMinions(R.Range, MinionTypes.All, MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth);

                if (mobs.Any())
                {
                    var mob = mobs.FirstOrDefault();

                    if (Menu.GetBool("JungleClearR") && R.IsReady() && R.Instance.Ammo >= Menu.GetSlider("JungleClearRLimit"))
                    {
                        R.Cast(mob, true);
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

            if (Menu.GetBool("FleeW") && W.IsReady())
            {
                W.Cast(Me.Position.Extend(Game.CursorPos, W.Range));
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
    }
}
