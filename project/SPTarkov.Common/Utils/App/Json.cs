﻿using Newtonsoft.Json;
using System.IO;

namespace SPTarkov.Common.Utils.App
{
    public static class Json
	{
		public static string Serialize<T>(T data)
		{
			return JsonConvert.SerializeObject(data);
		}

		public static T Deserialize<T>(string json)
		{
			return JsonConvert.DeserializeObject<T>(json);
		}

		public static void Save<T>(string filepath, T data)
		{
			string json = Serialize<T>(data);
			File.WriteAllText(filepath, json);
		}

		public static void SaveWithFormatting<T>(string filepath, T data, Formatting format)
		{
			File.WriteAllText(filepath, JsonConvert.SerializeObject(data, format));
		}

		public static T Load<T>(string filepath) where T : new()
		{
			if (!File.Exists(filepath))
			{
				Save(filepath, new T());
				return Load<T>(filepath);
			}

			string json = File.ReadAllText(filepath);
			return Deserialize<T>(json);
		}
	}
}
