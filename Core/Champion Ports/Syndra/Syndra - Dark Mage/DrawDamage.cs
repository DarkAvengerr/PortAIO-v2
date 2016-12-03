using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; 
using LeagueSharp.Common; 
namespace DarkMage
{
    class DrawDamage
    {
        SyndraCore core;
        public DrawDamage(SyndraCore core)
        {
            this.core = core;
            EloBuddy.Drawing.OnDraw += Ondraw;
        }
        private void Ondraw(EventArgs args)
        {
            var  DrawTotalDamage = core.GetMenu.GetMenu.Item("DTD").GetValue<bool>();

            if (!DrawTotalDamage) return;
            const int width = 103;
            const int height = 8;
            const int xOffset = 10;
            const int yOffset = 20;
            float qDamage = 0, wDamage = 0, eDamage = 0, rDamage = 0;
            //Based on Sebby's Damage Draw.
            foreach (var tar in HeroManager.Enemies.Where(x => !x.IsDead))
            {
                if (core.GetSpells.GetQ.IsReady()) qDamage = core.GetSpells.GetQ.GetDamage(tar);
                if (core.GetSpells.GetW.IsReady()) wDamage = core.GetSpells.GetW.GetDamage(tar);
                if (core.GetSpells.GetE.IsReady()) eDamage = core.GetSpells.GetE.GetDamage(tar);
                if (core.GetSpells.GetR.IsReady()) rDamage = core.GetSpells.RDamage(tar,core);
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
                var currentXPos= hpPos.X;


                float  qdmgDraw=0, wdmgDraw=0, edmgDraw=0, rdmgDraw=0;
                if (qDamage!=0)
                    qdmgDraw = (qDamage / totalSpellDamage);

                if (wDamage!=0)
                    wdmgDraw = (wDamage/ totalSpellDamage);

                if (eDamage!=0)
                    edmgDraw = (eDamage / totalSpellDamage);

                if (rDamage!=0)
                    rdmgDraw = (rDamage / totalSpellDamage);
                for (var i = 0; i < differenceInHp; i++)
                {
                    if (rDamage!=0 && i < rdmgDraw * differenceInHp)
                        Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + height, 1, System.Drawing.Color.Red);
                    else if (eDamage!=0 && i < (rdmgDraw + edmgDraw) * differenceInHp)
                        Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + height, 1, System.Drawing.Color.OrangeRed);
                    else if (wDamage != 0 && i < (rdmgDraw + edmgDraw + wdmgDraw) * differenceInHp)
                        Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + height, 1, System.Drawing.Color.Yellow);
                    else if (qDamage != 0 && i < (rdmgDraw + edmgDraw + wdmgDraw + qdmgDraw) * differenceInHp)
                        Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + height, 1, System.Drawing.Color.Aqua);


                }
                if(core.GetSpells.GetR.IsReady())
                if (core.GetSpells.RDamage(tar,core) >= tar.Health)
                {
                    Drawing.DrawText(hpPos.X,hpPos.Y-20,Color.CornflowerBlue,"Kill With R");
                }
                else
                {
                    var countCurrentSpheres = core.GetOrbs.Count;
                    var totalPossibleSpheres = 7;
                    for (var i = countCurrentSpheres; i < totalPossibleSpheres; i++)
                    {
                        if (core.GetSpells.RDamage(tar, i) >= tar.Health&& i-countCurrentSpheres-4 >0)
                        {
                                Drawing.DrawText(hpPos.X, hpPos.Y - 20, Color.CornflowerBlue, "Cast "+ (i-countCurrentSpheres-4 ) + " spheres to Kill With R");
                            break;
                        }
                    }
                }
            }
        }
    }
}
