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
        public readonly static Dictionary<string, BundleInfo> bundles;

        static BundleSettings()
        {
            bundles = new Dictionary<string, BundleInfo>();

            // clear cache
            if (VFS.Exists(cachePach))
            {
                VFS.DeleteDirectory(cachePach);
            }
        }

        public static void GetBundles()
        {
            var json = RequestHandler.GetBundles();
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
        }
    }
}
