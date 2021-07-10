using System.Linq;
using System.Reflection;
using Aki.Reflection.Patching;
using Aki.Reflection.Utils;

namespace Aki.SinglePlayer.Patches.Bots
{
    public class BossSpawnChancePatch : GenericPatch<BossSpawnChancePatch>
    {
        private static float[] _bossSpawnPercent;

        public BossSpawnChancePatch() : base(prefix: nameof(PrefixPatch), postfix: nameof(PostfixPatch))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            return Constants.LocalGameType.BaseType
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .SingleOrDefault(m => IsTargetMethod(m));
        }

        private static bool IsTargetMethod(MethodInfo mi)
        {
            var parameters = mi.GetParameters();
            return (parameters.Length != 2 || parameters[0].Name != "wavesSettings" || parameters[1].Name != "bossLocationSpawn") ? false : true;
        }

        private static void PrefixPatch(BossLocationSpawn[] bossLocationSpawn)
        {
            _bossSpawnPercent = bossLocationSpawn.Select(s => s.BossChance).ToArray();
        }

        private static void PostfixPatch(ref BossLocationSpawn[] __result)
        {
            if (__result.Length != _bossSpawnPercent.Length)
            {
                return;
            }

            for (var i = 0; i < _bossSpawnPercent.Length; i++)
            {
                __result[i].BossChance = _bossSpawnPercent[i];
            }
        }
    }
}
