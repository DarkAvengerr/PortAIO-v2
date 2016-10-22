using EloBuddy;
using LeagueSharp.SDK;
namespace Flowers_Karma
{
    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;
    using SharpDX;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static LeagueSharp.SDK.Events;

    internal class Program
    {
        private static Menu Menu;
        private static Spell Q, W, E, R;
        private static SpellSlot Ignite = SpellSlot.Unknown;
        private static AIHeroClient Me;
        private static int SwitchETickCount = 0, SwitchRTickCount = 0;
        private static HpBarDraw HpBarDraw = new HpBarDraw();
        private static List<BuffType> DebuffTypes = new List<BuffType>();
        private static string[] AutoEnableList =
        {
             "Annie", "Ahri", "Akali", "Anivia", "Annie", "Brand", "Cassiopeia", "Diana", "Evelynn", "FiddleSticks", "Fizz", "Gragas", "Heimerdinger", "Karthus",
             "Kassadin", "Katarina", "Kayle", "Kennen", "Leblanc", "Lissandra", "Lux", "Malzahar", "Mordekaiser", "Morgana", "Nidalee", "Orianna",
             "Ryze", "Sion", "Swain", "Syndra", "Teemo", "TwistedFate", "Veigar", "Viktor", "Vladimir", "Xerath", "Ziggs", "Zyra", "Velkoz", "Azir", "Ekko",
             "Ashe", "Caitlyn", "Corki", "Draven", "Ezreal", "Graves", "Jayce", "Jinx", "KogMaw", "Lucian", "MasterYi", "MissFortune", "Quinn", "Shaco", "Sivir",
             "Talon", "Tristana", "Twitch", "Urgot", "Varus", "Vayne", "Yasuo", "Zed", "Kindred", "AurelionSol"
        };

        public static void Main()
        {
            Bootstrap.Init(null);
            if (GameObjects.Player.ChampionName.ToLower() != "karma")
            {
                return;
            }

            Me = GameObjects.Player;

            LoadSpell();
            LoadMenu();
            LoadEvent();
        }

        private static void LoadSpell()
        {
            Q = new Spell(SpellSlot.Q, 950f);
            W = new Spell(SpellSlot.W, 700f);
            E = new Spell(SpellSlot.E, 800f);
            R = new Spell(SpellSlot.R);

            Q.SetSkillshot(0.25f, 60f, 1700f, true, SkillshotType.SkillshotLine);
            W.SetTargetted(0.25f, 2200f);
            E.SetTargetted(0.25f, float.MaxValue);

            Ignite = Me.GetSpellSlot("SummonerDot");
        }

