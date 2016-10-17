using LeagueSharp;
using Kennen.Core;
using LeagueSharp.Common;
using EloBuddy;
namespace Kennen.Modes
{
    internal class Harass
    {
        public static void ExecuteHarass()
        {
            var target = TargetSelector.GetTarget(Spells.Q.Range, TargetSelector.DamageType.Magical);

            if (target == null || !target.IsValid)
                return;

            var castQ = Configs.config.Item("useQHarass").GetValue<bool>() && Spells.Q.IsReady();
            var castW = Configs.config.Item("useWHarass").GetValue<bool>() && Spells.W.IsReady();
            var modeW = Configs.config.Item("useWmodeHarass").GetValue<StringList>();


            if (castQ && target.IsValidTarget(Spells.Q.Range) && ObjectManager.Player.ManaPercent >= Configs.config.Item("useQHarassMana").GetValue<Slider>().Value)
            {
                Spells.Q.CastSpell(target, "predMode", "hitchanceQ");
            }

            if (castW && target.IsValidTarget(Spells.W.Range) && ObjectManager.Player.ManaPercent >= Configs.config.Item("useWHarassMana").GetValue<Slider>().Value)
            {
                switch (modeW.SelectedIndex)
                {
                    case 0:
                        if (Champion.Kennen.HasMark(target))
                        {
                            Spells.W.Cast();
                        }
                        break;

                    case 1:
                        if (Champion.Kennen.CanStun(target))
                        {
                            Spells.W.Cast();
                        }
                        break;
                }
            }
        }
    }
}
