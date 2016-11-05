using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikksCassiopeia
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ItemData = LeagueSharp.Common.Data.ItemData;

    internal static class Computed
    {
        #region Static Fields

        private static readonly int Archangels = ItemData.Archangels_Staff.Id;

        private static readonly int Archangels2 = ItemData.Archangels_Staff_Crystal_Scar.Id;

        private static readonly int Manamune = ItemData.Manamune.Id;

        private static readonly int Manamune2 = ItemData.Manamune_Crystal_Scar.Id;

        private static readonly int Muraman2 = ItemData.Muramana2.Id;

        private static readonly int Muramana = ItemData.Muramana.Id;

        private static readonly int Serap2 = ItemData.Seraphs_Embrace2.Id;

        private static readonly int Seraph = ItemData.Seraphs_Embrace.Id;

        private static readonly int Tear = ItemData.Tear_of_the_Goddess.Id;

        private static readonly int Tear2 = ItemData.Tear_of_the_Goddess_Crystal_Scar.Id;

        #endregion

        #region Public Properties

        public static float TurretRange => 775;

        #endregion

        #region Public Methods and Operators

        public static double ComboDmg(Obj_AI_Base target)
        {
            var dmg = 0.0;
            if (Spells.Q.IsReady())
            {
                dmg += ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q);
            }

            if (Spells.W.IsReady())
            {
                dmg += ObjectManager.Player.GetSpellDamage(target, SpellSlot.W);
            }

            if (Spells.E.IsReady())
            {
                dmg += Spells.GetEDamage(target);
            }

            if (Spells.R.IsReady())
            {
                dmg += ObjectManager.Player.GetSpellDamage(target, SpellSlot.R);
            }

            dmg += ObjectManager.Player.GetAutoAttackDamage(target);
            return dmg;
        }

        public static bool CompletedTear()
        {
            return Items.HasItem(Muramana, ObjectManager.Player) || Items.HasItem(Seraph, ObjectManager.Player);
        }

        public static bool HasTear()
        {
            return (Items.HasItem(Tear, ObjectManager.Player) || Items.HasItem(Tear2, ObjectManager.Player))
                   || (Items.HasItem(Manamune, ObjectManager.Player) || Items.HasItem(Manamune2, ObjectManager.Player))
                   || Items.HasItem(Archangels, ObjectManager.Player)
                   || Items.HasItem(Archangels2, ObjectManager.Player) || Items.HasItem(Muraman2, ObjectManager.Player)
                   || Items.HasItem(Serap2, ObjectManager.Player);
        }

        public static int RandomDelay(int x)
        {
            var y = x;
            var i = Math.Abs(x);
            while (i >= 10)
            {
                i /= 10;
            }

            i = y / i;
            return Mainframe.RDelay.Next(y - i, y + i);
        }

        #endregion
    }
}