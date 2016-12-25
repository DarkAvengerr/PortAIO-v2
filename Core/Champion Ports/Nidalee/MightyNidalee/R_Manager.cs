using System;
using System.Linq;

using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Data;
using SharpDX;

using EloBuddy; 
using LeagueSharp.Common; 
namespace MightyNidalee
{
    class R_Manager : Mighty
    {
        public static bool CougarForm { get { return ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Name == "Takedown"; } }
        public static int qhumantime, ehumantime, qcougartime, wcougartime, ecougartime, huntedtime;
        public static void EventManager()
        {
            Obj_AI_Base.OnProcessSpellCast += GetCast;
        }
        public static bool IsHunted(Obj_AI_Base target)
        {
            if (target == null) return false;
            return (target.HasBuff("nidaleepassivehunted"));
        }

        public static int GameTimeTickCount
        {
            get { return (int)(Game.Time); }
        }
        public static void GetCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;
            var spell = args.SData;

            switch (spell.Name)
            {
                case "Swipe":
                    ecougartime = GameTimeTickCount;
                    break;
                case "Pounce":
                    wcougartime = GameTimeTickCount;
                    break;
                case "Takedown":
                    qcougartime = GameTimeTickCount;
                    break;
                case "JavelinToss":
                    qhumantime = GameTimeTickCount;
                    break;
                case "PrimalSurge":
                    ehumantime = GameTimeTickCount;
                    break;
            }
        }
        public float HuntedDuration(Obj_AI_Base Target)
        {       
            float DashBuffDuration = Target.GetBuff("nidaleepassivehunted").EndTime - Game.Time;
            return DashBuffDuration;
        }
        public static float Qlefttime()
        {
            if (GameTimeTickCount - qhumantime > 6 * (1 + ObjectManager.Player.FlatCooldownMod))
                return 0;

            else return (6 * (1 + ObjectManager.Player.FlatCooldownMod)) - (GameTimeTickCount - qhumantime);

        }
        public static float Wlefttime()
        {
            if (GameTimeTickCount - wcougartime > 5 * (1 + ObjectManager.Player.FlatCooldownMod))
                return 0;

            else return (5 * (1 + ObjectManager.Player.FlatCooldownMod)) - (GameTimeTickCount - wcougartime);

        }
        public static bool QhumanReady
        {
            get
            {
                return
                    ObjectManager.Player.Mana >= new int[] { 0, 50, 60, 70, 80, 90 }[Q.Level]
                    && GameTimeTickCount - qhumantime >= 6 * (1 + ObjectManager.Player.FlatCooldownMod);
            }
        }
        public static bool EhumanReady
        {
            
            get
            {
                return
                    ObjectManager.Player.Mana >= new int[] { 60, 75, 90, 105, 120 }[E.Level - 1]
                    && GameTimeTickCount - ehumantime >= 12 * (1 + ObjectManager.Player.FlatCooldownMod);
            }
        }
        public static bool QcougarReady
        {
            get
            {
                return GameTimeTickCount - qcougartime >= 5 * (1 + ObjectManager.Player.FlatCooldownMod);
            }
        }
        public static bool EcougarReady
        {
            get
            {
                return GameTimeTickCount - ecougartime >= 5 * (1 + ObjectManager.Player.FlatCooldownMod);
            }
        }
        public static bool WcougarReady
        {
            get
            {
                return (CougarForm && W.IsReady()) ? true : GameTimeTickCount - wcougartime >= 5 * (1 + ObjectManager.Player.FlatCooldownMod);
            }
        }

    }
}
