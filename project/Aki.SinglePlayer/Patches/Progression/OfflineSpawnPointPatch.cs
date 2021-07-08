using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EFT;
using EFT.Game.Spawning;
using Aki.Common.Utils;
using Aki.Common.Utils.Patching;

namespace Aki.SinglePlayer.Patches.Progression
{
    class OfflineSpawnPointPatch : GenericPatch<OfflineSpawnPointPatch>
    {
        public OfflineSpawnPointPatch() : base(prefix: nameof(PatchPrefix))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            var targetType = PatcherConstants.TargetAssembly.GetTypes().First(IsTargetType);
            return targetType.GetMethods(PatcherConstants.DefaultBindingFlags).First(m => m.Name.Contains("SelectSpawnPoint"));
        }

        private static bool IsTargetType(Type type)
        {
            var methods = type.GetMethods(PatcherConstants.DefaultBindingFlags);

            if (!methods.Any(x => x.Name.IndexOf("CheckFarthestFromOtherPlayers", StringComparison.OrdinalIgnoreCase) != -1))
            {
                return false;
            }

            return !type.IsInterface;
        }

        public static bool PatchPrefix(ref ISpawnPoint __result, GInterface229 ___ginterface229_0, ESpawnCategory category, EPlayerSide side, string infiltration)
        {
            var spawnPoints = ___ginterface229_0.ToList();
            var unfilteredSpawnPoints = spawnPoints.ToList();
            var infils = spawnPoints.Select(sp => sp.Infiltration).Distinct();

            Log.Info($"PatchPrefix SelectSpawnPoint Infiltrations: {spawnPoints.Count} | {string.Join(", ", infils)}");
            Log.Info($"Filter by Infiltration: {infiltration}");
            spawnPoints = spawnPoints.Where(sp =>  sp != null && sp.Infiltration != null && (string.IsNullOrEmpty(infiltration) || sp.Infiltration.Equals(infiltration))).ToList();

            if (spawnPoints.Count == 0)
            {
                __result = GetFallBackSpawnPoint(unfilteredSpawnPoints, category, side, infiltration);
                return false;
            }

            Log.Info($"Filter by Categories: {category}");
            spawnPoints = spawnPoints.Where(sp => sp.Categories.Contain(category)).ToList();

            if (spawnPoints.Count == 0)
            {
                __result = GetFallBackSpawnPoint(unfilteredSpawnPoints, category, side, infiltration);
                return false;
            }

            Log.Info($"Filter by Side: {side}");
            spawnPoints = spawnPoints.Where(sp => sp.Sides.Contain(side)).ToList();

            if (spawnPoints.Count == 0)
            {
                __result = GetFallBackSpawnPoint(unfilteredSpawnPoints, category, side, infiltration);
                return false;
            }

            __result = spawnPoints.RandomElement();
            Log.Info($"PatchPrefix SelectSpawnPoint: {__result.Id}");
            return false;
        }

        private static ISpawnPoint GetFallBackSpawnPoint(List<ISpawnPoint> spawnPoints, ESpawnCategory category, EPlayerSide side, string infiltration)
        {
            Log.Warning($"PatchPrefix SelectSpawnPoint: Couldn't find any spawn points for:  {category}  |  {side}  |  {infiltration}");
            var spawn = spawnPoints.Where(sp => sp.Categories.Contain(ESpawnCategory.Player)).RandomElement();
            Log.Info($"PatchPrefix SelectSpawnPoint: {spawn.Id}");
            return spawn;
        }
    }
}
