/* Settings.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * Merijn Hendriks
 */


using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json.Linq;
using SPTarkov.Common.Utils.HTTP;

namespace SPTarkov.RuntimeBundles.Utils
{
    public class Settings
    {
		private static string Session;
		private static string BackendUrl;
        public static readonly string cachePach = "Cache/StreamingAssets/windows/";
        public readonly static Dictionary<string, BundleInfo> bundles = new Dictionary<string, BundleInfo>();

		public Settings(string session, string backendUrl)
		{
            Session = session;
            BackendUrl = backendUrl;

            Init(); 
        }

		private void Init()
		{
            try
            {
                CleanCache();
            } catch
            {
                Debug.LogError("SPTarkov.RuntimeBundles: The cache cleanup failed and will try again at the next game startup.");
            }
            
            var json = new Request(Session, BackendUrl).GetJson("/singleplayer/bundles");

            if (string.IsNullOrWhiteSpace(json))
			{
				Debug.LogError("SPTarkov.RuntimeBundles: Bundles data is Null, using fallback");
				return;
			}

            var jArray = JArray.Parse(json);

            foreach (var jObj in jArray)
            {
                var bundle = (BundleInfo)null;

                if (!bundles.TryGetValue(jObj["key"].ToString(), out bundle))
                {
                    bundle = new BundleInfo(jObj["key"].ToString(), jObj["path"].ToString(), jObj["dependencyKeys"].ToObject<List<string>>().ToArray());
                    bundles.Add(bundle.Key, bundle);
                }
            }
            
            Debug.LogError("SPTarkov.RuntimeBundles: Successfully received Bundles");
		}

        private void CleanCache()
        {
            if (Directory.Exists(cachePach))
            {
                Directory.Delete(cachePach, true);
            }
        }
    }

    public class BundleInfo
    {
        public string Key { get;}
        public string Path { get;}
        public string[] DependencyKeys { get;}

        public BundleInfo(string key, string path, string[] dependencyKeys)
        {
            Key = key;
            Path = path;
            DependencyKeys = dependencyKeys;
        }
    }
}
