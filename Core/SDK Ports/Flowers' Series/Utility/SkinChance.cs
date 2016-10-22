using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Flowers_Series.Utility
{
    using LeagueSharp;
    using LeagueSharp.SDK.UI;
    using System;
    using System.Collections.Generic;

    internal class SkinChance
    {
        private static AIHeroClient Me => Program.Me;
        private static Menu Menu => Tools.Menu;

        private static int SkinID;

        public static void Inject()
        {
            SkinID = Me.SkinId;

            var SkinMenu = Menu.Add(new Menu("SkinChance", "Skin Chance"));
            {
                SkinMenu.Add(new MenuBool("Enable", "Enabled", false));
                
                switch (Me.ChampionName)
                {
                    case "Ahri":
                        SkinMenu.Add(new MenuList<string>("SkinName", "Select Skin", Ahri));
                        break;
                    case "Akali":
                        SkinMenu.Add(new MenuList<string>("SkinName", "Select Skin", Akali));
                        break;
                    case "Ashe":
                        SkinMenu.Add(new MenuList<string>("SkinName", "Select Skin", Ashe));
                        break;
                    case "Blitzcrank":
                        SkinMenu.Add(new MenuList<string>("SkinName", "Select Skin", Blitzcrank));
                        break;
                    case "Darius":
                        SkinMenu.Add(new MenuList<string>("SkinName", "Select Skin", Darius));
                        break;
                    case "Ezreal":
                        SkinMenu.Add(new MenuList<string>("SkinName", "Select Skin", Ezreal));
                        break;
                    case "Graves":
                        SkinMenu.Add(new MenuList<string>("SkinName", "Select Skin", Graves));
                        break;
                    case "Hecarim":
                        SkinMenu.Add(new MenuList<string>("SkinName", "Select Skin", Hecarim));
                        break;
                    case "Illaoi":
                        SkinMenu.Add(new MenuList<string>("SkinName", "Select Skin", Illaoi));
                        break;
                    case "Karma":
                        SkinMenu.Add(new MenuList<string>("SkinName", "Select Skin", Karma));
                        break;
                    case "Riven":
                        SkinMenu.Add(new MenuList<string>("SkinName", "Select Skin", Riven));
                        break;
                    case "Sivir":
                        SkinMenu.Add(new MenuList<string>("SkinName", "Select Skin", Sivir));
                        break;
                    case "Tristana":
                        SkinMenu.Add(new MenuList<string>("SkinName", "Select Skin", Tristana));
                        break;
                    case "TwistedFate":
                        SkinMenu.Add(new MenuList<string>("SkinName", "Select Skin", TwistedFate));
                        break;
                    case "Twitch":
                        SkinMenu.Add(new MenuList<string>("SkinName", "Select Skin", Twitch));
                        break;
                    case "Vayne":
                        SkinMenu.Add(new MenuList<string>("SkinName", "Select Skin", Vayne));
                        break;
                    case "Viktor":
                        SkinMenu.Add(new MenuList<string>("SkinName", "Select Skin", Viktor));
                        break; 
                    case "Vladimir":
                        SkinMenu.Add(new MenuList<string>("SkinName", "Select Skin", Vladimir));
                        break;
                    default:
                        break;
                }
            }
                
            Common.Manager.WriteConsole("SkinMenu Load!");

            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead)
            {
                return;
            }

            if (Menu["SkinChance"]["Enable"])
            {
            }
            else if (!Menu["SkinChance"]["Enable"])
            {
            }
        }

        #region SkinNama
        
        private static IEnumerable<string> Aatrox = new[]
        {
            "Classic", "Justicar Aatrox", "Mecha Aatrox", "Sea Hunter Aatrox"
        };

        private static IEnumerable<string> Ahri = new[]
        {
            "Classic", "Dynasty Ahri", "Midnight Ahri", "Foxfire Ahri", "Popstar Ahri", "Challenger Ahri", "Academy Ahri"
        };

        private static IEnumerable<string> Akali = new[]
        {
            "Classic", "Stinger Akali", "Crimson Akali", "All-star Akali", "Nurse Akali", "Blood Moon Akali", "Silverfang Akali", "Headhunter Akali"
        };

        private static IEnumerable<string> Alistar = new[]
        {
            "Classic", "Black Alistar", "Golden Alistar", "Matador Alistar", "Longhorn Alistar", "Unchained Alistar", "Infernal Alistar", "Sweeper Alistar", "Marauder Alistar"
        };

        private static IEnumerable<string> Amumu = new[]
        {
            "Classic", "Pharaoh Amumu", "Vancouver Amumu", "Emumu", "Re-Gifted Amumu", "Almost-Prom King Amumu", "Little Knight Amumu", "Sad Robot Amumu", "Surprise Party Amumu"
        };

        private static IEnumerable<string> Anivia = new[]
        {
            "Classic", "Team Spirit Anivia", "Bird of Prey Anivia", "Noxus Hunter Anivia", "Hextech Anivia", "Blackfrost Anivia", "Prehistoric Anivia"
        };

        private static IEnumerable<string> Annie = new[]
        {
            "Classic", "Goth Annie", "Red Riding Annie", "Annie in Wonderland", "Prom Queen Annie", "Frostfire Annie", "Reverse Annie", "FrankenTibbers Annie", "Panda Annie", "Sweetheart Annie", "Hextech Annie"
        };

        private static IEnumerable<string> Ashe = new[]
        {
            "Classic", "Freljord Ashe", "Sherwood Forest Ashe", "Woad Ashe", "Queen Ashe", "Amethyst Ashe", "Heartseeker Ashe", "Marauder Ashe", "Project Ashe"
        };

        private static IEnumerable<string> AurelionSol = new[]
        {
            "Classic", "Ashen Lord Aurelion Sol"
        };

        private static IEnumerable<string> Azir = new[]
        {
            "Classic", "Galactic Azir", "Gravelord Azir"
        };

        private static IEnumerable<string> Bard = new[]
        {
            "Classic", "Elderwood Bard", "Snow Day Bard"
        };

        private static IEnumerable<string> Blitzcrank = new[]
        {
            "Classic", "Definitely Not", "Boom Boom", "Rusty", "Goalkeeper", "Piltover Customs", "iBlitzcrank", "Riot", "Battle Boss"
        };

        private static IEnumerable<string> Brand = new[]
        {
            "Classic", "Apocalyptic Brand", "Vandal Brand", "Cryocore Brand", "Zombie Brand", "Spirit Fire Brand"
        };

        private static IEnumerable<string> Braum = new[]
        {
            "Classic", "Dragonslayer Braum", "El Tigre Braum", "Braum Lionheart"
        };

        private static IEnumerable<string> Caitlyn = new[]
        {
            "Classic", "Resistance Caitlyn", "Sheriff Caitlyn", "Safari Caitlyn", "Arctic Warfare Caitlyn", "Officer Caitlyn", "Headhunter Caitlyn", "Lunar Wraith Caitlyn"
        };

        private static IEnumerable<string> Cassiopeia = new[]
        {
            "Classic", "Desperada Cassiopeia", "Siren Cassiopeia", "Mythic Cassiopeia", "Jade Fang Cassiopeia"
        };

        private static IEnumerable<string> ChoGath = new[]
        {
            "Classic", "Nightmare Cho'Gath", "Gentleman Cho'Gath", "Loch Ness Cho'Gath", "Jurassic Cho'Gath", "Battlecast Prime Cho'Gath", "Prehistoric Cho'Gath"
        };

        private static IEnumerable<string> Corki = new[]
        {
            "Classic", "UFO Corki", "Ice Toboggan Corki", "Red Baron Corki", "Hot Rod Corki", "Urfrider Corki", "Dragonwing Corki", "Fnatic Corki"
        };

        private static IEnumerable<string> Darius = new[]
        {
            "Classic", "Lord", "Bioforge", "Woad King", "Dunkmaster", "Academy"
        };

        private static IEnumerable<string> Diana = new[]
        {
            "Classic", "Dark Valkyrie Diana", "Lunar Goddess Diana", "Infernal Diana"
        };

		private static IEnumerable<string> DrMundo = new[]
        {
            "Classic", "Toxic Dr. Mundo", "Mr. Mundoverse", "Corporate Mundo", "Mundo Mundo", "Executioner Mundo", "Rageborn Mundo", "TPA Mundo", "Pool Party Mundo", "El Macho Mundo"
        };
		
		private static IEnumerable<string> Draven = new[]
        {
            "Classic", "Soul Reaver Draven", "Gladiator Draven", "Primetime Draven", "Pool Party Draven", "Beast Hunter Draven", "Draven Draven"
        };
		
		private static IEnumerable<string> Ekko = new[]
        {
            "Classic", "Sandstorm Ekko", "Academy Ekko", "Project Ekko"
		};
		
		private static IEnumerable<string> Elise = new[]
        {
            "Classic", "Death Blossom Elise", "Victorious Elise", "Blood Moon Elise"
		};
		
		private static IEnumerable<string> Evelynn = new[]
        {
            "Classic", "Shadow Evelynn", "Masquerade Evelynn", "Tango Evelynn", "Safecracker Evelynn"
		};
		
		private static IEnumerable<string> Ezreal = new[]
        {
            "Classic", "Nottingham Ezreal", "Striker Ezreal", "Frosted Ezreal", "Explorer Ezreal", "Pulsefire Ezreal", "TPA Ezreal", "Debonair Ezreal", "Ace of Spades Ezreal"
		};
		
		private static IEnumerable<string> Fiddlesticks = new[]
        {
            "Classic", "Spectral Fiddlesticks", "Union Jack Fiddlesticks", "Bandito Fiddlesticks", "Pumpkinhead Fiddlesticks", "Fiddle Me Timbers", "Surprise Party Fiddlesticks", "Dark Candy Fiddlesticks", "Risen Fiddlesticks"
		};
		
		private static IEnumerable<string> Fiora = new[]
        {
            "Classic", "Royal Guard Fiora", "Nightraven Fiora", "Headmistress Fiora", "PROJECT: Fiora"
		};
		
		private static IEnumerable<string> Fizz = new[]
        {
            "Classic", "Atlantean Fizz", "Tundra Fizz", "Fisherman Fizz", "Void Fizz", "Cottontail Fizz", "Super Galaxy Fizz"
		};
		
		private static IEnumerable<string> Galio = new[]
        {
            "Classic", "Enchanted Galio", "Hextech Galio", "Commando Galio", "Gatekeeper Galio", "Debonair Galio"
		};
		
		private static IEnumerable<string> Gangplank = new[]
        {
            "Classic", "Spooky Gangplank", "Minuteman Gangplank", "Sailor Gangplank", "Toy Soldier Gangplank", "Special Forces Gangplank", "Sultan Gangplank", "Captain Gangplank"
		};

		private static IEnumerable<string> Garen = new[]
        {
            "Classic", "Sanguine Garen", "Desert Trooper Garen", "Commando Garen", "Dreadknight Garen", "Rugged Garen", "Steel Legion Garen", "Rogue Admiral Garen"
		};

		private static IEnumerable<string> Gnar = new[]
        {
            "Classic", "Dino Gnar", "Gentleman Gnar", "Snow Day Gnar", "El León Gnar"
		};
		
		private static IEnumerable<string> Gragas = new[]
        {
            "Classic", "Scuba Gragas", "Hillbilly Gragas", "Santa Gragas", "Gragas, Esq.", "Vandal Gragas", "Oktoberfest Gragas", "Superfan Gragas", "Fnatic Gragas", "Gragas Caskbreaker"
		};
		
		private static IEnumerable<string> Graves = new[]
        {
            "Classic", "Hired Gun Graves", "Jailbreak Graves", "Mafia Graves", "Riot Graves", "Pool Party Graves", "Cutthroat Graves"
		};
		
		private static IEnumerable<string> Hecarim = new[]
        {
            "Classic", "Blood Knight Hecarim", "Reaper Hecarim", "Headless Hecarim", "Arcade Hecarim", "Elderwood Hecarim"
		};
		
		private static IEnumerable<string> Heimerdinger = new[]
        {
            "Classic", "Alien Invader Heimerdinger", "Blast Zone Heimerdinger", "Piltover Customs Heimerdinger", "Snowmerdinger", "Hazmat Heimerdinger"
		};
		
		private static IEnumerable<string> Illaoi = new[]
        {
            "Classic", "Void Bringer Illaoi"
		};
		
		private static IEnumerable<string> Irelia = new[]
        {
            "Classic", "Nightblade Irelia", "Aviator Irelia", "Infiltrator Irelia", "Frostblade Irelia", "Order of the Lotus Irelia"
		};
		
		private static IEnumerable<string> Janna = new[]
        {
            "Classic", "Tempest Janna", "Hextech Janna", "Frost Queen Janna", "Victorious Janna", "Forecast Janna", "Fnatic Janna"
		};
		
		private static IEnumerable<string> JarvanIV = new[]
        {
            "Classic", "Commando Jarvan IV", "Dragonslayer Jarvan IV", "Darkforge Jarvan IV", "Victorious Jarvan IV", "Warring Kingdoms Jarvan IV", "Fnatic Jarvan IV"
		};
		
		private static IEnumerable<string> Jax = new[]
        {
            "Classic", "The Mighty Jax", "Vandal Jax", "Angler Jax", "PAX Jax", "Jaximus", "Temple Jax", "Nemesis Jax", "SKT T1 Jax", "Warden Jax"
		};
		
		private static IEnumerable<string> Jayce = new[]
        {
            "Classic", "Full Metal Jayce", "Debonair Jayce", "Forsaken Jayce"
		};
		
		private static IEnumerable<string> Jhin = new[]
        {
            "Classic", "High Noon Jhin"
		};
		
		private static IEnumerable<string> Jinx = new[]
        {
            "Classic", "Mafia Jinx", "Firecracker Jinx", "Slayer Jinx"
		};
		
		private static IEnumerable<string> Kalista = new[]
        {
            "Classic", "Blood Moon Kalista", "Championship Kalista"
		};
		
		private static IEnumerable<string> Karma = new[]
        {
            "Classic", "Sun Goddess Karma", "Sakura Karma", "Traditional Karma", "Order of the Lotus Karma", "Warden Karma"
		};
		
		private static IEnumerable<string> Karthus = new[]
        {
			"Classic", "Phantom Karthus", "Statue of Karthus", "Grim Reaper Karthus", "Pentakill Karthus", "Fnatic Karthus"
		};
		
		private static IEnumerable<string> Kassadin = new[]
        {
			"Classic", "Festival Kassadin", "Deep One Kassadin", "Pre-Void Kassadin", "Harbinger Kassadin", "Cosmic Reaver Kassadin"
		};

		private static IEnumerable<string> Katarina = new[]
        {
			"Classic", "Mercenary Katarina", "Red Card Katarina", "Bilgewater Katarina", "Kitty Cat Katarina", "High Command Katarina", "Sandstorm Katarina", "Slay Belle Katarina", "Warring Kingdoms Katarina", "Project Katarina"
		};
		
		private static IEnumerable<string> Kayle = new[]
        {
			"Classic", "Silver Kayle", "Viridian Kayle", "Unmasked Kayle", "Battleborn Kayle", "Judgment Kayle", "Aether Wing Kayle", "Riot Kayle", "Iron Inquisitor Kayle"
		};
		
		private static IEnumerable<string> Kennen = new[]
        {
			"Classic", "Deadly Kennen", "Swamp Master Kennen", "Karate Kennen", "Kennen M.D.", "Arctic Ops Kennen", "Blood Moon Kennen"
		};
		
		private static IEnumerable<string> KhaZix = new[]
        {
			"Classic", "Mecha Kha'Zix", "Guardian of the Sands Kha'Zix", "Death Blossom Kha'Zix"
		};
		
		private static IEnumerable<string> Kindred = new[]
        {
			"Classic", "Shadowfire Kindred", "Super Galaxy Kindred"
		};

        private static IEnumerable<string> Kled = new[]
        {
            "Classic", "Sir Kled"
        };

        private static IEnumerable<string> KogMaw = new[]
        {
			"Classic", "Caterpillar Kog'Maw", "Sonoran Kog'Maw", "Monarch Kog'Maw", "Reindeer Kog'Maw", "Lion Dance Kog'Maw", "Deep Sea Kog'Maw", "Jurassic Kog'Maw", "Battlecast Kog'Maw"
		};
		
		private static IEnumerable<string> LeBlanc = new[]
        {
			"Classic", "Wicked LeBlanc", "Prestigious LeBlanc", "Mistletoe LeBlanc", "Ravenborn LeBlanc", "Elderwood LeBlanc"
		};
		
		private static IEnumerable<string> LeeSin = new[]
        {
			"Classic", "Traditional Lee Sin", "Acolyte Lee Sin", "Dragon Fist Lee Sin", "Muay Thai Lee Sin", "Pool Party Lee Sin", "SKT T1 Lee Sin", "Knockout Lee Sin"
		};
		
		private static IEnumerable<string> Leona = new[]
        {
			"Classic", "Valkyrie Leona", "Defender Leona", "Iron Solari Leona", "Pool Party Leona", "PROJECT: Leona"
		};
		
		private static IEnumerable<string> Lissandra = new[]
        {
			"Classic", "Bloodstone Lissandra", "Blade Queen Lissandra", "Program Lissandra"
		};
		
		private static IEnumerable<string> Lucian = new[]
        {
			"Classic", "Hired Gun Lucian", "Striker Lucian", "PROJECT: Lucian"
		};
		
		private static IEnumerable<string> Lulu = new[]
        {
			"Classic", "Bittersweet Lulu", "Wicked Lulu", "Dragon Trainer Lulu", "Winter Wonder Lulu", "Pool Party Lulu"
		};
		
		private static IEnumerable<string> Lux = new[]
        {
			"Classic", "Sorceress Lux", "Spellthief Lux", "Commando Lux", "Imperial Lux", "Steel Legion Lux", "Star Guardian Lux"
		};
		
		private static IEnumerable<string> Malphite = new[]
        {
			"Classic", "Shamrock Malphite", "Coral Reef Malphite", "Marble Malphite", "Obsidian Malphite", "Glacial Malphite", "Mecha Malphite", "Ironside Malphite"
		};
		
		private static IEnumerable<string> Malzahar = new[]
        {
			"Classic", "Vizier Malzahar", "Shadow Prince Malzahar", "Djinn Malzahar", "Overlord Malzahar", "Snow Day Malzahar"
		};
		
		private static IEnumerable<string> Maokai = new[]
        {
			"Classic", "Charred Maokai", "Totemic Maokai", "Festive Maokai", "Haunted Maokai", "Goalkeeper Maokai", "Meowkai"
		};
		
		private static IEnumerable<string> MasterYi = new[]
        {
			"Classic", "Assassin Master Yi", "Chosen Master Yi", "Ionia Master Yi", "Samurai Yi", "Headhunter Master Yi", "PROJECT: Yi"
		};
		
		private static IEnumerable<string> MissFortune = new[]
        {
			"Classic", "Cowgirl Miss Fortune", "Waterloo Miss Fortune", "Secret Agent Miss Fortune", "Candy Cane Miss Fortune", "Road Warrior Miss Fortune", "Mafia Miss Fortune", "Arcade Miss Fortune", "Captain Fortune"
		};
		
		private static IEnumerable<string> Mordekaiser = new[]
        {
			"Classic", "Dragon Knight Mordekaiser", "Infernal Mordekaiser", "Pentakill Mordekaiser", "Lord Mordekaiser", "King of Clubs Mordekaiser"
		};
		
		private static IEnumerable<string> Morgana = new[]
        {
			"Classic", "Exiled Morgana", "Sinful Succulence Morgana", "Blade Mistress Morgana", "Blackthorn Morgana", "Ghost Bride Morgana", "Victorious Morgana", "Lunar Wraith Morgana"
		};
		
		private static IEnumerable<string> Nami = new[]
        {
			"Classic", "Koi Nami", "River Spirit Nami", "Urf the Nami-tee" 
		};
		
		private static IEnumerable<string> Nasus = new[]
        {
			"Classic", "Galactic Nasus", "Pharaoh Nasus", "Dreadknight Nasus", "Riot K-9 Nasus", "Infernal Nasus", "Archduke Nasus" 
		};
		
		private static IEnumerable<string> Nautilus = new[]
        {
			"Classic", "Abyssal Nautilus", "Subterranean Nautilus", "AstroNautilus", "Warden Nautilus"
		};
		
		private static IEnumerable<string> Nidalee = new[]
        {
			"Classic", "Snow Bunny Nidalee", "Leopard Nidalee", "French Maid Nidalee", "Pharaoh Nidalee", "Bewitching Nidalee", "Headhunter Nidalee", "Warring Kingdoms Nidalee", "Challenger Nidalee"
		};
		
		private static IEnumerable<string> Nocturne = new[]
        {
			"Classic", "Frozen Terror Nocturne", "Void Nocturne", "Ravager Nocturne", "Haunting Nocturne", "Eternum Nocturne", "Cursed Revenant Nocturne"
		};
		
		private static IEnumerable<string> Nunu = new[]
        {
			"Classic", "Sasquatch Nunu", "Workshop Nunu", "Grungy Nunu", "Nunu Bot", "Demolisher Nunu", "TPA Nunu", "Zombie Nunu"
		};
		
		private static IEnumerable<string> Olaf = new[]
        {
			"Classic", "Forsaken Olaf", "Glacial Olaf", "Brolaf", "Pentakill Olaf", "Marauder Olaf"
		};
		
		private static IEnumerable<string> Orianna = new[]
        {
			"Classic", "Gothic Orianna", "Sewn Chaos Orianna", "Bladecraft Orianna", "TPA Orianna", "Winter Wonder Orianna", "Heartseeker Orianna"
		};
		
		private static IEnumerable<string> Pantheon = new[]
        {
			"Classic", "Myrmidon Pantheon", "Ruthless Pantheon", "Perseus Pantheon", "Full Metal Pantheon", "Glaive Warrior Pantheon", "Dragonslayer Pantheon", "Slayer Pantheon"
		};
		
		private static IEnumerable<string> Poppy = new[]
        {
			"Classic", "Noxus Poppy", "Lollipoppy", "Blacksmith Poppy", "Ragdoll Poppy", "Battle Regalia Poppy", "Scarlet Hammer Poppy"
		};
		
		private static IEnumerable<string> Quinn = new[]
        {
			"Classic", "Phoenix Quinn", "Woad Scout Quinn", "Corsair Quinn"
		};
		
		private static IEnumerable<string> Rammus = new[]
        {
			"Classic", "King Rammus", "Chrome Rammus", "Molten Rammus", "Freljord Rammus", "Ninja Rammus", "Full Metal Rammus", "Guardian of the Sands Rammus"
		};
		
		private static IEnumerable<string> RekSai = new[]
        {
			"Classic", "Eternum Rek'Sai", "Pool Party Rek'Sai"
		};
		
		private static IEnumerable<string> Renekton = new[]
        {
			"Classic", "Galactic Renekton", "Outback Renekton", "Bloodfury Renekton", "Rune Wars Renekton", "Scorched Earth Renekton", "Pool Party Renekton", "Prehistoric Renekton"
		};
		
		private static IEnumerable<string> Rengar = new[]
        {
			"Classic", "Headhunter Rengar", "Night Hunter Rengar", "SSW Rengar"
		};
		
		private static IEnumerable<string> Riven = new[]
        {
			"Classic", "Redeemed Riven", "Crimson Elite Riven", "Battle Bunny Riven", "Championship Riven", "Dragonblade Riven", "Arcade Riven"
		};
		
		private static IEnumerable<string> Rumble = new[]
        {
			"Classic", "Rumble in the Jungle", "Bilgerat Rumble", "Super Galaxy Rumble"
		};
		
		private static IEnumerable<string> Ryze = new[]
        {
			"Classic", "Human Ryze", "Tribal Ryze", "Uncle Ryze", "Triumphant Ryze", "Professor Ryze", "Zombie Ryze", "Dark Crystal Ryze", "Pirate Ryze", "Ryze Whitebeard"
		};
		
		private static IEnumerable<string> Sejuani = new[]
        {
			"Classic", "Sabretusk Sejuani", "Darkrider Sejuani", "Traditional Sejuani", "Bear Cavalry Sejuani", "Poro Rider Sejuani", "Beast Hunter Sejuani"
		};
		
		private static IEnumerable<string> Shaco = new[]
        {
			"Classic", "Mad Hatter Shaco", "Royal Shaco", "Nutcracko", "Workshop Shaco", "Asylum Shaco", "Masked Shaco", "Wild Card Shaco"
		};
		
		private static IEnumerable<string> Shen = new[]
        {
			"Classic", "Frozen Shen", "Yellow Jacket Shen", "Surgeon Shen", "Blood Moon Shen", "Warlord Shen", "TPA Shen"
		};
		
		private static IEnumerable<string> Shyvana = new[]
        {
			"Classic", "Ironscale Shyvana", "Boneclaw Shyvana", "Darkflame Shyvana", "Ice Drake Shyvana", "Championship Shyvana", "Super Galaxy Shyvana"
		};
		
		private static IEnumerable<string> Singed = new[]
        {
			"Classic", "Riot Squad Singed", "Hextech Singed", "Surfer Singed", "Mad Scientist Singed", "Augmented Singed", "Snow Day Singed", "SSW Singed", "Black Scourge Singed"
		};
		
		private static IEnumerable<string> Sion = new[]
        {
			"Classic", "Hextech Sion", "Barbarian Sion", "Lumberjack Sion", "Warmonger Sion", "Mecha Zero Sion"
		};
		
		private static IEnumerable<string> Sivir = new[]
        {
			"Classic", "Warrior Princess Sivir", "Spectacular Sivir", "Huntress Sivir", "Bandit Sivir", "PAX Sivir", "Snowstorm Sivir", "Warden Sivir", "Victorious Sivir"
		};
		
		private static IEnumerable<string> Skarner = new[]
        {
			"Classic", "Sandscourge Skarner", "Earthrune Skarner", "Battlecast Alpha Skarner", "Guardian of the Sands Skarner"
		};
		
		private static IEnumerable<string> Sona = new[]
        {
			"Classic", "Muse Sona", "Pentakill Sona", "Silent Night Sona", "Guqin Sona", "Arcade Sona", "DJ Sona", "Sweetheart Sona"
		};
		
		private static IEnumerable<string> Soraka = new[]
        {
			"Classic", "Dryad Soraka", "Divine Soraka", "Celestine Soraka", "Reaper Soraka", "Order of the Banana Soraka", "Program Soraka"
		};
		
		private static IEnumerable<string> Swain = new[]
        {
			"Classic", "Northern Front Swain", "Bilgewater Swain", "Tyrant Swain"
		};
		
		private static IEnumerable<string> Syndra = new[]
        {
			"Classic", "Justicar Syndra", "Atlantean Syndra", "Queen of Diamonds Syndra", "Snow Day Syndra"
		};
		
		private static IEnumerable<string> TahmKench = new[]
        {
			"Classic", "Master Chef Tahm Kench", "Urf Kench"
		};
		
		private static IEnumerable<string> Taliyah = new[]
        {
			"Classic", "Freljord Taliyah"
		};
		
		private static IEnumerable<string> Talon = new[]
        {
			"Classic", "Renegade Talon", "Crimson Elite Talon", "Dragonblade Talon", "SSW Talon"
		};
		
		private static IEnumerable<string> Taric = new[]
        {
			"Classic", "Emerald Taric", "Armor of the Fifth Age Taric", "Bloodstone Taric"
		};
		
		private static IEnumerable<string> Teemo = new[]
        {
			"Classic", "Happy Elf Teemo", "Recon Teemo", "Badger Teemo", "Astronaut Teemo", "Cottontail Teemo", "Super Teemo", "Panda Teemo", "Omega Squad Teemo"
		};
		
		private static IEnumerable<string> Thresh = new[]
        {
			"Classic", "Deep Terror Thresh", "Championship Thresh", "Blood Moon Thresh", "SSW Thresh", "Dark Star Thresh"
		};
		
		private static IEnumerable<string> Tristana = new[]
        {
			"Classic", "Riot Girl Tristana", "Earnest Elf Tristana", "Firefighter Tristana", "Guerilla Tristana", "Buccaneer Tristana", "Rocket Girl Tristana", "Dragon Trainer Tristana"
		};
		
		private static IEnumerable<string> Trundle = new[]
        {
			"Classic", "Lil' Slugger Trundle", "Junkyard Trundle", "Traditional Trundle", "Constable Trundle"
		};
		
		private static IEnumerable<string> Tryndamere = new[]
        {
			"Classic", "Highland Tryndamere", "King Tryndamere", "Viking Tryndamere", "Demonblade Tryndamere", "Sultan Tryndamere", "Warring Kingdoms Tryndamere", "Nightmare Tryndamere", "Beast Hunter Tryndamere"
		};
		
		private static IEnumerable<string> TwistedFate = new[]
        {
			"Classic", "PAX Twisted Fate", "Jack of Hearts Twisted Fate", "The Magnificent Twisted Fate", "Tango Twisted Fate", "High Noon Twisted Fate", "Musketeer Twisted Fate", "Underworld Twisted Fate", "Red Card Twisted Fate", "Cutpurse Twisted Fate"
		};
		
		private static IEnumerable<string> Twitch = new[]
        {
			"Classic", "Kingpin Twitch", "Whistler Village Twitch", "Medieval Twitch", "Gangster Twitch", "Vandal Twitch", "Pickpocket Twitch", "SSW Twitch"
		};
		
		private static IEnumerable<string> Udyr = new[]
        {
			"Classic", "Black Belt Udyr", "Primal Udyr", "Spirit Guard Udyr", "Definitely Not Udyr"
		};
		
		private static IEnumerable<string> Urgot = new[]
        {
			"Classic", "Giant Enemy Crabgot", "Butcher Urgot", "Battlecast Urgot"
		};
		
		private static IEnumerable<string> Varus = new[]
        {
			"Classic", "Blight Crystal Varus", "Arclight Varus", "Arctic Ops Varus", "Heartseeker Varus", "Varus Swiftbolt", "Dark Star Varus"
		};
		
		private static IEnumerable<string> Vayne = new[]
        {
			"Classic", "Vindicator Vayne", "Aristocrat Vayne", "Dragonslayer Vayne", "Heartseeker Vayne", "SKT T1 Vayne", "Arclight Vayne"
		};
		
		private static IEnumerable<string> Veigar = new[]
        {
			"Classic", "White Mage Veigar", "Curling Veigar", "Veigar Greybeard", "Leprechaun Veigar", "Baron Von Veigar", "Superb Villain Veigar", "Bad Santa Veigar", "Final Boss Veigar"
		};
		
		private static IEnumerable<string> VelKoz = new[]
        {
			"Classic", "Battlecast Vel'Koz", "Arclight Vel'Koz", "Definitely Not Vel'Koz"
		};
		
		private static IEnumerable<string> Vi = new[]
        {
			"Classic", "Neon Strike Vi", "Officer Vi", "Debonair Vi", "Demon Vi"
		};
		
		private static IEnumerable<string> Viktor = new[]
        {
			"Classic", "Full Machine Viktor", "Prototype Viktor", "Creator Viktor"
		};
		
		private static IEnumerable<string> Vladimir = new[]
        {
			"Classic", "Count Vladimir", "Marquis Vladimir", "Nosferatu Vladimir", "Vandal Vladimir", "Blood Lord Vladimir", "Soulstealer Vladimir", "Academy Vladimir"
		};
		
		private static IEnumerable<string> Volibear = new[]
        {
			"Classic", "Thunder Lord Volibear", "Northern Storm Volibear", "Runeguard Volibear", "Captain Volibear", "BEl Rayo Volibear"
		};
		
		private static IEnumerable<string> Warwick = new[]
        {
			"Classic", "Grey Warwick", "Urf the Manatee", "Big Bad Warwick", "Tundra Hunter Warwick", "Feral Warwick", "Firefang Warwick", "Hyena Warwick", "Marauder Warwick"
		};
		
		private static IEnumerable<string> Wukong = new[]
        {
			"Classic", "Volcanic Wukong", "General Wukong", "Jade Dragon Wukong", "Underworld Wukong", "Radiant Wukong"
		};
		
		private static IEnumerable<string> Xerath = new[]
        {
			"Classic", "Runeborn Xerath", "Battlecast Xerath", "Scorched Earth Xerath", "Guardian of the Sands Xerath"
		};
		
		private static IEnumerable<string> XinZhao = new[]
        {
			"Classic", "Commando Xin Zhao", "Imperial Xin Zhao", "Viscero Xin Zhao", "Winged Hussar Xin Zhao", "Warring Kingdoms Xin Zhao", "Secret Agent Xin Zhao"
		};
		
		private static IEnumerable<string> Yasuo = new[]
        {
			"Classic", "High Noon Yasuo", "PROJECT: Yasuo", "Blood Moon Yasuo"
		};
		
		private static IEnumerable<string> Yorick = new[]
        {
			"Classic", "Undertaker Yorick", "Pentakill Yorick"
		};
		
		private static IEnumerable<string> Zac = new[]
        {
			"Classic", "Special Weapon Zac", "Pool Party Zac"
		};
		
		private static IEnumerable<string> Zed = new[]
        {
			"Classic", "Shockblade Zed", "SKT T1 Zed", "PROJECT: Zed"
		};
		
		private static IEnumerable<string> Ziggs = new[]
        {
			"Classic", "Mad Scientist Ziggs", "Major Ziggs", "Pool Party Ziggs", "Snow Day Ziggs", "Master Arcanist Ziggs"
		};
		
		private static IEnumerable<string> Zilean = new[]
        {
			"Classic", "Old Saint Zilean", "Groovy Zilean", "Shurima Desert Zilean", "Time Machine Zilean", "Blood Moon Zilean"
		};
		
		private static IEnumerable<string> Zyra = new[]
        {
			"Classic", "Wildfire Zyra", "Haunted Zyra", "SKT T1 Zyra"
		};
        #endregion

    }
}
