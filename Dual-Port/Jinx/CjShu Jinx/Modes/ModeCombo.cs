using CjShuJinx.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace CjShuJinx.Modes
{
    using System;
    using System.Drawing;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using Color = SharpDX.Color;

    internal static class ModeCombo
    {
        public static Menu MenuLocal { get; private set; }
        private static Spell Q => Champion.PlayerSpells.Q;
        private static Spell W => Champion.PlayerSpells.W;
        private static Spell E => Champion.PlayerSpells.E;
        private static Spell R => Champion.PlayerSpells.R;

        public static void Init()
        {
            MenuLocal = new Menu("Combo", "Combo").SetFontStyle(FontStyle.Regular, Color.Aqua);
            MenuLocal.AddItem(new MenuItem("Combo.Q", "Q:").SetValue(new StringList(new []{ "Off", "On"}, 1)).SetFontStyle(FontStyle.Regular, Q.MenuColor())).SetTooltip("Jinx's W / Youmuu", Color.AliceBlue);
            MenuLocal.AddItem(new MenuItem("Combo.W", "W:").SetValue(new StringList(new[] { "Off", "On" }, 1)).SetFontStyle(FontStyle.Regular, W.MenuColor()));
            MenuLocal.AddItem(new MenuItem("Combo.E", "E:").SetValue(new StringList(new[] { "Off", "On: Mode [Marksman]", "On: Mode [II]" }, 1)).SetFontStyle(FontStyle.Regular, E.MenuColor()));
            MenuLocal.AddItem(new MenuItem("Combo.R", "R:").SetValue(new StringList(new[] { "Off", "On" }, 1)).SetFontStyle(FontStyle.Regular, E.MenuColor()));

            ModeConfig.MenuConfig.AddSubMenu(MenuLocal);

            Game.OnUpdate += OnUpdate;
            Orbwalking.BeforeAttack += OrbwalkingOnBeforeAttack;
            // Drawing.OnDraw += DrawingOnOnDraw;

            Obj_AI_Base.OnBuffGain += (sender, args) =>
            {
                if (!E.IsReady())
                {
                    return;
                }

                BuffInstance aBuff =
                    (from fBuffs in
                        sender.Buffs.Where(
                            s =>
                                sender.Team != ObjectManager.Player.Team
                                && sender.Distance(ObjectManager.Player.Position) < E.Range)
                        from b in new[]
                        {
                            "teleport_", /* Teleport */
                            "pantheon_grandskyfall_jump", /* Pantheon */ 
                            "crowstorm", /* FiddleScitck */
                            "zhonya", "katarinar", /* Katarita */
                            "MissFortuneBulletTime", /* MissFortune */
                            "gate", /* Twisted Fate */
                            "chronorevive" /* Zilean */
                        }
                        where args.Buff.Name.ToLower().Contains(b)
                        select fBuffs).FirstOrDefault();

                if (aBuff != null)
                {
                    E.Cast(sender.Position);
                }

            };
        }

        private static void DrawingOnOnDraw(EventArgs args)
        {
            Render.Circle.DrawCircle(ObjectManager.Player.Position, CommonBuffs.MegaQRange, System.Drawing.Color.GreenYellow);
            
            var t = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);

            if (t.IsValidTarget(W.Range * 2 ))
            {
                Render.Circle.DrawCircle(t.Position, t.BoundingRadius * 2, System.Drawing.Color.GreenYellow);
            }
        }

        static void OrbwalkingOnBeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {

            if (!(args.Target is AIHeroClient))
            {
                return;
            }

            var t = (AIHeroClient) args.Target;

            if (t.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 95) && CommonBuffs.MegaQActive && Q.GetDamage(t) < t.Health)
            {
                Q.Cast();
            }
        }

        static void OnUpdate(EventArgs args)
        {
            
            // foreach (var b in ObjectManager.Player.Buffs)
            // {
            //    Console.WriteLine(b.DisplayName + " : " + b.Count);
            // }
            // Console.WriteLine("-------------------------------------------------");

            if (ModeConfig.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
            {
                return;
            }

            ExecuteCombo();
        }

        static void ExecuteQCombo()
        {
            if (MenuLocal.Item("Combo.Q").GetValue<StringList>().SelectedIndex == 0)
            {
                return;
            }

            var t = TargetSelector.GetTarget(CommonBuffs.MegaQRange + 400, TargetSelector.DamageType.Physical);
            if (!t.IsValidTarget())
            {
                return;
            }

            if (t.IsValidTarget() && !CommonBuffs.MegaQActive && !t.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 95))
            {
                Q.Cast();
                return;
            }
        }

        private static float GetSlowEndTime(Obj_AI_Base target)
        {
            return
                target.Buffs.OrderByDescending(buff => buff.EndTime - Game.Time)
                    .Where(buff => buff.Type == BuffType.Slow)
                    .Select(buff => buff.EndTime)
                    .FirstOrDefault();
        }

        private static bool IsEmpairedLight(AIHeroClient enemy)
        {
            return (enemy.HasBuffOfType(BuffType.Slow));
        }
        private static bool IsEmpaired(AIHeroClient enemy)
        {
            return (enemy.HasBuffOfType(BuffType.Stun) || enemy.HasBuffOfType(BuffType.Snare) ||
                    enemy.HasBuffOfType(BuffType.Charm) || enemy.HasBuffOfType(BuffType.Fear) ||
                    enemy.HasBuffOfType(BuffType.Taunt) || IsEmpairedLight(enemy));
        }

        private static bool IsMoving(Obj_AI_Base obj)
        {
            return obj.Path.Count() > 1;
        }

        private static float GetPerValue(bool mana)
        {
            return mana ? ObjectManager.Player.ManaPercent : ObjectManager.Player.HealthPercent;
        }


        private static void ECast_DZ()
        {
            if (!E.IsReady())
            {
                return;
            }

            foreach (
                var enemy in
                    ObjectManager.Get<AIHeroClient>().Where(h => h.IsValidTarget(E.Range - 140f) && (IsEmpaired(h))))
            {

                if (IsEmpairedLight(enemy) && IsMoving(enemy))
                {
                        E.CastIfHitchanceEquals(enemy, HitChance.High);
                        return;
                }
                E.CastIfHitchanceEquals(enemy, HitChance.VeryHigh);
            }
        }

        private static void ECast()
        {
            foreach (
                var enemy in
                    ObjectManager.Get<AIHeroClient>().Where(h => h.IsValidTarget(E.Range - 150)))
            {
                if (!E.IsReady() || !enemy.HasBuffOfType(BuffType.Slow))
                {
                    return;
                }

                var castPosition =
                    Prediction.GetPrediction(
                        new PredictionInput
                        {
                            Unit = enemy,
                            Delay = 0.7f,
                            Radius = 120f,
                            Speed = 1750f,
                            Range = 900f,
                            Type = SkillshotType.SkillshotCircle
                        }).CastPosition;
                if (GetSlowEndTime(enemy) >= (Game.Time + E.Delay + 0.5f))
                {
                    E.Cast(castPosition);
                }

                if (enemy.HasBuffOfType(BuffType.Stun) || enemy.HasBuffOfType(BuffType.Snare) ||
                     enemy.HasBuffOfType(BuffType.Charm) || enemy.HasBuffOfType(BuffType.Fear) ||
                     enemy.HasBuffOfType(BuffType.Taunt))
                {
                    E.CastIfHitchanceEquals(enemy, HitChance.High);
                }
            }
        }

        static void ExecuteCombo()
        {
            ExecuteQCombo();

            if (MenuLocal.Item("Combo.E").GetValue<StringList>().SelectedIndex == 0)
            {
                ECast();
            }

            if (MenuLocal.Item("Combo.E").GetValue<StringList>().SelectedIndex == 1)
            {
                ECast_DZ();
            }

            var t = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
            if (!t.IsValidTarget())
            {
                return;
            }
                
            if (MenuLocal.Item("Combo.W").GetValue<StringList>().SelectedIndex == 1 && W.IsReady())
            {
                HitChance[] hithChances = new[] {HitChance.Medium, HitChance.High, HitChance.VeryHigh};

                if (W.CanCast(t) && (W.GetDamage(t) > t.Health || !t.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 60)))
                {
                    W.CastIfHitchanceEquals(t, hithChances[Modes.ModeSettings.MenuSpellW.Item("Set.W.Hitchance").GetValue<StringList>().SelectedIndex]);
                }
            }
        }
    }
}
