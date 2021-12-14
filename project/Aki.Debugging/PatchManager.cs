using Aki.Debugging.Patches;
using Aki.Reflection.Patching;

namespace Aki.Debugging
{
    public static class PatchManager
    {
        public static readonly PatchList Patches;

        static PatchManager()
        {
            Patches = new PatchList
            {
                new CoordinatesPatch()
            };
        }
    }
}
