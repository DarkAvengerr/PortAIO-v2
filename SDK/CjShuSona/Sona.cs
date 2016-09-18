using EloBuddy;
using LeagueSharp.SDK;
namespace CjShuSona
{

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.UI;
    using SharpDX;
    using System;
    using System.Linq;

    class Sona
    {

        private static AIHeroClient Player, Target = null;
        private static Spell Q, W, E, R;
        private static SpellSlot Ignite = SpellSlot.Unknown;
        private static Menu Menu;
        private static HpBarDraw DrawHpBar = new HpBarDraw();
        private static string[] AutoEnableList =
        {
             "Annie", "Ahri", "Akali", "Anivia", "Annie", "Brand", "Cassiopeia", "Diana", "Evelynn", "FiddleSticks", "Fizz", "Gragas", "Heimerdinger", "Karthus",
             "Kassadin", "Katarina", "Kayle", "Kennen", "Leblanc", "Lissandra", "Lux", "Malzahar", "Mordekaiser", "Morgana", "Nidalee", "Orianna",
             "Ryze", "Sion", "Swain", "Syndra", "Teemo", "TwistedFate", "Veigar", "Viktor", "Vladimir", "Xerath", "Ziggs", "Zyra", "Velkoz", "Azir", "Ekko",
             "Ashe", "Caitlyn", "Corki", "Draven", "Ezreal", "Graves", "Jayce", "Jinx", "KogMaw", "Lucian", "MasterYi", "MissFortune", "Quinn", "Shaco", "Sivir",
             "Talon", "Tristana", "Twitch", "Urgot", "Varus", "Vayne", "Yasuo", "Zed", "Kindred", "AurelionSol"
        };

        internal static void OnLoad(object sender, EventArgs e)
        {
            if (GameObjects.Player.ChampionName.ToLower() != "sona")
                return;

            Player = GameObjects.Player;

            Chat.Print("<font color='#2848c9'>CjShu Sona</font> -> <font color='#b756c5'>Version : 1.0.0.0</font> <font size='30'><font color='#d949d4'>Good Luck!</font></font>");

            LoadSpell();
            LoadMenu();
            LoadEvent();
        }

        private static void LoadSpell()
        {
            Q = new Spell(SpellSlot.Q, 850f);
            W = new Spell(SpellSlot.W, 1000f);
            E = new Spell(SpellSlot.E, 350f);
            R = new Spell(SpellSlot.R, 1000f);

            R.SetSkillshot(0.5f, 125, 3000f, false, SkillshotType.SkillshotLine);
        }

