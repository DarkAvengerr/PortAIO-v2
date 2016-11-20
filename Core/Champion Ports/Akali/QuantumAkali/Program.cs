//star combo,  focus target have q mark, perman show auto q e
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Data;
using SharpDX;
using PortAIO.Properties;
using Color = System.Drawing.Color;
using System.Drawing;
using TreeLib.Objects;
using QuantumAkali;

using EloBuddy;
using LeagueSharp.Common;
namespace QuantumAkali
{
    class Program
    {
        #region Declaration
        private static Spell Q, W, E, R;
        private static Orbwalking.Orbwalker Orbwalker;
        private static Menu Menu;
        private static AIHeroClient Player { get { return ObjectManager.Player; } }
        private static string ChampionName = "Akali";
        private static int lvl1, lvl2, lvl3, lvl4;
        #endregion

        public static void Main()
        {
            Game_OnGameLoad();
        }

        private static void Game_OnGameLoad()
        {
            if (Player.ChampionName != ChampionName)
                return;
            Chat.Print("<font color='#f45c09'>[SugoiCollection]</font><font color='#03d8f6'> Quantum Akali </font><font color='#13d450'>Loaded.</font>");

            Q = new Spell(SpellSlot.Q, 600);
            W = new Spell(SpellSlot.W, 475);
            E = new Spell(SpellSlot.E, 325);
            R = new Spell(SpellSlot.R, 700);

            #region Menu
            Menu = new Menu("Quantum Akali", "Quantum Akali", true).SetFontStyle(
                FontStyle.Bold, SharpDX.Color.Red);
            Menu OrbwalkerMenu = Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(OrbwalkerMenu);
            Menu TargetSelectorMenu = Menu.AddSubMenu(new Menu("Target Selector", "TargetSelector"));
            TargetSelector.AddToMenu(TargetSelectorMenu);
            Menu ComboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            ComboMenu.AddItem(new MenuItem("ComboUseQ", "Use Q").SetValue(true));
            /* after r1?, before r1?
            ComboMenu.AddItem(new MenuItem("ComboUseW", "Use W").SetValue(false).SetTooltip("It Will Only Use To Gap Close To Enemy.(Better Cast Your Self)").SetFontStyle(
                        FontStyle.Bold, SharpDX.Color.Red));*/
            ComboMenu.AddItem(new MenuItem("ComboUseE", "Use E").SetValue(true));
            ComboMenu.AddItem(new MenuItem("ComboUseR", "Use R").SetValue(true));
            ComboMenu.AddItem(new MenuItem("QaA", "Try AA After Q Marked Target").SetValue(false));

            Menu UltimateMenu = Menu.AddSubMenu(new Menu("R Menu", "RMenu"));
            /* //maybe no need
            UltimateMenu.AddItem(new MenuItem("rStacks", "Keep stacks", false))
                .SetTooltip("You can set up how many stacks you want to keep In Combo")
                .SetValue(new Slider(0, 0, 3));*/
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                UltimateMenu.SubMenu("R List").AddItem(new MenuItem("UseR" + enemy.ChampionName, "Use R On " + enemy.ChampionName).SetValue(true)).SetTooltip("Turn Off Target Will Not Use R On Them, BUT Will KS");
            UltimateMenu.AddItem(
            new MenuItem("NoTD", "Don't R Under Enemy Tower").SetValue(false));

            Menu LastHitMenu = Menu.AddSubMenu(new Menu("LastHit", "LastHit"));
            LastHitMenu.AddItem(new MenuItem("LastHitQ", "Use Q").SetValue(true));

            Menu HarassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            HarassMenu.AddItem(new MenuItem("HarassUseQ", "Use Q").SetValue(true));
            HarassMenu.AddItem(new MenuItem("HarassUseE", "Use E").SetValue(true));
            /*HarassMenu.AddItem(new MenuItem("HarassUseStar", "Use Star Harass").SetValue(new KeyBind('G', KeyBindType.Press))).Permashow(true, "Use R+Q+E+R Back");*/
            HarassMenu.AddItem(new MenuItem("AutoQ", "Auto Q").SetValue(true).SetValue(new KeyBind('I', KeyBindType.Toggle)));
            HarassMenu.AddItem(new MenuItem("AutoE", "Auto E").SetValue(true).SetValue(new KeyBind('O', KeyBindType.Toggle)));

            Menu LaneCleaMenu = Menu.AddSubMenu(new Menu("Lane Clear", "LaneClear"));
            LaneCleaMenu.AddItem(new MenuItem("LaneClearUseQ", "Use Q").SetValue(true));
            LaneCleaMenu.AddItem(new MenuItem("LaneClearUseE", "Use E").SetValue(true));

            Menu JungleClearMenu = Menu.AddSubMenu(new Menu("Jungle Clear", "JungleClear"));
            JungleClearMenu.AddItem(new MenuItem("JungleClearUseQ", "Use Q").SetValue(true));
            JungleClearMenu.AddItem(new MenuItem("JungleClearUseE", "Use E").SetValue(true));

            Menu FleeMenu = Menu.AddSubMenu(new Menu("Flee", "Flee"));
            FleeMenu.AddItem(new MenuItem("FleeON", "Enable Flee").SetValue(true));
            FleeMenu.AddItem(new MenuItem("FleeK", "Flee Key").SetValue(
                            new KeyBind('J', KeyBindType.Press)));
            FleeMenu.AddItem(new MenuItem("FleeW", "Flee With W").SetValue(true));
            FleeMenu.AddItem(new MenuItem("FleeR", "Flee With R").SetValue(true));

            Menu MiscMenu = Menu.AddSubMenu(new Menu("Misc Menu", "Misc Menu"));
            MiscMenu.AddItem(new MenuItem("KillSteal", "Activate KillSteal?").SetValue(true));
            MiscMenu.AddItem(new MenuItem("QKillSteal", "Q KillSteal?").SetValue(true));
            MiscMenu.AddItem(new MenuItem("EKillSteal", "E KillSteal?").SetValue(true));
            MiscMenu.AddItem(new MenuItem("RKillSteal", "R KillSteal?").SetValue(false));
            MiscMenu.AddItem(new MenuItem("GapcloseR", "Gapclose R?").SetValue(false).SetTooltip("Warning: This Does Not Have Turret Check!"));
            MiscMenu.AddItem(new MenuItem("RCharges", "R Charges For Gapclose").SetValue(new StringList(new[] { "2", "3" })));

            Menu AutoLevelerMenu = Menu.AddSubMenu(new Menu("AutoLeveler Menu", "AutoLevelerMenu"));
            AutoLevelerMenu.AddItem(new MenuItem("AutoLevelUp", "AutoLevel Up Spells?").SetValue(true));
            AutoLevelerMenu.AddItem(new MenuItem("AutoLevelUp1", "First: ").SetValue(new StringList(new[] { "Q", "W", "E", "R" }, 3)));
            AutoLevelerMenu.AddItem(new MenuItem("AutoLevelUp2", "Second: ").SetValue(new StringList(new[] { "Q", "W", "E", "R" }, 0)));
            AutoLevelerMenu.AddItem(new MenuItem("AutoLevelUp3", "Third: ").SetValue(new StringList(new[] { "Q", "W", "E", "R" }, 1)));
            AutoLevelerMenu.AddItem(new MenuItem("AutoLevelUp4", "Fourth: ").SetValue(new StringList(new[] { "Q", "W", "E", "R" }, 2)));
            AutoLevelerMenu.AddItem(new MenuItem("AutoLvlStartFrom", "AutoLeveler Start from Level: ").SetValue(new Slider(2, 6, 1)));

            Menu SkinMenu = Menu.AddSubMenu(new Menu("Skins Menu", "SkinMenu"));
            SkinMenu.AddItem(new MenuItem("SkinID", "Skin ID")).SetValue(new Slider(4, 0, 8));
            var UseSkin = SkinMenu.AddItem(new MenuItem("UseSkin", "Enabled")).SetValue(true);
            UseSkin.ValueChanged += (sender, eventArgs) =>
            {
                if (!eventArgs.GetNewValue<bool>())
                {
                    //ObjectManager.//Player.SetSkin(ObjectManager.Player.CharData.BaseSkinName, ObjectManager.Player.SkinId);
                }
            };

            Menu DrawingMenu = Menu.AddSubMenu(new Menu("Drawing", "Drawing"));
            DrawingMenu.AddItem(new MenuItem("DR", "Draw Only Ready Spells").SetValue(true));
            DrawingMenu.AddItem(new MenuItem("DrawAA", "Draw AA Range").SetValue(true));
            DrawingMenu.AddItem(new MenuItem("DrawQ", "Draw Q Range").SetValue(false));
            DrawingMenu.AddItem(new MenuItem("DrawW", "Draw W Range").SetValue(false));
            DrawingMenu.AddItem(new MenuItem("DrawE", "Draw E Range").SetValue(false));
            DrawingMenu.AddItem(new MenuItem("DrawR", "Draw R Range").SetValue(false));
            /*DrawingMenu.AddItem(new MenuItem("DrawAQ", "Draw Auto Q Status").SetValue(true));
            DrawingMenu.AddItem(new MenuItem("DrawAE", "Draw Auto E Status").SetValue(true));*/

            Menu CreditMenu = Menu.AddSubMenu(new Menu("Credits", "Credits"));
            CreditMenu.AddItem(new MenuItem("ME: LOVETAIWAN♥", "ME: LOVETAIWAN♥")).SetTooltip("Learning From Devs Below!").SetFontStyle(
                FontStyle.Bold, SharpDX.Color.Pink);
            CreditMenu.AddItem(new MenuItem("H3stia", "H3stia")).SetFontStyle(
                FontStyle.Bold, SharpDX.Color.Blue);
            CreditMenu.AddItem(new MenuItem("SupportExTraGoZ", "SupportExTraGoZ")).SetFontStyle(
                FontStyle.Bold, SharpDX.Color.Purple);
            CreditMenu.AddItem(new MenuItem("Soresu", "Soresu")).SetFontStyle(
                FontStyle.Bold, SharpDX.Color.Yellow);
            CreditMenu.AddItem(new MenuItem("ChewyMoon", "ChewyMoon")).SetFontStyle(
                FontStyle.Bold, SharpDX.Color.LightBlue);
            CreditMenu.AddItem(new MenuItem("Trees", "Trees")).SetFontStyle(
                FontStyle.Bold, SharpDX.Color.Orange);
            CreditMenu.AddItem(new MenuItem("Exory", "Exory")).SetFontStyle(
                FontStyle.Bold, SharpDX.Color.Brown);
            CreditMenu.AddItem(new MenuItem("imop", "Be Sugoi").SetValue(true)).SetFontStyle(
                FontStyle.Bold, SharpDX.Color.Green);
            if (Menu.Item("imop").GetValue<bool>())
            {
                //new SoundObject(Resources.OnLoad).Play();
            }
            Menu.AddToMainMenu();
            #endregion

            //SupportExTraGoZ drawhp <3
            #region DrawHPDamage
            var dmgAfterShave = new MenuItem("QuantumAkali.DrawComboDamage", "Draw Damage on Enemy's HP Bar").SetValue(true);
            var drawFill =
                new MenuItem("QuantumAkali.DrawColour", "Fill Color", true).SetValue(
                    new Circle(true, Color.SeaGreen));
            DrawingMenu.AddItem(drawFill);
            DrawingMenu.AddItem(dmgAfterShave);
            DrawDamage.DamageToUnit = CalculateDamage;
            DrawDamage.Enabled = dmgAfterShave.GetValue<bool>();
            DrawDamage.Fill = drawFill.GetValue<Circle>().Active;
            DrawDamage.FillColor = drawFill.GetValue<Circle>().Color;
            dmgAfterShave.ValueChanged +=
                delegate (object sender, OnValueChangeEventArgs eventArgs)
                {
                    DrawDamage.Enabled = eventArgs.GetNewValue<bool>();
                };

            drawFill.ValueChanged += delegate (object sender, OnValueChangeEventArgs eventArgs)
            {
                DrawDamage.Fill = eventArgs.GetNewValue<Circle>().Active;
                DrawDamage.FillColor = eventArgs.GetNewValue<Circle>().Color;
            };
            #endregion

            Drawing.OnDraw += Drawing_OnDraw;
            /*Orbwalking.BeforeAttack += beforeAttack;*/
            Orbwalking.AfterAttack += AfterAttack;
            Obj_AI_Base.OnLevelUp += Obj_AI_Base_OnLevelUp;
            Game.OnUpdate += Game_OnUpdate;
        }//End Game_OnGameLoad + Menu

