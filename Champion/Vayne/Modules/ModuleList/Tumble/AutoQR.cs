using System.Linq;
using DZLib.Positioning;
using LeagueSharp;
using LeagueSharp.Common;
using VayneHunter_Reborn.Modules.ModuleHelpers;
using VayneHunter_Reborn.Skills.Tumble.VHRQ;
using VayneHunter_Reborn.Utility;
using VayneHunter_Reborn.Utility.MenuUtility;
using EloBuddy;

namespace VayneHunter_Reborn.Modules.ModuleList.Tumble
{
    class AutoQR : IModule
    {
        public void OnLoad()
        {
            Obj_AI_Base.OnProcessSpellCast += OnSpellcast;
        }

        private void OnSpellcast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.Slot == SpellSlot.R && ShouldGetExecuted())
            {
                var qCastPosition = ObjectManager.Player.ServerPosition.LSExtend(Game.CursorPos, 300f);
                if (qCastPosition.IsSafe() && qCastPosition.IsSafeEx())
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(250, () =>
                    { Variables.spells[SpellSlot.Q].Cast(qCastPosition); });
                }
            }
        }

        public bool ShouldGetExecuted()
        {
            return Variables.Menu.Item("dz191.vhr.misc.tumble.autoQR") != null
                && Variables.Menu.Item("dz191.vhr.misc.tumble.autoQR").GetValue<bool>() && Variables.spells[SpellSlot.Q].LSIsReady();
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
