/*
  ____              _                          _   _____  _____   ____    _   _     _     
 | __ )   _   _    | |       ___    _ __    __| | |__  / | ____| |  _ \  (_) | |_  | |__  
 |  _ \  | | | |   | |      / _ \  | '__|  / _` |   / /  |  _|   | | | | | | | __| | '_ \ 
 | |_) | | |_| |   | |___  | (_) | | |    | (_| |  / /_  | |___  | |_| | | | | |_  | | | |
 |____/   \__, |   |_____|  \___/  |_|     \__,_| /____| |_____| |____/  |_|  \__| |_| |_|
          |___/                                                                           
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace GodJungleTracker.Classes
{
    public class Packets
    {
        public static OnPatienceChange Patience;
        public static OnAttack Attack;
        public static OnMissileHit MissileHit;
        public static OnDisengaged Disengaged;
        public static OnMonsterSkill MonsterSkill;
        public static OnCreateGromp CreateGromp;
        public static OnCreateCampIcon CreateCampIcon;

        static Packets() 
        {
            try
            {
                Patience = new OnPatienceChange();
                Attack = new OnAttack();
                MissileHit = new OnMissileHit();
                Disengaged = new OnDisengaged();
                MonsterSkill = new OnMonsterSkill();
                CreateGromp = new OnCreateGromp();
                CreateCampIcon = new OnCreateCampIcon();
            }
            catch (Exception)
            {
                //ignored
            }
        }

        public class OnPatienceChange
        {
            public OnPatienceChange(int header = 0, int length = 17, int length2 = 95, int length3 = 20)
            {
                Length = length;
                Length = length2;
                Length = length3;
                Header = header;
            }
            public int Header { get; set; }
            public int Length { get; set; }
            public int Length2 { get; set; }
            public int Length3 { get; set; }
        }

        public class OnAttack
        {
            public OnAttack(int header = 0, int length = 71, int length2 = 50, int length3 = 35, int length4 = 281)
            {
                Length = length;
                Header = header;
                Length2 = length2;
                Length3 = length3;
                Length3 = length3;
                Length4 = length4;
                //Length5 = length5;
            }
            public int Header { get; set; }
            public int Length { get; set; }
            public int Length2 { get; set; }
            public int Length3 { get; set; }
            public int Length4 { get; set; }
            
        }

        public class OnMissileHit
        {
            public OnMissileHit(int header = 0, int length = 35)
            {
                Length = length;
                Header = header;
            }
            public int Header { get; set; }
            public int Length { get; set; }
        }

        public class OnDisengaged
        {
            public OnDisengaged(int header = 0, int length = 38, int length2 = 56, int length3 = 35)
            {
                Header = header;
                Length = length;            
                Length2 = length2;
                Length3 = length3;
            }
            public int Header { get; set; }
            public int Length { get; set; }
            public int Length2 { get; set; }
            public int Length3 { get; set; }
        }

        public class OnMonsterSkill
        {
            public OnMonsterSkill(int header = 0, int length = 47, int length2 = 68, int length3 = 17, int length4 = 59, int length5 = 83)
            {
                Length = length;
                Length2 = length2;
                Length3 = length3;
                Length4 = length4;
                Length5 = length5;
                Header = header;
            }
            public int Header { get; set; }
            public int Length { get; set; }
            public int Length2 { get; set; }
            public int Length3 { get; set; }
            public int Length4 { get; set; }
            public int Length5 { get; set; }
        }


            public class OnCreateGromp
            {
                public OnCreateGromp(int header = 0, int length = 302, int length2 = 311, int length3 = 17, int length4 = 95)
                {
                    Length = length;
                    Length2 = length2;
                    Length3 = length3;
                    Length4 = length4;
                
                    Header = header;
                }
                public int Header { get; set; }
                public int Length { get; set; }
                public int Length2 { get; set; }
                public int Length3 { get; set; }
                public int Length4 { get; set; }

        }

        public class OnCreateCampIcon
        {
            public OnCreateCampIcon(int header = 0, int length = 74, int length2 = 86, int length3 = 83, int length4 = 62, int length5 = 71, int length6 = 59, int length7 = 167)
            {
                Length = length;
                Length2 = length2;
                Length3 = length3;
                Length4 = length4;
                Length5 = length5;
                Length6 = length6;
                Length7 = length7;
                Header = header;
            }
            public int Header { get; set; }
            public int Length { get; set; }
            public int Length2 { get; set; }
            public int Length3 { get; set; }
            public int Length4 { get; set; }
            public int Length5 { get; set; }
            public int Length6 { get; set; }
            public int Length7 { get; set; }
        }
    }
}
