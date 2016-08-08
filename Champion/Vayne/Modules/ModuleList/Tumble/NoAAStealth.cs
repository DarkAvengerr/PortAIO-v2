using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using VayneHunter_Reborn.Modules.ModuleHelpers;
using VayneHunter_Reborn.Utility;
using VayneHunter_Reborn.Utility.MenuUtility;
using EloBuddy;

namespace VayneHunter_Reborn.Modules.ModuleList.Tumble
{
    class NoAAStealth : IModule
    {
        public void OnLoad()
        {
            if (Variables.Menu.Item("dz191.vhr.misc.tumble.ijava").GetValue<bool>())
            {
                Orbwalking.BeforeAttack += iJava;
            }
            else
            {
                Orbwalking.BeforeAttack += OW;
            }
        }

        private void OW(Orbwalking.BeforeAttackEventArgs args)
        {
            if (ShouldGetExecuted() && ObjectManager.Player.Buffs.Any(m => m.Name.ToLower() == "vaynetumblefade"))
            {
                if (ObjectManager.Player.LSCountEnemiesInRange(1100f) <= 1 
                    || ObjectManager.Player.LSCountEnemiesInRange(1100f) < Variables.Menu.Item("dz191.vhr.misc.tumble.noaa.enemies").GetValue<Slider>().Value
                    || ObjectManager.Player.HealthPercent > Variables.Menu.Item("dz191.vhr.misc.tumble.noaastealthex.hp").GetValue<Slider>().Value)
                {
                    return;
                }


                args.Process = false;
            }
        }

        private void iJava(Orbwalking.BeforeAttackEventArgs args)
        {
            if (!ShouldGetExecuted() || !args.Unit.IsMe || !args.Target.IsEnemy)
                return;

            if (args.Unit.HasBuff("vaynetumblefade"))
            {
                var stealthtime = Variables.Menu.Item("dz191.vhr.misc.tumble.noaastealth.duration").GetValue<Slider>().Value;
                var stealthbuff = args.Unit.GetBuff("vaynetumblefade");
                if (stealthbuff.EndTime - Game.Time > stealthbuff.EndTime - stealthbuff.StartTime - stealthtime / 1000f)
                {
                    args.Process = false;
                }
            }
            else
            {
                args.Process = true;
            }
        }

        public bool ShouldGetExecuted()
        {
            return Variables.Menu.Item("dz191.vhr.misc.tumble.noaastealthex") != null 
                && Variables.Menu.Item("dz191.vhr.misc.tumble.noaastealthex").GetValue<KeyBind>().Active;
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.Other;
        }

        public void OnExecute()
        {
        }
    }
}
