using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using Newtonsoft.Json;
using System.Xml.Serialization;
using System.Text;
using System.Runtime.Serialization.Json;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace CNLib {
	/// <summary>
	///     The storage, main purpose is to save share-able settings between assemblies.
	/// </summary>

	[XmlType(AnonymousType = true)]
	[XmlRoot(Namespace = "", IsNullable = false)]
	public class Storage{
		
		[JsonIgnore]
		private static string StoragePath { get; }
		private string storageName;
		private string StorageName
		{
			get
			{
				return this.storageName;
			}
			set
			{
				if (!Path.GetInvalidFileNameChars().Any(value.Contains))
				{
					this.storageName = value;
				}
			}
		}

		public List<StoragePair> Contents { get; set; } 

		static Storage() {
			StoragePath = Path.Combine(LeagueSharp.Common.Config.AppDataDirectory, "AsuvrilStorage");

			if (!Directory.Exists(StoragePath))
			{
				Directory.CreateDirectory(StoragePath);
			}
			
		}

		public void SaveFile(string s) {
			var path = Path.Combine(StoragePath, StorageName + ".json");
			File.WriteAllText(path, s);
		}

		public string ReadFile() {
			var path = Path.Combine(StoragePath, StorageName + ".json");
			DeBug.Debug("[存储]", $"StorageName：{StorageName} 存储位置：{path}");
			if (File.Exists(path))
			{
				return File.ReadAllText(path);
			}
			return string.Empty;
		}

		public Storage(string storageName = "Generic") {
			if (Path.GetInvalidFileNameChars().Any(storageName.Contains))
			{
				throw new InvalidDataException("Storage name can't have invalid file name characters.");
			}

			var path = Path.Combine(StoragePath, storageName + ".json");
			//if (File.Exists(path))
			//{
			//	//Contents = JsonConvert.DeserializeObject<Storage>(File.ReadAllText(path)).Contents;
			//}
			//else
			//{
			//	this.StorageName = storageName;
			//	//Contents = new List<StoragePair>();
			//}
			this.StorageName = storageName;
			AppDomain.CurrentDomain.ProcessExit += (sender, args) =>
			{
				Save();
			};
			AppDomain.CurrentDomain.DomainUnload += (sender, args) =>
			{
				Save();
			};
		}

		public static bool Exists(string storageName = "Generic") {
			if (Path.GetInvalidFileNameChars().Any(storageName.Contains))
			{
				throw new InvalidDataException("Storage name can't have invalid file name characters.");
			}
			return File.Exists(Path.Combine(StoragePath, storageName + ".json"));
		}

		public void Set(string key, string value) {
			if (this.Contents.Any(c=> c.key == key))
			{
				this.Contents.Find(c => c.key == key).value = value;
			}
			else
			{
				this.Contents.Add(new StoragePair {
					key = key,
					value = value
				});
			}
		}

		public bool Remove(string key) {
			if (this.Contents.Any(c => c.key == key))
			{
				this.Contents.Remove(this.Contents.Find(c => c.key == key));
				return true;
			}

			return false;
		}

		public void Save() {
			var path = Path.Combine(StoragePath, StorageName + ".json");

			DeBug.Debug("[存储]", $"StorageName：{StorageName} 存储位置：{path}");

			//DeBug.Debug("[存储]", $"============Instance.Contents内容===============");
			//foreach (var item in Contents)
			//{
			//	DeBug.Debug("[存储]", $"键：{item.key} 值：{item.value}");
			//}
			//DeBug.Debug("[存储]", $"============Instance.Contents内容===============");

			File.WriteAllText(path, JsonConvert.SerializeObject(this));
		}

		public string Get(string key) {
			return this.Contents.Find(c => c.key == key)?.value;
		}
	}
	
}
