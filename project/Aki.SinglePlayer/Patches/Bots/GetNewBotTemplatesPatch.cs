using Aki.Common;
using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using Comfort.Common;
using EFT;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Aki.SinglePlayer.Patches.Bots
{
    public struct BundleLoader
    {
        Profile Profile;
        TaskScheduler TaskScheduler { get; }

        public BundleLoader(TaskScheduler taskScheduler)
        {
            Profile = null;
            TaskScheduler = taskScheduler;
        }

        public Task<Profile> LoadBundles(Task<Profile> task)
        {
            Profile = task.Result;

            var loadTask = Singleton<PoolManager>.Instance.LoadBundlesAndCreatePools(
                PoolManager.PoolsCategory.Raid,
                PoolManager.AssemblyType.Local,
                Profile.GetAllPrefabPaths(false).ToArray(),
                JobPriority.General,
                null,
                default);

            return loadTask.ContinueWith(GetProfile, TaskScheduler);
        }

        private Profile GetProfile(Task task)
        {
            return Profile;
        }
    }

    public class GetNewBotTemplatesPatch : Patch
    {
        private static MethodInfo _getNewProfileMethod;

        static GetNewBotTemplatesPatch()
        {
            _ = nameof(IBotData.PrepareToLoadBackend);
            _ = nameof(BotsPresets.GetNewProfile);
            _ = nameof(PoolManager.LoadBundlesAndCreatePools);
            _ = nameof(JobPriority.General);
        }

        public GetNewBotTemplatesPatch() : base(T: typeof(GetNewBotTemplatesPatch), prefix: nameof(PatchPrefix))
        {
            _getNewProfileMethod = typeof(BotsPresets)
                .GetMethod(nameof(BotsPresets.GetNewProfile), Constants.PrivateFlags);
        }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(BotsPresets).GetMethod(nameof(BotsPresets.CreateProfile), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        }

        private static bool PatchPrefix(ref Task<Profile> __result, BotsPresets __instance, IBotData data)
        {
            /*
                in short when client wants new bot and GetNewProfile() return null (if not more available templates or they don't satisfied by Role and Difficulty condition)
                then client gets new piece of WaveInfo collection (with Limit = 30 by default) and make request to server
                but use only first value in response (this creates a lot of garbage and cause freezes)
                after patch we request only 1 template from server

                along with other patches this one causes to call data.PrepareToLoadBackend(1) gets the result with required role and difficulty:
                new[] { new WaveInfo() { Limit = 1, Role = role, Difficulty = difficulty } }
                then perform request to server and get only first value of resulting single element collection
            */

            var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            var taskAwaiter = (Task<Profile>)null;
            var profile = (Profile)_getNewProfileMethod.Invoke(__instance, new object[] { data });

            if (profile == null)
            {
                // load from server
                Log.Info("Loading bot profile from server");
                var source = data.PrepareToLoadBackend(1).ToList();
                taskAwaiter = Constants.BackEndSession.LoadBots(source).ContinueWith(GetFirstResult, taskScheduler);
            }
            else
            {
                // return cached profile
                Log.Info("Loading bot profile from cache");
                taskAwaiter = Task.FromResult(profile);
            }

            // load bundles for bot profile
            var continuation = new BundleLoader(taskScheduler);
            __result = taskAwaiter.ContinueWith(continuation.LoadBundles, taskScheduler).Unwrap();
            return false;
        }

        private static Profile GetFirstResult(Task<Profile[]> task)
        {
            return task.Result[0];
        }
    }
}
