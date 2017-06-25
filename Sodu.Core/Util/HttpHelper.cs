using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace Sodu.Core.Util
{
    public class HttpHelper
    {

        public CancellationTokenSource Cts = new CancellationTokenSource();

        HttpWebRequest Request;


        Encoding DefauleEncoding = Encoding.GetEncoding("GB2312");
        /// <summary>
        /// 获取html代码
        /// </summary>
        public string Html = string.Empty;

        public HttpHelper()
        {

        }


        public async Task<string> WebRequestGet(string url, bool isAddTime = false, Encoding encoding = null)
        {
            string html = null;
            try
            {
                if (isAddTime)
                {
                    url = url + "?time=" + GetTimeStamp();
                }
                Request = WebRequest.CreateHttp(new Uri(url)); //创建WebRequest对象              
                Request.Method = "GET";    //设置请求方式为GET : 
                Request.Headers["Timeout"] = "15000";
                Request.Headers[HttpRequestHeader.Accept] = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                Request.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.110 Safari/537.36";
                Request.Headers[HttpRequestHeader.AcceptEncoding] = "gzip, deflate, sdch"; //设置接收的编码 可以接受 gzip
                Request.Headers[HttpRequestHeader.AcceptLanguage] = "zh-CN,zh;q=0.8";
                Request.Headers[HttpRequestHeader.CacheControl] = "max-age=0";
                Request.Headers[HttpRequestHeader.Connection] = "keep-alive";
                Request.ContentType = "text/html; charset=utf-8";
                Request.Proxy = null;
                Request.ContinueTimeout = 350;
                await Task.Delay(500);
                html = await GetReponseHtml(Request, encoding);

            }
            catch (Exception)
            {
                html = null;
            }
            return html;
        }

        public async Task<string> WebRequestPost(string url, string postData)
        {
            Request = HttpWebRequest.CreateHttp(new Uri(url)); //创建WebRequest对象              
            Request.Method = "POST";    //设置请求方式为GET
            Request.ContentType = "application/x-www-form-urlencoded";

            Request.Headers[HttpRequestHeader.Accept] = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            Request.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/48.0.2564.97 Safari/537.36";
            Request.Headers[HttpRequestHeader.AcceptEncoding] = "gzip, deflate"; //设置接收的编码 可以接受 gzip
            Request.Proxy = null;


            using (Stream str = await Request.GetRequestStreamAsync())
            {
                string content = postData;
                byte[] data = Encoding.UTF8.GetBytes(content);
                str.Write(data, 0, data.Length);
            }
            string html = await GetReponseHtml(Request);
            return html;

        }

        public async Task<string> GetReponseHtml(WebRequest request, Encoding encoding = null)
        {
            string html = null;
            try
            {
                var response = await request.GetResponseAsync();
                Stream stream = null;
                if (response.Headers[HttpRequestHeader.ContentEncoding] != null)
                {
                    stream = response.Headers[HttpRequestHeader.ContentEncoding].Equals("gzip",
                                   StringComparison.CurrentCultureIgnoreCase) ? new GZipStream(response.GetResponseStream(), CompressionMode.Decompress) : response.GetResponseStream();
                }
                else
                {
                    stream = response.GetResponseStream();
                }
                var ms = new MemoryStream();
                var buffer = new byte[1024];
                while (true)
                {
                    if (stream == null) continue;
                    var sz = stream.Read(buffer, 0, 1024);
                    if (sz == 0) break;
                    ms.Write(buffer, 0, sz);
                }
                var bytes = ms.ToArray();

                encoding = encoding == null ? GetEncoding(bytes, response.Headers[HttpRequestHeader.ContentType]) : encoding;
                html = encoding.GetString(bytes);
                await stream.FlushAsync();
            }
            catch (Exception ex)
            {
                html = null;
                Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
            return html;
        }

        public Encoding GetEncoding(byte[] bytes, string charSet)
        {
            try
            {
                var html = Encoding.UTF8.GetString(bytes);
                string strCharSet =
               Regex.Match(html, @"<meta.*?charset=""?([a-z0-9-]+)\b", RegexOptions.IgnoreCase)
               .Groups[1].Value;
                return !string.IsNullOrEmpty(strCharSet) ? Encoding.GetEncoding(strCharSet) : Encoding.UTF8;
            }
            catch (Exception)
            {

            }
            return Encoding.UTF8;
        }


        /// <summary>
        /// Http Get Request
        /// </summary>
        /// <param name="url"></param>
        /// <param name="isAddTime"></param>
        /// <returns></returns>
        public async Task<string> HttpClientGetRequest(string url, bool isAddTime = true)
        {
            string html;
            try
            {
                Cts = new CancellationTokenSource();
                Cts.CancelAfter(TimeSpan.FromSeconds(15));
                HttpClient httpclient = new HttpClient();
                if (isAddTime)
                {
                    url = url + "?time=" + GetTimeStamp();
                }
                try
                {
                    HttpResponseMessage response = await httpclient.GetAsync(new Uri(url)).AsTask(Cts.Token);
                    Cts.Token.ThrowIfCancellationRequested();
                    html = response.Content.ToString();
                }
                catch (TaskCanceledException)
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
                html = null;
            }
            return html;
        }

        /// <summary>  
        /// 获取时间戳  
        /// </summary>  
        /// <returns></returns>  
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }


        public async Task<string> HttpClientPostRequest(string url, string postData, Encoding encode = null)
        {
            string html = string.Empty;
            try
            {
                HttpClient httpclient = new HttpClient();
                HttpStringContent httpStringContent = new HttpStringContent(postData);
                //就这个问题让我找了好几个小时
                httpStringContent.Headers.ContentType = new HttpMediaTypeHeaderValue("application/x-www-form-urlencoded");
                // httpStringContent.Headers["Connection"] = "Keep-Alive";
                //  HttpResponseMessage response = await httpclient.PostAsync(new Uri(url), httpStringContent).AsTask(Cts.Token);
                Cts = new CancellationTokenSource();
                Cts.CancelAfter(TimeSpan.FromSeconds(15));

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new Uri(url));
                request.Content = httpStringContent;

                request.Headers["Connection"] = "Keep-Alive";
                //  request.Headers["Cache"] = "private";
                var vache = request.Headers.CacheControl;
                HttpResponseMessage response = await httpclient.SendRequestAsync(request).AsTask(Cts.Token);
                Cts.Token.ThrowIfCancellationRequested();
                using (Stream responseStream = (await response.Content.ReadAsInputStreamAsync()).AsStreamForRead())
                {
                    using (var reader = new StreamReader(responseStream, DefauleEncoding))
                    {
                        html = reader.ReadToEnd();
                        reader.Dispose();
                        responseStream.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
                html = null;
            }
            return html;
        }

        public void HttpClientCancleRequest2()
        {
            if (Cts.Token.CanBeCanceled)
            {
                this.Cts.Cancel();
            }
        }

        public void HttpClientCancleRequest()
        {
            try
            {
                Request?.Abort();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }

        }
    }
}
