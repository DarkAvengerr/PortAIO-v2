using EloBuddy; 
using LeagueSharp.Common; 
 namespace Flowers_Yasuo
{
    using Evade;
    using System.Linq;
    using System.Collections.Generic;
    using Manager.Menu;
    using Manager.Events;
    using Manager.Spells;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using Common;

    internal class Logic
    {
        internal static Spell Q;
        internal static Spell Q3;
        internal static Spell W;
        internal static Spell E;
        internal static Spell R;
        internal static SpellSlot Ignite = SpellSlot.Unknown;
        internal static SpellSlot Flash = SpellSlot.Unknown;
        internal static Menu Menu;
        internal static bool isDashing;
        internal static int SkinID;
        internal static int lastCheckTime;
        internal static float lastWardCast;
        internal static int lastECast;
        internal static int lastHarassTime;
        internal static Vector3 lastEPos;       
        internal static AIHeroClient Me;
        internal static Orbwalking.Orbwalker Orbwalker;
        internal static readonly List<ChampionObject> championObject = new List<ChampionObject>();

        internal static void LoadYasuo()
        {
            Me = ObjectManager.Player;
            SkinID = ObjectManager.Player.SkinId;

            SpellManager.Init();
            MenuManager.Init();
            EvadeManager.Init();
            EvadeTargetManager.Init();
            EventManager.Init();
            Manager.Events.Games.Mode.WallJump.InitPos();
        }

        internal static bool IsDashing => isDashing || Me.IsDashing();

        internal static void EnbaleSkin(object obj, OnValueChangeEventArgs Args)
        {
            if (!Args.GetNewValue<bool>())
            {
                //ObjectManager.//Player.SetSkin(ObjectManager.Player.ChampionName, SkinID);
            }
        }

        public static bool CanCastDelayR(AIHeroClient target)
        {
            //copy from valvesharp
            var buff = target.Buffs.FirstOrDefault(i => i.Type == BuffType.Knockback || i.Type == BuffType.Knockup);

            return buff != null &&
                   buff.EndTime - Game.Time <=
                   (buff.EndTime - buff.StartTime) / (buff.EndTime - buff.StartTime <= 0.5 ? 1.5 : 3);
        }

        public static bool UnderTower(Vector3 pos)
        {
            return ObjectManager.Get<Obj_AI_Turret>().Any(turret => turret.Health > 1 && turret.IsValidTarget(950, true, pos));
        }

        public static Vector3 PosAfterE(Obj_AI_Base target)
        {
            return ObjectManager.Player.IsFacing(target)
                ? ObjectManager.Player.ServerPosition.Extend(target.ServerPosition, 475f)
                : ObjectManager.Player.ServerPosition.Extend(Prediction.GetPrediction(target, 350f).UnitPosition, 475f);
        }
    }
}
