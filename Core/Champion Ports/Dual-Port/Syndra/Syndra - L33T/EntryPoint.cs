using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SyndraL33T
{
    public class EntryPoint
    {
        public static Menu Menu;
        public static AIHeroClient Player;
        private Orbwalking.Orbwalker orbwalker;

        public EntryPoint(AIHeroClient player, Menu menu)
        {
            Player = player;
            Menu = menu;
        }

        public void RegisterCallbacks()
        {
            GameObject.OnCreate += ObjectCache.OnCreate;
            GameObject.OnCreate += Collision.OnCreate;

            GameObject.OnDelete += ObjectCache.OnDelete;
            GameObject.OnDelete += Collision.OnDelete;

            Game.OnUpdate += Game_OnUpdate;
            Game.OnUpdate += ObjectCache.OnUpdate;

            Game.OnProcessPacket += SphereManager.OnProcessPacket;

            Obj_AI_Base.OnSpellCast += SphereManager.OnProcessSpellCast;
            Obj_AI_Base.OnSpellCast += Collision.OnProcessSpellCast;
            Obj_AI_Base.OnSpellCast += Mechanics.OnProcessSpellCast;


            AntiGapcloser.OnEnemyGapcloser += Mechanics.OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Mechanics.OnInterruptableTarget;

            Drawing.OnDraw += Graphics.OnDraw;
        }

        private void Game_OnUpdate(EventArgs args)
        {
            if (orbwalker == null)
            {
                return;
            }

            var target = TargetSelector.GetTarget(Player, 1337f, TargetSelector.DamageType.Physical);
            switch (orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Mechanics.ProcessCombo(target);
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Mechanics.ProcessFarm(true);
                    Mechanics.JungleFarm();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    Mechanics.ProcessFarm();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    if (Menu.Item("l33t.stds.harass.usemixed").GetValue<bool>())
                    {
                        Mechanics.ProcessHarass(target);
                    }
                    break;
            }
            if (Menu.Item("l33t.stds.harass.togglekey").GetValue<KeyBind>().Active)
            {
                Mechanics.ProcessHarass(target);
            }
            if (Menu.Item("l33t.stds.qesettings.qetocursor").GetValue<KeyBind>().Active &&
                Mechanics.Spells[SpellSlot.E].IsReady() && Mechanics.Spells[SpellSlot.Q].IsReady())
            {
                foreach (var enemy in
                    ObjectCache.GetHeroes()
                        .Where(
                            e =>
                                e.IsValidTarget() &&
                                Player.Distance(e.Position, true) <=
                                Math.Pow(Mechanics.Spells[SpellSlot.SphereE].Range, 2) &&
                                e.Distance(Game.CursorPos, true) <= 22500))
                {
                    Mechanics.ProcessSphereE(enemy);
                }
            }
            if (Menu.Item("l33t.stds.ks.togglekey").GetValue<KeyBind>().Active)
            {
                Mechanics.ProcessKillSteal();
            }
        }

        public void RegisterMenu()
        {
            orbwalker = new Orbwalking.Orbwalker(Menu.AddSubMenu(new Menu("Orbwalker", "l33t.stds.orbwalker")));
            TargetSelector.AddToMenu(Menu.AddSubMenu(new Menu("TargetSelector", "l33t.stds.targetselector")));

            var combo = new Menu("Combo Settings", "l33t.stds.combo");
            combo.AddItem(new MenuItem("l33t.stds.combo.useQ", "Use Dark Sphere (Q)")).SetValue(true);
            combo.AddItem(new MenuItem("l33t.stds.combo.useW", "Use Force of Will (W)")).SetValue(true);
            combo.AddItem(new MenuItem("l33t.stds.combo.useE", "Use Scatter the Weak (E)")).SetValue(true);
            combo.AddItem(new MenuItem("l33t.stds.combo.useR", "Use Unleashed Power (R)")).SetValue(true);
            combo.AddItem(
                new MenuItem("l33t.stds.combo.useQE", "[Combination] Use Dark Sphere (Q) + Scatter the Weak (E)"))
                .SetValue(true);
            Menu.AddSubMenu(combo);
            var harass = new Menu("Harass Settings", "l33t.stds.harass");
            harass.AddItem(new MenuItem("l33t.stds.harass.useQ", "Use Dark Sphere (Q)")).SetValue(true);
            harass.AddItem(new MenuItem("l33t.stds.harass.useQAA", "Use Dark Sphere (Q) when enemy auto attacks (Me)"))
                .SetValue(false);
            harass.AddItem(new MenuItem("l33t.stds.harass.useW", "Use Force of Will (W)")).SetValue(true);
            harass.AddItem(new MenuItem("l33t.stds.harass.useE", "Use Scatter the Weak (E)")).SetValue(true);
            harass.AddItem(
                new MenuItem("l33t.stds.harass.useQE", "[Combination] Use Dark Sphere (Q) + Scatter the Weak (E)"))
                .SetValue(true);
            harass.AddItem(
                new MenuItem("l33t.stds.harass.turret", "[Condition] Disable Harass under turret").SetValue(false));
            harass.AddItem(
                new MenuItem("l33t.stds.harass.mana", "[Condition] Harass if mana is above").SetValue(new Slider(30)));
            harass.AddItem(
                new MenuItem("l33t.stds.harass.usemixed", "[Condition] Use Orbwalker 'Mixed' key for Harass").SetValue(
                    true));
            harass.AddItem(
                new MenuItem("l33t.stds.harass.togglekey", "[Condition] Harass Toggle Key").SetValue(
                    new KeyBind('Z', KeyBindType.Toggle)));
            Menu.AddSubMenu(harass);
            var farm = new Menu("Farming Settings", "l33t.stds.farming");
            farm.AddItem(
                new MenuItem("l33t.stds.farming.qmode", "Use Dark Sphere (Q)").SetValue(
                    new StringList(new[] { "Freeze", "Lane Clear", "Both", "None" }, 2)));
            farm.AddItem(
                new MenuItem("l33t.stds.farming.wmode", "Use Force of Will (W)").SetValue(
                    new StringList(new[] { "Freeze", "Lane Clear", "Both", "None" }, 1)));
            farm.AddItem(
                new MenuItem("l33t.stds.farming.emode", "Use Scatter the Weak (E)").SetValue(
                    new StringList(new[] { "Freeze", "Lane Clear", "Both", "None" }, 3)));
            farm.AddItem(new MenuItem("l33t.stds.farming.farmmana", "[Condition] Minimum % of Mana for Last Hit").SetValue(new Slider(30)));
            farm.AddItem(new MenuItem("l33t.stds.farming.lcmana", "[Condition] Minimum % of Mana for Lane Clear").SetValue(new Slider(30)));
            Menu.AddSubMenu(farm);
            var jungleFarm = new Menu("Jungle Farm Settings", "l33t.stds.junglefarming");
            jungleFarm.AddItem(new MenuItem("l33t.stds.junglefarming.qmode", "Use Dark Sphere (Q)").SetValue(true));
            jungleFarm.AddItem(new MenuItem("l33t.stds.junglefarming.wmode", "Use Force of Will (W)").SetValue(true));
            jungleFarm.AddItem(new MenuItem("l33t.stds.junglefarming.emode", "Use Scatter the Weak (E)").SetValue(true));
            Menu.AddSubMenu(jungleFarm);
            var ks = new Menu("Killsteal Settings", "l33t.stds.ks");
            ks.AddItem(new MenuItem("l33t.stds.ks.useQ", "Use Dark Sphere (Q)")).SetValue(true);
            ks.AddItem(new MenuItem("l33t.stds.ks.useW", "Use Force of Will (W)")).SetValue(true);
            ks.AddItem(new MenuItem("l33t.stds.ks.useE", "Use Scatter the Weak (E)")).SetValue(true);
            ks.AddItem(new MenuItem("l33t.stds.ks.useQE", "[Combination] Use Dark Sphere (Q) + Scatter the Weak (E)"))
                .SetValue(true);
            ks.AddItem(new MenuItem("l33t.stds.ks.useR", "Use Unleashed Power (R)")).SetValue(true);
            ks.AddItem(
                new MenuItem("l33t.stds.ks.togglekey", "[Condition] Killsteal Toggle Key").SetValue(
                    new KeyBind('U', KeyBindType.Toggle)));
            ks.AddItem(new MenuItem("l33t.stds.ks.spacer0", ""));
            ks.AddItem(
                new MenuItem("l33t.stds.ks.usefqe", "[Flash Kill] Use Dark Sphere (Q) + Scatter the Weak (E)").SetValue(
                    true));
            ks.AddItem(new MenuItem("l33t.stds.ks.usefr", "[Flash Kill] Use Unleashed Power (R)").SetValue(false));
            var ksEnemies = new Menu("Use Flash Kill on", "l33t.stds.ks.kenemies");
            foreach (var hero in ObjectCache.GetHeroes())
            {
                ksEnemies.AddItem(
                    new MenuItem("l33t.stds.ks.kenemies." + hero.ChampionName, hero.ChampionName).SetValue(true));
            }
            ks.AddSubMenu(ksEnemies);
            ks.AddItem(new MenuItem("l33t.stds.ks.maxenemy", "Max Enemies").SetValue(new Slider(2, 1, 5)));
            ks.AddItem(
                new MenuItem("l33t.stds.ks.mana", "[Flash Kill] Flash only if enough mana for combo.").SetValue(false));
            Menu.AddSubMenu(ks);
            var ultimateSettings = new Menu("Unleashed Power Settings", "l33t.stds.ups");
            foreach (var hero in ObjectCache.GetHeroes())
            {
                ultimateSettings.AddItem(
                    new MenuItem(
                        "l33t.stds.ups.ignore." + hero.ChampionName,
                        "[Usage] Use Unleashed Power on " + hero.ChampionName)).SetValue(true);
            }
            ultimateSettings.AddItem(new MenuItem("l33t.stds.ups.spacer0", ""));
            ultimateSettings.AddItem(
                new MenuItem("l33t.stds.ups.spacer1", "[ Don't use Unleashed Power (R) if can be killed with ]"));
            ultimateSettings.AddItem(
                new MenuItem("l33t.stds.ups.disable", "Damage From").SetValue(
                    new StringList(new[] { "All", "Either one", "None" })));
            ultimateSettings.AddItem(new MenuItem("l33t.stds.ups.disable.Q", "Dark Sphere (Q)").SetValue(true));
            ultimateSettings.AddItem(new MenuItem("l33t.stds.ups.disable.W", "Dark Sphere (W)").SetValue(true));
            ultimateSettings.AddItem(new MenuItem("l33t.stds.ups.disable.E", "Dark Sphere (E)").SetValue(true));
            ultimateSettings.AddItem(new MenuItem("l33t.stds.ups.disable.AA", "1 x Auto Attack").SetValue(true));
            ultimateSettings.AddItem(new MenuItem("l33t.stds.ups.spacer2", ""));
            ultimateSettings.AddItem(
                new MenuItem("l33t.stds.ups.disablebuff", "[ Don't use Unleased Power (R) if enemy has buff ]"));
            ultimateSettings.AddItem(
                new MenuItem("l33t.stds.ups.disablebuff.undying", "Tryndamere's Undying Rage (R)").SetValue(true));
            ultimateSettings.AddItem(
                new MenuItem("l33t.stds.ups.disablebuff.judicator", "Kayle's Intervention (R)").SetValue(true));
            ultimateSettings.AddItem(
                new MenuItem("l33t.stds.ups.disablebuff.alistar", "Alistar's Unbreakable Will (R)").SetValue(true));
            ultimateSettings.AddItem(
                new MenuItem("l33t.stds.ups.disablebuff.zilean", "Zilean's Chronoshift (R)").SetValue(true));
            ultimateSettings.AddItem(
                new MenuItem("l33t.stds.ups.disablebuff.zac", "Zac's Cell Division (Passive)").SetValue(true));
            ultimateSettings.AddItem(
                new MenuItem("l33t.stds.ups.disablebuff.aatrox", "Aatrox's Blood Well (Passive)").SetValue(true));
            ultimateSettings.AddItem(
                new MenuItem("l33t.stds.ups.disablebuff.sivir", "Sivir's Spell Shield (E)").SetValue(true));
            ultimateSettings.AddItem(
                new MenuItem("l33t.stds.ups.disablebuff.morgana", "Morgana's Black Shield (E)").SetValue(true));

            Menu.AddSubMenu(ultimateSettings);

            var qeSettings = new Menu("Dark Sphere (Q) + Scatter of the Weak (E) Settings", "l33t.stds.qesettings");
            qeSettings.AddItem(
                new MenuItem(
                    "l33t.stds.qesettings.qedelay", "[Combination] Dark Sphere (Q) + Scatter the Weak (E) delay")
                    .SetValue(new Slider(1, 0, 150)));
            qeSettings.AddItem(
                new MenuItem(
                    "l33t.stds.qesettings.qerange", "[Condition] Dark Sphere (Q) + Scatter the Weak (E) max range %")
                    .SetValue(new Slider(100)));
            qeSettings.AddItem(
                new MenuItem(
                    "l33t.stds.qesettings.qetocursor", "Use Dark Sphere (Q) + Scatter the Weak (E) to enemy near cursor")
                    .SetValue(new KeyBind('T', KeyBindType.Press)));
            Menu.AddSubMenu(qeSettings);

            var misc = new Menu("Miscellaneous", "l33t.stds.misc");
            misc.AddItem(
                new MenuItem(
                    "l33t.stds.misc.ignitecd", "[Summoner Spell] Use Ignite only when all spells are on cooldown")
                    .SetValue(false));
            misc.AddItem(new MenuItem("l33t.stds.misc.antigapcloser", "Use Anti Gapcloser").SetValue(true));
            misc.AddItem(
                new MenuItem("l33t.stds.misc.interrupt", "Use Interrupt on interruptable important spells").SetValue(
                    true));
            misc.AddItem(new MenuItem("l33t.stds.misc.welcomesound", "Startup Sound (Welcome)").SetValue(false));
            misc.AddItem(new MenuItem("l33t.stds.misc.sounds", "In-Game Sounds (Random)").SetValue(false));
            Menu.AddSubMenu(misc);

            var drawing = new Menu("Drawing", "l33t.stds.drawing");
            drawing.AddItem(new MenuItem("l33t.stds.drawing.enabledraw", "Enable Drawing").SetValue(true));
            drawing.AddItem(new MenuItem("l33t.stds.drawing.classic", "Draw Classic Circles").SetValue(false));
            drawing.AddItem(new MenuItem("l33t.stds.drawing.spacer0", ""));
            drawing.AddItem(
                new MenuItem("l33t.stds.drawing.drawQ", "Draw Dark Sphere (Q) Range").SetValue(
                    new Circle(true, Color.FromArgb(100, 255, 0, 255))));
            drawing.AddItem(
                new MenuItem("l33t.stds.drawing.drawW", "Draw Force of Will (W) Range").SetValue(
                    new Circle(false, Color.FromArgb(100, 255, 0, 255))));
            drawing.AddItem(
                new MenuItem("l33t.stds.drawing.drawE", "Draw Scatter the Weak (E) Range").SetValue(
                    new Circle(false, Color.FromArgb(100, 255, 0, 255))));
            drawing.AddItem(
                new MenuItem("l33t.stds.drawing.drawR", "Draw Unleashed Power (R) Range").SetValue(
                    new Circle(false, Color.FromArgb(100, 255, 0, 255))));
            drawing.AddItem(
                new MenuItem("l33t.stds.drawing.drawSphereE", "Draw Dark Sphere (Q) + Scatter the Weak (E) Range")
                    .SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
            drawing.AddItem(
                new MenuItem(
                    "l33t.stds.drawing.drawQEC", "Draw Dark Sphere (Q) + Scatter the Weak (E) Cursor Indicator")
                    .SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
            drawing.AddItem(new MenuItem("l33t.stds.drawing.spacer1", ""));
            drawing.AddItem(
                new MenuItem("l33t.stds.drawing.drawQEMAP", "Draw Dark Sphere (Q) + Scatter the Weak (E) Parameters")
                    .SetValue(true));
            drawing.AddItem(
                new MenuItem("l33t.stds.drawing.drawWMAP", "Draw Force of Will (W) Parameters").SetValue(true));
            drawing.AddItem(new MenuItem("l33t.stds.drawing.drawGank", "Draw Gankable Indicator").SetValue(true));
            drawing.AddItem(
                new MenuItem("l33t.stds.drawing.drawHPFill", "Draw After Combo Health Points").SetValue(true));
            drawing.AddItem(new MenuItem("l33t.stds.drawing.drawHUD", "Draw Heads-up Display").SetValue(true));
            drawing.AddItem(new MenuItem("l33t.stds.drawing.drawKillText", "Draw Kill Text").SetValue(true));
            drawing.AddItem(
                new MenuItem("l33t.stds.drawing.drawKillTextHP", "Draw % Health Points after Combo Text").SetValue(true));
            drawing.AddItem(new MenuItem("l33t.stds.drawing.drawDebug", "Draw Debug").SetValue(false));
            Menu.AddSubMenu(drawing);

            Menu.AddItem(new MenuItem("l33ts.stds.spacer0", ""));
            Menu.AddItem(new MenuItem("l33t.stds.mode", "Mode"))
                .SetValue(new StringList(new[] { "Target Focused", "Team Focused" }, 1));

            Menu.AddToMainMenu();
        }

        public static void RegisterSpells()
        {
            Mechanics.Spells = new Dictionary<SpellSlot, SpellData>
            {
                {
                    SpellSlot.Q,
                    new SpellData(
                        EloBuddy.SpellSlot.Q, TargetSelector.DamageType.Magical, 1, "SyndraQ", 0.6f, 150f, 800f,
                        int.MaxValue, null, @base => @base.GetDarkSphereDamage())
                },
                {
                    SpellSlot.W,
                    new SpellData(
                        EloBuddy.SpellSlot.W, TargetSelector.DamageType.Magical, 3, "syndrawcast", 0.25f, 210f, 950f,
                        1450, null, @base => @base.GetForceOfWillDamage())
                },
                {
                    SpellSlot.E,
                    new SpellData(
                        EloBuddy.SpellSlot.E, TargetSelector.DamageType.Magical, 4, "SyndraE", 0.3f, 0f, 0f, 1601,
                        () =>
                            (float)
                                (950 *
                                 (ObjectManager.Player.Spellbook.GetSpell(EloBuddy.SpellSlot.E).Level < 5 ? 0.5 : 1)),
                        @base => @base.GetScatterTheWeakDamage())
                },
                {
                    SpellSlot.R,
                    new SpellData(
                        EloBuddy.SpellSlot.R, TargetSelector.DamageType.Magical, 2, "SyndraR", 0.5f, 0f, 0f, 0,
                        () => ObjectManager.Player.Spellbook.GetSpell(EloBuddy.SpellSlot.R).Level < 3 ? 675 : 750,
                        @base => @base.GetUnleashedPowerDamage())
                },
                {
                    SpellSlot.SphereE,
                    new SpellData(
                        EloBuddy.SpellSlot.E, TargetSelector.DamageType.Magical, 1, "SyndraE", 0.98f, 55f, 1202f,
                        9000, () => Menu.Item("l33t.stds.qesettings.qerange").GetValue<Slider>().Value * .01f * 1292f)
                }
            };
            Mechanics.IgniteSpell = new Spell(Player.GetSpellSlot("SummonerDot"), 600f);
            Mechanics.FlashSpell = new Spell(Player.GetSpellSlot("summonerflash"), 400f);
        }
    }
}