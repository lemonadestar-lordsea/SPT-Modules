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

        public static byte[] GetData(string url)
        {
            PrepareRequest(url);
            
            var result = request.Send(host + url, "GET", null, headers: headers);
            
            ValidateData(result);
            return result;
        }

        public static string GetJson(string url)
        {
            PrepareRequest(url);

            var data = request.Send(host + url, "GET", headers: headers);
            var result = Encoding.UTF8.GetString(data);
            
            ValidateJson(result);
            return result;
        }

        public static string PostJson(string url, string json)
        {
            PrepareRequest(url);

            var data = request.Send(host + url, "POST", Encoding.UTF8.GetBytes(json), true, "application/json", headers);
            var result = Encoding.UTF8.GetString(data);
            
            ValidateJson(result);
            return result;
        }

        public static void PutJson(string url, string json)
        {
            PrepareRequest(url);
            request.Send(host + url, "PUT", Encoding.UTF8.GetBytes(json), true, "application/json", headers);
        }
    }   
}