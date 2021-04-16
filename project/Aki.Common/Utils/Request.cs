using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

namespace Aki.Common.Utils
{
    public static class HttpConstants
    {
        /// <summary>
        /// HTML GET method.
        /// </summary>
        public const string GET = "GET";

        /// <summary>
        /// HTML HEAD method.
        /// </summary>
        public const string HEAD = "HEAD";

        /// <summary>
        /// HTML POST method.
        /// </summary>
        public const string POST = "POST";

        /// <summary>
        /// HTML PUT method.
        /// </summary>
        public const string PUT = "PUT";

        /// <summary>
        /// HTML DELETE method.
        /// </summary>
        public const string DELETE = "DELETE";

        /// <summary>
        /// HTML CONNECT method.
        /// </summary>
        public const string CONNECT = "CONNECT";

        /// <summary>
        /// HTML OPTIONS method.
        /// </summary>
        public const string OPTIONS = "OPTIONS";

        /// <summary>
        /// HTML TRACE method.
        /// </summary>
        public const string TRACE = "TRACE";

        /// <summary>
        /// HTML MIME types.
        /// </summary>
        public static readonly Dictionary<string, string> MIME;

        static HttpConstants()
        {
            MIME = new Dictionary<string, string>()
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
            return method == GET
                || method == HEAD
                || method == POST
                || method == PUT
                || method == DELETE
                || method == CONNECT
                || method == OPTIONS
                || method == TRACE;
        }

        /// <summary>
        /// Is MIME type valid?
        /// </summary>
		public static bool IsValidMime(string mime)
        {
            foreach (KeyValuePair<string, string> item in MIME)
            {
                if (item.Value == mime)
                {
                    return true;
                }
            }

            return false;
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
			Uri uri = new Uri(url);
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

			if (uri.Scheme == "https")
			{
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
				request.ServerCertificateValidationCallback = delegate { return true; };
            }

			if (!HttpConstants.IsValidMethod(method))
			{
				throw new ArgumentException("request method is invalid");
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

			if (method != HttpConstants.GET && method != HttpConstants.HEAD && data != null)
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

			WebResponse response = request.GetResponse();

			try
			{
				using (MemoryStream ms = new MemoryStream())
				{
					response.GetResponseStream().CopyTo(ms);

					try
					{
						return Zlib.CheckHeader(ms.ToArray()) ? Zlib.Decompress(ms.ToArray()) : ms.ToArray();
					}
					catch
					{
						return ms.ToArray();
					}
				}
			}
			catch
			{
				return null;
			}
		}
	}
}
