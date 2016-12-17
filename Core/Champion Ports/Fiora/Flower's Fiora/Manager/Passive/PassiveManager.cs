using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Fiora.Manager.Passive
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;

    internal static class PassiveManager
    {
        private static readonly List<PassiveList> PassiveList = new List<PassiveList>();
        private static readonly List<string> PassiveName = new List<string>()
        {
            // 人物左边
            "Fiora_Base_Passive_SE_Warning.troy",
            "Fiora_Base_Passive_SE.troy",
            "Fiora_Base_Passive_SE_Timeout.troy",
            "Fiora_Base_R_Mark_SE_FioraOnly.troy",
            "Fiora_Base_R_SE_Timeout_FioraOnly.troy",

            // 人物北边
            "Fiora_Base_Passive_NE_Warning.troy",
            "Fiora_Base_Passive_NE.troy",
            "Fiora_Base_Passive_NE_Timeout.troy",
            "Fiora_Base_R_Mark_NE_FioraOnly.troy",
            "Fiora_Base_R_NE_Timeout_FioraOnly.troy",

            // 人物南边
            "Fiora_Base_Passive_SW_Warning.troy",
            "Fiora_Base_Passive_SW.troy",
            "Fiora_Base_Passive_SW_Timeout.troy",
            "Fiora_Base_R_Mark_SW_FioraOnly.troy",
            "Fiora_Base_R_SW_Timeout_FioraOnly.troy",

            // 人物右边
            "Fiora_Base_Passive_NW_Warning.troy",
            "Fiora_Base_Passive_NW.troy",
            "Fiora_Base_Passive_NW_Timeout.troy",
            "Fiora_Base_R_Mark_NW_FioraOnly.troy",
            "Fiora_Base_R_NW_Timeout_FioraOnly.troy"
        };

        public static bool IsPassive(this Obj_GeneralParticleEmitter emitter)
        {
            if (emitter == null)
            {
                return false;
            }

            if (!emitter.IsValid)
            {
                return false;
            }

            return PassiveName.Contains(emitter.Name);
        }

        public static int PassiveCount(AIHeroClient target)
        {
            var allPassive =
                PassiveList.Where(
                    x => x.Passive != null && x.Passive.IsValid && x.Passive.Position.Distance(target.Position) <= 50);

            return allPassive.Count();
        }

        public static List<Vector3> GetPassivePosList(AIHeroClient target)
        {
            var positionList = new List<Vector3>();

            var allPassive =
                PassiveList.Where(
                    x => x.Passive != null && x.Passive.IsValid && x.Passive.Position.Distance(target.Position) <= 50);

            var position = target.ServerPosition;

            foreach (var x in allPassive)
            {
                if (x.PassiveType == PassiveType.Ult)
                {
                    switch (x.Direction)
                    {
                        case PassiveDirection.NE:
                        {
                            var pos = new Vector2
                            {
                                X = position.X,
                                Y = position.Y + 200
                            };

                            positionList.Add(pos.To3D());
                        }
                            break;
                        case PassiveDirection.NW:
                        {
                            var pos = new Vector2
                            {
                                X = position.X + 200,
                                Y = position.Y
                            };

                            positionList.Add(pos.To3D());
                        }
                            break;
                        case PassiveDirection.SE:
                        {
                            var pos = new Vector2
                            {
                                X = position.X - 200,
                                Y = position.Y
                            };

                            positionList.Add(pos.To3D());
                        }
                            break;
                        case PassiveDirection.SW:
                        {
                            var pos = new Vector2
                            {
                                X = position.X,
                                Y = position.Y - 200
                            };

                            positionList.Add(pos.To3D());
                        }
                            break;
                    }
                }
                else
                {
                    switch (x.Direction)
                    {
                        case PassiveDirection.NE:
                            {
                                var pos = new Vector2
                                {
                                    X = position.X,
                                    Y = position.Y + 100
                                };

                                positionList.Add(pos.To3D());
                            }
                            break;
                        case PassiveDirection.NW:
                            {
                                var pos = new Vector2
                                {
                                    X = position.X + 100,
                                    Y = position.Y
                                };

                                positionList.Add(pos.To3D());
                            }
                            break;
                        case PassiveDirection.SE:
                            {
                                var pos = new Vector2
                                {
                                    X = position.X - 100,
                                    Y = position.Y
                                };

                                positionList.Add(pos.To3D());
                            }
                            break;
                        case PassiveDirection.SW:
                            {
                                var pos = new Vector2
                                {
                                    X = position.X,
                                    Y = position.Y - 100
                                };

                                positionList.Add(pos.To3D());
                            }
                            break;
                    }
                }
            }

            return positionList;
        }

        public static Vector3 CastQPosition(AIHeroClient target)
        {
            return GetPassivePosList(target)
                        .Where(x => x.Distance(ObjectManager.Player.Position) > 100 && x.Distance(target.Position) > 50)
                        .OrderBy(x => x.Distance(target.Position))
                        .ThenByDescending(x => x.Distance(ObjectManager.Player.Position))
                        .FirstOrDefault();
        }

        public static Vector3 OrbwalkerPosition(AIHeroClient target)
        {
            var pos = GetPassivePosList(target)
                .Where(x => x.Distance(ObjectManager.Player.Position) > 100 && x.Distance(target.Position) > 50)
                .OrderBy(x => x.Distance(target.Position))
                .ThenByDescending(x => x.Distance(ObjectManager.Player.Position))
                .FirstOrDefault();

            if ((!Logic.Q.IsReady() || (Logic.Q.IsReady() && target.Distance(ObjectManager.Player) <= 200)) &&
                ObjectManager.Player.Distance(pos) <= ObjectManager.Player.BoundingRadius + target.BoundingRadius + 80)
            {
                return target.ServerPosition.Extend(pos, 180);
            }

            return Game.CursorPos;
        }

        public static void Init()
        {
            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate(EventArgs Args)
        {
            PassiveList.Clear();

            var emitterList = ObjectManager.Get<Obj_GeneralParticleEmitter>().Where(IsPassive);

            foreach (var passive in emitterList)
            {
                if (passive.Name.Contains("_NE"))
                {
                    if (passive.Name.Contains("Fiora_Base_R") && !passive.Name.Contains("Timeout"))
                    {
                        PassiveList.Add(new PassiveList(passive, PassiveType.Ult, PassiveDirection.NE));
                    }
                    else if (passive.Name.Contains("Warning"))
                    {
                        PassiveList.Add(new PassiveList(passive, PassiveType.Prepassive, PassiveDirection.NE));
                    }
                    else if (passive.Name.Contains("Fiora_Base_Passive") && !passive.Name.Contains("Timeout"))
                    {
                        PassiveList.Add(new PassiveList(passive, PassiveType.Passive, PassiveDirection.NE));
                    }
                    else if (passive.Name.Contains("Timeout"))
                    {
                        PassiveList.Add(new PassiveList(passive, PassiveType.TimeOut, PassiveDirection.NE));
                    }
                }

                if (passive.Name.Contains("_SE"))
                {
                    if (passive.Name.Contains("Fiora_Base_R") && !passive.Name.Contains("Timeout"))
                    {
                        PassiveList.Add(new PassiveList(passive, PassiveType.Ult, PassiveDirection.SE));
                    }
                    else if (passive.Name.Contains("Warning"))
                    {
                        PassiveList.Add(new PassiveList(passive, PassiveType.Prepassive, PassiveDirection.SE));
                    }
                    else if (passive.Name.Contains("Fiora_Base_Passive") && !passive.Name.Contains("Timeout"))
                    {
                        PassiveList.Add(new PassiveList(passive, PassiveType.Passive, PassiveDirection.SE));
                    }
                    else if (passive.Name.Contains("Timeout"))
                    {
                        PassiveList.Add(new PassiveList(passive, PassiveType.TimeOut, PassiveDirection.SE));
                    }
                }

                if (passive.Name.Contains("_SW"))
                {
                    if (passive.Name.Contains("Fiora_Base_R") && !passive.Name.Contains("Timeout"))
                    {
                        PassiveList.Add(new PassiveList(passive, PassiveType.Ult, PassiveDirection.SW));
                    }
                    else if (passive.Name.Contains("Warning"))
                    {
                        PassiveList.Add(new PassiveList(passive, PassiveType.Prepassive, PassiveDirection.SW));
                    }
                    else if (passive.Name.Contains("Fiora_Base_Passive") && !passive.Name.Contains("Timeout"))
                    {
                        PassiveList.Add(new PassiveList(passive, PassiveType.Passive, PassiveDirection.SW));
                    }
                    else if (passive.Name.Contains("Timeout"))
                    {
                        PassiveList.Add(new PassiveList(passive, PassiveType.TimeOut, PassiveDirection.SW));
                    }
                }

                if (passive.Name.Contains("_NW"))
                {
                    if (passive.Name.Contains("Fiora_Base_R") && !passive.Name.Contains("Timeout"))
                    {
                        PassiveList.Add(new PassiveList(passive, PassiveType.Ult, PassiveDirection.NW));
                    }
                    else if (passive.Name.Contains("Warning"))
                    {
                        PassiveList.Add(new PassiveList(passive, PassiveType.Prepassive, PassiveDirection.NW));
                    }
                    else if (passive.Name.Contains("Fiora_Base_Passive") && !passive.Name.Contains("Timeout"))
                    {
                        PassiveList.Add(new PassiveList(passive, PassiveType.Passive, PassiveDirection.NW));
                    }
                    else if (passive.Name.Contains("Timeout"))
                    {
                        PassiveList.Add(new PassiveList(passive, PassiveType.TimeOut, PassiveDirection.NW));
                    }
                }
            }
        }
    }
}
