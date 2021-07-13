using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using Aki.Common;
using Aki.Reflection.Utils;
using IBundleLock = GInterface254;
using BindableState = GClass2206<Diz.DependencyManager.ELoadState>;

namespace Aki.SinglePlayer.Utils.Bundles
{
    public class EasyBundleHelper
    {
        private object _instance;
        private FieldInfo _pathField;
        private FieldInfo _keyWithoutExtensionField;
        private FieldInfo _bundleLockField;
        private FieldInfo _loadingJobField;
        private PropertyInfo _dependencyKeysProperty;
        private PropertyInfo _keyProperty;
        private PropertyInfo _loadStateProperty;
        private PropertyInfo _progressProperty;
        private FieldInfo _bundleField;
        private FieldInfo _loadingAssetOperationField;
        private PropertyInfo _assetsProperty;
        private PropertyInfo _sameNameAssetProperty;
        private static MethodInfo _loadingCoroutineMethod;
        public static Type Type;

        static EasyBundleHelper()
        {
            _ = nameof(IBundleLock.IsLocked);
            _ = nameof(BindableState.Bind);

            Type = Constants.EftTypes.Single(x => x.IsClass && x.GetProperty("SameNameAsset") != null);
            _loadingCoroutineMethod = Type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                    .Single(x => x.GetParameters().Length == 0 && x.ReturnType == typeof(Task));
        }

        public IEnumerable<string> DependencyKeys
        {
            get
            {
                return (IEnumerable<string>)_dependencyKeysProperty.GetValue(_instance);
            }
            set
            {
                _dependencyKeysProperty.SetValue(_instance, value);
            }
        }

        public IBundleLock BundleLock
        {
            get
            {
                return (IBundleLock)_bundleLockField.GetValue(_instance);
            }
            set
            {
                _bundleLockField.SetValue(_instance, value);
            }
        }

        public Task LoadingJob
        {
            get
            {
                return (Task)_loadingJobField.GetValue(_instance);
            }
            set
            {
                _loadingJobField.SetValue(_instance, value);
            }
        }

        public string Path
        {
            get
            {
                return (string)_pathField.GetValue(_instance);
            }
            set
            {
                _pathField.SetValue(_instance, value);
            }
        }

        public string Key
        {
            get
            {
                return (string)_keyProperty.GetValue(_instance);
            }
            set
            {
                _keyProperty.SetValue(_instance, value);
            }
        }

        public BindableState LoadState
        {
            get
            {
                return (BindableState)_loadStateProperty.GetValue(_instance);
            }
            set
            {
                _loadStateProperty.SetValue(_instance, value);
            }
        }

        public float Progress
        {
            get
            {
                return (float)_progressProperty.GetValue(_instance);
            }
            set
            {
                _progressProperty.SetValue(_instance, value);
            }
        }


        public AssetBundle Bundle
        {
            get
            {
                return (AssetBundle)_bundleField.GetValue(_instance);
            }
            set
            {
                _bundleField.SetValue(_instance, value);
            }
        }

        public AssetBundleRequest loadingAssetOperation
        {
            get
            {
                return (AssetBundleRequest)_loadingAssetOperationField.GetValue(_instance);
            }
            set
            {
                _loadingAssetOperationField.SetValue(_instance, value);
            }
        }


        public UnityEngine.Object[] Assets
        {
            get
            {
                return (UnityEngine.Object[])_assetsProperty.GetValue(_instance);
            }
            set
            {
                _assetsProperty.SetValue(_instance, value);
            }
        }

        public UnityEngine.Object SameNameAsset
        {
            get
            {
                return (UnityEngine.Object)_sameNameAssetProperty.GetValue(_instance);
            }
            set
            {
                _sameNameAssetProperty.SetValue(_instance, value);
            }
        }

        public string KeyWithoutExtension
        {
            get
            {
                return (string)_keyWithoutExtensionField.GetValue(_instance);
            }
            set
            {
                _keyWithoutExtensionField.SetValue(_instance, value);
            }
        }

        public EasyBundleHelper(object easyBundle)
        {
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;

            _instance = easyBundle;
            _pathField = Type.GetField("string_1", flags);
            _keyWithoutExtensionField = Type.GetField("string_0", flags);
            _bundleLockField = Type.GetField($"{nameof(GInterface254).ToLower()}_0", flags);
            _loadingJobField = Type.GetField("task_0", flags);
            _dependencyKeysProperty = Type.GetProperty("DependencyKeys");
            _keyProperty = Type.GetProperty("Key");
            _loadStateProperty = Type.GetProperty("LoadState");
            _progressProperty = Type.GetProperty("Progress");
            _bundleField = Type.GetField("assetBundle_0", flags);
            _loadingAssetOperationField = Type.GetField("assetBundleRequest_0");
            _assetsProperty = Type.GetProperty("Assets");
            _sameNameAssetProperty = Type.GetProperty("SameNameAsset");
        }

        public Task LoadingCoroutine()
        {
            return (Task)_loadingCoroutineMethod.Invoke(_instance, new object[] { });
        }
    }
}