        private static void LoadMenu()
        {
            Menu = new Menu("Karma - Never Falter", "Karma - Never Falter", true).Attach();

            var ComboMenu = Menu.Add(new Menu("Combo", "Combo"));
            {
                ComboMenu.Add(new MenuBool("Q", "Use Q", true));
                ComboMenu.Add(new MenuBool("W", "Use W", true));
                ComboMenu.Add(new MenuList<string>("E", "Use E | Mode", new[] { "Low Hp", "Gank", "Off" }));
                ComboMenu.Add(new MenuSlider("EHp", "Use E Low Hp Mode | Min HealthPercent >= %", 45));
                ComboMenu.Add(new MenuKeyBind("ESwitch", "Switch E Mode Key", System.Windows.Forms.Keys.G, KeyBindType.Press));
                ComboMenu.Add(new MenuList<string>("R", "Use R | Mode", new[] { "All", "Q", "W", "E", "Off" }));
                ComboMenu.Add(new MenuKeyBind("RSwitch", "Switch R Mode Key", System.Windows.Forms.Keys.H, KeyBindType.Press));
                ComboMenu.Add(new MenuBool("Ignite", "Use Ignite", true));
            }

            var HarassMenu = Menu.Add(new Menu("Harass", "Harass"));
            {
                HarassMenu.Add(new MenuBool("Q", "Use Q", true));
                HarassMenu.Add(new MenuBool("R", "Use R", true));
                HarassMenu.Add(new MenuSlider("Mana", "Harass Mode | Min ManaPercent >= %", 60));
                HarassMenu.Add(new MenuKeyBind("Auto", "Auto Q Harass", System.Windows.Forms.Keys.T, KeyBindType.Toggle));
                HarassMenu.Add(new MenuSlider("AutoMana", "Auto Harass Mode | Min ManaPercent >= %", 60));
            }

            var LaneClearMenu = Menu.Add(new Menu("LaneClear", "Lane Clear"));
            {
                LaneClearMenu.Add(new MenuSliderButton("Q", "Use Q | Min Hit Count >= ", 3, 1, 5, true));
                LaneClearMenu.Add(new MenuSlider("Mana", "LaneClear Mode | Min ManaPercent >= %", 60));
            }

            var JungleClearMenu = Menu.Add(new Menu("JungleClear", "Jungle Clear"));
            {
                JungleClearMenu.Add(new MenuBool("Q", "Use Q", true));
                JungleClearMenu.Add(new MenuBool("W", "Use W", true));
                JungleClearMenu.Add(new MenuBool("E", "Use E", true));
                JungleClearMenu.Add(new MenuBool("R", "Use RQ", true));
                JungleClearMenu.Add(new MenuSlider("Mana", "JungleClear Mode | Min ManaPercent >= %", 50));
            }

            var KillStealMenu = Menu.Add(new Menu("KillSteal", "Kill Steal"));
            {
                KillStealMenu.Add(new MenuBool("Q", "Use Q", true));
                KillStealMenu.Add(new MenuBool("W", "Use W", true));
                KillStealMenu.Add(new MenuBool("R", "Use R", true));
                KillStealMenu.Add(new MenuSeparator("KillStealList", "KillSteal List"));
                if (GameObjects.EnemyHeroes.Any())
                {
                    GameObjects.EnemyHeroes.ForEach(i => KillStealMenu.Add(new MenuBool(i.ChampionName.ToLower(), i.ChampionName, AutoEnableList.Contains(i.ChampionName))));
                }
            }

            var FleeMenu = Menu.Add(new Menu("Flee", "Flee"));
            {
                FleeMenu.Add(new MenuBool("Q", "Use Q", true));
                FleeMenu.Add(new MenuBool("W", "Use W", true));
                FleeMenu.Add(new MenuBool("E", "Use E", true));
                FleeMenu.Add(new MenuBool("R", "Use R", true));
                FleeMenu.Add(new MenuKeyBind("Key", "Key", System.Windows.Forms.Keys.Z, KeyBindType.Press));
            }

            var SkinMenu = Menu.Add(new Menu("Skin", "Skin"));
            {
                SkinMenu.Add(new MenuBool("Enable", "Enabled", false));
                SkinMenu.Add(new MenuList<string>("SkinName", "Select Skin", new[] { "Classic", "Sun Goddess", "Sakura", "Traditional", "Order of the Lotus", "Warden" }));
            }

            var MiscMenu = Menu.Add(new Menu("Misc", "Misc"));
            {
                MiscMenu.Add(new MenuBool("QGap", "Use Q Anti GapCloset", true));
                MiscMenu.Add(new MenuBool("WGap", "Use W Anti GapCloset", true));
                MiscMenu.Add(new MenuBool("EGap", "Use E Anti GapCloset", true));
                MiscMenu.Add(new MenuBool("RGap", "Use R Interrupt Spell", true));
                MiscMenu.Add(new MenuBool("Support", "Support Mode!", false));
            }

            var DrawMenu = Menu.Add(new Menu("Draw", "Draw"));
            {
                DrawMenu.Add(new MenuBool("Q", "Q Range"));
                DrawMenu.Add(new MenuBool("W", "W Range"));
                DrawMenu.Add(new MenuBool("E", "E Range"));
                DrawMenu.Add(new MenuBool("DrawDamage", "Draw Combo Damage", true));
                DrawMenu.Add(new MenuBool("Auto", "Draw Auto Q Status", true));
                DrawMenu.Add(new MenuBool("ComboE", "Draw Combo E Status", true));
                DrawMenu.Add(new MenuBool("ComboR", "Draw Combo R Status", true));
            }
        }

        private static void LoadEvent()
        {
            Events.OnGapCloser += OnGapCloser;
            Variables.Orbwalker.OnAction += OnAction;
            Drawing.OnDraw += OnDraw;
            Game.OnUpdate += OnUpdate;
        }

