namespace PandaTeemo
{
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;
    using EloBuddy;

    // Original Code Credit: UC2
    // To add a shroom location, go to LeagueSharp's AppData folder LSXXXXXXXX/PandaTeemo/Map Name/(xFile.txt, yFile.txt, zFile.txt) and modify the values to suite your needs.
    // This code has been modified by KarmaPanda

    /// <summary>
    /// Shroom Locations
    /// </summary>
    internal class ShroomTables
    { 
        /// <summary>
        /// List of Locations in Summoner's Rift
        /// </summary>
        public List<Vector3> SummonersRift = new List<Vector3>();

        /// <summary>
        /// List of Locations in Howling Abyss
        /// </summary>
        public List<Vector3> HowlingAbyss = new List<Vector3>();

        /// <summary>
        /// List of Locations in Butcher's Bridge
        /// </summary>
        public List<Vector3> ButcherBridge = new List<Vector3>();

        /// <summary>
        /// List of Location in Crystal Scar
        /// </summary>
        public List<Vector3> CrystalScar = new List<Vector3>();

        /// <summary>
        /// List of Location in Twisted Treeline
        /// </summary>
        public List<Vector3> TwistedTreeline = new List<Vector3>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ShroomTables"/> class.
        /// </summary>
        public ShroomTables()
        {
            this.CreateTables();

            var list = (from pos in this.SummonersRift
                        let x = pos.X
                        let y = pos.Y
                        let z = pos.Z
                        select new Vector3(x, z, y)).ToList();
            this.SummonersRift = list;

            list = (from pos in this.HowlingAbyss
                        let x = pos.X
                        let y = pos.Y
                        let z = pos.Z
                        select new Vector3(x, z, y)).ToList();
            this.HowlingAbyss = list;

            list = (from pos in this.CrystalScar
                        let x = pos.X
                        let y = pos.Y
                        let z = pos.Z
                        select new Vector3(x, z, y)).ToList();
            this.CrystalScar = list;

            list = (from pos in this.TwistedTreeline
                        let x = pos.X
                        let y = pos.Y
                        let z = pos.Z
                        select new Vector3(x, z, y)).ToList();
            this.TwistedTreeline = list;

            list = (from pos in this.ButcherBridge
                    let x = pos.X
                    let y = pos.Y
                    let z = pos.Z
                    select new Vector3(x, z, y)).ToList();
            this.ButcherBridge = list;
        }

