using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SNKarthus
{
    class Program
    {
        public static Check Check;

        private static bool cz = false;
        private static float czx = 0, czy = 0, czx2 = 0, czy2 = 0;

        private static AIHeroClient Player = ObjectManager.Player;
        private static Spell Q = new Spell(SpellSlot.Q, 875f, TargetSelector.DamageType.Magical);
        private static Spell W = new Spell(SpellSlot.W, 1000f, TargetSelector.DamageType.Magical);
        private static Spell E = new Spell(SpellSlot.E, 425f, TargetSelector.DamageType.Magical);
        private static Spell R = new Spell(SpellSlot.R, float.MaxValue, TargetSelector.DamageType.Magical);
        private static Spell _Ignite = new Spell(SpellSlot.Unknown, 600);

        private static Menu MenuIni;
        private static Orbwalking.Orbwalker orbwalker;
        private static AIHeroClient QTarget;
        private static AIHeroClient WTarget;
        private static AIHeroClient ETarget;
        private static bool NowE = false;

        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Karthus") return;

            Check = new Check();

            Q.SetSkillshot(1f, 160f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            W.SetSkillshot(.5f, 70f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(1f, 505f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(3f, float.MaxValue, float.MaxValue, false, SkillshotType.SkillshotCircle);

            MenuIni = new Menu("SN Karthus", "SN Karthus", true);
            MenuIni.AddToMainMenu();

            Menu orbwalkerMenu = new Menu("OrbWalker", "OrbWalker");
            orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            MenuIni.AddSubMenu(orbwalkerMenu);

            var targetSelectorMenu = new Menu("Target Selector", "TargetSelector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            MenuIni.AddSubMenu(targetSelectorMenu);

            var Combo = new Menu("Combo", "Combo");
            Combo.AddItem(new MenuItem("CUse_Q", "CUse_Q").SetValue(true));
            Combo.AddItem(new MenuItem("CUse_W", "CUse_W").SetValue(true));
            Combo.AddItem(new MenuItem("CUse_E", "CUse_E").SetValue(true));
            Combo.AddItem(new MenuItem("CUse_AA", "CUse_AA").SetValue(true));
            Combo.AddItem(new MenuItem("CEPercent", "Use E Mana %").SetValue(new Slider(30)));
            Combo.AddItem(new MenuItem("CE_Auto_False", "CE_Auto_False").SetTooltip("E auto false when target isn't valid").SetValue(true));
            MenuIni.AddSubMenu(Combo);

            var Harass = new Menu("Harass", "Harass");
            Harass.AddItem(new MenuItem("HUse_Q", "HUse_Q").SetValue(true));
            Harass.AddItem(new MenuItem("HUse_E", "HUse_E").SetValue(true));
            Harass.AddItem(new MenuItem("HEPercent", "Use E Mana %").SetValue(new Slider(30)));
            Harass.AddItem(new MenuItem("HUse_AA", "HUse_AA").SetValue(true));
            Harass.AddItem(new MenuItem("HUse_AA_to_minion", "HUse_AA_to_minion").SetTooltip("Use_AA_to_minion_for_lasthit").SetValue(true));
            Harass.AddItem(new MenuItem("E_LastHit", "E_LastHit").SetTooltip("Use E when killable minion is valid in range ").SetValue(true));
            Harass.AddItem(new MenuItem("HE_Auto_False", "HE_Auto_False").SetTooltip("E auto false when target isn't valid").SetValue(true));
            MenuIni.AddSubMenu(Harass);

            var Farm = new Menu("Farm", "Farm");
            Farm.AddItem(new MenuItem("FUse_Q", "FUse_Q").SetValue(true));
            Farm.AddItem(new MenuItem("FQPercent", "Use Q Mana %").SetValue(new Slider(15)));
            Farm.AddItem(new MenuItem("FUse_E", "FUse_E").SetValue(true));
            Farm.AddItem(new MenuItem("FEPercent", "Use E Mana %").SetValue(new Slider(15)));
            MenuIni.AddSubMenu(Farm);

            var LastHit = new Menu("LastHit", "LastHit");
            LastHit.AddItem(new MenuItem("LUse_Q", "LUse_Q").SetValue(true));
            MenuIni.AddSubMenu(LastHit);

            var Misc = new Menu("Misc", "Misc");
            Misc.AddItem(new MenuItem("NotifyUlt", "Ult_notify_text").SetValue(true));
            Misc.AddItem(new MenuItem("DeadCast", "DeadCast").SetValue(true));
            Misc.AddItem(new MenuItem("Ignite", "Ignite").SetValue(true));
            MenuIni.AddSubMenu(Misc);

            var Draw = new Menu("Draw", "Draw");
            Draw.AddItem(new MenuItem("Enabled", "Enabled").SetValue(true));
            Draw.AddItem(new MenuItem("Draw_Q", "Draw_Q").SetValue(new Circle(true, Color.Green)));
            MenuIni.AddSubMenu(Draw);

            Game.OnUpdate += zigzag;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
        }

        private static void zigzag(EventArgs args)
        {
            if (QTarget == null)
            {
                czx = 0;
                czx2 = 0;
                czy = 0;
                czy2 = 0;
                return;
            }

            if (czx < czx2)
            {
                if (czx2 >= QTarget.Position.X)
                    cz = true;
                else
                    cz = false;
            }
            else if (czx == czx2)
            {
                cz = false;
                czx = czx2;
                czx2 = QTarget.Position.X;
                return;
            }
            else
            {
                if (czx2 <= QTarget.Position.X)
                    cz = true;
                else
                    cz = false;
            }
            czx = czx2;
            czx2 = QTarget.Position.X;

            if (czy < czy2)
            {
                if (czy2 >= QTarget.Position.Y)
                    cz = true;
                else
                    cz = false;
            }
            else if (czy == czy2)
                cz = false;
            else
            {
                if (czy2 <= QTarget.Position.Y)
                    cz = true;
                else
                    cz = false;
            }
            czy = czy2;
            czy2 = QTarget.Position.Y;
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead) return;

            QTarget = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            WTarget = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
            ETarget = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

            var Ignite = Player.Spellbook.Spells.FirstOrDefault(spell => spell.Name == "summonerdot");
            if (Ignite != null && MenuIni.SubMenu("Misc").Item("Ignite").GetValue<bool>())
                _Ignite.Slot = Ignite.Slot;
            else
                _Ignite.Slot = SpellSlot.Unknown;

            var activeOrbwalker = orbwalker.ActiveMode;
            switch (activeOrbwalker)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    orbwalker.SetAttack(MenuIni.SubMenu("Combo").Item("CUse_AA").GetValue<bool>() || Player.Mana < Q.Instance.SData.Mana * 3);
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    orbwalker.SetAttack(true);
                    Farm();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    orbwalker.SetAttack(MenuIni.SubMenu("Harass").Item("HUse_AA").GetValue<bool>() || Player.Mana < Q.Instance.SData.Mana * 3);
                    Orbwalking.BeforeAttack += BeforeAttack;
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    orbwalker.SetAttack(true);
                    LastHit();
                    break;
                default:
                    orbwalker.SetAttack(true);
                    calcE();

                    if (MenuIni.SubMenu("Misc").Item("DeadCast").GetValue<bool>())
                        if (Player.IsZombie)
                            if (!Combo())
                                Farm(true);
                    break;
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (!Player.IsDead)
            {
                if (MenuIni.SubMenu("Draw").Item("Draw_Q").GetValue<Circle>().Active)
                {
                    Render.Circle.DrawCircle(Player.Position, Q.Range, MenuIni.SubMenu("Draw").Item("Draw_Q").GetValue<Circle>().Color);
                }
            }

            if (Player.Spellbook.GetSpell(SpellSlot.R).Level > 0)
            {
                var killable = "";

                var time = Utils.TickCount;

                foreach (TI target in Program.Check.TI.Where(x => x.Player.IsValid && !x.Player.IsDead && x.Player.IsEnemy && (Program.Check.recalltc(x) /*|| (x.Player.IsVisible && Utility.IsValidTarget(x.Player))*/) && Player.GetSpellDamage(x.Player, SpellSlot.R) >= Program.Check.GetTargetHealth(x, (int)(R.Delay * 1000f))))
                {
                    killable += target.Player.ChampionName + " ";
                }

                if (killable != "" && MenuIni.SubMenu("Misc").Item("NotifyUlt").GetValue<bool>())
                {
                    Drawing.DrawText(Drawing.Width * 0.44f, Drawing.Height * 0.7f, System.Drawing.Color.Red, "Killable by ult: " + killable);
                }
            }
        }

        private static void BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (MenuIni.SubMenu("Harass").Item("HUse_AA_to_minion").GetValue<bool>())
                return;
            else
            {
                if (args.Target.Type == GameObjectType.obj_AI_Minion)
                {
                    args.Process = false;
                }
            }
        }

        private static Vector3 PredPos(AIHeroClient Hero, float Delay)
        {
            float value = 0f;
            if (Hero.IsFacing(Player))
            {
                value = (50f - Hero.BoundingRadius);
            }
            else
            {
                value = -(100f - Hero.BoundingRadius);
            }
            var distance = Delay * Hero.MoveSpeed + value;
            var path = Hero.Path.ToList().To2D();

            for (var i = 0; i < path.Count - 1; i++)
            {
                var a = path[i];
                var b = path[i + 1];
                var d = a.Distance(b);

                if (d < distance)
                {
                    distance -= d;
                }
                else
                {
                    return (a + distance * (b - a).Normalized()).To3D();
                }
            }
            return (path[path.Count - 1]).To3D();
        }

        private static void calcE(bool TC = false)
        {
            if (!E.IsReady() || Player.IsZombie || Player.Spellbook.GetSpell(SpellSlot.E).ToggleState != 2) return;

            var minions = MinionManager.GetMinions(Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.NotAlly);

            if (!TC && (ETarget != null || (!NowE && minions.Count != 0)))
                return;

            E.Cast();
            NowE = false;
        }

        private static void Harass()
        {
            if (QTarget != null)
            {
                if (MenuIni.SubMenu("Harass").Item("HUse_Q").GetValue<bool>())
                    if (Q.IsReady() && QTarget.IsValidTarget(Q.Range))
                    {
                        if (!cz)
                            Q.Cast(PredPos(QTarget, 0.6f));
                        else
                            Q.Cast(QTarget);
                    }

            }

            if (MenuIni.SubMenu("Harass").Item("HUse_E").GetValue<bool>() && MenuIni.SubMenu("Harass").Item("E_LastHit").GetValue<bool>() && E.IsReady() && !Player.IsZombie)
            {
                if (!E.IsReady() || Player.IsZombie)
                    return;

                List<Obj_AI_Base> minions;
                bool jgm;

                NowE = false;

                minions = MinionManager.GetMinions(Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.NotAlly);
                minions.RemoveAll(x => x.MaxHealth <= 5);
                minions.RemoveAll(x => Player.Distance(x.ServerPosition) > E.Range || x.Health > Damage.GetSpellDamage(Player, x, SpellSlot.E));
                jgm = minions.Any(x => x.Team == GameObjectTeam.Neutral);

                if ((Player.Spellbook.GetSpell(SpellSlot.E).ToggleState == 1 && (minions.Count >= 1 || jgm)) && (((Player.Mana / Player.MaxMana) * 100f) >= MenuIni.SubMenu("Harass").Item("HEPercent").GetValue<Slider>().Value))
                    E.Cast();
                else if ((Player.Spellbook.GetSpell(SpellSlot.E).ToggleState == 2 && (minions.Count == 0 && !jgm)) || !(((Player.Mana / Player.MaxMana) * 100f) >= MenuIni.SubMenu("Harass").Item("HEPercent").GetValue<Slider>().Value))
                    calcE(true);
            }

            if (MenuIni.SubMenu("Harass").Item("HUse_E").GetValue<bool>() && E.IsReady() && !Player.IsZombie)
            {
                if (MenuIni.SubMenu("Harass").Item("HE_Auto_False").GetValue<bool>())
                {
                    if (ETarget != null)
                    {
                        if (Player.Spellbook.GetSpell(SpellSlot.E).ToggleState == 1)
                        {
                            if (Player.Distance(ETarget.ServerPosition) <= E.Range && (((Player.Mana / Player.MaxMana) * 100f) >= MenuIni.SubMenu("Harass").Item("HEPercent").GetValue<Slider>().Value))
                            {
                                NowE = true;
                                E.Cast();
                            }
                        }
                        else if (Player.Distance(ETarget.ServerPosition) >= E.Range || (((Player.Mana / Player.MaxMana) * 100f) <= MenuIni.SubMenu("Harass").Item("HEPercent").GetValue<Slider>().Value))
                        {
                            calcE(true);
                        }
                    }
                    else
                        calcE();
                }
                else
                {
                    if (ETarget != null)
                    {
                        if (Player.Spellbook.GetSpell(SpellSlot.E).ToggleState == 1)
                        {
                            if (Player.Distance(ETarget.ServerPosition) <= E.Range && (((Player.Mana / Player.MaxMana) * 100f) >= MenuIni.SubMenu("Harass").Item("HEPercent").GetValue<Slider>().Value))
                            {
                                NowE = true;
                                E.Cast();
                            }
                        }
                        else if ((((Player.Mana / Player.MaxMana) * 100f) <= MenuIni.SubMenu("Harass").Item("HEPercent").GetValue<Slider>().Value))
                        {
                            calcE(true);
                        }
                    }
                }

            }
        }

        private static bool Combo()
        {
            bool Qtarget = false;

            var Qm = MenuIni.SubMenu("Combo").Item("CUse_Q").GetValue<bool>();
            var Wm = MenuIni.SubMenu("Combo").Item("CUse_W").GetValue<bool>();
            var Em = MenuIni.SubMenu("Combo").Item("CUse_E").GetValue<bool>();
            var EFm = MenuIni.SubMenu("Combo").Item("CE_Auto_False").GetValue<bool>();

            if (WTarget == null)
                return false;

            if (QTarget != null)
            {
                if (QTarget.IsValid && Player.Distance(QTarget.Position) < _Ignite.Range)
                {
                    var Igd = Damage.GetSummonerSpellDamage(Player, QTarget, Damage.SummonerSpell.Ignite);
                    if (Igd > QTarget.Health)
                        _Ignite.CastOnUnit(QTarget);
                }
            }

            if (Wm && W.IsReady() && WTarget.IsValid)
            {
                double DS = 0;
                double countmana = W.ManaCost;

                if (R.IsReady())
                {
                    DS += Damage.GetSpellDamage(Player, QTarget, SpellSlot.R);
                    countmana += R.ManaCost;
                }

                while (DS < QTarget.MaxHealth)
                {
                    var Qd = Damage.GetSpellDamage(Player, QTarget, SpellSlot.Q);

                    DS += Qd;
                    countmana += Q.ManaCost;
                }


                if (Player.MaxMana > countmana || QTarget.GetAlliesInRange(W.Range).Count > 1 || Player.IsZombie)
                    W.Cast(PredPos(WTarget, 0.3f));
            }

            if (ETarget != null)
            {
                if (Em && E.IsReady() && !Player.IsZombie)
                {
                    if (EFm)
                    {
                        if (ETarget != null)
                        {
                            if (Player.Spellbook.GetSpell(SpellSlot.E).ToggleState == 1)
                            {
                                if (Player.Distance(ETarget.ServerPosition) <= E.Range && (((Player.Mana / Player.MaxMana) * 100f) >= MenuIni.SubMenu("Combo").Item("CEPercent").GetValue<Slider>().Value))
                                {
                                    NowE = true;
                                    E.Cast();
                                }
                            }
                            else if (Player.Distance(ETarget.ServerPosition) >= E.Range || (((Player.Mana / Player.MaxMana) * 100f) <= MenuIni.SubMenu("Combo").Item("CEPercent").GetValue<Slider>().Value))
                            {
                                calcE(true);
                            }
                        }
                        else
                            calcE();
                    }
                    else
                    {
                        if (Player.Spellbook.GetSpell(SpellSlot.E).ToggleState == 1)
                        {
                            if (Player.Distance(ETarget.ServerPosition) <= E.Range && (((Player.Mana / Player.MaxMana) * 100f) >= MenuIni.SubMenu("Combo").Item("CEPercent").GetValue<Slider>().Value))
                            {
                                NowE = true;
                                E.Cast();
                            }
                        }
                        else if ((((Player.Mana / Player.MaxMana) * 100f) <= MenuIni.SubMenu("Combo").Item("CEPercent").GetValue<Slider>().Value))
                        {
                            calcE(true);
                        }
                    }
                }
            }

            if (QTarget != null)
            {
                if (Qm && Q.IsReady() && QTarget.IsValid)
                {
                    Qtarget = true;
                    if (!cz)
                        Q.Cast(PredPos(QTarget, 0.6f));
                    else
                        Q.Cast(QTarget);
                }
            }

            return Qtarget;
        }

        private static void Farm(bool Can = false)
        {
            var canQ = Can || MenuIni.SubMenu("Farm").Item("FUse_Q").GetValue<bool>();
            var canE = Can || MenuIni.SubMenu("Farm").Item("FUse_E").GetValue<bool>();
            //bool QtoOne = MenuIni.SubMenu("Farm").Item("Q_to_One").GetValue<bool>();
            bool jgm;
            List<Obj_AI_Base> minions;

            if (canQ && Q.IsReady() && (((Player.Mana / Player.MaxMana) * 100f) >= MenuIni.SubMenu("Farm").Item("FQPercent").GetValue<Slider>().Value))
            {
                minions = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly);
                minions.RemoveAll(x => x.MaxHealth <= 5);
                var positions = new List<Vector2>();

                foreach (var minion in minions)
                {
                    positions.Add(minion.ServerPosition.To2D());
                }

                var location = MinionManager.GetBestCircularFarmLocation(positions, 160f, Q.Range);

                if (location.MinionsHit >= 1)
                    Q.Cast(location.Position);
            }

            if (!canE || !E.IsReady() || Player.IsZombie)
                return;
            NowE = false;

            minions = MinionManager.GetMinions(Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.NotAlly);
            minions.RemoveAll(x => x.MaxHealth <= 5);
            jgm = minions.Any(x => x.Team == GameObjectTeam.Neutral);

            if ((Player.Spellbook.GetSpell(SpellSlot.E).ToggleState == 1 && (minions.Count >= 3 || jgm)) && (((Player.Mana / Player.MaxMana) * 100f) >= MenuIni.SubMenu("Farm").Item("FEPercent").GetValue<Slider>().Value))
                E.Cast();
            else if ((Player.Spellbook.GetSpell(SpellSlot.E).ToggleState == 2 && (minions.Count <= 2 && !jgm)) || !(((Player.Mana / Player.MaxMana) * 100f) >= MenuIni.SubMenu("Farm").Item("FEPercent").GetValue<Slider>().Value))
                calcE();
        }

        private static void LastHit()
        {
            if (MenuIni.SubMenu("LastHit").Item("LUse_Q").GetValue<bool>())
            {
                List<Obj_AI_Base> minions, minions2;

                minions2 = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Enemy);
                minions = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly);
                minions.RemoveAll(x => x.MaxHealth <= 5);
                minions.RemoveAll(x => x.Health > Damage.GetSpellDamage(Player, x, SpellSlot.Q));
                var i = new List<int>() { -100, -70, 0, 70, 100 };
                var j = new List<int>() { -100, -70, 0, 70, 100 };

                foreach (var minion in minions)
                {
                    foreach (int xi in i)
                    {
                        foreach (int yj in j)
                        {
                            int cnt = 0;
                            Vector3 temp = new Vector3(Prediction.GetPrediction(minion, 250f).UnitPosition.X + xi, Prediction.GetPrediction(minion, 250f).UnitPosition.Y + yj, Prediction.GetPrediction(minion, 250f).UnitPosition.Z);
                            foreach (var minion2 in minions2.Where(x => Vector3.Distance(temp, Prediction.GetPrediction(x, 250f).UnitPosition) < 200))
                            {
                                cnt++;
                            }

                            if (cnt == 1 && minion.Health < Damage.GetSpellDamage(Player, minion, SpellSlot.Q))
                            {
                                Q.Cast(temp);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
