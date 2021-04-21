using System;
using UnityEngine;

namespace Aki.Loader
{
    public class Instance : MonoBehaviour
    {
        public void Start()
        {
            Debug.LogError("Aki.Loader: Loaded");

            ModuleLoader.AddRepository($@"{Environment.CurrentDirectory}\Aki_Data\Modules\");
            ModuleLoader.AddRepository($@"{Environment.CurrentDirectory}\user\mods\");
            ModuleLoader.LoadAllAssemblies();
        }
    }
}
