using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Lion.ExAPI.Utility.Helper {
    /// <summary>
    /// 
    /// </summary>
    public class HttpClientHelper {
        private static object _thisLock = new object();

        private static HttpClient _client;

        private HttpClientHelper() {

            if (_client == null) {
                lock (_thisLock) {
                    if (_client == null) {
                        HttpClientHandler handler = new HttpClientHandler();
                        _client = new HttpClient(handler);
                    }
                }
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Url"></param>
        /// <param name="content"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        public static httpClientResponse<T> PostFormAsync<T>(string Url, FormUrlEncodedContent content, NameValueCollection header = null) where T : class, new() {

            httpClientResponse<T> result = new httpClientResponse<T>();

            if (!String.IsNullOrWhiteSpace(Url) && Uri.IsWellFormedUriString(Url, UriKind.Absolute) && content != null) {

                if (header != null) {
                    foreach (var key in header.AllKeys) {
                        content.Headers.Add(key, header.Get(key));
                    }
                }

                HttpResponseMessage response = _client.PostAsync(Url, content).Result;

                result.StatusCode = response.StatusCode;

                if (response.IsSuccessStatusCode) {
                    var postResultData = response.Content.ReadAsStringAsync().Result;

                    result.result = JsonConvert.DeserializeObject<T>(postResultData);
                }
            }

            return result;


        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Url"></param>
        /// <param name="postData"></param>
        /// <param name="authorization"></param>
        /// <returns></returns>
        public static T PostJsonAsync<T>(string Url, HttpContent postData, string authorization = null) where T : class, new() {
            T result = default(T);





            return result;
        }





    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class httpClientResponse<T> where T : class, new() {
        /// <summary>
        /// 
        /// </summary>
        public T result = default(T);
        /// <summary>
        /// 
        /// </summary>
        public System.Net.HttpStatusCode StatusCode;

    }
}
