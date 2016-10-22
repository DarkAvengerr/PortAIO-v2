using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Azir_Creator_of_Elo
{
    class SoldierManager
    {
 

      
        private static Dictionary<int, string> Animations = new Dictionary<int, string>();
        private static List<Obj_AI_Minion> _soldiers = new List<Obj_AI_Minion>();
        public List<Obj_AI_Minion> Soldiers {
            get
            {
                return _soldiers;
            }
            }
     public SoldierManager()
        {
            Obj_AI_Minion.OnCreate += Obj_AI_Minion_OnCreate;
            Obj_AI_Minion.OnDelete += Obj_AI_Minion_OnDelete;
            Obj_AI_Minion.OnPlayAnimation += Obj_AI_Minion_OnPlayAnimation;

            Drawing.OnDraw += Drawing_OnDraw;
            
        }
        public  List<Obj_AI_Minion> ActiveSoldiers
        {
            get { return _soldiers.Where(s => s.IsValid && !s.IsDead && !s.IsMoving && (!Animations.ContainsKey(s.NetworkId) || Animations[s.NetworkId] != "Inactive")).ToList(); }
        }

        public bool CheckQCastAtLaneClear(List<Obj_AI_Base> minions ,AzirMain azir)
        {
            var nSoldiersToQ = azir.Menu.GetMenu.Item("LQM").GetValue<Slider>().Value;
            const int attackRange=315;
            int x=0;
            foreach (var sol in azir.SoldierManager.Soldiers)
            {
                if(!sol.IsDead)
                foreach (var min in minions)
                {
                    if(!min.IsDead)
                    if (min.ServerPosition.Distance(sol.ServerPosition)<= attackRange) //estos estan atacando
                    {
                        x++;
                        break;
                    }
                }
            }
            int z=azir.SoldierManager.Soldiers.Count - x;
        
            return z >= nSoldiersToQ;
        }

        public Obj_AI_Minion getSoldierOnRange(int range)
        {
          foreach(Obj_AI_Minion sol in ActiveSoldiers)
            {
                if(sol.Distance(HeroManager.Player.ServerPosition)<=range)
                    {
                    return sol;
                }
            }

            return null;
        }
        public bool ChecksToCastQHarrash(AzirMain azir, AIHeroClient target)
        {
            if (!azir.Spells.Q.IsReady()) return false;
            int x = 0; // soldados no atacando
            var nSoldiersToQ =azir.Menu.GetMenu.Item("SoldiersToQ").GetValue<Slider>().Value;
            switch (azir._menu.Orb.ActiveMode)
            {
                case LeagueSharp.Common.Orbwalking.OrbwalkingMode.Combo:
                    nSoldiersToQ = azir.Menu.GetMenu.Item("SoldiersToQ").GetValue<Slider>().Value;
                    break;
                case LeagueSharp.Common.Orbwalking.OrbwalkingMode.Mixed:
                    nSoldiersToQ = azir.Menu.GetMenu.Item("hSoldiersToQ").GetValue<Slider>().Value;
                    break;
            }
            if (!target.IsDead)
                foreach (Obj_AI_Minion me in azir.SoldierManager.Soldiers)
                {
                    if (!me.IsDead)
                    {

                        if (me.Distance(target) > 315)
                        {
                            x++;


                        }


                    }
                }
            return x >= nSoldiersToQ;



        }
        public bool ChecksToCastQ(AzirMain azir, AIHeroClient target)
        {
            if (!azir.Spells.Q.IsReady()) return false;
            int x = 0; // soldados no atacando
           
            var nSoldiersToQ = azir.Menu.GetMenu.Item("SoldiersToQ").GetValue<Slider>().Value;
            if (!target.IsDead)
            foreach (Obj_AI_Minion me in azir.SoldierManager.Soldiers)
            {
                if (!me.IsDead)
                {
                   
                        if (me.Distance(target) > 315)
                        {
                            x++;
                          

                        }

                    
                }
            }
            return x >= nSoldiersToQ;


            
        }

       
        private void Obj_AI_Minion_OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (sender is Obj_AI_Minion && IsSoldier((Obj_AI_Minion)sender))
            {
                Animations[sender.NetworkId] = args.Animation;
            }
        }

        private void Obj_AI_Minion_OnDelete(GameObject sender, EventArgs args)
        {
            _soldiers.RemoveAll(s => s.NetworkId == sender.NetworkId);
         //   Animations.Remove(sender.NetworkId);
        }
        private bool IsSoldier(Obj_AI_Minion soldier)
        {
            return soldier.IsAlly && String.Equals(soldier.BaseSkinName, "azirsoldier", StringComparison.InvariantCultureIgnoreCase);
        }
        private  void Obj_AI_Minion_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender is Obj_AI_Minion && IsSoldier((Obj_AI_Minion)sender))
            {
                _soldiers.Add((Obj_AI_Minion)sender);
            }
        }
        public  Obj_AI_Minion getClosestSolider(Vector3 pos)
        {
            return Soldiers.Where(sol => !sol.IsDead).OrderBy(sol => sol.Distance(pos, true) - ((sol.IsMoving) ? 500 : 0)).FirstOrDefault();
        }
        private  void Drawing_OnDraw(EventArgs args)
        {
   
       //     Chat.Print(""+ ActiveSoldiers.Count);
        }
     
        public bool SoldiersAttacking(AzirMain azir)
        {
            foreach (Obj_AI_Minion m in azir.SoldierManager.Soldiers)
            {
               
                foreach (AIHeroClient h in HeroManager.Enemies)
                {
                    if (m.Distance(h) < 315)
                    {
          
                        return true;
                    }
                }
            }

            return false;
        }

        }
}
