using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using WarwickII.Common;
using SharpDX;
using WarwickII.Modes;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace WarwickII.Champion
{
    internal static class PlayerSpells
    {
        public static List<Spell> SpellList = new List<Spell>();

        public static Spell Q, W, E, R;

        public static int LastAutoAttackTick;

        public static int LastQCastTick;

        public static int LastECastTick;

        public static int LastSpellCastTick;

        public static SpellSlot IgniteSlot = SpellSlot.Unknown;

        public static SpellSlot SmiteSlot = SpellSlot.Unknown;

        private static readonly int[] SmitePurple = { 3713, 3726, 3725, 3726, 3723 };

        private static readonly int[] SmiteGrey = { 3711, 3722, 3721, 3720, 3719 };

        private static readonly int[] SmiteRed = { 3715, 3718, 3717, 3716, 3714 };

        private static readonly int[] SmiteBlue = { 3706, 3710, 3709, 3708, 3707 };

        public static SpellSlot FlashSlot = SpellSlot.Unknown;

        public static SpellSlot TeleportSlot = ObjectManager.Player.GetSpellSlot("SummonerTeleport");

        public static void Init()
        {
            Q = new Spell(SpellSlot.Q, 400f);
            W = new Spell(SpellSlot.W, 1250f);
            E = new Spell(SpellSlot.E, 1750);
            R = new Spell(SpellSlot.R, 700f);

            SpellList.AddRange(new[] { Q, W, E, R });

            IgniteSlot = ObjectManager.Player.GetSpellSlot("SummonerDot");

            if (Modes.ModeConfig.MenuConfig != null && IgniteSlot != SpellSlot.Unknown && Modes.ModeConfig.MenuConfig != null)
            {
                Modes.ModeConfig.MenuConfig.AddItem(new MenuItem("Spells.Ignite", "Use Ignite!").SetValue(true));
            }

            SetSmiteSlot();
            if (Modes.ModeConfig.MenuConfig != null && SmiteSlot != SpellSlot.Unknown)
            {
                Modes.ModeConfig.MenuConfig.AddItem(new MenuItem("Spells.Smite", "Use Smite to Enemy!").SetValue(true));
            }

            Game.OnUpdate += GameOnOnUpdate;
        }

        private static void SetSmiteSlot()
        {
            foreach (
                var spell in
                    ObjectManager.Player.Spellbook.Spells.Where(
                        spell => string.Equals(spell.Name, Smitetype, StringComparison.CurrentCultureIgnoreCase)))
            {
                SmiteSlot = spell.Slot;
            }
        }

        private static string Smitetype
        {
            get
            {
                if (SmiteBlue.Any(i => LeagueSharp.Common.Items.HasItem(i)))
                {
                    return "s5_summonersmiteplayerganker";
                }

                if (SmiteRed.Any(i => LeagueSharp.Common.Items.HasItem(i)))
                {
                    return "s5_summonersmiteduel";
                }

                if (SmiteGrey.Any(i => LeagueSharp.Common.Items.HasItem(i)))
                {
                    return "s5_summonersmitequick";
                }

                if (SmitePurple.Any(i => LeagueSharp.Common.Items.HasItem(i)))
                {
                    return "itemsmiteaoe";
                }

                return "summonersmite";
            }
        }


        internal class Cast
        {
            public static void Q(Obj_AI_Base oBase)
            {
                if (!PlayerSpells.Q.IsReady())
                    return;

                if (ObjectManager.Player.Health + WarwickDamage.Q(WarwickDamage.QFor.Health) < ObjectManager.Player.MaxHealth || oBase.Health < WarwickDamage.Q(WarwickDamage.QFor.Enemy))
                {
                    PlayerSpells.Q.CastOnUnit(oBase);
                }
            }

            public static void W()
            {
                if (!PlayerSpells.W.IsReady())
                    return;

                PlayerSpells.W.Cast();
            }

            public static void W(Obj_AI_Base oBase)
            {
                if (!PlayerSpells.W.IsReady())
                    return;

                if (ModeConfig.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                {
                    if (oBase.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65))
                        PlayerSpells.W.CastOnUnit(ObjectManager.Player);
                }
            }

            public static void Ignite(AIHeroClient oHero)
            {
                var nIgniteRange = 550f;
                if (IgniteSlot != SpellSlot.Unknown 
                    && ObjectManager.Player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready 
                    && ObjectManager.Player.GetSummonerSpellDamage(oHero, Damage.SummonerSpell.Ignite) < oHero.Health && oHero.IsValidTarget(nIgniteRange))
                {
                    ObjectManager.Player.Spellbook.CastSpell(IgniteSlot, oHero);
                }
            }

            public static void Smite(AIHeroClient oHero)
            {
                var nSmiteRange = 550f;
                var itemCheck = SmiteBlue.Any(i => LeagueSharp.Common.Items.HasItem(i)) || SmiteRed.Any(i => LeagueSharp.Common.Items.HasItem(i));
                if (itemCheck && ObjectManager.Player.Spellbook.CanUseSpell(SmiteSlot) == SpellState.Ready && oHero.IsValidTarget(nSmiteRange))
                {
                    ObjectManager.Player.Spellbook.CastSpell(SmiteSlot, oHero);
                }
            }
        }

        internal class WarwickDamage
        {
            public enum QFor
            {
                Health,
                Enemy
            }

            public static float Q(QFor mode)
            {
                if (!PlayerSpells.Q.IsReady()) return 0;

                float minDamage = new[] { 75f, 125f, 175f, 225f, 275f }[PlayerSpells.Q.Level - 1]  + ObjectManager.Player.TotalMagicalDamage;

                float minHealth = minDamage * 0.8f;

                switch (mode)
                {
                    case QFor.Enemy:
                        return minDamage;
                    case QFor.Health:
                        return minHealth;
                    default:
                        return 0f;
                }
            }

            public static float W
            {
                get
                {
                    if (!PlayerSpells.W.IsReady()) return 0;

                    var baseAttackSpeed = 0.679348;

                    var wCdTime = 6;

                    var passiveDamage = 0; //2.5 + (Player.Level * 0.5);

                    var attackSpeed = (float) Math.Round(Math.Floor(1/ObjectManager.Player.AttackDelay*100)/100, 2, MidpointRounding.ToEven);

                    var aDmg = Math.Round(Math.Floor(ObjectManager.Player.TotalAttackDamage*100)/100, 2, MidpointRounding.ToEven);

                    aDmg = Math.Floor(aDmg);

                    int[] vAttackSpeedPerLevel = {40, 50, 60, 70, 80}; // Nerfed

                    var totalAttackSpeedWithWActive = (float) Math.Round((attackSpeed + baseAttackSpeed/100*vAttackSpeedPerLevel[PlayerSpells.W.Level - 1])*100/100, 2, MidpointRounding.ToEven);

                    var totalPossibleDamage = (float) Math.Round((totalAttackSpeedWithWActive*wCdTime*aDmg)*100/100, 2, MidpointRounding.ToEven);

                    return totalPossibleDamage + (float) passiveDamage;
                }
            }

            public static float R(AIHeroClient t)
            {
                if (!PlayerSpells.R.IsReady())
                {
                    return 0;
                }

                return PlayerSpells.R.GetDamage(t);
            }
        }

        private static void GameOnOnUpdate(EventArgs args)
        {
        }
    }
}