        private static void OnGapCloser(object obj, GapCloserEventArgs Args)
        {
            if (Args.IsDirectedToPlayer)
            {
                var sender = Args.Sender as AIHeroClient;

                if (sender.IsEnemy && sender.IsValidTarget(300))
                {
                    if (Menu["Misc"]["RGap"] && R.IsReady())
                    {
                        R.Cast();
                    }
                    else if (Menu["Misc"]["WGap"] && W.IsReady())
                    {
                        W.Cast(sender);
                    }
                    else if (Menu["Misc"]["EGap"] && E.IsReady())
                    {
                        E.Cast(Me);
                    }
                    else if (Menu["Misc"]["QGap"] && Q.IsReady())
                    {
                        Q.Cast(sender);
                    }
                }
            }
        }

        private static void OnAction(object obj, OrbwalkingActionArgs Args)
        {
            if (Args.Type == OrbwalkingType.BeforeAttack && Variables.Orbwalker.ActiveMode == OrbwalkingMode.Hybrid && Menu["Misc"]["Support"] && Args.Target.Type == GameObjectType.obj_AI_Minion)
            {
                Args.Process = false;
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (Me.IsDead)
            {
                return;
            }

            if (Menu["Draw"]["Q"] && Q.IsReady())
                Render.Circle.DrawCircle(Me.Position, Q.Range, System.Drawing.Color.AliceBlue, 2);

            if (Menu["Draw"]["W"] && (W.IsReady() || Me.HasBuff("HecarimW")))
                Render.Circle.DrawCircle(Me.Position, W.Range, System.Drawing.Color.LightSeaGreen, 2);

            if (Menu["Draw"]["E"] && E.IsReady())
                Render.Circle.DrawCircle(Me.Position, E.Range, System.Drawing.Color.OrangeRed, 2);

            if (Menu["Draw"]["DrawDamage"])
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(e => e.IsValidTarget() && e.IsValid && !e.IsDead && !e.IsZombie))
                {
                    HpBarDraw.Unit = target;
                    HpBarDraw.DrawDmg(GetDamage(target), new ColorBGRA(255, 204, 0, 170));
                }
            }

            if (Menu["Draw"]["Auto"].GetValue<MenuBool>())
            {
                var text = "";

                if (Menu["Harass"]["Auto"].GetValue<MenuKeyBind>().Active)
                {
                    text = "On";
                }
                else
                {
                    text = "Off";
                }

                Drawing.DrawText(Me.HPBarPosition.X + 30, Me.HPBarPosition.Y - 40, System.Drawing.Color.Red, "Auto Q (" + Menu["Harass"]["Auto"].GetValue<MenuKeyBind>().Key + "): ");
                Drawing.DrawText(Me.HPBarPosition.X + 115, Me.HPBarPosition.Y - 40, System.Drawing.Color.Yellow, text);
            }

            if (Menu["Draw"]["ComboE"].GetValue<MenuBool>())
            {
                var text = "";
                var x = Drawing.WorldToScreen(Me.Position).X;
                var y = Drawing.WorldToScreen(Me.Position).Y;

                if (Menu["Combo"]["E"].GetValue<MenuList>().Index == 0)
                    text = "Low Hp!";
                else if (Menu["Combo"]["E"].GetValue<MenuList>().Index == 1)
                    text = "Gank!";
                else
                    text = "Off!";

                Drawing.DrawText(x - 55, y + 40, System.Drawing.Color.Red, "Combo E (" + Menu["Combo"]["ESwitch"].GetValue<MenuKeyBind>().Key + "): ");
                Drawing.DrawText(x + 50, y + 40, System.Drawing.Color.Yellow, text);
            }

            if (Menu["Draw"]["ComboR"].GetValue<MenuBool>())
            {
                var text = "";
                var x = Drawing.WorldToScreen(Me.Position).X;
                var y = Drawing.WorldToScreen(Me.Position).Y;

                if (Menu["Combo"]["R"].GetValue<MenuList>().Index == 0)
                    text = "All!";
                else if (Menu["Combo"]["R"].GetValue<MenuList>().Index == 1)
                    text = "Q!";
                else if (Menu["Combo"]["R"].GetValue<MenuList>().Index == 2)
                    text = "W!";
                else if (Menu["Combo"]["R"].GetValue<MenuList>().Index == 3)
                    text = "E!";
                else if (Menu["Combo"]["R"].GetValue<MenuList>().Index == 4)
                    text = "Off!";

                Drawing.DrawText(x - 55, y + 60, System.Drawing.Color.Red, "Combo R (" + Menu["Combo"]["RSwitch"].GetValue<MenuKeyBind>().Key + "): ");
                Drawing.DrawText(x + 50, y + 60, System.Drawing.Color.Yellow, text);
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Me.IsDead)
            {
                return;
            }

