using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Twittimation.Http
{
    public class HttpRequest
    {
        public static int TimeoutInMillis = 5000;
        public static Encoding Encoding = Encoding.UTF8;
        /// <summary>
        /// Headers used in all requests, by default it has 1 header "Accept" with value "application/json".
        /// </summary>
        public static KeyValuePair<string, string>[] DefaultHeaders = new KeyValuePair<string, string>[]
            { new KeyValuePair<string, string>("Accept", "application/json") };
        public static bool AllowAutoRedirect = false;
        //So your cookies don't get changed. If you want them to get changed then always return the same container.
        public static Func<CookieContainer> DefaultCookies = () => new CookieContainer();

        protected string _url;
        protected bool _useParameter = false;
        protected byte[] _parameter = new byte[0];
        protected string _parameterAsString { get { return Encoding.GetString(_parameter); } set { _parameter = Encoding.GetBytes(value); } }
        protected string _method;
        protected KeyValuePair<string, string>[] _headers;
        protected List<Cookie> _relevantSuppliedCookies;

        public CookieContainer Cookies { get; private set; }
        public long Created { get; private set; }
        public bool CreatedAsBool => Created != 0;
        public long Sent { get; private set; }
        public bool SentAsBool => Sent != 0;
        public long Returned { get; private set; }
        public long TotalLatency { get; private set; }
        public byte[] Response { get; private set; }
        public string ResponseAsString => Encoding.GetString(Response);
        public HttpStatusCode StatusCode { get; private set; }
        public List<Action<HttpRequest>> CompletedHandles { get; } = new List<Action<HttpRequest>>();

        private HttpWebRequest request;

        #region constructors
        public HttpRequest(string url, params KeyValuePair<string, string>[] headers)
            : this(url, "GET", DefaultCookies(), headers) { }
        public HttpRequest(string url, CookieContainer cookies, params KeyValuePair<string, string>[] headers)
            : this(url, "GET", cookies, headers) { }
        public HttpRequest(string url, string method, params KeyValuePair<string, string>[] headers)
            : this(url, method, DefaultCookies(), headers) { }
        public HttpRequest(string url, string method, CookieContainer cookies, params KeyValuePair<string, string>[] headers)
        {
            _url = url;
            _method = method;
            Cookies = cookies;
            _headers = headers;

            _relevantSuppliedCookies = new List<Cookie>();
            var enumerator = Cookies.GetCookies(new Uri(_url)).GetEnumerator();
            while (enumerator.MoveNext())
                _relevantSuppliedCookies.Add((Cookie)enumerator.Current);
        }

        public HttpRequest(string url, string method, string parameter, params KeyValuePair<string, string>[] headers)
            : this(url, method, parameter, DefaultCookies(), headers) { }
        public HttpRequest(string url, string method, string parameter, CookieContainer cookies, params KeyValuePair<string, string>[] headers)
            : this(url, method, cookies, headers)
        {
            _parameterAsString = parameter;
            _useParameter = true;
        }

        public HttpRequest(string url, string method, byte[] parameter, params KeyValuePair<string, string>[] headers)
            : this(url, method, parameter, DefaultCookies(), headers)
        { }
        public HttpRequest(string url, string method, byte[] parameter, CookieContainer cookies, params KeyValuePair<string, string>[] headers)
            : this(url, method, cookies, headers)
        {
            _parameter = parameter;
            _useParameter = true;
        }
        #endregion

        /// <summary>
        /// Optional. Prepares the web request so that it can be immediately sent at a later time.
        /// </summary>
        public async Task Prepare()
        {
            if (!CreatedAsBool)
                request = await CreateHttpRequest();
        }

        public async Task Go()
        {
            if (!CreatedAsBool)
                request = await CreateHttpRequest();
            if (!SentAsBool)
                await SendHttpRequest(request);
            CompletedHandles.ForEach((h) => h(this));
            CompletedHandles.Clear();
        }

        private async Task<HttpWebRequest> CreateHttpRequest()
        {
            Created = DateTimeOffset.Now.Ticks;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_url);
            foreach (KeyValuePair<string, string> value in DefaultHeaders)
                request.SetRawHeader(value.Key, value.Value);
            foreach (KeyValuePair<string, string> value in _headers)
                request.SetRawHeader(value.Key, value.Value);
            request.AllowAutoRedirect = AllowAutoRedirect;
            request.Method = _method;
            request.CookieContainer = Cookies;
            if (_useParameter)
                (await request.GetRequestStreamAsync()).Write(_parameter, 0, _parameter.Length);
            return request;
        }

        private async Task SendHttpRequest(HttpWebRequest request)
        {
            Sent = DateTimeOffset.Now.Ticks;
            using (HttpWebResponse response = await HttpTaskWithTimeout(request.GetResponseAsync(), TimeoutInMillis))
            {
                StatusCode = response.StatusCode;
                Response = ExtractResponse(response.ContentLength, response.GetResponseStream());
            }
            Returned = DateTimeOffset.Now.Ticks;
            TotalLatency = Returned - Created;
        }

        private byte[] ExtractResponse(long length, Stream stream)
        {
            byte[] data;
            using (var mstrm = new MemoryStream())
            {
                var tempBuffer = new byte[4096];
                int bytesRead;
                while ((bytesRead = stream.Read(tempBuffer, 0, tempBuffer.Length)) != 0)
                    mstrm.Write(tempBuffer, 0, bytesRead);
                mstrm.Flush();
                data = mstrm.GetBuffer();
            }
            return data;
        }

        private Task<HttpWebResponse> HttpTaskWithTimeout(Task<WebResponse> task, int duration)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    bool b = task.Wait(duration);
                    if (b) return (HttpWebResponse)task.GetAwaiter().GetResult();
                }
                catch (AggregateException exception)
                {
                    var first = (WebException)exception.InnerExceptions[0];
                    if (first.Message ==
                    "An error occurred while sending the request. The text associated with this error code could not be found.\r\n" +
                    "\r\nA connection with the server could not be established\r\n")
                        throw new WebException("Server is Offline");
                    return first.Response as HttpWebResponse;
                }

                throw new TimeoutException();
            });
        }

        public static void Reset()
        {
            TimeoutInMillis = 5000;
            Encoding = Encoding.UTF8;
            DefaultCookies = () => new CookieContainer();
            DefaultHeaders = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("Accept", "application/json") };
            AllowAutoRedirect = false;
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj.GetType() == GetType() && Equals((HttpRequest)obj);
        }

        protected bool Equals(HttpRequest other)
        {
            return _url == other._url && _useParameter == other._useParameter && _parameter.SequenceEqual(other._parameter) && _method == other._method
                && _headers.SequenceEqual(other._headers) && _relevantSuppliedCookies.SequenceEqual(other._relevantSuppliedCookies);
        }

        public override int GetHashCode()
        {
            return (_url + _method).GetHashCode() + _parameter.GetHashCode();
        }
    }
}