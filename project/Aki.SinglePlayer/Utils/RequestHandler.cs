using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Aki.Common.Utils;

namespace Aki.SinglePlayer.Utils
{
    public static class RequestHandler
    {
        private static readonly Request _request;
        private static readonly string _host;
        private static string _session;
        private static Dictionary<string, string> _headers;

        static RequestHandler()
        {
            _host = Utils.Config.BackendUrl;
            _session = null;
            _request = new Request();
            Debug.LogError($"Aki.SinglePlayer: Request host: {_host}");
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

            if (_session == null)
            {
                _session = backend.GetPhpSessionId();
                _headers = new Dictionary<string, string>()
                {
                    { "Cookie", $"PHPSESSID={_session}" },
                    { "SessionId", _session }
                };

                Debug.LogError($"Aki.SinglePlayer: Request session: {_session}");
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
            
            var result = _request.Send(_host + url, "GET", null, headers: _headers);
            
            ValidateData(result);
            return result;
        }

        public static string GetJson(string url)
        {
            PrepareRequest(url);

            var data = _request.Send(_host + url, "GET", headers: _headers);
            var result = Encoding.UTF8.GetString(data);
            
            ValidateJson(result);
            return result;
        }

        public static string PostJson(string url, string json)
        {
            PrepareRequest(url);

            var data = _request.Send(_host + url, "POST", Encoding.UTF8.GetBytes(json), true, "application/json", _headers);
            var result = Encoding.UTF8.GetString(data);
            
            ValidateJson(result);
            return result;
        }

        public static void PutJson(string url, string json)
        {
            PrepareRequest(url);
            _request.Send(_host + url, "PUT", Encoding.UTF8.GetBytes(json), true, "application/json", _headers);
        }
    }
}