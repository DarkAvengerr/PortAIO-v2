using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_Jax
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;

    internal class Flee
    {

        #region #GET
        private static AIHeroClient Player
        {
            get
            {
                return SkyLv_Jax.Player;
            }
        }

        private static Spell Q
        {
            get
            {
                return SkyLv_Jax.Q;
            }
        }
        private static Spell W
        {
            get
            {
                return SkyLv_Jax.W;
            }
        }

        private static Spell E
        {
            get
            {
                return SkyLv_Jax.E;
            }
        }
        #endregion



        static Flee()
        {
            //Menu
            SkyLv_Jax.Menu.SubMenu("Flee").AddItem(new MenuItem("Jax.UseWardTrickFlee", "Use Ward Trick In Flee Mode").SetValue(true));
            SkyLv_Jax.Menu.SubMenu("Flee").AddItem(new MenuItem("Jax.MoveWhenFlee", "Move To Mouse Position In Flee Mode").SetValue(true));
            SkyLv_Jax.Menu.SubMenu("Flee").AddItem(new MenuItem("Jax.MinimumTargetGapCloseRange", "Minimum Range To Gapclose On Flee Target Instead Of WardTrick").SetValue(new Slider((int)E.Range, 0, (int)Q.Range)));
            SkyLv_Jax.Menu.SubMenu("Flee").AddItem(new MenuItem("Jax.MaximumTargetGapCloseRange", "Maximum Range To Gapclose On Flee Target Instead Of WardTrick").SetValue(new Slider((int)Q.Range, 0, (int)Q.Range + (int)E.Range)));
            SkyLv_Jax.Menu.SubMenu("Flee").AddItem(new MenuItem("Jax.FleeActive", "Flee !").SetValue(new KeyBind("U".ToCharArray()[0], KeyBindType.Press)));

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (SkyLv_Jax.Menu.Item("Jax.FleeActive").GetValue<KeyBind>().Active)
            {
                if (SkyLv_Jax.Menu.Item("Jax.MoveWhenFlee").GetValue<bool>())
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                    FleeLogic();
                }

                if (!SkyLv_Jax.Menu.Item("Jax.MoveWhenFlee").GetValue<bool>())
                {
                    FleeLogic();
                }
            }
        }

        public static void FleeLogic()
        {
            FleeMode(Game.CursorPos.To2D(), SkyLv_Jax.Menu.Item("Jax.MinimumTargetGapCloseRange").GetValue<Slider>().Value, SkyLv_Jax.Menu.Item("Jax.MaximumTargetGapCloseRange").GetValue<Slider>().Value, SkyLv_Jax.Menu.Item("Jax.UseWardTrickFlee").GetValue<bool>());
        }



        public static void FleeMode(Vector2 Position, float MinimumRange, float MaximumRange, bool WardTrick = false, List<AIHeroClient> ignore = null)
        {
            if (!Q.IsReady() || Player.Mana < Q.ManaCost)
            {
                return;
            }

            Vector2 PlayerPosition = Player.ServerPosition.To2D();
            Obj_AI_Base BestDashingTarget = null;

            Vector2 BestLocaltion = PlayerPosition + (Vector2.Normalize(Position - PlayerPosition) * (Player.MoveSpeed * 0.35f));
            float BestDistance = Position.Distance(PlayerPosition) - 50;

            foreach (Obj_AI_Base PotancialTarget in ObjectManager.Get<Obj_AI_Base>().Where(target => !target.IsDead && target.IsTargetable && target.IsValidTarget()))
            {
                float RealQRange = MaximumRange + PotancialTarget.BoundingRadius;
                float DistanceToEnemy = Player.Distance(PotancialTarget);

                //Declaring Minimum And Maximum Range =>
                if (DistanceToEnemy > MinimumRange && DistanceToEnemy < RealQRange)
                {
                    Vector2 PositionAfterQ = PotancialTarget.ServerPosition.To2D();
                    float DistanceQ = Position.Distance(PositionAfterQ);

                    if (BestDistance > DistanceQ)
                    {
                        BestDashingTarget = PotancialTarget;
                        BestDistance = DistanceQ;
                        BestLocaltion = PositionAfterQ;
                    }
                }
            }

            if (BestDashingTarget != null)
            {
                Q.CastOnUnit(BestDashingTarget);
                return;
            }

            if (BestDashingTarget == null)
            {
                if (WardTrick)
                {
                    SkyLv_Jax.WardPos = Game.CursorPos;
                    Obj_AI_Minion WardObject;
                    if (Game.CursorPos.Distance(Player.Position) <= 700)
                    {
                        WardObject = ObjectManager.Get<Obj_AI_Minion>().Where(Ward => Ward.Distance(Game.CursorPos) < 200 && !Ward.IsDead).MinOrDefault(i => i.Distance(Game.CursorPos));
                    }
                    else
                    {
                        Vector3 DWpos = Game.CursorPos - Player.ServerPosition;
                        DWpos.Normalize();
                        SkyLv_Jax.WardPos = Player.ServerPosition + DWpos * (600);
                        WardObject = ObjectManager.Get<Obj_AI_Minion>().Where(Ward => Ward.Distance(SkyLv_Jax.WardPos) < 200 && !Ward.IsDead).MinOrDefault(i => i.Distance(Game.CursorPos));
                    }

                    if (WardObject == null)
                    {
                        if (!SkyLv_Jax.WardPos.IsWall())
                        {
                            InventorySlot WardSlot = Items.GetWardSlot();
                            Items.UseItem((int)WardSlot.Id, SkyLv_Jax.WardPos);
                        }
                    }

                    else if (WardObject != null)
                    {
                        Q.CastOnUnit(WardObject);
                        return;
                    }
                }
            }
        }
    }
}
