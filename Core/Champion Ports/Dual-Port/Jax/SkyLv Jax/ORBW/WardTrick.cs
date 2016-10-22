using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_Jax
{
    using System;
    using System.Linq;
    using SharpDX;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class WardTrick
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

        static WardTrick()
        {
            //Menu
            SkyLv_Jax.Menu.SubMenu("Flee").AddSubMenu(new Menu("Ward Trick", "Ward Trick"));
            SkyLv_Jax.Menu.SubMenu("Flee").SubMenu("Ward Trick").AddItem(new MenuItem("Jax.MoveWhenWardTrick", "Move To Mouse Position In WardTrick Mode").SetValue(true));
            SkyLv_Jax.Menu.SubMenu("Flee").SubMenu("Ward Trick").AddItem(new MenuItem("Jax.WardTrick", "Ward Trick !").SetValue(new KeyBind("Z".ToCharArray()[0], KeyBindType.Press)));

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (SkyLv_Jax.Menu.Item("Jax.WardTrick").GetValue<KeyBind>().Active)
            {
                if (SkyLv_Jax.Menu.Item("Jax.MoveWhenWardTrick").GetValue<bool>())
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                    WardTrickLogic();
                }

                if (!SkyLv_Jax.Menu.Item("Jax.MoveWhenWardTrick").GetValue<bool>())
                {
                    WardTrickLogic();
                }
            }
        }

        public static void WardTrickLogic()
        {
            if (Q.IsReady() && Player.Mana >= Q.ManaCost)
            {
                SkyLv_Jax.WardPos = Game.CursorPos;
                Obj_AI_Minion WardObject;
                if (Game.CursorPos.Distance(Player.Position) <= 700)
                {
                    WardObject = ObjectManager.Get<Obj_AI_Minion>().Where(Ward => Ward.Distance(Game.CursorPos) < 200 && !Ward.IsDead && Ward.Name.ToLower().Contains("ward")).FirstOrDefault();
                }
                else
                {
                    Vector3 DWpos = Game.CursorPos - Player.ServerPosition;
                    DWpos.Normalize();
                    SkyLv_Jax.WardPos = Player.ServerPosition + DWpos * (600);
                    WardObject = ObjectManager.Get<Obj_AI_Minion>().Where(Ward => Ward.Distance(SkyLv_Jax.WardPos) < 200 && !Ward.IsDead && Ward.Name.ToLower().Contains("ward")).FirstOrDefault();
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
                }
            }
        }
    }
}   
