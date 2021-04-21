using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using EFT;
using Aki.Common.Utils;

namespace Aki.SinglePlayer.Utils
{
    public static class RequestHandler
    {
        private static readonly Request request;
        private static readonly string host;
        private static string session;
        private static Dictionary<string, string> headers;

        static RequestHandler()
        {
            host = Utils.Config.BackendUrl;
            session = null;
            request = new Request();
            Debug.LogError($"Aki.SinglePlayer: Request host: {host}");
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

            if (session == null)
            {
                session = backend.GetPhpSessionId();
                headers = new Dictionary<string, string>()
                {
                    { "Cookie", $"PHPSESSID={session}" },
                    { "SessionId", session }
                };

                Debug.LogError($"Aki.SinglePlayer: Request session: {session}");
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
            
            var result = request.Send(host + url, "GET", null, headers: headers);
            
            ValidateData(result);
            return result;
        }

        private static string GetJson(string url)
        {
            PrepareRequest(url);

            var data = request.Send(host + url, "GET", headers: headers);
            var result = Encoding.UTF8.GetString(data);
            
            ValidateJson(result);
            return result;
        }

        private static string PostJson(string url, string json)
        {
            PrepareRequest(url);

            var data = request.Send(host + url, "POST", Encoding.UTF8.GetBytes(json), true, "application/json", headers);
            var result = Encoding.UTF8.GetString(data);
            
            ValidateJson(result);
            return result;
        }

        private static void PutJson(string url, string json)
        {
            PrepareRequest(url);
            request.Send(host + url, "PUT", Encoding.UTF8.GetBytes(json), true, "application/json", headers);
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