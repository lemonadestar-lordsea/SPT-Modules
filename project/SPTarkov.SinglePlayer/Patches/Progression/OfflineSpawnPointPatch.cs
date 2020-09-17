
using System.Reflection;
using EFT;
using EFT.Interactive;
using SPTarkov.Common.Utils.Patching;
using UnityEngine;
using System.Linq;
using System;

namespace SPTarkov.SinglePlayer.Patches.Progression
{
    class OfflineSpawnPointPatch : GenericPatch<OfflineSpawnPointPatch>
    {
        public OfflineSpawnPointPatch() : base(prefix: nameof(PatchPrefix))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            var targetType = PatcherConstants.TargetAssembly.GetTypes().Single(IsTargetType);
            return targetType.GetMethod("SelectSpawnPoint", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        }

        private static bool IsTargetType(Type type)
        {
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            if (!methods.Any(x => x.Name == "CheckFarthestFromOtherPlayers"))
            {
                return false;
            }

            return true;
        }

        public static bool PatchPrefix(SpawnArea.SpawnAreaSettings[] ___spawnAreaSettings_0, EPlayerSide side, out Vector3 position, out Quaternion rotation, string spawnPointFilter = null, string infiltrationZone = null)
        {
            var spawnAreaSettingHelper = new SpawnAreaSettingHelper(side, spawnPointFilter, infiltrationZone);
            var spawnAreaSettings = ___spawnAreaSettings_0.Where(spawnAreaSettingHelper.isSpawnAreaSetting).RandomElement();

            if (spawnAreaSettings == null)
            {
                Debug.LogError("No spawn points for " + side + " found! Spawn points count: " +  ___spawnAreaSettings_0.Length);
                position = Vector3.zero;
                rotation = Quaternion.identity;
                return false;
            }

            position = spawnAreaSettings.Position;
            rotation = Quaternion.Euler(0f, spawnAreaSettings.Orientation, 0f);

            return false;
        }
    }

    public class SpawnAreaSettingHelper
    {
        private readonly EPlayerSide side;
        private readonly string spawnPointFilter;
        private readonly string infiltrationZone;

        public SpawnAreaSettingHelper(EPlayerSide side, string spawnPointFilter, string infiltrationZone)
        {
            this.side = side;
            this.spawnPointFilter = spawnPointFilter;
            this.infiltrationZone = infiltrationZone;
        }

        public bool isSpawnAreaSetting(SpawnArea.SpawnAreaSettings x)
        {
            return x.Sides.Contains(side)
                && (string.IsNullOrWhiteSpace(infiltrationZone) || x.InfiltrationZone == infiltrationZone)
                && (string.IsNullOrWhiteSpace(spawnPointFilter) || spawnPointFilter.Contains(x.Id));
        }
    }
}
