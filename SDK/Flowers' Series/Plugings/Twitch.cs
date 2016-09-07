using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Flowers_Series.Plugings
{
    using Common;
    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;
    using System;
    using System.Linq;
    using static Common.Manager;

    public static class Twitch
    {
        private static Spell Q;
        private static Spell W;
        private static Spell E;
        private static Spell R;
        private static bool PlayerIsKillTarget;
        private static HpBarDraw HpBarDraw = new HpBarDraw();

        private static Menu Menu => Program.Menu;
        private static AIHeroClient Me => Program.Me;

        public static void Init()
        {
            Q = new Spell(SpellSlot.Q, 0f);
            W = new Spell(SpellSlot.W, 950f);
            E = new Spell(SpellSlot.E, 1200f);
            R = new Spell(SpellSlot.R, 975f);

            W.SetSkillshot(0.25f, 100f, 1410f, false, SkillshotType.SkillshotCircle);

            var Combo = Menu.Add(new Menu("Twitch_Combo", "Combo"));
            {
                Combo.Add(new MenuBool("ComboQ", "Use Q", true));
                Combo.Add(new MenuSlider("ComboQRange", "Serach Range", (int)Me.GetRealAutoAttackRange(), (int)Me.GetRealAutoAttackRange() - 200, (int)Me.GetRealAutoAttackRange() * 2));
                Combo.Add(new MenuSlider("ComboQCounts", "Min Enemies", 2, 1, 5));
                Combo.Add(new MenuBool("ComboW", "Use W", true));
                Combo.Add(new MenuBool("ComboE", "Use E", true));
                Combo.Add(new MenuBool("ComboEFull", "If Enemy Full Stack", true));
                Combo.Add(new MenuSlider("ComboEMin", "Or Enemy Leave E Range Min Stacks Counts (7 is Off)", 3, 1, 7));
                Combo.Add(new MenuBool("ComboR", "Use R", true));
                Combo.Add(new MenuSlider("ComboRCounts", "Counts Enemies >=", 2, 1, 5));
                Combo.Add(new MenuSlider("ComboRRange", "Search Range", 800, 500, 1500));
            }

            var Harass = Menu.Add(new Menu("Twitch_Harass", "Harass"));
            {
                Harass.Add(new MenuBool("HarassW", "Use W", true));
                Harass.Add(new MenuSlider("HarassWMana", "Min Mana Percent", 60, 0, 100));
                Harass.Add(new MenuBool("HarassE", "Use E", true));
                Harass.Add(new MenuSlider("HarassEMin", "Min Stacks Counts", 2, 1, 6));
                Harass.Add(new MenuSlider("HarassEMana", "Min Mana Percent", 60, 0, 100));
            }

            var Misc = Menu.Add(new Menu("Twitch_Misc", "Misc"));
            {
                Misc.Add(new MenuSeparator("SWWW", "If Player Kill Emeny"));
                Misc.Add(new MenuSeparator("SWWE", "And Have Others Enemies Hereabout"));
                Misc.Add(new MenuBool("AutoQ", " Auto Q!", true));
                Misc.Add(new MenuBool("JungleStealE", "Use E Steal Dragon/Baron", true));
                Misc.Add(new MenuBool("OnlyInCleanMode", "Only In LaneClear Mode Steal", true));
                Misc.Add(new MenuBool("KillStealE", "Auto E KillSteal", true));
                Misc.Add(new MenuBool("AutoYoumuu", "Auto Youmuu | If Ult Active!", true));
            }

            var Draw = Menu.Add(new Menu("Twitch_Draw", "Draw"));
            {
                Draw.Add(new MenuBool("DrawW", "Draw W Range"));
                Draw.Add(new MenuBool("DrawE", "Draw E Range"));
                Draw.Add(new MenuBool("DrawR", "Draw R Range"));
                Draw.Add(new MenuBool("Damage", "Draw Combo Damage", true));
            }

            WriteConsole(GameObjects.Player.ChampionName + " Inject!");

            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Game.OnNotify += OnNotify;
            Drawing.OnDraw += OnDraw;
            Obj_AI_Base.OnSpellCast += OnSpellCast;
        }

        private static void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead)
                return;

            if (InCombo)
            {
                var e = GetTarget(1200);

                if (Menu["Twitch_Combo"]["ComboQ"] &&
                    Me.CountEnemyHeroesInRange(Menu["Twitch_Combo"]["ComboQRange"].GetValue<MenuSlider>().Value) >=
                    Menu["Twitch_Combo"]["ComboQCounts"].GetValue<MenuSlider>().Value && Q.IsReady())
                {
                    Q.Cast();
                }

                if (Menu["Twitch_Combo"]["ComboW"] && W.IsReady() && e.IsValidTarget(W.Range) && W.CanCast(e))
                {
                    W.Cast(e);
                }

                if (Menu["Twitch_Combo"]["ComboE"] && E.IsReady() && e.IsValidTarget(E.Range))
                {
                    if (e.DistanceToPlayer() >= E.Range * 0.7 && GetECount(e) >= Menu["Twitch_Combo"]["ComboEMin"].GetValue<MenuSlider>().Value)
                    {
                        E.Cast();
                    }

                    if (Menu["Twitch_Combo"]["ComboEFull"] && GetECount(e) == 6)
                    {
                        E.Cast();
                    }
                }

                if (Menu["Twitch_Combo"]["ComboR"] && R.IsReady() && 
                    Me.CountEnemyHeroesInRange(Menu["Twitch_Combo"]["ComboRRange"].GetValue<MenuSlider>().Value) >=
                    Menu["Twitch_Combo"]["ComboRCounts"].GetValue<MenuSlider>().Value)
                {
                    R.Cast();
                }
            }

            if (InHarass)
            {
                var WTarget = GetTarget(W.Range);
                var ETarget = GetTarget(E.Range);

                if (Menu["Twitch_Harass"]["HarassW"] && W.IsReady() && 
                    Me.ManaPercent >= Menu["Twitch_Harass"]["HarassWMana"].GetValue<MenuSlider>().Value && 
                    WTarget.IsValidTarget(W.Range) && W.CanCast(WTarget))
                {
                    W.Cast(WTarget);
                }

                if (Menu["Twitch_Harass"]["HarassE"] && E.IsReady()
                    && Me.ManaPercent >= Menu["Twitch_Harass"]["HarassEMana"].GetValue<MenuSlider>().Value &&
                    ETarget.IsValidTarget(E.Range))
                {
                    if (ETarget.DistanceToPlayer() >= E.Range * 0.7 && GetECount(ETarget) >= Menu["Twitch_Harass"]["HarassEMin"].GetValue<MenuSlider>())
                    {
                        E.Cast();
                    }
                }
            }

            if (Menu["Twitch_Misc"]["JungleStealE"])
            {
                if (Menu["Twitch_Misc"]["OnlyInCleanMode"] && InClear || !Menu["Twitch_Misc"]["OnlyInCleanMode"])
                {
                    var mobs = GetMobs(Me.Position, E.Range);

                    foreach (var mob in mobs)
                    {
                        if (mob.CharData.BaseSkinName.Contains("Dragon") || mob.CharData.BaseSkinName.Contains("Baron"))
                        {
                            if (E.IsReady() && E.CanKill(mob) && mob.IsValidTarget(E.Range))
                            {
                                E.Cast();
                            }
                        }
                    }
                }
            }

            if (Menu["Twitch_Misc"]["KillStealE"] && E.IsReady())
            {
                foreach (var e in GameObjects.EnemyHeroes)
                {
                    if (e.IsValidTarget(E.Range) && !e.IsZombie && Me.GetSpellDamage(e, SpellSlot.E) > e.Health + 5)
                    {
                        E.Cast();
                    }
                }
            }
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (Menu["Twitch_Misc"]["AutoQ"])
            {
                if (sender.IsMe && PlayerIsKillTarget == true && Q.IsReady() && Me.CountEnemyHeroesInRange(850) >= 1)
                {
                    Q.Cast();
                }
            }
        }

        private static void OnNotify(GameNotifyEventArgs Args)
        {
            if (Me.IsDead)
            {
                PlayerIsKillTarget = false;
            }
            else if (!Me.IsDead)
            {
                if (Args.EventId == GameEventId.OnChampionDie)
                {
                    if (Args.NetworkId == Me.NetworkId)
                    {
                        PlayerIsKillTarget = true;
                        DelayAction.Add(8000, () => { PlayerIsKillTarget = false; });
                    }
                }
            }
        }

        private static void OnDraw(EventArgs Args)
        {
            if (Me.IsDead)
                return;

            if (W.IsReady() && Menu["Twitch_Draw"]["DrawW"])
                Render.Circle.DrawCircle(Me.Position, W.Range, System.Drawing.Color.AliceBlue);

            if (E.IsReady() && Menu["Twitch_Draw"]["DrawE"])
                Render.Circle.DrawCircle(Me.Position, E.Range, System.Drawing.Color.AliceBlue);

            if (R.IsReady() && Menu["Twitch_Draw"]["DrawR"])
                Render.Circle.DrawCircle(Me.Position, R.Range, System.Drawing.Color.AliceBlue);

            if (Menu["Twitch_Draw"]["Damage"])
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget() && !x.IsDead && !x.IsZombie))
                {
                    if (target != null)
                    {
                        HpBarDraw.Unit = target;

                        HpBarDraw.DrawDmg((float)GetDamage(target), new SharpDX.ColorBGRA(255, 200, 0, 170));
                    }
                }
            }
        }

        private static void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (Menu["Twitch_Misc"]["AutoYoumuu"] &&
                InCombo && Variables.Orbwalker.GetTarget() != null &&
                Variables.Orbwalker.GetTarget() is AIHeroClient && Me.HasBuff("TwitchFullAutomatic"))
            {
                if (Items.HasItem(3142))
                {
                    Items.UseItem(3142);
                    return;
                }
            }
        }

        private static int GetECount(Obj_AI_Base target)
        {
            if (target.HasBuff("TwitchDeadlyVenom"))
            {
                return target.GetBuffCount("TwitchDeadlyVenom");
            }
            else
                return 0;
        }
    }
}
