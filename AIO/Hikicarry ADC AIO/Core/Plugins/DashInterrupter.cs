using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HikiCarry.Core.Plugins
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal class DashInterrupter
    {
        public static List<InterruptableDashSpell> Spells = new List<InterruptableDashSpell>();
        public static Spell BlockSpell { get; set; }
        public enum DetectionType
        {
            Blink,
            Dash
        }
        public static int Range { get; set; }

        public struct InterruptableDashSpell
        {
            public string championname { get; set; }
            public string spellname { get; set; }
            public int spellrange { get; set; }
            public int spelldelay { get; set; }
            public int speed { get; set; }
            public SpellSlot slot { get; set; }
            public DetectionType dtype { get; set; }
        }

        private static void Initialize()
        {
            Spells.Add(
                new InterruptableDashSpell
                {
                   championname = "Ahri",
                   spellname = "AhriTumble",
                   spellrange = 500,
                   spelldelay = 50,
                   speed = 1575,
                   slot = SpellSlot.R,
                   dtype = DetectionType.Dash,
                });

            Spells.Add(
                new InterruptableDashSpell
                {
                    championname = "Caitlyn",
                    spellname = "CaitlynEntrapment",
                    spellrange = 490,
                    spelldelay = 50,
                    speed = 1000,
                    slot = SpellSlot.E,
                    dtype = DetectionType.Dash,
                });

            Spells.Add(
                new InterruptableDashSpell
                {
                    championname = "Corki",
                    spellname = "CarpetBomb",
                    spellrange = 790,
                    spelldelay = 50,
                    speed = 975,
                    slot = SpellSlot.W,
                    dtype = DetectionType.Dash,
                });

            Spells.Add(
                new InterruptableDashSpell
                {
                    championname = "Fiora",
                    spellname = "FioraQ",
                    spellrange = 400,
                    spelldelay = 50,
                    speed = 1100,
                    slot = SpellSlot.Q,
                    dtype = DetectionType.Dash,
                });

            Spells.Add(
                new InterruptableDashSpell
                {
                    championname = "Gragas",
                    spellname = "GragasBodySlam",
                    spellrange = 600,
                    spelldelay = 50,
                    speed = 900,
                    slot = SpellSlot.E,
                    dtype = DetectionType.Dash,
                });

            Spells.Add(
                new InterruptableDashSpell
                {
                    championname = "Gnar",
                    spellname = "gnarbige",
                    spellrange = 475,
                    spelldelay = 50,
                    speed = 800,
                    slot = SpellSlot.E,
                    dtype = DetectionType.Dash,
                });

            Spells.Add(
                new InterruptableDashSpell
                {
                    championname = "Graves",
                    spellname = "GravesMove",
                    spellrange = 425,
                    spelldelay = 50,
                    speed = 1250,
                    slot = SpellSlot.E,
                    dtype = DetectionType.Dash,
                });

            Spells.Add(
               new InterruptableDashSpell
               {
                   championname = "Kindred",
                   spellname = "KindredQ",
                   spellrange = 300,
                   spelldelay = 50,
                   speed = 773,
                   slot = SpellSlot.Q,
                   dtype = DetectionType.Dash,
               });

            Spells.Add(
               new InterruptableDashSpell
               {
                   championname = "Leblanc",
                   spellname = "LeblancSlide",
                   spellrange = 600,
                   spelldelay = 50,
                   speed = 1600,
                   slot = SpellSlot.W,
                   dtype = DetectionType.Dash,
               });

            Spells.Add(
               new InterruptableDashSpell
               {
                   championname = "Leblanc",
                   spellname = "LeblancSlideM",
                   spellrange = 600,
                   spelldelay = 50,
                   speed = 1600,
                   slot = SpellSlot.R,
                   dtype = DetectionType.Dash,
               });

            Spells.Add(
               new InterruptableDashSpell
               {
                   championname = "LeeSin",
                   spellname = "BlindMonkWOne",
                   spellrange = 700,
                   spelldelay = 50,
                   speed = 1400,
                   slot = SpellSlot.W,
                   dtype = DetectionType.Dash,
               });

            Spells.Add(
              new InterruptableDashSpell
              {
                  championname = "Lucian",
                  spellname = "LucianE",
                  spellrange = 425,
                  spelldelay = 50,
                  speed = 1350,
                  slot = SpellSlot.E,
                  dtype = DetectionType.Dash,
              });


            Spells.Add(
              new InterruptableDashSpell
              {
                  championname = "Nidalee",
                  spellname = "Pounce",
                  spellrange = 375,
                  spelldelay = 150,
                  speed = 1750,
                  slot = SpellSlot.W,
                  dtype = DetectionType.Dash,
              });

            Spells.Add(
              new InterruptableDashSpell
              {
                  championname = "Riven",
                  spellname = "RivenTriCleave",
                  spellrange = 260,
                  spelldelay = 50,
                  speed = 560,
                  slot = SpellSlot.Q,
                  dtype = DetectionType.Dash,
              });

            Spells.Add(
             new InterruptableDashSpell
             {
                 championname = "Riven",
                 spellname = "RivenFeint",
                 spellrange = 325,
                 spelldelay = 50,
                 speed = 1200,
                 slot = SpellSlot.E,
                 dtype = DetectionType.Dash,
             });

            Spells.Add(
             new InterruptableDashSpell
             {
                 championname = "Tristana",
                 spellname = "RocketJump",
                 spellrange = 900,
                 spelldelay = 500,
                 speed = 1100,
                 slot = SpellSlot.W,
                 dtype = DetectionType.Dash,
             });

            Spells.Add(
             new InterruptableDashSpell
             {
                 championname = "Tryndamare",
                 spellname = "Slash",
                 spellrange = 660,
                 spelldelay = 50,
                 speed = 900,
                 slot = SpellSlot.E,
                 dtype = DetectionType.Dash,
             });

            Spells.Add(
             new InterruptableDashSpell
             {
                 championname = "Vayne",
                 spellname = "VayneTumble",
                 spellrange = 300,
                 spelldelay = 50,
                 speed = 900,
                 slot = SpellSlot.Q,
                 dtype = DetectionType.Dash,
             });

            Spells.Add(
             new InterruptableDashSpell
             {
                 championname = "Yasuo",
                 spellname = "YasuoDashWrapper",
                 spellrange = 475,
                 spelldelay = 50,
                 speed = 1000,
                 slot = SpellSlot.E,
                 dtype = DetectionType.Dash,
             });

            Spells.Add(
             new InterruptableDashSpell
             {
                 championname = "Ezreal",
                 spellname = "EzrealArcaneShift",
                 spellrange = 450,
                 spelldelay = 250,
                 slot = SpellSlot.E,
                 dtype = DetectionType.Blink,
             });

            Spells.Add(
             new InterruptableDashSpell
             {
                 championname = "Shaco",
                 spellname = "Deceive",
                 spellrange = 400,
                 spelldelay = 250,
                 slot = SpellSlot.Q,
                 dtype = DetectionType.Blink,
             });

            Spells.Add(
             new InterruptableDashSpell
             {
                 championname = "AllChampions",
                 spellname = "SummonerFlash",
                 spellrange = 400,
                 spelldelay = 50,
                 slot = SpellSlot.Unknown,
                 dtype = DetectionType.Blink,
             });
        }

        public DashInterrupter(Spell blockspell, int range)
        {
            BlockSpell = blockspell;
            Range = range;
            Initialize();
            Chat.Print("Dash Interrupter Loaded");
            Obj_AI_Base.OnProcessSpellCast += DashProcessSpellCast;
        }

        private void DashProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsEnemy && sender is AIHeroClient && !args.SData.IsAutoAttack() && Utilities.Utilities.Enabled("dash.block")
                && BlockSpell.IsReady() && sender.IsValidTarget(Range))
            {
                foreach (var d in Spells.Where(x=> x.championname == sender.CharData.BaseSkinName
                && Initializer.Config.Item("dash." + args.SData.Name, true).GetValue<bool>() && 
                Initializer.Config.Item("dash." + args.SData.Name, true) != null))
                {
                    var senderdashinfo = sender.GetDashInfo();
                    var arrivetime = sender.Position.Distance((Vector3) senderdashinfo.EndPos)/
                                     d.speed;
                    var spelltime = ObjectManager.Player.Position.Distance((Vector3)senderdashinfo.EndPos) /
                                    BlockSpell.Speed + BlockSpell.Delay;

                    if (arrivetime < spelltime)
                    {
                        BlockSpell.Cast(senderdashinfo.EndPos);
                    }
                }
            }
        }
    }
}
