using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Flowers_Fiora.Manager.Events
{
    using System;
    using LeagueSharp;

    internal class CreateManager
    {
        internal static void Init(GameObject sender, EventArgs args)
        {
            var emitter = sender as Obj_GeneralParticleEmitter;

            if (emitter != null && emitter.Name.Contains("Fiora_Base"))
            {
                var passive = emitter;

                //Chat.Print(passive.Name);
                // Fiora_Base_Passive_SE_Warning.troy  左边
                // Fiora_Base_Passive_SE.troy
                // Fiora_Base_Passive_SE_Timeout.troy

                // Fiora_Base_Passive_NE_Warning.troy 北边
                // Fiora_Base_Passive_NE.troy
                // Fiora_Base_Passive_NE_Timeout.troy

                // Fiora_Base_Passive_SW_Warning.troy 南边
                // Fiora_Base_Passive_SW.troy
                // Fiora_Base_Passive_SW_Timeout.troy

                // Fiora_Base_Passive_NW_Warning.troy 右边
                // Fiora_Base_Passive_NW.troy
                // Fiora_Base_Passive_NW_Timeout.troy

                //Fiora_Base_Passive_hit_tar.troy 击中目标

                //Fiora_Base_R_ALL_Warning.troy

                //Fiora_Base_R_Mark_NW_FioraOnly.troy
                //Fiora_Base_R_Mark_NE_FioraOnly.troy
                //Fiora_Base_R_Mark_SW_FioraOnly.troy
                //Fiora_Base_R_Mark_SE_FioraOnly.troy

                //Fiora_Base_R_SW_Timeout_FioraOnly.troy
                //Fiora_Base_R_SE_Timeout_FioraOnly.troy
                //Fiora_Base_R_NW_Timeout_FioraOnly.troy
                //Fiora_Base_R_NE_Timeout_FioraOnly.troy

                //Fiora_Base_R_hit_tar.troy 
            }
        }
    }
}