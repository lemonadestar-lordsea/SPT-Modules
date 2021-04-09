using System;
using Comfort.Common;
using UnityEngine;
using EFT;
using Aki.Common.Utils.App;
using Aki.Common.Utils.HTTP;
using Aki.SinglePlayer.Utils.DefaultSettings;

namespace Aki.SinglePlayer.Utils
{
    public static class RequestHandler
    {
        private static Request request;

        static RequestHandler()
        {
            request = new Request(Config.BackEndSession.GetPhpSessionId(), Config.BackendUrl);
        }

        public static void SaveLoot(string json)
        {
            request.Send("/raid/profile/save", "PUT", json.ToJson(), true, false);
        }

        public static string SynchroniseHealth(string json)
        {
            var result = request.PostJson("/player/health/sync", json);
            return string.IsNullOrWhiteSpace(result) ? null : result;
        }

        public static string SendLocationName(string locationId)
		{
			var result = request.GetJson($"/raid/map/name?locationId={locationId}");
            return string.IsNullOrWhiteSpace(result) ? null : result;
		}

        public static string GetBotCoreDifficulty()
        {
            var result = request.GetJson("/singleplayer/settings/bot/difficulty/core/core");

            if (string.IsNullOrWhiteSpace(result))
            {
                Debug.LogError("Aki.SinglePlayer: Received core bot difficulty data is NULL, using fallback");
                return null;
            }

            Debug.LogError("Aki.SinglePlayer: Successfully received core bot difficulty data");
            return result;
        }

        public static string GetBotDifficulty(WildSpawnType role, BotDifficulty botDifficulty)
        {
            var result = request.GetJson("/singleplayer/settings/bot/difficulty/" + role.ToString() + "/" + botDifficulty.ToString());

            if (string.IsNullOrWhiteSpace(result))
            {
                Debug.LogError("Aki.SinglePlayer: Received bot " + role.ToString() + " " + botDifficulty.ToString() + " difficulty data is NULL, using fallback");
                return null;
            }

            Debug.LogError("Aki.SinglePlayer: Successfully received bot " + role.ToString() + " " + botDifficulty.ToString() + " difficulty data");
            return result;
        }

        public static int GetBotLimit(WildSpawnType role)
        {
            var result = request.GetJson("/singleplayer/settings/bot/limit/" + role.ToString());

            if (string.IsNullOrWhiteSpace(result))
            {
                Debug.LogError("Aki.SinglePlayer: Received bot " + role.ToString() + " limit data is NULL, using fallback");
                return 30;
            }

            Debug.LogError("Aki.SinglePlayer: Successfully received bot " + role.ToString() + " limit data");
            return Convert.ToInt32(result);
        }

        public static bool GetDurabilityState()
		{
			var result = request.GetJson("/singleplayer/settings/weapon/durability");

			if (string.IsNullOrWhiteSpace(result))
			{
				Debug.LogError("Aki.SinglePlayer: Received weapon durability state data is NULL, using fallback");
				return false;
			}

			Debug.LogError("Aki.SinglePlayer: Successfully received weapon durability state");
			return Convert.ToBoolean(result);
		}

        public static bool GetEndState()
        {
            var json = request.GetJson("/singleplayer/settings/raid/endstate");

            if (string.IsNullOrWhiteSpace(json))
            {
                Debug.LogError("Aki.SinglePlayer: Received NULL response for DefaultRaidSettings. Defaulting to fallback.");
                return false;
            }

            Debug.LogError("Aki.SinglePlayer: Successfully received DefaultRaidSettings");
            return Convert.ToBoolean(json);
        }

        public static DefaultRaidSettings GetDefaultRaidSettings()
        {
            var json = request.GetJson("/singleplayer/settings/raid/menu");

            if (string.IsNullOrWhiteSpace(json))
            {
                Debug.LogError("Aki.SinglePlayer: Received NULL response for DefaultRaidSettings. Defaulting to fallback.");
                return null;
            }

            Debug.LogError("Aki.SinglePlayer: Successfully received DefaultRaidSettings");

            try
            {
                return Json.Deserialize<DefaultRaidSettings>(json);
            }
            catch (Exception exception)
            {
                Debug.LogError("Aki.SinglePlayer: Failed to deserialize DefaultRaidSettings from server. Check your gameplay.json config in your server. Defaulting to fallback. Exception: " + exception);
                return null;
            }
        }
    }   
}