            SwitchE();

            SwitchR();

            if (Variables.Orbwalker.ActiveMode == OrbwalkingMode.Combo)
            {
                Combo();
            }

            if (Variables.Orbwalker.ActiveMode == OrbwalkingMode.Hybrid)
            {
                Harass();
            }

            if (Variables.Orbwalker.ActiveMode == OrbwalkingMode.LaneClear)
            {
                Lane();
                Jungle();
            }

            if (Menu["Flee"]["Key"].GetValue<MenuKeyBind>().Active)
            {
                Flee();
            }

            if (Menu["Harass"]["Auto"].GetValue<MenuKeyBind>().Active)
            {
                AutoHarass();
            }

            KillSteal();

            SkinLogic();
        }

        private static void SwitchE()
        {
            if (Menu["Combo"]["ESwitch"].GetValue<MenuKeyBind>().Active)
            {
                if (Variables.TickCount - SwitchETickCount > 500)
                {
                    SwitchETickCount = Variables.TickCount;

                    var EMode = Menu["Combo"]["E"].GetValue<MenuList>().Index;

                    switch (EMode)
                    {
                        case 0:
                            Menu["Combo"]["E"].GetValue<MenuList>().Index = 1;
                            break;
                        case 1:
                            Menu["Combo"]["E"].GetValue<MenuList>().Index = 2;
                            break;
                        case 2:
                            Menu["Combo"]["E"].GetValue<MenuList>().Index = 0;
                            break;
                        default:
                            break;
                    }

                }
            }
        }

        private static void SwitchR()
        {
            if (Menu["Combo"]["RSwitch"].GetValue<MenuKeyBind>().Active)
            {
                if (Variables.TickCount - SwitchRTickCount > 500)
                {
                    SwitchRTickCount = Variables.TickCount;

                    var EMode = Menu["Combo"]["R"].GetValue<MenuList>().Index;

                    switch (EMode)
                    {
                        case 0:
                            Menu["Combo"]["R"].GetValue<MenuList>().Index = 1;
                            break;
                        case 1:
                            Menu["Combo"]["R"].GetValue<MenuList>().Index = 2;
                            break;
                        case 2:
                            Menu["Combo"]["R"].GetValue<MenuList>().Index = 3;
                            break;
                        case 3:
                            Menu["Combo"]["R"].GetValue<MenuList>().Index = 4;
                            break;
                        case 4:
                            Menu["Combo"]["R"].GetValue<MenuList>().Index = 0;
                            break;
                        default:
                            break;
                    }

                }
            }
        }

        private static void Combo()
        {
            var target = Variables.TargetSelector.GetTarget(1200, DamageType.Magical);

            if (target != null && target.IsHPBarRendered)
            {
                if (Menu["Combo"]["Q"] && Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    var QPred = Q.GetPrediction(target, (Menu["Combo"]["R"].GetValue<MenuList>().Index == 0 || Menu["Combo"]["R"].GetValue<MenuList>().Index == 1));

                    if (QPred.Hitchance >= HitChance.VeryHigh)
                    {
                        if ((Menu["Combo"]["R"].GetValue<MenuList>().Index == 0 || Menu["Combo"]["R"].GetValue<MenuList>().Index == 1) && R.IsReady())
                        {
                            R.Cast();
                        }
                        else
                        {
                            Q.Cast(QPred.CastPosition);
                        }
                    }
                }

                if (Menu["Combo"]["W"] && W.IsReady() && target.IsValidTarget(W.Range))
                {
                    if ((Menu["Combo"]["R"].GetValue<MenuList>().Index == 0 || Menu["Combo"]["R"].GetValue<MenuList>().Index == 2) && R.IsReady())
                    {
                        R.Cast();
                    }
                    else
                    {
                        W.Cast(target);
                    }
                }

                if (Menu["Combo"]["E"].GetValue<MenuList>().Index != 2 && E.IsReady())
                {
                    if (Menu["Combo"]["E"].GetValue<MenuList>().Index == 0)
                    {
                        if (Me.HealthPercent <= Menu["Combo"]["EHp"].GetValue<MenuSlider>().Value)
                        {
                            if ((Menu["Combo"]["R"].GetValue<MenuList>().Index == 0 || Menu["Combo"]["R"].GetValue<MenuList>().Index == 3) && R.IsReady())
                            {
                                R.Cast();
                            }
                            else
                            {
                                E.Cast(Me);
                            }
                        }
                    }
                    else if (Menu["Combo"]["E"].GetValue<MenuList>().Index == 1)
                    {
                        if (target.DistanceToPlayer() <= Q.Range + 100)
                        {
                            if ((Menu["Combo"]["R"].GetValue<MenuList>().Index == 0 || Menu["Combo"]["R"].GetValue<MenuList>().Index == 3) && R.IsReady())
                            {
                                R.Cast();
                            }
                            else
                            {
                                E.Cast(Me);
                            }
                        }
                    }
                }

                if (Menu["Combo"]["Ignite"] && Ignite != SpellSlot.Unknown && Ignite.IsReady() && target.IsValidTarget(600) && target.HealthPercent < 20)
                {
                    Me.Spellbook.CastSpell(Ignite, target);
                }
            }
        }

