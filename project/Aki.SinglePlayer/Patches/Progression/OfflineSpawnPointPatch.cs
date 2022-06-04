using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using EFT;
using EFT.Game.Spawning;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Aki.SinglePlayer.Patches.Progression
{
    public class OfflineSpawnPointPatch : ModulePatch
    {
        static OfflineSpawnPointPatch()
        {
            _ = nameof(ISpawnPoints.CreateSpawnPoint);
        }

        protected override MethodBase GetTargetMethod()
        {
            return PatchConstants.EftTypes.First(IsTargetType)
                .GetMethods(PatchConstants.PrivateFlags).First(m => m.Name.Contains("SelectSpawnPoint"));
        }

        private static bool IsTargetType(Type type)
        {
            //GClass1812 as of 17349
            return (type.GetMethods(PatchConstants.PrivateFlags).Any(x => x.Name.IndexOf("CheckFarthestFromOtherPlayers", StringComparison.OrdinalIgnoreCase) != -1)
                && type.IsClass);
        }

        [PatchPrefix]
        private static bool PatchPrefix(
            ref ISpawnPoint __result,
            object __instance,
            ESpawnCategory category,
            EPlayerSide side,
            string groupId,
            IAIDetails person,
            string infiltration)
        {
            var ginterface250_0 = Traverse.Create(__instance).Field<ISpawnPoints>("ginterface250_0").Value;

            var spawnPoints = ginterface250_0.ToList();

            // filter by 'customs', "woods"
            spawnPoints = spawnPoints.Where(sp => sp?.Infiltration != null && (string.IsNullOrEmpty(infiltration) || sp.Infiltration.Equals(infiltration))).ToList();

            // Filter by 'player'
            spawnPoints = spawnPoints.Where(sp => sp.Categories.Contain(category)).ToList();

            var partiallyFilteredSpawnPoints = spawnPoints.ToList();

            // Filter by 'usec', 'bear', 'scav'
            spawnPoints = spawnPoints.Where(sp => sp.Sides.Contain(side)).ToList();

            __result = spawnPoints.Count == 0 ? GetFallBackSpawnPoint(partiallyFilteredSpawnPoints, category, side, infiltration) : spawnPoints.RandomElement();
            Logger.LogInfo($"PatchPrefix SelectSpawnPoint: {__result.Id}");
            return false;
        }

        private static ISpawnPoint GetFallBackSpawnPoint(List<ISpawnPoint> spawnPoints, ESpawnCategory category, EPlayerSide side, string infiltration)
        {
            Logger.LogWarning($"PatchPrefix SelectSpawnPoint: Couldn't find any spawn points for: {category} | {side} | {infiltration} using random partially filtered spawn instead");
            return spawnPoints.Where(sp => sp.Categories.Contain(ESpawnCategory.Player)).RandomElement();
        }
    }
}
