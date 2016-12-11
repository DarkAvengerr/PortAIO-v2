using EloBuddy; 
using LeagueSharp.Common; 
namespace xSaliceResurrected_Rework.Pluging
{
    using Base;
    using System;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using Managers;
    using Utilities;
    using Color = System.Drawing.Color;
    using Orbwalking = Orbwalking;

    internal class Rumble : Champion
    {
        public Rumble()
        {
            SpellManager.P = new Spell(SpellSlot.R, 4000);
            SpellManager.Q = new Spell(SpellSlot.Q, 500);
            SpellManager.W = new Spell(SpellSlot.W);
            SpellManager.E = new Spell(SpellSlot.E, 950);
            SpellManager.R = new Spell(SpellSlot.R, 1700);
            SpellManager.R2 = new Spell(SpellSlot.R, 1000);

            SpellManager.E.SetSkillshot(0.25f, 70, 1200, true, SkillshotType.SkillshotLine);
            SpellManager.P.SetSkillshot(0.4f, 130, 2500, false, SkillshotType.SkillshotLine);
            SpellManager.R.SetSkillshot(0.4f, 130, 2500, false, SkillshotType.SkillshotLine);
            SpellManager.R2.SetSkillshot(0.4f, 130, 2600, false, SkillshotType.SkillshotLine);

            var spellMenu = new Menu("SpellMenu", "SpellMenu");
            {
                var qMenu = new Menu("QMenu", "QMenu");
                {
                    qMenu.AddItem(new MenuItem("Q_Auto_Heat", "Use Q To generate Heat", true).SetValue(true));
                    qMenu.AddItem(new MenuItem("Q_Over_Heat", "Q Smart OverHeat KS", true).SetValue(true));
                    spellMenu.AddSubMenu(qMenu);
                }

                var wMenu = new Menu("WMenu", "WMenu");
                {
                    wMenu.AddItem(new MenuItem("W_Auto_Heat", "Use W To generate Heat", true).SetValue(true));
                    wMenu.AddItem(new MenuItem("W_Always", "Use W Always On Combo/Harass", true).SetValue(false));
                    wMenu.AddItem(new MenuItem("W_Block_Spell", "Use W On Incoming Spells", true).SetValue(true));
                    spellMenu.AddSubMenu(wMenu);
                }

                var eMenu = new Menu("EMenu", "EMenu");
                {
                    eMenu.AddItem(new MenuItem("E_Auto_Heat", "Use E To generate Heat", true).SetValue(false));
                    eMenu.AddItem(new MenuItem("E_Over_Heat", "E Smart OverHeat KS", true).SetValue(true));
                    spellMenu.AddSubMenu(eMenu);
                }

                var rMenu = new Menu("RMenu", "RMenu");
                {
                    rMenu.AddItem(new MenuItem("UseMecR", "Force Best Mec Ult", true).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
                    rMenu.AddItem(new MenuItem("Line_If_Enemy_Count", "Auto R If >= Enemy, 6 = Off", true).SetValue(new Slider(4, 1, 6)));
                    rMenu.AddItem(new MenuItem("Line_If_Enemy_Count_Combo", "R if >= In Combo, 6 = off", true).SetValue(new Slider(3, 1, 6)));
                    spellMenu.AddSubMenu(rMenu);
                }

                Menu.AddSubMenu(spellMenu);
            }

            var combo = new Menu("Combo", "Combo");
            {
                combo.AddItem(new MenuItem("UseQCombo", "Use Q", true).SetValue(true));
                combo.AddItem(new MenuItem("UseWCombo", "Use W", true).SetValue(true));
                combo.AddItem(new MenuItem("UseECombo", "Use E", true).SetValue(true));
                combo.AddItem(new MenuItem("UseRCombos", "Use R", true).SetValue(false));
                Menu.AddSubMenu(combo);
            }

            var harass = new Menu("Harass", "Harass");
            {
                harass.AddItem(new MenuItem("UseQHarass", "Use Q", true).SetValue(false));
                harass.AddItem(new MenuItem("UseWHarass", "Use W", true).SetValue(false));
                harass.AddItem(new MenuItem("UseEHarass", "Use E", true).SetValue(true));
                harass.AddItem(new MenuItem("FarmT", "Harass (toggle)!", true).SetValue(new KeyBind("N".ToCharArray()[0], KeyBindType.Toggle)));
                Menu.AddSubMenu(harass);
            }

            var farm = new Menu("LaneClear", "LaneClear");
            {
                farm.AddItem(new MenuItem("UseQFarm", "Use Q", true).SetValue(true));
                farm.AddItem(new MenuItem("UseEFarm", "Use E", true).SetValue(true));
                Menu.AddSubMenu(farm);
            }

            var miscMenu = new Menu("Misc", "Misc");
            {
                miscMenu.AddItem(new MenuItem("Stay_Danger", "Stay In Danger Zone", true).SetValue(new KeyBind("I".ToCharArray()[0], KeyBindType.Toggle)));
                miscMenu.AddItem(new MenuItem("E_Gap_Closer", "Use E On Gap Closer", true).SetValue(true));
                Menu.AddSubMenu(miscMenu);
            }

            var drawMenu = new Menu("Drawing", "Drawing");
            {
                drawMenu.AddItem(new MenuItem("Draw_Disabled", "Disable All", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("Draw_Q", "Draw Q", true).SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_W", "Draw W", true).SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_E", "Draw E", true).SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_R", "Draw R", true).SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_R_Pred", "Draw R Best Line", true).SetValue(true));

                var drawComboDamageMenu = new MenuItem("Draw_ComboDamage", "Draw Combo Damage", true).SetValue(true);
                var drawFill = new MenuItem("Draw_Fill", "Draw Combo Damage Fill", true).SetValue(new Circle(true, Color.FromArgb(90, 255, 169, 4)));
                drawMenu.AddItem(drawComboDamageMenu);
                drawMenu.AddItem(drawFill);
                DamageIndicator.DamageToUnit = GetComboDamage;
                DamageIndicator.Enabled = drawComboDamageMenu.GetValue<bool>();
                DamageIndicator.Fill = drawFill.GetValue<Circle>().Active;
                DamageIndicator.FillColor = drawFill.GetValue<Circle>().Color;
                drawComboDamageMenu.ValueChanged +=
                    delegate (object sender, OnValueChangeEventArgs eventArgs)
                    {
                        DamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
                    };
                drawFill.ValueChanged +=
                    delegate (object sender, OnValueChangeEventArgs eventArgs)
                    {
                        DamageIndicator.Fill = eventArgs.GetNewValue<Circle>().Active;
                        DamageIndicator.FillColor = eventArgs.GetNewValue<Circle>().Color;
                    };

                Menu.AddSubMenu(drawMenu);
            }

            var customMenu = new Menu("Custom Perma Show", "Custom Perma Show");
            {
                var myCust = new CustomPermaMenu();
                customMenu.AddItem(new MenuItem("custMenu", "Move Menu", true).SetValue(new KeyBind("L".ToCharArray()[0], KeyBindType.Press)));
                customMenu.AddItem(new MenuItem("enableCustMenu", "Enabled", true).SetValue(true));
                customMenu.AddItem(myCust.AddToMenu("Combo Active: ", "Orbwalk"));
                customMenu.AddItem(myCust.AddToMenu("Harass Active: ", "Farm"));
                customMenu.AddItem(myCust.AddToMenu("Harass(T) Active: ", "FarmT"));
                customMenu.AddItem(myCust.AddToMenu("Laneclear Active: ", "LaneClear"));
                customMenu.AddItem(myCust.AddToMenu("Lasthit E Active: ", "LastHit"));
                customMenu.AddItem(myCust.AddToMenu("Force Ult: ", "UseMecR"));
                Menu.AddSubMenu(customMenu);
            }
        }


