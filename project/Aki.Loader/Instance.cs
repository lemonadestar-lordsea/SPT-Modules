using UnityEngine;
using Aki.Common.Utils;

namespace Aki.Loader
{
    public class Instance : MonoBehaviour
    {
        public void Start()
        {
            Debug.LogError("Aki.Loader: Loaded");

            ModuleLoader.AddRepository(VFS.Combine(VFS.Cwd, "/Aki_Data/Modules/"));
            ModuleLoader.AddRepository(VFS.Combine(VFS.Cwd, "/user/mods/"));
            ModuleLoader.LoadAllAssemblies();
        }
    }
}
