
using System.Reflection;
using EFT;
using SPTarkov.Common.Utils.Patching;
using UnityEngine;
using System.Linq;
using System;
using EFT.Game.Spawning;

namespace SPTarkov.SinglePlayer.Patches.Progression
{
    class OfflineSpawnPointPatch : GenericPatch<OfflineSpawnPointPatch>
    {
        public OfflineSpawnPointPatch() : base(prefix: nameof(PatchPrefix)) { }

        protected override MethodBase GetTargetMethod()
        {
            var targetType = PatcherConstants.TargetAssembly.GetTypes().First(IsTargetType);
            return targetType.GetMethods(PatcherConstants.DefaultBindingFlags).First(m => m.Name.Contains("SelectSpawnPoint"));
        }

        private static bool IsTargetType(Type type)
        {
            var methods = type.GetMethods(PatcherConstants.DefaultBindingFlags);

            if (!methods.Any(x => x.Name.IndexOf("CheckFarthestFromOtherPlayers", StringComparison.OrdinalIgnoreCase) != -1))
                return false;

            return !type.IsInterface;
        }

        public static bool PatchPrefix(ref ISpawnPoint __result, GInterface208 ___ginterface208_0, ESpawnCategory category, EPlayerSide side, string infiltration)
        {
            var spawnPoints = Enumerable.ToList(___ginterface208_0);
            
            spawnPoints = spawnPoints.Where(sp => sp.Sides.Contain(side) && sp.Categories.Contain(category)).ToList();
            var infils = spawnPoints.Select(sp => sp.Infiltration).Distinct();

            Debug.LogError($"PatchPrefix SelectSpawnPoint: {spawnPoints.Count} | {String.Join(", ", infils)}");

            __result = spawnPoints.Where(sp => sp.Infiltration.Equals(infiltration)).RandomElement();
            Debug.LogError($"Selected Spawn Point: {__result.Id}");
            return false;
        }
    }
}