        /// <summary>
        /// Creates a List of Shroom Locations around the map.
        /// </summary>
        private void CreateTables()
        {
            if (LeagueSharp.Common.Utility.Map.GetMap().Type == LeagueSharp.Common.Utility.Map.MapType.SummonersRift)
            {
                // Custom List
                if (FileHandler.Position.Any() &&
                    Essentials.Config.SubMenu("Misc").Item("customLocation").GetValue<bool>())
                {
                    foreach (var pos in FileHandler.Position)
                    {
                        this.SummonersRift.Add(pos);
                    }
                }

                // Summoner's Rift

                // Top Lane Blue Side including Baron
                this.SummonersRift.Add(new Vector3(2790f, 50.16358f, 7278f));
                this.SummonersRift.Add(new Vector3(3700.708f, -11.22648f, 9294.094f));
                this.SummonersRift.Add(new Vector3(2314f, 53.165f, 9722f));
                this.SummonersRift.Add(new Vector3(3090f, -68.03732f, 10810f));
                this.SummonersRift.Add(new Vector3(4722f, -71.2406f, 10010f));
                this.SummonersRift.Add(new Vector3(5208f, -71.2406f, 9114f));
                this.SummonersRift.Add(new Vector3(4724f, 52.53909f, 7590f));
                this.SummonersRift.Add(new Vector3(4564f, 51.83786f, 6060f));
                this.SummonersRift.Add(new Vector3(2760f, 52.96445f, 5178f));
                this.SummonersRift.Add(new Vector3(4440f, 56.8484f, 11840f));

                // Top Lane Tri Bush
                this.SummonersRift.Add(new Vector3(2420f, 52.8381f, 13482f));
                this.SummonersRift.Add(new Vector3(1630f, 52.8381f, 13008f));
                this.SummonersRift.Add(new Vector3(1172f, 52.8381f, 12302f));

                // Top Lane Red Side
                this.SummonersRift.Add(new Vector3(5666f, 52.8381f, 12722f));
                this.SummonersRift.Add(new Vector3(8004f, 56.4768f, 11782f));
                this.SummonersRift.Add(new Vector3(9194f, 53.35013f, 11368f));
                this.SummonersRift.Add(new Vector3(8280f, 50.06194f, 10254f));
                this.SummonersRift.Add(new Vector3(6728f, 53.82967f, 11450f));
                this.SummonersRift.Add(new Vector3(6242f, 54.09851f, 10270f));

                // Mid Lane
                this.SummonersRift.Add(new Vector3(6484f, -71.2406f, 8380f));
                this.SummonersRift.Add(new Vector3(8380f, -71.2406f, 6502f));
                this.SummonersRift.Add(new Vector3(9099.75f, 52.95337f, 7376.637f));
                this.SummonersRift.Add(new Vector3(7376f, 52.8726f, 8802f));
                this.SummonersRift.Add(new Vector3(7602f, 52.56985f, 5928f));

                // Dragon
                this.SummonersRift.Add(new Vector3(9372f, -71.2406f, 5674f));
                this.SummonersRift.Add(new Vector3(10148f, -71.2406f, 4801.525f));

                // Bot Lane Red Side
                this.SummonersRift.Add(new Vector3(9772f, 9.031885f, 6458f));
                this.SummonersRift.Add(new Vector3(9938f, 51.62378f, 7900f));
                this.SummonersRift.Add(new Vector3(11465f, 51.72557f, 7157.772f));
                this.SummonersRift.Add(new Vector3(12481f, 51.7294f, 5232.559f));
                this.SummonersRift.Add(new Vector3(11266f, -7.897567f, 5542f));
                this.SummonersRift.Add(new Vector3(11290f, 64.39886f, 8694f));
                this.SummonersRift.Add(new Vector3(12676f, 51.6851f, 7310.818f));
                this.SummonersRift.Add(new Vector3(12022f, 9154f, 51.25105f));


                // Bot Lane Blue Side (Bushes only)
                this.SummonersRift.Add(new Vector3(6544f, 48.257f, 4732f));
                this.SummonersRift.Add(new Vector3(5576f, 51.42581f, 3512f));
                this.SummonersRift.Add(new Vector3(6888f, 51.94016f, 3082f));
                this.SummonersRift.Add(new Vector3(8070f, 51.5508f, 3472f));
                this.SummonersRift.Add(new Vector3(8594f, 51.73177f, 4668f));
                this.SummonersRift.Add(new Vector3(10388f, 49.81641f, 3046f));
                this.SummonersRift.Add(new Vector3(9160f, 59.97022f, 2122f));

                // Bot Lane Tri Bush
                this.SummonersRift.Add(new Vector3(12518f, 53.66707f, 1504f));
                this.SummonersRift.Add(new Vector3(13404f, 51.3669f, 2482f));
                this.SummonersRift.Add(new Vector3(11854f, -68.06037f, 3922f));
            }
            else if (LeagueSharp.Common.Utility.Map.GetMap().Type == LeagueSharp.Common.Utility.Map.MapType.HowlingAbyss)
            {
                // Custom List
                if (FileHandler.Position.Any() &&
                    Essentials.Config.SubMenu("Misc").Item("customLocation").GetValue<bool>())
                {
                    foreach (var pos in FileHandler.Position)
                    {
                        var howlingAbyss = this.HowlingAbyss;
                        if (howlingAbyss != null)
                        {
                            howlingAbyss.Add(pos);
                        }
                    }
                }

                // Howling Abyss

                // Blue Side
                this.HowlingAbyss.Add(new Vector3(4787f, -178.3095f, 3909f));
                this.HowlingAbyss.Add(new Vector3(4107f, -178.3095f, 5175f));
                this.HowlingAbyss.Add(new Vector3(5927f, -178.3095f, 5197f));
                this.HowlingAbyss.Add(new Vector3(5633f, -178.3095f, 6390f));
                
                // Red Side
                this.HowlingAbyss.Add(new Vector3(8921f, -178.3095f, 7873f));
                this.HowlingAbyss.Add(new Vector3(7555f, -178.3095f, 6755f));
                this.HowlingAbyss.Add(new Vector3(7677f, -178.3095f, 8643f));
                this.HowlingAbyss.Add(new Vector3(6487f, -178.3095f, 7145f));
            }
            else if (LeagueSharp.Common.Utility.Map.GetMap().Type == LeagueSharp.Common.Utility.Map.MapType.CrystalScar)
            {
                // Custom List
                if (FileHandler.Position.Any() &&
                    Essentials.Config.SubMenu("Misc").Item("customLocation").GetValue<bool>())
                {
                    foreach (var pos in FileHandler.Position)
                    {
                        this.CrystalScar.Add(pos);
                    }
                }

                Notifications.AddNotification("Shrooms Loaded", 10000, true);
            }
            else if (LeagueSharp.Common.Utility.Map.GetMap().Type == LeagueSharp.Common.Utility.Map.MapType.TwistedTreeline)
            {
                // Custom List
                if (FileHandler.Position.Any() &&
                    Essentials.Config.SubMenu("Misc").Item("customLocation").GetValue<bool>())
                {
                    foreach (var pos in FileHandler.Position)
                    {
                        this.TwistedTreeline.Add(pos);
                    }
                }
            }
            else if (LeagueSharp.Common.Utility.Map.GetMap().Type.ToString() == "Unknown")
            {
                // Custom List
                if (FileHandler.Position.Any() &&
                    Essentials.Config.SubMenu("Misc").Item("customLocation").GetValue<bool>())
                {
                    foreach (var pos in FileHandler.Position)
                    {
                        this.ButcherBridge.Add(pos);
                    }
                }

                // Blue Side
                this.ButcherBridge.Add(new Vector3(5518f, -169.1383f, 6259f));
                this.ButcherBridge.Add(new Vector3(4036f, -169.1383f, 5067f));

                // Blue Side Relics
                this.ButcherBridge.Add(new Vector3(5928.342f, -169.1383f, 5193.888f));
                this.ButcherBridge.Add(new Vector3(4806f, -169.1383f, 3925.413f));

                // Red Side
                this.ButcherBridge.Add(new Vector3(6484f, -169.1383f, 7023.106f));
                this.ButcherBridge.Add(new Vector3(7743.811f, -169.1383f, 8605.378f));

                // Red Side Relics
                this.ButcherBridge.Add(new Vector3(8904f, -169.1383f, 7873f));
                this.ButcherBridge.Add(new Vector3(7558f, -169.1383f, 6790f));
            }
            else
            {
                Chat.Print("Please Lower/Increase the amount of Custom Shroom Locations for this assembly to work.");
            }
        }
    }
}
