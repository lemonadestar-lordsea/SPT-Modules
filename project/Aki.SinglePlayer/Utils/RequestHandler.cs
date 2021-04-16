using System;
using UnityEngine;
using EFT;
using Aki.Common.Utils;

namespace Aki.SinglePlayer.Utils
{
    public static class RequestHandler
    {
        private static readonly Request request;

        static RequestHandler()
        {
            var backendUrl = Utils.Config.BackendUrl;
            request = new Request(null, backendUrl);
            Debug.LogError($"Aki.SinglePlayer: Request host: {backendUrl}");
        }

        private static void PrepareRequest(string url)
        {
            Debug.LogError($"Aki.SinglePlayer: Request: {url}");

            var backend = Utils.Config.BackEndSession;

            if (backend == null)
            {
                Debug.LogError($"Aki.SinglePlayer: Request session not active");
                return;
            }

            if (request.Session == null)
            {
                request.Session = backend.GetPhpSessionId();
                Debug.LogError($"Aki.SinglePlayer: Request session: {request.Session}");
            }
        }

        private static void ValidateData(byte[] data)
        {
            if (data == null)
            {
                Debug.LogError($"Aki.SinglePlayer: Request failed, body is null");
            }

            Debug.LogError($"Aki.SinglePlayer: Request was successful");
        }

        private static void ValidateJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                Debug.LogError($"Aki.SinglePlayer: Request failed, body is null");
            }

            Debug.LogError($"Aki.SinglePlayer: Request was successful");
        }

        private static byte[] GetData(string url)
        {
            PrepareRequest(url);
            var result = request.GetData(url);
            ValidateData(result);
            return result;
        }

        private static string GetJson(string url)
        {
            PrepareRequest(url);
            var result = request.GetJson(url);
            ValidateJson(result);
            return result;
        }

        private static string PostJson(string url, string json)
        {
            PrepareRequest(url);
            var result = request.PostJson(url, json);
            ValidateJson(result);
            return result;
        }

        private static void PutJson(string url, string json)
        {
            PrepareRequest(url);
            request.PutJson(url, json);
        }

        public static void SaveLoot(string json)
        {
            PutJson("/raid/profile/save", json);
        }

        public static void SynchroniseHealth(string json)
        {
            PostJson("/player/health/sync", json);
        }

        public static void SendLocationName(string locationId)
		{
			GetJson($"/raid/map/name?locationId={locationId}");
		}

        public static string GetBundles()
        {
            return GetJson("/singleplayer/bundles");
        }

        public static byte[] GetBundle(string path)
        {
            return GetData(path);
        }

        public static string GetBotCoreDifficulty()
        {
            return GetJson("/singleplayer/settings/bot/difficulty/core/core");
        }

        public static string GetBotDifficulty(WildSpawnType role, BotDifficulty botDifficulty)
        {
            return GetJson($"/singleplayer/settings/bot/difficulty/{role.ToString()}/{botDifficulty.ToString()}");
        }

        public static int GetBotLimit(WildSpawnType role)
        {
            var json = GetJson($"/singleplayer/settings/bot/limit/{role.ToString()}");
            return (string.IsNullOrWhiteSpace(json)) ? 30 : Convert.ToInt32(json);
        }

        public static bool GetDurabilityState()
		{
			var json = GetJson("/singleplayer/settings/weapon/durability");
			return (string.IsNullOrWhiteSpace(json)) ? false : Convert.ToBoolean(json);
		}

        public static bool GetEndState()
        {
            var json = GetJson("/singleplayer/settings/raid/endstate");
            return (string.IsNullOrWhiteSpace(json)) ? false : Convert.ToBoolean(json);
        }

        public static string GetDefaultRaidSettings()
        {
            return GetJson("/singleplayer/settings/raid/menu");
        }
    }   
}