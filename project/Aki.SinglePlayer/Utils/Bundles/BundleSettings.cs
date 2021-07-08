using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Aki.Common.Utils;
using Aki.SinglePlayer.Models;

namespace Aki.SinglePlayer.Utils.Bundles
{
    public class BundleSettings
    {
        public const string CachePath = "Cache/StreamingAssets/windows/";
        public readonly static Dictionary<string, BundleInfo> Bundles;

        static BundleSettings()
        {
            Bundles = new Dictionary<string, BundleInfo>();

            // clear cache
            if (VFS.Exists(CachePath))
            {
                VFS.DeleteDirectory(CachePath);
            }
        }

        public static void GetBundles()
        {
            var json = RequestHandler.GetJson("/singleplayer/bundles");
            var jArray = JArray.Parse(json);

            foreach (var jObj in jArray)
            {
                var bundle = (BundleInfo)null;

                if (!Bundles.TryGetValue(jObj["key"].ToString(), out bundle))
                {
                    bundle = new BundleInfo(jObj["key"].ToString(), jObj["path"].ToString(), jObj["dependencyKeys"].ToObject<List<string>>().ToArray());
                    Bundles.Add(bundle.Key, bundle);
                }
            }
        }
    }
}
