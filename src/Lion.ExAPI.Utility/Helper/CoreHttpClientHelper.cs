using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Lion.ExAPI.Utility.Core.Helper {
    public class CoreHttpClientHelper {
        private static object _thisLock = new object();

        private static HttpClient _client;

        static CoreHttpClientHelper() {

            if (_client == null) {
                lock (_thisLock) {
                    if (_client == null) {
                        HttpClientHandler handler = new HttpClientHandler();

                        //handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                        handler.ClientCertificateOptions = ClientCertificateOption.Automatic;
                        handler.SslProtocols = System.Security.Authentication.SslProtocols.Tls | System.Security.Authentication.SslProtocols.Tls11 | System.Security.Authentication.SslProtocols.Tls12;

                        _client = new HttpClient(handler);
                        //_client.DefaultRequestHeaders.ConnectionClose = false;                        
                    }
                }
            }

        }

        public static async Task<httpClientResponse<T>> PostFormAsync<T>(string Url, FormUrlEncodedContent content, NameValueCollection header = null) where T : class, new() {

            httpClientResponse<T> result = new httpClientResponse<T>();

            if (!String.IsNullOrWhiteSpace(Url) && Uri.IsWellFormedUriString(Url, UriKind.Absolute) && content != null) {

                if (header != null) {
                    foreach (var key in header.AllKeys) {
                        content.Headers.Add(key, header.Get(key));
                    }
                }

                Stopwatch s = new Stopwatch();
                s.Start();
                var response = await _client.PostAsync(Url, content);

                s.Stop();
                result.execTime = String.Format("_1_{0}ms", s.ElapsedMilliseconds);

                if (response.IsSuccessStatusCode) {

                    s.Restart();
                    var postResultData = await response.Content.ReadAsStringAsync();
                    s.Stop();

                    result.execTime += String.Format("_2_{0}ms", s.ElapsedMilliseconds);


                    s.Restart();

                    result.result = JsonConvert.DeserializeObject<T>(postResultData);
                }

                s.Stop();
                result.execTime += String.Format("_3_{0}ms", s.ElapsedMilliseconds);

                result.StatusCode = response.StatusCode;
            }

            return result;


        }

        public static async Task<httpClientResponse<T>> PostAsJsonAsync<T>(string Url, string postData) {
            httpClientResponse<T> result = new httpClientResponse<T>();

            if (!String.IsNullOrWhiteSpace(Url) && Uri.IsWellFormedUriString(Url, UriKind.Absolute)) {

                HttpContent contentPost = new StringContent(postData, System.Text.Encoding.UTF8, "application/json");

                Stopwatch s = new Stopwatch();
                s.Start();
                var response = await _client.PostAsync(Url, contentPost);

                s.Stop();
                result.execTime = String.Format("_1_{0}ms", s.ElapsedMilliseconds);

                if (response.IsSuccessStatusCode) {

                    s.Restart();
                    var postResultData = await response.Content.ReadAsStringAsync();
                    s.Stop();

                    result.execTime += String.Format("_2_{0}ms", s.ElapsedMilliseconds);


                    s.Restart();
                    result.result = JsonConvert.DeserializeObject<T>(postResultData);
                }

                s.Stop();
                result.execTime += String.Format("_3_{0}ms", s.ElapsedMilliseconds);

                result.StatusCode = response.StatusCode;
            }

            return result;
        }


        public static async Task<httpClientResponse<String>> PostAsJsonAsync(string Url, string postData) {
            httpClientResponse<String> result = new httpClientResponse<String>();

            if (!String.IsNullOrWhiteSpace(Url) && Uri.IsWellFormedUriString(Url, UriKind.Absolute)) {

                HttpContent contentPost = new StringContent(postData, System.Text.Encoding.UTF8, "application/json");

                Stopwatch s = new Stopwatch();
                s.Start();
                var response = await _client.PostAsync(Url, contentPost);

                s.Stop();
                result.execTime = String.Format("_1_{0}ms", s.ElapsedMilliseconds);

                if (response.IsSuccessStatusCode) {

                    s.Restart();
                    var postResultData = await response.Content.ReadAsStringAsync();
                    s.Stop();

                    result.execTime += String.Format("_2_{0}ms", s.ElapsedMilliseconds);


                    s.Restart();
                    result.result = postResultData;
                }

                s.Stop();
                result.execTime += String.Format("_3_{0}ms", s.ElapsedMilliseconds);

                result.StatusCode = response.StatusCode;
            }

            return result;
        }


        public static async Task<httpClientResponse<T>> GetJsonAsync<T>(string Url) where T : class, new() {

            httpClientResponse<T> result = new httpClientResponse<T>();

            if (!String.IsNullOrWhiteSpace(Url) && Uri.IsWellFormedUriString(Url, UriKind.Absolute)) {


                Stopwatch s = new Stopwatch();
                s.Start();
                var response = await _client.GetAsync(Url);

                s.Stop();
                result.execTime = String.Format("_1_{0}ms", s.ElapsedMilliseconds);

                if (response.IsSuccessStatusCode) {

                    s.Restart();
                    var postResultData = await response.Content.ReadAsStringAsync();
                    s.Stop();

                    result.execTime += String.Format("_2_{0}ms", s.ElapsedMilliseconds);


                    s.Restart();
                    result.result = JsonConvert.DeserializeObject<T>(postResultData);
                }

                s.Stop();
                result.execTime += String.Format("_3_{0}ms", s.ElapsedMilliseconds);

                result.StatusCode = response.StatusCode;
            }

            return result;
        }

        public static async Task<httpClientResponse<String>> GetStringAsync(string Url) {


            httpClientResponse<String> result = new httpClientResponse<String>();

            if (!String.IsNullOrWhiteSpace(Url) && Uri.IsWellFormedUriString(Url, UriKind.Absolute)) {


                Stopwatch s = new Stopwatch();
                s.Start();
                var response = await _client.GetAsync(Url);

                s.Stop();
                result.execTime = String.Format("_1_{0}ms", s.ElapsedMilliseconds);

                if (response.IsSuccessStatusCode) {

                    s.Restart();
                    var postResultData = await response.Content.ReadAsStringAsync();
                    s.Stop();

                    result.execTime += String.Format("_2_{0}ms", s.ElapsedMilliseconds);


                    s.Restart();
                    result.result = postResultData;
                }

                s.Stop();
                result.execTime += String.Format("_3_{0}ms", s.ElapsedMilliseconds);

                result.StatusCode = response.StatusCode;
            }

            return result;

        }
    }


    public class httpClientResponse<T> {

        public T result = default(T);

        public System.Net.HttpStatusCode StatusCode;

        public string execTime = string.Empty;

    }
}
