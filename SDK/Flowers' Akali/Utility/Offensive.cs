using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Flowers_Akali.Utility
{
    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;
    using System;
    using System.Linq;

    internal class Offensive
    {
        private static AIHeroClient Me => Program.Me;
        private static Menu Menu => Tools.Menu;

        internal static void Inject()
        {
            var OffensiveMenu = Menu.Add(new Menu("Offensive", "Offensive"));
            {
                OffensiveMenu.Add(new MenuBool("Youmuus", "Use Youmuu's Ghostblade", true));
                OffensiveMenu.Add(new MenuBool("Cutlass", "Use Bilgewater Cutlass", true));
                OffensiveMenu.Add(new MenuBool("Botrk", "Use Blade of the Ruined King", true));
                OffensiveMenu.Add(new MenuBool("Tiamat", "Use Tiamat", true));
                OffensiveMenu.Add(new MenuBool("Hydra", "Use Ravenous Hydra", true));
                OffensiveMenu.Add(new MenuBool("Titanic", "Use Titanic Hydra", true));
                OffensiveMenu.Add(new MenuBool("HexRev", "Use Hextech Revolver", true));
                OffensiveMenu.Add(new MenuBool("HexGun", "Use Hextech Gunblade", true));
                OffensiveMenu.Add(new MenuBool("Hex800", "Use HextechGLP-800", true));
                OffensiveMenu.Add(new MenuSeparator("  ", "  "));
                OffensiveMenu.Add(new MenuBool("Combo", "Use In Combo", true));
                OffensiveMenu.Add(new MenuBool("Clear", "Use In Clear", true));
            }

            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead)
            {
                return;
            }

            if (Common.Manager.InCombo && Menu["Offensive"]["Combo"])
            {
                var target = Variables.TargetSelector.GetTarget(600, DamageType.Physical);

                if (target != null && target.IsHPBarRendered)
                {
                    if (Menu["Offensive"]["Youmuus"] && Items.HasItem(3142) && target.IsValidTarget(Me.GetRealAutoAttackRange() + 150))
                    {
                        Items.UseItem(3142);
                    }

                    if (Menu["Offensive"]["Cutlass"] && Items.HasItem(3144) && target.IsValidTarget(Me.GetRealAutoAttackRange()) && target.HealthPercent < 80)
                    {
                        Items.UseItem(3144, target);
                    }

                    if (Menu["Offensive"]["Botrk"] && Items.HasItem(3153) && target.IsValidTarget(Me.GetRealAutoAttackRange()) && (target.HealthPercent < 80 || Me.HealthPercent < 80))
                    {
                        Items.UseItem(3153, target);
                    }

                    if (Menu["Offensive"]["Hydra"] && Items.HasItem(3074) && target.IsValidTarget(Me.GetRealAutoAttackRange()))
                    {
                        Items.UseItem(3074, target);
                    }

                    if (Menu["Offensive"]["Tiamat"] && Items.HasItem(3077) && target.IsValidTarget(Me.GetRealAutoAttackRange()))
                    {
                        Items.UseItem(3077, target);
                    }

                    if (Menu["Offensive"]["Titanic"] && Items.HasItem(3053) && target.IsValidTarget(Me.GetRealAutoAttackRange()))
                    {
                        Items.UseItem(3053, target);
                    }

                    if (Menu["Offensive"]["HexRev"] && Items.HasItem(3145) && target.IsValidTarget(600) && target.HealthPercent < 80)
                    {
                        Items.UseItem(3145, target);
                    }

                    if (Menu["Offensive"]["HexGun"] && Items.HasItem(3146) && target.IsValidTarget(600) && target.HealthPercent < 80)
                    {
                        Items.UseItem(3146, target);
                    }

                    if (Menu["Offensive"]["Hex800"] && Items.HasItem(3030) && target.IsValidTarget(600) && target.HealthPercent < 80)
                    {
                        Items.UseItem(3030, target.ServerPosition);
                    }
                }
            }

            if (Common.Manager.InClear && Menu["Offensive"]["Clear"])
            {
                var Mobs = GameObjects.Jungle.Where(x => x.IsValidTarget(Me.GetRealAutoAttackRange()) && !GameObjects.JungleSmall.Contains(x)).ToList();

                if (Mobs.Count() > 0)
                {
                    if (Menu["Offensive"]["Hydra"] && Items.HasItem(3074) && Mobs.FirstOrDefault().IsValidTarget(Me.GetRealAutoAttackRange()))
                    {
                        Items.UseItem(3074, Mobs.FirstOrDefault());
                    }

                    if (Menu["Offensive"]["Tiamat"] && Items.HasItem(3077) && Mobs.FirstOrDefault().IsValidTarget(Me.GetRealAutoAttackRange()))
                    {
                        Items.UseItem(3077, Mobs.FirstOrDefault());
                    }

                    if (Menu["Offensive"]["Titanic"] && Items.HasItem(3053) && Mobs.FirstOrDefault().IsValidTarget(Me.GetRealAutoAttackRange()))
                    {
                        Items.UseItem(3053, Mobs.FirstOrDefault());
                    }
                }
            }
        }
    }
}