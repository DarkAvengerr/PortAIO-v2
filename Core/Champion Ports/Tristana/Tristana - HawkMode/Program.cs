using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using Microsoft.Win32;
using Color = SharpDX.Color;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace HawkTristana
{
    static class Program
    {

        static readonly string[] Gapcloser = new[]
        {
            "AkaliShadowDance", "Headbutt", "DianaTeleport", "IreliaGatotsu", "JaxLeapStrike", "JayceToTheSkies",
            "MaokaiUnstableGrowth", "MonkeyKingNimbus", "Pantheon_LeapBash", "PoppyHeroicCharge", "QuinnE",
            "XenZhaoSweep", "blindmonkqtwo", "FizzPiercingStrike", "RengarLeap", "LeonaZenithBlade","BandageToss","AlphaStrike","RenektonSliceAndDice"
        };

        static readonly string[] Interrupt = new[]
        {
            "KatarinaR", "GalioIdolOfDurand", "Crowstorm", "Drain", "AbsoluteZero", "ShenStandUnited", "UrgotSwap2",
            "AlZaharNetherGrasp", "FallenOne", "Pantheon_GrandSkyfall_Jump", "VarusQ", "CaitlynAceintheHole",
            "MissFortuneBulletTime", "InfiniteDuress", "LucianR"
        };

        public static Spell Q, W, E, R;
        public static AIHeroClient Hero => HeroManager.Player;
        public static Menu menu, combo, misc, drawing, orbwalkerMenu, targetSelectorMenu, Emenu, TargetEMenu;
        public static Orbwalking.Orbwalker Orb;
        public const string Name = "Tristana - Hawk Mode";

        public static void Main()
        {
            OnLoad();
        }

        private static void OnLoad()
        {
            if (Hero.ChampionName != "Tristana") return;


            LoadSpells();
            LoadMenu();
            Orbwalking.AfterAttack += AfterAttack;
        //    Interrupter2.OnInterruptableTarget += OnInterrupter;
            AntiGapcloser.OnEnemyGapcloser += OnAntiGapcloser;
            Obj_AI_Base.OnProcessSpellCast += Game_ProcessSpell;
            Game.OnUpdate += OnUpdate;
            EloBuddy.Drawing.OnDraw += Ondraw;
        }

        private static void Game_ProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var useAntiInterrupter = menu.Item("AI").GetValue<bool>();
            if(useAntiInterrupter)
            for (int i = 0; i < Interrupt.Length; i++)
            {
                if (args.SData.Name == Interrupt[i])
                {
                    var useAntiGapcloser = menu.Item(Interrupt[i]).GetValue<bool>();
                    if (useAntiGapcloser)
                    {
                        if (sender.IsValidTarget(R.Range))
                        {
                            R.Cast(sender);
                        }
                    }
                }
            }

        }

        private static
            void OnAntiGapcloser(ActiveGapcloser gapcloser)
        {
            //  gapcloser.Slot
            bool useAntiOnSpell = menu.Item(gapcloser.Sender.GetSpell(gapcloser.Slot).Name).GetValue<bool>();

            var useAntiGapcloser = menu.Item("AG").GetValue<bool>();
            if (R.IsReady() && useAntiGapcloser && gapcloser.Sender.IsValidTarget(300) && useAntiOnSpell)
            {
                R.Cast(gapcloser.Sender);
            }
        }
        public static void LoadSpells()
        {
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 900);
            E = new Spell(SpellSlot.E, 620);
            R = new Spell(SpellSlot.R, 620);

        }

        public static void LoadMenu()
        {
            menu = new LeagueSharp.Common.Menu(Name, Name, true).SetFontStyle(FontStyle.Regular, Color.Aquamarine);
            orbwalkerMenu = new LeagueSharp.Common.Menu("Orbwalker", "Orbwalker");
            Orb = new Orbwalking.Orbwalker(orbwalkerMenu);
            targetSelectorMenu = new LeagueSharp.Common.Menu("TargetSelector", "TargetSelector");

            TargetEMenu = new LeagueSharp.Common.Menu("Targets", "Targets");
            {
                foreach (var ai in HeroManager.Enemies)
                {
                    TargetEMenu.AddItem(new MenuItem("ET" + ai.ChampionName, ai.ChampionName).SetValue(true));
                }
            }
            var targetRMenu = new LeagueSharp.Common.Menu("Targets", "TargetsR");
            {
                foreach (var ai in HeroManager.Enemies)
                {
                    targetRMenu.AddItem(new MenuItem("RT" + ai.ChampionName, ai.ChampionName).SetValue(true));
                }
            }
            Emenu = new LeagueSharp.Common.Menu("E Menu", "EMenu");
            {
                Emenu.AddItem(new MenuItem("CE", "Use E").SetValue(true));
                Emenu.AddItem(new MenuItem("CFE", "Focus target with E").SetValue(true));
                Emenu.AddSubMenu(TargetEMenu);
            }
            var Rmenu = new LeagueSharp.Common.Menu("R Menu", "RMenu");
            {
                Rmenu.AddItem(new MenuItem("CR", "Use R to finish Target").SetValue(true));
                Rmenu.AddSubMenu(targetRMenu);
            }
            combo = new LeagueSharp.Common.Menu("Combo", "Combo Menu");
            {
                combo.AddItem(new MenuItem("CQ", "Use Q").SetValue(true));
                combo.AddSubMenu(Emenu);
                combo.AddSubMenu(Rmenu);
            }


            //    vgapcloserMenu
            var gapcloserMenu = new LeagueSharp.Common.Menu("Spells Gapclosers", "Gapcloser");
            {
                foreach (var hero in HeroManager.Enemies)
                {
                    Console.WriteLine(hero.GetSpell(SpellSlot.E).Name);
                    foreach (var spellName in Gapcloser)
                        if (hero.GetSpell(SpellSlot.Q).Name == spellName)
                        {
                            Console.WriteLine(spellName);
                            gapcloserMenu.AddItem(new MenuItem(spellName, spellName).SetValue(true));
                        }
                        else if (hero.GetSpell(SpellSlot.W).Name == spellName)
                            gapcloserMenu.AddItem(new MenuItem(spellName, spellName).SetValue(true));
                        else if (hero.GetSpell(SpellSlot.E).Name == spellName)
                            gapcloserMenu.AddItem(new MenuItem(spellName, spellName).SetValue(true));
                        else if (hero.GetSpell(SpellSlot.R).Name == spellName)
                            gapcloserMenu.AddItem(new MenuItem(spellName, spellName).SetValue(true));
                }
            }
            var interruptMenu = new LeagueSharp.Common.Menu("Interrupt Spells", "Interrupt");
            {
                foreach (var hero in HeroManager.Enemies)
                {
                    foreach (string spellName in Interrupt)
                        if (hero.GetSpell(SpellSlot.Q).Name == spellName)
                            interruptMenu.AddItem(new MenuItem("I" + spellName, "I" + spellName).SetValue(true));
                        else if (hero.GetSpell(SpellSlot.W).Name == spellName)
                            interruptMenu.AddItem(new MenuItem("I" + spellName, "I" + spellName).SetValue(true));
                        else if (hero.GetSpell(SpellSlot.E).Name == spellName)
                            interruptMenu.AddItem(new MenuItem("I" + spellName, "I" + spellName).SetValue(true));
                        else if (hero.GetSpell(SpellSlot.R).Name == spellName)
                            interruptMenu.AddItem(new MenuItem("I" + spellName, "I" + spellName).SetValue(true));
                }
            }
            misc = new LeagueSharp.Common.Menu("Misc", "Misc Menu");
            {
                misc.AddItem(new MenuItem("AG", "Anti Gapcloser").SetValue(true));
                misc.AddSubMenu(gapcloserMenu);
                misc.AddSubMenu(interruptMenu);
                misc.AddItem(new MenuItem("AI", "Auto Interrupter").SetValue(true));
            }
            drawing = new LeagueSharp.Common.Menu("Drawing", "Drawing Menu");
            {
                drawing.AddItem(new MenuItem("DW", "Draw W range").SetValue(true));
                drawing.AddItem(new MenuItem("DE", "Draw E range").SetValue(true));
                drawing.AddItem(new MenuItem("DDI", "Draw Damage Indicator").SetValue(true));
            }
            TargetSelector.AddToMenu(targetSelectorMenu);
            menu.AddSubMenu(orbwalkerMenu); //ORBWALKER
            menu.AddToMainMenu();
            menu.AddSubMenu(combo);
            menu.AddSubMenu(misc);
            menu.AddSubMenu(drawing);
        }

        private static void Ondraw(EventArgs args)
        {
            var DrawW = menu.Item("DW").GetValue<bool>();
            var DrawE = menu.Item("DE").GetValue<bool>();
            var DrawTDamage = menu.Item("DDI").GetValue<bool>();
            if (DrawW && W.IsReady())
                Render.Circle.DrawCircle(ObjectManager.Player.Position, 900, System.Drawing.Color.Yellow, 2);
            if (DrawE && E.IsReady())
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.Yellow, 2);

            //Draw Damage
            if (DrawTDamage)
            {
                foreach (var target in HeroManager.Enemies)
                {
                    if (!target.IsHPBarRendered || !target.Position.IsOnScreen()) continue;
                    float TotalRDamage = 0, TotalEDamage = 0;
                    int n1 = 0;
                    if (R.IsReady())
                    {
                        n1 = 1;
                        TotalRDamage = R.GetDamage(target);
                    }

                    TotalEDamage = GetEDmg(target, (GetTristanaEBuff(target)) + n1);
                    float TotalDamage = TotalRDamage + TotalEDamage;
                    drawTotalDamage(target, TotalDamage, TotalRDamage);
                }
            }
        }

        private static void drawTotalDamage(AIHeroClient tar, float totalSpellDamage, float TotalRDamage)
        {
            const int width = 103;
            const int height = 8;
            const int xOffset = 10;
            const int yOffset = 20;
            var percentHealthAfterDamage = Math.Max(0, tar.Health - totalSpellDamage)/tar.MaxHealth;
            var barPos = tar.HPBarPosition;
            var yPos = barPos.Y + yOffset;
            var xPosDamage = barPos.X + xOffset + width*percentHealthAfterDamage;
            var xPosCurrentHp = barPos.X + xOffset + width*tar.Health/tar.MaxHealth;
            var pos1 = barPos.X + xOffset + (107*percentHealthAfterDamage);
            var differenceInHp = xPosCurrentHp - xPosDamage;
            var hpPos = tar.HPBarPosition;
            for (var i = 0; i < differenceInHp; i++)
            {
                Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + height, 1, System.Drawing.Color.OrangeRed);



            }
            if (tar.Health <= totalSpellDamage)
            {
                Drawing.DrawText(hpPos.X, hpPos.Y - 20, System.Drawing.Color.CornflowerBlue, "Kill With R");
            }
            else if (tar.Health <= TotalRDamage)
            {
                Drawing.DrawText(hpPos.X, hpPos.Y - 20, System.Drawing.Color.CornflowerBlue, "Kill With R");
            }

        }

        public static int getTotalAAToKill(Obj_AI_Base target)
        {
            var critPercent = Hero.Crit;
            int result = (int) (target.Health/Hero.GetAutoAttackDamage(target, true));
            return result <= 0 ? 1 : result;
        }

        public static void SetRanges()
        {
            var up = 7*(Hero.Level - 1);
            E.Range = 620 + up;
            R.Range = 620 + up;
        }

        private static void OnUpdate(EventArgs args)
        {
            var focusE = menu.Item("CFE").GetValue<bool>();
            var eTarget =
                HeroManager.Enemies.FirstOrDefault(
                    target =>
                        target.IsValidTarget() && Orb.InAutoAttackRange(target) &&
                        target.HasBuff("tristanaechargesound"));
            if (focusE && eTarget != null)
            {
                Orb.ForceTarget(eTarget);
            }
            SetRanges();
            switch (Orb.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.LastHit:
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    break;
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Freeze:
                    break;
                case Orbwalking.OrbwalkingMode.CustomMode:
                    break;
                case Orbwalking.OrbwalkingMode.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        private static void Combo()
        {
            var useQ = menu.Item("CQ").GetValue<bool>();
            var useE = menu.Item("CE").GetValue<bool>();
            var ETarget = ComboTarget();
            if (ETarget != null)
            {

                if (E.IsReady() && useE && TargetECheck(ETarget))
                {
                    E.Cast(ETarget);

                }
                if (Q.IsReady() && useQ && (!E.IsReady() || !TargetECheck(ETarget)))
                {
                    Q.Cast();
                }
                CastR();
            }
            else
            {

            }
        }

        public static bool TargetECheck(AIHeroClient tar)
        {
            return menu.Item("ET" + tar.ChampionName).GetValue<bool>();
        }

        public static bool TargetRCheck(AIHeroClient tar)
        {
            return menu.Item("RT" + tar.ChampionName).GetValue<bool>();
        }

        public static void CastR()
        {
            //checks
            // E charges
            var useR = menu.Item("CR").GetValue<bool>();
            if (useR && R.IsReady())
                foreach (var tar in HeroManager.Enemies)
                {
                    if (tar.IsInvulnerable || !TargetRCheck(tar)) continue;
                    var rdamage = R.GetDamage(tar);
                    var totalDamage = GetEDmg(tar, (GetTristanaEBuff(tar)) + 1) + rdamage;
                    if (totalDamage > tar.Health)
                    {
                        R.Cast(tar);
                    }
                    else if (rdamage > tar.Health)
                    {
                        R.Cast(tar);
                    }
                }
        }

        public static bool TristanaHasBuffE(Obj_AI_Base target)
        {
            return target.HasBuff("tristanaechargesound");
        }

        private static float GetEDmg(Obj_AI_Base target, int charges)
        {
            if (!TristanaHasBuffE(target)) return 0;
            var dmg = E.GetDamage(target);
            var buffCount = charges;
            dmg += (dmg*0.3f*(buffCount - 1));
            return dmg - (target.HPRegenRate*4);
        }

        private static float GetEDmg(Obj_AI_Base target)
        {
            if (!TristanaHasBuffE(target)) return 0;
            var dmg = E.GetDamage(target);
            var buffCount = GetTristanaEBuff(target);
            dmg += (dmg*0.3f*(buffCount - 1));
            return dmg - (target.HPRegenRate*4);
        }

        public static int GetTristanaEBuff(Obj_AI_Base target)
        {
            if (target != null)
                foreach (BuffInstance buff in target.Buffs)
                {
                    if (buff.Name == "tristanaecharge") return buff.Count;
                }

            return 0;
        }

        public static AIHeroClient ComboTarget()
        {
            return TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
        }

        private static void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (unit.IsMe)
            {
                var target2 = ComboTarget();
                if (target2 == null) return;
                if (target.Name == target2.Name)
                {

                }
            }
        }

    }
}
