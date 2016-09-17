using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using SharpDX;
using Color = System.Drawing.Color;
using ItemData = LeagueSharp.Common.Data.ItemData;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoRengar
{
    public static class Helper
    {
        private static AIHeroClient Player { get{ return ObjectManager.Player; } }
        public static void QbeforeAttack(AttackableUnit target)
        {
            if (!Variables.Q.IsReady())
                return;
            if (Player.AttackRange > 500)
                return;
            if (target == null)
                return;
            if (!(target is Obj_AI_Base))
                return;
            if ((Player.AttackCastDelay * 1000 - 20) * (target as Obj_AI_Base).MoveSpeed + Player.Distance(target.Position) 
                < Orbwalking.GetRealAutoAttackRange(target))
                return;
            Variables.Q.Cast();
        }

        public static void CastE (Obj_AI_Base target)
        {
            if (!Player.IsDashing())
                Variables.E.Cast(target);
            if (Player.IsDashing())
            {
                var pos = Prediction.GetPrediction(Player, 0.25f).UnitPosition;
                Variables.E2.SetSkillshot(0.25f, 70, 1500, true, SkillshotType.SkillshotLine, pos, pos);
                Variables.E2.Cast(target);
            }
        }
        public static bool HasItem()
        {
            if (ItemData.Tiamat_Melee_Only.GetItem().IsReady() || ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsReady()
                || ItemData.Titanic_Hydra_Melee_Only.GetItem().IsReady())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static void CastItem()
        {

            if (ItemData.Tiamat_Melee_Only.GetItem().IsReady())
                ItemData.Tiamat_Melee_Only.GetItem().Cast();
            if (ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsReady())
                ItemData.Ravenous_Hydra_Melee_Only.GetItem().Cast();
            if (ItemData.Titanic_Hydra_Melee_Only.GetItem().IsReady())
                ItemData.Titanic_Hydra_Melee_Only.GetItem().Cast();
        }

        public static bool HasSmite { get { return Variables.Smite != SpellSlot.Unknown && Variables.Smite.IsReady(); } }
        private static List<int> listsmitedamge = new List<int> { 390, 410, 430, 450, 480, 510, 540, 570, 600, 640, 680, 720, 760, 800, 850, 900, 950, 1000 };
        public static bool hasSmiteRed
        {
            get
            {
                return HasSmite && (new string[] { "s5_summonersmiteduel" }).Contains(Player.GetSpell(Variables.Smite).Name.ToLower());
            }
        }
        public static bool hasSmiteBlue
        {
            get
            {
                return HasSmite && (new string[] { "s5_summonersmiteplayerganker" }).Contains(Player.GetSpell(Variables.Smite).Name.ToLower());
            }
        }
        public static int SmiteRedDamage { get { return 0; } }
        public static int SmiteBlueDamage { get { return 20 + 8 * Player.Level; } }
        public static int SmiteDamage { get { return listsmitedamge[Player.Level - 1]; } }
    }
}
