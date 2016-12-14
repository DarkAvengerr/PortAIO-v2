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
    

    internal class Sivir : Logic
    {
        public Sivir()
        {
            Q = new Spell(SpellSlot.Q, 1200f);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R);

            Q.SetSkillshot(0.25f, 90f, 1350f, false, SkillshotType.SkillshotLine);

            var comboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                comboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboR", "Use R", true).SetValue(true));
                comboMenu.AddItem(
                    new MenuItem("ComboRCount", "Use R| When Enemies Counts >= x", true).SetValue(new Slider(3, 1, 5)));
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
                    laneClearMenu.AddItem(new MenuItem("LaneClearW", "Use W", true).SetValue(true));
                    laneClearMenu.AddItem(new MenuItem("LaneClearWTurret", "Use W| Attack Turret", true).SetValue(true));
                    laneClearMenu.AddItem(
                        new MenuItem("LaneClearMana", "If Player ManaPercent >= %", true).SetValue(new Slider(60)));
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
                killStealMenu.AddItem(new MenuItem("KillStealQ", "Use Q", true).SetValue(true));
            }

            var miscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                var qMenu = miscMenu.AddSubMenu(new Menu("Q Settings", "Q Settings"));
                {
                    qMenu.AddItem(new MenuItem("AutoQ", "Auto Q?", true).SetValue(true));
                }

                var eMenu = miscMenu.AddSubMenu(new Menu("E Settings", "E Settings"));
                {
                    eMenu.AddItem(new MenuItem("AutoE", "Auto E?", true).SetValue(true));
                    eMenu.AddItem(
                        new MenuItem("AutoEHp", "Auto E| When Player HealthPercent <= x%", true).SetValue(new Slider(80)));
                }

                var rMenu = miscMenu.AddSubMenu(new Menu("R Settings", "R Settings"));
                {
                    rMenu.AddItem(new MenuItem("AutoR", "Auto R?", true).SetValue(false));
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
                ManaManager.AddDrawFarm(drawMenu);
                DamageIndicator.AddToMenu(drawMenu);
            }

            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnSpellCast += OnSpellCast;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Drawing.OnDraw += OnDraw;
        }

        private void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead || Me.IsRecalling())
            {
                return;
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

        private void Auto()
        {
            if (Me.UnderTurret(true))
            {
                return;
            }

            if (Menu.GetBool("AutoQ") && Q.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range) && !x.CanMoveMent()))
                {
                    if (target.Check(Q.Range))
                    {
                        SpellManager.PredCast(Q, target, true);
                    }
                }
            }

            if (Menu.GetBool("AutoR") && R.IsReady() && Me.CountEnemiesInRange(850) >= 3)
            {
                R.Cast();
            }
        }

        private void KillSteal()
        {
            if (Menu.GetBool("KillStealQ") && Q.IsReady())
            {
                foreach (
                    var target in
                    HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range) && x.Health < Q.GetDamage(x)))
                {
                    if (target.Check(Q.Range))
                    {
                        SpellManager.PredCast(Q, target, true);
                    }
                }
            }
        }

        private void Combo()
        {
            var target = TargetSelector.GetSelectedTarget() ??
                TargetSelector.GetTarget(1500f, TargetSelector.DamageType.Physical);

            if (target.Check(1500f))
            {
                if (Menu.GetBool("ComboQ") && Q.IsReady() && target.IsValidTarget(Q.Range) &&
                    !Me.IsDashing())
                {
                    SpellManager.PredCast(Q, target, true);
                }

                if (Menu.GetBool("ComboR") && Me.CountEnemiesInRange(850) >= Menu.GetSlider("ComboRCount") &&
                    ((target.Health <= Me.GetAutoAttackDamage(target)*3 && !Q.IsReady()) ||
                     (target.Health <= Me.GetAutoAttackDamage(target)*3 + Q.GetDamage(target))))
                {
                    R.Cast();
                }
            }
        }

        private void Harass()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("HarassMana")) && ManaManager.SpellFarm)
            {
                var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

                if (Menu.GetBool("HarassQ") && Q.IsReady() && target.Check(Q.Range))
                {
                    SpellManager.PredCast(Q, target, true);
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
                    var mobs =
                        MinionManager.GetMinions(Me.Position, Q.Range, MinionTypes.All, MinionTeam.Neutral,
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
            if (!sender.IsMe || !Orbwalking.IsAutoAttack(Args.SData.Name))
            {
                return;
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                var hero = Args.Target as AIHeroClient;

                if (hero != null && Menu.GetBool("ComboW") && W.IsReady())
                {
                    var WTarget = hero;

                    if (WTarget.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Me)) && Me.CanAttack)
                    {
                        W.Cast();
                        Orbwalking.ResetAutoAttackTimer();
                    }
                }
            }
            else if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                if (Args.Target is Obj_AI_Minion)
                {
                    LaneClearW();
                    JungleClearW();
                }
                else if (Args.Target is Obj_AI_Turret || Args.Target.Type == GameObjectType.obj_AI_Turret)
                {
                    if (ManaManager.HasEnoughMana(Menu.GetSlider("LaneClearMana")) && ManaManager.SpellFarm)
                    {
                        if (Menu.GetBool("LaneClearWTurret") && W.IsReady() && 
                            Me.CountEnemiesInRange(1000) == 0)
                        {
                            W.Cast();
                            Orbwalking.ResetAutoAttackTimer();
                        }
                    }
                }
            }
        }

        private void LaneClearW()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("LaneClearMana")) && ManaManager.SpellFarm)
            {
                if (Menu.GetBool("LaneClearW") && W.IsReady())
                {
                    var minions = MinionManager.GetMinions(Me.Position, Orbwalking.GetRealAutoAttackRange(Me));

                    if (minions.Count >= 3)
                    {
                        W.Cast();
                        Orbwalking.ResetAutoAttackTimer();
                    }
                }
            }
        }

        private void JungleClearW()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("JungleClearMana")) && ManaManager.SpellFarm)
            {
                if (Menu.GetBool("JungleClearW") && W.IsReady())
                {
                    var Mobs = MinionManager.GetMinions(Me.Position, Orbwalking.GetRealAutoAttackRange(Me),
                        MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

                    if (Mobs.Any())
                    {
                        W.Cast();
                        Orbwalking.ResetAutoAttackTimer();
                    }
                }
            }
        }

        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (Menu.GetBool("AutoE") && E.IsReady() && Me.HealthPercent <= Menu.GetSlider("AutoEHp"))
            {
                if (sender != null && sender.IsEnemy && sender is AIHeroClient)
                {
                    var e = (AIHeroClient)sender;

                    if (Args.Target != null)
                    {
                        if (Args.Target.IsMe)
                        {
                            if (CanE(e, Args))
                            {
                                LeagueSharp.Common.Utility.DelayAction.Add(120, () => E.Cast());
                            }
                        }
                    }
                }
            }
        }

        private bool CanE(AIHeroClient e, GameObjectProcessSpellCastEventArgs Args)
        {
            if (Orbwalking.IsAutoAttack(Args.SData.Name))
            {
                switch (e.ChampionName)
                {
                    case "TwistedFate":
                        if (Args.SData.Name == "GoldCardLock" || Args.SData.Name == "RedCardLock" || Args.SData.Name == "BlueCardLock")
                        {
                            return true;
                        }
                        break;
                    case "Leona":
                        if (Args.SData.Name == "LeonaQ")
                        {
                            return true;
                        }
                        break;
                    default:
                        return false;
                }
            }
            else
            {
                return !Args.SData.Name.ToLower().Contains("summoner");
            }

            return false;
        }

        private void OnDraw(EventArgs Args)
        {
            if (!Me.IsDead && !Shop.IsOpen && !MenuGUI.IsChatOpen  )
            {
                if (Menu.GetBool("DrawQ") && Q.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, Q.Range, Color.Green, 1);
                }
            }
        }
    }
}
