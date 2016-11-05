using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;
using Font = SharpDX.Direct3D9.Font;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Vi.Common
{

    public static class CommonBuffs
    {
        public static bool HasSheenBuff(this Obj_AI_Base obj)
        {
            return obj.Buffs.Any(buff => buff.Name.ToLower() == "sheen");
        }
        public static bool HasViQKnockBack(this Obj_AI_Base obj)
        {
            return obj.Buffs.Any(buff => buff.DisplayName == "ViQKnockBack");
        }

        public static bool ViHaveFrenziedStrikes
        {
            get { return ObjectManager.Player.Buffs.Any(buff => buff.DisplayName == "ViFrenziedStrikes"); }
        }

        public static bool ViHasEBuff { get { return ObjectManager.Player.Buffs.Any(buff => buff.DisplayName == "ViE"); } }
        public static bool ViHasQBuff { get { return ObjectManager.Player.Buffs.Any(buff => buff.DisplayName == "ViQ"); } }

        public static bool ViHasAttackSpeedBuff
        {
            get
            {
                return ObjectManager.Player.Buffs.Any(buff => buff.DisplayName == "SpectralFury");
            }
        }

        public static bool CanKillableWith(this Obj_AI_Base t, Spell spell)
        {
            return t.Health < spell.GetDamage(t) - 5;
        }

        public static bool CanStun(this Obj_AI_Base t)
        {
            float targetHealth = Champion.PlayerSpells.Q.IsReady() && !t.IsValidTarget(Champion.PlayerSpells.E.Range)
                ? t.Health + Champion.PlayerSpells.Q.GetDamage(t)
                : t.Health;
            return targetHealth / t.MaxHealth * 100 > ObjectManager.Player.Health / ObjectManager.Player.MaxHealth * 100;

            //return t.HealthPercent > ObjectManager.Player.HealthPercent;
        }


        public static bool HasPassive(this Obj_AI_Base obj)
        {
            return obj.PassiveCooldownEndTime - (Game.Time - 15.5) <= 0;
        }

        public static bool HasBuffInst(this Obj_AI_Base obj, string buffName)
        {
            return obj.Buffs.Any(buff => buff.DisplayName == buffName);
        }

        public static bool HasBlueBuff(this Obj_AI_Base obj)
        {
            return obj.Buffs.Any(buff => buff.DisplayName == "CrestoftheAncientGolem");
        }

        public static bool HasRedBuff(this Obj_AI_Base obj)
        {
            return obj.Buffs.Any(buff => buff.DisplayName == "BlessingoftheLizardElder");
        }
    }
}