        #region Auto LevelUp
        private static void Obj_AI_Base_OnLevelUp(Obj_AI_Base sender, EventArgs args)
        {
            if (!sender.IsMe || !Menu.Item("AutoLevelUp").GetValue<bool>() || ObjectManager.Player.Level < Menu.Item("AutoLvlStartFrom").GetValue<Slider>().Value)
                return;
            if (lvl2 == lvl3 || lvl2 == lvl4 || lvl3 == lvl4)
                return;
            int delay = 700;
            LeagueSharp.Common.Utility.DelayAction.Add(delay, () => LevelUp(lvl1));
            LeagueSharp.Common.Utility.DelayAction.Add(delay + 50, () => LevelUp(lvl2));
            LeagueSharp.Common.Utility.DelayAction.Add(delay + 100, () => LevelUp(lvl3));
            LeagueSharp.Common.Utility.DelayAction.Add(delay + 150, () => LevelUp(lvl4));
        }

        private static void LevelUp(int indx)
        {
            if (ObjectManager.Player.Level < 4)
            {
                if (indx == 0 && Player.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                if (indx == 1 && Player.Spellbook.GetSpell(SpellSlot.W).Level == 0)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                if (indx == 2 && Player.Spellbook.GetSpell(SpellSlot.E).Level == 0)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
            }
            else
            {
                if (indx == 0)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                if (indx == 1)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                if (indx == 2)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                if (indx == 3)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.R);
            }
        }
        #endregion

