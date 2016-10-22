using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace DicasteAshe.Handlers
{
    using System;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.Utils;

    using static MenuHandler;

    using static SpellHandler;

    internal static class ModeHandler
    {
        private static string AsheQBuffName { get; } = "asheqcastready";

        private static AIHeroClient Player { get; } = ObjectManager.Player;

        internal static void Init()
        {
            Game.OnUpdate += OnUpdate;
        }

        private static void CastActivatedSpell(
            Spell spell,
            bool condition = true,
            bool ignoreShields = true,
            bool killsteal = false)
        {
            if (!spell.IsReady())
            {
                return;
            }

            var target = spell.GetTarget(accountForCollision: spell.Collision);

            if (!target.IsValidTarget(spell.Range) || Invulnerable.Check(target, spell.DamageType, ignoreShields)
                || !condition)
            {
                return;
            }

            if (!killsteal)
            {
                spell.Cast();
            }
            else
            {
                if (spell.GetDamage(target) > target.Health)
                {
                    spell.Cast();
                }
            }
        }

        private static void CastSkillShot(
            Spell spell,
            bool condition = true,
            bool ignoreShields = true,
            bool killsteal = false)
        {
            if (!spell.IsReady())
            {
                return;
            }

            var target = spell.GetTarget(accountForCollision: spell.Collision);

            if (!target.IsValidTarget(spell.Range) || Invulnerable.Check(target, spell.DamageType, ignoreShields)
                || !condition)
            {
                return;
            }

            var prediction = spell.GetPrediction(target);

            if (prediction.Hitchance.Equals(HitChance.Impossible))
            {
                return;
            }

            if (killsteal)
            {
                if (spell.GetDamage(target) > target.Health)
                {
                    spell.Cast(prediction.UnitPosition);
                }
            }
            else
            {
                spell.Cast(prediction.UnitPosition);
            }
        }

        private static void Combo()
        {
            if (GetMenuBool(OrbwalkingMode.Combo, SpellSlot.Q))
            {
                CastActivatedSpell(Q, Player.HasBuff(AsheQBuffName));
            }

            if (GetMenuBool(OrbwalkingMode.Combo, SpellSlot.W))
            {
                CastSkillShot(W);
            }

            if (GetMenuBool(OrbwalkingMode.Combo, SpellSlot.R))
            {
                CastSkillShot(R, ignoreShields: false, killsteal: true);
            }
        }

        private static void Hybrid()
        {
            if (GetMenuBool(OrbwalkingMode.Combo, SpellSlot.Q))
            {
                CastActivatedSpell(Q, Player.HasBuff(AsheQBuffName));
            }

            if (GetMenuBool(OrbwalkingMode.Combo, SpellSlot.W))
            {
                CastSkillShot(W);
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            switch (Variables.Orbwalker.ActiveMode)
            {
                case OrbwalkingMode.Combo:
                    Combo();
                    break;
                case OrbwalkingMode.Hybrid:
                    Hybrid();
                    break;
            }
        }
    }
}