using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;

namespace KoreanCommon
{
    public abstract class CommonOrbwalkComplementation
    {
        protected CommonChampion champion { get; set; }
        protected readonly CommonSpells spells;
        protected readonly CommonSpell Q;
        protected readonly CommonSpell W;
        protected readonly CommonSpell E;
        protected readonly CommonSpell R;
        protected readonly CommonSpell RFlash;

        public abstract void LastHitMode();
        public abstract void HarasMode();
        public abstract void LaneClearMode();
        public abstract void ComboMode();
        public abstract void Ultimate(AIHeroClient target);

        public CommonOrbwalkComplementation(CommonChampion champion)
        {
            this.champion = champion;
            spells = champion.Spells;
            Q = champion.Spells.Q;
            W = champion.Spells.W;
            E = champion.Spells.E;
            R = champion.Spells.R;
            RFlash = champion.Spells.RFlash;

            Game.OnUpdate += UseSkills;
        }

        public void UseSkills(EventArgs args)
        {
            if (champion != null)
            {
                switch (champion.MainMenu.Orbwalker.ActiveMode)
                {
                    case Orbwalking.OrbwalkingMode.LastHit:
                        LastHitMode();
                        break;
                    case Orbwalking.OrbwalkingMode.Mixed:
                        HarasMode();
                        break;
                    case Orbwalking.OrbwalkingMode.LaneClear:
                        LaneClearMode();
                        break;
                    case Orbwalking.OrbwalkingMode.Combo:
                        ComboMode();
                        break;
                }
            }
        }
    }
}
