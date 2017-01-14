using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Lord_s_Vayne.Events
{
    class GameUpdate
    {
        public static AIHeroClient Player { get { return ObjectManager.Player; } }
        public static void Game_OnGameUpdate(EventArgs args)
        {
            if (Program.menu.Item("useR").GetValue<bool>() && Program.R.IsReady()
                && ObjectManager.Player.CountEnemiesInRange(1000) >= Program.menu.Item("enemys").GetValue<Slider>().Value
                && Program.orbwalker.ActiveMode.ToString() == "Combo")
            {
                Program.R.Cast();
            }

            if (Program.menu.Item("aaqaa").GetValue<KeyBind>().Active)
            {
                Orbwalking.Orbwalk(TargetSelector.GetTarget(625, TargetSelector.DamageType.Physical), Game.CursorPos);
            }
            
            if (Program.emenu.Item("UseCF").GetValue<KeyBind>().Active && Program.E.IsReady())
            {
                Misc.CondemnFlash.CondemnFlashs();
            }
            if (Program.emenu.Item("UseCFA").GetValue<KeyBind>().Active && Program.E.IsReady() && Player.HealthPercent < Program.emenu.Item("UseCFHP").GetValue<Slider>().Value)
            {
                Misc.CondemnFlash.CondemnFlashs();
            }
            //||
            //(orbwalker.ActiveMode.ToString() != "Combo" || !menu.Item("UseEC").GetValue<bool>()) &&
            //!menu.Item("UseET").GetValue<KeyBind>().Active)) return;
            if ((Program.orbwalker.ActiveMode.ToString() == "Combo" && Program.emenu.Item("UseEC").GetValue<bool>()) || (Program.orbwalker.ActiveMode.ToString() == "Mixed" && Program.emenu.Item("he").GetValue<bool>()) || Program.emenu.Item("UseET").GetValue<KeyBind>().Active)
            {
                switch (Program.emenu.Item("EMode", true).GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        {
                            Condemn.Lords.Run();
                        }
                        break;
                    case 1:
                        {
                            Condemn.Gosu.Run();
                        }
                        break;
                    case 2:
                        {
                            Condemn.Flowers.Run();
                        }
                        break;
                    case 3:
                        {
                            Condemn.VHR.Run();
                        }
                        break;

                    case 4:
                        {
                            Condemn.Marksman.Run();
                        }
                        break;
                    case 5:
                        {
                            Condemn.Sharpshooter.Run();
                        }
                        break;
                    case 6:
                        {
                            Condemn.OKTW.Run();
                        }
                        break;
                    case 7:
                        {
                            Condemn.Shine.Run();
                        }
                        break;
                    case 8:
                        {
                            Condemn.PRADASMART.Run();
                        }
                        break;

                    case 9:
                        {
                            Condemn.PRADAPERFECT.Run();
                        }
                        break;
                    case 10:
                        {
                            Condemn.OLDPRADA.Run();

                        }
                        break;
                    case 11:
                        {
                            Condemn.PRADALEAGACY.Run();
                        }
                        break;
                    case 12:
                        {
                            //Condemn.Lords2.Run();
                        }
                        break;
                }
            }
        }
    }
}
