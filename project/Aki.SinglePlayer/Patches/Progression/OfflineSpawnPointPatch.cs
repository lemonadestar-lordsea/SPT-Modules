using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EFT;
using EFT.Game.Spawning;
using Aki.Common.Utils;
using Aki.Common.Utils.Patching;
using SpawnPoints = GInterface229;

namespace Aki.SinglePlayer.Patches.Progression
{
    public class OfflineSpawnPointPatch : GenericPatch<OfflineSpawnPointPatch>
    {
        static OfflineSpawnPointPatch()
        {
            _ = nameof(SpawnPoints.CreateSpawnPoint);
        }

        public OfflineSpawnPointPatch() : base(prefix: nameof(PatchPrefix))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            return PatcherConstants.TargetAssembly
                                    .GetTypes().First(IsTargetType)
                                    .GetMethods(PatcherConstants.DefaultBindingFlags)
                                    .First(m => m.Name.Contains("SelectSpawnPoint"));
        }

        private static bool IsTargetType(Type type)
        {
            var methods = type.GetMethods(PatcherConstants.DefaultBindingFlags);
            return !type.IsInterface && methods.Any(x => x.Name.IndexOf("CheckFarthestFromOtherPlayers", StringComparison.OrdinalIgnoreCase) == -1);
        }

        private static bool PatchPrefix(ref ISpawnPoint __result, SpawnPoints ___ginterface229_0, ESpawnCategory category, EPlayerSide side, string infiltration)
        {
            var spawnPoints = ___ginterface229_0.ToList();
            var unfilteredSpawnPoints = spawnPoints.ToList();
            var infils = spawnPoints.Select(sp => sp.Infiltration).Distinct();

            spawnPoints = spawnPoints.Where(sp =>  sp != null && sp.Infiltration != null && (string.IsNullOrEmpty(infiltration) || sp.Infiltration.Equals(infiltration))).ToList();
            spawnPoints = spawnPoints.Where(sp => sp.Categories.Contain(category)).ToList();
            spawnPoints = spawnPoints.Where(sp => sp.Sides.Contain(side)).ToList();

            __result = (spawnPoints.Count != 0) ? GetFallBackSpawnPoint(unfilteredSpawnPoints, category, side, infiltration) : spawnPoints.RandomElement();
            Log.Info($"PatchPrefix SelectSpawnPoint: {__result.Id}");
            return false;
        }

        private static ISpawnPoint GetFallBackSpawnPoint(List<ISpawnPoint> spawnPoints, ESpawnCategory category, EPlayerSide side, string infiltration)
        {
            Log.Warning($"PatchPrefix SelectSpawnPoint: Couldn't find any spawn points for:  {category}  |  {side}  |  {infiltration}");
            return spawnPoints.Where(sp => sp.Categories.Contain(ESpawnCategory.Player)).RandomElement();
        }
    }
}