        private static void Harass()
        {
            if (Me.ManaPercent >= Menu["Harass"]["Mana"].GetValue<MenuSlider>().Value)
            {
                var target = Variables.TargetSelector.GetTarget(Q.Range, DamageType.Magical);

                if (target != null && !target.IsDead && !target.IsZombie && target.IsHPBarRendered)
                {
                    if (Menu["Harass"]["Q"] && Q.IsReady() && target.IsValidTarget(Q.Range) && Q.CanCast(target))
                    {
                        var QPred = Q.GetPrediction(target, Menu["Harass"]["R"]);

                        if (QPred.Hitchance >= HitChance.VeryHigh)
                        {
                            if (Menu["Harass"]["R"] && R.IsReady())
                            {
                                R.Cast();
                            }
                            else
                            {
                                Q.Cast(QPred.CastPosition);
                            }
                        }
                    }
                }
            }
        }

        private static void Lane()
        {
            if (Me.ManaPercent >= Menu["LaneClear"]["Mana"].GetValue<MenuSlider>().Value)
            {
                if (Menu["LaneClear"]["Q"].GetValue<MenuSliderButton>().BValue && Q.IsReady())
                {
                    var Minions = GameObjects.EnemyMinions.Where(x => x.IsValidTarget(Q.Range)).ToList();

                    if (Minions.Count() > 0)
                    {
                        var QFarm = Q.GetLineFarmLocation(Minions, Q.Width);

                        if (QFarm.MinionsHit >= Menu["LaneClear"]["Q"].GetValue<MenuSliderButton>().SValue)
                        {
                            Q.Cast(QFarm.Position);
                        }
                    }
                }
            }
        }

        private static void Jungle()
        {
            var Mobs = GameObjects.Jungle.Where(x => x.IsValidTarget(W.Range) && !GameObjects.JungleSmall.Contains(x)).ToList();

            if (Mobs.Count() > 0)
            {
                var mob = Mobs.FirstOrDefault();

                if (Me.ManaPercent >= Menu["JungleClear"]["Mana"].GetValue<MenuSlider>().Value)
                {
                    if (Menu["JungleClear"]["Q"] && Q.IsReady() && mob.IsValidTarget(Q.Range))
                    {
                        if (Menu["JungleClear"]["R"] && R.IsReady())
                        {
                            R.Cast();
                        }
                        else
                        {
                            Q.Cast(mob);
                        }
                    }

                    if (Menu["JungleClear"]["W"] && W.IsReady() && mob.IsValidTarget(W.Range))
                    {
                        W.Cast(mob);
                    }

                    if (Menu["JungleClear"]["E"] && W.IsReady() && mob.IsAttackingPlayer)
                    {
                        E.Cast(Me);
                    }
                }
            }
        }