        #region Drawings
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead)
                return;

            if (Menu.Item("DrawAA").GetValue<bool>())
            {
                Render.Circle.DrawCircle(Player.Position, Orbwalking.GetRealAutoAttackRange(null), Color.White);
            }

            if (Menu.Item("DR").GetValue<bool>() && Q.IsReady())
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range, Color.Aqua);
            }
            else
            {
                if (Menu.Item("DrawQ").GetValue<bool>())
                {
                    Render.Circle.DrawCircle(Player.Position, Q.Range, Color.DarkBlue);
                }
            }

            if (Menu.Item("DR").GetValue<bool>() && W.IsReady())
            {
                Render.Circle.DrawCircle(Player.Position, W.Range, Color.Gray);
            }
            else
            {
                if (Menu.Item("DrawW").GetValue<bool>())
                {
                    Render.Circle.DrawCircle(Player.Position, W.Range, Color.Aqua);
                }
            }
            if (Menu.Item("DR").GetValue<bool>() && E.IsReady())
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, Color.Silver);
            }
            else
            {
                if (Menu.Item("DrawE").GetValue<bool>())
                {
                    Render.Circle.DrawCircle(Player.Position, E.Range, Color.Red);
                }
            }
            if (Menu.Item("DR").GetValue<bool>() && R.IsReady())
            {
                Render.Circle.DrawCircle(Player.Position, R.Range, Color.Red);
                Drawing.WorldToMinimap(Player.Position).To3D2();
            }
            else
            {
                if (Menu.Item("DrawR").GetValue<bool>())
                {
                    Render.Circle.DrawCircle(Player.Position, R.Range, Color.Red);
                }
            }
            /*
            if (Menu.Item("DrawAQ").GetValue<bool>())
            {
                Menu.Item("AutoQ").Permashow(true, "Auto Q");
            }

            if (Menu.Item("DrawAE").GetValue<bool>())
            {
                Menu.Item("AutoE").Permashow(true, "Auto E");
            }*/
        }
        #endregion

        private static float CalculateDamage(Obj_AI_Base enemy)
        {
            float damage = 0;

            if (W.IsReady() && Player.Mana > W.Instance.SData.Mana)
            {
                damage += W.GetDamage(enemy);
            }
            if (R.IsReady() && Player.Mana > R.Instance.SData.Mana)
            {
                damage += R.GetDamage(enemy);
            }
            damage += (float)Player.GetAutoAttackDamage(enemy);

            return damage;
        }

        #region Game_OnUpdate
        private static void Game_OnUpdate(EventArgs args)
        {
            if (Player.IsDead || Player.IsRecalling())
                return;

            if (Menu.Item("UseSkin").GetValue<bool>())
            {
                //Player.SetSkin(Player.CharData.BaseSkinName, Menu.Item("SkinID").GetValue<Slider>().Value);
            }
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    LastHit();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    LastHit();
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LastHit();
                    LaneClear();
                    JungleClear();
                    break;
            }
            if (Menu.Item("FleeON").GetValue<bool>())
            {
                if (Menu.Item("FleeK").GetValue<KeyBind>().Active)
                {
                    Flee();
                }
            }
            if (Menu.Item("KillSteal").GetValue<bool>())
            {
                {
                    Ks();
                }
            }
            AutoQ();
            AutoE();
            //AutoLeveler
            if (Menu.Item("AutoLevelUp").GetValue<bool>())
            {
                lvl1 = Menu.Item("AutoLevelUp1").GetValue<StringList>().SelectedIndex;
                lvl2 = Menu.Item("AutoLevelUp2").GetValue<StringList>().SelectedIndex;
                lvl3 = Menu.Item("AutoLevelUp3").GetValue<StringList>().SelectedIndex;
                lvl4 = Menu.Item("AutoLevelUp4").GetValue<StringList>().SelectedIndex;
            }
        }
        #endregion

        #region KS
        private static void Ks()
        {
            if (Q.IsReady() && Menu.Item("QKillSteal").GetValue<bool>())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range) && x.Health < Q.GetDamage(x) && !x.HasBuff("guardianangle") && !x.IsZombie))
                    if (Q.IsReady())
                    {
                        Q.Cast(target);
                    }
            }
            if (E.IsReady() && Menu.Item("EKillSteal").GetValue<bool>())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(E.Range) && x.Health < E.GetDamage(x) && !x.HasBuff("guardianangle") && !x.IsZombie))
                    if (E.IsReady())
                    {
                        E.Cast();
                    }
            }
            if (R.IsReady() && Menu.Item("RKillSteal").GetValue<bool>())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range) && x.Health < R.GetDamage(x) && !x.HasBuff("guardianangle") && !x.IsZombie))
                    if (R.IsReady())
                    {
                        R.Cast(target);
                    }
            }
        }

        #endregion
        /*
        private static void NoTD()
        {
            if (Menu.Item("NoTD").GetValue<bool>())
            {
                var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);

                if (R.IsReady() && target.IsValidTarget() && target.UnderTurret(true) && !target.HasBuff("guardianangle"))
                {
                    return;
                }
            }
        }*/
        private static void AutoQ()
        {
            var autoQ = Menu.Item("AutoQ").GetValue<KeyBind>().Active && Q.IsReady();
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (autoQ && !Player.UnderTurret())
            {
                Q.Cast(target);
            }
        }

        private static void AutoE()
        {
            var autoE = Menu.Item("AutoE").GetValue<KeyBind>().Active && E.IsReady();
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            if (autoE && !Player.UnderTurret())
            {
                E.Cast();
            }
        }
        //ChewyMoon
        private static void GapcloseCombo()
        {
            var Rcharges = Menu.Item("RCharges").GetValue<StringList>().SelectedIndex == 0 ? 2 : 3;
            var Rammo = Player.Spellbook.GetSpell(SpellSlot.R).Ammo;

            if (!(Rammo >= Rcharges))
            {
                return;
            }

            if (!R.IsReady())
            {
                return;
            }

            var target = TargetSelector.GetTarget(R.Range * 3, TargetSelector.DamageType.Magical);
            if (target == null)
            {
                return;
            }

            var minion =
            MinionManager.GetMinions(Player.ServerPosition, R.Range)
                .Where(x => x.IsValidTarget())
                .FirstOrDefault(x => x.Distance(target) < R.Range);

            if (minion.IsValidTarget())
            {
                R.CastOnUnit(minion);
            }
        }
        /*
        private static void beforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            var target = Orbwalker.GetTarget() as AIHeroClient;
            if (target == null)
                target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            {
                var QT = HeroManager.Enemies.OrderBy(e => e.Distance(target)).FirstOrDefault(
                x =>
                x.ServerPosition.Distance(ObjectManager.Player.ServerPosition) < 125 &&
                x.HasBuff("Akalimota"));
                if (Menu.Item("ComboUseQ").GetValue<bool>() && target.IsValidTarget(Q.Range) && Q.CanCast(target) && Q.IsReady())
                {
                    Q.Cast(target);
                }
                if (Menu.Item("QaA").GetValue<bool>() && QT.IsValidTarget(125))
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AutoAttack, QT);
                }
            }
        }*/
        private static void AfterAttack(AttackableUnit unit, AttackableUnit attackableUnit)
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                AIHeroClient target = TargetSelector.GetTarget(300, TargetSelector.DamageType.Magical);
                if (Menu.Item("ComboUseE").GetValue<bool>() && E.IsReady())
                    E.Cast();
            }
        }
        #region Combo
        private static void Combo()
        {
            var target = Orbwalker.GetTarget() as AIHeroClient;
            if (target == null)
                target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (Menu.Item("GapcloseR").GetValue<bool>())
            {
                GapcloseCombo();
            }
            if (target != null && target.IsValidTarget())
            {
                /*var enemy = HeroManager.Enemies.OrderBy(e => e.Distance(target)).FirstOrDefault();
                    if(enemy != null)
                    {
                        foreach (var buff in enemy.Buffs)
                        {
                            Console.WriteLine(buff.Name);
                        }
                    }*/
                if (Menu.Item("ComboUseQ").GetValue<bool>() && target.IsValidTarget(Q.Range) && Q.CanCast(target) && Q.IsReady())
                {
                    Q.Cast(target);
                }

                if (Menu.Item("ComboUseR").GetValue<bool>() && R.IsReady())
                {
                    if (Menu.Item("NoTD").GetValue<bool>() && target.UnderTurret(true))
                    {
                        return;
                    }

                    if (Menu.Item("UseR" + target.ChampionName).GetValue<bool>())
                    {
                        if (Menu.Item("ComboUseR").GetValue<bool>() && !target.IsValidTarget(E.Range) && target.IsValidTarget(R.Range) && R.CanCast(target) && R.IsReady())
                            R.Cast(target);
                    }
                }


                /*
                if (Menu.Item("ComboUseW").GetValue<bool>() && target.IsValidTarget(700) && W.IsReady() && R.IsReady())
                    W.Cast(Game.CursorPos);
                    */
            }
        }
        #endregion

        //My favoriate mundo from H3stia
        private static void LastHit()
        {
            var castQ = Menu.Item("LastHitQ").GetValue<bool>() && Q.IsReady();
            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly);

            if (castQ)
            {
                foreach (var minion in minions)
                {
                    if (HealthPrediction.GetHealthPrediction(minion, (int)(Q.Delay + (minion.Distance(ObjectManager.Player.Position) / Q.Speed))) < ObjectManager.Player.GetSpellDamage(minion, SpellSlot.Q))
                    {
                        Q.Cast(minion);
                    }
                }
            }
        }
        private static void Harass()
        {
            var SpellQ = Menu.Item("HarassUseQ").GetValue<bool>();
            var SpellE = Menu.Item("HarassUseE").GetValue<bool>();
            /*var SpellStar = Menu.Item("HarassUseStar").GetValue<bool>();*/
            var target = Orbwalker.GetTarget() as AIHeroClient;
            if (target == null)
                target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (target != null && target.IsValidTarget())
            {
                if (target.IsValidTarget(Q.Range) && SpellQ)
                    Q.Cast(target);

                if (E.IsReady() && SpellE && target.IsValidTarget(E.Range))
                    E.Cast();
            }
        }

        private static void LaneClear()
        {
            var SpellQ = Menu.Item("LaneClearUseQ").GetValue<bool>();
            var SpellE = Menu.Item("LaneClearUseE").GetValue<bool>();
            var Minions = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.NotAlly);

            if (Minions.Count > 1)
            {
                foreach (var minion in Minions)
                {
                    if (!minion.IsValidTarget() || minion == null)
                        return;
                    if (SpellQ && Q.IsReady())
                        Q.Cast(minion);

                    if (SpellE && E.IsReady() && minion.IsValidTarget(E.Range))
                        E.Cast();
                }
            }
        }

        private static void JungleClear()
        {
            var allMinions = MinionManager.GetMinions(
                ObjectManager.Player.ServerPosition,
                Q.Range,
                MinionTypes.All,
                MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);
            if (Menu.Item("JungleClearUseQ").GetValue<bool>() && Q.IsReady())
            {
                foreach (var minion in allMinions)
                {
                    if (minion.IsValidTarget())
                    {
                        Q.Cast(minion);
                    }
                }
            }
            if (Menu.Item("JungleClearUseE").GetValue<bool>() && E.IsReady())
            {
                foreach (var minion in allMinions)
                {
                    if (minion.IsValidTarget() && minion.IsValidTarget(E.Range))
                    {
                        E.Cast();
                    }
                }
            }
        }

        private static void Flee()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            var FleeW = Menu.Item("FleeW").GetValue<bool>();
            if (FleeW && W.IsReady())
            {
                W.Cast(Game.CursorPos);
            }
            //how to only r to minions near game cursopos,maybe working need more test
            var FleeR = Menu.Item("FleeR").GetValue<bool>();
            var m = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsValidTarget(R.Range) && !x.IsAlly && R.CanCast(x)).
                    OrderBy(x => x.Position.Distance(Game.CursorPos)).FirstOrDefault();

            if (m != null && FleeR && R.IsReady())
            {
                R.CastOnUnit(m);
                return;
            }
        }

    }
}
