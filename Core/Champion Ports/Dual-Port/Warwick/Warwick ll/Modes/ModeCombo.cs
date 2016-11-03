using System;
using System.Drawing;
using LeagueSharp;
using LeagueSharp.Common;
using WarwickII.Common;
using WarwickII.Champion;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace WarwickII.Modes
{
    internal static class ModeCombo
    {
        public static Menu MenuLocal { get; private set; }
        private static Spell Q => Champion.PlayerSpells.Q;
        private static Spell W => Champion.PlayerSpells.W;
        private static Spell E => Champion.PlayerSpells.E;
        private static Spell R => Champion.PlayerSpells.R;

        public static void Initialize(Menu MenuParent)
        {

            MenuLocal = new Menu("Combo", "Combo");
            {
                MenuLocal.AddItem(new MenuItem("Mode.R.Active", "R: Active").SetValue(true)).ValueChanged +=
                    (sender, args) =>
                    {
                        foreach (var enemy in HeroManager.Enemies)
                        {
                            MenuLocal.Item("Mode.R" + enemy.ChampionName).Show(args.GetNewValue<bool>());
                        }
                    };

                foreach (var enemy in HeroManager.Enemies)
                {
                    MenuLocal.AddItem(new MenuItem("Mode.R" + enemy.ChampionName, "R: " + enemy.ChampionName).SetValue(new StringList(new []{"Off", "Instantly", "If Killable"}, 2))).SetFontStyle(FontStyle.Regular, R.MenuColor());
                }
            }
            MenuParent.AddSubMenu(MenuLocal);

            Game.OnUpdate += OnUpdate;
            Orbwalking.BeforeAttack += OrbwalkingOnBeforeAttack;
        }

        private static void OrbwalkingOnBeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (W.IsReady() && args.Target.IsEnemy && args.Target.Health > ObjectManager.Player.TotalAttackDamage * 2)
            {
                W.Cast();
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (ModeConfig.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
            {
                return;
            }

            ExecuteCombo();
        }
        
        private static void ExecuteCombo()
        {
            var t = CommonTargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
            if (!t.IsValidTarget())
            {
                return;
            }

            if (Q.CanCast(t))
            {
                if (Q.CanCast(t) && t.Health <= PlayerSpells.WarwickDamage.Q(PlayerSpells.WarwickDamage.QFor.Enemy))
                {
                    Q.CastOnUnit(t);
                }

                if (t.IsValidTarget(CommonHelper.PlayerAutoAttackRange) &&
                    ObjectManager.Player.Health + PlayerSpells.WarwickDamage.Q(PlayerSpells.WarwickDamage.QFor.Health) > ObjectManager.Player.MaxHealth)
                {
                    return;
                }

                Q.CastOnUnit(t);
            }

            if (R.IsReady())
            {
                if (Q.CanCast(t) && t.Health <= PlayerSpells.WarwickDamage.Q(PlayerSpells.WarwickDamage.QFor.Enemy))
                {
                    Q.CastOnUnit(t);
                    return;
                }

                if (t.IsValidTarget(CommonHelper.PlayerAutoAttackRange) && t.Health < ObjectManager.Player.TotalAttackDamage*3 && ObjectManager.Player.HealthPercent > t.HealthPercent)
                {
                    return;
                }

                if (t.HasBuff("bansheeveil")) // don't use R if enemy's banshee is active!
                {
                    return;
                }

                if (t.HasBuff("BlackShield")) // don't use R if enemy have morgana black shild!
                {
                    return;
                }

                if (MenuLocal.Item("Mode.R" + t.ChampionName).GetValue<StringList>().SelectedIndex != 0)
                {
                    switch (MenuLocal.Item("Mode.R" + t.ChampionName).GetValue<StringList>().SelectedIndex)
                    {
                        case 1:
                        {
                            R.CastOnUnit(t);
                            break;
                        }
                        case 2:
                            if (t.Health <= CommonMath.GetComboDamage(t))
                            {
                                R.CastOnUnit(t);
                            }
                            break;
                    }
                }
            }
        }
    }
}