        private static void LoadMenu()
        {
            Menu = new Menu("CjShu.Sona", "CjShu Sona (36E奶索娜)", true).Attach();

            var ComboMenu = Menu.Add(new Menu("Combo", "Combo(連招按鍵)"));
            {
                ComboMenu.Add(new MenuBool("Q", "Use Q (用Q)", true));
                ComboMenu.Add(new MenuBool("W", "Use W (用W)", true));
                ComboMenu.Add(new MenuSlider("WM", "W | Me Hp <= % (用W| 當自己HP少餘)", 40));
                ComboMenu.Add(new MenuSlider("WA", "W | Ally Hp <= % (用W |當隊友HP少餘)", 40));
                ComboMenu.Add(new MenuBool("R", "R (連招R)", true));
                ComboMenu.Add(new MenuSlider("RC", "R | Counts >= (最少開R數量)", 3, 1, 5));
            }

            var HarassMenu = Menu.Add(new Menu("Harass", "Harass(騷擾按鍵)"));
            {
                HarassMenu.Add(new MenuBool("Q", "Q(設定)", true));
                HarassMenu.Add(new MenuSlider("Mana", "Harass Mode | Min ManaPercent >= % (騷擾魔力)", 60));
            }

            var LaneClearMenu = Menu.Add(new Menu("LaneClear", "Lane Clear(清線設定)"));
            {
                LaneClearMenu.Add(new MenuSliderButton("Q", "Use Q | Min Hit Count >= (最少數用Q)", 3, 1, 5, true));
                LaneClearMenu.Add(new MenuSlider("Mana", "LaneClear Mode | Min ManaPercent >= % (清線魔力)", 60));
            }

            var JungleClearMenu = Menu.Add(new Menu("JungleClear", "Jungle Clear(清野設定)"));
            {
                JungleClearMenu.Add(new MenuBool("Q", "Use Q (使用Q)", true));
                JungleClearMenu.Add(new MenuBool("W", "Use W (使用W)", true));
                JungleClearMenu.Add(new MenuSlider("Mana", "JungleClear Mode | Min ManaPercent >= % (最少魔力)", 60));
            }

            var KillStealMenu = Menu.Add(new Menu("KillSteal", "Kill Steal(擊殺設定)"));
            {
                KillStealMenu.Add(new MenuBool("Q", "Use Q (使用Q)", true));
                KillStealMenu.Add(new MenuBool("R", "Use R (使用R)", true));
                KillStealMenu.Add(new MenuBool("GPE", "Use E Gapclose (使用E反突)", true));
                KillStealMenu.Add(new MenuSeparator("RList", "R List (R英雄列表)"));
                if (GameObjects.EnemyHeroes.Any())
                {
                    GameObjects.EnemyHeroes.ForEach(i => KillStealMenu.Add(
                        new MenuBool(i.ChampionName.ToLower(), i.ChampionName,
                        AutoEnableList.Contains(i.ChampionName))));
                }
            }

            var AutoMenu = Menu.Add(new Menu("Auto", "Auto (自動模式)"));
            {
                AutoMenu.Add(new MenuBool("W", "W (自動W)", true));
                AutoMenu.Add(new MenuSlider("WM", "W | Me Hp <= % (自己HP多少用W)", 40));
                AutoMenu.Add(new MenuSlider("WA", "W | Ally Hp <= % (對友HP用W)", 40));
                AutoMenu.Add(new MenuSlider("Mana", "Auto Mode | Min ManaPercent >= % (最低魔力停用)", 60));
            }

            var FleeMenu = Menu.Add(new Menu("Flee", "Flee (逃跑按鍵)"));
            {
                FleeMenu.Add(new MenuBool("E", "Use E (使用E)", true));
                FleeMenu.Add(new MenuKeyBind("Key", "Flee Key (逃跑按鍵)", System.Windows.Forms.Keys.Z, KeyBindType.Press));
            }

            var SkinMenu = Menu.Add(new Menu("Skin", "Skin (造型)"));
            {
                SkinMenu.Add(new MenuBool("Enable", "Enabled (啟動)", false));
                SkinMenu.Add(new MenuList<string>("SkinName", "Select Skin (36E造型)", new[] { "Classic(原味)", "Void Bringer (女神)" }));
            }

            var DrawMenu = Menu.Add(new Menu("Draw", "Draw"));
            {
                DrawMenu.Add(new MenuBool("Q", "Q Range(Q範圍)"));
                DrawMenu.Add(new MenuBool("W", "W Range(W範圍)"));
                DrawMenu.Add(new MenuBool("E", "E Range(E範圍)"));
                DrawMenu.Add(new MenuBool("R", "R Range(R範圍)"));
                DrawMenu.Add(new MenuBool("顯示魔力線條", "Draw Mana Bar Indicator(魔力線條)"));
                DrawMenu.Add(new MenuBool("DrawDamage", "Draw Combo Damage", true));
            }

            Menu.Add(new MenuBool("Support", "Support Mode!(索娜也瘋狂)", true));
        }

        private static void LoadEvent()
        {
            Variables.Orbwalker.OnAction += OnAction;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Drawing.OnEndScene += OnEndScene;
            Events.OnGapCloser += (sender, e) =>
            {
                if (Variables.Orbwalker.ActiveMode == OrbwalkingMode.Hybrid)
                {
                    if (Menu["KillSteal"]["GPE"].GetValue<MenuBool>() && E.IsReady())
                    {
                        return;
                    }
                    E.Cast();
                }
            };
        }


