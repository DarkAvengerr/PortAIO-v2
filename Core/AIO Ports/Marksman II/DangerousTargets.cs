using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Marksman
{
    public class DangerousTargets
    {

        Dictionary<string, SpellSlot> dangerDictionary = new Dictionary<string, SpellSlot>();
        public DangerousTargets()
        {
            dangerDictionary.Add("alistar", SpellSlot.W);
            dangerDictionary.Add("amumu", SpellSlot.Q);
            dangerDictionary.Add("annie", SpellSlot.R);
            dangerDictionary.Add("darius", SpellSlot.R);
            dangerDictionary.Add("fizz", SpellSlot.R);
            dangerDictionary.Add("garen", SpellSlot.R);
            dangerDictionary.Add("gragas", SpellSlot.R);
            dangerDictionary.Add("irelia", SpellSlot.Q);
            dangerDictionary.Add("jarvan", SpellSlot.E);
            dangerDictionary.Add("jax", SpellSlot.Q);
            dangerDictionary.Add("kassadin", SpellSlot.R);
            dangerDictionary.Add("katarina", SpellSlot.R);
            dangerDictionary.Add("kennen", SpellSlot.R);
            dangerDictionary.Add("khazix", SpellSlot.E);
            dangerDictionary.Add("leblanc", SpellSlot.W);
            dangerDictionary.Add("leblanc", SpellSlot.R);
            dangerDictionary.Add("leesin", SpellSlot.Q);
            dangerDictionary.Add("leesin", SpellSlot.R);
            dangerDictionary.Add("lissandra", SpellSlot.R);
            dangerDictionary.Add("lux", SpellSlot.Q);
            dangerDictionary.Add("malphine", SpellSlot.R);
            dangerDictionary.Add("malzahar", SpellSlot.R);
            dangerDictionary.Add("maokai", SpellSlot.W);
            dangerDictionary.Add("masteryi", SpellSlot.Q);
            dangerDictionary.Add("morgana", SpellSlot.Q);
            dangerDictionary.Add("morgana", SpellSlot.R);
            dangerDictionary.Add("nami", SpellSlot.Q);
            dangerDictionary.Add("nautilus", SpellSlot.Q);
            dangerDictionary.Add("orianna", SpellSlot.R);
            dangerDictionary.Add("pantheon", SpellSlot.W);
            dangerDictionary.Add("poppy", SpellSlot.E);
            dangerDictionary.Add("rammus", SpellSlot.E);
            dangerDictionary.Add("renekton", SpellSlot.E);
            dangerDictionary.Add("ryze", SpellSlot.W);
            dangerDictionary.Add("sejuani", SpellSlot.Q);
            dangerDictionary.Add("sejuani", SpellSlot.R);
            dangerDictionary.Add("shen", SpellSlot.E);
            dangerDictionary.Add("skarner", SpellSlot.R);
            dangerDictionary.Add("sona", SpellSlot.R);
            dangerDictionary.Add("syndra", SpellSlot.R);
            dangerDictionary.Add("talon", SpellSlot.E);
            dangerDictionary.Add("thresh", SpellSlot.Q);
            dangerDictionary.Add("thresh", SpellSlot.R);
            dangerDictionary.Add("twistedfate", SpellSlot.W);
            dangerDictionary.Add("udyr", SpellSlot.E);
            dangerDictionary.Add("urgot", SpellSlot.R);
            dangerDictionary.Add("veigar", SpellSlot.W);
            dangerDictionary.Add("veigar", SpellSlot.R);
            dangerDictionary.Add("vi", SpellSlot.Q);
            dangerDictionary.Add("vi", SpellSlot.R);
            dangerDictionary.Add("volibear", SpellSlot.Q);

            
            dangerDictionary.Add("garen", SpellSlot.R);
            dangerDictionary.Add("leona", SpellSlot.E);
            
            
            dangerDictionary.Add("warwick", SpellSlot.R);
            dangerDictionary.Add("zed", SpellSlot.R);
            
        }
    }
}
