using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EFT;

namespace Aki.SinglePlayer.Patches.MainMenu
{
    public class FieldOfViewPatch : Patch
    {
        private const int _min = 10;
        private const int _max = 360;
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

        public FieldOfViewPatch() : base(T: typeof(FieldOfViewPatch), prefix: nameof(PatchPrefix))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(GameSettings).GetNestedTypes().Single(x => IsTargetType(x))
                .GetProperties().Single(x => x.Name == _propertyName).GetGetMethod();
        }

        private static bool IsTargetType(Type type)
        {
            var properties = type.GetProperties();

            if (properties == null)
            {
                return false;
            }

            return properties.Any(x => x.Name == _propertyName);
        }

        private static bool PatchPrefix(ref object __result)
        {
            __result = _notches;
            return false;
        }
    }
}
