using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EFT;
using Aki.Reflection.Patching;

namespace Aki.SinglePlayer.Patches.MainMenu
{
    public class FieldOfViewPatch : GenericPatch<FieldOfViewPatch>
    {
        private const int _min = 10;
        private const int _max = 210;
        private const string _propertyName = "FieldOfViewNotches";
        private static string[] _notches;

        static FieldOfViewPatch()
        {
            var notches = new List<string>();

            for (int i = _min; i < _max; i++)
            {
                notches.Add(i.ToString());
            }

            _notches = notches.ToArray();
        }

        public FieldOfViewPatch() : base(prefix: nameof(PatchPrefix))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(GameSettings).GetNestedTypes().Single(x => IsTargetType(x))
                .GetProperties().Single(x => x.Name == _propertyName).GetGetMethod();
        }

        private static bool IsTargetType(Type type)
        {
            return type.GetProperties().Single(x => x.Name == _propertyName) != null;
        }

        private static bool PatchPrefix(ref object __result)
        {
            __result = _notches;
            return false;
        }
    }
}
