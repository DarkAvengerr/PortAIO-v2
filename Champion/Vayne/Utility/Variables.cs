using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using VayneHunter_Reborn.External.Translation.Languages;
using VayneHunter_Reborn.Modules;
using VayneHunter_Reborn.Modules.ModuleList.Condemn;
using VayneHunter_Reborn.Modules.ModuleList.Misc;
using VayneHunter_Reborn.Modules.ModuleList.Tumble;
using VayneHunter_Reborn.Utility.Helpers;
using EloBuddy;

namespace VayneHunter_Reborn.Utility
{
    class Variables
    {
        private const float Range = 1200f;

        public static Menu Menu { get; set; }

        public static float LastCondemnFlashTime { get; set; }

        public static Orbwalking.Orbwalker Orbwalker { get; set; }

        public static Dictionary<SpellSlot, Spell> spells = new Dictionary<SpellSlot, Spell>
        {
            { SpellSlot.Q, new Spell(SpellSlot.Q) },
            { SpellSlot.W, new Spell(SpellSlot.W) },
            { SpellSlot.E, new Spell(SpellSlot.E, 650f) {Width = 1f} },
            { SpellSlot.R, new Spell(SpellSlot.R) }
        };

        public static List<IModule> moduleList = new List<IModule>()
        {
            new AutoE(),
            new EKS(),
            new LowLifePeel(),
            new NoAAStealth(),
            new QKS(),
            new AutoQR(),
            new WallTumble(),
            new Focus2WStacks(),
            new Reveal(),
            new DisableMovement(),
            new CondemnJungleMobs(),
            new FlashRepel(),
            new FlashCondemn(),
        };

        public static List<IVHRLanguage> languageList = new List<IVHRLanguage>()
        {
            new English(),
            new Chinese(),
            new French(),
            new German(),
            new Portuguese(),
            new Italian()
        };

        public static IEnumerable<AIHeroClient> MeleeEnemiesTowardsMe
        {
            get
            {
                return
                    HeroManager.Enemies.FindAll(
                        m => m.IsMelee() && m.LSDistance(ObjectManager.Player) <= PlayerHelper.GetRealAutoAttackRange(m, ObjectManager.Player)
                            && (m.ServerPosition.LSTo2D() + (m.BoundingRadius + 25f) * m.Direction.LSTo2D().LSPerpendicular()).LSDistance(ObjectManager.Player.ServerPosition.LSTo2D()) <= m.ServerPosition.LSDistance(ObjectManager.Player.ServerPosition)
                            && m.LSIsValidTarget(Range, false));
            }
        }

        public static IEnumerable<AIHeroClient> EnemiesClose
        {
            get
            {
                return
                    HeroManager.Enemies.Where(
                        m =>
                            m.LSDistance(ObjectManager.Player, true) <= Math.Pow(1000, 2) && m.LSIsValidTarget(1500, false) &&
                            m.LSCountEnemiesInRange(m.IsMelee() ? m.AttackRange * 1.5f : m.AttackRange + 20 * 1.5f) > 0);
            }
        }

    }
}