        private static void Flee()
        {
            Variables.Orbwalker.Move(Game.CursorPos);

            if (Menu["Flee"]["E"] && E.IsReady())
            {
                if (Menu["Flee"]["R"] && R.IsReady())
                {
                    R.Cast();
                }
                else
                {
                    E.Cast(Me);
                }
            }

            var target = Variables.TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            if (target != null && target.IsHPBarRendered)
            {
                if (Menu["Flee"]["Q"] && Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    var QPred = Q.GetPrediction(target, Menu["Flee"]["R"]);

                    if (QPred.Hitchance >= HitChance.VeryHigh)
                    {
                        if (Menu["Flee"]["R"] && R.IsReady())
                        {
                            R.Cast();
                        }
                        else
                        {
                            Q.Cast(QPred.CastPosition);
                        }
                    }
                }

                if (Menu["Flee"]["W"] && W.IsReady() && target.IsValidTarget(W.Range))
                {
                    if (Menu["Flee"]["R"] && R.IsReady())
                    {
                        R.Cast();
                    }
                    else
                    {
                        W.Cast(target);
                    }
                }
            }
        }

        private static void AutoHarass()
        {
            if (!Me.IsRecalling() && !Me.IsUnderEnemyTurret() && Variables.Orbwalker.ActiveMode != OrbwalkingMode.Combo && Variables.Orbwalker.ActiveMode != OrbwalkingMode.Hybrid && Me.ManaPercent >= Menu["Harass"]["AutoMana"].GetValue<MenuSlider>().Value)
            {
                var target = Variables.TargetSelector.GetTarget(Q.Range, DamageType.Physical);

                if (target != null && !target.IsDead && !target.IsZombie && target.IsHPBarRendered)
                {
                    if (Q.IsReady() && target.IsValidTarget(Q.Range))
                    {
                        var QPred = Q.GetPrediction(target, Menu["Harass"]["R"]);

                        if (QPred.Hitchance >= HitChance.VeryHigh)
                        {
                            if (Menu["Harass"]["R"] && R.IsReady())
                            {
                                R.Cast();
                            }
                            else
                            {
                                Q.Cast(QPred.CastPosition);
                            }
                        }
                    }
                }
            }
        }

        private static void KillSteal()
        {
            foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(Q.Range) && !x.IsZombie && x.IsHPBarRendered && Menu["KillSteal"][x.ChampionName.ToLower()]))
            {
                if (target != null)
                {
                    if (Menu["KillSteal"]["Q"] && Q.IsReady())
                    {
                        var QPred = Q.GetPrediction(target, Menu["KillSteal"]["R"]);

                        if (QPred.Hitchance >= HitChance.VeryHigh)
                        {
                            if (Menu["Flee"]["R"] && R.IsReady())
                            {
                                R.Cast();
                            }
                            else
                            {
                                Q.Cast(QPred.CastPosition);
                            }
                        }
                    }

                    if (Menu["KillSteal"]["W"] && W.IsReady())
                    {
                        if (Menu["KillSteal"]["R"] && R.IsReady())
                        {
                            R.Cast();
                        }
                        else
                        {
                            W.Cast(target);
                        }
                    }
                }
            }
        }

        private static void SkinLogic()
        {
            if (Menu["Skin"]["Enable"])
            {
            }
            else if (!Menu["Skin"]["Enable"])
            {
            }
        }

        private static float GetDamage(Obj_AI_Base enemy)
        {
            float Damage = 0;

            if (Q.IsReady())
                Damage += Q.GetDamage(enemy);

            if (W.IsReady())
                Damage += W.GetDamage(enemy);

            return Damage;
        }

        private static bool CanMove(AIHeroClient hero)
        {
            bool CanMove = true;

            DebuffTypes.Add(BuffType.Blind);
            DebuffTypes.Add(BuffType.Charm);
            DebuffTypes.Add(BuffType.Fear);
            DebuffTypes.Add(BuffType.Flee);
            DebuffTypes.Add(BuffType.Stun);
            DebuffTypes.Add(BuffType.Snare);
            DebuffTypes.Add(BuffType.Taunt);
            DebuffTypes.Add(BuffType.Suppression);
            DebuffTypes.Add(BuffType.Polymorph);
            DebuffTypes.Add(BuffType.Blind);
            DebuffTypes.Add(BuffType.Silence);

            foreach (var buff in hero.Buffs)
            {
                if (DebuffTypes.Contains(buff.Type) && (buff.EndTime - Game.Time) * 1000 >= 800 && buff.IsActive)
                {
                    CanMove = false;
                }
            }

            if (!hero.CanMove)
            {
                CanMove = false;
            }

            return CanMove;
        }
    }
}