        private float GetComboDamage(Obj_AI_Base target)
        {
            double comboDamage = 0;

            if (Q.IsReady())
                comboDamage += GetCurrentHeat() > 50 ? Player.GetSpellDamage(target, SpellSlot.Q) * 2 : Player.GetSpellDamage(target, SpellSlot.Q);

            if (E.IsReady())
                comboDamage += GetCurrentHeat() > 50 ? Player.GetSpellDamage(target, SpellSlot.E) * 1.5 : Player.GetSpellDamage(target, SpellSlot.E);

            if (R.IsReady())
                comboDamage += Player.GetSpellDamage(target, SpellSlot.R) * 3;

            comboDamage = ItemManager.CalcDamage(target, comboDamage);

            return (float)(comboDamage + Player.GetAutoAttackDamage(target));
        }

        private void Combo()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

            if (target == null)
                return;

            if (Menu.Item("UseQCombo", true).GetValue<bool>() && ShouldQ(target))
                Q.Cast(target);

            if (Menu.Item("UseWCombo", true).GetValue<bool>() && Menu.Item("W_Always", true).GetValue<bool>() && W.IsReady())
                W.Cast();

            var itemTarget = TargetSelector.GetTarget(750, TargetSelector.DamageType.Physical);

            if (itemTarget != null)
            {
                var dmg = GetComboDamage(itemTarget);

                ItemManager.Target = itemTarget;

                if (dmg > itemTarget.Health - 50)
                    ItemManager.KillableTarget = true;

                ItemManager.UseTargetted = true;
            }

