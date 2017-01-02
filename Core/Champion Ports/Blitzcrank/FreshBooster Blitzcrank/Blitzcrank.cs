using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using static FreshBooster.FreshCommon;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace FreshBooster.Champion
{
    class Blitzcrank
    {
        // Default Setting
        public static int ErrorTime;
        public const string ChampName = "Blitzcrank";   // Edit
        public static AIHeroClient Player;
        public static Spell _Q, _W, _E, _R;
        
        // Default Setting

        private void SkillSet()
        {
            try
            {
                _Q = new Spell(SpellSlot.Q, 950f);
                _Q.SetSkillshot(0.25f, 70f, 1800f, true, SkillshotType.SkillshotLine);
                _W = new Spell(SpellSlot.W, 700f);
                _E = new Spell(SpellSlot.E, 150f);
                _R = new Spell(SpellSlot.R, 540f);
            }
            catch (Exception)
            {
                if (NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 01");
                    ErrorTime = TickCount(10000);
                }
            }
        }
        private void Menu()
        {
            try
            {
                var Combo = new Menu("Combo", "Combo");
                {
                    Combo.AddItem(new MenuItem("Blitzcrank_CUse_Q", "Use Q").SetValue(true));
                    Combo.AddItem(new MenuItem("Blitzcrank_CUse_W", "Use W").SetValue(true));
                    Combo.AddItem(new MenuItem("Blitzcrank_CUse_E", "Use E").SetValue(true));
                    Combo.AddItem(new MenuItem("Blitzcrank_CUse_R", "Use R").SetValue(true));
                    Combo.AddItem(new MenuItem("Blitzcrank_CUseQ_Hit", "Q HitChance").SetValue(new Slider(6, 1, 6)));
                    Combo.AddItem(new MenuItem("CKey", "Combo Key").SetValue(new KeyBind(32, KeyBindType.Press)));
                }
                _MainMenu.AddSubMenu(Combo);

                var Harass = new Menu("Harass", "Harass");
                {

                    Harass.AddItem(new MenuItem("Blitzcrank_HUse_Q", "Use Q").SetValue(true));
                    Harass.AddItem(new MenuItem("Blitzcrank_HUse_W", "Use W").SetValue(true));
                    Harass.AddItem(new MenuItem("Blitzcrank_HUse_E", "Use E").SetValue(true));
                    Harass.AddItem(new MenuItem("Blitzcrank_AManarate", "Mana %").SetValue(new Slider(20)));
                    Harass.AddItem(new MenuItem("HKey", "Harass Key").SetValue(new KeyBind('C', KeyBindType.Press)));
                }
                _MainMenu.AddSubMenu(Harass);

                var KillSteal = new Menu("KillSteal", "KillSteal");
                {
                    KillSteal.AddItem(new MenuItem("Blitzcran_KUse_Q", "Use Q").SetValue(true));                    
                    KillSteal.AddItem(new MenuItem("Blitzcran_KUse_R", "Use R").SetValue(true));
                }
                _MainMenu.AddSubMenu(KillSteal);

                var Misc = new Menu("Misc", "Misc");
                {
                    foreach (var enemy in ObjectManager.Get<AIHeroClient>())
                    {
                        if (enemy.Team != Player.Team)
                        {
                            Misc.SubMenu("SetGrab").AddItem(new MenuItem("Blitzcrank_GrabSelect" + enemy.ChampionName, enemy.ChampionName)).SetValue(new StringList(new[] { "Enable", "Dont", "Auto" }));
                        }
                    }
                    Misc.SubMenu("Interrupt").AddItem(new MenuItem("Blitzcrank_InterQ", "Use Q").SetValue(true));
                    Misc.SubMenu("Interrupt").AddItem(new MenuItem("Blitzcrank_InterE", "Use E").SetValue(true));
                    Misc.SubMenu("Interrupt").AddItem(new MenuItem("Blitzcrank_InterR", "Use R").SetValue(true));
                    Misc.AddItem(new MenuItem("Blitzcrank_GrabDash", "Grab to dashing enemy").SetValue(true));
                }
                _MainMenu.AddSubMenu(Misc);

                var Draw = new Menu("Draw", "Draw");
                {
                    Draw.AddItem(new MenuItem("Blitzcrank_Draw_Q", "Draw Q").SetValue(false));
                    Draw.AddItem(new MenuItem("Blitzcrank_Draw_R", "Draw R").SetValue(false));
                    Draw.AddItem(new MenuItem("Blitzcrank_Indicator", "Draw Damage Indicator").SetValue(true));
                }
                _MainMenu.AddSubMenu(Draw);
            }
            catch (Exception)
            {
                if (NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 02");
                    ErrorTime = TickCount(10000);
                }
            }
        }
        public static void Drawing_OnDraw(EventArgs args)
        {
            try
            {
                if (Player.IsDead) return;
                if (_MainMenu.Item("Blitzcrank_Draw_Q").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, _Q.Range, Color.White, 1);
                if (_MainMenu.Item("Blitzcrank_Draw_R").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, _R.Range, Color.White, 1);
                var QTarget = TargetSelector.GetTarget(_Q.Range, TargetSelector.DamageType.Magical);
                if (QTarget != null)
                    Drawing.DrawCircle(QTarget.Position, 150, Color.Green);
            }
            catch (Exception)
            {
                if (NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 03");
                    ErrorTime = TickCount(10000);
                }
            }
        }

        static float getComboDamage(Obj_AI_Base enemy)
        {
            try
            {
                if (enemy != null)
                {
                    float damage = 0;

                    if (_Q.IsReady())
                        damage += _Q.GetDamage(enemy);
                    if (_E.IsReady())
                        damage += _E.GetDamage(enemy);
                    if (_R.IsReady())
                        damage += _R.GetDamage(enemy);
                    if (!Player.Spellbook.IsAutoAttacking)
                        damage += (float)Player.GetAutoAttackDamage(enemy, true);
                    return damage;
                }
                return 0;
            }
            catch (Exception)
            {
                if (NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 04");
                    ErrorTime = TickCount(10000);
                }
                return 0;
            }
        }
        public static void Drawing_OnEndScene(EventArgs args)
        {
            try
            {
            }
            catch (Exception)
            {
                if (NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 05");
                    ErrorTime = TickCount(10000);
                }
            }
        }

        // OnLoad
        public Blitzcrank()
        {
            Player = ObjectManager.Player;
            SkillSet();
            Menu();
            Game.OnUpdate += OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += Drawing_OnEndScene;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Obj_AI_Base.OnSpellCast += OnProcessSpell;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;
            EloBuddy.Player.OnIssueOrder += Obj_AI_Base_OnIssueOrder;
        }

        private static void OnGameUpdate(EventArgs args)
        {
            try
            {
                if (Player.IsDead) return;
                var QTarget = TargetSelector.GetTarget(_Q.Range, TargetSelector.DamageType.Magical);
                var WTarget = TargetSelector.GetTarget(1500, TargetSelector.DamageType.Physical);
                var RTarget = TargetSelector.GetTarget(_R.Range, TargetSelector.DamageType.Magical);

                if (_MainMenu.Item("Blitzcrank_GrabDash").GetValue<bool>() && _Q.IsReady())
                    if (QTarget != null && _Q.GetPrediction(QTarget, false).Hitchance == HitChance.Dashing)
                        _Q.CastIfHitchanceEquals(QTarget, HitChance.Dashing, true);

                //killsteal
                if (_MainMenu.Item("Blitzcran_KUse_Q").GetValue<bool>() && QTarget != null && QTarget.Health < _Q.GetDamage(QTarget) && _Q.IsReady())
                {
                    _Q.CastIfHitchanceEquals(QTarget, HitChance.VeryHigh, true);
                    return;
                }
                if (_MainMenu.Item("Blitzcran_KUse_R").GetValue<bool>() && RTarget != null && RTarget.Health < _E.GetDamage(RTarget) && _R.IsReady())
                {
                    _R.Cast(true);
                    return;
                }

                if (QTarget == null) return;    // auto grab
                foreach (var enemy in ObjectManager.Get<AIHeroClient>())
                {
                    if (enemy.Team != Player.Team && QTarget != null
                        && _MainMenu.Item("Blitzcrank_GrabSelect" + enemy.ChampionName).GetValue<StringList>().SelectedIndex == 2 && _Q.IsReady()
                        && QTarget.ChampionName == enemy.ChampionName)
                    {
                        if (QTarget.CanMove && QTarget.Distance(Player.Position) < _Q.Range * 0.9)
                            _Q.CastIfHitchanceEquals(QTarget, Hitchance("Blitzcrank_CUseQ_Hit"), true);
                        if (!QTarget.CanMove)
                            _Q.CastIfHitchanceEquals(QTarget, Hitchance("Blitzcrank_CUseQ_Hit"), true);
                    }
                }

                // Combo
                if (_MainMenu.Item("CKey").GetValue<KeyBind>().Active)
                {
                    if (_MainMenu.Item("Blitzcrank_CUse_Q").GetValue<bool>() && _Q.IsReady() && QTarget != null
                        && _MainMenu.Item("Blitzcrank_GrabSelect" + QTarget.ChampionName).GetValue<StringList>().SelectedIndex != 1)
                    {
                        _Q.CastIfHitchanceEquals(QTarget, Hitchance("Blitzcrank_CUseQ_Hit"), true);
                    }
                    if (_MainMenu.Item("Blitzcrank_CUse_W").GetValue<bool>() && _W.IsReady() && WTarget != null)
                        _W.Cast(Player, true);
                    if (_MainMenu.Item("Blitzcrank_CUse_E").GetValue<bool>() && _E.IsReady() && QTarget.Distance(Player.ServerPosition) < 230)
                        _E.Cast(Player);
                    if (_MainMenu.Item("Blitzcrank_CUse_R").GetValue<bool>() && _R.IsReady() && RTarget != null)
                        _R.Cast();
                }

                // Harass
                if (_MainMenu.Item("HKey").GetValue<KeyBind>().Active && _MainMenu.Item("Blitzcrank_AManarate").GetValue<Slider>().Value < Player.ManaPercent)
                {
                    if (_MainMenu.Item("Blitzcrank_HUse_Q").GetValue<bool>() && _Q.IsReady() && QTarget != null
                        && _MainMenu.Item("Blitzcrank_GrabSelect" + QTarget.ChampionName).GetValue<StringList>().SelectedIndex != 1)
                    {
                        _Q.CastIfHitchanceEquals(QTarget, Hitchance("Blitzcrank_CUseQ_Hit"), true);
                    }
                    if (_MainMenu.Item("Blitzcrank_HUse_W").GetValue<bool>() && _W.IsReady() && WTarget != null)
                        _W.Cast(Player, true);
                    if (_MainMenu.Item("Blitzcrank_HUse_E").GetValue<bool>() && _E.IsReady() && QTarget.Distance(Player.ServerPosition) < 230)
                        _E.Cast(Player);
                }
            }
            catch (Exception)
            {
                if (NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 06");
                    ErrorTime = TickCount(10000);
                }
            }
        }
        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            try
            {

            }
            catch (Exception)
            {
                if (NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 07");
                    ErrorTime = TickCount(10000);
                }
            }
        }
        private static void OnProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            try
            {

            }
            catch (Exception)
            {
                if (NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 08");
                    ErrorTime = TickCount(10000);
                }
            }

        }
        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            try
            {
                if (Player.IsDead)
                    return;
                if (!sender.IsEnemy || !sender.IsValid<AIHeroClient>())
                    return;

                if (_MainMenu.Item("Blitzcrank_InterQ").GetValue<bool>() && _Q.IsReady())
                {
                    if (sender.Distance(Player.ServerPosition, true) <= _Q.RangeSqr)
                        _Q.Cast(sender);
                }
                if (_MainMenu.Item("Blitzcrank_InterR").GetValue<bool>() && _R.IsReady())
                {
                    if (sender.Distance(Player.ServerPosition, true) <= _R.RangeSqr)
                        _R.Cast();
                }
                if (_MainMenu.Item("Blitzcrank_InterR").GetValue<bool>() && _E.IsReady())
                {
                    if (sender.Distance(Player.ServerPosition, true) <= _E.RangeSqr)
                        _E.CastOnUnit(Player);
                }
            }
            catch (Exception)
            {
                if (NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 09");
                    ErrorTime = TickCount(10000);
                }
            }

        }
        public static void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            try
            {

            }
            catch (Exception)
            {
                if (NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 10");
                    ErrorTime = TickCount(10000);
                }
            }

        }
        private static void Obj_AI_Base_OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            try
            {

            }
            catch (Exception)
            {
                if (NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 11");
                    ErrorTime = TickCount(10000);
                }
            }

        }
    }
}
