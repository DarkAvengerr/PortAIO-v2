using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Security.AccessControl;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System;
using System.Linq;
using Color = System.Drawing.Color;
using EloBuddy;

namespace GragasTheDrunkCarry
{
    class Gragas
    {
        public static AIHeroClient Player;
        public static Spell Q, W, E, R;
        public static Orbwalking.Orbwalker Orbwalker;
        public static Menu Config;
        public static GameObject Bomb;
        public static Vector2 Rpos;
        public static float LastMove;


        public Gragas()
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {

            Q = new Spell(SpellSlot.Q, 775);
            W = new Spell(SpellSlot.W, 0);
            E = new Spell(SpellSlot.E, 600);
            R = new Spell(SpellSlot.R, 1050);
            Q.SetSkillshot(0.3f, 110f, 1000f, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.3f, 50, 1000, true, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.3f, 700, 1000, false, SkillshotType.SkillshotCircle);
            Config = new Menu("Gragas", "Gragas", true);
            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);
            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));
            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseQ", "Use Q?").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseW", "Use W?").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseE", "Use E?").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseR", "Use R?").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("AutoB", "Auto Bomb?").SetValue(true));
            Config.SubMenu("KillSteal").AddItem(new MenuItem("QKS", "KS with Q").SetValue(true));
            Config.SubMenu("KillSteal").AddItem(new MenuItem("EKS", "KS with E").SetValue(true));
            Config.SubMenu("KillSteal").AddItem(new MenuItem("RKS", "KS with R").SetValue(true));
            Config.SubMenu("KillSteal").AddItem(new MenuItem("SmartKs", "Auto Ks?").SetValue(true));
            Config.SubMenu("Jungle Farm").AddItem(new MenuItem("JQ", "Use Q").SetValue(true));
            Config.SubMenu("Jungle Farm").AddItem(new MenuItem("JW", "Use E").SetValue(true));
            Config.SubMenu("Jungle Farm").AddItem(new MenuItem("JE", "Use R").SetValue(true));
            Config.SubMenu("Jungle Farm").AddItem(new MenuItem("JF", "Jungle Farm").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));
            Config.SubMenu("Wave Clear").AddItem(new MenuItem("WQ", "Use Q").SetValue(true));
            Config.SubMenu("Wave Clear").AddItem(new MenuItem("WW", "Use E").SetValue(true));
            Config.SubMenu("Wave Clear").AddItem(new MenuItem("WE", "Use R").SetValue(true));
            Config.SubMenu("Wave Clear").AddItem(new MenuItem("WF", "Wave Clear").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));
            Config.AddSubMenu(new Menu("Harras", "Harras"));
            Config.SubMenu("Harras").AddItem(new MenuItem("UseQH", "Use Q?").SetValue(true));
            Config.SubMenu("Harras").AddItem(new MenuItem("UseEH", "Use E?").SetValue(true));
            Config.AddSubMenu(new Menu("Draw", "Draw"));
            Config.SubMenu("Draw").AddItem(new MenuItem("DrawIN", "Draw Insec Pos?").SetValue(false));
            Config.SubMenu("Draw").AddItem(new MenuItem("DrawQ", "Draw Q range").SetValue(false));
            Config.SubMenu("Draw").AddItem(new MenuItem("DrawE", "Draw E Range").SetValue(false));
            Config.SubMenu("Draw").AddItem(new MenuItem("Draw R", "Draw R Range").SetValue(false));
            Config.AddItem(
    new MenuItem("Insec", "Insec").SetValue(new KeyBind("A".ToCharArray()[0], KeyBindType.Press)));
            Config.AddToMainMenu();


            Player = ObjectManager.Player;
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnEndScene += Drawing_OnEndScene;
            GameObject.OnCreate += OnCreateObject;
            GameObject.OnDelete += GameObject_OnDelete;

        }
        private static void OnCreateObject(GameObject sender, EventArgs args)
        {
            if (sender.Name == "Gragas_Base_R_End.troy")
            {
                if (Config.Item("Insec").GetValue<KeyBind>().Active)
                {
                    InsecCombo(sender.Position.LSTo2D());
                }
            }
            if (sender.Name == "Gragas_Base_Q_Ally.troy")
            {
                Bomb = sender;
            }
        }

        static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (sender.Name == "Gragas_Base_Q_Ally.troy")
            {
                Bomb = null;
            }
        }

        static void Game_OnUpdate(EventArgs args)
        {
            var vTarget = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            var target = Orbwalker.GetTarget() as AIHeroClient;
            if (Config.Item("JF").GetValue<KeyBind>().Active)
            {
                JungleFarm();
            }
            if (Config.Item("WF").GetValue<KeyBind>().Active)
            {
                WaveClear();
            }
            if (Orbwalker.ActiveMode.ToString().ToLower() == "combo")
            {

                if (Config.Item("UseQ").GetValue<bool>() && Q.LSIsReady() && ((Environment.TickCount - LastMove) > 50))
                {
                    Qcast(vTarget);
                    LastMove = Environment.TickCount;
                }


                if (E.LSIsReady() && Player.LSDistance(vTarget) <= E.Range && Config.Item("UseE").GetValue<bool>() && ((Environment.TickCount - LastMove) > 50))
                {
                    E.Cast(vTarget, true);
                    LastMove = Environment.TickCount;
                }

                if (Config.Item("UseW").GetValue<bool>() && W.LSIsReady() && ((Environment.TickCount - LastMove) > 50))
                {
                    W.Cast();
                    LastMove = Environment.TickCount;
                }

                if (Config.Item("UseR").GetValue<bool>() && R.LSIsReady() && GetCDamage(vTarget) >= vTarget.Health && ((Environment.TickCount - LastMove) > 50))
                {
                    R.Cast(vTarget);
                    LastMove = Environment.TickCount;
                }









            }

            if (Orbwalker.ActiveMode.ToString().ToLower() == "mixed")
            {
                if (Config.Item("UseQH").GetValue<bool>() && Q.LSIsReady())
                {
                    Qcast(vTarget);
                }

                if (Config.Item("UseEH").GetValue<bool>() && E.LSIsReady() && Player.LSDistance(vTarget) <= E.Range)
                {
                    E.Cast(vTarget, true);
                }

            }


            if (Config.Item("AutoB").GetValue<bool>() && Bomb != null)
            {
                foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsEnemy && hero.LSDistance(Bomb.Position) <= 250))
                {
                    Qcast(hero);
                }
            }

            if (Config.Item("Insec").GetValue<KeyBind>().Active)
            {
                Insec(vTarget);
            }

        }

        private static int GetCDamage(Obj_AI_Base target)
        {
            var damage = 0;
            if (Q.LSIsReady())
            {
                damage += (int)Q.GetDamage(target);
            }
            if (E.LSIsReady())
            {
                damage += (int)E.GetDamage(target);
            }
            if (R.LSIsReady())
            {
                damage += (int)R.GetDamage(target);
            }
            return damage;
        }


        private static void Qcast(Obj_AI_Base target)
        {
            if (!Config.Item("UseQ").GetValue<bool>()) return;
            if (!(target.LSDistance(Player) <= Q.Range)) return;
            if (Bomb == null)
            {
                Q.Cast(target, true);
            }

            if (Bomb != null && target.LSDistance(Bomb.Position) <= 250)
            {
                Q.Cast();
            }
        }

        private static void InsecCombo(Vector2 pos)
        {
            var vTarget = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (!(vTarget.LSDistance(pos) <= 600)) return;
            var newpos = pos.LSExtend(vTarget.Position.LSTo2D(), 900);
            if (((Environment.TickCount - LastMove) > 50))
            {
                Q.Cast(newpos, true);
                LastMove = Environment.TickCount;
            }
            if (((Environment.TickCount - LastMove) > 50))
            {
                E.Cast(newpos, true);
                LastMove = Environment.TickCount;
            }
        }

        public static void Insec(AIHeroClient target)
        {
            Rpos = Player.Position.LSTo2D().LSExtend(target.Position.LSTo2D(), Player.LSDistance(target) + 300);
            if (Rpos.LSDistance(Player.Position) < R.Range-20)
            {
                if (Player.LSDistance(Rpos.LSExtend(target.Position.LSTo2D(), 900 - target.LSDistance(Rpos))) < E.Range && !IsWall(Rpos.To3D()) && target.LSIsFacing(Player))
                {
                    R.Cast(Rpos);
                }
            }
        }
        private static bool IsWall(Vector3 pos)
        {
            CollisionFlags cFlags = NavMesh.GetCollisionFlags(pos);
            return (cFlags == CollisionFlags.Wall);
        }
        static void Drawing_OnEndScene(EventArgs args)
        {
            var vTarget = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (vTarget != null && R.LSIsReady() && Config.Item("DrawIN").GetValue<bool>())
            {
                Render.Circle.DrawCircle(Rpos.To3D(), 50, Color.Red);
            }
            if (Config.Item("DrawQ").GetValue<bool>())
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range, Color.DarkSlateGray);
            }
            if (Config.Item("DrawE").GetValue<bool>())
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, Color.DarkSlateGray);
            }
            if (Config.Item("DrawR").GetValue<bool>())
            {
                Render.Circle.DrawCircle(Player.Position, R.Range, Color.DarkSlateGray);
            }

        }

        private void SmartKs()
        {
            if (!Config.Item("smartKS", true).GetValue<bool>())
                return;

            foreach (AIHeroClient target in ObjectManager.Get<AIHeroClient>().Where(x => x.LSIsValidTarget(700) && !x.HasBuffOfType(BuffType.Invulnerability)).OrderByDescending(GetCDamage))
            {
                if (target != null)
                {
                    //R
                    if ((Player.LSGetSpellDamage(target, SpellSlot.R)) > target.Health + 20 && Player.LSDistance(target) < R.Range && Config.Item("RKS", true).GetValue<bool>())
                    {
                        R.Cast(target);
                    }


                    //Q
                    if ((Player.LSGetSpellDamage(target, SpellSlot.Q)) > target.Health + 20 && Player.LSDistance(target) < Q.Range && Config.Item("QKS", true).GetValue<bool>())
                    {
                        Qcast(target);
                    }

                    //E
                    if ((Player.LSGetSpellDamage(target, SpellSlot.E)) > target.Health + 20 && Player.LSDistance(target) < E.Range && Config.Item("EKS", true).GetValue<bool>())
                    {
                        E.Cast(target);
                    }

                }
            }
        }

        public static void JungleFarm()
        {
            var minion = MinionManager.GetMinions(Player.Position, 600, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (Q.LSIsReady() && Config.Item("JQ").GetValue<bool>() && ((Environment.TickCount - LastMove) > 50))
            {
                if (minion.LSIsValidTarget(Q.Range))
                {
                    Qcast(minion);
                    LastMove = Environment.TickCount;
                }
            }

            if (E.LSIsReady() && Config.Item("JE").GetValue<bool>() && ((Environment.TickCount - LastMove) > 50))
            {
                if (minion.LSIsValidTarget(E.Range))
                {
                    E.Cast(minion.Position);
                    LastMove = Environment.TickCount;
                }
            }


            if (W.LSIsReady() && Config.Item("JW").GetValue<bool>() && ((Environment.TickCount - LastMove) > 50))
            {
                W.Cast();
                LastMove = Environment.TickCount;
            }

        } 

    

        public static void WaveClear()
        {
            var minion = MinionManager.GetMinions(Player.Position, 600, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (Q.LSIsReady() && Config.Item("WQ").GetValue<bool>() && ((Environment.TickCount - LastMove) > 50))
            {
                if (minion.LSIsValidTarget(Q.Range))
                {
                    Qcast(minion);
                    LastMove = Environment.TickCount;
                }
            }


            if (E.LSIsReady() && Config.Item("WE").GetValue<bool>() && ((Environment.TickCount - LastMove) > 50))
            {
                if (minion.LSIsValidTarget(E.Range))
                {
                    E.Cast(minion.Position);
                    LastMove = Environment.TickCount;
                }
            }


            if (W.LSIsReady() && Config.Item("WW").GetValue<bool>() && ((Environment.TickCount - LastMove) > 50))
            {
                W.Cast();
                LastMove = Environment.TickCount;
            }



        }


    }
}
