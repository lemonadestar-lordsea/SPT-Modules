using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using UnityEngine;

namespace Aki.Common.Utils
{
    public static class HttpConstants
    {
        /// <summary>
        /// HTML GET method.
        /// </summary>
        public const string Get = "GET";

        /// <summary>
        /// HTML HEAD method.
        /// </summary>
        public const string Head = "HEAD";

        /// <summary>
        /// HTML POST method.
        /// </summary>
        public const string Post = "POST";

        /// <summary>
        /// HTML PUT method.
        /// </summary>
        public const string Put = "PUT";

        /// <summary>
        /// HTML DELETE method.
        /// </summary>
        public const string Delete = "DELETE";

        /// <summary>
        /// HTML CONNECT method.
        /// </summary>
        public const string Connect = "CONNECT";

        /// <summary>
        /// HTML OPTIONS method.
        /// </summary>
        public const string Options = "OPTIONS";

        /// <summary>
        /// HTML TRACE method.
        /// </summary>
        public const string Trace = "TRACE";

        /// <summary>
        /// HTML MIME types.
        /// </summary>
        public static Dictionary<string, string> Mime { get; private set; }

        static HttpConstants()
        {
            Mime = new Dictionary<string, string>()
            {
                { ".bin", "application/octet-stream" },
                { ".txt", "text/plain" },
                { ".htm", "text/html" },
                { ".html", "text/html" },
                { ".css", "text/css" },
                { ".js", "text/javascript" },
                { ".jpeg", "image/jpeg" },
                { ".jpg", "image/jpeg" },
                { ".png", "image/png" },
                { ".ico", "image/vnd.microsoft.icon" },
                { ".json", "application/json" }
            };
        }

        /// <summary>
        /// Is HTML method valid?
        /// </summary>
        public static bool IsValidMethod(string method)
        {
            return method == Get
                || method == Head
                || method == Post
                || method == Put
                || method == Delete
                || method == Connect
                || method == Options
                || method == Trace;
        }

        /// <summary>
        /// Is MIME type valid?
        /// </summary>
		public static bool IsValidMime(string mime)
        {
            return Mime.Any(x => x.Value == mime);
        }
    }

    public class Request
    {
		/// <summary>
		/// Send a request to remote endpoint and optionally receive a response body.
		/// Deflate is the accepted compression format.
		/// </summary>
		public byte[] Send(string url, string method, byte[] data = null, bool compress = true, string mime = null, Dictionary<string, string> headers = null)
		{
            if (!HttpConstants.IsValidMethod(method))
			{
				throw new ArgumentException("request method is invalid");
			}

			Uri uri = new Uri(url);
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

			if (uri.Scheme == "https")
			{
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
				request.ServerCertificateValidationCallback = delegate { return true; };
            }

            request.Timeout = 1000;
			request.Method = method;
			request.Headers.Add("Accept-Encoding", "deflate");

			if (headers != null)
			{
				foreach (KeyValuePair<string, string> item in headers)
				{
                    Debug.Log($"[DEBUG] key: {item.Key}, value: {item.Value}");
					request.Headers.Add(item.Key, item.Value);
				}
			}

			if (method != HttpConstants.Get && method != HttpConstants.Head && data != null)
			{
				byte[] body = (compress) ? Zlib.Compress(data, ZlibCompression.Maximum) : data;

				request.ContentType = HttpConstants.IsValidMime(mime) ? mime : "application/octet-stream";
				request.ContentLength = body.Length;

                Debug.Log($"[DEBUG] mime: {request.ContentType}");

				if (compress)
				{
					request.Headers.Add("Content-Encoding", "deflate");
				}

				using (Stream stream = request.GetRequestStream())
				{
					stream.Write(body, 0, body.Length);
				}
			}

			using (WebResponse response = request.GetResponse())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    response.GetResponseStream().CopyTo(ms);
                    byte[] body = ms.ToArray();

                    if (body.Length == 0)
                    {
                        return null;
                    }                    

                    if (Zlib.IsCompressed(body))
                    {
                        return Zlib.Decompress(body);
                    }

                    return body;
                }
            }
		}
	}
}
