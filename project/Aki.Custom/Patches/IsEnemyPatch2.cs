using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using EFT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Aki.Custom.Patches
{
    public class IsEnemyPatch2 : ModulePatch
    {
        private static Type _targetType;
        private static FieldInfo _sideField;
        private static FieldInfo _enemiesField;
        private static FieldInfo _spawnTypeField;
        private static MethodInfo _addEnemy;

        public IsEnemyPatch2()
        {
            _targetType = PatchConstants.EftTypes.Single(IsTargetType);
            _sideField = _targetType.GetField("Side");
            _enemiesField = _targetType.GetField("Enemies");
            _spawnTypeField = _targetType.GetField("wildSpawnType_0", BindingFlags.NonPublic | BindingFlags.Instance);
            _addEnemy = _targetType.GetMethod("AddEnemy");
        }

        private bool IsTargetType(Type type)
        {
            if (type.GetMethod("AddEnemy") != null && type.GetMethod("AddEnemyGroupIfAllowed") != null)
            {
                return true;
            }

            return false;
        }

        protected override MethodBase GetTargetMethod()
        {
            return _targetType.GetMethod("CheckAndAddEnemy");
        }

        /// <summary>
        /// IsEnemy()
        /// Goal: if current entity is a boss/follower/raider AND target is usec/bear, add them to the entities' enemy list
        /// This patch lets bosses shoot back once a PMC has shot them
        /// </summary>
        [PatchPrefix]
        private static bool PatchPrefix(object __instance, IAIDetails player, bool ignoreAI = false)
        {
            //var side = (EPlayerSide)_sideField.GetValue(__instance);
            var botType = (WildSpawnType)_spawnTypeField.GetValue(__instance);
            //var enemies = (Dictionary<IAIDetails, BotSettingsClass>)_enemiesField.GetValue(__instance);

            if (!player.HealthController.IsAlive)
            {
                return false; // skip original
            }

            var bosses = new Enum[] { WildSpawnType.bossTagilla, WildSpawnType.bossBully, WildSpawnType.bossGluhar, WildSpawnType.bossKilla, WildSpawnType.bossKojaniy, WildSpawnType.bossSanitar, WildSpawnType.bossTagilla };
            if ((bosses.Contains(botType) || botType == WildSpawnType.pmcBot || botType.ToString().StartsWith("follower"))
                && player.Side != EPlayerSide.Savage)
            {
                _addEnemy.Invoke(__instance, new object[] { player });
                return false; // skip original
            }
            
            // perform original
            return true;
        }
    }
}
