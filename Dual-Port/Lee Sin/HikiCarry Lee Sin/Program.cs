using System;
using System.Drawing;
using System.Linq;
using HikiCarry_Lee_Sin.Core;
using HikiCarry_Lee_Sin.Plugins;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HikiCarry_Lee_Sin
{
    class Program
    {
        public static Menu Config;
        public static Orbwalking.Orbwalker Orbwalker;

        public static void Main()
        {
            Game_OnGameLoad();
        }

        private static void Game_OnGameLoad()
        {
            Spells.Init();
            Config = new Menu("Hikigaya - LeeSins", "Hikigaya - LeeSins", true);
            {
                Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker Settings"));

                var comboMenu = new Menu("-> Combo Settings", "-> Combo Settings");
                {
                    comboMenu.AddItem(new MenuItem("q1.combo", " Use (Q1) ").SetValue(true));
                    comboMenu.AddItem(new MenuItem("q2.combo", " Use (Q2) ").SetValue(true));

                    comboMenu.AddItem(new MenuItem("w1.combo", " Use (W1) ").SetValue(true));
                    comboMenu.AddItem(new MenuItem("w2.combo", " Use (W2) ").SetValue(true));

                    comboMenu.AddItem(new MenuItem("e1.combo", " Use (E1) ").SetValue(true));
                    comboMenu.AddItem(new MenuItem("e2.combo", " Use (E2) ").SetValue(true));

                    comboMenu.AddItem(new MenuItem("r.combo", " Use (R) ").SetValue(true)).SetTooltip("only using when enemy is killable");

                    comboMenu.AddItem(new MenuItem("passive.usage", "Passive Usage ?").SetValue(true)).SetTooltip("uses passive usage example: (q1 -> aa -> q2 -> aa -> w1 -> aa -> w2 -> e1 -> aa -> e2 -> aa)");
                    Config.AddSubMenu(comboMenu);
                }

                var jungMenu = new Menu("-> Jungle Settings", "-> Jungle Settings");
                {
                    jungMenu.AddItem(new MenuItem("q1.jungle", " Use (Q1) ").SetValue(true));
                    jungMenu.AddItem(new MenuItem("q2.jungle", " Use (Q2) ").SetValue(true));

                    jungMenu.AddItem(new MenuItem("w1.jungle", " Use (W1) ").SetValue(true));
                    jungMenu.AddItem(new MenuItem("w2.jungle", " Use (W2) ").SetValue(true));

                    jungMenu.AddItem(new MenuItem("e1.jungle", " Use (E1) ").SetValue(true));
                    jungMenu.AddItem(new MenuItem("e2.jungle", " Use (E2) ").SetValue(true));

                    Config.AddSubMenu(jungMenu);
                }

                var waveMenu = new Menu("-> Wave Clear Settings", "-> Wave Clear Settings");
                {
                    waveMenu.AddItem(new MenuItem("q1.wave", " Use (Q1) ").SetValue(true));
                    waveMenu.AddItem(new MenuItem("e1.wave", " Use (E1) ").SetValue(true));
                    Config.AddSubMenu(waveMenu);
                }

                var insecMenu = new Menu("-> Insec Settings", "-> Insec Settings");
                {
                    insecMenu.AddItem(new MenuItem("masterracec0mb0x", "          currently only supports click insec").SetFontStyle(FontStyle.Bold));
                    insecMenu.AddItem(new MenuItem("insec.style", "Insec Method").SetValue(new StringList(new[] { "Click Target" })));
                    insecMenu.AddItem(new MenuItem("insec.to", "Insec to ?").SetValue(new StringList(new[] { "Ally", "Tower", "Cursor Position" }, 2)));
                    insecMenu.AddItem(new MenuItem("insec.distance", "Min. Insec Distance").SetValue(new Slider(300, 1, 375)));
                    insecMenu.AddItem(new MenuItem("obj.usage", "Object Usage ?").SetValue(false)).SetTooltip("uses objects for insec (jungle mobs/enemy minions/enemies)");
                    insecMenu.AddItem(new MenuItem("flash.usage", "Flash Usage ?").SetValue(false)).SetTooltip("uses flash for insec");

                    Config.AddSubMenu(insecMenu);
                }

                var activatorMenu = new Menu("-> Activator Settings", "-> Activator Settings");
                {
                    var hydraMenu = new Menu(":: Hydra Settings", ":: Hydra Settings");
                    {
                        hydraMenu.AddItem(new MenuItem("use.hydra", "Use Ravenous Hydra (Combo)").SetValue(true)); 
                        hydraMenu.AddItem(new MenuItem("use.titanic", "Use Titanic Hydra (Combo)").SetValue(true));
                        hydraMenu.AddItem(new MenuItem("use.tiamat", "Use Tiamat(Combo)").SetValue(true));
                        activatorMenu.AddSubMenu(hydraMenu);
                    }
                    var youmuuMenu = new Menu(":: Youmuu Settings", ":: Youmuu Settings");
                    {
                        youmuuMenu.AddItem(new MenuItem("use.youmuu", "Use Youmuu (Combo)").SetValue(true));
                        activatorMenu.AddSubMenu(youmuuMenu);
                    }
                    var botrkMenu = new Menu(":: Botrk Settings", ":: Botrk Settings");
                    {
                        botrkMenu.AddItem(new MenuItem("use.botrk", "Use Botrk (Combo)").SetValue(true));
                        botrkMenu.AddItem(new MenuItem("botrk.hp", "If Lee HP < %").SetValue(new Slider(20, 1, 99)));
                        botrkMenu.AddItem(new MenuItem("botrk.enemy.hp", "If Enemy HP < %").SetValue(new Slider(20, 1, 99)));
                        activatorMenu.AddSubMenu(botrkMenu);
                    }
                    var bilgewaterMenu = new Menu(":: Bilgewater Settings", ":: Bilgewater Settings");
                    {
                        bilgewaterMenu.AddItem(new MenuItem("use.bilgewater", "Use Bilgewater (Combo)").SetValue(true));
                        bilgewaterMenu.AddItem(new MenuItem("bilgewater.hp", "If Lee HP < %").SetValue(new Slider(20, 1, 99)));
                        bilgewaterMenu.AddItem(new MenuItem("bilgewater.enemy.hp", "If Enemy HP < %").SetValue(new Slider(20, 1, 99)));
                        activatorMenu.AddSubMenu(bilgewaterMenu);
                    }
                    var randuinMenu = new Menu(":: Randuin Settings", ":: Randuin Settings");
                    {
                        randuinMenu.AddItem(new MenuItem("use.randuin", "Use Randuin (Combo)").SetValue(true));
                        randuinMenu.AddItem(new MenuItem("randuin.min.enemy.count", "Min. Enemy Count").SetValue(new Slider(3, 1, 5)));
                        activatorMenu.AddSubMenu(randuinMenu);
                    }
                    Config.AddSubMenu(activatorMenu);
                }
                var drawMenu = new Menu("-> Draw Settings", "-> Draw Settings");
                {
                    var skillDraw = new Menu(":: Skill Draws", ":: Skill Draws");
                    {
                        skillDraw.AddItem(new MenuItem("q.draw", "Q Range").SetValue(new Circle(true, Color.White)));
                        skillDraw.AddItem(new MenuItem("q2.draw", "Q2 Range").SetValue(new Circle(true, Color.DarkSeaGreen)));
                        skillDraw.AddItem(new MenuItem("w.draw", "W Range").SetValue(new Circle(false, Color.Gold)));
                        skillDraw.AddItem(new MenuItem("e.draw", "E Range").SetValue(new Circle(false, Color.DodgerBlue)));
                        skillDraw.AddItem(new MenuItem("e2.draw", "E2 Range").SetValue(new Circle(false, Color.SeaGreen)));
                        skillDraw.AddItem(new MenuItem("r.draw", "R Range").SetValue(new Circle(false, Color.GreenYellow)));
                        drawMenu.AddSubMenu(skillDraw);
                    }
                    var insecDraw = new Menu(":: Insec Draws", ":: Insec Draws");
                    {
                        insecDraw.AddItem(new MenuItem("insec.debug", "Insec Debug (Rectangle)").SetValue(false));
                        insecDraw.AddItem(new MenuItem("obj.debug", "Object Usage Debug (Rectangle)").SetValue(false));
                        drawMenu.AddSubMenu(insecDraw);
                    }
                    Config.AddSubMenu(drawMenu);
                }
                Config.AddItem(new MenuItem("masterracec0mb0", "                  Hikigaya Lee Sin Keys"));
                Config.AddItem(new MenuItem("insec.active", "Insec!").SetValue(new KeyBind("A".ToCharArray()[0], KeyBindType.Press)));
                Config.AddItem(new MenuItem("ward.active", "WardJump!").SetValue(new KeyBind("S".ToCharArray()[0], KeyBindType.Press)));
            }
            
            Config.AddToMainMenu();
            GameObject.OnCreate += GameObject_OnCreate;
            Orbwalking.AfterAttack += AfterAttack;
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw +=Drawing_OnDraw;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Config.Item("q.draw").GetValue<Circle>().Active && Spells.Q.IsReady() && Spells.Q.QOne())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells.Q.Range, Config.Item("q.draw").GetValue<Circle>().Color);
            }
            if (Config.Item("q2.draw").GetValue<Circle>().Active && Spells.Q2.IsReady() && Spells.Q.QTwo())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells.Q2.Range, Config.Item("q2.draw").GetValue<Circle>().Color);
            }
            if (Config.Item("w.draw").GetValue<Circle>().Active && Spells.W.IsReady() && Spells.W.WOne())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells.W.Range, Config.Item("w.draw").GetValue<Circle>().Color);
            }
            if (Config.Item("e.draw").GetValue<Circle>().Active && Spells.E.IsReady() && Spells.E.EOne())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells.E.Range, Config.Item("e.draw").GetValue<Circle>().Color);
            }
            if (Config.Item("e2.draw").GetValue<Circle>().Active && Spells.E2.IsReady() && Spells.E.ETwo())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells.E2.Range, Config.Item("e2.draw").GetValue<Circle>().Color);
            }
            if (Config.Item("r.draw").GetValue<Circle>().Active && Spells.R.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells.R.Range, Config.Item("r.draw").GetValue<Circle>().Color);
            }
            if (Config.Item("insec.debug").GetValue<bool>())
            {
                Insec.InsecDebug();
            }
            if (Config.Item("obj.debug").GetValue<bool>())
            {
                Insec.ObjectUsageDebug();
            }
            
        }

        private static void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            Plugins.Activator.HikiTiamat((AIHeroClient)unit);
            Plugins.Activator.HikiHydra((AIHeroClient)unit);
            Plugins.Activator.HikiBilgewater((AIHeroClient)unit);
            Plugins.Activator.HikiBlade((AIHeroClient)unit);
            Plugins.Activator.HikiRanduin((AIHeroClient)unit);
            Plugins.Activator.HikiYoumuu((AIHeroClient)unit);
            Plugins.Activator.HikiTitanic((AIHeroClient)unit);
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (WardJump.WardCastable || !(sender is Obj_AI_Base) || sender.IsEnemy)
            {
                return;
            }

            var wardObject = (Obj_AI_Base)sender;
            if (wardObject.Name.IndexOf("ward", StringComparison.InvariantCultureIgnoreCase) >= 0 &&
                Vector3.DistanceSquared(sender.Position, WardJump.WardCastPosition) <= 150 * 150)
            {
                Spells.W.CastOnUnit(wardObject);
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    General.GeneralCombo();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    JungleClear();
                    WaveClear();
                    break;
            }

            if (Config.Item("insec.active").GetValue<KeyBind>().Active)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                Insec.ClickInsec();
            }
            if (Config.Item("ward.active").GetValue<KeyBind>().Active)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                WardJump.HikiJump(ObjectManager.Player.Position.Extend(Game.CursorPos, 600));
            }
        }

        private static void WaveClear()
        {
            var qMinion = MinionManager.GetMinions(ObjectManager.Player.Position, Spells.Q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth);
            if (Spells.Q.IsReady() && Spells.Q.QOne() && Config.Item("q.wave").GetValue<bool>())
            {
                foreach (var minion in qMinion.Where(x => x.Health < Spells.Q.GetDamage(x)))
                {
                    Spells.Q.Cast(minion);
                }
            }
            if (Spells.E.IsReady() && Spells.E.EOne() && Config.Item("e.wave").GetValue<bool>())
            {
                var countMinion = qMinion.Count(x => x.IsValidTarget(Spells.E.Range));
                if (countMinion >= Insec.SliderCheck("e.minion.count"))
                {
                    Spells.E.Cast();
                }
            }
        }
        private static void JungleClear()
        {
            var mob = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (mob == null || mob.Count == 0 || ObjectManager.Player.Buffs.Any(buff => buff.Name == "blindmonkpassive_cosmetic"))
            {
                return;
            }

            if (Spells.Q.IsReady() && Config.Item("q1.jungle").GetValue<bool>() && Spells.Q.QOne())
            {
                if (Spells.Q.CanCast(mob[0]))
                {
                    Spells.Q.Cast(mob[0]);
                }
            }
            if (Spells.Q2.IsReady() && Config.Item("q2.jungle").GetValue<bool>() && Spells.Q.QTwo())
            {
                if (Spells.Q2.CanCast(mob[0]))
                {
                    Spells.Q2.Cast(mob[0]);
                }
               
            }

            if (Spells.E.IsReady() && Config.Item("e1.jungle").GetValue<bool>() && Spells.E.EOne())
            {
                if (Spells.E.CanCast(mob[0]))
                {
                    Spells.E.Cast();
                }
            }

            if (Spells.E2.IsReady() && Config.Item("e2.jungle").GetValue<bool>() && Spells.E2.ETwo())
            {
                if (Spells.E2.CanCast(mob[0]))
                {
                    Spells.E2.Cast(mob[0]);
                }
            }

            if (Spells.W.IsReady() && Config.Item("w1.jungle").GetValue<bool>() && Spells.W.WOne())
            {
                Spells.W.CastOnUnit(ObjectManager.Player);
            }

            if (Spells.W2.IsReady() && Config.Item("w2.jungle").GetValue<bool>() && Spells.W.WTwo())
            {
                Spells.W2.Cast();
            }
            
        }
    }
}
