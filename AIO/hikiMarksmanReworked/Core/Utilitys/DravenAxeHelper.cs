using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hikiMarksmanRework.Core.Menus;
using hikiMarksmanRework.Core.Spells;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace hikiMarksmanRework.Core.Utilitys
{
    class Axe
    {
        public Axe(GameObject obj)
        {
            AxeObj = obj;
            EndTick = Environment.TickCount + 1200;
        }

        public int EndTick;
        public GameObject AxeObj;
    }

    class DravenAxeHelper
    {
        public static List<Axe> AxeSpots = new List<Axe>();
        public static int CurrentAxes;
        public static int LastAa;
        public static int LastQ;
        public static List<String> AxesList = new List<string>()
        {
            "Draven_Base_Q_reticle.troy" , "Draven_Skin01_Q_reticle.troy" ,"Draven_Skin03_Q_reticle.troy"
        };

        public static List<String> QBuffList = new List<string>()
        {
            "Draven_Base_Q_buf.troy", "Draven_Skin01_Q_buf.troy", "Draven_Skin02_Q_buf.troy", "Draven_Skin03_Q_buf.troy"
        };
        public static AIHeroClient Draven { get { return ObjectManager.Player; } }
        public static bool HasQBuff { get { return Draven.Buffs.Any(a => a.DisplayName.ToLower().Contains("spinning")); } }

        public static int MidAirAxes
        {
            get { return AxeSpots.Count(a => a.AxeObj.IsValid && a.EndTick < Environment.TickCount); }
        }

        public static float RealAutoAttack(Obj_AI_Base target)
        {
            return (float)Draven.CalcDamage(target, Damage.DamageType.Physical, (ObjectManager.Player.BaseAttackDamage + ObjectManager.Player.FlatPhysicalDamageMod +
                (((DravenSpells.Q.Level) > 0 && HasQBuff ? new float[] { 45, 55, 65, 75, 85 }[DravenSpells.Q.Level - 1] : 0) / 100 * (ObjectManager.Player.BaseAttackDamage + ObjectManager.Player.FlatPhysicalDamageMod))));
        }
        public static bool InCatchRadius(Axe a)
        {
            var mode = DravenMenu.Config.Item("catchRadiusMode").GetValue<StringList>().SelectedIndex;
            switch (mode)
            {
                case 1:
                    var b = new Geometry.Polygon.Sector(Draven.Position.LSTo2D(), Game.CursorPos.LSTo2D() - Draven.Position.LSTo2D(), DravenMenu.Config.Item("sectorAngle").GetValue<Slider>().Value
                        * (float)Math.PI / 180, DravenMenu.Config.Item("catchRadius").GetValue<Slider>().Value).IsOutside(a.AxeObj.Position.LSExtend(Game.CursorPos, 30).LSTo2D());

                    return !b;
                default:
                    return a.AxeObj.Position.LSDistance(Game.CursorPos) <
                           DravenMenu.Config.Item("catchRadius").GetValue<Slider>().Value;
            }
        }

        public static void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;
            if (args.SData.LSIsAutoAttack())
            {
                LastAa = Environment.TickCount;
            }
            if (args.SData.Name == "dravenspinning")
            {
                LastQ = Environment.TickCount;
            }
        }

        public static void OnCreate(GameObject sender, EventArgs args)
        {
            var name = sender.Name;
            if ((AxesList.Contains(name)) && sender.Position.LSDistance(ObjectManager.Player.Position) / ObjectManager.Player.MoveSpeed <= 2)
            {
                AxeSpots.Add(new Axe(sender));
            }

            if ((QBuffList.Contains(name)) &&
                sender.Position.LSDistance(ObjectManager.Player.Position) < 100)
            {
                CurrentAxes += 1;
            }
        }

        public static void OnDelete(GameObject sender, EventArgs args)
        {
            for (var i = 0; i < AxeSpots.Count; i++)
            {
                if (AxeSpots[i].AxeObj.NetworkId == sender.NetworkId)
                {
                    AxeSpots.RemoveAt(i);
                    return;
                }
            }

            if ((QBuffList.Contains(sender.Name)) && sender.Position.LSDistance(ObjectManager.Player.Position) < 300)
            {
                if (CurrentAxes == 0)
                {
                    CurrentAxes = 0;
                }

                if (CurrentAxes <= 2)
                {
                    CurrentAxes = CurrentAxes - 1;
                }
                else
                {
                    CurrentAxes = CurrentAxes - 1;
                }
            }
        }
    }
}
