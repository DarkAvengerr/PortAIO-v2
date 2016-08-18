using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoTheLastMemebender.Skills
{
    internal static class SteelTempest
    {
        public static Spell Q = new Spell(SpellSlot.Q, 475);
        public static Spell QEmp = new Spell(SpellSlot.Q, 900f);
        public static Spell QDash = new Spell(SpellSlot.Q, 0f);
        public static bool Empowered => ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Name == "yasuoq3w";

        public static void CastQ(Obj_AI_Base target)
        {
            if (ObjectManager.Player.IsDashing())
            {
                if (target.IsValidTarget(QDash.Width))
                {
                    SteelTempest.QDash.Cast();
                }
            }
            else
            {
                if (target.IsValidTarget(QEmp.Range))
                {
                    QEmp.Cast(target);
                }
            }
        }
        public static void LastHitQ()
        {
            if (!Q.IsReady())
            {
                return;
            }
            var minion = MinionManager.GetMinions(ObjectManager.Player.ServerPosition,
                Empowered ? QEmp.Range : Q.Range).Where(m => Q.GetHealthPrediction(m) < Q.GetDamage(m))
                .OrderBy(m => m.Distance(ObjectManager.Player, true)).FirstOrDefault();

            if (minion == null)
            {
                return;
            }

            if ((!Empowered && Config.Param<bool>("ylm.lasthit.useq")) 
                || (Empowered && Config.Param<bool>("ylm.lasthit.useq3")))
            {
                CastQ(minion);

            }
        }

        public static void ClearQ(bool jungleClear = false)
        {
            if (!Q.IsReady())
            {
                return;
            }
            if(jungleClear && !((!Empowered && Config.Param<bool>("ylm.jungleclear.useq")) || (Empowered && Config.Param<bool>("ylm.jungleclear.useq3"))))
            {
                return;
            }
            var qRange = Empowered ? QEmp.Range : Q.Range;
            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition,
                Empowered ? QEmp.Range : Q.Range, MinionTypes.All, jungleClear ? MinionTeam.Neutral : MinionTeam.Enemy);
            // Console.WriteLine("Empowered: {0}, useQ: {1}, useQ3: {2}", Empowered, Config.Param<bool>("ylm.laneclear.useq"), Config.Param<bool>("ylm.laneclear.useq3"));

            if (minions.Count == 0)
            {
                return;
            }
            if (!Empowered)
            {
                if (!Config.Param<bool>("ylm.laneclear.useq"))
                {
                    return;
                }
                if (!ObjectManager.Player.IsDashing())
                {
                    //mini
                    //var Position = SteelTempest.Q.GetLineFarmLocation(minions).Position;
                    Q.Cast(Q.GetLineFarmLocation(minions).Position);
                    // MinionManager.GetBestLineFarmLocation(minions, SteelTempest.Q.Width, SteelTempest.Q.Range);
                }
                else
                {
                    minions.RemoveAll(m => m.Distance(ObjectManager.Player, true) > QDash.WidthSqr);
                    if (minions.Count > 0)
                    {
                        QDash.Cast(ObjectManager.Player);
                    }
                }
            }
            else if (Config.Param<bool>("ylm.laneclear.useq3"))
            {

                if (ObjectManager.Player.IsDashing())
                {
                    minions.RemoveAll(m => m.Distance(ObjectManager.Player, true) > QDash.WidthSqr);
                    if (minions.Count > 0)
                    {
                        QDash.Cast(ObjectManager.Player);
                    }
                }
                else
                {
                    var mode = Config.Param<StringList>("ylm.laneclear.modeq3").SelectedIndex;
                    var bestFarmLocation = QEmp.GetLineFarmLocation(minions);
                    if (mode == 0 ||
                        bestFarmLocation.MinionsHit >= Config.Param<Slider>("ylm.laneclear.modeq3amount").Value)
                    {
                        QEmp.Cast(bestFarmLocation.Position);
                    }
                }
            }
            // minions.
        }


    }
}