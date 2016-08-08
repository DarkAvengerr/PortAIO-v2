using System;
using System.Linq;
using Jhin___The_Virtuoso.Extensions;
using Jhin___The_Virtuoso.Modes;
using Jhin___The_Virtuoso.Plugins;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Jhin___The_Virtuoso
{
    public class Jhin
    {
        public Jhin()
        {
            JhinOnLoad();
        }
        /// <summary>
        /// Jhin On Load Event
        /// </summary>
        private static void JhinOnLoad()
        {
            Spells.Initialize();
            Menus.Initialize();
            VersionCheck.UpdateCheck();

            Game.OnUpdate += JhinOnUpdate;
            Drawing.OnDraw += JhinOnDraw;
        }

        /// <summary>
        /// Jhin's On Update Event
        /// </summary>
        /// <param name="args">args</param>
        private static void JhinOnUpdate(EventArgs args)
        {
            #region Orbwalker & Modes 
            switch (Menus.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo.ExecuteCombo();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Jungle.ExecuteJungle();
                    Clear.ExecuteClear();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Mixed.ExecuteHarass();
                    break;
                case Orbwalking.OrbwalkingMode.None:
                    None.ImmobileExecute();
                    None.KillSteal();
                    None.TeleportE();
                    Ultimate.ComboUltimate();
                    break;
            }
            #endregion

            #region Check Ultimate
            if (ObjectManager.Player.IsActive(Spells.R))
            {
                Menus.Orbwalker.SetAttack(false);
                Menus.Orbwalker.SetMovement(false);
            }
            else
            {
                Menus.Orbwalker.SetAttack(true);
                Menus.Orbwalker.SetMovement(true);
            }
            #endregion
        }

        private static void JhinOnDraw(EventArgs args)
        {
            if (Menus.Config.Item("q.draw").GetValue<Circle>().Active && Spells.Q.LSIsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells.Q.Range, Menus.Config.Item("q.draw").GetValue<Circle>().Color);
            }
            if (Menus.Config.Item("w.draw").GetValue<Circle>().Active && Spells.W.LSIsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells.W.Range, Menus.Config.Item("w.draw").GetValue<Circle>().Color);
            }
            if (Menus.Config.Item("e.draw").GetValue<Circle>().Active && Spells.E.LSIsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells.E.Range, Menus.Config.Item("e.draw").GetValue<Circle>().Color);
            }
            if (Menus.Config.Item("r.draw").GetValue<Circle>().Active && Spells.R.LSIsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells.R.Range, Menus.Config.Item("r.draw").GetValue<Circle>().Color);
            }
            if (Menus.Config.Item("aa.indicator").GetValue<Circle>().Active)
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(1500) && x.IsValid && x.IsVisible && !x.IsDead && !x.IsZombie))
                {
                    Drawing.DrawText(enemy.HPBarPosition.X, enemy.HPBarPosition.Y, Menus.Config.Item("aa.indicator").GetValue<Circle>().Color, string.Format("{0} Basic Attack = Kill", Provider.BasicAttackIndicator(enemy)));
                }
            }
            if (Menus.Config.Item("sniper.text").GetValue<Circle>().Active && ObjectManager.Player.IsActive(Spells.R))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x=> x.LSIsValidTarget(Spells.R.Range)))
                {
                    var damage = Spells.R.GetDamage(enemy)*4;
                    if (enemy.Health <= damage)
                    {
                        Render.Circle.DrawCircle(enemy.Position, 100, Menus.Config.Item("sniper.text").GetValue<Circle>().Color,10);

                    }
                }
            }
        }
    }
}
