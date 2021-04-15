using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Aki.Common.Utils;
using Aki.SinglePlayer.Models;

namespace Aki.SinglePlayer.Utils.Bundles
{
    public class BundleSettings
    {
        public const string cachePach = "Cache/StreamingAssets/windows/";
        public readonly static Dictionary<string, BundleInfo> bundles = new Dictionary<string, BundleInfo>();

		public BundleSettings()
		{
            try
            {
                if (VFS.Exists(cachePach))
                {
                    VFS.DeleteDirectory(cachePach);
                }
            }
            catch
            {
                Debug.LogError("Aki.CustomBundles: The cache cleanup failed and will try again at the next game startup.");
            }
            
            var jArray = JArray.Parse(RequestHandler.GetBundles());

            foreach (var jObj in jArray)
            {
                var bundle = (BundleInfo)null;

                if (!bundles.TryGetValue(jObj["key"].ToString(), out bundle))
                {
                    bundle = new BundleInfo(jObj["key"].ToString(), jObj["path"].ToString(), jObj["dependencyKeys"].ToObject<List<string>>().ToArray());
                    bundles.Add(bundle.Key, bundle);
                }
            }
		}
    }

    
}
