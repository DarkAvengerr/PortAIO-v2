using EloBuddy; 
using LeagueSharp.Common; 
namespace myCommon
{
    using System;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class ItemManager
    {
        private static Menu itemMenu;

        public static void AddToMenu(Menu mainMenu)
        {
            itemMenu = mainMenu;

            var hextechMenu = mainMenu.AddSubMenu(new Menu("Hextech Gunblade", "hextechMenu"));
            {
                hextechMenu.AddItem(new MenuItem("HextechEnabled", "Enabled", true).SetValue(true));
                hextechMenu.AddItem(new MenuItem("HextechCombo", "Use in Combo", true).SetValue(true));
                hextechMenu.AddItem(
                    new MenuItem("HextechComboHealth", "Use in Combo|When target HealthPercent <= x%", true).SetValue(
                        new Slider(80)));
                hextechMenu.AddItem(new MenuItem("HextechKillSteal", "When target Can Kill", true).SetValue(true));
            }

            var glp800Menu = mainMenu.AddSubMenu(new Menu("Hextech GLP-800", "glp800Menu"));
            {
                glp800Menu.AddItem(new MenuItem("GLP800Enabled", "Enabled", true).SetValue(true));
                glp800Menu.AddItem(new MenuItem("GLP800Combo", "Use in Combo", true).SetValue(true));
                glp800Menu.AddItem(
                    new MenuItem("GLP800ComboHealth", "Use in Combo|When target HealthPercent <= x%", true).SetValue(
                        new Slider(80)));
                glp800Menu.AddItem(new MenuItem("GLP800KillSteal", "When target Can Kill", true).SetValue(true));
            }

            var protobeltMenu = mainMenu.AddSubMenu(new Menu("Hextech Protobelt-01", "protobeltMenu"));
            {
                protobeltMenu.AddItem(new MenuItem("ProtobeltEnabled", "Enabled", true).SetValue(true));
                protobeltMenu.AddItem(new MenuItem("ProtobeltCombo", "Use in Combo", true).SetValue(true));
                protobeltMenu.AddItem(
                    new MenuItem("ProtobeltComboHealth", "Use in Combo|When target HealthPercent <= x%", true).SetValue(
                        new Slider(80)));
                protobeltMenu.AddItem(new MenuItem("ProtobeltKillSteal", "When target Can Kill", true).SetValue(true));
            }

            mainMenu.AddItem(new MenuItem("OffensivesEnabled", "Enabled", true).SetValue(true));
            mainMenu.AddItem(new MenuItem("OffensivesKey", "Combo Key", true).SetValue(new KeyBind(32, KeyBindType.Press)));

            Game.OnUpdate += OnUpdate;
        }

        private static bool isCombo => itemMenu.Item("OffensivesKey", true).GetValue<KeyBind>().Active;

        private static void OnUpdate(EventArgs Args)
        {
            if (ObjectManager.Player.IsDead || ObjectManager.Player.IsRecalling() || ObjectManager.Player.InFountain() ||
                ObjectManager.Player.Buffs.Any(x => x.Name.ToLower().Contains("katarinar")) ||
                ObjectManager.Player.IsChannelingImportantSpell() || 
                !itemMenu.Item("OffensivesEnabled", true).GetValue<bool>())
            {
                return;
            }

            #region Hextech Gun
            if (itemMenu.Item("HextechEnabled").GetValue<bool>() && Items.HasItem(3146) && Items.CanUseItem(3146))
            {
                if (itemMenu.Item("HextechCombo", true).GetValue<bool>() && isCombo)
                {
                    var target = TargetSelector.GetTarget(650f, TargetSelector.DamageType.Magical);

                    if (target.IsValidTarget(650f))
                    {
                        if (target.HealthPercent <= itemMenu.Item("HextechComboHealth", true).GetValue<Slider>().Value)
                        {
                            Items.UseItem(3146, target);
                        }
                    }
                }

                if (itemMenu.Item("HextechKillSteal", true).GetValue<bool>())
                {
                    foreach (
                        var target in
                        HeroManager.Enemies.Where(
                            x =>
                                !x.IsDead && !x.IsZombie &&
                                x.Health < ObjectManager.Player.CalcDamage(x, Damage.DamageType.Magical,
                                    250 + 0.3*ObjectManager.Player.FlatMagicDamageMod)))
                    {
                        if (target.IsValidTarget(650f))
                        {
                            Items.UseItem(3146, target);
                        }
                    }
                }
            }
            #endregion

            #region Hextech GLP-800
            if (itemMenu.Item("GLP800Enabled").GetValue<bool>() && Items.HasItem(3030) && Items.CanUseItem(3030))
            {
                if (itemMenu.Item("GLP800Combo", true).GetValue<bool>() && isCombo)
                {
                    var target = TargetSelector.GetTarget(650f, TargetSelector.DamageType.Magical);

                    if (target.IsValidTarget(650f))
                    {
                        if (target.HealthPercent <= itemMenu.Item("GLP800ComboHealth", true).GetValue<Slider>().Value)
                        {
                            var pred = Prediction.GetPrediction(target, 0.8f);

                            Items.UseItem(3030, pred.CastPosition);
                        }
                    }
                }

                if (itemMenu.Item("GLP800KillSteal", true).GetValue<bool>())
                {
                    foreach (
                        var target in
                        HeroManager.Enemies.Where(
                            x =>
                                !x.IsDead && !x.IsZombie &&
                                x.Health < ObjectManager.Player.CalcDamage(x, Damage.DamageType.Magical,
                                    200 + 0.2 * ObjectManager.Player.FlatMagicDamageMod)))
                    {
                        if (target.IsValidTarget(650f))
                        {
                            var pred = Prediction.GetPrediction(target, 0.8f);

                            Items.UseItem(3030, pred.CastPosition);
                        }
                    }
                }
            }
            #endregion

            #region Hextech Protobelt-01
            if (itemMenu.Item("ProtobeltEnabled").GetValue<bool>() && Items.HasItem(3152) && Items.CanUseItem(3152))
            {
                if (itemMenu.Item("ProtobeltCombo", true).GetValue<bool>() && isCombo)
                {
                    var target = TargetSelector.GetTarget(650f, TargetSelector.DamageType.Magical);

                    if (target.IsValidTarget(650f))
                    {
                        if (target.HealthPercent <= itemMenu.Item("ProtobeltComboHealth", true).GetValue<Slider>().Value)
                        {
                            var pred = Prediction.GetPrediction(target, 0.8f);

                            Items.UseItem(3152, pred.CastPosition);
                        }
                    }
                }

                if (itemMenu.Item("ProtobeltKillSteal", true).GetValue<bool>())
                {
                    foreach (
                        var target in
                        HeroManager.Enemies.Where(
                            x =>
                                !x.IsDead && !x.IsZombie &&
                                x.Health < ObjectManager.Player.CalcDamage(x, Damage.DamageType.Magical,
                                    150 + 0.35* ObjectManager.Player.FlatMagicDamageMod)))
                    {
                        if (target.IsValidTarget(650f))
                        {
                            var pred = Prediction.GetPrediction(target, 0.8f);

                            Items.UseItem(3152, pred.CastPosition);
                        }
                    }
                }
            }
            #endregion
        }
    }
}
