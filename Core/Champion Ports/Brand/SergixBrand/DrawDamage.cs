using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace SergixBrand
{
    class DrawDamage
    {
        private List<Damage> DamageList;
        private Core core;

        public DrawDamage(Core core,List<Damage> DamageList )
        {
            this.core = core;
            this.DamageList = DamageList;
            Drawing.OnDraw += OnDraw;
        }

        private void OnDraw(EventArgs args)
        {
            var DrawTotalDamage = core.GetMenu.GetMenu.Item("DTD").GetValue<bool>();
            
            if (!DrawTotalDamage) return;
            // values from sebbyLib.
            const int width = 103;
            const int height = 8;
            const int xOffset = 10;
            const int yOffset = 20;

            foreach (var tar in HeroManager.Enemies.Where(x => !x.IsDead))
            {
                if (!tar.IsHPBarRendered || !tar.Position.IsOnScreen()) continue;
                double totalDamage = 0;
                foreach (Damage dam in DamageList)
                {
                    totalDamage +=(double) dam.getDamageValue(tar);
                }
              var targets=  ObjectManager.Get<Obj_AI_Base>().Where(x => x.Distance(tar) <= 300 && core.GetSpells.isBlazed(x));
                if (targets != null)
                {
             //     Chat.Print( "Target count : "+ targets.Count());
                }
                double passiveDamage=0;
                if(core.GetSpells.isBlazed(tar))
               passiveDamage+= core.GetSpells.CalcPassiveDamage(tar);
                totalDamage += passiveDamage;
               double passiveTotalDamageDraw= (passiveDamage / totalDamage);
                //    core.GetSpells.getBlazed(tar).
                var percentHealthAfterDamage = Math.Max(0, tar.Health - totalDamage) / tar.MaxHealth;
                var barPos = tar.HPBarPosition;
                var yPos = barPos.Y + yOffset;
                var xPosDamage = barPos.X + xOffset + width * percentHealthAfterDamage;
                var xPosCurrentHp = barPos.X + xOffset + width * tar.Health / tar.MaxHealth;
                var pos1 = barPos.X + xOffset + (107 * percentHealthAfterDamage);
                var differenceInHp = xPosCurrentHp - xPosDamage;
                var hpPos = tar.HPBarPosition;
                var currentXPos = hpPos.X;


                for (var i = 0; i < differenceInHp; i++)
                {
                    if (passiveDamage != 0 && i < (passiveTotalDamageDraw * differenceInHp))
                    {
                        Drawing.DrawLine((float) pos1 + i, yPos, (float) pos1 + i, yPos + height, 1,
                            System.Drawing.Color.DarkRed);
                    }
                    else
                    {
                        Drawing.DrawLine((float) pos1 + i, yPos, (float) pos1 + i, yPos + height, 1,
                            System.Drawing.Color.Aqua);
                    }
                }
            }
        }

        public void AddDamage(Damage damage)
        {
            DamageList.Add(damage);
        }



    }
}
