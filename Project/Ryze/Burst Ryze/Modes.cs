using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RyzeAssembly
{
    class Modes
    {
        public void Update(RyzeMain ryze)
        {
          
            ryze.Spells.igniteCast();
         switch(ryze.Menu.orb.ActiveMode)
            {
                case LeagueSharp.Common.Orbwalking.OrbwalkingMode.Combo:
                    Combo(ryze);
                    break;
                case LeagueSharp.Common.Orbwalking.OrbwalkingMode.Mixed:
                    mixed(ryze);
                    break;
                case LeagueSharp.Common.Orbwalking.OrbwalkingMode.LaneClear:
                    JungleClear(ryze);
                    LaneClear(ryze);
                    break;
            }
        }

        private void LaneClear(RyzeMain ryze)
        {
            var Mana = ryze.Menu.menu.Item("ManaL").GetValue<Slider>().Value;
            var laneclearQ = ryze.Menu.menu.Item("QL").GetValue<bool>(); 
            var laneclearW = ryze.Menu.menu.Item("WL").GetValue<bool>(); 
            var laneclearE = ryze.Menu.menu.Item("EL").GetValue<bool>(); 
            var laneclearR = ryze.Menu.menu.Item("RL").GetValue<bool>(); 
            var minion = MinionManager.GetMinions(ryze.Spells.Q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth).FirstOrDefault();
            if (ryze.Hero.ManaPercent > Mana)
                if (minion != null )
            {
                if (laneclearQ && ryze.Spells.Q.IsReady())
                {
                    var Qpred = ryze.Spells.Q.GetPrediction(minion);
                    ryze.Spells.Q.Cast(Qpred.UnitPosition);
                }
                if (laneclearE && ryze.Spells.E.IsReady())
                {
                    ryze.Spells.E.Cast(minion);
                }
                if (laneclearW && ryze.Spells.W.IsReady())
                {
                    ryze.Spells.W.Cast(minion);
                }
                if (laneclearR && ryze.Spells.R.IsReady() && (ryze.GetPassiveBuff >= 4 || ryze.Hero.HasBuff("ryzepassivecharged")))
                {
                    ryze.Spells.R.Cast();
                }
            }
        
    }

        private static void JungleClear(RyzeMain ryze)
        {
            var Mana = ryze.Menu.menu.Item("ManaJ").GetValue<Slider>().Value;
            var jungleclearQ =  ryze.Menu.menu.Item("QJ").GetValue<bool>() ;
            var jungleclearW = ryze.Menu.menu.Item("WJ").GetValue<bool>();
            var jungleclearE = ryze.Menu.menu.Item("EJ").GetValue<bool>();
            var jungleclearR = ryze.Menu.menu.Item("RJ").GetValue<bool>();
            var minion = MinionManager.GetMinions(ryze.Spells.Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();
            if (ryze.Hero.ManaPercent > Mana)
                if (minion != null)
            {
                if (jungleclearQ && ryze.Spells.Q.IsReady())
                {
                    var Qpred = ryze.Spells.Q.GetPrediction(minion);
                    ryze.Spells.Q.Cast(Qpred.UnitPosition);
                }
                if (jungleclearE &&ryze.Spells.E.IsReady())
                {
                    ryze.Spells.E.Cast(minion);
                }
                if (jungleclearW && ryze.Spells.W.IsReady())
                {
                    ryze.Spells.W.Cast(minion);
                }
                if (jungleclearR && ryze.Spells.R.IsReady() && (ryze.GetPassiveBuff >= 4 || ryze.Hero.HasBuff("ryzepassivecharged")))
                {
                    ryze.Spells.R.Cast();
                }
            }
        }
        private void mixed(RyzeMain ryze)
        {
            var Q = ryze.Menu.menu.Item("QH").GetValue<bool>();
            var Mana = ryze.Menu.menu.Item("ManaH").GetValue<Slider>().Value;
            if (ryze.Hero.ManaPercent>Mana)
            if(Q)
            ryze.Spells.qCastPred();
        }

        private List<String> functions = new List<String>();
        public List<String> Functions
        {
            get
            {
                return functions;
            }
        }
        private int i;
        public int I
        {
            get
            {
                return i;
            }
        }
        private bool rev;
        public bool Rev
        {
            get
            {
                return rev;

            }
            set
            {
                rev = value;
            }
        }
        private bool qcast;
        public bool Qcast
        {
            get
            {
                return qcast;
            }
            set
            {
                qcast = value;
            }
        }
        public void Combo(RyzeMain ryze)
        {
            var Heal = ryze.Menu.menu.Item("%R").GetValue<Slider>().Value;
            if (functions != null)
            {
                if (i < functions.Count)
                {

                    sendSpell(functions[i], ryze);
                    if (rev)
                    {

                        i++;
                        rev = false;
                    }
                }
                else
                {

                    i = 0;
                    functions = null;
                    rev = false;
                }

            }
            else
            {
                if(ryze.Hero.HealthPercent<=Heal)
                {
                    ryze.Spells.R.Cast();
                }
            }
            var target = TargetSelector.GetTarget(600, TargetSelector.DamageType.Magical);

            if (target != null)
            {
                if (functions == null)
                {
                    if (ryze.Spells.Q.IsReady() && ryze.Spells.W.IsReady() && ryze.Spells.E.IsReady() && ryze.Spells.R.IsReady() && ryze.GetPassiveBuff > 0)
                    {
                        switch (ryze.GetPassiveBuff)
                        {

                            case 1:
                                functions = new List<String> { "R", "E", "Q", "W", "Q", "E", "Q", "W", "Q", "E", "Q" };
                                break;
                            case 2:
                                functions = new List<String> { "R", "Q", "W", "Q", "E", "Q", "W", "Q", "E", "Q" };
                                break;
                            case 3:
                                functions = new List<String> { "R", "W", "Q", "E", "Q", "W", "Q", "E", "Q", "W", "Q" };
                                break;
                            case 4:
                                functions = new List<String> { "R", "W", "Q", "E", "Q", "W", "Q", "E" };
                                break;
                        }
                    }

                    else if ((ryze.Spells.Q.IsReady()) && (ryze.Spells.W.IsReady()) && (ryze.Spells.E.IsReady()) && !(ryze.Spells.R.IsReady()) && ryze.GetPassiveBuff > 1)

                    {
                        switch (ryze.GetPassiveBuff)
                        {

                            case 2:
                                functions = new List<String> { "Q", "E", "W", "Q", "E", "Q", "W", "Q", "E" };
                                break;
                            case 3:
                                functions = new List<String> { "Q", "W", "Q", "E", "Q", "W", "Q", "E" };
                                break;
                            case 4:
                                functions = new List<String> { "W", "Q", "E", "Q", "W", "Q", "E", "Q", "W", "Q", "E", "Q" };
                                break;
                        }
                    }
                    else
                    {

                        if (ryze.Hero.HasBuff("ryzepassivecharged"))
                        {
                            if (qcast)
                            {
                                if (ryze.Spells.Q.IsReady())
                                    ryze.Spells.qCast();
                                else if (ryze.Spells.R.IsReady())
                                {
                                    ryze.Spells.rCast();

                                }

                            }
                            else
                            {
                                if (ryze.Spells.W.IsReady())
                                {

                                    ryze.Spells.wCast();
                                }

                                else if (ryze.Spells.E.IsReady())
                                {
                                    ryze.Spells.eCast();
                                }
                                else if (ryze.Spells.R.IsReady())
                                {

                                    ryze.Spells.rCast();
                                }



                            }

                        }
                        else
                        {

                            if (ryze.Spells.Q.IsReady())
                            {
                                ryze.Spells.qCast();
                            }
                            else if (ryze.Spells.W.IsReady())
                            {
                                ryze.Spells.wCast();
                            }
                            else if (ryze.Spells.E.IsReady())
                            {
                                ryze.Spells.eCast();
                            }
                        }





                    }

                }
            }
        
        }
        public bool sendSpell(string s, RyzeMain ryze)
        {
            switch (s)
            {
                case "Q":

                    return ryze.Spells.qCast();

                case "W":

                    return ryze.Spells.wCast();

                case "E":

                    return ryze.Spells.eCast();
                case "R":
                    return ryze.Spells.rCast();

            }
            return false;
        }
    }
}