        private static void OnEndScene(EventArgs args)
        {

            var color = new ColorBGRA(255, 255, 255, 255);
            var qMana = new[] { 0, 40, 50, 60, 70, 80 };
            var wMana = new[] { 0, 60, 70, 80, 90, 100 }; // W Mana Cost doesnt works :/
            var eMana = new[] { 0, 50, 50, 50, 50, 50 };
            var rMana = new[] { 0, 100, 100, 100 };

            if (!Menu["Draw"]["顯示魔力線條"].GetValue<MenuBool>())
            {
                var TotaCosMana = qMana[Q.Level] + wMana[W.Level] + eMana[E.Level] + rMana[R.Level];

                HpBarDraw.DrawDmg(TotaCosMana, TotaCosMana > Player.Mana ?
                    new ColorBGRA(255, 0, 0, 255) :
                    new ColorBGRA(255, 255, 255, 255));
            }
        }

        private static void OnAction(object sender, OrbwalkingActionArgs Args)
        {
            if (Args.Type == OrbwalkingType.BeforeAttack && Variables.Orbwalker.ActiveMode == OrbwalkingMode.Hybrid &&
                Menu["Support"] && Args.Target is Obj_AI_Minion)
            {
                Args.Process = false;
            }
        }

        private static void OnUpdate(EventArgs Args)
        {
            if (Player.IsDead)
            {
                return;
            }

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

            Auto();
            KillSteal();
            Skin();
        }

        private static void Auto()
        {
            if (Player.ManaPercent >= Menu["Auto"]["Mana"].GetValue<MenuSlider>().Value)
            {
                if (Menu["Auto"]["W"] && W.IsReady())
                {
                    if (Player.HealthPercent <= Menu["Auto"]["WM"].GetValue<MenuSlider>().Value)
                    {
                        W.Cast();
                    }

                    var ally = GameObjects.AllyHeroes.Where(x => x.IsValidTarget(W.Range) &&
                    x.HealthPercent <= Menu["Auto"]["WA"].GetValue<MenuSlider>().Value).FirstOrDefault();

                    if (ally != null)
                    {
                        W.Cast();
                    }
                }
            }
        }

        private static void Combo()
        {
            var target = Variables.TargetSelector.GetTarget(Q.Range, DamageType.Physical);

            if (target != null && !target.IsDead && !target.IsZombie && target.IsHPBarRendered)
            {
                if (Menu["Combo"]["Q"] && Q.IsReady() && target.IsValidTarget(Q.Range) && Q.CanCast(target))
                {
                    Q.Cast();
                }

                if (Menu["Combo"]["W"] && W.IsReady())
                {
                    if (Player.HealthPercent <= Menu["Combo"]["WM"].GetValue<MenuSlider>().Value)
                    {
                        W.Cast();
                    }

                    var ally = GameObjects.AllyHeroes.Where(x => x.IsValidTarget(W.Range) &&
                    x.HealthPercent <= Menu["Combo"]["WA"].GetValue<MenuSlider>().Value).FirstOrDefault();

                    if (ally != null)
                    {
                        W.Cast();
                    }
                }

                if (Menu["Combo"]["R"] && R.IsReady() && target.IsValidTarget(R.Range))
                {
                    if (target.Health <= GetDamage(target) + Player.GetAutoAttackDamage(target) * 2)
                    {
                        R.Cast(target);
                    }
                    else
                    {
                        R.CastIfWillHit(target, Menu["Combo"]["RCount"].GetValue<MenuSlider>().Value);
                    }
                }

                if (Menu["Combo"]["Ignite"] && Ignite != SpellSlot.Unknown && Ignite.IsReady() &&
                    target.IsValidTarget(600) && target.HealthPercent < 20)
                {
                    Player.Spellbook.CastSpell(Ignite, target);
                }
            }
        }

