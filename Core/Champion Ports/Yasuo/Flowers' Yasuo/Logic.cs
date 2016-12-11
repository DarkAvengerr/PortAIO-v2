using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Yasuo
{
    using Evade;
    using System;
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
        internal static readonly List<Vector2> WallJumpPos = new List<Vector2>();

        internal static void LoadYasuo()
        {
            Me = ObjectManager.Player;
            SkinID = ObjectManager.Player.SkinId;

            InitWallPos();

            SpellManager.Init();
            MenuManager.Init();
            EvadeManager.Init();
            EvadeTargetManager.Init();
            EventManager.Init();
        }

        private static void InitWallPos()
        {
            WallJumpPos.Add(new Vector2(7274, 5908));
            WallJumpPos.Add(new Vector2(8222, 3158));
            WallJumpPos.Add(new Vector2(7784, 9494));
            WallJumpPos.Add(new Vector2(6574, 12256));
            WallJumpPos.Add(new Vector2(10882, 8416));
            WallJumpPos.Add(new Vector2(11072, 8306));
            WallJumpPos.Add(new Vector2(12582, 6402));
            WallJumpPos.Add(new Vector2(3892, 6466));
            WallJumpPos.Add(new Vector2(8322, 2658));
            WallJumpPos.Add(new Vector2(7046, 5426));
            WallJumpPos.Add(new Vector2(2232, 8412));
            WallJumpPos.Add(new Vector2(7672, 8906));
            WallJumpPos.Add(new Vector2(4324, 6258));
            WallJumpPos.Add(new Vector2(3674, 7058));
            WallJumpPos.Add(new Vector2(8372, 9606));
            WallJumpPos.Add(new Vector2(6650, 11766));
            WallJumpPos.Add(new Vector2(1678, 8428));
            WallJumpPos.Add(new Vector2(6424, 5208));
            WallJumpPos.Add(new Vector2(13172, 6508));
            WallJumpPos.Add(new Vector2(11222, 7856));
            WallJumpPos.Add(new Vector2(10372, 8456));
        }

        internal static bool IsDashing => isDashing || Me.IsDashing();

        internal static void EnbaleSkin(object obj, OnValueChangeEventArgs Args)
        {
            if (!Args.GetNewValue<bool>())
            {
                //ObjectManager.//Player.SetSkin(ObjectManager.Player.ChampionName, SkinID);
            }
        }

        protected static bool CanCastDelayR(AIHeroClient target)
        {
            //copy from valvesharp
            var buff = target.Buffs.FirstOrDefault(i => i.Type == BuffType.Knockup);

            return target.HasBuffOfType(BuffType.Knockback) ||
                   (buff != null && (Game.Time - buff.StartTime >= 0.89*(buff.EndTime - buff.StartTime)));
        }

        public static bool UnderTower(Vector3 pos)
        {
            return ObjectManager.Get<Obj_AI_Turret>().Any(turret => turret.Health > 1 && turret.IsValidTarget(950, true, pos));
        }

        public static Vector3 PosAfterE(Obj_AI_Base target)
        {
            if (target.IsValidTarget())
            {
                return ObjectManager.Player.IsFacing(target)
                    ? ObjectManager.Player.ServerPosition.Extend(target.ServerPosition, 475f)
                    : ObjectManager.Player.ServerPosition.Extend(Prediction.GetPrediction(target, 350f).UnitPosition,
                        475f);
            }

            return Vector3.Zero;
        }

        protected static List<Vector3> FlashPoints()
        {
            var points = new List<Vector3>();

            for (var i = 1; i <= 360; i++)
            {
                var angle = i * 2 * Math.PI / 360;
                var point = new Vector2(Me.Position.X + 425f * (float)Math.Cos(angle),
                    Me.Position.Y + 425f * (float)Math.Sin(angle)).To3D();

                points.Add(point);
            }

            return points;
        }
    }
}
