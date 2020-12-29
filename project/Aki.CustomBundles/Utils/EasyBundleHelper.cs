/* EasyBundleHelper.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * Craink
 * reider123
 * GGaulin
 * Ginja
 * Merijn Hendriks
 */


using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using IBundleLock = GInterface250; //Property: IsLocked
using BindableState = GClass2160<Diz.DependencyManager.ELoadState>; //Construct method parameter: initialValue

/* Maintenance Tips
 * 
 * This class is used to help change the behaivior of the "Diz Plugings - Achievements System"
 * There are convenience methods
 * 
 * Note: It looks like there was an attempt to abstract some of the hard coded class / interface dependances in the patches but this looks like WIP and not fully implemented.
 * 
 * Use dnSpy to find the correct GClass/GInterface/Property Name used within each patch.
 * 
 * dnSpy:
 *   - Open the un-obfuscated EFT CSharp Assemply "\EscapeFromTarkov_Data\Managed\Assembly-CSharp.dll"
 *   - Within the Assembly Expoler Tress, select the "Assembly-CSharp (0.0.0.0) file
  *   - Search for "IsLocked"      using Options Search For: "Property", "Selected Files" and update "IBundleLock" to the Interface found
 *   - Search for "initialValue"  using Options Search For: "Parameter", "Selected Files" and update "BindableState" to the Class found
 */

namespace Aki.CustomBundles.Utils
{
    class EasyBundleHelper
    {
        private readonly object _instance;
        private readonly Traverse _trav;

        private static readonly string _pathFieldName = "string_1";
        private static readonly string _keyWithoutExtensionFieldName = "string_0";
        private static readonly string _loadingJobPropertyName = "task_0";
        private static readonly string _dependencyKeysPropertyName = "DependencyKeys";
        private static readonly string _keyPropertyName = "Key";
        private static readonly string _loadStatePropertyName = "LoadState";
        private static readonly string _progressPropertyName = "Progress";
        private static readonly string _bundlePropertyName = "assetBundle_0";
        private static readonly string _loadingAssetOperationFieldName = "assetBundleRequest_0";
        private static readonly string _assetsPropertyName = "Assets";
        private static readonly string _sameNameAssetPropertyName = "SameNameAsset";
        private static MethodInfo _loadingCoroutineMethod;

        public IEnumerable<string> DependencyKeys
        {
            get => _trav.Property<IEnumerable<string>>(_dependencyKeysPropertyName).Value; set => _trav.Property<IEnumerable<string>>(_dependencyKeysPropertyName).Value = value;
        }

        public IBundleLock BundleLock
        {
            get => _trav.Field<IBundleLock>($"{nameof(IBundleLock).ToLower()}_0").Value; set => _trav.Field<IBundleLock>($"{nameof(IBundleLock).ToLower()}_0").Value = value;
        }

        public Task LoadingJob
        {
            get => _trav.Field<Task>(_loadingJobPropertyName).Value; set => _trav.Field<Task>(_loadingJobPropertyName).Value = value;
        }

        public string Path
        {
            get => _trav.Field<string>(_pathFieldName).Value; set => _trav.Field<string>(_pathFieldName).Value = value;
        }

        public string Key
        {
            get => _trav.Property<string>(_keyPropertyName).Value; set => _trav.Property<string>(_keyPropertyName).Value = value;
        }

        public BindableState LoadState
        {
            get => _trav.Property<BindableState>(_loadStatePropertyName).Value; set => _trav.Property<BindableState>(_loadStatePropertyName).Value = value;
        }

        public float Progress
        {
            get => _trav.Property<float>(_progressPropertyName).Value; set => _trav.Property<float>(_progressPropertyName).Value = value;
        }

        
        public AssetBundle Bundle
        {
            get => _trav.Field<AssetBundle>(_bundlePropertyName).Value; set => _trav.Field<AssetBundle>(_bundlePropertyName).Value = value;
        }
        
        public AssetBundleRequest loadingAssetOperation
        {
            get => _trav.Field<AssetBundleRequest>(_loadingAssetOperationFieldName).Value; set => _trav.Field<AssetBundleRequest>(_loadingAssetOperationFieldName).Value = value;
        }


        public Object[] Assets
        {
            get => _trav.Property<UnityEngine.Object[]>(_assetsPropertyName).Value; set => _trav.Property<UnityEngine.Object[]>(_assetsPropertyName).Value = value;
        }

        public UnityEngine.Object SameNameAsset
        {
            get => _trav.Property<UnityEngine.Object>(_sameNameAssetPropertyName).Value; set => _trav.Property<UnityEngine.Object>(_sameNameAssetPropertyName).Value = value;
        }

        public string KeyWithoutExtension
        {
            get => _trav.Field<string>(_keyWithoutExtensionFieldName).Value; set => _trav.Field<string>(_keyWithoutExtensionFieldName).Value = value;
        }

        public EasyBundleHelper(object easyBundle)
        {
            _instance = easyBundle;
            _trav = Traverse.Create(easyBundle);

            if (_loadingCoroutineMethod == null)
            {
                _loadingCoroutineMethod = easyBundle.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Single(x => x.GetParameters().Length == 0 && x.ReturnType == typeof(Task));
                //TODO:Search member names by condition
            }
        }

        public Task LoadingCoroutine()
        {
            return (Task)_loadingCoroutineMethod.Invoke(_instance, new object[] { });
        }
    }
}
