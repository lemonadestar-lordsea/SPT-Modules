using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using Aki.SinglePlayer.Models;
using Aki.SinglePlayer.Utils;
using Comfort.Common;
using EFT;
using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Aki.SinglePlayer.Patches.Progression
{
    public class OfflineSaveProfilePatch : ModulePatch
    {
        private static readonly JsonConverter[] _defaultJsonConverters;

        static OfflineSaveProfilePatch()
        {
            _ = nameof(ClientMetrics.Metrics);

            var converterClass = typeof(AbstractGame).Assembly.GetTypes()
                .First(t => t.GetField("Converters", BindingFlags.Static | BindingFlags.Public) != null);

            _defaultJsonConverters = Traverse.Create(converterClass).Field<JsonConverter[]>("Converters").Value;
        }

        public OfflineSaveProfilePatch() : base(T: typeof(OfflineSaveProfilePatch), prefix: nameof(PatchPrefix))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            return Constants.EftTypes.Single(x => x.Name == "MainApplication")
                .GetMethod("method_44", Constants.PrivateFlags);
        }

        private static void PatchPrefix(ESideType ___esideType_0, Result<ExitStatus, TimeSpan, ClientMetrics> result)
        {
            var session = Constants.BackEndSession;

            SaveProfileRequest request = new SaveProfileRequest
			{
				Exit = result.Value0.ToString().ToLowerInvariant(),
				Profile = (___esideType_0 == ESideType.Savage) ? session.ProfileOfPet : session.Profile,
				Health = Utils.Healing.HealthListener.Instance.CurrentHealth,
				IsPlayerScav = (___esideType_0 == ESideType.Savage)
			};

			RequestHandler.PutJson("/raid/profile/save", request.ToJson(_defaultJsonConverters.AddItem(new NotesCustomJsonConverter()).ToArray()));
        }

        private class NotesCustomJsonConverter : JsonConverter
        {
            private static Type _targetType;

            public NotesCustomJsonConverter()
            {
                _targetType = typeof(AbstractGame).Assembly.GetTypes().First(t =>
                    t.GetProperty("TransactionInProcess", BindingFlags.Instance | BindingFlags.Public) != null);
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var valueToSerialize = Traverse.Create(_targetType).Field<List<object>>("Notes").Value;
                serializer.Serialize(writer, $"{{\"Notes\":{JsonConvert.SerializeObject(valueToSerialize)}}}");
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }

            public override bool CanConvert(Type objectType)
            {
                return objectType == _targetType;
            }
        }
    }
}
