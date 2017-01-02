using System;
using HikiCarry.Core.Plugins;
using HikiCarry.Core.Utilities;
using LeagueSharp;
using LeagueSharp.Common;
using Utilities = HikiCarry.Core.Utilities.Utilities;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HikiCarry.Champions
{
    class Kalista
    {
        internal static Spell Q;
        internal static Spell W;
        internal static Spell E;
        internal static Spell R;

        public Kalista()
        {
            Q = new Spell(SpellSlot.Q, 1150f);
            W = new Spell(SpellSlot.W, 5000f);
            E = new Spell(SpellSlot.E, 1000f);
            R = new Spell(SpellSlot.R, 1500f);

            Q.SetSkillshot(0.25f, 40f, 1200f, true, SkillshotType.SkillshotLine);

            var comboMenu = new Menu("Combo Settings", "Combo Settings");
            {
                comboMenu.AddItem(new MenuItem("qCombo", "Use Q",true).SetValue(true));
                comboMenu.AddItem(new MenuItem("eCombo", "Use E",true).SetValue(true));
                comboMenu.AddItem(new MenuItem("combo", "Combo!", true).SetValue(new KeyBind(32, KeyBindType.Press)));
                Initializer.Config.AddSubMenu(comboMenu);
            }

            var harassMenu = new Menu("Harass Settings", "Harass Settings");
            {
                harassMenu.AddItem(new MenuItem("qHarass", "Use Q",true).SetValue(true));
                harassMenu.AddItem(new MenuItem("eHarass", "Use E",true).SetValue(true));
                harassMenu.AddItem(new MenuItem("eSpearCount", "If Enemy Spear Count >= ", true).SetValue(new Slider(3, 0, 10)));
                harassMenu.AddItem(new MenuItem("manaHarass", "Harass Mana Manager", true).SetValue(new Slider(20, 1, 99)));
                Initializer.Config.AddSubMenu(harassMenu);
            }

            var laneMenu = new Menu("LaneClear Settings", "LaneClear Settings");
            {
                laneMenu.AddItem(new MenuItem("eClear", "Use E", true).SetValue(true));
                laneMenu.AddItem(new MenuItem("eClearCount", "If Can Kill Minion >= ", true).SetValue(new Slider(2, 1, 5)));
                laneMenu.AddItem(new MenuItem("manaClear", "Clear Mana Manager", true).SetValue(new Slider(20, 1, 99)));
                Initializer.Config.AddSubMenu(laneMenu);
            }

            var jungMenu = new Menu("JungleClear Settings", "JungleClear Settings");
            {
                jungMenu.AddItem(new MenuItem("qJungle", "Use Q", true).SetValue(true));
                jungMenu.AddItem(new MenuItem("eJungle", "Use E", true).SetValue(true));
                jungMenu.AddItem(new MenuItem("manaJungle", "Jungle Mana Manager", true).SetValue(new Slider(20, 1, 99)));
                Initializer.Config.AddSubMenu(jungMenu);
            }

            var ksMenu = new Menu("KillSteal Settings", "KillSteal Settings");
            {
                ksMenu.AddItem(new MenuItem("qKS", "Use Q", true).SetValue(true));
                ksMenu.AddItem(new MenuItem("eKS", "Use E", true).SetValue(true));
                Initializer.Config.AddSubMenu(ksMenu);
            }

            var miscMenu = new Menu("Miscellaneous", "Miscellaneous");
            {
                var lastJoke = new Menu("Last Joke Settings", "Last Joke Settings");
                {
                    lastJoke.AddItem(new MenuItem("last.joke", "Last Joke", true).SetValue(true));
                    lastJoke.AddItem(new MenuItem("last.joke.hp", "Kalista HP Percent", true).SetValue(new Slider(2, 1, 99)));
                    miscMenu.AddSubMenu(lastJoke);
                }
                miscMenu.AddItem(new MenuItem("qImmobile", "Auto Q to Immobile Target", true).SetValue(true));
                Initializer.Config.AddSubMenu(miscMenu);
            }

            var wCombo = new Menu("Wombo Combo with R", "Wombo Combo with R"); // beta
            {
                var balista = new Menu("Balista", "Balista");
                {
                    balista.AddItem(new MenuItem("use.balista", "Balista Active", true).SetValue(true));
                    balista.AddItem(new MenuItem("balista.maxrange", "Balista Max Range", true).SetValue(new Slider(700, 100, 1500)));
                    balista.AddItem(new MenuItem("balista.minrange", "Balista Min Range", true).SetValue(new Slider(700, 100, 1500)));
                    wCombo.AddSubMenu(balista);
                }
                var skalista = new Menu("Skalista", "Skalista");
                {
                    skalista.AddItem(new MenuItem("use.skalista", "SKalista Active", true).SetValue(true));
                    skalista.AddItem(new MenuItem("skalista.maxrange", "SKalista Max Range", true).SetValue(new Slider(700, 100, 1500)));
                    skalista.AddItem(new MenuItem("skalista.minrange", "SKalista Min Range", true).SetValue(new Slider(700, 100, 1500)));
                    wCombo.AddSubMenu(skalista);
                }
                Initializer.Config.AddSubMenu(wCombo);
            }

            var drawMenu = new Menu("Draw Settings", "Draw Settings");
            {
                var drawDamageMenu = new MenuItem("RushDrawEDamage", "Combo Damage").SetValue(true);
                var drawFill = new MenuItem("RushDrawEDamageFill", "Combo Damage Fill").SetValue(new Circle(true, System.Drawing.Color.Gold));

                drawMenu.SubMenu("Damage Draws").AddItem(drawDamageMenu);
                drawMenu.SubMenu("Damage Draws").AddItem(drawFill);

                //DamageIndicator.DamageToUnit = KalistaLogics.ChampionTotalDamage;
                //DamageIndicator.Enabled = drawDamageMenu.GetValue<bool>();
                //DamageIndicator.Fill = drawFill.GetValue<Circle>().Active;
                //DamageIndicator.FillColor = drawFill.GetValue<Circle>().Color;

                drawDamageMenu.ValueChanged +=
                delegate (object sender, OnValueChangeEventArgs eventArgs)
                {
                    //DamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
                };

                drawFill.ValueChanged +=
                delegate (object sender, OnValueChangeEventArgs eventArgs)
                {
                    //DamageIndicator.Fill = eventArgs.GetNewValue<Circle>().Active;
                    //DamageIndicator.FillColor = eventArgs.GetNewValue<Circle>().Color;
                };
                Initializer.Config.AddSubMenu(drawMenu);
            }

            Initializer.Config.AddItem(new MenuItem("saveSupport", "Save Support [R]",true).SetValue(true));
            Initializer.Config.AddItem(new MenuItem("savePercent", "Save Support Health Percent",true).SetValue(new Slider(10, 1, 99)));
            Initializer.Config.AddItem(new MenuItem("calculator", "E Damage Calculator",true).SetValue(new StringList(new[] { "Custom Calculator", "Common Calculator" })));

            Game.OnUpdate += KalistaOnUpdate;
        }

        private void KalistaOnUpdate(EventArgs args)
        {
            switch (Initializer.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Clear();
                    Jungle();
                    break;
            }
            if (Utilities.Enabled("use.balista"))
            {
                KalistaLogics.Balista(Utilities.Slider("balista.minrange"), Utilities.Slider("balista.maxrange"), R);
            }
            if (Utilities.Enabled("use.skalista"))
            {
                KalistaLogics.SKalista(Utilities.Slider("skalista.minrange"), Utilities.Slider("skalista.maxrange"), R);
            }
            if (Utilities.Enabled("qKS"))
            {
                KalistaLogics.KillStealWithPierce();
            }
            if (Utilities.Enabled("eKS"))
            {
                KalistaLogics.KillStealWithRend();
            }
            if (Utilities.Enabled("qImmobile"))
            {
                KalistaLogics.ImmobilePierce();
            }
            if (Utilities.Enabled("saveSupport"))
            {
                KalistaLogics.SupportProtector(R);
            }
        }
        private static void Combo()
        {
            if (Q.IsReady() && Utilities.Enabled("qCombo"))
            {
                KalistaLogics.PierceCombo();
            }
            if (E.IsReady() && Utilities.Enabled("eCombo"))
            {
                KalistaLogics.RendCombo();
            }
        }

        private static void Harass()
        {
            if (ObjectManager.Player.ManaPercent < Utilities.Slider("manaHarass"))
            {
                return;
            }
            if (Q.IsReady() && Utilities.Enabled("qHarass"))
            {
                KalistaLogics.PierceCombo();
            }
            if (E.IsReady() && Utilities.Enabled("eHarass"))
            {
                KalistaLogics.RendHarass(Utilities.Slider("eSpearCount"));
            }
        }

        private static void Clear()
        {
            if (ObjectManager.Player.ManaPercent < Utilities.Slider("manaClear"))
            {
                return;
            }


            if (E.IsReady() && Utilities.Enabled("eClear"))
            {
                KalistaLogics.RendClear(Utilities.Slider("eClearCount"));
            }
        }

        private static void Jungle()
        {
            if (ObjectManager.Player.ManaPercent < Utilities.Slider("manaJungle"))
            {
                return;
            }

            if (Q.IsReady() && Utilities.Enabled("qJungle"))
            {
                KalistaLogics.PierceJungleClear(Q);
            }
            if (E.IsReady() && Utilities.Enabled("eJungle"))
            {
                KalistaLogics.RendJungleClear();
            }
        }
    }
}
