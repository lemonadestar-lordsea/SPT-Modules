using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using SPTarkov.Common.Utils.App;
using SPTarkov.Common.Utils.HTTP;
using SPTarkov.Common.Utils.Patching;
using LocationInfo = GClass757.GClass759;

namespace SPTarkov.SinglePlayer.Patches.Progression
{
	public class OfflineLootPatch : GenericPatch<OfflineLootPatch>
	{
		public static PropertyInfo _property;

		public OfflineLootPatch() : base(prefix: nameof(PatchPrefix))
		{
            // compile-time check
            _ = nameof(LocationInfo.BotLocationModifier);
        }

        protected override MethodBase GetTargetMethod()
        {
			var localGameBaseType = PatcherConstants.LocalGameType.BaseType;

			_property = localGameBaseType.GetProperty($"{nameof(GClass757.GClass759)}_0", BindingFlags.NonPublic | BindingFlags.Instance);
			return localGameBaseType.GetMethod("method_5", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
		}

		/// <summary>
		/// Loads loot from SPTarkov's server.
		/// Falls back to the client's local location loot if it fails.
		/// </summary>
		public static bool PatchPrefix(ref Task<LocationInfo> __result, object __instance, string backendUrl)
		{
			if (__instance.GetType() != PatcherConstants.LocalGameType)
			{
				// online match
				Debug.LogError("OfflineLootPatch > Online match?!");
				return true;
			}

			var location = (LocationInfo)_property.GetValue(__instance);
			var request = new Request(Utils.Config.BackEndSession.GetPhpSessionId(), backendUrl);
			var json = request.GetJson("/api/location/" + location.Id);

			// some magic here. do not change =)
			var locationLoot = json.ParseJsonTo<LocationInfo>();

			request.PostJson("/raid/map/name", Json.Serialize(new LocationName(location.Id)));

            if (locationLoot == null)
			{
				// failed to download loot
				Debug.LogError("OfflineLootPatch > Failed to download loot, using fallback");
				return true;
			}

			Debug.LogError("OfflineLootPatch > Successfully received loot from server");
			__result = Task.FromResult(locationLoot);

			return false;
		}

		struct LocationName
		{
			public string Location { get; }

			public LocationName(string location)
			{
				Location = location;
			}
		}
	}
}