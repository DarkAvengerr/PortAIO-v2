using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;

using LeagueSharp;
using LeagueSharp.Common;
//using LConsole = LeagueSharp.Console.Console;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace PippyTaric
{
    class Program
    {

        public static bool DebugMode = false;

        private const string champName = "Taric";
        private static readonly Color pippyTaricColor = Color.FromArgb(60, 222, 203);
        private static readonly string[] qStringList = { "Don't use", "Only on me", "Only on ally" };
        private static readonly string buffName = "taricgemcraftbuff";

        private static Spell theQ, theW, theE, theR;
        private static Items.Item tiamat_item, hydra_item, youmuu_item, bc_item, ruined_item;
        private static SpellSlot ignite;
        private static Menu TaricMenu;
        private static Orbwalking.Orbwalker penisOrb;
        private static AIHeroClient target;
        private static Dictionary<string, float> spellInfo;

        private static float animationTime;
        private static bool TryDebug;
        private static bool SWcombo;
        private static bool hasPassive = false;


        /*static SpellData SpellInfo(string name)
        {
        return SpellData.GetSpellData(name);
        }
        */

        public static void LoadStuff()
        {

            if (ObjectManager.Player.ChampionName != champName)
            {
                return;
            }

            MySpellInfo.Initialize();
            spellInfo = MySpellInfo.SpellTable;
            //LConsole.Show();

            //#NeverForgetPrintchat
            Notifications.AddNotification("Pippy Taric Loaded!", 10).SetTextColor(pippyTaricColor);

            //Spells
            theQ = new Spell(SpellSlot.Q, spellInfo["qRange"]);
            theW = new Spell(SpellSlot.W, spellInfo["wRange"], TargetSelector.DamageType.Magical);
            theE = new Spell(SpellSlot.E, spellInfo["eRange"], TargetSelector.DamageType.Magical);
            theR = new Spell(SpellSlot.R, spellInfo["rRange"], TargetSelector.DamageType.Magical);

            //Spell type
            //theW.SetSkillshot(spellInfo["wDelay"], spellInfo["wRange"], float.PositiveInfinity, false, SkillshotType.SkillshotCircle);
            //theR.SetSkillshot(spellInfo["rDelay"], spellInfo["rRange"], float.PositiveInfinity, false, SkillshotType.SkillshotCircle);

            //Do we have ignite? I mean, you, not "we" ayy lmao
            ignite = ObjectManager.Player.GetSpellSlot("summonerdot");

            //Items
            tiamat_item = new Items.Item(3077, 400f);
            hydra_item = new Items.Item(3074, 400f);
            youmuu_item = new Items.Item(3142);
            bc_item = new Items.Item(3144, 550f);
            ruined_item = new Items.Item(31553, 550f);

            //Teh menu
            TaricMenuLoad();

            //Events
            Game.OnUpdate += TaricUpdate;
            Drawing.OnDraw += TaricDraw;
            //AIHeroClient.OnPlayAnimation += TaricAnimation;
            //AIHeroClient.OnProcessSpellCast += TaricProcessSpell;
            //Orbwalking.AfterAttack += TaricAfterAttack;
            //AIHeroClient.OnBuffGain += TaricBuffAdd;
            //Obj_AI_Base.OnBuffLose += TaricBuffRemove;
        }

        static void TaricMenuLoad()
        {
            TaricMenu = new Menu("Pippy Taric", "pippytaric", true);

            //Orbwalker
            var orbMenu = new Menu("Orbwalker Settings", "orbwalking");
            penisOrb = new Orbwalking.Orbwalker(orbMenu);
            TaricMenu.AddSubMenu(orbMenu);

            //Target Selector
            var tsMenu = new Menu("Target Selector", "targetselector");
            TargetSelector.AddToMenu(tsMenu);
            TaricMenu.AddSubMenu(tsMenu);

            //Combo
            var comboMenu = new Menu("Combo Settings", "combo");

            comboMenu.AddItem(new MenuItem("info1", "Normal Combo Settings"));

                var qComboMenu = new Menu("Q Settings", "qcombo");
                qComboMenu.AddItem(new MenuItem("useQ", "Use Q Mode:")).SetValue(new StringList(qStringList, 2));
                qComboMenu.AddItem(new MenuItem("useQslider", "Min Hp% to heal")).SetValue(new Slider(75, 0, 100));
                comboMenu.AddSubMenu(qComboMenu);

                var wComboMenu = new Menu("W Settings", "wcombo");
                wComboMenu.AddItem(new MenuItem("useW", "Use W")).SetValue(true);
                wComboMenu.AddItem(new MenuItem("useWenemies", "   on >= X enemies")).SetValue(new Slider(1, 1, 5));
                comboMenu.AddSubMenu(wComboMenu);

                var eComboMenu = new Menu("E Settings", "ecombo");
                eComboMenu.AddItem(new MenuItem("useE", "Use E")).SetValue(true);
                eComboMenu.AddItem(new MenuItem("useErange", "Max Range to use")).SetValue(new Slider((int)spellInfo["eRange"], 1, (int)spellInfo["eRange"]));
                comboMenu.AddSubMenu(eComboMenu);

                var rComboMenu = new Menu("R Settings", "rcombo");
                rComboMenu.AddItem(new MenuItem("useR", "Use R")).SetValue(true);
                rComboMenu.AddItem(new MenuItem("useRenemies", "   on >= X enemies")).SetValue(new Slider(2, 1, 5));
                comboMenu.AddSubMenu(rComboMenu);

                comboMenu.AddItem(new MenuItem("info2", string.Empty));

                comboMenu.AddItem(new MenuItem("toggleCombo", "Toggle SpellWeaving Combo")).SetValue(
                new KeyBind(90, KeyBindType.Toggle));
                TaricMenu.AddSubMenu(comboMenu);

            //Harass
            var harassMenu = new Menu("Harass Settings", "harass");
            harassMenu.AddItem(new MenuItem("useQharass", "Use Q in harass (only me)")).SetValue(true);
            harassMenu.AddItem(new MenuItem("useQharassSlider", "Min Hp% to heal")).SetValue(new Slider(75, 0, 100));
            harassMenu.AddItem(new MenuItem("separator1", String.Empty));
            harassMenu.AddItem(new MenuItem("useEharass", "Use E in harass (max range)")).SetValue(true);
            TaricMenu.AddSubMenu(harassMenu);

            //Healing
            var healingMenu = new Menu("Healing Settings", "healing");
            healingMenu.AddItem(new MenuItem("ahSelf", "Auto Heal Self")).SetValue(true);
            healingMenu.AddItem(new MenuItem("ahSelfSlider", "at % HP")).SetValue(new Slider(70, 0, 100));
            healingMenu.AddItem(new MenuItem("ahOther", "Auto Heal Other")).SetValue(false);
            healingMenu.AddItem(new MenuItem("ahOtherSlider", "at % HP")).SetValue(new Slider(70, 0, 100));
            TaricMenu.AddSubMenu(healingMenu);

            //Drawing
            var drawMenu = new Menu("Drawing Settings", "draw");
            drawMenu.AddItem(new MenuItem("qRangeDraw", "Draw Q Range")).SetValue(new Circle(true, pippyTaricColor));
            drawMenu.AddItem(new MenuItem("wRangeDraw", "Draw W Range")).SetValue(new Circle(true, pippyTaricColor));
            drawMenu.AddItem(new MenuItem("eRangeDrawMax", "Draw E MAX Range")).SetValue(new Circle(false, Color.Red));
            drawMenu.AddItem(new MenuItem("eRangeDraw", "Draw set E range")).SetValue(new Circle(true, pippyTaricColor));
            drawMenu.AddItem(new MenuItem("rRangeDraw", "Draw R Range")).SetValue(new Circle(true, pippyTaricColor));
            drawMenu.AddItem(new MenuItem("drawHide", "Hide Ranges if not available")).SetValue(false);
            drawMenu.AddItem(new MenuItem("drawTarget", "Draw Target")).SetValue(true);
            drawMenu.AddItem(new MenuItem("drawMode", "Draw Combo Mode")).SetValue(true);
            drawMenu.AddItem(new MenuItem("drawDamage", "Draw Damage on HealthBar")).SetValue(true);
            TaricMenu.AddSubMenu(drawMenu);

            var miscMenu = new Menu("Misc Settings", "misc");
            miscMenu.AddItem(new MenuItem("ksIgnite", "KS with Ignite")).SetValue(true);
            miscMenu.AddItem(new MenuItem("interrupt", "Interrupt Skills")).SetValue(true);
            TaricMenu.AddSubMenu(miscMenu);

            //TaricMenu.AddItem(new MenuItem("doDebug", "Debug Stuff?")).SetValue(true);

            TaricMenu.AddToMainMenu();
        }

        private static void TaricUpdate(EventArgs args)
        {
            target = TargetSelector.GetTarget(GetTargetRange(), TargetSelector.DamageType.Magical);

            //TryDebug = TaricMenu.Item("doDebug").GetValue<bool>();

            SWcombo = TaricMenu.Item("toggleCombo").GetValue<KeyBind>().Active;

            //animationTime = ObjectManager.Player.AttackCastDelay;

            hasPassive = ObjectManager.Player.HasBuff(buffName);

            HealingManager();

            switch (penisOrb.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    if (!SWcombo)
                    {
                        Combo();
                    }
                    else
                    {
                        SpellWeaving();
                    }
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    //LaneClear();
                    break;
            }

            if (TryDebug)
            {
                //LConsole.WriteLine(hasPassive);
            }
        }

        private static float GetTargetRange()
        {
            var eValue = TaricMenu.Item("useErange").GetValue<Slider>().Value;
            var wValue = spellInfo["wRange"];

            if (eValue >= wValue)
            {
                return eValue;
            }

            return wValue;
        }

        private static void Combo()
        {

            var allyList = ObjectManager.Get<AIHeroClient>().ToList().FindAll(ally => ally.IsAlly);
            var enemyList = ObjectManager.Get<AIHeroClient>().ToList().FindAll(enemy => enemy.IsEnemy);

            if (TryDebug)
            {
                //LConsole.WriteLine(TaricMenu.Item("useQ").GetValue<StringList>().SelectedIndex);
                //LConsole.WriteLine("My health percentage : " + ObjectManager.Player.HealthPercent);
            }

            if (target == null)
            {
                return;
            }

            if (theQ.IsReady())
            {
                switch (TaricMenu.Item("useQ").GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        break;
                    case 1:
                        if (ObjectManager.Player.HealthPercent <= TaricMenu.Item("useQslider").GetValue<Slider>().Value)
                        {
                            theQ.CastOnUnit(ObjectManager.Player);
                        }
                        break;
                    case 2:
                        foreach (AIHeroClient ally in allyList)
                        {
                            if (!ally.IsDead)
                            {
                                if (ally.HealthPercent <= TaricMenu.Item("useQslider").GetValue<Slider>().Value)
                                {
                                    theQ.CastOnUnit(ally);
                                }
                            }
                        }
                        break;
                }
            }

            if (theW.IsReady())
            {
                if (TaricMenu.Item("useW").GetValue<bool>())
                {
                    if (ObjectManager.Player.CountEnemiesInRange(spellInfo["wRange"]) >= TaricMenu.Item("useWenemies")//fag
                        .GetValue<Slider>().Value)
                    {
                        theW.Cast();
                    }
                }
            }

            if (theE.IsReady())
            {
                if (TaricMenu.Item("useE").GetValue<bool>())
                {
                    if (ObjectManager.Player.ServerPosition.Distance(target.ServerPosition) <= TaricMenu.Item("useErange")//fag
                        .GetValue<Slider>().Value)
                    {
                        theE.CastOnUnit(target);
                    }
                }
            }

            if (theR.IsReady())
            {
                if (TaricMenu.Item("useR").GetValue<bool>())
                {
                    if (ObjectManager.Player.CountEnemiesInRange(spellInfo["rRange"]) >= TaricMenu.Item("useRenemies")//fag
                        .GetValue<Slider>().Value)
                    {
                        theR.Cast();
                    }
                }
            }
        }

        private static void Harass()
        {
            if (target == null)
            {
                return;
            }

            if (theQ.IsReady())
            {
                if (TaricMenu.Item("useQharass").GetValue<bool>())
                {
                    if (ObjectManager.Player.HealthPercent <= TaricMenu.Item("useQharassSlider").GetValue<Slider>().Value)
                    {
                        theQ.Cast(ObjectManager.Player);
                    }
                }
            }

            if (theE.IsReady())
            {
                if (TaricMenu.Item("useEharass").GetValue<bool>())
                {
                    if (ObjectManager.Player.ServerPosition.Distance(target.ServerPosition) <= spellInfo["eRange"])
                    {
                        theE.CastOnUnit(target);
                    }
                }
            }
        }

        private static void HealingManager()
        {

            bool HealSelf = TaricMenu.Item("ahSelf").GetValue<bool>();
            bool HealOther = TaricMenu.Item("ahOther").GetValue<bool>();

            int HealSelfSlider = TaricMenu.Item("ahSelfSlider").GetValue<Slider>().Value;
            int HealOtherSlider = TaricMenu.Item("ahOtherSlider").GetValue<Slider>().Value;

            List<AIHeroClient> AllyList = ObjectManager.Get<AIHeroClient>().ToList().FindAll(ally => ally.IsAlly);

            if (!theQ.IsReady() || (!HealSelf && !HealOther))
            {
                return;
            }

            if (HealSelf)
            {
                if (ObjectManager.Player.HealthPercent <= HealSelfSlider)
                {
                    theQ.CastOnUnit(ObjectManager.Player);
                }
            }

            if (HealOther)
            {
                foreach (AIHeroClient ally in AllyList)
                {
                    if (ObjectManager.Player.ServerPosition.Distance(ally.ServerPosition) <= spellInfo["qRange"])
                    {
                        if (ally.HealthPercent <= HealOtherSlider)
                        {
                            theQ.CastOnUnit(ally);
                        }
                    }
                }
            }
        }

        private static void SpellWeaving()
        {
            if (SWcombo && penisOrb.ActiveMode == Orbwalking.OrbwalkingMode.Combo && target != null)
            {
                if (theE.IsReady()
                && target.IsValidTarget(spellInfo["eRange"]) && !GetPassiveState())
                {
                    theE.CastOnUnit(target);
                }
                else if (theR.IsReady() && !theE.IsReady()
                && target.IsValidTarget(spellInfo["rRange"]) && !GetPassiveState())
                {
                    theR.Cast();
                }
                else if (theW.IsReady() && !theR.IsReady() && !theE.IsReady()
                && target.IsValidTarget(spellInfo["wRange"]) && !GetPassiveState())
                {
                    theW.Cast();
                }
                else if (theQ.IsReady() && !theW.IsReady() && !theR.IsReady() && !theE.IsReady()
                && target.IsValidTarget(spellInfo["qRange"]) && !GetPassiveState())
                {
                    theQ.CastOnUnit(ObjectManager.Player);
                }
            }
        }

        private static void TaricDraw(EventArgs args)
        {
            bool HideNotReady = TaricMenu.Item("drawHide").GetValue<bool>();
            Vector2 ScreenPos = Drawing.WorldToScreen(ObjectManager.Player.Position);

            if (ObjectManager.Player.IsDead)
            {
                return;
            }

            if (TaricMenu.Item("qRangeDraw").GetValue<Circle>().Active)
            {
                if (!(!theQ.IsReady() && HideNotReady))
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spellInfo["qRange"],
                        TaricMenu.Item("qRangeDraw").GetValue<Circle>().Color);
                }
            }

            if (TaricMenu.Item("wRangeDraw").GetValue<Circle>().Active)
            {
                if (!(!theW.IsReady() && HideNotReady))
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spellInfo["wRange"],
                        TaricMenu.Item("wRangeDraw").GetValue<Circle>().Color);
                }
            }

            if (TaricMenu.Item("eRangeDrawMax").GetValue<Circle>().Active)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, spellInfo["eRange"],
                    TaricMenu.Item("eRangeDrawMax").GetValue<Circle>().Color);       
            }

            if (TaricMenu.Item("eRangeDraw").GetValue<Circle>().Active)
            {
                if (!(!theE.IsReady() && HideNotReady))
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position,
                        TaricMenu.Item("useErange").GetValue<Slider>().Value,
                        TaricMenu.Item("eRangeDraw").GetValue<Circle>().Color);
                }
            }

            if (TaricMenu.Item("rRangeDraw").GetValue<Circle>().Active)
            {
                if (!(!theR.IsReady() && HideNotReady))
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spellInfo["rRange"],
                        TaricMenu.Item("rRangeDraw").GetValue<Circle>().Color);
                }
            }

            if (TaricMenu.Item("drawTarget").GetValue<bool>() && target != null)
            {
                Render.Circle.DrawCircle(target.Position, target.BoundingRadius, Color.Orange, 8);
            }

            if (TaricMenu.Item("drawMode").GetValue<bool>())
            {
                if (TaricMenu.Item("toggleCombo").GetValue<KeyBind>().Active)
                {
                    Drawing.DrawText(ScreenPos[0] - 60, ScreenPos[1] + 50, Color.LimeGreen, "SpellWeaving ON");
                }
                else
                {
                    Drawing.DrawText(ScreenPos[0] - 60, ScreenPos[1] + 50, Color.Red, "SpellWeaving OFF");
                }
            }
        }

        /*private static void TaricBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (TryDebug)
            {
                if (sender.IsMe && args.Buff.Name != "shatterauraself")
                {
                    LConsole.WriteLine("Buff acquired: " + args.Buff.Name.ToString());
                }
            }

            if (sender.IsMe && args.Buff.Name == buffName)
            {
                hasPassive = true;
            }
        }

        private static void TaricBuffRemove(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            if (sender.IsMe && args.Buff.Name == buffName)
            {
                hasPassive = false;
            }
        }*/

        private static bool GetPassiveState()
        {
            return hasPassive;
        }

        private static float CalculateSWDamage(AIHeroClient faggot)
        {
            AIHeroClient Taric = ObjectManager.Player;

            int activeSpells = 0;

            float finalDamage = 0f;

            double spellDamage = 0d;

            var basic = Taric.GetAutoAttackDamage(faggot);
            var passive = Taric.CalcDamage(faggot, Damage.DamageType.Magical, 0.2f * Taric.Armor);
            var totalBasic = basic + passive;

            if (theQ.IsReady())
            {
                activeSpells += 1;
            }

            if (theW.IsReady())
            {
                activeSpells += 1;
                spellDamage += Taric.GetSpellDamage(faggot, SpellSlot.W);
            }

            if (theE.IsReady())
            {
                activeSpells += 1;
                spellDamage += Taric.GetSpellDamage(faggot, SpellSlot.E); //Needs precise tweaking
            }

            if (theR.IsReady())
            {
                activeSpells += 1;
                spellDamage += Taric.GetSpellDamage(faggot, SpellSlot.R);
            }


            if (activeSpells > 0)
            {
                spellDamage += totalBasic * activeSpells;
            }
            else
            {
                spellDamage += totalBasic;
            }
            return 0;
        }
    }
}
