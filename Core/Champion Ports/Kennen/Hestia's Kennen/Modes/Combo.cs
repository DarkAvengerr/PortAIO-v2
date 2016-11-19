using Kennen.Core;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Kennen.Modes
{
    internal class Combo
    {
        public static void ExecuteCombo()
        {
            var target = TargetSelector.GetTarget(Spells.Q.Range, TargetSelector.DamageType.Magical);

            if (target == null || !target.IsValid)
                return;

            var castQ = Configs.config.Item("useQ").GetValue<bool>() && Spells.Q.IsReady();
            var castW = Configs.config.Item("useW").GetValue<bool>() && Spells.W.IsReady();
            var modeW = Configs.config.Item("useWmodeCombo").GetValue<StringList>();
            var castR = Configs.config.Item("useR").GetValue<bool>() && Spells.R.IsReady();
            var castProto = Configs.config.Item("useProto").GetValue<bool>();

            if (castProto && target.IsValidTarget() && !Spells.R.IsReady())
            {
                ItemsHandler.UseProtobelt(target);
            }

            if (castQ && target.IsValidTarget(Spells.Q.Range))
            {
                Spells.Q.CastSpell(target, "predMode", "hitchanceQ");
            }

            if (castW && target.IsValidTarget(Spells.W.Range))
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
            if (castR && !ObjectManager.Player.HasBuffOfType(BuffType.Snare) && target.Health < Champion.Kennen.ComboDamage(target))
            {
                Spells.R.Cast();

                if (ObjectManager.Player.HasBuff("KennenShurikenStorm") && castProto)
                {
                    ItemsHandler.UseProtobelt(target);
                }

                Spells.W.Cast();
            }
        }
    }
}
