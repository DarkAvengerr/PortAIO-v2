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
    

    internal class Twitch : Logic
    {
        private bool PlayerIsKillTarget;

        public Twitch()
        {
            Q = new Spell(SpellSlot.Q, 0f);
            W = new Spell(SpellSlot.W, 950f);
            E = new Spell(SpellSlot.E, 1200f);
            R = new Spell(SpellSlot.R, 975f);

            W.SetSkillshot(0.25f, 100f, 1410f, false, SkillshotType.SkillshotCircle);

            var comboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                comboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                comboMenu.AddItem(
                    new MenuItem("ComboQRange", "Use Q | Search Enemy Range", true).SetValue(new Slider(900, 0, 1800)));
                comboMenu.AddItem(
                    new MenuItem("ComboQCount", "Use Q | Count Enemies >= x", true).SetValue(new Slider(3, 1, 5)));
                comboMenu.AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboEStack", "Use E| target will leave e range", true).SetValue(true));
                comboMenu.AddItem(
                    new MenuItem("ComboEStackCount", "Use E | Min E Stack Count(will Leave E Range Auto E)", true)
                        .SetValue(new Slider(3, 1, 6)));
                comboMenu.AddItem(new MenuItem("ComboEFull", "Use E | If enemy full stack", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboEKill", "Use E | If enemy can kill", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboR", "Use R", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboRYouMuu", "Use R| Auto Youmuu?", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboRKillSteal", "Use R| KillSteal", true).SetValue(true));
                comboMenu.AddItem(
                    new MenuItem("ComboRCount", "Use R | If enemies in counts >= x", true).SetValue(new Slider(3, 1, 5)));
            }

            var harassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                harassMenu.AddItem(new MenuItem("HarassW", "Use W", true).SetValue(false));
                harassMenu.AddItem(new MenuItem("HarassE", "Use E", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassEStack", "Min E Stack Count", true).SetValue(new Slider(2, 1, 6)));
                harassMenu.AddItem(new MenuItem("HarassMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var clearMenu = Menu.AddSubMenu(new Menu("Clear", "Clear"));
            {
                var laneClearMenu = clearMenu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
                {
                    laneClearMenu.AddItem(new MenuItem("LaneClearE", "Use E", true).SetValue(true));
                    laneClearMenu.AddItem(
                        new MenuItem("LaneClearECount", "If E CanHit Counts >= x", true).SetValue(new Slider(3, 1, 5)));
                    laneClearMenu.AddItem(
                        new MenuItem("LaneClearMana", "If Player ManaPercent >= %", true).SetValue(new Slider(60)));
                }

                var jungleClearMenu = clearMenu.AddSubMenu(new Menu("JungleClear", "JungleClear"));
                {
                    jungleClearMenu.AddItem(new MenuItem("JungleStealE", "Use E", true).SetValue(true));
                }

                clearMenu.AddItem(new MenuItem("asdqweqwe", " ", true));
                ManaManager.AddSpellFarm(clearMenu);
            }

            var killStealMenu = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            {
                killStealMenu.AddItem(new MenuItem("KillStealW", "Use W", true).SetValue(true));
                killStealMenu.AddItem(new MenuItem("KillStealE", "Use E", true).SetValue(true));
            }

            var miscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                miscMenu.AddItem(new MenuItem("AutoQ", "Auto Q?", true).SetValue(true));
            }

            var utilityMenu = Menu.AddSubMenu(new Menu("Utility", "Utility"));
            {
                var skinMenu = utilityMenu.AddSubMenu(new Menu("Skin Change", "Skin Change"));
                {
                    SkinManager.AddToMenu(skinMenu, 7);
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
                drawMenu.AddItem(new MenuItem("DrawW", "Draw W Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawE", "Draw E Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawR", "Draw R Range", true).SetValue(false));
                ManaManager.AddDrawFarm(drawMenu);
                //DamageIndicator.AddToMenu(drawMenu, GetRealEDamage);
            }

            Obj_AI_Base.OnSpellCast += OnSpellCast;
            Game.OnUpdate += OnUpdate;
            Game.OnNotify += OnNotify;
            Drawing.OnDraw += OnDraw;
        }

        private void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (Menu.GetBool("ComboRYouMuu") && Orbwalker.GetTarget() != null &&
                Orbwalker.GetTarget() is AIHeroClient && Me.HasBuff("TwitchFullAutomatic"))
            {
                if (Items.HasItem(3142))
                {
                    Items.UseItem(3142);
                }
            }
        }

        private void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead || Me.IsRecalling())
            {
                return;
            }

            if (Menu.GetBool("AutoQ"))
            {
                if (PlayerIsKillTarget && Q.IsReady() && Me.CountEnemiesInRange(1000) >= 1)
                {
                    Q.Cast();
                }
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
            if (Menu.GetBool("KillStealE") && E.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(E.Range) && x.Health < E.GetDamage(x) - 5))
                {
                    if (target.Check(E.Range))
                    {
                        E.Cast();
                    }
                }
            }

            if (Menu.GetBool("KillStealW") && W.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(W.Range) && x.Health < W.GetDamage(x)))
                {
                    if (target.Check(W.Range) && !Orbwalking.InAutoAttackRange(target))
                    {
                        SpellManager.PredCast(W, target, true);
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
                if (Menu.GetBool("ComboR") && R.IsReady())
                {
                    if (Menu.GetBool("ComboRKillSteal") &&
                        HeroManager.Enemies.Count(x => x.DistanceToPlayer() <= R.Range) <= 2 &&
                        target.Health <= Me.GetAutoAttackDamage(target, true) * 4 + GetRealEDamage(target)*2)
                    {
                        R.Cast();
                    }

                    if (HeroManager.Enemies
                            .Count(x => x.DistanceToPlayer() <= R.Range) >= Menu.GetSlider("ComboRCount"))
                    {
                        R.Cast();
                    }
                }

                if (Menu.GetBool("ComboQ") && Q.IsReady() &&
                    HeroManager.Enemies.Count(
                        x => x.DistanceToPlayer() <= Menu.GetSlider("ComboQRange")) >= Menu.GetSlider("ComboQCount"))
                {
                    Q.Cast();
                }

                if (Menu.GetBool("ComboW") && W.IsReady() && target.IsValidTarget(W.Range) &&
                    target.Health > W.GetDamage(target) && GetEStackCount(target) < 6 &&
                    Me.Mana >= Q.Instance.SData.Mana + W.Instance.SData.Mana + E.Instance.SData.Mana + R.Instance.SData.Mana)
                {
                    SpellManager.PredCast(W, target, true);
                }

                if (Menu.GetBool("ComboE") && E.IsReady() && target.IsValidTarget(E.Range) &&
                    target.HasBuff("TwitchDeadlyVenom"))
                {
                    if (Menu.GetBool("ComboEStack") && target.DistanceToPlayer() >= E.Range * 0.80 &&
                        target.IsValidTarget(E.Range) && GetEStackCount(target) >= Menu.GetSlider("ComboEStackCount"))
                    {
                        E.Cast();
                    }

                    if (Menu.GetBool("ComboEFull") && GetEStackCount(target) >= 6)
                    {
                        E.Cast();
                    }

                    if (Menu.GetBool("ComboEKill") && target.Health <= E.GetDamage(target) && target.IsValidTarget(E.Range))
                    {
                        E.Cast();
                    }
                }
            }
        }

        private void Harass()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("HarassMana")))
            {
                if (Menu.GetBool("HarassW") && W.IsReady())
                {
                    var wTarget = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);

                    if (wTarget.Check(W.Range))
                    {
                        SpellManager.PredCast(W, wTarget, true);
                    }
                }

                if (Menu.GetBool("HarassE") && E.IsReady())
                {
                    var eTarget = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

                    if (eTarget.Check(E.Range))
                    {
                        if (eTarget.DistanceToPlayer() > E.Range * 0.8 && eTarget.IsValidTarget(E.Range) && 
                            GetEStackCount(eTarget) >= Menu.GetSlider("HarassEStack"))
                        {
                            E.Cast();
                        }
                        else if (GetEStackCount(eTarget) >= 6)
                        {
                            E.Cast();
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
                if (Menu.GetBool("LaneClearE") && E.IsReady())
                {
                    var eKillMinionsCount =
                        MinionManager.GetMinions(Me.Position, E.Range)
                            .Count(
                                x =>
                                    x.DistanceToPlayer() <= E.Range && x.HasBuff("TwitchDeadlyVenom") &&
                                    x.Health < E.GetDamage(x));

                    if (eKillMinionsCount >= Menu.GetSlider("LaneClearECount"))
                    {
                        E.Cast();
                    }
                }
            }
        }

        private void JungleClear()
        {
            if (ManaManager.SpellFarm)
            {
                if (Menu.GetBool("JungleStealE") && E.IsReady())
                {
                    var mobs = MinionManager.GetMinions(Me.Position, E.Range, MinionTypes.All, MinionTeam.Neutral,
                        MinionOrderTypes.MaxHealth);

                    foreach (
                        var mob in
                        mobs.Where(
                            x =>
                                !x.Name.ToLower().Contains("mini") && x.DistanceToPlayer() <= E.Range &&
                                x.HasBuff("TwitchDeadlyVenom")))
                    {
                        if (mob.Health < E.GetDamage(mob))
                        {
                            E.Cast();
                        }
                    }
                }
            }
        }

        private void OnNotify(GameNotifyEventArgs Args)
        {
            if (Me.IsDead)
            {
                PlayerIsKillTarget = false;
            }
            else if (!Me.IsDead)
            {
                if (Args.EventId == GameEventId.OnChampionDie && Args.NetworkId == Me.NetworkId)
                {
                    PlayerIsKillTarget = true;

                    LeagueSharp.Common.Utility.DelayAction.Add(5000, () =>
                    {
                        PlayerIsKillTarget = false;
                    });
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

        private static int GetEStackCount(Obj_AI_Base target)
        {
            return target.HasBuff("TwitchDeadlyVenom") ? target.GetBuffCount("TwitchDeadlyVenom") : 0;
        }

        private float GetRealEDamage(Obj_AI_Base target)
        {
            if (target != null && !target.IsDead && !target.IsZombie && target.HasBuff("TwitchDeadlyVenom"))
            {
                if (target.HasBuff("KindredRNoDeathBuff"))
                {
                    return 0;
                }

                if (target.HasBuff("UndyingRage") && target.GetBuff("UndyingRage").EndTime - Game.Time > 0.3)
                {
                    return 0;
                }

                if (target.HasBuff("JudicatorIntervention"))
                {
                    return 0;
                }

                if (target.HasBuff("ChronoShift") && target.GetBuff("ChronoShift").EndTime - Game.Time > 0.3)
                {
                    return 0;
                }

                if (target.HasBuff("FioraW"))
                {
                    return 0;
                }

                if (target.HasBuff("ShroudofDarkness"))
                {
                    return 0;
                }

                if (target.HasBuff("SivirShield"))
                {
                    return 0;
                }

                var damage = 0f;

                damage += E.IsReady() ? E.GetDamage(target) : 0f;

                if (target.BaseSkinName == "Moredkaiser")
                {
                    damage -= target.Mana;
                }

                if (Me.HasBuff("SummonerExhaust"))
                {
                    damage = damage * 0.6f;
                }

                if (target.HasBuff("BlitzcrankManaBarrierCD") && target.HasBuff("ManaBarrier"))
                {
                    damage -= target.Mana / 2f;
                }

                if (target.HasBuff("GarenW"))
                {
                    damage = damage * 0.7f;
                }

                if (target.HasBuff("ferocioushowl"))
                {
                    damage = damage * 0.7f;
                }

                return damage;
            }

            return 0f;
        }
    }
}
