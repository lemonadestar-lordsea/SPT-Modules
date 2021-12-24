using Aki.Common.Utils;
using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using EFT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Aki.Custom.Patches
{
    public class IsEnemyPatch : ModulePatch
    {
        private static Type _targetType;
        private static FieldInfo _sideField;
        private static FieldInfo _enemiesField;

        public IsEnemyPatch()
        {
            _targetType = typeof(GClass480);
            _sideField = _targetType.GetField("Side");
            _enemiesField = _targetType.GetField("Enemies");
        }

        protected override MethodBase GetTargetMethod()
        {
            return _targetType.GetMethod("IsEnemy");
        }

        /// <summary>
        /// IsEnemy()
        /// Goal: if bot not found in enemy dictionary, we manually choose if they're an enemy or friend
        /// Check enemy cache list first, if not found, choose a value
        /// </summary>
        [PatchPrefix]
        private static bool PatchPrefix(ref bool __result, object __instance, GInterface62 requester)
        {
            var random = new Random();
            var side = (EPlayerSide)_sideField.GetValue(__instance);
            var enemies = (Dictionary<GInterface62, GClass487>)_enemiesField.GetValue(__instance);

            if (enemies.Any(x=> x.Value.Player.Id == requester.Id))
            {
                __result = true;
            }
            else
            {
                if (side == EPlayerSide.Usec)
                {
                    if (requester.Side == EPlayerSide.Usec)
                    {
                        //add 80% chance of being enemy
                        __result = random.Next(1, 10) > 3;
                    }

                    // everyone else is an enemy to usecs
                    __result = true;
                }

                if (side == EPlayerSide.Bear)
                {
                    if (requester.Side == EPlayerSide.Bear)
                    {
                        //add 80% chance of being enemy
                        __result = random.Next(1, 10) > 3;
                    }

                    // everyone else is an enemy to bears
                    __result = true;
                }

                if (side == EPlayerSide.Savage)
                {
                    if (requester.Side == EPlayerSide.Savage)
                    {
                        __result = false;
                    }

                    // everyone else is an enemy to savage (scavs)
                    __result = true;
                }
            }

            return false;
        }
    }
}
