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
            var Width = 103;
            var Height = 8;
            var XOffset = 10;
            var YOffset = 20;
            float QDamage = 0, WDamage = 0, EDamage = 0, RDamage = 0;
            //Based on Sebby's Damage Draw.
            foreach (var tar in HeroManager.Enemies.Where(x => !x.IsDead))
            {
                if (core.GetSpells.GetQ.IsReady()) QDamage = core.GetSpells.GetQ.GetDamage(tar);
                if (core.GetSpells.GetW.IsReady()) WDamage = core.GetSpells.GetW.GetDamage(tar);
                if (core.GetSpells.GetE.IsReady()) EDamage = core.GetSpells.GetE.GetDamage(tar);
                if (core.GetSpells.GetR.IsReady()) RDamage = core.GetSpells.RDamage(tar);
                float TotalSpellDamage = QDamage + WDamage + EDamage + RDamage;
                if (tar.IsHPBarRendered && tar.Position.IsOnScreen())
                {
                    var percentHealthAfterDamage = Math.Max(0, tar.Health - TotalSpellDamage) / tar.MaxHealth;
                    var barPos = tar.HPBarPosition;
                    var yPos = barPos.Y + YOffset;
                    var xPosDamage = barPos.X + XOffset + Width * percentHealthAfterDamage;
                    var xPosCurrentHp = barPos.X + XOffset + Width * tar.Health / tar.MaxHealth;
                    var pos1 = barPos.X + XOffset + (107 * percentHealthAfterDamage);
                    float differenceInHP = xPosCurrentHp - xPosDamage;
                    var HpPos = tar.HPBarPosition;
                    float currentXPos= HpPos.X;


                   float  QdmgDraw=0, WdmgDraw=0, EdmgDraw=0, RdmgDraw=0;
                    if (QDamage!=0)
                        QdmgDraw = (QDamage / TotalSpellDamage);

                    if (WDamage!=0)
                        WdmgDraw = (WDamage/ TotalSpellDamage);

                    if (EDamage!=0)
                        EdmgDraw = (EDamage / TotalSpellDamage);

                    if (RDamage!=0)
                        RdmgDraw = (RDamage / TotalSpellDamage);
                    for (var i = 0; i < differenceInHP; i++)
                    {
                        if (QDamage!=0 && i < QdmgDraw * differenceInHP)
                            Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, System.Drawing.Color.Cyan);
                        else if (WDamage!=0 && i < (QdmgDraw + WdmgDraw) * differenceInHP)
                            Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, System.Drawing.Color.OrangeRed);
                        else if (EDamage != 0 && i < (QdmgDraw + WdmgDraw + EdmgDraw) * differenceInHP)
                            Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, System.Drawing.Color.OrangeRed);
                        else if (RDamage != 0 && i < (QdmgDraw + WdmgDraw + EdmgDraw + RdmgDraw) * differenceInHP)
                            Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, System.Drawing.Color.Red);


                    }
                    if (core.GetSpells.RDamage(tar) >= tar.Health)
                    {
                        Drawing.DrawText(HpPos.X,HpPos.Y-20,Color.CornflowerBlue,"Kill With R");
                    }

                }
            }
        }
    }
}
