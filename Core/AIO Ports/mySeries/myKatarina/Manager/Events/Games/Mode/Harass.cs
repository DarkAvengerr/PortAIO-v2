using EloBuddy; 
using LeagueSharp.Common; 
namespace myKatarina.Manager.Events.Games.Mode
{
    using Spells;
    using System.Linq;
    using myCommon;
    using LeagueSharp.Common;

    internal class Harass : Logic
    {
        internal static void Init()
        {
            if (SpellManager.isCastingUlt)
            {
                return;
            }

            var target = TargetSelector.GetTarget(E.Range + 300f, TargetSelector.DamageType.Magical);

            if (target.Check(E.Range + 300f))
            {
                switch (Menu.GetList("HarassMode"))
                {
                    case 0:
                        SpellManager.QEWLogic(Menu.GetBool("HarassQ"), Menu.GetBool("HarassW"), Menu.GetBool("HarassE"));
                        break;
                    case 1:
                        SpellManager.EQWLogic(Menu.GetBool("HarassQ"), Menu.GetBool("HarassW"), Menu.GetBool("HarassE"));
                        break;
                }

                if (Menu.GetBool("HarassW") && W.IsReady() && target.IsValidTarget(W.Range))
                {
                    W.Cast();
                }
            }
        }
    }
}
