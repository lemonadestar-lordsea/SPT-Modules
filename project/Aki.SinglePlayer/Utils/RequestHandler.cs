using System;
using System.Collections.Generic;
using System.Text;
using Aki.Common.Utils;
using Aki.SinglePlayer.Models;

namespace Aki.SinglePlayer.Utils
{
    public static class RequestHandler
    {
        private static string _host;
        private static string _session;
        private static Request _request;
        private static Dictionary<string, string> _headers;

        static RequestHandler()
        {
            _request = new Request();

            var args = Environment.GetCommandLineArgs();

            foreach (var arg in args)
            {
                if (arg.Contains("BackendUrl"))
                {
                    var json = arg.Replace("-config=", string.Empty);
                    _host = Json.Deserialize<ServerConfig>(json).BackendUrl;
                }

                if (arg.Contains("-token="))
                {
                    _session =  arg.Replace("-token=", string.Empty);
                    _headers = new Dictionary<string, string>()
                    {
                        { "Cookie", $"PHPSESSID={_session}" },
                        { "SessionId", _session }
                    };
                }
            }
        }

        private static void ValidateData(byte[] data)
        {
            if (data == null)
            {
                Log.Error($"Request failed, body is null");
            }

            Log.Info($"Request was successful");
        }

        private static void ValidateJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                Log.Error($"Request failed, body is null");
            }

            Log.Info($"Request was successful");
        }

        public static byte[] GetData(string url)
        {
            Log.Info($"Request GET data: {_session}:{_host}{url}");
            
            var result = _request.Send(_host + url, "GET", null, headers: _headers);
            
            ValidateData(result);
            return result;
        }

        public static string GetJson(string url)
        {
            Log.Info($"Request GET json: {_session}:{_host}{url}");

            var data = _request.Send(_host + url, "GET", headers: _headers);
            var result = Encoding.UTF8.GetString(data);
            
            ValidateJson(result);
            return result;
        }

        public static string PostJson(string url, string json)
        {
            Log.Info($"Request POST json: {_session}:{_host}{url}");

            var data = _request.Send(_host + url, "POST", Encoding.UTF8.GetBytes(json), true, "application/json", _headers);
            var result = Encoding.UTF8.GetString(data);
            
            ValidateJson(result);
            return result;
        }

        public static void PutJson(string url, string json)
        {
            Log.Info($"Request PUT json: {_session}:{_host}{url}");
            _request.Send(_host + url, "PUT", Encoding.UTF8.GetBytes(json), true, "application/json", _headers);
        }
    }
}