            if (Menu.Item("UseECombo", true).GetValue<bool>() && ShouldE(target))
                E.Cast(target);

            if (Menu.Item("UseRCombos", true).GetValue<bool>() && GetComboDamage(target) > target.Health)
                SpellCastManager.CastSingleLine(R, R2, true);
        }

        private void Harass()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

            if (target == null)
                return;

            if (Menu.Item("UseQHarass", true).GetValue<bool>() && ShouldQ(target))
                Q.Cast(target);

            if (Menu.Item("UseWHarass", true).GetValue<bool>() && Menu.Item("W_Always", true).GetValue<bool>() && W.IsReady())
                W.Cast();

            if (Menu.Item("UseEHarass", true).GetValue<bool>() && ShouldE(target))
                E.Cast(target);
        }

        private void Farm()
        {
            var allMinionsQ = MinionManager.GetMinions(Player.ServerPosition, Q.Range,
                MinionTypes.All, MinionTeam.NotAlly);
            var allMinionsE = MinionManager.GetMinions(Player.ServerPosition, E.Range,
                MinionTypes.All, MinionTeam.NotAlly);

            var useQ = Menu.Item("UseQFarm", true).GetValue<bool>();
            var useE = Menu.Item("UseEFarm", true).GetValue<bool>();

            if (useQ && allMinionsQ.Count > 0)
                Q.Cast(allMinionsQ[0]);

            if (useE && allMinionsE.Count > 0)
                E.Cast(allMinionsE[0]);
        }

        private void LastHit()
        {
            var allMinionsE = MinionManager.GetMinions(Player.ServerPosition, E.Range,
                MinionTypes.All, MinionTeam.NotAlly);

            if (allMinionsE.Count > 0 && E.IsReady())
            {
                foreach (var minion in allMinionsE)
                {
                    if (E.IsKillable(minion))
                        E.Cast(minion);
                }
            }
        }

        private bool ShouldQ(AIHeroClient target)
        {
            if (!Q.IsReady())
                return false;

            if (Player.Distance(target.Position) > Q.Range)
                return false;

            if (!Menu.Item("Q_Over_Heat", true).GetValue<bool>() && GetCurrentHeat() > 80)
                return false;

            return !(GetCurrentHeat() > 80) || Player.GetSpellDamage(target, SpellSlot.Q, 1) + 
                Player.GetAutoAttackDamage(target) * 2 > target.Health;
        }

        private bool ShouldE(AIHeroClient target)
        {
            if (!E.IsReady())
                return false;

            if (Player.Distance(target.Position) > E.Range)
                return false;

            if (E.GetPrediction(target).Hitchance < HitChance.VeryHigh)

                if (!Menu.Item("E_Over_Heat", true).GetValue<bool>() && GetCurrentHeat() > 80)
                    return false;

            return !(GetCurrentHeat() > 80) || Player.GetSpellDamage(target, SpellSlot.E, 1) +
                Player.GetAutoAttackDamage(target) * 2 > target.Health;
        }

        private void StayInDangerZone()
        {
            if (Player.InFountain() || Player.IsRecalling())
                return;

            if (GetCurrentHeat() < 31 && W.IsReady() && Menu.Item("W_Auto_Heat", true).GetValue<bool>())
            {
                W.Cast();
                return;
            }

            if (GetCurrentHeat() < 31 && Q.IsReady() && Menu.Item("Q_Auto_Heat", true).GetValue<bool>())
            {
                var enemy = HeroManager.Enemies.Where(x => !x.IsDead)
                    .OrderBy(x => Player.Distance(x.Position)).FirstOrDefault();

                if (enemy != null)
                    Q.Cast(enemy.ServerPosition);
                return;
            }

            if (GetCurrentHeat() < 31 && E.IsReady() && Menu.Item("E_Auto_Heat", true).GetValue<bool>())
            {
                var enemy = HeroManager.Enemies.Where(x => !x.IsDead)
                    .OrderBy(x => Player.Distance(x.Position)).FirstOrDefault();

                if (enemy != null)
                    E.Cast(enemy);
            }

        }

        private float GetCurrentHeat()
        {
            return Player.Mana;
        }

        protected override void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead)
                return;

            SpellCastManager.CastBestLine(false, R, R2, (int)(R2.Range / 2), Menu, .9f);

            if (Menu.Item("Stay_Danger", true).GetValue<KeyBind>().Active)
                StayInDangerZone();

            if (Menu.Item("FarmT", true).GetValue<KeyBind>().Active)
                Harass();

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    LastHit();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Farm();
                    break;
                case Orbwalking.OrbwalkingMode.Freeze:
                    break;
                case Orbwalking.OrbwalkingMode.CustomMode:
                    break;
                case Orbwalking.OrbwalkingMode.None:
                    if (Menu.Item("UseMecR", true).GetValue<KeyBind>().Active)
                        SpellCastManager.CastBestLine(true, R, R2, (int)(R2.Range / 2 + 100), Menu, .9f);
                    break;
                case Orbwalking.OrbwalkingMode.Flee:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs args)
        {
            if (unit.IsEnemy && unit.Type == GameObjectType.AIHeroClient && W.IsReady() && 
                Menu.Item("W_Block_Spell", true).GetValue<bool>())
            {
                if (Player.Distance(args.End) < 400 && GetCurrentHeat() < 70)
                {
                    W.Cast();
                }
            }
        }

        protected override void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!Menu.Item("E_Gap_Closer", true).GetValue<bool>()) return;

            if (E.IsReady() && gapcloser.Sender.IsValidTarget(E.Range))
                E.Cast(gapcloser.Sender);
        }

        protected override void Drawing_OnDraw(EventArgs args)
        {
            if (Menu.Item("Draw_Disabled", true).GetValue<bool>())
                return;

            if (Menu.Item("Draw_Q", true).GetValue<bool>())
                if (Q.Level > 0)
                    Render.Circle.DrawCircle(Player.Position, Q.Range, Q.IsReady() ? Color.Green : Color.Red);

            if (Menu.Item("Draw_W", true).GetValue<bool>())
                if (W.Level > 0)
                    Render.Circle.DrawCircle(Player.Position, W.Range - 2, W.IsReady() ? Color.Green : Color.Red);

            if (Menu.Item("Draw_E", true).GetValue<bool>())
                if (E.Level > 0)
                    Render.Circle.DrawCircle(Player.Position, E.Range, E.IsReady() ? Color.Green : Color.Red);

            if (Menu.Item("Draw_R", true).GetValue<bool>())
                if (R.Level > 0)
                    Render.Circle.DrawCircle(Player.Position, R.Range, R.IsReady() ? Color.Green : Color.Red);


            if (Menu.Item("Draw_R_Pred", true).GetValue<bool>() && R.IsReady())
            {
                SpellCastManager.DrawBestLine(R, R2, (int)(R2.Range/2), .9f);
            }
        }
    }
}
