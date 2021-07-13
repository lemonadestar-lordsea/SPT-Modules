using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EFT;
using EFT.Game.Spawning;
using Aki.Common;
using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using SpawnPoints = GInterface222;

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
            return Constants.EftTypes.First(IsTargetType)
                .GetMethods(Constants.PrivateFlags).First(m => m.Name.Contains("SelectSpawnPoint"));
        }

        private static bool IsTargetType(Type type)
        {
            return (type.GetMethods(Constants.PrivateFlags).Any(x => x.Name.IndexOf("CheckFarthestFromOtherPlayers", StringComparison.OrdinalIgnoreCase) != -1)
                && type.IsClass);
        }

        private static bool PatchPrefix(ref ISpawnPoint __result, SpawnPoints ___ginterface222_0, ESpawnCategory category, EPlayerSide side, string infiltration)
        {
            var spawnPoints = ___ginterface222_0.ToList();
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