        private static void Harass()
        {
            if (Player.ManaPercent >= Menu["Harass"]["Mana"].GetValue<MenuSlider>().Value)
            {
                var target = Variables.TargetSelector.GetTarget(Q.Range, DamageType.Physical);

                if (target != null && !target.IsDead && !target.IsZombie && target.IsHPBarRendered)
                {
                    if (Menu["Harass"]["Q"] && Q.IsReady() && target.IsValidTarget(Q.Range)
                        && Q.CanCast(target))
                    {
                        Q.Cast(target);
                    }
                }
            }
        }

        private static void Lane()
        {
            if (Player.ManaPercent >= Menu["LaneClear"]["Mana"].GetValue<MenuSlider>().Value)
            {
                if (Menu["LaneClear"]["Q"].GetValue<MenuSliderButton>().BValue && Q.IsReady())
                {
                    var Minions = GameObjects.EnemyMinions.Where(x => x.IsValidTarget(Q.Range))
                        .ToList();

                    if (Minions.Count() > 0)
                    {
                        if (Minions.Count() >=
                            Menu["LaneClear"]["Q"].GetValue<MenuSliderButton>().SValue)
                        {
                            Q.Cast();
                        }
                    }
                }
            }
        }

        private static void Jungle()
        {
            var Mobs = GameObjects.Jungle.Where(x => x.IsValidTarget(Q.Range) &&
            !GameObjects.JungleSmall.Contains(x)).ToList();

            if (Mobs.Count() > 0)
            {
                if (Player.ManaPercent >= Menu["JungleClear"]["Mana"].GetValue<MenuSlider>().Value)
                {
                    if (Menu["JungleClear"]["Q"] && Q.IsReady())
                    {
                        Q.Cast();
                    }

                    if (Menu["JungleClear"]["W"] && W.IsReady() &&
                        Mobs.FirstOrDefault().IsAttackingPlayer)
                    {
                        W.Cast();
                    }
                }
            }
        }

        private static void Flee()
        {
            Variables.Orbwalker.Move(Game.CursorPos);

            if (Menu["Flee"]["E"] && E.IsReady())
                E.Cast();
        }

        private static void KillSteal()
        {
            if (Menu["KillSteal"]["Q"] && Q.IsReady())
            {
                var qt = GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(Q.Range) &&
                x.Health < Q.GetDamage(x)).FirstOrDefault();

                if (qt != null)
                {
                    Q.Cast();
                    return;
                }
            }

            if (Menu["KillSteal"]["R"] && R.IsReady())
            {
                var rt = GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(R.Range)
                && x.Health < R.GetDamage(x) && Menu["KillSteal"][x.ChampionName.ToLower()]).FirstOrDefault();

                if (rt != null)
                {
                    R.Cast(rt);
                    return;
                }
            }
        }

        private static void Skin()
        {
        }

        private static void OnDraw(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            if (Menu["Draw"]["Q"].GetValue<MenuBool>() && Q.IsReady())
                Drawing.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.AliceBlue);

            if (Menu["Draw"]["W"].GetValue<MenuBool>() && W.IsReady())
                Drawing.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.DarkBlue);

            if (Menu["Draw"]["E"].GetValue<MenuBool>() && E.IsReady())
                Drawing.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.Orange);

            if (Menu["Draw"]["R"].GetValue<MenuBool>() && E.IsReady())
                Drawing.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.Red);

            if (Menu["Draw"]["DrawDamage"].GetValue<MenuBool>())
            {
                foreach (var target in ObjectManager.Get<AIHeroClient>().
                    Where(e => e.IsValidTarget() && e.IsValid && !e.IsDead && !e.IsZombie))
                {
                    HpBarDraw.Unit = target;
                    HpBarDraw.DrawDmg(GetDamage(target), new SharpDX.ColorBGRA(255, 204, 0, 170));
                }
            }
        }

        private static float GetDamage(AIHeroClient target)
        {
            float Damage = 0f;

            if (Q.IsReady())
            {
                Damage += Q.GetDamage(target);
            }

            if (R.IsReady())
            {

                Damage += R.GetDamage(target);
            }

            return Damage;
        }
    }
}