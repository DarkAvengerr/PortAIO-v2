using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using MasterOfThorns;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace MasterOfPlants
{
    class DamageIndicator
    {
        private Program program;
        public DamageIndicator(Program program)
        {
            this.program = program;
            EloBuddy.Drawing.OnDraw += Ondraw;
        }

        private void Ondraw(EventArgs args)
        {
            var DrawTotalDamage = program.getMenu().Item("DrawDamageIndicator").GetValue<bool>();

            if (!DrawTotalDamage) return;
            const int width = 103;
            const int height = 8;
            const int xOffset = 10;
            const int yOffset = 20;
            float qDamage = 0, wDamage = 0, eDamage = 0, rDamage = 0;
            //Based on Sebby's Damage Draw.
            foreach (var tar in HeroManager.Enemies.Where(x => !x.IsDead))
            {
                if (program.getSkills().getQ().IsReady()) qDamage = program.getSkills().getQ().GetDamage(tar);
                if (program.getSkills().getE().IsReady()) eDamage = program.getSkills().getE().GetDamage(tar);
                if (program.getSkills().getR().IsReady()) rDamage = program.getSkills().getR().GetDamage(tar);
                var totalSpellDamage = qDamage + wDamage + eDamage + rDamage;
                if (!tar.IsHPBarRendered || !tar.Position.IsOnScreen()) continue;
                var percentHealthAfterDamage = Math.Max(0, tar.Health - totalSpellDamage) / tar.MaxHealth;
                var barPos = tar.HPBarPosition;
                var yPos = barPos.Y + yOffset;
                var xPosDamage = barPos.X + xOffset + width * percentHealthAfterDamage;
                var xPosCurrentHp = barPos.X + xOffset + width * tar.Health / tar.MaxHealth;
                var pos1 = barPos.X + xOffset + (107 * percentHealthAfterDamage);
                var differenceInHp = xPosCurrentHp - xPosDamage;
                var hpPos = tar.HPBarPosition;
                var currentXPos = hpPos.X;


                float qdmgDraw = 0, wdmgDraw = 0, edmgDraw = 0, rdmgDraw = 0;
                if (qDamage != 0)
                    qdmgDraw = (qDamage / totalSpellDamage);

                if (wDamage != 0)
                    wdmgDraw = (wDamage / totalSpellDamage);

                if (eDamage != 0)
                    edmgDraw = (eDamage / totalSpellDamage);

                if (rDamage != 0)
                    rdmgDraw = (rDamage / totalSpellDamage);
                for (var i = 0; i < differenceInHp; i++)
                {
                        Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + height, 1, System.Drawing.Color.OrangeRed);

                }
          
            }
        }
    }
}

