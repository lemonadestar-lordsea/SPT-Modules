
using System.Reflection;
using EFT;
using EFT.Interactive;
using SPTarkov.Common.Utils.Patching;
using UnityEngine;
using System.Linq;
using System;
using EFT.Game.Spawning;

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
            return targetType.GetMethods(BindingFlags.NonPublic| BindingFlags.Instance)
                .First(m => m.Name.Contains("SelectSpawnPoint"));
        }

        private static bool IsTargetType(Type type)
        {
            if (!type.IsSealed && type.IsInterface)
                return false;

            return type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Any(x => x.Name.Contains("CheckFarthestFromOtherPlayers"));
        }

        public static bool PatchPrefix(GInterface208 ___ginterface208_0, ESpawnCategory category, EPlayerSide side, string groupId, GInterface54 person, string infiltration = null)
        {
            var spawnAreaSettingHelper = new SpawnAreaSettingHelper(side, null, infiltration);
            var spawnAreaSettings = ___ginterface208_0.Where(spawnAreaSettingHelper.isSpawnAreaSetting).RandomElement();

            if (spawnAreaSettings == null)
            {
                Debug.LogError("No spawn points for " + side + " found! Spawn points count: " +  ___ginterface208_0.Count());
                //position = Vector3.zero;
                //rotation = Quaternion.identity;
                return false;
            }

            //position = spawnAreaSettings.Position;
            //rotation = Quaternion.Euler(0f, spawnAreaSettings.Orientation, 0f);

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

        public bool isSpawnAreaSetting(ISpawnPoint x)
        {
            return x.Sides.Contain(side)
                && (string.IsNullOrWhiteSpace(infiltrationZone) || x.Infiltration == infiltrationZone)
                && (string.IsNullOrWhiteSpace(spawnPointFilter) || spawnPointFilter.Contains(x.Id));
        }
    }
}
